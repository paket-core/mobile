using System;
using System.Globalization;

namespace PaketGlobal
{
    public class StellarConverter
    {
        public static double ThenMlns = 10000000.0f;

        public static string ConvertValueToString(long value)
        {
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            culture.NumberFormat.CurrencyDecimalSeparator = ".";
            culture.NumberFormat.CurrencyGroupSeparator = ",";
            culture.NumberFormat.CurrencySymbol = "";

            double result = value / ThenMlns;

            return result.ToString("C2", culture);
        }

        public static bool IsValidBUL(double value)
        {
            if(value<1){
                return false;
            }

            return true;
        }

        public static double ConvertBULToStroops(double value){
            return value * ThenMlns;
        }
    }
}
