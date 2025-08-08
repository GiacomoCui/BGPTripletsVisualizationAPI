using Shared.Extensions;
using System;
using System.IO;
using static Shared.Enums;

namespace Shared
{
    public static class Locations
    {
        public static readonly string DirectorySeparator = Path.DirectorySeparatorChar.ToString();


        public static readonly DirectoryInfo FileFolder = new DirectoryInfo(GetSolutionPath() + "io").ExistsOrCreate();
        public static readonly DirectoryInfo LoggerFolder = FileFolder.CombineWithDirectoryName("Logging").ExistsOrCreate();

        /// <summary>
        /// Restituisce il path della directory principale dov'è contenuto il file .solution
        /// </summary>
        private static string GetSolutionPath()
        {
            var currentPath = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.IndexOf("BGPTripletsVisualizationAPI"));
            return currentPath + "BGPTripletsVisualizationAPI" + DirectorySeparator;
        }

      
    }
}
