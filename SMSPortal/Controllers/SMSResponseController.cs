using SMSPortal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio.AspNet.Common;
using Twilio.AspNet.Mvc;
using Twilio.TwiML;

namespace SMSPortal.Controllers
{
    public class SMSResponseController : TwilioController
    {
        // GET: SMSResponse
        public ActionResult Index()
        {
            return View();
        }

        public string GetOrderInformation(string sales_doc_number)
        {

            String server = "LTDANHO-4057\\SQLEXPRESS_2016";
            String database = "DSRBAGCOUNTER";
            String username0 = "sa";
            String password = "password@123";

            using (var db = new ePOD_DBEntities1())
            {


                List<dds_config> dds_configs = db.dds_config.ToList();

                foreach (var config_info in dds_configs)
                {

                    server = config_info.ip_address;
                    database = config_info.database_name;
                    username0 = config_info.username;
                    password = config_info.password;

                    String ConnStr = "Data Source=" + server + ";Initial Catalog=" + database + ";User ID=" + username0 + ";Password=" + password + ";;MultipleActiveResultSets=True;Application Name=EntityFramework";

                    using (SqlConnection con = new SqlConnection())
                    {
                        String sql = @"select * from atc_data WHERE sales_doc_number = '" + sales_doc_number + "'";
                        con.ConnectionString = ConnStr;

                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = new SqlCommand(sql, con);

                        da.Fill(dt); //TODO: if unableto connect to db, then error occurs
                        if (dt.Rows.Count == 0)
                        {
                            return "NOT FOUND";
                        }


                        //get transaction_data
                        //foreach (DataRow row in dt.Rows)
                        //{
                        //    var bilVal = new damage_table()
                        //    {
                        //        BagCnt1 = row["AddBagCnt1"].ToString(),
                        //        BagCnt2 = row["AddBagCnt2"].ToString(),
                        //        BagCntRes1 = row["AddBagRes1"].ToString(),
                        //        BagCntRes2 = row["AddBagRes2"].ToString(),
                        //        DateTimeItem = DateTime.Parse(row["AdBgResDtTim"].ToString()),
                        //        LineNum = int.Parse(row["LineNum"].ToString())
                        //    };
                        //    prod_list.Add(bilVal);
                        //}
                    }
                }
            }

            return "SUCCESS";
        }

        [HttpPost]
        public TwiMLResult Respond(SmsRequest incomingMessage)
        {

            string message_var = incomingMessage.Body;

            if (message_var.Trim() == "")
            {
                var messagingResponse0 = new MessagingResponse();
                messagingResponse0.Message("Invalid Request");

                return TwiML(messagingResponse0);
            }
            var message = message_var.Trim().Split(' ');

            string code = "";
            string damaged = "";
            string caked = "";
            string missing = "";

            int damaged_int = 0;
            int caked_int = 0;
            int missing_int = 0;

            string complaints = "";
            if (message.Length > 0)
            {
                code = message[0].Trim();
            }

            var messagingResponse_en = new MessagingResponse();

            if (code.ToUpper() == "ORDER")
            {
                if (message.Length < 2)
                {

                    messagingResponse_en.Message("Please enter an Order Number for more information.");

                    return TwiML(messagingResponse_en);
                }

                string sales_doc_number = message[1].Trim();

                //GET Specific Order Information

                messagingResponse_en.Message("ORDER INFORMATION");

                return TwiML(messagingResponse_en);
            }
            else if (code.ToUpper() == "ORDERS")
            {

                //GET LAst 5 Orders Information

                messagingResponse_en.Message("Last 5 Orders");

                return TwiML(messagingResponse_en);
            }


            //CHECK for differenet codes
            for (int i = 1; i < message.Length; i++)
            {
                if (message[i].Trim() != "")
                {
                    string token = message[i].Trim();

                    if (token.Substring(0, 1) == "d")
                    {
                        damaged = token.Substring(1, token.Length - 1);
                    }
                    else if (token.Substring(0, 1) == "c")
                    {
                        caked = token.Substring(1, token.Length - 1);
                    }
                    else if (token.Substring(0, 1) == "m")
                    {
                        missing = token.Substring(1, token.Length - 1);
                    }
                }
            }

            complaints = message_var.Replace(code, "");
            complaints = complaints.Replace("d" + damaged, "");
            complaints = complaints.Replace("c" + caked, "");
            complaints = complaints.Replace("m" + missing, "");
            complaints = complaints.Trim();

            Int32.TryParse(damaged, out damaged_int);
            Int32.TryParse(caked, out caked_int);
            Int32.TryParse(missing, out missing_int);

            using (var db = new ePOD_DBEntities1())
            {
                baseSM previous_sms = db.baseSMS.Where(a => a.code_b == code && a.customer_no == incomingMessage.From).FirstOrDefault();

                if (previous_sms == null)
                {
                    var messagingResponse1 = new MessagingResponse();
                    messagingResponse1.Message("Invalid Code. Please enter the code that is displayed on the waybill.");

                    return TwiML(messagingResponse1);
                }

                SMSResponse new_response = new SMSResponse()
                {
                    caked = caked_int,
                    code_b = code,
                    missing = missing_int,
                    damaged = damaged_int,
                    complaints = complaints,
                    created_on = DateTime.Now,
                    sms_request_id = previous_sms.Id
                };

                db.SMSResponses.Add(new_response);
                db.SaveChanges();

            }

            var messagingResponse = new MessagingResponse();
            messagingResponse.Message("Thank you for your response. We are happy to serve you.");

            return TwiML(messagingResponse);
        }
    }
}