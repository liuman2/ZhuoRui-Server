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
    public class LectureController : BaseController
    {
        public LectureController(IUnitOfWork UOF)
            : base(UOF)
        {
        }

        public ActionResult Search(LectureRequest request)
        {
            Expression<Func<lecture, bool>> condition = c => true;
            if (!string.IsNullOrEmpty(request.title))
            {
                condition = c => (c.title.IndexOf(request.title) > -1);
            }

            // 形式
            Expression<Func<lecture, bool>> formQuery = c => true;
            if (!string.IsNullOrEmpty(request.form))
            {
                formQuery = c => (c.form == request.form);
            }
            
            // 成交开始日期
            Expression<Func<lecture, bool>> date1Query = c => true;
            Expression<Func<lecture, bool>> date2Query = c => true;
            if (request.start_time != null)
            {
                date1Query = c => (c.date_at >= request.start_time.Value);
            }
            // 成交结束日期
            if (request.end_time != null)
            {
                var endTime = request.end_time.Value.AddDays(1);
                date2Query = c => (c.date_at < endTime);
            }

            var list = Uof.IlectureService.GetAll(condition)
                .Where(formQuery)
                .Where(date1Query)
                .Where(date2Query)
                .OrderByDescending(item => item.id).Select(c => new
                {
                    id = c.id,
                    form = c.form,
                    title = c.title,
                    teacher = c.teacher,
                    date_at = c.date_at,
                    charge_id = c.charge_id,
                    chargeman = c.member.name,
                    city = c.city,
                    address = c.address,
                    sponsor = c.sponsor,
                    co_sponsor = c.co_sponsor,
                    customer_target = c.customer_target

                }).ToPagedList(request.index, request.size).ToList();

            var totalRecord = Uof.IlectureService.GetAll(condition)
                .Where(formQuery)
                .Where(date1Query)
                .Where(date2Query)
                .Count();

            var totalPages = 0;
            if (totalRecord > 0)
            {
                totalPages = (totalRecord + request.size - 1) / request.size;
            }
            var page = new
            {
                current_index = request.index,
                current_size = request.size,
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

        [HttpPost]
        public ActionResult Add(lecture l)
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


            l.creator_id = userId;
            l.date_created = DateTime.Now;
            
            var _l = Uof.IlectureService.AddEntity(l);
            return Json(new { id = _l.id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(lecture c)
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

            var _c = Uof.IlectureService.GetById(c.id);

            if (_c.title == c.title &&
                _c.form == c.form &&
                _c.city == c.city &&
                _c.teacher == c.teacher &&
                _c.date_at == c.date_at &&
                _c.city == c.city &&
                _c.address == c.address &&
                _c.charge_id == c.charge_id &&
                
                _c.sponsor == c.sponsor &&
                _c.co_sponsor == c.co_sponsor &&
                _c.customer_target == c.customer_target

                )
            {
                return SuccessResult;
            }

            _c.title = c.title;
            _c.form = c.form;
            _c.city = c.city;
            _c.teacher = c.teacher;
            _c.date_at = c.date_at;
            _c.address = c.address;
            _c.sponsor = c.sponsor;
            _c.co_sponsor = c.co_sponsor;
            _c.charge_id = c.charge_id;
            _c.customer_target = c.customer_target;
                       
            _c.date_updated = DateTime.Now;

            var r = Uof.IlectureService.UpdateEntity(_c);

            return Json(new { success = r, id = _c.id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int id)
        {
            var _l = Uof.IlectureService.GetAll(l => l.id == id).Select(l => new
            {
                id = l.id,
                form = l.form,
                title = l.title,
                city = l.city,
                address = l.address,
                teacher = l.teacher,
                date_at = l.date_at,
                charge_id = l.charge_id,
                chargeman = l.member.name,
                sponsor = l.sponsor,
                co_sponsor = l.co_sponsor,
                customer_target = l.customer_target
            }).FirstOrDefault();

            return Json(_l, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDetails(int id, int size, int index)
        {                      
            var list = Uof.Ilecture_customerService.GetAll(c=>c.lecture_id == id)
                .OrderByDescending(item => item.id).Select(c => new
                {
                    id = c.id,
                    lecture_id = c.lecture_id,
                    customer_id = c.customer_id,
                    code = c.customer.code,
                    name = c.customer.name,
                    industry = c.customer.industry,
                    province = c.customer.province,
                    city = c.customer.city,
                    county = c.customer.county,
                    address = c.customer.address,
                    contact = c.customer.contact,
                    mobile = c.customer.mobile,
                    tel = c.customer.tel,
                    fax = c.customer.fax,
                    email = c.customer.email,
                    QQ = c.customer.QQ,
                    wechat = c.customer.wechat,
                    source = c.customer.source,
                    creator_id = c.customer.creator_id,
                    salesman_id = c.customer.salesman_id,
                    salesman = c.customer.member1.name,
                    status = c.customer.status

                }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.Ilecture_customerService.GetAll(c => c.lecture_id == id).Count();

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

        [HttpPost]
        public ActionResult SaveCustomer(int leactueId, int[] customerIds)
        {

            var oldMembers = Uof.Ilecture_customerService.GetAll(m => m.lecture_id == leactueId).ToList();
            var adds = new List<lecture_customer>();

            if (oldMembers.Count() == 0)
            {
                foreach (var item in customerIds)
                {
                    adds.Add(new lecture_customer()
                    {
                        lecture_id = leactueId,
                        customer_id = item
                    });
                }

                Uof.Ilecture_customerService.AddEntities(adds);
                return SuccessResult;
            }

            foreach (var item in customerIds)
            {
                var exist = oldMembers.Where(o => o.customer_id == item);
                if (exist.Count() == 0)
                {
                    adds.Add(new lecture_customer()
                    {
                        lecture_id = leactueId,
                        customer_id = item
                    });
                }
            }

            if (adds.Count() > 0)
            {
                Uof.Ilecture_customerService.AddEntities(adds);
            }

            return SuccessResult;
        }

        public ActionResult DeleteLeactureCustomer(int leactureId, int customerId)
        {
            var customerMember = Uof.Ilecture_customerService.GetAll(m => m.customer_id == customerId && m.lecture_id == leactureId).FirstOrDefault();

            if (customerMember == null)
            {
                return ErrorResult;
            }

            var r = Uof.Ilecture_customerService.DeleteEntity(customerMember);

            return Json(new { success = r, message = r ? "" : "删除失败" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CusomerSearch(int lectureId, int index = 1, int size = 10, string name = "")
        {
            var customerIds = Uof.Ilecture_customerService.GetAll(c=>c.lecture_id == lectureId).Select(m => m.customer_id).ToList();
            Expression<Func<customer, bool>> excludeIds = m => true;
            if (customerIds.Count() > 0)
            {
                excludeIds = m => !customerIds.Contains(m.id);
            }

            Expression<Func<customer, bool>> nameQuery = c => true;

            if (!string.IsNullOrEmpty(name))
            {
                nameQuery = c => (c.name.IndexOf(name) > -1 || c.code.IndexOf(name) > -1);
            }

            var list = Uof.IcustomerService.GetAll(nameQuery)
                .Where(excludeIds)
                .OrderByDescending(item => item.id).Select(c => new
                {
                    id = c.id,
                    code = c.code,
                    name = c.name,
                    business_nature = c.business_nature,
                    status = c.status,
                    contact = c.contact,
                    mobile = c.mobile,
                    tel = c.tel,
                    email = c.email,
                    salesman = c.member1.name,
                    industry = c.industry,
                    province = c.province,
                    city = c.city,
                    county = c.county,
                    address = c.address
                }).ToPagedList(index, size).ToList();

            var totalRecord = Uof.IcustomerService.GetAll(nameQuery).Count();

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