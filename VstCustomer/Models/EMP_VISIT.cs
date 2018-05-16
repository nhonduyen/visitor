using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VstCustomer
{
    public class EMP_VISIT
    {
        public string EMP_ID { get; set; }
        public string VISIT_PLAN_MONTH { get; set; }
        public int VISIT_TARGET { get; set; }
        public int VISIT_RESULT { get; set; }
        public string ID { get; set; }

        public EMP_VISIT(string EMP_ID, string VISIT_PLAN_MONTH, int VISIT_TARGET, int VISIT_RESULT, string ID)
        {
            this.EMP_ID = EMP_ID;
            this.VISIT_PLAN_MONTH = VISIT_PLAN_MONTH;
            this.VISIT_TARGET = VISIT_TARGET;
            this.VISIT_RESULT = VISIT_RESULT;
            this.ID = ID;
        }
        public EMP_VISIT() { }

        public virtual List<EMP_VISIT> Select(int ID = 0)
        {
            var sql = "SELECT * FROM EMP_VISIT ";
            if (ID == 0) return DBManager<EMP_VISIT>.ExecuteReader(sql);
            sql += " WHERE ID=@ID";

            return DBManager<EMP_VISIT>.ExecuteReader(sql, new { ID = ID });
        }

        public bool CheckExist(string EMP_ID, string DATE)
        {
            var sql = "SELECT EMP_ID FROM EMP_VISIT WHERE EMP_ID=@EMP_ID AND VISIT_PLAN_MONTH LIKE @DATE +'%'";

            var t = DBManager<EMP_VISIT>.ExecuteReader(sql, new { EMP_ID = EMP_ID, DATE = DATE }).FirstOrDefault();
            return t == null;
        }

        public string GetTarget(string ID, string DATE)
        {
            var sql = "SELECT VISIT_TARGET FROM EMP_VISIT WHERE EMP_ID=@ID AND VISIT_PLAN_MONTH LIKE @DATE +'%'";
            List<string> target = new List<string>();
            var t = DBManager<EMP_VISIT>.ExecuteReader(sql, new { ID = ID, DATE = DATE });
            foreach (var item in t)
            {
                return item.VISIT_TARGET.ToString();
            }
          
            return string.Empty;
        }

        public virtual List<EMP_VISIT> SelectPaging(int start = 0, int end = 10)
        {
            var sql = "SELECT * FROM(SELECT ROW_NUMBER() OVER (order by id) AS ROWNUM, * FROM EMP_VISIT) as u  WHERE   RowNum >= @start   AND RowNum < @end ORDER BY RowNum;";

            return DBManager<EMP_VISIT>.ExecuteReader(sql, new { start = start, end = end });
        }

        public virtual int GetCount()
        {
            var sql = "SELECT COUNT(1) AS CNT FROM EMP_VISIT;";
            return (int)DBManager<EMP_VISIT>.ExecuteScalar(sql);
        }

        public virtual int Insert(string EMP_ID, string VISIT_PLAN_MONTH, int VISIT_TARGET, int VISIT_RESULT)
        {
            var sql = "INSERT INTO EMP_VISIT(EMP_ID,VISIT_PLAN_MONTH,VISIT_TARGET,VISIT_RESULT) VALUES(@EMP_ID,@VISIT_PLAN_MONTH,@VISIT_TARGET,@VISIT_RESULT)";
            return DBManager<EMP_VISIT>.Execute(sql, new { EMP_ID = EMP_ID, VISIT_PLAN_MONTH = VISIT_PLAN_MONTH, VISIT_TARGET = VISIT_TARGET, VISIT_RESULT = VISIT_RESULT });
        }

        public virtual int Update(string EMP_ID, string VISIT_PLAN_MONTH, int VISIT_TARGET)
        {
            var sql = "UPDATE EMP_VISIT SET EMP_ID=@EMP_ID,VISIT_PLAN_MONTH=@VISIT_PLAN_MONTH,VISIT_TARGET=@VISIT_TARGET WHERE EMP_ID=@EMP_ID AND VISIT_PLAN_MONTH=@VISIT_PLAN_MONTH";

            return DBManager<EMP_VISIT>.Execute(sql, new
            {
                EMP_ID = EMP_ID,
                VISIT_PLAN_MONTH = VISIT_PLAN_MONTH,
                VISIT_TARGET = VISIT_TARGET
            });
        }

        public virtual int Delete(int ID = 0)
        {
            var sql = "DELETE FROM EMP_VISIT ";
            if (ID == 0) return DBManager<EMP_VISIT>.Execute(sql);
            sql += " WHERE ID=@ID ";
            return DBManager<EMP_VISIT>.Execute(sql, new { ID = ID });
        }


    }

}
