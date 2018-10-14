using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PaketGlobal
{
    public static class AddressBookHelper
    {
        public static List<string> CallSigns = AddressBookHelper.GetCallSigns();

        public static List<string> GetItems()
        {
            List<string> items = new List<string>();

            object fromStorage;

            if (Application.Current.Properties.ContainsKey(Constants.ADDRESS_BOOK))
            {
                Application.Current.Properties.TryGetValue(Constants.ADDRESS_BOOK, out fromStorage);

                items = JsonConvert.DeserializeObject<List<string>>(fromStorage as string);
            }

            return items;
        }

        public static List<string> GetCallSigns()
        {
            List<string> items = new List<string>();

            object fromStorage;

            if (Application.Current.Properties.ContainsKey(Constants.CALL_SIGNS_BOOK))
            {
                Application.Current.Properties.TryGetValue(Constants.CALL_SIGNS_BOOK, out fromStorage);

                items = JsonConvert.DeserializeObject<List<string>>(fromStorage as string);
            }

            return items;
        }

        public static async void LoadCallSigns()
        {
            var result = await App.Locator.IdentityServiceClient.GetCallsigns();

            if (result != null)
            {
                if (result.Callsigns != null)
                {
                    AddressBookHelper.CallSigns = result.Callsigns;

                    var jsonValueToSave = JsonConvert.SerializeObject(result.Callsigns);

                    Application.Current.Properties[Constants.CALL_SIGNS_BOOK] = jsonValueToSave;

                    await Application.Current.SavePropertiesAsync();
                }
            }
        }

        public static void AddItem(string item)
        {
            List<string> items = new List<string>();

            object fromStorage;

            if(Application.Current.Properties.ContainsKey(Constants.ADDRESS_BOOK))
            {
                Application.Current.Properties.TryGetValue(Constants.ADDRESS_BOOK, out fromStorage);

                items = JsonConvert.DeserializeObject <List<string>>(fromStorage as string);
            }

            if(!items.Contains(item))
            {
                items.Insert(0, item);

                var jsonValueToSave = JsonConvert.SerializeObject(items);

                Application.Current.Properties[Constants.ADDRESS_BOOK] = jsonValueToSave;

                Application.Current.SavePropertiesAsync();
            }  
        }

    }
}

