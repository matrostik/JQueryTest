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
        public ContentResult UploadFiles()
        {
            //https://github.com/blueimp/jQuery-File-Upload/wiki/Basic-plugin
            string imgurl = string.Empty;
            HttpPostedFileBase img = Request.Files[0] as HttpPostedFileBase;
            if (img != null)
            {
                string pic = System.IO.Path.GetFileName(img.FileName);
                //url = UploadImage(file.FileName);

                using (MemoryStream ms = new MemoryStream())
                {
                    img.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                    imgurl = UploadImageToImgur(array);

                }

            }
            var res = new UploadFilesResult()
            {
                name = img.FileName,
                size = img.ContentLength,
                type = img.ContentType,
                url = imgurl
            };

            //var r = new List<UploadFilesResult>();
            //foreach (string file in Request.Files)
            //{
            //    HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
            //    if (hpf.ContentLength == 0)
            //        continue;

            //    string savedFileName = Path.Combine(Server.MapPath("~/App_Data"), Path.GetFileName(hpf.FileName));
            //    hpf.SaveAs(savedFileName);

            //    r.Add(new UploadFilesResult()
            //    {
            //        name = hpf.FileName,
            //        size = hpf.ContentLength,
            //        type = hpf.ContentType
            //    });
            //}
            string json = JsonConvert.SerializeObject(res);
            //string s = "{\"name\":\"" + r[0].name + "\",\"type\":\"" + r[0].type + "\",\"size\":\"" + string.Format("{0} bytes", r[0].size) + "\"}";
            return Content(json, "application/json");

            //return Content("{\"name\":\"" + r[0].Name + "\",\"type\":\"" + r[0].Type + "\",\"size\":\"" + string.Format("{0} bytes", r[0].Length) + "\"}", "application/json");
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
