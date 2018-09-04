using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PaketGlobal
{
    public static class AddressBookHelper
    {
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

