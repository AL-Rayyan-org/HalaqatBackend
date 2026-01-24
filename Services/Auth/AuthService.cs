using BCrypt.Net;
using HalaqatBackend.DTOs.Auth;
using HalaqatBackend.Models;
using HalaqatBackend.Repositories.RefreshTokens;
using HalaqatBackend.Repositories.Users;
using HalaqatBackend.Services.Jwt;
using HalaqatBackend.Utils;
using System.Security.Claims;

namespace HalaqatBackend.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IJwtService jwtService,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtService = jwtService;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            PasswordValidator.ValidateOrThrow(request.Password);

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordHash,
                Phone = request.PhoneNumber,
                Role = request.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(user);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.Phone,
                Role = user.Role,
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("Account is deactivated");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email!, user.Role);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenExpiryDays = int.Parse(_configuration["JwtSettings:RefreshTokenExpiryInDays"] ?? "7");
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpiryDays),
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepository.CreateAsync(refreshTokenEntity);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.Phone,
                Role = user.Role,
                AccessToken = accessToken
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var tokenEntity = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (tokenEntity == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            if (tokenEntity.RevokedAt != default)
            {
                throw new UnauthorizedAccessException("Token has been revoked");
            }

            if (tokenEntity.ExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Token has expired");
            }

            var user = await _userRepository.GetByIdAsync(tokenEntity.UserId);
            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("User not found or inactive");
            }

            await _refreshTokenRepository.RevokeAsync(refreshToken);

            var newAccessToken = _jwtService.GenerateAccessToken(user.Id, user.Email!, user.Role);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenExpiryDays = int.Parse(_configuration["JwtSettings:RefreshTokenExpiryInDays"] ?? "7");
            var newRefreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpiryDays),
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepository.CreateAsync(newRefreshTokenEntity);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.Phone,
                Role = user.Role,
                AccessToken = newAccessToken,
            };
        }
    }
}
