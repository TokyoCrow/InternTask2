using InternTask2.Website.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace InternTask2.Website.Controllers
{
    [Authorize(Roles = "user")]
    public class UserController : Controller
    {
        private ApplicationContext db;

        public UserController()
        {
            db = new ApplicationContext();
        }

        public ActionResult Upload()
            => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                string fileName = Path.GetFileName(upload.FileName);
                Document document = await db.Documents.FirstOrDefaultAsync(doc => doc.Name == fileName);
                if (document == null)
                {
                    byte[] content;
                    using (var binaryReader = new BinaryReader(upload.InputStream))
                        content = binaryReader.ReadBytes(upload.ContentLength);
                    document = new Document
                    {
                        Content = content,
                        Name = fileName
                    };
                    document.Modified = SharePointManager.AddNewDocument(document);
                    db.Documents.Add(document);

                    await db.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<ActionResult> Download(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Document document = await db.Documents.FirstOrDefaultAsync(doc => doc.Id == id);
            string fileName = document.Name;
            string fileType = MimeMapping.GetMimeMapping(fileName);
            return File(document.Content, fileType, fileName);
        }

        public ActionResult IsDocumentNameUnique(string name)
        {
            Document document = db.Documents.FirstOrDefault(doc => doc.Name == name);
            if (document != null)
                return Json(false);
            return Json(true);
        }

        public ActionResult Index(int page = 1)
        {
            int pageSize = 15;
            IEnumerable<Document> documentsPerPages = db.Documents
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            PageInfo pageInfo = new PageInfo
            {
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = db.Documents.Count()
            };
            DocumentsViewModel dvm = new DocumentsViewModel { PageInfo = pageInfo, Documents = documentsPerPages };
            return View(dvm);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
