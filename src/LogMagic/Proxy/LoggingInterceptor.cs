using System;
using Castle.DynamicProxy;

namespace LogMagic.Proxy
{
   class LoggingInterceptor : IInterceptor
   {
      private readonly ILog _log;
      private readonly bool _logExceptions;
      private readonly OnBeforeMethodExecution _onBeforeExecution;
      private readonly OnAfterMethodExecution _onAfterExecution;

      public LoggingInterceptor(ILog log,
         bool logExceptions,
         OnBeforeMethodExecution onBeforeExecution,
         OnAfterMethodExecution onAfterExecution)
      {
         _log = log;
         _logExceptions = logExceptions;
         _onBeforeExecution = onBeforeExecution;
         _onAfterExecution = onAfterExecution;
      }

      public void Intercept(IInvocation invocation)
      {
         TimeMeasure measure = (_onBeforeExecution != null || _onAfterExecution != null)
            ? new TimeMeasure()
            : null;
         Exception error = null;

         _onBeforeExecution?.Invoke(_log, invocation.Method, invocation.Arguments);

         try
         {
            invocation.Proceed();
         }
         catch(Exception ex)
         {
            if(_logExceptions)
            {
               _log.Write("fatal error executing '{0}'", invocation.Method.Name, ex);
            }

            error = ex;

            throw;
         }
         finally
         {
            if(_onAfterExecution != null)
            {
               long ticks = measure.ElapsedTicks;

               _onAfterExecution(_log, invocation.Method, invocation.Arguments, invocation.ReturnValue, ticks, error);
            }

            if(measure != null)
            {
               measure.Dispose();
            }
         }
      }
   }
}
