using System.Collections.Generic;
using System;
using SM.Store.Api.Contracts.Interfaces;
using Entities = SM.Store.Api.Contracts.Entities;
using System.Threading.Tasks;

namespace SM.Store.Api.BLL
{
    public class ContactBS : IContactBS
    {
        private IContactRepository _contactRepository;
        
        public ContactBS(IContactRepository contactRepository)
        {
            if (contactRepository != null)
                this._contactRepository = contactRepository;            
        }
        
        public async Task<IList<Entities.Contact>> GetContacts()
        {
            return await this._contactRepository.GetContacts();
        }

        public async Task<Entities.Contact> GetContactById(int id)
        {
            return await this._contactRepository.GetContactById(id);
        }

        public async Task<int> AddContact(Entities.Contact inputEt)
        {
            return await this._contactRepository.AddContact(inputEt);
        }

        public async Task UpdateContact(Entities.Contact inputEt)
        {
            await this._contactRepository.UpdateContact(inputEt);
        }

        public async Task DeleteContact(int id)
        {
            await this._contactRepository.DeleteContact(id);
        }
    }
}
