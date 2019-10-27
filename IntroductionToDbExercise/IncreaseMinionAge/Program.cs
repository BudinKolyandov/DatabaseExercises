using InitialSetup;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace IncreaseMinionAge
{
    public class Program
    {
        public const string SelectMinions = @"SELECT Name, Age FROM Minions";

        public const string UpdateMinions = @" UPDATE Minions
                                               SET Name = UPPER(LEFT(Name, 1)) + 
                                               SUBSTRING(Name, 2, LEN(Name)), Age += 1
                                               WHERE Id = @Id";

        public static void Main(string[] args)
        {
            List<int> minionIds = Console.ReadLine()
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();

            using (SqlConnection connection = new SqlConnection(Config.ConnnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(UpdateMinions, connection))
                {
                    foreach (var id in minionIds)
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                }

                using (SqlCommand commannd = new SqlCommand(SelectMinions, connection))
                {
                    SqlDataReader reader = commannd.ExecuteReader();

                    while (reader.Read())
                    {
                        string name = (string)reader[0];
                        int age = (int)reader[1];

                        Console.WriteLine($"{name} {age}");
                    }
                }

            }


        }
    }
}
