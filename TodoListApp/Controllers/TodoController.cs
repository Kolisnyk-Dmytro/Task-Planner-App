using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TodoListApp.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {
        private readonly JsonFileService _dataService;

        public TodoController(JsonFileService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult Index()
        {
            var todos = _dataService.GetTodos();
            return View(todos);
        }

        [HttpPost]
        public IActionResult Create(TodoItem todo)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", _dataService.GetTodos());
            }

            var todos = _dataService.GetTodos();
            todo.Id = todos.Any() ? todos.Max(t => t.Id) + 1 : 1;
            todo.UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            todo.CreatedDate = DateTime.Now;

            todos.Add(todo);
            _dataService.SaveTodos(todos);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ToggleComplete(int id)
        {
            var todos = _dataService.GetTodos();
            var todo = todos.FirstOrDefault(t => t.Id == id);

            if (todo != null)
            {
                todo.Completed = !todo.Completed;
                _dataService.SaveTodos(todos);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var todos = _dataService.GetTodos();
            todos.RemoveAll(t => t.Id == id);
            _dataService.SaveTodos(todos);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ClearCompleted()
        {
            var todos = _dataService.GetTodos();
            todos.RemoveAll(t => t.Completed);
            _dataService.SaveTodos(todos);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var todos = _dataService.GetTodos();
            var todo = todos.FirstOrDefault(t => t.Id == id);

            if (todo == null || todo.UserId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value))
            {
                return NotFound();
            }

            return View(todo);
        }

        [HttpPost]
        public IActionResult Edit(TodoItem updatedTodo)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedTodo);
            }

            var todos = _dataService.GetTodos();
            var existingTodo = todos.FirstOrDefault(t => t.Id == updatedTodo.Id);

            if (existingTodo == null || existingTodo.UserId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value))
            {
                return NotFound();
            }

            existingTodo.Title = updatedTodo.Title;
            existingTodo.Description = updatedTodo.Description;
            existingTodo.Completed = updatedTodo.Completed;

            _dataService.SaveTodos(todos);

            return RedirectToAction("Index");
        }
    }
}