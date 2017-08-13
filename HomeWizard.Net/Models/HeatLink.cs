using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HomeWizard.Net
{
    public class HeatLink : Device
    {
        public string Code { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public OnOff Pump { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public OnOff Heating { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public OnOff Dwh { get; set; }
        public decimal Rte { get; set; }
        public decimal Rsp { get; set; }
        public decimal Tte { get; set; }
        public string Ttm { get; set; } //something nullable
        public decimal Wp { get; set; }
        public decimal Wte { get; set; }
        public int Ofc { get; set; }
        public int Odc { get; set; }
        public Preset[] Presets { get; set; }

        public class Preset
        {
            public long Id { get; set; }
            public decimal Te { get; set; } //temperature
        }
    }
}
