# Ephany .NET SDK

The official .NET client for interacting with the **Ephany Framework**. This SDK provides a strongly-typed wrapper around the Ephany REST API, making it easy to integrate asset management into Revit plugins, Rhino/Grasshopper tools, and standalone .NET applications.

**Backend API Repository:** [TripleZeroLabs/Ephany-Framework](https://github.com/TripleZeroLabs/Ephany-Framework)

## Features
* **Hybrid Authentication:** Seamless support for both Machine-to-Machine (API Key) and User-Interactive (Auth Token) workflows.
* **Asset Browsing:** Search, filter, and paginate through your asset library.
* **Type Safety:** Full C# models for Assets, Files, Manufacturers, and Categories.
* **Multi-Targeting:** Compatible with .NET Framework 4.8 (legacy Revit versions) and .NET Framework 8 (for Revit 2025 and up).

## Installation
*Currently in v0.1.0*

1.  Download the latest release from the Releases Page.
2.  Add a reference to `Ephany.Framework.Sdk.dll` in your Visual Studio project.
3.  Ensure you have `Newtonsoft.Json` installed via NuGet.

## Getting Started

### 1. Authentication
The `EphanyClient` supports two modes of authentication depending on your use case.

#### A. Machine Authentication (API Key)
Use this for scripts, console apps, or background services that run without a user interface.
* **Header Sent:** `X-Api-Key: <your-key>`

```csharp
using Ephany.Framework.Sdk;

// 1. Configure the client with your Base URL and API Key
var opts = new EphanyClientOptions(
    "http://127.0.0.1:8000/api",    // Your API Base URL
    "your-machine-api-key",         // Your API Key
    AuthScheme.ApiKey               // Explicitly set scheme
);

// 2. Instantiate the client
using (var client = new EphanyClient(opts))
{
    // Ready to make requests
}
```

#### B. User Authentication (Login Token)
Use this for desktop plugins (Revit, AutoCAD, Rhino, etc.) where a user logs in with their credentials.
* **Header Sent:** `Authorization: Token <user-token>`

```csharp
using Ephany.Framework.Sdk;

// Assume you retrieved this token via your login UI
string userToken = "a1b2c3d4e5f6..."; 

var opts = new EphanyClientOptions(
    "http://127.0.0.1:8000/api",
    userToken,
    AuthScheme.UserToken // Tells the SDK to treat this as a Bearer/Token
);

using (var client = new EphanyClient(opts))
{
    // All requests are now authenticated as the logged-in user
}
```

### 2. Usage Example: Browsing Assets

```csharp
public async Task LoadFurnitureAsync()
{
    // Setup Client
    var opts = new EphanyClientOptions("http://localhost:8000/api", "my-key", AuthScheme.ApiKey);
    using (var client = new EphanyClient(opts))
    {
        // 1. Search for items
        var results = await client.SearchAssetsAsync("Chair", page: 1, pageSize: 20);
        
        Console.WriteLine($"Found {results.TotalCount} chairs.");

        foreach (var asset in results.Items)
        {
            Console.WriteLine($" - {asset.Name} ({asset.Manufacturer.Name})");
        }

        // 2. Pagination
        if (!string.IsNullOrEmpty(results.NextPageUrl))
        {
            // Load next page logic...
        }
    }
}
```

## License
This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.