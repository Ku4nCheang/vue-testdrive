namespace netcore.Core.Caches
{
    public class CacheEntry<T> where T: class {
        public string Key;
        public T Value; 
    }
}