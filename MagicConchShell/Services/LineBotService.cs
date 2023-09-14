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
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
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
                        string input = eventObject.Message.Text;
                        switch (eventObject.Message.Type)
                        {
                            case MessageTypeEnum.Text:
                                if (eventObject.Source.Type != "group")
                                {
                                    if (input.StartsWith("="))
                                    {
                                        string search = input.Substring(1, input.Length - 1);
                                        var matchingTitleData = spongebobDatas.Where(data => data.Title == search).ToList();
                                        if (matchingTitleData.Count == 0)
                                        {
                                            TextReply(eventObject, $"神奇海螺找不到\"{search}\"！");
                                        }
                                        else if (matchingTitleData.Count > 5)
                                        {
                                            var text = new List<TextMessageDto>();
                                            string tempStr = "";
                                            foreach (var data in matchingTitleData)
                                            {
                                                tempStr = $"{tempStr}\n{data.Title}";
                                            }
                                            TextReply(eventObject, $"神奇海螺找到過多結果：{tempStr}");
                                        }
                                        else
                                        {
                                            ImageReply(eventObject, matchingTitleData, true);
                                        }
                                    }
                                    else
                                    {
                                        var matchingTitleData = spongebobDatas.Where(data => data.Title.Contains(eventObject.Message.Text)).ToList();
                                        if (matchingTitleData.Count == 0)
                                        {
                                            TextReply(eventObject, $"神奇海螺找不到關於\"{eventObject.Message.Text}\"的結果！");
                                        }
                                        else if (matchingTitleData.Count > 1)
                                        {
                                            var text = new List<TextMessageDto>();
                                            string tempStr = "";
                                            foreach (var data in matchingTitleData)
                                            {
                                                tempStr = $"{tempStr}\n{data.Title}";
                                            }
                                            TextReply(eventObject, $"神奇海螺找到過多結果：{tempStr}");
                                        }
                                        else
                                        {
                                            ImageReply(eventObject, matchingTitleData);
                                        }
                                    }
                                }
                                else
                                {
                                    string prefix = "*=";
                                    if (input.StartsWith(prefix))
                                    {
                                        string search = input.Substring(prefix.Length);
                                        var matchingTitleData = spongebobDatas.Where(data => data.Title == search).ToList();
                                        if (matchingTitleData.Count == 0)
                                        {
                                            TextReply(eventObject, $"神奇海螺找不到關於\"{search}\"的結果！");
                                        }
                                        else if (matchingTitleData.Count > 5)
                                        {
                                            var text = new List<TextMessageDto>();
                                            string tempStr = "";
                                            foreach (var data in matchingTitleData)
                                            {
                                                tempStr = $"{tempStr}\n{data.Title}";
                                            }
                                            TextReply(eventObject, $"神奇海螺找到過多結果：{tempStr}");
                                        }
                                        else
                                        {
                                            ImageReply(eventObject, matchingTitleData, true);
                                        }
                                    }
                                    else if (input.StartsWith("*"))
                                    {
                                        string search = input.Substring(1, input.Length - 1);
                                        var matchingTitleData = spongebobDatas.Where(data => data.Title.Contains(search)).ToList();
                                        Console.WriteLine(matchingTitleData);
                                        if (matchingTitleData.Count == 0)
                                        {
                                            TextReply(eventObject, $"神奇海螺找不到關於\"{search}\"的結果！");
                                        }
                                        else if (matchingTitleData.Count > 1)
                                        {
                                            var text = new List<TextMessageDto>();
                                            string tempStr = "";
                                            foreach (var data in matchingTitleData)
                                            {
                                                tempStr = $"{tempStr}\n{data.Title}";
                                            }
                                            Console.WriteLine(tempStr);
                                            TextReply(eventObject, $"神奇海螺找到過多結果：{tempStr}");
                                        }
                                        else
                                        {
                                            ImageReply(eventObject, matchingTitleData);
                                        }
                                    }
                                }
                                break;
                            default:
                                TextReply(eventObject, "坐好！");
                                break;
                        }
                        break;
                    case WebhookEventTypeEnum.Join:
                        var state = new ReplyMessageRequestDto<TextMessageDto>()
                        {
                            ReplyToken = eventObject.ReplyToken,
                            Messages = new List<TextMessageDto>
                                {
                                    new TextMessageDto(){Text = "加入群組的神奇小海螺需在關鍵字前面加上星字號 (ex: *好棒三點了)"}
                                }
                        };
                        ReplyMessageHandler("text", state);
                        break;
                }
            }
        }

        private void ImageReply(LineBotMessage.Dtos.WebhookEventsDto eventObject, List<SpongebobDatum> matchingTitleData, bool isExact = false)
        {
            var urls = new List<ImageMessageDto>();
            foreach (var data in matchingTitleData)
            {
                urls.Add(new ImageMessageDto() { OriginalContentUrl = data.Url, PreviewImageUrl = data.Url });
            }

            if (isExact)
            {
                Random random = new Random();
                int randomIndex = random.Next(urls.Count);
                ImageMessageDto randomElement = urls[randomIndex];
                urls.Clear();
                urls.Add(randomElement);
            }

            var replyMessage = new ReplyMessageRequestDto<ImageMessageDto>()
            {
                ReplyToken = eventObject.ReplyToken,
                Messages = urls
            };
            ReplyMessageHandler("image", replyMessage);
        }

        private void TextReply(LineBotMessage.Dtos.WebhookEventsDto eventObject, string textMessage)
        {
            var Reply = new ReplyMessageRequestDto<TextMessageDto>()
            {
                ReplyToken = eventObject.ReplyToken,
                Messages = new List<TextMessageDto>
                {
                    new TextMessageDto() { Text = textMessage}
                }
            };
            ReplyMessageHandler("text", Reply);
        }
    }
}