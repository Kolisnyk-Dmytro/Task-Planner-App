public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Completed { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public int UserId { get; set; }
}