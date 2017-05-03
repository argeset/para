using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Xml;
using NLog;
using Para.Server.Contract.Argument;
using Para.Server.Contract.Enum;
using Para.Server.Contract.Response;

namespace Para.Server.Business.Strategy
{
    public class TCMBStrategy : BaseStrategy
    {
        public override Response GetValue(string day, Currency source, Currency target, CurrencyValueType type)
        {
            var response = new Response();

            try
            {
                response.Value = GetValueDbWork(day, source, target, type);
            }
            catch (Exception ex)
            {
                response.Message = ResponseMessage.SystemError;

                var logger = GetTCMBLogger();
                logger.Error(ex);
            }

            return response;
        }

        public override Response ConvertValue(string day, Currency source, Currency target, CurrencyValueType type, decimal? amount)
        {

            GetTCMBLogger().Info(string.Format("{0}-{1}-{2}-{3}-{4}", day, source, target, type, amount));

            var response = new Response();

            if (source == target)
            {
                response.Value = amount ?? 0;
                return response;
            }

            try
            {
                var value = GetValueDbWork(day, source, target, type);
                response.Value = value * (amount ?? 0);
            }
            catch (Exception ex)
            {
                response.Message = ResponseMessage.SystemError;

                var logger = GetTCMBLogger();
                logger.Error(ex);
            }

            return response;
        }

        public override void SaveValue()
        {
            var values = GetCurrencyValues();
            var dic = PrepareCrossRate(values);

            foreach (var currencyValue in dic)
            {
                SaveValueDbWork(currencyValue.Value);
            }
        }

        private static Dictionary<string, CurrencyValue> PrepareCrossRate(List<CurrencyValue> values)
        {
            var crossRate = new Dictionary<string, CurrencyValue>();

            foreach (var currencyValue in values)
            {
                foreach (var value in values.Where(value => currencyValue.ValueType == value.ValueType))
                {
                    var key = string.Format("{0}-{1}-{2}", currencyValue.Source, value.Source, value.ValueType);

                    crossRate[key] = new CurrencyValue
                    {
                        Day = value.Day,
                        Source = currencyValue.Source,
                        Target = value.Source,
                        ValueSoruce = value.ValueSoruce,
                        ValueType = value.ValueType,
                        Value = Convert.ToDecimal((currencyValue.Value / value.Value).ToString("##.####"))
                    };
                }
            }
            return crossRate;
        }

