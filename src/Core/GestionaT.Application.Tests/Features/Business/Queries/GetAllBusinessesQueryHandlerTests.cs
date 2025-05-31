using AutoMapper;
using GestionaT.Application.Common.Errors;
using GestionaT.Application.Common.Pagination;
using GestionaT.Application.Features.Business.Queries.GetAllBusinessesQuery;
using GestionaT.Application.Features.Business.Queries;
using GestionaT.Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;

namespace GestionaT.Application.Tests.Features.Business.Queries
{
    public class GetAllBusinessesQueryHandlerTests
    {
        private readonly Mock<ILogger<GetAllBusinessesQueryHandler>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IBusinessRepository> _businessRepositoryMock;
        private readonly GetAllBusinessesQueryHandler _handler;

        public GetAllBusinessesQueryHandlerTests()
        {
            _loggerMock = new Mock<ILogger<GetAllBusinessesQueryHandler>>();
            _mapperMock = new Mock<IMapper>();
            _businessRepositoryMock = new Mock<IBusinessRepository>();
            _handler = new GetAllBusinessesQueryHandler(_loggerMock.Object, _mapperMock.Object, _businessRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenNoBusinessesFound()
        {
            // Arrange
            var query = new GetAllBusinessesQuery(Guid.NewGuid(), new PaginationFilters { PageIndex = 1, PageSize = 25});

            _businessRepositoryMock
                .Setup(repo => repo.GetBusinessAccessibleByUser(query.UserId))
                .Returns([]);

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle();
            result.Errors.Should().ContainSingle(e => e.Metadata[MetaDataErrorValues.Code].ToString() == ErrorCodes.NotFound);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("No se encontraron negocios")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnBusinesses_WhenBusinessesExist()
        {
            // Arrange
            var query = new GetAllBusinessesQuery(Guid.NewGuid(), new PaginationFilters { PageIndex = 1, PageSize = 25 });

            var businesses = new List<Domain.Entities.Business>
                {
                    new Domain.Entities.Business { Id = Guid.NewGuid(), Name = "Business 1" },
                    new Domain.Entities.Business { Id = Guid.NewGuid(), Name = "Business 2" }
                };

            var businessResponses = new List<BusinessReponse>
        {
            new BusinessReponse { Id = Guid.NewGuid(), Name = "Business 1" },
            new BusinessReponse { Id = Guid.NewGuid(), Name = "Business 2" }
        };

            var pagedList = new PaginatedList<BusinessReponse>(businessResponses, businessResponses.Count, 1, 2);

            _businessRepositoryMock
                .Setup(r => r.GetBusinessAccessibleByUser(query.UserId))
                .Returns(businesses);

            _mapperMock
                .Setup(m => m.Map<List<BusinessReponse>>(It.IsAny<List<Domain.Entities.Business>>()))
                .Returns(businessResponses);

            // Act
            var result = await _handler.Handle(query, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(2);
            result.Value.Items.Should().ContainSingle(x => x.Name == "Business 1");
            result.Value.Items.Should().ContainSingle(x => x.Name == "Business 2");
        }
    }
}
