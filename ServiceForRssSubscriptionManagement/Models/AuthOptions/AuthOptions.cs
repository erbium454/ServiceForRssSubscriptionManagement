using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ServiceForRssSubscriptionManagement.Models.AuthOptions
{
    public class AuthOptions
    {
        public const string ISSUER = "FeedService";
        public const string AUDIENCE = "FeedService Audience";
        public const int LIFETIMEHOURS = 24;

        const string KEY = "Custom_Key_wefregd34gif34gewry48rh4nr43f0i34jgfm3";
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}