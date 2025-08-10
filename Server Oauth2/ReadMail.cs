using HtmlAgilityPack;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Net.Proxy;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Read_Hotmail_Outlook
{
    public class ReadCode
    {
        ImapClient client = null;
        Pop3Client clientp = null;
        string textfind = "";
        public ReadCode(string textfind_ = "")
        {
            textfind = textfind_;
        }
        public string StartReadCodeLogin(string mail, string pass, string proxy = null, string domain = "outlook.office365.com", string TokenAuth = null)
        {
            try
            {
                List<DataMailBoxPlus> dataMail = new List<DataMailBoxPlus>();
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        client = new ImapClient();
                        if (proxy != null)
                        {
                            string[] px = proxy.Split(':');
                            if (px.Length == 4)
                            {
                                client.ProxyClient = new HttpProxyClient(px[0], Convert.ToInt32(px[1]), new System.Net.NetworkCredential(px[2], px[3]));
                            }
                            else
                            {
                                client.ProxyClient = new HttpProxyClient(px[0], Convert.ToInt32(px[1]));
                            }
                        }
                        client.Connect(domain, 993, true);
                        if (TokenAuth == null)
                        {
                            client.Authenticate(mail, pass);
                        }
                        else
                        {
                            SaslMechanismOAuth2 auth2 = new SaslMechanismOAuth2(mail, TokenAuth);
                            client.Authenticate(auth2);
                        }
                        break;
                    }
                    catch
                    {
                        client = null;
                        Task.Delay(1000).Wait();
                    }
                }
                if (client == null)
                {
                    throw new Exception("");
                }
                dataMail.Clear();
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);
                IList<UniqueId> uids = inbox.Search(SearchQuery.All);
                var junk = client.GetFolder("Junk");
                junk.Open(FolderAccess.ReadOnly);
                IList<UniqueId> uids1 = junk.Search(SearchQuery.All);
                UniqueId Trc = UniqueId.MinValue;
                bool flag = false;
                if (uids1.Count == 0 && uids.Count == 0)
                {
                    try
                    {
                        client.Disconnect(true);
                        client.Dispose();
                    }
                    catch { }
                    return "";
                }
                if (uids1.Count > 0)
                {
                    Trc = uids1[uids1.Count - 1];
                    uids1.RemoveAt(uids1.Count - 1);
                    flag = true;
                }
                foreach (UniqueId lki in uids1)
                {
                    dataMail.Add(new DataMailBoxPlus { UID = lki, MailBox = "Junk" });
                }
                foreach (UniqueId lki in uids)
                {
                    dataMail.Add(new DataMailBoxPlus { UID = lki, MailBox = "INBOX" });
                }
                if (flag)
                {
                    dataMail.Add(new DataMailBoxPlus { UID = Trc, MailBox = "Junk" });
                }
                int lenginfos = dataMail.Count;
                int Dem = 0;
                for (int i = dataMail.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        Dem++;
                        if (Dem > lenginfos || Dem > 50)
                        {
                            try
                            {
                                client.Disconnect(true);
                                client.Dispose();
                            }
                            catch { }
                            return "";
                        }
                        MimeMessage message = null;
                        if (dataMail[i].MailBox == "INBOX")
                        {
                            inbox.Open(FolderAccess.ReadOnly);
                            message = inbox.GetMessage(dataMail[i].UID);
                        }
                        else
                        {
                            junk.Open(FolderAccess.ReadOnly);
                            message = junk.GetMessage(dataMail[i].UID);
                        }
                        Task.Delay(100).Wait();
                        string string0 = "";
                        if (message.HtmlBody != null && message.HtmlBody.Length > 0)
                        {
                            string0 = message.HtmlBody;
                            if (textfind == "")
                            {
                                HtmlDocument doc = new HtmlDocument();
                                doc.LoadHtml(string0);
                                string plainText = doc.DocumentNode.InnerText;
                                Regex extractUrlRegex = new Regex("https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)");
                                string[] allurl = extractUrlRegex.Matches(string0.Replace("&amp;", "&")).Cast<Match>().Select(m => m.Value).ToArray();
                                if (allurl != null && allurl.Length > 0)
                                {
                                    plainText += $"|{string.Join("|", allurl)}";
                                }
                                try
                                {
                                    client.Disconnect(true);
                                    client.Dispose();
                                }
                                catch { }
                                return plainText;
                            }
                            else if (string0.Contains(textfind))
                            {
                                HtmlDocument doc = new HtmlDocument();
                                doc.LoadHtml(string0);
                                string plainText = doc.DocumentNode.InnerText;
                                Regex extractUrlRegex = new Regex("https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)");
                                string[] allurl = extractUrlRegex.Matches(string0.Replace("&amp;", "&")).Cast<Match>().Select(m => m.Value).ToArray();
                                if (allurl != null && allurl.Length > 0)
                                {
                                    plainText += $"|{string.Join("|", allurl)}";
                                }
                                try
                                {
                                    client.Disconnect(true);
                                    client.Dispose();
                                }
                                catch { }
                                return plainText;
                            }
                        }
                        else if (message.TextBody != null && message.TextBody.Length > 0)
                        {
                            string0 = message.TextBody;
                            if (textfind == "")
                            {
                                Regex extractUrlRegex = new Regex("https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)");
                                string[] allurl = extractUrlRegex.Matches(string0.Replace("&amp;", "&")).Cast<Match>().Select(m => m.Value).ToArray();
                                if (allurl != null && allurl.Length > 0)
                                {
                                    string0 += $"|{string.Join("|", allurl)}";
                                }
                                try
                                {
                                    client.Disconnect(true);
                                    client.Dispose();
                                }
                                catch { }
                                return string0;
                            }
                            else if (string0.Contains(textfind))
                            {
                                Regex extractUrlRegex = new Regex("https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)");
                                string[] allurl = extractUrlRegex.Matches(string0.Replace("&amp;", "&")).Cast<Match>().Select(m => m.Value).ToArray();
                                if (allurl != null && allurl.Length > 0)
                                {
                                    string0 += $"|{string.Join("|", allurl)}";
                                }
                                try
                                {
                                    client.Disconnect(true);
                                    client.Dispose();
                                }
                                catch { }
                                return string0;
                            }
                        }
                    }
                    catch { }
                }
                try
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
                catch { }
            }
            catch
            {
                try
                {
                    client.Dispose();
                }
                catch { }
                return "";
            }
            return "";
        }
        public string StartReadCodeLogin_1(string mail, string pass, string proxy = null, string domain = "outlook.office365.com", string TokenAuth = null)
        {
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        clientp = new Pop3Client();
                        if (proxy != null)
                        {
                            string[] px = proxy.Split(':');
                            if (px.Length == 4)
                            {
                                clientp.ProxyClient = new HttpProxyClient(px[0], Convert.ToInt32(px[1]), new System.Net.NetworkCredential(px[2], px[3]));
                            }
                            else
                            {
                                clientp.ProxyClient = new HttpProxyClient(px[0], Convert.ToInt32(px[1]));
                            }
                        }
                        clientp.Connect(domain, 995, true);
                        if (TokenAuth == null)
                        {
                            clientp.Authenticate(mail, pass);
                        }
                        else
                        {
                            SaslMechanismOAuth2 auth2 = new SaslMechanismOAuth2(mail, TokenAuth);
                            clientp.Authenticate(auth2);
                        }
                        break;
                    }
                    catch
                    {
                        clientp = null;
                        Task.Delay(2000).Wait();
                    }
                }
                if (clientp == null)
                {
                    throw new Exception("");
                }
                int uids = clientp.Count;
                if (uids == 0)
                {
                    try
                    {
                        clientp.Disconnect(true);
                        clientp.Dispose();
                    }
                    catch { }
                    return "";
                }
                int Dem = 0;
                for (int i = uids - 1; i >= 0; i--)
                {
                    try
                    {
                        Dem++;
                        if (Dem > 50)
                        {
                            try
                            {
                                clientp.Disconnect(true);
                                clientp.Dispose();
                            }
                            catch { }
                            return "";
                        }
                        MimeMessage message = clientp.GetMessage(i);
                        Task.Delay(100).Wait();
                        string string0 = "";
                        if (message.HtmlBody != null && message.HtmlBody.Length > 0)
                        {
                            string0 = message.HtmlBody;
                            if (textfind == "")
                            {
                                HtmlDocument doc = new HtmlDocument();
                                doc.LoadHtml(string0);
                                string plainText = doc.DocumentNode.InnerText;
                                Regex extractUrlRegex = new Regex("https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)");
                                string[] allurl = extractUrlRegex.Matches(string0.Replace("&amp;", "&")).Cast<Match>().Select(m => m.Value).ToArray();
                                if (allurl != null && allurl.Length > 0)
                                {
                                    plainText += $"|{string.Join("|", allurl)}";
                                }
                                try
                                {
                                    clientp.Disconnect(true);
                                    clientp.Dispose();
                                }
                                catch { }
                                return plainText;
                            }
                            else if (string0.Contains(textfind))
                            {
                                HtmlDocument doc = new HtmlDocument();
                                doc.LoadHtml(string0);
                                string plainText = doc.DocumentNode.InnerText;
                                Regex extractUrlRegex = new Regex("https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)");
                                string[] allurl = extractUrlRegex.Matches(string0.Replace("&amp;", "&")).Cast<Match>().Select(m => m.Value).ToArray();
                                if (allurl != null && allurl.Length > 0)
                                {
                                    plainText += $"|{string.Join("|", allurl)}";
                                }
                                try
                                {
                                    clientp.Disconnect(true);
                                    clientp.Dispose();
                                }
                                catch { }
                                return plainText;
                            }
                        }
                        else if (message.TextBody != null && message.TextBody.Length > 0)
                        {
                            string0 = message.TextBody;
                            if (textfind == "")
                            {
                                Regex extractUrlRegex = new Regex("https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)");
                                string[] allurl = extractUrlRegex.Matches(string0.Replace("&amp;", "&")).Cast<Match>().Select(m => m.Value).ToArray();
                                if (allurl != null && allurl.Length > 0)
                                {
                                    string0 += $"|{string.Join("|", allurl)}";
                                }
                                try
                                {
                                    clientp.Disconnect(true);
                                    clientp.Dispose();
                                }
                                catch { }
                                return string0;
                            }
                            else if (string0.Contains(textfind))
                            {
                                Regex extractUrlRegex = new Regex("https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)");
                                string[] allurl = extractUrlRegex.Matches(string0.Replace("&amp;", "&")).Cast<Match>().Select(m => m.Value).ToArray();
                                if (allurl != null && allurl.Length > 0)
                                {
                                    string0 += $"|{string.Join("|", allurl)}";
                                }
                                try
                                {
                                    clientp.Disconnect(true);
                                    clientp.Dispose();
                                }
                                catch { }
                                return string0;
                            }
                        }
                    }
                    catch { }
                }
                try
                {
                    clientp.Disconnect(true);
                    clientp.Dispose();
                }
                catch { }
            }
            catch
            {
                try
                {
                    clientp.Dispose();
                }
                catch { }
                return "";
            }
            return "";
        }
        public bool CheckLive(string mail, string pass, string proxy = null, string domain = "outlook.office365.com")
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    using (ImapClient client1 = new ImapClient())
                    {
                        if (proxy != null)
                        {
                            string[] px = proxy.Split(':');
                            if (px.Length == 4)
                            {
                                client1.ProxyClient = new HttpProxyClient(px[0], Convert.ToInt32(px[1]), new System.Net.NetworkCredential(px[2], px[3]));
                            }
                            else
                            {
                                client1.ProxyClient = new HttpProxyClient(px[0], Convert.ToInt32(px[1]));
                            }
                        }
                        client1.Connect(domain, 993, true);
                        client1.Authenticate(mail, pass);
                        try
                        {
                            client1.DisconnectAsync(true);
                            client1.Dispose();
                        }
                        catch { }
                    }
                    return true;
                }
                catch { }
            }
            return false;
        }
        public bool CheckLive_1(string mail, string pass, string proxy = null, string domain = "outlook.office365.com")
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    using (Pop3Client client1 = new Pop3Client())
                    {
                        if (proxy != null)
                        {
                            string[] px = proxy.Split(':');
                            if (px.Length == 4)
                            {
                                client1.ProxyClient = new HttpProxyClient(px[0], Convert.ToInt32(px[1]), new System.Net.NetworkCredential(px[2], px[3]));
                            }
                            else
                            {
                                client1.ProxyClient = new HttpProxyClient(px[0], Convert.ToInt32(px[1]));
                            }
                        }
                        client1.Connect(domain, 995, true);
                        client1.Authenticate(mail, pass);
                        try
                        {
                            client1.DisconnectAsync(true);
                            client1.Dispose();
                        }
                        catch { }
                    }
                    return true;
                }
                catch { }
            }
            return false;
        }
    }
    public class DataMailBoxPlus
    {
        public UniqueId UID { set; get; }
        public string MailBox { set; get; } = "INBOX";
    }
}
