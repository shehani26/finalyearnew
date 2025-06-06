using ELDER_NUTRICIANTS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace ELDER_NUTRICIANTS.Controllers
{
    public class DoctorController : Controller
    {
        ELDER_NUTRICIANTSEntities db = new ELDER_NUTRICIANTSEntities();
        // GET: Doctor
        public ActionResult Index()
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            var q = db.DOCTORs.ToList();
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
        public ActionResult Create(DoctorVM stc)
        {
            try
            {

                try
                {

                    string username = Session["User_Name"].ToString();

                    var haverecord = db.DOCTORs.Where(x => x.MEDICAL_LICENSE_NO == stc.MEDICAL_LICENSE_NO).FirstOrDefault();

                    if (haverecord != null)
                    {
                        TempData["Alert1"] = "Doctor Licence No Cannot Be Dupplicated";
                        return RedirectToAction("Create");
                    }

                    if (stc.Img_File_F != null && stc.Img_File_F.ContentLength >= 0)
                    {
                        try
                        {
                            string fileanme = Path.GetFileNameWithoutExtension(stc.Img_File_F.FileName);
                            string extension = Path.GetExtension(stc.Img_File_F.FileName);
                            fileanme = fileanme + DateTime.Now.ToString("yymmssfff") + extension;
                            stc.PROFILE_IMAGE = "~/Images/" + fileanme;
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



                    DOCTOR st = new DOCTOR
                    {
                        D_CODE = db.PROC_INCREMENT_ID("D").FirstOrDefault(),
                        FULL_NAME = stc.FULL_NAME,
                        GENDER = stc.GENDER,

                        DOB = stc.DOB,
                        MEDICAL_LICENSE_NO = stc.MEDICAL_LICENSE_NO,

                        CONTACT_NO = stc.CONTACT_NO,
                        EMAIL = stc.EMAIL,

                        ADDRESS = stc.ADDRESS,
                        PROFILE_IMAGE = stc.PROFILE_IMAGE,

                        SPECIALIZATION = stc.SPECIALIZATION,
                        REGISTERED_HOSPITAL = stc.REGISTERED_HOSPITAL,
                        JOINED_DATE = stc.JOINED_DATE,
                        STATUS = 0,
                        CREATED_DATE = System.DateTime.Now,

                        CREATED_BY = username
                    };

                    db.DOCTORs.Add(st);
                    db.SaveChanges();


                    APP_USER st1 = new APP_USER
                    {
                        USER_NAME = st.D_CODE,
                        PASSWORD = EncyPassword,
                        FULL_NAME = stc.FULL_NAME,

                        ROLE = "DOCTOR",
                        LOGIN_ATTEMPT = 0,

                        ENTER_USER = username,
                        EMAIL = stc.EMAIL,

                        REF_ID = st.ID,
                        PIC = stc.PROFILE_IMAGE,



                        STATUS = 0,
                        ENTER_DATE = System.DateTime.Now


                    };

                    db.APP_USER.Add(st1);
                    db.SaveChanges();


                    db.PROC_INCREMENT_ID_UPADATE("D");

                    var companyemailDetails = db.COMPANies.FirstOrDefault();

                    if (companyemailDetails != null)
                    {
                        string from = companyemailDetails.EMAIL;
                        string fromname = companyemailDetails.COMPANYNAME;
                        string from_password = companyemailDetails.EMAIL_PASSWORD;
                        string to = stc.EMAIL;
                        string Toname = st1.FULL_NAME;
                        string subject = "L'derly Nutri Created New Account Alert";
                        string body = "Your user name is: " + st1.USER_NAME + " Password is: " + stc.PASSWORD;

                        var fromAddress = new MailAddress(from, fromname);
                        var toAddress = new MailAddress(to, Toname);


                        var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(fromAddress.Address, from_password)
                        };
                        using (var message = new MailMessage(fromAddress, toAddress)
                        {
                            Subject = subject,
                            Body = body
                        })
                        {
                            smtp.Send(message);
                        }
                    }

                    TempData["Alert"] = "Doctor Added Successfully & He/She User Name is " + st.D_CODE;
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



            DoctorVM Bedt = new DoctorVM();

            var DbData = db.DOCTORs.Where(x => x.ID == Cid).FirstOrDefault();





            DoctorVM Wareh = new DoctorVM()
            {

                ID = DbData.ID,
                D_CODE = DbData.D_CODE,
                FULL_NAME = DbData.FULL_NAME,
                GENDER = DbData.GENDER,

                DOB = DbData.DOB,
                MEDICAL_LICENSE_NO = DbData.MEDICAL_LICENSE_NO,
                CONTACT_NO = DbData.CONTACT_NO,
                EMAIL = DbData.EMAIL,
                ADDRESS = DbData.ADDRESS,
                PROFILE_IMAGE = DbData.PROFILE_IMAGE,
                SPECIALIZATION = DbData.SPECIALIZATION,
                REGISTERED_HOSPITAL = DbData.REGISTERED_HOSPITAL,
                JOINED_DATE = DbData.JOINED_DATE





            };
            Bedt.Equals(Wareh);


            return View(Wareh);
        }

        // POST: Caregiver/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, DoctorVM stc)
        {
            try
            {
                string username = Session["User_Name"].ToString();
                var haverecord = db.DOCTORs.Where(x => x.MEDICAL_LICENSE_NO == stc.MEDICAL_LICENSE_NO && x.ID != id).FirstOrDefault();
                if (haverecord != null)
                {
                    TempData["Alert1"] = "Doctor Licence Cannot Be Dupplicated";
                    return RedirectToAction("CEdit", new { Cid = id });
                }

                if (stc.Img_File_F != null && stc.Img_File_F.ContentLength >= 0)
                {
                    try
                    {
                        string fileanme = Path.GetFileNameWithoutExtension(stc.Img_File_F.FileName);
                        string extension = Path.GetExtension(stc.Img_File_F.FileName);
                        fileanme = fileanme + DateTime.Now.ToString("yymmssfff") + extension;
                        stc.PROFILE_IMAGE = "~/Images/" + fileanme;
                        fileanme = Path.Combine(Server.MapPath("~/Images/"), fileanme);
                        stc.Img_File_F.SaveAs(fileanme);



                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        // Handle the exception appropriately (log, display an error message, etc.)
                    }





                    DOCTOR st = db.DOCTORs.Where(x => x.ID == id).FirstOrDefault();
                    {
                        st.FULL_NAME = stc.FULL_NAME;
                        st.GENDER = stc.GENDER;
                        st.DOB = stc.DOB;
                        st.MEDICAL_LICENSE_NO = stc.MEDICAL_LICENSE_NO;
                        st.CONTACT_NO = stc.CONTACT_NO;
                        st.EMAIL = stc.EMAIL;
                        st.ADDRESS = stc.ADDRESS;
                        st.JOINED_DATE = stc.JOINED_DATE;

                        st.PROFILE_IMAGE = stc.PROFILE_IMAGE;
                        st.SPECIALIZATION = stc.SPECIALIZATION;
                        st.REGISTERED_HOSPITAL = stc.REGISTERED_HOSPITAL;
                        st.UPDATE_DATE = System.DateTime.Now;
                        st.UPDATED_BY = username;



                        db.Entry(st).State = EntityState.Modified;
                        db.SaveChanges();

                    }


                    APP_USER st1 = db.APP_USER.Where(x => x.REF_ID == id && x.ROLE == "DOCTOR").FirstOrDefault();
                    {

                        if (st1 != null)
                        {
                            st1.FULL_NAME = stc.FULL_NAME;
                            st1.EMAIL = stc.EMAIL;
                            st1.PIC = stc.PROFILE_IMAGE;




                            db.Entry(st1).State = EntityState.Modified;
                            db.SaveChanges();
                        }



                    }
                }
                else
                {
                    DOCTOR st = db.DOCTORs.Where(x => x.ID == id).FirstOrDefault();
                    {
                        st.FULL_NAME = stc.FULL_NAME;
                        st.GENDER = stc.GENDER;
                        st.DOB = stc.DOB;
                        st.MEDICAL_LICENSE_NO = stc.MEDICAL_LICENSE_NO;
                        st.CONTACT_NO = stc.CONTACT_NO;
                        st.EMAIL = stc.EMAIL;
                        st.ADDRESS = stc.ADDRESS;
                        st.JOINED_DATE = stc.JOINED_DATE;

                       
                        st.SPECIALIZATION = stc.SPECIALIZATION;
                        st.REGISTERED_HOSPITAL = stc.REGISTERED_HOSPITAL;
                        st.UPDATE_DATE = System.DateTime.Now;
                        st.UPDATED_BY = username;



                        db.Entry(st).State = EntityState.Modified;
                        db.SaveChanges();

                    }


                    APP_USER st1 = db.APP_USER.Where(x => x.REF_ID == id && x.ROLE == "DOCTOR").FirstOrDefault();
                    {

                        if (st1 != null)
                        {
                            st1.FULL_NAME = stc.FULL_NAME;
                            st1.EMAIL = stc.EMAIL;
                        




                            db.Entry(st1).State = EntityState.Modified;
                            db.SaveChanges();
                        }



                    }
                }



                TempData["Alert"] = "Doctor Updated Successfully....!";

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

            var caregiver = db.DOCTORs.FirstOrDefault(x => x.ID == id);
            if (caregiver != null)
            {
                caregiver.STATUS = 1;
                caregiver.UPDATE_DATE = DateTime.Now;
                caregiver.UPDATED_BY = username;
                db.Entry(caregiver).State = EntityState.Modified;
            }

            var user = db.APP_USER.FirstOrDefault(x => x.REF_ID == id && x.ROLE == "DOCTOR");
            if (user != null)
            {
                user.STATUS = 9;
                db.Entry(user).State = EntityState.Modified;
            }

            db.SaveChanges();


            TempData["Alert"] = "Doctor Deleted Successfully....!";
            return RedirectToAction("Index");
        }
    }
}
