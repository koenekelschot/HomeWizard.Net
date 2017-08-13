using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HomeWizard.Net
{
    public class HueLight : Switch
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public override SwitchType Type { get { return SwitchType.Hue; } }
        [JsonProperty("hue_id")]
        public long HueId { get; set; }
        [JsonProperty("light_id")]
        public long LightId { get; set; }
        public HueColor Color { get; set; }

        public class HueColor
        {
            public uint Hue { get; set; }
            public uint Sat { get; set; }
            public uint Bri { get; set; }

            /*public byte A => 255;
            public byte R => ToRgb().R;
            public byte G => ToRgb().G;
            public byte B => ToRgb().B;

            private struct RgbColor
            {
                public byte R;
                public byte G;
                public byte B;
            }

            private RgbColor ToRgb()
            {
                float r = Bri;
                float g = Bri;
                float b = Bri;
                if (Sat != 0)
                {
                    float max = Bri;
                    float dif = Bri * Sat / 255f;
                    float min = Bri - dif;

                    float h = (Hue / 255f) * 360f / 255f;

                    if (h < 60f)
                    {
                        r = max;
                        g = h * dif / 60f + min;
                        b = min;
                    }
                    else if (h < 120f)
                    {
                        r = -(h - 120f) * dif / 60f + min;
                        g = max;
                        b = min;
                    }
                    else if (h < 180f)
                    {
                        r = min;
                        g = max;
                        b = (h - 120f) * dif / 60f + min;
                    }
                    else if (h < 240f)
                    {
                        r = min;
                        g = -(h - 240f) * dif / 60f + min;
                        b = max;
                    }
                    else if (h < 300f)
                    {
                        r = (h - 240f) * dif / 60f + min;
                        g = min;
                        b = max;
                    }
                    else if (h <= 360f)
                    {
                        r = max;
                        g = min;
                        b = -(h - 360f) * dif / 60 + min;
                    }
                    else
                    {
                        r = 0;
                        g = 0;
                        b = 0;
                    }
                }

                return new RgbColor
                {
                    R = (byte) Math.Round(Math.Min(Math.Max(r, 0), 255)),
                    G = (byte) Math.Round(Math.Min(Math.Max(g, 0), 255)),
                    B = (byte) Math.Round(Math.Min(Math.Max(b, 0), 255))
                };
            }*/
        }
    }
}
