using BallastLane.BookVault.Application.Features.Books.AddBook;
using BallastLane.BookVault.Application.Features.Books.DeleteBook;
using BallastLane.BookVault.Application.Features.Books.GetBook;
using BallastLane.BookVault.Domain.Entities;
using BallastLane.BookVault.Domain.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallastLane.BookVault.Tests.UnitTests
{
    [TestFixture]
    public class GetBookHandlerTests
    {
        private Mock<IBookRepository> _bookRepositoryMock;
        private GetBookHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _handler = new GetBookHandler(new GetBookValidator(), _bookRepositoryMock.Object);
        }

        [Test]
        public async Task HandleAsync_Should_Return_Book()
        {
            // Arrange
            var request = new GetBookQuery(Guid.NewGuid());

            _bookRepositoryMock
                .Setup(p => p.GetByIdAsync(request.Id,It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Book { Id = request.Id}); // Setup the mock to return 1 row affected

            // Act
            var response = await _handler.HandleAsync(request, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Result.Should().NotBeNull();
            response.Result!.Book.Id.Should().Be(request.Id);
            _bookRepositoryMock.Verify(r => r.GetByIdAsync(request.Id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task HandleAsync_Should_Return_Failure_If_Book_Not_Found()
        {
            // Arrange
            var request = new GetBookQuery(Guid.NewGuid());

            // Act
            var response = await _handler.HandleAsync(request, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.ErrorMessages.Count().Should().Be(1);
            response.ErrorMessages.First().Should().Be("Book not found.");
        }
    }
}
