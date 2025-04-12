using System;
using System.Linq;
using System.Collections.Generic;
using SM.Store.Api.Common;
using Entities = SM.Store.Api.Contracts.Entities;
using Models = SM.Store.Api.Contracts.Models;
using SM.Store.Api.Contracts.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace SM.Store.Api.DAL
{
    public class ContactRepository : GenericRepository<Entities.Contact>, IContactRepository
    {
        private StoreDataContext storeDBContext;
        public ContactRepository(StoreDataContext context)
            : base(context)
        {
            storeDBContext = context;
        }

        public async Task<IList<Entities.Contact>> GetContacts()
        {
            return await this.GetAllAsync();
        }

        public async Task<Entities.Contact> GetContactById(int id)
        {
            return await this.GetByIdAsync(id);
        }

        public async Task<int> AddContact(Entities.Contact inputEt)
        {
            inputEt.ContactId = 0;
            inputEt.AuditTime = DateTime.Now;
            await this.InsertAsync(inputEt, true);
            //this.Commit();
            return inputEt.ContactId;
        }

        public async Task UpdateContact(Entities.Contact inputEt)
        {
            //Get entity to be updated
            Entities.Contact updEt = GetContactById(inputEt.ContactId).Result;

            if (!string.IsNullOrEmpty(inputEt.ContactName)) updEt.ContactName = inputEt.ContactName;
            if (!string.IsNullOrEmpty(inputEt.Phone)) updEt.Phone = inputEt.Phone;
            if (!string.IsNullOrEmpty(inputEt.Email)) updEt.Email = inputEt.Email;
            if (inputEt.PrimaryType != 0) updEt.PrimaryType = inputEt.PrimaryType;
            updEt.AuditTime = DateTime.Now;

            await this.UpdateAsync(updEt, true);
            //this.Commit();
        }

        public async Task DeleteContact(int id)
        {
            await this.DeleteAsync(id, true);
            //this.Commit();
        }
    }
}
