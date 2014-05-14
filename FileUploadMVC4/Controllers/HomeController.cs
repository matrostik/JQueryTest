using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FileUploadMVC4.Models;

namespace FileUploadMVC4.Controllers
{
	public class HomeController : Controller
	{
		//
		// GET: /Home/

		public ActionResult Index()
		{
			return View();
		}

		public FilePathResult Image()
		{
			string filename = Request.Url.AbsolutePath.Replace("/home/image", "");
			string contentType = "";
			var filePath = new FileInfo(Server.MapPath("~/App_Data") + filename);

			var index = filename.LastIndexOf(".") + 1;
			var extension = filename.Substring(index).ToUpperInvariant();

			// Fix for IE not handling jpg image types
			contentType = string.Compare(extension, "JPG") == 0 ? "image/jpeg" : string.Format("image/{0}", extension);

			return File(filePath.FullName, contentType);
		}

		[HttpPost]
		public ContentResult UploadFiles()
		{
			var r = new List<UploadFilesResult>();

			foreach (string file in Request.Files)
			{
				HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
				if (hpf.ContentLength == 0)
					continue;

				string savedFileName = Path.Combine(Server.MapPath("~/App_Data"), Path.GetFileName(hpf.FileName));
				hpf.SaveAs(savedFileName);

				r.Add(new UploadFilesResult()
				{
					Name = hpf.FileName,
					Length = hpf.ContentLength,
					Type = hpf.ContentType
				});
			}
			return Content("{\"name\":\"" + r[0].Name + "\",\"type\":\"" + r[0].Type + "\",\"size\":\"" + string.Format("{0} bytes", r[0].Length) + "\"}", "application/json");
		}

	}
}
