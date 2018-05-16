using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VstCustomer
{
    public class EMPLOYEE
    {
        public string EMP_ID { get; set; }
        public string EMP_NAME { get; set; }
        public string EMP_DEPT { get; set; }
        public string EMP_EMAIL { get; set; }
        public string EMP_MOBILE { get; set; }
        public string PASSWORD { get; set; }
        public int ROLE { get; set; }

        public EMPLOYEE(string EMP_ID, string EMP_NAME, string EMP_DEPT, string EMP_EMAIL, string EMP_MOBILE, string PASSWORD, int ROLE)
        {
            this.EMP_ID = EMP_ID;
            this.EMP_NAME = EMP_NAME;
            this.EMP_DEPT = EMP_DEPT;
            this.EMP_EMAIL = EMP_EMAIL;
            this.EMP_MOBILE = EMP_MOBILE;
            this.PASSWORD = PASSWORD;
            this.ROLE = ROLE;
        }
        public EMPLOYEE() { }

        public virtual List<EMPLOYEE> SelectSimple()
        {
            var sql = "SELECT EMP_ID,EMP_NAME FROM EMPLOYEE ";
           
            return DBManager<EMPLOYEE>.ExecuteReader(sql);
        }

        public virtual List<EMPLOYEE> Select(string ID = "")
        {
            var sql = "SELECT * FROM EMPLOYEE ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<EMPLOYEE>.ExecuteReader(sql);
            sql += " WHERE EMP_ID=@ID";

            return DBManager<EMPLOYEE>.ExecuteReader(sql, new { ID = ID });
        }

        public virtual dynamic SelectPaging(int start = 0, int end = 10, string key = "")
        {
            var sql =string.Format(@"SELECT * FROM(SELECT ROW_NUMBER() OVER (order by EMP_ID) AS ROWNUM, *,
            (SELECT TOP 1 ID FROM CUSTOMER WHERE PERSON=E.EMP_ID) AS C_ID,
            (SELECT TOP 1 NAME FROM CUSTOMER WHERE PERSON=E.EMP_ID) AS CUST_NAME 
            FROM EMPLOYEE AS E WHERE @key='' OR @key IS NULL OR EMP_ID LIKE @key +'%' OR EMP_NAME LIKE '%'+@key +'%' ) as u  WHERE   RowNum >= @start 
            AND RowNum < @end ORDER BY RowNum;");

            return DBManager<EMPLOYEE>.ExecuteDynamic(sql, new { start = start, end = end, key = key });
        }

        public virtual int GetCount(string key = "")
        {
            var sql = "SELECT COUNT(1) AS CNT FROM EMPLOYEE WHERE @key='' OR @key IS NULL OR EMP_ID LIKE @key +'%' OR EMP_NAME LIKE '%'+@key +'%';";
            return (int)DBManager<EMPLOYEE>.ExecuteScalar(sql, new { key = key });
        }

        public virtual int Insert(string EMP_ID, string EMP_NAME, string EMP_DEPT, string EMP_EMAIL, string EMP_MOBILE, string PASSWORD, int ROLE)
        {
            PASSWORD = this.Encode(PASSWORD);
            var sql = "INSERT INTO EMPLOYEE(EMP_ID,EMP_NAME,EMP_DEPT,EMP_EMAIL,EMP_MOBILE,PASSWORD,ROLE) VALUES(@EMP_ID,@EMP_NAME,@EMP_DEPT,@EMP_EMAIL,@EMP_MOBILE,@PASSWORD,@ROLE)";
            return DBManager<EMPLOYEE>.Execute(sql, new
            {
                EMP_ID = EMP_ID,
                EMP_NAME = EMP_NAME,
                EMP_DEPT = EMP_DEPT,
                EMP_EMAIL = EMP_EMAIL,
                EMP_MOBILE = EMP_MOBILE,
                PASSWORD = PASSWORD,
                ROLE = ROLE
            });
        }

        public virtual int Update(string EMP_ID, string EMP_NAME, string EMP_DEPT, string EMP_EMAIL, string EMP_MOBILE, int ROLE)
        {
            var sql = "UPDATE EMPLOYEE SET EMP_ID=@EMP_ID,EMP_NAME=@EMP_NAME,EMP_DEPT=@EMP_DEPT,EMP_EMAIL=@EMP_EMAIL,EMP_MOBILE=@EMP_MOBILE,ROLE=@ROLE WHERE EMP_ID=@EMP_ID";

            return DBManager<EMPLOYEE>.Execute(sql, new
            {
                EMP_ID = EMP_ID,
                EMP_NAME = EMP_NAME,
                EMP_DEPT = EMP_DEPT,
                EMP_EMAIL = EMP_EMAIL,
                EMP_MOBILE = EMP_MOBILE,
                ROLE = ROLE
            });
        }

        public virtual int Delete(string ID = "")
        {
            var sql = "DELETE FROM EMPLOYEE ";
            if (string.IsNullOrWhiteSpace(ID)) return DBManager<EMPLOYEE>.Execute(sql);
            sql += " WHERE EMP_ID=@ID ";
            return DBManager<EMPLOYEE>.Execute(sql, new { ID = ID });
        }

        public string Encode(string value)
        {
            var hash = System.Security.Cryptography.SHA1.Create();
            var encoder = new System.Text.ASCIIEncoding();
            var combined = encoder.GetBytes(value ?? "");
            return BitConverter.ToString(hash.ComputeHash(combined)).ToLower().Replace("-", "");
        }
        public bool Login(string EMP_ID, string Password)
        {
            var sql = "SELECT EMP_ID,EMP_NAME,EMP_DEPT,ROLE FROM EMPLOYEE WHERE EMP_ID=@EMP_ID AND PASSWORD=@PASSWORD";
            EMPLOYEE employee = DBManager<EMPLOYEE>.ExecuteReader(sql, new { EMP_ID = EMP_ID, PASSWORD = Password }).FirstOrDefault();
            if (employee == null)
                return false;
            HttpContext.Current.Session["Username"] = employee.EMP_ID.Trim();
            HttpContext.Current.Session["Name"] = employee.EMP_NAME.Trim();
            HttpContext.Current.Session["Dept"] = employee.EMP_DEPT;
            HttpContext.Current.Session["Role"] = employee.ROLE;
            return true;
        }

        public int ChangePassword(string EmpId, string pass, string newpass)
        {
            var sql = "UPDATE EMPLOYEE SET PASSWORD=@NEWPASS WHERE EMP_ID=@EMP_ID AND PASSWORD=@PASS";
            return DBManager<EMPLOYEE>.Execute(sql, new { NEWPASS = newpass, EMP_ID = EmpId, PASS = pass });
        }
        public int ResetPassword(string EMP_ID)
        {
            var password = this.Encode("123456");
            var sql = "UPDATE EMPLOYEE SET PASSWORD=@PASSWORD WHERE EMP_ID=@EMP_ID";
            return DBManager<EMPLOYEE>.Execute(sql, new { EMP_ID = EMP_ID, PASSWORD=password });
        }

        public List<CUSTOMER> GetCustomerByEmp(string EMP_ID)
        {
            var sql = "SELECT ID,NAME,CLASS FROM CUSTOMER WHERE PERSON=@ID ";
            return DBManager<CUSTOMER>.ExecuteReader(sql, new { ID = EMP_ID });
        }

        public List<EMPLOYEE> GetTeam()
        {
            var sql = "SELECT DISTINCT(EMP_DEPT) FROM EMPLOYEE WHERE EMP_DEPT IS NOT NULL AND EMP_DEPT <>''";
            return DBManager<EMPLOYEE>.ExecuteReader(sql);
        }
    }

}
