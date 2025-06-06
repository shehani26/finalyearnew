using ELDER_NUTRICIANTS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ELDER_NUTRICIANTS.Controllers
{
    public class FoodController : Controller
    {
        ELDER_NUTRICIANTSEntities db = new ELDER_NUTRICIANTSEntities();
        // GET: Food
        public ActionResult Index()
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            var q = db.GROCERY_ITEM_CATEGORY.Where(x=>x.STATUS == 0).ToList();
            return View(q);
        }


        // GET: Food/Create
        public ActionResult Create()
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            ViewBag.CatType = new SelectList(db.MDCODEs.Where(x => x.CODE == 4).ToList(), "NAME", "NAME");

            return View();
        }

        // POST: Food/Create
        [HttpPost]
        public ActionResult Create(GROCERY_ITEM_CATEGORY stc)
        {
            try
            {

                try
                {

                    string username = Session["User_Name"].ToString();

                    var haverecord = db.GROCERY_ITEM_CATEGORY.Where(x => x.CATEGORY_NAME == stc.CATEGORY_NAME).FirstOrDefault();

                    if (haverecord != null)
                    {
                        TempData["Alert1"] = "Food Category Cannot Be Dupplicated";
                        return RedirectToAction("Create");
                    }

                   



                    GROCERY_ITEM_CATEGORY st = new GROCERY_ITEM_CATEGORY
                    {
                        CATEGORY_NAME = stc.CATEGORY_NAME,
                        
                        STATUS = 0,
                        DATE = System.DateTime.Now,
                        TYPE = stc.TYPE
                    };

                    db.GROCERY_ITEM_CATEGORY.Add(st);
                    db.SaveChanges();



                    TempData["Alert"] = "Food Category Added Successfully";
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

        // GET: Food/Edit/5
        public ActionResult CEdit(int Cid)
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }
            ViewBag.CatTypeSS = new SelectList(db.MDCODEs.Where(x => x.CODE == 4).ToList(), "NAME", "NAME");
            var q = db.GROCERY_ITEM_CATEGORY.Where(x => x.ID == Cid).FirstOrDefault();

            return View(q);
        }

        // POST: Food/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, GROCERY_ITEM_CATEGORY stc)
        {
            try
            {
                var haverecord = db.GROCERY_ITEM_CATEGORY.Where(x => x.CATEGORY_NAME == stc.CATEGORY_NAME && x.ID != id).FirstOrDefault();

                if (haverecord != null)
                {
                    TempData["Alert1"] = "Food Category Cannot Be Dupplicated";
                    return RedirectToAction("CEdit", new { Cid = id });
                }

               
                    GROCERY_ITEM_CATEGORY st = db.GROCERY_ITEM_CATEGORY.Where(x => x.ID == id).FirstOrDefault();
                    {
                        st.CATEGORY_NAME = stc.CATEGORY_NAME;
                    st.TYPE = stc.TYPE;


                        db.Entry(st).State = EntityState.Modified;
                        db.SaveChanges();

                    }

                  



                  
                TempData["Alert"] = "Food Category Updated Successfully....!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Alert1"] = "Error" + " " + ex;
                return RedirectToAction("CEdit", new { Cid = id });
            }
        }

        // GET: Food/Delete/5
        public ActionResult Delete(int id)
        {
            GROCERY_ITEM_CATEGORY st = db.GROCERY_ITEM_CATEGORY.Where(x => x.ID == id).FirstOrDefault();
            {
                st.STATUS =9;



                db.Entry(st).State = EntityState.Modified;
                db.SaveChanges();

            }






            TempData["Alert"] = "Food Category Deleted Successfully....!";

            return RedirectToAction("Index");
        }


        public ActionResult MIndex()
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            var q = db.V_MEALS.Where(x=>x.STATUS == 0).ToList();

            return View(q);
        }


        // GET: Food/Create
        public ActionResult MCreate()
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            


            ViewBag.Cat = new SelectList(db.GROCERY_ITEM_CATEGORY.Where(x=>x.STATUS == 0 && x.TYPE == "FOODS").ToList(), "ID", "CATEGORY_NAME");

            return View();
        }

        [HttpPost]
        public ActionResult MCreate(MealVM stc, HttpPostedFileBase Img_File_F)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var haverecord = db.GROCERY_ITEMS.Where(x => x.NAME == stc.NAME).FirstOrDefault();

                    if (haverecord != null)
                    {
                        return Json(new { success = false, message = "Grocery Items Cannot be dupplicated" });
                    }


                    string username = Session["User_Name"].ToString();
                    // Handle file upload
                    if (stc.Img_File_F != null && stc.Img_File_F.ContentLength >= 0)
                    {
                        try
                        {
                            string fileanme = Path.GetFileNameWithoutExtension(stc.Img_File_F.FileName);
                            string extension = Path.GetExtension(stc.Img_File_F.FileName);
                            fileanme = fileanme + DateTime.Now.ToString("yymmssfff") + extension;
                            stc.PICTURE = "~/Images/" + fileanme;
                            fileanme = Path.Combine(Server.MapPath("~/Images/"), fileanme);
                            stc.Img_File_F.SaveAs(fileanme);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred: {ex.Message}");
                            // Handle the exception appropriately (log, display an error message, etc.)
                        }
                    }


                    var doclog = db.APP_USER.Where(x => x.USER_NAME == username && x.ROLE == "DOCTOR").FirstOrDefault();

                    int docid = 0;

                    if(doclog != null)
                    {
                        var docdetails = db.DOCTORs.Where(x => x.ID == doclog.REF_ID).FirstOrDefault();

                        if(docdetails != null)
                        {
                            docid = docdetails.ID;
                        }
                    }

                    // Map ViewModel to Entity Model if needed
                    GROCERY_ITEMS st = new GROCERY_ITEMS
                    {
                        NAME = stc.NAME,
                        CATEGORY_ID = stc.CATEGORY_ID,
                        BRAND = "",

                        BARCODE = "",
                        PICTURE = stc.PICTURE,

                        FOOD_GROUP = stc.FOOD_GROUP,
                        SOURCE = "",

                        DOC_ID = docid,
                       
                        STATUS = 0,
                        DATE = System.DateTime.Now,

                        CREATE_BY = username
                     
                    };

                    db.GROCERY_ITEMS.Add(st);
                    db.SaveChanges();


                    GROCERY_ITEM_NUTRICIANTS st2 = new GROCERY_ITEM_NUTRICIANTS
                    {
                        GROCERY_ITEMS_ID = st.ID,
                        CALORIES_kcal = stc.CALORIES_kcal,
                        PROTEIN = stc.PROTEIN,

                        CARBS = stc.CARBS,
                        SUGAR = stc.SUGAR,

                        FIBER = stc.FIBER,


                        FAT_TOTAL = stc.FAT_TOTAL,

                        FAT_SATURED = stc.FAT_SATURED,
                        CHOLESTROL = stc.CHOLESTROL,
                        SODIUM = stc.SODIUM,
                        POTESSIUM =stc.POTESSIUM,
                        IRON = stc.IRON,
                        VITAMIN_A = stc.VITAMIN_A,
                        VITAMIN_C = stc.VITAMIN_C

                    };

                    db.GROCERY_ITEM_NUTRICIANTS.Add(st2);
                    db.SaveChanges();

                    return Json(new { success = true, message = "Meal created successfully!" });
                }

                // If we got this far, something failed
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = string.Join(", ", errors) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }



        public ActionResult MCEdit(int Cid)
        {
            if (Session["User_Name"] == null)
            {
                return RedirectToAction("Error", "Home"); // Redirect to login page if not logged in
            }

            ViewBag.CatSS = new SelectList(db.GROCERY_ITEM_CATEGORY.Where(x => x.STATUS == 0 && x.TYPE == "FOODS").ToList(), "ID", "CATEGORY_NAME");


            MealVM Bedt = new MealVM();

            var DbData = db.GROCERY_ITEMS.Where(x => x.ID == Cid).FirstOrDefault();


            var dbdata2 = db.GROCERY_ITEM_NUTRICIANTS.Where(x => x.GROCERY_ITEMS_ID == DbData.ID).FirstOrDefault();


            MealVM Wareh = new MealVM()
            {

                ID = DbData.ID,
                NAME = DbData.NAME,
                CATEGORY_ID = DbData.CATEGORY_ID,
                PICTURE = DbData.PICTURE,

                FOOD_GROUP = DbData.FOOD_GROUP,

                GROCERY_ITEMS_ID = dbdata2.GROCERY_ITEMS_ID,
                CALORIES_kcal = dbdata2.CALORIES_kcal,
                PROTEIN = dbdata2.PROTEIN,
                CARBS = dbdata2.CARBS,
                SUGAR = dbdata2.SUGAR,
                FIBER = dbdata2.FIBER,
                FAT_TOTAL = dbdata2.FAT_TOTAL,
                FAT_SATURED = dbdata2.FAT_SATURED,
                CHOLESTROL = dbdata2.CHOLESTROL,
                SODIUM = dbdata2.SODIUM,
                POTESSIUM = dbdata2.POTESSIUM,
                IRON = dbdata2.IRON,
                VITAMIN_A = dbdata2.VITAMIN_A,
                VITAMIN_C = dbdata2.VITAMIN_C





            };
            Bedt.Equals(Wareh);


            return View(Wareh);
        }


        public ActionResult MDelete(int id)
        {
            GROCERY_ITEMS st = db.GROCERY_ITEMS.Where(x => x.ID == id).FirstOrDefault();
            {
                st.STATUS = 9;



                db.Entry(st).State = EntityState.Modified;
                db.SaveChanges();

            }






            TempData["Alert"] = "Meal Deleted Successfully....!";

            return RedirectToAction("MIndex");
        }
    }
}
