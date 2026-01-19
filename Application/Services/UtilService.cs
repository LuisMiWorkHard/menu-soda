namespace MenuSoda.Application.Services
{
    public class UtilService
    {
        public (decimal? lat, decimal? lon) ParseGeo(string latStr, string lonStr)
    {
        if (decimal.TryParse(latStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var lat) &&
            decimal.TryParse(lonStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var lon))
        {
            return (lat, lon);
        }
        return (null, null);
    }
    }
}