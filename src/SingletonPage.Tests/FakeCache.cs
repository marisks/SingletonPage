using System.Collections.Concurrent;
using EPiServer.Core;

namespace Marisks.SingletonPage.Tests
{
    public class FakeCache : DefaultContentReferenceCache
    {
        public ConcurrentDictionary<CacheKey, ContentReference> InternalCache => Cache;
    }
}