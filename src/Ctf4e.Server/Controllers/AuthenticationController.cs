﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ctf4e.Server.Constants;
using Ctf4e.Server.Models;
using Ctf4e.Server.Options;
using Ctf4e.Server.Services;
using Ctf4e.Server.ViewModels;
using Ctf4e.Utilities;
using Ctf4e.Utilities.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoodleLti;
using MoodleLti.Options;

namespace Ctf4e.Server.Controllers
{
    [Route("auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISlotService _slotService;
        private readonly IOptions<MoodleLtiOptions> _moodleLtiOptions;
        private readonly IConfigurationService _configurationService;

        public AuthenticationController(IUserService userService, IOptions<MainOptions> mainOptions, ISlotService slotService, IOptions<MoodleLtiOptions> moodleLtiOptions, IConfigurationService configurationService)
            : base(userService, mainOptions, "~/Views/Authentication.cshtml")
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _slotService = slotService ?? throw new ArgumentNullException(nameof(slotService));
            _moodleLtiOptions = moodleLtiOptions ?? throw new ArgumentNullException(nameof(moodleLtiOptions));
            _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        }

        private Task<IActionResult> RenderAsync(ViewType viewType, object model = null)
        {
            ViewData["ViewType"] = viewType;
            return RenderViewAsync(MenuItems.Authentication, model);
        }

        [HttpGet]
        public async Task<IActionResult> RenderAsync()
        {
            // Logged in?
            var currentUser = await GetCurrentUserAsync();
            if(currentUser == null)
            {
                AddStatusMessage("Ein Login ist nur über den Moodlekurs möglich.", StatusMessageTypes.Info);
                return await RenderAsync(ViewType.Blank);
            }

            // Assigned to group?
            if(currentUser.Group == null)
                return this.RedirectToAction<AuthenticationController>(nameof(ShowGroupFormAsync)); // Cookie is already set, so redirection is safe
            return await RenderAsync(ViewType.Redirect);
        }

#if DEBUG
        [HttpGet("dev/login")]
        public async Task<IActionResult> DevLoginAsync(int userId)
        {
            // Already logged in?
            var currentUser = await GetCurrentUserAsync();
            if(currentUser != null)
                return await RenderAsync(ViewType.Redirect);

            // Find user
            var user = await _userService.GetUserAsync(userId, HttpContext.RequestAborted);
            if(user == null)
            {
                AddStatusMessage("Dieser Benutzer existiert nicht.", StatusMessageTypes.Error);
                return await RenderAsync(ViewType.Blank);
            }

            // Sign in user
            await DoLoginAsync(user);

            // Done
            AddStatusMessage("Login erfolgreich!", StatusMessageTypes.Success);
            if(user.Group == null)
                return await ShowGroupFormAsync();
            return await RenderAsync(ViewType.Redirect);
        }
#endif

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync()
        {
            // Already logged in?
            var currentUser = await GetCurrentUserAsync();
            if(currentUser != null)
                return await RenderAsync(ViewType.Redirect);

            // Parse and check request
            var authData = await MoodleAuthenticationTools.ParseAuthenticationRequestAsync
            (
                Request,
                _moodleLtiOptions.Value.OAuthConsumerKey,
                _moodleLtiOptions.Value.OAuthSharedSecret
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

        [HttpGet("loginas")]
        [Authorize(Policy = AuthenticationStrings.PolicyIsAdmin)]
        public async Task<IActionResult> AdminLoginAsUserAsync(int id)
        {
            // Try to retrieve user
            var user = await _userService.GetUserAsync(id, HttpContext.RequestAborted);
            if(user == null)
            {
                AddStatusMessage("Dieser Benutzer existiert nicht.", StatusMessageTypes.Error);
                return await RenderAsync(ViewType.Blank);
            }

            // Sign in again, as given user
            await DoLoginAsync(user);

            // Done
            AddStatusMessage($"Login als {user.DisplayName} erfolgreich!", StatusMessageTypes.Success);
            if(user.Group == null)
                return await ShowGroupFormAsync();
            return await RenderAsync(ViewType.Redirect);
        }

        private async Task DoLoginAsync(User user)
        {
            // Prepare session data to identify user
            var claims = new List<Claim>
            {
                new Claim(AuthenticationStrings.ClaimUserId, user.Id.ToString())
            };
            if(user.IsAdmin)
                claims.Add(new Claim(AuthenticationStrings.ClaimIsAdmin, true.ToString()));
            if(user.IsTutor)
                claims.Add(new Claim(AuthenticationStrings.ClaimIsTutor, true.ToString()));
            if(user.GroupId != null)
                claims.Add(new Claim(AuthenticationStrings.ClaimIsGroupMember, true.ToString()));

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true
            };

            // Login
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);

            // Make sure the current user is set correctly
            await HandleUserLoginAsync(user.Id);
        }

        [HttpGet("logout")]
        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            // Logout
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Make sure the current user is set correctly
            HandleUserLogout();

            // Done
            AddStatusMessage("Logout erfolgreich.", StatusMessageTypes.Success);
            return await RenderAsync(ViewType.Blank);
        }

