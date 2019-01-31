using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;

namespace VstCustomer
{
    public class OrderReport
    {
        public string Name { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string Dept { get; set; }
        public string Status { get; set; }
        public string Usage { get; set; }
        public int InQuiryTotal { get; set; }
        public int InQuiry300 { get; set; }
        public int InQuiry400 { get; set; }
        public int InQuiry200 { get; set; }
        public int InQuiryNo1 { get; set; }
        public int InQuiryBA300 { get; set; }
        public int InQuiryBA400 { get; set; }
        public int InQuiryBA200 { get; set; }
        public int InQuiry2B2D300 { get; set; }
        public int InQuiry2B2D400 { get; set; }
        public int InQuiry2B2D200 { get; set; }

        public int Contract300 { get; set; }
        public int Contract400 { get; set; }
        public int Contract200 { get; set; }
        public int ContractTotal { get; set; }

        public decimal ContractPrice300 { get; set; }
        public decimal ContractPrice400 { get; set; }
        public decimal ContractPrice200 { get; set; }

        public decimal BidPrice300 { get; set; }
        public decimal BidPrice400 { get; set; }
        public decimal BidPrice200 { get; set; }

        public int ContractBA300 { get; set; }
        public int ContractBA400 { get; set; }
        public int ContractBA200 { get; set; }
        public int Contract2B2D300 { get; set; }
        public int Contract2B2D400 { get; set; }
        public int Contract2B2D200 { get; set; }

        public List<OrderReport> GetRowsBySalesAndKind(List<OrderResult> lst, string dept, string kind)
        {
            var result = new List<OrderReport>();
            var lstFilterByDeptAndKind = lst.Where(x => x.EMP_DEPT.Trim() == dept && x.ORD_USAGE.Trim() == kind).ToList();
            foreach (var item in lstFilterByDeptAndKind)
            {
                var row = new OrderReport();
                var id = result.FirstOrDefault(x => x.CUSTOMER_ID.Trim() == item.CUSTOMER_ID.Trim() && x.Dept.Contains(dept) && x.Usage.Contains(kind));
                if (id == null)
                {
                    row.Name = item.Name.Trim();
                    row.Dept = item.EMP_DEPT.Trim();
                    row.CUSTOMER_ID = item.CUSTOMER_ID;
                    row.Usage = item.ORD_USAGE.Trim();

                    row.InQuiry200 = (from x in lst where x.ORD_STAT.Contains("Inquiry") && x.STS_ST_CLS.Contains("200") && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                      && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                      select x.QUANTITY).Sum();
                    row.InQuiry300 = (from x in lst where x.ORD_STAT.Contains("Inquiry") && x.STS_ST_CLS.Contains("300") && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                      && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                      select x.QUANTITY).Sum();
                    row.InQuiry400 = (from x in lst where x.ORD_STAT.Contains("Inquiry") && x.STS_ST_CLS.Contains("400") && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                      && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                      select x.QUANTITY).Sum();
                    row.InQuiryTotal = row.InQuiry300 + row.InQuiry400 + row.InQuiry200;

                    row.Contract200 = (from x in lst where x.ORD_STAT.Contains("Confirmed") && x.STS_ST_CLS.Contains("200") && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                       && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                       select x.QUANTITY).Sum();
                    row.Contract300 = (from x in lst where x.ORD_STAT.Contains("Confirmed") && x.STS_ST_CLS.Contains("300") && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                       && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                       select x.QUANTITY).Sum();
                    row.Contract400 = (from x in lst where x.ORD_STAT.Contains("Confirmed") && x.STS_ST_CLS.Contains("400") && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                       && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                       select x.QUANTITY).Sum();
                    row.ContractTotal = row.Contract200 + row.Contract300 + row.Contract400;

                    row.ContractPrice200 = (from x in lst where x.ORD_STAT.Contains("Confirmed") && x.STS_ST_CLS.Contains("200") && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                            && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                            select x.EFFECT_PRICE).Sum();
                    row.ContractPrice300 = (from x in lst where x.ORD_STAT.Contains("Confirmed") && x.STS_ST_CLS.Contains("300") && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                            && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                            select x.EFFECT_PRICE).Sum();
                    row.ContractPrice400 = (from x in lst where x.ORD_STAT.Contains("Confirmed") && x.STS_ST_CLS.Contains("400") && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                            && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                            select x.EFFECT_PRICE).Sum();

                    row.BidPrice200 = (from x in lst where x.ORD_STAT.Contains("Confirmed") && x.STS_ST_CLS.Contains("200") && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                       && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                       select x.BIDD_PRICE).Sum();
                    row.BidPrice300 = (from x in lst where x.ORD_STAT.Contains("Confirmed") && x.STS_ST_CLS.Contains("300") && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                       && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                       select x.BIDD_PRICE).Sum();
                    row.BidPrice400 = (from x in lst where x.ORD_STAT.Contains("Confirmed") && x.STS_ST_CLS.Contains("400") && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                       && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                       select x.BIDD_PRICE).Sum();

                    row.InQuiryBA200 = (from x in lst where x.ORD_STAT.Contains("Inquiry") && x.STS_ST_CLS.Contains("200") && x.SURFACE_CD.Contains("BA") && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                        && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                        select x.QUANTITY).Sum();
                    row.InQuiryBA300 = (from x in lst where x.ORD_STAT.Contains("Inquiry") && x.STS_ST_CLS.Contains("300") && x.SURFACE_CD.Contains("BA") && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                        && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                        select x.QUANTITY).Sum();
                    row.InQuiryBA400 = (from x in lst where x.ORD_STAT.Contains("Inquiry") && x.STS_ST_CLS.Contains("400") && x.SURFACE_CD.Contains("BA") && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                        && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                        select x.QUANTITY).Sum();

                    row.InQuiry2B2D200 = (from x in lst where x.ORD_STAT.Contains("Inquiry") && x.STS_ST_CLS.Contains("200") && (x.SURFACE_CD.Contains("2B") || x.SURFACE_CD.Contains("2D")) && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                          && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                          select x.QUANTITY).Sum();
                    row.InQuiry2B2D400 = (from x in lst where x.ORD_STAT.Contains("Inquiry") && x.STS_ST_CLS.Contains("300") && (x.SURFACE_CD.Contains("2B") || x.SURFACE_CD.Contains("2D")) && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                          && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                          select x.QUANTITY).Sum();
                    row.InQuiry2B2D400 = (from x in lst where x.ORD_STAT.Contains("Inquiry") && x.STS_ST_CLS.Contains("400") && (x.SURFACE_CD.Contains("2B") || x.SURFACE_CD.Contains("2D")) && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                          && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                          select x.QUANTITY).Sum();

                    row.ContractBA200 = (from x in lst where x.ORD_STAT.Contains("Confirmed") && x.STS_ST_CLS.Contains("200") && x.SURFACE_CD.Contains("BA") && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                         && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                         select x.QUANTITY).Sum();
                    row.ContractBA300 = (from x in lst where x.ORD_STAT.Contains("Confirmed") && x.STS_ST_CLS.Contains("300") && x.SURFACE_CD.Contains("BA") && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                         && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                         select x.QUANTITY).Sum();
                    row.ContractBA400 = (from x in lst where x.ORD_STAT.Contains("Confirmed") && x.STS_ST_CLS.Contains("400") && x.SURFACE_CD.Contains("BA") && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                         && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                         select x.QUANTITY).Sum();

                    row.Contract2B2D200 = (from x in lst where x.ORD_STAT.Contains("Confirmed") && x.STS_ST_CLS.Contains("200") && (x.SURFACE_CD.Contains("2B") || x.SURFACE_CD.Contains("2D")) && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                           && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                           select x.QUANTITY).Sum();
                    row.Contract2B2D300 = (from x in lst where x.ORD_STAT.Contains("Confirmed") && x.STS_ST_CLS.Contains("300") && (x.SURFACE_CD.Contains("2B") || x.SURFACE_CD.Contains("2D")) && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                           && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                           select x.QUANTITY).Sum();
                    row.Contract2B2D400 = (from x in lst where x.ORD_STAT.Contains("Confirmed") && x.STS_ST_CLS.Contains("400") && (x.SURFACE_CD.Contains("2B") || x.SURFACE_CD.Contains("2D")) && x.CUSTOMER_ID.Contains(item.CUSTOMER_ID.Trim())
                                           && x.EMP_DEPT.Contains(dept) && x.ORD_USAGE.Contains(kind)
                                           select x.QUANTITY).Sum();

                    result.Add(row);
                }
            }
            var rowTotal = new OrderReport();
            rowTotal.Name = dept;
            rowTotal.InQuiryTotal = result.Select(o => o.InQuiryTotal).Sum();
            rowTotal.ContractTotal = result.Select(o => o.ContractTotal).Sum();

            if (rowTotal.InQuiryTotal > 0 || rowTotal.ContractTotal > 0)
            {
                rowTotal.InQuiry200 = result.Select(o => o.InQuiry200).Sum();
                rowTotal.InQuiry300 = result.Select(o => o.InQuiry300).Sum();
                rowTotal.InQuiry400 = result.Select(o => o.InQuiry400).Sum();

                rowTotal.Contract200 = result.Select(o => o.Contract200).Sum();
                rowTotal.Contract300 = result.Select(o => o.Contract300).Sum();
                rowTotal.Contract400 = result.Select(o => o.Contract400).Sum();

                result.Add(rowTotal);
            }
            return result;
        }
    }
}