using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var context = new ProductShopContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
            
            
            using (var context = new ProductShopContext() )
            {
                var usersJson = File.ReadAllText(@"../../../Datasets/users.json");
            
                var result = ImportUsers(context, usersJson);
                Console.WriteLine(result);
            }
            
            using (var context = new ProductShopContext())
            {
                var productsJson = File.ReadAllText(@"../../../Datasets/products.json");
            
                var result = ImportProducts(context, productsJson);
                Console.WriteLine(result);
            }
            
            using (var context = new ProductShopContext())
            {
                var categoriesJson = File.ReadAllText(@"../../../Datasets/categories.json");
            
                var result = ImportCategories(context, categoriesJson);
                Console.WriteLine(result);
            }
            
            using (var context = new ProductShopContext())
            {
                var categoriesProductsJson = File.ReadAllText(@"../../../Datasets/categories-products.json");
            
                var result = ImportCategoryProducts(context, categoriesProductsJson);
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



        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            User[] users = JsonConvert.DeserializeObject<User[]>(inputJson)
                .Where(x=>x.LastName != null && x.LastName.Length >= 3)
                .ToArray();

            context.Users.AddRange(users);
            var count = context.SaveChanges();

            return $"Successfully imported {count}";
        }


        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            Product[] products = JsonConvert.DeserializeObject<Product[]>(inputJson)
                .Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Length > 3)
                .ToArray();

            context.Products.AddRange(products);

            var count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            Category[] categories = JsonConvert.DeserializeObject<Category[]>(inputJson)
                .Where(x => x.Name != null)
                .ToArray();

            context.Categories.AddRange(categories);

            var count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoryProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);
            context.CategoryProducts.AddRange(categoryProducts);

            var count = context.SaveChanges();

            return $"Successfully imported {count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .Select(x => new
                {
                    name = x.Name,
                    price = x.Price,
                    seller = $"{x.Seller.FirstName} {x.Seller.LastName}"
                })
                .ToList(); ;

            var json = JsonConvert.SerializeObject(products, Formatting.Indented);

            return json;

        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var soldProducts = context.Users
                .Where(x => x.ProductsSold.Any(ps => ps.BuyerId != null))
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    soldProducts = x.ProductsSold
                    .Where(p => p.Buyer != null)
                    .Select(p => new
                    {
                        name = p.Name,
                        price = p.Price,
                        buyerFirstName = p.Buyer.FirstName,
                        buyerLastName = p.Buyer.LastName
                    })
                    .ToList()
                })
                .ToList();

            var json = JsonConvert.SerializeObject(soldProducts, Formatting.Indented);

            return json;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .OrderByDescending(x => x.CategoryProducts.Count())
                .Select(x => new
                {
                    category = x.Name,
                    productsCount = x.CategoryProducts.Count(),
                    averagePrice = $@"{x.CategoryProducts
                    .Sum(p => p.Product.Price) / x.CategoryProducts.Count():f2}",
                    totalRevenue = $"{x.CategoryProducts.Sum(p => p.Product.Price):f2}"
                })
                .ToList();
            var json = JsonConvert.SerializeObject(categories, Formatting.Indented);

            return json;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
               .Where(x => x.ProductsSold.Any(p => p.Buyer != null))
               .OrderByDescending(x => x.ProductsSold.Count(ps => ps.Buyer != null))
               .Select(x => new
               {
                   firstName = x.FirstName,
                   lastName = x.LastName,
                   age = x.Age,
                   soldProducts = new
                   {
                       count = x.ProductsSold.Count(p => p.Buyer != null),
                       products = x.ProductsSold
                       .Where(p => p.Buyer != null)
                       .Select(p => new
                       {
                           name = p.Name,
                           price = p.Price
                       })
                       .ToList()
                   }
               })
               .ToList();

            var result = new
            {
                usersCount = users.Count(),
                users = users
            };

            var json = JsonConvert.SerializeObject(result, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return json;
        }

    }
}