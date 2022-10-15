using API.Data;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Payloads.Requests;
using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using API.Services.Interfaces;
using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace API.Services.Implements
{
    public class ItemService : IItemService
    {
        private readonly BuildingContext _context;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IPhotoService _photoService;
        private readonly ICommentService _commentService;
        private readonly IBillService _billService;
        private readonly INotificationService _notificationService;
        private readonly IVehicleService _vehicleService;
        private readonly IRentRequestService _rentRequestService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPdfService _pdfService;

        public ItemService(BuildingContext context,
            IImageService imageService,
            IMapper mapper,
            IEmailService emailService,
            IPhotoService photoService,
            ICommentService commentService,
            IBillService billService,
            INotificationService notificationService,
            IVehicleService vehicleService,
            IRentRequestService rentRequestService,
            IWebHostEnvironment webHostEnvironment,
            IPdfService pdfService)
        {
            _context = context;
            _imageService = imageService;
            _mapper = mapper;
            _emailService = emailService;
            _photoService = photoService;
            _commentService = commentService;
            _billService = billService;
            _notificationService = notificationService;
            _vehicleService = vehicleService;
            _rentRequestService = rentRequestService;
            _webHostEnvironment = webHostEnvironment;
            _pdfService = pdfService;
        }

        public async Task<BaseResponse> CreateItem(CreateItemRequest request)
        {
            var item = _mapper.Map<Item>(request);
            item.Status = EItemStatus.Empty;
            item.Type = await _context.TypeItems.FindAsync(request.TypeId);
            await uploadMedia(item, request.VideoUrl, request.PictureUrl, request.AvatarUrl);
            _context.Items.Add(item);
            if (await _context.SaveChangesAsync() > 0)
            {
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "Đăng tin thành công",
                };
            }
            return new BaseResponse
            {
                IsSuccess = false,
                Message = "Đăng tin thất bại"
            };
        }

        public async Task<BaseResponse> DeleteItem(int id)
        {
            var item = await _context.Items.Include(i => i.Renter).FirstOrDefaultAsync(i => i.Renter == null);
            if (item == null)
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy tin tức"
                };
            _context.Items.Remove(item);
            if (await _context.SaveChangesAsync() > 0)
            {
                await deleteItemPhoto(item);
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "Xóa thành công",
                };
            }
            return new BaseResponse
            {
                IsSuccess = false,
                Message = "Xóa thất bại"
            };
        }

        public async Task<DataResponse<ItemResponse>> EditItem(EditItemRequest request)
        {
            var item = checkEditRequest(request).Result.Data;
            if (item != null)
            {
                await uploadMedia(item, request.VideoUrl, request.PictureUrl, request.AvatarUrl);
                _context.Items.Update(item);
                if (await _context.SaveChangesAsync() > 0)
                {
                    var response = _mapper.Map<ItemResponse>(item);
                    response.PictureUrl = new List<string>();
                    foreach (var pic in item.ItemPhotos)
                    {
                        response.PictureUrl.Add(pic.Url.ToString());
                    }
                    return new DataResponse<ItemResponse>
                    {
                        IsSuccess = true,
                        Message = "Cập nhật thành công",
                        Data = response
                    };
                }
            }
            return new DataResponse<ItemResponse>
            {
                IsSuccess = false,
                Message = "Cập nhật thất bại"
            };
        }

        public async Task<DataResponse<PagedList<ItemForSystemResponse>>> GetItemsForSystem(ItemForSystemParams itemParams)
        {
            var query = await _context.Items
                .Include(i => i.Type)
                .Include(i => i.Renter)
                .Include(i => i.ItemPhotos)
                .Status(EItemStatus.Empty)
                .Sort(itemParams.OrderBy)
                .Search(itemParams.SearchTerm)
                .Filter(itemParams.Types)
                .ToListAsync();
            List<ItemForSystemResponse> responses = _mapper.Map<List<ItemForSystemResponse>>(query);

            var items = PagedList<ItemForSystemResponse>.ToPagedList(responses,
                itemParams.PageNumber, itemParams.PageSize);


            return new DataResponse<PagedList<ItemForSystemResponse>>
            {
                IsSuccess = true,
                Data = items
            };
        }

        public async Task<DataResponse<ItemDetailForSystemResponse>> GetItem(int id)
        {
            var item = await _context.Items
                .Include(i => i.Type)
                .FirstOrDefaultAsync(i => i.Id == id && i.Status == EItemStatus.Empty);
            if (item == null)
                return new DataResponse<ItemDetailForSystemResponse>
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy"
                };
            var response = _mapper.Map<ItemDetailForSystemResponse>(item);
            response.PictureUrl = _photoService.GetPhotoUrlsByItemId(id).Result.Datas;
            response.Comments = await _commentService.GetCommentsByItemId(id);
            return new DataResponse<ItemDetailForSystemResponse>
            {
                IsSuccess = true,
                Data = response
            };
        }

        public async Task<DataResponse<PagedList<ItemForSystemResponse>>> GetItemsForAccount(int userId, ItemForSystemParams itemParams)
        {
            var query = await _context.Items
                .Include(i => i.Type)
                .Include(i => i.Renter)
                .Include(i => i.ItemPhotos)
                .Where(i => i.Renter.Id == userId)
                .OrderByDescending(i => i.RentedDate)
                .ToListAsync();
            List<ItemForSystemResponse> responses = _mapper.Map<List<ItemForSystemResponse>>(query);

            var items = PagedList<ItemForSystemResponse>.ToPagedList(responses,
                itemParams.PageNumber, itemParams.PageSize);


            return new DataResponse<PagedList<ItemForSystemResponse>>
            {
                IsSuccess = true,
                Data = items
            };
        }

        public async Task<BaseResponse> AssignUserForItem(AssignUserForItemRequest request)
        {
            var renter = await _context.Users.FindAsync(request.UserId);
            var item = await _context.Items
                .Include(i => i.Renter)
                .FirstOrDefaultAsync(i => i.Id == request.ItemId);
            var rentRequest = await _context.RentRequests.FirstOrDefaultAsync(i => i.ItemId == request.ItemId && i.RenterId == renter.Id && i.Status == false);
            if (rentRequest == null || item == null || rentRequest == null)
                return new BaseResponse { IsSuccess = false, Message = "Chỉ định thất bại" };

            if (item.Renter == null)
            {
                item.Renter = renter;
                item.RentedDate = DateTime.Today;
                item.MonthlyPaiedDate = DateTime.Today;
                item.MonthlyPaied = true;
                item.Status = EItemStatus.Rented;
                renter.NumberOfParent = rentRequest.NumberOfParent;
                rentRequest.Status = true;
            }
            try
            {
                var rs = await _context.SaveChangesAsync() > 0;
                if (rs)
                {
                    await _emailService.SendEmailAsync(renter.Email, "Rent Successful", $"<html><body><p>Chào {renter.UserName},</p><p>$\"Bạn đã được chỉ định căn hộ {{item.Name}}\"</p></body></html>", null);
                    await _notificationService.PostNotificatonForAccount(2, new CreateNotificationRequest { Title = "Thuê thành công", Content = $"Bạn đã được chỉ định căn hộ {item.Name}" }, request.UserId);
                    return new BaseResponse { IsSuccess = true, Message = "Chỉ định thành công" };
                }
            }
            catch (DbUpdateConcurrencyException e)
            {
                return new BaseResponse { IsSuccess = false, Message = "Chỉ định thất bại" + e.Message };
            }

            return new BaseResponse { IsSuccess = false, Message = "Chỉ định thất bại" };
        }

        public async Task<DataResponse<PagedList<ItemForAdminResponse>>> GetItemsForAdmin(ItemForAdminParams itemParams)
        {
            var query = await _context.Items
                .Include(i => i.Type)
                .Include(i => i.Renter)
                .Include(i => i.ItemPhotos)
                .Status(itemParams.Status)
                .Sort(itemParams.OrderBy)
                .Search(itemParams.SearchTerm)
                .Filter(itemParams.Types)
                .ToListAsync();
            List<ItemForAdminResponse> responses = _mapper.Map<List<ItemForAdminResponse>>(query);
            var items = PagedList<ItemForAdminResponse>.ToPagedList(responses,
                itemParams.PageNumber, itemParams.PageSize);
            return new DataResponse<PagedList<ItemForAdminResponse>>
            {
                IsSuccess = true,
                Data = items
            };
        }

        public async Task<DataResponse<ItemDetailForAdminResponse>> GetItemDetailForAdmin(int id)
        {
            var item = await _context.Items
                .Include(i => i.Type)
                .Include(i => i.Renter)
                .FirstOrDefaultAsync(i => i.Id == id);
            if (item == null)
                return new DataResponse<ItemDetailForAdminResponse>
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy"
                };
            var response = _mapper.Map<ItemDetailForAdminResponse>(item);
            response.PictureUrl = _photoService.GetPhotoUrlsByItemId(id).Result.Datas;
            response.Comments = await _commentService.GetCommentsByItemId(id);
            return new DataResponse<ItemDetailForAdminResponse>
            {
                IsSuccess = true,
                Data = response
            };
        }

        public async Task<BaseResponse> UnAssignUserForItem(int userId, int itemId, int adminId, bool hasRequest)
        {

            var item = await _context.Items
                .Include(i => i.Renter)
                .FirstOrDefaultAsync(i => i.Id == itemId);
            var renter = await _context.Users
                .Include(u => u.Members)
                .FirstOrDefaultAsync(u => u.Id == userId);
            var admin = await _context.Users.FindAsync(adminId);
            var oldUnRent = await _context.UnRentRequests.FirstOrDefaultAsync(i => i.ItemId == itemId && i.RenterId == renter.Id && i.Status == false);
            if (!hasRequest && oldUnRent == null)
            {
                CreateUnRentRequest request = new CreateUnRentRequest { FullName = $"Admin: {admin.UserName}", CCCD = "111111111", ItemId = itemId };
                await _rentRequestService.SendUnRentRequest(item.Renter.Id, request);
            }
            else if (!hasRequest && oldUnRent != null)
            {
                return new BaseResponse { IsSuccess = false, Message = "Yêu cầu có tồn tại nhưng quản trị viên xác nhận yêu cầu không tồn tại" };
            }
            var unRentRequest = await _context.UnRentRequests.FirstOrDefaultAsync(i => i.ItemId == itemId && i.RenterId == renter.Id && i.Status == false);
            if (renter == null || item == null || unRentRequest == null)
                return new BaseResponse { IsSuccess = false, Message = "Hủy thuê thất bại vì không tìm thấy đủ thông tin" };
            var vehicles = await _context.Vehicles.Where(v => v.AccountId == renter.Id).ToListAsync();

            item.Renter = null;
            item.Status = EItemStatus.Empty;
            item.MonthlyPaied = false;
            item.RentedDate = null;

            unRentRequest.Status = true;
            unRentRequest.HandlerId = adminId;
            unRentRequest.HandleTime = DateTime.Now;

            foreach (var member in renter.Members)
            {
                _context.Members.Remove(member);
            }
            foreach (var vehicle in vehicles)
            {
                _context.Vehicles.Remove(vehicle);
            }

            renter.NumberOfParent = 0;
            renter.Members = null;

            var notification = await _notificationService.CreateNotification("Yêu cầu hủy", $"Đã xử lý yêu cầu hủy thuê phòng {item.Name} của bạn", userId);
            _context.Notifications.Add(notification);
            if (await _context.SaveChangesAsync() > 0)
            {
                await _emailService.SendEmailAsync(renter.Email, "UnRentSuccess", $"<html><body><p>Chào {renter.UserName},</p><p>Đã xử lý yêu cầu hủy thuê</p></body></html>", null);
                await _notificationService.PostNotificatonForAccount(2, new CreateNotificationRequest { Title = "Hủy thuê thành công", Content = $"Bạn đã được loại khỏi căn hộ {item.Name}" }, userId);
                return new BaseResponse { IsSuccess = true, Message = "Hủy thuê thành công" };
            }
            return new BaseResponse { IsSuccess = false, Message = "Hủy thuê thất bại" };
        }

        public async Task<DataResponse<PagedList<ItemUnpaiedResponse>>> GetUnpaiedItems(PaginationParams itemParams)
        {
            var query = await _context.Items
                .Include(i => i.Renter)
                .Where(i => i.Status == EItemStatus.Rented
                    && i.Renter != null
                    && (i.MonthlyPaied == false || (
                        (
                            (i.MonthlyPaiedDate.Value.Day > DateTime.Today.Day) && (DateTime.Today.Month - i.MonthlyPaiedDate.Value.Month == 2)
                        ) ||
                        (
                            (i.MonthlyPaiedDate.Value.Day <= DateTime.Today.Day) && (DateTime.Today.Month - i.MonthlyPaiedDate.Value.Month == 1)
                        )
                        && (i.MonthlyPaiedDate.Value.Year <= DateTime.Today.Year)
                        )
                    ))
                .ToListAsync();
            var responses = _mapper.Map<List<ItemUnpaiedResponse>>(query);

            foreach (var item in responses)
            {
                var bill = await _context.Bills
                    .Where(b => b.ItemId == item.Id && b.Paied == false && (b.CreateDate.Month == DateTime.Now.Month && b.CreateDate.Year == DateTime.Now.Year))
                    .FirstOrDefaultAsync();
                if (bill != null)
                    item.HasMonthlyBill = true;

            }
            for (int i = 0; i < responses.Count(); i++)
            {
                int x = (DateTime.Now - query[i].MonthlyPaiedDate.Value).Days;
                int y = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                int lated = (DateTime.Now - query[i].MonthlyPaiedDate.Value).Days - DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                responses[i].LatedDay = lated;
                query[i].MonthlyPaied = false;
            }
            await _context.SaveChangesAsync();
            var items = PagedList<ItemUnpaiedResponse>.ToPagedList(responses,
                itemParams.PageNumber, itemParams.PageSize);
            return new DataResponse<PagedList<ItemUnpaiedResponse>>
            {
                IsSuccess = true,
                Data = items
            };
        }

        public async Task<DataResponse<byte[]>> SendMonthlyBillForUser(int adminId, int itemId, CreateMonthlyBillRequest request)
        {
            var item = await _context.Items
                .Include(i => i.Renter)
                .Where(i => i.MonthlyPaied == false && i.Status == EItemStatus.Rented && i.Renter != null)
                .FirstOrDefaultAsync(i => i.Id == itemId);
            if (item == null) return new DataResponse<byte[]> { IsSuccess = false, Message = "Thất bại" };
            var electricBill = await _imageService.AddImageAsync(request.ElectricBillUrl);
            var waterBill = await _imageService.AddImageAsync(request.WaterBillUrl);

            var bill = _billService.createBill(item.Renter.Id, item.Id, "Hóa đơn hàng tháng", item.Price,
                request.ElectricPrice, electricBill.SecureUrl.ToString(), request.WaterPrice, waterBill.SecureUrl.ToString(), request.VehiclePrice, request.OtherPrice);
            CreateNotificationRequest createNotificationRequest = new CreateNotificationRequest { Title = "Tiền hàng tháng", Content = $"Bạn cần thanh toán số tiền :{bill.SumPrice()}" };
            var notification = await _notificationService.PostNotificatonForAccount(adminId, createNotificationRequest, item.Renter.Id);

            _context.Bills.Add(bill);

            if (await _context.SaveChangesAsync() > 0)
            {
                /* Template*/
                var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() +
                "BillEmailTemplate.html";

                string HtmlBody = "";
                using (StreamReader sr = File.OpenText(PathToTemplate))
                {
                    HtmlBody = sr.ReadToEnd();
                }
                PdfBillRequest pdfBillRequest = new PdfBillRequest
                {
                    BillId = bill.Id,
                    AccountId = item.Renter.Id,
                    AccountName = item.Renter.UserName,
                    CreateDate = DateTime.Now,
                    ElectricBillUrl = bill.ElectricBillUrl,
                    ElectricPrice = bill.ElectricPrice,
                    ItemPrice = bill.ItemPrice,
                    OrtherPrice = bill.OtherPrice,
                    VehiclePrice = bill.VehiclePrice,
                    WaterBillUrl = bill.WaterBillUrl,
                    WaterPrice = bill.WaterPrice
                };
                var billPdf = _pdfService.GeneratePDF(pdfBillRequest);
                string messageBody = string.Format(HtmlBody,DateTime.Now.Month.ToString(), bill.Id, bill.ItemPrice, bill.ElectricPrice, bill.WaterPrice, bill.VehiclePrice, bill.OtherPrice, bill.ElectricBillUrl, bill.WaterBillUrl);
                await _emailService.SendEmailAsync(item.Renter.Email, "Tiền thuê hàng tháng", messageBody, billPdf);
                return new DataResponse<byte[]> { IsSuccess = true, Message = "Gửi nhắc nhở thành công", Data = billPdf };
            }

            return new DataResponse<byte[]> { IsSuccess = false, Message = "Gửi nhắc nhở thất bại" };

        }

        /*------------Private Function------------*/
        private async Task<DataResponse<Item>> checkEditRequest(EditItemRequest request)
        {
            var item = await _context.Items
                .Include(i => i.Type)
                .Include(i => i.Renter)
                .Include(i => i.ItemPhotos)
                .Where(i => i.Renter == null)
                .FirstOrDefaultAsync(i => i.Id == request.Id);
            if (item == null)
                return new DataResponse<Item>
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy tin tức"
                };
            if (!String.IsNullOrWhiteSpace(request.Name))
                item.Name = request.Name;
            if (!String.IsNullOrEmpty(request.Description))
                item.Description = request.Description;
            if (request.Price >= 10000)
                item.Price = request.Price;
            if (request.Status == EItemStatus.Empty && item.Renter != null)
            {
                item.Renter = null;
                item.Status = request.Status;
            }
            return new DataResponse<Item>
            {
                IsSuccess = true,
                Data = item
            };
        }
        private async Task<DataResponse<Item>> uploadMedia(Item item, IFormFile videoUrl, IFormFileCollection imageUrl, IFormFile avatarUrl)
        {
            if (imageUrl != null)
            {
                if (item.ItemPhotos != null)
                {
                    foreach (var photo in item.ItemPhotos)
                    {
                        await _imageService.DeleteMediaAsync(photo.PublicId);
                    }
                }
                item.ItemPhotos = new List<ItemPhoto>();
                foreach (var img in imageUrl)
                {
                    var imageResult = await _imageService.AddImageAsync(img);
                    if (imageResult.Error != null)
                        return new DataResponse<Item>
                        {
                            IsSuccess = false,
                            Message = "Lỗi tải hình ảnh"
                        };
                    var photo = new ItemPhoto { PublicId = imageResult.PublicId, Url = imageResult.SecureUrl.ToString(), Item = item, ItemId = item.Id };
                    item.ItemPhotos.Add(photo);
                }
            }
            if (videoUrl != null)
            {
                if (!string.IsNullOrEmpty(item.VideoId))
                {
                    await _imageService.DeleteMediaAsync(item.VideoId);
                }

                var videoResult = await _imageService.AddVideoAsync(videoUrl);

                if (videoResult.Error != null)
                    return new DataResponse<Item>
                    {
                        IsSuccess = false,
                        Message = "Lỗi tải video"
                    };
                item.VideoUrl = videoResult.SecureUrl.ToString();
                item.VideoId = videoResult.PublicId;
            }

            if (avatarUrl != null)
            {
                if (!string.IsNullOrEmpty(item.AvatarId))
                {
                    await _imageService.DeleteMediaAsync(item.AvatarId);
                }

                var avatarResult = await _imageService.AddImageAsync(avatarUrl);

                if (avatarResult.Error != null)
                    return new DataResponse<Item>
                    {
                        IsSuccess = false,
                        Message = "Lỗi tải video"
                    };
                item.AvatarUrl = avatarResult.SecureUrl.ToString();
                item.AvatarId = avatarResult.PublicId;
            }

            return new DataResponse<Item>
            {
                IsSuccess = true,
                Data = item
            };
        }
        private async Task deleteItemPhoto(Item item)
        {
            if (!string.IsNullOrEmpty(item.VideoId))
                await _imageService.DeleteMediaAsync(item.VideoId);
            if (item.ItemPhotos != null)
            {
                foreach (var photo in item.ItemPhotos)
                {
                    await _imageService.DeleteMediaAsync(photo.PublicId);
                    _context.ItemPhotos.Remove(photo);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
