
# BookVault

This is a CRUD application for managing books.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/Ivan-Ricardo-Lopes/BookVault.git
```

### Build and Run with Docker Compose

   * Ensure Docker is running on your machine.
   * Use Docker Compose to build and run the services:

```bash
docker-compose up --build
```

### Run the .Net project

Once the .Net project and containers are up and running, you can access the Swagger UI for the Web API by navigating to:

```
https://localhost:7245/swagger/index.html
```

### Calling the API via Swagger

* Create an user by calling the SignUp endpoint
* Retrieve a token by calling the SignIn endpoint
* Insert the token into the Authorize section
* Use the Book endpoints

## My Strategy

I began by creating the user stories (UserStories.pdf) and proceeded to set up the project's layers:

API
Application
Domain
Infrastructure
Tests
I then defined the Book and User entities along with the repository interfaces. 
In the Application layer, I established IRequestHandler interfaces and Result value objects to support the command handlers. Given the simplicity of the CRUD operations, I opted not to use MediatR and instead implemented a more straightforward solution with one command handler per feature, organizing each feature into its own folder.

Once all handler classes, command classes, responses, and validators were created, I moved to the API layer to set up minimal APIs that invoke the appropriate handlers. I configured dependency injection to ensure the correct handlers were injected into the minimal APIs.

For the testing project, due to the constraints of not using EntityFramework, I selected PostgreSQL in a Docker container and accessed it via ADO. I developed unit tests for the command handlers and integration tests for the repositories using the TestContainers library.

After completing the implementation, I identified the need for authentication. I implemented a simple JWT authentication mechanism and applied authorization across all endpoints.

Finally, I documented the project in the README.
