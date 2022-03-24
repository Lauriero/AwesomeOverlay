using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Core.Model.UserServices;
using AwesomeOverlay.Core.Utilities.CollectionUtilities;

using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AwesomeOverlay.ViewModels.UserServices
{
    public class UserServiceAuthVM : BindableBase, IMenuElement
    {
        public event Action AuthorizationComplited;

        private IUserService<UserBase> _userService;
        private IUserAuthorizer<UserBase> _userAuthorizer;

        public UserServiceAuthVM(IUserService<UserBase> userService) {
            if (userService is IUserAuthorizer<UserBase> userAuthorizer) {
                _userAuthorizer = userAuthorizer;
            } else {
                throw new Exception("UserServiceAuthVM can be constructed only on instance that implements both IUserService and IUserAuthorizer interfaces");
            }

            _userService = userService;

            AuthorizeCommand = new DelegateCommand(async () => {
                string error = await _userAuthorizer.Authorize();
                if (error is null) {
                    IsErrorMessageVisible = false;
                    AuthorizationComplited?.Invoke();
                } else {
                    ErrorMessage = error;
                    IsErrorMessageVisible = true;
                }
            });
        }

        public Color ServiceColor => _userService.ServiceColor;
        public string ServiceIconResourceKey => _userService.ServiceIconResourceKey;

        public ICommand AuthorizeCommand { get; private set; }

        public UserControl AuthorizationView => _userAuthorizer.AuthorizationView;

        private bool _isErrorMessageVisible;
        public bool IsErrorMessageVisible
        {
            get => _isErrorMessageVisible;
            set {
                SetProperty(ref _isErrorMessageVisible, value);
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set {
                SetProperty(ref _errorMessage, value);
            }
        }

        public MenuElementController ElementController { get; } = new MenuElementController();
    }
}
