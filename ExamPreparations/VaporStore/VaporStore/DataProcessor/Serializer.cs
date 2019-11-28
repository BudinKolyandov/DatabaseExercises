namespace VaporStore.DataProcessor
{
	using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.ExportDtos;

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
            var result = context.Genres
                .Where(x => genreNames.Contains(x.Name))
                .Select(genre => new GenreDto
                {
                    Id = genre.Id,
                    Genre = genre.Name,
                    Games = genre.Games
                    .Where(g => g.Purchases.Any())
                    .Select(game => new GameDto
                    {
                        Id = game.Id,
                        Title = game.Name,
                        Developer = game.Developer.Name,
                        Tags = string.Join(", ", game.GameTags.Select(x => x.Tag.Name)),
                        Players = game.Purchases.Count
                    })
                    .OrderByDescending(x => x.Players)
                    .ThenBy(x => x.Id)
                    .ToArray(),
                    TotalPlayers = genre.Games.Sum(x => x.Purchases.Count)
                })
                .OrderByDescending(g => g.TotalPlayers)
                .ThenBy(g => g.Id)
                .ToArray();

            var json = JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);

            return json;

		}

		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{
            var typeValue = Enum.Parse<PurchaseType>(storeType);

            var purchases = context.Users
                .Select(u => new UserDto
                {
                    Username = u.Username,
                    Purchases = u.Cards
                    .SelectMany(c => c.Purchases)
                    .Where(p => p.Type == typeValue)
                    .Select(p => new PurchaseDto
                    {
                        Card = p.Card.Number,
                        Cvc = p.Card.Cvc,
                        Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                        Game = new PurchasedGameDto
                        {
                            Title = p.Game.Name,
                            Genre = p.Game.Genre.Name,
                            Price = p.Game.Price
                        }
                    })
                    .OrderBy(p => p.Date)
                    .ToArray(),
                    TotalSpent = u.Cards
                    .SelectMany(c => c.Purchases)
                    .Where(p => p.Type == typeValue)
                    .Sum(p => p.Game.Price)
                })
                .Where(x => x.Purchases.Any())
                .OrderByDescending(x => x.TotalSpent)
                .ThenBy(x => x.Username)
                .ToArray();

            var serializer = new XmlSerializer(typeof(UserDto[]), new XmlRootAttribute("Users"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });
            serializer.Serialize(new StringWriter(sb), purchases, namespaces);

            var result = sb.ToString().TrimEnd();

            return result;
		}
	}
}