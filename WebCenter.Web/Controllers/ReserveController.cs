using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System;

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
                    if (hasDepart == null)
                    {
                        permQuery = c => (c.salesman_id == userId || c.assistant_id == userId);
                    } else
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


            var list = Uof.IcustomerService.GetAll(condition).Where(nameQuery).Where(permQuery).OrderBy(item => item.id).Select(c => new
            {
                id = c.id,
                name = c.name,
                contact = c.contact,
                mobile = c.mobile,
                tel = c.tel
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
            c.status = 0;

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

                return SuccessResult;
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
                _c.assistant_id == c.assistant_id
                )
            {
                return ErrorResult;
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

            return Json(new
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
                assistant_name = assistant_name

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