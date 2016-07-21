using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCenter.IServices;
using Common;
using WebCenter.Entities;
using System.Linq.Expressions;
using System.Collections;
using Newtonsoft.Json;

namespace WebCenter.Web.Controllers
{
    [JsonObject(IsReference = true)]
    public class ProjectController : BaseController
    {

        public ProjectController(IUnitOfWork UOF)
            : base(UOF)
        {
        }

        /// <summary>
        /// get the project_case List
        /// </summary>
        /// <param name="page_index"></param>
        /// <param name="page_size"></param>
        /// <param name="keyword"></param>
        /// <param name="company_id"></param>
        /// <returns></returns>
        [HttpGet]

        public ActionResult List(int page_index = 0, int page_size = 20, string keyword = "", int company_id = 0)
        {

            Expression<Func<project_case, bool>> condition = prj => prj.is_publish == 1&&(prj.is_company_intro==null||prj.is_company_intro==0);
            if (!string.IsNullOrEmpty(keyword))
            {
                Expression<Func<project_case, bool>> tmp = prj => (prj.title.IndexOf(keyword) > -1 || prj.descript.IndexOf(keyword) > -1) && prj.is_publish == 1 && (prj.is_company_intro == null || prj.is_company_intro == 0);
                condition = tmp;
            }
            if (company_id > 0)
            {
                Expression<Func<project_case, bool>> tmp = prj => prj.company.Id == company_id && prj.is_publish == 1 && (prj.is_company_intro == null || prj.is_company_intro == 0);
                condition = tmp;
            }

            PagedList<project_case> list = Uof.Iproject_caseService.GetAll(condition).OrderByDescending(item => item.Id).ToPagedList(page_index, page_size);
            var obj = new ArrayList();
            foreach (var item in list)
            {
                var it = new
                {
                    id = item.Id,
                    title = item.title,
                    description = item.descript,
                    type_name = item.sys_dictionary == null ? "" : item.sys_dictionary.value,
                    view_count = item.view_count,
                    main_image_path = item.main_image_path,
                    update_time=item.update_time.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd")
                };
                obj.Add(it);
            }
            var result = new
            {
                total_count = list.TotalCount,
                current_page = page_index,
                page_size = page_size,
                items = obj
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Set project_case view count
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SetViewCount(int id = 0)
        {
            if (id > 0)
            {
                project_case proj = Uof.Iproject_caseService.GetById(id);
                if (proj != null)
                {
                    if (proj.view_count == null)
                        proj.view_count = 0;
                    proj.view_count = proj.view_count + 1;
                    Uof.Iproject_caseService.UpdateEntity(proj);
                }

            }
            return Json(new { reuslt = true }, JsonRequestBehavior.AllowGet);
        }

        //get project_case details with company info
        public ActionResult Get(int id = 0)
        {
            if (id > 0)
            {
                project_case proj = Uof.Iproject_caseService.GetById(id);
                if (proj != null)
                {
                    company company = proj.company;


                    if (company != null)
                    {
                        user _user = company.user;

                        if (_user != null)
                        {

                            var comobj = new
                            {
                                id=company.Id,
                                name = company.name,
                                logo_path = company.logo_path,
                                mobile = _user.mobile,
                                phone = _user.phone,
                                address = company.address,
                                company_phone=company.company_phone
                            };
                            string user_name = "";
                            if (proj.user == null)
                            {
                                user_name = _user.real_name;
                            }
                            else
                            {
                                user_name = proj.user.real_name;
                            }
                            var obj = new
                            {
                                id = proj.Id,
                                title = proj.title,
                                descript = proj.descript,
                                project_contact_phone=proj.project_contact_phone,
                                project_address=proj.project_address,
                                project_action_company=proj.project_action_company,
                                project_design_company=proj.project_design_company,
                                project_type=proj.project_type,
                                project_name=proj.project_name,
                                project_area=proj.project_area,
                                product_metal = proj.product_metal,
                                product_ruler = proj.product_ruler,
                                product_cence = proj.product_cence,
                                product_price = proj.product_price,
                                product_address = proj.product_address,
                                is_product = proj.is_product,
                                type_name = proj.sys_dictionary == null ? "" : proj.sys_dictionary.value,
                                type_id = proj.sys_dictionary == null ? "0" : proj.sys_dictionary.id.ToString(),
                                update_time = proj.update_time.GetValueOrDefault(DateTime.Now).ToString("yy-MM-dd"),
                                content = proj.content,
                                main_image_path = proj.main_image_path,
                                user_name = user_name,
                                company = comobj
                            };
                            return Json(obj, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var obj = new
                            {
                                id = proj.Id,
                                title = proj.title,
                                descript = proj.descript,
                                project_contact_phone = proj.project_contact_phone,
                                project_address = proj.project_address,
                                project_action_company = proj.project_action_company,
                                project_design_company = proj.project_design_company,
                                project_type = proj.project_type,
                                project_name = proj.project_name,
                                project_area = proj.project_area,
                                product_metal = proj.product_metal,
                                product_ruler = proj.product_ruler,
                                product_cence = proj.product_cence,
                                product_price = proj.product_price,
                                product_address = proj.product_address,
                                is_product = proj.is_product,
                                type_name = proj.sys_dictionary == null ? "" : proj.sys_dictionary.value,
                                type_id=proj.sys_dictionary == null ? "0" : proj.sys_dictionary.id.ToString(),
                                update_time=proj.update_time.GetValueOrDefault(DateTime.Now).ToString("yy-MM-dd"),
                                content = proj.content,
                                main_image_path = proj.main_image_path,
                                user_name = proj.user == null ? "" : proj.user.real_name,
                                company = new
                                {
                                    id=company.Id,
                                    name = company.name,
                                    logo_path = company.logo_path,
                                    mobile = "",
                                    phone = "",
                                    address = company.address,
                                    company_phone=company.company_phone
                                }
                            };
                            return Json(obj, JsonRequestBehavior.AllowGet);

                        }




                    }
                    else {
                        var obj = new
                        {
                            id = proj.Id,
                            title = proj.title,
                            descript = proj.descript,
                            type_name = proj.sys_dictionary == null ? "" : proj.sys_dictionary.value,
                            content = proj.content,
                            main_image_path = proj.main_image_path,
                            user_name = proj.user == null ? "" : proj.user.real_name,
                            project_contact_phone = proj.project_contact_phone,
                            project_address = proj.project_address,
                            project_action_company = proj.project_action_company,
                            project_design_company = proj.project_design_company,
                            project_type = proj.project_type,
                            project_name = proj.project_name,
                            project_area = proj.project_area,
                            product_metal = proj.product_metal,
                            product_ruler = proj.product_ruler,
                            product_cence = proj.product_cence,
                            product_price = proj.product_price,
                            product_address = proj.product_address,
                            type_id = proj.sys_dictionary == null ? "0" : proj.sys_dictionary.id.ToString(),
                            update_time = proj.update_time.GetValueOrDefault(DateTime.Now).ToString("yy-MM-dd"),
                            is_product = proj.is_product,
                            company = new
                            {
                                id="",
                                name = "",
                                logo_path = "",
                                mobile = "",
                                phone = "",
                                address = ""
                            }
                        };
                        return Json(obj, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);

        }

    }
}