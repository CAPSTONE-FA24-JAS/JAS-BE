using Application.Interfaces;
using Application.Services;
using Application.ViewModels.BlogDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    
    public class BlogController : BaseController
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<IActionResult> ViewListBlog()
        {
            var result = await _blogService.GetBlogs();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet]
        public async Task<IActionResult> ViewDetailBlog(int blogId)
        {
            var result = await _blogService.GetDetailBlog(blogId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewBlog(CreateBlogDTO createBlogDTO)
        {
            var result = await _blogService.CreateNewBlog(createBlogDTO);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveBlog(int blogId)
        {
            var result = await _blogService.RemoveBlog(blogId);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateInforBlog(UpdateBlogDTO updateBlogDTO)
        {
            var result = await _blogService.UpdateBlog(updateBlogDTO);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
