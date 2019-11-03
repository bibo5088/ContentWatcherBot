# ContentWatcherBot
A discord bot watching for rss feeds, mangadex and more (soon™)!

## Add Watchers
> You must have admin permissions to use `add`.
##### RSS Feed
Use the link to the rss feed.  
###### Example 
To watch for https://example.com/rss-feed on #example-channel.
```
watch!add https://example.com/rss-feed #example-channel
```

##### Mangadex
Use the link of the manga title page.
###### Example
To watch for Beastars on #example-channel.
```
watch!add https://mangadex.org/title/20523/beastars in channel #example-channel
```

## List active watchers
```
watch!list
```  
Response format :
```
[id] ([type]) "[name]"
   [description]
   Update Message : [update message]
   [channel]
```

## Remove a watcher
> You must have admin permissions to use `remove`.
```
watch!remove [id]
```

## Update Message
> You must have admin permissions to use `set`.

When new content is detected the bot will send a message along with a link to said content, the default is `New content from "[name]".
It is possible to change this message with 
```
watch!set message [id] [message]
```
It can be used to ping roles or members.
```
watch!set message 1 @role @member
```