using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using OfflineSyncingExample.Models;
using OfflineSyncingExample.Services;
using OfflineSyncingExample.ViewModels;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace OfflineSyncingExample
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPageViewModel MainPageViewModel { get; set; }
        public MainPage()
        {
            InitializeComponent();

            MainPageViewModel = new MainPageViewModel(new ContactService());

            BindingContext = MainPageViewModel;

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await MainPageViewModel.OnAppearing();
        }

        private void OnDeleteClicked(object sender, EventArgs e)
        {
            var contact = (sender as MenuItem).CommandParameter as Contact;
            MainPageViewModel.DeleteCommand.Execute(contact);
        }
    }
}
