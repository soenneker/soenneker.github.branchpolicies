[![](https://img.shields.io/nuget/v/soenneker.github.branchpolicies.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.github.branchpolicies/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.github.branchpolicies/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.github.branchpolicies/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.github.branchpolicies.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.github.branchpolicies/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.github.branchpolicies/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.github.branchpolicies/actions/workflows/codeql.yml)

# Soenneker.GitHub.BranchPolicies

Reads branch protection, adds required status checks or review rules, and removes protection from GitHub branches.

## Installation

```bash
dotnet add package Soenneker.GitHub.BranchPolicies
```

## Configure and register

```json
{
  "GH": {
    "Token": "your-github-token"
  }
}
```

```csharp
using Soenneker.GitHub.BranchPolicies.Registrars;

services.AddGitHubBranchPoliciesUtilAsSingleton();
```

The token must be allowed to read and edit branch protection for the target repository.

## Read protection

```csharp
BranchProtectionSettings? protection = await policies.GetBranchPolicy(
    repo: "example-repository",
    owner: "example-org",
    branch: "main",
    cancellationToken);
```

`GetBranchPolicy()` returns `null` only when GitHub reports that branch protection was not found. Authentication, permission, rate-limit, and transport failures are not hidden.

## Require status checks

```csharp
await policies.AddBranchStatusCheckPolicy(
    repo: "example-repository",
    owner: "example-org",
    contexts: ["build-and-test", "CodeQL"],
    branch: "main",
    cancellationToken);
```

This replaces the branch's required status-check contexts while preserving its other protection categories.

`AddBranchReviewPolicy()` requires one approving review and code-owner review while preserving the branch's existing status checks, push restrictions, and other protection flags. `DeleteBranchPolicy()` removes all protection from the branch; that operation is destructive and allows the branch to become unprotected immediately.
