using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataPersist
{
    public class UserMessage
    {
        public MessageType MessageType { get; set; }
        public string Content { get; set; }
        public UserData Sender { get; set; }
    }
}
