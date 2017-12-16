using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.IO;

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

            var ops = arrs[4].Split(',');
            var strUserId = userId.ToString();
            //Expression<Func<customer, bool>> permQuery = c => true;
            //if (ops.Count() == 0)
            //{
            //    permQuery = c => (c.salesman_id == userId || c.assistant_id == userId || c.assistants == strUserId || (c.assistants.Contains(strUserId) && (c.assistants.Contains(strUserId + ",") || c.assistants.Contains("," + strUserId))));
            //}
            //else
            //{
            //    var hasCompany = ops.Where(o => o == "1").FirstOrDefault();
            //    if (hasCompany == null)
            //    {
            //        permQuery = c => (c.salesman_id == userId || c.assistant_id == userId || c.assistants == strUserId || (c.assistants.Contains(strUserId) && (c.assistants.Contains(strUserId + ",") || c.assistants.Contains("," + strUserId))));
            //    }
            //}


            Expression<Func<customer, bool>> condition = c => true;
            if (!string.IsNullOrEmpty(name))
            {
                Expression<Func<customer, bool>> tmp = c => (c.name.IndexOf(name) > -1 || c.code.IndexOf(name) > -1);
                condition = tmp;
            }

            var list = Uof.IcustomerService.GetAll(condition)
                //.Where(permQuery)
                .Where(c=>c.is_delete != 1)
                .OrderBy(item => item.id).Select(c => new
            {
                id = c.id,
                code = c.code,
                name = c.name
            }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.IcustomerService.GetAll(condition)
                //.Where(permQuery)
                .Where(c => c.is_delete != 1).Count();

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

        public ActionResult Search(int index = 1, int size = 10, string name = "", string type = "name")
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

            if (!string.IsNullOrEmpty(name) && type == "name")
            {
                nameQuery = c => (c.name.IndexOf(name) > -1 || c.code.IndexOf(name) > -1);
            }

            var userIds = new List<int>();
            if (type != "name" && !string.IsNullOrEmpty(name))
            {
                userIds = Uof.IcontactService.GetAll(t =>
                t.name.IndexOf(name) > -1 ||
                t.mobile.IndexOf(name) > -1 ||
                t.tel.IndexOf(name) > -1 ||
                t.email.IndexOf(name) > -1 ||
                t.QQ.IndexOf(name) > -1 ||
                t.wechat.IndexOf(name) > -1).Select(b => b.customer_id).ToList();
            }
            Expression<Func<customer, bool>> userIdsQuery = c => true;
            if (userIds.Count > 0)
            {
                userIdsQuery = c => userIds.Contains(c.id);
            }
            else
            {
                if (type != "name" && !string.IsNullOrEmpty(name))
                {
                    userIdsQuery = c => false;
                }
            }


            var ops = arrs[4].Split(',');
            var strUserId = userId.ToString();
            //Expression<Func<customer, bool>> permQuery = c => true;
            //if (ops.Count() == 0)
            //{
            //    permQuery = c => (c.salesman_id == userId || c.assistant_id == userId || c.assistants == strUserId || (c.assistants.Contains(strUserId) && (c.assistants.Contains(strUserId+",") || c.assistants.Contains("," + strUserId))));
            //}
            //else
            //{
            //    var hasCompany = ops.Where(o => o == "1").FirstOrDefault();
            //    if (hasCompany == null)
            //    {
            //        permQuery = c => (c.salesman_id == userId || c.assistant_id == userId || c.assistants == strUserId || (c.assistants.Contains(strUserId) && (c.assistants.Contains(strUserId + ",") || c.assistants.Contains("," + strUserId))));
            //    }
            //}

            

            var customerAllList = Uof.IcustomerService
                .GetAll(condition)
                .Where(nameQuery)
                .Where(c => c.is_delete != 1)
                .Where(userIdsQuery)
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
                    salesman_id = c.salesman_id,
                    salesman = c.member1.name,
                    source = c.source,
                    source_id = c.source_id,
                    source_name = "",
                    assistant_id = c.assistant_id,
                    assistant_name = "",
                    assistants = c.assistants,
                    date_created = c.date_created,
                }).ToList();

            var fullCustomerList = new List<Customer>();

            if (userId == 1)
            {
                fullCustomerList = customerAllList;
            }
            else
            {
                foreach (var item in customerAllList)
                {
                    var assIds = new List<int>();
                    if (!string.IsNullOrEmpty(item.assistants))
                    {
                        var assList = item.assistants.Split(',');
                        foreach (var ass in assList)
                        {
                            int aid = 0;
                            int.TryParse(ass, out aid);
                            assIds.Add(aid);
                        }
                    }
                    item.assistantIds = assIds;

                    if (ops.Count() == 0)
                    {
                        if (item.salesman_id == userId || item.assistant_id == userId || item.assistantIds.Contains(userId))
                        {
                            fullCustomerList.Add(item);
                        }
                    }
                    else
                    {
                        var hasCompany = ops.Where(o => o == "1").FirstOrDefault();
                        if (hasCompany == null)
                        {
                            if (item.salesman_id == userId || item.assistant_id == userId || item.assistantIds.Contains(userId))
                            {
                                fullCustomerList.Add(item);
                            }
                        }
                        else
                        {
                            fullCustomerList = customerAllList;
                        }
                    }
                }
            }
            

            //Expression<Func<Customer, bool>> permQuery = c => true;
            //if (ops.Count() == 0)
            //{
            //    permQuery = c => (c.salesman_id == userId || c.assistant_id == userId || c.assistantIds.Contains(userId));
            //}
            //else
            //{
            //    var hasCompany = ops.Where(o => o == "1").FirstOrDefault();
            //    if (hasCompany == null)
            //    {
            //        permQuery = c => (c.salesman_id == userId || c.assistant_id == userId || c.assistantIds.Contains(userId));
            //    }
            //}

            //var list = Uof.IcustomerService.GetAll(condition)
            //    .Where(nameQuery)
            //    .Where(permQuery)
            //    .Where(c => c.is_delete != 1)
            //    .OrderByDescending(item => item.id).Select(c => new Customer()
            //    {
            //        id = c.id,
            //        code = c.code,
            //        name = c.name,
            //        contact = c.contact,
            //        mobile = c.mobile,
            //        tel = c.tel,
            //        industry = c.industry,
            //        province = c.province,
            //        city = c.city,
            //        county = c.county,
            //        address = c.address,
            //        salesman_id = c.salesman_id,
            //        salesman = c.member1.name,
            //        source = c.source,
            //        source_id = c.source_id,
            //        source_name = "",
            //        assistant_id = c.assistant_id,
            //        assistant_name = "",
            //        assistants = c.assistants,
            //        date_created = c.date_created,
            //    }).ToPagedList(index, size).ToList();

            //var totalRecord = Uof.IcustomerService.GetAll(condition)
            //    .Where(nameQuery)
            //    .Where(permQuery)
            //    .Where(c => c.is_delete != 1)
            //    .Count();

            var list = fullCustomerList.OrderByDescending(item => item.id).Skip((index - 1) * size).Take(size).ToList();
            var totalRecord = fullCustomerList.Count();

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

                foreach (var item in list)
                {
                    var total = GetBusinessCountByCustomerId(item.id);
                    item.business_count = total;
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
            //Expression<Func<customer, bool>> permQuery = c => true;
            var strUserId = userId.ToString();

            //if (userId != 1)
            //{
            //    permQuery = c => (c.salesman_id == userId || c.assistant_id == userId || c.assistants == strUserId || (c.assistants.Contains(strUserId) && (c.assistants.Contains(strUserId + ",") || c.assistants.Contains("," + strUserId))));
            //}            

            if (!string.IsNullOrEmpty(name))
            {
                nameQuery = c => (c.name.IndexOf(name) > -1 || c.code.IndexOf(name) > -1);
            }

            var customerAllList = Uof.IcustomerService
                .GetAll(condition)
                .Where(nameQuery)
                .Where(c => c.is_delete != 1)
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
                    salesman_id = c.salesman_id,
                    salesman = c.member1.name,
                    source = c.source,
                    source_id = c.source_id,
                    source_name = "",
                    assistant_id = c.assistant_id,
                    assistant_name = "",
                    assistants = c.assistants,
                    date_created = c.date_created,
                    mailling_address = c.mailling_address,
                    mailling_province = c.mailling_province,
                    mailling_city = c.mailling_city,
                    mailling_county = c.mailling_county,
                }).ToList();

            var fullCustomerList = new List<Customer>();

            if (userId == 1)
            {
                fullCustomerList = customerAllList;
            }
            else
            {
                foreach (var item in customerAllList)
                {
                    var assIds = new List<int>();
                    if (!string.IsNullOrEmpty(item.assistants))
                    {
                        var assList = item.assistants.Split(',');
                        foreach (var ass in assList)
                        {
                            int aid = 0;
                            int.TryParse(ass, out aid);
                            assIds.Add(aid);
                        }
                    }
                    item.assistantIds = assIds;

                    if (userId != 1)
                    {
                        if (item.salesman_id == userId || item.assistant_id == userId || item.assistantIds.Contains(userId))
                        {
                            fullCustomerList.Add(item);
                        }
                    }
                }
            }
            

            var list = fullCustomerList.OrderByDescending(item => item.id).Skip((index - 1) * size).Take(size).ToList();
            var totalRecord = fullCustomerList.Count();

            //var list = Uof.IcustomerService.GetAll(condition)
            //    .Where(nameQuery)
            //    .Where(permQuery)
            //    .Where(c => c.is_delete != 1)
            //    .OrderByDescending(item => item.id).Select(c => new
            //    {
            //        id = c.id,
            //        code = c.code,
            //        name = c.name,
            //        contact = c.contact,
            //        mobile = c.mobile,
            //        tel = c.tel,
            //        industry = c.industry,
            //        province = c.province,
            //        city = c.city,
            //        county = c.county,
            //        address = c.address,

            //        mailling_address = c.mailling_address,
            //        mailling_province = c.mailling_province,
            //        mailling_city = c.mailling_city,
            //        mailling_county = c.mailling_county,

            //        salesman_id = c.salesman_id,
            //        salesman = c.member1.name,
            //    }).ToPagedList(index, size).ToList();

            //var totalRecord = Uof.IcustomerService.GetAll(condition).Where(nameQuery).Where(c => c.is_delete != 1).Count();

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

        public ActionResult Add(customer c, List<contact> contacts)
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
            c.is_delete = 0;
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

                if (contacts != null && contacts.Count() > 0)
                {
                    var newContacts = new List<contact>();
                    foreach (var item in contacts)
                    {
                        newContacts.Add(new contact()
                        {
                            name = item.name,
                            customer_id = _c.id,
                            date_created = DateTime.Now,
                            email = item.email,
                            mobile = item.mobile,
                            position = item.position,
                            QQ = item.QQ,
                            tel = item.tel,
                            type = item.type,
                            wechat = item.wechat
                        });
                    }

                    Uof.IcontactService.AddEntities(newContacts);
                }
                
                return Json(new { id = c.id }, JsonRequestBehavior.AllowGet);
            }

            return ErrorResult;
        }

        public ActionResult Update(customer c, List<contact> contacts)
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

            //if (_c.name == c.name &&
            //    _c.address == c.address &&
            //    _c.city == c.city &&
            //    _c.contact == c.contact &&
            //    _c.county == c.county &&
            //    _c.description == c.description &&
            //    _c.email == c.email &&
            //    _c.fax == c.fax &&
            //    _c.industry == c.industry &&
            //    _c.mobile == c.mobile &&
            //    _c.province == c.province &&
            //    _c.QQ == c.QQ &&
            //    _c.source == c.source &&
            //    _c.source_id == c.source_id &&
            //    _c.salesman_id == c.salesman_id &&
            //    _c.tel == c.tel &&
            //    _c.wechat == c.wechat &&
            //    _c.contacts == c.contacts &&
            //    _c.assistant_id == c.assistant_id &&
            //    _c.assistants == c.assistants
            //    )
            //{
            //    return Json(new { id = _c.id }, JsonRequestBehavior.AllowGet);
            //}

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
            //_c.contacts = c.contacts;
            _c.assistant_id = c.assistant_id;
            _c.assistants = c.assistants;

            _c.mailling_address = c.mailling_address;
            _c.mailling_province = c.mailling_province;
            _c.mailling_city = c.mailling_city;
            _c.mailling_county = c.mailling_county;

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

                #region 联系人
                var dbContacts = Uof.IcontactService.GetAll(s => s.customer_id == _c.id).ToList();

                var newContacts = new List<contact>();
                var deleteContacts = new List<contact>();
                var updateContacts = new List<contact>();

                if (contacts != null && contacts.Count() > 0)
                {
                    foreach (var item in contacts)
                    {
                        if (item.id == 0)
                        {
                            newContacts.Add(new contact
                            {
                                customer_id = _c.id,
                                name = item.name,
                                email = item.email,
                                mobile = item.mobile,
                                position = item.position,
                                QQ = item.QQ,
                                tel = item.tel,
                                type = item.type,
                                wechat = item.wechat,
                                date_created = DateTime.Today,
                                memo = item.memo,
                                responsable = item.responsable
                            });
                        }

                        if (item.id > 0)
                        {
                            var updateContact = dbContacts.Where(d => d.id == item.id).FirstOrDefault();
                            if (updateContact != null)
                            {
                                updateContact.name = item.name;
                                updateContact.email = item.email;
                                updateContact.mobile = item.mobile;
                                updateContact.position = item.position;
                                updateContact.QQ = item.QQ;
                                updateContact.tel = item.tel;
                                updateContact.type = item.type;
                                updateContact.wechat = item.wechat;
                                updateContact.memo = item.memo;
                                updateContact.responsable = item.responsable;

                                updateContact.date_updated = DateTime.Now;

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
                            Uof.IcontactService.DeleteEntity(item);
                        }
                    }

                    if (updateContacts.Count > 0)
                    {
                        Uof.IcontactService.UpdateEntities(updateContacts);
                    }

                    if (newContacts.Count > 0)
                    {
                        Uof.IcontactService.AddEntities(newContacts);
                    }
                }
                catch (Exception)
                {
                }

                #endregion

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
                //contacts = _customer.contacts,
                banks = banks,
                assistants = _customer.assistants,
                assistant_id = _customer.assistant_id,
                assistant_name = assistant_name,

                mailling_address = _customer.mailling_address,
                mailling_province = _customer.mailling_province,
                mailling_city = _customer.mailling_city,
                mailling_county = _customer.mailling_county,
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

            var contacts = Uof.IcontactService.GetAll(c => c.customer_id == id).Select(d => new
            {
                id = d.id,
                customer_id = d.customer_id,
                name = d.name,
                mobile = d.mobile,
                tel = d.tel,
                position = d.position,
                email = d.email,
                wechat = d.wechat,
                QQ = d.QQ,
                type = d.type,
                memo = d.memo,
                responsable = d.responsable,
            }).ToList();
            return Json(new { customer = customerEntity, contacts = contacts }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var c = Uof.IcustomerService.GetById(id);
            if (c == null)
            {
                return ErrorResult;
            }

            var r = Uof.IcustomerService.DeleteEntity(c);
            if (r)
            {
                var contacts = Uof.IcontactService.GetAll(a => a.customer_id == id).ToList();
                if (contacts != null && contacts.Count > 0)
                {
                    foreach (var item in contacts)
                    {
                        Uof.IcontactService.DeleteEntity(item);
                    }
                }
            }

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
                .Where(c => c.is_delete != 1)
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
                    address = c.address,

                    mailling_address = c.mailling_address,
                    mailling_province = c.mailling_province,
                    mailling_city = c.mailling_city,
                    mailling_county = c.mailling_county,

                }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.IcustomerService.GetAll(condition).Where(c => c.is_delete != 1).Count();

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
            List<FinanceCheck> orders = GetCustomerBusinessList(customerId);

            return Json(orders, JsonRequestBehavior.AllowGet);
        }

        private List<FinanceCheck> GetCustomerBusinessList(int customerId)
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

            ///
            var accountings = Uof.IaccountingService
                .GetAll(a => a.customer_id == customerId)
                .OrderByDescending(a => a.code)
                .Select(a => new FinanceCheck
                {
                    id = a.id,
                    order_code = a.code,
                    order_name = a.name,
                    order_type = "accounting",
                    order_type_name = "国内记账",
                    review_status = a.review_status,
                    status = a.status,
                    salesman = a.member5.name,
                    waitor = "-",
                    amount_transaction = a.amount_transaction,
                    date_transaction = a.date_transaction,
                    date_created = a.date_created,
                    date_setup = a.date_created,
                }).ToList();
            if (accountings.Count() > 0)
            {
                orders.AddRange(accountings);
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
            return orders;
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

            var _c = new
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
                //contacts = _customer.contacts,
                assistant_name = assistant_name,

                mailling_address = _customer.mailling_address,
                mailling_province = _customer.mailling_province,
                mailling_city = _customer.mailling_city,
                mailling_county = _customer.mailling_county,

            };

            var contacts = Uof.IcontactService.GetAll(c => c.customer_id == customer_id).Select(d => new
            {
                id = d.id,
                customer_id = d.customer_id,
                name = d.name,
                mobile = d.mobile,
                tel = d.tel,
                position = d.position,
                email = d.email,
                wechat = d.wechat,
                QQ = d.QQ,
                type = d.type,
                memo = d.memo,
                responsable = d.responsable,
            }).ToList();

            List<FinanceCheck> orders = GetCustomerBusinessList(customer_id);

            return Json(new { customer = _c, contacts = contacts, orders = orders }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateShortInfo(ShortInfoCustomer info)
        {
            var dbCust = Uof.IcustomerService.GetAll(c => c.id == info.id).FirstOrDefault();

            dbCust.province = info.province;
            dbCust.city = info.city;
            dbCust.county = info.county;
            dbCust.address = info.address;
            dbCust.fax = info.fax;

            //dbCust.contact = info.contact;
            //dbCust.mobile = info.mobile;
            //dbCust.tel = info.tel;
            //dbCust.fax = info.fax;
            //dbCust.email = info.email;
            //dbCust.QQ = info.QQ;
            //dbCust.wechat = info.wechat;

            dbCust.mailling_address = info.mailling_address;
            dbCust.mailling_province = info.mailling_province;
            dbCust.mailling_city = info.mailling_city;
            dbCust.mailling_county = info.mailling_county;

            dbCust.date_updated = DateTime.Now;

            Uof.IcustomerService.UpdateEntity(dbCust);

            return SuccessResult;
        }

        public ActionResult UpdateOldConatacts()
        {
            var customers = Uof.IcustomerService.GetAll();

            var contactList = new List<contact>();
            foreach (var item in customers)
            {                
                if (!string.IsNullOrEmpty(item.contact))
                {
                    contactList.Add(new contact()
                    {
                        customer_id = item.id,
                        name = item.contact,
                        date_created = item.date_created,
                        email = item.email,
                        mobile = item.mobile,
                        position = null,
                        QQ = item.QQ,
                        tel = item.tel,
                        type = "main",
                        wechat = item.wechat,
                    });
                }

                if (item.contacts != null && item.contacts != "[]" && item.contacts != "{}" && item.contacts.Length > 0 && !string.IsNullOrEmpty(item.contacts))
                {
                    JavaScriptSerializer jsonSerialize = new JavaScriptSerializer();
                    var list = jsonSerialize.Deserialize<List<Contact>>(item.contacts);
                    if (list.Count > 0)
                    {
                        foreach (var l in list)
                        {
                            contactList.Add(new contact()
                            {
                                customer_id = item.id,
                                name = l.name,
                                date_created = item.date_created,
                                email = l.email ?? "",
                                mobile = l.mobile ?? "",
                                position = l.position ?? "",
                                QQ = l.QQ ?? "",
                                tel = l.tel ?? "",
                                type = "other",
                                wechat = l.wechat,
                            });
                        }
                    }
                }                
            }

            if (contactList != null && contactList.Count > 0)
            {
                var updates = Uof.IcontactService.AddEntities(contactList);
            }

            return SuccessResult;
        }
        [HttpPost]
        public ActionResult UpdateContact(contact item)
        {
            if (item.id == 0)
            {
                var dbItem = Uof.IcontactService.AddEntity(item);
                return Json(dbItem, JsonRequestBehavior.AllowGet);
            }

            var d = Uof.IcontactService.GetById(item.id);
            d.email = item.email;
            d.memo = item.memo;
            d.mobile = item.mobile;
            d.name = item.name;
            d.position = item.position;
            d.QQ = item.QQ;
            d.tel = item.tel;
            d.wechat = item.wechat;
            d.date_updated = DateTime.Now;
            d.memo = item.memo;
            d.responsable = item.responsable;
            Uof.IcontactService.UpdateEntity(d);

            return Json(d, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TransferBack(int id)
        {
            var IsAuth = HttpContext.User.Identity.IsAuthenticated;
            if (!IsAuth)
            {
                return new HttpUnauthorizedResult();
            }

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                return new HttpUnauthorizedResult();
            }

            var c = Uof.IcustomerService.GetById(id);
            if (c == null)
            {
                return ErrorResult;
            }

            // TODO: 生成客户编码
            //c.code = GetNextCustomerCode(c.salesman_id.Value);
            c.status = 0;
            c.date_updated = DateTime.Now;

            var r = Uof.IcustomerService.UpdateEntity(c);

            if (r)
            {
                Uof.Icustomer_timelineService.AddEntity(new customer_timeline
                {
                    title = "转回意向客户",
                    customer_id = c.id,
                    content = string.Format("由正式客户转回意向客户, 操作人：{0}", arrs[3]),
                    date_business = DateTime.Now,
                    date_created = DateTime.Now,
                    is_system = 1
                });
            }

            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public FileStreamResult Export()
        {
            var r = HttpContext.User.Identity.IsAuthenticated;
            if (!r)
            {
                throw new Exception("您没登录");
            }

            var identityName = HttpContext.User.Identity.Name;
            var arrs = identityName.Split('|');
            if (arrs.Length == 0)
            {
                throw new Exception("您没登录");
            }

            if (arrs.Length < 5)
            {
                throw new Exception("您没登录");
            }

            var userId = 0;
            var deptId = 0;
            int.TryParse(arrs[0], out userId);
            int.TryParse(arrs[2], out deptId);

            Expression<Func<customer, bool>> condition = c => c.status == 1 && c.is_delete != 1;

            var list = Uof.IcustomerService.GetAll(condition)
                .OrderByDescending(item => item.id).Select(c => new Customer()
                {
                    id = c.id,
                    code = c.code,
                    name = c.name,
                    mobile = c.mobile,
                    tel = c.tel,
                    industry = c.industry,
                    province = c.province,
                    city = c.city,
                    county = c.county,
                    address = c.address,
                    salesman = c.member1.name,
                    source = c.source,
                    source_name = "",
                    date_created = c.date_created,
                    business_nature = c.business_nature,
                   
                }).OrderBy(c => c.id).ToList();

            if (list != null && list.Count() > 0)
            {
                var exportList = list.Select(c => new ExcelCustomerContact
                {
                    ID = c.id,
                    客户名称 = c.name,
                    业务性质 = c.business_nature,
                    行业类别 = c.industry,
                    客户来源 = c.source,
                    省份 = c.province,
                    城市 = c.city,
                    地区 = c.county,
                    地址 = c.address,
                    业务员 = c.salesman,
                    创建日期 = c.date_created.Value.ToString("yyyy-MM-dd")
                }).ToList();

                var sheet = ExportToExcel(exportList);
                var fileName = "客户列表.xml";
                var bytes = GenerateStreamFromString(sheet);
                return File(bytes, "application/xml", fileName);
            }
            throw new Exception("您没登录");
        }
        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private int GetBusinessCountByCustomerId(int customerId)
        {
            var total = 0;

            var regAborads = Uof.Ireg_abroadService
                .GetAll(a => a.customer_id == customerId)
                .OrderByDescending(a => a.code)
                .Count();

            total += regAborads;

            var regInterals = Uof.Ireg_internalService
                .GetAll(a => a.customer_id == customerId)
                .OrderByDescending(a => a.code)
                .Count();

            total += regInterals;

            var trademarks = Uof.ItrademarkService
                .GetAll(a => a.customer_id == customerId)
                .OrderByDescending(a => a.code)
                .Count();

            total += trademarks;

            var patents = Uof.IpatentService
                .GetAll(a => a.customer_id == customerId)
                .OrderByDescending(a => a.code)
                .Count();

            total += patents;

            var audits = Uof.IauditService
                .GetAll(a => a.customer_id == customerId)
                .OrderByDescending(a => a.code)
                .Count();

            total += audits;

            var accs = Uof.IaccountingService
                .GetAll(a => a.customer_id == customerId)
                .OrderByDescending(a => a.code)
                .Count();

            total += accs;

            return total;
        }
    }
}