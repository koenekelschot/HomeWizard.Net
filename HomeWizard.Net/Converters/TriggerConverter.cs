using Newtonsoft.Json.Linq;
using System;

namespace HomeWizard.Net.Converters
{
    internal class TriggerConverter : JsonCreationConverter<Trigger>
    {
        protected override Trigger Create(Type objectType, JObject jObject)
        {
            if (FieldExists("type", jObject))
            {
                string typeText = jObject["type"].ToString();
                if (typeText == "preset" || typeText == ((int)TriggerType.Preset).ToString())
                {
                    return new PresetTrigger();
                }
            }
            return new TimeTrigger();
        }
    }
}
