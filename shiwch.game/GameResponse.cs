using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shiwch.game
{
    public class GameResponse
    {
        internal GameResponse()
        {
            StatusCode = shiwch.game.StatusCode.Success;
            ResponseBody = new List<object>();
        }
        /// <summary>
        /// 操作码
        /// </summary>
        public int ActionId { get; internal set; }
        /// <summary>
        /// 结果状态码
        /// </summary>
        public int StatusCode { get; internal set; }
        /// <summary>
        /// 结果描述
        /// </summary>
        public string Description { get; internal set; }
        /// <summary>
        /// 回应的消息Id
        /// </summary>
        public int ReplyTo { get; internal set; }
        /// <summary>
        /// 响应体
        /// </summary>
        [JsonProperty]
        internal List<object> ResponseBody { get; set; }

        public void Write(object segment)
        {
            ResponseBody.Add(segment);
        }

        public void SetDescription(int statuscode, string description)
        {
            StatusCode = statuscode;
            Description = description;
        }

        public void SetStatus(int statuscode, string resourceid, params object[] args)
        {

        }
    }
}
