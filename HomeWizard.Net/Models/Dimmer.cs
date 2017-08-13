using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HomeWizard.Net
{
    public class Dimmer : Switch
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public override SwitchType Type { get { return SwitchType.Dimmer; } }
        public int DimLevel { get; set; }
    }
}
