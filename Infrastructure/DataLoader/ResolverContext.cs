using System;
using System.Threading;
using GraphQL;

namespace Afisha.Graphql.Infrastructure.DataLoader
{
    public class ResolverContext : IResolverContext
    {
        private readonly IDependencyResolver _dependencyResolver;

        public CancellationToken RequestAborted { get; }

        public ResolverContext(IDependencyResolver dependencyResolver, CancellationToken requestAborted)
        {
            _dependencyResolver = dependencyResolver;
            RequestAborted = requestAborted;
        }

        public T Service<T>() => _dependencyResolver.Resolve<T>();

        public object Service(Type service) => _dependencyResolver.Resolve(service);
    }
}
