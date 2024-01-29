
# ASP.NET Core Backend Project

## Introduction
This ASP.NET Core project serves as the backend for a React frontend, providing robust API endpoints, user management with ASP.NET Core Identity, and JWT authentication for secure access.

## Backend Structure
- **Controllers**
  - `AccountController`: Manages user registration and login.
  - `CartController`: Handles cart-related operations.
  - `ProductsController`: CRUD operations for product management.
  - `WeatherForecastController`: Example controller with weather forecasts.
- **Models**
  - `Cart`: Shopping cart entity.
  - `Product`: Product entity including details like name, price, and image path.
  - `Users`: User entity that extends IdentityUser.
  - `ProductDBContext`: EF Core database context.
- **DataSeeder**: Helper class to seed initial data into the database.

## Key Features
- User Authentication & Authorization using JWT.
- Role Management for 'Admin' and 'Client'.
- Admin account seeding.
- Entity Framework Core with SQL Server.
- Static file serving configured for images and other assets.

## Configuration
Configurations are managed through `appsettings.json`, which includes logging levels, connection strings for the database, JWT settings for authentication, and the path for static files.

## Setup and Installation
1. Install .NET SDK.
2. Clone the repository:
   ```
   git clone https://github.com/cl3arview/Project_backend.git
3. Restore dependencies and build the project:
   ```
   dotnet restore
   dotnet build
   ```
4. Update `appsettings.json` with your database and JWT settings.
5. Start the application:
   ```
   dotnet run
   ```

## Usage
Access the application via the provided endpoints to authenticate users, manage products, and shopping cart operations.
