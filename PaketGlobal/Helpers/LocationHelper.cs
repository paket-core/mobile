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
                var location = lat.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + lng.ToString(System.Globalization.CultureInfo.InvariantCulture);

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
                    var location = position.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + position.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);

                    lat = position.Latitude;
                    lng = position.Longitude;

                    return location;
                }
            }

            return null;
        }
    }
}
