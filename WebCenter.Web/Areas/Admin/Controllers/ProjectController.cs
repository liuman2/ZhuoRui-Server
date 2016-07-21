using Common;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebCenter.Entities;
using WebCenter.IServices;
using WebCenter.Web.Controllers;


namespace WebCenter.Web.Areas.Admin.Controllers
{
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
        [Authorize]
        [HttpGet]

        public ActionResult List(int page_index = 0, int page_size = 20, string keyword = "", int company_id = 0)
        {

            Expression<Func<project_case, bool>> condition = prj => true;
            if (!string.IsNullOrEmpty(keyword))
            {
                Expression<Func<project_case, bool>> tmp = prj => prj.title.IndexOf(keyword) > -1 || prj.descript.IndexOf(keyword) > -1;
                condition = tmp;
            }
            if (company_id > 0)
            {
                Expression<Func<project_case, bool>> tmp = prj => prj.company.Id == company_id;
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
                    descript = item.descript,
                    type_name = item.sys_dictionary == null ? "" : item.sys_dictionary.value,
                    view_count = item.view_count,
                    publish_time = item.publish_time,
                    is_company_intro = item.is_company_intro
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
        /// 文章发布与编辑
        /// </summary>
        /// <param name="proj"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult Save(string proj)
        {
            project_case project = new project_case();
            project = JsonConvert.DeserializeObject<project_case>(proj);
            if (project != null && project.Id > 0)
            {
                project.update_time = DateTime.Now;
                Uof.Iproject_caseService.UpdateEntity(project);
                AddLog("更新文章 文章ID：" + project.Id.ToString(), "更新文章", "成功");
                if (project.is_company_intro > 0)
                {
                    if (project.company_id.HasValue&&project.company_id.Value>0)
                    {
                        company com = Uof.IcompanyService.GetById(project.company_id.Value);
                        com.introduce_page_id = project.Id;
                        com.update_time = DateTime.Now;
                        Uof.IcompanyService.UpdateEntity(com);
                        SetIntroduceToCurrentArticle(com.Id,project);
                    }                   
                    return Json(new
                    {
                        result = true,
                        project_id = project.Id
                    });
                }
                return Json(new { result = true });
            }
            else if (project != null && project.Id == 0)
            {
                project.create_time = DateTime.Now;
                project.update_time = DateTime.Now;
                if (project.view_count == null)
                    project.view_count = 0;
                if (project.is_publish == null)
                {
                    project.is_publish = 0;
                }
                project = Uof.Iproject_caseService.AddEntity(project);
                AddLog("添加文章 文章ID：" + project.Id.ToString(), "添加文章", "成功");
                if (project.is_company_intro > 0)
                {

                    if (project.company_id.HasValue && project.company_id.Value > 0)
                    {
                        company com = Uof.IcompanyService.GetById(project.company_id.Value);
                        com.introduce_page_id = project.Id;
                        com.update_time = DateTime.Now;
                        Uof.IcompanyService.UpdateEntity(com);
                        SetIntroduceToCurrentArticle(com.Id, project);
                    }
                    return Json(new
                    {
                        result = true,
                        project_id = project.Id
                    });

                    //company com = Uof.IcompanyService.GetById(project.company_id.Value);
                    //com.introduce_page_id = project.Id;
                    //com.update_time = DateTime.Now;
                    //Uof.IcompanyService.UpdateEntity(com);
                    //return Json(new
                    //{
                    //    result = true,
                    //    project_id = project.Id
                    //});
                }
                else
                {
                    return Json(new
                    {
                        result = true,
                        project_id = project.Id
                    });
                }
            }
            else
            {
                return Json(new { result = false });
            }
           
        }
        /// <summary>
        /// companyList
        /// </summary>
        /// <param name="company"></param>
        private void SetIntroduceToCurrentArticle(int company_id,project_case proj)
        {
            try
            {
                if (proj.is_company_intro == 1)
                {
                    IList<project_case> list = Uof.Iproject_caseService.GetAll(p => p.company_id == company_id && p.Id != proj.Id).ToList();
                    if (list.Count > 0)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            list[i].is_company_intro = 0;
                        }

                        Uof.Iproject_caseService.UpdateEntities(list);

                    }
                } 
            }
            catch(Exception ex) 
            {
                LogHelper.LogError(ex.Message,ex);
            }
                       
        }
      

        [Authorize]
        [HttpPost]
        /// <summary>
        /// 删除文章
        /// </summary>
        /// <returns></returns>
        public ActionResult Delete(int id = 0)
        {
            if (id > 0)
            {
                IList<company> list = Uof.IcompanyService.GetAll(p => p.introduce_page_id == id).ToList();
                if (list.Count > 0)
                {
                    //删除Company引用 
                    try
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            list[i].introduce_page_id = null;
                        }
                        Uof.IcompanyService.UpdateEntities(list);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogError(ex.Message, ex);
                    }

                }
                 
                bool b = Uof.Iproject_caseService.DeleteEntity(id);
                if (b)
                {
                    AddLog("删除文章ID：" + id.ToString(), "删除文章", "成功");
                    return Json(new { result = true });
                }
                else
                {
                    AddLog("删除文章ID：" + id.ToString(), "删除文章", "失败");
                    return Json(new { result = false });
                }
            }
            return Json(new { result = false });
        }

        [Authorize]

        public ActionResult Get(int id = 0)
        {
            if (id > 0)
            {
                project_case proj = Uof.Iproject_caseService.GetById(id);
                var obj = new
                {
                    id = proj.Id,
                    title = proj.title,
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
                    descript = proj.descript,
                    type_id = proj.type_id,
                    content = proj.content,
                    main_image_path = proj.main_image_path,
                    company_id = proj.company_id,
                    is_company_intro = proj.is_company_intro,
                    view_count=proj.view_count,
                    is_publish = proj.is_publish==null?0:proj.is_publish
                };
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]

        public ActionResult SetPublish(int id,int type)
        {
            if (id > 0)
            {
                project_case proj = Uof.Iproject_caseService.GetById(id);
                proj.is_publish = type;
                proj.update_time = DateTime.Now;
                bool b = Uof.Iproject_caseService.UpdateEntity(proj);
                if (type > 0)
                {
                    AddLog("设置文章发布 文章ID：" + id.ToString(), "设置文章发布", "成功");
                }
                if (type ==0)
                {
                    AddLog("撤消文章发布 文章ID：" + id.ToString(), "撤消文章发布", "成功");
                }
                return Json(new { result = b } ,JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }





    }
}