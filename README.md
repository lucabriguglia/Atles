# Atles (Alpha)

![.NET Core](https://github.com/lucabriguglia/Atles/workflows/.NET%20Core/badge.svg)

A forum software built with ASP.NET Blazor WebAssembly and CQRS.

**Documentation**: [Atles Wiki](https://lucabriguglia.github.io/Atles).

![Forums Admin](docs/assets/img/admin-forums.png)
![Permission Set Admin](docs/assets/img/admin-permission-set-edit.png)

## Technology

- Blazor WebAssembly 6.0.1
- Entity Framework Core 6.0.1
- SQL Server

## Features

- Themes
- Multi language
- Granular permissions
- Markdown editor
- You can use your own ASP.NET Identity

## Run on local

- Clone the repository
- Run the **Atles.Server** project
- Database and default data will be created automatically
- Login with the default admin account:
  - **Email**: admin@default.com
  - **Password**: !P455w0rd?

**Note**: Please delete any databases previously created if you pull new versions. It's still an Alpha version and some breaking changes might occur between commits.
