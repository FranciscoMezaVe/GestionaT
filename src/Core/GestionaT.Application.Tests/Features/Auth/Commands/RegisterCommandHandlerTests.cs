using AutoMapper;
using FluentAssertions;
using FluentResults;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Common.Models;
using GestionaT.Application.Features.Auth.Commands.RegisterCommand;
using GestionaT.Application.Interfaces.Auth;
using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using GestionaT.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace GestionaT.Application.Tests.Features.Auth.Commands
{
    public class RegisterCommandHandlerTests
    {
        private readonly Mock<IAuthenticationService> _authServiceMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<ILogger<RegisterCommand>> _loggerMock = new();

        private readonly RegisterCommandHandler _handler;

        public RegisterCommandHandlerTests()
        {
            _handler = new RegisterCommandHandler(
                _authServiceMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _unitOfWorkMock.Object,
                _userRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnFailured_WhenUserAlreadyExistsWithOAuthProvider()
        {
            // Arrange
            var email = "existing@oauth.com";
            var userId = Guid.NewGuid();
            var userName = "Test User";

            var user = new UserDto
            {
                Id = userId,
                Email = email,
                Nombre = userName,
                EmailConfirmado = true
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(user);

            _unitOfWorkMock
                .Setup(u => u.Repository<OAuthProviders>().Query())
                .Returns(new[] {
                    new OAuthProviders { UserId = userId, ExternalProvider = OAuthProvidersValues.Facebook }
                }.AsQueryable());

            var command = new RegisterCommand(new RegisterCommandRequest(userName, email, "pass"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.AlreadyOAuthExists);

            // Verificar que no se intentó registrar usuario de nuevo
            _authServiceMock.Verify(s => s.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailured_WhenUserAlreadyExistsWithoutOAuth()
        {
            // Arrange
            var email = "existing@oauth.com";
            var userId = Guid.NewGuid();
            var name = "Test User";

            var user = new UserDto
            {
                Id = userId,
                Email = email,
                Nombre = name,
                EmailConfirmado = true
            };

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync(user);

            _unitOfWorkMock
                .Setup(u => u.Repository<OAuthProviders>().Query())
                .Returns(Enumerable.Empty<OAuthProviders>().AsQueryable());

            var command = new RegisterCommand(new RegisterCommandRequest(name, email, "pass"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.AlreadyExists);

            // Verificar que no se intentó registrar usuario de nuevo
            _authServiceMock.Verify(s => s.RegisterUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenAuthServiceFails()
        {
            // Arrange
            var email = "newuser@test.com";

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync((UserDto?)null);

            var failResult = Result.Fail<Guid>(AppErrorFactory.Internal("Fallo el registrar"));

            _authServiceMock
                .Setup(s => s.RegisterUserAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(failResult);

            var command = new RegisterCommand(new RegisterCommandRequest("newuser", email, "securepass"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.InternalError);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var email = "success@domain.com";
            var expectedId = Guid.NewGuid();

            _userRepositoryMock
                .Setup(r => r.GetByEmailAsync(email))
                .ReturnsAsync((UserDto?)null);

            _authServiceMock
                .Setup(s => s.RegisterUserAsync(email, "newuser", "securepass"))
                .ReturnsAsync(Result.Ok(expectedId));

            var command = new RegisterCommand(new RegisterCommandRequest("newuser", email, "securepass"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(expectedId);
        }
    }
}