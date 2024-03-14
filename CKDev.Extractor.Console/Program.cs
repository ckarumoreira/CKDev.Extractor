using System.Diagnostics;
using System.Reflection;
using CKDev.Extractor.Console;
using Newtonsoft.Json;

internal class Program {
    private static void Main(string[] args) {
        const int cardDepth = 1;

        string dir = AppDomain.CurrentDomain.BaseDirectory;

        string[] headers = new[] {
            "Set",
            "Number",
            "Name",
            "Mana_Cost",
            "Color_Identity",
            "CMC",
            "Type",
            "Rarity",
            "USD_Cost"
        };

        ConsoleColor startingForeground = AddHeaderToConsole();

        var sw = Stopwatch.StartNew();

        List<Card> data = new();

        var config = LoadAppConfig(dir);

        Console.Write("Are you ready to proceed? (Y/n)");

        ConsoleKey key = Console.ReadKey().Key;

        if (key != ConsoleKey.Y && key != ConsoleKey.Enter) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Operation cancelled!");
            Console.ForegroundColor = startingForeground;
            return;
        }

        var stream = new FileStream(config.InputBulkPath.Value, FileMode.Open);
        var textReader = new StreamReader(stream);
        var jsonReader = new JsonTextReader(textReader);

        // start array
        jsonReader.Read();

        int cardCount = 0;

        while (jsonReader.Read()) {
            var card = ProcessCard(jsonReader);
            cardCount++;
            if (card.Rarity is "C" or "U")
                data.Add(card);
            Console.CursorLeft = 0;
            Console.Write($"Cards processed: {cardCount:N0} | Valid cards: {data.Count:N0}");
        }

        Console.WriteLine();

        jsonReader.Close();
        sw.Stop();

        Console.WriteLine($"Extraction complete! ({sw.Elapsed.TotalSeconds:N2} seconds)");
        Console.WriteLine("Starting CSV file!");

        sw.Restart();

        string fileName = config.SaveTimestamp.Value
            ? $"{config.OutputFileName.Value}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.csv"
            : $"{config.OutputFileName.Value}.csv";

        var outputStream = File.Create(Path.Combine(config.OutputDir.Value, fileName));

        var writer = new StreamWriter(outputStream);
        writer.WriteLine(string.Join("|", headers));

        cardCount = 0;

        foreach (var card in data) {
            writer.WriteLine(card.ToString());
            Console.CursorLeft = 0;
            cardCount++;
            Console.Write($"{cardCount:N0} cards written to file.");
        }

        Console.WriteLine();

        writer.Close();
        sw.Stop();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"CSV file \"{fileName}\" created at \"{config.OutputDir.Value}\". ({sw.Elapsed.TotalSeconds:N2} seconds)");
        Console.ForegroundColor = startingForeground;
        Console.WriteLine("Press any key to close.");

        Console.ReadKey();

        Card ProcessCard(JsonTextReader jsonReader) {
            var currentCard = new Card();

            jsonReader.Read();
            while (jsonReader.Depth > cardDepth) {
                if (jsonReader.TokenType is JsonToken.PropertyName) {
                    var propName = jsonReader.Value.ToString();
                    if (Card.Fields.Contains(propName)) {
                        _ = propName switch {
                            Card.SetField => currentCard.Set = jsonReader.ReadAsString(),
                            Card.NumberField => currentCard.Number = jsonReader.ReadAsString(),
                            Card.RarityField => currentCard.Rarity = CardRarity.GetKey(jsonReader.ReadAsString()),
                            Card.NameField => currentCard.Name = jsonReader.ReadAsString(),
                            Card.TypeField => currentCard.Type = jsonReader.ReadAsString(),
                            Card.ManaCostField => currentCard.ManaCost = jsonReader.ReadAsString(),
                            Card.ColorIdentityField => currentCard.ColorIdentity = ReadColorIdentity(jsonReader),
                            Card.PricesField => currentCard.UsdCost = ReadPrices(jsonReader),
                            Card.CmcField => currentCard.Cmc = jsonReader.ReadAsDouble().Value.ToString("N0"),
                        };
                    } else {
                        jsonReader.Read();
                    }
                } else {
                    jsonReader.Read();
                }
            }

            return currentCard;
        }

        string? ReadColorIdentity(JsonTextReader jsonReader) {
            jsonReader.Read();

            string colorIdentity = string.Empty;

            if (jsonReader.TokenType is JsonToken.StartArray) {
                while (jsonReader.TokenType is not JsonToken.EndArray) {
                    colorIdentity += jsonReader.ReadAsString();
                }
            }


            return colorIdentity;
        }

        string? ReadPrices(JsonTextReader jsonReader) {
            while (jsonReader.Read()) {
                if (jsonReader.TokenType is JsonToken.PropertyName) {
                    var propName = jsonReader.Value.ToString();
                    if (propName is Card.UsdCostField) {
                        var value = jsonReader.ReadAsDouble();
                        return (value ?? 0).ToString("N2");
                    } else {
                        jsonReader.Read();
                    }
                }
            }

            return string.Empty;
        }
    }

    private static ConsoleColor AddHeaderToConsole() {
        Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
        var version = $"{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.MinorRevision} (build {currentVersion.Build})";
        var startingForeground = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Title = $"Scryfall Extractor v{version}";

        Console.WriteLine(@"
   _____                  __      _ _ ______      _                  _             
  / ____|                / _|    | | |  ____|    | |                | |            
 | (___   ___ _ __ _   _| |_ __ _| | | |__  __  _| |_ _ __ __ _  ___| |_ ___  _ __ 
  \___ \ / __| '__| | | |  _/ _` | | |  __| \ \/ / __| '__/ _` |/ __| __/ _ \| '__|
  ____) | (__| |  | |_| | || (_| | | | |____ >  <| |_| | | (_| | (__| || (_) | |   
 |_____/ \___|_|   \__, |_| \__,_|_|_|______/_/\_\\__|_|  \__,_|\___|\__\___/|_|   
                    __/ |                                                          
                   |___/");

        Console.WriteLine($"\nCurrent Version: v{version}\n");
        Console.ForegroundColor = startingForeground;
        return startingForeground;
    }

    private static ExtractorConfig LoadAppConfig(string dir) {
        var config = new ExtractorConfig(dir);

        try {
            var reader = new ConfigFileReader(dir, "config.cfg", config.GetKeys());
            var configurationPairs = reader.Read();
            config.Load(configurationPairs);
            Console.WriteLine("Configuration file loaded!");
        } catch {
            Console.WriteLine("No configuration file was found. Proceeding with default.");
        }

        Console.WriteLine("Current configuration:");
        Console.WriteLine("> Input File Path: {0}", config.InputBulkPath.Value);
        Console.WriteLine("> Output Directory: {0}", config.OutputDir.Value);
        Console.WriteLine("> Output File: {0}", config.OutputFileName.Value);
        Console.WriteLine("> Add Timestamp To Output File: {0}", config.SaveTimestamp.Value ? "Yes" : "No");
        Console.WriteLine();

        return config;
    }
}