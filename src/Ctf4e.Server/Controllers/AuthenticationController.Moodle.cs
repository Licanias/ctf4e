using System.Threading.Tasks;
using Ctf4e.Server.Models;
using Ctf4e.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoodleLti;
using MoodleLti.Options;

namespace Ctf4e.Server.Controllers
{
    public partial class AuthenticationController
    {
        [HttpPost("login/moodle")]
        public async Task<IActionResult> LoginMoodleAsync([FromServices] IOptions<MoodleLtiOptions> moodleLtiOptions)
        {
            // Already logged in?
            var currentUser = await GetCurrentUserAsync();
            if(currentUser != null)
                return await RenderAsync(ViewType.Redirect);

            // Parse and check request
            var authData = await MoodleAuthenticationTools.ParseAuthenticationRequestAsync
            (
                Request,
                moodleLtiOptions.Value.OAuthConsumerKey,
                moodleLtiOptions.Value.OAuthSharedSecret
            );

            // Does the user exist already?
            var user = await _userService.FindUserByMoodleUserIdAsync(authData.UserId, HttpContext.RequestAborted);
            if(user == null)
            {
                var newUser = new User
                {
                    DisplayName = authData.FullName,
                    MoodleUserId = authData.UserId,
                    MoodleName = authData.LoginName,
                    GroupFindingCode = RandomStringGenerator.GetRandomString(10),
                    IsAdmin = !await _userService.AnyUsers(HttpContext.RequestAborted)
                };
                user = await _userService.CreateUserAsync(newUser, HttpContext.RequestAborted);
                AddStatusMessage("Account erfolgreich erstellt!", StatusMessageTypes.Success);
            }

            // Sign in user
            await DoLoginAsync(user);

            // Done
            AddStatusMessage("Login erfolgreich!", StatusMessageTypes.Success);
            if(user.Group == null)
                return await ShowGroupFormAsync();
            return await RenderAsync(ViewType.Redirect);
        }
    }
}