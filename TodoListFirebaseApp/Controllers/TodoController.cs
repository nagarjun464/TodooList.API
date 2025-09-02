using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TodoListFirebaseApp.Services;
using TodoListFirebaseApp.Models;

[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly FirebaseService _firebaseService;
    

    public TodoController(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _firebaseService.GetTodosAsync());

    [HttpGet("{Title}")]
    public async Task<IActionResult> GetById(string Title)
    {
        var todo = await _firebaseService.GetTodoByIdAsync(Title);
        if (todo == null)
            return NotFound();
        return Ok(todo);
    }


    [HttpPost]
    public async Task<IActionResult> Post(TodoItem todo)
    {
        todo.CreatedAt = todo.CreatedAt.ToUniversalTime(); // safer
        await _firebaseService.AddTodoAsync(todo);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] TodoItem todo)
    {
        if (id != todo.Id)
            return BadRequest("ID mismatch");
        todo.CreatedAt = todo.CreatedAt.ToUniversalTime(); // safer
        var updated = await _firebaseService.UpdateTodoAsync(todo);
        return updated ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _firebaseService.DeleteTodoAsync(id);
        return success ? NoContent() : NotFound();
    }

}
