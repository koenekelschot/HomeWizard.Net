using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HomeWizard.Net
{
    public class Somfy : Switch
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public override SwitchType Type { get { return SwitchType.Somfy; } }
        public int Mode { get; set; }
    }
}
