using AwesomeOverlay.Core.Utilities.UPN;

using Prism.Commands;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace AwesomeOverlay.UserControls.MainMenu
{
    /// <summary>
    /// Логика взаимодействия для MainMenuControl.xaml
    /// </summary>
    [ContentProperty("MenuElements")]
    public partial class MainMenuControl : UserControl
    {
        public static readonly DependencyProperty MenuCommandPaletteProperty =
            DependencyProperty.Register("MenuCommandPalette", typeof(CommandPalette), typeof(MainMenuControl), new PropertyMetadata(null, MenuCommandPaletteChanged));

        public static readonly DependencyProperty MenuElementsProperty =
            DependencyProperty.Register("MenuElements", typeof(List<MainMenuElement>), typeof(MainMenuControl), new PropertyMetadata(new List<MainMenuElement> { }));

        private Dictionary<MainMenuElement, ICommand> menuElementSelectedCommands = new Dictionary<MainMenuElement, ICommand>();
        private MainMenuElement selectedElement;

        public MainMenuControl()
        {
            InitializeComponent();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]        
        public List<MainMenuElement> MenuElements
        {
            get { return (List<MainMenuElement>)GetValue(MenuElementsProperty); }
            set { SetValue(MenuElementsProperty, value); }
        }

        public CommandPalette MenuCommandPalette
        {
            get { return (CommandPalette)GetValue(MenuCommandPaletteProperty); }
            set { SetValue(MenuCommandPaletteProperty, value); }
        }

        private static void MenuCommandPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            MainMenuControl instance = d as MainMenuControl;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (MenuCommandPalette == null) {
                throw new Exception("Menu command palette must be defined");
            }

            if (MenuCommandPalette.CommandsCount != MenuElements.Count) {
                throw new Exception("Number of commands in command palette must be equal to number of menu elements. " +
                    $"Founded menu elements: {MenuElements.Count}, founded commands: {MenuCommandPalette.CommandsCount}");
            }

            double menuElementHeight = MenuElements.FirstOrDefault()?.ActualHeight ?? 0;

            ReadOnlyCollection<ICommand> commands = MenuCommandPalette.GetCommands();
            for (int i = 0; i < commands.Count; i++) {
                ICommand command = commands[i];
                MainMenuElement element = MenuElements[i];
                element.InputBindings.Clear();
                element.InputBindings.Add(new InputBinding(new DelegateCommand(() => {
                    ElementSelected(element, menuElementHeight);
                }), new MouseGesture(MouseAction.LeftClick)));

                menuElementSelectedCommands.Add(element, command);
            }

            for (int i = 0; i < MenuElements.Count; i++) {
                if (MenuElements[i].Selected) {
                    ElementSelected(MenuElements[i], menuElementHeight);
                    break;
                }
            }
        }

        private void ElementSelected(MainMenuElement element, double menuElementHeight)
        {
            menuElementSelectedCommands[element]?.Execute(null);

            element.Selected = true;
            selectedElement = element;
            MenuElements.Where(e => e != selectedElement).ToList().ForEach(e => e.Selected = false);

            ThicknessAnimation sliderAnimation = new ThicknessAnimation();
            sliderAnimation.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseInOut };
            sliderAnimation.Duration = TimeSpan.FromMilliseconds(280);
            sliderAnimation.To = new Thickness(0, MenuElementsControl.ItemContainerGenerator.IndexFromContainer(element) * menuElementHeight + (menuElementHeight - Slider.ActualHeight) / 2, 0, 0);

            Slider.BeginAnimation(MarginProperty, sliderAnimation);
        }
    }
}
