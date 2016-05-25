using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forecaster.Models;
using Forecaster.Resources;
using Forecaster.Utils;
using Newtonsoft.Json.Linq;

namespace Forecaster.Services
{
    /// <summary>
    ///     A client for working with OpenWeatherMap Rest API
    /// </summary>
    /// <remarks>Use Xamarin.Forms.Device class to retrieve a platform specific RestClient</remarks>
    public abstract class RestClient
    {
        protected readonly string AppId;
        protected readonly string BaseUrl;
        protected readonly string UnitsType;

        protected RestClient()
        {
            BaseUrl = RestApiConstants.BaseUri;
            AppId = RestApiConstants.AppId;
            UnitsType = "metric";
        }

        /// <summary>
        ///     Get all cities that match to the parameter.
        /// </summary>
        /// <param name="cityName">Queried city name</param>
        /// <returns></returns>
        public abstract Task<IEnumerable<City>> FindCitiesAsync(string cityName);

        /// <summary>
        ///     Load weather information about specific city.
        /// </summary>
        /// <param name="city">An instance of Forecaster.Models.City type</param>
        /// <returns>Full weather information for the city.</returns>
        public abstract Task<WeatherInfo> GetWeatherByCityAsync(City city);

        /// <summary>
        ///     Load weather icon by its name.
        /// </summary>
        /// <param name="iconId">Icon id from weather information.</param>
        /// <returns>System.IO.Stream instance with an image.</returns>
        public abstract Task<byte[]> LoadIconAsync(string iconId);

        /// <summary>
        ///     Extract the cities from json string.
        /// </summary>
        /// <param name="restResponse">Json string received by rest request.</param>
        /// <returns></returns>
        protected IEnumerable<City> ExtractCities(string restResponse)
        {
            JObject jObject = JObject.Parse(restResponse, new JsonLoadSettings
            {CommentHandling = CommentHandling.Ignore, LineInfoHandling = LineInfoHandling.Ignore});
            return jObject["list"].Children()
                .Select(token => new City
                {
                    Name = token.Value<string>("name"),
                    Id = token.Value<int>("id"),
                    Country = token["sys"].Value<string>("country")
                }
                ).AsEnumerable();
        }

        /// <summary>
        ///     Parse weather info from string to object.
        /// </summary>
        /// <param name="restResponse">Json string</param>
        /// <returns>An instance of Forecaster.Models.WeatherInfo type </returns>
        protected WeatherInfo ExtractWeatherInfo(string restResponse)
        {
            JToken infoObject = JObject.Parse(restResponse);

            WeatherInfo info = new WeatherInfo
            {
                City = new City
                {
                    Country = infoObject["sys"].Value<string>("country"),
                    Id = infoObject.Value<int>("id"),
                    Name = infoObject.Value<string>("name")
                },
                GeoCoords = new GeoCoords
                {
                    Lon = infoObject["coord"].Value<double>("lon"),
                    Latt = infoObject["coord"].Value<double>("lat")
                },
                Weather = new Weather
                {
                    Description = infoObject["weather"][0].Value<string>("description"),
                    Id = infoObject["weather"][0].Value<int>("Id"),
                    Icon = infoObject["weather"][0].Value<string>("icon"),
                    Main = infoObject["weather"][0].Value<string>("main")
                },
                Wind = new Wind
                {
                    Deg = infoObject["wind"].Value<double>("deg"),
                    Speed = infoObject["wind"].Value<double>("speed")
                },
                Humidity = infoObject["main"].Value<int>("humidity"),
                Pressure = infoObject["main"].Value<int>("pressure"),
                Sunrise = DateTimeHelper.FromUnixTime(infoObject["sys"].Value<int>("sunrise")),
                Sunset = DateTimeHelper.FromUnixTime(infoObject["sys"].Value<int>("sunset")),
                Temperature = infoObject["main"].Value<double>("temp"),
                Time = DateTimeHelper.FromUnixTime(infoObject.Value<int>("dt"))
            };

            return info;
        }
    }

    /// <summary>
    ///     A mock type for testing purposes.
    /// </summary>
    public class RestClientMock : RestClient
    {
        public override Task<IEnumerable<City>> FindCitiesAsync(string cityName)
        {
            string response =
                @"{""message"":""accurate"",""cod"":""200"",""count"":1,""list"":[{""id"":706369,""name"":""Khmelnytskyi raion"",""coord"":{""lon"":26.9743,""lat"":49.4168},""main"":{""temp"":15.76,""temp_min"":15.76,""temp_max"":15.76,""pressure"":982.76,""sea_level"":1018.95,""grnd_level"":982.76,""humidity"":74},""dt"":1464116198,""wind"":{""speed"":4.36,""deg"":80.002},""sys"":{""country"":""Ukraine""},""rain"":{""3h"":0.5075},""clouds"":{""all"":92},""weather"":[{""id"":500,""main"":""Rain"",""description"":""light rain"",""icon"":""10n""}]}]}";
            return Task.FromResult(ExtractCities(response));
        }

        public override Task<WeatherInfo> GetWeatherByCityAsync(City city)
        {
            string response =
                @"{""coord"":{""lon"":27,""lat"":49.42},""weather"":[{""id"":804,""main"":""Clouds"",""description"":""overcast clouds"",""icon"":""04d""}],""base"":""cmc stations"",""main"":{""temp"":16.26,""pressure"":982.08,""humidity"":80,""temp_min"":16.26,""temp_max"":16.26,""sea_level"":1018.41,""grnd_level"":982.08},""wind"":{""speed"":4.11,""deg"":93.0033},""clouds"":{""all"":92},""dt"":1464172169,""sys"":{""message"":0.0025,""country"":""UA"",""sunrise"":1464142548,""sunset"":1464199377},""id"":706369,""name"":""Khmelnytskyy"",""cod"":200}";
            WeatherInfo info = ExtractWeatherInfo(response);

            return Task.FromResult(info);
        }

        public override Task<byte[]> LoadIconAsync(string iconId)
        {
            throw new NotImplementedException();
        }
    }
}