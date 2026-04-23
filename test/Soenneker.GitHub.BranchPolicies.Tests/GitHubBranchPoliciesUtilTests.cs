using Soenneker.GitHub.BranchPolicies.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.GitHub.BranchPolicies.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class GitHubBranchPoliciesUtilTests : HostedUnitTest
{
    private readonly IGitHubBranchPoliciesUtil _util;

    public GitHubBranchPoliciesUtilTests(Host host) : base(host)
    {
        _util = Resolve<IGitHubBranchPoliciesUtil>(true);
    }

    [Test]
    public void Default()
    {

    }
}
