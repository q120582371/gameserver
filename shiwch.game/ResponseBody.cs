using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shiwch.game
{
    [ProtoContract]
    public class ResponseBody
    {
        [ProtoMember(1)]
        public int StatusCode { get; set; }
        [ProtoMember(2)]
        public string Description { get; set; }
        [ProtoMember(3)]
        public int ReplyTo { get; set; }
        public object Body { get; set; }
    }
}
