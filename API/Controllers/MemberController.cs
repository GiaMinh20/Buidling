using API.Extensions;
using API.Helpers;
using API.Payloads.Requests;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize(Roles = "Member")]
    public class MemberController : BaseApiController
    {
        private readonly IAccountService _accountService;
        private readonly IFavoriteService _favoriteService;
        private readonly ICommentService _commentService;
        private readonly IReportService _reportService;
        private readonly IRentRequestService _rentRequestService;
        private readonly IMemberService _memberService;
        private readonly IPaymentService _paymentService;
        private readonly IVehicleService _vehicleService;
        private readonly INotificationService _notificationService;
        private readonly IBillService _billService;
        private readonly IItemService _itemService;

        public MemberController(IAccountService accountService,
            IFavoriteService favoriteService,
            ICommentService commentService,
            IReportService reportService,
            IRentRequestService rentRequestService,
            IMemberService memberService,
            IPaymentService paymentService,
            IVehicleService vehicleService,
            INotificationService notificationService,
            IBillService billService,
            IItemService itemService)
        {
            _accountService = accountService;
            _favoriteService = favoriteService;
            _commentService = commentService;
            _reportService = reportService;
            _rentRequestService = rentRequestService;
            _memberService = memberService;
            _paymentService = paymentService;
            _vehicleService = vehicleService;
            _notificationService = notificationService;
            _billService = billService;
            _itemService = itemService;
        }


        [HttpGet("profile")]
        public async Task<ActionResult> GetProfile()
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.GetProfile(User.GetUsername());
                if (result != null)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("profile-member")]
        public async Task<ActionResult> GetMemberOfAccount()
        {
            if (ModelState.IsValid)
            {
                var result = await _memberService.GetMemberByUsername(User.GetUsername());
                if (result != null)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("profile-bills")]
        public async Task<ActionResult> GetBillOfAccount([FromQuery] BillForAccountParams param)
        {
            if (ModelState.IsValid)
            {
                var result = await _billService.GetBillsByUser(User.GetUserId(), param);
                if (result != null)
                {
                    var bills = result.Data.MetaData;
                    Response.AddPaginationHeader(bills.CurrentPage, bills.PageSize, bills.TotalCount, bills.TotalPages);
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("profile-items")]
        public async Task<ActionResult> GetItemOfAccount([FromQuery] ItemForSystemParams param)
        {
            if (ModelState.IsValid)
            {
                var result = await _itemService.GetItemsForAccount(User.GetUserId(), param);
                if (result != null)
                {
                    var items = result.Data.MetaData;
                    Response.AddPaginationHeader(items.CurrentPage, items.PageSize, items.TotalCount, items.TotalPages);
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("profile-vehicles")]
        public async Task<ActionResult> GetVehicleOfAccount([FromQuery] VehicleForAccountParams param)
        {
            if (ModelState.IsValid)
            {
                var result = await _vehicleService.GetVehiclesOfAccount(User.GetUserId(), param);
                if (result.IsSuccess)
                {
                    var vehicles = result.Data.MetaData;
                    Response.AddPaginationHeader(vehicles.CurrentPage, vehicles.PageSize, vehicles.TotalCount, vehicles.TotalPages);
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpPut("edit-profile")]
        public async Task<ActionResult> EditProfile([FromForm] EditProfileRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.EditProfile(User.GetUsername(), request);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpPost("save-favorite")]
        public async Task<ActionResult> AddItemToFavorites(int itemId)
        {
            if (ModelState.IsValid)
            {
                int userId = User.GetUserId();
                var result = await _favoriteService.AddItemToFavorites(itemId, userId);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("favorite")]
        public async Task<ActionResult> GetFavorite()
        {
            if (ModelState.IsValid)
            {
                var result = await _favoriteService.GetFavoritesByUserId(User.GetUserId());
                if (result != null)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpPost("comment")]
        public async Task<ActionResult> CommentItem(int itemId, string content)
        {
            if (ModelState.IsValid)
            {
                var result = await _commentService.CommentItem(User.GetUserId(), itemId, content);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpPost("report")]
        public async Task<ActionResult> SendReport([FromForm] ReportRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _reportService.SendReport(User.GetUserId(), request);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpPost("rent-request")]
        public async Task<ActionResult> SendRentRequest([FromForm] CreateRentRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _rentRequestService.SendRentRequest(User.GetUserId(), request);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpPost("member")]
        public async Task<ActionResult> AddMemberForAccount([FromForm] CreateMemberRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _memberService.AddMemberForAccount(User.GetUserId(), request);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpPost("vehicle-request")]
        public async Task<ActionResult> SendVehicleRequest([FromForm] CreateVehicleRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _vehicleService.SendVehicleRequest(User.GetUserId(), request);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("notifications")]
        public async Task<ActionResult> GetNotifications()
        {
            if (ModelState.IsValid)
            {
                var result = await _notificationService.GetAllNotifyOfAccount(User.GetUserId());
                if (result != null)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpPost("unrent-request")]
        public async Task<ActionResult> SendUnRentRequest([FromForm] CreateUnRentRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _rentRequestService.SendUnRentRequest(User.GetUserId(), request);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpPost("payment")]
        public async Task<ActionResult> PayRentMoney(PayRentMoney request)
        {
            if (ModelState.IsValid)
            {
                var result = await _paymentService.PayRentMoney(User.GetUserId(), request);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }
    }
}
