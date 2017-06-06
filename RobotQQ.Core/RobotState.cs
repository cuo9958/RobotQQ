
namespace RobotQQ.Core
{
    /// <summary>
    /// 后台工作状态
    /// </summary>
    public enum RobotState
    {
        /// <summary>
        /// 等待状态：不工作
        /// </summary>
        Wait,
        /// <summary>
        /// 检查登录结果
        /// </summary>
        CheckLogin,
        /// <summary>
        /// 登录之后的初始化
        /// </summary>
        LoginInit,
        /// <summary>
        /// 检查消息
        /// </summary>
        PollMessage
    }
}
