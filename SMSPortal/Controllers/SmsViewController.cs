using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMSPortal.Business_Objects;
using SMSPortal.Models;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace SMSPortal.Controllers
{
    public class SmsViewController : Controller
    {
        // GET: SmsView
        private ePOD_DBEntities1 db = new ePOD_DBEntities1();
        public ActionResult Index(DateTime? chosen_date)
        {
            DateTime selected_date = chosen_date.HasValue ? chosen_date.Value : DateTime.Today.Date;
            //DateTime.Today.Date;

            selected_date = DateTime.Parse(selected_date.ToString("yyyy-MM-dd"));

            ViewBag.selected_date = selected_date;

            DateTime end_date = DateTime.Today.AddDays(1);

            var baseSmsx = db.baseSMS.Where(a => a.create_datetime >= selected_date && selected_date < end_date).OrderByDescending(s => s.create_datetime);
            List<view_message_dto> message_full_list = new List<view_message_dto>();
            var value = baseSmsx.Count();

            message_full_list.AddRange(
                    from msgs in baseSmsx
                    select new view_message_dto
                    {
                        Id = msgs.Id,
                        atc_no = msgs.atc_no,
                        delivery_no = msgs.delivery_no,
                        customer_no = msgs.customer_no,
                        phone_no = msgs.phone_no,
                        waybill_no = msgs.waybill_no,
                        customer_name = msgs.customer_name,
                        ip_address = msgs.ip_address,
                        code_a = msgs.code_a,
                        code_b = msgs.code_b,
                        sms_status = true,
                        response = msgs.response,
                        create_datetime = msgs.create_datetime,
                        response_datetime = msgs.response_datetime,

                    }
                );
            
            return View(message_full_list);
            
        }

        public ActionResult ExportReportExcel(DateTime selected_date)
        {

            String company_name = "DANGOTE CEMENT";
            
            List<view_message_dto> _sms_list = TempData["sms_list"] as List<view_message_dto>;
            // Opening the Excel template...
            FileStream fs =
                new FileStream(Server.MapPath(@"\Content\template\reporting_sms_List_Selected.xls"), FileMode.Open, FileAccess.Read);


            MemoryStream ms = new MemoryStream();
            // Getting the complete workbook...
            HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
            try
            {

                // Getting the worksheet by its name...
                NPOI.HSSF.UserModel.HSSFSheet sheet = (HSSFSheet)templateWorkbook.GetSheet("SmsList");
                //RO's Name

                NPOI.HSSF.UserModel.HSSFRow dataRow = (HSSFRow)sheet.GetRow(3);

                ICell cell_default = dataRow.Cells[0];

                ICellStyle boldStyle = cell_default.CellStyle;

                dataRow = (HSSFRow)sheet.GetRow(0);
                dataRow.Cells[0].SetCellValue(company_name);

                int Row = 3;
                foreach (view_message_dto item in _sms_list)
                {
                    
                    dataRow = (HSSFRow)sheet.CreateRow(Row); //
                    dataRow.CreateCell(0).SetCellValue(Row - 2);
                    dataRow.CreateCell(1).SetCellValue(item.atc_no);
                    dataRow.CreateCell(2).SetCellValue(item.delivery_no);
                    dataRow.CreateCell(3).SetCellValue(item.customer_no);
                    dataRow.CreateCell(4).SetCellValue(item.phone_no);
                    dataRow.CreateCell(5).SetCellValue(item.waybill_no);
                    dataRow.CreateCell(6).SetCellValue(item.customer_name);
                    dataRow.CreateCell(7).SetCellValue(item.ip_address);
                    dataRow.CreateCell(8).SetCellValue(item.code_a);
                    dataRow.CreateCell(9).SetCellValue(item.code_b);
                    dataRow.CreateCell(10).SetCellValue(item.sms_status);
                    dataRow.CreateCell(11).SetCellValue(item.response);
                    dataRow.CreateCell(12).SetCellValue(item.create_datetime.ToString());
                    dataRow.CreateCell(13).SetCellValue(item.response_datetime.ToString());

                  
                    if (Row % 2 == 0)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            ICell cell = dataRow.Cells[i];
                            cell.CellStyle = boldStyle;

                        }
                    }

                    Row++;
                }

                sheet.ForceFormulaRecalculation = true;


                TempData["Message"] = "Excel report created successfully!";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Oops! Something went wrong." + "<br/>" + ex.Message.ToString();

            }



            String FileName = "sms_list_report_" + selected_date.ToString("-yyyy-MM-dd") + ".xls";

            // Writing the workbook content to the FileStream...
            templateWorkbook.Write(ms);
            return File(ms.ToArray(), "application/vnd.ms-excel", FileName);
        }

        public ActionResult getReportFromChosenDate(DateTime chosen_date)
        {
            DateTime selected_date = chosen_date;
            

            selected_date = DateTime.Parse(selected_date.ToString("yyyy-MM-dd"));

            //ViewBag.selected_date = selected_date;

            DateTime end_date = selected_date.AddDays(1);

            var baseSmsx = db.baseSMS.Where(a => a.create_datetime >= selected_date && selected_date < end_date).OrderByDescending(s => s.create_datetime);
            List<view_message_dto> message_full_list = new List<view_message_dto>();
            var value = baseSmsx.Count();

            message_full_list.AddRange(
                    from msgs in baseSmsx
                    select new view_message_dto
                    {
                        Id = msgs.Id,
                        atc_no = msgs.atc_no,
                        delivery_no = msgs.delivery_no,
                        customer_no = msgs.customer_no,
                        phone_no = msgs.phone_no,
                        waybill_no = msgs.waybill_no,
                        customer_name = msgs.customer_name,
                        ip_address = msgs.ip_address,
                        code_a = msgs.code_a,
                        code_b = msgs.code_b,
                        sms_status = true,
                        response = msgs.response,
                        create_datetime = msgs.create_datetime,
                        response_datetime = msgs.response_datetime

                    }
                );

            return View(message_full_list);
        }

        


        //[SessionExpire]
        //[Authorize]
        public ActionResult SearchResults(FormCollection myForm)
        {
            String chosen_datex = myForm["selected_datex"];
            String changed_date = Session["date_changed"].ToString();
            
            DateTime chosen_date = Convert.ToDateTime(changed_date).Date;            

            DateTime selected_date = DateTime.Parse(chosen_date.ToString("yyyy-MM-dd"));            

            ViewBag.selected_date = selected_date;

           
            DateTime end_date = selected_date.AddDays(1);

            var baseSmsx = db.baseSMS.Where(a => a.create_datetime >= selected_date && selected_date < end_date).OrderByDescending(s => s.create_datetime);
            List<view_message_dto> message_full_list = new List<view_message_dto>();
            var value = baseSmsx.Count();

            message_full_list.AddRange(
                    from msgs in baseSmsx
                    select new view_message_dto
                    {
                        Id = msgs.Id,
                        atc_no = msgs.atc_no,
                        delivery_no = msgs.delivery_no,
                        customer_no = msgs.customer_no,
                        phone_no = msgs.phone_no,
                        waybill_no = msgs.waybill_no,
                        customer_name = msgs.customer_name,
                        ip_address = msgs.ip_address,
                        code_a = msgs.code_a,
                        code_b = msgs.code_b,
                        sms_status = true,
                        response = msgs.response,
                        create_datetime = msgs.create_datetime,
                        response_datetime = msgs.response_datetime

                    }
                );



            var newSmsList = message_full_list.ToList();

            TempData["sms_List"] = newSmsList;

            return RedirectToAction("ExportReportExcel", new
            {
                selected_date = selected_date
            });


        }

        public EmptyResult changedDates(DateTime? changedDate)
        {
            DateTime date_changed = changedDate.HasValue ? changedDate.Value : DateTime.Today.Date;
            Session["date_changed"] = date_changed;
            return new EmptyResult();
         

        }
    }
}