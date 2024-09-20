using System.ComponentModel.DataAnnotations;

namespace Himu.EntityFramework.Core.Entity
{
    public class CompilerPreset
    {
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// Supported extensions, separated by comma(,)
        /// </summary>
        public string SupportedExtensions { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;
        
        public string Command { get; set; } = string.Empty;
        
        /// <summary>
        /// Can be shared to other users
        /// </summary>
        public bool Shared { get; set; }

        /// <summary>
        /// Max time limit for the compiler to execute
        /// </summary>
        public TimeSpan Timeout { get; set; }
    }
}
