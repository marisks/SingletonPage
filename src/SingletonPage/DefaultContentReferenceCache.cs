using System;
using System.Collections.Concurrent;
using EPiServer.Core;

namespace Marisks.SingletonPage
{
    public class DefaultContentReferenceCache : IContentReferenceCache
    {
        protected ConcurrentDictionary<CacheKey, ContentReference> Cache { get; }

        public DefaultContentReferenceCache()
        {
            Cache = new ConcurrentDictionary<CacheKey, ContentReference>();
        }

        public ContentReference GetOrAdd(CacheKey key, Func<ContentReference> valueFactory)
        {
            var value = valueFactory();
            return value == ContentReference.EmptyReference ? value : Cache.GetOrAdd(key, value);
        }
    }
}