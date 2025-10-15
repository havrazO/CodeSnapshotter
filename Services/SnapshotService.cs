using CodeSnapshotter.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CodeSnapshotter.Services
{
    public class SnapshotService
    {
        public async Task<string> CreateSnapshotAsync(StorageFolder rootFolder, SnapshotConfig config)
        {
            StringBuilder outputContent = new StringBuilder();
            await ProcessDirectoryAsync(rootFolder, rootFolder.Path, outputContent, config);
            return outputContent.ToString();
        }

        private async Task ProcessDirectoryAsync(StorageFolder currentFolder, string rootPath, StringBuilder output, SnapshotConfig config)
        {
            if (config.ExcludeDirectories.Contains(currentFolder.Name, StringComparer.OrdinalIgnoreCase))
            {
                return;
            }

            foreach (StorageFile? file in await currentFolder.GetFilesAsync())
            {
                string extension = Path.GetExtension(file.Name);
                if (config.IncludeExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase) &&
                  !config.ExcludeFiles.Contains(file.Name, StringComparer.OrdinalIgnoreCase))
                {
                    config.StatusUpdateCallback?.Invoke($"Verarbeite: {file.Path}");

                    string relativePath = Path.GetRelativePath(rootPath, file.Path);
                    string fileContent = await FileIO.ReadTextAsync(file);

                    AppendFileContent(output, relativePath, fileContent, config.IsMarkdown);
                }
            }

            foreach (StorageFolder? subFolder in await currentFolder.GetFoldersAsync())
            {
                await ProcessDirectoryAsync(subFolder, rootPath, output, config);
            }
        }

        private void AppendFileContent(StringBuilder output, string relativePath, string content, bool isMarkdown)
        {
            if (isMarkdown)
            {
                string language = GetMarkdownLanguage(Path.GetExtension(relativePath));
                output.AppendLine($"## `FILE: {relativePath.Replace("\\", "/")}`");
                output.AppendLine($"```{language}");
                output.AppendLine(content);
                output.AppendLine("```");
                output.AppendLine();
            }
            else
            {
                output.AppendLine($"# === FILE: {relativePath} ===");
                output.AppendLine(content);
                output.AppendLine("# === END OF FILE ===");
                output.AppendLine();
            }
        }

        private string GetMarkdownLanguage(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".ts" => "typescript",
                ".js" => "javascript",
                ".css" => "css",
                ".scss" => "scss",
                ".html" => "html",
                ".xml" => "xml",
                ".json" => "json",
                ".md" => "markdown",
                ".gitignore" => "plaintext",
                ".env" => "plaintext",
                _ => "plaintext",
            };
        }
    }
}