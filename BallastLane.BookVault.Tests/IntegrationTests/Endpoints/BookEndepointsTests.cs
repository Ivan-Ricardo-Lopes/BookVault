using AutoFixture;
using BallastLane.BookVault.Api.Endpoints.Books;
using BallastLane.BookVault.Application.Features.Books.AddBook;
using BallastLane.BookVault.Application.Features.Books.GetAllBooks;
using BallastLane.BookVault.Application.Features.Books.GetBook;
using BallastLane.BookVault.Application.Features.Books.UpdateBook;
using BallastLane.BookVault.Domain.Entities;
using BallastLane.BookVault.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace BallastLane.BookVault.Tests.IntegrationTests.Endpoints
{
    [TestFixture]
    public class BookEndepointsTests : BaseIntegrationTests
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
        public async Task AddBook_ReturnsId()
        {
            // Arrange
            var client = CreateClient();

            var request = _fixture.Build<AddBookRequest>()
                .With(p => p.ISBN, "1234567891")
                .Create<AddBookRequest>();

            // Act
            var response = await client.PostAsync("/books/", CreateContent(request));

            //Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AddBookResponse>(content, _jsonOptions);

            result.Should().NotBeNull();
            result!.Id.Should().NotBeEmpty();
        }

        [Test]
        public async Task UpdateBook_ReturnsNoContent()
        {
            // Arrange
            var seedBook = await SeedBook();

            await _bookRepository.AddAsync(seedBook, default);

            var request = _fixture.Build<UpdateBookCommand>()
                .With(p => p.Id, seedBook.Id)
                .With(p => p.ISBN, "2222222222")
                .Create<UpdateBookCommand>();

            var client = CreateClient();

            // Act
            var response = await client.PutAsync("/books/", CreateContent(request));


            // Assert
            response.EnsureSuccessStatusCode();

            var updatedBook = await _bookRepository.GetByIdAsync(seedBook.Id, default);

            updatedBook.Should().NotBeNull();
            updatedBook!.ISBN.Should().Be(request.ISBN);
            updatedBook!.Author.Should().Be(request.Author);
            updatedBook!.Title.Should().Be(request.Title);
        }

        [Test]
        public async Task GetBook_ReturnBook()
        {
            // Arrange
            var seedBook = await SeedBook();

            await _bookRepository.AddAsync(seedBook, default);

            var client = CreateClient();

            // Act
            var response = await client.GetAsync($"/books/{seedBook.Id}");

            // Assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<GetBookResponse>(content, _jsonOptions);

            result.Should().NotBeNull();
            result!.Book.ISBN.Should().Be(seedBook.ISBN);
            result!.Book.Author.Should().Be(seedBook.Author);
            result!.Book.Title.Should().Be(seedBook.Title);
        }

        [Test]
        public async Task DeleteBook_ReturnBook()
        {
            // Arrange
            var seedBook = await SeedBook();

            var client = CreateClient();

            // Act
            var response = await client.DeleteAsync($"/books/{seedBook.Id}");

            // Assert
            response.EnsureSuccessStatusCode();

            var deletedBook = await _bookRepository.GetByIdAsync(seedBook.Id, default);

            deletedBook.Should().BeNull();
        }


        [Test]
        public async Task GetAllBooks_ReturnsEmptyList()
        {
            // Arrange
            var client = CreateClient();

            // Act
            var response = await client.GetAsync("/books/");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var allresidentsResponse = JsonSerializer.Deserialize<GetAllBooksResponse>(content);

            allresidentsResponse.Should().NotBeNull();
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
