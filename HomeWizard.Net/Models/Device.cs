using HomeWizard.Net.Converters;
using Newtonsoft.Json;

namespace HomeWizard.Net
{
    public abstract class Device
    {
        public string Id { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(BooleanConverter))]
        public bool Favorite { get; set; }
    }
}
