using System.Collections.Generic;

namespace LogMagic.Enrichers
{
   class ConstantEnricher : IEnricher
   {
      private readonly string _propertyName;
      private readonly object _propertyValue;

      public ConstantEnricher(string propertyName, object propertyValue)
      {
         _propertyName = propertyName;
         _propertyValue = propertyValue;
      }

      public ConstantEnricher(KeyValuePair<string, object> constant) : this(constant.Key, constant.Value)
      {

      }

      public string Name => _propertyName;

      public object Value => _propertyValue;

      public void Enrich(LogEvent e, out string propertyName, out object propertyValue)
      {
         propertyName = _propertyName;
         propertyValue = _propertyValue;
      }
   }
}
