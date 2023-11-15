using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class Program
{
    static List<string> id_list = new List<string>();
    static string startPath = string.Empty;
    static string basePath = string.Empty;
    static string sitemap = string.Empty;

    static void Main(string[] args)
    {
        bool continueProgram = true;

        while (continueProgram)
        {
            try
            {
                Console.WriteLine("Enter the folder path:");
                startPath = Console.ReadLine();

                if (!Directory.Exists(startPath))
                {
                    Console.WriteLine("The specified folder does not exist.");
                    continue;
                }

                Console.WriteLine("Enter the HTML file name (without extension):");
                string outputFileName = Console.ReadLine();

                if (string.IsNullOrEmpty(outputFileName))
                {
                    Console.WriteLine("File name cannot be empty.");
                    continue;
                }

                string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{outputFileName}.html");

                basePath = startPath;
                sitemap = CreateSitemap(startPath, basePath);
                SaveHtmlToFile(sitemap, outputPath);

                Console.WriteLine("Done! HTML file saved at the specified path.");

                Console.WriteLine("Do you want to continue using the program? (y/n)");
                string input = Console.ReadLine();

                if (input.ToLower() != "y")
                {
                    continueProgram = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }

    static void JS()
    {
        Console.WriteLine(sitemap);

        StringBuilder json = new StringBuilder();
        json.AppendLine("{");
        for (int i = 0; i < id_list.Count; i++)
        {
            json.AppendLine($"  \"{id_list[i]}\": \"null\",");
        }
        json.AppendLine("}");

        Console.WriteLine(json.ToString());
    }

    static string CreateSitemap(string directoryPath, string basePath)
    {
        StringBuilder sitemap = new StringBuilder();
        sitemap.AppendLine("<div class=\"sitemap\">");

        sitemap.Append(CreateSection(directoryPath, basePath, 0));

        sitemap.AppendLine("</div>");
        return sitemap.ToString();
    }

    static string CreateSection(string directoryPath, string basePath, int depth)
    {
        StringBuilder section = new StringBuilder();
        var indent = new string(' ', depth * 2);
        var titleClass = "section-title" + depth;

        section.AppendLine($"{indent}<div class=\"{titleClass}\">");

        var directoryId = Path.GetFileNameWithoutExtension(directoryPath);
        id_list.Add(directoryId);

        section.AppendLine($"{indent}  <h2 id=\"{directoryId}\" class=\"section-title\" style=\"margin-left: {depth * 4}%;\">{Path.GetFileName(directoryPath)}</h2>");

        var files = Directory.GetFiles(directoryPath, "*.html");
        var subdirectories = Directory.GetDirectories(directoryPath);

        if (files.Length > 0)
        {
            section.AppendLine($"{indent}  <div class=\"subsection\">");
            section.AppendLine($"{indent}    <h3 class=\"subsection-title\" style=\"margin-left: {depth * 4}%;\">Files:</h3>");
            section.AppendLine($"{indent}    <ul class=\"subsection-list\" style=\"margin-left: {depth * 4}%;\">");

            foreach (var file in files)
            {
                var relativePath = Path.GetRelativePath(basePath, file).Replace("\\", "/");
                var fileId = Path.GetFileNameWithoutExtension(file);
                id_list.Add(fileId);

                var absolutePath = new Uri(file).AbsoluteUri;
                section.AppendLine($"{indent}      <li class=\"subsection-list-item\"><a id=\"{fileId}\" href=\"{absolutePath}\">{Path.GetFileNameWithoutExtension(file)}</a></li>");
            }

            section.AppendLine($"{indent}    </ul>");
            section.AppendLine($"{indent}  </div>");
        }

        foreach (var subdirectory in subdirectories)
        {
            var subDirFiles = Directory.GetFiles(subdirectory, "*.html");
            if (subDirFiles.Length > 0)
            {
                section.Append(CreateSection(subdirectory, basePath, depth + 1));
            }
        }

        section.AppendLine($"{indent}</div>");
        return section.ToString();
    }

    static void SaveHtmlToFile(string html, string outputPath)
    {
        try
        {
            string fullOutputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, outputPath);

            html = $@"<!DOCTYPE html>
                <html>
                <head>
                    <title>Your Title</title>
                    <style>
                        * {{
                            list-style-type: none;
                        }}

                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #1d1d1d;
                            margin: 0;
                            padding: 0;
                            color: #a7a7a7;
                            object-fit: fill;
                            align-items: center;
                            font-style: normal;
                            padding: 10px;
                        }}
                        
                        .h2{{
                            margin-bottom: 0px;
                        }}

                        .sitemap {{
                            background-color: #212121;
                            border-radius: 10px;
                            box-shadow: -5.9px 5.9px 14.75px hsl(0, 00%, 9.1%), 5.9px -4.9px 14.75px hsl(0, 00%, 16.900000000000002%), inset -0.43px -0.43px 1.72px hsl(0, 0%, 9.1%), inset 0.43px 0.43px 1.72px hsl(0, 0%, 16.900000000000002%);
                            margin: 20px auto;
                            max-width: 80%;
                            padding: 20px;
                        }}

                        .section {{
                            margin-top: 20px;
                        }}
                        
                        .section-title {{
                            color: #da4444;
                            font-size: 1.8em;
                            font-family: monospace;
                        }}
                        
                        .subsection {{
                            margin-left: 20px;
                            margin-bottom: 75px;
                        }}

                        .subsection-title {{
                            font-size: 18px;
                            margin-left: 20px;
                            margin-bottom: 5px;
                        }}

                        .subsection-list {{
                            list-style-type: none;
                            padding-left: 0;
                        }}

                        .subsection-list-item {{
                            margin-bottom: 5px;
                        }}

                        a {{
                            color: #ffef84;
                            text-decoration: none;
                        }}
                    </style>
                </head>
                <body>
                    {html}
                </body>
                </html>";

            File.WriteAllText(fullOutputPath, html, Encoding.UTF8);
            Console.WriteLine("HTML file successfully saved at the path: " + fullOutputPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while saving the HTML file: " + ex.Message);
        }
    }
}
