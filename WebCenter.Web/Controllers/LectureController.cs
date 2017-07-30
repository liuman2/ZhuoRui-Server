using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.IO;

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
                condition = c => (c.title.IndexOf(request.title) > -1 || 
                c.teacher.IndexOf(request.title) > -1 || 
                c.city.IndexOf(request.title) > -1 ||
                c.sponsor.IndexOf(request.title) > -1 ||
                c.address.IndexOf(request.title) > -1);
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
        public ActionResult Add(lecture lect, List<attachment> attachments)
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


            lect.creator_id = userId;
            lect.date_created = DateTime.Now;
            
            var _l = Uof.IlectureService.AddEntity(lect);

            if (_l != null && attachments != null && attachments.Count > 0)
            {
                var atts = new List<attachment>();
                foreach (var item in attachments)
                {
                    atts.Add(new attachment
                    {
                        source_id = _l.id,
                        source_name = "lecture",
                        name = item.name ?? "",
                        attachment_url = item.attachment_url,
                    });
                }
                Uof.IattachmentService.AddEntities(atts);
            }
            return Json(new { id = _l.id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Update(lecture lect, List<attachment> attachments)
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

            var _c = Uof.IlectureService.GetById(lect.id);

            //if (_c.title == c.title &&
            //    _c.form == c.form &&
            //    _c.city == c.city &&
            //    _c.teacher == c.teacher &&
            //    _c.date_at == c.date_at &&
            //    _c.city == c.city &&
            //    _c.address == c.address &&
            //    _c.charge_id == c.charge_id &&
                
            //    _c.sponsor == c.sponsor &&
            //    _c.co_sponsor == c.co_sponsor &&
            //    _c.customer_target == c.customer_target

            //    )
            //{
            //    return SuccessResult;
            //}

            _c.title = lect.title;
            _c.form = lect.form;
            _c.city = lect.city;
            _c.teacher = lect.teacher;
            _c.date_at = lect.date_at;
            _c.address = lect.address;
            _c.sponsor = lect.sponsor;
            _c.co_sponsor = lect.co_sponsor;
            _c.charge_id = lect.charge_id;
            _c.customer_target = lect.customer_target;
                       
            _c.date_updated = DateTime.Now;

            var r = Uof.IlectureService.UpdateEntity(_c);

            if (r && attachments != null && attachments.Count > 0)
            {
                var newAtts = new List<attachment>();
                var attIds = Uof.IattachmentService.GetAll(a => a.source_id == _c.id && a.source_name == "lecture").Select(a => a.id).ToList();
                if (attIds.Count > 0)
                {
                    newAtts = attachments.Where(a => !attIds.Contains(a.id)).ToList();
                }
                else
                {
                    newAtts = attachments;
                }

                if (newAtts.Count > 0)
                {
                    foreach (var item in newAtts)
                    {
                        item.source_id = _c.id;
                        item.source_name = "lecture";
                    }
                    Uof.IattachmentService.AddEntities(newAtts);
                }
            }

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

            var atts = new List<attachment>();
            if (_l != null)
            {
                atts = Uof.IattachmentService.GetAll(a => a.source_id == _l.id && a.source_name == "lecture").ToList();
            }

            var lecturePeriod = GetSettingByKey("LECTURE_PERIOD");
            if (lecturePeriod == null || _l.date_at == null)
            {
                return Json(new
                {
                    lect = _l,
                    attachments = atts,
                    period = new
                    {
                        disabled = false,
                        days = 0
                    }
                }, JsonRequestBehavior.AllowGet);
            }

            int period = 0;
            int.TryParse(lecturePeriod.value, out period);

            var limittedDate = _l.date_at.Value.AddDays(period);
            var disabled = DateTime.Today > limittedDate;
            

            return Json(new { lect = _l, attachments = atts, period = new { disabled = disabled, days = period } }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OldDataUpdate()
        {
            //旧数据迁移
            try
            {
                var dbLectureCustomers = Uof.Ilecture_customerService.GetAll(c => c.contact_id == null).ToList();
                if (dbLectureCustomers != null && dbLectureCustomers.Count() > 0)
                {
                    foreach (var item in dbLectureCustomers)
                    {
                        var dbContact = Uof.IcontactService.GetAll(c => c.customer_id == item.customer_id).FirstOrDefault();
                        if (dbContact != null)
                        {
                            item.contact_id = dbContact.id;
                            try
                            {
                                Uof.Ilecture_customerService.UpdateEntity(item);
                            }
                            catch (Exception)
                            {
                            }
                        }                                            
                    }
                }
            }
            catch (Exception)
            {

            }

            return SuccessResult;
        }

        public ActionResult GetDetails(int id, int size, int index)
        {
            size = 500;
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
            var strUserId = userId.ToString();

            var list = Uof.Ilecture_customerService
                .GetAll(c=>c.lecture_id == id)
                .OrderByDescending(item => item.id).Select(c => new LectureCustomer()
                {
                    id = c.id,
                    lecture_id = c.lecture_id,
                    customer_id = c.customer_id,
                    contact_id = c.contact_id,
                    code = c.customer.code,
                    name = c.customer.name,
                    industry = c.customer.industry,
                    province = c.customer.province,
                    city = c.customer.city,
                    county = c.customer.county,
                    address = c.customer.address,
                    contact = c.contact.name,
                    mobile = c.contact.mobile,
                    tel = c.contact.tel,
                    fax = c.customer.fax,
                    email = c.contact.email,
                    QQ = c.contact.QQ,
                    wechat = c.contact.wechat,
                    source = c.customer.source,
                    creator_id = c.customer.creator_id,
                    salesman_id = c.customer.salesman_id,
                    salesman = c.customer.member1.name,
                    status = c.customer.status

                }).ToPagedList(index, size).ToList();

            var ids = list.Select(l => l.customer_id).ToList();
            var myCustomers = Uof.IcustomerService
                .GetAll(c => c.salesman_id == userId || c.assistant_id == userId || c.assistants.Contains(strUserId) || userId == 1)
                .Where(c => ids.Contains(c.id)).Select(c => c.id).ToList();

            if (myCustomers.Count() == 0)
            {
                var page1 = new
                {
                    current_index = index,
                    current_size = size,
                    total_size = 0,
                    total_page = 0
                };

                var result1 = new
                {
                    page = page1,
                    items = new List<LectureCustomer>()
                };
                return Json(result1, JsonRequestBehavior.AllowGet);
            }

            var newList = list.Where(l => myCustomers.Contains(l.customer_id.Value)).ToList();

            var totalRecord = newList.Count(); //  Uof.Ilecture_customerService.GetAll(c => c.lecture_id == id).Count();

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
                items = newList
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveCustomer(int leactueId, List<LectureContactRequest> request)
        {
            var oldMembers = Uof.Ilecture_customerService.GetAll(m => m.lecture_id == leactueId).ToList();
            var adds = new List<lecture_customer>();

            if (oldMembers.Count() == 0)
            {
                foreach (var item in request)
                {
                    adds.Add(new lecture_customer()
                    {
                        lecture_id = leactueId,
                        customer_id = item.customer_id,
                        contact_id = item.contact_id
                    });
                }

                Uof.Ilecture_customerService.AddEntities(adds);
                return SuccessResult;
            }

            foreach (var item in request)
            {
                var exist = oldMembers.Where(o => o.customer_id == item.customer_id && o.contact_id == item.contact_id);
                if (exist.Count() == 0)
                {
                    adds.Add(new lecture_customer()
                    {
                        lecture_id = leactueId,
                        customer_id = item.customer_id,
                        contact_id = item.contact_id
                    });
                }
            }

            if (adds.Count() > 0)
            {
                Uof.Ilecture_customerService.AddEntities(adds);
            }

            return SuccessResult;
        }

        public ActionResult DeleteLeactureCustomer(int leactureId, int contactId)
        {
            var customerMember = Uof.Ilecture_customerService.GetAll(m => m.contact_id == contactId && m.lecture_id == leactureId).FirstOrDefault();

            if (customerMember == null)
            {
                return ErrorResult;
            }

            var r = Uof.Ilecture_customerService.DeleteEntity(customerMember);

            return Json(new { success = r, message = r ? "" : "删除失败" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CusomerSearch(int lectureId, int index = 1, int size = 10, string name = "")
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
            var strUserId = userId.ToString();

            var contactIds = Uof.Ilecture_customerService.GetAll(c=>c.lecture_id == lectureId).Select(m => m.contact_id).ToList();
            Expression<Func<contact, bool>> excludeIds = m => true;
            if (contactIds.Count() > 0)
            {
                excludeIds = m => !contactIds.Contains(m.id);
            }

            Expression<Func<customer, bool>> nameQuery = c => true;

            if (!string.IsNullOrEmpty(name))
            {
                nameQuery = c => (c.name.IndexOf(name) > -1 || c.code.IndexOf(name) > -1);
            }

            var customerIds = Uof.IcustomerService.GetAll(nameQuery)
                .Where(c => c.salesman_id == userId || c.assistant_id == userId || c.assistants.Contains(strUserId) || userId == 1)
                .Select(c=>c.id)
                .ToList();



            var list = Uof.IcontactService.GetAll(c => customerIds.Contains(c.customer_id))
                .Where(excludeIds)
                .OrderByDescending(item => item.id).Select(c => new LectureContact
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
                    responsable = c.responsable
                }).ToPagedList(index, size);

            var totalRecord = Uof.IcontactService
                .GetAll(c => customerIds.Contains(c.customer_id))
                .Where(excludeIds)
                .Count();

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
                var listCustomerIds = list.Select(l => l.customer_id).Distinct().ToList();
                var listCustomers = Uof.IcustomerService.GetAll(c => listCustomerIds.Contains(c.id)).Select(c => new
                {
                    id = c.id,
                    name = c.name,
                    industry = c.industry,
                    business_nature = c.business_nature,
                    province = c.province,
                    city = c.city,
                    county = c.county,
                    address = c.address,
                    salesman_id = c.salesman_id,
                    salesman = c.member1.name,
                    status = c.status,
                }).ToList();

                if (listCustomers!= null && listCustomers.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        var c = listCustomers.Where(l => l.id == item.customer_id).FirstOrDefault();
                        if (c != null)
                        {
                            item.customer_name = c.name;
                            item.industry = c.industry;
                            item.business_nature = c.business_nature;
                            item.province = c.province;
                            item.city = c.city;
                            item.county = c.county;
                            item.address = c.address;
                            item.salesman_id = c.salesman_id;
                            item.salesman = c.salesman;
                            item.status = c.status;
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

        public FileStreamResult Export(int lectureId)
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

            var userId = 0;
            int.TryParse(arrs[0], out userId);

            var contacts = Uof.Ilecture_customerService.GetAll(l => l.lecture_id == lectureId && l.contact_id != null && l.customer_id != null).Select(l => new LectureJoinMember
            {
                customerId = l.customer_id,
                lectureId = l.lecture_id,
                contactId = l.contact_id,

                //customerName = l.customer.name,
                //businessNature = l.customer.business_nature,
                //industry = l.customer.industry,
                //province = l.customer.province,
                //city = l.customer.city,
                //county = l.customer.county,
                //address = l.customer.address,
                //assistant_id = l.customer.assistant_id,
                //assistants = l.customer.assistants,
                //salesman_id = l.customer.salesman_id,
                //salesman = l.customer.member1.name,

                //contactName = l.contact.name,
                //mobile = l.contact.mobile,
                //tel = l.contact.tel,
                //email = l.contact.email,
                //wechat = l.contact.wechat
            }).ToList();

            if (contacts == null)
            {
                throw new Exception("无数据可导出");
            }

            if (contacts.Count() == 0)
            {
                throw new Exception("无数据可导出");
            }

            if (contacts != null && contacts.Count() > 0)
            {
                var customerIds = contacts.Select(c => c.customerId).Distinct().ToList();
                var customerList = Uof.IcustomerService.GetAll(c => customerIds.Contains(c.id)).Select(c => new
                {
                    id = c.id,
                    name = c.name,
                    business_nature = c.business_nature,
                    industry = c.industry,
                    province = c.province,
                    city = c.city,
                    county = c.county,
                    address = c.address,
                    assistant_id = c.assistant_id,
                    assistants = c.assistants,
                    salesman_id = c.salesman_id,
                    salesman = c.member1.name,
                }).ToList();

                if (customerList != null && customerList.Count() > 0)
                {
                    foreach (var item in contacts)
                    {
                        var _customer = customerList.Where(c => c.id == item.customerId).FirstOrDefault();
                        if (_customer != null)
                        {
                            item.customerName = _customer.name;
                            item.businessNature = _customer.business_nature;
                            item.industry = _customer.industry;
                            item.province = _customer.province;
                            item.city = _customer.city;
                            item.county = _customer.county;
                            item.address = _customer.address;
                            item.assistant_id = _customer.assistant_id;
                            item.assistants = _customer.assistants;
                            item.salesman_id = _customer.salesman_id;
                            item.salesman = _customer.salesman;
                        }
                    }
                }

                var contactIds = contacts.Select(c => c.contactId).Distinct().ToList();
                var contactList = Uof.IcontactService.GetAll(c => contactIds.Contains(c.id)).Select(c => new
                {
                    id = c.id,
                    contactName = c.name,
                    mobile = c.mobile,
                    tel = c.tel,
                    email = c.email,
                    wechat = c.wechat
                }).ToList();

                if (contactList != null && contactList.Count() > 0)
                {
                    foreach (var item in contacts)
                    {
                        var _contact = contactList.Where(c => c.id == item.contactId).FirstOrDefault();
                        if (_contact != null)
                        {
                            item.contactName = _contact.contactName;
                            item.mobile = _contact.mobile;
                            item.tel = _contact.tel;
                            item.email = _contact.email;
                            item.wechat = _contact.wechat;
                        }
                    }
                }
            }

            var strUserId = userId.ToString();
            var myCustomers = contacts.Where(c => c.salesman_id == userId || c.assistant_id == userId || (c.assistants!= null && c.assistants.Contains(strUserId)) || userId == 1).ToList();

            if (myCustomers.Count() > 0)
            {
                var exportList = myCustomers.Select(c => new ExcelLectureContact
                {
                    客户名称 = c.customerName,
                    业务性质 = c.businessNature,
                    行业类别 = c.industry,
                    联系人 = c.contactName,
                    手机 = c.mobile,
                    座机 = c.tel,
                    邮箱 = c.email,
                    省份 = c.province,
                    城市 = c.city,
                    地区 = c.county,
                    地址 = c.address,
                    业务员 = c.salesman,
                }).ToList();

                var title = Uof.IlectureService.GetAll(l => l.id == lectureId).Select(l => l.title).FirstOrDefault();
                if (string.IsNullOrEmpty(title))
                {
                    title = "参会客户名单";
                } else
                {
                    title = title + "参会客户名单";
                }
                var sheet = ExportToExcel(exportList);
                var fileName = title + ".xml";
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
    }
}