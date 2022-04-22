using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WpfApp1
{
    public class Service
    {

        public static string APIKEY = "YWYJ4HYBVI4URNDF";

        
        public APIAnswer PullData(string function, DigitalCurrency digitalCurrency, PhysicalCurrency physicalCurrency, string interval, string outputsize)
        {
            string QUERY_URL = GenerateURL(function, digitalCurrency.CurrencyCode, physicalCurrency.CurrencyCode, interval, outputsize);
            Dictionary<string, object> parameters = CallAPI(QUERY_URL);
            // provera da li je doslo do greske
            bool greska = ErrorHappened(parameters);
            if (greska)
                throw new ErrorFromAPI();
            APIAnswer answer = ParseAnswer(function, parameters);
            return answer;
        }

        public Dictionary<string, object> CallAPI(string url)
        {
            Uri queryUri = new Uri(url);

            dynamic json_data;
            using (WebClient client = new WebClient())
            {
                //json_data = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(client.DownloadString(queryUri));
                string data = client.DownloadString(queryUri);
                json_data = deserializeToDictionary(data);
            }
            return json_data;
        }

        private Dictionary<string, object> deserializeToDictionary(string jo)
        {
            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(jo);
            var values2 = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> d in values)
            {
                // if (d.Value.GetType().FullName.Contains("Newtonsoft.Json.Linq.JObject"))
                if (d.Value is JObject)
                {
                    values2.Add(d.Key, deserializeToDictionary(d.Value.ToString()));
                }
                else
                {
                    values2.Add(d.Key, d.Value);
                }
            }
            return values2;
        }

        public string GenerateURL(string function, string symbol, string market, string interval, string outputsize)
        {
            string queryUri = "https://www.alphavantage.co/query?";
            queryUri += "function=" + function;
            queryUri += "&";
            queryUri += "symbol=" + symbol;
            queryUri += "&";
            queryUri += "market=" + market;

            if(function == "CRYPTO_INTRADAY")
            {
                queryUri += "&";
                queryUri += "interval=" + interval;
                if(outputsize != null && outputsize != "")
                {
                    queryUri += "&";
                    queryUri += "outputsize=" + outputsize;
                }
            }

            queryUri += "&";
            queryUri += "apikey=" + APIKEY;

            return queryUri;
        }


        public APIAnswer ParseAnswer(string function, Dictionary<string, object> json_data)
        {
            APIAnswer answer = new APIAnswer();

            foreach (object obj in json_data)
            {
                KeyValuePair<string, object> pair = (KeyValuePair<string, object>)obj;
                //Console.WriteLine(pair.Key);
                Dictionary<string, object> value = (Dictionary<string, object>)pair.Value;

                if(pair.Key == "Meta Data")
                {
                    answer.metaData = ProcessMetaData(value);
                }
                else if(pair.Key.Contains("Time Series"))
                {
                    answer.marketCapInfos = ProcessTimeSeries(value, function);
                }
                
            }
            return answer;
        }



        public MetaData ProcessMetaData(Dictionary<string, object> values)
        {
            MetaData metaData = new MetaData();
            foreach (KeyValuePair<string, object> a in values)
            {
                string key = a.Key;
                string value = (string)a.Value;

                if (key.Contains("Information"))
                    metaData.Information = value;
                else if (key.Contains("Digital Currency Code"))
                    metaData.DigitalCurrencyCode = value;
                else if (key.Contains("Digital Currency Name"))
                    metaData.DigitalCurrencyName = value;
                else if (key.Contains("Market Code"))
                    metaData.MarketCode = value;
                else if (key.Contains("Market Name"))
                    metaData.MarketName = value;
                else if (key.Contains("Last Refreshed"))
                {
                    DateTime dateTime = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    metaData.LastRefreshed = dateTime;
                }
                else if (key.Contains("Time Zone"))
                    metaData.TimeZone = value;

                else if (key.Contains("Interval"))
                    metaData.Interval = value;
                else if (key.Contains("Output Size"))
                    metaData.OutputSize = value;

                //Console.WriteLine(a.Key);
                //Console.WriteLine((string)a.Value);
            }
            return metaData;
        }
        



        public List<MarketCapInfo> ProcessTimeSeries(Dictionary<string, object> value, string function)
        {
            if (function == "CRYPTO_INTRADAY") 
                return ProcessTimeSeriesIntraday(value);
            else
                return ProcessTimeSeriesDailyWeeklyMonthly(value);

        }

        public List<MarketCapInfo> ProcessTimeSeriesIntraday(Dictionary<string, object> values)
        {
            List<MarketCapInfo> list = new List<MarketCapInfo>();
            foreach (KeyValuePair<string, object> a in values)
            {
                string datum = a.Key;
                Dictionary<string, object> value_list = (Dictionary<string, object>)a.Value;

                MarketCapInfo marketCapInfo = new MarketCapInfo();
                string format = "yyyy-MM-dd HH:mm:ss";
                DateTime dateTime = DateTime.ParseExact(datum, format, CultureInfo.InvariantCulture);
                marketCapInfo.Timestamp = dateTime;

                foreach (KeyValuePair<string, object> kvp in value_list)
                {
                    string key = kvp.Key;
                    
                    if (key.Contains("open"))
                    {
                        string value = (string)kvp.Value;
                        marketCapInfo.OpenFirstCurrency = Double.Parse(value);
                    }
                    else if (key.Contains("high"))
                    {
                        string value = (string)kvp.Value;
                        marketCapInfo.HighFirstCurrency = Double.Parse(value);
                    }
                    else if (key.Contains("low"))
                    {
                        string value = (string)kvp.Value;
                        marketCapInfo.LowFirstCurrency = Double.Parse(value);
                    }
                    else if (key.Contains("close"))
                    {
                        string value = (string)kvp.Value;
                        marketCapInfo.CloseFirstCurrency = Double.Parse(value);
                    }
                    else if (key.Contains("volume"))
                    {
                        double value = Convert.ToDouble(kvp.Value);
                        marketCapInfo.Volume = value;
                    }
                }
                list.Add(marketCapInfo);
            }
            return list;
        }

        public List<MarketCapInfo> ProcessTimeSeriesDailyWeeklyMonthly(Dictionary<string, object> values)
        {
            List<MarketCapInfo> list = new List<MarketCapInfo>();
            foreach (KeyValuePair<string, object> a in values)
            {
                string datum = a.Key;
                Dictionary<string, object> value_list = (Dictionary<string, object>)a.Value;

                MarketCapInfo marketCapInfo = new MarketCapInfo();
                string format = "yyyy-MM-dd";
                DateTime dateTime = DateTime.ParseExact(datum, format, CultureInfo.InvariantCulture);
                marketCapInfo.Timestamp = dateTime;

                foreach (KeyValuePair<string, object> kvp in value_list)
                {
                    string key = kvp.Key;
                    string value = (string)kvp.Value;
                    if (key.Contains("1a"))
                        marketCapInfo.OpenFirstCurrency = Double.Parse(value);
                    else if (key.Contains("1b"))
                        marketCapInfo.OpenUSDCurrency = Double.Parse(value);
                    else if (key.Contains("2a"))
                        marketCapInfo.HighFirstCurrency = Double.Parse(value);
                    else if (key.Contains("2b"))
                        marketCapInfo.HighUSDCurrency = Double.Parse(value);
                    else if (key.Contains("3a"))
                        marketCapInfo.LowFirstCurrency = Double.Parse(value);
                    else if (key.Contains("3b"))
                        marketCapInfo.LowUSDCurrency = Double.Parse(value);
                    else if (key.Contains("4a"))
                        marketCapInfo.CloseFirstCurrency = Double.Parse(value);
                    else if (key.Contains("4b"))
                        marketCapInfo.CloseUSDCurrency = Double.Parse(value);
                    else if (key.Contains("5"))
                        marketCapInfo.Volume = Double.Parse(value);
                    else if (key.Contains("6"))
                        marketCapInfo.MarketCap = Double.Parse(value);
                }
                list.Add(marketCapInfo);                
            }
            return list;
        }

        public bool ErrorHappened(Dictionary<string, object> response)
        {
            foreach(var element in response)
            {
                if (element.Key == "Error Message")
                    return true;
            }
            return false;
        }

    }
}
