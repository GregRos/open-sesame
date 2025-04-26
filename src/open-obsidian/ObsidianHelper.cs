

using System.Text.RegularExpressions;

namespace OpenSesame {
    public static class ObsidianHelper {
        public static string GetOpenUrl(string vault, string? file) {
            var justName = Path.GetFileName(vault);
            if (file == null) {
                return GetOpenUrl(justName);
            }
            var justFileName = Path.GetFileName(file);
            var escaped = Uri.EscapeDataString(justFileName);
            return $"obsidian://adv-uri?vault={Uri.EscapeDataString(justName)}&filepath={escaped}&openmode=true";
        }

        public static string GetOpenUrl(string vault) {
            return $"obsidian://adv-uri?vault={Uri.EscapeDataString(vault)}";
        }

        public static string? GetVaultFromPath(string path) {
            static string? TryGetFolderAsVault(string folder) {
                var gitFolder = Paths.GetSpecialFolderFrom(folder, ".git");
                var vaultFolder = Paths.GetSpecialFolderFrom(folder, ".obsidian");
                if (gitFolder != null && vaultFolder != null) {
                    if (gitFolder == vaultFolder || vaultFolder.StartsWith(gitFolder + Path.DirectorySeparatorChar)) {
                        return vaultFolder;
                    }
                }
                return null;
            }
            var containing = Paths.GetContainingFolder(path);
            var options = new[] { path, $"${path}.vault" }.Select(TryGetFolderAsVault).Where(x => x != null).ToList();
            return options.FirstOrDefault();
        }


        public static bool OpenMarkdownOrFolderInObsidian(string path) {
            Logger.Info($"Input path: {path}");
            var argAsSinglePath = Paths.GetRootedPath(path);
            Logger.Info($"Rooted path: {argAsSinglePath}");
            var pathThatExists = new[] { argAsSinglePath, $"{argAsSinglePath}.md" }.FirstOrDefault(Paths.Exists) ?? throw new IOException($"Path {path} does not exist.");
            var containingFolder = Paths.GetContainingFolder(pathThatExists);
            string? file = null;
            if (containingFolder != pathThatExists) {
                file = Path.GetRelativePath(containingFolder, pathThatExists);
            }
            string? vaultPath = ObsidianHelper.GetVaultFromPath(pathThatExists);
            if (vaultPath == null) {
                return false;
            }
            var url = ObsidianHelper.GetOpenUrl(vaultPath, file);
            Logger.Info($"Opening {url}");
            Programs.Open(url);
            return true;
        }
    }
}
