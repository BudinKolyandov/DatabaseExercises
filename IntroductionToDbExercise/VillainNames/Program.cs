using InitialSetup;
using System;
using System.Data.SqlClient;

namespace VillainNames
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (SqlConnection connection = new SqlConnection(Config.ConnnectionString))
            {
                connection.Open();

                string selectVillains = $@"  SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
                                             FROM Villains AS v
                                             JOIN MinionsVillains AS mv ON v.Id = mv.VillainId
                                             GROUP BY v.Id, v.Name
                                             HAVING COUNT(mv.VillainId) > 3
                                             ORDER BY COUNT(mv.VillainId)";

                using (SqlCommand command = new SqlCommand(selectVillains, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {

                            string name = reader.GetString(0);

                            int count = reader.GetInt32(1);

                            Console.WriteLine($"{name} - {count}");
                        }
                    }


                }

            }
        }
    }
}


