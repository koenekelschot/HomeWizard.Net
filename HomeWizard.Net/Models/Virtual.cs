using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HomeWizard.Net
{
    public class Virtual : Switch
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public override SwitchType Type { get { return SwitchType.Virtual; } }
    }
}
