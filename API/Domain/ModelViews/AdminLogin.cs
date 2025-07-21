using MinimalApi.Domain.Enums;

namespace MinimalApi.Domain.ModelViews;

public record AdminLogin
{
    public string Email { get; set; } = default!;
    public string Perfil { get; set; } = default!;
    public string Token { get; set; } = default!;
}