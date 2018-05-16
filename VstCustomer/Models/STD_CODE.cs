using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VstCustomer
{
    public class STD_CODE
    {
        public string STD_CD_KIND { get; set; }
        public string STD_CD_NAME { get; set; }

        public STD_CODE(string STD_CD_KIND, string STD_CD_NAME)
        {
            this.STD_CD_KIND = STD_CD_KIND;
            this.STD_CD_NAME = STD_CD_NAME;
        }
        public STD_CODE() { }

        public virtual List<STD_CODE> Select(string STD_CD_KIND="")
        {
            var sql = "SELECT * FROM STD_CODE ";
            if (string.IsNullOrWhiteSpace(STD_CD_KIND)) return DBManager<STD_CODE>.ExecuteReader(sql);
            sql += " WHERE STD_CD_KIND=@STD_CD_KIND";

            return DBManager<STD_CODE>.ExecuteReader(sql, new { STD_CD_KIND = STD_CD_KIND });
        }

        public virtual List<STD_CODE> SelectPaging(int start=0, int end=10)
        {
            var sql = "SELECT * FROM(SELECT ROW_NUMBER() OVER (order by id) AS ROWNUM, * FROM STD_CODE) as u  WHERE   RowNum >= @start   AND RowNum < @end ORDER BY RowNum;";

            return DBManager<STD_CODE>.ExecuteReader(sql, new { start=start, end = end});
        }

        public virtual int GetCount()
        {
            var sql = "SELECT COUNT(1) AS CNT FROM STD_CODE;";
            return (int) DBManager<STD_CODE>.ExecuteScalar(sql);
        }

        public virtual int Insert(string STD_CD_KIND,string STD_CD_NAME)
        {
            var sql = "INSERT INTO STD_CODE(STD_CD_KIND,STD_CD_NAME) VALUES(@STD_CD_KIND,@STD_CD_NAME)";
            return DBManager<STD_CODE>.Execute(sql, new { STD_CD_KIND = STD_CD_KIND,STD_CD_NAME = STD_CD_NAME});
        }

        public virtual int Update(string STD_CD_KIND, string STD_CD_NAME)
        {
            var sql = "UPDATE STD_CODE SET STD_CD_KIND=@STD_CD_KIND,STD_CD_NAME=@STD_CD_NAME WHERE ID=@ID";

            return DBManager<STD_CODE>.Execute(sql,  new { STD_CD_KIND = STD_CD_KIND,STD_CD_NAME = STD_CD_NAME});
        }

        public virtual int Delete(int ID=0)
        {
            var sql = "DELETE FROM STD_CODE ";
            if (ID == 0) return DBManager<STD_CODE>.Execute(sql);
            sql += " WHERE ID=@ID ";
            return DBManager<STD_CODE>.Execute(sql, new { ID = ID });
        }


    }

}
