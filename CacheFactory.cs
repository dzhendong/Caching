using System.Configuration;

namespace ILvYou.Caching
{
    /// <summary>
    /// 缓存工厂
    /// </summary>
    public static class CacheFactory
    {
        #region Ctor
        private static CacheBase cacheBase = null;        

        static CacheFactory()
        {            
            if (cacheBase == null)
            {
                lock (typeof(CacheBase))
                {
                    if (cacheBase == null)
                    {
                        cacheBase = WebCache.Instance;
                    }
                }
            }            
        }

        public static CacheBase Instance
        {
            get
            {
                return cacheBase;
            }
        }
        #endregion
    }
}

