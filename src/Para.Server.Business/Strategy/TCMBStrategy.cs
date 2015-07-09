using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Net;
using System.Xml;

using NLog;

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
            var response = new Response();

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
            var logger = GetTCMBLogger();

            try
            {
                var xml = GetSourceXml();

                var day = Convert.ToDateTime(xml.SelectSingleNode("/Tarih_Date/@Tarih").Value, new CultureInfo("tr-TR")).ToString("yyyyMMdd");

                var allCurrenyInfos = xml.SelectNodes("/Tarih_Date/Currency");
                var count = allCurrenyInfos.Count;

                var valsBanknote = xml.SelectNodes("/Tarih_Date/Currency/BanknoteSelling");
                var valsBanknoteBuying = xml.SelectNodes("/Tarih_Date/Currency/BanknoteBuying");
                var valsForex = xml.SelectNodes("/Tarih_Date/Currency/ForexSelling");
                var valsForexBuying = xml.SelectNodes("/Tarih_Date/Currency/ForexBuying");

                var codes = xml.SelectNodes("/Tarih_Date/Currency/@Kod");

                for (var i = 0; i < count; i++)
                {
                    Currency currency;
                    var code = codes[i].InnerText;
                    if (!Enum.TryParse(code, out currency)) continue;

                    var banknote = Convert.ToDecimal(valsBanknote[i].InnerText, CultureInfo.InvariantCulture);
                    var banknoteBuying = Convert.ToDecimal(valsBanknoteBuying[i].InnerText, CultureInfo.InvariantCulture);
                    var forex = Convert.ToDecimal(valsForex[i].InnerText, CultureInfo.InvariantCulture);
                    var forexBuying = Convert.ToDecimal(valsForexBuying[i].InnerText, CultureInfo.InvariantCulture);

                    if (code == Currency.JPY.ToString())
                    {
                        banknote = Convert.ToDecimal((banknote / 100).ToString("##.####"));
                        banknoteBuying = Convert.ToDecimal((banknoteBuying / 100).ToString("##.####"));
                        forex = Convert.ToDecimal((forex / 100).ToString("##.####"));
                        forexBuying = Convert.ToDecimal((forexBuying / 100).ToString("##.####"));
                    }

                    SaveValueDbWork(day, code, banknote, CurrencyValueType.Banknote);
                    SaveValueDbWork(day, code, banknoteBuying, CurrencyValueType.BanknoteBuying);
                    SaveValueDbWork(day, code, forex, CurrencyValueType.Forex);
                    SaveValueDbWork(day, code, forexBuying, CurrencyValueType.ForexBuying);
                }

                logger.Info(string.Format("values saved for TCMB Strategy {0}", DateTime.Now.ToString("f")));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        private static void SaveValueDbWork(string day, string code, decimal value, CurrencyValueType valueType)
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
                                            
                                            INSERT INTO [CurrencyValue] ([Day],[Source],[Target],[ValueSource],[ValueType],[Value])
                                            VALUES (@day,@target,@source,@valueSource,@valueType,1/@value)
                                        END";

                    cmd.Parameters.AddWithValue("@day", day);
                    cmd.Parameters.AddWithValue("@source", Currency.TL.ToString());
                    cmd.Parameters.AddWithValue("@target", code);
                    cmd.Parameters.AddWithValue("@valueSource", CurrencyValueSource.TCMB.ToString());
                    cmd.Parameters.AddWithValue("@valueType", valueType.ToString());
                    cmd.Parameters.AddWithValue("@value", value);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private static decimal GetValueDbWork(string day, Currency source, Currency target, CurrencyValueType type)
        {
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