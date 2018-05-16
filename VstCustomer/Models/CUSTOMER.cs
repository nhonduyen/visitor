using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VstCustomer
{
    public class CUSTOMER
    {
        public string ID { get; set; }
        public string NAME { get; set; }
        public string ADDRESS { get; set; }
        public string LOCATION { get; set; }
        public string TEL { get; set; }
        public string FAX { get; set; }
        public string ESTABLE { get; set; }
        public string CONTACT { get; set; }
        public string CLASS { get; set; }
        public string APPLICATION { get; set; }
        public string PERSON { get; set; }
        public string SPEC { get; set; }
        public string TYPE { get; set; }

      
        public CUSTOMER() { }

        public virtual List<CUSTOMER> Select(string ID = "")
        {
            var sql = "SELECT * FROM CUSTOMER ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<CUSTOMER>.ExecuteReader(sql);
            sql += " WHERE ID=@ID";

            return DBManager<CUSTOMER>.ExecuteReader(sql, new { ID = ID });
        }
        public List<CUSTOMER> GetSimple()
        {
            var sql = "SELECT ID,NAME FROM CUSTOMER ORDER BY ID";
            return DBManager<CUSTOMER>.ExecuteReader(sql);
        }

        public List<END_USER> GetEndUser(string ID="")
        {
            var sql = "SELECT DISTINCT(END_USER_ID) FROM END_USER";
            if (string.IsNullOrWhiteSpace(ID))
            return DBManager<END_USER>.ExecuteReader(sql);
            sql += " WHERE CUS_ID=@ID";
            return DBManager<END_USER>.ExecuteReader(sql, new { ID=ID });
        }
        public List<END_USER> GetFullEndUser(string ID = "")
        {
            var sql = "SELECT * FROM END_USER";
            if (string.IsNullOrWhiteSpace(ID))
                return DBManager<END_USER>.ExecuteReader(sql);
            sql += " WHERE CUS_ID=@ID";
            return DBManager<END_USER>.ExecuteReader(sql, new { ID = ID });
        }
        public END_USER GetEndUserById(string ID = "")
        {
            var sql = "SELECT TOP 1 * FROM END_USER WHERE END_USER_ID=@ID";
          
            return DBManager<END_USER>.ExecuteReader(sql, new { ID = ID }).FirstOrDefault();
        }

        public dynamic SelectPaging(int start = 0, int end = 10, string key = "")
        {
            var sql =string.Format(@"SELECT * FROM(SELECT ROW_NUMBER() OVER (order by id) AS ROWNUM, CUS.*,EMP_NAME,(SELECT COUNT(*) FROM END_USER WHERE CUS_ID=ID) AS NUM_END FROM CUSTOMER AS CUS
LEFT JOIN EMPLOYEE AS E ON E.EMP_ID=CUS.PERSON
WHERE @key IS NULL OR @key='' OR  ID LIKE @key +'%' OR NAME LIKE '%'+@key+'%') as u  WHERE   RowNum >= @start   AND RowNum < @end ORDER BY RowNum;");

            return DBManager<CUSTOMER>.ExecuteDynamic(sql, new
            {
                start = start,
                end = end,
                key = key
            });
        }


        public virtual int GetCount(string key = "")
        {
            var sql = "SELECT COUNT(1) AS CNT FROM CUSTOMER WHERE @key='' OR @key IS NULL OR ID LIKE @key +'%' OR NAME LIKE '%'+@key+'%' ";
            return (int)DBManager<CUSTOMER>.ExecuteScalar(sql, new { key = key });
        }

        public virtual int Insert(string ID, string NAME, string ADDRESS, string LOCATION, string TEL, string FAX, string ESTABLE, string CONTACT, 
            string CLASS, string APPLICATION, string PERSON, string SPEC, string TYPE)
        {
            var sql = "INSERT INTO CUSTOMER(ID,NAME,ADDRESS,LOCATION,TEL,FAX,ESTABLE,CONTACT,CLASS,APPLICATION,PERSON,SPEC,TYPE) VALUES(@ID,@NAME,@ADDRESS,@LOCATION,@TEL,@FAX,@ESTABLE,@CONTACT,@CLASS,@APPLICATION,@PERSON,@SPEC,@TYPE)";
            return DBManager<CUSTOMER>.Execute(sql, new
            {
                ID = ID,
                NAME = NAME,
                ADDRESS = ADDRESS,
                LOCATION = LOCATION,
                TEL = TEL,
                FAX = FAX,
                ESTABLE = ESTABLE,
                CONTACT = CONTACT,
                CLASS = CLASS,
                APPLICATION = APPLICATION,
                PERSON = PERSON,
                SPEC = SPEC,
                TYPE=TYPE
            });
        }

        public virtual int Update(string ID, string NAME, string ADDRESS, string LOCATION, string TEL, string FAX, string ESTABLE, string CONTACT,
            string CLASS, string APPLICATION, string PERSON, string SPEC, string TYPE)
        {
            var sql = "UPDATE CUSTOMER SET NAME=@NAME,ADDRESS=@ADDRESS,LOCATION=@LOCATION,TEL=@TEL,FAX=@FAX,ESTABLE=@ESTABLE,CONTACT=@CONTACT,CLASS=@CLASS,APPLICATION=@APPLICATION,PERSON=@PERSON,SPEC=@SPEC,TYPE=@TYPE WHERE ID=@ID";

            return DBManager<CUSTOMER>.Execute(sql, new
            {
                ID = ID,
                NAME = NAME,
                ADDRESS = ADDRESS,
                LOCATION = LOCATION,
                TEL = TEL,
                FAX = FAX,
                ESTABLE = ESTABLE,
                CONTACT = CONTACT,
                CLASS = CLASS,
                APPLICATION = APPLICATION,
                PERSON = PERSON,
                SPEC = SPEC,
                TYPE = TYPE
            });
        }

        public virtual int Update(string CUS_ID, string ID)
        {
            var sql = "UPDATE CUSTOMER SET PERSON=@ID WHERE ID=@CUS_ID";

            return DBManager<CUSTOMER>.Execute(sql, new
            {
                ID = ID,
                CUS_ID = CUS_ID
            });
        }

        public virtual int Delete(string ID = "")
        {

            var sql = "DELETE FROM CUSTOMER ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<CUSTOMER>.Execute(sql);
            sql += " WHERE ID=@ID ";
            return DBManager<CUSTOMER>.Execute(sql, new { ID = ID });
        }

        public int ChangeId(string CUS_ID, string OLD_ID)
        {
            var sql1 = "UPDATE CONTACT SET CUS_ID=@CUS_ID WHERE CUS_ID=@OLD_ID";
            var sql2 = "UPDATE CLAIM SET CUSTOMER_ID=@CUS_ID WHERE CUSTOMER_ID=@OLD_ID";
            var sql3 = "UPDATE END_USER SET CUS_ID=@CUS_ID WHERE CUS_ID=@OLD_ID";
            var sql4 = "UPDATE ORDERED SET CUSTOMER_ID=@CUS_ID WHERE CUSTOMER_ID=@OLD_ID";
            var sql5 = "UPDATE VIST_CONTACTOR SET CUSTOMER_ID=@CUS_ID WHERE CUSTOMER_ID=@OLD_ID";
            DBManager<CUSTOMER>.Execute(sql1, new
            {
                CUS_ID = CUS_ID,
                OLD_ID = OLD_ID
            });
            DBManager<CUSTOMER>.Execute(sql2, new
            {
                CUS_ID = CUS_ID,
                OLD_ID = OLD_ID
            });
            DBManager<CUSTOMER>.Execute(sql3, new
            {
                CUS_ID = CUS_ID,
                OLD_ID = OLD_ID
            });
            DBManager<CUSTOMER>.Execute(sql4, new
            {
                CUS_ID = CUS_ID,
                OLD_ID = OLD_ID
            });
            DBManager<CUSTOMER>.Execute(sql5, new
            {
                CUS_ID = CUS_ID,
                OLD_ID = OLD_ID
            });
            var sql = "UPDATE CUSTOMER SET ID=@CUS_ID WHERE ID=@OLD_ID";

            return DBManager<CUSTOMER>.Execute(sql, new
            {
                CUS_ID = CUS_ID,
                OLD_ID = OLD_ID
            });
        }

        public string GenerateId()
        {
            string id = DateTime.Now.ToString("yyMMdd");
            string sql = "select top 1 ID  from CUSTOMER WHERE ID LIKE @id + '%' order by ID desc";

            CUSTOMER claim = DBManager<CUSTOMER>.ExecuteReader(sql, new { id = id }).FirstOrDefault();
            if (claim == null || claim.ID.Trim()==id)
            {
                id = id + "01";
            }
            else
            {
                string str = claim.ID.Trim().Substring(6);
                int num = Convert.ToInt32(str) + 1;
                if (num < 10)
                {
                    id += "0" + num.ToString();
                }
                else
                {
                    id += num.ToString();
                }
            }
            return id;
        }

    }

}
