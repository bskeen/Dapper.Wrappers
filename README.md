# Dapper.Wrappers

Dapper.Wrappers is a project aimed to help developers that want to use Dapper .Net, but want some of the query building capabilities abstracted away so they don't need to deal with them.
Projects that use little to no dynamic querying (beyond adding different values in the `WHERE` or `ORDER BY` clauses, for example) are good fits for Dapper.Wrappers. If you need dynamic
querying or want to support things like OData, you are probably better off using something such as Entity Framework.

In other words, Dapper.Wrappers is intended for use as a compile-time query builder (with minimal customizations at runtime).

## Overview

Dapper.Wrappers has several components that are used in querying the database:

- `IQueryGenerator` - Classes that implement this interface are expected to generate the queries given the input parameters and update the context to allow them to be run.
- `IQueryContext` - The QueryContext stores all of the input queries, along with handlers that will process the results after the query is run.
- `IQueryResultsHandler` and `IQueryResultsProcessor` - A results handler reads the query results from the Dapper .Net `GridReader` produced by the context. An `IQueryResultsProcessor` then takes those results and does any additional work to get them in the correct form for the consumers of the results.

## Dapper.Wrappers Roadmap

So far, Dapper.Wrappers only consists of a collection of some of the base classes. Some things planned for the near future are:

1. Tests
2. Documentation
3. An additional library containing helpers for using Dapper.Wrappers with a REST API (i.e. containing parsing logic and formatters to get incoming requests into the correct format).
4. A dotnet CLI tool that can be used to generate `IQueryGenerator`s and `IQueryResultsProcessor`s using POCO objects.

If you would like to contribute to the project, please review the guidelines in the [`CONTRIBUTING.md`](CONTRIBUTING.md) file found at the root of the project.

## Questions or Comments About Dapper.Wrappers

If you need any help getting up and running with Dapper.Wrappers, please add an issue here on Github. That will help answers to be publicly visible so others attempting to use the library with the same questions can easily find them.

I will be adding some docs to the repository eventually. For now, I just wanted to get the code up so others could see it.