using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace VstCustomer
{
    public class VIST_CONTACTOR
    {
        public string EMP_ID { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string CUST_CONTACTOR { get; set; }
        public DateTime? CONTACT_DATE { get; set; }
        public string CUST_VIST_TYPE { get; set; }
        public string CUST_VIST_PURPOSE { get; set; }
        public string VIST_REMARK { get; set; }
        public string ID { get; set; }

        public VIST_CONTACTOR(string EMP_ID, string CUSTOMER_ID, string CUST_CONTACTOR, DateTime CONTACT_DATE, string CUST_VIST_TYPE, string CUST_VIST_PURPOSE, string VIST_REMARK, string ID)
        {
            this.EMP_ID = EMP_ID;
            this.CUSTOMER_ID = CUSTOMER_ID;
            this.CUST_CONTACTOR = CUST_CONTACTOR;
            this.CONTACT_DATE = CONTACT_DATE;
            this.CUST_VIST_TYPE = CUST_VIST_TYPE;
            this.CUST_VIST_PURPOSE = CUST_VIST_PURPOSE;
            this.VIST_REMARK = VIST_REMARK;
            this.ID = ID;
        }
        public VIST_CONTACTOR() { }

        public List<string> Select(string EMP_ID, string from, string to)
        {
            var sql = string.Format(@"SELECT v.*,c.NAME,E.EMP_NAME as E_NAME FROM VIST_CONTACTOR as v inner join CUSTOMER as c 
on c.ID=v.CUSTOMER_ID inner join employee as e on e.EMP_ID=v.EMP_ID WHERE e.EMP_ID=@EMP_ID AND contact_date between @from and @to");
            List<string> lst = new List<string>();
            var t = DBManager<VIST_CONTACTOR>.ExecuteDynamic(sql, new
            {
                EMP_ID = EMP_ID,
                from = from,
                to = to
            });
            foreach (var item in t)
            {
                var k = "<tr>"+
                                "<td>"+
                                    "<input type='checkbox' class='ckb' id='"+item.ID+"' /></td>"+
                                "<td>" + item.CUSTOMER_ID + "</td>" +
                                "<td>" + item.NAME + "</td>" +
                                "<td>" + item.CUST_VIST_PURPOSE + "</td>" +
                                "<td>" + item.E_NAME + "</td>" +
                                "<td>" + item.VIST_REMARK + "</td>" +
                                "<td>" + item.CUST_VIST_TYPE + "</td>" +
                                "<td>" + item.CONTACT_DATE.ToString("yyyy-MM-dd") + "</td>" +
                            "</tr>";
                lst.Add(k);
            }
            return lst;
        }

        public virtual dynamic SelectPaging(string from, int start = 0, int end = 10)
        {
            var sql = string.Format(@"SELECT * FROM(SELECT ROW_NUMBER() OVER (order by e.emp_id) AS ROWNUM, 
 e.EMP_ID,e.EMP_NAME,e.EMP_DEPT,(select top 1 VISIT_TARGET from EMP_VISIT as em where em.EMP_ID=e.emp_id AND VISIT_PLAN_MONTH = @from) as tar,
  (select COUNT(*) from VIST_CONTACTOR as v where v.EMP_ID=e.emp_id and contact_date like @from +'%') as result,
  (select COUNT(*) from VIST_CONTACTOR as v where v.EMP_ID=e.emp_id and CUST_VIST_TYPE='DIRECT' and contact_date like @from +'%') as dir,
  (select COUNT(*) from VIST_CONTACTOR as v where v.EMP_ID=e.emp_id and CUST_VIST_TYPE='CALL' and contact_date like @from +'%') as ca,
  (select COUNT(*) from VIST_CONTACTOR as v where v.EMP_ID=e.emp_id and CUST_VIST_TYPE='E-MAIL' and contact_date like @from +'%') as email     
   from EMPLOYEE AS e
) as u  WHERE   RowNum >= @start   AND RowNum < @end ORDER BY RowNum;");

            return DBManager<VIST_CONTACTOR>.ExecuteDynamic(sql, new
            {
                from = from,
               
                start = start,
                end = end
            });
        }

        public List<VIST_CONTACTOR> GetVisit(string ID)
        {
            var sql = "SELECT * FROM VIST_CONTACTOR WHERE ID=@ID";
            return DBManager<VIST_CONTACTOR>.ExecuteReader(sql, new { ID = ID });
        }
        public virtual dynamic GetVisit(string from,string to,string cus_id, string em_id, int start = 0, int end = 10)
        {
            var sql = string.Format(@"SELECT * FROM(SELECT ROW_NUMBER() OVER (order by e.emp_id) AS ROWNUM,v.*,e.EMP_NAME,cus.NAME as CUS_NAME  
   from VIST_CONTACTOR as v inner join customer as cus on v.CUSTOMER_ID=cus.ID  left join employee as e on e.EMP_ID=v.EMP_ID WHERE (@FROM='' OR CONTACT_DATE BETWEEN @FROM AND @TO)
AND (@CUSTOMER_ID='' OR CUSTOMER_ID=@CUSTOMER_ID) AND (@EMP_ID='' OR e.EMP_ID=@EMP_ID)
) as u  WHERE   RowNum >= @start   AND RowNum < @end ORDER BY RowNum;");

            return DBManager<VIST_CONTACTOR>.ExecuteDynamic(sql, new
            {
                FROM = from,
                TO=to,
                EMP_ID=em_id,
                CUSTOMER_ID=cus_id,
                start = start,
                end = end
            });
        }
        public int GetCountVisit(string from="", string to="", string cus_id="", string em_id="")
        {
            var sql = string.Format(@"SELECT COUNT(*) FROM   
VIST_CONTACTOR WHERE (@FROM='' OR CONTACT_DATE BETWEEN @FROM AND @TO)
AND (@CUSTOMER_ID='' OR CUSTOMER_ID=@CUSTOMER_ID) AND (@EMP_ID='' OR EMP_ID=@EMP_ID);");

            return (int)DBManager<VIST_CONTACTOR>.ExecuteScalar(sql, new
            {
                FROM = from,
                TO = to,
                EMP_ID = em_id,
                CUSTOMER_ID = cus_id
            });
        }
        public virtual int GetCount(string from = "")
        {
            var sql = "SELECT COUNT(1) AS CNT FROM VIST_CONTACTOR";
            if (string.IsNullOrWhiteSpace(from))
                return (int)DBManager<VIST_CONTACTOR>.ExecuteScalar(sql);
            sql += " where contact_date like @from +'%'";
            return (int)DBManager<VIST_CONTACTOR>.ExecuteScalar(sql, new
            {
                from = from
            });
        }

        public virtual int Insert(string EMP_ID, string CUSTOMER_ID, string CUST_CONTACTOR, DateTime? CONTACT_DATE, string CUST_VIST_TYPE, string CUST_VIST_PURPOSE, string VIST_REMARK)
        {
            var ID = GenerateId();
            var sql = "INSERT INTO VIST_CONTACTOR(ID,EMP_ID,CUSTOMER_ID,CUST_CONTACTOR,CONTACT_DATE,CUST_VIST_TYPE,CUST_VIST_PURPOSE,VIST_REMARK) VALUES(@ID,@EMP_ID,@CUSTOMER_ID,@CUST_CONTACTOR,@CONTACT_DATE,@CUST_VIST_TYPE,@CUST_VIST_PURPOSE,@VIST_REMARK)";
            return DBManager<VIST_CONTACTOR>.Execute(sql, new
            {
                ID = ID,
                EMP_ID = EMP_ID,
                CUSTOMER_ID = CUSTOMER_ID,
                CUST_CONTACTOR = CUST_CONTACTOR,
                CONTACT_DATE = CONTACT_DATE,
                CUST_VIST_TYPE = CUST_VIST_TYPE,
                CUST_VIST_PURPOSE = CUST_VIST_PURPOSE,
                VIST_REMARK = VIST_REMARK
            });
        }

        public virtual int Update(string EMP_ID, string CUSTOMER_ID, string CUST_CONTACTOR, DateTime? CONTACT_DATE, string CUST_VIST_TYPE, string CUST_VIST_PURPOSE, string VIST_REMARK, string ID)
        {
            var sql = "UPDATE VIST_CONTACTOR SET EMP_ID=@EMP_ID,CUSTOMER_ID=@CUSTOMER_ID,CUST_CONTACTOR=@CUST_CONTACTOR,CONTACT_DATE=@CONTACT_DATE,CUST_VIST_TYPE=@CUST_VIST_TYPE,CUST_VIST_PURPOSE=@CUST_VIST_PURPOSE,VIST_REMARK=@VIST_REMARK WHERE ID=@ID";

            return DBManager<VIST_CONTACTOR>.Execute(sql, new
            {
                EMP_ID = EMP_ID,
                CUSTOMER_ID = CUSTOMER_ID,
                CUST_CONTACTOR = CUST_CONTACTOR,
                CONTACT_DATE = CONTACT_DATE,
                CUST_VIST_TYPE = CUST_VIST_TYPE,
                CUST_VIST_PURPOSE = CUST_VIST_PURPOSE,
                VIST_REMARK = VIST_REMARK,
                ID = ID
            });
        }

        public virtual int Delete(string ID = "")
        {
            var sql = "DELETE FROM VIST_CONTACTOR ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<VIST_CONTACTOR>.Execute(sql);
            sql += " WHERE ID=@ID ";
            return DBManager<VIST_CONTACTOR>.Execute(sql, new { ID = ID });
        }

        public string GenerateId()
        {
            string id = DateTime.Now.ToString("yyMMdd");
            string sql = "select top 1 ID  from VIST_CONTACTOR WHERE ID LIKE @id + '%' order by ID desc";

            VIST_CONTACTOR cont = DBManager<VIST_CONTACTOR>.ExecuteReader(sql, new { id = id }).FirstOrDefault();
            if (cont == null)
            {
                id = id + "0001";
            }
            else
            {
                string str = cont.ID.Trim().Substring(6);
                string num = (Convert.ToInt32(str) + 1).ToString();
                for (int i = 0; i < str.Length - num.Length; i++)
                {
                    id += "0";
                }
                id += num.ToString();
            }
            return id;
        }
        public DataTable Export(string from, string to, string cus_id, string em_id)
        {
            var sql = string.Format(@"SELECT v.*,e.EMP_NAME,cus.NAME as CUS_NAME  
   from VIST_CONTACTOR as v inner join customer as cus on v.CUSTOMER_ID=cus.ID  left join employee as e on e.EMP_ID=v.EMP_ID WHERE (@FROM='' OR CONTACT_DATE BETWEEN @FROM AND @TO)
AND (@CUSTOMER_ID='' OR CUSTOMER_ID=@CUSTOMER_ID) AND (@EMP_ID='' OR e.EMP_ID=@EMP_ID)");
            DataTable dtb = new DataTable();
            var result = DBManager<VIST_CONTACTOR>.ExecuteDynamic(sql, new
            {
                FROM = from,
                TO = to,
                EMP_ID = em_id,
                CUSTOMER_ID = cus_id
            });
            dtb.Clear();

            dtb.Columns.Add("DATE");
            dtb.Columns.Add("CUSTOMER_ID");
            dtb.Columns.Add("CUS_NAME");
            dtb.Columns.Add("VISIT_TYPE");
            dtb.Columns.Add("PURPOSE");
            dtb.Columns.Add("CONTENT");
            dtb.Columns.Add("EMPLOYEE");
            foreach (var item in result)
            {
                DataRow r = dtb.NewRow();
                r["DATE"] = item.CONTACT_DATE == null ? "" : item.CONTACT_DATE.ToString("yyyy-MM-dd");
                r["CUSTOMER_ID"] = item.CUSTOMER_ID;
                r["CUS_NAME"] = item.CUS_NAME;
                r["VISIT_TYPE"] = item.CUST_VIST_TYPE;
                r["PURPOSE"] = item.CUST_VIST_PURPOSE;
                r["CONTENT"] = item.VIST_REMARK;
                r["EMPLOYEE"] = item.EMP_NAME;
               
                dtb.Rows.Add(r);
            }
            return dtb;
        }
    }

}
