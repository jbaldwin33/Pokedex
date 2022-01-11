﻿using MVVMFramework.ViewNavigator;
using MVVMFramework.Views;
using Pokedex.PokedexApp;
using Pokedex.PokedexApp.ViewModels;
using System;
using System.Windows;

namespace Pokedex
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var types = new (Type, string, bool)[]
            {
                (typeof(DetailsViewModel), "Details", true),
                
            };

            var window = new MainWindow(types, new MainViewModel(Navigator.Instance));
            window.Show();
        }
    }
}
