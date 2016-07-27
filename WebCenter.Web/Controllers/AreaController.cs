﻿using System.Linq;
using System.Web.Mvc;
using WebCenter.IServices;
using WebCenter.Entities;
using Common;

namespace WebCenter.Web.Controllers
{
    public class AreaController : BaseController
    {
        public AreaController(IUnitOfWork UOF)
            : base(UOF)
        {
        }

        public ActionResult List()
        {
            var list = Uof.IareaService.GetAll().Select(a => new
            {
                id = a.id,
                name = a.name,
                description = a.description
            }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Get(int id)
        {
            var _area = Uof.IareaService.GetAll(a => a.id == id).FirstOrDefault();
            if (_area == null)
            {
                return ErrorResult;
            }

            return Json(_area, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Add(string name, string description)
        {
            var r = Uof.IareaService.AddEntity(new area()
            {
                name = name,
                description = description
            });

            return SuccessResult;
        }

        [HttpPost]
        public ActionResult Update(int id, string name, string description)
        {
            var _area = Uof.IareaService.GetAll(a=>a.id == id).FirstOrDefault();
            if (_area == null)
            {
                return ErrorResult;
            }
            if (_area.name == name && _area.description == description)
            {
                return SuccessResult;
            }
            _area.name = name;
            _area.description = description;

            var r = Uof.IareaService.UpdateEntity(_area);
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var _area = Uof.IareaService.GetAll(a => a.id == id).FirstOrDefault();
            if (_area == null)
            {
                return ErrorResult;
            }

            var r = Uof.IareaService.DeleteEntity(_area);
            return Json(new { success = r }, JsonRequestBehavior.AllowGet);
        }
    }
}