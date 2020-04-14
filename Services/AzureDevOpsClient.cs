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
           
            // Headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Blazor.MSAL");
            _httpClient.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);

            //Get Organizations - simple
            return await _httpClient.GetJsonAsync<IEnumerable<AzureDevOpsOrganization>>("https://app.vssps.visualstudio.com/_apis/accounts");
            
            //Get Organizations - more errorhandling
           
            HttpResponseMessage response = await _httpClient.GetAsync("https://app.vssps.visualstudio.com/_apis/accounts");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("\tSuccesful REST call");
                var result = response.Content.ReadAsStringAsync().Result;
                //Deserialize result and return
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
                return null;
            }
            else
            {
                Console.WriteLine("{0}:{1}", response.StatusCode, response.ReasonPhrase);
                return null;
            }
        }
    }
}

