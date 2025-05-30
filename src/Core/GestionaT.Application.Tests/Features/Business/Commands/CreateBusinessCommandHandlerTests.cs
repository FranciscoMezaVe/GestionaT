using AutoMapper;
using FluentAssertions;
using FluentResults;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Features.Business.Commands.CreateBusinessCommand;
using GestionaT.Application.Features.Members.Commands.CreateMembersCommand;
using GestionaT.Application.Features.Roles.Commands.CreateRolesCommand;
using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Shared.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace GestionaT.Application.Tests.Features.Business.Commands
{
    public class CreateBusinessCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<CreateBusinessCommandHandler>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IBusinessRepository> _businessRepositoryMock;
        private readonly Mock<IMediator> _mediatorMock;

        private readonly CreateBusinessCommandHandler _handler;

        public CreateBusinessCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CreateBusinessCommandHandler>>();
            _mapperMock = new Mock<IMapper>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _businessRepositoryMock = new Mock<IBusinessRepository>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new CreateBusinessCommandHandler(
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _mapperMock.Object,
                _currentUserServiceMock.Object,
                _businessRepositoryMock.Object,
                _mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateBusiness_WhenUserHasNoBusiness()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var businessId = Guid.NewGuid();
            var businessName = "Negocio Prueba";
            var request = new CreateBusinessCommand(businessName);

            _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
            _businessRepositoryMock.Setup(x => x.GetAllByUserId(userId)).Returns(new List<Domain.Entities.Business>());

            var mappedBusiness = new Domain.Entities.Business { Id = businessId, Name = businessName };
            _mapperMock.Setup(x => x.Map<Domain.Entities.Business>(request)).Returns(mappedBusiness);

            _mediatorMock.Setup(x => x.Send(It.IsAny<CreateRolesCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok(Guid.NewGuid()));

            _mediatorMock.Setup(x => x.Send(It.IsAny<CreateMembersCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok(Guid.NewGuid()));

            _unitOfWorkMock.Setup(x => x.Repository<Domain.Entities.Business>().AddAsync(mappedBusiness)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(businessId);
            _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenUserAlreadyHasBusiness()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var businessName = "Negocio Prueba";
            var request = new CreateBusinessCommand(businessName);

            _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
            _businessRepositoryMock.Setup(x => x.GetAllByUserId(userId)).Returns(new List<Domain.Entities.Business>
        {
            new Domain.Entities.Business { Id = Guid.NewGuid(), OwnerId = userId, Name = businessName }
        });

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.Conflict);
            _unitOfWorkMock.Verify(x => x.RollbackTransactionAsync(), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenCreateRoleFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var businessName = "Negocio Prueba";
            var request = new CreateBusinessCommand(businessName);

            _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
            _businessRepositoryMock.Setup(x => x.GetAllByUserId(userId)).Returns(new List<Domain.Entities.Business>());

            var business = new Domain.Entities.Business { Id = Guid.NewGuid(), Name = businessName };
            _mapperMock.Setup(x => x.Map<Domain.Entities.Business>(request)).Returns(business);

            _mediatorMock.Setup(x => x.Send(It.IsAny<CreateRolesCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(AppErrorFactory.Internal("Error al crear rol")));

            _unitOfWorkMock.Setup(x => x.Repository<Domain.Entities.Business>().AddAsync(business)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.InternalError);
            _unitOfWorkMock.Verify(x => x.RollbackTransactionAsync(), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldFail_WhenCreateMemberFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var businessName = "Negocio Prueba";
            var request = new CreateBusinessCommand(businessName);

            _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
            _businessRepositoryMock.Setup(x => x.GetAllByUserId(userId)).Returns(new List<Domain.Entities.Business>());

            var business = new Domain.Entities.Business { Id = Guid.NewGuid(), Name = businessName };
            _mapperMock.Setup(x => x.Map<Domain.Entities.Business>(request)).Returns(business);

            var roleId = Guid.NewGuid();
            _mediatorMock.Setup(x => x.Send(It.IsAny<CreateRolesCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok(roleId));

            _mediatorMock.Setup(x => x.Send(It.IsAny<CreateMembersCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail(AppErrorFactory.Internal("Error al crear miembro")));

            _unitOfWorkMock.Setup(x => x.Repository<Domain.Entities.Business>().AddAsync(business)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.InternalError);
            _unitOfWorkMock.Verify(x => x.RollbackTransactionAsync(), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Never);
        }
    }
}
