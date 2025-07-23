using BlogProject.Business.Abstract;
using BlogProject.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Utilities.Results.Abstract;

namespace BlogProject.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }

    // GET api/posts
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        IDataResult<List<Post>> result = await _postService.GetAllAsync();
        if (result.Success)
        {
            return Ok(result.Data);
        }
        return BadRequest(result.Message);
    }

    // GET api/posts/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        IDataResult<Post> result = await _postService.GetByIdAsync(id);
        if (result.Success)
        {
            if (result.Data == null)
            {
                return NotFound(result.Message);
            }
            return Ok(result.Data);
        }
        return BadRequest(result.Message);
    }

    // GET api/posts/category/{categoryId}
    // Belirli bir kategoriye ait makaleleri getirir.
    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetPostsByCategoryId(int categoryId)
    {
        IDataResult<List<Post>> result = await _postService.GetPostsByCategoryIdAsync(categoryId);
        if (result.Success)
        {
            return Ok(result.Data);
        }
        return BadRequest(result.Message);
    }

    // GET api/posts/user/{userId}
    // Belirli bir kullanıcıya (yazar) ait makaleleri getirir.
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetPostsByUserId(int userId)
    {
        IDataResult<List<Post>> result = await _postService.GetPostsByUserIdAsync(userId);
        if (result.Success)
        {
            return Ok(result.Data);
        }
        return BadRequest(result.Message);
    }

    // POST api/posts
    [HttpPost]
    public async Task<IActionResult> Add(Post post)
    {
        IMyResult result = await _postService.AddAsync(post);
        if (result.Success)
        {
            return StatusCode(201, result.Message);
        }
        return BadRequest(result.Message);
    }

    // PUT api/posts
    [HttpPut]
    public async Task<IActionResult> Update(Post post)
    {
        IMyResult result = await _postService.UpdateAsync(post);
        if (result.Success)
        {
            return Ok(result.Message);
        }
        return BadRequest(result.Message);
    }

    // DELETE api/posts/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        IMyResult result = await _postService.DeleteAsync(id);
        if (result.Success)
        {
            return Ok(result.Message);
        }
        return BadRequest(result.Message);
    }
}