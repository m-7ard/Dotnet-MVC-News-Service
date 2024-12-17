# MVC News - A .NET MVC Web Application

## Key Features
- User registration and login
- Admin user roles with additional permissions
- Create, update, preview, and delete news articles
- Search and filter articles by title, author, tags
- Display articles with formatted content
- Subscription-based access control for premium articles
- Redirection based global error handling
- Unit Tests written in xUnit

## Technologies Used
- .NET Core 8
- ASP.NET MVC
- Entity Framework Core
- FluentValidation
- MediatR
- Moq (for unit testing)
- Typescript (for custom UI Widgets)
- Tailwind

## Directory Structure
The main directories and files in the project are:

- `MVC_News.MVC`: Contains the MVC controllers, views, and models.
- `MVC_News.MVC/wwwroot`: Contains the compiled typescript and SCSS and tailwind code.
- `MVC_News.Application`: Includes the application layer logic, such as commands, queries, and handlers.
- `MVC_News.Domain`: Defines the domain entities and factories.
- `MVC_News.Infrastructure`: Includes the repository implementations and other infrastructure-related code.
- `MVC_News.Tests`: Contains the unit tests for the application.

## Project Setup
```
Go to the "MVC_News.MVC" folder and make a .env file based off of ".env_pattern" with your own database access data
```

## Getting Started
1. Clone the repository:
```
git clone https://github.com/m-7ard/Dotnet-MVC-News-Service.git
```
```
>> dotnet restore
>> cd MVC_News.MVC
>> dotnet watch run
```
```
(in another console)
>> cd MVC_News.MVC/wwwroot
>> npm i
>> npm run dev
```
