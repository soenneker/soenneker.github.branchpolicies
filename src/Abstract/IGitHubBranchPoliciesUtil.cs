using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Octokit;

namespace Soenneker.GitHub.BranchPolicies.Abstract;

/// <summary>
/// Provides utility methods for managing GitHub branch policies.
/// </summary>
public interface IGitHubBranchPoliciesUtil
{
    /// <summary>
    /// Retrieves the branch protection policy for the specified repository and branch.
    /// </summary>
    /// <param name="repo">The name of the repository.</param>
    /// <param name="owner">The owner of the repository.</param>
    /// <param name="branch">The branch name (default is "main").</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The branch protection settings if they exist; otherwise, null.</returns>
    [Pure]
    ValueTask<BranchProtectionSettings?> GetBranchPolicy(string repo, string owner, string branch = "main", CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a status check policy to the specified branch of a repository.
    /// </summary>
    /// <param name="repo">The name of the repository.</param>
    /// <param name="owner">The owner of the repository.</param>
    /// <param name="contexts">The list of required status check contexts.</param>
    /// <param name="branch">The branch name (default is "main").</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ValueTask AddBranchStatusCheckPolicy(string repo, string owner, List<string> contexts, string branch = "main", CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a review policy to the specified branch of a repository.
    /// </summary>
    /// <param name="repo">The name of the repository.</param>
    /// <param name="owner">The owner of the repository.</param>
    /// <param name="branch">The branch name (default is "main").</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ValueTask AddBranchReviewPolicy(string repo, string owner, string branch = "main", CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the branch protection policy for the specified branch of a repository.
    /// </summary>
    /// <param name="repo">The name of the repository.</param>
    /// <param name="owner">The owner of the repository.</param>
    /// <param name="branch">The branch name (default is "main").</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ValueTask DeleteBranchPolicy(string repo, string owner, string branch = "main", CancellationToken cancellationToken = default);
}