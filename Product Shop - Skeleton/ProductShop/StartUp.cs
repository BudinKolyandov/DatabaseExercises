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
            using (var context = new ProductShopContext() )
            {
                var usersJson = File.ReadAllText(@"../../../Datasets/users.json");

                var result = ImportUsers(context, usersJson);
                Console.WriteLine(result);
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
    }
}