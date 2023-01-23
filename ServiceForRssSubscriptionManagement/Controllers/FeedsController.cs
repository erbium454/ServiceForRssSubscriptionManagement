using System;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceForRssSubscriptionManagement.Models.RssModels;
using ServiceForRssSubscriptionManagement.Models.DataModels;
using ServiceForRssSubscriptionManagement.Models.ActionResults;
using ServiceForRssSubscriptionManagement.Models.DataAccess.Feeds;
using Microsoft.AspNetCore.Authorization;

namespace ServiceForRssSubscriptionManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeedsController : ControllerBase
    {
        FeedContext feedsDB;
        
        public FeedsController()
        {
            feedsDB = new FeedContext();
            feedsDB.Database.EnsureCreated();
        }

        async Task<RssResult> RespondWithRssFeed(IQueryable<ItemEntity> itemEntities) 
        {
            return new RssResult(new RssResultParams
            {
                rss_version = "2.0",
                title = "Feeds Service API",
                description = "Feeds Service API",
                link = Request.Host.ToString(),
                items = itemEntities.Select(item => new FeedItem
                {
                    title = item.Title,
                    link = item.Link,
                    description = item.Description,
                    category = item.Category,
                    comments = item.Comments,
                    enclosure =
                        !string.IsNullOrWhiteSpace(item.EnclosureUrl) ||
                        !string.IsNullOrWhiteSpace(item.EnclosureLength) ||
                        !string.IsNullOrWhiteSpace(item.EnclosureType) ?
                        new FeedItem.Enclosure
                        {
                            url = item.EnclosureUrl,
                            length = item.EnclosureLength,
                            type = item.EnclosureType,
                        } : null,
                    guid = item.Id,
                    pubDate = item.PubDate.Value.ToString("R"),
                    source =
                        !string.IsNullOrWhiteSpace(item.SourceName) ||
                        !string.IsNullOrWhiteSpace(item.SourceUrl) ?
                        new FeedItem.Source
                        {
                            name = item.SourceName,
                            url = item.SourceUrl
                        } : null
                })
            });
        }

        [HttpGet]
        public async Task<RssResult> Get()
        {
            return await RespondWithRssFeed(feedsDB.Items.AsNoTracking());
        }
        [HttpGet("unread")]
        public async Task<RssResult> GetUnread(DateTime? date)
        {
            var query = feedsDB.Items.AsNoTracking()
                .Where(item => item.IsRead == false);
            if (date != null)
                query = query
                    .Where(item => item.PubDate >= date.Value.ToUniversalTime())
                    .OrderByDescending(item => item.PubDate);

            return await RespondWithRssFeed(query);
        }
        //[HttpGet("test")]
        //public IActionResult GetTest()
        //{
        //    return Ok(feedsDB.Items.AsNoTracking());
        //}

        [HttpPost]
        public async Task<IActionResult> Post(string feed_url)
        {
            var client = new HttpClient();
            var ser = new XmlSerializer(typeof(FeedRss));
            var feedRss = ser.Deserialize(await client.GetStreamAsync(feed_url)) as FeedRss;

            feedsDB.Items.AddRange(feedRss.channel.items.Select(item => new ItemEntity
            {
                Id = Guid.NewGuid().ToString(),
                Title = item.title,
                Link = item.link,
                Description = item.description,
                Category = item.category,
                Comments = item.comments,
                EnclosureUrl = item.enclosure?.url,
                EnclosureLength = long.TryParse(item.enclosure?.length, out long length) ? length : null,
                EnclosureType = item.enclosure?.type,
                PubDate = DateTime.TryParse(item.pubDate, out DateTime pub_date) ? pub_date.ToUniversalTime() : null,
                SourceName = item.source?.name,
                SourceUrl = item.source?.url
            }));

            await feedsDB.SaveChangesAsync();

            return StatusCode(201);
        }

        [HttpPatch("isread/{item_id}")]
        public async Task<IActionResult> PatchIsRead(string item_id) 
        {
            var item = new ItemEntity { Id = item_id, IsRead = true };
            feedsDB.Entry(item).Property(i => i.IsRead).IsModified = true;
            await feedsDB.SaveChangesAsync();
            return Ok();
        }
        [HttpPatch("isread")]
        public async Task<IActionResult> PatchIsRead(ItemIdsModel itemIdsModel)
        {
            var items = itemIdsModel.item_ids.Select(id => new ItemEntity { Id = id, IsRead = true });
            foreach (var item in items)
                feedsDB.Entry(item).Property(i => i.IsRead).IsModified = true;
            await feedsDB.SaveChangesAsync();
            return Ok();
        }
    }
}
