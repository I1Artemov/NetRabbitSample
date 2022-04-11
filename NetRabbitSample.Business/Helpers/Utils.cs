using System;

namespace NetRabbitSample.Business.Helpers
{
    public static class Utils
    {
        public static void WriteDebugLog(string text)
        {
            string dateStr = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            System.Console.WriteLine(string.Format("[{0}] {1}", dateStr, text));
        }
    }
}
