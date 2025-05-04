using System.Diagnostics;

static class Vscode {
    static void Main(string[] args) {
        var jArgs = string.Join(" ", args);
        var names = new[] { "code-insiders", "code" };
        var success = names.Select(name => Programs.Open(name, jArgs)).Any(x => x);
        if (!success) {
            Logger.Warn("Failed to open VSCode");
            Environment.Exit(1);
        }

    }
}