using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAlwaysRunningCheckingServiceBus.Impl
{
    public class Data
    {
        public string sessionIdentifier { get; set; }
        public int hotelIdentifier { get; set; }
        public string providerEmailAddress { get; set; }
        public string subject { get; set; }
        public string guestName { get; set; }
        public string guestTelephone { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string roomType { get; set; }
        public decimal? amountPaid { get; set; }
        public DateTime DatePosted { get; set; }
    }

    public class MessagePayload
    {
        public string subject { get; set; }
        public string eventType { get; set; }
        public DateTime eventTime { get; set; }
        public string id { get; set; }
        public Data data { get; set; }
        public string dataVersion { get; set; }
        public string metadataVersion { get; set; }
    }
}
