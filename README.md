# Atles (Beta)

![.NET Core](https://github.com/lucabriguglia/Atles/workflows/.NET%20Core/badge.svg)

A forum software built with ASP.NET Core Blazor WebAssembly.

**Documentation**: [Atles Wiki](https://lucabriguglia.github.io/Atles).

![Forums Admin](docs/assets/img/admin-forums.png)
![Permission Set Admin](docs/assets/img/admin-permission-set-edit.png)

## Technology

- Blazor WebAssembly 3.2.0
- Entity Framework Core 3.1
- SQL Server _(more to come)_

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

**Note**: Please delete any databases previously created if you pull new versions. It's still a beta and some breaking changes might occur between commits.
