using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shiwch.game
{
    /// <summary>
    /// 参数校验属性，默认：必须传参，自动Trim，不允许空字符串，字符串无最大上限，数值必须大于零
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ParamCheckAttribute : Attribute
    {
        /// <summary>
        /// 是否允许不传该参数
        /// </summary>
        public bool AllowDefault { get; set; }
        /// <summary>
        /// 是否自动Trim
        /// </summary>
        public bool Trim { get; set; }
        /// <summary>
        /// 是否允许空字符串
        /// </summary>
        public bool AllowStringEmpty { get; set; }
        /// <summary>
        /// 字符串最大长度
        /// </summary>
        public int MaxStringLength { get; set; }
        /// <summary>
        /// 数值是否必须大于零
        /// </summary>
        public bool IsPositive { get; set; }
        /// <summary>
        /// 跳过所有检查
        /// </summary>
        public bool Ignore { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ParamCheckAttribute()
        {
            AllowDefault = false;
            Trim = true;
            AllowStringEmpty = false;
            IsPositive = true;
            MaxStringLength = int.MaxValue;
        }
    }
}
