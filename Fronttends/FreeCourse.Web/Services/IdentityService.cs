using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;

namespace FreeCourse.Web.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClientSettings _clientSettings; 
        private readonly ServiceApiSettings _serviceApiSettings;

        public IdentityService(HttpClient client, IHttpContextAccessor httpContextAccessor, IOptions<ClientSettings> clientSettings, IOptions<ServiceApiSettings> serviceApiSettings)
        {
            _httpClient = client;
            _httpContextAccessor = httpContextAccessor;
            // todo : bunları program cs de singleton olarak da ekleyebilirik
            _clientSettings = clientSettings.Value;
            _serviceApiSettings = serviceApiSettings.Value;
        }

        public async Task<TokenResponse> GetAccessTokenByRefreshToken()
        {
            throw new NotImplementedException();
        }

        public async Task RevokeRefreshToken()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// bu method identiy servera istek yapıp
        /// token ve user bilgilerini mvc tarafına dönderir
        /// </summary>
        /// <param name="signInInput"></param>
        /// <returns></returns>
        public async Task<Response<bool>> SignIn(SignInInput signInInput)
        {
            var discoveryEndPoints = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
            {
                Address=_serviceApiSettings.BaseUri,
                Policy=new DiscoveryPolicy() { RequireHttps = false }
            });

            if (discoveryEndPoints.IsError)
            {
                throw discoveryEndPoints.Exception;
            }

            var passwordTokenRequest = new PasswordTokenRequest()
            {
                ClientId = _clientSettings.WebClientForUsers.ClientId,
                ClientSecret = _clientSettings.WebClientForUsers.ClientSecret,
                UserName = signInInput.Email,
                Password = signInInput.Password,
                Address = discoveryEndPoints.TokenEndpoint
            };

            var token = await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest);
            if (token.IsError)
            {
                var responseContent = await token.HttpResponse.Content.ReadAsStringAsync();
                var errorDto = JsonSerializer.Deserialize<ErrodDto>(responseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                return Response<bool>.Fail(errorDto.Errors, 404);
            }

            var userInfoREquest = new UserInfoRequest()
            {
                Token = token.AccessToken,
                Address = discoveryEndPoints.UserInfoEndpoint,
            };

            var userInfo = await _httpClient.GetUserInfoAsync(userInfoREquest);

            if (userInfo.IsError)
            {
                throw userInfo.Exception;
            }

            #region Cookie
            //burası çok önemli
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(userInfo.Claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", "role");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            AuthenticationProperties authenticationProperties = new AuthenticationProperties();

            authenticationProperties.StoreTokens(new List<AuthenticationToken>()
            {
                new AuthenticationToken(){ Name = OpenIdConnectParameterNames.AccessToken, Value = token.AccessToken},
                new AuthenticationToken(){ Name = OpenIdConnectParameterNames.RefreshToken, Value = token.RefreshToken},
                new AuthenticationToken(){ Name = OpenIdConnectParameterNames.ExpiresIn, Value= DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture)}
            });

            authenticationProperties.IsPersistent = signInInput.IsRemember;
            #endregion

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authenticationProperties);

            return Response<bool>.Success(200);
        }
    }
}
