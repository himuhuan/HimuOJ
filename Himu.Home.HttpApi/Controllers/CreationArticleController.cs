using Himu.EntityFramework.Core;
using Himu.EntityFramework.Core.Entity;
using Himu.HttpApi.Utility;
using Himu.HttpApi.Utility.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Himu.Home.HttpApi.Controllers
{
    [Route("api/creation/article")]
    [ApiController]
    public class CreationArticleController : ControllerBase
    {
        private readonly HimuMySqlContext _context;

        public CreationArticleController(HimuMySqlContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HimuArticle>> GetArticle(long id)
        {
            HimuArticle? article = await _context.Articles.Where(a => a.Id == id)
                .SingleOrDefaultAsync();

            HimuApiResponse<HimuArticle> response = new();
            if (article == null)
            {
                response.Failed("article {id} not found");
                return NotFound(response);
            }
            response.Value = article;
            return Ok(response);
        }
        
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse>> PostArticle([FromBody] PostArticleRequest request)
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            HimuHomeUser? user = await _context.Users.Where(u => u.Id.ToString() == id)
                                                     .Include(u => u.Articles)
                                                     .SingleOrDefaultAsync();
            HimuApiResponse response = new();
            if (user == null)
            {
                response.Failed($"User {id} not exist", HimuApiResponseCode.UnexpectedError);
                return BadRequest(response);
            }
            HimuArticle article = new()
            {
                Author = user,
                Content = request.Content,
                Title = request.Title,
                Brief = request.Brief
            };
            await _context.Articles.AddAsync(article);
            await _context.SaveChangesAsync();
            return Ok(response);
        }

        [HttpPut("{articleId}")]
        [Authorize]
        public async Task<ActionResult<HimuApiResponse>>
            UpdateArticle(long articleId, [FromBody] PostArticleRequest request)
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            HimuHomeUser? user = await _context.Users.Where(u => u.Id.ToString() == id)
                                                     .Include(u => u.Articles)
                                                     .SingleOrDefaultAsync();
            HimuApiResponse response = new();
            if (user == null)
            {
                response.Failed($"User {id} not exist", HimuApiResponseCode.UnexpectedError);
                return BadRequest(response);
            }

            var articleInfo = await _context.Articles.Where(a => a.Id == articleId)
                                                     .Include(a => a.Author)
                                                     .SingleOrDefaultAsync();

            if (articleInfo == null || articleInfo.Author.Id != user.Id)
            {
                response.Failed(
                    "The article does not exist or the user does not have permission to update on the article");
                return BadRequest(response);
            }

            if (!string.IsNullOrEmpty(request.Content))
                articleInfo.Content = request.Content;
            if (!string.IsNullOrEmpty(request.Title))
                articleInfo.Title = request.Title;
            if (!string.IsNullOrEmpty(request.Brief))
                articleInfo.Brief = request.Brief;

            await _context.SaveChangesAsync();
            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<ActionResult<HimuArticleSearchResponse>>
            SearchArticleByAuthorName(string authorName, int pageSize, int page)
        {
            HimuArticleSearchResponse response = new();
            var users = await _context.Users.Where(u => u.UserName.Contains(authorName)).ToListAsync();

            foreach (var user in users)
            {
                var results = await
                    _context.Articles.Where(a => a.Author.Id == user.Id)
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize)
                                     .OrderBy(a => a.Id)
                                     .Select(a => new HimuArticleSearchResponseResult(a.Id, a.Author.Id, a.Title))
                                     .ToListAsync();
                response.Value.Results.AddRange(results);
                if (results.Count >= pageSize)
                    return Ok(response);
            }
            return Ok(response);
        }

        [HttpGet("count")]
        public async Task<ActionResult<HimuApiResponse<int>>>
            CountArticleByAuthorName(string authorName)
        {
            HimuApiResponse<int> response = new();
            var users = _context.Users.Where(u => u.UserName.Contains(authorName));
            int total = 0;
            await foreach (var user in users.AsAsyncEnumerable())
            {
                total += await _context.Articles.Where(a => a.Author.Id == user.Id)
                                                .CountAsync();
            }
            response.Value = total;
            return Ok(response);
        }
    }
}