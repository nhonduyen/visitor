using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using OfficeOpenXml;
using System.IO;

namespace VstCustomer.Controllers
{
    public class HomeController : Controller
    {
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

        public ActionResult ChangePassword()
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Home");
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string EMP_ID, string Password, string NewPassword, string PwConfirm)
        {
            if (!NewPassword.Equals(PwConfirm))
            {
                return RedirectToAction("ChangePassword", new { success = -1, message = "Password does not match" });
            }
            EMPLOYEE em = new EMPLOYEE();
            Password = em.Encode(Password);
            NewPassword = em.Encode(NewPassword);
            var result = em.ChangePassword(EMP_ID, Password, NewPassword);
            var message = result > 0 ? "Success" : "Fail";
            return RedirectToAction("ChangePassword", new { success = result, message = message });
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signin(string username, string password)
        {
            EMPLOYEE em = new EMPLOYEE();
            password = em.Encode(password);
            bool login = em.Login(username, password);
            if (login)
                return RedirectToAction("Index");
            TempData["alert"] = "Login fail. Please check you ID and Password.";
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login","Home");
        }

        [HttpPost]
        public JsonResult GetVisit(DataTableParameters dataTableParameters, string from)
        {
            VIST_CONTACTOR vit = new VIST_CONTACTOR();
            EMPLOYEE em = new EMPLOYEE();

            var resultSet = new DataTableResultSet();
            resultSet.draw = dataTableParameters.Draw;
            var lst = vit.SelectPaging(from, dataTableParameters.Start + 1,
                dataTableParameters.Start + dataTableParameters.Length + 1);
            resultSet.recordsTotal = resultSet.recordsFiltered = em.GetCount();

            foreach (var i in lst)
            {
                var columns = new List<string>();
                var tar = i.tar == null ? 0 : i.tar;
                var Ratio = tar == 0 ? 0 : Math.Round(Convert.ToDouble(i.result) / i.tar * 100, 2, MidpointRounding.ToEven).ToString();
                columns.Add("<a href='#' class='emp' data-emid='" + i.EMP_ID + "'>"+i.EMP_ID+"</a>");
                columns.Add(i.EMP_NAME.Trim());
                columns.Add(tar.ToString());
                columns.Add(i.result.ToString());
                columns.Add(i.dir.ToString());
                columns.Add(i.ca.ToString());
                columns.Add(i.email.ToString());
                columns.Add(Ratio.ToString());
                columns.Add(i.EMP_DEPT==null ? "" : i.EMP_DEPT.Trim());
                resultSet.data.Add(columns);

            }
            return Json(resultSet);

        }

        [HttpPost]
        public JsonResult InsertUpdateTarget(EMP_VISIT TARGET, int ROLE)
        {

            var result = 0;
            if (ROLE > 0)
            {
                if (!string.IsNullOrWhiteSpace(TARGET.VISIT_TARGET.ToString()))
                {
                    var checkExist = TARGET.CheckExist(TARGET.EMP_ID, TARGET.VISIT_PLAN_MONTH.ToString());
                    if (checkExist)
                    {
                        result = TARGET.Insert(TARGET.EMP_ID, TARGET.VISIT_PLAN_MONTH, TARGET.VISIT_TARGET, 0);
                    }
                    else
                    {
                        result = TARGET.Update(TARGET.EMP_ID, TARGET.VISIT_PLAN_MONTH, TARGET.VISIT_TARGET);
                    }
                }
            }
          
            return Json(result);
        }

        [HttpPost]
        public JsonResult InsertUpdateVisit( VIST_CONTACTOR VISIT, int ROLE)
        {

            var result = 0;
            VISIT.CUSTOMER_ID = VISIT.CUSTOMER_ID.ToUpper();
            if (!string.IsNullOrWhiteSpace(VISIT.CUSTOMER_ID))
            {
                if (string.IsNullOrEmpty(VISIT.CUST_CONTACTOR))
                    return Json(-1);
                if (string.IsNullOrWhiteSpace(VISIT.ID))
                {
                    result = VISIT.Insert(VISIT.EMP_ID, VISIT.CUSTOMER_ID, VISIT.CUST_CONTACTOR, VISIT.CONTACT_DATE, VISIT.CUST_VIST_TYPE, VISIT.CUST_VIST_PURPOSE, VISIT.VIST_REMARK);
                }
                else
                {
                    result = VISIT.Update(VISIT.EMP_ID, VISIT.CUSTOMER_ID, VISIT.CUST_CONTACTOR, VISIT.CONTACT_DATE, VISIT.CUST_VIST_TYPE, VISIT.CUST_VIST_PURPOSE, VISIT.VIST_REMARK,VISIT.ID);
                }
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetTarget(string ID, string DATE)
        {
            EMP_VISIT ev = new EMP_VISIT();
            var result = ev.GetTarget(ID, DATE);
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetHistory(string EMP_ID, string FROM, string TO)
        {
            VIST_CONTACTOR vc = new VIST_CONTACTOR();
            var lst = vc.Select(EMP_ID, FROM, TO);
            return Json(lst);
        }

        [HttpPost]
        public JsonResult GetContact(string CUS_ID)
        {
            CUSTOMER cus = new CUSTOMER();
            CONTACT vc = new CONTACT();
            var lst = vc.SelectCusContact(CUS_ID);
            cus = cus.Select(CUS_ID).FirstOrDefault();
            var result = new { CONTACTS= lst, CUSTOMER=cus };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(string ID)
        {
            VIST_CONTACTOR vc = new VIST_CONTACTOR();
            var result = vc.Delete(ID);
            return Json(result);
        }

        public ActionResult Export(string month)
        {
            VIST_CONTACTOR vit = new VIST_CONTACTOR();
            EMPLOYEE em = new EMPLOYEE();
            var lst = vit.SelectPaging(month, 0,1000);

            DataTable dtb = new DataTable();

            dtb.Clear();

            dtb.Columns.Add("EMP_ID");
            dtb.Columns.Add("NAME");
            dtb.Columns.Add("TARGET");
            dtb.Columns.Add("VISITED");
            dtb.Columns.Add("DIRECT");
            dtb.Columns.Add("CALL");
            dtb.Columns.Add("EMAIL");

            dtb.Columns.Add("RATIO");
            dtb.Columns.Add("TEAM");

            foreach (var i in lst)
            {
                DataRow r=dtb.NewRow();
                var columns = new List<string>();
                var tar = i.tar == null ? 0 : i.tar;
                var Ratio = tar == 0 ? 0 : (Convert.ToDouble(i.result) / i.tar) * 100;

                r["EMP_ID"] = i.EMP_ID;
                r["NAME"] = i.EMP_NAME;
                r["TARGET"] = i.tar;
                r["VISITED"] = i.result;
                r["DIRECT"] = i.dir;
                r["CALL"] = i.ca;
                r["EMAIL"] = i.email;
                r["RATIO"] = Ratio;
                r["TEAM"] = i.EMP_DEPT;
                dtb.Rows.Add(r);
            }
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            // Gọi lại hàm để tạo file excel
            var stream = CreateExcelFile(dtb);
            // Tạo buffer memory strean để hứng file excel
            var buffer = stream as MemoryStream;
            // Đây là content Type dành cho file excel, còn rất nhiều content-type khác nhưng cái này mình thấy okay nhất
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            // Dòng này rất quan trọng, vì chạy trên firefox hay IE thì dòng này sẽ hiện Save As dialog cho người dùng chọn thư mục để lưu
            // File name của Excel này là ExcelDemo
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".xlsx");
            // Lưu file excel của chúng ta như 1 mảng byte để trả về response
            Response.BinaryWrite(buffer.ToArray());
            // Send tất cả ouput bytes về phía clients
            Response.Flush();
            Response.End();
            return RedirectToAction("Home");
        }
        private Stream CreateExcelFile(DataTable dtb, Stream stream = null)
        {
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                // Tạo author cho file Excel
                excelPackage.Workbook.Properties.Author = "Export";
                // Tạo title cho file Excel
                excelPackage.Workbook.Properties.Title = "Export";
                // Add Sheet vào file Excel
                excelPackage.Workbook.Worksheets.Add("First Sheet");
                // Lấy Sheet bạn vừa mới tạo ra để thao tác 
                var workSheet = excelPackage.Workbook.Worksheets[1];
                // Đổ data vào Excel file
                workSheet.Cells[1, 1].LoadFromDataTable(dtb, true);
                // BindingFormatForExcel(workSheet, list);
                excelPackage.Save();
                return excelPackage.Stream;
            }
        }
    }
}
