using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GreenDonut;

namespace Afisha.Graphql.DataLoaders
{
    public class CustomStringLoader : DataLoaderBase<int, string>
    {
        protected override async Task<IReadOnlyList<Result<string>>> FetchAsync(IReadOnlyList<int> keys, CancellationToken cancellationToken)
        {
            var result = keys.ToDictionary(x => x, x => (x * 2).ToString()); //some hard work here

            var items = new Result<string>[keys.Count];

            for (int i = 0; i < keys.Count; i++)
            {
                if (result.TryGetValue(keys[i], out string value))
                {
                    items[i] = value;
                }
            }

            return items;
        }
    }
}
