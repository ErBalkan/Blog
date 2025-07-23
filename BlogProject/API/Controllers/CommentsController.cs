using BlogProject.Business.Abstract;
using BlogProject.Entities;
using Core.Utilities.Results.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogProject.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    // GET api/comments
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        IDataResult<List<Comment>> result = await _commentService.GetAllAsync();
        if (result.Success)
        {
            return Ok(result.Data);
        }
        return BadRequest(result.Message);
    }

    // GET api/comments/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        IDataResult<Comment> result = await _commentService.GetByIdAsync(id);
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

    // GET api/comments/post/{postId}
    // Belirli bir makaleye ait yorumları getirir.
    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetCommentsByPostId(int postId)
    {
        IDataResult<List<Comment>> result = await _commentService.GetCommentsByPostIdAsync(postId);
        if (result.Success)
        {
            return Ok(result.Data);
        }
        return BadRequest(result.Message);
    }

    // POST api/comments
    [HttpPost]
    public async Task<IActionResult> Add(Comment comment)
    {
        IMyResult result = await _commentService.AddAsync(comment);
        if (result.Success)
        {
            return StatusCode(201, result.Message);
        }
        return BadRequest(result.Message);
    }

    // PUT api/comments
    [HttpPut]
    public async Task<IActionResult> Update(Comment comment)
    {
        IMyResult result = await _commentService.UpdateAsync(comment);
        if (result.Success)
        {
            return Ok(result.Message);
        }
        return BadRequest(result.Message);
    }

    // DELETE api/comments/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        IMyResult result = await _commentService.DeleteAsync(id);
        if (result.Success)
        {
            return Ok(result.Message);
        }
        return BadRequest(result.Message);
    }
}