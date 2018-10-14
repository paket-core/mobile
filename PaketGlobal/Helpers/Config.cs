namespace PaketGlobal
{
	public class Config
	{
        internal const string BridgeService = "BridgeService1";
        internal const string RouteService = "RouteService1";
        internal const string IdentityService = "IdentityService1";

		internal static string BridgeServerUrl = "http://itd.pub:11251";//Local
        internal static string RouteServerUrl = "http://itd.pub:11250";//Local
        internal static string IdentityServerUrl = "http://itd.pub:11252";//Local
	
        //internal static string BridgeServerUrl = "https://bridge.paket.global";//Global
        //internal static string RouteServerUrl = "https://route.paket.global";//Global
        //internal static string IdentityServerUrl = "https://fund.paket.global";//Global

		internal const string PrefundTestUrl = "https://friendbot.stellar.org";
		
		internal const string FundServerVersion = "v2";

        internal const string BridgeServerVersion = "v3";
        internal const string RouteServerVersion = "v3";
        internal const string IdentityServerVersion = "v2";

        internal const string CountlyServerURL = "http://c.paket.global";
		internal const string CountlyAppKey = "21cba638718aa2289f59862a87919d9cb159ba99";

        internal const string GooglePlacesKEY = "AIzaSyDBRCrs7vfP_9gSaieSNs2wMqSZYFRuiz8";
        internal const string GoogleFirebase = "https://paketglobal-fierbase.firebaseio.com/";

	}
}
