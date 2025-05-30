using FluentAssertions;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Features.Business.Commands.DeleteBusinessCommand;
using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Shared.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;

namespace GestionaT.Application.Tests.Features.Business.Commands
{
    public class DeleteBusinessCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<DeleteBusinessCommandHandler>> _loggerMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        private readonly DeleteBusinessCommandHandler _handler;

        public DeleteBusinessCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<DeleteBusinessCommandHandler>>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new DeleteBusinessCommandHandler(
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _currentUserServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_Should_ReturnFail_When_Business_NotFound_Or_NotOwned()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new DeleteBusinessCommand(Guid.NewGuid());

            _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
            _unitOfWorkMock.Setup(x => x.Repository<Domain.Entities.Business>().Query())
                                   .Returns(new List<Domain.Entities.Business>().AsQueryable());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.NotFound);

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_SoftDeleteBusiness_And_ReturnSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var businessName = "Test Business";
            var businessId = Guid.NewGuid();
            var business = new Domain.Entities.Business
            {
                Id = businessId,
                OwnerId = userId,
                IsDeleted = false,
                Name = businessName
            };

            var command = new DeleteBusinessCommand(businessId);

            _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
            _unitOfWorkMock.Setup(x => x.Repository<Domain.Entities.Business>().Query())
                                   .Returns(new List<Domain.Entities.Business> { business }.AsQueryable());
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(CancellationToken.None)).ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            business.IsDeleted.Should().BeTrue();

            _unitOfWorkMock.Verify(x => x.Repository<Domain.Entities.Business>().Update(It.Is<Domain.Entities.Business>(b => b.Id == businessId && b.IsDeleted)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }
    }
}
