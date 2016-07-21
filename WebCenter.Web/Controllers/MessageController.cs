using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCenter.IServices;

namespace WebCenter.Web.Controllers
{
    public class MessageController : BaseController
    {
        public MessageController(IUnitOfWork UOF)
            : base(UOF)
        {

        }
                
        public ActionResult GetMessages(int pageIndex, int pageSize, int userId)
        {
            var _count = Uof.ImessageService.GetAll(m => m.user_id == userId).Count();

            var list = Uof.ImessageService.GetAll(m => m.user_id == userId).OrderByDescending(item => item.id)
                .Select(m => new
                {
                    id = m.id,
                    source_id = m.source_id,
                    content = m.content,
                    date_created = m.date_created.Value,
                    type = m.type
                }).ToPagedList(pageIndex, pageSize).ToList();

            var messageResponse = new List<MessageResponse>();
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    var t = (MessageType)Enum.ToObject(typeof(MessageType), item.type);
                    var icon = "";
                    switch (t)
                    {
                        case MessageType.Organization:
                            icon = "k-cube";
                            if (item.content.IndexOf("创建") >-1)
                            {
                                icon = "k-cubes";
                            }                            
                            break;
                        case MessageType.Project:
                            icon = "k-building";
                            break;
                        case MessageType.Business:
                            icon = "k-bell";
                            break;
                        case MessageType.Permission:
                            icon = "k-cog-alt";
                            break;
                        default:
                            break;
                    }

                    messageResponse.Add(new MessageResponse()
                    {
                        id = item.id,
                        source_id = item.source_id,
                        content = item.content,
                        date_created = item.date_created.ToString("yyyy年MM月dd日"),
                        type = item.type.Value,
                        icon = icon
                    });
                }
            }

            return Json(new { total = _count, messages = messageResponse }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownRefresh(int firstId, int userId)
        {
            var list = Uof.ImessageService.GetAll(m => m.user_id == userId && m.id > firstId).OrderBy(item => item.id)
                .Select(m => new
                {
                    id = m.id,
                    source_id = m.source_id,
                    content = m.content,
                    date_created = m.date_created.Value,
                    type = m.type
                }).ToList();

            var messageResponse = new List<MessageResponse>();
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    var t = (MessageType)Enum.ToObject(typeof(MessageType), item.type);
                    var icon = "";
                    switch (t)
                    {
                        case MessageType.Organization:
                            icon = "k-cube";
                            if (item.content.IndexOf("创建") > -1)
                            {
                                icon = "k-cubes";
                            }
                            break;
                        case MessageType.Project:
                            icon = "k-building";
                            break;
                        case MessageType.Business:
                            icon = "k-bell";
                            break;
                        case MessageType.Permission:
                            icon = "k-cog-alt";
                            break;
                        default:
                            break;
                    }

                    messageResponse.Add(new MessageResponse()
                    {
                        id = item.id,
                        source_id = item.source_id,
                        content = item.content,
                        date_created = item.date_created.ToString("yyyy年MM月dd日"),
                        type = item.type.Value,
                        icon = icon
                    });
                }
            }

            return Json(new { messages = messageResponse }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDetail(int source_id)
        {
            var deal = Uof.IwaitdealService.GetById(source_id);
            var reviewer_name = "";

            if (deal.reviewer != null)
            {
               var reviewer =  Uof.IuserService.GetById(deal.reviewer.Value);
                if (reviewer != null)
                {
                    reviewer_name = reviewer.name;
                }
            }
            var response = new MessageDetailResponse()
            {
                title = deal.title,
                content_line = JsonConvert.DeserializeObject<List<WaitdealLine>>(deal.content),
                creator_name = deal.user.name,
                reviewer_name = reviewer_name,
                date_created = deal.date_created,
                icon = "k-pencil",
                time_desc = "",
                pass_desc = deal.status == -1 ? string.Format("管理员{0}拒绝了该项申请", reviewer_name) : "已通过"
            };

            if (response.title.Contains("更改"))
            {
                response.icon = "k-pencil";
            }
            else if (response.title.Contains("删除"))
            {
                response.icon = "k-trash";
            }

            var now = DateTime.Now;
            var diff = now - response.date_created.Value;
            if (diff.Days > 0)
            {
                response.time_desc = string.Format("由{0}在{1}天前发起", response.creator_name, diff.Days);
            }
            else if (diff.Hours > 0)
            {
                response.time_desc = string.Format("由{0}在{1}小时前发起", response.creator_name, diff.Hours);
            }
            else if (diff.Minutes > 0)
            {
                response.time_desc = string.Format("由{0}在{1}分钟前发起", response.creator_name, diff.Minutes);
            }
            else
            {
                response.time_desc = string.Format("由{0}刚刚发起", response.creator_name);
            }

            return Json(new { detail = response }, JsonRequestBehavior.AllowGet);
        }
    }
}