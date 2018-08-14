namespace PaketGlobal
{
	public class Config
	{
        internal const string PackageService = "PackageService"; //TODO: should be split: bridge, route, identity
        internal const string FundService = "FundService";

        internal const string BridgeService = "BridgeService";
        internal const string RouteService = "RouteService";
        internal const string IdentityService = "IdentityService";

		internal const string ServerUrl = "http://api.paket.global";
		internal const string FundServerUrl = "http://fund.paket.global";
        internal const string BridgeServerUrl = "";
        internal const string RouteServerUrl = "";
        internal const string IdentityServerUrl = "";

		internal const string PrefundTestUrl = "https://friendbot.stellar.org";
		
        internal const string ServerVersion = "v3";
		internal const string FundServerVersion = "v2";
        internal const string BridgeServerVersion = "v2";
        internal const string RouteServerVersion = "v2";
        internal const string IdentityServerVersion = "v2";

        internal const string CountlyServerURL = "http://c.paket.global";
		internal const string CountlyAppKey = "21cba638718aa2289f59862a87919d9cb159ba99";
	}
}
