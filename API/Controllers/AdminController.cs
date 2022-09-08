using API.Extensions;
using API.Helpers;
using API.Payloads.Requests;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseApiController
    {
        private readonly ITypeService _typeService;
        private readonly IReportService _reportService;
        private readonly IRentRequestService _rentRequestService;
        private readonly IBillService _billService;
        private readonly IVehicleService _vehicleService;
        private readonly IAccountService _accountService;
        private readonly IStatisticService _statisticService;
        private readonly IMemberService _memberService;
        private readonly IExportDataService _exportDataService;
        private readonly IItemService _itemService;
        public AdminController(IItemService itemService,
            ITypeService typeService,
            IReportService reportService,
            IRentRequestService rentRequestService,
            IBillService billService,
            IVehicleService vehicleService,
            IAccountService accountService,
            IStatisticService statisticService,
            IMemberService memberService,
            IExportDataService exportDataService)
        {
            _typeService = typeService;
            _reportService = reportService;
            _rentRequestService = rentRequestService;
            _billService = billService;
            _vehicleService = vehicleService;
            _accountService = accountService;
            _statisticService = statisticService;
            _memberService = memberService;
            _exportDataService = exportDataService;
            _itemService = itemService;
        }

        [HttpPost("types")]
        public async Task<ActionResult> CreateItemType(CreateTypeRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _typeService.CreateType(request);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpPost("item/create")]
        public async Task<ActionResult> CreateItem([FromForm] CreateItemRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _itemService.CreateItem(request);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpPut("item/edit/{id}")]
        public async Task<ActionResult> EditItem(int id, [FromForm] EditItemRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _itemService.EditItem(id, request);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpDelete("item/delete/{id}")]
        public async Task<ActionResult> DeleteItem(int id)
        {
            if (ModelState.IsValid)
            {
                var result = await _itemService.DeleteItem(id);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("reports")]
        public async Task<ActionResult> GetReports([FromQuery] PaginationParams param)
        {
            if (ModelState.IsValid)
            {
                var result = await _reportService.GetReports(param);
                if (result.IsSuccess)
                {
                    var reports = result.Data.MetaData;
                    Response.AddPaginationHeader(reports.CurrentPage, reports.PageSize, reports.TotalCount, reports.TotalPages);
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("reports/{id}")]
        public async Task<ActionResult> GetReportDetail(int id)
        {
            if (ModelState.IsValid)
            {
                var result = await _reportService.GetReporDetail(id);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("rent-request")]
        public async Task<ActionResult> GetRentRequests([FromQuery] RentRequestParams param)
        {
            if (ModelState.IsValid)
            {
                var result = await _rentRequestService.GetRentRequest(param);
                if (result.IsSuccess)
                {
                    var reports = result.Data.MetaData;
                    Response.AddPaginationHeader(reports.CurrentPage, reports.PageSize, reports.TotalCount, reports.TotalPages);
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpPut("item/assign")]
        public async Task<ActionResult> AssignUserForItem(int userId, int itemId)
        {
            if (ModelState.IsValid)
            {
                var result = await _itemService.AssignUserForItem(userId, itemId);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("bills")]
        public async Task<ActionResult> GetBills([FromQuery] BillForAdminParam param)
        {
            if (ModelState.IsValid)
            {
                var result = await _billService.GetBillsByAdmin(param);
                if (result.IsSuccess)
                {
                    var reports = result.Data.MetaData;
                    Response.AddPaginationHeader(reports.CurrentPage, reports.PageSize, reports.TotalCount, reports.TotalPages);
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("vehicles")]
        public async Task<ActionResult> GetVehicles([FromQuery] VehicleParams param)
        {
            if (ModelState.IsValid)
            {
                var result = await _vehicleService.GetVehicles(param);
                if (result.IsSuccess)
                {
                    var reports = result.Data.MetaData;
                    Response.AddPaginationHeader(reports.CurrentPage, reports.PageSize, reports.TotalCount, reports.TotalPages);
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpPut("vehicles/accept")]
        public async Task<ActionResult> AcceptVehicle(int vehicleId)
        {
            if (ModelState.IsValid)
            {
                var result = await _vehicleService.AcceptVehicle(vehicleId);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("unpaied-items")]
        public async Task<ActionResult> GetUnpaiedItems([FromQuery] PaginationParams itemParams)
        {
            if (ModelState.IsValid)
            {
                var result = await _itemService.GetUnpaiedItems(itemParams);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("accounts")]
        public async Task<ActionResult> GetAccounts([FromQuery] AccountParams param)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.GetAccounts(param);
                if (result.IsSuccess)
                {
                    var reports = result.Data.MetaData;
                    Response.AddPaginationHeader(reports.CurrentPage, reports.PageSize, reports.TotalCount, reports.TotalPages);
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpPost("account-status/{id}")]
        public async Task<ActionResult> BanMemberAccount(int id)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.BanMemberAccount(id);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("items")]
        public async Task<ActionResult> GetSystemItems([FromQuery] ItemForAdminParams itemParams)
        {
            if (ModelState.IsValid)
            {
                var result = await _itemService.GetItemsForAdmin(itemParams);
                if (result.IsSuccess)
                {
                    var items = result.Data.MetaData;
                    Response.AddPaginationHeader(items.CurrentPage, items.PageSize, items.TotalCount, items.TotalPages);
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("items/{id}")]
        public async Task<ActionResult> GetItemDetailForAdmin(int id)
        {
            if (ModelState.IsValid)
            {
                var result = await _itemService.GetItemDetailForAdmin(id);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("statistic")]
        public async Task<ActionResult> GetStatisticOfBuilding()
        {
            if (ModelState.IsValid)
            {
                var result = await _statisticService.GetStatisticOfBuilding();
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("members")]
        public async Task<ActionResult> GetMembers([FromQuery] MemberParams param)
        {
            if (ModelState.IsValid)
            {
                var result = await _memberService.GetMembers(param);
                if (result.IsSuccess)
                {
                    var items = result.Data.MetaData;
                    Response.AddPaginationHeader(items.CurrentPage, items.PageSize, items.TotalCount, items.TotalPages);
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("unrent-request")]
        public async Task<ActionResult> GetUnRentRequests([FromQuery] RentRequestParams param)
        {
            if (ModelState.IsValid)
            {
                var result = await _rentRequestService.GetUnRentRequest(param);
                if (result.IsSuccess)
                {
                    var reports = result.Data.MetaData;
                    Response.AddPaginationHeader(reports.CurrentPage, reports.PageSize, reports.TotalCount, reports.TotalPages);
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpPut("item/unassign")]
        public async Task<ActionResult> UnAssignUserForItem(int userId, int itemId)
        {
            if (ModelState.IsValid)
            {
                var result = await _itemService.UnAssignUserForItem(userId, itemId, User.GetUserId());
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("statistic/{from}&{to}")]
        public async Task<ActionResult> GetStatisticByTime(DateTime from, DateTime to)
        {
            if (ModelState.IsValid)
            {
                var result = await _statisticService.GetStatictisByTime(from, to);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpPost("monthly-bill")]
        public async Task<ActionResult> SendMonthlyBillForUser(int itemId, [FromForm] CreateMonthlyBillRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _itemService.SendMonthlyBillForUser(itemId, request);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("revenue/{from}&{to}")]
        public async Task<ActionResult> GetMonthlyRevenue(DateTime from, DateTime to)
        {
            if (ModelState.IsValid)
            {
                var result = await _statisticService.GetMonthlyRevenue(from, to);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("export/item")]
        public ActionResult ExportItem()
        {
            if (ModelState.IsValid)
            {
                string time = DateTime.Now.ToString();
                return File(_exportDataService.ExportRentedItem(), "application/vnd.openxmltormats-officedocument.spreadsheetml.sheet", $"RentedItemExport_{time}.xlsx");
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("export/accounts")]
        public async Task<ActionResult> ExportAccounts()
        {
            if (ModelState.IsValid)
            {
                string time = DateTime.Now.ToString();
                return File(await _exportDataService.ExportAccounts(), "application/vnd.openxmltormats-officedocument.spreadsheetml.sheet", $"ExportAccounts_{time}.xlsx");
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("export/bills")]
        public ActionResult ExportBills()
        {
            if (ModelState.IsValid)
            {
                string time = DateTime.Now.ToString();
                return File(_exportDataService.ExportBills(), "application/vnd.openxmltormats-officedocument.spreadsheetml.sheet", $"ExportBills_{time}.xlsx");
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("export/members")]
        public ActionResult ExportMembers()
        {
            if (ModelState.IsValid)
            {
                string time = DateTime.Now.ToString();
                return File(_exportDataService.ExportMembers(), "application/vnd.openxmltormats-officedocument.spreadsheetml.sheet", $"ExportMembers_{time}.xlsx");
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("export/rent-requests")]
        public ActionResult ExportRentRequests()
        {
            if (ModelState.IsValid)
            {
                string time = DateTime.Now.ToString();
                return File(_exportDataService.ExportRentRequests(), "application/vnd.openxmltormats-officedocument.spreadsheetml.sheet", $"ExportRentRequests_{time}.xlsx");
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("export/reports")]
        public ActionResult ExportReports()
        {
            if (ModelState.IsValid)
            {
                string time = DateTime.Now.ToString();
                return File(_exportDataService.ExportReports(), "application/vnd.openxmltormats-officedocument.spreadsheetml.sheet", $"ExportReports_{time}.xlsx");
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("export/unrent-requests")]
        public ActionResult ExportUnRentRequests()
        {
            if (ModelState.IsValid)
            {
                string time = DateTime.Now.ToString();
                return File(_exportDataService.ExportUnRentRequests(), "application/vnd.openxmltormats-officedocument.spreadsheetml.sheet", $"ExportUnRentRequests_{time}.xlsx");
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("export/vehicle")]
        public ActionResult ExportVehicles()
        {
            if (ModelState.IsValid)
            {
                string time = DateTime.Now.ToString();
                return File(_exportDataService.ExportVehicles(), "application/vnd.openxmltormats-officedocument.spreadsheetml.sheet", $"ExportVehicles_{time}.xlsx");
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }
    }
}
