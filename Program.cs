using System;
using System.IO;
using System.Text;

class Program
{
    static List<string> id_list = new List<string>();

    static void Main(string[] args)
    {
        string startPath = @"C:\Users\"; // Replace with the path to your directory
        string basePath = @"C:\Users\"; // Replace with base path for relative links
        string sitemap = CreateSitemap(startPath, basePath);
        Console.WriteLine(sitemap);

        // Создание JSON-структуры
        StringBuilder json = new StringBuilder();
        json.AppendLine("{");
        for (int i = 0; i < id_list.Count; i++)
        {
            json.AppendLine($"  \"{id_list[i]}\": \"null\",");
        }
        json.AppendLine("}");

        Console.WriteLine(json.ToString());
    }

    static string CreateSitemap(string startPath, string basePath)
    {
        StringBuilder sitemap = new StringBuilder();
        sitemap.AppendLine("<div class=\"sitemap\">");

        sitemap.Append(CreateSection(startPath, basePath, 0));

        sitemap.AppendLine("</div>");
        return sitemap.ToString();
    }

    static string CreateSection(string directoryPath, string basePath, int depth)
    {
        StringBuilder section = new StringBuilder();
        var indent = new String(' ', depth * 2);
        var titleClass = "section-title" + depth;

        section.AppendLine($"{indent}<div class=\"{titleClass}\">");

        var directoryId = Path.GetFileNameWithoutExtension(directoryPath);
        id_list.Add(directoryId);

        section.AppendLine($"{indent}  <h2 id=\"{directoryId}\" class=\"section-title\" style=\"margin-left: {depth * 4}%;\">{Path.GetFileName(directoryPath)}</h2>");

        var files = Directory.GetFiles(directoryPath, "*.html");
        for (int i = 0; i < files.Length; i++)
        {
            var file = files[i];
            var relativePath = Path.GetRelativePath(basePath, file).Replace("\\", "/");
            var prefix = i == 0 ? "┌" : i == files.Length - 1 ? "└" : "├";
            //var prefix = files.Length == 1 ? "▧" : i == 0 ? "┌" : i == files.Length - 1 ? "└" : "├";

            var fileId = Path.GetFileNameWithoutExtension(file);
            id_list.Add(fileId);

            section.AppendLine($"{indent}  <ul class=\"subsection-list\" style=\"margin-left: {depth * 4}%;\">");
            section.AppendLine($"{indent}    <li class=\"subsection-list-item\">{prefix} <a id=\"{fileId}\" href=\"./{relativePath}\">{Path.GetFileNameWithoutExtension(file)}</a></li>");
            section.AppendLine($"{indent}  </ul>");
        }

        foreach (var subdirectory in Directory.GetDirectories(directoryPath))
        {
            section.Append(CreateSection(subdirectory, basePath, depth + 1));
        }

        section.AppendLine($"{indent}</div>");
        return section.ToString();
    }
}