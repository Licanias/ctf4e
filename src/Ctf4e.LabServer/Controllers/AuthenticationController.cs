﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ctf4e.Api;
using Ctf4e.Api.Models;
using Ctf4e.Api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ctf4e.LabServer.Constants;
using Ctf4e.LabServer.Options;
using Ctf4e.LabServer.Services;
using Ctf4e.Utilities;
using Microsoft.AspNetCore.Http;

namespace Ctf4e.LabServer.Controllers
{
    [Route("auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ICryptoService _cryptoService;
        private readonly IStateService _stateService;

        public AuthenticationController(ICryptoService cryptoService, IStateService stateService, IOptionsSnapshot<LabOptions> labOptions)
            : base("~/Views/Authentication.cshtml", labOptions)
        {
            _cryptoService = cryptoService ?? throw new ArgumentNullException(nameof(cryptoService));
            _stateService = stateService ?? throw new ArgumentNullException(nameof(stateService));
        }

        private IActionResult Render(ViewType viewType, object model = null)
        {
            ViewData["ViewType"] = viewType;
            return RenderView(MenuItems.Authentication, model);
        }

        [HttpGet]
        public IActionResult Render()
        {
            // Logged in?
            var currentUser = GetCurrentUser();
            if(currentUser == null)
            {
                AddStatusMessage("Der Zugriff ist nur über das CTF-System möglich.", StatusMessageTypes.Info);
                return Render(ViewType.Blank);
            }

            return Render(ViewType.Redirect);
        }

#if DEBUG
        [HttpGet("login/dev/")]
        public async Task<IActionResult> DevLoginAsync(int userId, string userName, int? groupId, string groupName, bool admin)
        {
            // Already logged in?
            var currentUser = GetCurrentUser();
            if(currentUser != null)
                return Render(ViewType.Redirect);

            // Make sure user account exists
            await _stateService.ProcessLoginAsync(userId, groupId);

            // Sign in user
            await DoLoginAsync(userId, userName, groupId, groupName, admin);

            // Done
            AddStatusMessage("Login erfolgreich!", StatusMessageTypes.Success);
            return Render(ViewType.Redirect);
        }
#endif

        [HttpGet("login")]
        public async Task<IActionResult> LoginAsync(string code)
        {
            // Already logged in?
            var currentUser = GetCurrentUser();
            if(currentUser != null)
                return Render(ViewType.Redirect);

            // Parse and check request
            var loginData = UserLoginRequest.Deserialize(_cryptoService.Decrypt(code));

            // Make sure user account exists
            await _stateService.ProcessLoginAsync(loginData.UserId, loginData.GroupId);

            // Sign in user
            await DoLoginAsync(loginData.UserId, loginData.UserDisplayName, loginData.GroupId, loginData.GroupName, loginData.AdminMode);

            // Done
            AddStatusMessage("Login erfolgreich!", StatusMessageTypes.Success);
            return Render(ViewType.Redirect);
        }

        private async Task DoLoginAsync(int userId, string userDisplayName, int? groupId, string groupName, bool adminMode)
        {
            // Prepare session data to identify user
            var claims = new List<Claim>
            {
                new Claim(AuthenticationStrings.ClaimUserId, userId.ToString()),
                new Claim(AuthenticationStrings.ClaimUserDisplayName, userDisplayName),
                new Claim(AuthenticationStrings.ClaimGroupId, groupId.ToString()),
                new Claim(AuthenticationStrings.ClaimGroupName, groupName)
            };
            if(adminMode)
                claims.Add(new Claim(AuthenticationStrings.ClaimAdminMode, true.ToString()));

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true
            };

            // Login
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);

            // Make sure the current user is set correctly
            HandleUserLogin(userId, userDisplayName, groupId, groupName, adminMode);
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
            return Render(ViewType.Blank);
        }

        public enum ViewType
        {
            Blank,
            Redirect
        }
    }
}