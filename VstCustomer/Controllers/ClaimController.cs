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
    public class ClaimController : Controller
    {
        //
        // GET: /Claim/

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
            ViewBag.ClaimStatus = code.Select("CLIAM_STATUS");
            ViewBag.DefectLine = code.Select("DEFECT_LINE");
            return View();
        }

        [HttpPost]
        public JsonResult GetClaim(DataTableParameters dataTableParameters, string month, string cust_id, string status)
        {
            CLAIM cl = new CLAIM();

            var resultSet = new DataTableResultSet();
            resultSet.draw = dataTableParameters.Draw;
            var lst = cl.SelectPaging(dataTableParameters.Start + 1,
                dataTableParameters.Start + dataTableParameters.Length + 1, month,  cust_id,  status);
            resultSet.recordsTotal = resultSet.recordsFiltered = cl.GetCount(month, cust_id, status);
            CUSTOMER cus = new CUSTOMER();
            foreach (var i in lst)
            {
                END_USER end = cus.GetEndUserById(i.END_USER);
                var columns = new List<string>();
                columns.Add("<input type='checkbox' class='ckb' id='" + i.CLAIM_NO + "' data-cus='" + i.CUSTOMER_ID + "' data-emp='"+i.EMP_ID.Trim()+"' />");
                columns.Add(i.CLAIM_DATE == null? "" : i.CLAIM_DATE.ToShortDateString());
                columns.Add(i.NAME);
                columns.Add(end == null ? i.NAME : end.NAME);
                columns.Add(i.LOC);
                columns.Add(i.COIL_NO);
                columns.Add(i.SPEC);
                columns.Add(i.SURFACE_CD);
                columns.Add(Math.Round(Convert.ToDouble(i.COIL_THK), 2, MidpointRounding.ToEven).ToString());
                columns.Add(Math.Round(Convert.ToDouble(i.COIL_WTH), 2, MidpointRounding.ToEven).ToString());
                columns.Add(Math.Round(Convert.ToDouble(i.NET_WGT), 2, MidpointRounding.ToEven).ToString());
                columns.Add(Math.Round(Convert.ToDouble(i.CLAIM_WGT), 2, MidpointRounding.ToEven).ToString());
                columns.Add(i.VISIT_DATE == null ? "" : i.VISIT_DATE.ToShortDateString());
                columns.Add(i.REMARK);
                columns.Add(i.DEFFECT_KIND == null ? "" : i.DEFFECT_KIND.Trim());
                columns.Add(i.EMP_NAME);
                resultSet.data.Add(columns);

            }
            return Json(resultSet);

        }

        [HttpPost]
        public JsonResult InsertUpdateClaim(CLAIM claim, int ACTION)
        {
            claim.CUSTOMER_ID = claim.CUSTOMER_ID.ToUpper();
            claim.END_USER = claim.END_USER.ToUpper();
            var result = 0;
            if (ACTION == 1)
            {
                //Update(string EMP_ID, string CLAIM_NO, DateTime CLAIM_DATE, string CUSTOMER_ID, string COIL_NO, decimal CLAIM_WGT, decimal NET_WGT, DateTime VISIT_DATE, string DEFECT_CD, string DEFECT_LINE, DateTime FINISH_DATE, decimal COMPENT, string REMARK, string STATUS, decimal COIL_THK, decimal COIL_WTH, string STS_ST_CLS, string SURFACE_CD, string GRADE)
                result = claim.Update(claim.EMP_ID,claim.CLAIM_NO, claim.CLAIM_DATE,claim.CUSTOMER_ID,claim.COIL_NO,
                    claim.CLAIM_WGT,claim.NET_WGT,claim.VISIT_DATE,claim.DEFECT_CD,claim.DEFECT_LINE,claim.FINISH_DATE,
                    claim.COMPENT, claim.REMARK, claim.STATUS, claim.COIL_THK, claim.COIL_WTH, claim.STS_ST_CLS, claim.SURFACE_CD, claim.GRADE,
                    claim.DEFFECT_KIND,claim.END_USER,claim.SPEC,claim.TYPE,claim.ATTACHMENT);
               
            }
            else
            {
                
                //Insert(string EMP_ID, string CLAIM_NO, DateTime CLAIM_DATE, string CUSTOMER_ID, string COIL_NO, decimal CLAIM_WGT, decimal NET_WGT, DateTime VISIT_DATE, string DEFECT_CD, string DEFECT_LINE, DateTime FINISH_DATE, decimal COMPENT, string REMARK, string STATUS, decimal COIL_THK, decimal COIL_WTH, string STS_ST_CLS, string SURFACE_CD, string GRADE)
                result = claim.Insert(claim.EMP_ID, claim.CLAIM_NO, claim.CLAIM_DATE, claim.CUSTOMER_ID, claim.COIL_NO,
                     claim.CLAIM_WGT, claim.NET_WGT, claim.VISIT_DATE, claim.DEFECT_CD, claim.DEFECT_LINE, claim.FINISH_DATE,
                     claim.COMPENT, claim.REMARK, claim.STATUS, claim.COIL_THK, claim.COIL_WTH, claim.STS_ST_CLS, claim.SURFACE_CD, claim.GRADE,
                     claim.DEFFECT_KIND,claim.END_USER,claim.SPEC,claim.TYPE,claim.ATTACHMENT);
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetClaimById(string ID)
        {
            CLAIM claim = new CLAIM();

            List<CLAIM> lst = claim.Select(ID);
            return Json(lst);
        }

        [HttpPost]
        public JsonResult GetEndUser(string CUS_ID)
        {
            CUSTOMER cus = new CUSTOMER();
            var end = cus.GetEndUser(CUS_ID);

            return Json(end);
        }

        [HttpPost]
        public JsonResult GetInfo(string CUS_ID, string EMP_ID)
        {
            CUSTOMER cus = new CUSTOMER();
            EMPLOYEE em = new EMPLOYEE();
            List<string> lst = new List<string>();
            //List<END_USER> end = cus.GetEndUser(CUS_ID);
            cus = cus.Select(CUS_ID).FirstOrDefault();
            em = em.Select(EMP_ID).FirstOrDefault();
            lst.Add(em.EMP_NAME);
            lst.Add(cus==null ? "" : cus.NAME);
          
            return Json(lst);
        }

        [HttpPost]
        public JsonResult Delete(string ID)
        {
            CLAIM claim = new CLAIM();
            var result = claim.Delete(ID);
            return Json(result);
        }

        [HttpPost]
        public JsonResult Upload()
        {
            var file = Request.Files[0];
            var path = Server.MapPath("~/Upload/" + DateTime.Now.ToString("yyyyMMdd") + "/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var fileName = Path.GetFileName(file.FileName);

            var pathSave = Path.Combine(path, fileName);
            file.SaveAs(pathSave);
            return Json("/Upload/" + DateTime.Now.ToString("yyyyMMdd") + "/" + file.FileName);
        }


        public FileResult Download(string file)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath(file));
            var response = new FileContentResult(fileBytes, "application/octet-stream");
            response.FileDownloadName = Path.GetFileName(Server.MapPath(file));
            return response;
        }

        public ActionResult Export(string month, string cus_id, string status)
        {
            CLAIM c = new CLAIM();
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            var list = c.GetExport(month, cus_id, status);
            DataTable dtb = new DataTable();

            dtb.Clear();

            dtb.Columns.Add("CLAIM_DATE");
            dtb.Columns.Add("CUS_NAME");
            dtb.Columns.Add("END_USER_NAME");
            dtb.Columns.Add("LOC");
            dtb.Columns.Add("COIL_NO");
            dtb.Columns.Add("SPEC");
            dtb.Columns.Add("SURFACE_CD");

            dtb.Columns.Add("COIL_THK", typeof(decimal));
            dtb.Columns.Add("COIL_WTH", typeof(decimal));
            dtb.Columns.Add("NET_WGT", typeof(decimal));
            dtb.Columns.Add("CLAIM_WGT", typeof(decimal));
            dtb.Columns.Add("VISIT_DATE");
            dtb.Columns.Add("REMARK");
            dtb.Columns.Add("DEFFECT_KIND");
            dtb.Columns.Add("EMP_NAME");

            CUSTOMER cus = new CUSTOMER();
            foreach (var item in list)
            {
                END_USER end = cus.GetEndUserById(item.END_USER);
                DataRow r = dtb.NewRow();
                r["CLAIM_DATE"] = item.CLAIM_DATE == null ? "" : item.CLAIM_DATE.ToString("yyyy-MM-dd");
                r["CUS_NAME"] = item.NAME;
                r["END_USER_NAME"] = end==null ?  item.NAME : end.NAME;
                r["LOC"] = item.LOC;
                r["COIL_NO"] = item.COIL_NO;
                r["SPEC"] = item.SPEC;
                r["SURFACE_CD"] = item.SURFACE_CD;
                r["COIL_THK"] = item.COIL_THK;
                r["COIL_WTH"] = item.COIL_WTH;
                r["NET_WGT"] = item.NET_WGT;
                r["CLAIM_WGT"] = item.CLAIM_WGT;
                r["VISIT_DATE"] = item.VISIT_DATE == null ? "" : item.VISIT_DATE.ToString("yyyy-MM-dd");
                r["REMARK"] = item.REMARK;
                r["DEFFECT_KIND"] = item.DEFFECT_KIND;
                r["EMP_NAME"] = item.EMP_NAME;
               
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
            return RedirectToAction("Claim");
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
