using Microsoft.Reporting.WebForms;
using SanayeeIdentity_5.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
namespace SanayeeIdentity_5.Controllers
{

    [Authorize(Roles = "Admins")]

    public class ReportController : Controller
    {
        ReportsEntites DB = new ReportsEntites();
        Entities Db2 = new Entities();


        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SummeryPage()
        {
            List<int> years = new List<int>();
            for (int i = DateTime.Now.Year; i > DateTime.Now.Year - 12; i--)
            {
                years.Add(i);
            }

            @ViewBag.Years = new SelectList(years, DateTime.Now.Year);
            List<RequstsInMonthOfYear_Result> ex = DB.RequstsInMonthOfYear(DateTime.Now.Year).ToList();
            @ViewBag.MaxReq = DB.RequstsInfoes.Max(ri => ri.NumofReqursts);

            return View(ex);
        }
        [HttpPost]
        public ActionResult SummeryPageAjax(int Years)
        {

            List<RequstsInMonthOfYear_Result> ex = DB.RequstsInMonthOfYear(Years).ToList();

            return PartialView("RequstsInMonthParial", ex);
        }

        [HttpGet]

        public ActionResult ReportsView()
        {
            List<int> years = new List<int>();
            List<int> Months = new List<int>();
            for (int i = DateTime.Now.Year; i > DateTime.Now.Year - 12; i--)
            {
                years.Add(i);
            }
            for (int i = 1; i <= 12; i++)
            {
                Months.Add(i);
            }
            @ViewBag.Years = new SelectList(years, DateTime.Now.Year);
            @ViewBag.Months = new SelectList(Months, DateTime.Now.Month);
            @ViewBag.Cites = new SelectList(Db2.Citys, "CityId", "Name");
            @ViewBag.Areas = new SelectList(Db2.Areas.Where(r => r.CityId == 0), "AreaId", "Name");
            @ViewBag.Types = new SelectList(Db2.Types, "TypeId", "Name");
            return View();
        }



        [HttpPost]
        public ActionResult RequstsPrintReport(string Years, string Months, string Cites, string Areas, string PrintFile = "bdf")
        {
            if (Years == "" || Years == null) Years = "0";
            if (Months == "" || Months == null) Months = "0";
            if (Cites == "" || Cites == null) Cites = "0";
            if (Areas == "" || Areas == null) Areas = "0";


            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reports"), "Report1.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return RedirectToAction("RequstrsReport");
            }
            List<RequstsInfo> cm = new List<RequstsInfo>();
            try
            {
                cm = GetReportData(int.Parse(Years), int.Parse(Months), int.Parse(Cites), int.Parse(Areas));
                ReportDataSource rd = new ReportDataSource("DataSet1", cm);
                lr.DataSources.Add(rd);

                string reporttype = PrintFile;
                string mimetype;
                string encoding;
                string fileNameExtention;
                string deviceinfo =
                    "<DeviceInfo>" +
                    " <OutputFormat>" + PrintFile + "</OutputFormat>" +
                     " <PageWidth>10in</PageWidth>" +
                     " <PageHeight>11in</PageHeight>" +
                     " <MarginTop>0.5in</MarginTop>" +
                      " <MarginLeft>1in</MarginLeft>" +
                      " <MarginRight>1in</MarginRight>" +
                       " <MarginBottom>0.5in</MarginBottom>" +
                     " </DeviceInfo>";


                Warning[] warnings;
                string[] straems;
                byte[] renderbytes;
                renderbytes = lr.Render(
                    reporttype,
                    deviceinfo,
                    out mimetype,
                    out encoding,
                    out fileNameExtention,
                    out straems,
                    out warnings
                     );

                return File(renderbytes, mimetype);

            }
            catch (Exception)
            {

                return RedirectToAction("RequstrsReport");
            }


        }




