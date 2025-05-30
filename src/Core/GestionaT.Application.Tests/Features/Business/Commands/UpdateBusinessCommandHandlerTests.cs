using AutoMapper;
using FluentAssertions;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Features.Business.Commands.UpdateBusinessCommand;
using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Application.Interfaces.UnitOfWork;
using Microsoft.Extensions.Logging;
using Moq;

namespace GestionaT.Application.Tests.Features.Business.Commands
{
    public class UpdateBusinessCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRepository<Domain.Entities.Business>> _businessRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<UpdateBusinessCommandHandler>> _loggerMock;

        private readonly UpdateBusinessCommandHandler _handler;

        public UpdateBusinessCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _businessRepositoryMock = new Mock<IRepository<Domain.Entities.Business>>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<UpdateBusinessCommandHandler>>();

            _unitOfWorkMock.Setup(u => u.Repository<Domain.Entities.Business>())
                           .Returns(_businessRepositoryMock.Object);

            _handler = new UpdateBusinessCommandHandler(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_Should_ReturnFail_When_Business_NotFound()
        {
            // Arrange
            var businessName = "NonExistentBusiness";
            var command = new UpdateBusinessCommand(Guid.NewGuid(), new UpdateBusinessDto(businessName));

            _businessRepositoryMock.Setup(r => r.Query())
                                   .Returns(new List<Domain.Entities.Business>().AsQueryable());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.NotFound);

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_UpdateBusiness_And_ReturnSuccess()
        {
            // Arrange
            var businessId = Guid.NewGuid();
            var existingBusiness = new Domain.Entities.Business { Id = businessId, Name = "Old Name" };
            var command = new UpdateBusinessCommand(businessId, new UpdateBusinessDto("Updated Name"));

            _businessRepositoryMock.Setup(r => r.Query())
                                   .Returns(new List<Domain.Entities.Business> { existingBusiness }.AsQueryable());

            _mapperMock.Setup(m => m.Map(command.Business, existingBusiness));

            _businessRepositoryMock.Setup(r => r.Update(existingBusiness));
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(CancellationToken.None))
                           .ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            _mapperMock.Verify(m => m.Map(command.Business, existingBusiness), Times.Once);
            _businessRepositoryMock.Verify(r => r.Update(existingBusiness), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
