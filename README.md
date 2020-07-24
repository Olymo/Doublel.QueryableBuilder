![Build](https://github.com/Olymo/Doublel.QueryableBuilder/workflows/Build/badge.svg) ![](https://img.shields.io/nuget/dt/Doublel.QueryableBuilder?color=gree&label=Downloads)
## Queryable Builder for .NET

The idea behind quryable builder is very simple - it should reduce the amount of boilerplate query code in your codebase. It is designed around the idea of [query object](https://martinfowler.com/eaaCatalog/queryObject.html) and tries to further automate the process. You should simply design a class that will serve as a **query object**, decorate it with some of the predifined attributes, and, if needed, inherit from our base clasess and let the _**builder**_ to the heavy lifting of query composition. 

In it's essence, it is a collection of extension methods for predicate building, sorting and paging - all the ever present boring things we have to write. Based on your use-case, there are several options to choose from. 

### Download & Install

**Nuget Package [Doublel.QueryableBuilder](https://www.nuget.org/packages/Doublel.QueryableBuilder/)**

```powershell
Install-Package Doublel.QueryableBuilder
```
OR
```powershell
dotnet add package Doublel.QueryableBuilder
```
Minimum Requirements: **.NET Standard 2.0**

The tool was initialy designed for company's internal purposes, but we've figured out it could help someone else as well. If the example clicks for you, proceed to [building some queryes](https://github.com/Olymo/Doublel.QueryableBuilder/wiki/Building-Queries).
### Quick example
Let's say you have a user storage you would like to query and it's exposed via the IQueryable<TestUser> interface.

First, you have your user class:
```cs
public class TestUser
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string Username { get; set; }
    public int Age { get; set; }
}
```

Then, you should define your **_query object_** class:
```cs
public class UserQuery 
{
    [QueryProperty]
    public int? Age { get; set; }
    [QueryProperties(ComparisonOperator.Contains, "FirstName", "Username")]
    public string Keyword { get; set; }
}
```

And finally, you query your datasource:
```cs
var queryObject = new UserQuery 
{ 
    Age = 20; 
    Keyword = "ma"
}
var users = new List<User> { /* a bunch of users */}.AsQueryable();

var result = users.BuildQuery(queryObject);
```
The resulting dataset will consist of all the users having being exactly 20 years old and containing string "ma" (case-insensitive) in either their Username of FirstName properties.
