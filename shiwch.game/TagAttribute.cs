using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shiwch.game
{
    /// <summary>
    /// 输出顺序标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class TagAttribute : Attribute
    {
        /// <summary>
        /// 顺序号
        /// </summary>
        public int Tag { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tag"></param>
        public TagAttribute(int tag)
        {
            Tag = tag;
        }
    }
}
