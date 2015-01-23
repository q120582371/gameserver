using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shiwch.game
{
    /// <summary>
    /// 游戏Action标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class GameActionAttribute : Attribute
    {
        /// <summary>
        /// ActionId
        /// </summary>
        public int ActionId { get; set; }
        /// <summary>
        /// 响应的结构体
        /// </summary>
        public Type ResponseType { get; set; }
        /// <summary>
        /// 缓存时间，以秒为单位
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// 版本参数
        /// </summary>
        public string VaryByParam { get; set; }
    }
}
