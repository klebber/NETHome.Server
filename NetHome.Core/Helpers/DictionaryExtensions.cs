﻿using System.Collections.Generic;
using System.Collections.Specialized;

namespace NetHome.Core.Helpers
{
    public static class DictionaryExtensions
    {
        public static NameValueCollection ToNameValueCollection<T>(this IDictionary<string, T> dictionary)
        {
            var collection = new NameValueCollection();
            foreach (var pair in dictionary)
                collection.Add(pair.Key, pair.Value?.ToString());
            return collection;
        }
    }
}
