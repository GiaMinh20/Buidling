using API.Data;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services.Implements
{
    public class BillService : IBillService
    {
        private readonly BuildingContext _context;
        private readonly IMapper _mapper;

        public BillService(BuildingContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Bill createBill(int userId, int itemId, string title, int itemPrice, int electricPrice, int waterPrice, int vehiclePrice, int otherPrice)
        {
            return new Bill
            {
                AccountId = userId,
                ItemId = itemId,
                ItemPrice = itemPrice,
                ElectricPrice = electricPrice,
                WaterPrice = waterPrice,
                VehiclePrice = vehiclePrice,
                OtherPrice = otherPrice,
                Paied = false,
                Title = title
            };

        }


        public async Task<DataResponse<PagedList<BillForAdminResponse>>> GetBillsByAdmin(BillForAdminParam param)
        {
            var query = await _context.Bills
                .Paied(param.Paied)
                .Account(param.AccountId)
                .Item(param.ItemId)
                .Bill(param.BillId)
                .ToListAsync();
            var bills = _mapper.Map<List<BillForAdminResponse>>(query);

            var response = PagedList<BillForAdminResponse>.ToPagedList(bills,
                param.PageNumber, param.PageSize);

            return new DataResponse<PagedList<BillForAdminResponse>>
            {
                IsSuccess = true,
                Data = response
            };
        }
    }
}
