using System;
using System.ComponentModel.DataAnnotations;

public class TodoItem
{
    public TodoItem(string name, int id)
    {
        Name = name;
        Id = id;
    }

    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
}