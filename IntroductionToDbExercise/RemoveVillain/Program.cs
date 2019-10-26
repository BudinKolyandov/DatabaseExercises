using InitialSetup;
using System;
using System.Data.SqlClient;

namespace RemoveVillain
{
    public class Program
    {
        public const string SelectVillain = @"SELECT Name FROM Villains WHERE Id = @villainId";

        public const string FreeMinions = @"DELETE FROM MinionsVillains 
                                              WHERE VillainId = @villainId";

        public const string DeleteVillain = @"DELETE FROM Villains
                                              WHERE Id = @villainId";

        public static void Main(string[] args)
        {
            int villainId = int.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(Config.ConnnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(SelectVillain, connection))
                {
                    command.Parameters.AddWithValue("@villainId", villainId);

                    string villainName = (string)command.ExecuteScalar();

                    if (villainName == null)
                    {
                        Console.WriteLine("No such villain was found.");
                        return;
                    }

                    using (SqlCommand minionsFreed = new SqlCommand(FreeMinions, connection))
                    {
                        minionsFreed.Parameters.AddWithValue("@villainId", villainId);
                        int count = minionsFreed.ExecuteNonQuery();

                        using (SqlCommand villainToDelete = new SqlCommand(DeleteVillain,connection))
                        {
                            villainToDelete.Parameters.AddWithValue("@villainId", villainId);
                            villainToDelete.ExecuteNonQuery();
                        }
                        Console.WriteLine($"{villainName} was deleted.");
                        Console.WriteLine($"{count} minions were released.");

                    }
                }
            }



        }
    }
}
