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

namespace ELDER_NUTRICIANTS.Controllers
{
    public class CaregiverController : Controller
    {
        ELDER_NUTRICIANTSEntities db = new ELDER_NUTRICIANTSEntities();
        // GET: Caregiver
        public ActionResult Index()
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            var q = db.CAREGIVERs.ToList();
            return View(q);
        }

     

        // GET: Caregiver/Create
        public ActionResult Create()
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            

            return View();
        }

        // POST: Caregiver/Create
        [HttpPost]
        public ActionResult Create(CaregiverVM stc)
        {
            try
            {

                try
                {

                    string username = Session["User_Name"].ToString();

                    var haverecord = db.CAREGIVERs.Where(x => x.NIC == stc.NIC).FirstOrDefault();

                    if (haverecord != null)
                    {
                        TempData["Alert1"] = "Caregiver NIC Cannot Be Dupplicated";
                        return RedirectToAction("Create");
                    }

                    if (stc.Img_File_F != null && stc.Img_File_F.ContentLength >= 0)
                    {
                        try
                        {
                            string fileanme = Path.GetFileNameWithoutExtension(stc.Img_File_F.FileName);
                            string extension = Path.GetExtension(stc.Img_File_F.FileName);
                            fileanme = fileanme + DateTime.Now.ToString("yymmssfff") + extension;
                            stc.PROFILE_IMG = "~/Images/" + fileanme;
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



                    CAREGIVER st = new CAREGIVER
                    {
                        CAREGIVER_CODE = db.PROC_INCREMENT_ID("C").FirstOrDefault(),
                        FULL_NAME = stc.FULL_NAME,
                        GENDER = stc.GENDER,

                        DOB = stc.DOB,
                        NIC = stc.NIC,

                        CONTACT_NO = stc.CONTACT_NO,
                        EMAIL = stc.EMAIL,

                        ADDRESS = stc.ADDRESS,
                        PROFILE_IMG = stc.PROFILE_IMG,

                        HIGHER_QULIFICATION = stc.HIGHER_QULIFICATION,
                        YEAR_OF_EXPERIENCE = stc.YEAR_OF_EXPERIENCE,

                        STATUS = 0,
                        ENTER_DATE = System.DateTime.Now,

                        ENTER_BY = username
                    };

                    db.CAREGIVERs.Add(st);
                    db.SaveChanges();


                    APP_USER st1 = new APP_USER
                    {
                        USER_NAME = st.CAREGIVER_CODE,
                        PASSWORD = EncyPassword,
                        FULL_NAME = stc.FULL_NAME,

                         ROLE= "CAREGIVER",
                        LOGIN_ATTEMPT = 0,

                        ENTER_USER = username,
                        EMAIL = stc.EMAIL,

                        REF_ID = st.ID,
                        PIC = stc.PROFILE_IMG,

                       

                        STATUS = 0,
                        ENTER_DATE = System.DateTime.Now

                        
                    };

                    db.APP_USER.Add(st1);
                    db.SaveChanges();


                    db.PROC_INCREMENT_ID_UPADATE("C");

                    TempData["Alert"] = "Caregiver Added Successfully & He/She User Name is "+ st.CAREGIVER_CODE;
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Rollback the transaction if any error occurs


                    TempData["Alert1"] = "Error: " + ex.Message;
                    return RedirectToAction("Create");
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
                return RedirectToAction("Create");
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
        // GET: Caregiver/Edit/5
        public ActionResult CEdit(int Cid)
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }
          


            CaregiverVM Bedt = new CaregiverVM();

            var DbData = db.CAREGIVERs.Where(x => x.ID == Cid).FirstOrDefault();





            CaregiverVM Wareh = new CaregiverVM()
            {

                ID = DbData.ID,
                CAREGIVER_CODE = DbData.CAREGIVER_CODE,
                FULL_NAME = DbData.FULL_NAME,
                GENDER = DbData.GENDER,

                DOB = DbData.DOB,
                NIC = DbData.NIC,
                CONTACT_NO = DbData.CONTACT_NO,
                EMAIL = DbData.EMAIL,
                ADDRESS = DbData.ADDRESS,
                PROFILE_IMG = DbData.PROFILE_IMG,
                HIGHER_QULIFICATION = DbData.HIGHER_QULIFICATION,
                YEAR_OF_EXPERIENCE = DbData.YEAR_OF_EXPERIENCE




            };
            Bedt.Equals(Wareh);


            return View(Wareh);
        }

        // POST: Caregiver/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, CaregiverVM stc)
        {
            try
            {
                string username = Session["User_Name"].ToString();
                var haverecord = db.CAREGIVERs.Where(x => x.NIC == stc.NIC && x.ID != id).FirstOrDefault();
                if (haverecord != null)
                {
                    TempData["Alert1"] = "Caregiver NIC Cannot Be Dupplicated";
                    return RedirectToAction("CEdit", new { Cid = id });
                }

                if (stc.Img_File_F != null && stc.Img_File_F.ContentLength >= 0)
                {
                    try
                    {
                        string fileanme = Path.GetFileNameWithoutExtension(stc.Img_File_F.FileName);
                        string extension = Path.GetExtension(stc.Img_File_F.FileName);
                        fileanme = fileanme + DateTime.Now.ToString("yymmssfff") + extension;
                        stc.PROFILE_IMG = "~/Images/" + fileanme;
                        fileanme = Path.Combine(Server.MapPath("~/Images/"), fileanme);
                        stc.Img_File_F.SaveAs(fileanme);



                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        // Handle the exception appropriately (log, display an error message, etc.)
                    }

                 

                  

                    CAREGIVER st = db.CAREGIVERs.Where(x => x.ID == id).FirstOrDefault();
                    {
                        st.FULL_NAME = stc.FULL_NAME;
                        st.GENDER = stc.GENDER;
                        st.DOB = stc.DOB;
                        st.NIC = stc.NIC;
                        st.CONTACT_NO = stc.CONTACT_NO;
                        st.EMAIL = stc.EMAIL;
                        st.ADDRESS = stc.ADDRESS;

                        st.PROFILE_IMG = stc.PROFILE_IMG;
                        st.HIGHER_QULIFICATION = stc.HIGHER_QULIFICATION;
                        st.YEAR_OF_EXPERIENCE = stc.YEAR_OF_EXPERIENCE;
                        st.UPDATED_DATE =System.DateTime.Now;
                        st.UPDATE_BY = username;

                      

                        db.Entry(st).State = EntityState.Modified;
                        db.SaveChanges();

                    }


                    APP_USER st1 = db.APP_USER.Where(x => x.REF_ID == id && x.ROLE == "CAREGIVER").FirstOrDefault();
                    {

                        if (st1 != null)
                        {
                            st1.FULL_NAME = stc.FULL_NAME;
                            st1.EMAIL = stc.EMAIL;
                            st1.PIC = stc.PROFILE_IMG;




                            db.Entry(st1).State = EntityState.Modified;
                            db.SaveChanges();
                        }



                    }
                }
                else
                {
                    CAREGIVER st = db.CAREGIVERs.Where(x => x.ID == id).FirstOrDefault();
                    {
                        st.FULL_NAME = stc.FULL_NAME;
                        st.GENDER = stc.GENDER;
                        st.DOB = stc.DOB;
                        st.NIC = stc.NIC;
                        st.CONTACT_NO = stc.CONTACT_NO;
                        st.EMAIL = stc.EMAIL;
                        st.ADDRESS = stc.ADDRESS;

                    
                        st.HIGHER_QULIFICATION = stc.HIGHER_QULIFICATION;
                        st.YEAR_OF_EXPERIENCE = stc.YEAR_OF_EXPERIENCE;
                        st.UPDATED_DATE = System.DateTime.Now;
                        st.UPDATE_BY = username;



                        db.Entry(st).State = EntityState.Modified;
                        db.SaveChanges();

                    }


                    APP_USER st1 = db.APP_USER.Where(x => x.REF_ID == id && x.ROLE == "CAREGIVER").FirstOrDefault();
                    {

                        if(st1 != null)
                        {
                            st1.FULL_NAME = stc.FULL_NAME;
                            st1.EMAIL = stc.EMAIL;





                            db.Entry(st1).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                      

                    }
                }



                TempData["Alert"] = "Caregiver Updated Successfully....!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Alert1"] = "Error" + " " + ex;
                return RedirectToAction("CEdit", new { Cid = id });
            }
        }

        // GET: Caregiver/Delete/5
        public ActionResult Delete(int id)
        {
            string username = Session["User_Name"]?.ToString();

            if (username == null)
                return RedirectToAction("Login", "Home"); // fallback

            var caregiver = db.CAREGIVERs.FirstOrDefault(x => x.ID == id);
            if (caregiver != null)
            {
                caregiver.STATUS = 1;
                caregiver.UPDATED_DATE = DateTime.Now;
                caregiver.UPDATE_BY = username;
                db.Entry(caregiver).State = EntityState.Modified;
            }

            var user = db.APP_USER.FirstOrDefault(x => x.REF_ID == id && x.ROLE == "CAREGIVER");
            if (user != null)
            {
                user.STATUS = 9;
                db.Entry(user).State = EntityState.Modified;
            }

            db.SaveChanges();


            TempData["Alert"] = "Caregiver Deleted Successfully....!";
            return RedirectToAction("Index");
        }



    }
}
