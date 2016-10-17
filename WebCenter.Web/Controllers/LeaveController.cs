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
    public class LeaveController : BaseController
    {
        public LeaveController(IUnitOfWork UOF)
            : base(UOF)
        {
            
        }

        public ActionResult Add(leave _leave)
        { 
            _leave.status = 0;
            var dbLeave = Uof.IleaveService.AddEntity(_leave);

            if (dbLeave != null)
            {
                Uof.IwaitdealService.AddEntity(new waitdeal
                {
                    source = "leave",
                    source_id = dbLeave.id,
                    user_id = dbLeave.auditor_id,
                    router = "leave_view",
                    content = "您有新的假单需要审批",
                    read_status = 0
                });
            }

            return SuccessResult;
        }
        
    }
}