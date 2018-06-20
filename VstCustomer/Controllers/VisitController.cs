using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Data;
using OfficeOpenXml.Style;

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

        public ActionResult Export(string from, string to, string cus_id, string emp)
        {
            
            VIST_CONTACTOR vc = new VIST_CONTACTOR();
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            var list = vc.Export(from, to, cus_id, emp);
           
            // Gọi lại hàm để tạo file excel
            var stream = CreateExcelFile(list);
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
            return RedirectToAction("Visit");
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
