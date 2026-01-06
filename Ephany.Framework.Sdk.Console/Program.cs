using Ephany.Framework.Sdk;
using Ephany.Framework.Sdk.Models;
using System.Text;

//string? baseUrl = Environment.GetEnvironmentVariable("EPHANY_BASE_URL");
//string? apiKey = Environment.GetEnvironmentVariable("EPHANY_API_KEY");
string? baseUrl = Environment.GetEnvironmentVariable("EPHANY_BASE_URL_LOCAL");
string? apiKey = Environment.GetEnvironmentVariable("EPHANY_API_KEY_LOCAL");

if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("Error: Please set EPHANY_BASE_URL and EPHANY_API_KEY environment variables.");
    return;
}

var opts = new EphanyClientOptions(baseUrl, apiKey, EphanyClientOptions.AuthScheme.ApiKey);
using var client = new EphanyClient(opts);

bool keepRunning = true;

while (keepRunning)
{
    Console.WriteLine("\n=== Ephany SDK CLI Menu ===");
    Console.WriteLine("1. Browse Assets (Paginated)");
    Console.WriteLine("2. View All Assets (Crawler)");
    Console.WriteLine("3. View Only Revit-Ready Assets (RFA)");
    Console.WriteLine("4. Search Assets");
    Console.WriteLine("5. Exit");
    Console.Write("\nSelect an option: ");

    string? choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
                await HandlePaginationMode(client);
                break;

            case "2":
                Console.WriteLine("\nCrawling all pages... please wait.");
                var allAssets = await client.GetAllAssetsAsync();
                PrintAssetTable(allAssets);
                Console.WriteLine($"Total Assets retrieved: {allAssets.Count}");
                break;

            case "3":
                Console.WriteLine("\nFiltering for assets with Revit Families...");
                var revitAssets = await client.GetRevitAssetsAsync();
                // Pass true to show row numbers for selection
                PrintAssetTable(revitAssets, true);
                Console.WriteLine($"Found {revitAssets.Count} assets with RFA files.");

                /* DEV NOTE: Logic for row-based selection and RFA download */
                if (revitAssets.Count > 0)
                {
                    Console.Write("\nEnter row number to download (or press Enter to skip): ");
                    string? input = Console.ReadLine();
                    if (int.TryParse(input, out int row) && row > 0 && row <= revitAssets.Count)
                    {
                        var selectedAsset = revitAssets[row - 1];
                        string fileName = $"{selectedAsset.TypeId}.rfa";
                        string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", fileName);

                        Console.WriteLine($"Downloading to: {downloadPath}...");
                        await client.DownloadFileAsync(selectedAsset.RevitFamilyFile!, downloadPath);
                        Console.WriteLine("Download successful.");
                    }
                }
                break;

            case "4":
                Console.Write("\nEnter search keyword: ");
                string? keyword = Console.ReadLine();
                if (!string.IsNullOrEmpty(keyword))
                {
                    Console.WriteLine($"\nSearching for '{keyword}'...");
                    var searchResult = await client.SearchAssetsAsync(keyword);
                    PrintAssetTable(searchResult.Items);
                    Console.WriteLine($"Found {searchResult.TotalCount} matches.");
                }
                break;

            case "5":
                keepRunning = false;
                break;

            default:
                Console.WriteLine("Invalid selection. Try again.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[Error] {ex.Message}");
        Console.ResetColor();
    }
}

async Task HandlePaginationMode(EphanyClient client)
{
    int currentPage = 1;
    bool inPagination = true;

    while (inPagination)
    {
        Console.WriteLine($"\n--- Page {currentPage} ---");
        var result = await client.GetAssetPageAsync(page: currentPage, pageSize: 10);
        PrintAssetTable(result.Items);

        Console.WriteLine($"Total Assets: {result.TotalCount} | Showing {result.Items.Count} on this page.");

        StringBuilder menuBuilder = new StringBuilder();

        bool hasNext = !string.IsNullOrEmpty(result.NextPageUrl);
        bool hasPrev = currentPage > 1;

        if (hasNext) menuBuilder.Append("[N] Next Page | ");
        if (hasPrev) menuBuilder.Append("[P] Previous Page | ");
        menuBuilder.Append("[E] Exit to Main Menu");

        Console.WriteLine($"\n{menuBuilder.ToString()}");
        Console.Write("Command: ");

        var cmd = Console.ReadLine()?.ToUpper();

        if (cmd == "N" && hasNext)
        {
            currentPage++;
        }
        else if (cmd == "P" && hasPrev)
        {
            currentPage--;
        }
        else if (cmd == "E")
        {
            inPagination = false;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Command unavailable or invalid. Please choose from the options above.");
            Console.ResetColor();
        }
    }
}

/* DEV NOTE: Added showRowNumbers parameter to facilitate selection */
void PrintAssetTable(List<Asset> assets, bool showRowNumbers = false)
{
    if (assets == null || assets.Count == 0)
    {
        Console.WriteLine("No assets found.");
        return;
    }

    const int indexWidth = 4;
    const int nameWidth = 35;
    const int typeWidth = 10;
    const int mfgWidth = 15;
    const int revitReadyWidth = 10;

    /* DEV NOTE: Dynamically build header based on whether row numbers are required */
    string indexHeader = showRowNumbers ? $"{"#".PadRight(indexWidth)} | " : "";
    string header = $"{indexHeader}{"Name".PadRight(nameWidth)} | {"Type ID".PadRight(typeWidth)} | {"Manufacturer".PadRight(mfgWidth)} | {"RFA".PadRight(revitReadyWidth)}";

    Console.WriteLine(new string('-', header.Length));
    Console.WriteLine(header);
    Console.WriteLine(new string('-', header.Length));

    for (int i = 0; i < assets.Count; i++)
    {
        var asset = assets[i];
        string revitLabel = asset.HasRevitFamily ? " [YES]" : " [NO]";

        string indexCol = showRowNumbers ? $"{(i + 1).ToString().PadRight(indexWidth)} | " : "";

        string row = string.Format("{0}{1} | {2} | {3} | {4}",
            indexCol,
            Truncate(asset.Name, nameWidth).PadRight(nameWidth),
            Truncate(asset.TypeId, typeWidth).PadRight(typeWidth),
            Truncate(asset.ManufacturerName ?? "N/A", mfgWidth).PadRight(mfgWidth),
            revitLabel.PadRight(revitReadyWidth));

        Console.WriteLine(row);
    }
    Console.WriteLine(new string('-', header.Length));
}

string Truncate(string? value, int maxLength)
{
    if (string.IsNullOrEmpty(value)) return "";
    return value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
}