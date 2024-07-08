using AutoFixture;
using BallastLane.BookVault.Domain.Entities;
using BallastLane.BookVault.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace BallastLane.BookVault.Tests.IntegrationTests.Repositories
{
    [TestFixture]
    public class BookRepositoryTests : BaseIntegrationTests
    {
        private IBookRepository _bookRepository;

        [OneTimeSetUp]
        public new async Task OneTimeSetUp()
        {
            await base.OneTimeSetUp();

            using var scope = _factory.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            _bookRepository = serviceProvider.GetRequiredService<IBookRepository>();
        }

        [Test]
        public async Task AddAsync_ShouldAddBook()
        {
            // Arrange
            var book = _fixture.Build<Book>()
                .With(p => p.ISBN, "1111111111")
                .With(p => p.CreatedBy, "TestUser")
                .With(p => p.CreatedDateUtc, DateTimeOffset.UtcNow)
                .With(p => p.DeletedBy, (string?)null)
                .With(p => p.DeletedDateUtc, (DateTimeOffset?)null)
                .With(p => p.ModifiedBy, (string?)null)
                .With(p => p.ModifiedDateUtc, (DateTimeOffset?)null)
                .Create();

            // Act
            await _bookRepository.AddAsync(book, default);

            // Assert
            var addedBook = await _bookRepository.GetByIdAsync(book.Id, default);
            addedBook.Should().NotBeNull();
            addedBook!.ISBN.Should().Be(book.ISBN);
            addedBook!.Author.Should().Be(book.Author);
            addedBook!.Title.Should().Be(book.Title);
            addedBook!.CreatedBy.Should().Be(book.CreatedBy);
            addedBook!.CreatedDateUtc.Date.Should().Be(book.CreatedDateUtc.Date);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnBook()
        {
            // Arrange
            var seedBook = await SeedBook();

            // Act
            var retrievedBook = await _bookRepository.GetByIdAsync(seedBook.Id, default);

            // Assert
            retrievedBook.Should().NotBeNull();
            retrievedBook!.ISBN.Should().Be(seedBook.ISBN);
            retrievedBook!.Author.Should().Be(seedBook.Author);
            retrievedBook!.Title.Should().Be(seedBook.Title);
            retrievedBook!.CreatedBy.Should().Be(seedBook.CreatedBy);
            retrievedBook!.CreatedDateUtc.Date.Should().Be(seedBook.CreatedDateUtc.Date);
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateBook()
        {
            // Arrange
            var seedBook = await SeedBook();
            seedBook.Title = "Updated Title";
            seedBook.ISBN = "2222222222";
            seedBook.ModifiedDateUtc = DateTimeOffset.UtcNow;
            seedBook.ModifiedBy = "TestUser";

            // Act
            await _bookRepository.UpdateAsync(seedBook, default);

            // Assert
            var updatedBook = await _bookRepository.GetByIdAsync(seedBook.Id, default);
            updatedBook.Should().NotBeNull();
            updatedBook!.ISBN.Should().Be(seedBook.ISBN);
            updatedBook!.Title.Should().Be(seedBook.Title);
            updatedBook!.ModifiedBy.Should().Be(seedBook.ModifiedBy);
            updatedBook!.ModifiedDateUtc!.Value.Date.Should().Be(seedBook.ModifiedDateUtc.Value.Date);
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveBook()
        {
            // Arrange
            var seedBook = await SeedBook();

            // Act
            await _bookRepository.DeleteAsync(seedBook.Id, "testUser", default);

            // Assert
            var deletedBook = await _bookRepository.GetByIdAsync(seedBook.Id, default);
            deletedBook.Should().BeNull();
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllBooks()
        {
            // Arrange
            var seedBook1 = await SeedBook();
            var seedBook2 = await SeedBook();

            // Act
            var books = await _bookRepository.GetAllAsync(null, null,default);

            // Assert
            books.Should().NotBeNull();
            books.Should().Contain(b => b.Id == seedBook1.Id);
            books.Should().Contain(b => b.Id == seedBook2.Id);
        }

        private async Task<Book> SeedBook()
        {
            var seedBook = _fixture.Build<Book>()
                .With(p => p.ISBN, "1111111111")
                .With(p => p.CreatedBy, "TestUser")
                .With(p => p.CreatedDateUtc, DateTimeOffset.UtcNow)
                .With(p => p.DeletedBy, (string?)null)
                .With(p => p.DeletedDateUtc, (DateTimeOffset?)null)
                .With(p => p.ModifiedBy, (string?)null)
                .With(p => p.ModifiedDateUtc, (DateTimeOffset?)null)
                .Create();

            await _bookRepository.AddAsync(seedBook, default);

            return seedBook;
        }
    }
}
