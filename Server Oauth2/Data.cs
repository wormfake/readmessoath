using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Read_Hotmail_Outlook
{
    public class DataGraphApi
    {
        [JsonProperty("@odata.context")]
        public string my { set; get; } = null;
        public List<DataMailGraphApi> value { set; get; } = new List<DataMailGraphApi>();
    }
    public class DataMailGraphApi
    {
        [JsonProperty("@odata.etag")]
        public string my { set; get; } = null;
        public string id { set; get; } = null;
        public string createdDateTime { set; get; } = null;
        public string lastModifiedDateTime { set; get; } = null;
        public string changeKey { set; get; } = null;
        public object categories { set; get; } = null;
        public DateTime DateTime { set; get; } = new DateTime();
        public string receivedDateTime { set; get; } = null;
        public string sentDateTime { set; get; } = null;
        public bool hasAttachments { set; get; } = false;
        public string internetMessageId { set; get; } = null;
        public string subject { set; get; } = null;
        public string bodyPreview { set; get; } = null;
        public string importance { set; get; } = null;
        public string parentFolderId { set; get; } = null;
        public string conversationId { set; get; } = null;
        public string conversationIndex { set; get; } = null;
        public object isDeliveryReceiptRequested { set; get; } = null;
        public bool isReadReceiptRequested { set; get; } = false;
        public bool isRead { set; get; } = false;
        public bool isDraft { set; get; } = false;
        public string webLink { set; get; } = null;
        public string inferenceClassification { set; get; } = null;
        public DataBodyGraphApi body { set; get; } = new DataBodyGraphApi();
        public DataSenderGraphApi sender { set; get; } = new DataSenderGraphApi();
        public DataFromGraphApi from { set; get; } = new DataFromGraphApi();

        public List<DataToRecipientsGraphApi> toRecipients { set; get; } = new List<DataToRecipientsGraphApi>();
        public object ccRecipients { set; get; } = null;
        public object bccRecipients { set; get; } = null;
        public object replyTo { set; get; } = null;
        public DataFlagEmailAddressGraphApi flag { set; get; } = new DataFlagEmailAddressGraphApi();
    }
    public class DataBodyGraphApi
    {
        public string contentType { set; get; } = null;
        public string content { set; get; } = null;
    }
    public class DataSenderGraphApi
    {
        public DataSenderEmailAddressGraphApi emailAddress { set; get; } = new DataSenderEmailAddressGraphApi();
    }
    public class DataSenderEmailAddressGraphApi
    {
        public string name { set; get; } = null;
        public string address { set; get; } = null;
    }
    public class DataFromGraphApi
    {
        public DataFromEmailAddressGraphApi emailAddress { set; get; } = new DataFromEmailAddressGraphApi();
    }
    public class DataFromEmailAddressGraphApi
    {
        public string name { set; get; } = null;
        public string address { set; get; } = null;
    }
    public class DataToRecipientsGraphApi
    {
        public DataToRecipientsEmailAddressGraphApi emailAddress { set; get; } = new DataToRecipientsEmailAddressGraphApi();
    }
    public class DataToRecipientsEmailAddressGraphApi
    {
        public string name { set; get; } = null;
        public string address { set; get; } = null;
    }
    public class DataFlagEmailAddressGraphApi
    {
        public string flagStatus { set; get; } = null;
    }
}
