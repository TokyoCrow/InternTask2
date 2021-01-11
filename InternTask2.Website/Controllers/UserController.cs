using AutoMapper;
using InternTask2.BLL.Models;
using InternTask2.BLL.Services.Abstract;
using InternTask2.BLL.Services.Concrete;
using InternTask2.Core.Models;
using InternTask2.Website.Models;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace InternTask2.Website.Controllers
{
    [Authorize(Roles = "user")]
    public class UserController : Controller
    {
        private readonly IUserService db;

        public UserController(IUserService ius)
        {
            db = ius;
        }

        public ActionResult Upload()
            => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.UploadDocument(upload);
                }
                catch(ValidationException ex)
                {
                    ModelState.AddModelError(ex.Message, "");
                }
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public ActionResult Download(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<DocumentDTO, Document>()).CreateMapper();
            var document = mapper.Map<DocumentDTO, Document>(db.GetDocumentById((int)id));
            string fileName = document.Name;
            string fileType = MimeMapping.GetMimeMapping(fileName);
            return File(document.Content, fileType, fileName);
        }

        public ActionResult IsDocumentNameUnique(string name)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<DocumentDTO, Document>()).CreateMapper();
            Document document = mapper.Map<DocumentDTO, Document>(db.GetDocumentByName(name));
            if (document != null)
                return Json(false);
            return Json(true);
        }

        public ActionResult Index(int page = 1)
        {
            int pageSize = 15;
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<DocumentDTO, DocumentView>()).CreateMapper();
            IEnumerable<DocumentView> documentsPerPages = mapper.Map<IEnumerable<DocumentDTO>, IEnumerable<DocumentView>>(db.GetDocumentsPage(page, pageSize));

            var pageInfo = new PageInfo
            {
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = db.DocumentsCount()
            };
            var dvm = new DocumentsViewModel { PageInfo = pageInfo, Documents = documentsPerPages };
            return View(dvm);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
