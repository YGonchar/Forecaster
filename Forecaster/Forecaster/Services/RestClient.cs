using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forecaster.Models;
using Forecaster.Resources;
using Newtonsoft.Json.Linq;

namespace Forecaster.Services
{
    /// <summary>
    ///     A client for working with OpenWeatherMap Rest API
    /// </summary>
    /// <remarks>Use Xamarin.Forms.Device class to retrieve a platform specific RestClient</remarks>
    public abstract class RestClient
    {
        protected readonly string BaseUrl;
        protected readonly string AppId;
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
        ///     Extract the cities from json string.
        /// </summary>
        /// <param name="restResponse">Json string received by rest request.</param>
        /// <returns></returns>
        protected IEnumerable<City> ExtractCities(string restResponse)
        {
            JObject jObject = JObject.Parse(restResponse, new JsonLoadSettings
            { CommentHandling = CommentHandling.Ignore, LineInfoHandling = LineInfoHandling.Ignore });
            return jObject["list"].Children()
                .Select(token => new City
                {
                    Name = token.Value<string>("name"),
                    Id = token.Value<int>("id"),
                    Country = token["sys"].Value<string>("country")
                }
                ).AsEnumerable();
        }
    }
}
