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
        public JsonResult GetOrder(DataTableParameters dataTableParameters, string from, string to, string cust_id, string status)
        {
            ORDERED order = new ORDERED();
         
            var resultSet = new DataTableResultSet();
            resultSet.draw = dataTableParameters.Draw;
            var lst = order.SelectPaging(dataTableParameters.Start + 1,
                dataTableParameters.Start + dataTableParameters.Length + 1, from, to, cust_id, status);
            resultSet.recordsTotal = resultSet.recordsFiltered = order.GetCount( from, to,  cust_id, status);
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
                columns.Add(i.ORD_THK == null ? "" : Math.Round(Convert.ToDouble(i.ORD_THK), 2, MidpointRounding.ToEven).ToString());
                columns.Add(i.ORD_WTH == null ? "" : Math.Round(Convert.ToDouble(i.ORD_WTH), 2, MidpointRounding.ToEven).ToString());
                columns.Add(i.ORD_EDGE == null ? "" : i.ORD_EDGE.ToString());
                columns.Add(i.QUANTITY == null ? "" : Math.Round(Convert.ToDouble(i.QUANTITY), 2, MidpointRounding.ToEven).ToString());
                //columns.Add(i.ORD_WGT == null ? "" : i.ORD_WGT.ToString());
                columns.Add(i.BASE_PRICE == null ? "" : Math.Round(Convert.ToDouble(i.BASE_PRICE), 2, MidpointRounding.ToEven).ToString());
                columns.Add(i.EFFECT_PRICE == null ? "" : Math.Round(Convert.ToDouble(i.EFFECT_PRICE), 2, MidpointRounding.ToEven).ToString());
                columns.Add(i.BIDD_PRICE == null ? "" : Math.Round(Convert.ToDouble(i.BIDD_PRICE), 2, MidpointRounding.ToEven).ToString());
                columns.Add(i.CONTRACT_NO == null ? "" : i.CONTRACT_NO.ToString());
                columns.Add(i.ORD_USAGE);
                columns.Add(i.ORD_STAT);
                columns.Add(i.EMP_NAME.Trim());
                columns.Add(i.DELIVERY_TIME == null ? "" : i.DELIVERY_TIME.Trim());
                columns.Add(i.REMARK == null ? "" : i.REMARK.Trim());
                columns.Add(i.CREATE_AT == null ? "" : i.CREATE_AT.ToString());
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


        public ActionResult Export(string from, string to, string cus_id, string status)
        {
            ORDERED order = new ORDERED();
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            var list = order.GetExport(from, to, cus_id, status);
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
            dtb.Columns.Add("CREATE_AT");

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
                r["CREATE_AT"] = item.CREATE_AT;
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

        public ActionResult ExportReport(string from, string to, string cus_id, string status)
        {
            var order = new ORDERED();
            var orderReport = new OrderReport();
            var lstOrderReport = new List<OrderReport>();

            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            var list = order.GetSumExport(from.Trim(), to.Trim(), cus_id.Trim(), status.Trim());

            var orderKinds = list.Select(o => o.ORD_USAGE).Distinct().ToList();
            foreach (var kind in orderKinds)
            {
                var salesTeamResult = orderReport.GetRowsBySalesAndKind(list, "Sales Team", kind);
                var solutionTeamResult = orderReport.GetRowsBySalesAndKind(list, "Solution Sales", kind);
                var solutionImproveTeamResult = orderReport.GetRowsBySalesAndKind(list, "Solution Improvement ", kind);
                if (salesTeamResult != null)
                {
                    lstOrderReport.AddRange(salesTeamResult);
                }
                if (solutionTeamResult != null)
                {
                    lstOrderReport.AddRange(solutionTeamResult);
                }
                if (solutionImproveTeamResult != null)
                {
                    lstOrderReport.AddRange(solutionImproveTeamResult);
                }
          
                var sumRow = new OrderReport();
                sumRow.Name = kind;
                sumRow.InQuiryTotal = (from o in lstOrderReport where o.Usage != null && o.Usage.Contains(kind) select o.InQuiryTotal).Sum();
                sumRow.InQuiry200 = (from o in lstOrderReport where o.Usage != null && o.Usage.Contains(kind) select o.InQuiry200).Sum();
                sumRow.InQuiry300 = (from o in lstOrderReport where o.Usage != null && o.Usage.Contains(kind) select o.InQuiry300).Sum();
                sumRow.InQuiry400 = (from o in lstOrderReport where o.Usage != null && o.Usage.Contains(kind) select o.InQuiry400).Sum();

                sumRow.ContractTotal = (from o in lstOrderReport where o.Usage != null && o.Usage.Contains(kind) select o.ContractTotal).Sum();
                sumRow.Contract200 = (from o in lstOrderReport where o.Usage != null && o.Usage.Contains(kind) select o.Contract200).Sum();
                sumRow.Contract300 = (from o in lstOrderReport where o.Usage != null && o.Usage.Contains(kind) select o.Contract300).Sum();
                sumRow.Contract400 = (from o in lstOrderReport where o.Usage != null &&  o.Usage.Contains(kind) select o.Contract400).Sum();
                lstOrderReport.Add(sumRow);
            }
            var Total = new OrderReport();
            Total.Name = "Grand Total";
            Total.InQuiryTotal = (from o in lstOrderReport where o.CUSTOMER_ID == null select o.InQuiryTotal).Sum();
            Total.InQuiry200 = (from o in lstOrderReport where o.CUSTOMER_ID == null select o.InQuiry200).Sum();
            Total.InQuiry300 = (from o in lstOrderReport where o.CUSTOMER_ID == null select o.InQuiry300).Sum();
            Total.InQuiry400 = (from o in lstOrderReport where o.CUSTOMER_ID == null select o.InQuiry400).Sum();

            Total.ContractTotal = (from o in lstOrderReport where o.CUSTOMER_ID == null select o.ContractTotal).Sum();
            Total.Contract200 = (from o in lstOrderReport where o.CUSTOMER_ID == null select o.Contract200).Sum();
            Total.Contract300 = (from o in lstOrderReport where o.CUSTOMER_ID == null  select o.Contract300).Sum();
            Total.Contract400 = (from o in lstOrderReport where o.CUSTOMER_ID == null select o.Contract400).Sum();

            var saleTeamTotal = new OrderReport();
            saleTeamTotal.Name = "Sales Team Total";
            saleTeamTotal.InQuiryTotal = (from o in lstOrderReport where o.Name.Contains("Sales Team") select o.InQuiryTotal).Sum();
            saleTeamTotal.InQuiry200 = (from o in lstOrderReport where o.Name.Contains("Sales Team") select o.InQuiry200).Sum();
            saleTeamTotal.InQuiry300 = (from o in lstOrderReport where o.Name.Contains("Sales Team") select o.InQuiry300).Sum();
            saleTeamTotal.InQuiry400 = (from o in lstOrderReport where o.Name.Contains("Sales Team") select o.InQuiry400).Sum();

            saleTeamTotal.ContractTotal = (from o in lstOrderReport where o.Name.Contains("Sales Team") select o.ContractTotal).Sum();
            saleTeamTotal.Contract200 = (from o in lstOrderReport where o.Name.Contains("Sales Team") select o.Contract200).Sum();
            saleTeamTotal.Contract300 = (from o in lstOrderReport where o.Name.Contains("Sales Team") select o.Contract300).Sum();
            saleTeamTotal.Contract400 = (from o in lstOrderReport where o.Name.Contains("Sales Team") select o.Contract400).Sum();

            var solutionTeamTotal = new OrderReport();
            solutionTeamTotal.Name = "Solution Sales Total";
            solutionTeamTotal.InQuiryTotal = (from o in lstOrderReport where o.Name.Contains("Solution Sales") select o.InQuiryTotal).Sum();
            solutionTeamTotal.InQuiry200 = (from o in lstOrderReport where o.Name.Contains("Solution Sales") select o.InQuiry200).Sum();
            solutionTeamTotal.InQuiry300 = (from o in lstOrderReport where o.Name.Contains("Solution Sales") select o.InQuiry300).Sum();
            solutionTeamTotal.InQuiry400 = (from o in lstOrderReport where o.Name.Contains("Solution Sales") select o.InQuiry400).Sum();

            solutionTeamTotal.ContractTotal = (from o in lstOrderReport where o.Name.Contains("Solution Sales") select o.ContractTotal).Sum();
            solutionTeamTotal.Contract200 = (from o in lstOrderReport where o.Name.Contains("Solution Sales") select o.Contract200).Sum();
            solutionTeamTotal.Contract300 = (from o in lstOrderReport where o.Name.Contains("Solution Sales") select o.Contract300).Sum();
            solutionTeamTotal.Contract400 = (from o in lstOrderReport where o.Name.Contains("Solution Sales") select o.Contract400).Sum();

            //Solution Improvement          

            var solutionImprovementTeamTotal = new OrderReport();
            solutionImprovementTeamTotal.Name = "Solution Improvement Total";
            solutionImprovementTeamTotal.InQuiryTotal = (from o in lstOrderReport where o.Name.Contains("Solution Improvement") select o.InQuiryTotal).Sum();
            solutionImprovementTeamTotal.InQuiry200 = (from o in lstOrderReport where o.Name.Contains("Solution Improvement") select o.InQuiry200).Sum();
            solutionImprovementTeamTotal.InQuiry300 = (from o in lstOrderReport where o.Name.Contains("Solution Improvement") select o.InQuiry300).Sum();
            solutionImprovementTeamTotal.InQuiry400 = (from o in lstOrderReport where o.Name.Contains("Solution Improvement") select o.InQuiry400).Sum();

            solutionImprovementTeamTotal.ContractTotal = (from o in lstOrderReport where o.Name.Contains("Solution Improvement") select o.ContractTotal).Sum();
            solutionImprovementTeamTotal.Contract200 = (from o in lstOrderReport where o.Name.Contains("Solution Improvement") select o.Contract200).Sum();
            solutionImprovementTeamTotal.Contract300 = (from o in lstOrderReport where o.Name.Contains("Solution Improvement") select o.Contract300).Sum();
            solutionImprovementTeamTotal.Contract400 = (from o in lstOrderReport where o.Name.Contains("Solution Improvement") select o.Contract400).Sum();

            lstOrderReport.Add(saleTeamTotal);
            lstOrderReport.Add(solutionTeamTotal);
            lstOrderReport.Add(solutionImprovementTeamTotal);
            lstOrderReport.Add(Total);
             var template = Server.MapPath("~/Template/order.xlsx");
            // start from cell E:4
             var i = 4;
             using (ExcelPackage package = new ExcelPackage(new FileInfo(template)))
             {
                 ExcelWorksheet ws = package.Workbook.Worksheets.FirstOrDefault();
                 ws.Cells["K1"].Value = string.Format("DOMESTIC ORDER RECORD {0} ~ {1}", from, to);
                 foreach (var item in lstOrderReport)
                 {
                     ws.Cells["E"+i].Value = item.Name.Trim();
                     ws.Cells["F" + i].Value = item.InQuiryTotal;
                     ws.Cells["G" + i].Value = item.InQuiry300;
                     ws.Cells["H" + i].Value = item.InQuiry400;
                     ws.Cells["I" + i].Value = item.InQuiry200;
                     ws.Cells["J" + i].Value = 0; // No1
                     ws.Cells["K" + i].Value = item.ContractTotal;
                     ws.Cells["L" + i].Value = item.Contract300;
                     ws.Cells["M" + i].Value = item.Contract400;
                     ws.Cells["N" + i].Value = item.Contract200;
                     ws.Cells["O" + i].Value = 0; // No1
                     ws.Cells["P" + i].Value = item.ContractPrice300;
                     ws.Cells["Q" + i].Value = item.ContractPrice400;
                     ws.Cells["R" + i].Value = item.ContractPrice200;
                     ws.Cells["S" + i].Value = 0; // No1
                     ws.Cells["T" + i].Value = item.BidPrice300;
                     ws.Cells["U" + i].Value = item.BidPrice400;
                     ws.Cells["V" + i].Value = item.BidPrice200;
                     ws.Cells["W" + i].Value = 0; // No1

                     ws.Cells["Z" + i].Value = item.InQuiryBA300;
                     ws.Cells["AA" + i].Value = item.InQuiryBA400;
                     ws.Cells["AB" + i].Value = item.InQuiryBA200;
                     ws.Cells["AC" + i].Value = item.InQuiry2B2D300;
                     ws.Cells["AD" + i].Value = item.InQuiry2B2D400;
                     ws.Cells["AE" + i].Value = item.InQuiry2B2D200;
                     ws.Cells["AF" + i].Value = 0; // NO 1

                     ws.Cells["AG" + i].Value = item.InQuiry300;
                     ws.Cells["AH" + i].Value = item.InQuiry400;
                     ws.Cells["AI" + i].Value = item.InQuiry200;
                     ws.Cells["AJ" + i].Value = 0; // No 1
                     ws.Cells["AK" + i].Value = item.InQuiryTotal;
                     // kkk
                     ws.Cells["AO" + i].Value = item.ContractBA300;
                     ws.Cells["AP" + i].Value = item.ContractBA400;
                     ws.Cells["AQ" + i].Value = item.ContractBA200;
                     ws.Cells["AR" + i].Value = item.Contract2B2D300;
                     ws.Cells["AS" + i].Value = item.Contract2B2D400;
                     ws.Cells["AT" + i].Value = item.Contract2B2D200;
                     ws.Cells["AU" + i].Value = 0; // NO 1

                     ws.Cells["AV" + i].Value = item.Contract300;
                     ws.Cells["AW" + i].Value = item.Contract400;
                     ws.Cells["AX" + i].Value = item.Contract200;
                     ws.Cells["AY" + i].Value = 0; // No 1
                     ws.Cells["AZ" + i].Value = item.ContractTotal;
                    
                     i++;
                 }
                 var index2 = i + 2;
                 var index3 = i + 3;
                 var index4 = i + 4;
                 ws.Cells["E" + index2].Value = "INQUIRY";
                 ws.Cells["N" + index2].Value = "CONTRACT";
                 ws.Cells["G" + index2].Value = ws.Cells["Q" + index2].Value = "300";
                 ws.Cells["H" + index2].Value = ws.Cells["R" + index2].Value = "400";
                 ws.Cells["I" + index2].Value = ws.Cells["S" + index2].Value = "200";
                 ws.Cells["G" + index2].Value = ws.Cells["Q" + index2].Value = "300";
                 ws.Cells["J" + index2].Value = ws.Cells["T" + index2].Value = "SUB TOTAL";
                 ws.Cells["L" + index2].Value = ws.Cells["V" + index2].Value = "TOTAL";
                 ws.Cells["E" + index3].Value = ws.Cells["N" + index3].Value = "SURFACE";
                 ws.Cells["F" + index3].Value = ws.Cells["P" + index3].Value = "BA";
                 ws.Cells["F" + index4].Value = ws.Cells["P" + index4].Value = "2B & 2D";

                 var inquiryBA300 = (from o in lstOrderReport where o.CUSTOMER_ID != null select o.InQuiryBA300).Sum();
                 var inquiryBA400 = (from o in lstOrderReport where o.CUSTOMER_ID != null select o.InQuiryBA400).Sum();
                 var inquiryBA200 = (from o in lstOrderReport where o.CUSTOMER_ID != null select o.InQuiryBA200).Sum();
                 var subTotalInquiryBA = inquiryBA300 + inquiryBA400 + inquiryBA200;
                 var inquiry2B2D300 = (from o in lstOrderReport where o.CUSTOMER_ID != null select o.InQuiry2B2D300).Sum();
                 var inquiry2B2D400 = (from o in lstOrderReport where o.CUSTOMER_ID != null select o.InQuiry2B2D400).Sum();
                 var inquiry2B2D200 = (from o in lstOrderReport where o.CUSTOMER_ID != null select o.InQuiry2B2D200).Sum();
                 var subTotalInquiry2B2D = inquiry2B2D300 + inquiry2B2D400 + inquiry2B2D200;

                 var inquiryTotal = subTotalInquiryBA + subTotalInquiry2B2D;

                 var contractBA300 = (from o in lstOrderReport where o.CUSTOMER_ID != null select o.ContractBA300).Sum();
                 var contractBA400 = (from o in lstOrderReport where o.CUSTOMER_ID != null select o.ContractBA400).Sum();
                 var contractBA200 = (from o in lstOrderReport where o.CUSTOMER_ID != null select o.ContractBA200).Sum();
                 var subTotalContractBA = contractBA300 + contractBA400 + contractBA200;
                 var contract2B2D300 = (from o in lstOrderReport where o.CUSTOMER_ID != null select o.Contract2B2D300).Sum();
                 var contract2B2D400 = (from o in lstOrderReport where o.CUSTOMER_ID != null select o.Contract2B2D400).Sum();
                 var contract2B2D200 = (from o in lstOrderReport where o.CUSTOMER_ID != null select o.Contract2B2D200).Sum();
                 var subTotalContrac2B2D = contract2B2D300 + contract2B2D400 + contract2B2D200;

                 var contractTotal = subTotalContractBA + subTotalContrac2B2D;

                 ws.Cells["G" + index3].Value = inquiryBA300;
                 ws.Cells["H" + index3].Value = inquiryBA400;
                 ws.Cells["I" + index3].Value = inquiryBA200;
                 ws.Cells["J" + index3].Value = subTotalInquiryBA;
                 ws.Cells["G" + index4].Value = inquiry2B2D300;
                 ws.Cells["H" + index4].Value = inquiry2B2D400;
                 ws.Cells["I" + index4].Value = inquiry2B2D200;
                 ws.Cells["J" + index4].Value = subTotalInquiry2B2D;
                 ws.Cells["L" + index3].Value = inquiryTotal;

                 ws.Cells["Q" + index3].Value = contractBA300;
                 ws.Cells["R" + index3].Value = contractBA400;
                 ws.Cells["S" + index3].Value = contractBA200;
                 ws.Cells["T" + index3].Value = subTotalContractBA;
                 ws.Cells["V" + index3].Value = contractTotal;
                 ws.Cells["Q" + index4].Value = contract2B2D300;
                 ws.Cells["R" + index4].Value = contract2B2D400;
                 ws.Cells["S" + index4].Value = contract2B2D200;
                 ws.Cells["T" + index4].Value = subTotalContrac2B2D;
                 ws.Column(5).Width = 50;
                 //buffer = package.Stream as MemoryStream;
                 Byte[] fileBytes = package.GetAsByteArray();
                 Response.ClearContent();
                 Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                 Response.AddHeader("content-disposition", "attachment;  filename=" + fileName);
                 Response.BinaryWrite(fileBytes);
                 Response.Flush();
                 Response.End();
             }

            return RedirectToAction("Order");;
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
