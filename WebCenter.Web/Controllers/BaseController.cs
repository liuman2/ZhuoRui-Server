using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using CacheManager.Core;
using System.Configuration;
using Common;
using Newtonsoft.Json;
using WebCenter.Web.Code;



namespace WebCenter.Web.Controllers
{
    [JsonObject(IsReference = true)]
    public class BaseController : Controller
    {

        protected ICache<object> Cache;
        protected IUnitOfWork Uof;



        public BaseController(IUnitOfWork uof)
        {
            Uof = uof;
            Cache = CacheUtil.Cache;

        }

        public ActionResult ErrorResult
        {
            get { return Json(new { success = false }, JsonRequestBehavior.AllowGet); }
        }

        public ActionResult SuccessResult
        {
            get { return Json(new { success = true }, JsonRequestBehavior.AllowGet); }
        }

        public void AddLog(string name, string descipt, string result)
        {

        }

        /// <summary>
        /// GetCompandyId
        /// </summary>
        /// <param name="project_id"></param>
        /// <returns></returns>
        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        public string GetNextCustomerCode(int userId)
        {
            var areaId = Uof.ImemberService.GetAll(m => m.id == userId).Select(m => m.area_id).FirstOrDefault();

            var codeSetting = Uof.IsettingService.GetAll(s => s.name == "CODING").FirstOrDefault();
            var codingObj = JsonConvert.DeserializeObject<Coding>(codeSetting.value);

            var suffix = codingObj.customer.suffix;
            var codeStr = codingObj.customer.area_code.Where(a => a.id == areaId).Select(a => a.value).FirstOrDefault();


            var c = Uof.IcustomerService.GetAll(a => a.status == 1 && a.code.Contains(codeStr)).Select(a => new
            {
                id = a.id,
                code = a.code,
                name = a.name
            }).OrderByDescending(a => a.id).FirstOrDefault();

            if (c == null)
            {
                return string.Format("{0}{1}", codeStr, 1.ToString().PadLeft(suffix, '0'));
            }

            var indexStr = c.code.Replace(codeStr, "").Replace("0", "");

            var index = 0;
            int.TryParse(indexStr, out index);

            return string.Format("{0}{1}", codeStr, (index + 1).ToString().PadLeft(suffix, '0'));
        }

        public string GetNextLetterCode(string type)
        {
            var codeStr = "OT";
            if (codeStr == "收件")
            {
                codeStr = "IN";
            }

            var c = Uof.ImailService.GetAll(a => a.code.Contains(codeStr)).Select(a => new
            {
                id = a.id,
                code = a.code,
            }).OrderByDescending(a => a.code).FirstOrDefault();

            if (c == null)
            {
                return string.Format("{0}{1}", codeStr, 1.ToString().PadLeft(5, '0'));
            }

            var indexStr = c.code.Replace(codeStr, "").Replace("0", "");

            var index = 0;
            int.TryParse(indexStr, out index);

            return string.Format("{0}{1}", codeStr, (index + 1).ToString().PadLeft(5, '0'));
        }

