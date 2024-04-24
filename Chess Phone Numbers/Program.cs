using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Phone_Number_Permutations;

namespace Chess_Phone_Numbers
{
    internal class Program
    {
        private static dynamic _savedConfigJson;

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Please choose an option:");
                Console.WriteLine("1. Enter configuration");
                Console.WriteLine("2. Select piece and run program");
                Console.WriteLine("3. Exit");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        EnterConfiguration();
                        break;
                    case "2":
                        RunProgram();
                        break;
                    case "3":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please enter 1, 2, or 3.");
                        break;
                }
            }
        }

        static void EnterConfiguration()
        {
            Console.WriteLine("Enter the path to the configuration file (e.g., config.json, press Enter to use default):");
            string filePath = Console.ReadLine();

            if (string.IsNullOrEmpty(filePath))
            {
                filePath = "Data/KeypadChessTest.json"; // Default configuration file path
            }

            LoadConfiguration(filePath);
        }

        static void LoadConfiguration(string filePath)
        {
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                return;
            }

            // Read the JSON string from the file
            string configJson = File.ReadAllText(filePath);

            // Check if the JSON string is empty
            if (string.IsNullOrWhiteSpace(configJson))
            {
                Console.WriteLine("Configuration JSON is empty.");
                return;
            }

            // Attempt to parse the JSON string
            try
            {
                var jsonConfig = JObject.Parse(configJson);
                _savedConfigJson = jsonConfig;
                Console.WriteLine("Configuration saved.");
            }
            catch (JsonReaderException)
            {
                Console.WriteLine("Invalid JSON format.");
            }
        }

        static void RunProgram()
        {
            if (_savedConfigJson == null)
            {
                Console.WriteLine("No configuration loaded. Loading default configuration.");
                LoadConfiguration("Data/KeypadChessTest.json"); // Load default configuration
            }

            // Define the piece
            Console.WriteLine("Select the piece to use:");
            Console.WriteLine("1. Pawn");
            Console.WriteLine("2. Rook");
            Console.WriteLine("3. Knight");
            Console.WriteLine("4. Bishop");
            Console.WriteLine("5. Queen");
            Console.WriteLine("6. King");

            string pieceChoice = Console.ReadLine();

            IPiece selectedPiece = null;
            switch (pieceChoice)
            {
                case "1":
                    selectedPiece = ChessPieces.Pawn;
                    break;
                case "2":
                    selectedPiece = ChessPieces.Rook;
                    break;
                case "3":
                    selectedPiece = ChessPieces.Knight;
                    break;
                case "4":
                    selectedPiece = ChessPieces.Bishop;
                    break;
                case "5":
                    selectedPiece = ChessPieces.Queen;
                    break;
                case "6":
                    selectedPiece = ChessPieces.King;
                    break;
                default:
                    Console.WriteLine("Invalid piece choice.");
                    return;
            }

            // Parse exclusions from saved configuration
            var exclusions = new List<Func<string, bool>>();
            foreach (var exclusion in _savedConfigJson["exclusions"])
            {
                var condition = exclusion.ToString();
                exclusions.Add(p => p.Contains(condition));
            }

            // Parse terminators from saved configuration
            var terminators = new List<Func<string, bool>>();
            foreach (var terminator in _savedConfigJson["terminators"])
            {
                var condition = terminator.ToString();
                terminators.Add(p => p == condition);
            }

            // Parse board layout from saved configuration
            char[][] boardValues;
            try
            {
                boardValues = JsonConvert.DeserializeObject<char[][]>(_savedConfigJson.board.ToString());
            }
            catch (JsonException)
            {
                Console.WriteLine("Invalid JSON format for the board layout in saved configuration.");
                return;
            }

            // Create board instance
            IBoard board = new Board(boardValues);

            // Create Permuter instance
            var permuter = new Permuter(board, new List<IPiece> { selectedPiece }, exclusions, terminators);

            // Get permutations
            var permutations = permuter.GetPermutations(selectedPiece);

            // Print permutations
            foreach (var permutation in permutations)
            {
                Console.WriteLine(permutation);
            }
        }
    }
}
