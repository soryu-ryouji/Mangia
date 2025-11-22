using System.IO;

namespace Mangia;

public class MangaInfo
{
    public string Title { get; set; }
    public string Url { get; set; }
    public string CoverUrl { get; set; }
    public List<ChapterInfo> ChapterInfos { get; set; }

    public MangaInfo(string title, string url)
    {
        Title = title;
        Url = url;

        var urlDir = new DirectoryInfo(url);
        CoverUrl = urlDir.GetFiles()
            .Where(file => file.Name.StartsWith("cover", StringComparison.OrdinalIgnoreCase))
            .Where(file => file.Name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                           file.Name.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault().FullName;

        ChapterInfos = urlDir.GetDirectories()
            .Where(dir => dir.Name.StartsWith('.') is false)
            .Where(dir => dir.Attributes.HasFlag(FileAttributes.Hidden) is false)
            .Select(dir => new ChapterInfo
            {
                Title = dir.Name,
                Url = dir.FullName
            }).ToList();
    }
}

public class ChapterInfo
{
    public string Title { get; set; }
    public string Url { get; set; }
}
