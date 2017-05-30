using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Modules.EmailCampaign;
using Sitecore.Modules.EmailCampaign.Core.Services;
using Sitecore.Modules.EmailCampaign.Exceptions;
using Sitecore.Modules.EmailCampaign.Messages;
using Sitecore.Modules.EmailCampaign.Recipients;
using Sitecore.Modules.EmailCampaign.Xdb;
using Sitecore.StringExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Sitecore.Support
{
    public class SendingHelper
    {
        static PropertyInfo instance= (typeof(ClientApi)).GetProperty("Instance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        static bool? isCD;
        private static bool IsCD
        {
            get
            {
                if (!isCD.HasValue)
                {
                    var value = instance.GetValue(null);
                    Assert.IsNotNull(value, "Cannot get instance of Sitecore.Modules.EmailCampaign.Core.ClientApiBase");
                    var test = value as EcmServiceClient;
                    isCD= test != null;
                }              
                
                return isCD.Value;
            }
        }

        public static void SendStandardMessage(string messageId, XdbContactId recipientId, string language, Dictionary<string,string> tokens)
        {
            
            if (IsCD)
            {
                SendStandardMessageRemoteEvent re = new SendStandardMessageRemoteEvent();
                re.Language = language;
                re.MessageId = messageId;
                re.RecipientId = recipientId.ToString();
                re.Tokens = tokens;
                Sitecore.Context.Database.RemoteEvents.Queue.QueueEvent<SendStandardMessageRemoteEvent>(re);
            }
            else
            {
                Send(messageId, language, tokens, recipientId);
            }
        }

        internal static void Send(string messageId, string language, Dictionary<string,string> tokens, XdbContactId recipientId)
        {

            Language lang;
            MessageItem message = null;
            if (Language.TryParse(language, out lang))
            {
                message = Factory.GetMessage(ID.Parse(messageId), lang);
            }
            else
            {
                message = Factory.GetMessage(messageId);
                Log.Warn("Sitecore.Support.101226.165115. Message language caannot be parsed. Default language will be used.", new object());
            }

            if (message == null)
            {
                Log.Error("Sitecore.Support.101226.165115. No message item was found by the id '{0}'.".FormatWith(new object[]
                {
            messageId
                }), new object());
                return;
            }

            if (tokens != null)
            {
                foreach (KeyValuePair<string, string> p in tokens)
                {
                    message.CustomPersonTokens.Add(p.Key, p.Value);
                }
            }

           

            try
            {


                new AsyncSendingManager(false, message).SendStandardMessage(recipientId);

            }
            catch (EmailCampaignException e)
            {
                Log.Error("Sitecore.Support.101226.165115. Failed to send Standard message.", e, new object());
            }
        }
       
    }
}