﻿using System;
using System.Linq;
using System.Text;

using Castle.Core.Logging;
using Castle.DynamicProxy;

namespace Para.Server.Host.Configuration.Helpers
{
    public class ExceptionInterceptor : IInterceptor
    {
        private ILogger _logger;
        public ILogger Logger
        {
            get
            {
                return _logger ?? (_logger = NullLogger.Instance);
            }
            set { _logger = value; }
        }

        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("{0} || {1}", ex.Message, CreateInvocationLogString(invocation)));
                if (ex.InnerException != null)
                {
                    Logger.Error(string.Format("{0}", ex.InnerException.Message));
                }
                throw;
            }
        }

        private string CreateInvocationLogString(IInvocation invocation)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Called: {0}.{1} (", invocation.TargetType.Name, invocation.Method.Name);
            foreach (var argument in invocation.Arguments)
            {
                var argumentDescription = argument == null ? "null" : GetPropertiesAndValues(argument);
                sb.Append(argumentDescription).Append("|");
            }
            if (invocation.Arguments.Any()) sb.Length--;
            sb.Append(")");
            return sb.ToString();
        }

        private string GetPropertiesAndValues(object argument)
        {
            var str = new StringBuilder();
            if (argument is string)
            {
                str.AppendFormat(" {0} ", argument);
            }
            else
            {
                var propertyInfos = argument.GetType().GetProperties();
                foreach (var propertyInfo in propertyInfos)
                {
                    try
                    {
                        str.AppendFormat(" {0} # {1} ", propertyInfo.Name, propertyInfo.GetValue(argument, null));
                    }
                    catch { }
                }
            }

            return str.ToString();
        }
    }
}