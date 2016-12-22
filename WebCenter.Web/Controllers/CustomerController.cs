﻿using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;

namespace WebCenter.Web.Controllers
{
    public class CustomerController : BaseController
    {
        public CustomerController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        public ActionResult Introducers(int index = 1, int size = 10, string name = "")
        {
            Expression<Func<customer, bool>> condition = c => true;
            if (!string.IsNullOrEmpty(name))
            {
                Expression<Func<customer, bool>> tmp = c => (c.name.IndexOf(name) > -1 || c.code.IndexOf(name) > -1);
                condition = tmp;
            }

            var list = Uof.IcustomerService.GetAll(condition).OrderBy(item => item.id).Select(c => new
            {
                id = c.id,
                code = c.code,
                name = c.name
            }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.IcustomerService.GetAll(condition).Count();

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

        public ActionResult Search(int index = 1, int size = 10, string name = "")
        {
            size = 10;
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

            if (arrs.Length < 5)
            {
                return new HttpUnauthorizedResult();
            }

            var userId = 0;
            var deptId = 0;
            int.TryParse(arrs[0], out userId);
            int.TryParse(arrs[2], out deptId);

            Expression<Func<customer, bool>> condition = c => c.status == 1; // && c.salesman_id == userId;
            Expression<Func<customer, bool>> nameQuery = c => true;
                
            if (!string.IsNullOrEmpty(name))
            {
                nameQuery = c => (c.name.IndexOf(name) > -1 || c.code.IndexOf(name) > -1);
            }

            var ops = arrs[4].Split(',');

            Expression<Func<customer, bool>> permQuery = c => true;
            if (ops.Count() == 0)
            {
                var strUserId = userId.ToString();
                permQuery = c => (c.salesman_id == userId || c.assistant_id == userId || c.assistants.Contains(strUserId));
            }
            else
            {
                var hasCompany = ops.Where(o => o == "1").FirstOrDefault();
                var hasDepart = ops.Where(o => o == "2").FirstOrDefault();
                if (hasCompany == null)
                {
                    if (hasDepart == null)
                    {
                        permQuery = c => (c.salesman_id == userId || c.assistant_id == userId);
                    }
                    else
                    {
                        var ids = GetChildrenDept(deptId);
                        if (ids.Count > 0)
                        {
                            permQuery = c => c.organization_id == deptId;
                        }
                        else
                        {
                            permQuery = c => ids.Contains(c.organization_id.Value);
                        }
                    }
                }
            }

            var list = Uof.IcustomerService.GetAll(condition)
                .Where(nameQuery)
                .Where(permQuery)
                .OrderByDescending(item => item.id).Select(c => new Customer()
                {
                id = c.id,
                code = c.code,
                name = c.name,
                contact = c.contact,
                mobile = c.mobile,
                tel = c.tel,
                industry = c.industry,
                province = c.province,
                city = c.city,
                county = c.county,
                address = c.address,

                salesman = c.member1.name,
                source = c.source,
                source_id = c.source_id,
                source_name = "",
                assistant_id = c.assistant_id,
                assistant_name = "",
                assistants = c.assistants,
                }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.IcustomerService.GetAll(condition)
                .Where(nameQuery)
                .Where(permQuery).Count();

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

            if (list.Count > 0)
            {
                var sourceIds = list.Where(l => l.source_id != null).Select(l => l.source_id).Distinct().ToList();
                if (sourceIds != null && sourceIds.Count() > 0)
                {
                    var customers = Uof.IcustomerService.GetAll(c => sourceIds.Contains(c.id)).Select(c => new
                    {
                        id = c.id,
                        name = c.name
                    }).ToList();
                    if (customers != null && customers.Count() > 0)
                    {
                        foreach (var item in customers)
                        {
                            var tls = list.Where(l => l.source_id == item.id).ToList();
                            foreach (var tl in tls)
                            {
                                tl.source_name = item.name;
                            }
                        }
                    }
                }

                var memberIds = list.Where(l => l.assistant_id != null).Select(l => l.assistant_id).Distinct().ToList();
                if (memberIds != null && memberIds.Count > 0)
                {
                    var members = Uof.ImemberService.GetAll(c => memberIds.Contains(c.id)).Select(c => new
                    {
                        id = c.id,
                        name = c.name
                    }).ToList();
                    if (members != null && members.Count > 0)
                    {
                        foreach (var item in members)
                        {
                            var tls = list.Where(l => l.assistant_id == item.id).ToList();
                            foreach (var tl in tls)
                            {
                                tl.assistant_name = item.name;
                            }
                        }
                    }
                }
            }

            var result = new
            {
                page = page,
                items = list
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchForDropdown(int index = 1, int size = 10, string name = "")
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
            if (arrs.Length < 5)
            {
                return new HttpUnauthorizedResult();
            }

            var userId = 0;
            int.TryParse(arrs[0], out userId);

            Expression<Func<customer, bool>> condition = c => c.status == 1;
            Expression<Func<customer, bool>> nameQuery = c => true;
            Expression<Func<customer, bool>> permQuery = c => true;
            var strUserId = userId.ToString();
            permQuery = c => (c.salesman_id == userId || c.assistant_id == userId || c.assistants.Contains(strUserId));

            if (!string.IsNullOrEmpty(name))
            {
                nameQuery = c => (c.name.IndexOf(name) > -1 || c.code.IndexOf(name) > -1);
            }
            
            var list = Uof.IcustomerService.GetAll(condition)
                .Where(nameQuery)
                .Where(permQuery)
                .OrderByDescending(item => item.id).Select(c => new
                {
                    id = c.id,
                    code = c.code,
                    name = c.name,
                    contact = c.contact,
                    mobile = c.mobile,
                    tel = c.tel,
                    industry = c.industry,
                    province = c.province,
                    city = c.city,
                    county = c.county,
                    address = c.address
                }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.IcustomerService.GetAll(condition).Where(nameQuery).Count();

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

                return Json(new { id = c.id }, JsonRequestBehavior.AllowGet);
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
                _c.wechat == c.wechat &&
                _c.contacts == c.contacts &&
                _c.assistant_id == c.assistant_id &&
                _c.assistants == c.assistants
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
            _c.contacts = c.contacts;
            _c.assistant_id = c.assistant_id;
            _c.assistants = c.assistants;

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
            var assistant_name = "";
            if (_customer != null && _customer.source_id != null)
            {
                source_name = Uof.IcustomerService.GetAll(c => c.id == _customer.source_id).Select(c => c.name).FirstOrDefault();
            }
            if (_customer != null && _customer.assistant_id != null)
            {
                assistant_name = Uof.ImemberService.GetAll(c => c.id == _customer.assistant_id).Select(c => c.name).FirstOrDefault();
            }

            var banks = Uof.Ibank_accountService.GetAll(b => b.customer_id == _customer.id).Select(b => new Bank
            {
                id = b.id,
                customer_id = b.customer_id,
                bank = b.bank,
                holder = b.holder,
                account = b.account,
            }).ToList();

            var customerEntity = new Customer()
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
                banks = banks,
                assistants = _customer.assistants,
                assistant_id = _customer.assistant_id,
                assistant_name = assistant_name
            };

            if (!string.IsNullOrEmpty(customerEntity.assistants))
            {
                var assistantIds = new List<int>();
                var ids = customerEntity.assistants.Split(',');
                if (ids.Length > 0)
                {
                    foreach (var item in ids)
                    {
                        int _id = 0;
                        int.TryParse(item, out _id);
                        assistantIds.Add(_id);
                    }

                    var _members = Uof.ImemberService.GetAll(m => assistantIds.Contains(m.id)).Select(m => new Assistant()
                    {
                        id = m.id,
                        name = m.name
                    }).ToList();

                    customerEntity.assistantList = _members;
                }
            }
            
            return Json(customerEntity, JsonRequestBehavior.AllowGet);
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

        public ActionResult Bank(int id)
        {
            var bank = Uof.Ibank_accountService.GetAll(b => b.id == id).Select(b => new
            {
                id = b.id,
                customer_id = b.customer_id,
                bank = b.bank,
                holder = b.holder,
                account = b.account
            }).FirstOrDefault();

            return Json(bank, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteBank(int id)
        {
            var bank = Uof.Ibank_accountService.GetAll(b => b.id == id).FirstOrDefault();

            var r = Uof.Ibank_accountService.DeleteEntity(bank);

            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddBank(int customer_id, string bank, string holder, string account)
        {
            var _bank = new bank_account
            {
                customer_id = customer_id,
                bank = bank,
                holder = holder,
                account = account
            };

            Uof.Ibank_accountService.AddEntity(_bank);

            return SuccessResult;
        }

        [HttpPost]
        public ActionResult UpdateBank(int id, string bank, string holder, string account)
        {
            var _bank = Uof.Ibank_accountService.GetById(id);
            if (_bank.bank == bank &&_bank.holder == holder && _bank.account == account)
            {
                return SuccessResult;
            }

            _bank.bank = bank;
            _bank.holder = holder;
            _bank.account = account;

            var r = Uof.Ibank_accountService.UpdateEntity(_bank);

            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Banks(int customer_id, string name = "")
        {
            Expression<Func<bank_account, bool>> condition = c => c.customer_id == customer_id;
            Expression<Func<bank_account, bool>> nameQuery = c => true;
            if (!string.IsNullOrEmpty(name))
            {
                nameQuery = c => (c.bank.IndexOf(name) > -1);
            }

            var list = Uof.Ibank_accountService.GetAll(condition)
                .Where(nameQuery)
                .OrderBy(item => item.id).Select(c => new
            {
                id = c.id,
                name = c.bank,
                holder = c.holder,
                account = c.account
            }).ToList();

            var result = new
            {
                items = list
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DropDownSearch(int index = 1, int size = 10, string name = "")
        {
            Expression<Func<customer, bool>> condition = c => c.status == 1;
            Expression<Func<customer, bool>> nameQuery = c => true;

            if (!string.IsNullOrEmpty(name))
            {
                nameQuery = c => (c.name.IndexOf(name) > -1 || c.code.IndexOf(name) > -1);
            }

            var list = Uof.IcustomerService.GetAll(condition)
                .Where(nameQuery)
                .OrderByDescending(item => item.id).Select(c => new
                {
                    id = c.id,
                    code = c.code,
                    name = c.name,
                    contact = c.contact,
                    mobile = c.mobile,
                    tel = c.tel,
                    industry = c.industry,
                    province = c.province,
                    city = c.city,
                    county = c.county,
                    address = c.address
                }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.IcustomerService.GetAll(condition).Count();

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

        public ActionResult GetBusinessByCustomerId(int customerId)
        {
            var orders = new List<FinanceCheck>();

            var regAborads = Uof.Ireg_abroadService
                .GetAll(a => a.customer_id == customerId)
                .OrderByDescending(a => a.code)
                .Select(a => new FinanceCheck
                {
                    id = a.id,
                    order_code = a.code,
                    order_name = a.name_cn ?? a.name_en,
                    order_name_en = a.name_en,
                    order_type = "reg_abroad",
                    order_type_name = "境外注册",
                    review_status = a.review_status,
                    status = a.status,
                    salesman = a.member4.name,
                    waitor = a.member6.name,
                    date_created = a.date_created,
                    date_setup = a.date_setup,
                }).ToList();

            if (regAborads.Count() > 0)
            {
                orders.AddRange(regAborads);
            }

            var regInterals = Uof.Ireg_internalService
                .GetAll(a => a.customer_id == customerId)
                .OrderByDescending(a => a.code)
                .Select(a => new FinanceCheck
                {
                    id = a.id,
                    order_code = a.code,
                    order_name = a.name_cn,
                    order_type = "reg_internal",
                    order_type_name = "境内注册",
                    review_status = a.review_status,
                    status = a.status,
                    salesman = a.member4.name,
                    waitor = a.member6.name,
                    amount_transaction = a.amount_transaction,
                    date_transaction = a.date_transaction,
                    date_created = a.date_created,
                    date_setup = a.date_setup,
                }).ToList();

            if (regInterals.Count() > 0)
            {
                orders.AddRange(regInterals);
            }

            var trademarks = Uof.ItrademarkService
                .GetAll(a => a.customer_id == customerId)
                .OrderByDescending(a => a.code)
                .Select(a => new FinanceCheck
                {
                    id = a.id,                    
                    order_code = a.code,
                    order_name = a.name,
                    order_type = "trademark",
                    order_type_name = "商标",
                    review_status = a.review_status,
                    status = a.status,
                    salesman = a.member3.name,
                    waitor = a.member5.name,
                    amount_transaction = a.amount_transaction,
                    date_transaction = a.date_transaction,
                    date_created = a.date_created,
                    date_setup = a.date_regit,
                }).ToList();

            if (trademarks.Count() > 0)
            {
                orders.AddRange(trademarks);
            }

            var patents = Uof.IpatentService
                .GetAll(a => a.customer_id == customerId)
                .OrderByDescending(a => a.code)
                .Select(a => new FinanceCheck
                {
                    id = a.id,
                    order_code = a.code,
                    order_name = a.name,
                    order_name_en = "",
                    order_type = "patent",
                    order_type_name = "专利",
                    review_status = a.review_status,
                    status = a.status,
                    salesman = a.member3.name,
                    waitor = a.member5.name,
                    amount_transaction = a.amount_transaction,
                    date_transaction = a.date_transaction,
                    date_created = a.date_created,
                    date_setup = a.date_regit,
                }).ToList();

            if (patents.Count() > 0)
            {
                orders.AddRange(patents);
            }

            var audits = Uof.IauditService
                .GetAll(a => a.customer_id == customerId)
                .OrderByDescending(a => a.code)
                .Select(a => new FinanceCheck
                {
                    id = a.id,
                    order_code = a.code,
                    order_name = a.name_cn,
                    order_type = "audit",
                    order_type_name = "审计",
                    review_status = a.review_status,
                    status = a.status,
                    salesman = a.member4.name,
                    waitor = "-",
                    amount_transaction = a.amount_transaction,
                    date_transaction = a.date_transaction,
                    date_created = a.date_created,
                    date_setup = a.date_setup,
                }).ToList();
            if (audits.Count() > 0)
            {
                orders.AddRange(audits);
            }

            //var annuals = Uof.Iannual_examService.GetAll(a => a.customer_id == customerId).OrderByDescending(a => a.id).Select(a => a.date_transaction).ToList();
            //if (annuals.Count() > 0)
            //{
            //    orders.Add(new CustomerOrder
            //    {
            //        count = annuals.Count(),
            //        last_date = annuals.FirstOrDefault(),
            //        order_name = "年检"
            //    });
            //}

            return Json(orders, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetShortInfo(int customer_id)
        {
            var _customer = Uof.IcustomerService.GetAll(c => c.id == customer_id).FirstOrDefault();

            var source_name = "";
            var assistant_name = "";
            if (_customer != null && _customer.source_id != null)
            {
                source_name = Uof.IcustomerService.GetAll(c => c.id == _customer.id).Select(c => c.name).FirstOrDefault();
            }
            if (_customer != null && _customer.assistant_id != null)
            {
                assistant_name = Uof.ImemberService.GetAll(c => c.id == _customer.assistant_id).Select(c => c.name).FirstOrDefault();
            }

            return Json(new
            {
                id = _customer.id,
                //code = _customer.code,
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
                assistant_name = assistant_name
            }, JsonRequestBehavior.AllowGet);
        }
    }
}