﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Ctf4e.Server.Constants;
using Ctf4e.Server.Options;
using Ctf4e.Server.Services;
using Ctf4e.Server.ViewModels;
using Ctf4e.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Ctf4e.Server.Controllers
{
    [Route("admin/config")]
    [Authorize(Policy = AuthenticationStrings.PolicyIsAdmin)]
    public class AdminConfigurationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfigurationService _configurationService;

        public AdminConfigurationController(IUserService userService, IOptions<MainOptions> mainOptions, IConfigurationService configurationService)
            : base(userService, mainOptions, "~/Views/AdminConfiguration.cshtml")
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        }

        private async Task<IActionResult> RenderAsync(AdminConfigurationData configurationData)
        {
            var config = configurationData ?? new AdminConfigurationData
            {
                FlagHalfPointsSubmissionCount = await _configurationService.GetFlagHalfPointsSubmissionCountAsync(HttpContext.RequestAborted),
                FlagMinimumPointsDivisor = await _configurationService.GetFlagMinimumPointsDivisorAsync(HttpContext.RequestAborted),
                ScoreboardEntryCount = await _configurationService.GetScoreboardEntryCountAsync(HttpContext.RequestAborted),
                ScoreboardCachedSeconds = await _configurationService.GetScoreboardCachedSecondsAsync(HttpContext.RequestAborted),
                PassAsGroup = await _configurationService.GetPassAsGroupAsync(HttpContext.RequestAborted),
                PageTitle = await _configurationService.GetPageTitleAsync(HttpContext.RequestAborted),
                NavbarTitle = await _configurationService.GetNavbarTitleAsync(HttpContext.RequestAborted),
                FlagPrefix=await _configurationService.GetFlagPrefixAsync(HttpContext.RequestAborted),
                FlagSuffix=await _configurationService.GetFlagSuffixAsync(HttpContext.RequestAborted)
            };

            int groupCount = await _userService.GetGroupsAsync().CountAsync(HttpContext.RequestAborted);
            ViewData["GroupCount"] = groupCount;

            return await RenderViewAsync(MenuItems.AdminConfiguration, config);
        }

        [HttpGet]
        public Task<IActionResult> RenderAsync()
        {
            return RenderAsync(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateConfigAsync(AdminConfigurationData configurationData)
        {
            if(!ModelState.IsValid)
            {
                AddStatusMessage("Ungültige Eingabe.", StatusMessageTypes.Error);
                return await RenderAsync(configurationData);
            }

            try
            {
                // Update configuration
                await _configurationService.SetFlagHalfPointsSubmissionCountAsync(configurationData.FlagHalfPointsSubmissionCount, HttpContext.RequestAborted);
                await _configurationService.SetFlagMinimumPointsDivisorAsync(configurationData.FlagMinimumPointsDivisor, HttpContext.RequestAborted);
                await _configurationService.SetScoreboardEntryCountAsync(configurationData.ScoreboardEntryCount, HttpContext.RequestAborted);
                await _configurationService.SetScoreboardCachedSecondsAsync(configurationData.ScoreboardCachedSeconds, HttpContext.RequestAborted);
                await _configurationService.SetPassAsGroupAsync(configurationData.PassAsGroup, HttpContext.RequestAborted);
                await _configurationService.SetPageTitleAsync(configurationData.PageTitle, HttpContext.RequestAborted);
                await _configurationService.SetNavbarTitleAsync(configurationData.NavbarTitle, HttpContext.RequestAborted);
                await _configurationService.SetFlagPrefixAsync(configurationData.FlagPrefix, HttpContext.RequestAborted);
                await _configurationService.SetFlagSuffixAsync(configurationData.FlagSuffix, HttpContext.RequestAborted);

                AddStatusMessage("Die Konfiguration wurde erfolgreich aktualisiert.", StatusMessageTypes.Success);
            }
            catch(Exception ex)
            {
                AddStatusMessage("Fehler: " + ex.Message, StatusMessageTypes.Error);
                return await RenderAsync(configurationData);
            }

            return await RenderAsync(null);
        }
    }
}