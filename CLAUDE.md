# CLAUDE.md

Guidance for working in this repository.

## What this is

The owner's fork of [`soheil-mp/solidworks-mcp`](https://github.com/soheil-mp/solidworks-mcp),
run on **SOLIDWORKS 2023 SP3**, which upstream does not support (it tests on 2025). It is
registered in Claude Code as `solidworks-mcp` (scope local, in the owner's project). The
code, its conventions and its tests are upstream's: read `CONTRIBUTING.md`,
`docs/DESIGN.md` and `ROADMAP.md` before changing it.

**Read [`open_items.md`](open_items.md) before assuming the server works for a task.** What
sessions found is in [`findings.md`](findings.md). `python board.py` shows both as a page
(read-only, on `127.0.0.1:8766`); `python board.py --check` lists what does not parse,
including an `**Open: …**` line that no longer matches the Open section: run it after
editing either file.

## Branches and remotes

Commits and pull requests follow the `git-workflow:pr` skill (config in
`.claude/git-workflow.json`): read it before the first edit or the first commit. A new
branch and its name are the owner's call. A PR here goes to `upstream`, from a branch on the
fork: the owner opens it from the compare link `soheil-mp/solidworks-mcp/compare/main...ahmadaziz6720:<branch>`.

- `origin` = the fork `ahmadaziz6720/solidworks-mcp`, `upstream` = `soheil-mp/solidworks-mcp`.
- `main` tracks `upstream/main` and carries nothing of ours.
- `sw2023` is what is built and run here: upstream plus the 2023 interop patch, local fixes,
  and this file, `findings.md`, `open_items.md` and `board.py`.
- **A PR branch starts from `upstream/main`** and holds only the fix (cherry-picked or
  squashed from `sw2023`). The interop patch, these three files and `board.py` never go
  upstream.
- Update: `git fetch upstream`, `git rebase upstream/main` on `sw2023`, then build (below).
  A member missing in the 2023 interop shows as a compile error.

## Build, release and run

**A release is a build that is registered**, the one sessions in the owner's project run.
A build for a live test while a fix is in progress is not one. A release is named by its
stamp, the publish time as `yyyyMMdd-HHmmss`, the name `open_items.md`, `findings.md` and
the scenario test already use ("build 204719"). Its tag is `build-<stamp>`. No semver: the
only user is the owner, several builds a day would each need a bump decided, and it would
clash with upstream's own `0.x` (`ROADMAP.md`, *Release plan*). `<v>` below is
`<Version>` in `Directory.Build.props`, upstream's.

1. **From a clean tree, at a commit pushed to `origin/sw2023`.** A tree that holds other
   work publishes from a worktree at that commit instead. Builds 145700 and 163234 report
   upstream's `fd03a7f` and ran the interop patch uncommitted, so no commit says what ran.
2. **Publish with the stamp in the version**, into a new folder:
   ```
   dotnet publish src\SolidWorksMcp.Server -c Release -p:Version=<v>-sw2023-<stamp> -o "$env:LOCALAPPDATA\solidworks-mcp\bin\<v>-<stamp>"
   ```
   `sw_status` then reports `serverVersion` `<v>-sw2023-<stamp>` (S16), and the dll's
   product version adds `+<commit>`.
3. `solidworks-mcp.exe --check-catalog` prints `CATALOG: ok`.
4. **The tag and the registration, in one question to the owner.** On the yes: register
   `solidworks-mcp` (scope local, in the owner's project) again on the new exe, then tag
   the built commit and push the tag alone:
   ```
   git tag -a build-<stamp> -m "build <stamp>" <commit>
   git push origin refs/tags/build-<stamp>
   git ls-remote --tags origin 'build-<stamp>*'
   ```
5. **The release page on the fork, with no files attached**: the build embeds type
   information from the builder's own SOLIDWORKS interop (`THIRD-PARTY-NOTICES.md`), and
   the page is a record only. `gh` has no default repo here (two remotes), so name it:
   ```
   gh release create build-<stamp> --repo ahmadaziz6720/solidworks-mcp --verify-tag --title "build <stamp>" --notes-file <file>
   ```
   The notes: the upstream commit it stands on (`git merge-base upstream/main <commit>`),
   the S-items this build fixes, each moved to Waiting naming the build, the server
   commits since the previous `build-` tag (`git log --oneline <prev>..<commit> -- src`),
   and "Scenario test: not run yet". English, as the fork is public, and naming no repo,
   file or design of the owner's, as `findings.md`'s top says.
6. **A short check** in SOLIDWORKS 2023, in a new session there: `sw_status` reads the
   new version, one part, one section. The server is not reloaded in a running session.
7. **After the scenario test**, when its entry is sorted here, its result per item
   replaces "Scenario test: not run yet" (`gh release edit build-<stamp> --repo
   ahmadaziz6720/solidworks-mcp --notes-file <file>`).

**Rollback**: register the exe of an earlier build folder again; its tag names the commit.
Keep the build folders. **A rebase onto upstream** (above) leaves the old tags off
`sw2023`'s history. They stay on the remote as the record of what was built, and the
release pages with them.

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
