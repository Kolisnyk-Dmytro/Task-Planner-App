# Task Planner Web App 📝

A secure and intuitive web application for managing personal tasks, built with ASP.NET Core. Features user authentication, task CRUD operations, and responsive design.

## Features ✨
- **User Authentication**: Register, login, and manage your profile (change password or delete account).
- **Task Management**: Create, edit, complete, and delete tasks. Organize tasks into active/completed lists.
- **Data Security**: Passwords hashed with BCrypt. User-specific task isolation.
- **Responsive UI**: Auto-resizing textareas, Bootstrap styling, and clean alerts for errors/successes.

## Installation 🛠️
1. **Prerequisites**:  
   - [.NET SDK](https://dotnet.microsoft.com/download) (v8.0)
2. **Clone & Run**:  
   ```bash
   git clone https://github.com/your-repo/task-planner-app.git
   cd task-planner-app
   dotnet restore   # Restore packages
   dotnet run       # Start the app
3. **Access**:
   - If the application does not open automatically, then go to the URL that the console issued.

## Usage 🚀
1. **Register/Login**:
   - New users: Navigate to ```Register```.
   - Existing users: Log in at ```Login```.
2. **Manage Tasks**:
   - **Add**: Use the form on the home page.
   - **Edit/Delete**: Click buttons next to each task.
   - **Toggle Completion**: Mark tasks as done or undo them.
   - **Clear Completed**: Remove all finished tasks at once.
3. **Profile Settings**:
   - Change password or delete account at ```Profile```.

## Tech Stack 💻
- **Backend**: ASP.NET Core, Cookie Authentication
- **Frontend**: Bootstrap, Razor Views, jQuery (for auto-resize)
- **Data Storage**: JSON files (```users.json```, ```todos.json```)
- **Security**: BCrypt.Net, Anti-Forgery Tokens
