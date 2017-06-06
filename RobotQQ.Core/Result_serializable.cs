
using System.Collections.Generic;

namespace RobotQQ.Core
{
    sealed class Result_serializable
    {
        public RobotState state { get; set; }
        public RobotStatus status { get; set; }
        public string config_url { get; set; }
        public string _qrsig { get; set; }
        public string Ptwebqq { get; set; }
        public string Vfwebqq { get; set; }
        public string Psessionid { get; set; }
        public long uin { get; set; }
        public List<string> lastPeople { get; set; }
        public List<string> friends { get; set; }
        public List<string> groups { get; set; }
        public string cookies { get; set; }
    }
}
