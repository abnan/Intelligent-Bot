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
using System.Diagnostics;
using System.Text.RegularExpressions;
using Word = Microsoft.Office.Interop.Word;
using System.Net.Http.Headers;

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
                var attachments = activity?.Attachments?
                    .Where(attachment => attachment.ContentUrl != null)
                    .Select(c => Tuple.Create(c.ContentType, c.ContentUrl));
                reply = activity.CreateReply("Working on it.");
                await connector.Conversations.ReplyToActivityAsync(reply);
                StateClient stateClient = activity.GetStateClient();
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                string search = null, search1 = null, searchingfor = "", search3 = null;
                List<string> searchContent = new List<string>();
                List<string> contentpages = new List<string>();
                List<string> indexpages = new List<string>();

                if (activity.Text != null)
                {
                    search = findSearchString("tell me about", activity.Text.ToLowerInvariant());
                    search1 = findSearchString("source:", activity.Text);
                    search3 = findSearchString("feedback:", activity.Text.ToLowerInvariant());
                }
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
                            userData.SetProperty<string>("ext", "HTML");
                        }
                    }
                }
                else if (search1 != null)
                {
                    string url = search1;
                    var ext = System.IO.Path.GetExtension(url);
                    if (((ext == null || ext == "") && (url.StartsWith("http") || url.StartsWith("https"))) || (ext == ".html") || (ext == ".htm"))
                    {
                        userData.SetProperty<string>("URL", url);
                        userData.SetProperty<string>("ext", "HTML");
                    }
                    if (ext == ".pdf")
                    {
                        var client = new WebClient();
                        Random rnd = new Random();
                        string filename = rnd.Next(1, 50).ToString();
                        var savepath = HttpContext.Current.Server.MapPath(".") + @"\..\Data\Books\";
                        client.DownloadFile(url, savepath + filename + ".pdf");
                        ProcessStartInfo ProcessInfo;
                        Process Process;

                        ProcessInfo = new ProcessStartInfo("cmd.exe", "/K " + "pdftohtml.exe " + filename + ".pdf");
                        ProcessInfo.CreateNoWindow = false;
                        ProcessInfo.UseShellExecute = false;
                        ProcessInfo.WorkingDirectory = savepath;

                        Process = Process.Start(ProcessInfo);
                        userData.SetProperty<string>("ext", "HTMLs");
                        userData.SetProperty<string>("filename", savepath + filename + "s.html");
                    }
                    reply = activity.CreateReply("Context set");
                }
                else if (activity.Attachments != null && activity.Attachments.Count > 0)
                {
                    string url = activity.Attachments[0].ContentUrl;
                    var client = new WebClient();
                    Random rnd = new Random();
                    string filename = rnd.Next(1, 50).ToString();
                    var savepath = HttpContext.Current.Server.MapPath(".") + @"\..\Data\Books\";
                    client.DownloadFile(url, savepath + filename + ".pdf");
                    ProcessStartInfo ProcessInfo;
                    Process Process;

                    ProcessInfo = new ProcessStartInfo("cmd.exe", "/K " + "pdftohtml.exe " + filename + ".pdf");
                    ProcessInfo.CreateNoWindow = false;
                    ProcessInfo.UseShellExecute = false;
                    ProcessInfo.WorkingDirectory = savepath;

                    Process = Process.Start(ProcessInfo);
                    userData.SetProperty<string>("ext", "HTMLs");
                    userData.SetProperty<string>("filename", savepath + filename + "s.html");
                    reply = activity.CreateReply("Context set");
                }
                else if (search3 != null)
                {
                    reply = activity.CreateReply("Thank you for your feedback");
                }
                else if ((userData.GetProperty<string>("ext") == "HTML") || (userData.GetProperty<string>("ext") == "HTMLs"))
                {
                    if (userData.GetProperty<string>("ext") == "HTML")
                    {
                        string url = userData.GetProperty<string>("URL");
                        HtmlWeb web = new HtmlWeb();
                        HtmlDocument doc = web.Load(url);
                        HtmlNodeCollection contents = doc.DocumentNode.SelectNodes("//p");
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
                            var paraTemp = contents[i].InnerText.Replace("Mr.", "Mr").Replace("Mrs.", "Mrs").Replace("Dr.", "Dr").Replace("St.", "St");
                            var temp = paraTemp.Split('.').ToList();
                            searchContent.AddRange(temp);
                        }
                    }
                    else if (userData.GetProperty<string>("ext") == "HTMLs")
                    {
                        //string filename = userData.GetProperty<string>("filename");
                        //HtmlWeb web = new HtmlWeb();
                        //HtmlDocument doc = new HtmlDocument();
                        //doc.Load(filename);
                        //HtmlNodeCollection contents = doc.DocumentNode.SelectNodes("//text()");
                        //List<string> newlist = new List<string>();
                        //foreach (var content in contents)
                        //    newlist.Add(Regex.Replace(content.InnerText, "<.*?>", String.Empty));
                        //for (int i = 0; i < contents.Count; i++)
                        //{
                        //    var paraTemp = contents[i].InnerText.Replace("Mr.", "Mr").Replace("Mrs.", "Mrs").Replace("Dr.", "Dr").Replace("St.", "St");
                        //    var temp = paraTemp.Split('.').ToList();
                        //    searchContent.AddRange(temp);
                        //}
                        string filename = userData.GetProperty<string>("filename");
                        HtmlWeb web = new HtmlWeb();
                        HtmlDocument doc = new HtmlDocument();
                        doc.Load(filename);
                        bool skip = false;
                        string contents = doc.DocumentNode.InnerHtml.ToString();
                        var pages = Regex.Split(contents, "<a name");
                        foreach (var page in pages)
                        {
                            if (page.Contains("</b><br><b>"))
                            { }
                            if (page.Contains("<b>Contents"))
                            {
                                contentpages.Add(page);
                            }
                            else if (page.Contains("<b>Index</b>"))
                            {
                                skip = true;
                                indexpages.Add(page);
                            }
                            else if (skip == true)
                            {
                                indexpages.Add(page);
                            }
                            else
                            {
                                searchContent.AddRange(page.Split('.').ToList());
                            }
                        }
                    }
                    else
                    {
                        string url = userData.GetProperty<string>("URL");
                        //using (PdfReader reader = new PdfReader(url))
                        //{
                        //    for (int i = 1; i <= reader.NumberOfPages; i++)
                        //    {
                        //        searchContent.AddRange(PdfTextExtractor.GetTextFromPage(reader, i).ToString().Split('.').ToList());
                        //    }
                        //}
                    }
                    if (searchingfor == "")
                        searchingfor = activity.Text;
                    Dictionary<string, int> dict = paraCompare(connector, reply, searchingfor, searchContent);
                    int numOfResults = 5;
                    var abc = dict.OrderByDescending(x => x.Value).Where(x => x.Value > 0).Take(numOfResults).Select(x => x.Key).ToList();
                    string replymsg = "";
                    if (abc.Count > 0)
                    {
                        for (int i = 0; i < Math.Min(numOfResults, abc.Count); i++)
                        {
                            int index = searchContent.IndexOf(abc[i]);
                            if (Regex.Matches(abc[i], "<b>").Count > 1)
                                continue;
                            replymsg = replymsg + "\n" + (i+1).ToString() + ". " + $"{HttpUtility.HtmlDecode(searchContent[index].ToString())}" + "." + $"{HttpUtility.HtmlDecode(searchContent[index +1].ToString())}" + "." + $"{HttpUtility.HtmlDecode(searchContent[index + 2].ToString())}" + "." + $"{Environment.NewLine}";
                        }
                    }
                    else
                    {
                        replymsg = "No relevant results found!";
                    }

                    //replymsg = $"1. {abc[0]} : **{dict[abc[0]]}**{Environment.NewLine}2. {abc[1]} : **{dict[abc[1]]}**{Environment.NewLine}3. {abc[2]} : **{dict[abc[2]]}**{Environment.NewLine}";
                    reply = activity.CreateReply(replymsg);

                    //await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
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
        public static int dictionaryCompare(List<string> resSyn, List<string> resHypernym, List<string> resHyponym, string compareTo, Dictionary<string, List<string>> synDict, Dictionary<string, List<string>> hypernymDict, Dictionary<string, List<string>> hyponymDict, string activityText)
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
            int retVal = 0;

            if (Regex.Matches(compareTo, "<b>").Count > 1)
                retVal = 0;
            else
                retVal = (resSyn.Intersect(resSyn2).Count() * 20
                + resSyn.Intersect(resHyponym2).Count() * 5
                + resHyponym.Intersect(resSyn2).Count() * 5
                + resHyponym.Intersect(resHyponym2).Count() * 40
                + resSyn.Intersect(resHypernym2).Count() * 4
                + resHypernym.Intersect(resHypernym2).Count() * 1
                + compareTo.Intersect(activityText).Count() * 10);

            if (Regex.Matches(compareTo, "<b>").Count > 1)
                retVal = retVal * 2;
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
                int a = dictionaryCompare(resSyn1, resHypernym1, resHyponym1, s, synDict, hypernymDict, hyponymDict, activityText);
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
                string RequestURI = string.Format("https://en.wikipedia.org/w/api.php?action=opensearch&search={0}&limit=2&namespace=0&format=xml", Query2);
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