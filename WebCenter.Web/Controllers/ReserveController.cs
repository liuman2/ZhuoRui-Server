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
    public class ReserveController : BaseController
    {
        public ReserveController(IUnitOfWork UOF)
            : base(UOF)
        {

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

            Expression<Func<customer, bool>> condition = c => c.status == 0;
            Expression<Func<customer, bool>> nameQuery = c => true;
            if (!string.IsNullOrEmpty(name))
            {
                nameQuery = c => (c.name.IndexOf(name) > -1);
            }

            var ops = arrs[4].Split(',');
                        
            Expression<Func<customer, bool>> permQuery = c => true;
            if (ops.Count() == 0)
            {
                permQuery = c => (c.salesman_id == userId || c.assistant_id == userId);
            } else
            {
                var hasCompany = ops.Where(o => o == "1").FirstOrDefault();
                var hasDepart = ops.Where(o => o == "2").FirstOrDefault();
                if (hasCompany == null)
                {
                    permQuery = c => (c.salesman_id == userId || c.assistant_id == userId);
                    //if (hasDepart == null)
                    //{
                    //    permQuery = c => (c.salesman_id == userId || c.assistant_id == userId);
                    //} else
                    //{
                    //    var ids = GetChildrenDept(deptId);
                    //    if (ids.Count > 0)
                    //    {
                    //        permQuery = c => c.organization_id == deptId;
                    //    }
                    //    else
                    //    {
                    //        permQuery = c => ids.Contains(c.organization_id.Value);
                    //    }
                    //}
                }
            }


            var list = Uof.IcustomerService
                .GetAll(condition)
                .Where(nameQuery)
                .Where(permQuery)
                .Where(c => c.is_delete != 1)
                .OrderBy(item => item.id)
                .Select(c => new Customer()
            {
                id = c.id,
                name = c.name,
                contact = c.contact,
                mobile = c.mobile,
                salesman = c.member1.name,
                source = c.source,
                source_id = c.source_id,
                source_name = "",
                assistant_id = c.assistant_id,
                assistant_name = "",
                tel = c.tel,
                date_created = c.date_created,
            }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.IcustomerService.GetAll(condition).Where(nameQuery).Where(permQuery).Where(c => c.is_delete != 1).Count();

            if (list.Count > 0)
            {
                var sourceIds = list.Where(l => l.source_id != null).Select(l => l.source_id).Distinct().ToList();
                if (sourceIds != null && sourceIds.Count > 0)
                {
                    var customers = Uof.IcustomerService.GetAll(c => sourceIds.Contains(c.id)).Select(c => new
                    {
                        id = c.id,
                        name = c.name
                    }).ToList();
                    if (customers!= null && customers.Count() > 0)
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
                    if (members!= null && members.Count > 0)
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
            c.status = 0;
            c.is_delete = 0;
            var _c = Uof.IcustomerService.AddEntity(c);

            if (_c != null)
            {
                Uof.Icustomer_timelineService.AddEntity(new customer_timeline
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

                return SuccessResult;
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
            //    _c.assistant_id == c.assistant_id
            //    )
            //{
            //    return ErrorResult;
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
            _c.contacts = c.contacts;
            _c.assistant_id = c.assistant_id;

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

                return SuccessResult;
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

            var customerEntity = new
            {
                id = _customer.id,
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
                contacts = _customer.contacts,
                QQ = _customer.QQ,
                wechat = _customer.wechat,
                source = _customer.source,
                creator_id = _customer.creator_id,
                salesman_id = _customer.salesman_id,
                salesman = _customer.member1.name,
                organization_id = _customer.organization_id,
                source_id = _customer.source_id,
                source_name = source_name,
                description = _customer.description,
                assistant_id = _customer.assistant_id,
                assistant_name = assistant_name,

                mailling_address = _customer.mailling_address,
                mailling_province = _customer.mailling_province,
                mailling_city = _customer.mailling_city,
                mailling_county = _customer.mailling_county,
            };

            var contacts = Uof.IcontactService.GetAll(c => c.customer_id == id).Select(c=> new
            {
                id = c.id,
                customer_id = c.customer_id,
                name = c.name,
                mobile = c.mobile,
                tel = c.tel,
                position = c.position,
                email = c.email,
                wechat = c.wechat,
                QQ = c.QQ,
                type = c.type,
                memo = c.memo,
                responsable = c.responsable,
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

        public ActionResult Transfer(int id)
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
            c.code = GetNextCustomerCode(c.salesman_id.Value);
            c.status = 1;
            c.date_updated = DateTime.Now;

            var r = Uof.IcustomerService.UpdateEntity(c);

            if (r)
            {
                Uof.Icustomer_timelineService.AddEntity(new customer_timeline
                {
                    title = "转为正式客户",
                    customer_id = c.id,
                    content = string.Format("转为正式客户, 操作人：{0}", arrs[3]),
                    date_business = DateTime.Now,
                    date_created = DateTime.Now,
                    is_system = 1
                });
            }

            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }
    }
}