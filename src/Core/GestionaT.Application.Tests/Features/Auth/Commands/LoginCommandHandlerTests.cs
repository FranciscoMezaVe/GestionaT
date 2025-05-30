using FluentAssertions;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Features.Auth.Commands.LoginCommand;
using GestionaT.Application.Interfaces.Auth;
using Microsoft.Extensions.Logging;
using Moq;

namespace GestionaT.Application.Tests.Features.Auth.Commands
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IAuthenticationService> _authenticationServiceMock;
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
        private readonly Mock<ILogger<LoginCommandHandler>> _loggerMock;
        private readonly LoginCommandHandler _handler;

        public LoginCommandHandlerTests()
        {
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _loggerMock = new Mock<ILogger<LoginCommandHandler>>();

            _handler = new LoginCommandHandler(
                _authenticationServiceMock.Object,
                _jwtTokenServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenCredentialsAreValid()
        {
            // Arrange
            var request = new LoginCommand("test@example.com", "password123");
            var userId = Guid.NewGuid();
            var roles = new[] { "Admin", "User" };
            var token = "mocked-jwt-token";
            var refreshToken = "mocked-refresh-token";

            _authenticationServiceMock.Setup(x => x.Authenticate(request.Email, request.Password))
                .ReturnsAsync(true);

            _authenticationServiceMock.Setup(x => x.GetUserIdAsync(request.Email))
                .ReturnsAsync(userId);

            _authenticationServiceMock.Setup(x => x.GetUserRolesAsync(userId))
                .ReturnsAsync(roles);

            _jwtTokenServiceMock.Setup(x => x.GenerateToken(userId, request.Email, roles))
                .Returns(token);

            _jwtTokenServiceMock.Setup(x => x.GenerateRefreshTokenAsync(userId))
                .ReturnsAsync(refreshToken);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Token.Should().Be(token);
            result.Value.RefreshToken.Should().Be(refreshToken);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var request = new LoginCommand("test@example.com", "wrongpassword");

            _authenticationServiceMock.Setup(x => x.Authenticate(request.Email, request.Password))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.Unauthorized);
        }
    }
}
