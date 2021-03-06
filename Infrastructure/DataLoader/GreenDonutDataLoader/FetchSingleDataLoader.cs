﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;

namespace Afisha.Graphql.Infrastructure.DataLoader.GreenDonutDataLoader
{
    internal sealed class FetchSingleDataLoader<TKey, TValue>
        : DataLoaderBase<TKey, TValue>
    {
        private readonly FetchCache<TKey, TValue> _fetch;

        public FetchSingleDataLoader(FetchCache<TKey, TValue> fetch)
            : this(fetch, DataLoaderDefaults.CacheSize)
        {
        }

        public FetchSingleDataLoader(
            FetchCache<TKey, TValue> fetch,
            int cacheSize)
            : base(new DataLoaderOptions<TKey>
            {
                AutoDispatching = false,
                Batching = false,
                CacheSize = cacheSize,
                MaxBatchSize = DataLoaderDefaults.MaxBatchSize,
                SlidingExpiration = TimeSpan.Zero
            })
        {
            _fetch = fetch ?? throw new ArgumentNullException(nameof(fetch));
        }

        protected override async Task<IReadOnlyList<Result<TValue>>> FetchAsync(
            IReadOnlyList<TKey> keys,
            CancellationToken cancellationToken)
        {
            var items = new Result<TValue>[keys.Count];

            for (int i = 0; i < keys.Count; i++)
            {
                try
                {
                    TValue value = await _fetch(keys[i]).ConfigureAwait(false);
                    items[i] = value;
                }
                catch (Exception ex)
                {
                    items[i] = ex;
                }
            }

            return items;
        }
    }
}
