
using System.Collections.Generic;

namespace RobotQQ.Core
{
    public class Result_message
    {
        public string poll_type { get; set; }
        public Result_message_value value { get; set; }
    }
    public class Result_message_value
    {
        public List<object> content { get; set; }
        public long from_uin { get; set; }
        public long group_code { get; set; }
        public long msg_id { get; set; }
        public int msg_type { get; set; }
        public long send_uin { get; set; }
        public long time { get; set; }
        public long to_uin { get; set; }
    }
}
