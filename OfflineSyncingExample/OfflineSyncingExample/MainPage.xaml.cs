using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using OfflineSyncingExample.Models;
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
        private const string AzureEndPoint = "https://offlinesyncingexample.azurewebsites.net";
        private MobileServiceClient MobileServiceClient;
        private IMobileServiceSyncTable<Contact> ContactTable;

        private ObservableCollection<Contact> contacts;

        public ObservableCollection<Contact> Contacts
        {
            get => contacts;
            set
            {
                if (contacts == value)
                    return;

                contacts = value;
                OnPropertyChanged();
            }
        }

        public MainPage()
        {
            InitializeComponent();

            Contacts = new ObservableCollection<Contact>();
            BindingContext = this;

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MobileServiceClient = new MobileServiceClient(AzureEndPoint);
            var store = new MobileServiceSQLiteStore("local.db");
            store.DefineTable<Contact>();
            await MobileServiceClient.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());
            ContactTable = MobileServiceClient.GetSyncTable<Contact>();

            await ContactTable.PullAsync(null, ContactTable.CreateQuery());
            Contacts = new ObservableCollection<Contact>(await ContactTable.ToListAsync());
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            var entry = new Contact
            {
                Name = "Ahmed" + DateTime.Now,
                Number = "01020" + DateTime.Now.Year + DateTime.Today.Second
            };

            try
            {
                Contacts.Add(entry);
                await ContactTable.InsertAsync(entry);
            }
            catch (Exception ex)
            {

            }
        }

        private async void OnSyncClicked(object sender, EventArgs e)
        {
            await SyncronizeAsync();
        }

        private async Task SyncronizeAsync()
        {
            if (!CrossConnectivity.Current.IsConnected)
                return;

            try
            {
                await MobileServiceClient.SyncContext.PushAsync();
                await ContactTable.PullAsync(null, ContactTable.CreateQuery());
                Contacts = new ObservableCollection<Contact>(await ContactTable.ToListAsync());
            }
            catch (Exception ex)
            {

            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var contact = (sender as MenuItem).CommandParameter as Contact;
            Contacts.Remove(contact);

            await ContactTable.DeleteAsync(contact);
            await SyncronizeAsync();
        }
    }
}
