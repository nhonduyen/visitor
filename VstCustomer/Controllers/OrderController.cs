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
    public class OrderController : Controller
    {
        //
        // GET: /Order/

        public ActionResult Index()
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Home");
            STD_CODE code = new STD_CODE();
            CUSTOMER cus = new CUSTOMER();
            EMPLOYEE em = new EMPLOYEE();

            ViewBag.Emp = em.SelectSimple();
            ViewBag.Customer = cus.GetSimple();
            ViewBag.EndUser = cus.GetEndUser();
            ViewBag.Code = code.Select();

            return View();
        }

        [HttpPost]
        public JsonResult GetOrder(DataTableParameters dataTableParameters, string month, string cust_id, string status)
        {
            ORDERED order = new ORDERED();

            var resultSet = new DataTableResultSet();
            resultSet.draw = dataTableParameters.Draw;
            var lst = order.SelectPaging(dataTableParameters.Start + 1,
                dataTableParameters.Start + dataTableParameters.Length + 1, month, cust_id, status);
            resultSet.recordsTotal = resultSet.recordsFiltered = order.GetCount(month, cust_id, status);
            CUSTOMER cus = new CUSTOMER();
            foreach (var i in lst)
            {
                END_USER end = cus.GetEndUserById(i.END_USER);
                var columns = new List<string>();
                columns.Add("<input type='checkbox' class='ckb' id='" + i.ID + "' data-cus='" + i.CUSTOMER_ID + "' data-emp='" + i.EMP_ID + "' />");
                columns.Add(i.ORDED_DATE.ToShortDateString());
                columns.Add(i.CUSTOMER_ID);
                columns.Add(i.NAME);
                columns.Add(end == null ? i.NAME.Trim() : end.NAME.Trim());
                columns.Add(i.ORDER_CR_HR);
                columns.Add(i.STS_ST_CLS);
                columns.Add(i.STS_ST_SER);
                columns.Add(i.SURFACE_CD);
                columns.Add(i.ORD_THK == null ? "" : i.ORD_THK.ToString());
                columns.Add(i.ORD_WTH == null ? "" : i.ORD_WTH.ToString());
                columns.Add(i.ORD_EDGE == null ? "" : i.ORD_EDGE.ToString());
                columns.Add(i.QUANTITY == null ? "" : i.QUANTITY.ToString());
                //columns.Add(i.ORD_WGT == null ? "" : i.ORD_WGT.ToString());
                columns.Add(i.BASE_PRICE == null ? "" : i.BASE_PRICE.ToString());
                columns.Add(i.EFFECT_PRICE == null ? "" : i.EFFECT_PRICE.ToString());
                columns.Add(i.BIDD_PRICE == null ? "" : i.BIDD_PRICE.ToString());
                columns.Add(i.CONTRACT_NO == null ? "" : i.CONTRACT_NO.ToString());
                columns.Add(i.ORD_USAGE);
                columns.Add(i.ORD_STAT);
                columns.Add(i.EMP_NAME.Trim());
                columns.Add(i.DELIVERY_TIME == null ? "" : i.DELIVERY_TIME.Trim());
                columns.Add(i.REMARK == null ? "" : i.REMARK.Trim());
                resultSet.data.Add(columns);

            }
            return Json(resultSet);

        }

        [HttpPost]
        public JsonResult InsertUpdateOrder(ORDERED order, int ACTION)
        {
            order.CUSTOMER_ID = order.CUSTOMER_ID.ToUpper();
            order.END_USER = order.END_USER.ToUpper();
            var result = 0;
            if (ACTION == 1)
            {
                //Update(string EMP_ID, string CLAIM_NO, DateTime CLAIM_DATE, string CUSTOMER_ID, string COIL_NO, decimal CLAIM_WGT, decimal NET_WGT, DateTime VISIT_DATE, string DEFECT_CD, string DEFECT_LINE, DateTime FINISH_DATE, decimal COMPENT, string REMARK, string STATUS, decimal COIL_THK, decimal COIL_WTH, string STS_ST_CLS, string SURFACE_CD, string GRADE)
                result = order.Update(order.ID,order.EMP_ID, order.ORDED_DATE, order.CUSTOMER_ID, order.ORDER_CR_HR, order.STS_ST_CLS, order.STS_ST_SER, order.SURFACE_CD,
                    order.ORD_THK, order.ORD_WTH, order.ORD_EDGE, order.ORD_WGT, order.BASE_PRICE, order.EFFECT_PRICE, order.BIDD_PRICE,
                    order.CONTRACT_NO, order.ORD_USAGE, order.ORD_STAT, order.END_USER, order.QUANTITY,order.DELIVERY_TIME,order.REMARK);

            }
            else
            {

                //Insert(string EMP_ID, string CLAIM_NO, DateTime CLAIM_DATE, string CUSTOMER_ID, string COIL_NO, decimal CLAIM_WGT, decimal NET_WGT, DateTime VISIT_DATE, string DEFECT_CD, string DEFECT_LINE, DateTime FINISH_DATE, decimal COMPENT, string REMARK, string STATUS, decimal COIL_THK, decimal COIL_WTH, string STS_ST_CLS, string SURFACE_CD, string GRADE)
                result = order.Insert(order.EMP_ID, order.ORDED_DATE, order.CUSTOMER_ID, order.ORDER_CR_HR, order.STS_ST_CLS, order.STS_ST_SER, order.SURFACE_CD,
                    order.ORD_THK, order.ORD_WTH, order.ORD_EDGE, order.ORD_WGT, order.BASE_PRICE, order.EFFECT_PRICE, order.BIDD_PRICE,
                    order.CONTRACT_NO, order.ORD_USAGE, order.ORD_STAT, order.END_USER, order.QUANTITY, order.DELIVERY_TIME, order.REMARK);
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetOrderById(string ID)
        {
            ORDERED order = new ORDERED();
           
            List<ORDERED> lst = order.Select(ID);
            return Json(lst);
        }

      
        [HttpPost]
        public JsonResult GetEndUserById(string ID)
        {
            CUSTOMER cus = new CUSTOMER();
         
            END_USER end = cus.GetEndUserById(ID);
            if (end == null)
            {
                end = new END_USER();
                cus = cus.Select(ID).FirstOrDefault();
                end.CUS_ID = ID;
                end.NAME = cus.NAME;
                end.END_USER_ID = ID;
            }
            
          
            return Json(end);
        }

        [HttpPost]
        public JsonResult GetEndUser(string CUS_ID)
        {
            CUSTOMER cus = new CUSTOMER();
          
            List<END_USER> end = cus.GetEndUser(CUS_ID);
            cus = cus.Select(CUS_ID).FirstOrDefault();
            if (end == null || end.Count == 0)
            {
                var endUser = new END_USER();
                endUser.END_USER_ID = CUS_ID;
                endUser.CUS_ID = CUS_ID;
                endUser.NAME = cus.NAME;
                end.Add(endUser);
               
            }
            var result =new { CUSTOMER=cus, END=end };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Delete(string ID)
        {
            ORDERED order = new ORDERED();
            var result = order.Delete(ID);
            return Json(result);
        }


        public ActionResult Export(string month = "", string cus_id = "", string status = "")
        {
            ORDERED order = new ORDERED();
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            var list = order.GetExport(month, cus_id, status);
            DataTable dtb = new DataTable();

            dtb.Clear();

            dtb.Columns.Add("Date");
            dtb.Columns.Add("CUS_CODE");
            dtb.Columns.Add("CUS_NAME");
            dtb.Columns.Add("END_USER");
            dtb.Columns.Add("END_USER_NAME");
            dtb.Columns.Add("CR_HR");
            dtb.Columns.Add("GRADE");
            dtb.Columns.Add("Surface");
            dtb.Columns.Add("Surface_cd");
            dtb.Columns.Add("Thk", typeof(decimal));
            dtb.Columns.Add("Wth", typeof(decimal));
            dtb.Columns.Add("ed");
            dtb.Columns.Add("Qty", typeof(int));
            //dtb.Columns.Add("WGT");
            dtb.Columns.Add("BASE_PRICE", typeof(decimal));
            dtb.Columns.Add("EFF_PRICE", typeof(decimal));
            dtb.Columns.Add("CONTRACT_NO");
            dtb.Columns.Add("USG");
            dtb.Columns.Add("STATUS");
            dtb.Columns.Add("EMPLOYEE");
            dtb.Columns.Add("BIDD_PRICE", typeof(decimal));
            dtb.Columns.Add("DELIVERY_TIME");
            dtb.Columns.Add("REMARK");

            CUSTOMER cus = new CUSTOMER();
            foreach (var item in list)
            {
                END_USER end = cus.GetEndUserById(item.END_USER);
                DataRow r = dtb.NewRow();
                r["DATE"] = item.ORDED_DATE == null ? "" : item.ORDED_DATE.ToString("yyyy-MM-dd");
                r["CUS_CODE"] = item.CUSTOMER_ID;
                r["CUS_NAME"] = item.NAME;
                r["END_USER"] = item.END_USER;
                r["END_USER_NAME"] = end == null ? item.NAME : end.NAME;
                r["CR_HR"] = item.ORDER_CR_HR;
                r["GRADE"] = item.STS_ST_CLS;
                r["Surface"] = item.STS_ST_SER;
                r["Surface_cd"] = item.SURFACE_CD;
                r["Thk"] = item.ORD_THK;
                r["Wth"] = item.ORD_WTH;
                r["ed"] = item.ORD_EDGE;
                r["Qty"] =Convert.ToInt32( item.QUANTITY);
                //r["WGT"] = item.ORD_WGT;
                r["BASE_PRICE"] = item.BASE_PRICE;
                r["EFF_PRICE"] = item.EFFECT_PRICE;
                r["CONTRACT_NO"] = item.CONTRACT_NO;
                r["USG"] = item.ORD_USAGE;
                r["STATUS"] = item.ORD_STAT;
                r["EMPLOYEE"] = item.EMP_NAME;
                r["BIDD_PRICE"] = item.BIDD_PRICE;
                r["DELIVERY_TIME"] = item.DELIVERY_TIME;
                r["REMARK"] = item.REMARK;
                dtb.Rows.Add(r);
            }
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
            return RedirectToAction("Order");
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
