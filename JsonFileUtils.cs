
using System.IO;
using Newtonsoft.Json;

public static class JsonFileUtils
{
    private static readonly JsonSerializerSettings _options
        = new() { NullValueHandling = NullValueHandling.Ignore };

    // Newtonsoft/JsonFileUtils.cs
    public static void SaveToFile(object obj, string fileName)
    {
        using var streamWriter = File.CreateText(fileName);
        using var jsonWriter = new JsonTextWriter(streamWriter);
        JsonSerializer.CreateDefault(_options).Serialize(jsonWriter, obj);
    }

    public static List<TodoItem> ReadFromFile(string fileName)
    {
        using var streamReader = File.OpenText(fileName);
        using var jsonWriter = new JsonTextReader(streamReader);
        var result = JsonSerializer.CreateDefault(_options).Deserialize<List<TodoItem>>(jsonWriter);
        if (result != null)
        {
            return result;
        }

        return new List<TodoItem>();
    }
}