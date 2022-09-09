namespace Data.Models;

public class FileStorageConfiguration
{
    public string Filename { get; set; } = null!;
    public int IoRetryCount { get; set; }
    public int IoRetryDelayMilliseconds { get; set; }
}