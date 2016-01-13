using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MemcachedProviders.Cache;

namespace ILvYou.Caching
{
	/// <summary>
	/// Memcached缓存
	/// </summary>
	public class MemcachedCache : CacheBase
    {
        #region Ctor
        private static object objLock = new object();        
        private static volatile MemcachedCache memcachedCache = null;
        static IDictionary<string, string> cacheKeys = new Dictionary<string, string>();
        
        private MemcachedCache() { }

        static MemcachedCache()
        {
            if (memcachedCache == null)
            {
                memcachedCache = new MemcachedCache();
            }            
        }

        public static MemcachedCache Instance
        {
            get
            {
                return memcachedCache;
            }
        }
        #endregion

        #region APIs
        public override bool Add<T>(string prefix,string key, T value, TimeSpan duration)
		{
            lock (objLock)
            {
                if (duration <= TimeSpan.Zero)
                {
                    duration = this.MaxDuration;
                }
                string fullName = this.GetFullName(prefix, key);
                bool b1 = DistCache.Add(fullName, value);
                bool b2 = cacheKeys.ContainsKey(fullName);
                if (b1 && !b2) cacheKeys.Add(fullName, fullName);
                return b1;
            }
		}

        public override bool Set<T>(string prefix, string key, T value, TimeSpan duration)
        {
            return Add(prefix,key, value, duration);
        }

        private void Clean(Object paras)
        {            
            string prefix = paras as string;
            Clear(prefix);
        }

        public override void Clear(string prefix)
		{
            if (string.IsNullOrWhiteSpace(prefix)) return;
            IList<string> keys = new List<string>();

            foreach (KeyValuePair<string, string> kvp in cacheKeys)
            {
                if (kvp.Key.Contains(prefix))
                {
                    DistCache.Remove(kvp.Key);
                    keys.Add(kvp.Key);
                }                
            }

            foreach (string key in keys)
            {
                cacheKeys.Remove(key);
            }
		}
        
        public override void ClearAll()
        {
            DistCache.RemoveAll();

            IList<string> keys = new List<string>();

            foreach (KeyValuePair<string, string> kvp in cacheKeys)
            {
                keys.Add(kvp.Key);
            }

            foreach (string key in keys)
            {
                cacheKeys.Remove(key);
            }
        }

        public override void AsyncClear(string prefix)
        {
            Object paras = prefix;
            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(Clean);
            Thread thread = new Thread(threadStart);
            thread.IsBackground = true;
            thread.Start(paras);   
        }
        
        public override void AsyncClearAll()
        {
            ThreadStart threadStart = new ThreadStart(ClearAll);
            Thread thread = new Thread(threadStart);
            thread.Start();
        }

		public override T Get<T>(string prefix,string key)
		{
            return DistCache.Get<T>(this.GetFullName(prefix,key));			
		}

		public override IDictionary<string, object> MultiGet(IList<string> keys)
		{			
            IDictionary<string, object> result = new Dictionary<string, object>();
            foreach (string key in keys)
            {
                result.Add(key, this.Get<object>(key));
            }

            return result;			
		}

        public override void Remove(string prefix, string key, Guid timestamp)
		{
            string fullName = this.GetFullName(prefix,key); 
			DistCache.Remove(fullName);
            cacheKeys.Remove(fullName);
        }
        #endregion
    }
}
