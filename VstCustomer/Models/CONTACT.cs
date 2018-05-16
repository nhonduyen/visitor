using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VstCustomer
{
    public class CONTACT
    {
        public string ID { get; set; }
        public string CUS_ID { get; set; }
        public string NAME { get; set; }
        public string DOB { get; set; }
        public string POSITION { get; set; }
        public string MOBILE { get; set; }
        public string EMAIL { get; set; }
        public string SEX { get; set; }



        public CONTACT() { }

        public virtual List<CONTACT> Select(string ID = "")
        {
            var sql = "SELECT * FROM CONTACT ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<CONTACT>.ExecuteReader(sql);
            sql += " WHERE ID=@ID";

            return DBManager<CONTACT>.ExecuteReader(sql, new { ID = ID });
        }

        public virtual List<CONTACT> SelectCusContact(string ID)
        {
            var sql = "SELECT * FROM CONTACT WHERE CUS_ID=@ID";

            return DBManager<CONTACT>.ExecuteReader(sql, new { ID = ID });
        }

        public virtual List<CONTACT> SelectPaging(int start = 0, int end = 10)
        {
            var sql = "SELECT * FROM(SELECT ROW_NUMBER() OVER (order by id) AS ROWNUM, * FROM CONTACT) as u  WHERE   RowNum >= @start   AND RowNum < @end ORDER BY RowNum;";

            return DBManager<CONTACT>.ExecuteReader(sql, new { start = start, end = end });
        }

        public virtual int GetCount()
        {
            var sql = "SELECT COUNT(1) AS CNT FROM CONTACT;";
            return (int)DBManager<CUSTOMER>.ExecuteScalar(sql);
        }

        public virtual int Insert(string CUS_ID, string NAME, string DOB, string POSITION, string MOBILE, string EMAIL, string SEX)
        {
            var CONT_ID = GenerateId();
            var sql = "INSERT INTO CONTACT(ID,CUS_ID,NAME,DOB,POSITION,MOBILE,EMAIL,SEX) VALUES(@ID,@CUS_ID,@NAME,@DOB,@POSITION,@MOBILE,@EMAIL,@SEX)";
            return DBManager<CUSTOMER>.Execute(sql, new
            {
                ID = CONT_ID,
                CUS_ID = CUS_ID,
                NAME = NAME,
                DOB = DOB,
                POSITION = POSITION,
                MOBILE = MOBILE,
                EMAIL = EMAIL,
                SEX = SEX
            });
        }

        public virtual int Update(string ID, string NAME, string DOB, string POSITION, string MOBILE, string EMAIL, string SEX)
        {

            var sql = "UPDATE CONTACT SET NAME=@NAME,DOB=@DOB,POSITION=@POSITION,MOBILE=@MOBILE,EMAIL=@EMAIL,SEX=@SEX WHERE ID=@ID";
            return DBManager<CUSTOMER>.Execute(sql, new
            {
                ID = ID,
                NAME = NAME,
                DOB = DOB,
                POSITION = POSITION,
                MOBILE = MOBILE,
                EMAIL = EMAIL,
                SEX = SEX
            });
        }

        public virtual int DeleteContactByCusId(string ID = "")
        {
            var sql = "DELETE FROM CONTACT ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<CONTACT>.Execute(sql);
            sql += " WHERE CUS_ID=@ID ";
            return DBManager<CONTACT>.Execute(sql, new { ID = ID });
        }
        public virtual int Delete(string ID = "")
        {
            var sql = "DELETE FROM CONTACT ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<CONTACT>.Execute(sql);
            sql += " WHERE ID=@ID ";
            return DBManager<CONTACT>.Execute(sql, new { ID = ID });
        }
        public string GenerateId()
        {
            string id = DateTime.Now.ToString("yyMMdd");
            string sql = "select top 1 ID  from CONTACT WHERE ID LIKE @id + '%' order by ID desc";

            CONTACT cont = DBManager<CONTACT>.ExecuteReader(sql, new { id = id }).FirstOrDefault();
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

    }
}