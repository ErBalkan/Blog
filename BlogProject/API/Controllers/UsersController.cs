using BlogProject.Business.Abstract;
using Core.Utilities.Results.Abstract;
using BlogProject.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BlogProject.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET api/users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        IDataResult<List<User>> result = await _userService.GetAllAsync();
        if (result.Success)
        {
            return Ok(result.Data);
        }
        return BadRequest(result.Message);
    }

    // GET api/users/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        IDataResult<User> result = await _userService.GetByIdAsync(id);
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

    // GET api/users/username/{username}
    [HttpGet("username/{username}")]
    public async Task<IActionResult> GetByUsername(string username)
    {
        IDataResult<User> result = await _userService.GetByUsernameAsync(username);
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

    // GET api/users/email/{email}
    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        IDataResult<User> result = await _userService.GetByEmailAsync(email);
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

    // POST api/users/register (Kayıt işlemi)
    // Kullanıcı oluşturma endpoint'i.
    [HttpPost("register")]
    public async Task<IActionResult> Register(User user) // Buradaki User nesnesi, kayıt için gerekli bilgileri içerecek.
    {
        IMyResult result = await _userService.AddAsync(user);
        if (result.Success)
        {
            return StatusCode(201, result.Message); // HTTP 201 Created
        }
        return BadRequest(result.Message);
    }

    // POST api/users/login (Giriş işlemi)
    // Kullanıcı kimlik doğrulama (giriş) endpoint'i.
    // Genellikle giriş için ayrı bir DTO (Data Transfer Object) kullanılır, ancak şimdilik string'leri doğrudan alabiliriz.
    // Daha iyi bir yaklaşım için bir LoginDto oluşturulabilir.
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User loginUser) // Kullanıcı adı ve parola için bir User nesnesi beklenecek.
    {
        // UserManager'daki AuthenticateUserAsync metodu sadece username ve password alır.
        IDataResult<bool> authResult = await _userService.AuthenticateUserAsync(loginUser.Username, loginUser.PasswordHash);

        if (authResult.Success && authResult.Data == true) // Kimlik doğrulama hem genel olarak başarılı hem de parola eşleşti.
        {
            return Ok(authResult.Message); // "Giriş başarılı."
        }
        else // Kimlik doğrulama başarısız oldu (kullanıcı bulunamadı veya parola eşleşmedi).
        {
            return BadRequest(authResult.Message); // "Kullanıcı bulunamadı." veya "Kullanıcı adı veya parola hatalı."
        }
    }

    // PUT api/users
    [HttpPut]
    public async Task<IActionResult> Update(User user)
    {
        IMyResult result = await _userService.UpdateAsync(user);
        if (result.Success)
        {
            return Ok(result.Message);
        }
        return BadRequest(result.Message);
    }

    // DELETE api/users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        IMyResult result = await _userService.DeleteAsync(id);
        if (result.Success)
        {
            return Ok(result.Message);
        }
        return BadRequest(result.Message);
    }
}