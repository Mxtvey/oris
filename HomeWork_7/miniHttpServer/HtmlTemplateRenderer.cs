using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using miniHttpServer.Intreface;

public class HtmlTemplateRenderer : IHtmlTemplateRenderer
{
    public string RenderFromString(string htmlTemplate, object dataModel)
    {
        string result = htmlTemplate;

        result = ProcessForeach(result, dataModel);
        result = ProcessIfElse(result, dataModel);
        result = ReplaceVariables(result, dataModel);

        return result;
    }

    public string RenderFromFile(string filePath, object dataModel)
    {
        string htmlTemplate = File.ReadAllText(filePath);
        return RenderFromString(htmlTemplate, dataModel);
    }

    public string RenderToFile(string inputFilePath, string outputFilePath, object dataModel)
    {
        string rendered = RenderFromFile(inputFilePath, dataModel);
        File.WriteAllText(outputFilePath, rendered);
        return rendered;
    }


    private string ProcessForeach(string template, object model)
    {
        var foreachPattern = new Regex(@"\$foreach\(var (\w+) in ([^)]+)\)([\s\S]*?)\$endfor");

        while (foreachPattern.IsMatch(template))
        {
            template = foreachPattern.Replace(template, match =>
            {
                string varName = match.Groups[1].Value;          
                string collectionPath = match.Groups[2].Value;   
                string innerTemplate = match.Groups[3].Value;   

                object collectionObj = GetPropertyValue(model, collectionPath);
                if (collectionObj is not IEnumerable collection)
                    return "";

                string result = "";
                foreach (var element in collection)
                {
                    string processed = innerTemplate;
                    processed = ProcessIfElse(processed, element);
                    processed = ReplaceVariables(processed, element, varName);
                    result += processed;
                }

                return result;
            });
        }

        return template;
    }

    private string ProcessIfElse(string template, object model)
    {
        var ifPattern = new Regex(@"\$if\(([^)]+)\)([\s\S]*?)(?:\$else([\s\S]*?))?\$endif");

        while (ifPattern.IsMatch(template))
        {
            template = ifPattern.Replace(template, match =>
            {
                string conditionPath = match.Groups[1].Value.Trim();
                string trueBlock = match.Groups[2].Value;
                string falseBlock = match.Groups[3].Value;

                object value = GetPropertyValue(model, conditionPath);
                bool condition = value is bool b && b;

                return condition ? trueBlock : falseBlock;
            });
        }

        return template;
    }

    private string ReplaceVariables(string template, object model, string prefix = null)
    {
        var varPattern = new Regex(@"\$\{([^}]+)\}");

        return varPattern.Replace(template, match =>
        {
            string path = match.Groups[1].Value.Trim();

            if (prefix != null && path.StartsWith(prefix + "."))
                path = path.Substring(prefix.Length + 1);

            object value = GetPropertyValue(model, path);
            return value?.ToString() ?? "";
        });
    }

 
    private object GetPropertyValue(object obj, string path)
    {
        if (obj == null || string.IsNullOrEmpty(path))
            return null;

        string[] parts = path.Split('.');
        object current = obj;

        foreach (var part in parts)
        {
            if (current == null)
                return null;

            Type type = current.GetType();
            PropertyInfo prop = type.GetProperty(part, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop == null)
                return null;

            current = prop.GetValue(current);
        }

        return current;
    }
}
