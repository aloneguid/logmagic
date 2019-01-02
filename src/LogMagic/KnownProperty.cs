using System;

namespace LogMagic
{
   /// <summary>
   /// Contains a curated list of known property names
   /// </summary>
   public static class KnownProperty
   {
      /// <summary>
      /// Standard property name to store error information
      /// </summary>
      public const string Error = "error";

      /// <summary>
      /// Application name
      /// </summary>
      public const string ApplicationName = "appName";

      /// <summary>
      /// Application version
      /// </summary>
      public const string Version = "version";

      /// <summary>
      /// In multiinstance scenarios this property identifies the application component.
      /// </summary>
      public const string RoleName = "roleName";

      /// <summary>
      /// In multiinstance scenarios this property identifies the application component physical instance identifier.
      /// </summary>
      public const string RoleInstance = "roleInstance";

      /// <summary>
      /// Node IP address
      /// </summary>
      public const string NodeIp = "nodeIp";

      /// <summary>
      /// Method name the code is executing in
      /// </summary>
      public const string MethodName = "methodName";

      /// <summary>
      /// ID of a thread the code is executing from
      /// </summary>
      public const string ThreadId = "threadId";

      /// <summary>
      /// Tracks distributed operation across the layers. OperationId is a constant between all the calls affected in a distributed operation.
      /// </summary>
      public const string OperationId = "operationId";

      /// <summary>
      /// If current activity is initiated by another one, indicates the parent activity ID.
      /// </summary>
      public const string ParentActivityId = "parentActivityId";

      /// <summary>
      /// Used to override the ID set by a telemetry call
      /// </summary>
      public const string ActivityId = "activityId";

      /// <summary>
      /// Operation duration, has to be <see cref="Int64"/>
      /// </summary>
      public const string Duration = "duration";

      /// <summary>
      /// Event name
      /// </summary>
      public const string EventName = "eventName";

      /// <summary>
      /// Request name
      /// </summary>
      public const string RequestName = "requestName";

      /// <summary>
      /// Absolute URI of a request
      /// </summary>
      public const string RequestUri = "requestUri";

      /// <summary>
      /// Response code, for instance for HTTP requests. Can be an arbitrary string, but for HTTP codes it is advised to be a number
      /// </summary>
      public const string ResponseCode = "resultCode";

      /// <summary>
      /// Metric name
      /// </summary>
      public const string MetricName = "metricName";

      /// <summary>
      /// Dependency name
      /// </summary>
      public const string DependencyName = "dependencyName";

      /// <summary>
      /// Dependency type
      /// </summary>
      public const string DependencyType = "dependencyType";

      /// <summary>
      /// Dependency command
      /// </summary>
      public const string DependencyCommand = "dependencyCommand";

      /// <summary>
      /// Dependency target
      /// </summary>
      public const string DependencyTarget = "dependencyTarget";

      /// <summary>
      /// Metric value
      /// </summary>
      public const string MetricValue = "metricValue";

      /// <summary>
      /// Used by some loggers to report cluster health.
      /// </summary>
      public const string ClusterHealthProperty = "clusterHealth";

      /// <summary>
      /// Logging severity, for values see <see cref="LogSeverity"/>
      /// </summary>
      public const string Severity = "severity";
   }
}
