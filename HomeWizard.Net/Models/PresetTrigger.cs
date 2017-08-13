using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HomeWizard.Net
{
    public class PresetTrigger : Trigger
    {
        public override TriggerType Type { get { return TriggerType.Preset; } }
        [JsonConverter(typeof(StringEnumConverter))]
        public Preset Preset { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
