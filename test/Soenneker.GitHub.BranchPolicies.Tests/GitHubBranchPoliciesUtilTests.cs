using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Soenneker.GitHub.BranchPolicies.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;


namespace Soenneker.GitHub.BranchPolicies.Tests;

[Collection("Collection")]
public class GitHubBranchPoliciesUtilTests : FixturedUnitTest
{
    private readonly IGitHubBranchPoliciesUtil _util;

    public GitHubBranchPoliciesUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<IGitHubBranchPoliciesUtil>(true);
    }
}
