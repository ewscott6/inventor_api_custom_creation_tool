using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using Figgle;

namespace InventorTest
{
    public class FlangeIdentifier
    {
        public string Part { get; set; }
        public string Label { get; set; }
        public double SB_OD { get; set; }
        public double SB_ID { get; set; }
        public double SB_Area { get; set; }
        public string Material { get; set; }
        public string Type { get; set; }
        public double Tube_OD { get; set; }


    }

    public class FlangeDatabase
    {
        private static string connectionString;

        // Constructor
        public FlangeDatabase(string databasePath)
        {
            connectionString = $"Data Source={databasePath};Version=3;";
        }

        // Get a flange by its unique ID
        public static FlangeIdentifier GetFlangeById(string part)
        {
            FlangeIdentifier flange = null;

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                
                string query = "SELECT * FROM Flanges WHERE PART = @part"; 
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@part", part);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            flange = new FlangeIdentifier
                            {
                                Part = reader["PART"].ToString(),
                                Label = reader["LABEL"].ToString(),
                                SB_Area = Convert.ToDouble(reader["SB_Area"]),
                                Material = reader["MAT"].ToString(),
                                Type = reader["TYPE"].ToString(),
                                Tube_OD = Convert.ToDouble(reader["TUBE_OD"])
                            };
                        }
                    }
                }
            }

            return flange;
        }

        // Method to get initial unique values for the type, material, and tube OD columns
        public static List<string> GetUniqueValues(string column)
        {
            string query = $"SELECT DISTINCT {column} FROM Flanges";
            return QueryResults(query);
        }

        // Method to update the options for one column based on the selection in the others
        public static List<string> GetFilteredOptions(string column, string type = null, string material = null, double tubeOD = -1)
        {
            List<string> conditions = new List<string>();

            if(column != "TYPE" && !string.IsNullOrEmpty(type))
            {
                conditions.Add($"TYPE = '{type}'");
            }
            if(column != "MAT" && !string.IsNullOrEmpty(material))
            {
                conditions.Add($"MAT = '{material}'");
            }
            if(column != "TUBE_OD" && tubeOD != -1)
            {
                conditions.Add($"TUBE_OD = {tubeOD}");
            }

            string whereClause = conditions.Count > 0 ? $"WHERE {string.Join(" AND ", conditions)}" : "";
            string query = $"SELECT DISTINCT {column} FROM Flanges {whereClause}";

            return QueryResults(query);
        }

        // Common method to execute a query and return the results
        private static List<string> QueryResults(string query)
        {
            var results = new List<string>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (var cmd = new SQLiteCommand(query, connection))
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(reader[0].ToString());
                    }
                }
            }

            return results;
        }
        /*
                 // Get all flange types available in the database for a selected material
        public List<string> GetFlangeTypesForMaterial(string material)
        {
            var types = new List<string>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT DISTINCT TYPE FROM Flanges WHERE MAT = @mat";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@mat", material);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            types.Add(reader["TYPE"].ToString());
                        }
                    }
                }
            }

            return types;
        }

        // Get all flange types available in the database
        public List<string> GetFlangeTypes()
        {
            var types = new List<string>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT DISTINCT TYPE FROM Flanges";
                using (var cmd = new SQLiteCommand(query, connection))
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        types.Add(reader["TYPE"].ToString());
                    }
                }
            }

            return types;
        }

        // Get all materials available in the database for all/selected flange types
        public List<string> GetMaterialsForType(string type)
        {
            var materials = new List<string>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT DISTINCT MAT FROM Flanges WHERE TYPE = @type";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@type", type);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            materials.Add(reader["MAT"].ToString());
                        }
                    }
                }
            }

            return materials;
        }

        // Get all parts and labels available in the database for filterd available flanges
        public List<FlangeIdentifier> GetPartsAndLabels(string type, string mat, double tubeOD)
        {
            var flanges = new List<FlangeIdentifier>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT PART, LABEL, TUBE_OD FROM Flanges WHERE TYPE = @type AND MAT = @mat AND TUBE_OD = @tubeOD";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@mat", mat);
                    cmd.Parameters.AddWithValue("@tubeOD", tubeOD);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            flanges.Add(new FlangeIdentifier
                            {
                                Part = reader["PART"].ToString(),
                                Label = reader["LABEL"].ToString(),
                                Tube_OD = Convert.ToDouble(reader["TUBE_OD"])
                            });
                        }
                    }
                }
            }

            return flanges;
        }



        // Get all materials available in the database for all/selected flange types
        public List<string> GetAllMaterials()
        {
            var materials = new List<string>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT DISTINCT MAT FROM Flanges";
                using (var cmd = new SQLiteCommand(query, connection))
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        materials.Add(reader["MAT"].ToString());
                    }
                }
            }

            return materials;
        }

        // Get all parts and labels available in the database for all/selected flange types and materials
        public List<double> GetTubeODForTypeAndMaterial(string type, string mat)
        {
            var tubeODs = new List<double>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT DISTINCT TUBE_OD FROM Flanges WHERE TYPE = @type AND MAT = @mat";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@mat", mat);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tubeODs.Add(Convert.ToDouble(reader["TUBE_OD"]));
                        }
                    }
                }
            }

            return tubeODs;
        }

        // Function to get all of the tube ODs for the case when there is no material and/or type selected
        public List<double> GetAllTubeOD()
        {
            var tubeODs = new List<double>();

            using(var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT DISTINCT TUBE_OD FROM Flanges";
                using(var cmd = new SQLiteCommand(query, connection))
                using(SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tubeODs.Add(Convert.ToDouble(reader["TUBE_OD"]));
                    }
                }
            }
            return tubeODs;
        }

         
         */

        public static List<FlangeIdentifier> GetFilteredFlanges(string type = null, string material = null, double tubeOD = -1)
        {
            var flanges = new List<FlangeIdentifier>();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                var conditions = new List<string>();
                if (!string.IsNullOrEmpty(type)) conditions.Add($"TYPE = '{type}'");
                if (!string.IsNullOrEmpty(material)) conditions.Add($"MAT = '{material}'");
                if (tubeOD != -1) conditions.Add($"TUBE_OD = {tubeOD}");

                string whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";
                string query = $"SELECT * FROM Flanges {whereClause}";

                using (var cmd = new SQLiteCommand(query, connection))
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        flanges.Add(new FlangeIdentifier
                        {
                            Part = reader["PART"].ToString(),
                            Label = reader["LABEL"].ToString(),
                            SB_OD = Convert.ToDouble(reader["SB_OD"]),
                            SB_ID = Convert.ToDouble(reader["SB_ID"]),
                            SB_Area = Convert.ToDouble(reader["SB_Area"]),
                            Material = reader["MAT"].ToString(),
                            Type = reader["TYPE"].ToString(),
                            Tube_OD = Convert.ToDouble(reader["TUBE_OD"])
                        });
                    }
                }
            }

            return flanges;
        }

    }


    public class SQL
    {
        private static FlangeDatabase database = new FlangeDatabase($"{FilePaths.flange2Path}flanges.db");
        private static string selectedType = null;
        private static string selectedMaterial = null;
        static FlangeIdentifier selectedFlange = null;
        private static double selectedTubeOD = -1; // Assuming -1 as default to indicate no selection

        // Get the initial unique values for the type, material, and tube OD columns
        private static List<string> typeOptions = FlangeDatabase.GetUniqueValues("TYPE");
        private static List<string> materialOptions = FlangeDatabase.GetUniqueValues("MAT");
        private static  List<string> tubeODOptions = FlangeDatabase.GetUniqueValues("TUBE_OD");

        // Main function
        public static FlangeIdentifier flangeMenu(string flangeNumber)
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                DisplayFlangeMenu(flangeNumber);
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        //SelectFlangeType();
                        typeOptions = FlangeDatabase.GetFilteredOptions("TYPE", selectedType, selectedMaterial, selectedTubeOD);
                        displayOptions(typeOptions);
                        SelectFlangeType();
                        Console.ReadKey();
                        break;
                    case "2":
                        //SelectMaterial();
                        materialOptions = FlangeDatabase.GetFilteredOptions("MAT", selectedType, selectedMaterial, selectedTubeOD);
                        displayOptions(materialOptions);
                        SelectFlangeMaterial();
                        Console.ReadKey();
                        break;
                    case "3":
                        tubeODOptions = FlangeDatabase.GetFilteredOptions("TUBE_OD", selectedType, selectedMaterial, selectedTubeOD);
                        //SelectTubeOD();
                        displayOptions(tubeODOptions, true);
                        Console.ReadKey();
                        break;
                    case "4":
                        DisplayAvailableFlanges();
                        Console.WriteLine("\nPress any key to return to the main menu...");
                        Console.ReadKey();
                        break;

                    case "5":
                        ClearFilter();
                        break;
                    case "6":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
            return selectedFlange;
        }

        // Method to display the typeOptions list 
        public static void displayOptions(List<string> options, bool sorted = false)
        {
            Console.Clear();
            int i = 0;
            Console.WriteLine("Available:");
            if (sorted)
            {
                // Convert the strings to doubles and sort them
                List<double> sortedOptions = options
                    .Select(str => double.Parse(str))
                    .OrderBy(num => num)
                    .ToList();
                

                sortedOptions.ForEach(d => Console.WriteLine($"{ ++i}. {d} in"));
                SelectTubingOD(sortedOptions);
            }

            else
            options.ForEach(d => Console.WriteLine($"{++i}. {d}"));
        }

        // Function to display the flange menu
        static void DisplayFlangeMenu(string f)
        {
            Console.Clear();  // Clear the console for a fresh view of the menu each time it's displayed
            Console.WriteLine($"Flange {f} Selection:\n");
            Console.WriteLine($"Current selected flange type: {(selectedType != null ? $"{selectedType}": "none")}");
            Console.WriteLine($"Current selected material: {(selectedMaterial != null ? $"{selectedMaterial}" : "none")}");
            Console.WriteLine($"Current selected Tube OD: {(selectedTubeOD != -1 ? $"{selectedTubeOD} in" : "none")}");
            // Display the current selected flange's Part and Label or "none" if not selected
            Console.WriteLine($"Current selected flange: {(selectedFlange != null ? $"{selectedFlange.Part}, {selectedFlange.Label}" : "none")}\n");
            Console.WriteLine("1. Select Flange Type");
            Console.WriteLine("2. Select Material");
            Console.WriteLine("3. Select Tube OD");
            Console.WriteLine($"4. Show available {FlangeDatabase.GetFilteredFlanges(selectedType, selectedMaterial, selectedTubeOD).Count} flanges");
            Console.WriteLine("5. Clear Filter");
            Console.WriteLine("6. Exit\n");
            Console.Write("Your choice: ");
        }

        static void DisplayAvailableFlanges()
        {
            var filteredFlanges = FlangeDatabase.GetFilteredFlanges(selectedType, selectedMaterial, selectedTubeOD);
            int i = 0;
            if (filteredFlanges.Any())
            {
                Console.WriteLine("\nAvailable Flanges:");
                foreach (var flange in filteredFlanges)
                {
                    Console.WriteLine($"{++i}. Part: {flange.Part}, Label: {flange.Label}, Type: {flange.Type}, Material: {flange.Material}, Tube OD: {flange.Tube_OD}");
                }
                Console.WriteLine("\nSelect a flange by number or press Enter to return to the main menu: ");
                if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= filteredFlanges.Count)
                {
                    selectedFlange = filteredFlanges[index - 1];
                    Console.WriteLine($"\nFlange selected: {selectedFlange.Part}, {selectedFlange.Label}\n\nPress any key to return to the main menu...");
                    Console.ReadKey();
                }

            }
            else
            {
                Console.WriteLine("No flanges available with the selected filters.");
            }
        }


        // Function to clear the filter
        static void ClearFilter()
        {
            selectedType = null;
            selectedMaterial = null;
            selectedTubeOD = -1; // Reset the Tube OD selection
            Console.WriteLine("Filter has been cleared. Press any key to return to the main menu...");
            Console.ReadKey();
        }


        static void SelectFlangeType()
        {

            Console.Write("\nSelect a Flange Type by number: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= typeOptions.Count)
            {
                selectedType = typeOptions[index - 1];
                Console.WriteLine($"\nFlange Type selected: {selectedType}\n\nPress any key to return to the main menu...");
            }
            else
            {
                Console.WriteLine("Invalid choice, returning to main menu...");
                Console.ReadKey();
            }
        }

        static void SelectFlangeMaterial()
        {

            Console.Write("\nSelect a Flange Material by number: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= materialOptions.Count)
            {
                selectedMaterial = materialOptions[index - 1];
                Console.WriteLine($"\nFlange Material selected: {selectedMaterial}\n\nPress any key to return to the main menu...");
            }
            else
            {
                Console.WriteLine("Invalid choice, returning to main menu...");
                Console.ReadKey();
            }
        }

        static void SelectTubingOD(List<double> t_od)
        {
            Console.WriteLine("Select a Tube OD by number: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= tubeODOptions.Count)
            {
                selectedTubeOD = t_od[index - 1];
                Console.WriteLine($"\nFlange Material selected: {selectedTubeOD}\n\nPress any key to return to the main menu...");
            }
            else
            {
                Console.WriteLine("Invalid choice, returning to main menu...");
                Console.ReadKey();
            }
        }
      


        // Function to compare the tube OD of two flanges to check if they are compatible. 
        static public void isCompatible(FlangeIdentifier fx, FlangeIdentifier fy)
        {
            while (fx.Tube_OD != fy.Tube_OD)
            {
                Console.WriteLine("Flanges are not compatible");
                Console.WriteLine("");
            }
            
        }

        //  Function to print the main menu
        static public void printMainMenu(FlangeIdentifier[] oFS)
        {
            Console.Clear();  // Clear the console for a fresh view of the menu each time it's displayed
            Console.WriteLine(FiggleFonts.Gothic.Render("got flanges?"));
            // Display for each side (top, bottom, etc) the oFS[i-1].Part and oFS[i-1].Label for each flange or "none" if not selected
            Console.WriteLine("Welcome to the Flange Identifier!\n\nPlease choose where on the tubing cross to attach a flange:");
            Console.WriteLine($"1. Top ({(oFS[0] != null ? $"{oFS[0].Part}, {oFS[0].Label}" : "none")})");
            Console.WriteLine($"2. Bottom ({ (oFS[1] != null ? $"{oFS[1].Part}, {oFS[1].Label}" : "none")})");
            Console.WriteLine($"3. Left ({(oFS[2] != null ? $"{oFS[2].Part}, {oFS[2].Label}" : "none")})"); 
            Console.WriteLine($"4. Right ({(oFS[3] != null ? $"{oFS[3].Part}, {oFS[3].Label}" : "none")})");
            Console.WriteLine($"5. Front ({(oFS[4] != null ? $"{oFS[4].Part}, {oFS[4].Label}" : "none")})");
            Console.WriteLine($"6. Back ({(oFS[5] != null ? $"{oFS[5].Part}, {oFS[5].Label}" : "none")})");
            Console.WriteLine("7. Finish Selection");

            Console.Write("\nYour choice: ");

        }
    }

}


