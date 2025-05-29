using FluentAssertions;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Features.Auth.Commands.RefreshTokenCommand;
using GestionaT.Application.Interfaces.Auth;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace GestionaT.Application.Tests.Features.Auth.Commands
{
    public class RefreshTokenCommandHandlerTests
    {
        private readonly Mock<IAuthenticationService> _authServiceMock = new();
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<ILogger<RefreshTokenCommandHandler>> _loggerMock = new();

        private readonly RefreshTokenCommandHandler _handler;

        public RefreshTokenCommandHandlerTests()
        {
            _handler = new RefreshTokenCommandHandler(
                _authServiceMock.Object,
                _jwtTokenServiceMock.Object,
                _loggerMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenTokenAndUserAreValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var refreshToken = "valid-token";
            var email = "test@example.com";
            var newJwtToken = "new-access-token";
            var newRefreshToken = "new-refresh-token";

            var token = new RefreshToken
            {
                Token = refreshToken,
                UserId = userId,
                Expires = DateTime.UtcNow.AddDays(1),
                IsRevoked = false
            };

            _unitOfWorkMock
                .Setup(x => x.Repository<RefreshToken>().Query())
                .Returns(new[] { token }.AsQueryable());

            _authServiceMock.Setup(x => x.IsExistsUserByIdAsync(userId))
                .ReturnsAsync(true);

            _authServiceMock.Setup(x => x.GetUserEmailAsync(userId))
                .ReturnsAsync(email);

            _authServiceMock.Setup(x => x.GetUserRolesAsync(userId))
                .ReturnsAsync(new List<string> { "Worker"});

            _jwtTokenServiceMock.Setup(x => x.GenerateToken(userId, email, It.IsAny<IList<string>>()))
                .Returns(newJwtToken);

            _jwtTokenServiceMock.Setup(x => x.GenerateRefreshTokenAsync(userId))
                .ReturnsAsync(newRefreshToken);

            var command = new RefreshTokenCommand(refreshToken);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.NewToken.Should().Be(newJwtToken);
            result.Value.NewRefreshToken.Should().Be(newRefreshToken);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenTokenNotFound()
        {
            // Arrange
            var command = new RefreshTokenCommand("missing-token");

            _unitOfWorkMock
                .Setup(x => x.Repository<RefreshToken>().Query())
                .Returns(Enumerable.Empty<RefreshToken>().AsQueryable());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().Contain(e =>
                e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.Unauthorized);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenTokenIsInvalid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expiredToken = new RefreshToken
            {
                Token = "expired-token",
                UserId = userId,
                Expires = DateTime.UtcNow.AddMinutes(-1),
                IsRevoked = false
            };

            _unitOfWorkMock
                .Setup(x => x.Repository<RefreshToken>().Query())
                .Returns(new[] { expiredToken }.AsQueryable());

            var command = new RefreshTokenCommand("expired-token");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().Contain(e =>
                e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.Unauthorized);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var refreshToken = "valid-token";

            var token = new RefreshToken
            {
                Token = refreshToken,
                UserId = userId,
                Expires = DateTime.UtcNow.AddDays(1),
                IsRevoked = false
            };

            _unitOfWorkMock
                .Setup(x => x.Repository<RefreshToken>().Query())
                .Returns(new[] { token }.AsQueryable());

            _authServiceMock.Setup(x => x.IsExistsUserByIdAsync(userId))
                .ReturnsAsync(false);

            var command = new RefreshTokenCommand(refreshToken);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().Contain(e =>
                e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.Unauthorized);
        }
    }
}