using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VstCustomer
{
    public class END_USER
    {
        public string CUS_ID { get; set; }
        public string END_USER_ID { get; set; }
        public string NAME { get; set; }

        public END_USER()
        {
        }


        public int Insert(string CUS_ID, string END_USER_ID, string NAME)
        {
            var sql = "INSERT INTO END_USER(CUS_ID, END_USER_ID,NAME) VALUES(@CUS_ID, @END_USER_ID,@NAME)";
            return DBManager<END_USER>.Execute(sql, new
           {
               CUS_ID = CUS_ID,
               END_USER_ID = END_USER_ID,
               NAME = NAME
           });
        }

        public List<END_USER> GetEndUser(string CUS_ID, string END_USER_ID)
        {
            var sql = "SELECT * FROM END_USER WHERE CUS_ID=@CUS_ID AND END_USER_ID=@END_USER_ID";
            return DBManager<END_USER>.ExecuteReader(sql, new
            {
                CUS_ID = CUS_ID,
                END_USER_ID = END_USER_ID
            });
        }
    }
}