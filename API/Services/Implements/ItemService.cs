using API.Data;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Payloads.Requests;
using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public ItemService(BuildingContext context,
            IImageService imageService,
            IMapper mapper,
            IEmailService emailService,
            IPhotoService photoService,
            ICommentService commentService,
            IBillService billService,
            INotificationService notificationService)
        {
            _context = context;
            _imageService = imageService;
            _mapper = mapper;
            _emailService = emailService;
            _photoService = photoService;
            _commentService = commentService;
            _billService = billService;
            _notificationService = notificationService;
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

        public async Task<DataResponse<ItemResponse>> EditItem(int id, EditItemRequest request)
        {
            var item = checkEditRequest(id, request).Result.Data;
            await uploadMedia(item, request.VideoUrl, request.PictureUrl, request.AvatarUrl);
            _context.Items.Update(item);
            if (await _context.SaveChangesAsync() > 0)
            {
                var response = _mapper.Map<ItemResponse>(item);
                response.PictureUrl = new List<string>();
                foreach (var pic in item.Photos)
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
                .Include(i => i.Photos)
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

        public async Task<BaseResponse> AssignUserForItem(int userId, int itemId)
        {
            var renter = await _context.Users.FindAsync(userId);
            var item = await _context.Items
                .Include(i => i.Renter)
                .FirstOrDefaultAsync(i => i.Id == itemId);
            var rentRequest = await _context.RentRequests.FirstOrDefaultAsync(i => i.ItemId == itemId && i.RenterId == renter.Id);
            if (rentRequest == null || item == null || rentRequest == null)
                return new BaseResponse { IsSuccess = false, Message = "Chỉ định thất bại" };

            if (item.Renter == null)
            {
                item.Renter = renter;
                item.RentedDate = DateTime.Today;
                renter.NumberOfParent = rentRequest.NumberOfParent;
                rentRequest.status = true;
            }
            var bill = _billService.createBill(userId, itemId, "Thanh toán yêu cầu thuê", item.Price, 0, 0, 0, 0);
            _context.Bills.Add(bill);
            if (await _context.SaveChangesAsync() > 0)
            {
                await _emailService.SendEmailAsync(renter.Email, "ConfirmMemberEmail", $"<html><body><p>Chào {renter.UserName},</p><p>Vui lòng kiểm thanh toán tiền thuê và đăng ký thông tin các thành viên</p></body></html>");

                return new BaseResponse { IsSuccess = true, Message = "Chỉ định thành công" };
            }
            return new BaseResponse { IsSuccess = false, Message = "Chỉ định thất bại" };
        }

        public async Task<DataResponse<PagedList<ItemUnpaiedResponse>>> GetUnpaiedItems(PaginationParams itemParams)
        {
            var query = await _context.Items
                .Include(i => i.Renter)
                .Where(i => i.MonthlyPaied == false && i.Status == EItemStatus.Rented && i.Renter != null)
                .ToListAsync();
            var responses = _mapper.Map<List<ItemUnpaiedResponse>>(query);
            for (int i = 0; i < responses.Count(); i++)
            {
                int lated = DateTime.Now.Day - query[i].RentedDate.Value.Day;
                responses[i].LatedDay = lated;
            }
            var items = PagedList<ItemUnpaiedResponse>.ToPagedList(responses,
                itemParams.PageNumber, itemParams.PageSize);
            return new DataResponse<PagedList<ItemUnpaiedResponse>>
            {
                IsSuccess = true,
                Data = items
            };
        }

        public async Task<BaseResponse> SendMonthlyBillForUser()
        {
            var items = await _context.Items
                .Include(i => i.Renter)
                .Where(i => i.Status == EItemStatus.Rented && i.Renter != null
                    && i.RentedDate.Value.Day == DateTime.Now.Day
                    && (i.RentedDate.Value.Month < DateTime.Now.Month || i.RentedDate.Value.Year < DateTime.Now.Year))
                .ToListAsync();

            foreach (var item in items)
            {
                item.MonthlyPaied = false;
                var bill = _billService.createBill(item.Renter.Id, item.Id, "Tiền hàng tháng", item.Price, 0, 0, 0, 0);
                var notification = await _notificationService.CreateNotification("Tiền hàng tháng", $"Bạn cần thanh toán số tiền :{item.Price}", item.Renter.Id);
                _context.Bills.Add(bill);
                _context.Notifications.Add(notification);

                if (await _context.SaveChangesAsync() > 0)
                {
                    await _emailService.SendEmailAsync(item.Renter.Email, "Tiền thuê hàng tháng", "Bạn cần đóng tiền thuê trong vòng 7 ngày từ hôm nay");
                    return new BaseResponse { IsSuccess = true, Message = "Gửi thông báo thành công" };
                }
            }
            return new BaseResponse { IsSuccess = false, Message = "Thất bại" };

        }

        public async Task<DataResponse<PagedList<ItemForAdminResponse>>> GetItemsForAdmin(ItemForAdminParams itemParams)
        {
            var query = await _context.Items
                .Include(i => i.Type)
                .Include(i => i.Renter)
                .Include(i => i.Photos)
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
                .Include(i=>i.Renter)
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

        public async Task<BaseResponse> UnAssignUserForItem(int userId, int itemId, int adminId)
        {
            var renter = await _context.Users
                .Include(u => u.Members)
                .FirstOrDefaultAsync(u => u.Id == userId);
            var item = await _context.Items
                .Include(i => i.Renter)
                .FirstOrDefaultAsync(i => i.Id == itemId);
            var unRentRequest = await _context.UnRentRequests.FirstOrDefaultAsync(i => i.ItemId == itemId && i.RenterId == renter.Id);
            if (unRentRequest == null || item == null || unRentRequest == null)
                return new BaseResponse { IsSuccess = false, Message = "Chỉ định thất bại" };

            item.Renter = null;
            item.Status = EItemStatus.Empty;
            item.MonthlyPaied = false;
            item.RentedDate = null;

            foreach (var member in renter.Members)
            {
                _context.Members.Remove(member);
            }
            renter.NumberOfParent = 0;
            renter.Members = null;

            unRentRequest.status = true;
            unRentRequest.HandlerId = adminId;
            unRentRequest.HandleTime = DateTime.Now;

            var notification = await _notificationService.CreateNotification("Yêu cầu hủy", $"Đã xử lý yêu cầu hủy thuê phòng {item.Name} của bạn", userId);
            _context.Notifications.Add(notification);
            if (await _context.SaveChangesAsync() > 0)
            {
                await _emailService.SendEmailAsync(renter.Email, "ConfirmMemberEmail", $"<html><body><p>Chào {renter.UserName},</p><p>Đã xử lý yêu cầu hủy thuê</p></body></html>");

                return new BaseResponse { IsSuccess = true, Message = "Chỉ định thành công" };
            }
            return new BaseResponse { IsSuccess = false, Message = "Chỉ định thất bại" };
        }

        /*------------Private Function------------*/
        private async Task<DataResponse<Item>> checkEditRequest(int id, EditItemRequest request)
        {
            var item = await _context.Items
                .Include(i => i.Type)
                .Include(i => i.Renter)
                .Include(i => i.Photos)
                .FirstOrDefaultAsync(i => i.Id == id);
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
                if (item.Photos != null)
                {
                    foreach (var photo in item.Photos)
                    {
                        await _imageService.DeleteMediaAsync(photo.PublicId);
                    }
                }
                item.Photos = new List<Photo>();
                foreach (var img in imageUrl)
                {
                    var imageResult = await _imageService.AddImageAsync(img);
                    if (imageResult.Error != null)
                        return new DataResponse<Item>
                        {
                            IsSuccess = false,
                            Message = "Lỗi tải hình ảnh"
                        };
                    var photo = new Photo { PublicId = imageResult.PublicId, Url = imageResult.SecureUrl.ToString(), Item = item, ItemId = item.Id };
                    item.Photos.Add(photo);
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
            if (item.Photos != null)
            {
                foreach (var photo in item.Photos)
                {
                    await _imageService.DeleteMediaAsync(photo.PublicId);
                    _context.Photos.Remove(photo);
                    await _context.SaveChangesAsync();
                }
            }
        }


    }
}
