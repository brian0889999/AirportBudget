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
            CreateMap<BudgetAmount, BudgetAmountViewModel>();
            //.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Trim()));

            CreateMap<Budget, BudgetViewModel>();
            CreateMap<User, User>();
            CreateMap<UserViewModel, User>();
            CreateMap<User, UserViewModel>();
                //.ForMember(dest => dest.System, opt => opt.MapFrom(src => src.System != null ? src.System.Trim() : ""));
            CreateMap<Group, GroupViewModel>();
            CreateMap<RolePermission, RolePermissionViewModel>();
            CreateMap<BudgetAmount, BudgetAmount>();
            CreateMap<AllocateFormViewModel, BudgetAmount>();

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

            CreateMap<User, User>();
                //.ForMember(dest => dest.System, opt => opt.MapFrom(src => src.System != null ? src.System.Trim() : ""));
        }
    }
}
