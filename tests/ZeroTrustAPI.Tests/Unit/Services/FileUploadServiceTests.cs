using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text;
using Xunit;
using ZeroTrustAPI.Api.Data;
using ZeroTrustAPI.Api.Services.Implementations;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ZeroTrustAPI.Tests.Unit.Services;

public class FileUploadServiceTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private IConfiguration GetConfiguration(string uploadPath = "TestUploads")
    {
        // Fixed: Use Dictionary<string, string?> to match AddInMemoryCollection
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FileStorage:Path"] = uploadPath
            })
            .Build();
        return config;
    }

    private IFormFile CreateMockFile(string fileName = "test.txt", string content = "hello world")
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var file = new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/plain"
        };
        return file;
    }

    [Fact]
    public async Task UploadAsync_SavesFileAndRecord()
    {
        var testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);
        try
        {
            await using var context = GetDbContext();
            var config = GetConfiguration(testDir);
            var service = new FileUploadService(context, config);
            var userId = Guid.NewGuid();
            var file = CreateMockFile();

            var result = await service.UploadAsync(file, userId);

            Assert.NotNull(result);
            Assert.Equal(file.FileName, result.FileName);
            Assert.Equal(file.Length, result.Size);
            Assert.Equal(userId, result.UploadedByUserId);
            Assert.Equal(DateTime.UtcNow.Date, result.UploadedAt.Date);
            Assert.True(File.Exists(result.StoragePath));

            var saved = await context.UploadedFiles.FindAsync(result.Id);
            Assert.NotNull(saved);
        }
        finally
        {
            if (Directory.Exists(testDir))
                Directory.Delete(testDir, true);
        }
    }

    [Fact]
    public async Task UploadAsync_WithoutFile_ThrowsArgumentNullException()
    {
        await using var context = GetDbContext();
        var config = GetConfiguration();
        var service = new FileUploadService(context, config);
        var userId = Guid.NewGuid();

        await Assert.ThrowsAsync<ArgumentNullException>(() => service.UploadAsync(null!, userId));
    }
}