## Deployment

Please follow these instructions in order to deploy Atles to a web server.
The target framework is **.NET Core 3.1**.

- [Certificate](#certificate)
- [Mail Settings](#mail-settings)
- [Connection Strings](#connection-strings)
- [Other App Settings](#other-app-strings)
- [Default Users](#default-users)
- [web.config](#web-config)

<a name="certificate"></a>
### Certificate

[This tutorial](https://benjii.me/2017/06/creating-self-signed-certificate-identity-server-azure/) explains how to create a self-signed certicate for Identity Server and use it on Azure App Service.

App settings configuration for Identity Server should then be modified from this:

```
"IdentityServer": {
    "Clients": {
        "Atles.Client": {
        "Profile": "IdentityServerSPA"
        }
    },
},
```
to this:

```
"IdentityServer": {
    "Clients": {
        "Atles.Client": {
        "Profile": "IdentityServerSPA"
        }
    },
    "Key": {
        "Type": "Store",
        "StoreName": "My",
        "StoreLocation": "CurrentUser",
        "Name": "CN=example.com"
    }
},
```

<a name="mail-settings"></a>
### Mail Settings

In order for Atles to work properly and emails to users (including confirming their email address after registration) you need to configure a SMTP provider in app settings.

```
"MailSettings": {
    "Mail": "<fromemail>",
    "DisplayName": "<displayname>",
    "Password": "<yourpasswordhere>",
    "Host": "smtp.gmail.com",
    "Port": 587
},
```

If you want to use Gmail as your SMTP provider, please follow the instructions in [here](https://support.google.com/accounts/answer/185833?hl=en).

<a name="connection-strings"></a>
### Connection Strings

You need to specify two connection strings: `ApplicationConnection` and `AtlesConnection` in app settings.

```
"ConnectionStrings": {
    "ApplicationConnection": "Server=(localdb)\\mssqllocaldb;Database=Atles;Trusted_Connection=True;MultipleActiveResultSets=true",
    "AtlesConnection": "Server=(localdb)\\mssqllocaldb;Database=Atles;Trusted_Connection=True;MultipleActiveResultSets=true"
},
```

`ApplicationConnection` is used for ASP.NET Identity data while `AtlesConnection` is used for Atles data.
The two connection strings can be the same or different depending on whether you want membership data on a separate database or you want to use an existing database with membership data.
In case you are going to use an existing ASP.NET Identity database, the Atles member profile will be created the first time a user logs in.
To migrate your datbase(s) you can use .NET CLI to apply the migrations or run the sql scripts included in the sql folder.

<a name="other-app-strings"></a>
### Other App Settings

- `MigrateDatabases` should be set to `false` in a production environment.
- `EnsureDefaultSiteInitialized` should be set to `true` only for the first time the application runs in production. This setting should be set to `false` after that.

<a name="default-users"></a>
### Default Users

The only default user that needs to be created is the admin user. 
Please specify email address, user name, display name and password in app settings.

```
"DefaultAdminUserEmail": "admin@default.com",
"DefaultAdminUserName": "admin@default.com",
"DefaultAdminUserDisplayName": "Admin",
"DefaultAdminUserPassword": "!P455w0rd?",
"CreateDefaultNormalUser": "true",
"DefaultNormalUserEmail": "user@default.com",
"DefaultNormalUserName": "user@default.com",
"DefaultNormalUserDisplayName": "User",
"DefaultNormalUserPassword": "!P455w0rd?",
"CreateDefaultModeratorUser": "true",
"DefaultModeratorUserEmail": "moderator@default.com",
"DefaultModeratorUserName": "moderator@default.com",
"DefaultModeratorUserDisplayName": "Moderator",
"DefaultModeratorUserPassword": "!P455w0rd?",
```

Remember to change the default password after the first login.
The other two users (Normal User and Moderator User) are optional and the creation can be disabled by setting `CreateDefaultNormalUser` and `CreateDefaultModeratorUser` to false.

<a name="web-config"></a>
### web.config

Make sure that in the generated `web.config` the value for `hostingModel` is set to `OutOfProcess`.

The web.config should look similar to the following:

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModule" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" arguments=".\Atles.Server.dll" stdoutLogEnabled="false" stdoutLogFile="\\?\%home%\LogFiles\stdout" hostingModel="OutOfProcess" />
    </system.webServer>
  </location>
</configuration>
```