        private async Task<IActionResult> ShowGroupFormAsync(GroupSelection groupSelection)
        {
            // Does the user already have a group?
            var currentUser = await GetCurrentUserAsync();
            if(currentUser.Group != null)
                return this.RedirectToAction<ScoreboardController>(nameof(ScoreboardController.RenderScoreboardAsync));

            // Do group splits?
            ViewData["CreateSplitGroups"] = await _configurationService.GetCreateSplitGroupsAsync(HttpContext.RequestAborted);

            // Pass slots
            ViewData["Slots"] = await _slotService.GetSlotsAsync().ToListAsync();

            return await RenderAsync(ViewType.GroupSelection, groupSelection);
        }

        [HttpGet("selgroup")]
        [Authorize]
        public Task<IActionResult> ShowGroupFormAsync()
        {
            return ShowGroupFormAsync(null);
        }

        [HttpPost("selgroup")]
        [Authorize]
        public async Task<IActionResult> HandleGroupSelectionAsync(GroupSelection groupSelection)
        {
            // Some input validation
            if(!ModelState.IsValid)
            {
                AddStatusMessage("Ungültige Eingabe.", StatusMessageTypes.Error);
                return await ShowGroupFormAsync(groupSelection);
            }

            // Try to create group
            // Error checking is done by service method
            var currentUser = await GetCurrentUserAsync();
            try
            {
                var group = new Group
                {
                    DisplayName = groupSelection.DisplayName,
                    SlotId = groupSelection.SlotId,
                    ShowInScoreboard = groupSelection.ShowInScoreboard
                };
                bool createSplitGroups = await _configurationService.GetCreateSplitGroupsAsync(HttpContext.RequestAborted);
                await _userService.CreateGroupAsync(group, createSplitGroups, currentUser.GroupFindingCode, groupSelection.OtherUserCode, HttpContext.RequestAborted);
            }
            catch(ArgumentException)
            {
                AddStatusMessage("Ungültige Eingabe.", StatusMessageTypes.Error);
                return await ShowGroupFormAsync(groupSelection);
            }
            catch(InvalidOperationException)
            {
                AddStatusMessage("Der dem eingegebenen Code zugehörige Benutzer ist bereits einer Gruppe zugewiesen.", StatusMessageTypes.Error);
                return await ShowGroupFormAsync(groupSelection);
            }
            catch
            {
                // Should only happen on larger database failures or when users mess around with the input model
                AddStatusMessage("Ein Fehler ist aufgetreten.", StatusMessageTypes.Error);
                return await ShowGroupFormAsync(groupSelection);
            }

            // Success
            AddStatusMessage("Gruppeneintragung erfolgreich.", StatusMessageTypes.Success);
            AddStatusMessage("Bitte loggen Sie sich erneut über den Moodlekurs ein, um die Registrierung abzuschließen.", StatusMessageTypes.Info);
            return await LogoutAsync();
        }

        public enum ViewType
        {
            Blank,
            GroupSelection,
            Redirect
        }
    }
}
