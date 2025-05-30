using FluentAssertions;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Features.Business.Commands.UploadBusinessImage;
using GestionaT.Application.Interfaces.Images;
using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Application.Interfaces.UnitOfWork;
using GestionaT.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Moq;

namespace GestionaT.Application.Tests.Features.Business.Commands
{
    public class UploadBusinessImageCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRepository<Domain.Entities.Business>> _businessRepoMock;
        private readonly Mock<IRepository<Domain.Entities.BusinessImage>> _imageRepoMock;
        private readonly Mock<IImageStorageService> _imageServiceMock;
        private readonly Mock<ILogger<UploadBusinessImageCommandHandler>> _loggerMock;

        private readonly UploadBusinessImageCommandHandler _handler;

        public UploadBusinessImageCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _businessRepoMock = new Mock<IRepository<Domain.Entities.Business>>();
            _imageRepoMock = new Mock<IRepository<BusinessImage>>();
            _imageServiceMock = new Mock<IImageStorageService>();
            _loggerMock = new Mock<ILogger<UploadBusinessImageCommandHandler>>();

            _unitOfWorkMock.Setup(u => u.Repository<Domain.Entities.Business>())
                           .Returns(_businessRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<BusinessImage>())
                           .Returns(_imageRepoMock.Object);
            _handler = new UploadBusinessImageCommandHandler(
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _imageServiceMock.Object
            );
        }

        private static IFormFile CreateFakeFormFile()
        {
            var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            return new FormFile(stream, 0, stream.Length, "image", "logo.png");
        }

        [Fact]
        public async Task Handle_Should_Fail_When_Business_Not_Found()
        {
            // Arrange
            var command = new UploadBusinessImageCommand(Guid.NewGuid(), CreateFakeFormFile());

            _businessRepoMock.Setup(r => r.GetByIdAsync(command.BusinessId))
                             .ReturnsAsync((Domain.Entities.Business?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.NotFound);
        }

        [Fact]
        public async Task Handle_Should_Fail_When_Previous_Image_Cannot_Be_Deleted()
        {
            // Arrange
            var businessId = Guid.NewGuid();
            var businessName = "Test Business";
            var command = new UploadBusinessImageCommand(businessId, CreateFakeFormFile());

            var business = new Domain.Entities.Business { Id = businessId, Name = businessName };
            var existingImage = new BusinessImage { BusinessId = businessId, PublicId = "old-public-id", ImageUrl = "url" };

            _businessRepoMock.Setup(r => r.GetByIdAsync(businessId))
                             .ReturnsAsync(business);

            _imageRepoMock.Setup(r => r.Query())
                          .Returns(new List<BusinessImage> { existingImage }.AsQueryable());

            _imageServiceMock.Setup(s => s.DeleteImageAsync(existingImage.PublicId))
                             .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().Contain(e => e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.InternalError);
        }

        [Fact]
        public async Task Handle_Should_Replace_Image_When_Previous_Exists()
        {
            // Arrange
            var businessId = Guid.NewGuid();
            var businessName = "Test Business";
            var command = new UploadBusinessImageCommand(businessId, CreateFakeFormFile());

            var business = new Domain.Entities.Business { Id = businessId, Name = businessName };
            var oldImage = new BusinessImage { BusinessId = businessId, PublicId = "old-public", ImageUrl = "url" };

            _businessRepoMock.Setup(r => r.GetByIdAsync(businessId))
                             .ReturnsAsync(business);

            _imageRepoMock.Setup(r => r.Query())
                          .Returns(new List<BusinessImage> { oldImage }.AsQueryable());

            _imageServiceMock.Setup(s => s.DeleteImageAsync("old-public"))
                             .ReturnsAsync(true);

            var newImageUrl = "https://img/new-image.png";
            var newPublicId = "new-public-id";

            _imageServiceMock
                .Setup(s => s.SaveImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
                .ReturnsAsync(newImageUrl);

            _imageServiceMock.Setup(s => s.ExtractPublicIdFromUrl(newImageUrl))
                             .Returns(newPublicId);

            _imageRepoMock.Setup(r => r.AddAsync(It.IsAny<BusinessImage>()))
                          .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(CancellationToken.None))
                           .ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(newImageUrl);

            _imageRepoMock.Verify(r => r.Remove(oldImage), Times.Once);
            _imageRepoMock.Verify(r => r.AddAsync(It.Is<BusinessImage>(b =>
                b.BusinessId == businessId && b.ImageUrl == newImageUrl && b.PublicId == newPublicId
            )), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Add_Image_When_None_Exists()
        {
            // Arrange
            var businessId = Guid.NewGuid();
            var businessName = "Test Business";
            var image = CreateFakeFormFile();
            var command = new UploadBusinessImageCommand(businessId, image);

            var business = new Domain.Entities.Business { Id = businessId, Name = businessName };

            _businessRepoMock.Setup(r => r.GetByIdAsync(businessId))
                             .ReturnsAsync(business);

            _imageRepoMock.Setup(r => r.Query())
                          .Returns(new List<BusinessImage>().AsQueryable());

            var newImageUrl = "https://img/image-added.png";
            var newPublicId = "image-added-public";

            _imageServiceMock
                .Setup(s => s.SaveImageAsync(It.IsAny<IFormFile>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>()))
                .ReturnsAsync(newImageUrl);

            _imageServiceMock.Setup(s => s.ExtractPublicIdFromUrl(newImageUrl))
                             .Returns(newPublicId);

            _imageRepoMock.Setup(r => r.AddAsync(It.IsAny<BusinessImage>()))
                          .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(CancellationToken.None))
                           .ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(newImageUrl);

            _imageRepoMock.Verify(r => r.AddAsync(It.Is<BusinessImage>(b =>
                b.BusinessId == businessId && b.ImageUrl == newImageUrl && b.PublicId == newPublicId
            )), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenImageUrlIsNullOrEmpty()
        {
            // Arrange
            var businessId = Guid.NewGuid();
            var command = new UploadBusinessImageCommand(businessId, CreateFakeFormFile());

            var business = new Domain.Entities.Business { Id = businessId, Name = "Test Business" };

            _businessRepoMock.Setup(r => r.GetByIdAsync(businessId))
                             .ReturnsAsync(business);

            _imageRepoMock.Setup(r => r.Query())
                          .Returns(Enumerable.Empty<BusinessImage>().AsQueryable());

            _imageServiceMock.Setup(s => s.SaveImageAsync(
                    It.IsAny<IFormFile>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>()))
                .ReturnsAsync(string.Empty); // <- Devuelve URL vacía

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e =>
                e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.InternalError);

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error al guardar la imagen para el negocio")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once
            );
        }
    }
}
