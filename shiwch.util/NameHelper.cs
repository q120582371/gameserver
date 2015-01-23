using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shiwch.util
{
    public static class NameHelper
    {
        #region 私有
        static string[] _firstName = new string[]{  
                    "白","毕","卞","蔡","曹","岑","常","车","陈","成" ,"程","池","邓","丁","范","方","樊","费","冯","符"
                   ,"傅","甘","高","葛","龚","古","关","郭","韩","何" ,"贺","洪","侯","胡","华","黄","霍","姬","简","江"
                   ,"姜","蒋","金","康","柯","孔","赖","郎","乐","雷" ,"黎","李","连","廉","梁","廖","林","凌","刘","柳"
                   ,"龙","卢","鲁","陆","路","吕","罗","骆","马","梅" ,"孟","莫","母","穆","倪","宁","欧","区","潘","彭"
                   ,"蒲","皮","齐","戚","钱","强","秦","丘","邱","饶" ,"任","沈","盛","施","石","时","史","司徒","苏","孙"
                   ,"谭","汤","唐","陶","田","童","涂","王","危","韦" ,"卫","魏","温","文","翁","巫","邬","吴","伍","武"
                   ,"席","夏","萧","谢","辛","邢","徐","许","薛","严" ,"颜","杨","叶","易","殷","尤","于","余","俞","虞"
                   ,"元","袁","岳","云","曾","詹","张","章","赵","郑" ,"钟","周","邹","朱","褚","庄","卓","东方","上官"
                   ,"令狐","申屠","欧阳","诸葛","尉迟" };

        static string _lastNameMale = "伟刚勇毅俊峰强军平保东文辉力明永健世广志义兴良海山仁波宁贵福生龙元全国胜学祥才发武新利清飞彬富顺信子杰涛昌成康星光天达安岩中茂进林有坚和彪博诚先敬震振壮会思群豪心邦承乐绍功松善厚庆磊民友裕河哲江超浩亮政谦亨奇固之轮翰朗伯宏言若鸣朋斌梁栋维启克伦翔旭鹏泽晨辰士以建家致树炎德行时泰盛雄琛钧冠策腾楠榕风航弘";

        static string _lastNameFeMa = "秀娟英华慧巧美娜静淑惠珠翠雅芝玉萍红娥玲芬芳燕彩春菊兰凤洁梅琳素云莲真环雪荣爱妹霞香月莺媛艳瑞凡佳嘉琼勤珍贞莉桂娣叶璧璐娅琦晶妍茜秋珊莎锦黛青倩婷姣婉娴瑾颖露瑶怡婵雁蓓纨仪荷丹蓉眉君琴蕊薇菁梦岚苑婕馨瑗琰韵融园艺咏卿聪澜纯毓悦昭冰爽琬茗羽希宁欣飘育滢馥筠柔竹霭凝鱼晓欢霄枫芸菲寒伊亚宜可姬舒影荔枝思丽墨";
        
        private static Random rnd = new Random();
        private static int RndNext(int min, int max)
        {
            lock (rnd)
            {
                return rnd.Next(min, max);
            }
        }
        /// <summary>
        /// 返回指定范围内不重复的随机数组，如范围小于数组长度，则返回最多能生成的随机数组
        /// </summary>
        private static int[] GenerateNonRepeatArray(int max, int count)
        {
            if (max <= 0) { throw new Exception("范围不能等于或小于0"); }

            int size = count > max ? count - max : 0;

            List<int> temp = new List<int>();
            Random ran = new Random(Guid.NewGuid().GetHashCode());

            while (temp.Count < (count - size))
            {
                int item = ran.Next(max);
                if (!temp.Contains(item))
                {
                    temp.Add(item);
                }
            }

            return temp.ToArray();
        }
        #endregion

        /// <summary>
        /// 0 不限定,1男 2女
        /// </summary>
        public static string GetSingleName(int Sex)
        {
            if (Sex == 0) Sex = RndNext(1, 3);
            int NameLength = RndNext(1, 3);
            var Name = "";
            if (Sex == 1)
            {
                for (int i = 0; i < NameLength; i++)
                {
                    Name += _lastNameMale[RndNext(0, _lastNameMale.Length)];
                }
            }
            if (Sex == 2)
            {
                for (int i = 0; i < NameLength; i++)
                {
                    Name += _lastNameFeMa[RndNext(0, _lastNameMale.Length)];
                }
            }

            var FirstName = _firstName[RndNext(0, _firstName.Length)];

            return FirstName + Name;
        }

    }
}
