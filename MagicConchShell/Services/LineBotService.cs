using MagicConchShell.Dtos.Message;
using MagicConchShell.Dtos.Message.Request;
using MagicConchShell.Dtos.Webhook;
using MagicConchShell.Enum;
using MagicConchShell.Models;
using MagicConchShell.Providers;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;

namespace MagicConchShell.Services
{
    public class LineBotService
    {
        private readonly IOptions<LineBotSettings> _lineBotSettings;
        private readonly string accessToken;
        private readonly string channelSecret;
        private readonly string replyMessageUri = "https://api.line.me/v2/bot/message/reply";
        private readonly string broadcastMessageUri = "https://api.line.me/v2/bot/message/broadcast";
        private static HttpClient client = new HttpClient();
        private readonly JsonProvider _jsonProvider = new JsonProvider();

        public LineBotService(IOptions<LineBotSettings> lineBotSettings)
        {
            _lineBotSettings = lineBotSettings;
            accessToken = lineBotSettings.Value.AccessToken;
            channelSecret = lineBotSettings.Value.ChannelSecret;
        }

        public void ReplyMessageHandler<T>(string messageType, ReplyMessageRequestDto<T> requestBody)
        {
            ReplyMessage(requestBody);
        }

        public async void ReplyMessage<T>(ReplyMessageRequestDto<T> request)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken); //帶入 channel access token
            var json = _jsonProvider.Serialize(request);
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(replyMessageUri),
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(requestMessage);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        // 負責處理收到的廣播訊息類行為合併正確反序列化
        public void BroadcastMessageHandler(string messageType, object requestBody)
        {
            string strBody = requestBody.ToString();
            switch (messageType)
            {
                case MessageTypeEnum.Text:
                    var messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<TextMessageDto>>(strBody);
                    BroadcastMessage(messageRequest);
                    break;
            }

        }

