namespace Himu.EntityFramework.Core.Entity
{
    public class HimuArticle
    {
        public long Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Brief { get; set; }

        /// <summary>
        /// Saved by HTML
        /// </summary>
        public string Content { get; set; } = string.Empty;

        public HimuHomeUser Author { get; set; } = null!;

        // public List<HimuArticleComment>? Comments { get; set; } = new();
    }
}