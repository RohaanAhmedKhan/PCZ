using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PCZ.Helpers {
    public class ImageIO : Controller {


        public string saveImage(HttpPostedFileBase image, string name) {

            string filename = name.Replace("-", "") + Path.GetExtension(image.FileName);
            string fileLocation = Path.Combine(Server.MapPath("~/Content/Images/"), filename);

            image.SaveAs(fileLocation);
            return "/Content/Images/" + filename;
        }

        public void deleteImage(string url) {

            string file = Path.Combine(Server.MapPath(url));
            System.IO.File.Delete(file);

        }
    }
}