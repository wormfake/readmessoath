using Leaf.xNet;
using Newtonsoft.Json;
using SimpleHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;


namespace Read_Hotmail_Outlook
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private static List<DataGMail> AllData = new List<DataGMail>();
        private static bool Wait = true;
        private static object LockGet1 = new object();
        private static object LockSetMail = new object();
        private void Form1_Load(object sender, EventArgs e)
        {
            //ReadMail readMail = new ReadMail();
            //readMail.StartReadMail("chinhvipbl@gmail.com", "ecwzxbbluubeclis");
            Route.Add("/", (req, res, props) =>
            {
                res.AsText("Connected", "text/plain");
            });
            //Route.Add("/getcode/{data}", (req, res, props) =>
            //{
            //    string data = $"{props["data"]}";
            //    lock (LockGet1)
            //    {
            //        try
            //        {
            //            if (!data.Contains("=") || !data.ToLower().Contains("@gmail.com"))
            //            {
            //                res.AsText("Data error");
            //                return;
            //            }
            //            string[] string0 = data.Split('=');
            //            if (string0.Length < 2 || string0[1].Length < 6 || !string0[0].ToLower().Contains("@gmail.com"))
            //            {
            //                res.AsText("Data error", "text/plain");
            //                return;
            //            }
            //            string0[0] = string0[0].ToLower();
            //            bool flag = false;
            //            foreach (var item in AllData)
            //            {
            //                if (item.Mail == string0[0])
            //                {
            //                    flag = true;
            //                    break;
            //                }
            //            }
            //            if (!flag)
            //            {
            //                lock (LockSetMail)
            //                {
            //                    AllData.Add(new DataGMail { Mail = string0[0], Pass = string0[1], Code = "" });
            //                    Task.Run(() =>
            //                    {
            //                        string mail = string0[0];
            //                        string pass = string0[1];
            //                        Wait = false;
            //                        ReadMail readMail = new ReadMail();
            //                        string code = readMail.StartReadMail(mail, pass);
            //                        if (code == null)
            //                        {
            //                            code = "false";
            //                        }
            //                        else if (code == "")
            //                        {
            //                            code = "exit";
            //                        }
            //                        lock (LockSetMail)
            //                        {
            //                            for (int i = 0; i < AllData.Count; i++)
            //                            {
            //                                if (AllData[i].Mail == mail)
            //                                {
            //                                    AllData[i].Code = code;
            //                                    break;
            //                                }
            //                            }
            //                        }
            //                    });
            //                    while (Wait)
            //                    {
            //                        Task.Delay(10).Wait();
            //                    }
            //                    Wait = true;
            //                    res.AsText("ok", "text/plain");
            //                }
            //            }
            //            else
            //            {
            //                lock (LockSetMail)
            //                {
            //                    for (int i = 0; i < AllData.Count; i++)
            //                    {
            //                        if (AllData[i].Mail == string0[0])
            //                        {
            //                            if (AllData[i].Code == "")
            //                            {
            //                                res.AsText("wait", "text/plain");
            //                                break;
            //                            }
            //                            else
            //                            {
            //                                res.AsText(AllData[i].Code);
            //                                AllData.RemoveRange(i, 1);
            //                                break;
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        catch
            //        {
            //            res.AsText("Error", "text/plain");
            //        }
            //    }
            //});
            Route.Add("/getmail/{data}", (req, res, props) =>
            {
                string dt = $"{props["data"]}";
                lock (LockGet1)
                {
                    try
                    {
                        if (dt == null || dt == "")
                        {
                            res.AsText("error data", "text/plain");
                            return;
                        }
                        string data = WebUtility.UrlDecode(dt);
                        if (!data.StartsWith("data?"))
                        {
                            res.AsText("error url", "text/plain");
                            return;
                        }
                        data = data.Substring(5, data.Length - 5);
                        if (!data.Contains("|"))
                        {
                            res.AsText("error data", "text/plain");
                            return;
                        }
                        if (!data.ToLower().Contains("@outlook.") && !data.ToLower().Contains("@hotmail."))
                        {
                            res.AsText("error data", "text/plain");
                            return;
                        }
                        string[] string0 = data.Split('|');
                        string textfind = string0[0];
                        if (string0.Length < 4 || string0[4].Length != 36)
                        {
                            res.AsText("error data", "text/plain");
                            return;
                        }
                        if (!string0[1].ToLower().Contains("@outlook.") && !string0[1].ToLower().Contains("@hotmail."))
                        {
                            res.AsText("error data", "text/plain");
                            return;
                        }
                        bool checkLive = false;
                        if (textfind == "checkmaillive")
                        {
                            checkLive = true;
                        }
                        string Client_ID = string0[4];
                        string refresh_token = string0[3];
                        string access_token = "";
                        if (string0.Length > 5 && string0[5].Length > 500)
                        {
                            if (string0[5].Contains("+") || string0[5].Contains(" "))
                            {
                                access_token = string0[5].Replace(" ",  "+");
                            }
                        }
                        using (HttpRequest request = new HttpRequest())
                        {
                            request.IgnoreProtocolErrors = true;
                            request.AllowAutoRedirect = false;
                            request.ConnectTimeout = 10000;
                            #region Proxy_
                            //if (Proxy != null && Proxy.Contains(":"))
                            //{
                            //    string[] px = Proxy.Split(':');
                            //    if (HttpProxy)
                            //    {
                            //        if (px.Length == 4)
                            //        {
                            //            request.Proxy = HttpProxyClient.Parse($"{px[0]}:{px[1]}");
                            //            request.Proxy.Username = px[2];
                            //            request.Proxy.Password = px[3];
                            //        }
                            //        else
                            //        {
                            //            request.Proxy = HttpProxyClient.Parse($"{px[0]}:{px[1]}");
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (px.Length == 4)
                            //        {
                            //            request.Proxy = Socks5ProxyClient.Parse($"{px[0]}:{px[1]}");
                            //            request.Proxy.Username = px[2];
                            //            request.Proxy.Password = px[3];
                            //        }
                            //        else
                            //        {
                            //            request.Proxy = Socks5ProxyClient.Parse($"{px[0]}:{px[1]}");
                            //        }
                            //    }
                            //}
                            #endregion
                            bool Layaccess_token = false;
                            bool DALayACtoken = false;
                            for (int i = 0; i < 3; i++)
                            {
                                if (Layaccess_token)
                                {
                                    DALayACtoken = true;
                                    Layaccess_token = false;
                                    request.ClearAllHeaders();
                                    string postData = $"client_id={Client_ID}" +
                                        $"&refresh_token={refresh_token}" +
                                        $"&grant_type=refresh_token";
                                    string response = request.Post("https://login.microsoftonline.com/common/oauth2/v2.0/token", postData, "application/x-www-form-urlencoded").ToString();
                                    if (response.Contains("authenticated as the grant is expired"))
                                    {
                                        res.AsText("new refresh_token", "text/plain");
                                        return;
                                    }
                                    else if (response.Contains("User account is found to be in service abuse mode"))
                                    {
                                        res.AsText("die", "text/plain");
                                        return;
                                        throw new Exception("die");
                                    }
                                    else if (response.Contains(" Cross-origin ") || response.Contains("\"error_description\""))
                                    {
                                        request.ClearAllHeaders();
                                        request.AddHeader("Origin", "https://developer.microsoft.com");
                                        response = request.Post("https://login.microsoftonline.com/common/oauth2/v2.0/token", postData, "application/x-www-form-urlencoded").ToString();
                                    }
                                    if (response.Contains("authenticated as the grant is expired"))
                                    {
                                        res.AsText("new refresh_token", "text/plain");
                                        return;
                                    }
                                    dynamic json = JsonConvert.DeserializeObject(response);
                                    access_token = json.access_token;
                                    if (access_token != null && access_token.Length > 12)
                                    {
                                        if (checkLive)
                                        {
                                            res.AsText("live", "text/plain");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Không lấy được access token!");
                                    }
                                }
                                if (access_token != "")
                                {
                                    request.ClearAllHeaders();
                                    request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36";
                                    request.Authorization = "Bearer " + access_token;
                                    string response = request.Get("https://graph.microsoft.com/v1.0/me/messages").ToString();
                                    if (response.Contains("InvalidAuthenticationToken") && DALayACtoken)
                                    {
                                        throw new Exception("Access token không đủ quyền hoặc hết hạn!");
                                    }
                                    else if (response.Contains("\"InvalidAuthenticationToken\""))
                                    {
                                        Layaccess_token = true;
                                        continue;
                                    }
                                    else if (response.Contains("\"@odata.context\""))
                                    {
                                        if (checkLive)
                                        {
                                            res.AsText("live", "text/plain");
                                            return;
                                        }
                                        DataGraphApi mail = JsonConvert.DeserializeObject<DataGraphApi>(response);
                                        if (mail != null)
                                        {
                                            if (mail.my != null && mail.value != null && mail.value.Count > 0)
                                            {
                                                int Dem = 0;
                                                foreach (var item in mail.value)
                                                {
                                                    if (textfind == "")
                                                    {
                                                        HtmlDocument doc = new HtmlDocument();
                                                        doc.LoadHtml(item.body.content);
                                                        string plainText = doc.DocumentNode.InnerText;
                                                        //var normalizedString = plainText.Normalize(NormalizationForm.FormD);
                                                        //var stringBuilder = new StringBuilder();
                                                        //foreach (var c in normalizedString)
                                                        //{
                                                        //    var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                                                        //    if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                                                        //    {
                                                        //        stringBuilder.Append(c);
                                                        //    }
                                                        //}
                                                        //string text = stringBuilder.ToString().Normalize(NormalizationForm.FormC);
                                                        Regex extractUrlRegex = new Regex("https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)");
                                                        string[] allurl = extractUrlRegex.Matches(item.body.content.Replace("&amp;", "&")).Cast<Match>().Select(m => m.Value).ToArray();
                                                        if (allurl != null && allurl.Length > 0)
                                                        {
                                                            plainText += $"|{string.Join("|", allurl)}";
                                                        }
                                                        res.AsBytes(req, Encoding.UTF8.GetBytes(plainText), "text/plain; charset=UTF-8;");
                                                        //res.AsText(text, "text/plain");
                                                        return;
                                                    }
                                                    else if (item.body.content.Contains(textfind))
                                                    {
                                                        HtmlDocument doc = new HtmlDocument();
                                                        doc.LoadHtml(item.body.content);
                                                        string plainText = doc.DocumentNode.InnerText;
                                                        //var normalizedString = plainText.Normalize(NormalizationForm.FormD);
                                                        //var stringBuilder = new StringBuilder();
                                                        //foreach (var c in normalizedString)
                                                        //{
                                                        //    var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                                                        //    if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                                                        //    {
                                                        //        stringBuilder.Append(c);
                                                        //    }
                                                        //}
                                                        //string text = stringBuilder.ToString().Normalize(NormalizationForm.FormC);
                                                        Regex extractUrlRegex = new Regex("https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)");
                                                        string[] allurl = extractUrlRegex.Matches(item.body.content.Replace("&amp;", "&")).Cast<Match>().Select(m => m.Value).ToArray();
                                                        if (allurl != null && allurl.Length > 0)
                                                        {
                                                            plainText += $"|{string.Join("|", allurl)}";
                                                        }
                                                        res.AsBytes(req, Encoding.UTF8.GetBytes(plainText), "text/plain; charset=UTF-8;");
                                                        //res.AsText(text, "text/plain");
                                                        return;
                                                    }
                                                    if (Dem >= 50)
                                                    {
                                                        break;
                                                    }
                                                    Dem++;
                                                }
                                            }
                                            if (checkLive)
                                            {
                                                res.AsText("check fail", "text/plain");
                                                return;
                                            }
                                            else
                                            {
                                                res.AsText("no mail", "text/plain");
                                                return;
                                            }
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        throw new Exception("Lỗi không xác định: " + response);
                                    }
                                }
                                else
                                {
                                    Layaccess_token = true;
                                    continue;
                                }
                            }
                            if (checkLive)
                            {
                                res.AsText("check fail", "text/plain");
                                return;
                            }
                            else
                            {
                                res.AsText("no mail", "text/plain");
                                return;
                            }
                        }
                    }
                    catch
                    {
                        res.AsText("error", "text/plain");
                        return;
                    }
                }
            });
            Route.Add("/getmailimap/{data}", (req, res, props) =>
            {
                string dt = $"{props["data"]}";
                lock (LockGet1)
                {
                    try
                    {
                        if (dt == null || dt == "")
                        {
                            res.AsText("error data", "text/plain");
                            return;
                        }
                        string data = WebUtility.UrlDecode(dt);
                        if (!data.StartsWith("data?"))
                        {
                            res.AsText("error url", "text/plain");
                            return;
                        }
                        data = data.Substring(5, data.Length - 5);
                        if (!data.Contains("|"))
                        {
                            res.AsText("error data", "text/plain");
                            return;
                        }
                        if (!data.ToLower().Contains("@outlook.") && !data.ToLower().Contains("@hotmail."))
                        {
                            res.AsText("error data", "text/plain");
                            return;
                        }
                        string[] string0 = data.Split('|');
                        string textfind = string0[0];
                        if (string0.Length < 4 || string0[4].Length != 36)
                        {
                            res.AsText("error data", "text/plain");
                            return;
                        }
                        if (!string0[1].ToLower().Contains("@outlook.") && !string0[1].ToLower().Contains("@hotmail."))
                        {
                            res.AsText("error data", "text/plain");
                            return;
                        }
                        bool checkLive = false;
                        if (textfind == "checkmaillive")
                        {
                            checkLive = true;
                        }
                        string Client_ID = string0[4];
                        string refresh_token = string0[3];
                        string access_token = "";
                        if (string0.Length > 5 && string0[5].Length > 500)
                        {
                            if (string0[5].Contains("+") || string0[5].Contains(" "))
                            {
                                access_token = string0[5].Replace(" ", "+");
                            }
                        }
                        using (HttpRequest request = new HttpRequest())
                        {
                            request.IgnoreProtocolErrors = true;
                            request.AllowAutoRedirect = false;
                            request.ConnectTimeout = 10000;
                            #region Proxy_
                            //if (Proxy != null && Proxy.Contains(":"))
                            //{
                            //    string[] px = Proxy.Split(':');
                            //    if (HttpProxy)
                            //    {
                            //        if (px.Length == 4)
                            //        {
                            //            request.Proxy = HttpProxyClient.Parse($"{px[0]}:{px[1]}");
                            //            request.Proxy.Username = px[2];
                            //            request.Proxy.Password = px[3];
                            //        }
                            //        else
                            //        {
                            //            request.Proxy = HttpProxyClient.Parse($"{px[0]}:{px[1]}");
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (px.Length == 4)
                            //        {
                            //            request.Proxy = Socks5ProxyClient.Parse($"{px[0]}:{px[1]}");
                            //            request.Proxy.Username = px[2];
                            //            request.Proxy.Password = px[3];
                            //        }
                            //        else
                            //        {
                            //            request.Proxy = Socks5ProxyClient.Parse($"{px[0]}:{px[1]}");
                            //        }
                            //    }
                            //}
                            #endregion
                            bool Layaccess_token = false;
                            bool DALayACtoken = false;
                            for (int i = 0; i < 3; i++)
                            {
                                if (Layaccess_token)
                                {
                                    DALayACtoken = true;
                                    Layaccess_token = false;
                                    request.ClearAllHeaders();
                                    string postData = $"client_id={Client_ID}" +
                                        $"&refresh_token={refresh_token}" +
                                        $"&grant_type=refresh_token";
                                    string response = request.Post("https://login.microsoftonline.com/common/oauth2/v2.0/token", postData, "application/x-www-form-urlencoded").ToString();
                                    if (response.Contains("authenticated as the grant is expired"))
                                    {
                                        res.AsText("new refresh_token", "text/plain");
                                        return;
                                    }
                                    else if (response.Contains("User account is found to be in service abuse mode"))
                                    {
                                        res.AsText("die", "text/plain");
                                        return;
                                        throw new Exception("die");
                                    }
                                    else if (response.Contains(" Cross-origin ") || response.Contains("\"error_description\""))
                                    {
                                        request.ClearAllHeaders();
                                        request.AddHeader("Origin", "https://developer.microsoft.com");
                                        response = request.Post("https://login.microsoftonline.com/common/oauth2/v2.0/token", postData, "application/x-www-form-urlencoded").ToString();
                                    }
                                    if (response.Contains("authenticated as the grant is expired"))
                                    {
                                        res.AsText("new refresh_token", "text/plain");
                                        return;
                                    }
                                    dynamic json = JsonConvert.DeserializeObject(response);
                                    access_token = json.access_token;
                                    if (access_token != null && access_token.Length > 12)
                                    {
                                        if (checkLive)
                                        {
                                            res.AsText("live", "text/plain");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Không lấy được access token!");
                                    }
                                }
                                if (access_token != "")
                                {
                                    if (checkLive && DALayACtoken)
                                    {
                                        res.AsText("live", "text/plain");
                                        return;
                                    }
                                    ReadCode readCode = new ReadCode(textfind);
                                    string plainText = readCode.StartReadCodeLogin(string0[1], string0[2], TokenAuth: access_token);
                                    if (plainText != "")
                                    {
                                        res.AsBytes(req, Encoding.UTF8.GetBytes(plainText), "text/plain; charset=UTF-8;");
                                        return;
                                    }
                                    else
                                    {
                                        res.AsText("no mail", "text/plain");
                                        return;
                                    }
                                }
                                else
                                {
                                    Layaccess_token = true;
                                    continue;
                                }
                            }
                            if (checkLive)
                            {
                                res.AsText("check fail", "text/plain");
                                return;
                            }
                            else
                            {
                                res.AsText("no mail", "text/plain");
                                return;
                            }
                        }
                    }
                    catch
                    {
                        res.AsText("error", "text/plain");
                        return;
                    }
                }
            });
            Route.Add("/getmailpop/{data}", (req, res, props) =>
            {
                string dt = $"{props["data"]}";
                lock (LockGet1)
                {
                    try
                    {
                        if (dt == null || dt == "")
                        {
                            res.AsText("error data", "text/plain");
                            return;
                        }
                        string data = WebUtility.UrlDecode(dt);
                        if (!data.StartsWith("data?"))
                        {
                            res.AsText("error url", "text/plain");
                            return;
                        }
                        data = data.Substring(5, data.Length - 5);
                        if (!data.Contains("|"))
                        {
                            res.AsText("error data", "text/plain");
                            return;
                        }
                        if (!data.ToLower().Contains("@outlook.") && !data.ToLower().Contains("@hotmail."))
                        {
                            res.AsText("error data", "text/plain");
                            return;
                        }
                        string[] string0 = data.Split('|');
                        string textfind = string0[0];
                        if (string0.Length < 4 || string0[4].Length != 36)
                        {
                            res.AsText("error data", "text/plain");
                            return;
                        }
                        if (!string0[1].ToLower().Contains("@outlook.") && !string0[1].ToLower().Contains("@hotmail."))
                        {
                            res.AsText("error data", "text/plain");
                            return;
                        }
                        bool checkLive = false;
                        if (textfind == "checkmaillive")
                        {
                            checkLive = true;
                        }
                        string Client_ID = string0[4];
                        string refresh_token = string0[3];
                        string access_token = "";
                        if (string0.Length > 5 && string0[5].Length > 500)
                        {
                            if (string0[5].Contains("+") || string0[5].Contains(" "))
                            {
                                access_token = string0[5].Replace(" ", "+");
                            }
                        }
                        using (HttpRequest request = new HttpRequest())
                        {
                            request.IgnoreProtocolErrors = true;
                            request.AllowAutoRedirect = false;
                            request.ConnectTimeout = 10000;
                            #region Proxy_
                            //if (Proxy != null && Proxy.Contains(":"))
                            //{
                            //    string[] px = Proxy.Split(':');
                            //    if (HttpProxy)
                            //    {
                            //        if (px.Length == 4)
                            //        {
                            //            request.Proxy = HttpProxyClient.Parse($"{px[0]}:{px[1]}");
                            //            request.Proxy.Username = px[2];
                            //            request.Proxy.Password = px[3];
                            //        }
                            //        else
                            //        {
                            //            request.Proxy = HttpProxyClient.Parse($"{px[0]}:{px[1]}");
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (px.Length == 4)
                            //        {
                            //            request.Proxy = Socks5ProxyClient.Parse($"{px[0]}:{px[1]}");
                            //            request.Proxy.Username = px[2];
                            //            request.Proxy.Password = px[3];
                            //        }
                            //        else
                            //        {
                            //            request.Proxy = Socks5ProxyClient.Parse($"{px[0]}:{px[1]}");
                            //        }
                            //    }
                            //}
                            #endregion
                            bool Layaccess_token = false;
                            bool DALayACtoken = false;
                            for (int i = 0; i < 3; i++)
                            {
                                if (Layaccess_token)
                                {
                                    DALayACtoken = true;
                                    Layaccess_token = false;
                                    request.ClearAllHeaders();
                                    string postData = $"client_id={Client_ID}" +
                                        $"&refresh_token={refresh_token}" +
                                        $"&grant_type=refresh_token";
                                    string response = request.Post("https://login.microsoftonline.com/common/oauth2/v2.0/token", postData, "application/x-www-form-urlencoded").ToString();
                                    if (response.Contains("authenticated as the grant is expired"))
                                    {
                                        res.AsText("new refresh_token", "text/plain");
                                        return;
                                    }
                                    else if (response.Contains("User account is found to be in service abuse mode"))
                                    {
                                        res.AsText("die", "text/plain");
                                        return;
                                        throw new Exception("die");
                                    }
                                    else if (response.Contains(" Cross-origin ") || response.Contains("\"error_description\""))
                                    {
                                        request.ClearAllHeaders();
                                        request.AddHeader("Origin", "https://developer.microsoft.com");
                                        response = request.Post("https://login.microsoftonline.com/common/oauth2/v2.0/token", postData, "application/x-www-form-urlencoded").ToString();
                                    }
                                    if (response.Contains("authenticated as the grant is expired"))
                                    {
                                        res.AsText("new refresh_token", "text/plain");
                                        return;
                                    }
                                    dynamic json = JsonConvert.DeserializeObject(response);
                                    access_token = json.access_token;
                                    if (access_token != null && access_token.Length > 12)
                                    {
                                        if (checkLive)
                                        {
                                            res.AsText("live", "text/plain");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Không lấy được access token!");
                                    }
                                }
                                if (access_token != "")
                                {
                                    if (checkLive && DALayACtoken)
                                    {
                                        res.AsText("live", "text/plain");
                                        return;
                                    }
                                    ReadCode readCode = new ReadCode(textfind);
                                    string plainText = readCode.StartReadCodeLogin_1(string0[1], string0[2], TokenAuth: access_token);
                                    if (plainText != "")
                                    {
                                        res.AsBytes(req, Encoding.UTF8.GetBytes(plainText), "text/plain; charset=UTF-8;");
                                        return;
                                    }
                                    else
                                    {
                                        res.AsText("no mail", "text/plain");
                                        return;
                                    }
                                }
                                else
                                {
                                    Layaccess_token = true;
                                    continue;
                                }
                            }
                            if (checkLive)
                            {
                                res.AsText("check fail", "text/plain");
                                return;
                            }
                            else
                            {
                                res.AsText("no mail", "text/plain");
                                return;
                            }
                        }
                    }
                    catch
                    {
                        res.AsText("error", "text/plain");
                        return;
                    }
                }
            });
            HttpServer.ListenAsync(1111, CancellationToken.None, Route.OnHttpRequestAsync, false, 255);//http://127.0.0.1:1111/getmail/data?
            Task.Run(() =>
            {
                try
                {
                    Task.Delay(1000).Wait();
                    using (HttpClient http = new HttpClient())
                    {
                        string string0 = http.GetAsync($"http://127.0.0.1:1111/").Result.Content.ReadAsStringAsync().Result;
                        if (string0 == "Connected")
                        {
                            MessageBox.Show($"Server Is Running Port 1111");
                            return;
                        }
                    }
                }
                catch
                {

                }
                MessageBox.Show("Server Is Not Running. Please Restart App.");
                this.Invoke((MethodInvoker)(() => this.Close()));
            });
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