        // 負責推播訊息
        public async void BroadcastMessage<T>(BroadcastMessageRequestDto<T> request)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var json = _jsonProvider.Serialize(request);
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(broadcastMessageUri),
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(requestMessage);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        public void ReceiveWebhook(WebhookRequestBodyDto requestBody, List<SpongebobDatum> spongebobDatas)
        {
            foreach (var eventObject in requestBody.Events)
            {
                switch (eventObject.Type)
                {
                    case WebhookEventTypeEnum.Message:
                        switch (eventObject.Message.Type)
                        {
                            case MessageTypeEnum.Text:
                                if (eventObject.Source.Type != "group")
                                {
                                    var matchingTitleData = spongebobDatas.Where(data => data.Title.Contains(eventObject.Message.Text)).ToList();
                                    if (matchingTitleData.Count == 0)
                                    {
                                        var noResult = new ReplyMessageRequestDto<TextMessageDto>()
                                        {
                                            ReplyToken = eventObject.ReplyToken,
                                            Messages = new List<TextMessageDto>
                                        {
                                            new TextMessageDto() { Text = $"神奇海螺找不到關於\"{eventObject.Message.Text}\"的結果！"}
                                        }
                                        };
                                        ReplyMessageHandler("text", noResult);
                                    }
                                    else if (matchingTitleData.Count > 5)
                                    {
                                        var text = new List<TextMessageDto>();
                                        string tempStr = "";
                                        foreach (var data in matchingTitleData)
                                        {
                                            tempStr = $"{tempStr}\n{data.Title}";
                                        }
                                        var manyResult = new ReplyMessageRequestDto<TextMessageDto>()
                                        {
                                            ReplyToken = eventObject.ReplyToken,
                                            Messages = new List<TextMessageDto>
                                        {
                                            new TextMessageDto() { Text = $"神奇海螺找到過多結果：{tempStr}"}
                                        }
                                        };
                                        ReplyMessageHandler("text", manyResult);
                                    }
                                    else
                                    {
                                        var urls = new List<ImageMessageDto>();
                                        foreach (var data in matchingTitleData)
                                        {
                                            urls.Add(new ImageMessageDto() { OriginalContentUrl = data.Url, PreviewImageUrl = data.Url });
                                        }
                                        var replyMessage = new ReplyMessageRequestDto<ImageMessageDto>()
                                        {
                                            ReplyToken = eventObject.ReplyToken,
                                            Messages = urls
                                        };
                                        ReplyMessageHandler("image", replyMessage);
                                    }
                                }
                                else
                                {
                                    string inputString = eventObject.Message.Text;
                                    string prefix = "*=";

                                    if (inputString.StartsWith("*="))
                                    {
                                        string search = inputString.Substring(prefix.Length);
                                        Console.WriteLine("TEST");
                                        var matchingTitleData = spongebobDatas.Where(data => data.Title == search).ToList();
                                        if (matchingTitleData.Count == 0)
                                        {
                                            var noResult = new ReplyMessageRequestDto<TextMessageDto>()
                                            {
                                                ReplyToken = eventObject.ReplyToken,
                                                Messages = new List<TextMessageDto>
                                        {
                                            new TextMessageDto() { Text = $"神奇海螺找不到關於\"{search}\"的結果！"}
                                        }
                                            };
                                            ReplyMessageHandler("text", noResult);
                                        }
                                        else if (matchingTitleData.Count > 5)
                                        {
                                            var text = new List<TextMessageDto>();
                                            string tempStr = "";
                                            foreach (var data in matchingTitleData)
                                            {
                                                tempStr = $"{tempStr}\n{data.Title}";
                                            }
                                            var manyResult = new ReplyMessageRequestDto<TextMessageDto>()
                                            {
                                                ReplyToken = eventObject.ReplyToken,
                                                Messages = new List<TextMessageDto>
                                        {
                                            new TextMessageDto() { Text = $"神奇海螺找到過多結果：{tempStr}"}
                                        }
                                            };
                                            ReplyMessageHandler("text", manyResult);
                                        }
                                        else
                                        {
                                            var urls = new List<ImageMessageDto>();
                                            foreach (var data in matchingTitleData)
                                            {
                                                urls.Add(new ImageMessageDto() { OriginalContentUrl = data.Url, PreviewImageUrl = data.Url });
                                            }
                                            var replyMessage = new ReplyMessageRequestDto<ImageMessageDto>()
                                            {
                                                ReplyToken = eventObject.ReplyToken,
                                                Messages = urls
                                            };
                                            ReplyMessageHandler("image", replyMessage);
                                        }
                                    }
                                    else if (inputString.StartsWith("*"))
                                    {
                                        string search = inputString.Substring(1, inputString.Length - 1);
                                        var matchingTitleData = spongebobDatas.Where(data => data.Title.Contains(search)).ToList();
                                        if (matchingTitleData.Count == 0)
                                        {
                                            var noResult = new ReplyMessageRequestDto<TextMessageDto>()
                                            {
                                                ReplyToken = eventObject.ReplyToken,
                                                Messages = new List<TextMessageDto>
                                        {
                                            new TextMessageDto() { Text = $"神奇海螺找不到關於\"{search}\"的結果！"}
                                        }
                                            };
                                            ReplyMessageHandler("text", noResult);
                                        }
                                        else if (matchingTitleData.Count > 1)
                                        {
                                            var text = new List<TextMessageDto>();
                                            string tempStr = "";
                                            foreach (var data in matchingTitleData)
                                            {
                                                tempStr = $"{tempStr}\n{data.Title}";
                                            }
                                            var manyResult = new ReplyMessageRequestDto<TextMessageDto>()
                                            {
                                                ReplyToken = eventObject.ReplyToken,
                                                Messages = new List<TextMessageDto>
                                        {
                                            new TextMessageDto() { Text = $"神奇海螺找到過多結果：{tempStr}"}
                                        }
                                            };
                                            ReplyMessageHandler("text", manyResult);
                                        }
                                        else
                                        {
                                            var urls = new List<ImageMessageDto>();
                                            foreach (var data in matchingTitleData)
                                            {
                                                urls.Add(new ImageMessageDto() { OriginalContentUrl = data.Url, PreviewImageUrl = data.Url });
                                            }
                                            var replyMessage = new ReplyMessageRequestDto<ImageMessageDto>()
                                            {
                                                ReplyToken = eventObject.ReplyToken,
                                                Messages = urls
                                            };
                                            ReplyMessageHandler("image", replyMessage);
                                        }
                                    }
                                }
                                break;
                            default:
                                var reply = new ReplyMessageRequestDto<TextMessageDto>()
                                {
                                    ReplyToken = eventObject.ReplyToken,
                                    Messages = new List<TextMessageDto>
                                {
                                    new TextMessageDto(){Text = "坐好！"}
                                }
                                };
                                ReplyMessageHandler("text", reply);
                                break;
                        }
                        break;
                    case WebhookEventTypeEnum.Unsend:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}在聊天室收回訊息！");
                        break;
                    case WebhookEventTypeEnum.Follow:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}將我們新增為好友！");
                        break;
                    case WebhookEventTypeEnum.Unfollow:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}封鎖了我們！");
                        break;
                    case WebhookEventTypeEnum.Join:
                        Console.WriteLine("我們被邀請進入聊天室了！");
                        break;
                    case WebhookEventTypeEnum.Leave:
                        Console.WriteLine("我們被聊天室踢出了");
                        break;
                    case WebhookEventTypeEnum.MemberJoined:
                        string joinedMemberIds = "";
                        foreach (var member in eventObject.Joined.Members)
                        {
                            joinedMemberIds += $"{member.UserId} ";
                        }
                        Console.WriteLine($"使用者{joinedMemberIds}加入了群組！");
                        break;
                    case WebhookEventTypeEnum.MemberLeft:
                        string leftMemberIds = "";
                        foreach (var member in eventObject.Left.Members)
                        {
                            leftMemberIds += $"{member.UserId} ";
                        }
                        Console.WriteLine($"使用者{leftMemberIds}離開了群組！");
                        break;
                    case WebhookEventTypeEnum.Postback:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}觸發了postback事件");
                        break;
                    case WebhookEventTypeEnum.VideoPlayComplete:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}");
                        break;
                }
            }
        }
    }
}