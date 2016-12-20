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
using System.Text;
using System.Reflection;

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

        public string ExportToExcel<T>(List<T> list)
        {
            int columnCount = 0;

            DateTime StartTime = DateTime.Now;

            StringBuilder rowData = new StringBuilder();

            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            rowData.Append("<Row ss:StyleID=\"s62\">");
            foreach (PropertyInfo p in properties)
            {
                if (p.PropertyType.Name != "EntityCollection`1" && p.PropertyType.Name != "EntityReference`1" && p.PropertyType.Name != p.Name)
                {
                    columnCount++;
                    rowData.Append("<Cell><Data ss:Type=\"String\">" + p.Name + "</Data></Cell>");
                }
                else
                    break;

            }
            rowData.Append("</Row>");

            foreach (T item in list)
            {
                rowData.Append("<Row>");
                for (int x = 0; x < columnCount; x++) //each (PropertyInfo p in properties)
                {
                    object o = properties[x].GetValue(item, null);
                    string value = o == null ? "" : o.ToString();
                    rowData.Append("<Cell><Data ss:Type=\"String\">" + value + "</Data></Cell>");

                }
                rowData.Append("</Row>");
            }

            var sheet = @"<?xml version=""1.0""?>
                    <?mso-application progid=""Excel.Sheet""?>
                    <Workbook xmlns=""urn:schemas-microsoft-com:office:spreadsheet""
                        xmlns:o=""urn:schemas-microsoft-com:office:office""
                        xmlns:x=""urn:schemas-microsoft-com:office:excel""
                        xmlns:ss=""urn:schemas-microsoft-com:office:spreadsheet""
                        xmlns:html=""http://www.w3.org/TR/REC-html40"">
                        <DocumentProperties xmlns=""urn:schemas-microsoft-com:office:office"">
                            <Author>MSADMIN</Author>
                            <LastAuthor>MSADMIN</LastAuthor>
                            <Created>2011-07-12T23:40:11Z</Created>
                            <Company>Microsoft</Company>
                            <Version>12.00</Version>
                        </DocumentProperties>
                        <ExcelWorkbook xmlns=""urn:schemas-microsoft-com:office:excel"">
                            <WindowHeight>6600</WindowHeight>
                            <WindowWidth>12255</WindowWidth>
                            <WindowTopX>0</WindowTopX>
                            <WindowTopY>60</WindowTopY>
                            <ProtectStructure>False</ProtectStructure>
                            <ProtectWindows>False</ProtectWindows>
                        </ExcelWorkbook>
                        <Styles>
                            <Style ss:ID=""Default"" ss:Name=""Normal"">
                                <Alignment ss:Vertical=""Bottom""/>
                                <Borders/>
                                <Font ss:FontName=""Calibri"" x:Family=""Swiss"" ss:Size=""11"" ss:Color=""#000000""/>
                                <Interior/>
                                <NumberFormat/>
                                <Protection/>
                            </Style>
                            <Style ss:ID=""s62"">
                                <Font ss:FontName=""Calibri"" x:Family=""Swiss"" ss:Size=""11"" ss:Color=""#000000""
                                    ss:Bold=""1""/>
                            </Style>
                        </Styles>
                        <Worksheet ss:Name=""Sheet1"">
                            <Table ss:ExpandedColumnCount=""" + (properties.Count() + 1) + @""" ss:ExpandedRowCount=""" + (list.Count() + 1) + @""" x:FullColumns=""1""
                                x:FullRows=""1"" ss:DefaultRowHeight=""15"">
                                " + rowData.ToString() + @"
                            </Table>
                            <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
                                <PageSetup>
                                    <Header x:Margin=""0.3""/>
                                    <Footer x:Margin=""0.3""/>
                                    <PageMargins x:Bottom=""0.75"" x:Left=""0.7"" x:Right=""0.7"" x:Top=""0.75""/>
                                </PageSetup>
                                <Print>
                                    <ValidPrinterInfo/>
                                    <HorizontalResolution>300</HorizontalResolution>
                                    <VerticalResolution>300</VerticalResolution>
                                </Print>
                                <Selected/>
                                <Panes>
                                    <Pane>
                                        <Number>3</Number>
                                        <ActiveCol>2</ActiveCol>
                                    </Pane>
                                </Panes>
                                <ProtectObjects>False</ProtectObjects>
                                <ProtectScenarios>False</ProtectScenarios>
                            </WorksheetOptions>
                        </Worksheet>
                        <Worksheet ss:Name=""Sheet2"">
                            <Table ss:ExpandedColumnCount=""1"" ss:ExpandedRowCount=""1"" x:FullColumns=""1""
                                x:FullRows=""1"" ss:DefaultRowHeight=""15"">
                            </Table>
                            <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
                                <PageSetup>
                                    <Header x:Margin=""0.3""/>
                                    <Footer x:Margin=""0.3""/>
                                    <PageMargins x:Bottom=""0.75"" x:Left=""0.7"" x:Right=""0.7"" x:Top=""0.75""/>
                                </PageSetup>
                                <ProtectObjects>False</ProtectObjects>
                                <ProtectScenarios>False</ProtectScenarios>
                            </WorksheetOptions>
                        </Worksheet>
                        <Worksheet ss:Name=""Sheet3"">
                            <Table ss:ExpandedColumnCount=""1"" ss:ExpandedRowCount=""1"" x:FullColumns=""1""
                                x:FullRows=""1"" ss:DefaultRowHeight=""15"">
                            </Table>
                            <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
                                <PageSetup>
                                    <Header x:Margin=""0.3""/>
                                    <Footer x:Margin=""0.3""/>
                                    <PageMargins x:Bottom=""0.75"" x:Left=""0.7"" x:Right=""0.7"" x:Top=""0.75""/>
                                </PageSetup>
                                <ProtectObjects>False</ProtectObjects>
                                <ProtectScenarios>False</ProtectScenarios>
                            </WorksheetOptions>
                        </Worksheet>
                    </Workbook>";

            return sheet;
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

        public int? GetSubmitMemberByKey(string key)
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

        public setting GetSettingByKey(string key)
        {
            var dbSetting = Uof.IsettingService.GetAll(s => s.name == key).FirstOrDefault();
            if (dbSetting == null)
            {
                return null;
            }

            return dbSetting;
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