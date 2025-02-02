using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Xml;
using Newtonsoft.Json;
using TodoListApp.Models;

namespace TodoListApp.Services
{
    public class JsonFileService
    {
        private readonly string _todoFilePath;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JsonFileService(IHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _todoFilePath = Path.Combine(env.ContentRootPath, "App_Data", "todos.json");
            _httpContextAccessor = httpContextAccessor;
            EnsureFileExists(_todoFilePath);
        }

        private int CurrentUserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return int.Parse(userIdClaim);
            }
        }

        private void EnsureFileExists(string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            if (!File.Exists(path))
            {
                File.WriteAllText(path, "[]");
            }
        }

        public List<TodoItem> GetTodos()
        {
            var json = File.ReadAllText(_todoFilePath);
            var allTodos = JsonConvert.DeserializeObject<List<TodoItem>>(json) ?? new List<TodoItem>();
            return allTodos.Where(t => t.UserId == CurrentUserId).ToList();
        }

        public void SaveTodos(List<TodoItem> todos)
        {
            var json = File.ReadAllText(_todoFilePath);
            var allTodos = JsonConvert.DeserializeObject<List<TodoItem>>(json) ?? new List<TodoItem>();

            allTodos.RemoveAll(t => t.UserId == CurrentUserId);

            allTodos.AddRange(todos);

            File.WriteAllText(_todoFilePath, JsonConvert.SerializeObject(allTodos, Newtonsoft.Json.Formatting.Indented));
        }

        public void DeleteUserTasks(int userId)
        {
            var json = File.ReadAllText(_todoFilePath);
            var allTodos = JsonConvert.DeserializeObject<List<TodoItem>>(json) ?? new List<TodoItem>();

            allTodos.RemoveAll(t => t.UserId == userId);

            File.WriteAllText(_todoFilePath, JsonConvert.SerializeObject(allTodos, Newtonsoft.Json.Formatting.Indented));
        }
    }
}