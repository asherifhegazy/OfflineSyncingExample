using OfflineSyncingExample.Models;
using OfflineSyncingExample.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace OfflineSyncingExample.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        #region NotifyPropertyChanged Implementaion

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return;

            backingField = value;

            OnPropertyChanged(propertyName);

        }

        #endregion

        bool isLoading = false;

        public bool IsLoading
        {
            get => isLoading;

            set
            {
                if (isLoading != value)
                {
                    SetValue(ref isLoading, value);
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

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

        private IContactService _contactService;

        public ICommand AddCommand { get; set; }
        public ICommand SyncCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public MainPageViewModel(IContactService contactService)
        {
            _contactService = contactService;

            Contacts = new ObservableCollection<Contact>();

            AddCommand = new Command(OnAddCommand);
            SyncCommand = new Command(OnSyncCommand);
            DeleteCommand = new Command<object>(OnDeleteCommand);
        }

        public async Task OnAppearing()
        {
            IsLoading = true;

            await _contactService.Init();

            var contactsList = await _contactService.GetContacts();

            IsLoading = false;

            Contacts = new ObservableCollection<Contact>(contactsList);

        }

        private async void OnAddCommand()
        {
            IsLoading = true;

            var entry = new Contact
            {
                Name = "Ahmed" + DateTime.Now,
                Number = "01020" + DateTime.Now.Year + DateTime.Today.Second
            };

            var isAdded = await _contactService.AddContact(entry);

            IsLoading = false;

            if (isAdded)
            {
                Contacts.Add(entry);
                await Application.Current.MainPage.DisplayAlert("Success", "Contact Added Successfully", "OK");
            }
            else
                await Application.Current.MainPage.DisplayAlert("Failure", "Something Went Wrong", "Cancel");

        }

        private async void OnSyncCommand()
        {
            await _contactService.SyncronizeAsync();

            var contactsList = await _contactService.GetContacts();
            Contacts = new ObservableCollection<Contact>(contactsList);
        }

        private async void OnDeleteCommand(object obj)
        {
            IsLoading = true;

            var contact = obj as Contact;

            var isRemoved = await _contactService.RemoveContact(contact);

            IsLoading = false;

            if (isRemoved)
            {
                Contacts.Remove(contact);
                await Application.Current.MainPage.DisplayAlert("Success", "Contact Removed Successfully", "OK");
            }
            else
                await Application.Current.MainPage.DisplayAlert("Failure", "Something Went Wrong", "Cancel");
        }
    }
}
