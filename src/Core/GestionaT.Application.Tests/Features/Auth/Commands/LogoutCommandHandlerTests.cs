using FluentAssertions;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Features.Auth.Commands.LogoutCommand;
using GestionaT.Application.Interfaces.Auth;
using GestionaT.Shared.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace GestionaT.Application.Tests.Features.Auth.Commands
{
    public class LogoutCommandHandlerTests
    {
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<ILogger<LogoutCommandHandler>> _loggerMock;
        private readonly LogoutCommandHandler _handler;

        public LogoutCommandHandlerTests()
        {
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _loggerMock = new Mock<ILogger<LogoutCommandHandler>>();

            _handler = new LogoutCommandHandler(
                _jwtTokenServiceMock.Object,
                _loggerMock.Object,
                _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenLogoutIsSuccessful()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
            _jwtTokenServiceMock.Setup(x => x.RemoveRefreshTokenAsync(userId))
                .ReturnsAsync(true);

            var command = new LogoutCommand();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserIdIsEmpty()
        {
            // Arrange
            _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.Empty);
            var command = new LogoutCommand();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e =>
                e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.NotFound);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenRemoveRefreshTokenFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
            _jwtTokenServiceMock.Setup(x => x.RemoveRefreshTokenAsync(userId))
                .ReturnsAsync(false);

            var command = new LogoutCommand();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e =>
                e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.InternalError);
        }
    }
}
