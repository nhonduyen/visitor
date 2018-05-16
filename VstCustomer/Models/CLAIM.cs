using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VstCustomer
{
    public class CLAIM
    {
        public string EMP_ID { get; set; }
        public string CLAIM_NO { get; set; }
        public DateTime? CLAIM_DATE { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string COIL_NO { get; set; }
        public decimal CLAIM_WGT { get; set; }
        public decimal NET_WGT { get; set; }
        public DateTime? VISIT_DATE { get; set; }
        public string DEFECT_CD { get; set; }
        public string DEFECT_LINE { get; set; }
        public DateTime? FINISH_DATE { get; set; }
        public decimal COMPENT { get; set; }
        public string REMARK { get; set; }
        public string STATUS { get; set; }
        public decimal COIL_THK { get; set; }
        public decimal COIL_WTH { get; set; }
        public string STS_ST_CLS { get; set; }
        public string SURFACE_CD { get; set; }
        public string TYPE { get; set; }
        public string END_USER { get; set; }
        public string GRADE { get; set; }
        public string DEFFECT_KIND { get; set; }
        public string SPEC { get; set; }

        public string ATTACHMENT { get; set; }

       
        public CLAIM() { }

        public virtual List<CLAIM> Select(string ID = "")
        {
            var sql = "SELECT * FROM CLAIM ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<CLAIM>.ExecuteReader(sql);
            sql += " WHERE CLAIM_NO=@ID";

            return DBManager<CLAIM>.ExecuteReader(sql, new { ID = ID });
        }

        public virtual dynamic SelectPaging(int start = 0, int end = 10, string month = "", string cust_id = "", string status = "")
        {

            var sql = string.Format(@"SELECT * FROM(SELECT ROW_NUMBER() OVER (order by CLAIM_DATE) AS ROWNUM, 
           C.*,CUS.NAME,CUS.LOCATION AS LOC,E.EMP_NAME,(SELECT TOP 1 NAME FROM CUSTOMER WHERE ID=C.END_USER) AS END_USER_NAME
FROM CLAIM AS C INNER JOIN CUSTOMER AS CUS ON C.CUSTOMER_ID=CUS.ID INNER JOIN EMPLOYEE AS E ON E.EMP_ID=C.EMP_ID
WHERE (@month='' OR CLAIM_DATE LIKE @month+'%')
            AND (@cust_id='' OR CUSTOMER_ID=@cust_id) AND(@status='' OR STATUS=@status)
            ) as u  WHERE   RowNum >= @start   AND RowNum < @end ORDER BY RowNum;");

            return DBManager<CLAIM>.ExecuteDynamic(sql, new
            {
                start = start,
                end = end,
                month = month,
                cust_id = cust_id,
                status = status
            });
        }

        public virtual int GetCount(string month = "", string cust_id = "", string status = "")
        {
            var sql = string.Format(@"SELECT COUNT(1) AS CNT FROM CLAIM WHERE (@month IS NULL OR CLAIM_DATE LIKE @month+'%')
            AND (@cust_id='' OR CUSTOMER_ID=@cust_id) AND(@status='' OR STATUS=@status)");

            return (int)DBManager<CLAIM>.ExecuteScalar(sql, new
            {

                month = month,
                cust_id = cust_id,
                status = status
            });
        }

        public dynamic GetExport(string month = "", string cust_id = "", string status = "")
        {

            var sql = string.Format(@"SELECT 
           C.*,CUS.NAME,CUS.LOCATION AS LOC,E.EMP_NAME,(SELECT TOP 1 NAME FROM CUSTOMER WHERE ID=C.END_USER) AS END_USER_NAME
FROM CLAIM AS C INNER JOIN CUSTOMER AS CUS ON C.CUSTOMER_ID=CUS.ID INNER JOIN EMPLOYEE AS E ON E.EMP_ID=C.EMP_ID
WHERE (@month='' OR CLAIM_DATE LIKE @month+'%')
            AND (@cust_id='' OR CUSTOMER_ID=@cust_id) AND(@status='' OR STATUS=@status)
           ");

            return DBManager<CLAIM>.ExecuteDynamic(sql, new
            {
               
                month = month,
                cust_id = cust_id,
                status = status
            });
        }

        public virtual int Insert(string EMP_ID, string CLAIM_NO, DateTime? CLAIM_DATE, string CUSTOMER_ID, string COIL_NO, decimal CLAIM_WGT, 
            decimal NET_WGT, DateTime? VISIT_DATE, string DEFECT_CD, string DEFECT_LINE, DateTime? FINISH_DATE, decimal COMPENT, string REMARK, 
            string STATUS, decimal COIL_THK, decimal COIL_WTH, string STS_ST_CLS, string SURFACE_CD, string GRADE, string DEFFECT_KIND,
            string END_USER, string SPEC, string TYPE, string ATTACHMENT="")
        {
            CLAIM_NO = this.GenerateId();
            var sql =string.Format(@"
INSERT INTO CLAIM(EMP_ID,CLAIM_NO,CLAIM_DATE,CUSTOMER_ID,COIL_NO,CLAIM_WGT,NET_WGT,VISIT_DATE,DEFECT_CD,DEFECT_LINE,FINISH_DATE,COMPENT,REMARK,STATUS,
COIL_THK,COIL_WTH,SURFACE_CD,GRADE,DEFFECT_KIND,END_USER,SPEC,TYPE,ATTACHMENT) VALUES(@EMP_ID,@CLAIM_NO,@CLAIM_DATE,@CUSTOMER_ID,@COIL_NO,@CLAIM_WGT,@NET_WGT,
@VISIT_DATE,@DEFECT_CD,@DEFECT_LINE,@FINISH_DATE,@COMPENT,@REMARK,@STATUS,@COIL_THK,@COIL_WTH,@SURFACE_CD ,@GRADE,@DEFFECT_KIND,@END_USER,@SPEC,@TYPE,@ATTACHMENT)");
            return DBManager<CLAIM>.Execute(sql, new
            {
                EMP_ID = EMP_ID,
                CLAIM_NO = CLAIM_NO,
                CLAIM_DATE = CLAIM_DATE,
                CUSTOMER_ID = CUSTOMER_ID,
                COIL_NO = COIL_NO,
                CLAIM_WGT = CLAIM_WGT,
                NET_WGT = NET_WGT,
                VISIT_DATE = VISIT_DATE,
                DEFECT_CD = DEFECT_CD,
                DEFECT_LINE = DEFECT_LINE,
                FINISH_DATE = FINISH_DATE,
                COMPENT = COMPENT,
                REMARK = REMARK,
                STATUS = STATUS,
                COIL_THK = COIL_THK,
                COIL_WTH = COIL_WTH,
              DEFFECT_KIND=DEFFECT_KIND,
                SURFACE_CD = SURFACE_CD,
                END_USER=END_USER,
                GRADE = GRADE,
                SPEC=SPEC,
                TYPE=TYPE,
                ATTACHMENT = ATTACHMENT
            });
        }

        public virtual int Update(string EMP_ID, string CLAIM_NO, DateTime? CLAIM_DATE, string CUSTOMER_ID, string COIL_NO, decimal CLAIM_WGT, 
            decimal NET_WGT, DateTime? VISIT_DATE, string DEFECT_CD, string DEFECT_LINE, DateTime? FINISH_DATE, decimal COMPENT, string REMARK, 
            string STATUS, decimal COIL_THK, decimal COIL_WTH, string STS_ST_CLS, string SURFACE_CD, string GRADE, string DEFFECT_KIND, 
            string END_USER, string SPEC, string TYPE, string ATTACHMENT="")
        {
            var sql =string.Format(@"UPDATE CLAIM SET EMP_ID=@EMP_ID,TYPE=@TYPE,CLAIM_DATE=@CLAIM_DATE,CUSTOMER_ID=@CUSTOMER_ID,COIL_NO=@COIL_NO,
CLAIM_WGT=@CLAIM_WGT,NET_WGT=@NET_WGT,VISIT_DATE=@VISIT_DATE,DEFECT_CD=@DEFECT_CD,DEFECT_LINE=@DEFECT_LINE,FINISH_DATE=@FINISH_DATE,COMPENT=@COMPENT,
REMARK=@REMARK,STATUS=@STATUS,COIL_THK=@COIL_THK,COIL_WTH=@COIL_WTH,STS_ST_CLS=@STS_ST_CLS,SURFACE_CD=@SURFACE_CD,GRADE=@GRADE,
DEFFECT_KIND=@DEFFECT_KIND,END_USER=@END_USER,SPEC=@SPEC,ATTACHMENT=@ATTACHMENT WHERE CLAIM_NO=@CLAIM_NO");

            return DBManager<CLAIM>.Execute(sql, new
            {
                EMP_ID = EMP_ID,
                CLAIM_NO = CLAIM_NO,
                CLAIM_DATE = CLAIM_DATE,
                CUSTOMER_ID = CUSTOMER_ID,
                COIL_NO = COIL_NO,
                CLAIM_WGT = CLAIM_WGT,
                NET_WGT = NET_WGT,
                VISIT_DATE = VISIT_DATE,
                DEFECT_CD = DEFECT_CD,
                DEFECT_LINE = DEFECT_LINE,
                FINISH_DATE = FINISH_DATE,
                COMPENT = COMPENT,
                REMARK = REMARK,
                STATUS = STATUS,
                COIL_THK = COIL_THK,
                COIL_WTH = COIL_WTH,
                STS_ST_CLS = STS_ST_CLS,
                SURFACE_CD = SURFACE_CD,
                GRADE = GRADE,
                DEFFECT_KIND = DEFFECT_KIND,
                END_USER=END_USER,
                SPEC=SPEC,
                TYPE=TYPE,
                ATTACHMENT = ATTACHMENT
            });
        }

        public virtual int Delete(string ID = "")
        {
            var sql = "DELETE FROM CLAIM ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<CLAIM>.Execute(sql);
            sql += " WHERE CLAIM_NO=@ID ";
            return DBManager<CLAIM>.Execute(sql, new { ID = ID });
        }

        public string GenerateId()
        {
            string id = DateTime.Now.ToString("yyMMdd");
            string sql = "select top 1 CLAIM_NO  from CLAIM WHERE CLAIM_NO LIKE @id + '%' order by CLAIM_NO desc";

            CLAIM claim = DBManager<CLAIM>.ExecuteReader(sql, new { id = id }).FirstOrDefault();
            if (claim == null)
            {
                id = id + "0001";
            }
            else
            {
                string str = claim.CLAIM_NO.Trim().Substring(6);
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
