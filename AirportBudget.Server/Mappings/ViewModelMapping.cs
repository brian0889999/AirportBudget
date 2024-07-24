using AirportBudget.Server.DTOs;
using AirportBudget.Server.Models;
using AirportBudget.Server.Utilities;
using AirportBudget.Server.ViewModels;
using AutoMapper;

namespace AirportBudget.Server.Mappings
{
    public class ViewModelMapping : Profile
    {
        public ViewModelMapping()
        {
            CreateMap<BudgetAmount, BudgetAmountViewModel>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Trim()));

            CreateMap<Budget, BudgetViewModel>();
            CreateMap<User, User>();
            CreateMap<UserViewModel, User>();
            CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.System, opt => opt.MapFrom(src => src.System != null ? src.System.Trim() : ""));
            CreateMap<Group, GroupViewModel>();
            CreateMap<RolePermission, RolePermissionViewModel>();
            CreateMap<BudgetAmount, BudgetAmount>();
            CreateMap<AllocateFormViewModel, BudgetAmount>();

            //CreateMap<SoftDeleteViewModel, Money3>()
            //.ForMember(dest => dest.Money, opt => opt.Ignore());

            //CreateMap<User, UserDataViewModel>()
            // .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name != null ? src.Name.Trim() : ""))
            // .ForMember(dest => dest.Account, opt => opt.MapFrom(src => src.Account != null ? src.Account.Trim() : ""))
            // //.ForMember(dest => dest.Password, opt => opt.Ignore()) // 不在這裡處理Password
            // .ForMember(dest => dest.Auth, opt => opt.MapFrom(src => src.Auth != null ? src.Auth.Trim() : ""))
            // .ForMember(dest => dest.Status1, opt => opt.MapFrom(src => src.Status1 != null ? src.Status1.Trim() : ""))
            // .ForMember(dest => dest.Status2, opt => opt.MapFrom(src => src.Status2 != null ? src.Status2.Trim() : ""))
            // .ForMember(dest => dest.Status3, opt => opt.MapFrom(src => src.Status3 != null ? src.Status3.Trim() : ""));


            // New mapping for AddUser and UpdateUser
            //CreateMap<UserDataViewModel, User>()
            //    //.ForMember(dest => dest.Password, opt => opt.Ignore()) // 不在這裡處理Password
            //    .ForMember(dest => dest.Status1, opt => opt.MapFrom(src => src.Status1 != null ? src.Status1.Trim() : ""))
            //    //.ForMember(dest => dest.Status2, opt => opt.MapFrom(src => src.Status2 != null ? src.Status2.Trim() : ""))
            //    .ForMember(dest => dest.Status3, opt => opt.MapFrom(src => src.Status3 != null ? src.Status3.Trim() : ""))
            //    .ForMember(dest => dest.Status2, opt => opt.MapFrom(src => "O")) // 預設值
            //    .ForMember(dest => dest.Account_Open, opt => opt.MapFrom(src => "y")) // 預設值
            //    .ForMember(dest => dest.Time, opt => opt.MapFrom(src => new DateTime(1990, 1, 1, 0, 0, 0))) // 預設值
            //    .ForMember(dest => dest.Time1, opt => opt.MapFrom(src => DateTime.Now)) // 當前時間
            //    .ForMember(dest => dest.Count, opt => opt.MapFrom(src => 0)) // 預設值
            //    .ForMember(dest => dest.Unit_No, opt => opt.MapFrom(src => "M")); // 預設值


            // New mapping for AddUser and UpdateUser
            CreateMap<UserViewModel, User>();
               //.ForMember(dest => dest.Password, opt => opt.Ignore()) // 不在這裡處理Password
               //.ForMember(dest => dest.Status1, opt => opt.MapFrom(src => src.Status1 != null ? src.Status1.Trim() : ""))
               //.ForMember(dest => dest.Status2, opt => opt.MapFrom(src => src.Status2 != null ? src.Status2.Trim() : ""))
               //.ForMember(dest => dest.Status3, opt => opt.MapFrom(src => src.Status3 != null ? src.Status3.Trim() : ""))
               //.ForMember(dest => dest.Status2, opt => opt.MapFrom(src => "O")) // 預設值
               //.ForMember(dest => dest.Account_Open, opt => opt.MapFrom(src => "y")) // 預設值
               //.ForMember(dest => dest.Time, opt => opt.MapFrom(src => new DateTime(1990, 1, 1, 0, 0, 0))) // 預設值
               //.ForMember(dest => dest.Time1, opt => opt.MapFrom(src => DateTime.Now)) // 當前時間
               //.ForMember(dest => dest.Count, opt => opt.MapFrom(src => 0)) // 預設值
               //.ForMember(dest => dest.Unit_No, opt => opt.MapFrom(src => "M")); // 預設值


            //CreateMap<Money3DbModel, Money3DbModel>(); 
            //CreateMap<THospital, MCIHospital>() 
            //.ForMember(dest => dest.Id, opt => opt.Ignore())
            //.ForMember(dest => dest.HospitalId, opt => opt.MapFrom(src => src.Id))
            //.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? string.Empty))
            //.ForMember(dest => dest.Abbreviation, opt => opt.MapFrom(src => src.CallName ?? string.Empty))
            //.ForMember(dest => dest.Alias, opt => opt.MapFrom(src => src.Alias ?? string.Empty))
            //.ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color ?? string.Empty))
            //.ForMember(dest => dest.DistanceSort, opt => opt.Ignore());

            //CreateMap<Money3, DeletedRecordsViewModel>()
            //.ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text != null ? src.Text.Trim() : ""))
            //.ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note != null ? src.Note.Trim() : ""))
            //.ForMember(dest => dest.People, opt => opt.MapFrom(src => src.People != null ? src.People.Trim() : ""))
            //.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name != null ? src.Name.Trim() : ""))
            //.ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.Remarks != null ? src.Remarks.Trim() : ""))
            //.ForMember(dest => dest.People1, opt => opt.MapFrom(src => src.People1 != null ? src.People1.Trim() : ""))
            //.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status != null ? src.Status.Trim() : ""))
            //.ForMember(dest => dest.Group1, opt => opt.MapFrom(src => src.Group1 != null ? src.Group1.Trim() : ""))
            //.ForMember(dest => dest.All, opt => opt.MapFrom(src => src.All != null ? src.All.Trim() : ""))
            //.ForMember(dest => dest.True, opt => opt.MapFrom(src => src.True != null ? src.True.Trim() : ""))
            //.ForMember(dest => dest.Year1, opt => opt.MapFrom(src => src.Year1 != null ? src.Year1.Trim() : ""));

            CreateMap<User, User>()
                .ForMember(dest => dest.System, opt => opt.MapFrom(src => src.System != null ? src.System.Trim() : ""));
        }
    }
}
