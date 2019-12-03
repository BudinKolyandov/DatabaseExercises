namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Cinema.Data.Models;
    using Cinema.Data.Models.Enums;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie 
            = "Successfully imported {0} with genre {1} and rating {2}!";
        private const string SuccessfulImportHallSeat 
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection 
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket 
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var serialize = JsonConvert.DeserializeObject<ImportMoviesDto[]>(jsonString);

            var movies = new List<Movie>();

            foreach (var movieDto in serialize)
            {
                if (!IsValid(movieDto) || movies.Exists(x=>x.Title == movieDto.Title))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Genre genre;

                if (Enum.TryParse(movieDto.Genre, out genre))
                {
                    var movie = new Movie
                    {
                        Title = movieDto.Title,
                        Genre = genre,
                        Duration = TimeSpan.Parse(movieDto.Duration),
                        Rating = movieDto.Rating,
                        Director = movieDto.Director
                    };
                    movies.Add(movie);
                    sb.AppendLine($"Successfully imported {movie.Title} with genre {movie.Genre} and rating {movie.Rating:f2}!");
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
            }
            context.Movies.AddRange(movies);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var hallDtos = JsonConvert.DeserializeObject<ImportHallDto[]>(jsonString);

            var halls = new List<Hall>();
            foreach (var hallDto in hallDtos)
            {
                if (!IsValid(hallDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var hall = new Hall
                {
                    Name = hallDto.Name,
                    Is4Dx = hallDto.Is4Dx,
                    Is3D = hallDto.Is3D
                };

                for (int i = 0; i < hallDto.Seats; i++)
                {
                    hall.Seats.Add(new Seat());
                }
                halls.Add(hall);
                string status = string.Empty;

                if (hall.Is4Dx)
                {
                    if (hall.Is3D)
                    {
                        status = "4Dx/3D";
                    }
                    else
                    {
                        status = "4Dx";
                    }
                }
                else if (hall.Is3D)
                {
                    status = "3D";
                }
                else
                {
                    status = "Normal";
                }

                sb.AppendLine($"Successfully imported {hall.Name}({status}) with {hall.Seats.Count} seats!");
            }
            context.Halls.AddRange(halls);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var xmlSerializer = new XmlSerializer(typeof(ImportProjectionDto[]), new XmlRootAttribute("Projections"));
            var projectionsDtos = (ImportProjectionDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var projections = new List<Projection>();

            foreach (var projectionDto in projectionsDtos)
            {
                var movie = context.Movies.Find(projectionDto.MovieId);
                var hall = context.Halls.Find(projectionDto.HallId);

                if (movie == null || hall == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var projection = new Projection
                {
                    MovieId = projectionDto.MovieId,
                    HallId = projectionDto.HallId,
                    DateTime = DateTime.ParseExact(projectionDto.DateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                };
                projections.Add(projection);
                sb.AppendLine($"Successfully imported projection {movie.Title} on {projection.DateTime.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}!");
            }
            context.Projections.AddRange(projections);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var xmlSerializer = new XmlSerializer(typeof(ImportCustomerDto[]), new XmlRootAttribute("Customers"));
            var customersDtos = (ImportCustomerDto[])xmlSerializer.Deserialize(new StringReader(xmlString));
            var customers = new List<Customer>();

            foreach (var customerDto in customersDtos)
            {
                var projections = context.Projections.Select(x => x.Id).ToArray();
                var validProjection = projections.Any(x => customerDto.Tickets.Any(s => s.ProjectionId != x));

                if (!IsValid(customerDto) && customerDto.Tickets.All(IsValid) && validProjection)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var customer = new Customer
                {
                    FirstName = customerDto.FirstName,
                    LastName = customerDto.LastName,
                    Age = customerDto.Age,
                    Balance = customerDto.Balance
                };

                foreach (var ticket in customerDto.Tickets)
                {
                    customer.Tickets.Add(new Ticket
                    {
                        ProjectionId = ticket.ProjectionId,
                        Price = ticket.Price
                    });
                }

                sb.AppendLine($"Successfully imported customer {customer.FirstName} {customer.LastName} with bought tickets: {customer.Tickets.Count}!");

                customers.Add(customer);
            }
            context.Customers.AddRange(customers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, validationContext, validationResult, true);

            return isValid;
        }

    }
}