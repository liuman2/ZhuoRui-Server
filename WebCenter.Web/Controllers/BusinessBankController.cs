﻿using System;
using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using Common;
using System.Web.Security;
using System.Collections.Generic;
using WebCenter.Entities;
using System.IO;
using System.Drawing;
using System.Web;

namespace WebCenter.Web.Controllers
{
    public class BusinessBankController : BaseController
    {
        public BusinessBankController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult Add(business_bank bank)
        {
            Uof.Ibusiness_bankService.AddEntity(bank);
            return SuccessResult;
        }

        public ActionResult AddOpenBank(open_bank openBank, List<bank_contact> contacts)
        {
            var dbBank = Uof.Iopen_bankService.AddEntity(openBank);

            if (contacts.Count > 0)
            {
                foreach (var item in contacts)
                {
                    item.bank_id = dbBank.id;
                }

                Uof.Ibank_contactService.AddEntities(contacts);
            }

            return SuccessResult;
        }


        public ActionResult UpdateBank(open_bank openBank, List<bank_contact> contacts)
        {
            var dbBank = Uof.Iopen_bankService.GetById(openBank.id);

            dbBank.name = openBank.name;
            dbBank.address = openBank.name;
            dbBank.area = openBank.area;
            dbBank.date_updated = DateTime.Now;
            dbBank.memo = openBank.memo;

            Uof.Iopen_bankService.UpdateEntity(dbBank);

            #region 联系人
            var dbContacts = Uof.Ibank_contactService.GetAll(s => s.bank_id == openBank.id).ToList();

            var newContacts = new List<bank_contact>();
            var deleteContacts = new List<bank_contact>();
            var updateContacts = new List<bank_contact>();

            if (contacts != null && contacts.Count() > 0)
            {
                foreach (var item in contacts)
                {
                    if (item.id == 0)
                    {
                        newContacts.Add(new bank_contact
                        {
                            bank_id = openBank.id,
                            name = item.name,
                            email = item.email,                           
                            tel = item.tel,                            
                            memo = item.memo,
                        });
                    }

                    if (item.id > 0)
                    {
                        var updateContact = dbContacts.Where(d => d.id == item.id).FirstOrDefault();
                        if (updateContact != null)
                        {
                            updateContact.name = item.name;
                            updateContact.email = item.email;                           
                            updateContact.tel = item.tel;                           
                            updateContact.memo = item.memo;
                            updateContacts.Add(updateContact);
                        }
                    }
                }

                if (dbContacts.Count() > 0)
                {
                    foreach (var item in dbContacts)
                    {
                        var _contact = contacts.Where(s => s.id == item.id).FirstOrDefault();
                        if (_contact == null)
                        {
                            deleteContacts.Add(item);
                        }
                    }
                }
            }
            else
            {
                deleteContacts = dbContacts;
            }

            try
            {
                if (deleteContacts.Count > 0)
                {
                    foreach (var item in deleteContacts)
                    {
                        Uof.Ibank_contactService.DeleteEntity(item);
                    }
                }

                if (updateContacts.Count > 0)
                {
                    Uof.Ibank_contactService.UpdateEntities(updateContacts);
                }

                if (newContacts.Count > 0)
                {
                    Uof.Ibank_contactService.AddEntities(newContacts);
                }
            }
            catch (Exception)
            {
            }

            #endregion

            return SuccessResult;
        }



    }
}