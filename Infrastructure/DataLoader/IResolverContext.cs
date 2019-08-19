using System;
using System.Threading;

namespace Afisha.Graphql.Infrastructure.DataLoader
{
    public interface IResolverContext
    {
        /// <summary>
        /// Gets as specific service from the dependency injection container.
        /// </summary>
        /// <typeparam name="T">
        /// The service type.
        /// </typeparam>
        /// <returns>
        /// Returns the specified service.
        /// </returns>
        T Service<T>();

        /// <summary>
        /// Gets as specific service from the dependency injection container.
        /// </summary>
        /// <param name="service">The service type.</param>
        /// <returns>
        /// Returns the specified service.
        /// </returns>
        object Service(Type service);

        /// <summary>
        /// Notifies when the connection underlying this request is aborted
        /// and thus request operations should be cancelled.
        /// </summary>
        CancellationToken RequestAborted { get; }
    }
}
