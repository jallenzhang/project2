using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web.Caching;
using System.Web;

namespace LilyServer.Helper
{
    /// <summary>
    /// 数据缓存管理：自定义控件国际化功能(多语言)的缓存管理
    /// </summary>
    public class CacheHelper
    {
        #region 私有字段

        private static readonly Cache _cache;
        private static int Factor = 1;

        #endregion

        #region 构造函数

        #region 私有构造函数

        private CacheHelper() { }

        #endregion

        #region 静态构造函数，保证只有一个Cache

        /// <summary>
        /// 静态初始化，保证只有一个cache
        /// </summary>
        static CacheHelper()
        {           
             _cache = HttpRuntime.Cache;
        }

        #endregion

        #endregion

        #region 静态只读域，初始化日期、小时、分钟、秒过期因素

        /// <summary>
        /// 日期因素
        /// </summary>
        public static readonly int DayFactor = 17280;

        /// <summary>
        /// 小时因素
        /// </summary>
        public static readonly int HourFactor = 720;

        /// <summary>
        /// 分钟因素
        /// </summary>
        public static readonly int MinuteFactor = 12;

        /// <summary>
        /// 秒因素
        /// </summary>
        public static readonly double SecondFactor = 0.2;

        #endregion

        #region 清空应用程序的 Cache 对象

        /// <summary>
        /// 清空Cache对象
        /// </summary>
        public static void Clear()
        {
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            ArrayList al = new ArrayList();
            while (CacheEnum.MoveNext())
            {
                al.Add(CacheEnum.Key);
            }

            foreach (string key in al)
            {
                _cache.Remove(key);
            }

        }

        #endregion

        #region 从应用程序的 Cache 对象移除指定项

        /// <summary>
        /// 从应用程序的 Cache 对象移除指定项（根据键值移除Cache）
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            _cache.Remove(key);
        }

        /// <summary>
        /// 从应用程序的 Cache 对象移除指定项（根据正则表达式模式移除Cache）
        /// </summary>
        /// <param name="pattern">模式</param>
        public static void RemoveByPattern(string pattern)
        {
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            while (CacheEnum.MoveNext())
            {
                if (regex.IsMatch(CacheEnum.Key.ToString()))
                {
                    _cache.Remove(CacheEnum.Key.ToString());
                }
            }
        }
        #endregion

        #region 向 Cache 中插入具有依赖项和过期策略的对象。
        /// <summary>
        /// 向 Cache 中插入具有依赖项和过期策略的对象。
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键。</param>
        /// <param name="value">要插入缓存中的对象。</param>
        public static void Insert(string key, object value)
        {
            Insert(key, value, null, 1);
        }

        /// <summary>
        /// 向 Cache 中插入具有依赖项和过期策略的对象，附加缓存依赖。
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键。</param>
        /// <param name="value">要插入缓存中的对象。</param>
        /// <param name="dependencies">所插入对象的文件依赖项或缓存键依赖项。当任何依赖项更改时，该对象即无效，并从缓存中移除。如果没有依赖项，则此参数包含 null，比如设置和一个文件相关，文件一变，就失效。</param>
        public static void Insert(string key, object value, CacheDependency dependencies)
        {
            //Insert(key, value, dependencies, MinuteFactor * 3);
            _cache.Insert(key, value, dependencies);
        }

        /// <summary>
        /// 向 Cache 中插入具有依赖项和过期策略的对象，附加过期时间（多少秒后过期）。
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键。</param>
        /// <param name="value">要插入缓存中的对象。</param>
        /// <param name="seconds">秒因素（多少秒后过期）</param>
        public static void Insert(string key, object value, int seconds)
        {
            Insert(key, value, null, seconds);
        }

        /// <summary>
        /// 向 Cache 中插入具有依赖项和过期策略的对象，附加过期时间（多少秒后过期）及优先级。
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键。</param>
        /// <param name="value">要插入缓存中的对象。</param>
        /// <param name="seconds">秒因素（多少秒后过期）</param>
        /// <param name="priority">该对象相对于缓存中存储的其他项的成本，由 CacheItemPriority 枚举表示。该值由缓存在退出对象时使用；具有较低成本的对象在具有较高成本的对象之前被从缓存移除。</param>
        public static void Insert(string key, object value, int seconds, CacheItemPriority priority)
        {
            Insert(key, value, null, seconds, priority);
        }

        /// <summary>
        /// 向 Cache 中插入具有依赖项和过期策略的对象，附加缓存依赖、过期时间（多少秒后过期）及优先级（默认为Normal）。
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键。</param>
        /// <param name="value">要插入缓存中的对象。</param>
        /// <param name="dependencies">所插入对象的文件依赖项或缓存键依赖项。当任何依赖项更改时，该对象即无效，并从缓存中移除。如果没有依赖项，则此参数包含 null，比如设置和一个文件相关，文件一变，就失效。</param>
        /// <param name="seconds">秒因素（多少秒后过期）</param>
        public static void Insert(string key, object value, CacheDependency dependencies, int seconds)
        {
            Insert(key, value, dependencies, seconds, CacheItemPriority.Normal);
        }

