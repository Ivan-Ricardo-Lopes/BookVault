using Moq;
using BallastLane.BookVault.Domain.Repositories;
using FluentAssertions;
using BallastLane.BookVault.Domain.Entities;
using BallastLane.BookVault.Application.Features.Books.UpdateBook;

namespace BallastLane.BookVault.Tests.UnitTests
{
    [TestFixture]
    public class UpdateBookHandlerTests
    {
        private Mock<IBookRepository> _bookRepositoryMock;
        private UpdateBookHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _handler = new UpdateBookHandler(new UpdateBookValidator(), _bookRepositoryMock.Object);
        }

        [TestCase("", "Valid Author", "1234567890", $"'{nameof(UpdateBookCommand.Title)}' must not be empty.")] // Invalid Title
        [TestCase("Valid Title", "", "1234567890", $"'{nameof(UpdateBookCommand.Author)}' must not be empty.")] // Invalid Author
        [TestCase("Valid Title", "Valid Author", "123", $"'{nameof(UpdateBookCommand.ISBN)}' is not in the correct format.")] // Invalid ISBN
        public async Task HandleAsync_Should_Return_Failure_If_Validation_Fails(string title, string author, string isbn, string expectedErrorMessage)
        {
            // Arrange
            var command = new UpdateBookCommand("TestUser", Guid.NewGuid(), title, author, isbn);

            // Act
            var response = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.ErrorMessages.Count().Should().Be(1);
            response.ErrorMessages.First().Should().Be(expectedErrorMessage);
        }

        [TestCase("Valid Title", "Valid Author", "1234567890")] // ISBN 10 digits
        [TestCase("Valid Title", "Valid Author", "1234567890123")] // ISBN 13 digits
        public async Task HandleAsync_Should_Update_Book_And_Return_Success_If_Validation_Passes(string title, string author, string isbn)
        {
            // Arrange
            var command = new UpdateBookCommand("TestUser", Guid.NewGuid(), title, author, isbn);

            _bookRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1); // Simulate successful update

            // Act
            var response = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.ErrorMessages.Should().BeEmpty();

            _bookRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task HandleAsync_Should_Return_Failure_If_Book_Not_Found()
        {
            // Arrange
            var command = new UpdateBookCommand("TestUser", Guid.NewGuid(), "Valid Title", "Valid Author", "1234567890");

            _bookRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0); // Simulate book not found

            // Act
            var response = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.ErrorMessages.Count().Should().Be(1);
            response.ErrorMessages.First().Should().Be("Book not found.");

            _bookRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
