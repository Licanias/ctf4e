﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ctf4e.Api;
using Ctf4e.Api.Models;
using Ctf4e.Api.Services;
using Ctf4e.Server.Constants;
using Ctf4e.Server.Models;
using Ctf4e.Server.Options;
using Ctf4e.Server.Services;
using Ctf4e.Server.Services.Sync;
using Ctf4e.Server.ViewModels;
using Ctf4e.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// TODO cleanup dependencies (too many rarely used services)
namespace Ctf4e.Server.Controllers
{
    [Route("admin/scoreboard")]
    [Authorize(Policy = AuthenticationStrings.PolicyIsPrivileged)]
    public class AdminScoreboardController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IScoreboardService _scoreboardService;
        private readonly IExerciseService _exerciseService;
        private readonly IFlagService _flagService;
        private readonly ILabService _labService;
        private readonly IMoodleService _moodleService;
        private readonly ICsvService _csvService;
        private readonly ILabExecutionService _labExecutionService;

        public AdminScoreboardController(IUserService userService, IOptions<MainOptions> mainOptions, IScoreboardService scoreboardService, IExerciseService exerciseService,
                                         IFlagService flagService, ILabService labService, IMoodleService moodleService, ICsvService csvService,
                                         ILabExecutionService labExecutionService)
            : base("~/Views/AdminScoreboard.cshtml", userService, mainOptions)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _scoreboardService = scoreboardService ?? throw new ArgumentNullException(nameof(scoreboardService));
            _exerciseService = exerciseService ?? throw new ArgumentNullException(nameof(exerciseService));
            _flagService = flagService ?? throw new ArgumentNullException(nameof(flagService));
            _labService = labService ?? throw new ArgumentNullException(nameof(labService));
            _moodleService = moodleService ?? throw new ArgumentNullException(nameof(moodleService));
            _csvService = csvService ?? throw new ArgumentNullException(nameof(csvService));
            _labExecutionService = labExecutionService ?? throw new ArgumentNullException(nameof(labExecutionService));
        }

        private async Task<IActionResult> RenderAsync(int labId, int slotId)
        {
            var scoreboard = await _scoreboardService.GetAdminScoreboardAsync(labId, slotId, HttpContext.RequestAborted);

            return await RenderViewAsync(MenuItems.AdminScoreboard, scoreboard);
        }

        [HttpGet]
        public async Task<IActionResult> RenderScoreboardAsync(int? labId, int? slotId)
        {
            if(labId == null || slotId == null)
            {
                // Show the most recently executed lab and slot as default
                var recentLabExecution = await _labExecutionService.GetMostRecentLabExecutionAsync(HttpContext.RequestAborted);
                if(recentLabExecution != null)
                {
                    labId = recentLabExecution.LabId;
                    slotId = recentLabExecution.Group?.SlotId;
                }
            }

            return await RenderAsync(labId ?? 0, slotId ?? 0);
        }

        [HttpPost("exercisesubmission/delete")]
        [Authorize(Policy = AuthenticationStrings.PolicyIsAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExerciseSubmissionAsync(int labId, int slotId, int submissionId)
        {
            try
            {
                // Delete submission
                await _exerciseService.DeleteExerciseSubmissionAsync(submissionId, HttpContext.RequestAborted);

                AddStatusMessage("Die Aufgabeneinreichung wurde erfolgreich gelöscht.", StatusMessageTypes.Success);
            }
            catch(Exception ex)
            {
                AddStatusMessage(ex.ToString(), StatusMessageTypes.Error);
            }

            return await RenderAsync(labId, slotId);
        }

        [HttpPost("exercisesubmission/deletemultiple")]
        [Authorize(Policy = AuthenticationStrings.PolicyIsAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExerciseSubmissionsAsync(int labId, int slotId, List<int> submissionIds)
        {
            try
            {
                // Delete submissions
                await _exerciseService.DeleteExerciseSubmissionsAsync(submissionIds, HttpContext.RequestAborted);

                AddStatusMessage("Die Aufgabeneinreichungen wurden erfolgreich gelöscht.", StatusMessageTypes.Success);
            }
            catch(Exception ex)
            {
                AddStatusMessage(ex.ToString(), StatusMessageTypes.Error);
            }

            return await RenderAsync(labId, slotId);
        }

        [HttpPost("exercisesubmission/create")]
        [Authorize(Policy = AuthenticationStrings.PolicyIsAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExerciseSubmissionAsync(int labId, int slotId, int exerciseId, int userId, DateTime submissionTime, bool passed, int weight)
        {
            try
            {
                // Create submission
                var submission = new ExerciseSubmission
                {
                    ExerciseId = exerciseId,
                    UserId = userId,
                    ExercisePassed = passed,
                    SubmissionTime = submissionTime,
                    Weight = passed ? 1 : weight
                };
                await _exerciseService.CreateExerciseSubmissionAsync(submission, HttpContext.RequestAborted);

                AddStatusMessage("Die Aufgabeneinreichung wurde erfolgreich erstellt.", StatusMessageTypes.Success);
            }
            catch(InvalidOperationException ex)
            {
                AddStatusMessage(ex.Message, StatusMessageTypes.Error);
            }

            return await RenderAsync(labId, slotId);
        }

        [HttpPost("flagsubmission/delete")]
        [Authorize(Policy = AuthenticationStrings.PolicyIsAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFlagSubmissionAsync(int labId, int slotId, int userId, int flagId)
        {
            try
            {
                // Delete submission
                await _flagService.DeleteFlagSubmissionAsync(userId, flagId, HttpContext.RequestAborted);

                AddStatusMessage("Die Flageinreichung wurde erfolgreich gelöscht.", StatusMessageTypes.Success);
            }
            catch(Exception ex)
            {
                AddStatusMessage(ex.ToString(), StatusMessageTypes.Error);
            }

            return await RenderAsync(labId, slotId);
        }

        [HttpPost("flagsubmission/create")]
        [Authorize(Policy = AuthenticationStrings.PolicyIsAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFlagSubmissionAsync(int labId, int slotId, int userId, int flagId, DateTime submissionTime)
        {
            try
            {
                // Create submission
                var submission = new FlagSubmission
                {
                    UserId = userId,
                    FlagId = flagId,
                    SubmissionTime = submissionTime
                };
                await _flagService.CreateFlagSubmissionAsync(submission, HttpContext.RequestAborted);

                AddStatusMessage("Die Flageinreichung wurde erfolgreich erstellt.", StatusMessageTypes.Success);
            }
            catch(InvalidOperationException ex)
            {
                AddStatusMessage(ex.Message, StatusMessageTypes.Error);
            }

            return await RenderAsync(labId, slotId);
        }

        [HttpGet("labserver")]
        public async Task<IActionResult> CallLabServerAsync(int labId, int userId)
        {
            // Retrieve lab data
            var lab = await _labService.GetLabAsync(labId, HttpContext.RequestAborted);
            if(lab == null)
            {
                AddStatusMessage("Das angegebene Praktikum konnte nicht abgerufen werden.", StatusMessageTypes.Error);
                return await RenderViewAsync();
            }

            // Build authentication string
            var user = await _userService.GetUserAsync(userId, HttpContext.RequestAborted);
            var group = user.GroupId == null ? null : await _userService.GetGroupAsync(user.GroupId ?? -1);
            var authData = new UserLoginRequest
            {
                UserId = userId,
                UserDisplayName = user.DisplayName,
                GroupId = group?.Id,
                GroupName = group?.DisplayName,
                AdminMode = true
            };
            string authString = new CryptoService(lab.ApiCode).Encrypt(authData.Serialize());

            // Build final URL
            string url = lab.ServerBaseUrl.TrimEnd().TrimEnd('/') + "/auth/login?code=" + authString;

            // Forward to server
            return Redirect(url);
        }

        [HttpPost("sync/moodle")]
        [Authorize(Policy = AuthenticationStrings.PolicyIsAdmin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadToMoodleAsync()
        {
            try
            {
                await _moodleService.UploadStateToMoodleAsync(HttpContext.RequestAborted);

                AddStatusMessage("Hochladen der Ergebnisse in den Moodle-Kurs erfolgreich.", StatusMessageTypes.Success);
            }
            catch(InvalidOperationException ex)
            {
                AddStatusMessage(ex.Message, StatusMessageTypes.Error);
            }

            return await RenderAsync(0, 0);
        }

        [HttpGet("sync/csv")]
        [Authorize(Policy = AuthenticationStrings.PolicyIsAdmin)]
        public async Task<IActionResult> DownloadAsCsvAsync()
        {
            try
            {
                string csv = await _csvService.GetLabStatesAsync(HttpContext.RequestAborted);
                return File(Encoding.UTF8.GetBytes(csv), "text/csv", "labstates.csv");
            }
            catch(InvalidOperationException ex)
            {
                AddStatusMessage(ex.Message, StatusMessageTypes.Error);
            }

            return await RenderAsync(0, 0);
        }
    }
}