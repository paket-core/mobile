using System;
using System.Threading.Tasks;
using Plugin.Geolocator;

namespace PaketGlobal
{
    public class LocationHelper
    {
        public double lat = 0.0;
        public double lng = 0.0;

        public LocationHelper()
        {
            
        }

        public static string TrimLocation(double lat, double lng)
        {
            try{
                var latString = lat.ToString("0.######",System.Globalization.CultureInfo.InvariantCulture);
                var lngString = lng.ToString("0.######",System.Globalization.CultureInfo.InvariantCulture);

                var location = latString + "," + lngString;

                if (location != null)
                {
                    if (location.Length > 24)
                    {
                        location = location.Substring(0, 24);
                    }

                    return location;
                }

                return "";
            }
            catch{
                return "";
            }
        }

        public async Task<Plugin.Geolocator.Abstractions.Position> GetLocation(bool canGetLast = false)
        {
            if (canGetLast && !lat.Equals(0.0))
            {
                return new Plugin.Geolocator.Abstractions.Position(lat,lng);
            }
            var hasPermission = await Utils.CheckPermissions(Plugin.Permissions.Abstractions.Permission.Location);

            if (hasPermission)
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 100;

                var position = await locator.GetPositionAsync();

                if (position != null)
                {
                    return position;
                }
            }

            return null;
        }

        public async Task<String> GetStringLocation(bool canGetLast = false)
        {
            if(canGetLast && !lat.Equals(0.0))
            {
                var location = LocationHelper.TrimLocation(lat, lng);

                return location; 
            }
            var hasPermission = await Utils.CheckPermissions(Plugin.Permissions.Abstractions.Permission.Location);

            if (hasPermission)
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 100;

                var position = await locator.GetPositionAsync();

                if (position != null)
                {
                    var location = LocationHelper.TrimLocation(position.Latitude, position.Longitude);

                    lat = position.Latitude;
                    lng = position.Longitude;

                    return location;
                }
            }

            return null;
        }
    }
}
