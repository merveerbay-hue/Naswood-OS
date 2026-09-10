using System.Reflection;

namespace Naswood.Modules.Business.Application.Production;

public static class ShopFloorReleaseStamp
{
    public static (string Version, string GitSha) Current()
    {
        var version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? Assembly.GetExecutingAssembly().GetName().Version?.ToString()
            ?? "dev";
        var sha = Environment.GetEnvironmentVariable("NASWOOD_GIT_SHA")
            ?? Environment.GetEnvironmentVariable("GITHUB_SHA")
            ?? ReadLocalGitSha()
            ?? "";
        if (sha.Length > 40) sha = sha[..40];
        var plus = version.IndexOf('+');
        if (string.IsNullOrWhiteSpace(sha) && plus > 0 && plus < version.Length - 1)
            sha = version[(plus + 1)..];
        return (version, sha.Trim());
    }

    private static string? ReadLocalGitSha()
    {
        try
        {
            var head = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", ".git", "HEAD");
            var roots = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), ".git", "HEAD"),
                Path.GetFullPath(head)
            };
            foreach (var path in roots)
            {
                if (!File.Exists(path)) continue;
                var text = File.ReadAllText(path).Trim();
                if (text.StartsWith("ref:", StringComparison.OrdinalIgnoreCase))
                {
                    var gitDir = Path.GetDirectoryName(path)!;
                    var refPath = Path.Combine(gitDir, text[4..].Trim().Replace('/', Path.DirectorySeparatorChar));
                    if (File.Exists(refPath))
                        return File.ReadAllText(refPath).Trim();
                }
                return text;
            }
        }
        catch
        {
            /* stamp is diagnostic only */
        }
        return null;
    }
}