        static List<RequstsInfo> GetReportData(int Years, int Months, int Cites, int Areas)
        {
            ReportsEntites DB = new ReportsEntites();

            Entities Db2 = new Entities();
            string AreaName = "", cityName = "";
            if (Cites != 0)
            {
                cityName = Db2.Citys.Where(c => c.CityId == Cites).FirstOrDefault().Name;
            }
            if (Areas != 0)
            {
                AreaName = Db2.Areas.Where(c => c.AreaId == Areas).FirstOrDefault().Name;

            }

            if (Years != 0 & Months != 0 & Cites != 0 & Areas != 0)
            {
                return DB.RequstsInfoes.Where(ri => ri.year == Years & ri.month == Months & ri.cityid == cityName & ri.AreaName == AreaName).ToList();

            }

            if (Years != 0 & Months != 0 & Cites != 0 & Areas == 0)
            {
                return DB.RequstsInfoes.Where(ri => ri.year == Years & ri.month == Months & ri.cityid == cityName).ToList();

            }
            if (Years == 0 & Cites != 0 & Areas != 0)
            {
                return DB.RequstsInfoes.Where(ri => ri.cityid == cityName & ri.AreaName == AreaName).ToList();

            }
            if (Years == 0 & Cites != 0 & Areas == 0)
            {
                return DB.RequstsInfoes.Where(ri => ri.cityid == cityName).ToList();

            }

            if (Years != 0 & Months == 0 & Cites != 0 & Areas != 0)
            {
                return DB.RequstsInfoes.Where(ri => ri.year == Years & ri.cityid == cityName & ri.AreaName == AreaName).ToList();

            }

            if (Years != 0 & Months == 0 & Cites != 0)
            {
                return DB.RequstsInfoes.Where(ri => ri.year == Years & ri.cityid == cityName).ToList();

            }





            if (Years != 0 & Months != 0 & Cites == 0)
            {
                return DB.RequstsInfoes.Where(ri => ri.year == Years & ri.month == Months).ToList();

            }

            if (Years != 0 & Months == 0 & Cites == 0)
            {
                return DB.RequstsInfoes.Where(ri => ri.year == Years).ToList();

            }

            return DB.RequstsInfoes.ToList();


        }





        public JsonResult GetAreaList(string CityId)
        {
            List<Area> Arealist = new List<Data.Area>();
            if (CityId != null && CityId != "0")
            {
                Db2.Configuration.ProxyCreationEnabled = false;
                int x = int.Parse(CityId);
                Arealist = Db2.Areas.Where(p => p.CityId == x).ToList();
            }


            return Json(Arealist, JsonRequestBehavior.AllowGet);
        }





        public ActionResult PricesPrintReport(string Years, string Months, string Types, string PrintFile = "bdf")
        {
            if (Years == "") Years = "0";
            if (Months == "") Months = "0";
            if (Types == "") Types = "0";


            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reports"), "Report2.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return RedirectToAction("RequstrsReport");
            }
            List<PricesInfo> cm = new List<PricesInfo>();
            cm = GetReportData2(int.Parse(Years), int.Parse(Months), int.Parse(Types));
            ReportDataSource rd = new ReportDataSource("DataSet2", cm);
            lr.DataSources.Add(rd);

            string reporttype = PrintFile;
            string mimetype;
            string encoding;
            string fileNameExtention;
            string deviceinfo =
                "<DeviceInfo>" +
                " <OutputFormat>" + PrintFile + "</OutputFormat>" +
                 " <PageWidth>8.5in</PageWidth>" +
                 " <PageHeight>11in</PageHeight>" +
                 " <MarginTop>0.5in</MarginTop>" +
                  " <MarginLeft>1in</MarginLeft>" +
                  " <MarginRight>1in</MarginRight>" +
                   " <MarginBottom>0.5in</MarginBottom>" +
                 " </DeviceInfo>";


            Warning[] warnings;
            string[] straems;
            byte[] renderbytes;
            renderbytes = lr.Render(
                reporttype,
                deviceinfo,
                out mimetype,
                out encoding,
                out fileNameExtention,
                out straems,
                out warnings
                 );

