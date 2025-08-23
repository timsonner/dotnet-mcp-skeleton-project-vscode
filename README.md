# dotnet-mcp-skeleton-project-vscode
Skeleton project for .NET Model Context Protocol server with VSCode

### Create new project
```bash
dotnet new console -n <name of project>
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
cd <project name>
mkdir -p .vscode && touch .vscode/mcp.json
```
Modify path to .csproj file
This file contains environmental variables and a reference to the main project file.

mcp.json
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
Program.cs is the main entry point for the application and acts as the server. This file is what starts the MCP server and keeps it running, handling requests. It sets up a host, configures logging, registers the MCP server and tools.

Program.cs
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

### Open mcp.json and start server
