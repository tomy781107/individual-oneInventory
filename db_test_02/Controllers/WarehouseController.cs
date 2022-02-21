using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using PagedList;
using PagedList.Mvc;


namespace db_test_02.Controllers
{
    // CRUD --- Create/ Read / Update / Delete

    //******      /Warehouse/Home

    
    public class WarehouseController : Controller
    {
        public int page_size = 4;

        DH_warehouse_Entities db = new DH_warehouse_Entities();

        public ActionResult Read(int? page_active = 1, string itemid = "" )
        {

            string[] search_conditions_by_user = itemid.Split(' ').Distinct().ToArray();

            ViewBag.itemid = itemid; //search
            ViewBag.ActivePage = page_active;
            
            int totalPage = (search_conditions_by_user.Length + page_size - 1) / page_size;
         
            // calcualte the total_page OR try below  
            // var total_page = (int)Math.Ceiling((double)search_conditions_by_user.Length / page_size);


            DH_warehouse_Entities db = new DH_warehouse_Entities();
            List<DH_warehouse_table> items = db.DH_warehouse_table.ToList();

            List<DH_warehouse_table> result = new List<DH_warehouse_table>();
            

            // LINQ syntax
            //db.DH_warehouse_table.Where(x => x.ItemID == itemid).ToList();

            foreach (DH_warehouse_table item in items)
            {
                foreach (string search_condition_by_user in search_conditions_by_user)
                {
                    if (item.ItemID == search_condition_by_user)
                    {
                        result.Add(item);
                    }
                }
            }

            if (result.Count > 0)
            {
                //return View(result.ToPagedList(page_active ?? 1 , page_size));
                //return Json(new { view = View(result.ToPagedList(page_active ?? 1, page_size)), page_active = page_active, totalPage = totalPage }, JsonRequestBehavior.AllowGet);
                return Json(new { obj = result.ToPagedList(page_active ?? 1, page_size), page_active = page_active, totalPage = totalPage }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { not_found = true, responseText = "" }, JsonRequestBehavior.AllowGet);
            }



        }

      
        public ActionResult Home()
        {

            //List<DH_warehouse_table> Warehouse_Items = db.DH_warehouse_table.ToList();
            return View();
        }
        


        [HttpPost]
        public ActionResult Create(string itemID, string itemName, string where)
        {

            // Below is SQL sytanx , pros is good for complex SQL command, cons is SQL injection 
            /*
            string queryString = "select * from [dbo].[DH_warehouse_table] where ItemID = '109'";

            string connectionString = "Server=RODZORKDYV;Database=DH02;User Id=DH;Password=123456";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                //command.Parameters.AddWithValue("@tPatSName", "Your-Parm-Value");
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                
                if (reader.HasRows) {
                    reader.Close();
                    connection.Close();
                    return Content("itemID:{0} is already existed", itemID);
                }

                reader.Close();

                connection.Close();
            }
            */

            // Below is LINQ syntax
            try
            {
                using (DH_warehouse_Entities context = new DH_warehouse_Entities())
                {
                    //check item is already exist or not ?
                    List<DH_warehouse_table> obj = context.DH_warehouse_table.Where(x => x.ItemID == itemID).ToList();
                    if (obj.Count > 0)
                        return Content("Oppos... Item ID: " + itemID + " is already exist");
                    else
                    {
                        DH_warehouse_table model = new DH_warehouse_table();
                        model.ItemID = itemID;
                        model.ItemName = itemName;
                        model.ItemWhere = where;
                        context.DH_warehouse_table.Add(model);
                        context.SaveChanges();
                        return Content("Item created successfully");
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlException)
                {
                    //switch (sqlException.Number)
                    //{
                    // If the tag already exists

                }
                return Content("SQL error");
            }

        }

   


    }
}