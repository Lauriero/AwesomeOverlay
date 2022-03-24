using AwesomeOverlay.Core.Model.Notifications.Attachments.Abstractions;

using Prism.Mvvm;

using System;
using System.Collections.Generic;

namespace AwesomeOverlay.Model.Notifications.Messages
{
	public class MessageNotification : BindableBase
	{ 
		public Uri SenderAvatar { get; set; }
		public string SenderName { get; set; }
		public string MessageText { get; set; }
        public AudioMessage VoiceMessage { get; set; }
		
		public Uri PathToChatAvatarImage { get; set; }
		public string ChatTitle { get; set; }

		public List<AttachmentCategory> AttachmentCategories { get; set; } = new List<AttachmentCategory>();
	}
}
