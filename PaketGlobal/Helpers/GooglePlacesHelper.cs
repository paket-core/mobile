using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Geolocator;

namespace PaketGlobal
{
    public enum PlaceType
    {
        All,
        Geocode,
        Address,
        Establishment,
        Regions,
        Cities
    }

    public class GooglePlace
    {
        
        public string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Raw { get; set; }

        public string Address { get; set; }

        public string Country { get; set; }

        public GooglePlace(JObject jsonObject)
        {
            Name = (string)jsonObject["result"]["name"];
            Latitude = (double)jsonObject["result"]["geometry"]["location"]["lat"];
            Longitude = (double)jsonObject["result"]["geometry"]["location"]["lng"];
            JArray address_components = (JArray)jsonObject["result"]["address_components"];
            var count = address_components.Count;

            for (int i = 0; i < count; i++)
            {
                var obj = address_components[i];
                var type = (string)obj["types"][0];
                if(type=="country")
                {
                    Country = (string)obj["short_name"];
                }

            }
            Raw = jsonObject.ToString();
        }

        public GooglePlace()
        {
        }
    }

    public class StructuredFormatting
    {
        private string icon;

        [JsonProperty("main_text")]
        public string MainText { get; set; }

        [JsonProperty("secondary_text")]
        public string SecondText { get; set; }

        public string Icon { 
            get{
                if(icon==null)
                {
                    return "map_cion.png";
                }
                return icon;
            } 
            set{
                icon = value;  
            } 
        }
    }

    public class AutoCompletePrediction
    {

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("place_id")]
        public string Place_ID { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("structured_formatting")]
        public StructuredFormatting Structured { get; set; }
    }

    public class AutoCompleteResult
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("predictions")]
        public List<AutoCompletePrediction> AutoCompletePlaces { get; set; }
    }

    public class GooglePlacesHelper
    {
        private string Location = null;

        public GooglePlacesHelper()
        {
            
        }

        public async Task<AutoCompleteResult> GetPlaces(string newTextValue,string location)
        {
            try
            {
                Location = location;

                var requestURI = CreatePredictionsUri(newTextValue);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, requestURI);
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();

                if (result == "ERROR")
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<AutoCompleteResult>(result);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string CreatePredictionsUri(string newTextValue)
        {
            var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            var url = "https://maps.googleapis.com/maps/api/place/autocomplete/json";
            var input = Uri.EscapeUriString(newTextValue);
            var pType = PlaceTypeValue(PlaceType.Geocode);
            var constructedUrl = $"{url}?input={input}&types={pType}&key={Config.GooglePlacesKEY}&language={language}";
            if(Location!=null)
            {
                constructedUrl = constructedUrl + "&location=" + Location + "&radius=50000&strictbounds";
            }
            return constructedUrl;
        }

        private string PlaceTypeValue(PlaceType type)
        {
            switch (type)
            {
                case PlaceType.All:
                    return "";
                case PlaceType.Geocode:
                    return "geocode";
                case PlaceType.Address:
                    return "address";
                case PlaceType.Establishment:
                    return "establishment";
                case PlaceType.Regions:
                    return "(regions)";
                case PlaceType.Cities:
                    return "(cities)";
                default:
                    return "";
            }
        }


        public async Task<GooglePlace> GetPlace(string placeID)
        {
            try
            {
                var requestURI = CreateDetailsRequestUri(placeID, Config.GooglePlacesKEY);
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, requestURI);
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();

                if (result == "ERROR")
                {
                    return null;
                }

                return new GooglePlace(JObject.Parse(result));
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string CreateDetailsRequestUri(string place_id, string apiKey)
        {
            var url = "https://maps.googleapis.com/maps/api/place/details/json";
            return $"{url}?placeid={Uri.EscapeUriString(place_id)}&key={apiKey}";
        }
    }
}
