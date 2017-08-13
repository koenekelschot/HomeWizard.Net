using HomeWizard.Net.Converters;
using Newtonsoft.Json;

namespace HomeWizard.Net
{
    public class Handshake
    {
        [JsonConverter(typeof(BooleanConverter))]
        public bool HomeWizard { get; set; }
        public string Version { get; set; }
        [JsonConverter(typeof(BooleanConverter))]
        public bool FirmwareUpdateAvailable { get; set; }
        [JsonConverter(typeof(BooleanConverter))]
        public bool AppUpdateRequired { get; set; }
        public string Serial { get; set; }
    }
}
