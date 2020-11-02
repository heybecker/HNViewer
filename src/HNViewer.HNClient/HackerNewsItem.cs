namespace HNViewer.HNClient
{
    public class HackerNewsItem
    {
        public int Id { get; set; }
        public bool? Deleted { get; set; }
        public string Type { get; set; }
        public string By { get; set; }
        public int? Time { get; set; }
        public string Title { get; set; }
    }
}
