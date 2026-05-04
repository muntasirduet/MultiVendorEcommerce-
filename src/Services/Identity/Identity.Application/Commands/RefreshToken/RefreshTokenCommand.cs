using Identity.Application.Common;
using Identity.Application.DTOs;
using MediatR;

namespace Identity.Application.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<TokenResponse>>;
