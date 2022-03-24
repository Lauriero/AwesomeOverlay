using AwesomeOverlay.Core.Services.Abstractions;
using AwesomeOverlay.Core.Utilities.UPN;
using AwesomeOverlay.View.Pages;
using AwesomeOverlay.ViewModel.Pages;
using AwesomeOverlay.ViewModels;

using Prism.Mvvm;

using System.Windows.Controls;

namespace AwesomeOverlay.ViewModel
{
    class MainWindowVM : BindableBase
    {
        public NotificationLayerVM NotificationLayer { get; }

        public AccountsPageVM AccountsPage;
        public MessagesPageVM MessagesPage;

        public MainWindowVM(AccountsPageVM accountsPage, MessagesPageVM messagesPage, NotificationLayerVM notificationLayer, IHotKeyService hotKeys)
        {
            NotificationLayer = notificationLayer;

            AccountsPage = accountsPage;
            MessagesPage = messagesPage;

            NavigationPalette = UniversalPageNavigator.RegisterController(this, "CurrentPage")
                                    .AddNavigatedPage(new AccountsPage(), accountsPage)
                                    .AddNavigatedPage(new MessagesPage(), messagesPage)
                                    .BuildPalette();

            NavigationPalette.GetCommands()[0].Execute(null);

            hotKeys.MainWindowStateChanged += () => { MainWindowVisible = !MainWindowVisible; };
        }

        public CommandPalette NavigationPalette { get; private set; }

        private Page _currentPage;
        public Page CurrentPage
        {
            get => _currentPage;
            set {
                SetProperty(ref _currentPage, value);
            }
        }

        private bool _mainWindowVisible = false;
        public bool MainWindowVisible
        {
            get => _mainWindowVisible;
            set {
                SetProperty(ref _mainWindowVisible, value);
            }
        }

    }
}