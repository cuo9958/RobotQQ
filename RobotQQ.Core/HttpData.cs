using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web;

namespace RobotQQ.Core
{
    [Serializable]
    public class HttpData
    {
        #region 共享
        public static readonly HttpData instance = new HttpData();
        #endregion

        public HttpData()
        {
            item = new HttpItem()
            {
                Cookie = "",//字符串Cookie     可选项
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36",
                Accept = "*/*",//    可选项有默认值
                ContentType = "text/html",//返回类型    可选项有默认值
                Referer = "http://w.qq.com/",//来源URL     可选项
                Allowautoredirect = false,//是否根据３０１跳转     可选项
                Connectionlimit = 1024,//最大连接数     可选项 默认为1024
                //Postdata = "C:\\PERKYSU_20121129150608_ScrubLog.txt",//Post数据     可选项GET时不需要写
                ResultType = ResultType.String,//返回数据类型，是Byte还是String
                CookieCollection = new CookieCollection(),//可以直接传一个Cookie集合进来
            };
        }
        #region 变量
        /// <summary>
        /// 主要的发送主体
        /// </summary>
        private HttpHelper client = new HttpHelper();
        /// <summary>
        /// 发送参数
        /// </summary>
        private HttpItem item;
        /// <summary>
        /// 一直保存的refer
        /// </summary>
        private string myRefer = "https://ui.ptlogin2.qq.com/cgi-bin/login?daid=164&target=self&style=16&mibao_css=m_webqq&appid=501004106&enable_qlogin=0&no_verifyimg=1&s_url=http%3A%2F%2Fw.qq.com%2Fproxy.html&f_url=loginerroralert&strong_login=1&login_state=10&t=20131024001";
        /// <summary>
        /// 保存当前的cookies
        /// </summary>
        public string cookies;
        private Dictionary<string, string> cookieList = new Dictionary<string, string>();
        #endregion
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
        /// 时间戳
        /// </summary>
        /// <returns></returns>
        private long getTimes()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }
        /// <summary>
        /// 重新设置cookie
        /// </summary>
        /// <param name="cookie"></param>
        private void setCookie(string cookie)
        {
            var list = HttpCookieHelper.GetCookieList(cookie);
            if (list.Count == 0) return;
            cookies = string.Empty;
            foreach (var item in list)
            {
                if (item.Key.Trim() == "EXPIRES") continue;
                if (item.Key.Trim() == "PATH") continue;
                if (item.Key.Trim() == "DOMAIN") continue;
                if (item.Value.Trim() == "") continue;
                cookies += HttpCookieHelper.CookieFormat(item.Key, item.Value);
            }
        }
        /// <summary>
        /// 登录之前的操作
        /// </summary>
        /// <returns></returns>
        public string preLogin()
        {
            Debug.WriteLine("登录之前的操作");
            return GetString("https://ui.ptlogin2.qq.com/cgi-bin/login?daid=164&target=self&style=16&mibao_css=m_webqq&appid=501004106&enable_qlogin=0&no_verifyimg=1&s_url=http%3A%2F%2Fw.qq.com%2Fproxy.html&f_url=loginerroralert&strong_login=1&login_state=10&t=20131024001");
        }
        /// <summary>
        /// 获取登录的二维码
        /// </summary>
        /// <returns></returns>
        public Image GetQCode()
        {
            Debug.WriteLine("获取登录的二维码");
            return GetImage("https://ssl.ptlogin2.qq.com/ptqrshow?appid=501004106&e=0&l=M&s=5&d=72&v=4&t=0.6246572878509447", "https://ui.ptlogin2.qq.com/cgi-bin/login?daid=164&target=self&style=16&mibao_css=m_webqq&appid=501004106&enable_qlogin=0&no_verifyimg=1&s_url=http%3A%2F%2Fw.qq.com%2Fproxy.html&f_url=loginerroralert&strong_login=1&login_state=10&t=20131024001");
        }
        /// <summary>
        /// 获取登录扫描的结果
        /// </summary>
        /// <returns></returns>
        public string checkQCodeResult(string _qrsig)
        {
            var url = string.Format("https://ssl.ptlogin2.qq.com/ptqrlogin?ptqrtoken={0}&webqq_type=10&remember_uin=1&login2qq=1&aid=501004106&u1=http%3A%2F%2Fw.qq.com%2Fproxy.html%3Flogin2qq%3D1%26webqq_type%3D10&ptredirect=0&ptlang=2052&daid=164&from_ui=1&pttype=1&dumy=&fp=loginerroralert&action=0-0-10331&mibao_css=m_webqq&t=1&g=1&js_type=0&js_ver=10220&login_sig=&pt_randsalt=3", Hash33(_qrsig));
            return GetString(url, "https://ui.ptlogin2.qq.com/cgi-bin/login?daid=164&target=self&style=16&mibao_css=m_webqq&appid=501004106&enable_qlogin=0&no_verifyimg=1&s_url=http%3A%2F%2Fw.qq.com%2Fproxy.html&f_url=loginerroralert&strong_login=1&login_state=10&t=20131024001");
        }
        /// <summary>
        /// 注册刚才得到的ptwebqq
        /// </summary>
        /// <param name="url"></param>
        public void RegPtwebqq(string url)
        {
            Debug.WriteLine("注册刚才得到的ptwebqq");
            GetString(url);
        }
        /// <summary>
        /// 获取vfwebqq
        /// </summary>
        /// <param name="Ptwebqq"></param>
        /// <returns></returns>
        public Result_vfwebqq GetVfwebqq(string Ptwebqq)
        {
            Debug.WriteLine("获取vfwebqq");
            var url = string.Format("http://s.web2.qq.com/api/getvfwebqq?ptwebqq={0}&clientid=53999199&psessionid=&t=" + getTimes(), Ptwebqq);
            var result = GetString(url, "http://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1", "s.web2.qq.com", "utf-8");
            if (string.IsNullOrEmpty(result)) return null;
            var json = JsonConvert.DeserializeObject<Result_Base<Result_vfwebqq>>(result);
            if (json.retcode == 0) return json.result;
            return null;
        }
        /// <summary>
        /// 达成二次登录
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Result_login2 UinAndPsessionid(object obj)
        {
            Debug.WriteLine("获取uin和psessionid");
            var result = PostString("http://d1.web2.qq.com/channel/login2", "r=" + HttpUtility.UrlEncode(JsonConvert.SerializeObject(obj)), "http://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2", "d1.web2.qq.com");
            if (string.IsNullOrEmpty(result)) return null;
            var json = JsonConvert.DeserializeObject<Result_Base<Result_login2>>(result);
            if (json.retcode == 0) return json.result;
            return null;
        }
        /// <summary>
        /// 从远程获取消息
        /// </summary>
        /// <param name="obj"></param>
        public List<Result_message> GetMessage(object obj)
        {
            Debug.WriteLine("拉取消息");
            var result = PostString("http://d1.web2.qq.com/channel/poll2", "r=" + HttpUtility.UrlEncode(JsonConvert.SerializeObject(obj)), "http://d1.web2.qq.com/cfproxy.html?v=20151105001&callback=1", "d1.web2.qq.com");
            if (string.IsNullOrEmpty(result)) return null;
            var json = JsonConvert.DeserializeObject<Result_Base<List<Result_message>>>(result);
            if (json.retcode == 0) return json.result;
            return null;
        }
        /// <summary>
        /// 获取资源
        /// </summary>
        /// <param name="url"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public string GetString(string url, string refer = "", string host = "", string contentType = "")
        {
            //调试输出
            Debug.WriteLine("获取远程资源：" + url);
            //地址
            item.URL = url;
            //保存上一个来源
            myRefer = item.Referer;
            //设置当前的来源
            if (!string.IsNullOrEmpty(refer)) item.Referer = refer;
            //设置返回类型
            item.ResultType = ResultType.String;
            //添加cookie
            item.Cookie = cookies;
            //设置host
            if (!string.IsNullOrEmpty(host)) item.Host = host;
            //设置contenttype
            if (!string.IsNullOrEmpty(contentType)) item.ContentType = contentType;
            //设置get请求
            item.Method = "get";
            //从远程获取
            HttpResult result = client.GetHtml(item);
            //保存cookie
            if (!string.IsNullOrEmpty(result.Cookie)) setCookie(result.Cookie);
            //设置为原来的来源
            item.Referer = myRefer;
            //调试输出
            Debug.WriteLine("cookie@@" + cookies);
            Debug.WriteLine(result.Html);
            //返回字符串
            return result.Html;
        }
        public string PostString(string url, string postData, string refer = "", string host = "", string contentType = "application/x-www-form-urlencoded")
        {
            //调试输出
            Debug.WriteLine("获取远程资源：" + url);
            //地址
            item.URL = url;
            //保存上一个来源
            myRefer = item.Referer;
            //设置当前的来源
            if (!string.IsNullOrEmpty(refer)) item.Referer = refer;
            //设置返回类型
            item.ResultType = ResultType.String;
            //添加cookie
            item.Cookie = cookies;
            //设置host
            if (!string.IsNullOrEmpty(host)) item.Host = host;
            if (!string.IsNullOrEmpty(host)) item.Header["Origin"] = "http://" + host;
            //设置contenttype
            if (!string.IsNullOrEmpty(contentType)) item.ContentType = contentType;
            //设置post请求
            item.Method = "post";
            item.Postdata = postData;
            //从远程获取
            HttpResult result = client.GetHtml(item);
            //保存cookie
            if (!string.IsNullOrEmpty(result.Cookie)) setCookie(result.Cookie);
            //设置为原来的来源
            item.Referer = myRefer;
            //调试输出
            Debug.WriteLine("cookie@@" + cookies);
            Debug.WriteLine(result.Html);
            //返回字符串
            return result.Html;
        }
        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public Image GetImage(string url, string refer = "")
        {
            item.URL = url;
            item.ResultType = ResultType.Byte;
            myRefer = item.Referer;
            if (string.IsNullOrEmpty(refer)) item.Referer = refer;
            item.Cookie = cookies;
            HttpResult result = client.GetHtml(item);
            MemoryStream ms = new MemoryStream(result.ResultByte);
            Image outputImg = Image.FromStream(ms);
            ms.Close();
            ms.Dispose();
            cookies = result.Cookie;
            item.Referer = myRefer;
            return outputImg;
        }
    }
}
