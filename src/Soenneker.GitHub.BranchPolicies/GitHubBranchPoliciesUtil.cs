using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Octokit;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.GitHub.BranchPolicies.Abstract;
using Soenneker.GitHub.Client.Abstract;

namespace Soenneker.GitHub.BranchPolicies;

/// <inheritdoc cref="IGitHubBranchPoliciesUtil" />
public sealed class GitHubBranchPoliciesUtil : IGitHubBranchPoliciesUtil
{
    private readonly ILogger<GitHubBranchPoliciesUtil> _logger;
    private readonly IGitHubClientUtil _gitHubClientUtil;

    public GitHubBranchPoliciesUtil(ILogger<GitHubBranchPoliciesUtil> logger, IGitHubClientUtil gitHubClientUtil)
    {
        _logger = logger;
        _gitHubClientUtil = gitHubClientUtil;
    }

    public async ValueTask<BranchProtectionSettings?> GetBranchPolicy(string repo, string owner, string branch = "main", CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting branch policy for repository ({repo}) for branch ({branch}) ...", repo, branch);

        try
        {
            BranchProtectionSettings? branchPolicy = await (await _gitHubClientUtil.Get(cancellationToken).NoSync()).Repository.Branch.GetBranchProtection(owner, repo, branch).NoSync();
            return branchPolicy;
        }
        catch (NotFoundException)
        {
            _logger.LogDebug("Branch protection not found for repository ({repo}) for branch ({branch})", repo, branch);
            return null;
        }
    }

    public async ValueTask AddBranchStatusCheckPolicy(string repo, string owner, List<string> contexts, string branch = "main", CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding branch status check policy for branch ('{branch}')...", branch);

        var requiredStatusChecksUpdate = new BranchProtectionRequiredStatusChecksUpdate(false, contexts);
        await (await _gitHubClientUtil.Get(cancellationToken).NoSync()).Repository.Branch
            .UpdateRequiredStatusChecks(owner, repo, branch, requiredStatusChecksUpdate)
            .NoSync();
    }

    public async ValueTask AddBranchReviewPolicy(string repo, string owner, string branch = "main", CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding branch review policy for branch ('{branch}')...", branch);

        BranchProtectionSettings? existing = await GetBranchPolicy(repo, owner, branch, cancellationToken).NoSync();

        var requiredReviewsUpdate = new BranchProtectionRequiredReviewsUpdate(false, true, 1,
            existing?.RequiredPullRequestReviews?.RequireLastPushApproval ?? false);

        BranchProtectionRequiredStatusChecksUpdate? statusChecks = existing?.RequiredStatusChecks is { } currentChecks
            ? new BranchProtectionRequiredStatusChecksUpdate(currentChecks.Strict, currentChecks.Contexts)
            : null;

        BranchProtectionPushRestrictionsUpdate? restrictions = CreateRestrictionsUpdate(existing?.Restrictions);

        var settings = new BranchProtectionSettingsUpdate(
            statusChecks,
            requiredReviewsUpdate,
            restrictions,
            existing?.RequiredSignatures?.Enabled ?? false,
            existing?.EnforceAdmins?.Enabled ?? false,
            existing?.RequiredLinearHistory?.Enabled ?? false,
            existing?.AllowForcePushes?.Enabled,
            existing?.AllowDeletions?.Enabled ?? false,
            existing?.BlockCreations?.Enabled ?? false,
            existing?.RequiredConversationResolution?.Enabled ?? false,
            existing?.LockBranch?.Enabled ?? false);

        await (await _gitHubClientUtil.Get(cancellationToken).NoSync()).Repository.Branch.UpdateBranchProtection(owner, repo, branch, settings).NoSync();
    }

    public async ValueTask DeleteBranchPolicy(string repo, string owner, string branch = "main", CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing branch policy for repository ({repo}) for branch ({branch}) ...", repo, branch);

        await (await _gitHubClientUtil.Get(cancellationToken).NoSync()).Repository.Branch.DeleteBranchProtection(owner, repo, branch).NoSync();
    }

    private static BranchProtectionPushRestrictionsUpdate? CreateRestrictionsUpdate(BranchProtectionPushRestrictions? restrictions)
    {
        if (restrictions == null)
            return null;

        var teams = new BranchProtectionTeamCollection(restrictions.Teams
            .Where(static team => !string.IsNullOrEmpty(team.Slug))
            .Select(static team => team.Slug)
            .ToList());

        var users = new BranchProtectionUserCollection(restrictions.Users
            .Where(static user => !string.IsNullOrEmpty(user.Login))
            .Select(static user => user.Login)
            .ToList());

        return new BranchProtectionPushRestrictionsUpdate(teams, users);
    }
}
