using SMSPortal.Business_Objects;
using SMSPortal.Models;
using SMSPortal.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SMSPortal.Controllers
{
    
    public class HomeController : Controller
    {
        ICharts _ICharts;
        ICharts2 _ICharts2;
        public HomeController()
        {
            _ICharts = new ChartsConcrete();
            _ICharts2 = new ChartConcrete2();

        }

        private ePOD_DBEntities1 db = new ePOD_DBEntities1();

        public ActionResult Index()
        {
            //return View();
            return RedirectToAction("Login");
            //return RedirectToAction("Layout");
            //return RedirectToAction("Layouts");
            //return RedirectToAction("Dashboard");
            //test repush
            //test push again
            //
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return View();
        }


        [AllowAnonymous]
        public ActionResult LoginTest(string returnUrl)
        {
            return View();
        }


        [AllowAnonymous]
        public ActionResult Layout(string returnUrl)
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Layouts(string returnUrl)
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Dashboard(string returnUrl)
        {
            try
            {
                string tempSms = string.Empty;
                string tempMonth = string.Empty;
                
                _ICharts.smsCounter(out tempSms, out tempMonth);
                
                ViewBag.sms_List = tempSms.Trim();
                
                ViewBag.month_List = tempMonth.Trim();
               


                string DailySmsWhatsappCount = string.Empty;
                string msgTypeCount = string.Empty;
                _ICharts2.smsWhatsappCounter(out DailySmsWhatsappCount, out msgTypeCount);
                ViewBag.smsWhatssappDaily_List = DailySmsWhatsappCount.Trim();
                ViewBag.msgType_List = msgTypeCount.Trim();

                return View();
            }
            catch (Exception)
            {
                throw;
            }
            //return View();
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
          

            return View();
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult UserRegistration(//String __RequestVerificationToken, 
            String firstname, String lastname, String email, String userPass
                                        )
        {
            newUser user_reg = new newUser();
            user_reg.msg = "test";
            user_reg.status = true;
            
            try
            {
                userlogin new_user2 = new userlogin();
                
                new_user2.username = firstname + "." + lastname;
                new_user2.password = Hash(userPass);
                new_user2.email = email;
                new_user2.roleID = 1;
                new_user2.datecreated = DateTime.Now;

              

                db.userlogins.Add(new_user2);
                db.SaveChanges();

               
                user_reg.msg = "User created succesfully!!";
                user_reg.status = true;
                return Json(user_reg);
            }
            catch (Exception ex)
            {

                user_reg.msg = "Error in user creation -- " + ex.Message;
                user_reg.status = false;
                return Json(user_reg);
            }
            
            
        }

        public string Hash(string password)
        {
            var bytes = new UTF8Encoding().GetBytes(password);
            var hashBytes = System.Security.Cryptography.MD5.Create().ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UserLogin(String __RequestVerificationToken, String username, String userPass
                                        )
        {
            
            userLoging user_login = new userLoging();
            string token = __RequestVerificationToken;
            string password = Hash(userPass);
            string msge;


            try
            {
                userlogin LoginUserx = db.userlogins.Where(a => a.username == username && a.password == password).FirstOrDefault();
                if (LoginUserx != null)
                {
                    user_login.msg = "successful  login";
                    user_login.status = true;
                    Session["user_ID"] = username;
                    return Json(user_login);
                }
                else
                {
                    user_login.msg = "invalid login username/password";
                    user_login.status = false;
                    return Json(user_login);
                }
            }
            catch(Exception ex)
            {

                msge = ex.Message;
            }

            return Json(user_login);



        }

        public ActionResult LogOff()
        {            

            Session.Clear();

            return RedirectToAction("Login");
        }
    }




}