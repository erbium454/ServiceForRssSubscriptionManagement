using System.Xml.Serialization;

namespace ServiceForRssSubscriptionManagement.Models.RssModels
{
    [Serializable, XmlRoot("rss")]
    public class FeedRss
    {
        [XmlAttribute]
        public string version { get; set; }
        public FeedChannel channel { get; set; }
    }
    public class FeedChannel
    {
        public string title { get; set; }
        public string link { get; set; }
        public string description { get; set; }

        [XmlElement("item")]
        public List<FeedItem> items { get; set; }
    }
    [Serializable, XmlRoot("item")]
    public class FeedItem
    {
        public class Enclosure
        {
            [XmlAttribute]
            public string? url { get; set; }
            [XmlAttribute]
            public string? length;
            [XmlAttribute]
            public string? type { get; set; }
        }
        public class Source
        {
            [XmlText]
            public string? name { get; set; }
            [XmlAttribute]
            public string? url { get; set; }
        }

        public string title { get; set; }
        public string link { get; set; }
        public string description { get; set; }

        public string? author { get; set; }
        public string? category { get; set; }
        public string? comments { get; set; }
        public Enclosure? enclosure { get; set; }
        public string? guid { get; set; }
        public string? pubDate { get; set; }
        public Source? source { get; set; }
    }
}
