using API.Data;
using API.Entities;
using API.Helpers;
using API.Services.Interfaces;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services.Implements
{
    public class ExportDataService : IExportDataService
    {
        private readonly string[] RentedItems = new string[] { "Id", "Type", "Location", "Price", "Renter", "RenterId", "RentedDate", "NumberOfParents" };
        private readonly string[] Reports = new string[] { "Id", "Title", "Content", "CreateDate", "Account" };
        private readonly string[] Vehicles = new string[] { "Id", "LicensePlates", "Transportation", "AccountName", "CreateDate" };
        private readonly string[] Members = new string[] { "Id", "FullName", "DateOfBirth", "Gender", "CCCD", "PlaceOfOrigin", "Nationality", "AccountId" };
        private readonly string[] Bills = new string[] { "Id", "Title", "ItemPrice", "ElectricPrice", "WaterPrice", "VehiclePrice", "OtherPrice", "ItemId", "CreateDate", "DatePaied", "AccountId", "SumPrice" };
        private readonly string[] RentRequest = new string[] { "Id", "RenterId", "ItemId", "CreateDate", "FullName", "CCCD", "NumberOfParent" };
        private readonly string[] UnRentRequest = new string[] { "Id", "RenterId", "ItemId", "CreateDate", "FullName", "CCCD", "HandleTime", "HandlerId" };
        private readonly string[] Accounts = new string[] { "Id", "Username", "Email", "PhoneNumber", "TotalItem", "NumberOfParent", "NumberOfVehicles", "CreateDate" };

        private readonly BuildingContext _context;
        private readonly UserManager<Account> _userManager;

        public ExportDataService(BuildingContext context, UserManager<Account> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public byte[] ExportRentedItem(DateTime from, DateTime to)
        {
            var items = _context.Items
                .Include(i => i.Type)
                .Include(i => i.Renter)
                .Where(i => i.Status == EItemStatus.Rented && i.RentedDate >= from && i.RentedDate <= to)
                .OrderBy(i => i.RentedDate)
                .ToList();
            using (var workBook = new XLWorkbook())
            {
                var worksheet = workBook.Worksheets.Add();
                var currentRow = 1;
                for (int i = 0; i < RentedItems.Length; i++)
                {
                    worksheet.Cell(currentRow, i + 1).Value = RentedItems[i];
                }
                foreach (var item in items)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.Id;
                    worksheet.Cell(currentRow, 2).Value = item.Type.Name;
                    worksheet.Cell(currentRow, 3).Value = item.Location;
                    worksheet.Cell(currentRow, 4).Value = item.Price;
                    worksheet.Cell(currentRow, 5).Value = item.Renter != null ? item.Renter.UserName : "";
                    worksheet.Cell(currentRow, 6).Value = item.Renter != null ? item.Renter.Id : "";
                    worksheet.Cell(currentRow, 7).Value = item.Renter != null ? item.RentedDate : "";
                    worksheet.Cell(currentRow, 8).Value = item.Renter != null ? item.Renter.NumberOfParent : "";
                }
                using (var stream = new MemoryStream())
                {
                    workBook.SaveAs(stream);
                    var content = stream.ToArray();
                    return content;
                }
            }
        }

        public byte[] ExportReports(DateTime from, DateTime to)
        {
            var reports = _context.ReportBuildings
                .Include(i => i.Account)
                .Where(b => b.CreateDate >= from && b.CreateDate <= to)
                .OrderBy(b => b.CreateDate)
                .ToList();
            using (var workBook = new XLWorkbook())
            {
                var worksheet = workBook.Worksheets.Add();
                var currentRow = 1;
                for (int i = 0; i < Reports.Length; i++)
                {
                    worksheet.Cell(currentRow, i + 1).Value = Reports[i];
                }
                foreach (var report in reports)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = report.Id;
                    worksheet.Cell(currentRow, 2).Value = report.Title;
                    worksheet.Cell(currentRow, 3).Value = report.Content;
                    worksheet.Cell(currentRow, 4).Value = report.CreateDate;
                    worksheet.Cell(currentRow, 5).Value = report.Account.UserName;
                }
                using (var stream = new MemoryStream())
                {
                    workBook.SaveAs(stream);
                    var content = stream.ToArray();
                    return content;
                }
            }
        }

        public byte[] ExportVehicles(DateTime from, DateTime to)
        {
            var vehicles = _context.Vehicles.Where(v => v.Status == true && v.CreateDate >= from && v.CreateDate <= to).OrderBy(b => b.CreateDate).ToList();
            using (var workBook = new XLWorkbook())
            {
                var worksheet = workBook.Worksheets.Add();
                var currentRow = 1;
                for (int i = 0; i < Vehicles.Length; i++)
                {
                    worksheet.Cell(currentRow, i + 1).Value = Vehicles[i];
                }
                foreach (var vehicle in vehicles)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = vehicle.Id;
                    worksheet.Cell(currentRow, 2).Value = vehicle.LicensePlates;
                    worksheet.Cell(currentRow, 3).Value = vehicle.Transportation;
                    worksheet.Cell(currentRow, 4).Value = vehicle.AccountName;
                    worksheet.Cell(currentRow, 5).Value = vehicle.CreateDate;
                }
                using (var stream = new MemoryStream())
                {
                    workBook.SaveAs(stream);
                    var content = stream.ToArray();
                    return content;
                }
            }
        }

        public byte[] ExportMembers(DateTime from, DateTime to)
        {
            var members = _context.Members.Include(m => m.PlaceOfOrigin).Where(b =>b.Status == true && b.CreateDate >= from && b.CreateDate <= to).OrderBy(b => b.AccountId).ToList();
            using (var workBook = new XLWorkbook())
            {
                var worksheet = workBook.Worksheets.Add();
                var currentRow = 1;
                for (int i = 0; i < Members.Length; i++)
                {
                    worksheet.Cell(currentRow, i + 1).Value = Members[i];
                }
                foreach (var member in members)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = member.Id;
                    worksheet.Cell(currentRow, 2).Value = member.FullName;
                    worksheet.Cell(currentRow, 3).Value = member.DateOfBirth;
                    worksheet.Cell(currentRow, 4).Value = member.Gender;
                    worksheet.Cell(currentRow, 5).Value = member.CCCD;
                    worksheet.Cell(currentRow, 6).Value = member.PlaceOfOrigin.SumAddress();
                    worksheet.Cell(currentRow, 7).Value = member.Nationality;
                    worksheet.Cell(currentRow, 8).Value = member.AccountId;
                }
                using (var stream = new MemoryStream())
                {
                    workBook.SaveAs(stream);
                    var content = stream.ToArray();
                    return content;
                }
            }
        }

        public byte[] ExportBills(DateTime from, DateTime to)
        {
            var bills = _context.Bills.Where(b => b.Paied == true && b.CreateDate >= from && b.CreateDate <= to).OrderBy(b => b.DatePaied).ToList();
            using (var workBook = new XLWorkbook())
            {
                var worksheet = workBook.Worksheets.Add();
                var currentRow = 1;
                for (int i = 0; i < Bills.Length; i++)
                {
                    worksheet.Cell(currentRow, i + 1).Value = Bills[i];
                }
                foreach (var bill in bills)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = bill.Id;
                    worksheet.Cell(currentRow, 2).Value = bill.Title;
                    worksheet.Cell(currentRow, 3).Value = bill.ItemPrice;
                    worksheet.Cell(currentRow, 4).Value = bill.ElectricPrice;
                    worksheet.Cell(currentRow, 5).Value = bill.WaterPrice;
                    worksheet.Cell(currentRow, 6).Value = bill.VehiclePrice;
                    worksheet.Cell(currentRow, 7).Value = bill.OtherPrice;
                    worksheet.Cell(currentRow, 8).Value = bill.ItemId;
                    worksheet.Cell(currentRow, 9).Value = bill.CreateDate;
                    worksheet.Cell(currentRow, 10).Value = bill.DatePaied;
                    worksheet.Cell(currentRow, 11).Value = bill.AccountId;
                    worksheet.Cell(currentRow, 12).Value = bill.SumPrice();
                }
                using (var stream = new MemoryStream())
                {
                    workBook.SaveAs(stream);
                    var content = stream.ToArray();
                    return content;
                }
            }
        }

        public byte[] ExportRentRequests(DateTime from, DateTime to)
        {
            var rentRequests = _context.RentRequests
                .Where(r => r.Status == true && r.CreateDate >= from && r.CreateDate <= to)
                .OrderBy(r => r.CreateDate)
                .ToList();
            using (var workBook = new XLWorkbook())
            {
                var worksheet = workBook.Worksheets.Add();
                var currentRow = 1;
                for (int i = 0; i < RentRequest.Length; i++)
                {
                    worksheet.Cell(currentRow, i + 1).Value = RentRequest[i];
                }
                foreach (var rentRequest in rentRequests)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = rentRequest.Id;
                    worksheet.Cell(currentRow, 2).Value = rentRequest.RenterId;
                    worksheet.Cell(currentRow, 3).Value = rentRequest.ItemId;
                    worksheet.Cell(currentRow, 4).Value = rentRequest.CreateDate;
                    worksheet.Cell(currentRow, 5).Value = rentRequest.FullName;
                    worksheet.Cell(currentRow, 6).Value = rentRequest.CCCD;
                    worksheet.Cell(currentRow, 7).Value = rentRequest.NumberOfParent;
                }
                using (var stream = new MemoryStream())
                {
                    workBook.SaveAs(stream);
                    var content = stream.ToArray();
                    return content;
                }
            }
        }

        public byte[] ExportUnRentRequests(DateTime from, DateTime to)
        {
            var unRentRequests = _context.UnRentRequests
                .Where(b => b.Status == true && b.CreateDate >= from && b.CreateDate <= to)
                .OrderBy(b => b.CreateDate)
                .ToList();
            using (var workBook = new XLWorkbook())
            {
                var worksheet = workBook.Worksheets.Add();
                var currentRow = 1;
                for (int i = 0; i < UnRentRequest.Length; i++)
                {
                    worksheet.Cell(currentRow, i + 1).Value = UnRentRequest[i];
                }
                foreach (var unRentRequest in unRentRequests)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = unRentRequest.Id;
                    worksheet.Cell(currentRow, 2).Value = unRentRequest.RenterId;
                    worksheet.Cell(currentRow, 3).Value = unRentRequest.ItemId;
                    worksheet.Cell(currentRow, 4).Value = unRentRequest.CreateDate;
                    worksheet.Cell(currentRow, 5).Value = unRentRequest.FullName;
                    worksheet.Cell(currentRow, 6).Value = unRentRequest.CCCD;
                    worksheet.Cell(currentRow, 7).Value = unRentRequest.HandleTime;
                    worksheet.Cell(currentRow, 8).Value = unRentRequest.HandlerId;
                }
                using (var stream = new MemoryStream())
                {
                    workBook.SaveAs(stream);
                    var content = stream.ToArray();
                    return content;
                }
            }
        }

        public async Task<byte[]> ExportAccounts(DateTime from, DateTime to)
        {
            var accounts = _context.Users
                .Include(i => i.Items)
                .Include(i => i.Vehicles)
                .Where(b => b.EmailConfirmed == true && b.CreateDate >= from && b.CreateDate <= to)
                .OrderBy(b => b.CreateDate)
                .ToList();
            List<Account> memberAccounts = new List<Account>();
            foreach (var account in accounts)
            {
                var roles = await _userManager.GetRolesAsync(account);
                if (!roles.Contains("Admin"))
                {
                    memberAccounts.Add(account);
                }
            }
            using (var workBook = new XLWorkbook())
            {
                var worksheet = workBook.Worksheets.Add();
                var currentRow = 1;
                for (int i = 0; i < Accounts.Length; i++)
                {
                    worksheet.Cell(currentRow, i + 1).Value = Accounts[i];
                }
                foreach (var account in memberAccounts)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = account.Id;
                    worksheet.Cell(currentRow, 2).Value = account.UserName;
                    worksheet.Cell(currentRow, 3).Value = account.Email;
                    worksheet.Cell(currentRow, 4).Value = account.PhoneNumber;
                    worksheet.Cell(currentRow, 5).Value = account.Items.Count;
                    worksheet.Cell(currentRow, 6).Value = account.NumberOfParent;
                    worksheet.Cell(currentRow, 7).Value = account.Vehicles.Count;
                    worksheet.Cell(currentRow, 8).Value = account.CreateDate;
                }
                using (var stream = new MemoryStream())
                {
                    workBook.SaveAs(stream);
                    var content = stream.ToArray();
                    return content;
                }
            }
        }
    }
}