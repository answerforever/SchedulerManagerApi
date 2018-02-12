using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace YQNScheduler.DomainService
{
    public class SchedulerProviderException : Exception
    {
        public SchedulerProviderException(string message, NameValueCollection properties) : this(message, null, properties)
        {            
        }

        public SchedulerProviderException(string message, Exception innerException, NameValueCollection properties)
            : base(message, innerException)
        {
            SchedulerInitialProperties = properties;
        }

        public NameValueCollection SchedulerInitialProperties { get; private set; }
    }
}
