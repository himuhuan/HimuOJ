using Microsoft.AspNetCore.Http;

namespace Himu.HttpApi.Utility
{
    public delegate bool FileExtensionChecker(string extension);

    public class UploadActionFactory
    {
        /// <summary>
        /// 小于或等于0相当于无限制
        /// </summary>
        public long FileSizeLimit { get; set; }

        /// <summary>
        /// 默认不对文件后缀名检查
        /// </summary>
        /// <remarks> ExtensionChecker 只对 <see cref="Extension"/> 负责 </remarks>
        public FileExtensionChecker ExtensionChecker { get; set; } = _ => true;

        /// <summary>
        /// 默认保存文件本身的后缀名
        /// </summary>
        /// <remarks>
        /// <see cref="SaveAs(string)"> SaveAs </see> 和
        /// <see cref="ChangeFileExtension(string)"> ChangeFileExtension </see> 均会改变本项的值, 取决于最后一个调用的函数。
        /// </remarks>
        public string? Extension { get; set; }

        /// <summary>
        /// 为了安全，默认保存随机文件名 (<b>本属性不会保存文件名后缀</b>)
        /// </summary>
        public string NameWithoutExtension { get; set; } = Path.GetTempFileName();

        /// <summary>
        /// 文件保存根路径，如果路径不存在会自动创建, 默认为空
        /// </summary>
        public string RootPath { get; set; } = string.Empty;

        /// <summary>
        /// 文件保存相对路径，如果路径不存在会自动创建, 默认在本目录生成
        /// </summary>
        public string RelativePath { get; set; } = string.Empty;

        public string RelativePathWithFullName => Path.Combine(RelativePath, NameWithoutExtension) + Extension;

        public string AbsolutePath => Path.Combine(RootPath, RelativePath);

        public string AbsolutePathWithFullName => Path.Combine(AbsolutePath, NameWithoutExtension) + Extension;

        public UploadActionFactory()
        { }

        /// <summary>
        /// 终结方法: 保存文件并异步写入物理存储.
        /// </summary>
        /// <param name="file"> 要保存的文件 </param>
        /// <returns> HimuApiResponse 作为回应 </returns>
        public async Task<HimuApiResponse> SaveAsync(IFormFile file)
        {
            HimuApiResponse response = new();
            if (file.Length < 0) { return response.Failed("illegal request"); }
            if (FileSizeLimit > 0 && file.Length > FileSizeLimit) return response.Failed("too large file");
            string extension = Extension ??= Path.GetExtension(file.FileName);
            if (!ExtensionChecker(extension)) return response.Failed("Unsupported file format");
            if (!Directory.Exists(AbsolutePath)) Directory.CreateDirectory(AbsolutePath);
            using (var stream = File.Create(AbsolutePathWithFullName))
            {
                await file.CopyToAsync(stream);
            }
            response.Message = RelativePathWithFullName.Replace("\\", "/");
            return response;
        }

        /// <summary>
        /// 设定文件大小限制, 小于或等于0相当于无限制
        /// </summary>
        public UploadActionFactory MaxFileSize(long fileSize)
        {
            FileSizeLimit = fileSize;
            return this;
        }

        /// <summary>
        /// 设定文件后缀名检查器, 默认无检查
        /// </summary>
        /// <param name="checker">检查器, 参见<see cref="FileExtensionChecker"/></param>
        public UploadActionFactory CheckExtensions(FileExtensionChecker checker)
        {
            ExtensionChecker = checker;
            return this;
        }

        /// <summary>
        /// 设定文件保存后缀名，实际保存的后缀名取决于调用的先后顺序
        /// </summary>
        /// <param name="extension">后缀名</param>
        public UploadActionFactory ChangeFileExtension(string extension)
        {
            Extension = extension;
            return this;
        }

        /// <summary>
        /// 设定文件保存名，实际保存的后缀名取决于调用的先后顺序
        /// </summary>
        /// <param name="fileName">文件名</param>
        public UploadActionFactory SaveAs(string fileName)
        {
            NameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            if (!string.IsNullOrEmpty(extension))
                Extension = extension;
            return this;
        }

        /// <summary>
        /// 设定保存的路径文件保存名，实际保存的后缀名和路径取决于调用的先后顺序
        /// </summary>
        /// <param name="path"> 路径 </param>
        /// <param name="fileName">文件名</param>
        public UploadActionFactory SaveAs(string path, string fileName)
        {
            RelativePath = path;
            return SaveAs(fileName);
        }

        /// <summary>
        /// 设定文件保存的根路径, <see cref="SaveAs(string)"/> 或者其它改变路径的函数会在此基础上继续
        /// </summary>
        /// <param name="path">根路径</param>
        public UploadActionFactory SetRootPath(string path)
        {
            RootPath = path;
            return this;
        }
    }

    public static class DefaultExtensionsChecker
    {
        public static bool ImageExtensionChecker(string extension)
        {
            extension = extension.ToLowerInvariant();
            return extension switch
            {
                ".png" or ".webp" or ".jpg" or ".jpeg" => true,
                _ => false
            };
        }

        public static bool SourceExtensionChecker(string extension)
        {
            extension = extension.ToLowerInvariant();
            return extension switch
            {
                ".cc" or ".c++" or ".cpp" or ".cxx" or ".C" => true,
                _ => false
            };
        }
    }
}