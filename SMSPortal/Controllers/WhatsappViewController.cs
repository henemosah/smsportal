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
    public class WhatsappViewController : Controller
    {
        private ePOD_DBEntities1 db = new ePOD_DBEntities1();
        // GET: WhatsappView
        public ActionResult Index()
        {
            DateTime selected_date = DateTime.Today.Date;
            selected_date = DateTime.Parse(selected_date.ToString("yyyy-MM-dd"));

            ViewBag.selected_date = selected_date;

            DateTime end_date = DateTime.Today.AddDays(1);

            var smsResponsex = db.SMSResponses.Where(a => a.created_on >= selected_date && selected_date < end_date).OrderByDescending(s => s.created_on);
            List<view_messagesW_dto> message_full_list = new List<view_messagesW_dto>();
            var value = smsResponsex.Count();

            message_full_list.AddRange(
                    from msgs in smsResponsex
                    select new view_messagesW_dto
                    {
                        Id = msgs.Id,
                        code_b = msgs.code_b,
                        damaged = msgs.damaged.HasValue? msgs.damaged.Value : 0,
                        caked = msgs.caked.HasValue? msgs.caked.Value : 0,
                        complaints = msgs.complaints,
                        created_on = msgs.created_on,
                        sms_request_id = msgs.sms_request_id.HasValue? msgs.sms_request_id.Value : 0,
                        missing = msgs.missing.HasValue? msgs.missing.Value : 0,                       
                        status = true,
                        
                    }
                );





            return View(message_full_list);
        }

        public ActionResult ExportReportExcel(DateTime selected_date)
        {

            String company_name = "DANGOTE CEMENT";

            List<view_messagesW_dto> _whatsapp_list = TempData["whatsapp_list"] as List<view_messagesW_dto>;
            // Opening the Excel template...
            FileStream fs =
                new FileStream(Server.MapPath(@"\Content\template\reporting_Whatsapp_List_Selected.xls"), FileMode.Open, FileAccess.Read);


            MemoryStream ms = new MemoryStream();
            // Getting the complete workbook...
            HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);
            try
            {

                // Getting the worksheet by its name...

                NPOI.HSSF.UserModel.HSSFSheet sheet = (HSSFSheet)templateWorkbook.GetSheet("WhatsappList");
                //RO's Name

                NPOI.HSSF.UserModel.HSSFRow dataRow = (HSSFRow)sheet.GetRow(3);

                ICell cell_default = dataRow.Cells[0];

                ICellStyle boldStyle = cell_default.CellStyle;

                dataRow = (HSSFRow)sheet.GetRow(0);
                dataRow.Cells[0].SetCellValue(company_name);

                int Row = 3;
                foreach (view_messagesW_dto item in _whatsapp_list)
                {

                    dataRow = (HSSFRow)sheet.CreateRow(Row); //
                    dataRow.CreateCell(0).SetCellValue(Row - 2);
                    dataRow.CreateCell(1).SetCellValue(item.code_b);
                    dataRow.CreateCell(2).SetCellValue(item.damaged);
                    dataRow.CreateCell(3).SetCellValue(item.caked);
                    dataRow.CreateCell(4).SetCellValue(item.complaints);
                    dataRow.CreateCell(5).SetCellValue(item.created_on.ToString());
                    dataRow.CreateCell(6).SetCellValue(item.sms_request_id);
                    dataRow.CreateCell(7).SetCellValue(item.missing);
                    


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



            String FileName = "whatsapp_list_report_" + selected_date.ToString("-yyyy-MM-dd") + ".xls";

            // Writing the workbook content to the FileStream...
            templateWorkbook.Write(ms);
            return File(ms.ToArray(), "application/vnd.ms-excel", FileName);
        }

        public ActionResult SearchResults(FormCollection myForm)
        {
            String chosen_datex = myForm["selected_datex"];
            String changed_date = Session["date_changed"].ToString();

            DateTime chosen_date = Convert.ToDateTime(changed_date).Date;

            DateTime selected_date = DateTime.Parse(chosen_date.ToString("yyyy-MM-dd"));

            ViewBag.selected_date = selected_date;


            DateTime end_date = selected_date.AddDays(1);

            var smsResponsex = db.SMSResponses.Where(a => a.created_on >= selected_date && selected_date < end_date).OrderByDescending(s => s.created_on);
            List<view_messagesW_dto> message_full_list = new List<view_messagesW_dto>();
            var value = smsResponsex.Count();

            message_full_list.AddRange(
                    from msgs in smsResponsex
                    select new view_messagesW_dto
                    {
                        Id = msgs.Id,
                        code_b = msgs.code_b,
                        damaged = msgs.damaged.HasValue ? msgs.damaged.Value : 0,
                        caked = msgs.caked.HasValue ? msgs.caked.Value : 0,
                        complaints = msgs.complaints,
                        created_on = msgs.created_on,
                        sms_request_id = msgs.sms_request_id.HasValue ? msgs.sms_request_id.Value : 0,
                        missing = msgs.missing.HasValue ? msgs.missing.Value : 0,
                        status = true,

                    }
                );



            var newWhatsappList = message_full_list.ToList();

            TempData["whatsapp_List"] = newWhatsappList;

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