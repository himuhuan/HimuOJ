namespace Himu.EntityFramework.Core.Entity.Components
{
    public class CompilerInfo
    {
        public string CompilerName { get; set; } = string.Empty;

        public string? MessageFromCompiler { get; set; } = null;


        /// <summary>
        /// Get compiler info from source file
        /// </summary>
        /// <param name="sourceFile"> source file </param>
        /// <returns> compiler info </returns>
        public static CompilerInfo GetCompilerInfoFromFile(string sourceFile)
        {
            string extension = Path.GetExtension(sourceFile);
            string compilerName = extension switch
            {
                ".cpp" or ".cxx" or ".C" or ".cc" => "g++",
                ".c" => "gcc",
                ".java" => "java",
                ".cs" => "csc",
                _ => "unknown"
            };
            return new CompilerInfo
            {
                CompilerName = compilerName
            };
        }

        /// <summary>
        /// Get compiler info from language
        /// </summary>
        public static CompilerInfo GetCompilerInfoFromLanguage(string lang)
        {
            string compilerName = lang.ToLower() switch
            {
                "cpp" or "c++" or "cxx" => "g++",
                "c" => "gcc",
                "java" => "java",
                "cs" or "csharp" => "csc",
                _ => "unknown"
            };

            return new CompilerInfo
            {
                CompilerName = compilerName
            };
        }
    }
}
