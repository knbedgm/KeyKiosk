using System.Collections.Generic;

namespace KeyKiosk.Data
{
    public class DrawerConfig
    {
        public string Name { get; set; }
        public int RelayIndex { get; set; }
        public int SensorIndex { get; set; }

    }

    public static class DrawerConfigExtentions
    {
        public static ICollection<DrawerConfig>? GetDrawerConfigs(this IConfiguration configuration)
        {
            return configuration.GetSection("Drawers").Get<ICollection<DrawerConfig>>();
        }
    }
}


//    {
//      "Name": "A1",
//      "RelayIndex": 1,
//      "SensorIndex": 1
//    },
