using AutoMapper;
using CarDealer.Dtos.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<ImportSuppliersDto, Supplier>();

            this.CreateMap<ImportPatrsDto, Part>();

            this.CreateMap<ImportCarsDto, Car>();

            this.CreateMap<ImportCustomersDto, Customer>();

            this.CreateMap<ImportSalesDto, Sale>();
        }
    }
}
