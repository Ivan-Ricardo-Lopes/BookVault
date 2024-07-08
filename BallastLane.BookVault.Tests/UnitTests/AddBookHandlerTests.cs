using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using BallastLane.BookVault.Domain.Repositories;
using FluentAssertions;
using BallastLane.BookVault.Domain.Entities;
using BallastLane.BookVault.Application.Features.Books.AddBook;

namespace BallastLane.BookVault.Tests.UnitTests
{
    [TestFixture]
    public class AddBookHandlerTests
    {
        private Mock<IBookRepository> _bookRepositoryMock;
        private AddBookHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _handler = new AddBookHandler(new AddBookValidator(), _bookRepositoryMock.Object);
        }

        [TestCase("", "Valid Author", "1234567890", $"'{nameof(AddBookCommand.Title)}' must not be empty.")] // Invalid Title
        [TestCase("Valid Title", "", "1234567890", $"'{nameof(AddBookCommand.Author)}' must not be empty.")] // Invalid Author
        [TestCase("Valid Title", "Valid Author", "123", $"'{nameof(AddBookCommand.ISBN)}' is not in the correct format.")] // Invalid ISBN
        public async Task HandleAsync_Should_Return_Failure_If_Validation_Fails(string title, string author, string isbn, string expectedErrorMessage)
        {
            // Arrange
            var command = new AddBookCommand("TestUser", title, author, isbn);

            // Act
            var response = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.ErrorMessages.Count().Should().Be(1);
            response.ErrorMessages.First().Should().Be(expectedErrorMessage);
        }

        [TestCase("Valid Title", "Valid Author", "1234567890")] // ISBN 10 digits
        [TestCase("Valid Title", "Valid Author", "1234567890123")] // ISBN 13 digits
        public async Task HandleAsync_Should_Add_Book_And_Return_Success_If_Validation_Passes(string title, string author, string isbn)
        {
            // Arrange
            var command = new AddBookCommand("TestUser", title, author, isbn);

            // Act
            var response = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Result.Should().NotBeNull();
            response.Result!.Id.Should().NotBe(Guid.Empty);

            _bookRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}