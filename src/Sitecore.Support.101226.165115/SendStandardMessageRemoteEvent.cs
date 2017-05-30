using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Sitecore.Support
{
    [DataContract]
    public class SendStandardMessageRemoteEvent
    {
        [DataMember]
        public string MessageId
        {
            get;set;
        }

        [DataMember]
        public string Language
        {
            get;set;
        }

       

        [DataMember]
        public string RecipientId
        {
            get;set;
        }

        [DataMember]
        public Dictionary<string,string> Tokens
        {
            get;set;
        }
    }
}