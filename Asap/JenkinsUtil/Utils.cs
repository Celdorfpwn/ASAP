using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JenkinsService
{
    internal class Utils
    {
        /// <summary>
        /// Converts the provided double timestamp to DateTime
        /// </summary>
        /// <param name="timestamp">string timestamp to convert</param>
        /// <returns>DateTime object</returns>
        public static DateTime ConvertTimestampToDateTime(long timestamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            double converted = Math.Round(Convert.ToDouble(timestamp) / 1000);
            return dtDateTime.AddSeconds(converted).ToLocalTime();
        }

        /// <summary>
        /// Converts the provided result string to the Enums.BuildProcess
        /// </summary>
        /// <param name="input">The string input to parse</param>
        /// <returns>The returned build state</returns>
        public static BuildState StringToBuildResultConverter(string input)
        {
            switch (input)
            {
                case "SUCCESS":
                    return BuildState.Success;
                case null:
                    return BuildState.Building;
                case "FAILURE":
                default:
                    return BuildState.Failed;
            }
        }

        /// <summary>
        /// Convert iteration name to version
        /// </summary>
        /// <returns>string representing version</returns>
        public static string GetVersionFromInterim(string iteration)
        {
            string ver = String.Empty;

            // iteration example: 15Q3-4
            // version example:   1.153.4.0
            string year = iteration.Substring(0, 2); //15
            string quater_1 = iteration.Substring(3, 1); //3
            string quater_2 = iteration.Substring(5, 1); //4

            ver = String.Format("1.{0}{1}.{2}.0", year, quater_1, quater_2);
            return ver;
        }

        /// <summary>
        /// Copy source folder content to destination folder
        /// </summary>
        /// <param name="SourcePath">Source folder</param>
        /// <param name="DestinationPath">Destination folder</param>
        /// <returns>True if copy was successfull, otherwise false</returns>
        public static bool CopyFolderContents(string SourcePath, string DestinationPath)
        {
            SourcePath = SourcePath.EndsWith(@"\") ? SourcePath : SourcePath + @"\";
            DestinationPath = DestinationPath.EndsWith(@"\") ? DestinationPath : DestinationPath + @"\";

            try
            {
                if (Directory.Exists(SourcePath))
                {
                    if (Directory.Exists(DestinationPath) == false)
                    {
                        Directory.CreateDirectory(DestinationPath);
                    }

                    foreach (string files in Directory.GetFiles(SourcePath))
                    {
                        FileInfo fileInfo = new FileInfo(files);
                        fileInfo.CopyTo(string.Format(@"{0}\{1}", DestinationPath, fileInfo.Name), true);
                    }

                    foreach (string drs in Directory.GetDirectories(SourcePath))
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(drs);
                        if (CopyFolderContents(drs, DestinationPath + directoryInfo.Name) == false)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
