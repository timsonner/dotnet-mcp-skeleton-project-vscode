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