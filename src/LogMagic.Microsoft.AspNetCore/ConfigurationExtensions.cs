using LogMagic;
using LogMagic.Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LogMagic
{
   /// <summary>
   /// Configuration extensions
   /// </summary>
   public static class ConfigurationExtensions
   {
      /// <summary>
      /// Enable LogMagic automatic tracing for ASP.NET Core.
      /// Important!!! Make sure to call this after installation of other pieces of middleware handling exceptions (like UseDeveloperExceptionPage and UseExceptionHandler), but before any calls to UseStaticFiles, UseMvc and similar.
      /// </summary>
      public static IApplicationBuilder UseLogMagic(this IApplicationBuilder app,
         string applicationName,
         string applicationInstanceName)
      {
         L.Config
            .EnrichWith.Constant(KnownProperty.RoleName, applicationName)
            .EnrichWith.Constant(KnownProperty.RoleInstance, applicationInstanceName);

         return app.UseMiddleware<LogMagicMiddleware>();
      }

      /// <summary>
      /// Forwards asp.net core logging to LogMagic
      /// </summary>
      /// <param name="builder"></param>
      /// <returns></returns>
      public static ILoggingBuilder AddLogMagic(this ILoggingBuilder builder)
      {
         builder.Services.AddSingleton<ILoggerProvider, LogMagicLoggerProvider>();

         return builder;
      }
   }
}