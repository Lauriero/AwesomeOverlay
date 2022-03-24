using AwesomeOverlay.Core.Attributes;
using AwesomeOverlay.Core.FileStorageSystem;
using AwesomeOverlay.Core.Model.Notifications.Attachments;
using AwesomeOverlay.Core.Model.Notifications.Attachments.Abstractions;
using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Model.Notifications.Messages;

using ByteSizeLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using VkNet;
using VkNet.Enums;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace AwesomeOverlay.Model.Users
{
    public class VkUser : LogoutAbleUser, INotificationProvider
    {
        /// <inheritdoc />
        public event Action<MessageNotification> NotificationRecieved;

        private VkApi _api;
        private long _vkUserId;
        private SecureString _token;

        [DataBaseColumn("token", deleteOnUnauthorize: true)]
        public SecureString Token
        {
            get => _token;
            set {
                SetProperty(ref _token, value);
            }
        }

        public VkUser(UserFileStoragesAggregator userFileStorages) : base(userFileStorages) {
            _userFileStorages
                .AddStorage("avatars")
                .AddStorage("pictures")
                .AddStorage("documents");

            Avatar = _userFileStorages["avatars"].GetFile("avatar.jpg");
        }

        public void ProvideApi(VkApi api)
        {
            _api = api;
            _vkUserId = _api.UserId ?? 0;
            Authorized = true;

            if (Token == null) {
                unsafe {
                    fixed (char* token = api.Token) {
                        Token = new SecureString(token, api.Token.Length);      
                    }
                }
            }
        }

        public async Task UpdateProfileData()
        {
            var users = await _api.Users.GetAsync(new List<long>(), ProfileFields.Photo100 | ProfileFields.ScreenName);
            _vkUserId = users[0].Id;

            FirstName = users[0].FirstName;
            SecondName = users[0].LastName;

            if (string.IsNullOrEmpty(users[0].ScreenName)) {
                Nickname = $"@id{_vkUserId}";
            } else {
                Nickname = $"@{users[0].ScreenName}";
            }

            Avatar = await _userFileStorages["avatars"].CreateDownload(users[0].Photo100, "avatar.jpg", FileStoringMode.Eternal).DownloadAsync();
        }

        private ulong _ts;
        private ulong? _pts;

        /// <inheritdoc />
        public async Task StartNotificationRecieving()
        {
            if (!Authorized) {
                return;
            }

            LongPollServerResponse response = await _api.Messages.GetLongPollServerAsync(true);
            _pts = response.Pts;
            _ts = Convert.ToUInt64(response.Ts);

            Task.Run(async () => await LongPollEventLoop());
        }

        public async Task LongPollEventLoop(CancellationToken token = default)
        {
            while (true) {
                LongPollHistoryResponse response = await _api.Messages.GetLongPollHistoryAsync(new MessagesGetLongPollHistoryParams() {
                    Ts = _ts,
                    Pts = _pts,
                    Fields = UsersFields.Photo100,
                });

                if (response.Messages.Count > 0) {
                    Console.WriteLine("Сообщение");
                }

                foreach (Message message in response.Messages.Where(m => m.Type == MessageType.Received || m.FromId == _vkUserId)) {
                    User sender = response.Profiles.Where(p => p.Id == message.FromId).FirstOrDefault();

                    Uri senderAvatar = _userFileStorages["avatars"].GetFile($"{sender.Id}.png");
                    if (senderAvatar == null) {
                        senderAvatar = await _userFileStorages["avatars"]
                            .CreateDownload(sender.Photo100, $"{sender.Id}.png", FileStoringMode.Temp)
                            .DownloadAsync();
                    }

                    List<IAttachment> attachments = new List<IAttachment>();
                    foreach (Attachment attachment in message.Attachments) {
                        if (attachment.Type == typeof(Photo)) {
                            Photo photo = attachment.Instance as Photo;

                            Uri photoSource = _userFileStorages["pictures"].GetFile($"{attachment.Instance.Id}.jpg");
                            if (photoSource == null) {
                                photoSource = await _userFileStorages["pictures"]
                                    .CreateDownload(photo.Sizes[3].Url, $"{attachment.Instance.Id}.jpg", FileStoringMode.Temp)
                                    .DownloadAsync();
                            }

                            attachments.Add(new ImageAttachment(photoSource));
                        } else if (attachment.Type == typeof(Video)) {
                            Video video = await GetFullVideoInfo(attachment.Instance as Video);
                            if (video != null) {
                                foreach (VideoImage img in video.Image) {
                                    if (img.Height > 240) {
                                        Uri previewSource = _userFileStorages["pictures"].GetFile($"{video.Id}_preview.jpg");
                                        if (previewSource == null) {
                                            previewSource = await _userFileStorages["pictures"]
                                                .CreateDownload(img.Url, $"{video.Id}_preview.jpg", FileStoringMode.Temp)
                                                .DownloadAsync();
                                        }
                                        
                                        attachments.Add(new VideoAttachment(new Uri(FindBiggerVideoRes(video.Files).AbsoluteUri.Remove(4, 1), UriKind.Absolute), previewSource, TimeSpan.FromSeconds((int)video.Duration)));
                                        break;
                                    }
                                }
                            }
                        } else if (attachment.Type == typeof(Document)) {
                            Document doc = (attachment.Instance as Document);
                            attachments.Add(new DocumentAttachment(_userFileStorages["documents"].CreateDownload(new Uri(doc.Uri), $"{doc.Id}", FileStoringMode.Temp), doc.Ext.ToUpper(), ByteSize.FromBytes((long)doc.Size)));
                        }
                    }

                    NotificationRecieved?.Invoke(new MessageNotification() {
                        SenderName = $"{sender.FirstName} {sender.LastName}",
                        SenderAvatar = senderAvatar,
                        MessageText = message.Text,
                        AttachmentCategories = AttachmentCategory.Parse(attachments)
                    });
                }

                _pts = response.NewPts;

                await Task.Delay(500);
            }
        }
        
        private async Task<Video> GetFullVideoInfo(Video video)
        {
            return (await _api.Video.GetAsync(new VideoGetParams() {
                OwnerId = video.OwnerId,
                Videos = new Video[] { video },
                Count = 1
            })).FirstOrDefault();
        }

        private Uri FindBiggerVideoRes(VideoFiles videoFiles)
        {
            Uri biggerResUrl = null;
            if (videoFiles.Mp4_1080 != null)
                biggerResUrl = videoFiles.Mp4_1080;
            else if (videoFiles.Mp4_720 != null)
                biggerResUrl = videoFiles.Mp4_720;
            else if (videoFiles.Mp4_480 != null)
                biggerResUrl = videoFiles.Mp4_480;
            else if (videoFiles.Mp4_360 != null)
                biggerResUrl = videoFiles.Mp4_360;
            else if (videoFiles.Mp4_240 != null)
                biggerResUrl = videoFiles.Mp4_240;

            return biggerResUrl;
        }
    }
}