        /// <summary>
        /// 向 Cache 中插入具有依赖项和过期策略的对象，附加缓存依赖、过期时间（多少秒后过期）及优先级。
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键。</param>
        /// <param name="value">要插入缓存中的对象。</param>
        /// <param name="dependencies">所插入对象的文件依赖项或缓存键依赖项。当任何依赖项更改时，该对象即无效，并从缓存中移除。如果没有依赖项，则此参数包含 null，比如设置和一个文件相关，文件一变，就失效。</param>
        /// <param name="seconds">秒因素（多少秒后过期）</param>
        /// <param name="priority">该对象相对于缓存中存储的其他项的成本，由 CacheItemPriority 枚举表示。该值由缓存在退出对象时使用；具有较低成本的对象在具有较高成本的对象之前被从缓存移除。</param>
        public static void Insert(string key, object value, CacheDependency dependencies, int seconds, CacheItemPriority priority)
        {
            if (value != null)
            {
                _cache.Insert(key, value, dependencies, DateTime.Now.AddSeconds(Factor * seconds), TimeSpan.Zero, priority, null);
            }

        }

        public static void Insert(string key, object value, CacheDependency dependencies, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
        {
            if (value!=null)
            {
                _cache.Insert(key, value, dependencies,Cache.NoAbsoluteExpiration, TimeSpan.Zero, priority, onRemoveCallback);
            }
        }
        public static void Insert(string key, object value, int seconds, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
        {
            if (value != null)
            {
                _cache.Insert(key, value, null, DateTime.Now.AddSeconds(seconds), TimeSpan.Zero, priority, onRemoveCallback);
            }
        }

        /// <summary>
        /// 向 Cache 中插入具有依赖项和过期策略的对象，并忽略优先级。
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键。</param>
        /// <param name="value">要插入缓存中的对象。</param>
        /// <param name="second"></param>
        public static void InsertByMicro(string key, object value, int second)
        {
            if (value != null)
            {
                _cache.Insert(key, value, null, DateTime.Now.AddSeconds(Factor * second), TimeSpan.Zero);
            }
        }

        /// <summary>
        /// 向 Cache 中插入具有依赖项和过期策略的对象，把过期时间设为最大值。
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键。</param>
        /// <param name="value">要插入缓存中的对象。</param>
        public static void InsertByMax(string key, object value)
        {
            InsertByMax(key, value, null);
        }

        /// <summary>
        /// 向 Cache 中插入具有依赖项和过期策略的对象，把过期时间设为最大值，附加缓存依赖信息。
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键。</param>
        /// <param name="value">要插入缓存中的对象。</param>
        /// <param name="dependencies">所插入对象的文件依赖项或缓存键依赖项。当任何依赖项更改时，该对象即无效，并从缓存中移除。如果没有依赖项，则此参数包含 null，比如设置和一个文件相关，文件一变，就失效。</param>
        public static void InsertByMax(string key, object value, CacheDependency dependencies)
        {
            if (value != null)
            {
                _cache.Insert(key, value, dependencies, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.AboveNormal, null);
            }
        }

        /// <summary>
        /// 向 Cache 中插入具有依赖项和过期策略的对象，插入持久性缓存。
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键。</param>
        /// <param name="value">要插入缓存中的对象。</param>
        public static void InsertByPermanent(string key, object value)
        {
            InsertByPermanent(key, value, null);
        }

        /// <summary>
        /// 向 Cache 中插入具有依赖项和过期策略的对象，插入持久性缓存，附加缓存依赖。
        /// </summary>
        /// <param name="key">用于引用该对象的缓存键。</param>
        /// <param name="value">要插入缓存中的对象。</param>
        /// <param name="dependencies">所插入对象的文件依赖项或缓存键依赖项。当任何依赖项更改时，该对象即无效，并从缓存中移除。如果没有依赖项，则此参数包含 null，比如设置和一个文件相关，文件一变，就失效。</param>
        public static void InsertByPermanent(string key, object value, CacheDependency dependencies)
        {
            if (value != null)
            {
                _cache.Insert(key, value, dependencies, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.NotRemovable, null);
            }
        }

        #endregion

        #region 获取 Cache 中的对象。
        /// <summary>
        /// 获取 Cache 中的对象。
        /// </summary>
        /// <param name="key">缓存项的 String 标识符</param>
        /// <returns>缓存中的对象</returns>
        public static object Get(string key)
        {
            return _cache[key];
        }
        #endregion

        #region 因素设定及计算
        /// <summary>
        /// 秒因素计算
        /// </summary>
        /// <param name="seconds">秒因素（多少秒后过期）</param>
        public static int SecondFactorCalculate(int seconds)
        {
            return Convert.ToInt32(Math.Round((double)seconds * SecondFactor));
        }

        /// <summary>
        /// 重新设置因素
        /// </summary>
        /// <param name="cacheFactor">要设置的因素值</param>
        public static void ReSetFactor(int cacheFactor)
        {
            Factor = cacheFactor;
        }
        #endregion
    }
}
