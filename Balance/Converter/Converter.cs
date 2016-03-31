using Converter.XmlElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Converter
{
    public static class CurrencyType
    {
        public readonly static string USD = "USD";
        public readonly static string EUR = "EUR";
        public readonly static string RUB = "RUB";
        public readonly static string GBP = "GBP";
        public readonly static string CNY = "CNY";
        public readonly static string JPY = "JPY";
        public readonly static string PLN = "PLN";
    }
    public static class Converter
    {
        public static async Task<decimal>  Convert(string currencyFrom, string currencyTo, decimal value)
        {
            HttpWebRequest http = (HttpWebRequest)WebRequest.Create("http://www.nbrb.by/Services/XmlExRates.aspx");
            WebResponse response = await http.GetResponseAsync();
            StreamReader sr = new StreamReader(response.GetResponseStream());
            string content = sr.ReadToEnd();
            var serializer = new XmlSerializer(typeof(DailyExRates));
            var document = XDocument.Parse(content);
            var reader = document.CreateReader();
            var obj = serializer.Deserialize(reader);
            var dailyExRates = (DailyExRates)obj;
            var list = dailyExRates.Currencies;
            return (value * list.FirstOrDefault(l => l.CharCode == currencyFrom).Rate) / list.FirstOrDefault(l => l.CharCode == currencyTo).Rate;
        }
    }
}
