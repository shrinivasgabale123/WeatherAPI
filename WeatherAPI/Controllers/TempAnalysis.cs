using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeatherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TempAnalysis : ControllerBase
    {
        private IConfiguration Configuration;

        public TempAnalysis(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        [HttpPost]
        [Route("GetHottestCity")]
        public string GetHottestCity([FromBody] string value)
        {
            try
            {
                string APIKey = this.Configuration.GetSection("APIKey").Value;

                Dictionary<string, double> dict = new Dictionary<string, double>();
                string[] cityarr = value.Split(',');
                HttpResponseMessage response = new HttpResponseMessage();
                int i = 0;
                while (i <= cityarr.Length-1)
                {
                    var urlloc = "http://api.openweathermap.org/geo/1.0/direct?q=" + cityarr[i] + APIKey;

                    HttpClient client = new HttpClient();
                    response = client.GetAsync(urlloc).Result;

                    string obj = response.Content.ReadAsStringAsync().Result;

                    dynamic data = JObject.Parse(obj.TrimEnd(']').TrimStart('['));

                    string lat = data.lat;
                    string lon = data.lon;
                    var urltemp = "https://api.openweathermap.org/data/2.5/weather?lat=" + lat + "&lon=" + lon + APIKey;

                    HttpResponseMessage responsetemp = client.GetAsync(urltemp).Result;
                    string obj1 = responsetemp.Content.ReadAsStringAsync().Result;

                    dynamic data1 = JObject.Parse(obj1.TrimEnd(']').TrimStart('['));

                    dict.Add(cityarr[i].ToString(), Convert.ToDouble(data1.main.temp.ToString()));

                    i++;
                }

                var maxValue = dict.Values.Max();
                var maxKey = dict.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

                return "City Name : " + maxKey + " Temp : " + (maxValue - 273.15);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
