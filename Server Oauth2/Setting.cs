using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MimeKit;

namespace Read_Hotmail_Outlook
{
    public class ListMailMessage
    {
        public MimeMessage MailMessage { set; get; }
        public string MailBox { set; get; }
    }
    public class DataPOP3GetMail
    {
        public string Status { set; get; } = "";
        public bool Online { set; get; }
        public Pop3Client client { set; get; }
    }
    public class DataIMAP4GetMail
    {
        public string Status { set; get; } = "";
        public string MailBox { set; get; } = "INBOX";
        public bool Online { set; get; }
        public ImapClient client { set; get; }
    }
    public class DataMail
    {
        public string From { set; get; }
        public string Header { set; get; }
        public string Html { set; get; }
        public string Text { set; get; }
    }
    public class DataMailBox
    {
        public int UID { set; get; }
        public string MailBox { set; get; } = "INBOX";
    }
    public class DataGMail
    {
        public string Mail { set; get; }
        public string Pass { set; get; }
        public string Code { set; get; } = "";
    }
}
