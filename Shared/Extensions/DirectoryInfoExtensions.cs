using System.IO;

namespace Shared.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static FileInfo CombineWithFileName(this DirectoryInfo directoryInfo, string fileName)
        {
            return new FileInfo(Path.Combine(directoryInfo.FullName, fileName));
        }

        public static DirectoryInfo CombineWithDirectoryName(this DirectoryInfo directoryInfo, string directoryName)
        {
            return new DirectoryInfo(Path.Combine(directoryInfo.FullName, directoryName));
        }

        public static DirectoryInfo ExistsOrCreate(this DirectoryInfo directoryInfo)
        {

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            return directoryInfo;
        }
    }
}
