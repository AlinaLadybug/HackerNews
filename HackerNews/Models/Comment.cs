namespace HackerNews.Models
{
    public class Comment : INews
    {
        public int Id { get; set; }
        public string By { get; set; }
        public int Score { get; set; }
        public int Time { get; set; }
        public NewsType Type { get; set; }
        public string Url { get; set; }
        public int Parent { get; set; }
        public int[] Kids { get; set; }
        public string Text { get; set; }
    }
}
