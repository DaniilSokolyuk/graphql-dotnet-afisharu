﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;

namespace Afisha.Graphql.Infrastructure.DataLoader.GreenDonutDataLoader
{
    internal sealed class FetchDataLoader<TKey, TValue>
        : DataLoaderBase<TKey, TValue>
    {
        private readonly FetchBatch<TKey, TValue> _fetch;

        public FetchDataLoader(FetchBatch<TKey, TValue> fetch)
            : base(new DataLoaderOptions<TKey>
            {
                AutoDispatching = false,
                Batching = true,
                CacheSize = DataLoaderDefaults.CacheSize,
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
            IReadOnlyDictionary<TKey, TValue> result =
                await _fetch(keys).ConfigureAwait(false);

            var items = new Result<TValue>[keys.Count];

            for (int i = 0; i < keys.Count; i++)
            {
                if (result.TryGetValue(keys[i], out TValue value))
                {
                    items[i] = value;
                }
            }

            return items;
        }
    }
}
