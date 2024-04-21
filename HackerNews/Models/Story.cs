using Newtonsoft.Json;

namespace HackerNews.Models
{
    public class Story : BaseNews
    {
        public int Descendants { get; set; }

        [JsonIgnore]
        public int[] Kids { get; set; }
        public string Title { get; set; }
    }
}
