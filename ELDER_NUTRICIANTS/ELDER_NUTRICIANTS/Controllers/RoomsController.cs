using ELDER_NUTRICIANTS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ELDER_NUTRICIANTS.Controllers
{
    public class RoomsController : Controller
    {
        ELDER_NUTRICIANTSEntities db = new ELDER_NUTRICIANTSEntities();
        // GET: Rooms
        public ActionResult Index()
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            var q = db.ROOMS.ToList();
            return View(q);
        }



        // GET: Caregiver/Create
        public ActionResult Create()
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            ViewBag.Type = new SelectList(db.MDCODEs.Where(x => x.CODE == 2).ToList(), "NAME", "NAME");

            return View();
        }

        // POST: Caregiver/Create
        [HttpPost]
        public ActionResult Create(ROOM stc)
        {
            try
            {

                try
                {

                    string username = Session["User_Name"].ToString();

                    var haverecord = db.ROOMS.Where(x => x.ROOM_NUMBER == stc.ROOM_NUMBER && x.STATUS == 0).FirstOrDefault();

                    if (haverecord != null)
                    {
                        TempData["Alert1"] = "Room No Cannot Be Dupplicated";
                        return RedirectToAction("Create");
                    }

                  

                 



                    ROOM st = new ROOM
                    {
                        FLOOR_NUMBER = stc.FLOOR_NUMBER,
                        ROOM_TYPE = stc.ROOM_TYPE,
                        CAPACITY = stc.CAPACITY,

                        DESCRIPTION = stc.DESCRIPTION,
                      

                        ROOM_NUMBER = stc.ROOM_NUMBER,
                      
                        STATUS = 0,
                        CREATE_DATE = System.DateTime.Now,

                      
                    };

                    db.ROOMS.Add(st);
                    db.SaveChanges();


                   

                    TempData["Alert"] = "Room Added Successfully";
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

    
        // GET: Caregiver/Edit/5
        public ActionResult CEdit(int Cid)
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            ViewBag.TypeSS = new SelectList(db.MDCODEs.Where(x => x.CODE == 2).ToList(), "NAME", "NAME");
            var q = db.ROOMS.Where(x => x.ID == Cid).FirstOrDefault();


            return View(q);
        }

        // POST: Caregiver/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, ROOM stc)
        {
            try
            {
                string username = Session["User_Name"].ToString();
                var haverecord = db.ROOMS.Where(x => x.ROOM_NUMBER == stc.ROOM_NUMBER && x.ID != id).FirstOrDefault();
                if (haverecord != null)
                {
                    TempData["Alert1"] = "Room Number Cannot Be Dupplicated";
                    return RedirectToAction("CEdit", new { Cid = id });
                }

              
                    ROOM st = db.ROOMS.Where(x => x.ID == id).FirstOrDefault();
                    {
                        st.ROOM_NUMBER = stc.ROOM_NUMBER;
                        st.FLOOR_NUMBER = stc.FLOOR_NUMBER;
                        st.ROOM_TYPE = stc.ROOM_TYPE;
                        st.CAPACITY = stc.CAPACITY;
                        st.DESCRIPTION = stc.DESCRIPTION;

                        



                        st.UPDATE_DATE = System.DateTime.Now;
                        



                        db.Entry(st).State = EntityState.Modified;
                        db.SaveChanges();

                    }

                   
              



                TempData["Alert"] = "Room Updated Successfully....!";

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



            var haveroomnu = db.ROOMS.Where(x => x.ID == id).FirstOrDefault();


            if(haveroomnu != null)
            {
                var haveRoomuse = db.ELDERs.Where(x => x.ROOM_NUMBER == haveroomnu.ROOM_NUMBER).FirstOrDefault();

                if(haveRoomuse != null)
                {

                    TempData["Alert1"] = "Cannot deleted this room because its used....!";


                    return RedirectToAction("Index");
                }
            }
            else
            {
                var caregiver = db.ROOMS.FirstOrDefault(x => x.ID == id);
                if (caregiver != null)
                {
                    caregiver.STATUS = 1;
                    caregiver.UPDATE_DATE = DateTime.Now;
                
                    db.Entry(caregiver).State = EntityState.Modified;
                }
            }

         


           

            db.SaveChanges();


            TempData["Alert"] = "Room Deleted Successfully....!";
            return RedirectToAction("Index");
        }
    }
}
