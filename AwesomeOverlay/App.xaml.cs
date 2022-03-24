using AwesomeOverlay.Core.KeyboardHookerService;
using AwesomeOverlay.Core.Model.Users;
using AwesomeOverlay.Core.Model.UserServices;
using AwesomeOverlay.Core.Services;
using AwesomeOverlay.Core.Services.Abstractions;
using AwesomeOverlay.Model.UserServices;
using AwesomeOverlay.Services;
using AwesomeOverlay.UserDataStorage.Abstraction;
using AwesomeOverlay.UserDataStorage.Core;
using AwesomeOverlay.ViewModel;
using AwesomeOverlay.ViewModel.Pages;
using AwesomeOverlay.ViewModels;
using AwesomeOverlay.Views;

using Microsoft.Extensions.DependencyInjection;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace AwesomeOverlay
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            //Database
            services.AddSingleton<IUserDataStorageManager, UserDataStorageManager>();

            //Hot keys handler
            services.AddSingleton<IKeyboardHooker, KeyboardHooker>();

            //Services
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IUserFactoryService, UserFactoryService>();
            services.AddSingleton<IUserStorageService, UserStorageService>();
            services.AddSingleton<IAuthorizationService, AuthorizationService>();
            services.AddSingleton<ILocalFileStorageService, LocalFileStorageService>();
            services.AddSingleton<IHotKeyService, HotKeyService>();

            //Aggregator
            services.AddSingleton<IGlobalServiceAggregator, GlobalServiceAggregator>();

            //ViewModels
            services.AddTransient<MainWindow>();
            services.AddTransient<MainWindowVM>();
            services.AddTransient<AccountsPageVM>();
            services.AddTransient<MessagesPageVM>();
            services.AddTransient<NotificationLayerVM>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            IGlobalServiceAggregator serviceAggregator = serviceProvider.GetService<IGlobalServiceAggregator>();
            serviceAggregator.InitAsync(UserServices.ToList()).Wait();

            MainWindow view = serviceProvider.GetService<MainWindow>();
            MainWindowVM viewModel = serviceProvider.GetService<MainWindowVM>();

            view.DataContext = viewModel;
            view.Show();
        }

        public static ReadOnlyCollection<IUserService<UserBase>> UserServices = new ReadOnlyCollection<IUserService<UserBase>>(new List<IUserService<UserBase>>() {
            new VkUserService(),
            new TgUserService()
        });
    }
}