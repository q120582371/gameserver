using shiwch.util;
using shiwch.util.FastMember;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace shiwch.game
{
    public interface IRequestParamParser
    {
        IDictionary<string, string> Parse(byte[] param);
    }

    public class UrlParamParser : IRequestParamParser
    {
        IDictionary<string, string> IRequestParamParser.Parse(byte[] param)
        {
            string strParam = Encoding.UTF8.GetString(param);
            NameValueCollection nvc = HttpUtility.ParseQueryString(strParam);
            IDictionary<string, string> result = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            for (int i = 0; i < nvc.Keys.Count; i++)
            {
                var k = nvc.Keys[i];
                var v = nvc[i];
                if (k != null)
                {
                    result[k] = v;
                }
            }
            return result;
        }
    }
}
