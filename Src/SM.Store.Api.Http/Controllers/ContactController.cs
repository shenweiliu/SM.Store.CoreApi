using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using SM.Store.Api.BLL;
using SM.Store.Api.Common;
using SM.Store.Api.DAL;
using SM.Store.Api.Contracts.Interfaces;
using Entities = SM.Store.Api.Contracts.Entities;
using Models = SM.Store.Api.Contracts.Models;
using System.Threading.Tasks;

namespace SM.Store.Api.Http
{
    [CustomAuthorize]
    [Route("api")]
    public class ContactsController : ControllerBase
    {
        private IContactBS bs;
        private IAutoMapConverter<Entities.Contact, Models.Contact> mapEntityToModel;
        private IAutoMapConverter<Models.Contact, Entities.Contact> mapModelToEntity;
        public ContactsController(IContactBS contactBS,
                                  IAutoMapConverter<Entities.Contact, Models.Contact> convertEntityToModel,
                                  IAutoMapConverter<Models.Contact, Entities.Contact> convertModelToEntity)
        {
            bs = contactBS;
            this.mapEntityToModel = convertEntityToModel;
            this.mapModelToEntity = convertModelToEntity;
        }

        [HttpGet("getcontactlist")]
        public async Task<Models.ContactListResponse> GetContactList()
        {
            var resp = new Models.ContactListResponse();
            resp.Contacts = new Models.Contacts();           
                        
            var rtnList = await bs.GetContacts();
            var convtList = mapEntityToModel.ConvertObjectCollection(rtnList);
            resp.Contacts.AddRange(convtList);           
            return resp;           
        }

        [HttpGet("GetContactById/{id:int}")]
        //[ResponseType(typeof(Contact))]
        public async Task<IActionResult> GetContactById(int id)
        {            
            var eContact = await bs.GetContactById(id);
            if (eContact == null)
            {
                return NotFound();
            }
            else
            {                
                Models.Contact mContact = mapEntityToModel.ConvertObject(eContact);
                return Ok(mContact);
            }
        }

        [HttpPost("addcontact")]
        public async Task<Models.AddContactResponse> Post_AddContact([FromBody] Models.Contact mContact)
        {
            var eContact = mapModelToEntity.ConvertObject(mContact);
            await bs.AddContact(eContact);
            
            var addContactResponse = new Models.AddContactResponse() 
            {
                ContactId =  eContact.ContactId
            };
            return addContactResponse; 
        }

        [HttpPost("addcontacts")]
        public async Task<Models.AddContactsResponse> Post_AddContacts([FromBody] List<Models.Contact> mContactList)
        {
            var contactIdList = new List<int>();
            Entities.Contact eContact = null;
            foreach (var mContact in mContactList)
            {                
                eContact = mapModelToEntity.ConvertObject(mContact);
                await bs.AddContact(eContact);
                contactIdList.Add(eContact.ContactId);                
            }
            var resp = new Models.AddContactsResponse();
            resp.ContactIdList = contactIdList;
            return resp;
        }

        [HttpPost("updatecontact")]
        public async Task Post_UpdateContact([FromBody] Models.Contact mContact)
        {            
            var eContact = mapModelToEntity.ConvertObject(mContact);
            await bs.UpdateContact(eContact);
        }

        [HttpPost("updatecontacts")]
        public async Task Post_UpdateContacts([FromBody] List<Models.Contact> mContactList)
        {
            Entities.Contact eContact = null;
            foreach (var mContact in mContactList)
            {
                eContact = mapModelToEntity.ConvertObject(mContact);
                await bs.UpdateContact(eContact);
            }            
        }

        [HttpPost("deletecontact")]
        public async Task DeleteContact([FromBody] int id)
        {            
            await bs.DeleteContact(id);
        }

        [HttpPost("deletecontacts")]
        public async Task Post_DeleteContact([FromBody] List<int> ids)
        {            
            if (ids.Count > 0)
            {
                //await not working for delegated ForEach.
                //ids.ForEach((Action<int>)delegate(int id)
                //{
                //    await bs.DeleteContact((int)id);
                //});
                foreach (int id in ids)
                {
                    await bs.DeleteContact(id);
                }
            }
        }        
    }
}
