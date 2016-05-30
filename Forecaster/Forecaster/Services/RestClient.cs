using System.Collections.Generic;
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
        ///     Load weather information about specific city.
        /// </summary>
        /// <param name="cityId">An id of requested city</param>
        /// <returns>Full weather information for the city.</returns>
        public abstract Task<WeatherInfo> GetWeatherByCityIdAsync(int cityId);

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
            JObject jObject = JObject.Parse(restResponse);
            if (jObject.Value<int>("code") == 404)
                yield break;

            foreach (JToken token in jObject["list"])
            {
                yield return new City
                {
                    Name = token.Value<string>("name"),
                    Id = token.Value<int>("id"),
                    Country = token["sys"].Value<string>("country")
                };
            }
        }

        /// <summary>
        ///     Parse weather info from string to object.
        /// </summary>
        /// <param name="restResponse">Json string</param>
        /// <returns>An instance of Forecaster.Models.WeatherInfo type </returns>
        protected WeatherInfo ExtractWeatherInfo(string restResponse)
        {
            JToken infoObject = JObject.Parse(restResponse);

            if (infoObject.Value<int>("cod") == 404)
                return null;

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
}