using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;

namespace WebCenter.Web.Controllers
{
    public class NoticeController : BaseController
    {
        public NoticeController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult Search(int type, int index = 1, int size = 10, string name = "")
        {
            Expression<Func<notice, bool>> condition = c => c.type == type;
            Expression<Func<notice, bool>> nameQuery = c => true;

            if (!string.IsNullOrEmpty(name))
            {
                nameQuery = c => (c.title.IndexOf(name) > -1 || c.code.IndexOf(name) > -1);
            }
            
            var list = Uof.InoticeService.GetAll(condition)
                .Where(nameQuery)
                .OrderByDescending(item => item.id).Select(c => new
                {
                    id = c.id,
                    code = c.code,
                    title = c.title,
                    type = c.type,
                    creator_id = c.creator_id,
                    content = c.content,
                    date_created = c.date_created,
                    status = c.status
                }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.InoticeService.GetAll(condition).Count();

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

        public ActionResult Add(customer c)
        {
            var r = HttpContext.User.Identity.IsAuthenticated;
            if (!r)
            {
                return new HttpUnauthorizedResult();
            }

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            var userId = 0;
            var organization_id = 0;
            int.TryParse(arrs[0], out userId);
            int.TryParse(arrs[2], out organization_id);


            //c.salesman_id = userId;
            c.organization_id = organization_id;
            c.status = 1;

            c.code = GetNextCustomerCode(c.salesman_id.Value);

            var _c = Uof.IcustomerService.AddEntity(c);

            if (_c != null)
            {
                var newCustomer = Uof.Icustomer_timelineService.AddEntity(new customer_timeline
                {
                    title = "建立客户资料",
                    customer_id = _c.id,
                    content = string.Format("建立了客户资料, 操作人：{0}", arrs[3]),
                    date_business = DateTime.Now,
                    date_created = DateTime.Now,
                    is_system = 1
                });

                return Json(new { id = newCustomer.id }, JsonRequestBehavior.AllowGet);
            }

            return ErrorResult;
        }

        public ActionResult Update(customer c)
        {
            var isAuth = HttpContext.User.Identity.IsAuthenticated;
            if (!isAuth)
            {
                return new HttpUnauthorizedResult();
            }

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            var _c = Uof.IcustomerService.GetById(c.id);

            if (_c.name == c.name &&
                _c.address == c.address &&
                _c.city == c.city &&
                _c.contact == c.contact &&
                _c.county == c.county &&
                _c.description == c.description &&
                _c.email == c.email &&
                _c.fax == c.fax &&
                _c.industry == c.industry &&
                _c.mobile == c.mobile &&
                _c.province == c.province &&
                _c.QQ == c.QQ &&
                _c.source == c.source &&
                _c.source_id == c.source_id &&
                _c.salesman_id == c.salesman_id &&
                _c.tel == c.tel &&
                _c.wechat == c.wechat
                )
            {
                return Json(new { id = _c.id }, JsonRequestBehavior.AllowGet);
            }

            _c.name = c.name;
            _c.address = c.address;
            _c.city = c.city;
            _c.contact = c.contact;
            _c.county = c.county;
            _c.description = c.description;
            _c.email = c.email;
            _c.fax = c.fax;
            _c.industry = c.industry;
            _c.mobile = c.mobile;
            _c.province = c.province;
            _c.QQ = c.QQ;
            _c.source = c.source;
            _c.salesman_id = c.salesman_id;

            if (c.source != "客户介绍")
            {
                _c.source_id = null;
            }
            else
            {
                _c.source_id = c.source_id;
            }

            _c.tel = c.tel;
            _c.wechat = c.wechat;
            _c.date_updated = DateTime.Now;

            var r = Uof.IcustomerService.UpdateEntity(_c);

            if (r)
            {
                Uof.Icustomer_timelineService.AddEntity(new customer_timeline
                {
                    title = "修改客户资料",
                    customer_id = _c.id,
                    content = string.Format("{0}修改了客户资料", arrs[3], _c.source),
                    date_business = DateTime.Now,
                    date_created = DateTime.Now,
                    is_system = 1
                });

                return Json(new { id = _c.id }, JsonRequestBehavior.AllowGet);
            }

            return ErrorResult;
        }

        public ActionResult Get(int id)
        {
            var _customer = Uof.IcustomerService.GetById(id);

            var source_name = "";
            if (_customer != null && _customer.source_id != null)
            {
                source_name = Uof.IcustomerService.GetAll(c => c.id == _customer.id).Select(c => c.name).FirstOrDefault();
            }

            var banks = Uof.Ibank_accountService.GetAll(b => b.customer_id == _customer.id).Select(b => new
            {
                id = b.id,
                customer_id = b.customer_id,
                bank = b.bank,
                holder = b.holder,
                account = b.account,
            }).ToList();

            return Json(new
            {
                id = _customer.id,
                code = _customer.code,
                name = _customer.name,
                industry = _customer.industry,
                province = _customer.province,
                city = _customer.city,
                county = _customer.county,
                address = _customer.address,
                contact = _customer.contact,
                mobile = _customer.mobile,
                tel = _customer.tel,
                fax = _customer.fax,
                email = _customer.email,
                QQ = _customer.QQ,
                wechat = _customer.wechat,
                source = _customer.source,
                creator_id = _customer.creator_id,
                salesman_id = _customer.salesman_id,
                organization_id = _customer.organization_id,
                source_id = _customer.source_id,
                source_name = source_name,
                description = _customer.description,
                salesman = _customer.member1.name,
                contacts = _customer.contacts,
                banks = banks, //_customer.bank_account

            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var c = Uof.IcustomerService.GetById(id);
            if (c == null)
            {
                return ErrorResult;
            }

            var r = Uof.IcustomerService.DeleteEntity(c);

            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

    }
}