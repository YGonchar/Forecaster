using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Forecaster.iOS.SpecificServices;
using Forecaster.Models;
using Forecaster.Resources;
using RestSharp;
using Xamarin.Forms;
using RestClientBase = Forecaster.Services.RestClient;

[assembly: Dependency(typeof(RestClientiOS))]

namespace Forecaster.iOS.SpecificServices
{
    // ReSharper disable once InconsistentNaming
    public class RestClientiOS : RestClientBase
    {
        private readonly RestClient _client;

        public RestClientiOS()
        {
            _client = new RestClient(BaseUrl);
        }

        public override async Task<IEnumerable<City>> FindCitiesAsync(string cityName)
        {
            var tcs = new TaskCompletionSource<IEnumerable<City>>();

            RestRequest request = new RestRequest(string.Format(RestApiConstants.FindManyCities, cityName, AppId));
            request.AddQueryParameter(RestApiConstants.UnitsParam, UnitsType);
            request.RequestFormat = DataFormat.Json;
            var response = _client.Execute(request);

            try
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new Exception("No cities with such name were not found");
                var cities = ExtractCities(response.Content);
                tcs.SetResult(cities);
            }
            catch (Exception exc)
            {
                tcs.SetException(exc);
            }
            return await tcs.Task;
        }

        public override async Task<WeatherInfo> GetWeatherByCityAsync(City city)
        {
            var tcs = new TaskCompletionSource<WeatherInfo>();

            string resouceUri = city.Id != 0
                ? string.Format(RestApiConstants.GetWeatherInfoForCity, city.Id, AppId)
                : string.Format(RestApiConstants.GetWeatherInfoForCity, city.Name, AppId);


            RestRequest request = new RestRequest(resouceUri);
            request.AddQueryParameter(RestApiConstants.UnitsParam, UnitsType);
            request.RequestFormat = DataFormat.Json;
            var response = _client.Execute(request);

            try
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new Exception("Incorrect city ID");
                if (response.Content == string.Empty)
                {
                    tcs.SetResult(null);
                }
                else
                {
                    var info = ExtractWeatherInfo(response.Content);
                    tcs.SetResult(info);
                }
            }
            catch (WebException exc) when (exc.Status == WebExceptionStatus.NameResolutionFailure)
            {
                throw new WebException("No internet connection", exc);
            }
            catch (Exception exc)
            {
                tcs.SetException(exc);
            }
            return await tcs.Task;
        }

        public override Task<WeatherInfo> GetWeatherByCityIdAsync(int cityId)
        {
            return GetWeatherByCityAsync(new City {Id = cityId});
        }

        public override async Task<byte[]> LoadIconAsync(string iconId)
        {
            var tcs = new TaskCompletionSource<byte[]>();
            WebClient cl = new WebClient();
            string adress = string.Format(RestApiConstants.LoadWeatherIcon, iconId);
            try
            {
                var data = cl.DownloadData(new Uri(adress));
                tcs.SetResult(data);
            }
            catch (Exception exc)
            {
                tcs.SetException(exc);
            }

            return await tcs.Task;
        }
    }
}