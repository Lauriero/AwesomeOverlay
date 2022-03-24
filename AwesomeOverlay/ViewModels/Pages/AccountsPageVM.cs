using AwesomeOverlay.Core.Services.Abstractions;
using AwesomeOverlay.ViewModels.Users;
using AwesomeOverlay.ViewModels.UserServices;
using AwesomeOverlay.Core.Utilities.CollectionUtilities;

using Prism.Commands;
using Prism.Mvvm;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

namespace AwesomeOverlay.ViewModel.Pages
{
    class AccountsPageVM : BindableBase
    {
        private IUserStorageService _userStorage;
        private IAuthorizationService _authorization;

        public AccountsPageVM(IUserStorageService userStorage, IAuthorizationService authorization)
        {
            _authorization = authorization;
            _userStorage = userStorage;
         
            Users.SynchroniseWith(_userStorage.Users, u => new UserVM(u));
            Users.CreateMenu(u => { SelectedUser = u; });
            Users.FirstOrDefault()?.ElementController.Select();
            Users.SubscribeToChangedEvent(NotifyCollectionChangedAction.Remove, () => {
                if (Users.Count > 0) {
                    Users[0].ElementController.Select();
                } else {
                    SelectedUser = null;
                }
            });

            if (Users.Count == 0) {
                IsAuthViewVisible = true;
            }

            UserAuthorizers = new ReadOnlyCollection<UserServiceAuthVM>(_authorization.UserAuthorizers.Select(us => new UserServiceAuthVM(us)).ToList());
            UserAuthorizers.CreateMenu(us => { SelectedUserService = us; });
            UserAuthorizers.FirstOrDefault()?.ElementController.Select();
            UserAuthorizers.ToList().ForEach(
                us => us.AuthorizationComplited += () => { 
                    IsAuthViewVisible = false; 
                }
            );

            AddUserCommand = new DelegateCommand(() => {
                IsAuthViewVisible = true;
            });

            BackToAccountsCommand = new DelegateCommand(() => {
                IsAuthViewVisible = false;
            });
        }
            
        public ICommand AddUserCommand { get; private set; }
        public ICommand BackToAccountsCommand { get; private set; }
        
        public ReadOnlyCollection<UserServiceAuthVM> UserAuthorizers { get; private set; }
        public ObservableCollection<UserVM> Users { get; private set; } = new ObservableCollection<UserVM>();

        private UserVM _selectedUser;
        public UserVM SelectedUser
        {
            get => _selectedUser;
            set {
                SetProperty(ref _selectedUser, value);
            }
        }

        private UserServiceAuthVM _selectedUserService;
        public UserServiceAuthVM SelectedUserService
        {
            get => _selectedUserService;
            set {
                SetProperty(ref _selectedUserService, value);
            }
        }

        private bool _isAuthViewVisible = false;
        public bool IsAuthViewVisible
        {
            get => _isAuthViewVisible;
            set {
                SetProperty(ref _isAuthViewVisible, value);
            }
        }
    }
}
