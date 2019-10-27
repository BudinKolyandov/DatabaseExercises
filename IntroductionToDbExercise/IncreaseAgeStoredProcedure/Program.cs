using InitialSetup;
using System;
using System.Data;
using System.Data.SqlClient;

namespace IncreaseAgeStoredProcedure
{
    public class Program
    {
        public const string SelectMinions = @"SELECT Name, Age FROM Minions WHERE Id = @Id";

        public static void Main(string[] args)
        {
            int id = int.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(Config.ConnnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("usp_GetOlder", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Id", id);

                    command.ExecuteNonQuery();
                }

                using (SqlCommand command = new SqlCommand(SelectMinions, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string name = (string)reader[0];
                        int age = (int)reader[1];

                        Console.WriteLine($"{name} – {age} years old");
                    }

                }

            }



        }
    }
}
