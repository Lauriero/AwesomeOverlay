using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Core.Model.UserServices;
using AwesomeOverlay.Core.Utilities.CollectionUtilities;

using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AwesomeOverlay.ViewModels.Users
{
    public class UserVM : BindableBase, IMenuElement
    {
        public event Action<UserVM> UserDeleted;
        public event Action<UserVM> UserUnauthorized;

        private UserBase _model;
        private LogoutAbleUser _logoutAbleModel;
        private IUserService<UserBase> _userService;

        public UserVM(UserBase model)
        {
            _model = model;
            _model.PropertyChanged += (o, e) => RaisePropertyChanged(e.PropertyName);

            if (_model is LogoutAbleUser logoutAbleUser) {
                _logoutAbleModel = logoutAbleUser;
                IsLogoutIconVisible = true;
            }

            foreach (var service in App.UserServices) {
                Type userModelType = service.GetType().GetInterface("IUserService`1").GetGenericArguments()[0];
                if (model.GetType() == userModelType) {
                    _userService = service;
                    
                    ServiceColor = _userService.ServiceColor;
                    ServiceIconResourceKey = _userService.ServiceIconResourceKey;

                    break;
                }
            }

            DeleteCommand = new DelegateCommand(async () => {
                await _model.DeleteAsync();
            });

            LogoutCommand = new DelegateCommand(async () => {
                await _logoutAbleModel.UnauthorizeAsync();
            });
        }

        public ICommand DeleteCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }

        public Color ServiceColor { get; } 
        public string ServiceIconResourceKey { get; }

        public bool Authorized => _model.Authorized;
        public string FirstName => _model.FirstName;
        public string SecondName => _model.SecondName;
        public string Nickname => _model.Nickname;
        public Uri Avatar => _model.Avatar;

        public MenuElementController ElementController { get; private set; } = new MenuElementController();

        private bool _isLogoutIconVisible;
        public bool IsLogoutIconVisible
        {
            get => _isLogoutIconVisible;
            set {
                SetProperty(ref _isLogoutIconVisible, value);
            }
        }

        public static IEnumerable<UserVM> ConvertMultiple(IEnumerable<UserBase> models)
        {
            UserVM[] viewModels = new UserVM[models.Count()];
            
            for (int i = 0; i < models.Count(); i++) {
                viewModels[i] = new UserVM(models.ElementAt(i));
            }

            return viewModels;
        }
    }
}
