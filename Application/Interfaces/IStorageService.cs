namespace MenuSoda.Application.Interfaces;

public interface IStorageService
{
    Task<string> UploadAsync(Stream content, string objectName, string contentType, CancellationToken ct);
    Task DeleteAsync(string objectName, CancellationToken ct);
    string GetSignedUrl(string objectName);
}
