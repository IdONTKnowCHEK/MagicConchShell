namespace LineBotMessage.Dtos
{
    public class WebhookEventsDto
    {
        // ---- common property ----
        public string Type { get; set; } // Identifier for the type of event
        public string Mode { get; set; } // active | standby
        public long Timestamp { get; set; } // event occurred time in milliseconds
        public SourceDto Source { get; set; } //  user | group chat | multi-person chat
        public string WebhookEventId { get; set; } // webhook event id - ULID format
        public DeliverycontextDto DeliveryContext { get; set; }
        // Whether the webhook event is a redelivered one or not. DeliveryContext.IsRedelivery : true | false

        // ---- event properties ----
        public string? ReplyToken { get; set; } // Reply token used to send reply message to this event
        public MessageEventDto? Message { get; set; } // text | sticker | image | file | video | audio | location
        public UnsendEventDto? Unsend { get; set; } // Event object for when the user unsends a message.
        public MemberEventDto? Joined { get; set; } // Memmber Joined Event
        public MemberEventDto? Left { get; set; } // Member Leave Event
        public PostbackEventDto? Postback { get; set; } // Postback Event
        public VideoViewingCompleteEventDto? VideoPlayComplete { get; set; } // Video viewing complete event
    }

    // ---- common property ----
    public class SourceDto
    {
        public string Type { get; set; }
        public string? UserId { get; set; }
        public string? GroupId { get; set; }
        public string? RoomId { get; set; }
    }

    public class DeliverycontextDto
    {
        public bool IsRedelivery { get; set; }

    }

    // ---- message event ----
    public class MessageEventDto
    {
        public string Id { get; set; }
        public string Type { get; set; }

        // Text Message Event
        public string? Text { get; set; }
        public List<TextMessageEventEmojiDto>? Emojis { get; set; }
        public TextMessageEventMentionDto? Mention { get; set; }

        // Image & Video & Audio Message Event
        public ContentProviderDto? ContentProvider { get; set; }
        public ImageMessageEventImageSetDto? ImageSet { get; set; }
        public int? Duration { get; set; }

        //File Message Event
        public string? FileName { get; set; }
        public int? FileSize { get; set; }

        //Location Message Event
        public string? Title { get; set; }
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Sticker Message Event
        public string? PackageId { get; set; }
        public string? StickerId { get; set; }
        public string? StickerResourceType { get; set; }
        public List<string>? Keywords { get; set; }
    }

    public class TextMessageEventEmojiDto
    {
        public int Index { get; set; }
        public int Length { get; set; }
        public string ProductId { get; set; }
        public string EmojiId { get; set; }
    }

    public class TextMessageEventMentionDto
    {
        public List<TextMessageEventMentioneeDto> Mentionees { get; set; }
    }

    public class TextMessageEventMentioneeDto
    {
        public int Index { get; set; }
        public int Length { get; set; }
        public string UserId { get; set; }
    }

    public class ContentProviderDto
    {
        public string Type { get; set; }
        public string? OriginalContentUrl { get; set; }
        public string? PreviewImageUrl { get; set; }
    }

    public class ImageMessageEventImageSetDto
    {
        public string Id { get; set; }
        public string Index { get; set; }
        public string Total { get; set; }
    }

    // ---- unsend event ----
    public class UnsendEventDto
    {
        public string messageId { get; set; }
    }


    // ---- member joined & left event ----
    public class MemberEventDto
    {
        public List<SourceDto> Members { get; set; }
    }

    // ---- postback event ----
    public class PostbackEventDto
    {
        public string? Data { get; set; }
        public PostbackEventParamDto? Params { get; set; }
    }

    public class PostbackEventParamDto
    {
        public string? Date { get; set; }
        public string? Time { get; set; }
        public string? Datetime { get; set; }
        public string? NewRichMenuAliasId { get; set; }
        public string? Status { get; set; }
    }

    // ---- videoViewingCompleteEventDto event ----
    public class VideoViewingCompleteEventDto
    {
        public string TrackingId { get; set; }
    }
}