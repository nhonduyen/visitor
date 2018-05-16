using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VstCustomer.Controllers
{
    public class EmployeeController : Controller
    {
        //
        // GET: /Employee/

        public ActionResult Index()
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Home");
            CUSTOMER cus = new CUSTOMER();
            EMPLOYEE em = new EMPLOYEE();
            ViewBag.EMPS = em.GetTeam();
            ViewBag.CUSTOMERS = cus.GetSimple();
            return View();
        }

        [HttpPost]
        public JsonResult GetEmployee(DataTableParameters dataTableParameters)
        {
            EMPLOYEE cus = new EMPLOYEE();

            var resultSet = new DataTableResultSet();
            resultSet.draw = dataTableParameters.Draw;
            var lst = cus.SelectPaging(dataTableParameters.Start + 1,
                dataTableParameters.Start + dataTableParameters.Length + 1, dataTableParameters.Search.Value);
            resultSet.recordsTotal = resultSet.recordsFiltered = cus.GetCount(dataTableParameters.Search.Value);

            foreach (var i in lst)
            {
                var columns = new List<string>();
                columns.Add("<input type='checkbox' class='ckb' id='" + i.EMP_ID.Trim() + "' />");
                columns.Add(i.EMP_NAME.Trim());
                columns.Add(i.EMP_ID.Trim());
                columns.Add(i.C_ID == null ? "" : i.C_ID.Trim());
                columns.Add(i.CUST_NAME == null ? "" : i.CUST_NAME.Trim());
                columns.Add(i.EMP_MOBILE == null ? "" : i.EMP_MOBILE.Trim());
                columns.Add(i.EMP_EMAIL == null ? "" : i.EMP_EMAIL.Trim());
                resultSet.data.Add(columns);

            }
            return Json(resultSet);

        }
        [HttpPost]
        public JsonResult InsertUpdateEmployee(EMPLOYEE EMP, int ACTION)
        {
            EMPLOYEE em = new EMPLOYEE();
            EMP_CUST ec = new EMP_CUST();

            var result = 0;
            if (ACTION == 1)
            {
                result = em.Update(EMP.EMP_ID, EMP.EMP_NAME, EMP.EMP_DEPT, EMP.EMP_EMAIL, EMP.EMP_MOBILE, EMP.ROLE);

            }
            else
            {
                EMPLOYEE checkExist = em.Select(EMP.EMP_ID).FirstOrDefault();
                if (checkExist != null)
                    return Json(-1);
                result = em.Insert(EMP.EMP_ID, EMP.EMP_NAME, EMP.EMP_DEPT, EMP.EMP_EMAIL, EMP.EMP_MOBILE, "123456", EMP.ROLE);

            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetCustomerById(string ID)
        {
            EMPLOYEE em = new EMPLOYEE();
            List<CUSTOMER> lst = em.GetCustomerByEmp(ID);
            return Json(lst);
        }

        [HttpPost]
        public JsonResult GetEmployeeById(string ID)
        {
            EMPLOYEE em = new EMPLOYEE();
            List<EMPLOYEE> result = em.Select(ID);
            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(string ID)
        {
            EMPLOYEE emp = new EMPLOYEE();
            EMP_CUST ec = new EMP_CUST();
            ec.Delete(ID);
            var result = emp.Delete(ID);
            return Json(result);
        }

        [HttpPost]
        public JsonResult InsertEmpCus(string ID, string CUS_ID, string NAME = "")
        {
            CUSTOMER cus = new CUSTOMER();

            var result = cus.Update(CUS_ID, ID);
            return Json(result);
        }
        [HttpPost]
        public JsonResult DeleteCustomer(string ID, List<string> CUS_ID)
        {
            var result = 0;
            CUSTOMER cus = new CUSTOMER();
            foreach (var item in CUS_ID)
            {
                result = cus.Update(item, "");

            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult ResetPassword(string ID)
        {
            EMPLOYEE em = new EMPLOYEE();
            var result = em.ResetPassword(ID);
            return Json(result);
        }
    }

}
