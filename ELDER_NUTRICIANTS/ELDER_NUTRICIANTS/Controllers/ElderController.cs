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
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ELDER_NUTRICIANTS.Controllers
{
    public class ElderController : Controller
    {
        ELDER_NUTRICIANTSEntities db = new ELDER_NUTRICIANTSEntities();
        // GET: Elder
        public ActionResult Index()
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            var q = db.V_ELDER.ToList();
            return View(q);
        }



        // GET: Caregiver/Create
        public ActionResult Create()
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            ViewBag.Relationship = new SelectList(db.MDCODEs.Where(x => x.CODE == 1).ToList(), "NAME", "NAME");

            return View();
        }

        // POST: Caregiver/Create
        [HttpPost]
        public ActionResult Create(ElderVM stc)
        {
            try
            {

                try
                {

                    string username = Session["User_Name"].ToString();

                    var haverecord = db.ELDERs.Where(x => x.NIC == stc.NIC && x.STATUS == 0).FirstOrDefault();

                    if (haverecord != null)
                    {
                        TempData["Alert1"] = "Elder NIC Cannot Be Dupplicated";
                        return RedirectToAction("Create");
                    }

                    if (stc.Img_File_F != null && stc.Img_File_F.ContentLength >= 0)
                    {
                        try
                        {
                            string fileanme = Path.GetFileNameWithoutExtension(stc.Img_File_F.FileName);
                            string extension = Path.GetExtension(stc.Img_File_F.FileName);
                            fileanme = fileanme + DateTime.Now.ToString("yymmssfff") + extension;
                            stc.PROFIL_PIC = "~/Images/" + fileanme;
                            fileanme = Path.Combine(Server.MapPath("~/Images/"), fileanme);
                            stc.Img_File_F.SaveAs(fileanme);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred: {ex.Message}");
                            // Handle the exception appropriately (log, display an error message, etc.)
                        }
                    }

                    string password = GeneratePassword();

                    string EncyPassword = HashPassword(password);



                    ELDER st = new ELDER
                    {
                        ELDER_CODE = db.PROC_INCREMENT_ID("E").FirstOrDefault(),
                        FULL_NAME = stc.FULL_NAME,
                        GENDER = stc.GENDER,

                        DOB = stc.DOB,
                        NIC = stc.NIC,

                        CONTACT_NO = stc.CONTACT_NO,
                        IS_DEAD =0,

                        ADDRESS = stc.ADDRESS,
                        PROFIL_PIC = stc.PROFIL_PIC,

                        ROOM_NUMBER = "",
                        CURRENT_SUGAR_LEVEL =0,
                        CURRENT_SUGAR_LEVEL_ENTER_DATE = null,
                        STATUS = 0,
                        CREATE_DATE = System.DateTime.Now,

                        CREATED_BY = username,
                        IS_DIABITCS = 0
                    };

                    db.ELDERs.Add(st);
                    db.SaveChanges();


                    ELDER_GURDIAN st2 = new ELDER_GURDIAN
                    {
                        ELDER_ID = st.ID,
                        FULL_NAME = stc.G_FULL_NAME,
                        E_GURDIAN_CODE = db.PROC_INCREMENT_ID("G").FirstOrDefault(),

                        RELATIONSHIP = stc.RELATIONSHIP,
                        EMAIL = stc.EMAIL,

                        CONTACT_NO = stc.G_CONTACT_NO,
                     

                        ADDRESS = stc.G_ADDRESS,
                      
                        STATUS = 0,
                      
                    };

                    db.ELDER_GURDIAN.Add(st2);
                    db.SaveChanges();


                    APP_USER st1 = new APP_USER
                    {
                        USER_NAME = st2.E_GURDIAN_CODE,
                        PASSWORD = EncyPassword,
                        FULL_NAME = stc.FULL_NAME,

                        ROLE = "GURDIAN",
                        LOGIN_ATTEMPT = 0,

                        ENTER_USER = username,
                        EMAIL = stc.EMAIL,

                        REF_ID = st.ID,
                        PIC = stc.PROFIL_PIC,



                        STATUS = 0,
                        ENTER_DATE = System.DateTime.Now


                    };

                    db.APP_USER.Add(st1);
                    db.SaveChanges();


                    db.PROC_INCREMENT_ID_UPADATE("G");
                    db.PROC_INCREMENT_ID_UPADATE("E");

                    var companyemailDetails = db.COMPANies.FirstOrDefault();

                    if (companyemailDetails != null)
                    {
                        string from = companyemailDetails.EMAIL;
                        string fromname = companyemailDetails.COMPANYNAME;
                        string from_password = companyemailDetails.EMAIL_PASSWORD;
                        string to = stc.EMAIL;
                        string Toname = st2.FULL_NAME;
                        string subject = "L'derly Nutri Created New Account Alert";
                        string body = "Your user name is: " + st1.USER_NAME + " Password is: " + password;

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

                    TempData["Alert"] = "Elder Added Successfully & Gurdian User Name is " + st1.USER_NAME;
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

        public static string GeneratePassword(int length = 10)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+";
            StringBuilder password = new StringBuilder();
            Random rnd = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = rnd.Next(validChars.Length);
                password.Append(validChars[index]);
            }

            return password.ToString();
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

            ViewBag.RelationshipSS = new SelectList(db.MDCODEs.Where(x => x.CODE == 1).ToList(), "NAME", "NAME");

            ElderVM Bedt = new ElderVM();

            var DbData = db.ELDERs.Where(x => x.ID == Cid).FirstOrDefault();


            var dbdata2 = db.ELDER_GURDIAN.Where(x => x.ELDER_ID == DbData.ID).FirstOrDefault();


            ElderVM Wareh = new ElderVM()
            {

                ID = DbData.ID,
            NIC = DbData.NIC,
                FULL_NAME = DbData.FULL_NAME,
                GENDER = DbData.GENDER,

                DOB = DbData.DOB,
               
                CONTACT_NO = DbData.CONTACT_NO,
                EMAIL = dbdata2.EMAIL,
                ADDRESS = DbData.ADDRESS,
                PROFIL_PIC = DbData.PROFIL_PIC,
                G_FULL_NAME = dbdata2.FULL_NAME,
                RELATIONSHIP = dbdata2.RELATIONSHIP,
                G_CONTACT_NO = dbdata2.CONTACT_NO,
                G_ADDRESS = dbdata2.ADDRESS





            };
            Bedt.Equals(Wareh);


            return View(Wareh);
        }

        // POST: Caregiver/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, ElderVM stc)
        {
            try
            {
                string username = Session["User_Name"].ToString();
                var haverecord = db.ELDERs.Where(x => x.NIC == stc.NIC && x.ID != id).FirstOrDefault();
                if (haverecord != null)
                {
                    TempData["Alert1"] = "Elder NIC Cannot Be Dupplicated";
                    return RedirectToAction("CEdit", new { Cid = id });
                }

                if (stc.Img_File_F != null && stc.Img_File_F.ContentLength >= 0)
                {
                    try
                    {
                        string fileanme = Path.GetFileNameWithoutExtension(stc.Img_File_F.FileName);
                        string extension = Path.GetExtension(stc.Img_File_F.FileName);
                        fileanme = fileanme + DateTime.Now.ToString("yymmssfff") + extension;
                        stc.PROFIL_PIC = "~/Images/" + fileanme;
                        fileanme = Path.Combine(Server.MapPath("~/Images/"), fileanme);
                        stc.Img_File_F.SaveAs(fileanme);



                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        // Handle the exception appropriately (log, display an error message, etc.)
                    }





                    ELDER st = db.ELDERs.Where(x => x.ID == id).FirstOrDefault();
                    {
                        st.FULL_NAME = stc.FULL_NAME;
                        st.GENDER = stc.GENDER;
                        st.DOB = stc.DOB;
                        st.NIC = stc.NIC;
                        st.CONTACT_NO = stc.CONTACT_NO;
                    
                        st.ADDRESS = stc.ADDRESS;
                    

                        st.PROFIL_PIC = stc.PROFIL_PIC;
                       
                        st.UPDATE_DATE = System.DateTime.Now;
                        st.UPDATED_BY = username;



                        db.Entry(st).State = EntityState.Modified;
                        db.SaveChanges();

                    }

                    ELDER_GURDIAN st1 = db.ELDER_GURDIAN.Where(x => x.ELDER_ID == id).FirstOrDefault();
                    {
                        st1.FULL_NAME = stc.G_FULL_NAME;
                        st1.RELATIONSHIP = stc.GENDER;
                        st1.CONTACT_NO = stc.G_CONTACT_NO;
                        st1.EMAIL = stc.EMAIL;
                        st1.CONTACT_NO = stc.CONTACT_NO;



                        db.Entry(st1).State = EntityState.Modified;
                        db.SaveChanges();

                    }



                    APP_USER st2 = db.APP_USER.Where(x => x.REF_ID == id && x.ROLE == "GURDIAN").FirstOrDefault();
                    {

                        if (st2 != null)
                        {
                            st2.FULL_NAME = stc.FULL_NAME;
                            st2.EMAIL = stc.EMAIL;
                            st2.PIC = stc.PROFIL_PIC;




                            db.Entry(st2).State = EntityState.Modified;
                            db.SaveChanges();
                        }



                    }
                }
                else
                {
                    ELDER st = db.ELDERs.Where(x => x.ID == id).FirstOrDefault();
                    {
                        st.FULL_NAME = stc.FULL_NAME;
                        st.GENDER = stc.GENDER;
                        st.DOB = stc.DOB;
                        st.NIC = stc.NIC;
                        st.CONTACT_NO = stc.CONTACT_NO;

                        st.ADDRESS = stc.ADDRESS;


                    

                        st.UPDATE_DATE = System.DateTime.Now;
                        st.UPDATED_BY = username;



                        db.Entry(st).State = EntityState.Modified;
                        db.SaveChanges();

                    }

                    ELDER_GURDIAN st1 = db.ELDER_GURDIAN.Where(x => x.ELDER_ID == id).FirstOrDefault();
                    {
                        st1.FULL_NAME = stc.G_FULL_NAME;
                        st1.RELATIONSHIP = stc.GENDER;
                        st1.CONTACT_NO = stc.G_CONTACT_NO;
                        st1.EMAIL = stc.EMAIL;
                        st1.CONTACT_NO = stc.CONTACT_NO;



                        db.Entry(st1).State = EntityState.Modified;
                        db.SaveChanges();

                    }



                    APP_USER st2 = db.APP_USER.Where(x => x.REF_ID == id && x.ROLE == "GURDIAN").FirstOrDefault();
                    {

                        if (st2 != null)
                        {
                            st2.FULL_NAME = stc.FULL_NAME;
                            st2.EMAIL = stc.EMAIL;
                   




                            db.Entry(st2).State = EntityState.Modified;
                            db.SaveChanges();
                        }



                    }
                }



                TempData["Alert"] = "Elder Updated Successfully....!";

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

            var caregiver = db.ELDERs.FirstOrDefault(x => x.ID == id);
            if (caregiver != null)
            {
                caregiver.STATUS = 1;
                caregiver.UPDATE_DATE = DateTime.Now;
                caregiver.UPDATED_BY = username;
                db.Entry(caregiver).State = EntityState.Modified;
            }


            var elderg = db.ELDER_GURDIAN.Where(x => x.ELDER_ID == id).FirstOrDefault();

            if(elderg != null)
            {
                elderg.STATUS = 1;
                db.Entry(elderg).State = EntityState.Modified;
            }


            var user = db.APP_USER.FirstOrDefault(x => x.REF_ID == elderg.ID && x.ROLE == "GURDIAN");
            if (user != null)
            {
                user.STATUS = 9;
                db.Entry(user).State = EntityState.Modified;
            }

            db.SaveChanges();


            TempData["Alert"] = "Elder Deleted Successfully....!";
            return RedirectToAction("Index");
        }


        public ActionResult Death(int Cid)
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            ViewBag.Elderid = Cid;

            return View();
        }

        // POST: Caregiver/Edit/5
        [HttpPost]
        public ActionResult DeathSave(int Eid, ELDER_DEATH stc)
        {
            try
            {
                string username = Session["User_Name"].ToString();



                ELDER_DEATH st = new ELDER_DEATH
                {
                    ELDER_ID = Eid,
                    DATE_OF_DEATH = stc.DATE_OF_DEATH,
                    TIME_OF_DEATH = stc.TIME_OF_DEATH,

                    CAUSE_OF_DEATH = stc.CAUSE_OF_DEATH,
                    PLACE_OF_DEATH = stc.PLACE_OF_DEATH,

                    REPORTED_BY = username,
                    REMARKS = stc.REMARKS,

                  
                    STATUS = 0,
                    ENTER_DATE = System.DateTime.Now

                  
                };

                db.ELDER_DEATH.Add(st);
                db.SaveChanges();


                ELDER st2 = db.ELDERs.Where(x => x.ID == Eid).FirstOrDefault();
                    {
                    st2.STATUS = 2;
                    st2.ROOM_NUMBER = "";
                    st2.IS_DEAD = 1;



                    st2.UPDATE_DATE = System.DateTime.Now;
                    st2.UPDATED_BY = username;



                        db.Entry(st2).State = EntityState.Modified;
                        db.SaveChanges();

                    }

                    CAREGIVER_ASSIGN_ELDER st1 = db.CAREGIVER_ASSIGN_ELDER.Where(x => x.ELDER_ID == Eid && x.STATUS==0).FirstOrDefault();
                    {
                        st1.STATUS = 1;
                    



                        db.Entry(st1).State = EntityState.Modified;
                        db.SaveChanges();

                    }



                    ROOM_ASSIGNED st3 = db.ROOM_ASSIGNED.Where(x => x.ELDER_ID == Eid && x.STATUS == 0).FirstOrDefault();
                    {

                        if (st3 != null)
                        {
                        st3.STATUS = 1;

                        st3.UPDATED_DATE = System.DateTime.Now;
                        st3.RELEASED_DATE = System.DateTime.Now;



                            db.Entry(st3).State = EntityState.Modified;
                            db.SaveChanges();




                        ROOM st4 = db.ROOMS.Where(x => x.ID == st3.ROOM_ID).FirstOrDefault();
                        {

                            if (st4 != null)
                            {
                                st4.STATUS = 0;

                                st4.UPDATE_DATE = System.DateTime.Now;




                                db.Entry(st4).State = EntityState.Modified;
                                db.SaveChanges();
                            }



                        }
                    }



                    }

                var gurdiandetails = db.ELDER_GURDIAN.Where(x => x.ELDER_ID == Eid).FirstOrDefault();

                var companyemailDetails = db.COMPANies.FirstOrDefault();

                if (companyemailDetails != null)
                {
                    string from = companyemailDetails.EMAIL;
                    string fromname = companyemailDetails.COMPANYNAME;
                    string from_password = companyemailDetails.EMAIL_PASSWORD;
                    string to = gurdiandetails.EMAIL;
                    string Toname = gurdiandetails.FULL_NAME;
                    string subject = "L'derly Nutri – Elder Death Notification";

                    string body = "Dear Guardian,\n\n" +
                                  "We are deeply saddened to inform you that your loved one, " + st2.FULL_NAME + ", " +
                                  "passed away on " + Convert.ToDateTime(st.DATE_OF_DEATH).ToString("dd MMMM yyyy") +
                                  " at " + Convert.ToDateTime(st.TIME_OF_DEATH).ToString(@"hh\:mm") + ".\n\n" +
                                  "Cause of Death: " + st.CAUSE_OF_DEATH + "\n" +
                                  "Place of Death: " + st.PLACE_OF_DEATH + "\n\n" +
                                  "Please accept our heartfelt condolences during this difficult time.\n\n" +
                                  "If you have any questions or need assistance, feel free to contact our support team.\n\n" +
                                  "Sincerely,\n" +
                                  "L'derly Nutri Management Team";


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



                TempData["Alert"] = "Elder Death Added Successfully....!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Alert1"] = "Error" + " " + ex;
                return RedirectToAction("Death", new { Cid = Eid });
            }
        }


        public ActionResult EIndex()
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            var q = db.V_ELDER.Where(x=>x.STATUS ==0).ToList();
            return View(q);
        }

        public ActionResult RNext(int Cid)
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            ViewBag.elderid = Cid;

            ReportVM data = new ReportVM();

            ViewBag.Relationship = new SelectList(db.MDCODEs.Where(x => x.CODE == 1).ToList(), "NAME", "NAME");

            ViewBag.BloodRPTType = new SelectList(db.MDCODEs.Where(x => x.CODE == 3).ToList(), "NAME", "NAME");


            data.ve = db.V_ELDER.Where(x => x.ID == Cid).FirstOrDefault();

            data.vebr = db.V_ELDER_BLOOD_RPT.Where(x => x.ELDER_ID == Cid).ToList();
           
            return View(data);
        }

        // POST: Caregiver/Create
        [HttpPost]
        public ActionResult RCreate(int elid,ReportVM stc)
        {
            try
            {

                try
                {

                    string username = Session["User_Name"].ToString();

                  

                    if (stc.Img_File_F != null && stc.Img_File_F.ContentLength >= 0)
                    {
                        try
                        {
                            string fileanme = Path.GetFileNameWithoutExtension(stc.Img_File_F.FileName);
                            string extension = Path.GetExtension(stc.Img_File_F.FileName);
                            fileanme = fileanme + DateTime.Now.ToString("yymmssfff") + extension;
                            stc.REPORT = "~/Images/" + fileanme;
                            fileanme = Path.Combine(Server.MapPath("~/Images/"), fileanme);
                            stc.Img_File_F.SaveAs(fileanme);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred: {ex.Message}");
                            // Handle the exception appropriately (log, display an error message, etc.)
                        }
                    }

                    int DID = 0;

                    var userrole = db.APP_USER.Where(x => x.USER_NAME == username && x.ROLE == "DOCTOR").FirstOrDefault();

                    if(userrole != null)
                    {
                        DID = (int)userrole.REF_ID;
                    }
                   


                    GLUCODE_READING st = new GLUCODE_READING
                    {
                        ELDER_ID = elid,
                        READING_DATE = System.DateTime.Now,
                        GLUCOSE_LEVEL = stc.GLUCOSE_LEVEL,

                        RECORDED_BY = username,
                        DOC_ID = DID,

                        SOURCE = stc.SOURCE,
                        DESCRIPTION = stc.DESCRIPTION,

                       
                        CREATE_DATE = System.DateTime.Now

                   
                    };

                    db.GLUCODE_READING.Add(st);
                  


                    GLUCOSE_READING_REPORT st1 = new GLUCOSE_READING_REPORT
                    {
                        ELDER_ID = elid,
                        GLUCODE_READING_ID = st.ID,
                        REPORT_UPLOAD_DATE = System.DateTime.Now,

                        REPORT = stc.REPORT,
                        STATUS = 0


                    };

                    db.GLUCOSE_READING_REPORT.Add(st1);
                    db.SaveChanges();


                    string havediabitics = GetDiabetesStatus((decimal)stc.GLUCOSE_LEVEL);

                    int diabitcs = 0;

                    if(havediabitics == "Normal")
                    {
                        diabitcs = 0;
                    }
                    else if(havediabitics == "Prediabetes")
                    {
                        diabitcs = 1;
                    }
                    else
                    {
                        diabitcs = 2;
                    }


                    ELDER st2 = db.ELDERs.Where(x => x.ID == elid).FirstOrDefault();
                    {
                        st2.CURRENT_SUGAR_LEVEL = stc.GLUCOSE_LEVEL;
                        st2.CURRENT_SUGAR_LEVEL_ENTER_DATE = System.DateTime.Now;
                        st2.IS_DIABITCS = diabitcs;
                       



                        db.Entry(st).State = EntityState.Modified;
                        db.SaveChanges();

                    }


                    TempData["Alert"] = "Elder Glucode Level Add Sucessfully";
                    return RedirectToAction("EIndex");
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
                    return RedirectToAction("RNext", new { Cid = elid });
                    //save = false;
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
                return RedirectToAction("RNext", new { Cid = elid });
                //save = false;
            }
        }


        public string GetDiabetesStatus(decimal glucoseLevel)
        {
            if (glucoseLevel < 70)
                return "Low Glucose (Hypoglycemia)";
            else if (glucoseLevel >= 70 && glucoseLevel <= 99)
                return "Normal";
            else if (glucoseLevel >= 100 && glucoseLevel <= 125)
                return "Prediabetes";
            else // glucoseLevel >= 126
                return "Diabetes";
        }

        public ActionResult CIndex()
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            var q = db.V_ELDER.Where(x => x.STATUS == 0).ToList();
            return View(q);
        }

        public ActionResult CNext(int Cid)
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            ViewBag.elderid = Cid;

            ReportVM data = new ReportVM();

            ViewBag.Relationship = new SelectList(db.MDCODEs.Where(x => x.CODE == 1).ToList(), "NAME", "NAME");

            ViewBag.BloodRPTType = new SelectList(db.MDCODEs.Where(x => x.CODE == 3).ToList(), "NAME", "NAME");


            data.ve = db.V_ELDER.Where(x => x.ID == Cid).FirstOrDefault();

            data.vebr = db.V_ELDER_BLOOD_RPT.Where(x => x.ELDER_ID == Cid).ToList();

            data.vac = db.V_ASSIGN_CAREGIVER.Where(x => x.ELDER_ID == Cid).ToList();

            data.ca = db.CAREGIVERs.Where(x=>x.STATUS == 0).ToList();

            return View(data);
        }

        [HttpGet]
        public ActionResult AssignCaregiver(int caregiverId, int elderId)
        {
            try
            {

                try
                {

                    string username = Session["User_Name"].ToString();


                    var alreadyadd = db.CAREGIVER_ASSIGN_ELDER.Where(x => x.CAREGIVER_ID == caregiverId && x.ELDER_ID == elderId && x.STATUS == 0).FirstOrDefault();

                    if(alreadyadd != null)
                    {
                        TempData["Alert1"] = "Already Add this caregiver";
                        return RedirectToAction("CNext", new { Cid = elderId });
                    }

                    CAREGIVER_ASSIGN_ELDER st2 = db.CAREGIVER_ASSIGN_ELDER.Where(x => x.ELDER_ID == elderId && x.STATUS == 0).FirstOrDefault();
                    {

                        if (st2 != null)
                        {
                            st2.STATUS = 1;
                       





                            db.Entry(st2).State = EntityState.Modified;
                            db.SaveChanges();
                        }



                    }





                    CAREGIVER_ASSIGN_ELDER st = new CAREGIVER_ASSIGN_ELDER
                    {
                        ELDER_ID = elderId,
                        CAREGIVER_ID = caregiverId,
                        ROLE = "",

                        ASSIGNED_BY = username,
                        STATUS = 0,

                       
                        DESCRIPTION = "",


                        CREATE_DATE = System.DateTime.Now


                    };

                    db.CAREGIVER_ASSIGN_ELDER.Add(st);
                    db.SaveChanges();


                   


                  



                    TempData["Alert"] = "Caregiver Assign Sucessfully";
                    return RedirectToAction("CNext", new { Cid = elderId });
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
                    return RedirectToAction("CNext", new { Cid = elderId });
                    //save = false;
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
                return RedirectToAction("CNext", new { Cid = elderId });
                //save = false;
            }
        }

        public ActionResult RoIndex()
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            var q = db.V_ELDER.Where(x => x.STATUS == 0).ToList();
            return View(q);
        }

        public ActionResult RoNext(int Cid)
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            ViewBag.elderid = Cid;

            ReportVM data = new ReportVM();

            ViewBag.Relationship = new SelectList(db.MDCODEs.Where(x => x.CODE == 1).ToList(), "NAME", "NAME");

            ViewBag.BloodRPTType = new SelectList(db.MDCODEs.Where(x => x.CODE == 3).ToList(), "NAME", "NAME");


            data.ve = db.V_ELDER.Where(x => x.ID == Cid).FirstOrDefault();

            data.vebr = db.V_ELDER_BLOOD_RPT.Where(x => x.ELDER_ID == Cid).ToList();

            data.vac = db.V_ASSIGN_CAREGIVER.Where(x => x.ELDER_ID == Cid).ToList();

            data.ca = db.CAREGIVERs.Where(x => x.STATUS == 0).ToList();

            data.vars = db.V_ASSIGN_ROOMS.Where(x => x.ELDER_ID == Cid).ToList();

            data.ro = db.ROOMS.Where(x => x.STATUS == 0).ToList();

            return View(data);
        }

        [HttpGet]
        public ActionResult AssignRoom(int Roomid, int elderId)
        {
            try
            {

                try
                {

                    string username = Session["User_Name"].ToString();


                    var alreadyadd = db.ROOM_ASSIGNED.Where(x => x.ROOM_ID == Roomid && x.ELDER_ID == elderId && x.STATUS == 0).FirstOrDefault();

                    if (alreadyadd != null)
                    {
                        TempData["Alert1"] = "Already this elder has room";
                        return RedirectToAction("RoNext", new { Cid = elderId });
                    }


                    var haveroomtype = db.ROOMS.Where(x => x.ROOM_TYPE == "SHARED" && x.ID == Roomid).FirstOrDefault();

                    if(haveroomtype != null)
                    {
                        int assignedrooms = db.ROOM_ASSIGNED.Where(x => x.ROOM_ID == Roomid && x.STATUS == 0).Count();


                        if(assignedrooms == haveroomtype.CAPACITY)
                        {
                            ROOM st3 = db.ROOMS.Where(x => x.ID == Roomid).FirstOrDefault();
                            {

                                if (st3 != null)
                                {
                                    st3.STATUS = 1;
                                    st3.UPDATE_DATE = System.DateTime.Now;





                                    db.Entry(st3).State = EntityState.Modified;
                                    db.SaveChanges();
                                }



                            }
                        }
                    }
                    else
                    {
                        ROOM st3 = db.ROOMS.Where(x => x.ID == Roomid).FirstOrDefault();
                        {

                            if (st3 != null)
                            {
                                st3.STATUS = 1;

                                st3.UPDATE_DATE = System.DateTime.Now;




                                db.Entry(st3).State = EntityState.Modified;
                                db.SaveChanges();
                            }



                        }
                    }

                  

                    ROOM_ASSIGNED st = new ROOM_ASSIGNED
                    {
                        ROOM_ID = Roomid,
                        ELDER_ID = elderId,
                        ASSIGNED_DATE = System.DateTime.Now,

                        ASSIGNED_BY = username,
                        STATUS = 0,


                        DESCRIPTION = "",


                        ENTED_DATE = System.DateTime.Now


                    };

                    db.ROOM_ASSIGNED.Add(st);
                    db.SaveChanges();



                    var haveroomno = db.ROOMS.Where(x => x.ID == Roomid).FirstOrDefault();

                    if (haveroomno != null)
                    {
                        ELDER st2 = db.ELDERs.Where(x => x.ID == elderId).FirstOrDefault();
                        {

                            if (st2 != null)
                            {
                                st2.ROOM_NUMBER = haveroomno.ROOM_NUMBER;






                                db.Entry(st2).State = EntityState.Modified;
                                db.SaveChanges();
                            }



                        }
                    }


                    TempData["Alert"] = "Room Assign Sucessfully";
                    return RedirectToAction("RoNext", new { Cid = elderId });
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
                    return RedirectToAction("RoNext", new { Cid = elderId });
                    //save = false;
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
                return RedirectToAction("RoNext", new { Cid = elderId });
                //save = false;
            }
        }


        [HttpPost]
        public JsonResult RemoveRoom(int id)
        {
            try
            {

                var haveroomno = db.ROOM_ASSIGNED.Where(x => x.ID == id).FirstOrDefault();

                if (haveroomno != null)
                {
                    ROOM_ASSIGNED st2 = db.ROOM_ASSIGNED.Where(x => x.ID == haveroomno.ID).FirstOrDefault();
                    {

                        if (st2 != null)
                        {
                            st2.STATUS = 1;
                            st2.RELEASED_DATE = System.DateTime.Now;
                            st2.UPDATED_DATE = System.DateTime.Now;




                            db.Entry(st2).State = EntityState.Modified;
                            db.SaveChanges();
                        }



                    }


                    ROOM st3 = db.ROOMS.Where(x => x.ID == haveroomno.ROOM_ID).FirstOrDefault();
                    {

                        if (st3 != null)
                        {
                            st3.STATUS = 0;






                            db.Entry(st3).State = EntityState.Modified;
                            db.SaveChanges();
                        }



                    }


                    ELDER st4 = db.ELDERs.Where(x => x.ID == haveroomno.ELDER_ID).FirstOrDefault();
                    {

                        if (st4 != null)
                        {
                            st4.ROOM_NUMBER = "";






                            db.Entry(st4).State = EntityState.Modified;
                            db.SaveChanges();
                        }



                    }

                    return Json(new { success = true, message = "Room removed successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Room not found." });
                }


               
               
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

    }
}
