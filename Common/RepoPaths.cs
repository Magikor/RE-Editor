using System;
using System.IO;

namespace RE_Editor.Common;

public static class RepoPaths {
    public static string RepoRoot { get; } = FindRepoRoot();

    public static string PathFromRepo(params string[] parts) {
        if (parts == null || parts.Length == 0) return RepoRoot;
        return Path.Combine([RepoRoot, ..parts]);
    }

    private static string FindRepoRoot() {
        var startDirs = new[] {
            Environment.CurrentDirectory,
            AppContext.BaseDirectory,
        };

        foreach (var startDir in startDirs) {
            if (string.IsNullOrWhiteSpace(startDir)) continue;

            try {
                var dirInfo = new DirectoryInfo(startDir);
                for (var i = 0; i < 12 && dirInfo != null; i++) {
                    if (IsRepoRoot(dirInfo.FullName)) return dirInfo.FullName;
                    dirInfo = dirInfo.Parent;
                }
            } catch {
                // ignore and try next
            }
        }

        throw new InvalidOperationException(
            "Unable to locate RE-Editor repo root. " +
            $"CurrentDirectory='{Environment.CurrentDirectory}', BaseDirectory='{AppContext.BaseDirectory}'.");
    }

    private static bool IsRepoRoot(string dir) {
        return File.Exists(Path.Combine(dir, "RE-Editor.sln")) ||
               File.Exists(Path.Combine(dir, "Directory.Build.props"));
    }
}
