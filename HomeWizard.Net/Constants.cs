namespace HomeWizard.Net
{
    public static class Constants
    {
        public const string BaseUrl = "http://{ipAddress}/{password}/{command}/";
        public const string HandshakeUrl = "http://{ipAddress}/{command}/";
        public const string DiscoveryUrl = "http://gateway.homewizard.nl/discovery.php";

        public const decimal TemperatureMax = 35;
        public const decimal TemperatureMin = 5;
        public const int DimLevelMin = 0;
        public const int DimLevelMax = 100;
        public const int SwitchNameLength = 15;
    }
}
