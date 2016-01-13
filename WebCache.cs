using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using System.Web.Caching;

namespace ILvYou.Caching
{
	/// <summary>
	/// WebCache 缓存
	/// </summary>
    public class WebCache : CacheBase
    {
        #region Ctor
        private static volatile WebCache webCache = null;        
		private System.Web.Caching.Cache cache = HttpRuntime.Cache;
        private static object objLock = new object();        

        private WebCache() { }

        static WebCache()
        {
            if (webCache == null)
            {
                webCache = new WebCache();
            }            
        }

        public static WebCache Instance
        {
            get
            {
                return webCache;
            }
        }
        #endregion

        #region APIs
        public override bool Add<T>(string prefix,string key, T value, TimeSpan duration)
		{
			bool result = false;

            lock (objLock)
            {
			    if (value != null)
			    {
				    if (duration <= TimeSpan.Zero)
				    {
					    duration = this.MaxDuration;
				    }

                    result = this.cache.Add(this.GetFullName(prefix,key), value, null, DateTime.Now.Add(duration), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Default, null) == null;
			    }
            }
			return result;
		}
        
        public override bool Set<T>(string prefix,string key, T value, TimeSpan duration)
        {
            Remove(prefix,key);

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
			IDictionaryEnumerator caches = this.cache.GetEnumerator();
			while (caches.MoveNext())
			{
				string key = caches.Key.ToString();
                if (key.Contains(prefix))
				{
					keys.Add(key);
				}
			}

            foreach (string key in keys)
            {
                this.cache.Remove(key);
            }			
		}
        
        public override void ClearAll()
        {
            IList<string> keys = new List<string>();
            IDictionaryEnumerator caches = this.cache.GetEnumerator();
            while (caches.MoveNext())
            {
                keys.Add(caches.Key.ToString());                
            }

            foreach (string key in keys)
            {
                this.cache.Remove(key);
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
			T result = default(T);
            object value = this.cache.Get(this.GetFullName(prefix,key));
			if (value is T)
			{
				result = (T)value;
			}

			return result;
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
            this.cache.Remove(this.GetFullName(prefix,key));
        }
        #endregion
    }
}
