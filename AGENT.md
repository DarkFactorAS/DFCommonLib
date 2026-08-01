---
toolRestrictions: []
maxParallelToolCalls: 20
---

# Project Agent Configuration: DarkFactor CommonLib

## Purpose

This agent.md file defines the agent's role, coding standards, and conventions
for the DFCommon library. It serves as a guide for automated agents and
contributors working in this repository.

## Goals for this app

- This project is a common library used by the other Dark Factor applications. It has easy
  access to read config files, database data, etc
- The instructions should be comprehensive and clear, guiding GitHub Copilot
  to best practices for all developers.
- Instructions should be adaptable to different repositories, scenarios, and
  user needs.
- You have the flexibility to adjust the content based on the repository in
  context and the user's requirements.

---

## Agent Role

- Maintain and extend the codebase, focusing on reliability,
  maintainability, and security.
- Automate repetitive tasks such as builds, tests, and deployments.
- Enforce code quality and project conventions.

## Coding Standards

- Language: C# (dotnet)
- Follow .NET best practices for structure, naming, and error handling.
- Use async/await for I/O-bound operations.
- Prefer dependency injection for service management.
- Write clear, concise, and well-documented code.

## Project Conventions

- All configuration files are in the Config folder.
- Use tasks defined in .vscode/tasks.json for build and run operations.
- Automated tests are in DFCommonLib.Unittests.

## Build & Run

- Use `dotnet build` / `dotnet test` (or the VS Code tasks) for compiling and running the projects.

## Agent Instructions

- When modifying code, ensure all related tests pass and that the TestAppClient
  and TestAppServer projects succeed
- Update documentation and configuration as needed for new features or
  changes.
- Maintain compatibility with existing deployment scripts and Docker
  configurations.

## Git Workflow Requirements

- At the start of every new session, create and switch to a new Git branch
  before making changes.
- Use a descriptive branch name that reflects the session scope (for example:
  `session/feature-name` or `session/bugfix-name`).
- If the `jira-task-ingest` skill is involved when starting a new session,
  name the branch after the Jira task and include the Jira key (for example:
  `session/DF-123-short-description`).
- After each resolved action, commit the completed code to the current session
  branch with a clear commit message.
- Continue committing incrementally after each resolved action rather than
  batching multiple completed actions into a single commit.
- Before opening a pull request, make sure all unit tests pass and Docker containers build and run successfully.

## Contact

For questions or contributions, refer to the repository maintainers or open an issue.
