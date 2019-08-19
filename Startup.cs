using Afisha.Graphql.DataLoaders;
using Afisha.Graphql.Infrastructure;
using Afisha.Graphql.Infrastructure.DataLoader;
using Afisha.Graphql.Infrastructure.DataLoader.CustomStrategy;
using Afisha.Graphql.Infrastructure.FieldMiddleware;
using Afisha.Graphql.Infrastructure.UserContext;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Internal;
using GraphQL.Server.Ui.GraphiQL;
using GraphQL.Server.Ui.Playground;
using GraphQL.Server.Ui.Voyager;
using GraphQL.Types;
using GreenDonut;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Afisha.Graphql
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            //auto register graph type
            services.Scan(scan =>
                scan.FromApplicationDependencies()
                    .AddClasses(classes => classes.AssignableTo<IGraphType>())
                    .AsSelf()
                    .WithSingletonLifetime());

            //register all dataloaders
            services.Scan(scan =>
                scan.FromApplicationDependencies()
                    .AddClasses(classes => classes.AssignableTo<IDataLoader>().InNamespaceOf<CustomStringLoader>())
                    .AsSelf()
                    .WithTransientLifetime());

            services
                .AddGraphQL(options =>
                {
                    options.EnableMetrics = true;
                    options.ExposeExceptions = true;
                })
                .AddUserContextBuilder<UserContextBuilder>();

            //field resolver middleware support, go to GraphQlExecutorDecorator.cs
            services.Replace(new ServiceDescriptor(typeof(IGraphQLExecuter<>),
                typeof(GraphQlExecutorDecorator<>), ServiceLifetime.Transient));

            //GreenDonut from hotchocolate cool data loaders
            services.AddDataLoaderRegistry();
            services.Replace(new ServiceDescriptor(typeof(IDocumentExecuter),
                typeof(DocumentExecutorWithChangedStrategySelector), ServiceLifetime.Singleton));

            //TODO: wait https://github.com/dotnet/corefx/issues/39808 in preview9
            //services.Replace(new ServiceDescriptor(typeof(IDocumentWriter),
            //  typeof(Utf8DocumentWriter), ServiceLifetime.Singleton));

            services.AddSingleton<AfishaSchema>();
            services.AddSingleton<IDependencyResolver, AfishaDependencyResolver>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<GraphMiddleWare<AfishaSchema>>();

            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());
            app.UseGraphiQLServer(new GraphiQLOptions());
            app.UseGraphQLVoyager(new GraphQLVoyagerOptions());
        }
    }
}
