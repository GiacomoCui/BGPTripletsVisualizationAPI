using System.Collections.Generic;

namespace Shared.Extensions
{
	public static class DictionaryExtensions
	{
		public static TValue GetValueOrAddDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
		{
			if(!dictionary.TryGetValue(key, out var v))
			{
				dictionary.Add(key, defaultValue);
				v = defaultValue;
			}
			return v;
		}
	}
}
