using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RobotQQ.Core
{
    [Serializable]
    public class WebQQ
    {
        #region 单例
        private static WebQQ _instance;
        public static WebQQ instance
        {
            get
            {
                if (_instance == null) _instance = new WebQQ();
                return _instance;
            }
        }

        public void save()
        {
            var result = new Result_serializable();
            result.state = state;
            result.status = status;
            result.config_url = config_url;
            result._qrsig = _qrsig;
            result.Ptwebqq = Ptwebqq;
            result.Vfwebqq = Vfwebqq;
            result.Psessionid = Psessionid;
            result.uin = uin;
            result.lastPeople = lastPeople;
            result.friends = friends;
            result.groups = groups;
            result.cookies = data.cookies;
            var qqfile = AppDomain.CurrentDomain.BaseDirectory + "/robot.sv";
            StreamWriter sw = new StreamWriter(qqfile, false);
            sw.Write(JsonConvert.SerializeObject(result));
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        public void back()
        {
            var qqfile = AppDomain.CurrentDomain.BaseDirectory + "/robot.sv";
            if (!File.Exists(qqfile)) return;
            StreamReader sr = new StreamReader(qqfile, Encoding.Default);
            var json = sr.ReadToEnd();
            var result = JsonConvert.DeserializeObject<Result_serializable>(json);
            if (result != null)
            {
                config_url = result.config_url;
                _qrsig = result._qrsig;
                Ptwebqq = result.Ptwebqq;
                Vfwebqq = result.Vfwebqq;
                Psessionid = result.Psessionid;
                uin = result.uin;
                lastPeople = result.lastPeople;
                friends = result.friends;
                groups = result.groups;
                status = result.status;
                state = result.state;
                data.cookies = result.cookies;
            }
            sr.Close();
            sr.Dispose();
        }
        #endregion

        #region 初始化
        public WebQQ()
        {
            //设置状态为等待
            state = RobotState.Wait;
            back();
            //开启线程
            Task.Run(() =>
            {
                banckWork();
            });
        }
        #endregion

        #region 状态
        /// <summary>
        /// 登录状态
        /// </summary>
        public RobotStatus status = RobotStatus.offline;
        #endregion
        #region 基本参数
        private HttpData data = HttpData.instance;
        /// <summary>
        /// 后台工作状态
        /// </summary>
        private RobotState state = RobotState.Wait;

        /// <summary>
        /// 设置参数用到的url
        /// </summary>
        private string config_url = string.Empty;
        //二维码的验证参数
        private string _qrsig = string.Empty;
        //几个登录之前用到的参数
        private string Ptwebqq = string.Empty;
        private string Vfwebqq = string.Empty;
        private const long ClientId = 53999199;
        private string Psessionid = string.Empty;
        private long uin = 0L;
        //近期联系人
        private List<string> lastPeople = new List<string>();
        //qq好友
        private List<string> friends = new List<string>();
        //qq群
        private List<string> groups = new List<string>();

        #endregion

        #region 基本事件
        /// <summary>
        /// 登录成功之前的消息事件
        /// </summary>
        public event EventHandler<string> onLoginMessage;
        /// <summary>
        /// 消息事件
        /// </summary>
        public event EventHandler<string> onMessage;
        /// <summary>
        /// 日志事件
        /// </summary>
        public event EventHandler<string> onLog;
        #endregion

        #region 私有方法
        /// <summary>
        /// 对二维码中的验证字符串进行加密
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static int Hash33(string s)
        {
            int e = 0, i = 0, n = s.Length;
            for (; n > i; ++i)
                e += (e << 5) + s[i];
            return 2147483647 & e;
        }

        /// <summary>
        /// 获取二维码之前的动作
        /// </summary>
        public void preLogin()
        {
            data.preLogin();
        }
        /// <summary>
        /// 背后的努力
        /// </summary>
        private void banckWork()
        {
            while (true)
            {
                Thread.Sleep(1000);
                switch (state)
                {
                    case RobotState.CheckLogin:
                        checkLoginResult();
                        break;
                    case RobotState.LoginInit:
                        init();
                        break;
                    case RobotState.PollMessage:
                        pollMessage();
                        break;
                }
            }
        }
        /// <summary>
        /// 检查登录结果
        /// </summary>
        private void checkLoginResult()
        {
            var result = data.checkQCodeResult(_qrsig);
            //判断登录失效
            if (result.Contains("已失效"))
            {

            }
            if (result.Contains("认证"))
            {

            }
            if (result.Contains("成功"))
            {
                var cookie = HttpCookieHelper.GetCookieValue("ptwebqq", data.cookies);
                Debug.WriteLine("ptwebqq:" + Ptwebqq);
                if (cookie != null) Ptwebqq = cookie;
                foreach (var content in result.Split(new[] { "','" }, StringSplitOptions.None))
                {
                    if (!content.StartsWith("http")) continue;
                    config_url = content;
                }
                state = RobotState.LoginInit;
            }

        }
        /// <summary>
        /// 初始化登录
        /// </summary>
        private void init()
        {
            if (string.IsNullOrEmpty(config_url))
            {
                state = RobotState.Wait;
                onLoginMessage?.Invoke(this, "初始化失败");
                return;
            }
            data.RegPtwebqq(config_url);
            //第二步
            var result = data.GetVfwebqq(Ptwebqq);
            if (result == null)
            {
                state = RobotState.Wait; return;
            }
            Vfwebqq = result.vfwebqq;
            //第三步 获取uin和psessionid
            var login2 = data.UinAndPsessionid(new { ptwebqq = Ptwebqq, clientid = ClientId, psessionid = "", status = "online" });
            if (login2 == null)
            {
                state = RobotState.Wait; return;
            }
            Psessionid = login2.psessionid;
            uin = login2.uin;
            //登录完成
            state = RobotState.PollMessage;
            status = RobotStatus.online;
            //获取最近对话

            //获取好友

            //获取qq群

            //保存
            save();
        }
        /// <summary>
        /// 检查是否有消息
        /// </summary>
        private void pollMessage()
        {
            var message = data.GetMessage(new { ptwebqq = Ptwebqq, clientid = ClientId, psessionid = Psessionid, key = "" });
            if (message == null) return;

        }
        #endregion
        /// <summary>
        /// 使用二维码登录
        /// </summary>
        /// <returns></returns>
        public Image Login_QCode()
        {
            preLogin();
            onLoginMessage?.Invoke(this, "准备完成，开始获取二维码");

            Image outputImg = data.GetQCode();
            _qrsig = HttpCookieHelper.GetCookieValue("qrsig", data.cookies);

            onLoginMessage?.Invoke(this, "等待扫描结果");
            state = RobotState.CheckLogin;
            return outputImg;
        }
    }
}
