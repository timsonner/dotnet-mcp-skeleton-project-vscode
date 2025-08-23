# dotnet-mcp-skeleton-project-vscode
Skeleton project for .NET Model Context Protocol server with VSCode

### Project structure
```
├── Program.cs
├── README.md
├── <project name>.csproj
├── .vscode
│   └── mcp.json
└── Tools
    └── CharacterCreator.cs
```

### Create new project
```bash
dotnet new console -n <name of project>
cd <project name>
```

### Install the ModelContextProtocol SDK
Note: This version is subject to change. Check most recent version at: https://www.nuget.org/packages/ModelContextProtocol/
```bash
dotnet add package ModelContextProtocol --version 0.3.0-preview.4
```

### Install Hosting package
```bash
dotnet add package Microsoft.Extensions.Hosting
```

### Add MCP server
```
mkdir -p .vscode && touch .vscode/mcp.json
```

This file contains environmental variables and a reference to the main project file.
Note: Environmental variables are not a requirement, but there as deomonstration.

Modify the path to point to the `.csproj` file in the project root

`mcp.json`
```json
{
  "servers": {
    "SkeletonMcpServer": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "./<name of project>.csproj"
      ],
        "env": {
          "PLAYER_CLASS": "Warrior, Mage, Rogue, Paladin"
        }
    }
  }
}
```

### Add Program.cs
`Program.cs` is the main entry point for the application and acts as the server. This file is what starts the MCP server and keeps it running, handling requests. It sets up a host, configures logging, registers the MCP server and tools.

`Program.cs`
```c#
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Configure all logs to go to stderr (stdout is used for the MCP protocol messages).
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

// Add the MCP services: the transport to use (stdio) and the tools to register.
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<CharacterCreator>();

await builder.Build().RunAsync();
```
The `Program.cs` contains a reference to the `CharacterCreator` class in the `CharacterCreator.cs` Tool

### Create Tools for MCP server
```
mkdir -p Tools && touch CharacterCreator.cs
```

The `CharacterCreator.cs` file is an example of MCP server tools accesible to the agent. Add tools to the `Tools` folder using similar syntax.

`CharacterCreator.cs`
```c#
using System.ComponentModel;
using ModelContextProtocol.Server;

internal class CharacterCreator
{
    [McpServerTool]
    [Description("Generates a DnD character with randomclass and base stats.")]
    public string GetRandomCharacter()
    {
        // Get random class from environment variable
        var playerClass = Environment.GetEnvironmentVariable("PLAYER_CLASS");

        // If environment variable is not set, use default classes
        if (string.IsNullOrWhiteSpace(playerClass))
        {
            playerClass = "Warrior, Mage, Rogue, Paladin, Cleric, Ranger, Bard, Barbarian, Druid, Monk, Sorcerer, Warlock";
        }
        var classChoices = playerClass.Split(",");
        var selectedClassIndex = Random.Shared.Next(0, classChoices.Length);
        var chosenClass = classChoices[selectedClassIndex].Trim();

        // Get random stats
        int[] RollStat()
        {
            var rolls = new int[4]; // Create an array to hold 4 dice rolls.
            for (int i = 0; i < 4; i++) // Number of rolls (four rolls 0-3)
                rolls[i] = Random.Shared.Next(1, 7); // Roll a 6-sided die (1–6)
            Array.Sort(rolls); // Sort rolls in ascending order.
            return new int[] { rolls[1], rolls[2], rolls[3] }; // Return highest three rolls (drop the lowest).
        }

        string[] statNames = { "STR", "DEX", "CON", "INT", "WIS", "CHA" };
        int[] stats = new int[6]; // Array to hold the six stat scores.
        for (int i = 0; i < 6; i++) // Iterate over each stat score
        {
            var statRolls = RollStat(); // Perform 4 rolls for this stat
            stats[i] = statRolls[0] + statRolls[1] + statRolls[2]; // Sum the highest three rolls
        }

        var result = $"Your random DnD character:\nClass: {chosenClass}\nStats (4d6 drop lowest):\n";
        for (int i = 0; i < 6; i++)
        {
            result += $"{statNames[i]}: {stats[i]}\n";
        }
        return result.Trim();
    }
}
```

### Open `mcp.json` and start server

### Sample output
```
lets build another character

Ran get_random_character SkeletonMcpServer (MCP Server)
Here is your random DnD character: Class: Rogue
Stats (4d6 drop lowest):
STR: 11
DEX: 10
CON: 12
INT: 12
WIS: 12
CHA: 13

Let me know if you want to generate another or add more details!
```
