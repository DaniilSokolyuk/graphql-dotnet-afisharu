using Afisha.Graphql.Infrastructure.DataLoader.GreenDonutDataLoader;
using Microsoft.Extensions.DependencyInjection;

namespace Afisha.Graphql.Infrastructure.DataLoader
{
    public static class DataLoaderServiceCollectionExtensions
    {
        public static IServiceCollection AddDataLoaderRegistry(
            this IServiceCollection services)
        {
            return services
                .AddScoped<IDataLoaderRegistry, DataLoaderRegistry>()
                .AddScoped<IBatchOperation>(sp =>
                {
                    var batchOperation = new DataLoaderBatchOperation();

                    foreach (IDataLoaderRegistry registry in sp.GetServices<IDataLoaderRegistry>())
                    {
                        registry.Subscribe(batchOperation);
                    }

                    return batchOperation;
                });
        }
    }
}
