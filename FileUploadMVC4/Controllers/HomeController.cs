using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FileUploadMVC4.Models;
using System.Net;
using System.Xml.Linq;
using System.Text;
using Newtonsoft.Json;

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
        public ContentResult UploadFiles(HttpPostedFileBase file)
        {
            //https://github.com/blueimp/jQuery-File-Upload/wiki/Basic-plugin

            string imgurl = string.Empty;
            if (file != null)
            {
                string pic = System.IO.Path.GetFileName(file.FileName);
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                    imgurl = UploadImageToImgur(array);
                }

            }
            var res = new UploadFilesResult()
            {
                name = file.FileName,
                size = file.ContentLength,
                type = file.ContentType,
                url = imgurl
            };

            string json = JsonConvert.SerializeObject(res);
            return Content(json, "application/json");
        }

        string ClientId = "6b18f55eeee07f1";
        public string UploadImageToImgur(byte[] image)
        {
            WebClient w = new WebClient();
            w.Headers.Add("Authorization", "Client-ID " + ClientId);
            System.Collections.Specialized.NameValueCollection Keys = new System.Collections.Specialized.NameValueCollection();
            try
            {
                Keys.Add("image", Convert.ToBase64String(image));
                byte[] responseArray = w.UploadValues("https://api.imgur.com/3/image.xml", Keys);
                dynamic result = Encoding.ASCII.GetString(responseArray);
                XDocument xml = XDocument.Parse(result);
                var i = xml.Root.Element("link").Value;
                return i;
            }
            catch (Exception s)
            {
                //MessageBox.Show("Something went wrong. " + s.Message);
                return s.Message;
            }
        }

    }
}
