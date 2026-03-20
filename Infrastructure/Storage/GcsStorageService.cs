using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using MenuSoda.Application.Interfaces;
using MenuSoda.Application.Options;
using Microsoft.Extensions.Options;

namespace MenuSoda.Infrastructure.Storage;

public class GcsStorageService : IStorageService
{
    private readonly StorageClient _storageClient;
    private readonly UrlSigner _urlSigner;
    private readonly string _bucketName;
    private readonly int _expirationMinutes;

    public GcsStorageService(IOptions<GcsOptions> options, IConfiguration configuration)
    {
        var gcsOptions = options.Value;
        _bucketName = gcsOptions.BucketName;
        _expirationMinutes = gcsOptions.SignedUrlExpirationMinutes;

        var credentialsJson = configuration["Gcs:CredentialsJson"]
                              ?? Environment.GetEnvironmentVariable("GCS_CREDENTIALS_JSON");

        ServiceAccountCredential serviceAccountCredential;
        if (!string.IsNullOrWhiteSpace(credentialsJson))
        {
            serviceAccountCredential = CredentialFactory.FromJson<ServiceAccountCredential>(credentialsJson);
        }
        else
        {
            var credential = GoogleCredential.GetApplicationDefault();
            serviceAccountCredential = credential.UnderlyingCredential as ServiceAccountCredential
                ?? throw new InvalidOperationException(
                    "Error al obtener las credenciales GCS. Asegúrate de que la variable esté configurada correctamente.",
                    new Exception("La credencial GCS debe ser una Service Account para generar signed URLs."));
        }

        _storageClient = StorageClient.Create(serviceAccountCredential.ToGoogleCredential());
        _urlSigner = UrlSigner.FromCredential(serviceAccountCredential);
    }

    public async Task<string> UploadAsync(
        Stream content, string objectName, string contentType, CancellationToken ct)
    {
        await _storageClient.UploadObjectAsync(
            _bucketName,
            objectName,
            contentType,
            content,
            cancellationToken: ct);

        return objectName;
    }

    public async Task DeleteAsync(string objectName, CancellationToken ct)
    {
        await _storageClient.DeleteObjectAsync(_bucketName, objectName, cancellationToken: ct);
    }

    public string GetSignedUrl(string objectName)
    {
        return _urlSigner.Sign(
            _bucketName,
            objectName,
            TimeSpan.FromMinutes(_expirationMinutes),
            HttpMethod.Get);
    }
}
