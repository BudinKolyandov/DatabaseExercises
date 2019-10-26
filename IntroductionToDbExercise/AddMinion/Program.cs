using InitialSetup;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace AddMinion
{
    class Program
    {
        public const string SelectMinionId = "SELECT Id FROM Minions WHERE Name = @Name";

        public const string SelectVillainId = "SELECT Id FROM Villains WHERE Name = @Name";

        public const string InsertIntoMV = "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@villainId, @minionId)";
                                  
        public const string InsertIntoVillain = "INSERT INTO Villains (Name, EvilnessFactorId)  VALUES (@villainName, 4)";
                                  
        public const string InsertIntoMinion = "INSERT INTO Minions (Name, Age, TownId) VALUES (@nam, @age, @townId)";
                                  
        public const string InsertIntoTown = "INSERT INTO Towns (Name) VALUES (@townName)";

        public const string SelectTownId = "SELECT Id FROM Towns WHERE Name = @townName";

        static void Main(string[] args)
        {
            int townId = -1;
            int minionId = -1;
            int villainId = -1;

            List<string> minionInfo = Console.ReadLine()
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .ToList();

            string minionName = minionInfo[0];
            int minionAge = int.Parse(minionInfo[1]);
            string minionTown = minionInfo[2];

            List<string> villainInfo = Console.ReadLine()
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Skip(1)
                .ToList();

            string villainName = villainInfo[0];

            using (SqlConnection connection = new SqlConnection(Config.ConnnectionString))
            {
                connection.Open();

                using (SqlCommand selectTown = new SqlCommand(SelectTownId, connection))
                {
                    selectTown.Parameters.AddWithValue("@townName", minionTown);
                    var townCheck = selectTown.ExecuteScalar();
                    
                    if (townCheck != null)
                    {
                        townId = (int)townCheck;
                    }
                    else
                    {
                        using (SqlCommand insertTown = new SqlCommand(InsertIntoTown, connection))
                        {
                            insertTown.Parameters.AddWithValue("@townName", minionTown);
                            insertTown.ExecuteNonQuery();

                            Console.WriteLine($"Town {minionTown} was added to the database.");
                        }
                    }
                }

                using (SqlCommand selectMinion = new SqlCommand(SelectMinionId, connection))
                {
                    selectMinion.Parameters.AddWithValue("@Name", minionName);

                    var minionCheck = selectMinion.ExecuteScalar();                    

                    if (minionCheck != null)
                    {
                        minionId = (int)minionCheck;
                    }
                    else
                    {
                        using (SqlCommand insertMinion = new SqlCommand(InsertIntoMinion, connection))
                        {
                            insertMinion.Parameters.AddWithValue("@Name", minionName);
                            insertMinion.Parameters.AddWithValue("@Age", minionAge);
                            insertMinion.Parameters.AddWithValue("@TownId", townId);

                            insertMinion.ExecuteNonQuery();
                            Console.WriteLine($"Minion {minionName} was added to the database.");
                        }
                    }

                }

                using (SqlCommand sellcetVillain = new SqlCommand(SelectVillainId, connection))
                {
                    sellcetVillain.Parameters.AddWithValue("@Name", villainName);

                    var targetId = sellcetVillain.ExecuteScalar();
                    if (targetId != null)
                    {
                        villainId = (int)targetId;
                    }
                    else
                    {
                        using (SqlCommand insertVillain = new SqlCommand(InsertIntoVillain, connection))
                        {
                            insertVillain.Parameters.AddWithValue("@VillainName", villainName);
                            insertVillain.ExecuteNonQuery();

                            Console.WriteLine($"Villain {villainName} was added to the database.");
                        }
                    }

                }

                using (SqlCommand insertMinionToVillain = new SqlCommand(InsertIntoMV, connection))
                {
                    insertMinionToVillain.Parameters.AddWithValue("@VillainId", villainId);
                    insertMinionToVillain.Parameters.AddWithValue("@MinionId", minionId);
                    insertMinionToVillain.ExecuteNonQuery();

                    Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
                }
            }

        }
    }
}
