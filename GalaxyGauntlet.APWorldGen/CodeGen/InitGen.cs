using GalaxyGauntlet.Common;

namespace GalaxyGauntlet.APWorldGen.CodeGen
{
    public static partial class APWorldCodeGen
    {
        public static void GenerateInitFile()
        {
            FileLogger.QuietLogMessage("Creating __init__.py...");
            WriteFileContents("__init__.py", [$"from .world import {ClassPrefix}World"]);
            FileLogger.QuietLogMessage("File created");
        }
    }
}
