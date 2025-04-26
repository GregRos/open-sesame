using System.Text.RegularExpressions;
static partial class Paths {

    public static bool Exists(string path) {
        return Directory.Exists(path) || File.Exists(path);
    }

    public static string? GetSpecialFolderFrom(string startAt, string specialFolder) {
        var path = startAt;
        while (true) {

            if (path == null) {
                return null;
            }

            if (Directory.Exists(Path.Combine(path, specialFolder))) {
                return path;
            }

            path = Path.GetDirectoryName(path);
        }
    }

    public static string GetRootedPath(string path) {
        return Path.GetFullPath(path);
    }

    public static string GetContainingFolder(string path) {
        if (File.Exists(path)) {
            return Path.GetDirectoryName(path)!;
        } else {
            return path;
        }
    }

    public static bool IsMarkdownFile(string path) {
        return MarkdownRegex().IsMatch(path);
    }

    public static string SplatPath(IEnumerable<string> args) {
        return string.Join(" ", args).Replace("\"", "");
    }

    [GeneratedRegex(@"\.(md|markdown)$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex MarkdownRegex();


}