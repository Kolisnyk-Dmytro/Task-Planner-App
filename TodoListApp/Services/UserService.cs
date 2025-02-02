using Newtonsoft.Json;

public class UserService
{
    private readonly string _filePath;

    public UserService(IHostEnvironment env)
    {
        _filePath = Path.Combine(env.ContentRootPath, "App_Data", "users.json");
        EnsureFileExists(_filePath);
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

    public List<User> GetUsers()
    {
        var json = File.ReadAllText(_filePath);
        return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
    }

    public void SaveUsers(List<User> users)
    {
        var json = JsonConvert.SerializeObject(users, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(_filePath, json);
    }

    public User CreateUser(string username, string password)
    {
        var users = GetUsers();

        if (users.Any(u => u.Username == username))
        {
            throw new InvalidOperationException("Username already exists.");
        }

        var newUser = new User
        {
            Id = users.Any() ? users.Max(u => u.Id) + 1 : 1,
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            CreatedAt = DateTime.Now
        };

        users.Add(newUser);
        SaveUsers(users);

        return newUser;
    }
}