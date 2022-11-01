using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OAuthProject.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OAuthProject.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : Controller
    {

        private readonly IConfiguration _configuration;
        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("Index")]
        public ActionResult Index()
        {
            ActionResult returnResult = GetInvalidResult();

            string code = Request.Query["code"];
            if (code != null)
            {
                GetUserDetailsAsync(code);
                var redirectURI = _configuration["SingleSignOn:Redirect_uri"];

                returnResult = Redirect(redirectURI);
            }
            return returnResult;
        }

        public async Task GetUserDetailsAsync(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                HttpResponseMessage response = GetResponseFromGetOAuthTokenURL(code);
                OAuthToken oAuthToken = new OAuthToken();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    oAuthToken = JsonConvert.DeserializeObject<OAuthToken>(responseBody);

                    List<string> decodedIDToken = DecodeIDToken(oAuthToken);

                    string accessToken = decodedIDToken[0];
                    string refreshToken = decodedIDToken[1];
                    string email = decodedIDToken[2];
                }
            }
        }

        private List<string> DecodeIDToken(OAuthToken oAuthToken)
        {
            string accessToken = oAuthToken.AccessToken;
            string refreshToken = oAuthToken.RefreshToken;

            string stream = oAuthToken.IDToken;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;

            string email = tokenS.Claims.First(claim => claim.Type == "email").Value;

            List<string> decodedList = new List<string>();
            decodedList.Add(accessToken);
            decodedList.Add(refreshToken);
            decodedList.Add(email);

            return decodedList;
        }


        private HttpResponseMessage GetResponseFromGetOAuthTokenURL(string code)
        {
            var url = _configuration["SingleSignOn:Access_Token_Url"];

            var grantType = _configuration["SingleSignOn:GrantType"];
            var clientID = _configuration["SingleSignOn:ClientId"];
            var clientSecret = _configuration["SingleSignOn:ClientSecret"];
            var redirectURI = _configuration["SingleSignOn:Redirect_uri"];

            var parameters = new Dictionary<string, string> { { "grant_type", grantType }, { "client_id", clientID }, { "client_secret", clientSecret }, { "code", code }, { "redirect_uri", redirectURI } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            HttpClient client = new HttpClient();

            HttpResponseMessage response = client.PostAsync(url, encodedContent).Result;
            return response;
        }

        ActionResult GetInvalidResult()
        {
            string redirectPage = "LogoutRedirect";
            return View(redirectPage);
        }
    }
}
