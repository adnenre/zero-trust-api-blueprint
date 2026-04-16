using System;

namespace ZeroTrustAPI.Api.Entities;

public class UploadedFile
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public long Size { get; set; }
    public Guid UploadedByUserId { get; set; }
    public DateTime UploadedAt { get; set; }
    public User UploadedByUser { get; set; } = null!;
}