using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace LogMagic.Enrichers
{
   class MethodNameEnricher : IEnricher
   {
      //the rest of the methods up the chain should be marked too so we can get method name in release version
      [MethodImpl(MethodImplOptions.NoInlining)]
      public void Enrich(LogEvent e, out string propertyName, out object propertyValue)
      {
         var frame = new StackFrame(4);   //warning! this can change after refactoring

         propertyName = KnownProperty.MethodName;

         MethodBase method = frame.GetMethod();
         var sb = new StringBuilder();

         sb.Append(method.DeclaringType.FullName);
         sb.Append(".");
         sb.Append(method.Name);
         sb.Append("(");
         bool isFirst = true;
         foreach(ParameterInfo p in method.GetParameters())
         {
            if (!isFirst)
            {
               sb.Append(", ");
            }
            else
            {
               isFirst = false;
            }
            sb.Append(p.ParameterType.Name);
            sb.Append(" ");
            sb.Append(p.Name);
         }
         sb.Append(")");

         propertyValue = sb.ToString();
      }
   }
}