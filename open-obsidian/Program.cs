using System.Diagnostics;
using OpenSesame;

static class OpenInObsidian {
    static void Main(string[] args) {
        if (args.Length == 0) {
            Logger.Error("No arguments provided.");
            return;
        }
        var path = Paths.SplatPath(args);
        var didOpen = ObsidianHelper.OpenMarkdownOrFolderInObsidian(path);
        if (!didOpen) {
            Typora.Open(path);
        }
    }
}