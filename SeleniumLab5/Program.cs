using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using CsvHelper;
using System.Globalization;
using System.IO;

namespace SeleniumLab5
{
    public class DateValue
    {
        public string DateTime { get; set; }
        public decimal Value { get; set; }
    }
    public class Valute
    {
        public string ID { get; set; }
        public string NumCode { get; set; }
        public string CharCode { get; set; }
        public int Nominal { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public decimal Previous { get; set; }
    }

    public class ExchangeRates
    {
        public DateTime Date { get; set; }
        public DateTime PreviousDate { get; set; }
        public string PreviousURL { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, Valute> Valute { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            List<DateValue> dateValues = new List<DateValue>();
            var chrome = new OpenQA.Selenium.Chrome.ChromeDriver();
            DateTime dateTime = new DateTime(2023, 11, 21);
            for (int i = 0; i < 10; i++)
            {
                chrome.Navigate().GoToUrl($"https://www.cbr-xml-daily.ru/archive/{dateTime.Year}/{dateTime.Month.ToString().PadLeft(2, '0')}/{dateTime.Day.ToString().PadLeft(2, '0')}/daily_json.js");
                IWebElement targetElement = new WebDriverWait(chrome, TimeSpan.FromSeconds(100))
                           .Until(d => d.FindElement(By.XPath("/html/body/pre")));

                string data = targetElement.Text;
                ExchangeRates exchangeRates = Newtonsoft.Json.JsonConvert.DeserializeObject<ExchangeRates>(data);

                if (exchangeRates.Valute != null)
                {
                    var USD = exchangeRates.Valute.FirstOrDefault(x => x.Key == "USD");
                    dateValues.Add(new DateValue { DateTime = dateTime.ToString("D"), Value = USD.Value.Value });
                }
                else
                {
                    dateValues.Add(new DateValue { DateTime = dateTime.ToString("D") + " no data this day", Value = 0 });
                }
                using (var writer = new StreamWriter(@"C:\Users\newac\OneDrive\Desktop\PAD - Programarea aplicațiilor distribuite\lab5\values.csv"))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(dateValues);
                }

                dateTime = dateTime - TimeSpan.FromDays(1);
            }
        }
    }
}