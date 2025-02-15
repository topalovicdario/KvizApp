using Microsoft.Extensions.Caching.Memory;

namespace ServerKVIZ.Services
{
    public class LocalDataBase 
    {
        private IMemoryCache _cache;
        public LocalDataBase(IMemoryCache cache)
        {
            _cache = cache;
        }
      
        public void StorePlayers()
        {

        }
             

    }
}
