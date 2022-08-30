using API.Extensions;
using API.Helpers;
using API.Payloads.Requests;
using API.Payloads.Response;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class SystemController : BaseApiController
    {
        private readonly IAccountService _accountService;
        private readonly ITypeService _typeService;
        private readonly IItemService _itemService;

        public SystemController(IAccountService accountService,
            ITypeService typeService,
            IItemService itemService)
        {
            _accountService = accountService;
            _typeService = typeService;
            _itemService = itemService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.Register(request);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("confirm-email")]
        public async Task<ActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return NotFound();

            var result = await _accountService.ConfirmEmail(userId, token);

            if (result.IsSuccess)
            {
                return Redirect("http://localhost:5000/swagger/index.html");
            }
            else
            {
                await _accountService.RemoveUser(Int32.Parse(userId));
            }

            return BadRequest(result);
        }

        [Authorize]
        [HttpGet("currentUser")]
        public async Task<ActionResult> GetCurrentUser()
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.GetCurrentUser(User.Identity.Name);

                if (result != null)
                    return Ok(result);

                return BadRequest(result);
            }

            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.Login(request);
                if (result != null)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return NotFound();

            var result = await _accountService.ForgetPasswordAsync(email);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword(ResetPasswordRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountService.ResetPasswordAsync(request);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }

            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        //Type
        [HttpGet("types")]
        public async Task<ActionResult> GetTypes()
        {
            if (ModelState.IsValid)
            {
                var result = await _typeService.GetTypes();
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }
        [HttpGet("types/{id}")]
        public async Task<ActionResult> GetTypeById(int id)
        {
            if (ModelState.IsValid)
            {
                var result = await _typeService.GetTypeById(id);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest("Một số thuộc tính không hợp lệ");
        }

        [HttpGet("items")]
        public async Task<ActionResult> GetSystemItems([FromQuery] ItemForSystemParams itemParams)
        {
            if (ModelState.IsValid)
            {
                var result = await _itemService.GetItemsForSystem(itemParams);
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
        public async Task<ActionResult> GetItem(int id)
        {
            if (ModelState.IsValid)
            {
                var result = await _itemService.GetItem(id);
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
