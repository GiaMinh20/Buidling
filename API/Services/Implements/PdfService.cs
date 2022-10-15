using API.Payloads.Requests;
using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using API.Services.Interfaces;
using DocumentFormat.OpenXml.Office2016.Excel;
using API.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using API.Helpers;

namespace API.Services.Implements
{
    public class PdfService : IPdfService
    {
        private readonly string[] RentedItems = new string[] { "Id", "Type", "Location", "Price", "Renter", "RenterId", "RentedDate", "NumberOfParents" };
        private readonly string[] Reports = new string[] { "Id", "Title", "Content", "CreateDate", "Account" };
        private readonly string[] Vehicles = new string[] { "Id", "LicensePlates", "Transportation", "AccountName", "CreateDate" };
        private readonly string[] Members = new string[] { "Id", "FullName", "DateOfBirth", "Gender", "CCCD", "PlaceOfOrigin", "Nationality", "AccountId" };
        private readonly string[] Bills = new string[] { "Id", "Title", "ItemPrice", "ElectricPrice", "WaterPrice", "VehiclePrice", "OtherPrice", "ItemId", "CreateDate", "DatePaied", "AccountId", "SumPrice" };
        private readonly string[] RentRequest = new string[] { "Id", "RenterId", "ItemId", "CreateDate", "FullName", "CCCD", "NumberOfParent" };
        private readonly string[] UnRentRequest = new string[] { "Id", "RenterId", "ItemId", "CreateDate", "FullName", "CCCD", "HandleTime", "HandlerId" };
        private readonly string[] Accounts = new string[] { "Id", "Username", "Email", "PhoneNumber", "TotalItem", "NumberOfParent", "NumberOfVehicles", "CreateDate" };

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConverter _converter;
        private readonly BuildingContext _context;

        public PdfService(IWebHostEnvironment webHostEnvironment,
            IConverter converter,
            BuildingContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _converter = converter;
            _context = context;
        }

        public byte[] ExportBills(DateTime from, DateTime to)
        {
            var bills = _context.Bills.Where(b => b.Paied == true && b.DatePaied >= from && b.DatePaied <= to).ToList();

            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() + "ExportListTemplate.html";
            string HtmlBody = "";
            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }
            string body = "<tr>";
            for (int i = 0; i < Bills.Length; i++)
            {
                body += $"<th style=\"border:1px solid black;\">{Bills[i]}</th>";
            }
            body += "</tr>";
            //string body = "<tr>\r\n    <th style=\"border:1px solid black;\">Id</th>\r\n    <th style=\"border:1px solid black;\">Title</th>\r\n    <th style=\"border:1px solid black;\">ItemPrice</th>\r\n    <th style=\"border:1px solid black;\">ElectricPrice</th>\r\n    <th style=\"border:1px solid black;\">WaterPrice</th>\r\n    <th style=\"border:1px solid black;\">VehiclePrice</th>\r\n    <th style=\"border:1px solid black;\">OtherPrice</th>\r\n    <th style=\"border:1px solid black;\">ItemId</th>\r\n    <th style=\"border:1px solid black;\">CreateDate</th>\r\n    <th style=\"border:1px solid black;\">DatePaied</th>\r\n    <th style=\"border:1px solid black;\">AccountId</th>\r\n    <th style=\"border:1px solid black;\">SumPrice</th>\r\n</tr>";
            foreach (var bill in bills)
            {
                body += "<tr>";
                body += $"<td style=\"border: 1px solid black;\">{bill.Id}</td>";
                body += $"<td style=\"border: 1px solid black;\">{bill.Title}</td>";
                body += $"<td style=\"border: 1px solid black;\">{bill.ItemPrice}</td>";
                body += $"<td style=\"border: 1px solid black;\">{bill.ElectricPrice}</td>";
                body += $"<td style=\"border: 1px solid black;\">{bill.WaterPrice}</td>";
                body += $"<td style=\"border: 1px solid black;\">{bill.VehiclePrice}</td>";
                body += $"<td style=\"border: 1px solid black;\">{bill.OtherPrice}</td>";
                body += $"<td style=\"border: 1px solid black;\">{bill.ItemId}</td>";
                body += $"<td style=\"border: 1px solid black;\">{bill.CreateDate.ToShortDateString()}</td>";
                body += $"<td style=\"border: 1px solid black;\">{bill.DatePaied}</td>";
                body += $"<td style=\"border: 1px solid black;\">{bill.AccountId}</td>";
                body += $"<td style=\"border: 1px solid black;\">{bill.SumPrice()}</td>";
                body += "</tr>";
            }
            string messageBody = string.Format(HtmlBody, "Danh sách hóa đơn", body);

