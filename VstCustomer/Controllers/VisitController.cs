using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VstCustomer.Controllers
{
    public class VisitController : Controller
    {
        //
        // GET: /Visit/

        public ActionResult Index()
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Home");
            STD_CODE code = new STD_CODE();
            CUSTOMER cus = new CUSTOMER();
            EMPLOYEE em = new EMPLOYEE();

            ViewBag.Emp = em.SelectSimple();
            ViewBag.Customer = cus.GetSimple();
            ViewBag.Code = code.Select();
            return View();
        }

        [HttpPost]
        public JsonResult GetVisit(DataTableParameters dataTableParameters, string from, string to, string cus_id, string emp_id)
        {
            VIST_CONTACTOR vit = new VIST_CONTACTOR();
          
            var resultSet = new DataTableResultSet();
            resultSet.draw = dataTableParameters.Draw;
            var lst = vit.GetVisit(from,to,cus_id,emp_id, dataTableParameters.Start + 1,
                dataTableParameters.Start + dataTableParameters.Length + 1);
            resultSet.recordsTotal = resultSet.recordsFiltered = vit.GetCountVisit(from,to,cus_id,emp_id);

            foreach (var i in lst)
            {
                var columns = new List<string>();
                columns.Add("<input type='checkbox' class='ckb' id='" + i.ID + "' data-emp='" + i.EMP_ID + "' />");
                columns.Add(i.CONTACT_DATE.ToShortDateString());
                columns.Add(i.CUSTOMER_ID.ToString());
                columns.Add(i.CUS_NAME.ToString());
                columns.Add(i.CUST_VIST_TYPE == null ? "" : i.CUST_VIST_TYPE);
                columns.Add(i.CUST_VIST_PURPOSE == null ? "" : i.CUST_VIST_PURPOSE);
                columns.Add(i.VIST_REMARK == null ? "" : i.VIST_REMARK);
                columns.Add(i.EMP_NAME==null? "":i.EMP_NAME);

                resultSet.data.Add(columns);

            }
            return Json(resultSet);
        }

        [HttpPost]
        public JsonResult GetVisitById(string ID)
        {
            VIST_CONTACTOR vit = new VIST_CONTACTOR();
            var result = vit.GetVisit(ID);
            return Json(result);
        }
    }
}
