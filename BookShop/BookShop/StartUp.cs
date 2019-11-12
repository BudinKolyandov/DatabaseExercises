namespace BookShop
{
    using Data;
    using System;
    using System.Linq;
    using System.Text;
    using Models;
    using BookShop.Models.Enums;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            string age = Console.ReadLine();

            using (var context = new BookShopContext())
            {
                var result = GetBooksByAgeRestriction(context, age);
                Console.WriteLine(result);
            }

            using (var context = new BookShopContext())
            {
                var result = GetGoldenBooks(context);
                Console.WriteLine(result);
            }

            using (var context = new BookShopContext())
            {
                var result = GetBooksByPrice(context);
                Console.WriteLine(result);
            }
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder sb = new StringBuilder();

            int ageRestriction = int.MinValue;

            if (command.ToLower() == "minor")
            {
                ageRestriction = 0;
            }
            else if (command.ToLower() == "teen")
            {
                ageRestriction = 1;
            }
            else if (command.ToLower() == "adult")
            {
                ageRestriction = 2;
            }

            var bookTitles = context.Books
                .Where(x => (int)x.AgeRestriction == ageRestriction)
                .OrderBy(x => x.Title)
                .Select(x => x.Title)
                .ToList();

            foreach (var title in bookTitles)
            {
                sb.AppendLine(title);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var titles = context.Books
                .Where(x => x.EditionType == EditionType.Gold && x.Copies < 5000)
                .OrderBy(x => x.BookId)
                .Select(x => x.Title)
                .ToList();

            foreach (var title in titles)
            {
                sb.AppendLine(title);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(x=>x.Price > 40)
                .OrderByDescending(x=>x.Price)
                .Select(x=> new
                {
                    x.Title,
                    x.Price
                });

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }


    }
}
