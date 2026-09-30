using GalaxyGauntlet.Common;

namespace GalaxyGauntlet.APWorldGen.CodeGen
{
    public static partial class APWorldCodeGen
    {
        public static void GenerateOptionClass()
        {
            FileLogger.QuietLogMessage("Creating options.py...");
            List<string> fileLines = [
                "from dataclasses import dataclass",
                "from Options import Choice, DeathLink, PerGameCommonOptions, Range, Toggle",
                "",
                "class PlayerSpriteOption(Choice):",
                "    \"\"\"",
                "    Determines the sprite to use for Chip when displaying the player character",
                "    \"\"\"",
                "    display_name = \"Player Sprite\"",
                "    option_Chip = 0",
                "    option_Teeth = 1",
                "    option_Tank = 2",
                "    option_Glider = 3",
                "    option_Bug = 4",
                "    option_Paramecium = 5",
                "    option_Fireball = 6",
                "",
                "class ChipsAsCheckProbability(Range):",
                "    \"\"\"",
                "    Specifies the odds (out of 100) that a given chip item will be included as a location.",
                "    Set this value to 0 to disable chips as checks.",
                "    \"\"\"",
                "    display_name = \"Chip as Check Probability\"",
                "    range_start = 0",
                "    range_end = 100",
                "    default = 0",
                "",
                "class ScoreGoal(Range):",
                "    \"\"\"",
                "    Specifies the minimum score requirement to reach the goal condition.",
                "    Set this value to 0 to disable the score as a goal.",
                "    \"\"\"",
                "    display_name = \"Goal Score Requirement\"",
                "    range_start = 0",
                $"    range_end = {MaximumScore}",
                "    default = 0",
                "",
                "class AddExtendedScoreChecks(Toggle):",
                "    \"\"\"",
                "    Add additional checks for higher score milestones.",
                "    \"\"\"",
                "    display_name = \"Include Extended Score Checks\"",
                ""
            ];
            if(IncludeExtendedScoreChecks)
            {
                fileLines.AddRange([
                    "class AddExtendedScoreChecks(Toggle):",
                    "    \"\"\"",
                    "    Add additional checks for higher score milestones.",
                    "    \"\"\"",
                    "    display_name = \"Include Extended Score Checks\"",
                    ""
                ]);
            }
            fileLines.AddRange([
                "@dataclass",
                $"class {ClassPrefix}Options(PerGameCommonOptions):",
                "    player_sprite: PlayerSpriteOption",
                "    chip_as_check_probability: ChipsAsCheckProbability",
                "    maximum_score_goal: ScoreGoal",
                "    death_link: DeathLink"
            ]);
            if(IncludeExtendedScoreChecks)
            {
                fileLines.Add(
                "    include_extended_score_checks: AddExtendedScoreChecks"
                );
            }

            WriteFileContents("options.py", fileLines);
            FileLogger.QuietLogMessage("File created");
        }
    }
}
