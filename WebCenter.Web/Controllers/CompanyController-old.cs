using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using System.Web.Security;
using System.Linq.Expressions;
using Common;
using System.Collections;
using Newtonsoft.Json;

namespace WebCenter.Web.Controllers
{

    [JsonObject(IsReference = true)]
    public class CompanyController : BaseController
    {


        public CompanyController(IUnitOfWork UOF)
            : base(UOF)
        {
        }


        /// <summary>
        /// 工程微网列表
        /// </summary>
        /// <param name="page_index"></param>
        /// <param name="page_size"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public ActionResult List(int page_index, int page_size, string keyword = "",int type_id=0)
        {            
            Expression<Func<company, bool>> condition= condition = com => com.introduce_page_id>0;
            
            if (!string.IsNullOrEmpty(keyword))
            {
                Expression<Func<company, bool>> tmp = com => com.name.IndexOf(keyword)>-1&&com.introduce_page_id>0;                
                condition = tmp;
            }
            PagedList<company> list = Uof.IcompanyService.GetAll(condition,new string[2]{"city","sys_dictionary"}).OrderByDescending(item => item.Id).ToPagedList(page_index, page_size);
            var obj = new ArrayList();
            foreach (var item in list)
            {
                var it = new
                {
                    id = item.Id,
                    name = item.name,
                    logo_path = item.logo_path,
                    type_name = item.sys_dictionary == null ? "" : item.sys_dictionary.value,
                    city_name = item.city == null ? "" : item.city.city_name
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

        public ActionResult Get(int id = 0)
        {
            if (id > 0)
            {
                company com = Uof.IcompanyService.GetById(id);
                if (com != null)
                {

                    project_case proj = com.project_case;
                    
                    if (proj != null)
                    {
                        var result = new
                          {
                              id = com.Id,
                              name = com.name,
                              type_name = com.sys_dictionary == null ? "" : com.sys_dictionary.value,
                              city_name = com.city == null ? "" : com.city.city_name,
                              phone = com.phone,
                              mobile=com.mobile,
                              address=com.address,
                              logo_path=com.logo_path,
                              company_phone=com.company_phone,
                              introduce_page = new
                              {
                                  id = proj.Id,
                                  title = proj.title,
                                  description = proj.descript,
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
                                  content=proj.content,
                                  view_count = proj.view_count,
                                  main_image_path = proj.main_image_path
                              }
                          };
                        return Json(result,JsonRequestBehavior.AllowGet); 
                    }
                    else
                    {
                        var result = new
                        {
                            id = com.Id,
                            name = com.name,
                            type_name = com.sys_dictionary == null ? "" : com.sys_dictionary.value,
                            city_name = com.city == null ? "" : com.city.city_name,
                            phone = com.phone,
                            logo_path = com.logo_path,
                            company_phone=com.company_phone,
                            introduce_page = new { }
                        };
                        return Json(result, JsonRequestBehavior.AllowGet); 

                    }
                }
            }
            return Json(new {result=false },JsonRequestBehavior.AllowGet);

        }

    }
}