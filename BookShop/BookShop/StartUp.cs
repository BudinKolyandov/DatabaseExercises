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
            using (var context = new BookShopContext())
            {
                Console.WriteLine("Please enter the age restriction type you want to see:");
                string age = Console.ReadLine();
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
                        
            using (var context = new BookShopContext())
            {
                Console.WriteLine("Books that are not released in:");
                int year = int.Parse(Console.ReadLine());
                var result = GetBooksNotReleasedIn(context, year);
                Console.WriteLine(result);
            }

            using (var contex = new BookShopContext())
            {
                Console.WriteLine("Please enter the book category you need:");
                string input = Console.ReadLine();
                var result = GetBooksByCategory(contex, input);
                Console.WriteLine(result);
            }

            using (var context = new BookShopContext())
            {
                Console.WriteLine("Please enter the date to get the books released before it:");
                string input = Console.ReadLine();
                var result = GetBooksReleasedBefore(context, input);
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

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder sb = new StringBuilder();

            var titles = context.Books
                .Where(x => x.ReleaseDate.Value.Year != year)
                .OrderBy(x => x.BookId)
                .Select(x => x.Title)
                .ToList();

            foreach (var title in titles)
            {
                sb.AppendLine(title);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            string[] categories = input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower()).ToArray();

            var books = context.Books
                .Where(x => x.BookCategories
                .Any(c => categories.Contains(c.Category.Name.ToLower())))
                .OrderBy(x => x.Title)
                .Select(x => x.Title)
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder sb = new StringBuilder();

            DateTime releaseDate = DateTime.ParseExact(date, "dd-MM-yyyy", null);

            var books = context.Books
                .Where(x => x.ReleaseDate < releaseDate)
                .OrderByDescending(x => x.ReleaseDate)
                .Select(x => new
                {
                    x.Title,
                    x.EditionType,
                    x.Price
                })
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }



    }
}
