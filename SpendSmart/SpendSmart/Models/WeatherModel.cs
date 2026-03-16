namespace SpendSmart.Models
{
    public class WeatherModel
    {
        public class IpLocation
        {
            public double lat { get; set; }
            public double lon { get; set; }
            public string city { get; set; }
            public string IpAddress { get; set; }
        }

        public class DailyWeather
        {
            public long dt { get; set; }
            public Temp temp { get; set; }
        }

        public class Temp
        {
            public double day { get; set; }
        }

    }
}
