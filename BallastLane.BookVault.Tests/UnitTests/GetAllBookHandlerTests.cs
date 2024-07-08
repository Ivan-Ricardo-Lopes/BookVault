using AutoFixture;
using BallastLane.BookVault.Application.Features.Books.GetAllBooks;
using BallastLane.BookVault.Domain.Entities;
using BallastLane.BookVault.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace BallastLane.BookVault.Tests.UnitTests
{
    [TestFixture]
    public class GetAllBookHandlerTests
    {
        private Mock<IBookRepository> _bookRepositoryMock;
        private GetAllBooksHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _handler = new GetAllBooksHandler(new GetAllBooksValidator(), _bookRepositoryMock.Object);
        }

        [Test]
        public async Task HandleAsync_Should_Return_AllBooks_Paged()
        {
            // Arrange
            var request = new GetAllBooksQuery(1, 5);

            var fixture = new Fixture();
            var mockedBooks = fixture.Build<Book>().CreateMany(15);

            _bookRepositoryMock
                .Setup(p => p.GetAllAsync(request.PageNumber, request.PageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockedBooks.Take(5)); // Setup the mock to many books

            // Act
            var response = await _handler.HandleAsync(request, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Result.Should().NotBeNull();
            response.Result!.Books.Count().Should().Be(5);
            response.Result!.Books.First().Should().Be(mockedBooks.First());
            response.Result!.Books.Last().Should().Be(mockedBooks.ElementAt(4));
        }
    }
}
