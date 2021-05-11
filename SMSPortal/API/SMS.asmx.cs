using Newtonsoft.Json;
using SMSPortal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace SMSPortal.API
{
    /// <summary>
    /// Summary description for SMS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SMS : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public string TestWhatsappMessage()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            const string accountSid = "ACbd9ba0009201625ce0fcd76c20af56fa";
            const string authToken = "7b66a2a45430b10ea9df92fd1a92a209";

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: "This is a whatsapp test",
                from: new Twilio.Types.PhoneNumber("whatsapp:+14155238886"),
                to: new Twilio.Types.PhoneNumber("whatsapp:+2348111845310") //2349030061590
            );

            Console.WriteLine(message.Sid);
            return message.Sid;
        }

        public base_customer GetPhoneNo(string customer_no)
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
                        String sql = @"select * from customer WHERE customer_no = '" + customer_no + "' and sms_enabled = 1";
                        con.ConnectionString = ConnStr;

                        DataTable dt = new DataTable();
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = new SqlCommand(sql, con);

                        da.Fill(dt); //TODO: if unableto connect to db, then error occurs
                        if (dt.Rows.Count == 0)
                        {
                            return null;
                        }


                        //get transaction_data
                        foreach (DataRow row in dt.Rows)
                        {

                            string phone = row["phone"].ToString();
                            string customer_name = row["customer_name"].ToString();

                            return new base_customer()
                            { customer_name= customer_name, phone = phone
                            };
                        }
                    }
                }

            }


            return null;

        }
        public class base_customer
        {
            public string phone { get; set; }
            public string customer_name { get; set; }
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SendSMSviaGateWay(string atc_no, string delivery_no,
                                                            string waybill_no, string customer_no,
                                                            string customer_name, string code_b)
        {
            int sms_id = 0;
            using (var db = new ePOD_DBEntities1())
            {
                baseSM previous_sms = db.baseSMS.Where(a => a.atc_no == atc_no && a.delivery_no == delivery_no).FirstOrDefault();

                base_customer customer = GetPhoneNo(customer_no);
                if (previous_sms != null)
                {
                    //messge exists
                }
                else if (customer == null)
                {
                    //messge exists
                }
                else
                {
                    string phone = customer.phone;
                    customer_name = customer.customer_name;
                    //New Message
                    baseSM new_sms = new baseSM()
                    {
                        atc_no = atc_no,
                        code_a = "code_a",
                        code_b = code_b,
                        customer_name = customer_name,
                        customer_no = customer_no,
                        create_datetime = DateTime.Now,
                        delivery_no = delivery_no,
                        phone_no = phone,
                        waybill_no = waybill_no,
                        sms_status = true
                    };

                    db.baseSMS.Add(new_sms);
                    db.SaveChanges();
                    sms_id = new_sms.Id;


                    string msg = "";
                    msg += "Dear " + customer_name + ", Dispatch Details.";
                    msg += "ATC: " + atc_no + ", ";
                    msg += "Del: " + delivery_no + ", ";
                    msg += "WB: " + waybill_no + ", ";
                    msg += "Code: " + code_b + ". Reply with Code shown on Waybill.";


                    string user = "apiuser";
                    string pass = "apipass";
                    string port = "1"; //Get list of ports
                    string destination = HttpUtility.UrlEncode(phone); // "%2B2348150886674";// "%2B2349030061590";// "+2349030061590";

                    //Destination must have +234X0

                    string content = HttpUtility.UrlEncode(msg); // "Message+from+c%23+through+SMS+GateWay";// "Message from c# through SMS GateWay";

                    string sms_api = "http://172.18.19.7//cgi/WebCGI?1500101=";
                    sms_api += "&account=" + user;
                    sms_api += "&password=" + pass;
                    sms_api += "&port=" + port;
                    sms_api += "&destination=" + destination;
                    sms_api += "&content=" + content;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@sms_api);
                    request.Credentials = new NetworkCredential("apiuser", "apipass");
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    string return_response = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    //-- MessageBox.Show("Done.\n\n" + return_response);

                    baseSM current_sms = db.baseSMS.Where(a => a.Id == sms_id).FirstOrDefault();

                    if (current_sms != null)
                    {
                        current_sms.response = return_response;
                        db.Entry(current_sms).State = System.Data.Entity.EntityState.Modified;

                        db.SaveChanges();
                    }
                    Context.Response.Write(JsonConvert.SerializeObject(return_response, Formatting.Indented,
                            new JsonSerializerSettings
                            {
                                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            }));
                }
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SendSMSviaGateWayPH(string atc_no, string delivery_no,
                                                            string waybill_no, string customer_no,
                                                            string customer_name, string phone, string code_b)
        {
            int sms_id = 0;
            using (var db = new ePOD_DBEntities1())
            {
                baseSM previous_sms = db.baseSMS.Where(a => a.atc_no == atc_no && a.delivery_no == delivery_no).FirstOrDefault();

                if (previous_sms != null)
                {
                    //messge exists
                }
                else
                {
                    //New Message
                    baseSM new_sms = new baseSM()
                    {
                        atc_no = atc_no,
                        code_a = "code_a",
                        code_b = code_b,
                        customer_name = customer_name,
                        customer_no = customer_no,
                        create_datetime = DateTime.Now,
                        delivery_no = delivery_no,
                        phone_no = phone,
                        waybill_no = waybill_no,
                        sms_status = true
                    };

                    db.baseSMS.Add(new_sms);
                    db.SaveChanges();
                    sms_id = new_sms.Id;
                }


                string msg = "";
                msg += "Dear " + customer_name + ", Dispatch Details.";
                msg += "ATC: " + atc_no + ", ";
                msg += "Del: " + delivery_no + ", ";
                msg += "WB: " + waybill_no + ", ";
                msg += "Code: " + code_b + ". Reply with Code shown on Waybill.";


                string user = "apiuser";
                string pass = "apipass";
                string port = "1"; //Get list of ports
                string destination = HttpUtility.UrlEncode(phone); // "%2B2348150886674";// "%2B2349030061590";// "+2349030061590";

                //Destination must have +234X0

                string content = HttpUtility.UrlEncode(msg); // "Message+from+c%23+through+SMS+GateWay";// "Message from c# through SMS GateWay";

                string sms_api = "http://172.18.19.7//cgi/WebCGI?1500101=";
                sms_api += "&account=" + user;
                sms_api += "&password=" + pass;
                sms_api += "&port=" + port;
                sms_api += "&destination=" + destination;
                sms_api += "&content=" + content;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@sms_api);
                request.Credentials = new NetworkCredential("apiuser", "apipass");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string return_response = new StreamReader(response.GetResponseStream()).ReadToEnd();

                //-- MessageBox.Show("Done.\n\n" + return_response);

                baseSM current_sms = db.baseSMS.Where(a => a.Id == sms_id).FirstOrDefault();

                if (current_sms != null)
                {
                    current_sms.response = return_response;
                    db.Entry(current_sms).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }
                Context.Response.Write(JsonConvert.SerializeObject(return_response, Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            DateFormatHandling = DateFormatHandling.IsoDateFormat,
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }));
            }
        }
    }
}
