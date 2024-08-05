using AirportBudget.Server.DTOs;
using AirportBudget.Server.Models;
using AirportBudget.Server.Utilities;
using AirportBudget.Server.ViewModels;
using AutoMapper;

namespace AirportBudget.Server.Mappings
{
    public class DTOMapping : Profile
    {
        public DTOMapping()
        {
            CreateMap<BudgetAmount, ExportBudgetAmountDTO>();
        }
    }
}
