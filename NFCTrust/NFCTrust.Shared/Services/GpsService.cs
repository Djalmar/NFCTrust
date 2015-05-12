using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace NFCTrust.Services
{
    public class GpsService
    {
        public async Task<KeyValuePair<double, double>> GetCurrentGeoposition()
        {
            Geolocator locator = new Geolocator();
            locator.DesiredAccuracyInMeters = 10;
            locator.DesiredAccuracy = PositionAccuracy.High;
            var geoposition = await locator.GetGeopositionAsync();
            return new KeyValuePair<double, double>(geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude);
        }
    }
}
