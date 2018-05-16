using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VstCustomer
{
    public class EMP_CUST
    {
        public string EMP_ID { get; set; }
        public string CUST_ID { get; set; }

        public EMP_CUST(string EMP_ID, string CUST_ID)
        {
            this.EMP_ID = EMP_ID;
            this.CUST_ID = CUST_ID;
        }
        public EMP_CUST() { }

        public virtual List<EMP_CUST> Select(int ID=0)
        {
            var sql = "SELECT * FROM EMP_CUST ";
            if (ID == 0) return DBManager<EMP_CUST>.ExecuteReader(sql);
            sql +=" WHERE ID=@ID";

            return DBManager<EMP_CUST>.ExecuteReader(sql, new { ID = ID});
        }

        public virtual List<EMP_CUST> SelectPaging(int start=0, int end=10)
        {
            var sql = "SELECT * FROM(SELECT ROW_NUMBER() OVER (order by id) AS ROWNUM, * FROM EMP_CUST) as u  WHERE   RowNum >= @start   AND RowNum < @end ORDER BY RowNum;";

            return DBManager<EMP_CUST>.ExecuteReader(sql, new { start=start, end = end});
        }

        public virtual int GetCount()
        {
            var sql = "SELECT COUNT(1) AS CNT FROM EMP_CUST;";
            return (int) DBManager<EMP_CUST>.ExecuteScalar(sql);
        }

        public virtual int Insert(string EMP_ID,string CUST_ID)
        {
            var sql = "INSERT INTO EMP_CUST(EMP_ID,CUST_ID) VALUES(@EMP_ID,@CUST_ID)";
            return DBManager<EMP_CUST>.Execute(sql, new { EMP_ID = EMP_ID,CUST_ID = CUST_ID});
        }

        public virtual int Update(string EMP_ID, string CUST_ID)
        {
            var sql = "UPDATE EMP_CUST SET EMP_ID=@EMP_ID,CUST_ID=@CUST_ID WHERE EMP_ID=@EMP_ID AND CUST_ID=@CUST";

            return DBManager<EMP_CUST>.Execute(sql,  new { EMP_ID = EMP_ID,CUST_ID = CUST_ID});
        }

        public virtual int Delete(string ID="")
        {
            var sql = "DELETE FROM EMP_CUST ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<EMP_CUST>.Execute(sql);
            sql += " WHERE EMP_ID=@ID ";
            return DBManager<EMP_CUST>.Execute(sql, new { ID = ID });
        }
        public virtual int DeleteList(string ID , List<string> CUS)
        {
            var str = "(";
            foreach (var item in CUS)
            {
                str += "'" + item + "',";
            }
            var output = str.Remove(str.Length - 1, 1) + ")";
            var sql = "DELETE FROM EMP_CUST WHERE EMP_ID=@ID AND CUST_ID IN "+output;
         
            return DBManager<EMP_CUST>.Execute(sql, new { ID = ID });
        }

    }

}
