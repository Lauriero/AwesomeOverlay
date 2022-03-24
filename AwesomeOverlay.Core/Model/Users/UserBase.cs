using AwesomeOverlay.Core.Attributes;
using AwesomeOverlay.Core.FileStorageSystem;

using Prism.Mvvm;

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AwesomeOverlay.Core.Model.Users
{
    public class UserBase : BindableBase
    {
        private string _firstName;
        private string _secondName;
        private string _nickname;
        private bool _authorized;

        public event UserDeletedEventHandler UserDeleted;
        protected UserFileStoragesAggregator _userFileStorages;

        public int UserId { get; set; }
        public string StorageName { get; set; }

        [DataBaseColumn("first_name")]
        public string FirstName
        {
            get => _firstName;
            set {
                SetProperty(ref _firstName, value);
            }
        }

        [DataBaseColumn("second_name")]
        public string SecondName
        {
            get => _secondName;
            set {
                SetProperty(ref _secondName, value);
            }
        }

        [DataBaseColumn("nickname")]
        public string Nickname
        {
            get => _nickname;
            set {
                SetProperty(ref _nickname, value);
            }
        }

        [DataBaseColumn("authorized")]
        public bool Authorized
        {
            get => _authorized;
            set {
                SetProperty(ref _authorized, value);
            }
        }

        private Uri _avatar;
        public Uri Avatar
        {
            get => _avatar;
            set {
                SetProperty(ref _avatar, value);
            }
        }

        public UserBase(UserFileStoragesAggregator userFileStorages) {
            _userFileStorages = userFileStorages;
        }

        /// <summary>
        /// Invokes when user should be deleted
        /// </summary>
        public async Task DeleteAsync()
        {
            await UserDeleted?.Invoke(this);
        }
    }

    /// <summary>
    /// Delegate for user deleted event
    /// </summary>
    public delegate Task UserDeletedEventHandler(UserBase user);
}