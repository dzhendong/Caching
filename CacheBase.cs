using System;
using System.Collections.Generic;

namespace ILvYou.Caching
{
    /// <summary>
    /// 缓存基类
    /// </summary>
    public abstract class CacheBase : ICache
    {
        #region Ctor
        private TimeSpan maxDuration = TimeSpan.FromDays(15);

        /// <summary>
        /// 最长持续时间
        /// </summary>
        public TimeSpan MaxDuration
        {
            get
            {
                return this.maxDuration;
            }            
        }

        /// <summary>
        ///  获取全名
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>全名</returns>
        public virtual string GetFullName(string prefix,string key)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                string result = string.Format("{0}.{1}", prefix, key);
                return result;
            }

            return key;
        }
        #endregion

        #region APIs
        public bool Add<T>(string key, T value)
        {
            return this.Add<T>("",key, value);
        }

        public bool Add<T>(string prefix, string key, T value)
        {
            return this.Add<T>(prefix,key, value, this.MaxDuration);
        }

        public abstract bool Add<T>(string prefix, string key, T value, TimeSpan duration);

        public bool Set<T>(string key, T value)
        {
            return this.Set<T>("",key, value);
        }

        public bool Set<T>(string prefix, string key, T value)
        {
            return this.Set<T>(prefix,key, value, this.MaxDuration);
        }

        public abstract bool Set<T>(string prefix, string key, T value, TimeSpan duration);

        public abstract void Clear(string prefix);

        public abstract void ClearAll();

        public abstract void AsyncClear(string prefix);

        public abstract void AsyncClearAll();

        public T Get<T>(string key)
        {
            return this.Get<T>("", key);
        }

        public abstract T Get<T>(string prefix,string key);
        
        public abstract IDictionary<string, object> MultiGet(IList<string> keys);

        public void Remove(string key)
        {
            this.Remove("", key);
        }

        public void Remove(string prefix, string key)
        {
            this.Remove(prefix, key, Guid.NewGuid());
        }

        public abstract void Remove(string prefix, string key, Guid timestamp);
        #endregion
    }
}
