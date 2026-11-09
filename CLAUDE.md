# CLAUDE.md

Guidance for working in this repository.

## What this is

The owner's fork of [`soheil-mp/solidworks-mcp`](https://github.com/soheil-mp/solidworks-mcp),
run on **SOLIDWORKS 2023 SP3**, which upstream does not support (it tests on 2025). It is
registered in Claude Code as `solidworks-mcp` (scope local, in the owner's project). The
code, its conventions and its tests are upstream's: read `CONTRIBUTING.md`,
`docs/DESIGN.md` and `ROADMAP.md` before changing it.

**Read [`open_items.md`](open_items.md) before assuming the server works for a task.** What
sessions found is in [`findings.md`](findings.md).

## Branches and remotes

Commits and pull requests follow the `git-workflow:pr` skill (config in
`.claude/git-workflow.json`): read it before the first edit or the first commit. A new
branch and its name are the owner's call. A PR here goes to `upstream`, from a branch on the
fork: the owner opens it from the compare link `soheil-mp/solidworks-mcp/compare/main...ahmadaziz6720:<branch>`.

- `origin` = the fork `ahmadaziz6720/solidworks-mcp`, `upstream` = `soheil-mp/solidworks-mcp`.
- `main` tracks `upstream/main` and carries nothing of ours.
- `sw2023` is what is built and run here: upstream plus the 2023 interop patch, local fixes,
  and this file, `findings.md` and `open_items.md`.
- **A PR branch starts from `upstream/main`** and holds only the fix (cherry-picked or
  squashed from `sw2023`). The interop patch and these three files never go upstream.
- Update: `git fetch upstream`, `git rebase upstream/main` on `sw2023`, then build (below).
  A member missing in the 2023 interop shows as a compile error.

## Build and run

`dotnet publish` the Server to a new `%LOCALAPPDATA%\solidworks-mcp\bin\0.1.0-<stamp>`,
check `solidworks-mcp.exe --check-catalog` prints `CATALOG: ok`, register
`solidworks-mcp` (scope local, in the owner's project) again on the new exe, and run a
short check in SOLIDWORKS 2023 in a new session there: `sw_status`, one part, one section.
Keep the previous build folder for a rollback. The server is not reloaded in a running
session.

## Findings, and the entries other sessions leave here

Sessions that use the server write into `findings.md` without being asked; they run in
other repos and never commit here (the owner's global `CLAUDE.md`).

- **Commit what they left, at the start of a session here, before any work, asking the
  owner first.** Read `git status`: a change to `findings.md` or `open_items.md` this
  session did not make is such an entry. Read its diff, ask, and on the owner's yes commit
  it alone on `sw2023` with a message that says whose entries they are, then start.
- **Sort the entries into items**, as `findings.md`'s top says: each point to an S-item in
  `open_items.md`, new or existing, and a `Sorted into …` line under the entry.
- **An item that is fixed** moves to Waiting with its commit; to Closed when the fix ran and
  held in a session and, if it was sent upstream, the PR is settled.
