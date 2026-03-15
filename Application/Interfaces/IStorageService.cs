namespace MenuSoda.Application.Interfaces;

public interface IStorageService
{
    Task<string> UploadAsync(Stream content, string objectName, string contentType, CancellationToken ct);
    string GetSignedUrl(string objectName);
}
