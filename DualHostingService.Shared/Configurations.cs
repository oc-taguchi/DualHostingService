namespace DualHostingService.Shared
{
    public class ServiceConfig
    {
        public string TextToPrint { get; set; }
        public int Delay { get; set; }
    }

    public class EchoConfig
    {
        public string Url { get; set; }
    }

    public static class ConfigExtensions
    {
        public static string ToJsonString(this ServiceConfig source)
        {
            return $@"{{
    ""{nameof(source.TextToPrint)}"": ""{source.TextToPrint}"",
    ""{nameof(source.Delay)}"": {source.Delay}
}}";
        }
    }
}