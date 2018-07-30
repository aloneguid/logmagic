# LogMagic [![NuGet](https://img.shields.io/nuget/v/LogMagic.svg?maxAge=2592000?style=flat-square)](https://www.nuget.org/packages/LogMagic/)

![logmagic icon](http://i.isolineltd.com/nuget/logmagic.png)

## Why LogMagic

Like many other libraries for .NET, LogMagic provides diagnostic logging into files, the console, and *elsewhere*. It's probably the easiest framework to setup, has a clean API, extremely extensible.

LogMagic also supports a relatively new paradigm of *structured logging*.

## Index

- [Installation](#installation)
- [Setup](#setup)
- [Example Application](#example-application)
- [Configuration Basics](#configuration-basics)
- [Writing Log Events](#writing-log-events)
- [Known Writers and Enrichers](#known-writers-and-enrichers)
- [Async Helpers](#async-helpers)
- [Visual Studio Integration (snippets)](doc/vssnippets.md)

## Installation

### Installing from NuGet

The core logging package is [LogMagic](https://www.nuget.org/packages/LogMagic). Supported frameworks are:

* **.NET 4.5**
* **.NET Standard 1.6**. By supporting .NET Standard we ensure that LogMagic works on .NET Core, Android, iOS or anything else.

> Note to ASP.NET Core users: LogMagic does not integrate with ASP.NET Core logging system, it's a separate logging framework you can use in conjunction with or instead of it. Both system have their pros and cons.

```
PM> Install-Package LogMagic
```

## Setup

Types are in `LogMagic` namespace

```csharp
using LogMagic;
```

An `ILog` instance is the one used to log events and can be created in one of three ways by calling to global static `L` class.

Create using current class type name

```csharp
ILog _log = L.G();
```

Or by specifying type explicitly

```csharp
ILog _log = L.G<T>;			//using generics
ILog _log = L.G(typeof(T));	//passing type
```

Or by specifying name explicitly

```csharp
ILog _log = L.G("instance name");
```

By default LogMagic doesn't write events anywhere and you need to configure it:

```csharp
L.Config.WriteTo.Console();
```

This is typically done once at application startup.

## Example application

The complete example below shows logging in a simple console application, with events sent to the console as well as to file on disk.

1. **Create a new Console Application project**
2. **Install the core LogMagic package**

In Visual Studio, open Package Manager Console and type:

```powershell
Install-Package LogMagic
```

3. **Add the following code to `Program.cs`**

```csharp
using System;
using LogMagic;

namespace LogMagicExample
{
   public class Program
   {
      private readonly ILog _log = L.G();

      public static void Main(string[] args)
      {
         L.Config
            .WriteTo.Console()
            .EnrichWith.ThreadId();

         new Program().Run();

         Console.ReadLine();
      }

      private void Run()
      {
         _log.Trace("hello, LogMagic!");

         int a = 10, b = 0;

         try
         {
            _log.Trace("dividing {a} by {b}", a, b);
            Console.WriteLine(a / b);
         }
         catch(Exception ex)
         {
            _log.Trace("unexpected error", ex);
         }

         _log.Trace("attempting to divide by zero");
      }

   }
}
```

4. **Run the program**

## Logging Exceptions

LogMagic always check last parameter of `Trace()` arguments whether it's an exception class and eliminates from the argument list.


## Configuration Basics

LogMagic uses C# API to configure logging.

### Log writers

Log event writers generally record log events to some external representation, typically console, file, or external data store. LogMagic has a few built-in writers to get you started. More writers are redistributed via NuGet.

A curated list of available packages are listed below on this page.

Writers are configure using `WriteTo` congiguration object.

```csharp
L.Config.WriteTo.PoshConsole();
```

Multiple writers can be active at the same time.

```csharp
L.Config
	.WriteTo.PoshConsole()
	.WriteTo.Trace();
```

### Formatting

Text-based writers support message formatting. Whenever a `format` parameter appears in writer configuration you can specify your own one:

```csharp
L.Config
	.EnrichWith.ThreadId()
	.WriteTo.Console("{time:H:mm:ss,fff}|{threadId}{level,-7}|{source}|{message}{error}");
```

Enriched properties can also appear in the output. In the example above a `threadId` comes from thread ID ennricher.

#### Formatting syntax

Built-in property names:
- `{time}` - used to reference event time
- `{level}` - prints message severity
- `{source}` - logging source name
- `{message}` - log message
- `{error}` - error

All of the properties support standard .NET formatting used in `string.Format()`.

### Enrichers

Enrichers are simple components that add properties to a log event. This can be used for the purpose of attaching a thread id, machine IP address etc. for example.

```csharp
L.Config.EnrichWith.ThreadId();
```

### Context Information

Sometimes it's useful to add context information to a logging session during a call duration scope. LogMagic achieves it by dynamically adding and removing propeties from the ambient "execution context". For example, all messages during a transaction might carry the id of that transaction, and so on.

The feature does not need any special configuration, and properties can be added an removed using `L.Context()`:

```csharp
log.Trace("no properties");

using(L.Context("A", "id1"))
{
	log.Trace("carries property A=id1");

	using(L.Context("B", "id2"))
	{
		log.Trace("carries A=id1 and B=id2");
	}

	log.Trace("carries property A=id1");

	using(L.Context("A", "id3"))
	{
		log.Trace("carries property A=id3");
	}

	log.Trace("carries property A=id1");
}

log.Trace("no properties");
```

Pushing property onto the context will override any existing properties with the same name, until the object returned from `L.Context()` is disposed, as the property A in the example demonstrates.

`L.Context()` accepts multiple properties at once if you need to:

```csharp
using(L.Context("A", "id1",  "B", "id2"))
{
	//...
}
```

Logging context is used extensively by LogMagic plugins to carry execution context information across the application. For instance, web applications need to known which request the code is related to etc.

**Important:** properties must be popped from the context in the precise order in which they were added. Behavior otherwise is undefined.

You can also get context property value by name at any time in any place in your code by calling to `L.GetContextValue(propertyName)` which returns null if property doesn't exist.

**Important:** Log context is not available when you target for .NET Framework 4.5 due to the reason that this version doesn't have any options to track execution. If you are still using .NET 4.5 or earlier it's time to upgrade to .NET 4.6.

## Writing log events

Log events are written to writers using the `ILog` interface. Typically you will instantiate it on top of your class definition you want to log from. 

```csharp
private readonly ILog _log = L.G<Class>();

// ...

_log.Trace("application v{version} started on {date}", "1.0", DateTime.UtcNow);
```

### Message template syntax

the string above `"application v{version} started on {date}"` is a *message template*. Message templates are superset of standard .NET format string, so any format string acceptable to `string.Format()` will also be correctly processed by LogMagic.

- Property names are written between `{` and `}` brackets
- Formats that use numeric property names, like `{0}` and `{1}` exclusively, will be matched with the log method's parameters by treating the property names as indexes; this is identical to `string.Format()`'s behavior
- If any of the property names are non-numeric, then all property names will be matched from left-to-right with the log method's parameters
- Property names, both numeric and named, may be suffixed with an optional format, e.g. `:000` to control how the property is rendered; these format strings behave exactly as their counterparts within the `string.Format()` syntax

### Log event levels

LogMagic doesn't have the classic logging levels (i.e. debug, info, warn etc.) as this is proven to be rarely used. Instead you only need one single `Trace()` method. Due to the fact that structured logging is supported and promoted there is no need to have logging levels as you can always filter based on a custom property if you ever need to.

## Known Writers and Enrichers

|Name|Description|
|----|-----------|
|[System Console](doc/impl/system-console.md)|Simplest logger that outputs events to the system console.|
|[Posh Console](doc/impl/posh-console.md)|Simplest logger that outputs events to the system console, but also supports colorisation.|
|[System Trace](doc/impl/system-trace.md)|Writes to the system trace.|
|[File on disk](doc/impl/disk-file.md)|A really simple writer that outputs to a file on disk.|
|[Azure Application Insights](doc/impl/azure-appinsights.md)|Emits telemetry into [Azure Application Insights](https://azure.microsoft.com/en-us/services/application-insights/).|
|[Azure Service Fabric](doc/impl/azure-servicefabric.md)|Integrates with [Azure Service Fabric](https://azure.microsoft.com/en-us/services/service-fabric/) by building correlating proxies, enrichers, and emitting cluster health events|
|[ASP.NET Core](doc/impl/aspnetcore.md)|Provides a custom middleware that automatically logs requests.|


### Built-in enrichers

| Writer Syntax | Meaning        |
|---------------|----------------|
|  `.Constant()` | adds a constant value to every log event |
| `.MachineIp()` | current machine IP address |
| `.MachineName()` | current machine DNS name |
| `.MethodName()` | caller's method name |
| `.ThreadId()` | managed thread id |

Note that external packages may add more enrichers which are not listed here but documented on specific package page.

## Async Helpers

Often tracking dependencies involves measuring time and catching exception whcih is a bit tedious.
