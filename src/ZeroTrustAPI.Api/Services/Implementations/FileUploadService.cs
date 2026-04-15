using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ZeroTrustAPI.Api.Data;
using ZeroTrustAPI.Api.Entities;
using ZeroTrustAPI.Api.Services.Interfaces;

namespace ZeroTrustAPI.Api.Services.Implementations;

public class FileUploadService : IFileUploadService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    public FileUploadService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<UploadedFile> UploadAsync(IFormFile file, Guid userId)
    {
        // Add null check to throw expected ArgumentNullException
        if (file == null)
            throw new ArgumentNullException(nameof(file), "File cannot be null.");

        var uploadsFolder = _configuration["FileStorage:Path"] ?? Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var uploadedFile = new UploadedFile
        {
            Id = Guid.NewGuid(),
            FileName = file.FileName,
            StoragePath = filePath,
            Size = file.Length,
            UploadedByUserId = userId,
            UploadedAt = DateTime.UtcNow
        };
        _context.UploadedFiles.Add(uploadedFile);
        await _context.SaveChangesAsync();
        return uploadedFile;
    }
}