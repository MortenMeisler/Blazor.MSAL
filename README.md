# Blazor MSAL with Azure DevOps API
This is a sample project showing how to configure MSAL Authentication in Blazor WebAssembly against Azure DevOps API

## Setup
This project is based on the Microsoft Doc [Secure an ASP.NET Core Blazor WebAssembly standalone app with Azure Active Directory](https://docs.microsoft.com/en-us/aspnet/core/security/blazor/webassembly/standalone-with-azure-active-directory?view=aspnetcore-3.1)

What is added is code showing how to use the same access token from the MSAL flow to consume Azure DevOps API. 

The important part from `AzureDevOpsClient.cs`:
```
var tokenResult = await _authenticationService.RequestAccessToken(
           new AccessTokenRequestOptions
           {
               //Azure Devops default scope - constant, do not change.
               Scopes = new[] {
                    "499b84ac-1321-427f-aa17-267ca6975798/.default",
            }
           });
```

### Added in Azure App Registration to test:

![alt text](https://github.com/MortenMeisler/Blazor.MSAL/blob/master/appreg.png)

- Authentication redirect URis: https://localhost:5001
- Implicit grant: Access Token, ID Tokens
- Treat application as a public client: Yes


### Appsettings.json:
Make sure to replace with your client and tenant id in appsettings.json (located in the wwwroot folder)

```
{
  "AzureAd": {
    "Authority": "https://login.microsoftonline.com/{yourtenantid}",
    "ClientId": "{yourclientid}",
    "ValidateAuthority": true
  }
}
```






