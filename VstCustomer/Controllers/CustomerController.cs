using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VstCustomer.Controllers
{
    public class CustomerController : Controller
    {
        //
        // GET: /Customer/

        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult GetCustomer(DataTableParameters dataTableParameters)
        {
            CUSTOMER cus = new CUSTOMER();

            var resultSet = new DataTableResultSet();
            resultSet.draw = dataTableParameters.Draw;
            var lst = cus.SelectPaging(dataTableParameters.Start + 1, 
                dataTableParameters.Start + dataTableParameters.Length + 1,dataTableParameters.Search.Value);
            resultSet.recordsTotal = resultSet.recordsFiltered = cus.GetCount(dataTableParameters.Search.Value);

            foreach (var i in lst)
            {
                var columns = new List<string>();
                columns.Add("<input type='checkbox' class='ckb' id='"+i.ID+"' />");
                columns.Add(i.ID);
                columns.Add(i.NAME);
                columns.Add(i.LOCATION == null ? "" : i.LOCATION.Trim());
                columns.Add(i.EMP_NAME == null ? "" : i.EMP_NAME);
                columns.Add(i.TYPE == null ? "": i.TYPE);
                columns.Add("<a href='#' class='end' title='End User' id='" + i.ID + "'><span class='badge badge-pill badge-primary'>" + i.NUM_END + "</span></a>");
                resultSet.data.Add(columns);

            }
            return Json(resultSet);

        }
        [HttpPost]
        public JsonResult InsertUpdateCustomer(CUSTOMER CUS, List<CONTACT> CONTACTS, int ACTION)
        {
            CUSTOMER cus = new CUSTOMER();
            CONTACT con = new CONTACT();
            var result = 0;
            if (ACTION == 1)
            {
                result = cus.Update(CUS.ID, CUS.NAME, CUS.ADDRESS, CUS.LOCATION, CUS.TEL, CUS.FAX, CUS.ESTABLE, CUS.CONTACT,
                   CUS.CLASS, CUS.APPLICATION, CUS.PERSON, CUS.SPEC, CUS.TYPE);
                foreach (var contact in CONTACTS)
                {
                    if (string.IsNullOrWhiteSpace(contact.ID))
                    {
                        con.Insert(CUS.ID, contact.NAME, contact.DOB, contact.POSITION, contact.MOBILE, contact.EMAIL, contact.SEX);
                    }
                    else
                    {
                        con.Update(contact.ID, contact.NAME, contact.DOB, contact.POSITION, contact.MOBILE, contact.EMAIL, contact.SEX);
                    }
                }
            }
            else
            {
                CUSTOMER checkExist = cus.Select(CUS.ID).FirstOrDefault();
                if (checkExist != null)
                    return Json(-1);
                result = cus.Insert(CUS.ID, CUS.NAME, CUS.ADDRESS, CUS.LOCATION, CUS.TEL, CUS.FAX, CUS.ESTABLE, CUS.CONTACT,
                    CUS.CLASS, CUS.APPLICATION, CUS.PERSON, CUS.SPEC, CUS.TYPE);
                foreach (var contact in CONTACTS)
                {
                    con.Insert(CUS.ID, contact.NAME, contact.DOB, contact.POSITION, contact.MOBILE, contact.EMAIL, contact.SEX);
                }
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetCustomerById(string ID)
        {
            CUSTOMER cus = new CUSTOMER();
            List<CUSTOMER> lst = cus.Select(ID);
            return Json(lst);
        }

        [HttpPost]
        public JsonResult GetContactByCusId(string ID)
        {
            CONTACT con = new CONTACT();
            List<CONTACT> lst = con.SelectCusContact(ID);
            return Json(lst);
        }

        [HttpPost]
        public JsonResult Delete(string ID)
        {
            CONTACT con = new CONTACT();
            CUSTOMER cus = new CUSTOMER();
            con.DeleteContactByCusId(ID);
            var result = cus.Delete(ID);
            return Json(result);
        }

        [HttpPost]
        public JsonResult DeleteContact(string ID)
        {
            CONTACT con = new CONTACT();
          
            var result = con.Delete(ID);
            return Json(result);
        }

        [HttpPost]
        public JsonResult GenerateCode()
        {
            CUSTOMER cus = new CUSTOMER();
            var id = cus.GenerateId();
            return Json(id);
        }

        [HttpPost]
        public JsonResult ChangeId(string NewId, string OldId)
        {
            CUSTOMER cus = new CUSTOMER();
            var id = cus.ChangeId(NewId, OldId);
            return Json(id);
        }

        [HttpPost]
        public JsonResult AddEndUser(END_USER END)
        {
            var listEndUser = END.GetEndUser(END.CUS_ID, END.END_USER_ID);
            if (listEndUser != null && listEndUser.Count > 0)
                return Json(-1);
            var id = END.Insert(END.CUS_ID.ToUpper(), END.END_USER_ID.ToUpper(), END.NAME);
            return Json(id);
        }

        [HttpPost]
        public JsonResult GetListEndUser(string CUS_ID)
        {
            CUSTOMER cus = new CUSTOMER();
            var lst = cus.GetFullEndUser(CUS_ID);
            return Json(lst);
        }
    }
}
