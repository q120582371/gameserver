using shiwch.util;
using shiwch.util.FastMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shiwch.game
{
    public abstract class GameHandler
    {
        private GameContext context;
        private Dictionary<string, GameEntity> cachedEntity = new Dictionary<string, GameEntity>();
        private static Type checkType = typeof(ParamCheckAttribute);

        protected GameContext Context { get { return context; } }
        protected GameRequest Request { get { return context.Request; } }
        protected GameResponse Response { get { return context.Response; } }
        protected GameSession Session { get { return context.Session; } }

        internal GameHandler(GameContext context)
        {
            this.context = context;
        }

        internal bool Init(IDictionary<string, string> param)
        {
            bool preCheckResult = true;

            Type type = GetType();
            bool AllowDefault = false;
            bool AllowStringEmpty = false;
            int MaxStringLength = int.MaxValue;
            bool Trim = true;
            bool IsPositive = true;
            string errmsg = "";
            ObjectAccessor accessor = ObjectAccessor.Create(this);
            foreach (var p in type.GetProperties())
            {
                if (p.GetSetMethod() == null) continue;
                object[] attrs = p.GetCustomAttributes(checkType, false);
                bool needCheck = true;

                #region 获取校验相关参数
                if (attrs.Length == 1)
                {
                    ParamCheckAttribute attr = attrs[0] as ParamCheckAttribute;
                    if (attr.Ignore)
                    {
                        needCheck = false;
                    }
                    else
                    {
                        AllowDefault = attr.AllowDefault;
                        AllowStringEmpty = attr.AllowStringEmpty;
                        MaxStringLength = attr.MaxStringLength;
                        Trim = attr.Trim;
                        IsPositive = attr.IsPositive;
                        errmsg = attr.ErrorMsg;
                    }
                }
                else
                {
                    AllowDefault = false;
                    AllowStringEmpty = false;
                    MaxStringLength = int.MaxValue;
                    Trim = true;
                    IsPositive = true;
                    errmsg = string.Empty;
                }
                #endregion

                #region check
                if (needCheck)
                {
                    if (!AllowDefault && !param.ContainsKey(p.Name))
                    {
                        errmsg = string.IsNullOrEmpty(errmsg) ? string.Format("缺少参数[{0}]", p.Name) : errmsg;
                        preCheckResult = false;
                        break;
                    }
                    if (!AllowStringEmpty && param[p.Name] == string.Empty)
                    {
                        errmsg = string.IsNullOrEmpty(errmsg) ? string.Format("参数[{0}]不允许空字符串", p.Name) : errmsg;
                        preCheckResult = false;
                        break;
                    }
                    if (param[p.Name].Length > MaxStringLength)
                    {
                        errmsg = string.IsNullOrEmpty(errmsg) ? string.Format("参数[{0}]超过最大允许长度[{1}]", p.Name, MaxStringLength) : errmsg;
                        preCheckResult = false;
                        break;
                    }
                }
                #endregion

                if (param.ContainsKey(p.Name))
                {
                    #region 根据属性赋值
                    if (p.PropertyType.IsEnum)
                    {
                        int v;
                        if (!int.TryParse(param[p.Name], out v))
                        {
                            errmsg = string.IsNullOrEmpty(errmsg) ? string.Format("参数[{0}]无法解析为有效的枚举值", p.Name) : errmsg;
                            preCheckResult = false;
                            break;
                        }
                        accessor[p.Name] = Enum.ToObject(p.PropertyType, v);
                    }
                    else if (p.PropertyType == typeof(bool))
                    {
                        accessor[p.Name] = param[p.Name] == "1";
                    }
                    else if (p.PropertyType == typeof(string))
                    {
                        accessor[p.Name] = param[p.Name];
                    }
                    else
                    {
                        var typecode = Type.GetTypeCode(p.PropertyType);

                        if (typecode == TypeCode.Int16 ||
                            typecode == TypeCode.Int32 ||
                            typecode == TypeCode.Int64 ||
                            typecode == TypeCode.SByte)
                        {
                            #region 无符号数
                            long v;
                            if (long.TryParse(param[p.Name], out v))
                            {
                                if (needCheck && IsPositive && v <= 0)
                                {
                                    errmsg = string.IsNullOrEmpty(errmsg) ? string.Format("参数[{0}]必须是正数", p.Name) : errmsg;
                                    preCheckResult = false;
                                    break;
                                }
                                accessor[p.Name] = Convert.ChangeType(v, p.PropertyType);
                            }
                            else
                            {
                                errmsg = string.IsNullOrEmpty(errmsg) ? string.Format("参数[{0}]无法解析为数值", p.Name) : errmsg;
                                preCheckResult = false;
                                break;
                            }
                            #endregion
                        }
                        else if (typecode == TypeCode.Byte ||
                                 typecode == TypeCode.UInt16 ||
                                 typecode == TypeCode.UInt32 ||
                                 typecode == TypeCode.UInt64)
                        {
                            #region 有符号数
                            ulong v;
                            if (ulong.TryParse(param[p.Name], out v))
                            {
                                if (needCheck && IsPositive && v <= 0)
                                {
                                    errmsg = string.IsNullOrEmpty(errmsg) ? string.Format("参数[{0}]必须是正数", p.Name) : errmsg;
                                    preCheckResult = false;
                                    break;
                                }
                                accessor[p.Name] = Convert.ChangeType(v, p.PropertyType);
                            }
                            else
                            {
                                errmsg = string.IsNullOrEmpty(errmsg) ? string.Format("参数[{0}]无法解析为数值", p.Name) : errmsg;
                                preCheckResult = false;
                                break;
                            }
                            #endregion
                        }
                        else if (typecode == TypeCode.Double || typecode == TypeCode.Single)
                        {
                            #region 浮点数
                            double v;
                            if (double.TryParse(param[p.Name], out v))
                            {
                                accessor[p.Name] = Convert.ChangeType(v, p.PropertyType);
                            }
                            else
                            {
                                errmsg = string.IsNullOrEmpty(errmsg) ? string.Format("参数[{0}]无法解析为数值", p.Name) : errmsg;
                                preCheckResult = false;
                                break;
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
            }

            if (preCheckResult)
            {
                return InitParams(param);
            }
            else
            {
                context.Response.StatusCode = StatusCode.ParamError;
                context.Response.Description = errmsg;
                return false;
            }
        }

        protected virtual bool InitParams(IDictionary<string, string> param)
        {
            return true;
        }

        internal async Task DoWork()
        {
            await BeforeProcess();
            await Process();
            await AfterProcess();
        }

        protected virtual Task BeforeProcess()
        {
            return Task.FromResult<object>(null);
        }

        protected abstract Task Process();

        protected virtual Task AfterProcess()
        {
            return Task.FromResult<object>(null);
        }

        public void SetDescription(int statuscode, string description)
        {

        }

        public void SetStatus(int statuscode, string resourceid, params object[] args)
        {

        }

        protected async Task<T> Load<T>(long id) where T : GameEntity
        {
            var cacheKey = string.Format("{0}:{1}", typeof(T).Name, id);
            GameEntity result;
            if (cachedEntity.TryGetValue(cacheKey, out result)) return (T)result;
            byte[] byteResult;
            if (DataStorage.Instance.Dict.TryGetValue(cacheKey, out byteResult))
            {
                result = NetworkBytes.AsObject<T>(byteResult);
                cachedEntity[cacheKey] = result;
            }
            else
            {
                var redis = RedisHelper.GetMultiplexer(ParamHelper.Redis).GetDatabase();
                var redisValue = await redis.StringGetAsync(cacheKey);
                if (!redisValue.IsNull)
                {
                    result = NetworkBytes.AsObject<T>(redisValue);
                    if (result != null)
                    {
                        cachedEntity[cacheKey] = result;
                        DataStorage.Instance.Dict[cacheKey] = redisValue;
                    }
                }
            }
            return (T)result;
        }
    }
}