        private static List<CurrencyValue> GetCurrencyValues()
        {
            var logger = GetTCMBLogger();
            var currencyValues = new List<CurrencyValue>();

            try
            {
                var xml = GetSourceXml();

                var day = Convert.ToDateTime(xml.SelectSingleNode("/Tarih_Date/@Tarih").Value, new CultureInfo("tr-TR")).ToString("yyyyMMdd");

                var allCurrenyInfos = xml.SelectNodes("/Tarih_Date/Currency");
                var count = allCurrenyInfos.Count;

                var valsUnit = xml.SelectNodes("/Tarih_Date/Currency/Unit");

                var valsBanknote = xml.SelectNodes("/Tarih_Date/Currency/BanknoteSelling");
                var valsBanknoteBuying = xml.SelectNodes("/Tarih_Date/Currency/BanknoteBuying");
                var valsForex = xml.SelectNodes("/Tarih_Date/Currency/ForexSelling");
                var valsForexBuying = xml.SelectNodes("/Tarih_Date/Currency/ForexBuying");

                var codes = xml.SelectNodes("/Tarih_Date/Currency/@Kod");

                PrepareCurrencyValue(currencyValues, day);

                for (var i = 0; i < count; i++)
                {
                    Currency currency;
                    var code = codes[i].InnerText;
                    if (!Enum.TryParse(code, out currency)) continue;

                    currencyValues.Add(new CurrencyValue
                    {
                        Day = day,
                        Source = currency,
                        Target = Currency.TRY,
                        ValueSoruce = CurrencyValueSource.TCMB,
                        ValueType = CurrencyValueType.Banknote,
                        Value = GetValueFormated(valsBanknote[i].InnerText, valsUnit[i].InnerText)
                    });


                    currencyValues.Add(new CurrencyValue
                    {
                        Day = day,
                        Source = currency,
                        Target = Currency.TRY,
                        ValueSoruce = CurrencyValueSource.TCMB,
                        ValueType = CurrencyValueType.BanknoteBuying,
                        Value = GetValueFormated(valsBanknoteBuying[i].InnerText, valsUnit[i].InnerText)
                    });

                    currencyValues.Add(new CurrencyValue
                    {
                        Day = day,
                        Source = currency,
                        Target = Currency.TRY,
                        ValueSoruce = CurrencyValueSource.TCMB,
                        ValueType = CurrencyValueType.Forex,
                        Value = GetValueFormated(valsForex[i].InnerText, valsUnit[i].InnerText)
                    });

                    currencyValues.Add(new CurrencyValue
                    {
                        Day = day,
                        Source = currency,
                        Target = Currency.TRY,
                        ValueSoruce = CurrencyValueSource.TCMB,
                        ValueType = CurrencyValueType.ForexBuying,
                        Value = GetValueFormated(valsForexBuying[i].InnerText, valsUnit[i].InnerText)
                    });

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            return currencyValues;
        }

        private static void PrepareCurrencyValue(List<CurrencyValue> currencyValues, string day)
        {
            currencyValues.Add(new CurrencyValue
            {
                Day = day,
                Source = Currency.TRY,
                Target = Currency.TRY,
                ValueSoruce = CurrencyValueSource.TCMB,
                ValueType = CurrencyValueType.Banknote,
                Value = 1
            });

            currencyValues.Add(new CurrencyValue
            {
                Day = day,
                Source = Currency.TRY,
                Target = Currency.TRY,
                ValueSoruce = CurrencyValueSource.TCMB,
                ValueType = CurrencyValueType.BanknoteBuying,
                Value = 1
            });

            currencyValues.Add(new CurrencyValue
            {
                Day = day,
                Source = Currency.TRY,
                Target = Currency.TRY,
                ValueSoruce = CurrencyValueSource.TCMB,
                ValueType = CurrencyValueType.Forex,
                Value = 1
            });

            currencyValues.Add(new CurrencyValue
            {
                Day = day,
                Source = Currency.TRY,
                Target = Currency.TRY,
                ValueSoruce = CurrencyValueSource.TCMB,
                ValueType = CurrencyValueType.ForexBuying,
                Value = 1
            });
        }

        private static decimal GetValueFormated(string value, string unit)
        {
            return Convert.ToDecimal((Convert.ToDecimal(value, CultureInfo.InvariantCulture) / Convert.ToDecimal(unit, CultureInfo.InvariantCulture)).ToString("##.########"));
        }

        private static void SaveValueDbWork(CurrencyValue value)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Para"].ConnectionString))
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"IF NOT EXISTS (SELECT * 
			                                           FROM [CurrencyValue] 
                                                       WHERE [Day] = @day
					                                         AND [Source] = @source
					                                         AND [Target] = @target
					                                         AND [ValueSource] = @valueSource
					                                         AND [ValueType] = @valueType
					                                         AND [Value] = @value)
                                        BEGIN
	                                        INSERT INTO [CurrencyValue] ([Day],[Source],[Target],[ValueSource],[ValueType],[Value])
	                                        VALUES (@day,@source,@target,@valueSource,@valueType,@value)                                            
                                        END";

                    cmd.Parameters.AddWithValue("@day", value.Day);
                    cmd.Parameters.AddWithValue("@source", value.Source.ToString());
                    cmd.Parameters.AddWithValue("@target", value.Target.ToString());
                    cmd.Parameters.AddWithValue("@valueSource", value.ValueSoruce.ToString());
                    cmd.Parameters.AddWithValue("@valueType", value.ValueType.ToString());
                    cmd.Parameters.AddWithValue("@value", value.Value);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static decimal GetValueDbWork(string day, Currency source, Currency target, CurrencyValueType type)
        {
            GetTCMBLogger().Info(string.Format("{0}-{1}-{2}-{3}", day, source, target, type));
            if (source == target) return 1;

            object result;
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Para"].ConnectionString))
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"IF NOT EXISTS (SELECT Value
			                                           FROM [CurrencyValue] 
                                                       WHERE [Day] = @day
				 	                                         AND [Source] = @source
				 	                                         AND [Target] = @target
				 	                                         AND [ValueSource] = @valueSource
				  	                                         AND [ValueType] = @valueType)
	                                        BEGIN
		                                        SELECT TOP 1 Value
		                                        FROM [CurrencyValue] 
		                                        WHERE [Source] = @source
				                                        AND [Target] = @target
				                                        AND [ValueSource] = @valueSource
				                                        AND [ValueType] = @valueType
		                                        ORDER BY [Day] DESC
	                                        END
                                        ELSE
	                                        BEGIN
		                                        SELECT Value
		                                        FROM [CurrencyValue] 
		                                        WHERE [Day] = @day
				                                        AND [Source] = @source
				                                        AND [Target] = @target
				                                        AND [ValueSource] = @valueSource
				                                        AND [ValueType] = @valueType
	                                        END";

                    cmd.Parameters.AddWithValue("@day", day);
                    cmd.Parameters.AddWithValue("@source", source.ToString());
                    cmd.Parameters.AddWithValue("@target", target.ToString());
                    cmd.Parameters.AddWithValue("@valueSource", CurrencyValueSource.TCMB.ToString());
                    cmd.Parameters.AddWithValue("@valueType", type.ToString());

                    conn.Open();
                    result = cmd.ExecuteScalar();
                }
            }

            return Convert.ToDecimal(result);
        }

        private static XmlDocument GetSourceXml()
        {
            var client = new WebClient();
            var data = client.DownloadString("http://www.tcmb.gov.tr/kurlar/today.xml");
            var xml = new XmlDocument();
            xml.LoadXml(data);
            return xml;
        }

        private static Logger GetTCMBLogger()
        {
            var logger = LogManager.GetLogger("TCMBStrategy");
            return logger;
        }
    }
}