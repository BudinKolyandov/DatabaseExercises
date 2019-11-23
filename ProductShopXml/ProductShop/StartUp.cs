using AutoMapper;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile<ProductShopProfile>());

            using (var context = new ProductShopContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }

            using (var context = new ProductShopContext())
            {
                var inputXml = File.ReadAllText(@"./../../../Datasets/users.xml");
                var result = ImportUsers(context, inputXml);
                Console.WriteLine(result);
            }

            using (var context = new ProductShopContext())
            {
                var inputXml = File.ReadAllText(@"./../../../Datasets/products.xml");
                var result = ImportProducts(context, inputXml);
                Console.WriteLine(result);
            }

            using (var context = new ProductShopContext())
            {
                var inputXml = File.ReadAllText(@"./../../../Datasets/categories.xml");
                var result = ImportCategories(context, inputXml);
                Console.WriteLine(result);
            }

            using (var context = new ProductShopContext())
            {
                var inputXml = File.ReadAllText(@"./../../../Datasets/categories-products.xml");
                var result = ImportCategoryProducts(context, inputXml);
                Console.WriteLine(result);
            }

            using (var context = new ProductShopContext())
            {
                Console.WriteLine(GetProductsInRange(context));
            }

            using (var context = new ProductShopContext())
            {
                Console.WriteLine(GetSoldProducts(context));
            }

            using (var context = new ProductShopContext())
            {
                Console.WriteLine(GetCategoriesByProductsCount(context));
            }

            using (var context = new ProductShopContext())
            {
                Console.WriteLine(GetUsersWithProducts(context));
            }
        }


        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportUsersDto[]), new XmlRootAttribute("Users"));
            ImportUsersDto[] usersDtos;

            using (var reader = new StringReader(inputXml))
            {
                usersDtos = (ImportUsersDto[])xmlSerializer.Deserialize(reader);
            }

            var users = Mapper.Map<User[]>(usersDtos);

            context.Users.AddRange(users);
            var count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportProductDto[]), new XmlRootAttribute("Products"));

            ImportProductDto[] productDtos;

            using (var reader = new StringReader(inputXml))
            {
                productDtos = (ImportProductDto[])xmlSerializer.Deserialize(reader);
            }

            var products = Mapper.Map<Product[]>(productDtos);

            context.Products.AddRange(products);

            var count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCategoriesDto[]), new XmlRootAttribute("Categories"));

            ImportCategoriesDto[] categoriesDtos;

            using (var reader = new StringReader(inputXml))
            {
                categoriesDtos = (ImportCategoriesDto[])xmlSerializer.Deserialize(reader);
            }

            var categories = Mapper.Map<Category[]>(categoriesDtos);

            context.Categories.AddRange(categories);

            var count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCategorysProductsDto[]), new XmlRootAttribute("CategoryProducts"));

            ImportCategorysProductsDto[] categoryProductsDtos;

            using (var reader = new StringReader(inputXml))
            {
                categoryProductsDtos = (ImportCategorysProductsDto[])xmlSerializer.Deserialize(reader);
            }

            List<CategoryProduct> categoryProducts = new List<CategoryProduct>();

            foreach (var categoryProduct in categoryProductsDtos)
            {
                var product = context.Products.Find(categoryProduct.ProductId);
                var category = context.Categories.Find(categoryProduct.CategoryId);

                if (product != null && category != null)
                {
                    var currentcategoryProduct = Mapper.Map<CategoryProduct>(categoryProduct);
                    categoryProducts.Add(currentcategoryProduct);
                }
            }

            context.CategoryProducts.AddRange(categoryProducts);

            var count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .Select(p => new ProductsInRangeDto
                {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = $"{p.Buyer.FirstName} {p.Buyer.LastName}"
                })
                .Take(10)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ProductsInRangeDto[]), new XmlRootAttribute("Products"));

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(sb), products, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Any(p => p.Buyer != null))
                .Select(u => new UsersWithSoldProductsDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Products = u.ProductsSold.Select(p => new SoldProductsDto
                    {
                        Name = p.Name,
                        Price = p.Price
                    })
                    .ToArray()
                })
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(UsersWithSoldProductsDto[]), new XmlRootAttribute("Users"));

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            xmlSerializer.Serialize(new StringWriter(sb), users, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(c => new CategoriesByProductsCountDto
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count(),
                    AveragePrice = c.CategoryProducts.Average(cp => cp.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(p => p.Product.Price)
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.TotalRevenue)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(CategoriesByProductsCountDto[]), new XmlRootAttribute("Categories"));

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            xmlSerializer.Serialize(new StringWriter(sb), categories, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersWithProducts = context.Users
                .Where(x => x.ProductsSold.Any())
                .OrderByDescending(p => p.ProductsSold.Count())
                .Select(u => new UsersWithProductsSoldDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new SoldProductsWithCountDto
                    {
                        Count = u.ProductsSold.Count(),
                        Products = u.ProductsSold
                        .Select(p => new SoldProductsDto
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ToArray()
                    }

                })
                .Take(10)
                .ToArray();

            var result = new UsersWithProductsDto
            {
                Count = context.Users.Count(p => p.ProductsSold.Any()),
                Users = usersWithProducts
            };

            var xmlSerializer = new XmlSerializer(typeof(UsersWithProductsDto), new XmlRootAttribute("Users"));

            StringBuilder sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            xmlSerializer.Serialize(new StringWriter(sb), result, namespaces);

            return sb.ToString().TrimEnd();
        }

    }
}