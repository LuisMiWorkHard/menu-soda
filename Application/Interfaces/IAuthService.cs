using MenuSoda.Application.Dto;

namespace MenuSoda.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginServiceRequest request, CancellationToken ct);
    Task<RefreshServiceResponse> RefreshAsync(RefreshServiceRequest request, CancellationToken ct);
    Task LogoutAsync(LogoutServiceRequest request, CancellationToken ct);
}
