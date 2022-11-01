using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthProject.Models
{
    public class OAuthToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("sfdc_community_url")]
        public string SfdcCommunityUrl { get; set; }

        [JsonProperty("sfdc_community_id")]
        public string SfdcCommunityId { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("id_token")]
        public string IDToken { get; set; }

        [JsonProperty("instance_url")]
        public string InstanceUrl { get; set; }

        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("issued_at")]
        public string IssuedAt { get; set; }

        public string Email { get; set; }

        public string SESAID { get; set; }
    }
}
