using MVVMFramework.Controls;
using MVVMFramework.Utilities;
using MVVMFramework.ViewModels;
using MVVMFramework.ViewNavigator;
using MVVMFramework.Views;
using Pokedex.PkdxDatabase.Context;
using Pokedex.PkdxDatabase.Models;
using Pokedex.PokedexApp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MainViewModel = Pokedex.PokedexApp.ViewModels.MainViewModel;

namespace Pokedex.PokedexApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ViewBaseWindow
    {
        public MainWindow((Type, string, bool)[] viewModelTypes) : base(new MainViewModel(Navigator.Instance))
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (viewModelTypes == null || viewModelTypes.Length == 0)
                throw new ArgumentNullException(nameof(viewModelTypes));

            InitializeComponent();
            Navigator.Instance.NavigationBar = navigationBar;
            foreach (var (type, name, show) in viewModelTypes)
            {
                var instance = (ViewModel)Activator.CreateInstance(type);
                if (Navigator.Instance.CurrentViewModel == null)
                    Navigator.Instance.CurrentViewModel = instance;

                Navigator.Instance.ViewModels.Add(instance);
                var button = new DefaultButton
                {
                    CommandParameter = instance,
                    Content = name,
                    Command = Navigator.Instance.UpdateCurrentViewModelCommand,
                };
                var binding = new Binding("IsShown")
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Converter = new InverseBooleanConverter(),
                    Source = Navigator.Instance.ViewModels.FirstOrDefault(vm => vm.GetType() == type)
                };

                button.SetBinding(IsEnabledProperty, binding);
                button.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
                navigationBar.stackPanel.Children.Add(button);
            }
        }

        protected override void BeforeShow(object sender, RoutedEventArgs e) { }

        protected override void AfterShow(object sender, EventArgs e) { }

        protected override void OnClosing(object sender, CancelEventArgs e) { }

        protected override void OnClosed(object sender, EventArgs e) { }
    }
}
