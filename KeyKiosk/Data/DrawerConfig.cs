namespace KeyKiosk.Data
{
    public class DrawerConfig
    {
        public required string Name { get; set; }
        public int RelayIndex { get; set; }
        //public int SensorIndex { get; set; }

    }

    public static class DrawerConfigExtentions
    {
        public static IList<DrawerConfig> GetDrawerConfigs(this IConfiguration configuration)
        {
            var config = configuration.GetSection("Drawers").Get<IList<DrawerConfig>>();

            if (config == null)
            {
				throw new InvalidOperationException("Configuration section 'Drawers' not found.");
			}

            return config;
        }
    }
}


//    {
//      "Name": "A1",
//      "RelayIndex": 1,
//      "SensorIndex": 1
//    },
