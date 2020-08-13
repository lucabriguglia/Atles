# Deployment

Please follow these instructions in order to deploy Atlas to a web server.

- [Certificate](#certificate)
- [Mail Settings](#mail-settings)
- [Connection Strings](#connection-strings)
- [Default Users](#default-users)
- [web.config](#web-config)

<a name="certificate"></a>
## Certificate

[This tutorial](https://benjii.me/2017/06/creating-self-signed-certificate-identity-server-azure/) explains how to create a self-signed certicate for Identity Server and use it on Azure App Service.

App settings configuration for Identity Server should then be modified from this:

```
"IdentityServer": {
    "Clients": {
        "Atlas.Client": {
        "Profile": "IdentityServerSPA"
        }
    },
},
```
to this:

```
"IdentityServer": {
    "Clients": {
        "Atlas.Client": {
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
## Mail Settings

In order for the Atlas to work properly you need to configure a SMTP provider in app settings.

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
## Connection Strings

You need to specify two connection strings: `ApplicationConnection` and `AtlasConnection` in app settings.

```
"ConnectionStrings": {
    "ApplicationConnection": "Server=(localdb)\\mssqllocaldb;Database=Atlas;Trusted_Connection=True;MultipleActiveResultSets=true",
    "AtlasConnection": "Server=(localdb)\\mssqllocaldb;Database=Atlas;Trusted_Connection=True;MultipleActiveResultSets=true"
},
```

`ApplicationConnection` is used for ASP.NET Identity data while `AtlasConnection` is used for Atlas data.
The two connection strings can be the same or different depending on whether you want membership data on a separate database or you want to use existing membership data.
In case you are going to use an existing ASP.NET Identity database, the Atlas member profile will be created the first time the user logs in.

<a name="default-users"></a>
## Default Users

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
## web.config

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
      <aspNetCore processPath="dotnet" arguments=".\Atlas.Server.dll" stdoutLogEnabled="false" stdoutLogFile="\\?\%home%\LogFiles\stdout" hostingModel="OutOfProcess" />
    </system.webServer>
  </location>
</configuration>
```