            HtmlToPdfDocument htmlToPdfDocument = PdfReturn(messageBody, Orientation.Landscape);

            return _converter.Convert(htmlToPdfDocument);
        }

        public byte[] ExportMembers(DateTime from, DateTime to)
        {
            var members = _context.Members.Include(m => m.PlaceOfOrigin).Where(m => m.Status == true && m.CreateDate >= from && m.CreateDate <= to).OrderBy(m => m.AccountId).ToList();

            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() + "ExportListTemplate.html";
            string HtmlBody = "";
            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }
            string body = "<tr>";
            for (int i = 0; i < Members.Length; i++)
            {
                body += $"<th style=\"border:1px solid black;\">{Members[i]}</th>";
            }
            body += "</tr>";
            foreach (var item in members)
            {
                body += "<tr>";
                body += $"<td style=\"border: 1px solid black;\">{item.Id}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.FullName}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.DateOfBirth.ToShortDateString()}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.Gender}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.CCCD}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.PlaceOfOrigin.SumAddress()}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.AccountId}</td>";
                body += "</tr>";
            }
            string messageBody = string.Format(HtmlBody, "Danh sách cư dân", body);

            HtmlToPdfDocument htmlToPdfDocument = PdfReturn(messageBody, Orientation.Landscape);

            return _converter.Convert(htmlToPdfDocument);
        }

        public byte[] ExportRentedItem(DateTime from, DateTime to)
        {
            var items = _context.Items
                            .Include(i => i.Type)
                            .Include(i => i.Renter)
                            .Where(i => i.Status == EItemStatus.Rented && i.RentedDate >= from && i.RentedDate <= to)
                            .ToList();
            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() + "ExportListTemplate.html";
            string HtmlBody = "";
            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }
            string body = "<tr>";
            for (int i = 0; i < RentedItems.Length; i++)
            {
                body += $"<th style=\"border:1px solid black;\">{RentedItems[i]}</th>";
            }
            body += "</tr>";
            foreach (var item in items)
            {
                body += "<tr>";
                body += $"<td style=\"border: 1px solid black;\">{item.Id}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.Type.Name}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.Location}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.Price}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.Renter}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.Renter.Id}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.RentedDate.Value.ToShortDateString()}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.Renter.NumberOfParent}</td>";
                body += "</tr>";
            }
            string messageBody = string.Format(HtmlBody, "Danh sách căn hộ đã được thuê", body);

            HtmlToPdfDocument htmlToPdfDocument = PdfReturn(messageBody, Orientation.Landscape);

            return _converter.Convert(htmlToPdfDocument);
        }

        public byte[] ExportRentRequests(DateTime from, DateTime to)
        {
            var rentRequests = _context.RentRequests.Where(r => r.Status == true && r.CreateDate >= from && r.CreateDate <= to).ToList();
            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() + "ExportListTemplate.html";
            string HtmlBody = "";
            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }
            string body = "<tr>";
            for (int i = 0; i < RentRequest.Length; i++)
            {
                body += $"<th style=\"border:1px solid black;\">{RentRequest[i]}</th>";
            }
            body += "</tr>";
            foreach (var item in rentRequests)
            {
                body += "<tr>";
                body += $"<td style=\"border: 1px solid black;\">{item.Id}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.RenterId}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.ItemId}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.CreateDate.ToShortDateString()}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.FullName}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.CCCD}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.NumberOfParent}</td>";
                body += "</tr>";
            }
            string messageBody = string.Format(HtmlBody, "Danh sách yêu cầu thuê", body);

            HtmlToPdfDocument htmlToPdfDocument = PdfReturn(messageBody, Orientation.Landscape);

            return _converter.Convert(htmlToPdfDocument);
        }

        public byte[] ExportReports(DateTime from, DateTime to)
        {
            var reports = _context.ReportBuildings
                            .Include(i => i.Account)
                            .Where(r => r.CreateDate >= from && r.CreateDate <= to)
                            .ToList();
            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() + "ExportListTemplate.html";
            string HtmlBody = "";
            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }
            string body = "<tr>";
            for (int i = 0; i < Reports.Length; i++)
            {
                body += $"<th style=\"border:1px solid black;\">{Reports[i]}</th>";
            }
            body += "</tr>";
            foreach (var item in reports)
            {
                body += "<tr>";
                body += $"<td style=\"border: 1px solid black;\">{item.Id}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.Title}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.Content}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.CreateDate.ToShortDateString()}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.Account.UserName}</td>";
                body += "</tr>";
            }
            string messageBody = string.Format(HtmlBody, "Danh sách yêu tố cáo", body);

            HtmlToPdfDocument htmlToPdfDocument = PdfReturn(messageBody, Orientation.Landscape);

            return _converter.Convert(htmlToPdfDocument);
        }

        public byte[] ExportUnRentRequests(DateTime from, DateTime to)
        {
            var unRentRequests = _context.UnRentRequests.Where(r => r.Status == true && r.CreateDate >= from && r.CreateDate <= to).ToList();
            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() + "ExportListTemplate.html";
            string HtmlBody = "";
            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }
            string body = "<tr>";
            for (int i = 0; i < UnRentRequest.Length; i++)
            {
                body += $"<th style=\"border:1px solid black;\">{UnRentRequest[i]}</th>";
            }
            body += "</tr>";
            foreach (var item in unRentRequests)
            {
                body += "<tr>";
                body += $"<td style=\"border: 1px solid black;\">{item.Id}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.RenterId}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.ItemId}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.CreateDate.ToShortDateString()}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.FullName}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.CCCD}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.HandleTime.ToShortDateString()}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.HandlerId}</td>";
                body += "</tr>";
            }
            string messageBody = string.Format(HtmlBody, "Danh sách yêu cầu thuê", body);

            HtmlToPdfDocument htmlToPdfDocument = PdfReturn(messageBody, Orientation.Landscape);

            return _converter.Convert(htmlToPdfDocument);
        }

        public byte[] ExportVehicles(DateTime from, DateTime to)
        {
            var vehicles = _context.Vehicles.Where(r => r.Status == true && r.CreateDate >= from && r.CreateDate <= to).ToList();
            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() + "ExportListTemplate.html";
            string HtmlBody = "";
            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }
            string body = "<tr>";
            for (int i = 0; i < Vehicles.Length; i++)
            {
                body += $"<th style=\"border:1px solid black;\">{Vehicles[i]}</th>";
            }
            body += "</tr>";
            foreach (var item in vehicles)
            {
                body += "<tr>";
                body += $"<td style=\"border: 1px solid black;\">{item.Id}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.LicensePlates}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.Transportation}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.AccountName}</td>";
                body += $"<td style=\"border: 1px solid black;\">{item.CreateDate.ToShortDateString()}</td>";
                body += "</tr>";
            }
            string messageBody = string.Format(HtmlBody, "Danh sách yêu cầu thuê", body);

            HtmlToPdfDocument htmlToPdfDocument = PdfReturn(messageBody, Orientation.Landscape);

            return _converter.Convert(htmlToPdfDocument);
        }

        public byte[] GeneratePDF(PdfBillRequest request)
        {
            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() +
                "BillTemplate.html";

            string HtmlBody = "";
            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }
            string messageBody = string.Format(HtmlBody, request.CreateDate, request.BillId,
                DateTime.Now.Month, request.AccountName,
                request.AccountId, request.ItemPrice, request.ElectricPrice, request.WaterPrice,
                request.VehiclePrice, request.OrtherPrice, request.SumPrice(), request.WaterBillUrl, request.ElectricBillUrl);


            HtmlToPdfDocument htmlToPdfDocument = PdfReturn(messageBody, Orientation.Portrait);

            return _converter.Convert(htmlToPdfDocument);
        }

        private HtmlToPdfDocument PdfReturn(string messageBody, Orientation orientation)
        {
            GlobalSettings globalSettings = new GlobalSettings();
            globalSettings.ColorMode = ColorMode.Color;
            globalSettings.Orientation = orientation;
            globalSettings.PaperSize = PaperKind.A4;
            globalSettings.Margins = new MarginSettings { Top = 25, Bottom = 25 };
            ObjectSettings objectSettings = new ObjectSettings();
            objectSettings.PagesCount = true;
            objectSettings.HtmlContent = messageBody;
            WebSettings webSettings = new WebSettings();
            webSettings.DefaultEncoding = "utf-8";
            HeaderSettings headerSettings = new HeaderSettings();
            headerSettings.FontSize = 15;
            headerSettings.FontName = "Ariel";
            headerSettings.Right = "";
            headerSettings.Line = true;
            FooterSettings footerSettings = new FooterSettings();
            footerSettings.FontSize = 12;
            footerSettings.FontName = "Ariel";
            footerSettings.Center = "";
            footerSettings.Line = true;
            objectSettings.HeaderSettings = headerSettings;
            objectSettings.FooterSettings = footerSettings;
            objectSettings.WebSettings = webSettings;
            HtmlToPdfDocument htmlToPdfDocument = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings },
            };
            return htmlToPdfDocument;
        }
    }
}
