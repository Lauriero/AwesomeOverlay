using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Core.Model.UserServices;
using AwesomeOverlay.Core.Utilities.ColorUtilities;
using AwesomeOverlay.Core.Utilities.SecurityUtilities;
using AwesomeOverlay.Model.Users;
using AwesomeOverlay.Views.UserServices;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Model;

namespace AwesomeOverlay.Model.UserServices
{
    public class VkUserService : ILogoutAbleUserService<VkUser>, IUserAuthorizer<VkUser>
    {
        /// <inheritdoc />
        public string StorageName { get; } = "vk";

        /// <inheritdoc />
        public Color ServiceColor { get; } = "#6A8DC1".TurnHexIntoColor();

        /// <inheritdoc />
        public string ServiceIconResourceKey { get; } = "VkIcon";


        #region IUserAuthorizer impl

        /// <inheritdoc />
        public event Func<VkUser, bool, Task> UserAuthorized;

        /// <inheritdoc />
        public async Task<string> Authorize()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddAudioBypass();

            VkApi api = new VkApi(services);

            try {
                await api.AuthorizeAsync(new ApiAuthParams() {
                    Login = _authorizationView.LoginTextInput.Value,
                    Password = _authorizationView.PasswordInput.Value.Unsecure(),
                });

                VkUser user = await CreateNewUser.Invoke() as VkUser;
                user.ProvideApi(api);

                await UserAuthorized?.Invoke(user, true);
                await user.UpdateProfileData();

                return null;
            } catch (VkAuthorizationException) {
                return "Ошибка в логине или пароле";
            }
        }

        /// <summary>
        /// Authorize user using data saved in storage
        /// </summary>
        public async Task AuthorizeFromStorage(UserBase user)
        {
            Console.WriteLine("Авторизация");

            VkUser vkUser = user as VkUser;

            VkApi api = new VkApi();
            await api.AuthorizeAsync(new ApiAuthParams() {
                AccessToken = vkUser.Token.Unsecure()
            }).ConfigureAwait(false);

            vkUser.ProvideApi(api);
            await UserAuthorized?.Invoke(vkUser, false);
            await vkUser.UpdateProfileData();
        }

        /// <inheritdoc />
        public UserControl AuthorizationView => _authorizationView;
        private VKAuthorizationControl _authorizationView = new VKAuthorizationControl();

        /// <inheritdoc />
        public Func<Task<UserBase>> CreateNewUser { private get; set; }
        
        /// <inheritdoc />
        public bool IsTheFinalAuthorizationStepActive
        {
            get => _isTheFinalAuthorizationStepActive;
            set {
                if (_isTheFinalAuthorizationStepActive != value) {
                    _isTheFinalAuthorizationStepActive = value;
                    OnPropertyChanged("IsTheFinalAuthorizationStepActive");
                }
            }
        }

        private bool _isTheFinalAuthorizationStepActive;

        #endregion

        #region INotifyPropertyChanged impl

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
