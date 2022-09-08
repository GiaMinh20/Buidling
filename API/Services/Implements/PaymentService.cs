using API.Data;
using API.Helpers;
using API.Payloads.Requests;
using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Threading.Tasks;

namespace API.Services.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly BuildingContext _context;

        public PaymentService(IConfiguration config,
            BuildingContext context)
        {
            _config = config;
            _context = context;
        }

        public async Task<DataResponse<PaymentResponse>> PayRentMoney(int userId, PayRentMoney payRentMoney)
        {
            var bill = await _context.Bills.FirstOrDefaultAsync(b => b.AccountId == userId && b.Id == payRentMoney.BllId && b.Paied == false);
            if (bill == null)
                return new DataResponse<PaymentResponse> { IsSuccess = false, Message = "Không tìm thấy hóa đơn" };
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == bill.ItemId);
            var payRs = await PayAsync(payRentMoney.CardNumber, payRentMoney.Month, payRentMoney.Year, payRentMoney.CVC, $"Thanh toán hóa đơn {bill.Id}", bill.SumPrice(), bill.Title);
            if (!payRs.IsSuccess)
                return new DataResponse<PaymentResponse> { IsSuccess = false, Message = "Thanh toán thất bại" };
            item.Status = EItemStatus.Rented;
            item.MonthlyPaied = true;
            item.MonthlyPaiedDate = DateTime.Now;
            bill.Paied = true;
            bill.DatePaied = DateTime.Now;
            if (await _context.SaveChangesAsync() > 0) 
            {
                var rs = new PaymentResponse
                {
                    AccountId = userId,
                    ItemPrice = bill.ItemPrice,
                    ElectricPrice = bill.ElectricPrice,
                    WaterPrice = bill.WaterPrice,
                    VehiclePrice = bill.VehiclePrice,
                    OtherPrice = bill.OtherPrice,
                    TotalPrice = bill.SumPrice(),
                    ItemId = item.Id,
                    Title = bill.Title,
                    Time = DateTime.Now
                };
                return new DataResponse<PaymentResponse> { IsSuccess = true, Data = rs };
            }
            return new DataResponse<PaymentResponse> { IsSuccess = false, Message = "Thanh toán thất bại" };
        }

        private async Task<BaseResponse> PayAsync(string cardnumber, int month, int year, string cvc, string name, int amount, string description)
        {
            try
            {
                StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];
                var optionstoken = new TokenCreateOptions
                {
                    Card = new CreditCardOptions
                    {
                        Number = cardnumber,
                        ExpMonth = month,
                        ExpYear = year,
                        Cvc = cvc,
                        Name = name,
                        Currency = "usd",
                    }
                };
                var servicetoken = new TokenService();
                Token stripetoken = await servicetoken.CreateAsync(optionstoken);
                var options = new ChargeCreateOptions
                {
                    Amount = amount * 100,
                    Currency = "usd",
                    Description = description,
                    Source = stripetoken.Id
                };
                var service = new ChargeService();
                Charge charge = await service.CreateAsync(options);
                if (charge.Paid)
                {
                    return new BaseResponse { IsSuccess = true };
                }
                return new BaseResponse { IsSuccess = false };
            }
            catch (System.Exception e)
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }
    }
}
