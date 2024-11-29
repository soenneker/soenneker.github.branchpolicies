using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Octokit;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.GitHub.BranchPolicies.Abstract;
using Soenneker.GitHub.Client.Abstract;

namespace Soenneker.GitHub.BranchPolicies;

///<inheritdoc cref="IGitHubBranchPoliciesUtil"/>
public class GitHubBranchPoliciesUtil : IGitHubBranchPoliciesUtil
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
        catch (Exception e)
        {
            _logger.LogDebug("Branch protection not found for repository ({repo}) for branch ({branch})", repo, branch);
            return null;
        }
    }

    public async ValueTask AddBranchStatusCheckPolicy(string repo, string owner, List<string> contexts, string branch = "main", CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding branch status check policy for branch ('{branch}')...", branch);

        var requiredStatusChecksUpdate = new BranchProtectionRequiredStatusChecksUpdate(false, contexts);
        var settings = new BranchProtectionSettingsUpdate(requiredStatusChecksUpdate, null, false);

        await (await _gitHubClientUtil.Get(cancellationToken).NoSync()).Repository.Branch.UpdateBranchProtection(owner, repo, branch, settings).NoSync();
    }

    public async ValueTask AddBranchReviewPolicy(string repo, string owner, string branch = "main", CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding branch review policy for branch ('{branch}')...", branch);

        var requiredReviewsUpdate = new BranchProtectionRequiredReviewsUpdate(false, true, 1);
        var settings = new BranchProtectionSettingsUpdate(null, requiredReviewsUpdate, false);

        await (await _gitHubClientUtil.Get(cancellationToken).NoSync()).Repository.Branch.UpdateBranchProtection(owner, repo, branch, settings).NoSync();
    }

    public async ValueTask DeleteBranchPolicy(string repo, string owner, string branch = "main", CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Removing branch policy for repository ({repo}) for branch ('main') ...", repo);

        await (await _gitHubClientUtil.Get(cancellationToken).NoSync()).Repository.Branch.DeleteBranchProtection(owner, repo, branch).NoSync();
    }
}