            return File(renderbytes, mimetype);


        }

        private List<PricesInfo> GetReportData2(int Year, int month, int type)
        {
            if (type != 0)
            {
                if (Year != 0 & month != 0)
                {
                    return DB.PricesInfoes.Where(pi => pi.TypeId == type & pi.year == Year & pi.month == month).ToList();

                }
                if (Year != 0)
                {
                    return DB.PricesInfoes.Where(pi => pi.TypeId == type & pi.year == Year).ToList();
                }
                return DB.PricesInfoes.Where(pi => pi.TypeId == type).ToList();
            }
            return DB.PricesInfoes.ToList(); ;
        }

        public ActionResult NumOfWorkersPerCityPrintReport(string Types, string Cites2, string Areas2, string PrintFile = "bdf")
        {
            if (Types == "") Types = "0";

            if (Cites2 == "") Cites2 = "0";
            if (Areas2 == "") Areas2 = "0";


            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/Reports"), "Report3.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return RedirectToAction("ReportsView");
            }
            List<NumOfWorekerPerCityArea> cm = new List<NumOfWorekerPerCityArea>();
            cm = GetReportData3(int.Parse(Types), int.Parse(Cites2), int.Parse(Areas2));
            ReportDataSource rd = new ReportDataSource("DataSet3", cm);
            lr.DataSources.Add(rd);

            string reporttype = PrintFile;
            string mimetype;
            string encoding;
            string fileNameExtention;
            string deviceinfo =
                "<DeviceInfo>" +
                " <OutputFormat>" + PrintFile + "</OutputFormat>" +
                 " <PageWidth>8.5in</PageWidth>" +
                 " <PageHeight>11in</PageHeight>" +
                 " <MarginTop>0.5in</MarginTop>" +
                  " <MarginLeft>1in</MarginLeft>" +
                  " <MarginRight>1in</MarginRight>" +
                   " <MarginBottom>0.5in</MarginBottom>" +
                 " </DeviceInfo>";


            Warning[] warnings;
            string[] straems;
            byte[] renderbytes;
            renderbytes = lr.Render(
                reporttype,
                deviceinfo,
                out mimetype,
                out encoding,
                out fileNameExtention,
                out straems,
                out warnings
                 );

            return File(renderbytes, mimetype);


        }

        private List<NumOfWorekerPerCityArea> GetReportData3(int Type, int City, int Area)
        {
            string AreaName = "", cityName = "", TypeName = "";
            if (City != 0)
            {
                cityName = Db2.Citys.Where(c => c.CityId == City).FirstOrDefault().Name;
            }
            if (Area != 0)
            {
                AreaName = Db2.Areas.Where(c => c.AreaId == Area).FirstOrDefault().Name;

            }
            if (Type != 0)
            {
                TypeName = Db2.Types.Where(c => c.TypeId == Type).FirstOrDefault().Name;

            }
            if (Type != 0 & City != 0 & Area != 0)
            {
                return DB.NumOfWorekerPerCityAreas.Where(n => n.CityName == cityName & n.AreaName == AreaName & n.TypeName == TypeName).ToList();

            }
            if (Type != 0 & City != 0 & Area == 0)
            {
                return DB.NumOfWorekerPerCityAreas.Where(n => n.CityName == cityName & n.TypeName == TypeName).ToList();

            }
            if (Type != 0 & City == 0)
            {
                return DB.NumOfWorekerPerCityAreas.Where(n => n.TypeName == TypeName).ToList();

            }
            if (Type == 0 & City != 0 & Area != 0)
            {
                return DB.NumOfWorekerPerCityAreas.Where(n => n.CityName == cityName & n.AreaName == AreaName).ToList();

            }
            if (Type == 0 & City != 0 & Area == 0)
            {
                return DB.NumOfWorekerPerCityAreas.Where(n => n.CityName == cityName).ToList();

            }

            return DB.NumOfWorekerPerCityAreas.ToList();
        }




        //public ActionResult NumOfWorkersPerCityPrintReport( string Types, string PrintFile = "bdf")
        //{




        //    LocalReport lr = new LocalReport();
        //    string path = Path.Combine(Server.MapPath("~/Reports"), "Report2.rdlc");
        //    if (System.IO.File.Exists(path))
        //    {
        //        lr.ReportPath = path;
        //    }
        //    else
        //    {
        //        return RedirectToAction("RequstrsReport");
        //    }
        //    List<PricesInfo> cm = new List<PricesInfo>();
        //    cm = GetReportData3(int.Parse(Years), int.Parse(Months), int.Parse(Types));
        //    ReportDataSource rd = new ReportDataSource("DataSet2", cm);
        //    lr.DataSources.Add(rd);

        //    string reporttype = PrintFile;
        //    string mimetype;
        //    string encoding;
        //    string fileNameExtention;
        //    string deviceinfo =
        //        "<DeviceInfo>" +
        //        " <OutputFormat>" + PrintFile + "</OutputFormat>" +
        //         " <PageWidth>8.5in</PageWidth>" +
        //         " <PageHeight>11in</PageHeight>" +
        //         " <MarginTop>0.5in</MarginTop>" +
        //          " <MarginLeft>1in</MarginLeft>" +
        //          " <MarginRight>1in</MarginRight>" +
        //           " <MarginBottom>0.5in</MarginBottom>" +
        //         " </DeviceInfo>";


        //    Warning[] warnings;
        //    string[] straems;
        //    byte[] renderbytes;
        //    renderbytes = lr.Render(
        //        reporttype,
        //        deviceinfo,
        //        out mimetype,
        //        out encoding,
        //        out fileNameExtention,
        //        out straems,
        //        out warnings
        //         );

        //    return File(renderbytes, mimetype);


        //}

    }
}