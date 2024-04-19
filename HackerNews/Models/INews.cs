using System.Text.Json.Serialization;

namespace HackerNews.Models
{
    public interface INews
    {
        public int Id { get; set; }
        public string By { get; set; }
        public int Score { get; set; }
        public int Time { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NewsType Type  { get; set; }
        public string Url { get; set; }
    }
}
