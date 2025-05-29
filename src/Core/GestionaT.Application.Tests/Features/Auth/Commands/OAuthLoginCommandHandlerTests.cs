using Moq;
using FluentAssertions;
using GestionaT.Application.Features.Auth.Commands.OAuthLoginCommand;
using GestionaT.Application.Interfaces.Auth;
using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Application.Common.Errors;
using Microsoft.Extensions.Logging;
using FluentResults;
using GestionaT.Domain.ValueObjects;
using GestionaT.Application.Common.Models;
using GestionaT.Domain.Enums;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;

namespace GestionaT.Application.Tests.Features.Auth.Commands
{
    public class OAuthLoginCommandHandlerTests
    {
        private readonly Mock<IOAuthServiceFactory> _factoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
        private readonly Mock<IAuthenticationService> _authServiceMock;
        private readonly Mock<IOAuthService> _oAuthServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<OAuthLoginCommandHandler>> _loggerMock;

        private readonly OAuthLoginCommandHandler _handler;

        public OAuthLoginCommandHandlerTests()
        {
            _factoryMock = new Mock<IOAuthServiceFactory>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _authServiceMock = new Mock<IAuthenticationService>();
            _oAuthServiceMock = new Mock<IOAuthService>();
            _loggerMock = new Mock<ILogger<OAuthLoginCommandHandler>>();
            _unitOfWorkMock = new();

            _handler = new OAuthLoginCommandHandler(
                _factoryMock.Object,
                _userRepositoryMock.Object,
                _jwtTokenServiceMock.Object,
                _authServiceMock.Object,
                _loggerMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenProviderIsNotSupported()
        {
            _factoryMock.Setup(f => f.GetService("InvalidProvider")).Returns((IOAuthService?)null);

            var command = new OAuthLoginCommand("InvalidProvider", "token");

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.NotSupported);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenUserInfoIsNull()
        {
            _factoryMock.Setup(f => f.GetService(OAuthProvidersValues.Facebook)).Returns(_oAuthServiceMock.Object);
            _oAuthServiceMock.Setup(s => s.GetUserInfoAsync("token")).ReturnsAsync((OAuthUserInfoResult?)null);

            var command = new OAuthLoginCommand("token", OAuthProvidersValues.Facebook);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.InternalError);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenEmailIsNullOrEmpty()
        {
            _factoryMock.Setup(f => f.GetService(OAuthProvidersValues.Facebook)).Returns(_oAuthServiceMock.Object);
            _oAuthServiceMock.Setup(s => s.GetUserInfoAsync("token")).ReturnsAsync(new OAuthUserInfoResult { Email = "" });

            var command = new OAuthLoginCommand("token", OAuthProvidersValues.Facebook);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.BadRequest);
        }

        [Fact]
        public async Task Handle_ShouldLogin_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            var email = "test@example.com";
            var userInfo = new OAuthUserInfoResult { Email = email };

            _factoryMock.Setup(f => f.GetService(OAuthProvidersValues.Facebook)).Returns(_oAuthServiceMock.Object);
            _oAuthServiceMock.Setup(s => s.GetUserInfoAsync("token")).ReturnsAsync(userInfo);
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(new UserDto { Id = userId });
            _authServiceMock.Setup(a => a.GetUserRolesAsync(userId)).ReturnsAsync([RolesValues.Worker]);
            _jwtTokenServiceMock.Setup(j => j.GenerateToken(userId, email, It.IsAny<IList<string>>())).Returns("jwt-token");
            _jwtTokenServiceMock.Setup(j => j.GenerateRefreshTokenAsync(userId)).ReturnsAsync("refresh-token");

            var command = new OAuthLoginCommand("token", OAuthProvidersValues.Facebook);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Token.Should().Be("jwt-token");
            result.Value.RefreshToken.Should().Be("refresh-token");
        }

        [Fact]
        public async Task Handle_ShouldRegisterAndLogin_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var email = "test@example.com";
            var userInfo = new OAuthUserInfoResult { Email = email };

