using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;
using PermutationsLibrary;
using PostSharp.Patterns.Caching.Backends;
using PostSharp.Patterns.Caching;
using System.Diagnostics;

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
                Console.WriteLine("1. Enter configuration.");
                Console.WriteLine("2. Select piece and run program.");
                if (_isVerbose)
                {
                    Console.WriteLine("3. Just output number of permutations, i.e., phone numbers.");
                }
                else
                {
                    Console.WriteLine("3. Show more details, i.e., verbose option.");
                }                
                Console.WriteLine("4. Run off of custom input.");
                Console.WriteLine("5. Exit");
                Console.WriteLine(); // Add a blank line here
                Console.Write("Input: ");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        EnterConfiguration();
                        break;
                    case "2":

                        RunProgramWithJsonConfigurationAndBenchmark();
                        break;
                    case "3":
                        _isVerbose = !_isVerbose;
                        break;
                    case "4": //coincidentally, same problem, same week, different firm
                        MainViaConsoleInput(args);
                        break;
                    case "5":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please enter 1, 2, 3, 4, or 5.");
                        break;
                }
                Console.WriteLine(); // Add another blank line after each iteration
            }
        }

        //NOTE: see CustomInput.txt for sample data
        static void MainViaConsoleInput(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("Enter the name of the piece:");
            string pieceName = Console.ReadLine();

            Console.WriteLine("Enter the number length:");
            int numberLength = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter the valid starting digits (space-separated):");
            string[] validStartingDigits = Console.ReadLine().Split(" ");

            Console.WriteLine("Enter the number of rows:");
            int rows = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter the number of columns:");
            int cols = int.Parse(Console.ReadLine());

            char[][] boardValues = new char[rows][];
            Console.WriteLine("Enter the board layout:");
            for (int i = 0; i < rows; i++)
            {
                string input = Console.ReadLine().Trim(); // Read input and remove leading/trailing whitespace
                string[] parts = input.Split(' '); // Split input by spaces
                char[] charArray = string.Join("", parts).ToCharArray(); // Concatenate parts and convert to char array
                boardValues[i] = charArray;
            }

            // Create the JSON object
            JObject config = new JObject();
            config["board"] = JArray.FromObject(boardValues);
            //config["exclusions"] = new JArray
            //{
            //    $"p => {string.Join(" && ", Array.ConvertAll(validStartingDigits, digit => $"!p.StartsWith(\"{digit}\")"))}", // Exclude numbers not starting with any of the valid starting digits
            //    //"p => !p.All(char.IsDigit)" // Exclude numbers containing any non-digit characters
            //    //"p => p.Contains(\"*\") || p.Contains(\"#\")" // Exclude numbers containing * or #
            //};
            //config["terminators"] = new JArray { $"p => p.Length == {numberLength}" };
            config["pieces"] = JArray.Parse(@"
            [
                {
                    ""name"": ""Pawn"",
                    ""moves"": [ ""u"" ]
                },
                {
                    ""name"": ""Rook"",
                    ""moves"": [ ""U"", ""D"", ""R"", ""L"" ]
                },
                {
                    ""name"": ""Knight"",
                    ""moves"": [ ""uur"", ""uul"", ""llu"", ""lld"", ""rru"", ""rrd"", ""ddl"", ""ddr"" ]
                },
                {
                    ""name"": ""Bishop"",
                    ""moves"": [ ""Q"", ""W"", ""S"", ""A"" ]
                },
                {
                    ""name"": ""Queen"",
                    ""moves"": [ ""U"", ""D"", ""R"", ""L"", ""Q"", ""W"", ""S"", ""A"" ]
                },
                {
                    ""name"": ""King"",
                    ""moves"": [ ""u"", ""d"", ""r"", ""l"", ""q"", ""w"", ""s"", ""a"" ]
                }
            ]");

            Console.WriteLine("JSON configuration:");
            Console.WriteLine(config);

            RunProgramWithConsoleConfiguration(config, pieceName, validStartingDigits.ToList(), numberLength);
        }

        static int RunProgramWithConsoleConfiguration(dynamic config, string pieceName, IList<string> validStartingDigits, int numberLength)
        {
            // Parse board layout from saved configuration
            char[][] boardValues = JsonConvert.DeserializeObject<char[][]>(config.board.ToString());

            // Create board instance
            IBoard board = new Board(boardValues);

            // Parse exclusions from saved configuration
            List<Func<string, bool>> exclusions = new List<Func<string, bool>>(); // LoadTerminators((JArray)config["exclusions"]);
            //exclusions.Add(p => string.Join(" && ", Array.ConvertAll(validStartingDigits, digit => $"!p.StartsWith(\"{digit}\")")));
            exclusions.Add(p => !validStartingDigits.Contains(p.First().ToString())); //hackerrank doesn't support dynamic, so do this
            exclusions.Add(p => !p.All(char.IsDigit)); //make this explicit, breaks when parsed via config

            // Parse terminators from saved configuration
            List<Func<string, bool>> terminators = new List<Func<string, bool>>(); // LoadTerminators((JArray)config["terminators"]);
            terminators.Add(p => p.Length == numberLength);

            // Parse pieces from saved configuration and store configurations
            var pieceConfigs = new List<(string name, string[] moves)>();
            foreach (var pieceConfig in config.pieces)
            {
                string name = pieceConfig["name"].ToString();
                var moves = ((JArray)pieceConfig["moves"]).Select(m => m.ToString()).ToArray();
                pieceConfigs.Add((name, moves));
            }

            // Get the selected piece configuration
            var selectedPieceConfig = pieceConfigs.Single(lmb => lmb.name.ToLower() == pieceName.ToLower());

            // Create the selected piece
            IBoardPiece selectedPiece = new Board.BoardPiece(board, selectedPieceConfig.name, selectedPieceConfig.moves);

            // Create Permuter instance
            var permuter = new Permuter(board, new List<IBoardPiece> { selectedPiece }, exclusions, terminators);

            // Get permutations
            var permutations = permuter.GetPermutations(selectedPiece);
            Console.WriteLine($"Number of permutations: {permutations.Count()}");

            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine();

            return permutations.Count();
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

        static void RunProgramWithJsonConfigurationAndBenchmark()
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

            RunProgramWithJsonConfigurationWithCaching(board, selectedPiece, exclusions, terminators);
            RunProgramWithJsonConfigurationWithNoCaching(board, selectedPiece, exclusions, terminators);
        }

        static void RunProgramWithJsonConfigurationWithCaching(IBoard board, IBoardPiece selectedPiece, IEnumerable<Func<string, bool>> exclusions, IEnumerable<Func<string, bool>> terminators)
        {
            Console.WriteLine("YES Caching (i.e., faster):");
            CachingServices.DefaultBackend = new MemoryCachingBackend();
            RunProgramWithJsonConfiguration(board, selectedPiece, exclusions, terminators);
        }

        static void RunProgramWithJsonConfigurationWithNoCaching(IBoard board, IBoardPiece selectedPiece, IEnumerable<Func<string, bool>> exclusions, IEnumerable<Func<string, bool>> terminators)
        {
            Console.WriteLine("NO Caching (i.e., slower):");
            CachingServices.DefaultBackend = null;
            RunProgramWithJsonConfiguration(board, selectedPiece, exclusions, terminators);
        }

        static void RunProgramWithJsonConfiguration(IBoard board, IBoardPiece selectedPiece, IEnumerable<Func<string, bool>> exclusions, IEnumerable<Func<string, bool>> terminators)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); // Start the stopwatch at the beginning of the method

            // Create Permuter instance
            var permuter = new Permuter(board, new List<IBoardPiece> { selectedPiece }, exclusions, terminators);

            // Get permutations
            var permutations = permuter.GetPermutations(selectedPiece);
            Console.WriteLine($"Number of permutations: {permutations.Count()}");

            if (_isVerbose)
            {
                Console.WriteLine($"Permutations: {string.Join(",", permutations)}");
            }

            stopwatch.Stop(); // Stop the stopwatch after the main operations

            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"Elapsed Time: {stopwatch.ElapsedMilliseconds} ms"); // Display the elapsed time in milliseconds
            Console.WriteLine();
        }
    }
}
