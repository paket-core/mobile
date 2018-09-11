using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using RestSharp;

namespace PaketGlobal
{
  
  
    public class MapHelper
    {
        public const string accessToken = "pk.eyJ1IjoiYW5kcmV5bSIsImEiOiJjajBiMDZudWwwMDE1MzNud3F5dnkzMnRxIn0.BrHmtPI7u5gYbSDUbkjAgA";

        readonly RestClient client;
        CancellationTokenSource cts;

        public MapHelper()
        {
            client = new RestClient("https://api.mapbox.com");
            client.UserAgent = "Cycliq";
            client.Timeout = 20 * 1000;
            cts = new CancellationTokenSource();
        }

        public async Task<byte[]> GetStaticMap(double latitude, double longitude, double zoom, int width, int height, float bearing = 0.0f, float pitch = 0.0f, bool classicMap = false, bool retina = false)
        {
            string res = this.GetStaticMapUri(latitude, longitude, zoom, width, height, bearing, pitch, classicMap, retina);

            var request = PrepareRequest(res, Method.GET);

            RawBytes rb = new RawBytes();

            var result = await SendRequest<byte[]>(request, rb);

            return rb.Data;
        }

        public string GetStaticMapUri(double latitude, double longitude, double zoom, int width, int height, float bearing = 0.0f, float pitch = 0.0f, bool classicMap = false, bool retina = false)
        {
            var latStr = latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var longStr = longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var zoomStr = zoom.ToString(System.Globalization.CultureInfo.InvariantCulture);

            string res;

            if (classicMap)
            {
                res = String.Format("/v4/mapbox.streets-basic/{0},{1},{2}/{3}x{4}{5}.png", longStr, latStr, zoomStr, width, height, retina ? "@2x" : "");
            }
            else
            {
                res = String.Format("/styles/v1/mapbox/streets-v8/static/{0},{1},{2},{3},{4}/{5}x{6}{7}", longStr, latStr, zoomStr, bearing, pitch, width, height, retina ? "@2x" : "");
            }

            res = "https://api.mapbox.com" + res + "?access_token=" + accessToken;

            return res;
        }

        private RestRequest PrepareRequest(string uri, Method method)
        {
            var request = new RestRequest(uri);
            request.RequestFormat = DataFormat.Json;
            request.Method = method;
            return request;
        }

        private async Task<T> SendRequest<T>(RestRequest request, RawBytes rb = null)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Sending request: {0}{1}", client.BaseUrl, request.Resource));
                var response = await client.ExecuteTaskAsync<T>(request, cts.Token);
                if (rb != null)
                {
                    rb.Data = response.RawBytes;
                }
                return response.Data;
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return default(T);
        }

        public double distance(double lat1, double lon1, double lat2, double lon2)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            //to km
            dist = dist * 1.609344;

            return (dist);
        }

        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }
    }
}