        public string GetNextOrderCode(int userId, string moduleCode)
        {
            var areaId = Uof.ImemberService.GetAll(m => m.id == userId).Select(m => m.area_id).FirstOrDefault();

            var codeSetting = Uof.IsettingService.GetAll(s => s.name == "CODING").FirstOrDefault();
            var codingObj = JsonConvert.DeserializeObject<Coding>(codeSetting.value);

            var areaCodeStr = codingObj.customer.area_code.Where(a => a.id == areaId).Select(a => a.value).FirstOrDefault();

            var suffix = codingObj.order.suffix;
            var codeStr = codingObj.order.code.Where(a => a.module == moduleCode).Select(a => a.value).FirstOrDefault();

            var dbCode = "";
            var preCode = string.Format("{0}{1}", areaCodeStr, codeStr);
            switch (moduleCode)
            {
                // 境外注册
                case "ZW":
                   dbCode = Uof.Ireg_abroadService.GetAll(r=>r.code.Contains(preCode)).OrderByDescending(a=>a.code).Select(a => a.code).FirstOrDefault();
                    break;
                // 境内注册
                case "ZN":
                    dbCode = Uof.Ireg_internalService.GetAll(r => r.code.Contains(preCode)).OrderByDescending(a => a.code).Select(a => a.code).FirstOrDefault();
                    break;
                // 审计
                case "SJ":
                    dbCode = Uof.IauditService.GetAll(r => r.code.Contains(preCode)).OrderByDescending(a => a.code).Select(a => a.code).FirstOrDefault();
                    break;
                // 商标
                case "SB":
                    dbCode = Uof.ItrademarkService.GetAll(r => r.code.Contains(preCode)).OrderByDescending(a => a.code).Select(a => a.code).FirstOrDefault();
                    break;
                // 专利
                case "ZL":
                    dbCode = Uof.IpatentService.GetAll(r => r.code.Contains(preCode)).OrderByDescending(a => a.code).Select(a => a.code).FirstOrDefault();
                    break;
                //年审
                case "NS":
                    break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(dbCode))
            {
                return string.Format("{0}{1}{2}", areaCodeStr, codeStr, 1.ToString().PadLeft(suffix, '0'));
            }

            var indexStr = dbCode.Replace(preCode, "");

            var index = 0;
            int.TryParse(indexStr, out index);

            return string.Format("{0}{1}{2}", areaCodeStr, codeStr, (index + 1).ToString().PadLeft(suffix, '0'));
        }

        /// <summary>
        /// 获取财务审核人员
        /// </summary>
        /// <returns></returns>
        //public List<int> GetFinanceMembers()
        //{
        //    var roleIds = Uof.Irole_operationService.GetAll(r => r.operation_id == 3).Select(r => r.role_id).ToList();

        //    if (roleIds.Count() == 0)
        //    {
        //        return new List<int>();
        //    }

        //    var ids = Uof.Irole_memberService.GetAll(m => roleIds.Contains(m.role_id)).Select(m => m.member_id.Value).ToList();
        //    return ids;
        //}

        /// <summary>
        /// 获取审核人
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int? GetAuditorByKey(string key)
        {
            var auditor = Uof.IsettingService.GetAll(s => s.name == key).FirstOrDefault();
            if (auditor == null)
            {
                return null;
            }

            int id = 0;
            int.TryParse(auditor.value, out id);
            return id;
        }
        /// <summary>
        /// 获取提交审核人员
        /// </summary>
        /// <returns></returns>
        public List<int> GetSubmitMembers()
        {
            var roleIds = Uof.Irole_operationService.GetAll(r => r.operation_id == 4).Select(r => r.role_id).ToList();

            if (roleIds.Count() == 0)
            {
                return new List<int>();
            }

            var ids = Uof.Irole_memberService.GetAll(m => roleIds.Contains(m.role_id)).Select(m => m.member_id.Value).ToList();
            return ids;
        }

        public int GetOrgIdByUserId(int userid)
        {
            var orgId = Uof.ImemberService.GetAll(a => a.id == userid).Select(a => a.organization_id.Value).FirstOrDefault();
            return orgId;
        }

        public List<int> GetChildrenDept(int parentId)
        {
            var allDept = Uof.IorganizationService.GetAll().Select(a => new DepartmentIds
            {
                id = a.id,
                parent_id = a.parent_id
            }).ToList();

            var deptIds = new List<int>();

            var ids = allDept.Where(a => a.parent_id == parentId).Select(a => a.id).ToList();
            if (ids.Count > 0)
            {
                foreach (var id in ids)
                {
                    deptIds.Add(id);
                    GetNextChildrenDept(id, allDept, deptIds);
                }                
            }

            deptIds.Add(parentId);

            return deptIds;
        }

        private void GetNextChildrenDept(int parentId, List<DepartmentIds> allDept, List<int> deptIds)
        {
            var ids = allDept.Where(a => a.parent_id == parentId).Select(a => a.id).ToList();
            if (ids.Count > 0)
            {
                foreach (var id in ids)
                {
                    deptIds.Add(id);
                    GetNextChildrenDept(id, allDept, deptIds);
                }
            }
        } 
    }
}