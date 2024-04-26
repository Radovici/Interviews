using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;
using PermutationsLibrary;

namespace TelephoneNumberChessTester
{
    internal class Program
    {
        private static bool _isVerbose = false;
        private static dynamic _savedConfigJson;        

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Please choose an option:");
                Console.WriteLine("1. Enter configuration");
                Console.WriteLine("2. Select piece and run program");
                if (_isVerbose)
                {
                    Console.WriteLine("3. Just output number of permutations, i.e., phone numbers.");
                }
                else
                {
                    Console.WriteLine("3. Show more details, i.e., verbose option.");
                }                
                Console.WriteLine("4. Exit");
                Console.WriteLine(); // Add a blank line here
                Console.Write("Input: ");
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
                        _isVerbose = !_isVerbose;
                        break;
                    case "4":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please enter 1, 2, 3, or 4.");
                        break;
                }

                Console.WriteLine(); // Add another blank line after each iteration
            }
        }

        static void EnterConfiguration()
        {
            Console.Write("Enter the path to the configuration file (e.g., config.json, press Enter to use default): ");
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

        private static List<Func<string, bool>> LoadTerminators(JArray terminators)
        {
            var terminatorList = new List<Func<string, bool>>();
            foreach (var terminator in terminators)
            {
                string condition = terminator.ToString();
                var lambdaExpression = ParseLambda(condition);
                terminatorList.Add((Func<string, bool>)lambdaExpression.Compile());
            }
            return terminatorList;
        }

        private static LambdaExpression ParseLambda(string expression)
        {
            // Split the string into parameter and body
            string[] parts = expression.Split(new[] { "=>" }, StringSplitOptions.None);

            // Parse the parameter
            ParameterExpression parameter = Expression.Parameter(typeof(string), parts[0].Trim());

            // Parse the body
            Expression body = System.Linq.Dynamic.Core.DynamicExpressionParser.ParseLambda(new[] { parameter }, null, parts[1].Trim()).Body;

            // Create and return the lambda expression
            return Expression.Lambda(body, parameter);
        }

        static void RunProgram()
        {
            if (_savedConfigJson == null)
            {
                Console.WriteLine("No configuration loaded. Loading default configuration.");
                LoadConfiguration("Data/KeypadChessTest.json"); // Load default configuration
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

            // Parse exclusions from saved configuration
            List<Func<string, bool>> exclusions = LoadTerminators((JArray)_savedConfigJson["exclusions"]);

            // Parse terminators from saved configuration
            List<Func<string, bool>> terminators = LoadTerminators((JArray)_savedConfigJson["terminators"]);

            // Parse pieces from saved configuration and store configurations
            var pieceConfigs = new List<(string name, string[] moves)>();
            foreach (var pieceConfig in _savedConfigJson.pieces)
            {
                string name = pieceConfig["name"].ToString();
                var moves = ((JArray)pieceConfig["moves"]).Select(m => m.ToString()).ToArray();
                pieceConfigs.Add((name, moves));
            }
            //Add manual double down piece
            DoubleDownBoardPiece doubleDownBoardPiece = new DoubleDownBoardPiece(board);
            pieceConfigs.Add((doubleDownBoardPiece.Name, doubleDownBoardPiece.Moves.ToArray()));

            // Display available pieces
            Console.WriteLine("Available pieces:");
            for (int i = 0; i < pieceConfigs.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {pieceConfigs[i].name}");
            }

            // Ask user to select a piece
            Console.Write("Enter the number of the piece you want to play with: ");
            int pieceIndex;
            if (!int.TryParse(Console.ReadLine(), out pieceIndex) || pieceIndex < 1 || pieceIndex > pieceConfigs.Count)
            {
                Console.WriteLine("Invalid input. Please enter a valid piece number.");
                return;
            }

            // Get the selected piece configuration
            var selectedPieceConfig = pieceConfigs[pieceIndex - 1];

            // Create the selected piece
            IBoardPiece selectedPiece;
            if (selectedPieceConfig.name == doubleDownBoardPiece.Name)
            {
                selectedPiece = doubleDownBoardPiece;
            }
            else
            {
                selectedPiece = new Board.BoardPiece(board, selectedPieceConfig.name, selectedPieceConfig.moves);
            }

            // Create Permuter instance
            var permuter = new Permuter(board, new List<IBoardPiece> { selectedPiece }, exclusions, terminators);

            // Get permutations
            var permutations = permuter.GetPermutations(selectedPiece);
            Console.WriteLine($"Number of permutations: {permutations.Count()}");

            if (_isVerbose)
            {
                Console.WriteLine($"Permutations: {string.Join(",", permutations)}");
            }

            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine();
        }
    }
}
