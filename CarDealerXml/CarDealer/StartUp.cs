using AutoMapper;
using CarDealer.Data;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile<CarDealerProfile>());

            //using (var context = new CarDealerContext())
            //{
            //    context.Database.EnsureDeleted();
            //    context.Database.EnsureCreated();
            //}
            //
            //using (var context = new CarDealerContext())
            //{
            //    var inputXml = File.ReadAllText(@"./../../../Datasets/suppliers.xml");
            //    var result = ImportSuppliers(context, inputXml);
            //    System.Console.WriteLine(result);
            //}
            //
            //using (var context = new CarDealerContext())
            //{
            //    var inputXml = File.ReadAllText(@"./../../../Datasets/parts.xml");
            //    var result = ImportParts(context, inputXml);
            //    System.Console.WriteLine(result);
            //}
            //
            //using (var context = new CarDealerContext())
            //{
            //    var inputXml = File.ReadAllText(@"./../../../Datasets/cars.xml");
            //    var result = ImportCars(context, inputXml);
            //    System.Console.WriteLine(result);
            //}
            //
            //using (var context = new CarDealerContext())
            //{
            //    var inputXml = File.ReadAllText(@"./../../../Datasets/customers.xml");
            //    var result = ImportCustomers(context, inputXml);
            //    System.Console.WriteLine(result);
            //}
            //
            //using (var context = new CarDealerContext())
            //{
            //    var inputXml = File.ReadAllText(@"./../../../Datasets/sales.xml");
            //    var result = ImportSales(context, inputXml);
            //    System.Console.WriteLine(result);
            //}
            //
            //using (var context = new CarDealerContext())
            //{
            //    System.Console.WriteLine(GetCarsWithDistance(context));
            //}

            //using (var context = new CarDealerContext())
            //{
            //    System.Console.WriteLine(GetCarsFromMakeBmw(context));
            //}

            //using (var context = new CarDealerContext())
            //{
            //    System.Console.WriteLine(GetLocalSuppliers(context));
            //}
            //
            //using (var context = new CarDealerContext())
            //{
            //    System.Console.WriteLine(GetCarsWithTheirListOfParts(context));
            //}

            //using (var context = new CarDealerContext())
            //{
            //    System.Console.WriteLine(GetTotalSalesByCustomer(context));
            //}

            using (var context = new CarDealerContext())
            {
                System.Console.WriteLine(GetSalesWithAppliedDiscount(context));
            }
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportSuppliersDto[]), new XmlRootAttribute("Suppliers"));

            ImportSuppliersDto[] suppliersDtos;

            using (var reader = new StringReader(inputXml))
            {
                suppliersDtos = (ImportSuppliersDto[])xmlSerializer.Deserialize(reader);
            }

            var suppliers = Mapper.Map<Supplier[]>(suppliersDtos);

            context.Suppliers.AddRange(suppliers);

            var count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportPatrsDto[]), new XmlRootAttribute("Parts"));

            var partsDtos = (ImportPatrsDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            List<Part> parts = new List<Part>();

            foreach (var partDto in partsDtos)
            {
                var supplier = context.Suppliers.Find(partDto.SupplierId);

                if (supplier != null)
                {
                    var part = Mapper.Map<Part>(partDto);
                    parts.Add(part);
                }
            }

            context.Parts.AddRange(parts);

            var count = context.SaveChanges();
            return $"Successfully imported {count}";

        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCarsDto[]), new XmlRootAttribute("Cars"));

            var carsDtos = (ImportCarsDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            List<Car> cars = new List<Car>();

            foreach (var carDto in carsDtos)
            {
                var car = Mapper.Map<Car>(carDto);

                context.Cars.Add(car);

                foreach (var part in carDto.Parts.PartsId)
                {
                    if (car.PartCars
                        .FirstOrDefault(x=>x.PartId == part.PartId) == null &&
                        context.Parts.Find(part.PartId) != null)
                    {
                        var partCar = new PartCar
                        {
                            CarId = car.Id,
                            PartId = part.PartId
                        };
                        car.PartCars.Add(partCar);
                    }
                }
                cars.Add(car);
            }
            
            context.SaveChanges();
            return $"Successfully imported {cars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCustomersDto[]), new XmlRootAttribute("Customers"));

            var customersDtos = (ImportCustomersDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var customers = Mapper.Map<Customer[]>(customersDtos);

            context.Customers.AddRange(customers);

            var count = context.SaveChanges();
            return $"Successfully imported {count}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportSalesDto[]), new XmlRootAttribute("Sales"));

            var salesDtos = (ImportSalesDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            List<Sale> sales = new List<Sale>();

            foreach (var saleDto in salesDtos)
            {
                if (context.Cars.Find(saleDto.CarId) != null)
                {
                    var sale = Mapper.Map<Sale>(saleDto);
                    sales.Add(sale);
                }
            }
            context.Sales.AddRange(sales);

            var count = context.SaveChanges();
            return $"Successfully imported {count}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.TravelledDistance > 2000000)
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .Select(c => new CarsWithDistanceDto
                {
                    Model = c.Model,
                    Make = c.Make,
                    TravelledDistance = c.TravelledDistance
                })
                .Take(10)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(CarsWithDistanceDto[]), new XmlRootAttribute("cars"));

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.Make == "BMW")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .Select(c => new CarsFromMakeBmwDto
                {
                    Id = c.Id,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(CarsFromMakeBmwDto[]), new XmlRootAttribute("cars"));

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(s => new GetLocalSuppliersDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(GetLocalSuppliersDto[]), new XmlRootAttribute("suppliers"));

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(sb), suppliers, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .OrderByDescending(x => x.TravelledDistance)
                .ThenBy(x => x.Model)
                .Select(c => new CarsWithTheirListOfPartsDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = c.PartCars
                    .Select(p => new ListOfPartsDto
                    {
                        Name = p.Part.Name,
                        Price = p.Part.Price
                    })
                    .OrderByDescending(x=>x.Price)
                    .ToArray()
                })
                .Take(5)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(CarsWithTheirListOfPartsDto[]), new XmlRootAttribute("cars"));

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(x => x.Sales.Count >= 1)
                .Select(c => new TotalSalesByCustomerDto
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales.SelectMany(x => x.Car.PartCars).Sum(cp => cp.Part.Price)
                })
                .OrderByDescending(x => x.SpentMoney)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(TotalSalesByCustomerDto[]), new XmlRootAttribute("customers"));

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(sb), customers, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(s => new GetSalesWithDiscountDto
                {
                    Car = new ExportCarDto
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    },
                    Discount = s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartCars.Sum(p => p.Part.Price),
                    PriceWithDiscount = s.Car.PartCars.Sum(p => p.Part.Price) -
                        s.Car.PartCars.Sum(p => p.Part.Price) * s.Discount / 100
                })
                .ToArray();


            var xmlSerializer = new XmlSerializer(typeof(GetSalesWithDiscountDto[]), new XmlRootAttribute("sales"));

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(sb), sales, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}