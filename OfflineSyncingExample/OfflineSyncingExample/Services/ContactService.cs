using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using OfflineSyncingExample.Models;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OfflineSyncingExample.Services
{
    public class ContactService : IContactService
    {
        private const string AzureEndPoint = "https://offlinesyncingexample.azurewebsites.net";
        private MobileServiceClient MobileServiceClient;
        private IMobileServiceSyncTable<Contact> ContactTable;

        public async Task Init()
        {
            MobileServiceClient = new MobileServiceClient(AzureEndPoint);
            var store = new MobileServiceSQLiteStore("local.db");
            store.DefineTable<Contact>();
            await MobileServiceClient.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());
            ContactTable = MobileServiceClient.GetSyncTable<Contact>();

            await ContactTable.PullAsync(null, ContactTable.CreateQuery());
        }

        public async Task<bool> AddContact(Contact contact)
        {
            try
            {
                await ContactTable.InsertAsync(contact);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<Contact>> GetContacts()
        {
             return await ContactTable.ToListAsync();
        }

        public async Task<bool> RemoveContact(Contact contact)
        {
            try
            {
                await ContactTable.DeleteAsync(contact);
                await SyncronizeAsync();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
            
        }

        public async Task SyncronizeAsync()
        {
            if (!CrossConnectivity.Current.IsConnected)
                return;

            try
            {
                await MobileServiceClient.SyncContext.PushAsync();
                await ContactTable.PullAsync(null, ContactTable.CreateQuery());
            }
            catch (Exception ex)
            {

            }
        }
    }
}
