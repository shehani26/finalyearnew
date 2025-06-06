using ELDER_NUTRICIANTS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ELDER_NUTRICIANTS.Controllers
{
    public class HomeController : Controller
    {
        ELDER_NUTRICIANTSEntities db = new ELDER_NUTRICIANTSEntities();
        public ActionResult Index()
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }
            return View();
        }


        public ActionResult Login()
        {
            return View();
        }

        public ActionResult CheckLogin(string username, string password)
        {
            int isLogged = 0;

            var user = db.APP_USER.FirstOrDefault(x =>
            x.USER_NAME.Equals(username, StringComparison.Ordinal) &&
            x.STATUS == 0);

            if (user != null)
            {
                //// Verify the password (Assuming passwords are stored as hashes)
                if (VerifyPassword(password, user.PASSWORD))
                {
                    FormsAuthentication.SetAuthCookie(user.USER_NAME, false);
                    Session["User_Name"] = user.USER_NAME;
                    Session["Role"] = user.ROLE;
                    Session["Pro"] = user.PIC;





                    


                    isLogged = 1;





                }
            }
            else
            {
                isLogged = 9;
            }

            return Json(isLogged, JsonRequestBehavior.AllowGet);
        }
        public static bool VerifyPassword(string inputPassword, string storedPassword)
        {
            try
            {

                string[] parts = storedPassword.Split(':');
                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] storedHash = Convert.FromBase64String(parts[1]);

                using (var algorithm = new Rfc2898DeriveBytes(inputPassword, salt, 10000, HashAlgorithmName.SHA256))
                {
                    byte[] computedHash = algorithm.GetBytes(32);
                    return computedHash.SequenceEqual(storedHash);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ActionResult Error()
        {
            return View();
        }


        public ActionResult Logout()
        {
            // Logout logic here
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login", "Home");
        }

        public ActionResult AIndex()
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            var q = db.APP_USER.Where(x=>x.ROLE == "ADMIN").ToList();
            return View(q);
        }



        // GET: Caregiver/Create
        public ActionResult ACreate()
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }
            


            return View();
        }

        // POST: Caregiver/Create
        [HttpPost]
        public ActionResult ACreate(AdminVM stc)
        {
            try
            {

                try
                {

                    string username = Session["User_Name"].ToString();

                    var haverecord = db.APP_USER.Where(x => x.USER_NAME == stc.USER_NAME).FirstOrDefault();

                    if (haverecord != null)
                    {
                        TempData["Alert1"] = "User name Cannot Be Dupplicated";
                        return RedirectToAction("ACreate");
                    }

                    if (stc.Img_File_F != null && stc.Img_File_F.ContentLength >= 0)
                    {
                        try
                        {
                            string fileanme = Path.GetFileNameWithoutExtension(stc.Img_File_F.FileName);
                            string extension = Path.GetExtension(stc.Img_File_F.FileName);
                            fileanme = fileanme + DateTime.Now.ToString("yymmssfff") + extension;
                            stc.PIC = "~/Images/" + fileanme;
                            fileanme = Path.Combine(Server.MapPath("~/Images/"), fileanme);
                            stc.Img_File_F.SaveAs(fileanme);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred: {ex.Message}");
                            // Handle the exception appropriately (log, display an error message, etc.)
                        }
                    }

                    string EncyPassword = HashPassword(stc.PASSWORD);



                  


                    APP_USER st1 = new APP_USER
                    {
                        USER_NAME = stc.USER_NAME,
                        PASSWORD = EncyPassword,
                        FULL_NAME = stc.FULL_NAME,

                        ROLE = "ADMIN",
                        LOGIN_ATTEMPT = 0,

                        ENTER_USER = username,
                        EMAIL = stc.EMAIL,

                        REF_ID =0,
                        PIC = stc.PIC,



                        STATUS = 0,
                        ENTER_DATE = System.DateTime.Now


                    };

                    db.APP_USER.Add(st1);
                    db.SaveChanges();


              

                    

                    TempData["Alert"] = "Admin Added Successfully & He/She User Name is " + st1.USER_NAME;
                    return RedirectToAction("AIndex");
                }
                catch (Exception ex)
                {
                    // Rollback the transaction if any error occurs


                    TempData["Alert1"] = "Error: " + ex.Message;
                    return RedirectToAction("ACreate");
                }
            }

            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

                TempData["Alert1"] = "Error" + " " + dbEx;
                return RedirectToAction("ACreate");
                //save = false;
            }
        }

        public static string HashPassword(string password)
        {
            using (var algorithm = new Rfc2898DeriveBytes(password, 16, 10000, HashAlgorithmName.SHA256))
            {
                byte[] salt = algorithm.Salt;
                byte[] hash = algorithm.GetBytes(32);

                return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
            }
        }


        public ActionResult ADelete(int id)
        {
            string username = Session["User_Name"]?.ToString();

            if (username == null)
                return RedirectToAction("Login", "Home"); // fallback

           

            var user = db.APP_USER.FirstOrDefault(x => x.ID == id);
            if (user != null)
            {
                user.STATUS = 9;
                db.Entry(user).State = EntityState.Modified;
            }

            db.SaveChanges();


            TempData["Alert"] = "Admin Deleted Successfully....!";
            return RedirectToAction("AIndex");
        }
    }
}