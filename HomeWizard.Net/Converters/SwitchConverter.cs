using Newtonsoft.Json.Linq;
using System;

namespace HomeWizard.Net.Converters
{
    internal class SwitchConverter : JsonCreationConverter<Switch>
    {
        protected override Switch Create(Type objectType, JObject jObject)
        {
            if (FieldExists("type", jObject))
            {
                string typeText = jObject["type"].ToString();
                if (typeText == "dimmer" || typeText == ((int)SwitchType.Dimmer).ToString())
                {
                    return new Dimmer();
                }
                if (typeText == "hue" || typeText == ((int)SwitchType.Hue).ToString())
                {
                    return new HueLight();
                }
            }
            return new Switch();
        }
    }
}
