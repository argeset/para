using System;

using Para.Server.Business.Strategy;
using Para.Server.Contract.Argument;
using Para.Server.Contract.Response;

namespace Para.Server.Business.Manager
{
    public class ParaManager : IParaManager
    {
        public Response ConvertValue(ConvertValueArgument argument)
        {
            var strategy = BaseStrategy.Strategies[argument.ValueSource];
            var response = strategy.ConvertValue(argument.Time, argument.Source, argument.Target, argument.Type, argument.Amount);
            return response;
        }

        public Response GetValue(GetValueArgument argument)
        {
            var strategy = BaseStrategy.Strategies[argument.ValueSource];
            var response = strategy.GetValue(argument.Time, argument.Source, argument.Target, argument.Type);
            return response;
        }

        public void SaveValue()
        {
            foreach (var strategy in BaseStrategy.Strategies.Values)
            {
                strategy.SaveValue();
            }

            Console.WriteLine("values saved for all strategies > {0}", DateTime.Now.ToString("f"));
        }
    }
}