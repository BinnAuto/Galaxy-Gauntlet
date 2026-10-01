using Godot;

namespace GalaxyGauntlet.Common
{
    public static class FileLogger
    {
        private const string logDirectory = "./logs";

        private static bool _initialized = false;

        public static void Initialize()
        {
            string fullLogPath = Path.GetFullPath(logDirectory);
            if(false == Directory.Exists(fullLogPath))
            {
                Console.WriteLine("Creating log directory...");
                Directory.CreateDirectory(fullLogPath);
                LogMessage("Log directory created");
            }

            #region Delete old logs

            DateTime now = DateTime.Now;
            string[] files = Directory.GetFiles(fullLogPath);
            foreach(string file in files)
            {
                FileInfo fileInfo = new(file);
                var timeSinceLastWrite = (now - fileInfo.LastWriteTime);
                if(timeSinceLastWrite.Days > 2)
                {
                    File.Delete(file);
                    QuietLogMessage($"Log {file} deleted");
                }
            }

            #endregion

            _initialized = true;
        }


        /// <summary>
        /// Writes a message to the log without repeating it to the console
        /// </summary>
        public static void QuietLogMessage(object message)
        {
            if (false == _initialized)
            {
                Initialize();
            }
            DateTime timeStamp = DateTime.Now;
            string logMessage = $"[{timeStamp:MM/dd HH:mm:ss.fff}] - {message}";
            string filePath = $"./logs/{timeStamp:yyyy-MM-dd}.log";
            File.AppendAllLines(filePath, [logMessage]);
        }


        public static void LogException(string message, Exception e)
        {
            LogMessage($"{message}: {e.Message}");
            QuietLogMessage(e.StackTrace);
        }


        /// <summary>
        /// Writes a message to the log and repeats it to the console
        /// </summary>
        public static void LogMessage(object message)
        {
            QuietLogMessage(message);
            Console.WriteLine(message);
        }
    }
}
