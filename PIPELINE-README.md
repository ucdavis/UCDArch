# Azure DevOps Pipeline Setup for UCDArch.Consolidated

This repository contains an Azure DevOps pipeline configuration (`azure-pipelines.yml`) that builds and deploys the UCDArch.Consolidated .NET 6.0 project to NuGet.

## Pipeline Overview

The pipeline consists of two stages:

### 1. Build Stage
- **Triggers**: Runs on commits to master/main branch and PRs affecting the UCDArch.Consolidated project
- **Steps**:
  - Uses .NET 6.0 SDK
  - Restores NuGet packages
  - Builds the project in Release configuration
  - Runs tests (if any exist)
  - Creates NuGet package with automatic versioning
  - Publishes build artifacts

### 2. Deploy Stage
- **Triggers**: Runs only on master branch after successful build
- **Steps**:
  - Downloads build artifacts
  - Pushes NuGet package to NuGet.org

## Setup Instructions

### 1. Azure DevOps Project Setup
1. Create a new Azure DevOps project or use an existing one
2. Connect your GitHub repository to Azure DevOps
3. Import this pipeline configuration

### 2. NuGet API Key Setup
1. Get your NuGet API Key:
   - Go to https://www.nuget.org/account/apikeys
   - Create a new API key with "Push new packages and package versions" permission
   - Optionally scope it to specific package ID patterns (e.g., `UCDArch.*`)
2. Add the API key to Azure DevOps:
   - Go to your pipeline → Edit → Variables
   - Add a new variable named `NUGET_API_KEY`
   - Paste your API key as the value
   - **Important**: Mark it as "Secret" to keep it secure
   - Alternatively, use Library Variable Groups for multiple pipelines

### 3. Environment Setup (Optional)
1. Go to **Pipelines** > **Environments**
2. Create an environment named `nuget-production`
3. Add approval gates if you want manual approval before deployment

### 4. Pipeline Variables
You can customize these variables in the pipeline:
- `buildConfiguration`: Build configuration (default: Release)
- `projectPath`: Path to the project file (default: UCDArch/UCDArch.Consolidated/UCDArch.Consolidated.csproj)
- `majorVersion`: Major version number (default: 1)
- `minorVersion`: Minor version number (default: 0)
- `NUGET_API_KEY`: Your NuGet.org API key (required, should be marked as secret)

## Package Versioning

The pipeline uses variable-based automatic versioning with the pattern: `{major}.{minor}.{patch}`
- **Major**: Manually controlled via `majorVersion` variable (default: 1)
- **Minor**: Manually controlled via `minorVersion` variable (default: 0)
- **Patch**: Auto-incremented using Azure DevOps counter function
  - Increments for each build within the same major.minor version
  - Resets to 0 when major or minor version changes

### Version Examples:
- First build: `1.0.0`
- Second build: `1.0.1`
- After updating minorVersion to 1: `1.1.0` (patch resets)

To release a new minor or major version, simply update the `majorVersion` or `minorVersion` variables in the pipeline.

## Project Configuration

The UCDArch.Consolidated project should be configured with NuGet package metadata in the .csproj file. Key properties include:
- **PackageId**: UCDArch.Consolidated
- **Description**: Project description
- **Authors**: UC Davis
- **License**: Project license
- **Repository**: Links to the GitHub repository
- **Version**: Controlled by the pipeline variables

## Path Filtering

The pipeline is configured to only trigger when files in the `UCDArch/UCDArch.Consolidated/` directory change, excluding build output directories (`bin/`, `obj/`). This ensures that changes to other legacy projects in the solution don't trigger unnecessary builds.

## Troubleshooting

### Common Issues:
1. **NuGet push fails**: Verify the `NUGET_API_KEY` variable is set correctly and marked as secret
2. **Duplicate package version**: The pipeline uses `--skip-duplicate` to handle this automatically
3. **Build fails on dependencies**: Ensure all required NuGet packages are available
4. **Path not found**: Verify the `projectPath` variable matches your repository structure
5. **API key permissions**: Ensure your NuGet API key has "Push new packages and package versions" permission
3. **Path not found**: Verify the `projectPath` variable matches your repository structure

### Logs:
- Build logs are available in the Azure DevOps pipeline run details
- Failed deployments will show detailed error messages in the deployment job logs
