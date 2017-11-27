using Newtonsoft.Json;

namespace HomeWizard.Net
{
    public class RainMeter : Device
    {
        public int model { get; set; }
        public double mm { get; set; }
        [JsonProperty(PropertyName = "3h")]
        public double _3h { get; set; }
    }
}
