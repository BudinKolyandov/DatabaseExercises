using InitialSetup;
using System;
using System.Data.SqlClient;

namespace MinionNames
{
    class Program
    {
        static void Main(string[] args)
        {
            int id = int.Parse(Console.ReadLine());

            string villainQuery = @"SELECT Name FROM Villains WHERE Id = @Id";

            string minionsQuery = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = @Id
                                ORDER BY m.Name";

            using (SqlConnection connection = new SqlConnection(Config.ConnnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(villainQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    string villainName = (string)command.ExecuteScalar();

                    if (villainName == null)
                    {
                        Console.WriteLine($"No villain with ID {id} exists in the database.");
                        return;
                    }

                    Console.WriteLine($"Villain: {villainName}");
                }

                using (SqlCommand command = new SqlCommand(minionsQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("(no minions)");
                            return;
                        }

                        while (reader.Read())
                        {
                            long row = (long)reader[0];
                            string name = (string)reader[1];
                            int age = (int)reader[2];

                            Console.WriteLine($"{row}. {name} {age}");
                        }
                    }
                }

            }

        }
    }
}
