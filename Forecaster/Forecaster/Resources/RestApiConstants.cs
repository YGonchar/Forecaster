namespace Forecaster.Resources
{
    /// <summary>
    ///     Rest API query resources.
    /// </summary>
    public static class RestApiConstants
    {
        public const string BaseUri = "http://api.openweathermap.org/data/2.5/";
        public const string AppId = "36dbca175b1cbc2b8f0a917d5df4c457";
        public const string FindManyCities = "find?q={0}&type=like&appid={1}";
        public const string UnitsParam = "units";
    }
}
