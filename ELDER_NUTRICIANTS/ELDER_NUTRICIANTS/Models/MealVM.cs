using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELDER_NUTRICIANTS.Models
{
    public class MealVM
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public Nullable<int> CATEGORY_ID { get; set; }

        public string PICTURE { get; set; }
        public string FOOD_GROUP { get; set; }




        public Nullable<int> GROCERY_ITEMS_ID { get; set; }
        public Nullable<decimal> CALORIES_kcal { get; set; }
        public Nullable<decimal> PROTEIN { get; set; }
        public Nullable<decimal> CARBS { get; set; }
        public Nullable<decimal> SUGAR { get; set; }
        public Nullable<decimal> FIBER { get; set; }
        public Nullable<decimal> FAT_TOTAL { get; set; }
        public Nullable<decimal> FAT_SATURED { get; set; }
        public Nullable<decimal> CHOLESTROL { get; set; }
        public Nullable<decimal> SODIUM { get; set; }
        public Nullable<decimal> POTESSIUM { get; set; }
        public Nullable<decimal> IRON { get; set; }
        public Nullable<decimal> VITAMIN_A { get; set; }
        public Nullable<decimal> VITAMIN_C { get; set; }
        public HttpPostedFileBase Img_File_F { get; set; }
    }
}