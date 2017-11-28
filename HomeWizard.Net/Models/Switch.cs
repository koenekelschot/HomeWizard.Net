using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HomeWizard.Net
{
    public class Switch : Device
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public virtual SwitchType Type { get { return SwitchType.Switch; } }
        [JsonConverter(typeof(StringEnumConverter))]
        public OnOff Status { get; set; }

        public int OnStatus { get; set; }
        public int OffStatus { get; set; }

        public bool IsDimmer
        {
            get { return GetType() == typeof (Dimmer) && Type == SwitchType.Dimmer; }
        }

        public bool IsHue
        {
            get { return GetType() == typeof (HueLight) && Type == SwitchType.Hue; }
        }

        public bool IsSomfy
        {
            get { return GetType() == typeof(Somfy) && Type == SwitchType.Somfy; }
        }

        public bool IsVirtual
        {
            get { return GetType() == typeof(Virtual) && Type == SwitchType.Virtual; }
        }
        public string Code { get; set; }
    }
}
