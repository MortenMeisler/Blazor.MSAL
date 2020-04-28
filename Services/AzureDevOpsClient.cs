using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blazor.MSAL.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Text.Json;

namespace Blazor.MSAL.Services
{
    public class AzureDevOpsClient
    {
        private readonly HttpClient _httpClient;
        private readonly IAccessTokenProvider _authenticationService;

        public AzureDevOpsClient(HttpClient httpClient, IAccessTokenProvider authenticationService)
        {
            _httpClient = httpClient;
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Get Bearer token to be used for retrieving Azure DevOps resources. The token call is pretty standard, so if you have other 
        /// API's other than DevOps, you could put this or part of this in a common method to be used by other clients.
        /// </summary>
        /// <returns></returns>
        public async Task<AccessToken> GetDevOpsToken()
        {
           
            var tokenResult = await _authenticationService.RequestAccessToken(
           new AccessTokenRequestOptions
           {
               //Azure Devops default scope - constant, do not change.
               Scopes = new[] {
                    "499b84ac-1321-427f-aa17-267ca6975798/.default",
            }
           });

            if (tokenResult.TryGetToken(out var token))
            {
               
                if (token == null)
                {
                    //Issue when running in debug: https://github.com/dotnet/aspnetcore/issues/19998
                    Console.WriteLine("How can this be null");
                    //It works anyway, called again.

                }
                
                return token;
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<AzureDevOpsOrganization>> ListAzureDevOpsOrganizations()
        {
            var token = await GetDevOpsToken();

           
            var request = new HttpRequestMessage(HttpMethod.Get, "https://app.vssps.visualstudio.com/_apis/accounts");
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.TryAddWithoutValidation("User-Agent", "Blazor.MSAL");
            request.Headers.TryAddWithoutValidation("X-TFS-FedAuthRedirect", "Suppress");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
            {
                
                try
                {
                    return await response.Content.ReadFromJsonAsync<IEnumerable<AzureDevOpsOrganization>>();
                }
                catch (NotSupportedException ex) // When content type is not valid
                {
                    Console.WriteLine($"The content type is not supported. Exception: {ex.Message}");
                }
                catch (JsonException ex) // Invalid JSON
                {
                    Console.WriteLine($"Invalid JSON. Exception: {ex.Message}");
                }
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Console.WriteLine($"Unauthorized: {response.ReasonPhrase}");
                throw new UnauthorizedAccessException(response.ReasonPhrase);
                
            }

            return null;
        }
            
        
    }
}

