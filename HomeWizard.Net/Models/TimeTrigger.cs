using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HomeWizard.Net
{
    public class TimeTrigger : Trigger
    {
        public override TriggerType Type { get { return TriggerType.Time; } }
        public string Time { get; set; }
        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        public IList<Preset> Presets { get; set; } 
    }
}
