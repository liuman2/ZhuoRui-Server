using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebCenter.Entities;
using WebCenter.IServices;

namespace WebCenter.Web.Controllers
{
    public class FeedbackController : BaseController
    {
        public FeedbackController(IUnitOfWork UOF)
            : base(UOF)
        {

        }

        [HttpPost]
        public ActionResult Send(FeedbackRequest request)
        {
            request.content = Regex.Replace(request.content, @"\p{Cs}", " ");
            

            Uof.IfeedbackService.AddEntity(new feedback()
            {
                creator = request.creator,
                content = request.content,
                date_created = DateTime.Now
            });

            return base.SuccessResult;
        }
    }
}