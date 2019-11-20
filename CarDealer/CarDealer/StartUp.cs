using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var context = new CarDealerContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
            using (var context = new CarDealerContext())
            {
                var suppliersJson = File.ReadAllText(@"../../../Datasets/suppliers.json");
            
                var result = ImportSuppliers(context, suppliersJson);
                Console.WriteLine(result);
            }
            using (var context = new CarDealerContext())
            {
                var partsJson = File.ReadAllText(@"../../../Datasets/parts.json");
            
                var result = ImportSuppliers(context, partsJson);
                Console.WriteLine(result);
            }
            using (var context = new CarDealerContext())
            {
                var carsJson = File.ReadAllText(@"../../../Datasets/cars.json");
            
                var result = ImportCars(context, carsJson);
                Console.WriteLine(result);
            }
            using (var context = new CarDealerContext())
            {
                var customersJson = File.ReadAllText(@"../../../Datasets/customers.json");
            
                var result = ImportCustomers(context, customersJson);
                Console.WriteLine(result);
            }

            using (var context = new CarDealerContext())
            {
                var salesJson = File.ReadAllText(@"../../../Datasets/customers.json");

                var result = ImportSales(context, salesJson);
                Console.WriteLine(result);
            }

        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            Supplier[] suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            context.Suppliers.AddRange(suppliers);
            var count = context.SaveChanges();
            return $"Successfully imported {count}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            Part[] parts = JsonConvert.DeserializeObject<Part[]>(inputJson)
                .Where(x=>x.SupplierId <= 31)
                .ToArray();

            context.Parts.AddRange(parts);
            var count = context.SaveChanges();
            return $"Successfully imported {count}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carsDto = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);

            foreach (var carDto in carsDto)
            {
                Car car = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TravelledDistance
                };

                context.Cars.Add(car);

                foreach (var partId in carDto.PartsId)
                {
                    PartCar partCar = new PartCar
                    {
                        PartId = partId,
                        CarId = car.Id
                    };

                    if (car.PartCars.FirstOrDefault(p => p.PartId == partId) == null)
                    {
                        context.PartCars.Add(partCar);
                    }
                }
            }

            context.SaveChanges();

            return $"Successfully imported {carsDto.Count()}.";

        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            Customer[] customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);

            context.Customers.AddRange(customers);
            var count = context.SaveChanges();
            return $"Successfully imported {count}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            Sale[] sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            context.Sales.AddRange(sales);
            var count = context.SaveChanges();
            return $"Successfully imported {count}.";
        }


    }
}