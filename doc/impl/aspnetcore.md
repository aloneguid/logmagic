# ASP.NET Core

**Package**: [LogMagic.Microsoft.AspNetCore](https://www.nuget.org/packages/LogMagic.Microsoft.AspNetCore/)

Provides integration with [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/) in a form of a custom middleware that can be injected into usual request pipeline.

## Installing

After referencing the NuGet package find your `Startup.cs` file and add LogMagic middleware:

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
   if (env.IsDevelopment())
   {
      app.UseDeveloperExceptionPage();
   }

   app.UseLogMagic();

   app.UseMvc();
}
```

> Make sure to call this method after installation of other pieces of middleware handling exceptions (like UseDeveloperExceptionPage and UseExceptionHandler), but before any calls to UseStaticFiles, UseMvc and similar.

## What does it do

LogMagic middleware doesn't do any kind of special logging, or in fact any logging at all. If you need to perform custom logging such as log all requests you could [write your own middleware](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/index?tabs=aspnetcore2x) which is a trivial thing to do, or contribute to this repository. This library doesn't include a standard middleware because every app's requirements are so different we can't possibly cover all of them.

Instead, the middleware sets the current operation context for logging, so that any kind of custom logging will be logged with the same operation ID property.

## Microsof Application Insights

This library will try to get operation ID from Microsoft Application Insights if you are using it, which is popular amongst asp.net developers. When available, LogMagic middleware will use AppInsights operation ID, otherwise a new one will be generated.
