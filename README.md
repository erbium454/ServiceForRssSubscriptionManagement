# ServiceForRssSubscriptionManagement

POST api/feeds?feed_url=[Url to feed]              - Add feed items to DB<br />
GET api/feeds                                      - Get all feed items<br />
GET api/feeds/unread?date=[Start date]             - Get unread feed items starting from date<br />
PATCH api/feeds/isread/{item_id}                   - Mark item(news) as already read<br />
PATCH api/feeds/isread BODY: { "item_ids": [] }    - Mark all specified items(news) as already read<br />
