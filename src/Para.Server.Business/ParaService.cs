using System;
using System.Collections.Generic;
using System.Configuration;
using System.Timers;

using Para.Server.Business.Manager;
using Para.Server.Contract;
using Para.Server.Contract.Argument;
using Para.Server.Contract.Response;

namespace Para.Server.Business
{
    public class ParaService : IParaService
    {
        private readonly Timer _timer;
        private readonly IParaManager _paraManager;

        public ParaService(IParaManager paraManager)
        {
            _paraManager = paraManager;

            _timer = new Timer();
            _timer.Elapsed += TimerHandler;
            _timer.Interval = TimeSpan.FromHours(Convert.ToDouble(ConfigurationManager.AppSettings["TimerTick"])).TotalMilliseconds;
            _timer.Enabled = true;
        }

        public Response GetValue(GetValueArgument argument)
        {
            var response = _paraManager.GetValue(argument);
            return response;
        }

        public Response ConvertValue(ConvertValueArgument argument)
        {
            var response = _paraManager.ConvertValue(argument);
            return response;
        }

        public List<ExchangeRate> GetExchangeRate(string day)
        {
            var response = _paraManager.GetExchangeRate(day);
            return response;
        }

        public void SaveValue()
        {
            _paraManager.SaveValue();
        }

        public void StartTimer()
        {
            _timer.Start();
        }

        private void TimerHandler(object state, ElapsedEventArgs e)
        {
            Console.WriteLine("timer ticked");

            SaveValue();
        }
    }
}