            _factoryMock.Setup(f => f.GetService(OAuthProvidersValues.Facebook)).Returns(_oAuthServiceMock.Object);
            _oAuthServiceMock.Setup(s => s.GetUserInfoAsync("token")).ReturnsAsync(userInfo);
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((UserDto?)null);
            _authServiceMock.Setup(a => a.RegisterUserOAuthAsync(userInfo)).ReturnsAsync(Result.Ok(userId));
            _authServiceMock.Setup(a => a.GetUserRolesAsync(userId)).ReturnsAsync([RolesValues.Worker]);
            _jwtTokenServiceMock.Setup(j => j.GenerateToken(userId, email, It.IsAny<IList<string>>())).Returns("jwt-token");
            _jwtTokenServiceMock.Setup(j => j.GenerateRefreshTokenAsync(userId)).ReturnsAsync("refresh-token");

            var command = new OAuthLoginCommand("token", OAuthProvidersValues.Facebook);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Token.Should().Be("jwt-token");
            result.Value.RefreshToken.Should().Be("refresh-token");
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenRegistrationFails()
        {
            var email = "test@example.com";
            var userInfo = new OAuthUserInfoResult { Email = email };

            _factoryMock.Setup(f => f.GetService(OAuthProvidersValues.Facebook)).Returns(_oAuthServiceMock.Object);
            _oAuthServiceMock.Setup(s => s.GetUserInfoAsync("token")).ReturnsAsync(userInfo);
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((UserDto?)null);
            _authServiceMock.Setup(a => a.RegisterUserOAuthAsync(userInfo)).ReturnsAsync(Result.Fail("Error"));

            var command = new OAuthLoginCommand("token", OAuthProvidersValues.Facebook);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.InternalError);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenEmailIsAlreadyRegistered()
        {
            // Arrange
            var email = "existing@example.com";
            var userId = Guid.NewGuid();
            var accessToken = "valid-token";

            var userInfo = new OAuthUserInfoResult
            {
                Email = email,
                Name = "Existing User",
                Picture = new Picture
                {
                    Data = new PictureData
                    {
                        Url = "http://example.com/picture.jpg",
                    }
                },
                Provider = OAuthProvidersValues.Facebook
            };

            var command = new OAuthLoginCommand(Provider: OAuthProvidersValues.Facebook, AccessToken: accessToken);

            _factoryMock
                .Setup(x => x.GetService(command.Provider))
                .Returns(_oAuthServiceMock.Object);

            _oAuthServiceMock
                .Setup(x => x.GetUserInfoAsync(accessToken))
                .ReturnsAsync(userInfo);

            _userRepositoryMock
                .Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(new UserDto { Id = userId, Email = email });

            _authServiceMock
                .Setup(x => x.GetUserRolesAsync(userId))
                .ReturnsAsync([RolesValues.Worker]);

            _jwtTokenServiceMock
                .Setup(x => x.GenerateToken(userId, email, It.IsAny<IList<string>>()))
                .Returns("jwt-token");

            _jwtTokenServiceMock
                .Setup(x => x.GenerateRefreshTokenAsync(userId))
                .ReturnsAsync("refresh-token");

            _unitOfWorkMock
                .Setup(x => x.Repository<OAuthProviders>().Query())
                .Returns(new List<OAuthProviders>().AsQueryable());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.NotLinked);

            // Very importante: Verificar que NO se llamó a registrar al usuario
            _authServiceMock.Verify(x => x.RegisterUserOAuthAsync(It.IsAny<OAuthUserInfoResult>()), Times.Never);
            _jwtTokenServiceMock.Verify(x => x.GenerateToken(Guid.NewGuid(), email, It.IsAny<IList<string>>()), Times.Never);
            _jwtTokenServiceMock.Verify(x => x.GenerateRefreshTokenAsync(Guid.NewGuid()), Times.Never);
        }

    }
}