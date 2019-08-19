# graphql-dotnet-afisharu

# [GreenDonut](https://greendonut.io/) data loaders,  [documentation](https://hotchocolate.io/docs/dataloaders)

* multi await support, default data loaders [do not support this](https://github.com/graphql-dotnet/graphql-dotnet/issues/945)
* better api
* better support

## Usage
[Code example](https://github.com/DaniilSokolyuk/graphql-dotnet-afisharu/blob/master/Types/RootQuery.cs#L23)

Test query
```
query {
  test1: customLoaderTest(count:3)
  test2: customLoaderTest(count:7)
  test3: fetchLoaderTest(count: 2)
  test4: fetchLoaderTest(count: 4)
}
```
[CustomStringLoader](https://github.com/DaniilSokolyuk/graphql-dotnet-afisharu/blob/master/DataLoaders/CustomStringLoader.cs#L12) is called only once

[Adhoc batch data loader](https://github.com/DaniilSokolyuk/graphql-dotnet-afisharu/blob/master/DataLoaders/CustomStringLoader.cs#L12) is called only once


# [Field Middlewares](https://graphql-dotnet.github.io/docs/getting-started/field-middleware/)
Usage example in ASP.NET Core

[sample usage](https://github.com/DaniilSokolyuk/graphql-dotnet-afisharu/blob/master/Infrastructure/FieldMiddleware/GraphQlExecutorDecorator.cs#L31)

#  Other
