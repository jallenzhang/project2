using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;

using WebNotifications.Models;
using WebNotifications.Helper;
using WebNotifications.Actions;
using WebNotifications.Models.Custom;
using Webdiyer.WebControls.Mvc;
using WebNotifications.Permission;

namespace WebNotifications.Controllers
{
    //[Authorize(Roles = "Administrator")]
    [Description("推广(底下权限可以继承当前权限)")]
    public class PromotionController : BaseController
    {
        DataJackModelDataContext _ctx = new DataJackModelDataContext();        

        [AvoidBrushUserCountAction]
        public void ActionRedirect(string Parterid)
        {
            var parter = AvoidBrushCountHelper.GetParter(Parterid);
            this.Response.Redirect(parter.pageurl);
        }

        [Description("推广--合作商推广用户URL")]
        public ActionResult GetUrlView()
        {
            ViewData["dpPartersList"] = GetDropDownParterName();
            ViewBag.Message = string.Empty;

            return View();
        }

        [Description("推广--合作商推广用户URL")]
        [HttpPost]        
        public ActionResult GetUrlView(string tbName, string dpPartersList)
        {
            ViewData["dpPartersList"] = GetDropDownParterName();

            if (ModelState.IsValid)
            {
                string parterId = dpPartersList.Trim();
                var exist_partner = _ctx.Partners.FirstOrDefault(m => m.partnerid == parterId && m.pageurl == tbName);
                if (null == exist_partner)
                {
                    var m_parner = _ctx.Partners.FirstOrDefault(m => m.partnerid == parterId && (m.pageurl == null || m.pageurl == ""));
                    if (null == m_parner)
                    {
                        var c_parner = _ctx.Partners.FirstOrDefault(m => m.partnerid == parterId);
                        var new_parner = new Partner
                        {
                            name = c_parner.name,
                            note = Server.HtmlEncode(c_parner.note),
                            partnerid = c_parner.partnerid,
                            count = 0,
                            pageurl = tbName
                        };
                        _ctx.Partners.InsertOnSubmit(new_parner);
                    }
                    else
                        m_parner.pageurl = tbName;

                    _ctx.SubmitChanges();
                }
                 
                ViewBag.Message = GenerateUrl(tbName, parterId);
            }

            return View();
        }

        [Description("推广--添加合作商页面")]
        public ActionResult AddParter()
        {
            return View();
        }

        [Description("推广--添加合作商操作")]
        public ActionResult AddParterAction(string tbName, string tbNote)
        {
            Partner parter = new Partner
            {
                name = tbName.Trim(),
                note = Server.HtmlEncode(tbNote.Trim()),
                partnerid = Guid.NewGuid().ToString().Replace("-",""),
                count = 0,
                pageurl = string.Empty
            };
            _ctx.Partners.InsertOnSubmit(parter);
            _ctx.SubmitChanges();

            return View("AddParter");
        }

        [Description("推广--合作商列表")]
        public ActionResult ListParter(int? page)
        {
            var parters = from m in _ctx.Partners
                          select new PartnerExtern
                          {
                              Partner_P = m,
                              Redirect_url = GenerateUrl(m.pageurl, m.partnerid)
                          };
            var pageList = parters.ToPagedList(page ?? 1, SiteConfigHelper.DefaultPageSize);

            return View(pageList);
        }

        #region Private Method

        private SelectList GetDropDownParterName()
        {
            var parters = (from m in _ctx.Partners
                           select new
                           {
                               m.name,
                               m.partnerid
                           }).Distinct();
            var typeList = parters.ToArray().Select(m => new SelectListItem() { Text = m.name, Value = m.partnerid });

            return new SelectList(typeList, "Value", "Text");
        }

        private string GenerateUrl(string source_url, string parterId)
        {
            source_url = source_url.Trim();
            string tparterId = parterId.Trim();
            var obj = (from m in _ctx.Partners
                       where m.pageurl == source_url
                       && m.partnerid == tparterId
                       select m.id).FirstOrDefault();

            var url = SuffixOfUrl() + tparterId + "i" + obj;

            return url;
        }

        private string SuffixOfUrl()
        {
            Uri uri = HttpContext.Request.Url;
            string res = string.Empty;
            if(uri.Port != 80)
                res = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
            else
                res = uri.Scheme + Uri.SchemeDelimiter + uri.Host;
            res += "/Promotion/ActionRedirect?Parterid=";

            return res;
        }

        #endregion
    }
}
