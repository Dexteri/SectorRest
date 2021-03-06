﻿using RBSector.UserClient.ViewModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RBSector.UserClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //this is a bad way, we shouldn't do that
            var vm = (MainPageViewModel)DataContext;
            vm.CreateCommand.Execute(null);



            vm.SelectCommand.Execute(null);
            //Frame.Navigate(typeof());
        }
    }
}
