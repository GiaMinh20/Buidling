using API.Data;
using API.Entities;
using API.Helpers;
using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services.Implements
{
    public class StatisticService : IStatisticService
    {
        private readonly BuildingContext _context;
        private readonly UserManager<Account> _userManager;

        public StatisticService(BuildingContext context, UserManager<Account> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<DataResponse<StatisticResponse>> GetStatisticOfBuilding()
        {
            var persons = (await _context.Members.ToListAsync()).Count;
            var vehicles = (await _context.Vehicles.ToListAsync()).Count;
            var rented = (await _context.Items.Where(i => i.Status == EItemStatus.Rented).ToListAsync()).Count;
            var rentRequest = (await _context.RentRequests.Where(r => r.CreateDate.Month == DateTime.Now.Month && r.CreateDate.Year == DateTime.Now.Year).ToListAsync()).Count;
            var unRentRequest = (await _context.UnRentRequests.Where(r => r.CreateDate.Month == DateTime.Now.Month && r.CreateDate.Year == DateTime.Now.Year).ToListAsync()).Count;
            var reports = (await _context.ReportBuildings.ToListAsync()).Count;
            var accounts = await _context.Users.ToListAsync();
            int numberOfAccount = accounts.Count;
            if (persons < 0 || vehicles < 0 || rented < 0 || rentRequest < 0 || numberOfAccount < 0)
                return new DataResponse<StatisticResponse> { IsSuccess = false, Message = "Lấy dữ liệu thất bại" };
            foreach (var account in accounts)
            {
                if ((await _userManager.GetRolesAsync(account)).Contains("Admin"))
                    numberOfAccount--;
            }
            return new DataResponse<StatisticResponse>
            {
                IsSuccess = true,
                Data = new StatisticResponse
                {
                    NumberOfAccount = numberOfAccount,
                    NumberOfPerson = persons,
                    NumberOfVehicle = vehicles,
                    NumberOfRented = rented,
                    NumberOfRentRequestInMonth = rentRequest,
                    NumberOfReport = reports
                }
            };
        }

        public async Task<DataResponse<StatisticResponse>> GetStatictisByTime(DateTime from, DateTime to)
        {
            var persons = (await _context.Members.Where(m => m.CreateDate >= from && m.CreateDate <= to).ToListAsync()).Count;
            var vehicles = (await _context.Vehicles.Where(m => m.CreateDate >= from && m.CreateDate <= to).ToListAsync()).Count;
            var rented = (await _context.Items.Where(i => i.Status == EItemStatus.Rented && i.RentedDate >= from && i.RentedDate <= to).ToListAsync()).Count;
            var rentRequest = (await _context.RentRequests.Where(r => r.CreateDate >= from && r.CreateDate <= to).ToListAsync()).Count;
            var unRentRequest = (await _context.UnRentRequests.Where(r => r.CreateDate >= from && r.CreateDate <= to).ToListAsync()).Count;
            var reports = (await _context.ReportBuildings.Where(r => r.CreateDate >= from && r.CreateDate <= to).ToListAsync()).Count;
            var accounts = await _context.Users.Where(r => r.CreateDate >= from && r.CreateDate <= to).ToListAsync();
            int numberOfAccount = accounts.Count;
            if (persons < 0 || vehicles < 0 || rented < 0 || rentRequest < 0 || numberOfAccount < 0)
                return new DataResponse<StatisticResponse> { IsSuccess = false, Message = "Lấy dữ liệu thất bại" };
            foreach (var account in accounts)
            {
                if ((await _userManager.GetRolesAsync(account)).Contains("Admin"))
                    numberOfAccount--;
            }
            return new DataResponse<StatisticResponse>
            {
                IsSuccess = true,
                Data = new StatisticResponse
                {
                    NumberOfAccount = numberOfAccount,
                    NumberOfPerson = persons,
                    NumberOfVehicle = vehicles,
                    NumberOfRented = rented,
                    NumberOfRentRequestInMonth = rentRequest,
                    NumberOfReport = reports
                }
            };
        }

        public async Task<DataResponse<MonthlyRevenueResponse>> GetMonthlyRevenue(DateTime from, DateTime to)
        {
            //var item = await _context.Items.Where(i => i.Status == EItemStatus.Rented && i.MonthlyPaiedDate >= from && i.MonthlyPaiedDate <= to).ToListAsync();
            var response = new MonthlyRevenueResponse();
            var bills = await _context.Bills.Where(b => b.DatePaied >= from && b.DatePaied <= to).ToListAsync();
            foreach (var bill in bills)
            {
                response.TotalRentPrice += (bill.ItemPrice + bill.VehiclePrice + bill.OtherPrice);
                response.TotalWaterPrice += bill.WaterPrice;
                response.TotalElectricPrice += bill.ElectricPrice;
                response.TotalPaidPrice += bill.SumPrice();
                response.TotalStriprFee += Math.Round((bill.SumPrice() * 0.029 + 0.3), 2);
                response.Revenue += Math.Round(response.TotalPaidPrice - response.TotalStriprFee, 2);
            }
            return new DataResponse<MonthlyRevenueResponse>
            {
                IsSuccess = true,
                Data = response
            };
        }
    }
}

