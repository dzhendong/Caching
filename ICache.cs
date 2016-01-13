using System;
using System.Collections.Generic;

namespace ILvYou.Caching
{
	/// <summary>
	/// 缓存接口
	/// </summary>
	public interface ICache
    {
        #region Ctor
        bool Add<T>(string key, T value);  
              
        bool Add<T>(string prefix, string key, T value);	

        bool Add<T>(string prefix, string key, T value, TimeSpan duration);

        bool Set<T>(string key, T value);     
  
        bool Set<T>(string prefix, string key, T value);   
     
        bool Set<T>(string prefix, string key, T value, TimeSpan duration);
        
        void Clear(string prefix);

        void ClearAll();        

        void AsyncClear(string prefix);

        void AsyncClearAll();

        T Get<T>(string key);

        T Get<T>(string prefix, string key);
		
		IDictionary<string, object> MultiGet(IList<string> keys);

        void Remove(string key);    
    
		void Remove(string prefix,string key);

        void Remove(string prefix,string key,Guid timestamp);
        #endregion
    }
}
