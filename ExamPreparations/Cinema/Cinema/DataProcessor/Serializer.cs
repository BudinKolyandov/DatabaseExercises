namespace Cinema.DataProcessor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Cinema.DataProcessor.ExportDto;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var movies = context.Movies
                .Where(x => x.Rating >= rating && x.Projections.Any(p => p.Tickets.Count >= 1))
                .OrderByDescending(x => x.Rating)
                .ThenByDescending(m => m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)))
                .Select(m => new
                {
                    MovieName = m.Title,
                    Rating = m.Rating.ToString("F2"),
                    TotalIncomes = m.Projections.Sum(p => p.Tickets.Sum(t => t.Price)).ToString("F2"),
                    Customers = m.Projections.SelectMany(p => p.Tickets).Select(t => new
                    {
                        FirstName = t.Customer.FirstName,
                        LastName = t.Customer.LastName,
                        Balance = t.Customer.Balance.ToString("F2"),
                    })
                    .OrderByDescending(x => x.Balance)
                    .ThenBy(x => x.FirstName)
                    .ThenBy(x => x.LastName)
                    .ToArray()
                })
                .Take(10)
                .ToArray();

            var json = JsonConvert.SerializeObject(movies, Newtonsoft.Json.Formatting.Indented);

            return json;
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var sb = new StringBuilder();

            var customers = context.Customers
                .Where(x => x.Age >= age)
                .OrderByDescending(x => x.Tickets.Sum(t=>t.Price))
                .Select(c => new ExportCustomerDto
                {
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    SpentMoney = c.Tickets.Sum(t => t.Price).ToString("F2"),
                    SpentTime = TimeSpan.FromSeconds(c.Tickets.Sum(
                        s => s.Projection.Movie.Duration.TotalSeconds)).ToString(@"hh\:mm\:ss")

                })
                .Take(10)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportCustomerDto[]), new XmlRootAttribute("Customers"));

            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            xmlSerializer.Serialize(new StringWriter(sb), customers, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}