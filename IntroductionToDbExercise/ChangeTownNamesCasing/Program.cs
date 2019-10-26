using InitialSetup;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ChangeTownNamesCasing
{
    public class Program
    {
        public const string TownsUpdate = @"UPDATE Towns
                                            SET Name = UPPER(Name)
                                            WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)";

        public const string TownsSelect = @" SELECT t.Name 
                                             FROM Towns as t
                                             JOIN Countries AS c ON c.Id = t.CountryCode
                                             WHERE c.Name = @countryName";


        public static void Main(string[] args)
        {
            string country = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(Config.ConnnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(TownsUpdate, connection))
                {
                    command.Parameters.AddWithValue(@"countryName", country);
                    int count = command.ExecuteNonQuery();

                    Console.WriteLine($"{count} town names were affected.");
                }

                using (SqlCommand command = new SqlCommand(TownsSelect, connection))
                {
                    command.Parameters.AddWithValue(@"countryName", country);
                    List<string> towns = new List<string>();

                    using (SqlDataReader reader =command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            towns.Add((string)reader[0]);
                        }
                    }

                    if (towns.Count == 0)
                    {
                        Console.WriteLine("No town names were affected.");
                    }
                    else
                    {
                        Console.WriteLine(string.Join(", ", towns));
                    }

                }

            }





        }
    }
}
