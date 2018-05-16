using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VstCustomer
{
    public class ORDERED
    {
        public string ID { get; set; }
        public string EMP_ID { get; set; }
        public DateTime ORDED_DATE { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string ORDER_CR_HR { get; set; }
        public string STS_ST_CLS { get; set; }
        public string STS_ST_SER { get; set; }
        public string SURFACE_CD { get; set; }
        public decimal ORD_THK { get; set; }
        public decimal ORD_WTH { get; set; }
        public string ORD_EDGE { get; set; }
        public decimal ORD_WGT { get; set; }
        public decimal BASE_PRICE { get; set; }
        public decimal EFFECT_PRICE { get; set; }
        public decimal BIDD_PRICE { get; set; }
        public string CONTRACT_NO { get; set; }
        public string ORD_USAGE { get; set; }
        public string ORD_STAT { get; set; }
        public string END_USER { get; set; }
        public int QUANTITY { get; set; }
        public string DELIVERY_TIME { get; set; }
        public string REMARK { get; set; }

        public ORDERED() { }

        public virtual List<ORDERED> Select(string ID = "")
        {
            var sql = "SELECT * FROM ORDERED ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<ORDERED>.ExecuteReader(sql);
            sql += " WHERE ID=@ID";

            return DBManager<ORDERED>.ExecuteReader(sql, new { ID = ID });
        }

        public virtual dynamic SelectPaging(int start = 0, int end = 10, string month = "", string cust_id = "", string status = "")
        {
            var sql = string.Format(@"SELECT * FROM(SELECT ROW_NUMBER() OVER (order by ORDED_DATE) AS ROWNUM, O.*,CUS.NAME,E.EMP_NAME,
(SELECT TOP 1 NAME FROM END_USER WHERE CUS_ID=O.END_USER) as END_USER_NAME
FROM ORDERED AS O
INNER JOIN CUSTOMER AS CUS ON O.CUSTOMER_ID=CUS.ID INNER JOIN EMPLOYEE AS E ON E.EMP_ID=O.EMP_ID
WHERE (@month='' OR ORDED_DATE LIKE @month+'%')
            AND (@cust_id='' OR CUSTOMER_ID=@cust_id) AND(@status='' OR ORD_STAT=@status)
) as u  WHERE   RowNum >= @start   AND RowNum < @end ORDER BY RowNum;");

            return DBManager<ORDERED>.ExecuteDynamic(sql, new
            {
                start = start,
                end = end,
                month = month,
                cust_id = cust_id,
                status = status
            });
        }
        public  dynamic GetExport( string month = "", string cust_id = "", string status = "")
        {
            var sql = string.Format(@"SELECT O.*,CUS.NAME,E.EMP_NAME,
(SELECT TOP 1 NAME FROM END_USER WHERE CUS_ID=O.END_USER) as END_USER_NAME
FROM ORDERED AS O
INNER JOIN CUSTOMER AS CUS ON O.CUSTOMER_ID=CUS.ID INNER JOIN EMPLOYEE AS E ON E.EMP_ID=O.EMP_ID
WHERE (@month='' OR ORDED_DATE LIKE @month+'%')
            AND (@cust_id='' OR CUSTOMER_ID=@cust_id) AND(@status='' OR ORD_STAT=@status)
");

            return DBManager<ORDERED>.ExecuteDynamic(sql, new
            {
              
                month = month,
                cust_id = cust_id,
                status = status
            });
        }

        public virtual int GetCount(string month = "", string cust_id = "", string status = "")
        {
            var sql = string.Format(@"SELECT COUNT(1) AS CNT FROM ORDERED WHERE (@month='' OR ORDED_DATE LIKE @month+'%')
            AND (@cust_id='' OR CUSTOMER_ID=@cust_id) AND(@status='' OR ORD_STAT=@status);");
            return (int)DBManager<ORDERED>.ExecuteScalar(sql, new
            {

                month = month,
                cust_id = cust_id,
                status = status
            });
        }

        public virtual int Insert(string EMP_ID, DateTime ORDED_DATE, string CUSTOMER_ID, string ORDER_CR_HR, string STS_ST_CLS, string STS_ST_SER,
            string SURFACE_CD, decimal ORD_THK, decimal ORD_WTH, string ORD_EDGE, decimal ORD_WGT, decimal BASE_PRICE, decimal EFFECT_PRICE,
            decimal BIDD_PRICE, string CONTRACT_NO, string ORD_USAGE, string ORD_STAT, string END_USER, int QUANTITY, string DELIVERY_TIME, string REMARK)
        {
            var id = this.GenerateId();
            var sql = "INSERT INTO ORDERED(ID,EMP_ID,ORDED_DATE,CUSTOMER_ID,ORDER_CR_HR,STS_ST_CLS,STS_ST_SER,SURFACE_CD,ORD_THK,ORD_WTH,ORD_EDGE,ORD_WGT,BASE_PRICE,EFFECT_PRICE,BIDD_PRICE,CONTRACT_NO,ORD_USAGE,ORD_STAT,END_USER,QUANTITY,DELIVERY_TIME,REMARK) VALUES(@ID,@EMP_ID,@ORDED_DATE,@CUSTOMER_ID,@ORDER_CR_HR,@STS_ST_CLS,@STS_ST_SER,@SURFACE_CD,@ORD_THK,@ORD_WTH,@ORD_EDGE,@ORD_WGT,@BASE_PRICE,@EFFECT_PRICE,@BIDD_PRICE,@CONTRACT_NO,@ORD_USAGE,@ORD_STAT,@END_USER,@QUANTITY,@DELIVERY_TIME,@REMARK)";
            return DBManager<ORDERED>.Execute(sql, new
            {
                ID = id,
                EMP_ID = EMP_ID,
                ORDED_DATE = ORDED_DATE,
                CUSTOMER_ID = CUSTOMER_ID,
                ORDER_CR_HR = ORDER_CR_HR,
                STS_ST_CLS = STS_ST_CLS,
                STS_ST_SER = STS_ST_SER,
                SURFACE_CD = SURFACE_CD,
                ORD_THK = ORD_THK,
                ORD_WTH = ORD_WTH,
                ORD_EDGE = ORD_EDGE,
                ORD_WGT = ORD_WGT,
                BASE_PRICE = BASE_PRICE,
                EFFECT_PRICE = EFFECT_PRICE,
                BIDD_PRICE = BIDD_PRICE,
                CONTRACT_NO = CONTRACT_NO,
                ORD_USAGE = ORD_USAGE,
                ORD_STAT = ORD_STAT,
                END_USER = END_USER,
                QUANTITY=QUANTITY,
                DELIVERY_TIME=DELIVERY_TIME,
                REMARK=REMARK
            });
        }

        public virtual int Update(string ID, string EMP_ID, DateTime ORDED_DATE, string CUSTOMER_ID, string ORDER_CR_HR, string STS_ST_CLS, string STS_ST_SER, string SURFACE_CD, decimal ORD_THK, decimal ORD_WTH, string ORD_EDGE, decimal ORD_WGT, decimal BASE_PRICE, decimal EFFECT_PRICE, decimal BIDD_PRICE, string CONTRACT_NO, string ORD_USAGE, string ORD_STAT, string END_USER, int QUANTITY,string DELIVERY_TIME, string REMARK)
        {
            var sql = "UPDATE ORDERED SET EMP_ID=@EMP_ID, ORDED_DATE=@ORDED_DATE,CUSTOMER_ID=@CUSTOMER_ID,ORDER_CR_HR=@ORDER_CR_HR,STS_ST_CLS=@STS_ST_CLS,STS_ST_SER=@STS_ST_SER,SURFACE_CD=@SURFACE_CD,ORD_THK=@ORD_THK,ORD_WTH=@ORD_WTH,ORD_EDGE=@ORD_EDGE,ORD_WGT=@ORD_WGT,BASE_PRICE=@BASE_PRICE,EFFECT_PRICE=@EFFECT_PRICE,BIDD_PRICE=@BIDD_PRICE,CONTRACT_NO=@CONTRACT_NO,ORD_USAGE=@ORD_USAGE,ORD_STAT=@ORD_STAT,END_USER=@END_USER,QUANTITY=@QUANTITY,DELIVERY_TIME=@DELIVERY_TIME,REMARK=@REMARK WHERE ID=@ID";

            return DBManager<ORDERED>.Execute(sql, new
            {
                ID = ID,
                EMP_ID = EMP_ID,
                ORDED_DATE = ORDED_DATE,
                CUSTOMER_ID = CUSTOMER_ID,
                ORDER_CR_HR = ORDER_CR_HR,
                STS_ST_CLS = STS_ST_CLS,
                STS_ST_SER = STS_ST_SER,
                SURFACE_CD = SURFACE_CD,
                ORD_THK = ORD_THK,
                ORD_WTH = ORD_WTH,
                ORD_EDGE = ORD_EDGE,
                ORD_WGT = ORD_WGT,
                BASE_PRICE = BASE_PRICE,
                EFFECT_PRICE = EFFECT_PRICE,
                BIDD_PRICE = BIDD_PRICE,
                CONTRACT_NO = CONTRACT_NO,
                ORD_USAGE = ORD_USAGE,
                ORD_STAT = ORD_STAT,
                QUANTITY = QUANTITY,
                END_USER=END_USER,
                REMARK = REMARK,
                DELIVERY_TIME=DELIVERY_TIME
            });
        }

        public virtual int Delete(string ID = "")
        {
            var sql = "DELETE FROM ORDERED ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<ORDERED>.Execute(sql);
            sql += " WHERE ID=@ID ";
            return DBManager<ORDERED>.Execute(sql, new { ID = ID });
        }

        public string GenerateId()
        {
            string id = DateTime.Now.ToString("yyMMdd");
            string sql = "select top 1 ID  from ORDERED WHERE ID LIKE @id + '%' order by ID desc";

            ORDERED claim = DBManager<ORDERED>.ExecuteReader(sql, new { id = id }).FirstOrDefault();
            if (claim == null)
            {
                id = id + "0001";
            }
            else
            {
                string str = claim.ID.Trim().Substring(6);
                string num = (Convert.ToInt32(str) + 1).ToString();
                for (int i = 0; i < str.Length - num.Length; i++)
                {
                    id += "0";
                }
                id += num.ToString();
            }
            return id;
        }
    }

}
