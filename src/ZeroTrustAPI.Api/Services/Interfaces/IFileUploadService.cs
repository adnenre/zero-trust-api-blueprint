using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ZeroTrustAPI.Api.Entities;

namespace ZeroTrustAPI.Api.Services.Interfaces;

public interface IFileUploadService
{
    Task<UploadedFile> UploadAsync(IFormFile file, Guid userId);
}