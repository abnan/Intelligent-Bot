using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using depolybot;

namespace deploybot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                Activity reply = activity.CreateReply();
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                StateClient stateClient = activity.GetStateClient();
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                var search = findSearchString("tell me about", activity.Text.ToLowerInvariant());
                if (search != null)
                {
                    SearchSuggestion searchRes = await GetWikiResultAsync(search);
                    var url = searchRes.Section.Item.Url;
                    if (searchRes.Query == null)
                    {
                        reply = activity.CreateReply("Couldn't find article!");
                    }
                    else
                    {
                        if (searchRes.Section.Item.Image == null)
                        {
                            reply = activity.CreateReply($"Sorry, there seems to be more than one article about this topic. We will add support for this soon in the future!");
                        }
                        else
                        {
                            //reply = activity.CreateReply($"![Alt]({searchRes.Section.Item.Image.source})<br/>" + searchRes.Section.Item.Description.Value);
                            reply = activity.CreateReply();
                            reply.Attachments = new List<Attachment>();
                            List<CardImage> cardImages = new List<CardImage>();
                            cardImages.Add(new CardImage(url: $"{searchRes.Section.Item.Image.source}"));
                            List<CardAction> cardButtons = new List<CardAction>();
                            CardAction plButton = new CardAction()
                            {
                                Value = url.Value,
                                Type = "openUrl",
                                Title = "Read more on Wikipedia"
                            };
                            cardButtons.Add(plButton);
                            ThumbnailCard plCard = new ThumbnailCard()
                            {
                                Title = searchRes.Section.Item.Text.Value,
                                Text = searchRes.Section.Item.Description.Value,
                                Images = cardImages,
                                Buttons = cardButtons
                            };
                            Attachment plAttachment = plCard.ToAttachment();
                            reply.Attachments.Add(plAttachment);
                            userData.SetProperty<string>("URL", url.Value);
                        }
                    }
                }
                else
                {
                    string url = userData.GetProperty<string>("URL");
                    HtmlWeb web = new HtmlWeb();
                    HtmlDocument doc = web.Load(url);
                    HtmlNodeCollection contents = doc.DocumentNode.SelectNodes("//p");
                    List<string> searchContent = new List<string>();
                    int s_index = 0, e_index = 0;
                    foreach (HtmlNode content in contents)
                    {
                        s_index = content.InnerHtml.ToString().IndexOf("<sup");
                        while (s_index != -1)
                        {
                            e_index = content.InnerHtml.ToString().IndexOf("</sup>");
                            content.InnerHtml = content.InnerHtml.Replace(content.InnerHtml.Substring(s_index, e_index - s_index + 6), "");
                            s_index = content.InnerHtml.ToString().IndexOf("<sup");
                        }
                        s_index = 0;
                    }
                    for (int i = 0; i < contents.Count; i++)
                    {
                        var paraTemp = contents[i].InnerText.Replace("Mr.", "Mr").Replace("Mrs.", "Mrs").Replace("Dr.","Dr").Replace("St.","St");
                        var temp = paraTemp.Split('.').ToList();
                        searchContent.AddRange(temp);
                    }
                    Dictionary<string, int> dict = paraCompare(connector, reply, activity.Text, searchContent);
                    int numOfResults = 5;
                    var abc = dict.OrderByDescending(x => x.Value).Where(x => x.Value > 0).Take(numOfResults).Select(x => x.Key).ToList();
                    string replymsg = "";
                    if(abc.Count>0)
                    {
                        for (int i = 0; i < Math.Min(numOfResults, abc.Count); i++)
                        {
                            replymsg += $"{i + 1}. {HttpUtility.HtmlDecode(abc[i]).ToString()} : **{dict[abc[i]]}**{Environment.NewLine}";
                        }
                    }
                    else
                    {
                        replymsg = "No relevant results found!";
                    }
                    
                    //replymsg = $"1. {abc[0]} : **{dict[abc[0]]}**{Environment.NewLine}2. {abc[1]} : **{dict[abc[1]]}**{Environment.NewLine}3. {abc[2]} : **{dict[abc[2]]}**{Environment.NewLine}";
                    reply = activity.CreateReply(replymsg);
                    
                }

                await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
        private string findSearchString(string template, string userInput)
        {
            if (userInput.Contains(template))
            {
                return userInput.Substring(userInput.IndexOf(template) + template.Length + 1).Trim();
            }
            else return null;
        }
        public static int dictionaryCompare(List<string> resSyn, List<string> resHypernym, List<string> resHyponym, string compareTo, Dictionary<string, List<string>> synDict, Dictionary<string, List<string>> hypernymDict, Dictionary<string, List<string>> hyponymDict)
        {
            compareTo = StopwordTool.RemoveStopwords(compareTo);
            List<string> resSyn2 = new List<string>();
            List<string> resHyponym2 = new List<string>();
            List<string> resHypernym2 = new List<string>();
            foreach (string s in compareTo.Split(' '))
            {
                List<string> temp = null;
                List<string> temp2 = null;
                List<string> temp3 = null;
                if (synDict.ContainsKey(s.ToLowerInvariant()))
                {
                    temp = synDict[s.ToLowerInvariant()];
                    List<string> _temp = new List<string>();
                    foreach(var a in temp)
                    {
                        if(a.Contains("_"))
                        {
                            _temp.AddRange(StopwordTool.RemoveStopwords(a.Replace("_", " ")).Split(' ').ToList().ConvertAll(d => d.ToLower()));
                        }
                        else
                        {
                            _temp.Add(a);
                        }
                    }
                    temp = _temp.Distinct().ToList();
                }
                if (hyponymDict.ContainsKey(s.ToLowerInvariant()))
                {
                    temp2 = hyponymDict[s.ToLowerInvariant()];
                    List<string> _temp2 = new List<string>();
                    foreach (var a in temp2)
                    {
                        if (a.Contains("_"))
                        {
                            _temp2.AddRange(StopwordTool.RemoveStopwords(a.Replace("_", " ")).Split(' ').ToList().ConvertAll(d => d.ToLower()));
                        }
                        else
                        {
                            _temp2.Add(a);
                        }
                    }
                    temp2 = _temp2.Distinct().ToList();
                }
                if (hypernymDict.ContainsKey(s.ToLowerInvariant()))
                {
                    temp3 = hypernymDict[s.ToLowerInvariant()];
                    List<string> _temp3 = new List<string>();
                    foreach (var a in temp3)
                    {
                        if (a.Contains("_"))
                        {
                            _temp3.AddRange(StopwordTool.RemoveStopwords(a.Replace("_", " ")).Split(' ').ToList().ConvertAll(d => d.ToLower()));
                        }
                        else
                        {
                            _temp3.Add(a);
                        }
                    }
                    temp3 = _temp3.Distinct().ToList();
                }
                if (temp != null) resSyn2.AddRange(temp);
                if (temp2 != null) resHyponym2.AddRange(temp2);
                if (temp3 != null) resHypernym2.AddRange(temp3);
            }

            int retVal = (resSyn.Intersect(resSyn2).Count() * 20
                + resSyn.Intersect(resHyponym2).Count() * 5
                + resHyponym.Intersect(resSyn2).Count() * 5
                + resHyponym.Intersect(resHyponym2).Count() * 40
                + resSyn.Intersect(resHypernym2).Count() * 4
                + resHypernym.Intersect(resHypernym2).Count() * 1);
            return retVal;
        }


        private static Dictionary<string, int> paraCompare(ConnectorClient connector, Activity reply, string activityText, List<string> searchContext)
        {
            List<string> resSyn1 = new List<string>();
            List<string> resHypernym1 = new List<string>();
            List<string> resHyponym1 = new List<string>();
            var synDict = (Dictionary<string, List<string>>)HttpContext.Current.Application["synDict"];
            var hypernymDict = (Dictionary<string, List<string>>)HttpContext.Current.Application["hypernymDict"];
            var hyponymDict = (Dictionary<string, List<string>>)HttpContext.Current.Application["hyponymDict"];
            foreach (string s in (StopwordTool.RemoveStopwords(activityText)).Split(' '))
            {
                List<string> temp = null;
                List<string> temp2 = null;
                List<string> temp3 = null;
                if (synDict.ContainsKey(s.ToLowerInvariant()))
                {
                    temp = synDict[s.ToLowerInvariant()];
                    List<string> _temp = new List<string>();
                    foreach (var a in temp)
                    {
                        if (a.Contains("_"))
                        {
                            _temp.AddRange(StopwordTool.RemoveStopwords(a.Replace("_", " ")).Split(' ').ToList().ConvertAll(d => d.ToLower()));
                        }
                        else
                        {
                            _temp.Add(a);
                        }
                    }
                    temp = _temp.Distinct().ToList();
                }
                if (hyponymDict.ContainsKey(s.ToLowerInvariant()))
                {
                    temp2 = hyponymDict[s.ToLowerInvariant()];
                    List<string> _temp2 = new List<string>();
                    foreach (var a in temp2)
                    {
                        if (a.Contains("_"))
                        {
                            _temp2.AddRange(StopwordTool.RemoveStopwords(a.Replace("_", " ")).Split(' ').ToList().ConvertAll(d => d.ToLower()));
                        }
                        else
                        {
                            _temp2.Add(a);
                        }
                    }
                    temp2 = _temp2.Distinct().ToList();
                }
                if (hypernymDict.ContainsKey(s.ToLowerInvariant()))
                {
                    temp3 = hypernymDict[s.ToLowerInvariant()];
                    List<string> _temp3 = new List<string>();
                    foreach (var a in temp3)
                    {
                        if (a.Contains("_"))
                        {
                            _temp3.AddRange(StopwordTool.RemoveStopwords(a.Replace("_", " ")).Split(' ').ToList().ConvertAll(d => d.ToLower()));
                        }
                        else
                        {
                            _temp3.Add(a);
                        }
                    }
                    temp3 = _temp3.Distinct().ToList();
                }
                if (temp != null) resSyn1.AddRange(temp);
                if (temp2 != null) resHyponym1.AddRange(temp2);
                if (temp3 != null) resHypernym1.AddRange(temp3);
            }
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (string s in searchContext)
            {
                int a = dictionaryCompare(resSyn1, resHypernym1, resHyponym1, s, synDict, hypernymDict, hyponymDict);
                dict[s] = a;
            }
            return dict;
        }
        private static async Task<SearchSuggestion> GetWikiResultAsync(string conetxt)
        {
            SearchSuggestion sData = new SearchSuggestion();
            string Query2 = Uri.EscapeDataString(conetxt);

            using (HttpClient client = new HttpClient())
            {
                string RequestURI = string.Format("https://en.wikipedia.org/w/api.php?action=opensearch&search={0}&limit=1&namespace=0&format=xml", Query2);
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var xmlDataResponse = await msg.Content.ReadAsStringAsync();
                    var serializer = new XmlSerializer(typeof(SearchSuggestion));
                    using (TextReader reader = new StringReader(xmlDataResponse))
                    {
                        sData = (SearchSuggestion)serializer.Deserialize(reader);
                    }
                }
            }
            return sData;
        }
        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}