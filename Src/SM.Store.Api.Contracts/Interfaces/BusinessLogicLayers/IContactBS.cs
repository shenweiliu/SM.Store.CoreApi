using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace SM.Store.Api.Contracts.Interfaces
{
    public interface IContactBS
    {
        Task<IList<Entities.Contact>> GetContacts();
        Task<Entities.Contact> GetContactById(int id);
        
        Task<int> AddContact(Entities.Contact inputEt);
        Task UpdateContact(Entities.Contact inputEt);
        Task DeleteContact(int id);
    }

}