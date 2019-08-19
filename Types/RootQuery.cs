using System.Collections.Generic;
using System.Linq;
using Afisha.Graphql.DataLoaders;
using Afisha.Graphql.Infrastructure;
using Afisha.Graphql.Infrastructure.DataLoader.GreenDonutDataLoader;
using GraphQL.Types;

namespace Afisha.Graphql.Types
{
    public class RootQuery : ObjectGraphType
    {
        public RootQuery()
        {
            Field<ListGraphType<StringGraphType>, IReadOnlyCollection<string>>()
                .Name("customLoaderTest")
                .Argument<NonNullGraphType<IntGraphType>>("count", "")
                .ResolveAsync(async ctx =>
                {
                    var count = ctx.GetArgument<int>("count");

                    var keys = Enumerable.Range(0, count).ToArray();

                    var multipleKeys = await ctx.Fix().DataLoader<CustomStringLoader>().LoadAsync(keys, ctx.CancellationToken);
                    var singleKey = await ctx.Fix().DataLoader<CustomStringLoader>().LoadAsync(keys.Last(), ctx.CancellationToken);

                    return multipleKeys.Append(singleKey).ToArray();
                });

            Field<ListGraphType<StringGraphType>, IReadOnlyCollection<string>>()
                .Name("fetchLoaderTest")
                .Argument<NonNullGraphType<IntGraphType>>("count", "")
                .ResolveAsync(async ctx =>
                {
                    var count = ctx.GetArgument<int>("count");

                    var keys = Enumerable.Range(0, count).ToArray();

                    var dataLoader = ctx.Fix().BatchDataLoader<int, string>("fetchLoaderTest", async list => list.ToDictionary(x => x, x => (x * 2).ToString()));

                    var multipleKeys = await dataLoader.LoadAsync(keys, ctx.CancellationToken);
                    var singleKey = await dataLoader.LoadAsync(keys.Last(), ctx.CancellationToken);

                    return multipleKeys.Append(singleKey).ToArray();
                });
        }
    }
}
