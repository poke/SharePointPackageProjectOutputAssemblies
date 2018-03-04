using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace SharePointPackageProjectOutputAssemblies
{
    /// <summary>
    /// Process the path of <c>EnumeratedFiles</c> items, replacing tokens in
    /// the path with values available during the build process.
    ///
    /// Available tokens are <c>$Configuration$</c> for the current build
    /// configuration, and <c>$OutputPath$</c> for the build output directory.
    /// </summary>
    public class ProcessEnumeratedFilePaths : Task
    {
        public const string ConfigurationToken = "$Configuration$";
        public const string OutputPathToken = "$OutputPath$";

        /// <summary>
        /// Current configuration.
        /// </summary>
        [Required]
        public string Configuration
        { get; set; }

        /// <summary>
        /// Output path of the build process.
        /// </summary>
        [Required]
        public string OutputPath
        { get; set; }

        /// <summary>
        /// Original enumerated files collection.
        /// </summary>
        [Required]
        public ITaskItem[] OriginalEnumeratedFiles
        { get; set; }

        /// <summary>
        /// Resulting enumerated files collection.
        /// </summary>
        [Output]
        public ITaskItem[] EnumeratedFiles
        { get; private set; }

        public override bool Execute()
        {
            EnumeratedFiles = OriginalEnumeratedFiles;
            foreach (var item in EnumeratedFiles)
            {
                // in order to keep all additional metadata, just update the
                // item spec property directly
                item.ItemSpec = ReplaceTokens(item.ItemSpec);
            }
            return true;
        }

        /// <summary>
        /// Replace the tokens in the path.
        /// </summary>
        /// <param name="path">Original file path.</param>
        /// <returns>Resulting file path.</returns>
        private string ReplaceTokens(string path)
        {
            path = Regex.Replace(path, ConfigurationToken, Configuration, RegexOptions.IgnoreCase);
            path = Regex.Replace(path, OutputPathToken, OutputPath, RegexOptions.IgnoreCase);

            return path;
        }
    }
}
