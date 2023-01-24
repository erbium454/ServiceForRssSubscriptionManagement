using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ServiceForRssSubscriptionManagement.Models.RssModels;
using System.Net.NetworkInformation;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ServiceForRssSubscriptionManagement.Models.ActionResults
{
    public sealed class StrWriter : StringWriter
    {
        private readonly Encoding encoding;

        public StrWriter(Encoding encoding)
        {
            this.encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return encoding; }
        }
    }

    public class RssResultParams
    {
        public string rss_version { get; set; }
        public string title { get; set; }
        public string link { get; set; }
        public string description { get; set; }
        public IQueryable<FeedItem> items { get; set; }
    }
    public class RssResult : IActionResult
    {
        private static readonly XmlSerializer xmlSer = new(typeof(FeedItem));
        private readonly RssResultParams rssParams;

        public RssResult(RssResultParams rssParams)
        {
            this.rssParams = rssParams;
        }

        public async Task ExecuteResultAsync(ActionContext actionContext)
        {
            var Response = actionContext.HttpContext.Response;

            Response.ContentType = "application/xml; charset=utf-8";

            await using XmlWriter writer = XmlWriter.Create(Response.Body, new XmlWriterSettings { Encoding = Encoding.UTF8, Async = true });

            // rss
            await writer.WriteStartElementAsync(null, "rss", null);
                await writer.WriteAttributeStringAsync(null, "version", null, rssParams.rss_version);
                // channel
                await writer.WriteStartElementAsync(null, "channel", null);
                    await writer.WriteElementStringAsync(null, "title", null, rssParams.title);
                    await writer.WriteElementStringAsync(null, "link", null, rssParams.link);
                    await writer.WriteElementStringAsync(null, "description", null, rssParams.description);
                    // item
                    foreach (var item in rssParams.items)
                    {
                        await using var sw = new StrWriter(Encoding.UTF8);
                        await using var item_writer = XmlWriter.Create(sw, new XmlWriterSettings() { Encoding = Encoding.UTF8, Async = true, OmitXmlDeclaration = true });
                        xmlSer.Serialize(item_writer, item, new XmlSerializerNamespaces(new XmlQualifiedName[] { XmlQualifiedName.Empty }));
                        await writer.WriteRawAsync(sw.ToString());
                    }
                // channel
                await writer.WriteEndElementAsync();
            // rss
            await writer.WriteEndElementAsync();

            await writer.FlushAsync();
        }
    }
}
