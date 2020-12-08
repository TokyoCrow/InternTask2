﻿using InternTask2.Website.Models;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace InternTask2.Website.Helpers
{
    public  static class ApproveHelper
    {
        public static MvcHtmlString ApproveRejectBtns(this HtmlHelper html, User user)
        {
            StringBuilder result = new StringBuilder();
            if(!user.IsApproved)
            {
                TagBuilder tagAApprove = new TagBuilder("a");
                TagBuilder tagAReject = new TagBuilder("a");
                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                tagAApprove.MergeAttribute("href", urlHelper.Action("Approve","Admin", new { id = user.Id }));
                tagAApprove.InnerHtml = "Approve";
                tagAApprove.AddCssClass("btn btn-success");
                tagAReject.MergeAttribute("href", urlHelper.Action("Reject", "Admin", new { id = user.Id }));
                tagAReject.InnerHtml = "Reject";
                tagAReject.AddCssClass("btn btn-danger");
                result.Append(tagAApprove.ToString());
                result.Append(tagAReject.ToString());
            }
            return MvcHtmlString.Create(result.ToString());
        }
    }
}