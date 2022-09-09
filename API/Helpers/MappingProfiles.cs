using API.Entities;
using API.Payloads.Requests;
using API.Payloads.Response;
using AutoMapper;
using System.Collections.Generic;

namespace API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            //Map CreateItemRequest -> Item
            CreateMap<CreateItemRequest, Item>()
                .ForMember(dst => dst.Type, src => src.Ignore());

            //Map Item -> ItemResponse
            CreateMap<Item, ItemResponse>()
                .ForMember(dst => dst.TypeId, src => src.MapFrom(i => i.Type.Id))
                .ForMember(dst => dst.VideoUrl, src => src.MapFrom(i => i.VideoUrl));

            //Map Item -> ItemForSystemResponse
            CreateMap<Item, ItemDetailForSystemResponse>()
                .ForMember(dst => dst.Type, src => src.MapFrom(i => i.Type.Name));

            //Map Comment -> CommentResponse
            CreateMap<Comment, CommentResponse>()
                .ForMember(dst => dst.AccountName, src => src.MapFrom(i => i.Account.UserName))
                .ForMember(dst => dst.AccountAvatar, src => src.MapFrom(i => i.Account.AvatarUrl));

            //Map Account -> ProfileResponse
            CreateMap<Account, ProfileResponse>()
                .ForMember(dst => dst.CountPerson, src => src.MapFrom(i => i.Members.Count));

            //Map Member -> PersonResponse
            CreateMap<Member, PersonResponse>()
                .ForMember(dst => dst.PlaceOfOrigin, src => src.MapFrom(i => i.PlaceOfOrigin.SumAddress()));

            //Map Item -> ItemForFavoriteResponse
            CreateMap<Item, ItemForFavoriteResponse>()
                .ForMember(dst => dst.Type, src => src.MapFrom(i => i.Type.Name));

            //Map Item -> ItemForSystem
            CreateMap<Item, ItemForSystemResponse>()
                .ForMember(dst => dst.Type, src => src.MapFrom(i => i.Type.Name));

            //Map ReportBuilding -> ReportResponse
            CreateMap<ReportBuilding, ReportResponse>()
                .ForMember(dst => dst.Username, src => src.MapFrom(i => i.Account.UserName));

            //Map ReportBuilding -> ReportResponse
            CreateMap<ReportBuilding, ReportDetailResponse>()
                .ForMember(dst => dst.Username, src => src.MapFrom(i => i.Account.UserName));

            //Map CreateMemberRequest -> Member
            CreateMap<CreateMemberRequest, Member>();

            //Map Item -> ItemUnpaiedResponse
            CreateMap<Item, ItemUnpaiedResponse>()
                .ForMember(dst => dst.RenterName, src => src.MapFrom(i => i.Renter.UserName))
                .ForMember(dst => dst.RenterId, src => src.MapFrom(i => i.Renter.Id));

            //Map Bill -> BillForAdminResponse
            CreateMap<Bill, BillResponse>()
                .ForMember(dst => dst.SumPrice, src => src.MapFrom(i => i.SumPrice()))
                .ForMember(dst => dst.Username, src => src.MapFrom(i => i.Account.UserName));

            //Map Account -> GetAccountResponse
            CreateMap<Account, AccountResponse>();

            //Map Item -> ItemResponse
            CreateMap<Item, ItemForAdminResponse>()
                .ForMember(dst => dst.Type, src => src.MapFrom(i => i.Type.Name));

            //Map Item -> ItemDetailForAdminResponse
            CreateMap<Item, ItemDetailForAdminResponse>()
                .ForMember(dst => dst.Type, src => src.MapFrom(i => i.Type.Name))
                .ForMember(dst => dst.RenterName, src => src.MapFrom(i => i.Renter.UserName))
                .ForMember(dst => dst.NumberOfParent, src => src.MapFrom(i => i.Renter.NumberOfParent));

            //Map Member -> MemberForAdminResponse
            CreateMap<Member, MemberForAdminResponse>()
                .ForMember(dst => dst.MemberOfAccount, src => src.MapFrom(i => i.Account.UserName))
                .ForMember(dst => dst.MemberOfAccountId, src => src.MapFrom(i => i.AccountId))
                .ForMember(dst => dst.PlaceOfOrigin, src => src.MapFrom(i => i.PlaceOfOrigin.SumAddress()));

            //Map Account -> GetAccountResponse
            CreateMap<CreateNotificationRequest, Notification>();
        }
    }
}
