using API.Data;
using API.Entities;
using API.Helpers;
using API.Payloads.Requests;
using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services.Implements
{
    public class ReportService : IReportService
    {
        private readonly BuildingContext _context;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IPhotoService _photoService;

        public ReportService(BuildingContext context, 
            IMapper mapper, 
            IImageService imageService,
            IPhotoService photoService)
        {
            _context = context;
            _mapper = mapper;
            _imageService = imageService;
            _photoService = photoService;
        }

        public async Task<DataResponse<ReportDetailResponse>> GetReporDetail(int id)
        {
            var report = await _context.ReportBuildings
                .Include(i => i.Account)
                .FirstOrDefaultAsync(i => i.Id == id);
            if (report == null)
                return new DataResponse<ReportDetailResponse>
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy"
                };
            var response = _mapper.Map<ReportDetailResponse>(report);
            response.PictureUrl = _photoService.GetReportPhotoUrlsByReportId(id).Result.Datas;
            return new DataResponse<ReportDetailResponse>
            {
                IsSuccess = true,
                Data = response
            };
        }

        public async Task<DataResponse<PagedList<ReportResponse>>> GetReports(PaginationParams param)
        {
            var query = await _context.ReportBuildings
                .Include(i => i.Account)
                .Include(i => i.ReportPhotos)
                .ToListAsync();
            List<ReportResponse> responses = _mapper.Map<List<ReportResponse>>(query);

            var reports = PagedList<ReportResponse>.ToPagedList(responses,
                param.PageNumber, param.PageSize);


            return new DataResponse<PagedList<ReportResponse>>
            {
                IsSuccess = true,
                Data = reports
            };
        }

        public async Task<BaseResponse> SendReport(int userId, ReportRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            ReportBuilding report = new ReportBuilding
            {
                Account = user,
                Title = request.Title,
                Content = request.Content                
            };
            await uploadMedia(report, request.PictureUrl, request.AvatarUrl);
            _context.ReportBuildings.Add(report);
            if (await _context.SaveChangesAsync() > 0)
                return new BaseResponse { IsSuccess = true, Message = "Gửi report thành công" };
            return new BaseResponse { IsSuccess = false, Message = "Gửi report thất bại" };
        }

        private async Task<DataResponse<ReportBuilding>> uploadMedia(ReportBuilding report, IFormFileCollection imageUrl, IFormFile avatarUrl)
        {
            if (imageUrl != null)
            {
                if (report.ReportPhotos != null)
                {
                    foreach (var photo in report.ReportPhotos)
                    {
                        await _imageService.DeleteMediaAsync(photo.PublicId);
                    }
                }
                report.ReportPhotos = new List<ReportPhoto>();
                foreach (var img in imageUrl)
                {
                    var imageResult = await _imageService.AddImageAsync(img);
                    if (imageResult.Error != null)
                        return new DataResponse<ReportBuilding>
                        {
                            IsSuccess = false,
                            Message = "Lỗi tải hình ảnh"
                        };
                    var photo = new ReportPhoto { PublicId = imageResult.PublicId, Url = imageResult.SecureUrl.ToString(), Report = report, ReportId = report.Id };
                    report.ReportPhotos.Add(photo);
                }
            }

            if (avatarUrl != null)
            {
                if (!string.IsNullOrEmpty(report.AvatarId))
                {
                    await _imageService.DeleteMediaAsync(report.AvatarId);
                }

                var avatarResult = await _imageService.AddImageAsync(avatarUrl);

                if (avatarResult.Error != null)
                    return new DataResponse<ReportBuilding>
                    {
                        IsSuccess = false,
                        Message = "Lỗi tải video"
                    };
                report.AvatarUrl = avatarResult.SecureUrl.ToString();
                report.AvatarId = avatarResult.PublicId;
            }

            return new DataResponse<ReportBuilding>
            {
                IsSuccess = true,
                Data = report
            };
        }
    }
}
