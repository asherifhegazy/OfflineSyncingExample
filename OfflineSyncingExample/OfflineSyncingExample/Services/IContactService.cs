using OfflineSyncingExample.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OfflineSyncingExample.Services
{
    public interface IContactService
    {
        Task Init();

        Task<bool> AddContact(Contact contact);

        Task<bool> RemoveContact(Contact contact);

        Task<List<Contact>> GetContacts();

        Task SyncronizeAsync();
    }
}
