using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using System.IO;
using Common;
using System.Web.Security;
using System.Text;
using System.Net;
using System.Drawing;
using System.Linq.Expressions;

namespace WebCenter.Web.Controllers
{
    public class BankController : BaseController
    {
        public BankController(IUnitOfWork UOF)
            : base(UOF)
        {
        }

        public ActionResult Add(bank _bank)
        {
            var dbInc = Uof.IbankService.AddEntity(_bank);
            if (dbInc == null)
            {
                return Json(new { success = false, message = "保存失败" }, JsonRequestBehavior.AllowGet);
            }

            return SuccessResult;
        }

        public ActionResult Get(int id)
        {
            var dbIncome = Uof.IbankService.GetAll(i => i.id == id).Select(i => new
            {
                id = i.id,
                name = i.name,
                account = i.account,
                owner = i.owner,
                date_created = i.date_created
            }).FirstOrDefault();

            return Json(dbIncome, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(bank _bank)
        {
            var dbBank = Uof.IbankService.GetAll(i => i.id == _bank.id).FirstOrDefault();

            if (dbBank.name == _bank.name &&
                dbBank.account == _bank.account &&
                dbBank.owner == _bank.owner)
            {
                return SuccessResult;
            }
            dbBank.name = _bank.name;
            dbBank.account = _bank.account;
            dbBank.owner = _bank.owner;
            dbBank.date_updated = DateTime.Now;
            var r = Uof.IbankService.UpdateEntity(dbBank);            
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var db = Uof.IbankService.GetAll(b => b.id == id).FirstOrDefault();

            var r = Uof.IbankService.DeleteEntity(db);

            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(int index = 1, int size = 30, string name = "")
        {

            Expression<Func<bank, bool>> nameQuery = c => true;

            if (!string.IsNullOrEmpty(name))
            {
                nameQuery = c => (c.name.IndexOf(name) > -1);
            }
                        
            var list = Uof.IbankService.GetAll(nameQuery)
                .OrderByDescending(item => item.id).Select(c => new
                {
                    id = c.id,
                    name = c.name,
                    account = c.account,
                    owner = c.owner,
                    date_created = c.date_created
                }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.IbankService.GetAll(nameQuery).Count();

            var totalPages = 0;
            if (totalRecord > 0)
            {
                totalPages = (totalRecord + size - 1) / size;
            }
            var page = new
            {
                current_index = index,
                current_size = size,
                total_size = totalRecord,
                total_page = totalPages
            };

            var result = new
            {
                page = page,
                items = list
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}