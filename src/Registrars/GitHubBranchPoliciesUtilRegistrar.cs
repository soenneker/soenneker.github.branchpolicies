using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.GitHub.BranchPolicies.Abstract;
using Soenneker.GitHub.Client.Registrars;

namespace Soenneker.GitHub.BranchPolicies.Registrars;

/// <summary>
/// A utility library for GitHub Branch Policy operations
/// </summary>
public static class GitHubBranchPoliciesUtilRegistrar
{
    /// <summary>
    /// Adds <see cref="IGitHubBranchPoliciesUtil"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddGitHubBranchPoliciesUtilAsSingleton(this IServiceCollection services)
    {
        services.AddGitHubClientUtilAsSingleton();
        services.TryAddSingleton<IGitHubBranchPoliciesUtil, GitHubBranchPoliciesUtil>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="IGitHubBranchPoliciesUtil"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddGitHubBranchPoliciesUtilAsScoped(this IServiceCollection services)
    {
        services.AddGitHubClientUtilAsSingleton();
        services.TryAddScoped<IGitHubBranchPoliciesUtil, GitHubBranchPoliciesUtil>();

        return services;
    }
}
