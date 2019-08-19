using System;

namespace Afisha.Graphql.Infrastructure.DataLoader.GreenDonutDataLoader
{
    public static class DataLoaderRegistryExtensions
    {
        public static bool Register<TKey, TValue>(
            this IDataLoaderRegistry registry,
            string key,
            FetchBatchFactory<TKey, TValue> factory)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(
                    "KeyNullOrEmpty",
                    nameof(key));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return registry.Register(key, services =>
                new FetchDataLoader<TKey, TValue>(
                    factory(services)));
        }

        public static bool Register<TKey, TValue>(
            this IDataLoaderRegistry registry,
            string key,
            FetchGroupeFactory<TKey, TValue> factory)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(
                    "KeyNullOrEmpty",
                    nameof(key));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return registry.Register(key, services =>
                new FetchGroupedDataLoader<TKey, TValue>(
                    factory(services)));
        }


        public static bool Register<TKey, TValue>(
            this IDataLoaderRegistry registry,
            string key,
            FetchCacheFactory<TKey, TValue> factory)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(
                    "KeyNullOrEmpty",
                    nameof(key));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return registry.Register(key, services =>
                new FetchSingleDataLoader<TKey, TValue>(
                    factory(services)));
        }

        public static bool Register<TValue>(
            this IDataLoaderRegistry registry,
            string key,
            FetchOnceFactory<TValue> factory)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(
                    "KeyNullOrEmpty",
                    nameof(key));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return registry.Register(key, services =>
            {
                FetchOnce<TValue> fetch = factory(services);
                return new FetchSingleDataLoader<string, TValue>(
                    k => fetch(), DataLoaderDefaults.MinCacheSize);
            });
        }
    }
}
