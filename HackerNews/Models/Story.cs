using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HackerNews.Models
{
    public class Story : INews
    {
        public int Id { get; set; }
        public string By { get; set; }
        public int Score { get; set; }
        public int Time { get; set; }
        public NewsType Type { get; set; }
        public string Url { get; set; }
        public int Descendants { get; set; }

        [JsonIgnore]
        public int[] Kids { get; set; }
        public string Title { get; set; }

    }

}
