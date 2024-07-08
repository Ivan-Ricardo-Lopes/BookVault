using BallastLane.BookVault.Application.Features.Books.AddBook;
using BallastLane.BookVault.Application.Features.Books.DeleteBook;
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
    public class DeleteBookHandlerTests
    {
        private Mock<IBookRepository> _bookRepositoryMock;
        private DeleteBookHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _handler = new DeleteBookHandler(new DeleteBookValidator(), _bookRepositoryMock.Object);
        }

        [Test]
        public async Task HandleAsync_Should_DeleteBook()
        {
            // Arrange
            var command = new DeleteBookCommand("testUser", Guid.NewGuid());
            _bookRepositoryMock
                .Setup(p => p.DeleteAsync(command.Id, command.DeletedBy, It.IsAny<CancellationToken>()))
                .ReturnsAsync(1); // Setup the mock to return 1 row affected

            // Act
            var response = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeTrue();
            _bookRepositoryMock.Verify(r => r.DeleteAsync(command.Id, command.DeletedBy, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task HandleAsync_Should_Return_Failure_If_Book_Not_Found()
        {
            // Arrange
            var command = new DeleteBookCommand("testUser", Guid.NewGuid());
            _bookRepositoryMock
                .Setup(p => p.DeleteAsync(command.Id, command.DeletedBy, It.IsAny<CancellationToken>()))
                .ReturnsAsync(0); // Setup the mock to return 0 row affected

            // Act
            var response = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.ErrorMessages.Count().Should().Be(1);
            response.ErrorMessages.First().Should().Be("Book not found.");
            _bookRepositoryMock.Verify(r => r.DeleteAsync(command.Id, command.DeletedBy, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
