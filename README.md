# dotnet-mcp-skeleton-project-vscode

A .NET console app that hosts a [Model Context Protocol](https://modelcontextprotocol.io/) server on standard input and output. An MCP client starts the process and can call one tool, `get_random_character`, which returns a random class and six ability scores.

`Program.cs` sends logs to standard error so they stay off the protocol stream on standard output.

## Prerequisites

The project file targets `net9.0`, so you need the .NET 9 SDK.

```bash
dotnet --version
```

## Install, build, run, and test

Run these from the repository root.

```bash
dotnet restore
dotnet build
dotnet run --project dotnet-mcp-skeleton-project-vscode.csproj
```

`dotnet run` starts the server and waits for newline-delimited JSON-RPC on standard input. It does not print a prompt. Stop it with Ctrl+C.

This repository has no test project. `dotnet test` restores and builds the app, then exits without running tests:

```bash
dotnet test
```

## Usage

After the client sends `initialize`, `tools/list` returns one tool:

| Field | Value |
| --- | --- |
| Name | `get_random_character` |
| Description | `Generates a DnD character with randomclass and base stats.` |
| Arguments | none |

`Tools/CharacterCreator.cs` implements it as `GetRandomCharacter`. The method:

1. Picks one class.
2. Rolls Strength, Dexterity, Constitution, Intelligence, Wisdom, and Charisma.
3. Scores each ability as the sum of the highest three of four six-sided dice (4d6 drop lowest).

Class names come from the `PLAYER_CLASS` environment variable, a comma-separated list. Each entry is trimmed. If `PLAYER_CLASS` is missing or blank, the tool uses:

```text
Warrior, Mage, Rogue, Paladin, Cleric, Ranger, Bard, Barbarian, Druid, Monk, Sorcerer, Warlock
```

Start the server with a shorter list:

```bash
PLAYER_CLASS="Warrior, Mage, Rogue, Paladin" dotnet run --project dotnet-mcp-skeleton-project-vscode.csproj
```

A `tools/call` request has no arguments:

```json
{"jsonrpc":"2.0","id":3,"method":"tools/call","params":{"name":"get_random_character","arguments":{}}}
```

The text result looks like this. The class and scores change on each call:

```text
Your random DnD character:
Class: Mage
Stats (4d6 drop lowest):
STR: 14
DEX: 13
CON: 10
INT: 11
WIS: 13
CHA: 13
```

`initialize` reports the server name `dotnet-mcp-skeleton-project-vscode` and version `1.0.0.0`. The project file does not set an assembly version, so that version is the runtime default.

### VS Code

`.gitignore` ignores `.vscode/`, so this repository does not include a client config. To launch the server from VS Code, create `.vscode/mcp.json` with a top-level `servers` entry ([MCP configuration reference](https://code.visualstudio.com/docs/agents/reference/mcp-configuration)):

```json
{
  "servers": {
    "dotnet-mcp-skeleton-project-vscode": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "./dotnet-mcp-skeleton-project-vscode.csproj"
      ],
      "env": {
        "PLAYER_CLASS": "Warrior, Mage, Rogue, Paladin"
      }
    }
  }
}
```

`env` is optional. Leave it out to use the built-in class list. Because `.vscode/` is ignored, this file stays on your machine unless you change `.gitignore`.

## Layout

```text
.gitignore
dotnet-mcp-skeleton-project-vscode.csproj
Program.cs
README.md
Tools/CharacterCreator.cs
```

- `Program.cs` builds a generic host, registers a stdio MCP server, and adds `CharacterCreator`.
- `Tools/CharacterCreator.cs` defines the only tool.
- `dotnet-mcp-skeleton-project-vscode.csproj` references `ModelContextProtocol` 0.3.0-preview.4 and `Microsoft.Extensions.Hosting` 9.0.8.

`dotnet build` writes `bin/` and `obj/`. Both are gitignored.

No license file is present.
