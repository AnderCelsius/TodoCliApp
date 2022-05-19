namespace TodoCliApp;

class Program
{
    static int idCount = 1;
    static string fileName = @"task.json";
    static List<TodoItem> todoList = new List<TodoItem>();

    static Dictionary<string, Command> commandLookup = new Dictionary<string, Command>()
    {
        {"add", new Command { Id = 1, Name = "add", Description = "Adds a new item to list"}},
        {"del", new Command { Id = 2, Name = "delete", Description = "Deletes an item from list"}},
        {"list", new Command { Id = 3, Name = "list", Description = "Prints all items in the list"}},
        {"info", new Command { Id = 4, Name = "info", Description = "Prints details about an item in the list"}},
        {"help", new Command { Id = 5, Name = "help", Description = "Shows all the available commands"}},
        {"complete", new Command { Id = 6, Name = "complete", Description = "Marks a task as Completed"}},
    };
    static void Main(string[] args)
    {

        //Create a string variable and get user input from the keyboard and store it in the variable
        string userInput = string.Join(" ", args);

        var userInputIsValid = ValidateUserInput(userInput);

        if (!userInputIsValid)
        {
            Console.WriteLine("You have entered an invalid command.");
            Console.WriteLine("Please enter a valid command or type help for more information");
        }
        else
        {
            ProcessCommand(userInput);
        }
    }

    string GetTodoItemById(int id)
    {
        TodoItem todoItem = null;
        if (id > 0)
        {
            foreach (var item in todoList)
            {
                if (item.Id == id)
                {
                    todoItem = item;
                    break;
                }
            }
        }

        if (todoItem == null) return $"Item with id ${id} not found";

        return $"Task: {todoItem.Name} \nDate Created: {todoItem.Created} \nStatus: {todoItem.IsCompleted} ? 'Completed' : 'Not Completed'";
    }

    static void MarkTaskCompleted(string userInput)
    {
        var tasks = JsonFileUtils.ReadFromFile(fileName);
        bool isValid = ValidateTaskCompletionAction(userInput);
        if (!isValid)
        {
            Console.WriteLine("You entered an invalid task completion action.\nType 'help' for more information.");
        }
        else
        {
            string input = userInput.Split("=")[1];
            bool isInteger = int.TryParse(input, out int index);

            if (!isInteger || index < 1)
            {
                Console.WriteLine("Please enter a valid task number.\nType 'help' for more information.");
            }
            else
            {
                if (tasks != null)
                {
                    var selectedTask = tasks[index-1];
                    selectedTask.IsCompleted = true;
                    JsonFileUtils.SaveToFile(tasks, fileName);
                }
                else
                {
                    Console.WriteLine("You tried to select a task outside the list of tasks");
                }
            }
        }
    }

    static bool ValidateTaskCompletionAction(string userInput)
    {
        if (string.IsNullOrEmpty(userInput)) return false;

        var inputArray = userInput.Trim().Split("=");
        var command = inputArray[0].ToLowerInvariant();
        if (command != Commands.COMPLETE || inputArray.Length != 2) return false;
        return true;
    }

    static string[] GetCommandAndTask(string userInput)
    {
        string[] inputArray = userInput.Split(" ");

        if(inputArray.Length == 1)
        {
            string[] newInputArray = userInput.Split("=");
            return new string[2] { newInputArray[0], ""};
        }
        var command = inputArray[0];
        var taskArray = inputArray.Skip(1).Take(inputArray.Length);
        var task = string.Join(" ", taskArray);
        return new string[2] { command, task };
    }

    static int ValidateIndex(List<TodoItem> items, string indexString)
    {
        int index = 0;
        int.TryParse(indexString, out index);
        if (index - 1 < 0 || index > items.Count) return -1;
        return index;
    }

    static void DeleteTask(List<TodoItem> tasks, string stringIndex)
    {
        int index = ValidateIndex(tasks, stringIndex);
        if (tasks != null)
        {
            tasks.RemoveAt(index);
        }
    }

    static void PrintTasks()
    {
        todoList = JsonFileUtils.ReadFromFile(fileName);
        if (todoList.Any())
        {
            for (int i = 0; i < todoList.Count; i++)
            {
                string template = "";
                if (todoList[i].IsCompleted)
                {
                    template += $"({i + 1}*)";
                }
                else
                {
                    template += $"({i + 1})";
                }
                Console.WriteLine(template + " " + todoList[i].Name);
            }
        }
        else
        {
            Console.WriteLine("Your todo list is empty. To add a new item, type 'add', followed by the task description. \nExample: add Read a book. \nFor more information, type 'help'");
        }
    }

    static void PrintHelp()
    {
        foreach (var command in commandLookup)
        {
            Console.WriteLine(command.Key + ": " + command.Value.Description);
        }
    }

    static void CreateTask(string[] data)
    {
        var tasks = JsonFileUtils.ReadFromFile(fileName);
        todoList.AddRange(tasks);
        var task = data[1];
        todoList.Add(new TodoItem(task, idCount));
        idCount += 1;
    }

    static bool ValidateUserInput(string userInput)
    {
        if (string.IsNullOrEmpty(userInput)) return false;

        string command = "";
        if (userInput.ToLowerInvariant().StartsWith("complete"))
        {
            var input = userInput.Split("=");
            command += input[0];
            if (commandLookup != null && commandLookup.Keys.Any() && !commandLookup.ContainsKey(command)) return false;
            return true;
        }

        var inputArray = userInput.Trim().Split(" ");
        command += inputArray[0].ToLowerInvariant();

        if (commandLookup != null && commandLookup.Keys.Any() && !commandLookup.ContainsKey(command)) return false;
        if (command.Length == 1) return false;

        return true;
    }

    static void ProcessCommand(string userInput)
    {
        var result = GetCommandAndTask(userInput);
        var command = result[0].ToLowerInvariant();
        var task = result[1];
        switch (command)
        {
            case Commands.ADD:
                CreateTask(result);
                JsonFileUtils.SaveToFile(todoList, fileName);
                break;
            case Commands.LIST:
                PrintTasks();
                break;
            case Commands.DELETE:
                DeleteTask(todoList, task);
                break;
            case Commands.HELP:
                PrintHelp();
                break;
            case Commands.COMPLETE:
                MarkTaskCompleted(userInput);
                break;
            default:
                Console.WriteLine("unknown command " + command);
                Console.WriteLine("Type 'help' for more information on how to use this application");
                break;

        }
    }
}









