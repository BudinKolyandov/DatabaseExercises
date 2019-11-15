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
            
            using (var context = new BookShopContext())
            {
                Console.WriteLine("Please enter the book category you need:");
                string input = Console.ReadLine();
                var result = GetBooksByCategory(context, input);
                Console.WriteLine(result);
            }
            
            using (var context = new BookShopContext())
            {
                Console.WriteLine("Please enter the date to get the books released before t:");
                string input = Console.ReadLine();
                var result = GetBooksReleasedBefore(context, input);
                Console.WriteLine(result);
            }
            
            using (var context = new BookShopContext())
            {
                Console.WriteLine("Please enter the characters that the authors name ends ith:");
                string input = Console.ReadLine();
                var result = GetAuthorNamesEndingIn(context, input);
                Console.WriteLine(result);
            }
            
            using (var context = new BookShopContext())
            {
                Console.WriteLine(@"Please enter the characters that are contained in the ook's name:");
                string input = Console.ReadLine();
                var result = GetBookTitlesContaining(context, input);
                Console.WriteLine(result);
            }
            
            
            using (var context = new BookShopContext())
            {
                Console.WriteLine(@"Please enter the characters that the authors last name tarts with:");
                string input = Console.ReadLine();
                var result = GetBooksByAuthor(context, input);
                Console.WriteLine(result);
            }
            
            using (var context = new BookShopContext())
            {
                Console.WriteLine(@"Please enter the minimum length of the title:");
                int input = int.Parse(Console.ReadLine());
                var result = CountBooks(context, input);
                Console.WriteLine(result);
            }
            
            
            using (var context = new BookShopContext())
            {
                var result = CountCopiesByAuthor(context);
                Console.WriteLine(result);
            }
            
            using (var context = new BookShopContext())
            {
                var result = GetTotalProfitByCategory(context);
                Console.WriteLine(result);
            }
            
            using (var context = new BookShopContext())
            {
                var result = GetMostRecentBooks(context);
                Console.WriteLine(result);
            }
            
            using (var context = new BookShopContext())
            {
                IncreasePrices(context);
            }
            

            using (var context =  new BookShopContext())
            {
                var result = RemoveBooks(context);
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

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var authorNames = context.Authors
                .Where(x => x.FirstName.EndsWith(input))
                .Select(x => $"{x.FirstName} {x.LastName}")
                .OrderBy(x => x)
                .ToList();

            foreach (var name in authorNames)
            {
                sb.AppendLine(name);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var bookNames = context.Books
                .Where(x => x.Title.ToLower().Contains(input.ToLower()))
                .OrderBy(x => x.Title)
                .Select(x => x.Title)
                .ToList();

            foreach (var name in bookNames)
            {
                sb.AppendLine(name);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(x => x.BookId)
                .Select(x => new
                {
                    x.Title,
                    x.Author.FirstName,
                    x.Author.LastName
                })
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} ({book.FirstName} {book.LastName})");
            }

            return sb.ToString().TrimEnd();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context.Books
                .Where(x => x.Title.Length > lengthCheck)
                .Count();
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var authors = context.Authors
                .Select(x => new
                {
                    Name = $"{x.FirstName} {x.LastName}",
                    Copies = x.Books.Select(b => b.Copies).Sum()
                })
                .OrderByDescending(x => x.Copies)
                .ToList();

            foreach (var author in authors)
            {
                sb.AppendLine($"{author.Name} - {author.Copies}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var profitsForCategory = context.Categories
                .Select(x => new
                {
                    x.Name,
                    Profit = x.CategoryBooks.Select(b => b.Book.Copies * b.Book.Price).Sum()
                })
                .OrderByDescending(x=>x.Profit)
                .ThenBy(x=>x.Name)
                .ToList();

            foreach (var profit in profitsForCategory)
            {
                sb.AppendLine($"{profit.Name} ${profit.Profit:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categories = context.Categories
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    x.Name,
                    Books = x.CategoryBooks
                    .Select(cb => new 
                    { 
                        cb.Book.Title,
                        cb.Book.ReleaseDate
                    })
                    .OrderByDescending(b => b.ReleaseDate)
                    .Take(3)
                })
                .ToList();

            foreach (var category in categories)
            {
                sb.AppendLine($"--{category.Name}");
                foreach (var book in category.Books)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            context.Books
                .Where(x => x.ReleaseDate.Value.Year < 2010)
                .ToList()
                .ForEach(x => x.Price += 5);

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var booksToDelete = context.Books
                .Where(x => x.Copies < 4200)
                .ToList();

            context.RemoveRange(booksToDelete);
            context.SaveChanges();

            return booksToDelete.Count();

        }

    }
}
