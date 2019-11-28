namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.ImportDtos;

    public static class Deserializer
	{
		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
            StringBuilder sb = new StringBuilder();
            var games = new List<Game>();
            var gamesDto = JsonConvert.DeserializeObject<ImportGameDto[]>(jsonString);

            foreach (var gameDto in gamesDto)
            {
                if (!IsValid(gameDto) || gameDto.Tags.Count == 0)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var game = new Game
                {
                    Name = gameDto.Name,
                    Price = gameDto.Price,
                    ReleaseDate = DateTime.ParseExact(gameDto.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                };

                var developer = GetDeveloper(context, gameDto.Developer);

                var genre = GetGenre(context, gameDto.Genre);

                game.Developer = developer;
                game.Genre = genre;

                foreach (var currentTag in gameDto.Tags)
                {
                    var tag = GetTag(context, currentTag);
                    game.GameTags.Add(new GameTag
                    {
                        Tag = tag
                    });
                }

                games.Add(game);
                sb.AppendLine($"Added {game.Name} ({game.Genre.Name}) with {game.GameTags.Count} tags");
            }

            context.Games.AddRange(games);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
		}

        private static Tag GetTag(VaporStoreDbContext context, string currentTag)
        {
            var tag = context.Tags.FirstOrDefault(x => x.Name == currentTag);

            if (tag == null)
            {
                tag = new Tag
                {
                    Name = currentTag
                };

                context.Tags.Add(tag);
                context.SaveChanges();
            }
            return tag;
        }

        private static Genre GetGenre(VaporStoreDbContext context, string gameDtogenre)
        {
            var genre = context.Genres.FirstOrDefault(x => x.Name == gameDtogenre);

            if (genre == null)
            {
                genre = new Genre
                {
                    Name = gameDtogenre
                };
                context.Genres.Add(genre);
                context.SaveChanges();
            }

            return genre;
        }

        private static Developer GetDeveloper(VaporStoreDbContext context, string gameDtoDev)
        {
            var developer = context.Developers.FirstOrDefault(x => x.Name == gameDtoDev);

            if (developer == null)
            {
                developer = new Developer
                {
                    Name = gameDtoDev
                };

                context.Developers.Add(developer);
                context.SaveChanges();
            }

            return developer;
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
            StringBuilder sb = new StringBuilder();

            var users = new List<User>();

            var userDtos = JsonConvert.DeserializeObject<ImportUsersDto[]>(jsonString);

            foreach (var userDto in userDtos)
            {
                if (!IsValid(userDto) || !userDto.Cards.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var user = new User
                {
                    FullName = userDto.FullName,
                    Username = userDto.Username,
                    Email = userDto.Email,
                    Age = userDto.Age
                };

                foreach (var card in userDto.Cards)
                {
                    var currentCard = context.Cards.FirstOrDefault(x => x.Number == card.Number);
                    if (currentCard == null)
                    {
                        context.Cards.Add(card);
                        user.Cards.Add(card);
                    }
                }

                users.Add(user);
                sb.AppendLine($"Imported {user.Username} with {user.Cards.Count} cards");

            }
            context.Users.AddRange(users);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
		}


        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(ImportPurchasesDto[]), new XmlRootAttribute("Purchases"));

            var purchaseDtos = (ImportPurchasesDto[])serializer.Deserialize(new StringReader(xmlString));

            var purchases = new List<Purchase>();

            foreach (var purchaseDto in purchaseDtos)
            {
                if (!IsValid(purchaseDto))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var game = context.Games.Single(x => x.Name == purchaseDto.Title);
                var card = context.Cards.Include(c => c.User).Single(x => x.Number == purchaseDto.Card);
                var date = DateTime.ParseExact(purchaseDto.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                var purchase = new Purchase
                {
                    Game = game,
                    Type = purchaseDto.Type,
                    Card = card,
                    ProductKey = purchaseDto.Key,
                    Date = date
                };

                purchases.Add(purchase);
                sb.AppendLine($"Imported {purchase.Game.Name} for {purchase.Card.User.Username}");
            }
            context.Purchases.AddRange(purchases);

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