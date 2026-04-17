using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MenuSoda.Application.Utils;

public static class FuzzyTextUtil
{
    private static readonly HashSet<string> Stopwords = new(StringComparer.OrdinalIgnoreCase)
    {
        "el","la","los","las","de","del","en","al","a",
        "con","y","e","o","u","un","una","unos","unas","por","para"
    };

    public static string Normalize(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        var nfd = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var c in nfd)
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        return string.Join(" ", Regex.Replace(sb.ToString().ToLowerInvariant(), @"[^a-z0-9\s]", " ")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(w => !Stopwords.Contains(w)));
    }

    public static int Levenshtein(string a, string b)
    {
        int m = a.Length, n = b.Length;
        var dp = new int[m + 1, n + 1];
        for (int i = 0; i <= m; i++) dp[i, 0] = i;
        for (int j = 0; j <= n; j++) dp[0, j] = j;
        for (int i = 1; i <= m; i++)
            for (int j = 1; j <= n; j++)
                dp[i, j] = a[i - 1] == b[j - 1]
                    ? dp[i - 1, j - 1]
                    : 1 + Math.Min(dp[i - 1, j - 1], Math.Min(dp[i - 1, j], dp[i, j - 1]));
        return dp[m, n];
    }

    public static double Jaccard(string a, string b)
    {
        var sa = new HashSet<string>(a.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        var sb = new HashSet<string>(b.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        if (sa.Count == 0 && sb.Count == 0) return 1.0;
        int intersection = sa.Count(t => sb.Contains(t));
        int union = sa.Count + sb.Count - intersection;
        return union == 0 ? 0 : (double)intersection / union;
    }

    public static bool IsFuzzyMatch(string query, string candidate)
    {
        var q = Normalize(query);
        var c = Normalize(candidate);
        if (string.IsNullOrEmpty(q)) return true;
        if (c.Contains(q) || q.Contains(c)) return true;
        if (Jaccard(q, c) >= 0.35) return true;
        if (q.Length >= 4 && c.Length >= 4 && Levenshtein(q, c) <= 2) return true;
        return false;
    }

    public static bool IsDuplicate(string query, string candidate)
    {
        var q = Normalize(query);
        var c = Normalize(candidate);
        if (string.IsNullOrEmpty(q) || string.IsNullOrEmpty(c)) return false;
        if (q == c) return true;
        if (Jaccard(q, c) >= 0.60) return true;
        if (q.Length >= 4 && c.Length >= 4 && Levenshtein(q, c) <= 1) return true;
        return false;
    }
}
