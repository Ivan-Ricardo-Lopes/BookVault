
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