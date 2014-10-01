using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.EntityClient;
using System.Data.SqlClient;
using LoginForm.Models;
using System.Web.Routing;
using System.IO;
using System.Data;
using System.Net;
using System.Xml;

namespace LoginForm.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        Database1Entities cx = new Database1Entities();
       
       // List<Login> list = cx.Logins.ToList();
        public ActionResult Index()
        {
            List<Login> list = cx.Logins.ToList();
            return View(list);
        }

        public ActionResult verify()
        {
            List<Login> list = cx.Logins.ToList();
            String userName = Request["us"];
            String pass = Request["pass"];
            if (pass.Length < 4)
            {
                return RedirectToAction("SecQuestion", new { id = list[0].Id.ToString() });
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].UserName == userName && list[i].Password == pass)
                {
                    int id =  list[i].Id ;
                    TempData["id"] = id.ToString();
                    return RedirectToAction("SecQuestion", new { id = id.ToString()});
                }
            }
            return RedirectToAction("Invalid");
        }

        

        
        public ActionResult SecQuestion(String id)
        {

            ViewBag.ques = int.Parse(id);
            //Login log = cx.Logins.Find(int.Parse(Id));
            return View(int.Parse(id));
        }

      public ActionResult validate(int id)
        {
            List<Login> list = cx.Logins.ToList();
            String ans = Request["ans"];
            Login log = cx.Logins.Find(id);

            if (log.Answer == ans)
            {
                return RedirectToAction("successfull");
            }
            return RedirectToAction("Invalid");
        }


      public ActionResult Invalid()
      {
          return View();
      }

        
        public ActionResult successfull()
        {
            return View();
        }

        public static string GetIPAddress(HttpRequestBase request)
        {
            string ip;
            try
            {
                ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(ip))
                {
                    if (ip.IndexOf(",") > 0)
                    {
                        string[] ipRange = ip.Split(',');
                        int le = ipRange.Length - 1;
                        ip = ipRange[le];
                    }
                }
                else
                {
                    ip = request.UserHostAddress;
                }
            }
            catch { ip = null; }

            return ip;
        }

        private DataTable GetLocation(string ipaddress)
        {
            WebRequest rssReq = WebRequest.Create("http://freegeoip.appspot.com/xml/" + ipaddress);
            WebProxy px = new WebProxy("http://freegeoip.appspot.com/xml/" + ipaddress, true);
            rssReq.Proxy = px;
            rssReq.Timeout = 2000;
            try
            {
                WebResponse rep = rssReq.GetResponse();
                XmlTextReader xtr = new XmlTextReader(rep.GetResponseStream());
                DataSet ds = new DataSet();
                ds.ReadXml(xtr);
                return ds.Tables[0];
            }
            catch
            {
                return null;
            }
        }
    }
}
