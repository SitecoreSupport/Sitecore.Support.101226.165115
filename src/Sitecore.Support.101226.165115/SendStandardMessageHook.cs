using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Modules.EmailCampaign;
using Sitecore.Modules.EmailCampaign.Exceptions;
using Sitecore.Modules.EmailCampaign.Messages;
using Sitecore.Modules.EmailCampaign.Recipients;
using Sitecore.Modules.EmailCampaign.Xdb;
using Sitecore.StringExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Support
{
    public class SendStandardMessageHook : Sitecore.Events.Hooks.IHook
    {
        public void Initialize()
        {            
            Sitecore.Eventing.EventManager.Subscribe<SendStandardMessageRemoteEvent>(new Action<SendStandardMessageRemoteEvent>(SendStandardMessageHook.Run));
        }

        public static void Run(SendStandardMessageRemoteEvent remoteEvent)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNull(remoteEvent, "remoteEvent");
            XdbContactId recipientId;
            if (!XdbContactId.TryParse(remoteEvent.RecipientId, out recipientId))
            {
                Log.Error("Sitecore.Support.101226.165115. XdbContactId cannot be parsed '{0}'.".FormatWith(new object[]
               {
            remoteEvent.RecipientId
               }), new object());
                return;
            }

            SendingHelper.Send(remoteEvent.MessageId, remoteEvent.Language, remoteEvent.Tokens, recipientId);
           
        }
    }
}