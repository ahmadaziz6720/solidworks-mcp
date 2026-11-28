# Open Items

What sessions found in this fork's server and what was decided about each, S-numbered.
The raw entries are in [`findings.md`](findings.md); this file holds the items. Each item
names where it goes: **upstream** (an issue or PR at `soheil-mp/solidworks-mcp`), **local**
(a fix on `sw2023`, sent upstream as a PR when it is general), **usage** (a note for the
sessions that use the server), or **not taken**.

In the `backlog` plugin's format since 2026-11-15 (its `format.md`; `.claude/backlog.json`
is the config). Two sections here: **Open**, not yet fixed, an item's status marking it
`parked` when it is not for the next run; **Waiting**, fixed and not yet run and held in a
session, its status naming the proof. **Closed**, run and held, so ready to use, or
decided against, is in [`closed_items.md`](closed_items.md), with the runs done; an item
whose PR was merged at `soheil-mp/solidworks-mcp` is marked `merged upstream` in its
status there. A PR is optional (the owner, 2026-11-14): an item with a PR open goes to
Closed all the same, its PR named, and an open PR no longer holds an item in Waiting. An
item sent upstream ends with `**Upstream: PR #9, open.**`, one per PR or issue
(`merged`, `closed unmerged`; an issue `open` or `closed`), kept to its state.

**Open: S4, S5, S7, S10 to S12, S33, S34, S60, S64, S65, S79 to S82, S85, S86, S92, S94, S96, S99, S105, S109, S111, S115, S129, S134 to S139, S143, S145, S147 to S154, S157 to S159, S161, S162.**
Sorted 2026-11-27, the scenario test of build `20261127-085502` (findings 2026-11-27): S101,
S123, S144, S155, S140, S141, S142 and S160 held and closed, S160 on its own case only (an L
outline drew as a corner rectangle, the fallback not needed there), S80 (a), (c) and (d),
S82 (a) and S86 (a) with them; S161 new (`contours.regions` 0 in two shapes S120's rule
does not cover, the count only; S64 (l) again) and S162 new (a large `sketch_add`'s result
repeating its per-entity rows, the owner's choice of its shape), not in this run; S5 met
again. The run goes on with group 2.
Planned 2026-11-27, `## Run 2026-11-27` below, on the owner's opening answer: S52 closed
(every point held or done), S81 and S109 parked (probed, not reproduced, not met since),
S158 and S159 filed from the run of 2026-11-21's leftovers at its closing.
Sorted 2026-11-27, twelve working sessions' entries of 2026-11-24 to 2026-11-26 on build
`20261124-174916`: S143 to S157 new, reported `ok` while wrong S145 (`parallel: false` for
parallel cylinders), S148 (a) (a stale subassembly read from its parent), S152 (`0 touching`
for a part inside a subassembly) and S156 (a pattern count raised with nothing formed); S64
(q), S65 (m) to (s) and S80 (d) new parts; S64, S65, S92 (twice, with a lead: the state met
about two hours into a process, negatives read 0, stored values held through rebuilds) and
S139 met again; S118 checked in S92's state, its own sign not seen alone; S140's case met
before its fix. Next: the owner's decisions on S143 to S157 and the new parts, for the next
plan; S156, S145 and S148 first.
Sorted 2026-11-24, the scenario test of build `20261124-174916` (findings 2026-11-24): S132,
S133, S100 (whole, its (a) to (d)), S102, S103, S127, S128, S130 and S131 held and closed;
S134 (a save in place of a library feature part already known dissolved overwrites the only
good file) and S135 (a hole table's default place over the title block) new, taken into the
run's group 7; S136 new (a cut line's letters over a table and a label, with "no overlaps",
reported `ok` while wrong; S14 and S106 again), not in this run, a probe first; S137, S138
and S139 new (a close held by an open drawing, `allowEdit` on a read-only document, a
drawing dirty after a save and a PDF export), not in this run; S5 met again. The run goes on
with group 7.
Sorted 2026-11-22, the scenario test of build `20261122-194424` (findings 2026-11-22): S98,
S108, S90, S106, S126, S97 and S104 held and closed, S104 as far as this machine allows (no
8.3 folder in its File Locations); S1 held for `drw_view_detail` and a view at an angle and
closed (`broken_out` not run); S127, S128, S130 and S131 new, S127 reported `ok` while
wrong, taken into the run's group 6; S129 new (an auxiliary view's arrow far from its edge),
not in this run, a probe first; S5 met again. The run goes on with group 6, in a new
session.
Sorted 2026-11-22, the scenario test of build `20261122-142805` (findings 2026-11-22): S119,
S84, S120, S121, S122, S93 and S117 held and closed; S118 not proved there (SOLIDWORKS in
the good state, no false alarm), its proof moved to a session that meets S92's state; S123
to S125 new, S124 reported `ok` while wrong, taken into the run's group 7; S5 met again. The
run goes on with group 5.
Sorted 2026-11-22, the scenario test of build `20261122-030022` (findings 2026-11-22): S116
and S114 held and closed; S89 not proved there, its proof moved to a session that launches
SOLIDWORKS; S117 and S118 new, reported `ok` while wrong, taken into the run's group 4; S92
met again live; S5 met again. The run goes on with group 4.
Sorted 2026-11-22, the scenario test of build `20261122-002030` (findings 2026-11-22): S112,
S113 and S95 held and closed; S114 new, taken into the run's group 3; S115 new, not in this
run; S5 met again. The run goes on with group 3.
Sorted 2026-11-21, the scenario test of build `20261121-205309` (findings 2026-11-21): S61,
S83, S107 and S110 held and closed, S61 (a) in every case, the three that failed before
included; S111 new (`asm_mate_list` gives no limits of a limit mate), not in this run, for
the next plan; S5 met again. The run goes on with group 2.
Sorted 2026-11-21, the scenario test of build `20261121-175345` (findings 2026-11-21): S61
(a) not held again (a STEP inserted after another component, the assembly saved in another
folder than the STEP, lost on reopen, reported `ok`), fixed a third time in the run, group
5 built and checked again; S5 met again.
Sorted 2026-11-21, six working sessions' entries of 2026-11-18 on build `20261118-060615`:
S103 to S110 new, reported `ok` while wrong S106; S65 (k) and (l) and S80 (c) new parts;
S10, S81, S84, S89 and S92 met again, S92 six times more, with a lead on what brings it on
(an assembly holding the part loaded); S91 held again. Next: the owner's decisions on S92 to
S110 and the new parts, for the next plan; S92 (now with a test to start from), S100 (d) and
S106 first.
Sorted 2026-11-18, 22 working sessions' entries of 2026-11-17 and 2026-11-18 on build
`20261117-012659` and the scenario test of build `20261118-060615`: S91 held and closed;
S92 to S102 new, reported `ok` while wrong S92 (b) and S100 (d); S64 (m) to (p) and S65 (g)
to (j) new parts, S84 (c) a new part; S5, S52, S64, S80, S81, S84 to S86 and S89 met again.
Sorted 2026-11-17, the scenario test of build `20261117-012659` (findings 2026-11-17): S71,
S72, S87 and S88 held and closed, S52 (c) held and closed (S52 stays Open for `body_list`'s
box); S61 (a) not held (a STEP inserted into an assembly that already holds a component is
not written by the assembly's save and is lost on reopen, reported `ok`), fixed again in the
run, group 5 built and checked again; S90 new, not in this run, for the next plan; S5 met
again. Sorted 2026-11-16, the scenario test of build `20261116-205845` (findings 2026-11-16): S59
held and closed, S65 (b) to (e) held and those parts closed; S87 and S88 new, taken into
the run's group 5; S89 new (S60 (b) again, a SOLIDWORKS started by `launch`), not in this
run, for the next plan; S5 met again. Sorted 2026-11-16, working sessions' entries on build `20261116-124830` (findings
2026-11-16, seven entries): S80 to S86 new, S64 (i) to (l) and S65 (f) new parts, reported
`ok` while wrong S80 (a), S82 (a) and S64 (k); S52 met again. None of them taken into the
run: groups 4 and 5 go as planned (the owner, 2026-11-16); they wait for the next plan. Sorted 2026-11-16, the scenario test of build `20261116-124830` (findings 2026-11-16):
S49, S74, S67, S76, S77 and S78 held and closed; S52 (c) new, taken into the run's group
5; S79 (b) new, reported `ok` while wrong, not in this run, for the next plan first.
Sorted 2026-11-16, the scenario test of build `20261116-080311` (findings 2026-11-16):
S68, S69, S70, S62 and S75 held and closed; S76, S77 and S78 new, taken into the run's
group 3; S79 new, not in this run. Sorted 2026-11-16, the scenario test of build `20261116-050908` (findings 2026-11-16):
S51 and S73 held and closed; S75 new, taken into the run's group 2. The scenario test of build `20261114-024455` (`cfd987e`) ran 2026-11-14 (findings
2026-11-14). Held, and closed on the owner's word: S47, S48, S50, S53 to S58, S63, S66,
and the fixed parts of S60 to S62, S64 and S65. Back to Open: S51 (`flipEdges` in a
chamfer's edit flips every edge, reported `ok`), S49 (a line's wording), S52 (the box of a
STEP's surface bodies) and S59 (c) (the type of a sketch line not inferred). New: S72 (a
process in a bad state, a material applied at 1 kg/m³, reported `ok`). S60 to S62, S64
and S65 are from twenty working sessions on build `20261112-091908` (findings 2026-11-12
and 13), S67 to S71 seen by the run of 2026-11-13 in its own tests. Reported `ok` while
wrong or misleading: S51, S52, S60 (a), S72.
S24's fix joined PR #9 (`2c2a98a`) and S20 (d) PR #10 (`d801b06`) on 2026-11-14, as S3
and S20 say. Next: the owner's decisions on S79, S49, S51, S52, S59 (c), S72, and on what the
`findings.md` entry of 2026-11-13 added, sorted 2026-11-16: S73 and S74, new, S64 (h) and
S65 (e). The run of 2026-11-13 is done, its section in
`closed_items.md`. Not in it: S4, S5, S7 and S12 wait on upstream; S10, S11, S33 and S34 are parked until a job
needs them; S61 (a), S62 (b), S64 (c) and S65 (b to d) were offered for this run and not
chosen (2026-11-13), and stay open. The round before (S37 to S46, S6's wording, S13, S14,
S24, S32) went out in build `20261112-091908` (`276d378`) and its scenario test of
2026-11-12; the record is in each item.

The run of 2026-11-21 was closed 2026-11-27 on the owner's word, its group 7 not built (S140
to S142 committed, S101 fixed in the tree, the rest not started), its section in
`closed_items.md`, its leftovers filed as S158 and S159. A run is planned, `## Run
2026-11-27` below: 42 points of 31 items in six groups, each its own build and scenario
test, what was left of group 7 first.

## Run 2026-11-27

The run takes 42 points of 31 open items in six groups, 42 steps each done by one subagent,
each group its own build and its own scenario test: what was left of the run of 2026-11-21
(its group 7: S101, fixed in the tree and not committed, S123, S124, S125, S134 and S135;
closed 2026-11-27 on the owner's word, its section in `closed_items.md`), the items sorted
2026-11-27 (S143 to S157 and the new parts S64 (q), S65 (m) to (s), S80 (d)), and the
owner's extras (S64 (k), S111, S115, S129, S136 to S139). Group 1's check is also the proof
of S140 to S142 (Waiting, committed in the run of 2026-11-21 and never built). Phase 2
(`backlog:run`) goes unattended after the opening answer below, across as many sessions as
it needs, from the first step not ticked under *Where the run stands*.

### How it runs

- **Unattended.** After the opening answer nothing is asked. A decision met mid-run is
  the run's: the option that keeps the server's checks honest and its results true, the
  smaller change when two are equal, written into its item as "Decided <date> by the run,
  not the owner: ..., because ..." and listed in the report. Plugin questions later in the
  run (`clockify`'s offer after a commit, `git-workflow`'s push notes) are not asked; each
  is named in one line of the report. No timer is kept by the run (no `clockify` config in
  this repo).
- **Never stop on one item.** A fix that does not hold after a fair try, or a probe that
  finds no way, stays Open with what was tried written into it, its partial code in a
  stash named for it (`git stash push -m run-1127-<S> -- <files>`), and the run goes on.
  SOLIDWORKS hung or gone: end it and start it again (granted). A permission prompt or a
  dialog nobody answers is the one thing it cannot pass: it writes where it stood below,
  and stops.
- **One SOLIDWORKS, so serial** (`serial: true`): one item at a time, in the main tree on
  `sw2023`, and never a live test beside the scenario session. **One fresh subagent per
  step**, the main session thin: it hands the step over, reads the ten-line report and the
  diff, and commits. The release and the check of each group are handed to subagents too
  (as session 3 of the run of 2026-11-21 did): a Sonnet subagent for the release, a Sonnet
  subagent that frees memory, restarts SOLIDWORKS, starts the check detached, watches it to
  its `EXIT` and resumes it, one report at the end; the sort of the check's entry to a
  subagent on the default model, with the decisions per point. Models as each step names:
  the default model for a fix or a probe that needs diagnosis, Sonnet for a step whose fix
  is written out here; the run moves a step up or down by its reading, with a line in the
  report.
- **The owner moves to a new session after each group's sort** (their rule of 2026-11-22),
  not mid-group: the session that sorted a group's check hands over, the next group starts
  in a new session from the first step not ticked.
- **The step rules and launchers** are kept outside the session's scratchpad, in
  `%LOCALAPPDATA%\solidworks-mcp\livetest\run-1127\`: the first session copies
  `run-1121\brief-rules.md` there as the brief every item's subagent reads, its run named
  2026-11-27 and its stash prefix `run-1127-`, and writes `check<n>.ps1` there per group.
- **Per step**, as `CONTRIBUTING.md` (*Tests*) and the owner's rules give it: first the
  live test that shows the fault, run on the current code (a test that does not fail
  closes the point instead, the test kept when worth keeping); then the fix; then that test
  and the live tests that call the changed code (`--filter-method`, about 18 s each, never
  the whole area); unit tests offline; `python tools/check_repo.py`; a description changed
  means `--check-catalog` and `ContractTests.Tool_catalog_follows_the_conventions`. **The
  tool list is at its byte budget**: build `20261124-174916` reads `CATALOG: 165 tools,
  tools/list 247.6 KB` against `CatalogCheck.AverageToolBudget` 1536 bytes a tool (247.5 KB
  for 165), so any new option or line is paid for by trimming another description, as S51,
  S59 and S65 (a) did; a field in a result costs none of it, and a new tool brings its own
  1536 bytes. A probe step is a live probe first, its result written into the item; the
  fix follows when it shows the cause, else the point stays Open with the probe recorded.
  Where the fault cannot be made live (a SOLIDWORKS state, a process left over), the rule
  goes into an interop-free function with a unit test, as S72's did.
- **A subagent's brief**: its step as written here and its item; read `CLAUDE.md`,
  `CONTRIBUTING.md`, the parts of `docs/solidworks-api-notes.md` it needs, and this
  section; never switch branches, touch `.claude/worktrees/*`, commit, push, tag or reset;
  a stash only by name and file. SOLIDWORKS 2023 (`C:\Program Files\SOLIDWORKS
  Corp\SOLIDWORKS\SLDWORKS.exe`) started from its exe if not running, never by `sw_connect
  launch` (S89); live tests as `$env:SWMCP_LIVE_TESTS=1; dotnet test --project
  tests/SolidWorksMcp.LiveTests --filter-method "<Class>.<Method>"`. Then the item: the
  point moved to the end of Waiting with its cause, fix, the test before (its message), the
  tests after with counts, "Made stricter: …", its "Decided … by the run" lines, and
  "Local: on `sw2023`, in the commit that moves it here; not yet in a build. Upstream: not
  yet decided."; for a lettered item only the point done, split out with a new S-number
  (`board.py --next-code`) as the run of 2026-11-21 did, the item staying Open for the
  rest; the `**Open: …**` line from `board.py --open-line` and `board.py --check` clean.
  `findings.md` is not the subagent's. Report, ten lines: the outcome, the failing test
  before, the tests after with counts, every file changed, its decisions, a proposed commit
  subject and body. Nothing that names where a finding came from (`findings.md`'s top).
- **Commits**: one per step, its fix and its note, as `git-workflow:pr` gives a commit on
  `sw2023`: as the owner, no trailer. Pushed at the group's release; no PR upstream (each
  upstream line stays "not yet decided").
- **Per group, the release**: the area's live tests in full once, on what is to be built
  (the owner's global `CLAUDE.md`), class by class with SOLIDWORKS restarted between and
  `dotnet build-server shutdown` before (about 2.5 to 2.9 GB free before each start, no
  "low on memory" dialog), the runner `%LOCALAPPDATA%\solidworks-mcp\livetest\runall.ps1`
  (`-MinFreeMB 400`, as group 4 of the last run, to keep the requeues down); a flake rerun
  alone after a restart and named. It took 41 to 195 min a group in the last run.
  `docs/tools.md` regenerated when a description changed; then `CLAUDE.md`'s *Build,
  release and run*, steps 1 to 5: push `sw2023`, publish from a clean tree at the pushed
  commit, `--check-catalog`, register `solidworks-mcp` (scope local) on the new exe in
  every project `~/.claude.json` lists with it (at planning `claude_solidworks`,
  `Upwork/Project/clove sorter` with its `SWMCP_VBA=on` kept, and this repo, all on
  `0.1.0-20261124-174916`), tag `build-<stamp>` pushed alone and read back, the release
  page with no files and "Scenario test: not run yet". The points of the group move to
  Waiting naming the build, status `BUILT <date>, proof: the scenario test of build
  <stamp>`.
- **Per group, the check**, written whole so the run works nothing out mid-way. Before it:
  `dotnet build-server shutdown`, free memory read (about 2.7 GB free with SOLIDWORKS
  started is enough; under 2 GB, SOLIDWORKS restarted first), SOLIDWORKS started from
  `C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\SLDWORKS.exe`, never by the server's
  `launch`. A session id made first (`[guid]::NewGuid()`) and written on the group's check
  line, committed. A launcher `check<n>.ps1` in `run-1127\`, run from the owner's test
  project (`~\Data\Project\claude_solidworks`) so it loads the build just registered
  there: `claude -p "Uji skenario build …-<stamp> (<S-items>), menurut
  reports/uji-skenario.md. Tanpa nanya apa-apa." --session-id <uuid> --permission-mode
  auto --allowedTools mcp__solidworks-mcp --add-dir "<this repo>"`, its output to
  `check<n>.out.txt` and an `EXIT <code>` line at its end; started detached by
  `Start-Process` with its path quoted (`-File "\"<path>\""`: unquoted, the space in the
  user folder killed it at once), never as a background task of the running session; the
  pid and the transcript read about 25 s after the start. Watched by a background `Bash`
  until-loop of the watching subagent (the launcher's `EXIT` line, `tasklist` on the pid,
  the transcript's mtime past 30 minutes, `sleep 60`; the transcript
  `~/.claude/projects/C--Users-Ahmad-Aziz-Data-Project-claude-solidworks/<uuid>.jsonl`),
  one notification at the end (a Haiku watcher on `Monitor` handed back at once twice in the
  last run). It took 6 to 13 minutes in the last run. Stopped with no `findings.md` entry,
  or the transcript unwritten for 30 minutes with the process still there (stopped first):
  resumed at once, unasked, by the same launcher with `claude -p --resume <uuid>` and the
  same flags, up to three times, then Blocked with what was seen. A usage limit in the
  output: the watcher waits past the reset time it names, then resumes. A SOLIDWORKS the
  stopped session started by `sw_connect launch` is closed and started from its exe (S89).
  It starts with `sw_status` reading the new version (step 6's short check). Its
  `findings.md` entry is committed here unasked and sorted (`backlog:sort`), the release
  page's notes updated (step 7). What it finds goes four ways: **held**, the point to
  `closed_items.md`, `CLOSED <date>`; **a fix of this group not holding**, back under
  `## Open`, fixed and its group built and checked again before the next group; **a new
  point, small and in the group's files**, a later step of this run, a new S-number,
  taken into the next group; **a new point needing the owner** (a choice, a new option, a
  large change), a new Open item, not in this run. A point the check could not reach (no
  case the scenario project makes) keeps its Waiting status, its proof moved to the first
  session that meets it, as S89 and S118 were. The next group starts only after the check
  is sorted.
- **The check's brief** is the items' failure cases from their records, not the fixes: the
  scenario project's `reports/uji-skenario.md` reads the S-items named in the sentence.

### Items

Order: group 1 first, as S101's change is in the tree and S140 to S142 wait on a build;
then the groups that hold a result reported `ok` while wrong (S156 and S64 (k) in group 2;
S145, S148 (a) and S152 in group 3; S136 in group 5); then by files. Each step's test to
start from is to fail on the code at planning (`4e942ec`, the S101 diff aside, and no server
change since build `20261124-174916` beyond S140 to S142); one that does not fail closes its
point. Decided 2026-11-27 by the owner for the new options: the cheapest form, each paid
for by trimming (S65 (o) and (p) by loosening a refusal, not a new option; S65 (m) by its
hint, `drw_sheet_setup {sheet}` having made a sheet current since `200e7a1`; S65 (r) by a
line in `part_derive`'s description, not an `into`).

- **Group 1, `sketch_add` and its results** (`SketchService.cs`, `SketchRelations.cs`,
  `SketchEditService.cs`):
  - **S101** (default model): the change in the tree (`SketchService.cs`,
    `SketchRelations.cs`: the sketch's segment names, point count and point owners read once
    a call and kept up as each entity draws; a limit of the default or 0.5 s a segment,
    the longer) and its test
    `SketchEditLiveTests.Sketch_add_of_a_hundred_lines_finishes_within_its_limit` (100
    lines in one call, a closed contour, under 30 s). Run the test with the two `src` files
    stashed by name (fails: past 30 s or `TIMEOUT`), then with them (the diff's own note:
    17 s, from 165 s), and the `sketch_add` tests that call the changed code (S56's merges,
    S80 (a)'s drift, S65 (e)'s merge pass, S140's). The probe's result (where the time
    went: 150 of 165 s in the per-entity reads) into the item, which moves to Waiting. Not:
    a batch path.
  - **S123** (Sonnet), as written in the run of 2026-11-21: `sketch_finish`'s `dimensions`
    names each dimension once, and `sketch_edit` and `sketch_finish` count segments alike.
    Test: S93's case of point 6, `D3@Sketch1` once and the two counts equal (fails now:
    twice, 7 and 5).
  - **S155**, a probe (default model): centre-form arcs drawn end to end at the same
    coordinates; whether their computed ends differ from the given ones by more than the
    merge tolerance (1e-4 mm, S65 (e)) or the merge pass passes centre-form arcs over;
    then they merge as lines do. Test: a slot of two lines and two centre-form arcs end to
    end at exact coordinates, one call, one closed contour (fails now: `ManyOpen`). Not
    found: Open with the probe recorded.
  - **S144**, a probe (default model): whether a centre rectangle leaves a sketch point at
    its centre; then `{at: <centre>}` and `{type:"sketch_point", at: <centre>}` resolve to
    it (a point there taken before the diagonals through it), and the rectangle's result
    names it. Test: a centre rectangle, a horizontal dimension from the origin to its
    centre (fails now: `ENTITY_AMBIGUOUS` on the diagonals, `ENTITY_NOT_FOUND` as a sketch
    point). S140's wording ("a centre rectangle's own centre point") kept true.
  - **S80 (d)**, a probe (default model): a `rectangle {corners}` on the Front plane whose
    corners lie on the projection of two model edges; whether the inference to those edges
    makes it fail; then the rectangle is drawn as the polyline is (its four lines and
    their horizontal and vertical relations), or the hint names model edges under its
    corners. Test: the finding's case, a plate's outline over two members (fails now:
    `FEATURE_FAILED`, the hint of degenerate geometry).
- **Group 2, features and patterns** (`FeatureManagementService.cs` `dimension_set`,
  `PartFeatureService.cs` patterns, `FeatureService.cs` and `FeatureService.Fillets.cs`):
  - **S156** (default model): `dimension_set` of a pattern's count checks what
    `feature_pattern_linear` checks (each instance's faces formed) and warns, naming the
    instances that formed nothing and the direction's flip as the way, and when a count
    raised leaves the volume unchanged (a warning, as the owner decided S42). Test: a 1 x 2
    pattern of a through hole made with direction 1 unused, its direction-1 count set 1 to
    2 where SOLIDWORKS' default sense runs off the body (fails now: `ok`, `volumeDeltaMm3:
    0`, no warning).
  - **S125** (Sonnet), as written in the run of 2026-11-21: the volume per instance not
    averaged over an instance that formed nothing, or that instance named; shares S156's
    instance check where it can. Test: point 2's second case of its entry (fails now: "200
    mm³ (3 instance(s))").
  - **S124** (default model), as written in the run of 2026-11-21: a constant fillet that
    consumes a face of its edge, or whose radius exceeds a blended face's width, warns
    naming that face (a warning, no new option). Test: R20 on a 10 x 10 section's edge
    (fails now: none).
  - **S146** (Sonnet): each row of a linear pattern's dimensions, in
    `feature_pattern_linear`'s result and `dimension_list {feature}`, carries its role and
    direction (`count1`, `spacing1`, `count2`, `spacing2`), a field in the result. Test: a 2
    x 3 pattern, each row's role matching its value (fails now: no role).
  - **S64 (q)** (Sonnet): an extrude's `FEATURE_FAILED` merging into an existing body names
    contact along an edge only (non-manifold) as a cause beside profile closure and depth.
    Test: a mid-plane boss whose plate edge meets another body's edge along a line (fails
    now: the hint names closure and depth only).
  - **S64 (k)**, a probe (default model): the leg check of an `angle_distance` chamfer on
    plane-plane edges gives both faces of an edge the same plane; each face's own normal
    read. Test: the finding's groove, `angle_distance` 1 / 60° with `flipEdges` on two
    edges, the leg line naming two different normals (fails now: one plane for both).
- **Group 3, measure and assemblies** (`MeasureService.cs`, `AssemblyService.cs`,
  `AssemblyService.Structure.cs`, `FeatureManagementService.cs` `dimension_list`,
  `DocumentService.cs` `doc_rebuild`):
  - **S145** (default model): `model_measure` of two cylindrical or conical faces computes
    `parallel` from their axes (and the axes' distance), or leaves the field out where it
    did not compute it. Test: two parallel holes Ø5.5 and Ø5.2, in a part and in an
    assembly (fails now: `parallel: false`).
  - **S152** (Sonnet): `asm_interference` with a component named that sits inside a
    subassembly says that pairs inside that subassembly are not checked from the parent,
    naming it and the way (run it in the subassembly). Test: a subassembly of two touching
    parts in an assembly, one of them named (fails now: "0 touching", no word).
  - **S148 (a)** (default model): `asm_component_list` and `asm_interference` on a parent
    warn when a subassembly's own document is open and dirty, or its child count differs
    from the one the parent reads, naming it and the way (`doc_save` it, `doc_rebuild
    {force}` the parent). Test: three components inserted unsaved in an open subassembly,
    the parent listed (fails now: the old children, no word).
  - **S148 (b)** (Sonnet): `doc_rebuild {force:true}` names the documents it left dirty,
    owned and foreign apart. Test: a parent with an owned and a foreign component, forced
    rebuild (fails now: nothing named).
  - **S153 (a)**, a probe (default model): when SOLIDWORKS' Measure gives nothing for a
    curve and a face, whether the curve crosses the face can be read (the curve against the
    face's surface); then distance 0 and the crossing point are reported. Test: a sketch
    spline crossing a cylindrical face (fails now: `NOT_SUPPORTED`). Not found: Open with
    the probe recorded.
  - **S153 (b)** (Sonnet): a body taken as a `model_measure` entity (by name or id), the
    distance the minimum over its faces with that face named; the `at`-for-body hint no
    longer points at a form the tool refuses. Test: a point and a body (fails now: refused
    twice).
  - **S111** (Sonnet): `asm_mate_list` gives a limit mate's limits beside its distance
    (`MinimumDistance` and `MaximumDistance`, an angle mate's `MinimumAngle` and
    `MaximumAngle`, which S110's refusal already reads). Test: a limit distance mate 5 to
    20 (fails now: no limits).
  - **S115** (Sonnet): `dimension_list` on an assembly lists the components' dimensions by
    the names `dimension_set` takes there, beside the assembly's own, capped as the tool
    caps rows with the rest counted; no new option. Test: an assembly whose component's
    dimension `dimension_set` sets (fails now: none listed).
- **Group 4, sketch reads and relations** (`SketchEditService.cs`, `SketchRelations.cs`,
  `SketchService.cs` `sketch_info`, `sketch_constrain`, `sketch_fillet`):
  - **S143**, a probe (default model): the finding's 8-line profile (3 lines fixed, 2
    horizontal, 1 vertical, a midpoint to the origin, 3 lengths); which read gives "under"
    (the sketch's solve status against its points' and segments' own) and whether the two
    refused dimensions were redundant to the fixed lines; then the status read right, or
    the refusal naming what already defines the entity. Test: that case (fails now: under,
    then both refused as over-defining).
  - **S147**, a probe (default model): what a dimension `D1` of value 0 that no call made
    is (an offset's or a converted entity's own parameter, a hidden dimension); then the
    results say what it is or leave it out. Test: a `horizontal` between two circle centres
    and dimensions to a converted line (fails now: `D1` 0 in `sketch_info` and the
    extrude's dimensions).
  - **S154** (default model): `sketch_info` names relations whose external reference is
    lost (dangling) and does not read such a sketch fully defined; `sketch_finish`'s
    warning says "dangling relation" where SOLIDWORKS knows it. Test: `collinear` to a
    base feature's edge, the base outline changed under it (fails now: `fully`, nothing).
  - **S65 (p)** (default model): `dimension_set` of a dimension of the open sketch is made
    (the sketch solved and the value read back) instead of refused; no new option. Test: an
    open sketch's `D1` 10 set to 12, its line 12 (fails now: refused for the open sketch).
  - **S65 (q)** (Sonnet): `sketch_fillet` takes `corners: [{at, radiusMm}, ...]` beside one
    corner, each checked as one is now, all or none. Test: four corners of a rectangle in
    one call (fails now: one corner a call).
  - **S65 (n)** (default model): a sketch dimension takes a model vertex or edge as an
    entity, as SOLIDWORKS does, its value read back. Test: a horizontal dimension from a
    sketch circle's centre to a model vertex (fails now: refused "needs sketch_segment or
    sketch_point or origin").
- **Group 5, documents, session and drawings** (`DocumentService.cs`, `SessionService.cs`,
  `PartFeatureService.Mirror.cs`, `BodyService.cs`, `FeatureManagementService.cs`
  `feature_delete`, `DrawingService*.cs`), with the owner's extras:
  - **S134** (default model), as written in the run of 2026-11-21: a save in place of a
    library feature part already known dissolved is refused before writing. Test: an owned
    copy of a shipped profile, `doc_save {mode:"copy"}` to a `.sldprt`, then `doc_save` in
    place: refused, the `.sldlfp` unchanged and reopening with its sketch (fails now:
    overwritten, then `VERIFY_FAILED`).
  - **S137** (Sonnet): `doc_close` of a part an open drawing or assembly uses names those
    documents ("the drawing <name> uses it; close it first, or close the part with it") and
    says whether the discard took. Test: an owned part and its drawing open, `doc_close
    {discard:true}` on the part (fails now: `VERIFY_FAILED` with a dialog's hint).
  - **S138** (Sonnet): `doc_activate {allowEdit:true}` on a document opened read-only warns
    that `allowEdit` does not apply, with S98's reopen hint. Test: that call (fails now:
    "Active and targeted.", nothing more).
  - **S139**, a probe (default model): which call leaves a drawing dirty after a save (the
    save as, the PDF export, the drawing's rebuild touching its part's cut list); then that
    call says so, or leaves the flag as it found it. Test: a drawing saved as, then exported
    to PDF, `doc_list` reading it clean (fails now: `dirty: true`).
  - **S150 (a)** (default model): `NOT_CONNECTED` where the one `SLDWORKS.exe` is out of the
    running object table, has no main window and started 3 minutes or more before names it
    a leftover of a closed SOLIDWORKS ("end SLDWORKS.exe (pid …) and start SOLIDWORKS from
    its exe"), in an interop-free function with a unit test (not made live). Test: the
    rule's cases, a 4 h old windowless process and a 30 s old one (fails now: "still
    starting" for both).
  - **S150 (b)** (Sonnet): `sw_status` reports the attached process's id and start time,
    from the process the server is attached to, and lists any other `SLDWORKS.exe` as
    leftovers. Test: `sw_status`'s pid equal to the attached one's (fails now: no pid; the
    start time of the first process found).
  - **S157** (Sonnet): (a) `part_derive {kind:"insert"}` lists the new bodies' ids, as
    `feature_mirror` and `body_delete` do; (b) a `body_delete` feature named `-1` is renamed
    by the server to a readable default (`Body-Delete<n>`), no new option; (c) the folders a
    Mirror Part feature carries (`Solid Bodies`, `Surface Bodies`, `Planes`, their end tags)
    left out of `feature_delete`'s "not planned" list. Tests: each (fails now: no ids; `-1`;
    ten warnings).
  - **S135** (default model), as written in the run of 2026-11-21: a table with no `atMm`
    is placed by the views' free-space search, clear of the title block, or warned with an
    `atMm` hint. Test: a hole table with no `atMm` below a 1:2 plan (fails now: over the
    title block).
  - **S65 (m)** (Sonnet): the refusal of a drawing tool for a view on another sheet names
    `drw_sheet_setup {sheet}` as the way to make it current (the hint in
    `DrawingService.cs` says "switch sheets in SOLIDWORKS"). Test: `drw_annotation_list
    {view}` on a view of Sheet1 with Sheet2 current (fails now: the hint names no tool).
  - **S136**, a probe (default model): whether SOLIDWORKS lists a section line's letters as
    annotations of the parent view, or gives their place another way (the section line's
    ends), so the overlap check counts them; then they are counted and warned of (or moved).
    Test: a vertical cut drawn after a hole table whose letter falls on the table (fails
    now: "no overlaps"). Not found: Open with the probe recorded.
  - **S129**, a probe (default model): where the API puts an auxiliary view's arrow for the
    edge the call picks, whether the pick point decides it, and whether it can be moved
    next to the edge or its place read and warned of when it lies on another view. Test: an
    auxiliary view square to an edge at 24.23° in a 1:2 plan (fails now: arrow about 90 mm
    off). SOLIDWORKS' own and not movable: Open with the probe recorded.
- **Group 6, batch calls and descriptions** (`FeatureManagementTools.cs` and
  `FeatureManagementService.cs` `feature_rename`, `DocumentService.cs` and
  `SessionService.cs` `doc_list` and `sw_status`, `AssemblyService.cs` mates,
  `PropertiesService.ConfigTable.cs`, the tools' descriptions):
  - **S149 (a)** (Sonnet): `feature_rename` takes `renames: [{name, to}, ...]` beside one,
    all or none. Test: three renames in one call (fails now: one a call).
  - **S149 (b)** (default model): `doc_list` takes a filter (owned, dirty), and `sw_status`
    lists the owned and the dirty documents in full and counts the rest ("31 more open;
    doc_list lists them"). Test: 36 documents open, `sw_status` listing only the owned
    and dirty ones (fails now: all in full).
  - **S151** (Sonnet): a clause each in the descriptions of `sketch_convert` (a face's
    edges converted as construction to dimension from them), `part_derive` (mirror: the
    part stays linked to its source) and `ref_plane` (a negative offset is stored as a flip
    with a positive value), paid for by trimming. Verified by `--check-catalog` and the
    contract test, not by a live test.
  - **S65 (o)**, a probe (default model): whether SOLIDWORKS takes a distance mate of 0;
    then 0 allowed, or the refusal says to use `coincident` and that a variable reaching 0
    needs it. Test: `asm_mate_add {type:"distance", distanceMm:0}` (fails now: refused by
    the schema with no word of coincident).
  - **S65 (r)** (Sonnet): one line in `part_derive`'s description that a part keeps its
    file when mirrored in place by `part_derive {kind:"insert"}`, `feature_mirror
    {bodies}` and `body_delete` (S157 makes that route's results whole), paid for by
    trimming; no `into`. Verified by `--check-catalog` and the contract test.
  - **S65 (s)**, a probe (default model): the header SOLIDWORKS' design table takes for a
    global variable (`<name>@equations` or `$VALUE@<name>@equations`); then `config_table`
    takes a global-variable column, each configuration's value read back. Test: five
    configurations of a bore global (fails now: refused). Not found: Open with the probe
    recorded.
- **For the owner**: nothing to prepare.
- **Out of this run**: S4, S5, S7, S12 (wait on upstream); S10, S11, S33, S34 (parked,
  taken up when a job needs them); S60 (a), S81, S109 (parked, taken up when met again);
  S64 (b) (left out on the owner's word, 2026-11-16); S64 (j), (m) to (p), S65 (b)'s move
  and (f) to (l), S79 (a), S80 (b), S82 (b), S85, S86 (b), S92 (c), S94, S96, S99, S105
  (not in the owner's scope for this run: the next plan); S158 and S159 (the run of
  2026-11-21's leftovers, filed at its closing: the next plan); S161 and S162 (filed
  2026-11-27 from group 1's scenario test: the next plan, S162 the owner's choice of a
  result's shape); S89 and S118 (Waiting,
  proved by a session that meets their case).

### Opening answer

2026-11-27, the owner, in one `AskUserQuestion` call:

- Grants: **"Semua (Recommended)"**: "Sama dengan run lalu: push sw2023 tiap grup, tag
  build-<stamp>, halaman release tanpa file, registrasi exe baru di claude_solidworks,
  clove sorter (SWMCP_VBA=on tetap) dan solidworks-mcp, uji skenario headless (claude -p
  dari claude_solidworks) beserta stop/resume-nya, commit entry-nya tanpa nanya, restart
  SOLIDWORKS kalau hang. Plus: Run 2026-11-21 ditutup (sisanya dicatat jadi item,
  bagiannya pindah ke closed_items.md). Tanpa PR upstream."
- Groups: **"6 grup, 34 poin (Recommended)"**: the six groups above (the extras then put
  eight more points into groups 2, 3 and 5, as the extras' options named).
- New options: **"Bentuk termurah (Recommended)"**: "Opsi baru: S65 (q) sketch_fillet
  corners[], S149 (a) feature_rename renames[], S149 (b) filter doc_list + sw_status hanya
  dokumen milik/dirty plus hitungan, S153 (b) body sebagai entitas measure, S65 (n)
  vertex/edge model sebagai entitas dimensi, S65 (s) kolom global variable (probe dulu).
  Tanpa opsi baru: S65 (o)(p) dengan melonggarkan penolakan; S65 (m) cukup hint
  (drw_sheet_setup {sheet} sudah bisa pindah sheet sejak M8); S65 (r) satu baris
  deskripsi, bukan `into` (S157 merapikan rute 3 panggilan)."
- Extras, all four: close S52, park S81 and S109; S137, S138, S139 into group 5; S64 (k)
  into group 2, S111 and S115 into group 3; the drawing probes S129 and S136 into group 5.

### Where the run stands

- [x] Phase 1: the opening question asked and answered (2026-11-27)
- [x] S101: committed (0274bca)
- [x] S123: committed (36936e0)
- [x] S155: probed (68e6358)
- [x] S144: probed (323c69d)
- [x] S80 (d): probed (S160, ab67bdc)
- [x] Group 1: full live tests and release (build: 20261127-085502, tag build-20261127-085502, pushed)
- [x] Group 1: scenario test with S140 to S142's proof (session id: 69f65c04-85ed-4a72-88e2-1100f3d684b1; all eight held, closed; S161, S162 filed), its entry committed and sorted
- [x] S156: committed (889429e)
- [x] S125: committed (fe3de5e)
- [x] S124: committed (4ed7fd4)
- [x] S146: committed (0b4df4d)
- [x] S64 (q): committed (S163, ffccaea)
- [x] S64 (k): probed (S164, d617e5a)
- [ ] Group 2: full live tests and release (build: ; live tests on d617e5a, stopped 2026-11-28 17:52 on the owner's word, mid-AssemblyLiveTests: done SketchEdit 89/89, PartFeature 58/58, FeatureManagement 42/42, AdvancedFeature 30/30, BodySheetMetal 52/52, Reference 47/47, Modelling 11/11, SurfaceShape 8/8, Weldment 18/18, FaceEdit 5/6 (S5), Molding 4/4, Properties 26/27 (S72), per `livetest\logs\summary.txt` after `GROUP2-1127 START`; to run: AssemblyLiveTests whole (three memory-guard stops before the stop, about 0.4 to 1.5 GB free), then DrawingLiveTests, ExportLiveTests, M0LiveTests, TopDownLiveTests, RecognitionLiveTests, AssemblyStructureLiveTests, ConfigurationLiveTests, HardwareLiveTests, VisualsLiveTests, SimulationLiveTests, SimulationExtrasLiveTests, TemplateLiveTests, by `run-1127\group2.ps1` with its class list cut to those; then flakes, release; the 13 commits from 889429e not pushed yet)
- [ ] Group 2: scenario test (session id: ), its entry committed and sorted
- [ ] S145: committed
- [ ] S152: committed
- [ ] S148 (a): committed
- [ ] S148 (b): committed
- [ ] S153 (a): probed
- [ ] S153 (b): committed
- [ ] S111: committed
- [ ] S115: committed
- [ ] Group 3: full live tests and release (build: )
- [ ] Group 3: scenario test (session id: ), its entry committed and sorted
- [ ] S143: probed
- [ ] S147: probed
- [ ] S154: committed
- [ ] S65 (p): committed
- [ ] S65 (q): committed
- [ ] S65 (n): committed
- [ ] Group 4: full live tests and release (build: )
- [ ] Group 4: scenario test (session id: ), its entry committed and sorted
- [ ] S134: committed
- [ ] S137: committed
- [ ] S138: committed
- [ ] S139: probed
- [ ] S150 (a): committed
- [ ] S150 (b): committed
- [ ] S157: committed
- [ ] S135: committed
- [ ] S65 (m): committed
- [ ] S136: probed
- [ ] S129: probed
- [ ] Group 5: full live tests and release (build: )
- [ ] Group 5: scenario test (session id: ), its entry committed and sorted
- [ ] S149 (a): committed
- [ ] S149 (b): committed
- [ ] S151: committed
- [ ] S65 (o): probed
- [ ] S65 (r): committed
- [ ] S65 (s): probed
- [ ] Group 6: full live tests and release (build: )
- [ ] Group 6: scenario test (session id: ), its entry committed and sorted
- [ ] The report

## Open

### S4 - `drw_dimension_add` keeps wrong values: a supplement reported as a match, an angle for `vertical` - OPEN 2026-11-09, parked: waits on upstream

Upstream: issue #5.

**Upstream: issue #5, open.**

### S5 - `doc_open` STEP imports surfaces with 3D Interconnect forced off, reported `ok` - OPEN 2026-11-09, parked: waits on upstream

Upstream: issue #2. Usage until fixed: import by File > Open and `doc_activate`, or check
`solidBodies` after `doc_open`. Still so in builds `…-204719`, `…-215810` and `…-230924`:
a STEP round trip gave 16 surface bodies and no solid. Again in `…-235702`,
`…-010708`, `…-014952`, `…-024431`, `…-034022`, `…-064352`, `…-091645`, `…-115800`,
`…-134233`, `…-144220`, `20261111-140208` and `20261112-091908` (export `solids: 1`,
import 16 surface bodies). The usage line misses one case: a STEP that comes in as an
assembly reads `solidBodies: 0` on a good solid (S61 b); read `model_mass` there until the
build with S61 (b), which counts them through the components.
Again in `20261114-024455` (findings 2026-11-14, point 5): a STEP that `model_export`
wrote from an assembly of three 20 mm cubes and a turned cylinder brought the cubes in as
solids where they were and the cylinder as 3 surface bodies (S52); `model_export` of that
assembly had reported `solids: 2`, two distinct parts of four components.
Again in `20261116-050908` (findings 2026-11-16, point 5): the shelled tray's STEP, 16
surface bodies.
Met again 2026-11-16 (findings 2026-11-16, build `20261116-080311`, point 10): the
shelled tray's STEP, 16 surface bodies, as before.
Met again 2026-11-16 (findings 2026-11-16, build `20261116-124830`, point 9): the
shelled tray's STEP, 16 surface bodies, as before.
Met again 2026-11-16 (findings 2026-11-16, build `20261116-205845`, point 7): the same, 16
surface bodies.
Met again 2026-11-17 (findings 2026-11-17, build `20261117-012659`, point 8): the same, 16
surface bodies.
Probed 2026-11-16 by the run (S52): the STEP of a 20 mm cube and a shaft r 5 × 30 turned
45° about [1, 1, 0] brings the shaft in at the identity, its turn lost, as 3 surface
bodies: the side exact (942.478 mm²), each cap a plane bounded by two spline curves from
(0, ·, -5) to (6, ·, 5), 89.816 mm² against a Ø10 disc's 78.540.
Met again 2026-11-18 (findings 2026-11-18, the scenario test of build `20261118-060615`,
point 4): the shelled tray's STEP, 16 surface bodies, as before.
Met again 2026-11-21 (findings 2026-11-21, the scenario test of build `20261121-175345`,
point 2): the shelled tray's STEP, 16 surface bodies, as before.
Met again 2026-11-21 (findings 2026-11-21, the scenario test of build `20261121-205309`,
point 7): a STEP export opened by `doc_open`, 16 surface bodies, as before.
Met again 2026-11-22 (findings 2026-11-22, the scenario test of build `20261122-002030`,
point 7): a STEP export opened by `doc_open`, 16 surface bodies, as before.
Met again 2026-11-22 (findings 2026-11-22, the scenario test of build `20261122-030022`,
point 7): a STEP export opened by `doc_open`, 16 surface bodies, as before.
Met again 2026-11-22 (findings 2026-11-22, the scenario test of build `20261122-142805`,
point 11): a STEP export opened by `doc_open`, 16 surface bodies, as before.
Met again 2026-11-22 (findings 2026-11-22, the scenario test of build `20261122-194424`,
point 13): a STEP export opened by `doc_open`, 16 surface bodies, as before.
Met again 2026-11-24 (findings 2026-11-24, the scenario test of build `20261124-174916`,
point 15): a STEP export opened by `doc_open`, 16 surface bodies, as before.
Met again 2026-11-27 (findings 2026-11-27, the scenario test of build `20261127-085502`,
point 11): a STEP export opened by `doc_open`, 16 surface bodies, as before.

**Upstream: issue #2, open.**

### S7 - `dimension_set` accepts a change that splits the body - OPEN 2026-11-09, parked: waits on upstream

Upstream: issue #4. Usage:
read `bodies` in the result after each `dimension_set`. A model driven by global variables
changes through `equation_set`, whose result has no `bodies`: read `model_mass` after it
until the build with S37 (b), then `model.bodies` in its result.

**Upstream: issue #4, open.**

### S10 - Drawing format and title block - OPEN 2026-11-09, parked: taken up when a job needs it

Precision and decimal separator of drawing
dimensions, parentheses for reference dimensions, `n×` in hole callouts, DWG NO from a
property, material and weight, section label text and place, unaligning a view, rotating
a projected view (a side view projected from a top view came rotated 90°). Upstream
ROADMAP T10.3, T10.10, T10.12 partly. Not yet decided which to build. Usage for hole
callouts until then: model holes as one feature plus a pattern, end condition through, so
the callout gets `n×` and "through". Parked 2026-11-11 by the owner: taken up when a job
needs it.
Met again 2026-11-18 (findings 2026-11-18, "a two-sheet A3 drawing of a 16-member weldment
part", point 5), build `20261118-060615`: dimensions at two decimals with a comma
(`1200,00`), the title block not filled.
Met again 2026-11-18 (findings 2026-11-18, "a custom sheet format made from the ISO one with
`vba_run`", points 2 and 3), the same build: a custom format made with `vba_run` in 4 calls,
and it held. `DrawingDoc.EditTemplate`; the format sketch's lines from
`GetActiveSketch2().GetSketchSegments()`, selected by `Select4(True, Nothing)` when both ends
lie in the block's box, deleted by `Extension.DeleteSelection2(0)`; the format's notes from
the sheet view's `GetFirstNote` chain, selected by `Annotation.Select2(True, 0)`
(`Select3(True, Nothing)` selected none, with no error); new lines by
`SketchManager.CreateLine` with `AddToDB = True`; notes by
`ModelDoc2.InsertNote("$PRP:""Name""")`, then `SetTextFormat` and `SetPosition2`;
`DrawingDoc.EditSheet`; `Sheet.SaveFormat(path)`. `drw_sheet_setup {format}` then read the
new format back, `titleBlockMm` found from its lines. And `drw_annotation_list
{format:true}` gives a format note's resolved text, not its link: an empty field reads `""`,
and which property fills it cannot be told without `vba_run` (`Note.PropertyLinkedText`).
Proposed by the session: a tool for the format on these calls (delete in a box, add lines,
add linked notes, save the format), and the link listed beside a note's text.

### S11 - No dimension to a section's cut face - OPEN 2026-11-09, parked: taken up when a job needs it

`drw_dimension_add` takes model entities only.
Parked 2026-11-11 by the owner: taken up when a job needs it.

### S12 - Section orientation is not in the tool description - OPEN 2026-11-09, parked: waits on upstream

Arrow side and view direction
(findings 2026-11-09, point 12). Upstream: proposed in PR #1's text; a description change
when PR #1 is settled.

### S33 - No equations linked to a file - OPEN 2026-11-11, parked: taken up when a job needs it

From reading the source (findings 2026-11-11,
first entry, point 1): `equation_set`, `equation_list` and `equation_delete` act on one
document, and nothing reaches `IEquationMgr.FilePath` or `LinkToFile`. A design of several
part files that share key sizes holds each as its own global variable per part: a change
is one `equation_set` per part, and one made by hand in one part does not reach the others.
`asm_part_new` with a face (T9.22 partly, T9.23 not built) and `part_derive` `insert`
(solids, surfaces, material) do not cover it. Local: link, unlink and read a document's
external equation file, `equation_list` naming each variable's source; general, fit for a
PR. Parked 2026-11-11 by the owner: taken up when a design of several parts needs it.
Met 2026-11-13 (findings, "a revolved part remade", point 6): four key sizes of one part
typed again as global variables in a second part, one `equation_set` in each per change.

### S34 - Reading and checking a model costs a caller listings and judgment - OPEN 2026-11-11, parked: taken up when a job needs it

From reading
`ROADMAP.md` and the tool reference (findings 2026-11-11, first entry, points 2 to 4).
(a) `model_review` against a rules file (T20.8, partly built): a feature left with its
default name, a fillet or chamfer before the last boss, cut or hole, a sketch on a face or
edge of a later feature, a literal dimension where a global variable of that meaning
exists, no global variable at all. A caller now checks these from `feature_list`. (b)
Explain a model in one call (T20.7, not built): each feature with the dimensions that drive
it, sketch relations and fragile references, in place of `model_info`, `feature_list`,
`dimension_list` and `equation_list`. (c) Session export as a replayable recipe (T3.15,
not built), an idea for now. Upstream roadmap items; (a) first if built. Parked 2026-11-11
by the owner: taken up when a job needs it.

### S60 - What a session cannot tell about the process it runs on - OPEN 2026-11-12, parked: (a) not reproduced, taken up when met again

On build
`20261112-091908`. (a) `sw_status` and `doc_activate` gave `dirty:false` for a part
holding ~2 hours of unsaved work from the session before, a second server process listed
in `otherServerPids` (findings 2026-11-12, "a printed part's tree deleted", point 1): a
session trusting it would have left the work unsaved. (b) `NOT_CONNECTED: SOLIDWORKS is
running … but can't be reached over COM`, with another server process listed, the hint
naming privileges only (findings 2026-11-12, "a two-part assembly", point 5). (c) `sw_connect` after a restart that had not happened gave
the same PID with no word (findings 2026-11-12, "a second helical part", point 2; bears on
S16). (d) `sw_connect` warns "revision 31.3.0 is untested (tested: 33.x = 2025)" on a
build made and tested on 2023 (findings 2026-11-12, "a two-part assembly", point 2); the
same check as M0's live test that expects `33.` (S2, S17). Local: (a) probe how `dirty`
is read and whether another process's changes hide it; (b) the hint names the other server
process; (c) "same process as before" or "a new one"; (d) the fork's tested revision is
31.x. Taken into the run of 2026-11-13: (a) a probe first, fixed in the run when it shows
the cause; (b) and (c) as proposed, no choice in them. (d) Decided 2026-11-13 by the
owner: on `sw2023` only, the tested revision 31.x, M0's live test with it; not for
upstream. (b) to (d) fixed in the run of 2026-11-13; (a) probed, its cause not found,
still open. (a) The probe: `dirty` is SOLIDWORKS's own `IModelDoc2.GetSaveFlag`, read the
same way by `doc_list`, `sw_status` and `doc_activate` (`DocumentService.Info`); nothing in
the server sets or clears it (no `SetSaveFlag`). Live on 2023 SP3, a part saved and then
changed read `dirty:true` in all three after a change by the server, and after a change
made by another process through COM (a VBScript client setting a dimension and
rebuilding, the way a second server process would); it stayed true through
`model_export` (STL, 3MF, STEP, PNG, PDF, Parasolid), `view_screenshot`, `doc_save`
`copy`, `doc_undo` back to the saved state, `feature_rename`, `feature_suppress` and back,
`doc_rebuild` `force`, a sketch, `feature_delete`, and 13 minutes idle with SOLIDWORKS'
auto-recover on (every 10 minutes); only a save cleared it, and a call refused before any
change left it false. So another process's changes do not hide it; what cleared it in that
session is not known (a save the session did not make, by the user or by a tool that saves
another document, is left). Kept as
`M0LiveTests.Dirty_reads_unsaved_changes_made_through_another_process` (passes on the code
before and after). Usage until found: a part opened before the session, of unknown
history, is saved or asked about, never closed on `dirty:false` alone. (b) Cause: the
`NOT_CONNECTED` text named only "still starting" and privileges. Fix: the error (now
`SessionNotes.Unreachable`) names each other `solidworks-mcp` process with its start time,
"an older build or another session", and the hint says it may have started SOLIDWORKS at its
own level or be stuck in a call, never to end it, and to ask the user to close that session
or restart SOLIDWORKS; with no other process the text is as before. (c) Cause: the result
carried the PID only. Fix: `sw_connect` gives `processStarted` and `process`: "same as
before" (the same PID and start time as the instance this server was last attached to, kept
across a lost connection), "a new one (before: pid …, started …)", or "first connection of
this server". (d) Fix: `sw_connect` warns of an untested revision unless it is 31.x
(2023), `SessionService.TestedMajor`; `M0LiveTests.Connect_reports_the_revision` expects
`31.` and no warning. Before, on the code before the fix:
`Connect_reports_the_revision` failed ("Assert.Null() Failure: … SOLIDWORKS revision 31.3.0
is untested (tested: 33…"), and the new
`Connect_says_whether_it_found_the_same_SOLIDWORKS_process_as_before` failed ("Sub-string
not found … "process":"same as before""). (b) and (c)'s "a new one" cannot be shown live
(another server holding COM, a SOLIDWORKS restart mid-test): unit tests instead,
`SessionNoteTests` (6, the logic in the interop-free `SessionNotes.cs`, compiled into the
unit tests). After: both pass, with `Status_names_the_commit_…`,
`Status_gives_a_failed_call_s_error_code_…`, `A_modal_dialog_returns_DIALOG_BLOCKING_…`,
`Dirty_owned_documents_need_save_or_discard` and the new dirty test, 7/7 live; unit tests
284/284; `check_repo` ok; no description changed. Made stricter: none. Decided 2026-11-13
by the run, not the owner: (1) "before" is the instance this server process was last
attached to, through any call, not a previous server's, because a new process has no
connection of its own to compare and says "first connection of this server"; (2) the same
PID with another start time reads as a new process, because Windows reuses PIDs; (3) (a)
stays open with a usage line rather than a change to how `dirty` is read, because the probe
found the reading right in every case it tried. Local: (b) to (d) on `sw2023`, `7601276`;
in build `20261114-024455`. Upstream: not yet decided; (d) never.
Held in the scenario test of build `20261114-024455` (findings 2026-11-14, point 6): (c)
"same as before" and, after a restart, "a new one (before: pid …)"; (d) no revision
warning; (b) is not named in the entry. Those parts closed 2026-11-14 on the owner's word;
the item stays Open for (a).
Parked 2026-11-16 by the owner: (a), probed 2026-11-13 and not reproduced, taken up when
met again; the usage line above holds.

### S64 - Feature calls refused, failed or counted wrong, with hints that miss - OPEN 2026-11-12

On build
`20261112-091908`. (a) A blind cut from a plane inside the body whose end lies outside it
was refused, "no floor face at 13 mm … (planar faces at 0, 0 mm)", while right; kept with
`keepOnFailure` (findings 2026-11-12, "a part of extrudes", point 4; bears on S40, whose
fix passes a cut with no floor face at all). The same point: a second feature from a
consumed sketch worked, which the description does not say. (b) `feature_extrude` boss
toward +X from an offset plane failed `FEATURE_FAILED` (nothing created), `reverse` built
at once; the hint did not apply ("a part's body raised", point 3). (c) `feature_fillet`
and `feature_chamfer` of 1 mm over a tangent chain failed five times, `FEATURE_FAILED`,
the hint "a radius too large" on faces 7 and 10 wide; 0.5 went through (2026-11-13, "a
plate's two ends", point 1). Proposed: retry per edge and name the one that fails. (d)
`feature_delete` of three named features failed `VERIFY_FAILED` after removing two of
them, and the dry run had not said the call would need `deleteChildren` ("a printed part's
walls", point 3): a failure that changed the model. (e) `sketch_constrain` with a
reference `{name:"Line1", sketch:"<other>"}` resolved to the open sketch's `Line1` ("a
printed part's tree deleted", point 2). (f) `sketch_add` of one hexagon on a planar face
reported `regions: 2`, on a plane 1 ("a second helical part", point 3). (g)
`feature_sweep` merged into a body warned that the start face (109.52 mm²) is smaller than
the profile (155.295), the difference inside the existing body: a false alarm on a merged
sweep ("a helical screw part", point 3). Taken into the run of 2026-11-13: (a), (d), (e),
(f), (g), no choice in them: (a) by S40's rule, the cut passing on its volume; (d) the
delete's arguments checked before the first removal, so a refusal changes nothing. (b) a
probe first. (c) offered 2026-11-13 and not chosen; stays open. **Done in the run of
2026-11-13: (a), (d), (e), (f) and (g)** (in that order: (e) and (f), then (a), (d), (g)).
**Left open: (b)**, probed and not reproduced, **and (c)**, not chosen. (e) Cause: the sketch
tools' resolver did resolve `{name, sketch}` and `{at, sketch}` in the named sketch, but
then dropped duplicates by label, and a segment's label is its name alone ("sketch
segment Line1"), so the other sketch's `Line1` was dropped as the open sketch's (a point
the same way, by its coordinates, which are its own sketch's). Fix
(`SketchEditService.SameEntity`): two segments are one only when name and owning sketch
match, two points only in the same sketch; an entity of another sketch is labelled "… of
<sketch>" in `resolved`. `sketch_constrain`, `sketch_relation_add` and
`sketch_dimension_add` share that resolver (`Refs`), so all three are fixed. (f) Cause:
`regions` was `ISketch.GetSketchRegionCount()`, which on a face counts the region the
face's edges close around the sketch (probed: the second region's edges are the face's 4
and the hexagon's 6). Fix (`SketchService.Contours`): when the count exceeds the closed
contours, a region counts only when every edge of it lies on the sketch's own contour
edges (compared by geometry: the COM objects are never the same). Decided 2026-11-13 by
the run, not the owner: the edges are read only when `regions` exceeds the closed
contours, because a region closed by model edges adds to the count past them and every
`sketch_add` and feature reads the contours, where an edge read costs a few COM calls.
Before, on the code
before the fix:
`Constrain_relates_a_segment_to_the_segment_of_the_same_name_in_another_sketch` failed
"Step 1 of 1 (collinear relation) failed: 'collinear' needs 2 different entities; some
references name the same one.",
`Dimension_and_relation_take_a_point_in_another_sketch_on_a_segment_of_the_same_name`
failed "'entities' names the same entity twice.", and
`Regions_count_the_sketchs_own_closed_regions_on_a_face_as_on_a_plane` failed "Expected:
1, Actual: 2" for the hexagon on the face. After: the three pass, with the pierce,
merge, relation, constrain, dimension-argument, info, convert-of-a-face and sketch-add
tests of `SketchEditLiveTests` and two face sketches of `ModellingLiveTests`, 17 of 17
live; unit tests 278 of 278; `check_repo` ok; no description changed. Made stricter:
none (duplicates are still dropped within one sketch; the regions check reads only when
the count exceeds the closed contours, and on a plane it kept the one region of a
hexagon crossed by a line, where SOLIDWORKS reads 0 closed contours). Local: (e) and (f) on
`sw2023`, `9e65c62`; in build `20261114-024455`. Upstream: not
yet decided.
(a) Cause: a blind cut from a plane inside the body leaves a planar face on that plane,
where it starts; S40's rule passed a cut with no floor only when every planar face lay at
the asked depth, so the start face at 0 mm made it a failure. Fix (`ExtrudeIntent`): for
a blind cut a face at 0 mm counts as no floor, once every face of the cut lies on the
asked side of the sketch plane (a cut gone the other way has its faces behind it); the
cut then passes on its volume with S40's line "no floor at 13 mm: the cut broke out …".
A floor at another depth is still refused, and a cut that ends inside shows its floor.
The second feature from a consumed sketch: `sketch`'s description now says "a used one
too". Before, on the code before the fix,
`A_blind_cut_from_a_plane_inside_the_body_that_leaves_it_passes_on_its_volume` failed
"no floor face at 13 mm along [0, 1, 0] (planar faces at 0 mm)". After: it passes (the
cut out through the top, a second cut from the same consumed sketch out through the
bottom, a 5 mm pocket from the same plane with its floor), with S40's test, the cut, mid
plane, oblique, buried-boss and box tests of `M0LiveTests` and the engraved text of
`ModellingLiveTests`, 8 of 8 live; `ContractTests.Tool_catalog_follows_the_conventions`
and `--check-catalog` ok (247.7 KB); unit tests 278 of 278; `check_repo` ok. Made
stricter: none (a check made looser, for blind cuts with a face on the sketch plane
only). (b) Probe, 2026-11-13: a boss along +X from offset planes parallel to Right on a
100 × 20 × 60 block, 17 cases, `normal`, `+x` and `reverse`: the profile sharing the top
face, a side face, an edge line or a corner point with the body, straddling or beside a
face's edge, on the body's ±X faces, clear of the body, `merge` false. None returned
nothing: each built and verified, or failed a check for its own reason (a profile inside
the body: "no faces"; a boss from outside ending inside the body: "no cap face", as S40
leaves a boss). The fault was not reproduced and its cause is not known, so the hint is
not changed. Decided 2026-11-13 by the run, not the owner: (b) stays open with no code
change, because a hint written for a cause no probe showed would miss as the one now
does; a repro needs the session's own part. Local: (a) on `sw2023`, `576a1c2` (which
records (b)'s probe too); (a) in build `20261114-024455`. Upstream: not yet decided.
(d) Cause: the delete checked only for children outside the list. A plane, the boss
sketched on it and the boss's mirror, all three named, had none, so the call went to
SOLIDWORKS without `swDelete_Children`, and SOLIDWORKS then deleted the parent alone (live:
the plane went, the boss and the mirror stayed), a `VERIFY_FAILED` that had changed the
model. A dry run that met dependents listed them and did not say the call would refuse. Fix
(`FeatureManagementService.DeletePlan`, `Dependents`): when a target is the parent of a
feature the list removes, every child being planned, the delete passes
`swDelete_Children`, which then removes no more than the plan; a kept absorbed sketch whose
parent is another target (the boss's sketch on the plane, without `deleteAbsorbed`) is a
dependent, as is a feature made from an absorbed sketch that goes (a second extrude from a
consumed sketch, a body of its own); the refusal is built before anything is removed, and
the dry run warns with it word for word ("The real call would refuse it and change
nothing: INVALID_ARGUMENT: …"). Decided 2026-11-13 by the run, not the owner: the option is
passed only when a target is the parent of a planned feature, not on every delete with no
outside dependents, because that is the case SOLIDWORKS got wrong and every other delete
keeps the call it had. Before, on the code before the fix,
`Feature_delete_of_a_plane_its_boss_and_the_boss_s_mirror_removes_all_three_or_nothing`
failed first on the dry run's missing warning ("Filter not matched in collection"), and
with that line left out, "VERIFY_FAILED: 'Boss-Extrude2', 'Mirror1' still exist after the
delete (removed: 'Plane1')";
`Feature_delete_of_a_sketch_another_feature_uses_refuses_before_removing_anything` failed
"Expected: ["Boss-Extrude2"], Actual: null" (the dry run listed no dependent).
`Feature_delete_of_a_feature_with_its_child_in_the_same_list_removes_both` (an extrude and
its fillet) passed before too and is kept as the guard on the option. After: the three
pass, with the eight other delete tests of `FeatureManagementLiveTests` (fillet, parent
refused, child's sketch, absorbed kept, equations listed and deleted, the name that picks up
equations, the user's documents refused), 11 of 11 live; unit tests 278 of 278;
`check_repo` ok; no description changed. Made stricter: a kept absorbed sketch on a plane
that goes and a feature made from a sketch that goes are now dependents, refused without
`deleteChildren` (before, both went to SOLIDWORKS unchecked); the
single-feature deletes of those tests are not refused. Local: (d) on `sw2023`, `fe1a1af`;
in build `20261114-024455`. Upstream: not yet decided.
(g) Cause: the start-face check compared the sweep's planar faces in the profile plane with
the area the profile sketch encloses, merged or not; a merged sweep has no start face where
its profile lies inside the existing body, so a profile partly buried warned "SOLIDWORKS may
have used only some of its contours". The same case, with a second contour clear of the
body, failed `VERIFY_FAILED` on the volume: the body count grew, so the sweep was taken as
unmerged and its volume compared with profile area x path length. Fix
(`AdvancedFeatureService.SweepIntent`, `AdvancedFeatureLogic.StartFace`, `InteriorPoints`,
`Topology.InsideSolid`): on a merged boss sweep whose start face is smaller, the profile's
region is sampled (a 16 x 16 grid, holes and a quarter cell along the contours left out),
the points off the start face are ray-cast against the solid bodies after the sweep (a ray
each way, an odd count of hits on one body both ways is inside), and when every one is inside,
the result's checks say "the start face is … mm², but the profile sketch encloses … mm²: the
rest lies inside the existing body" instead of the warning, and the volume is not compared.
A point outside the bodies, or on a face of one, keeps the warning ("; not all the rest lies
inside the existing body"), as do an unmerged sweep, a cut, and a start face larger than the
profile. Decided 2026-11-13 by the run, not the owner: a start face smaller than the profile
with no sampled point off it (a sliver between grid points) still warns, and a point on a
body's face counts as not inside, because the check can let a merged sweep off only on
points it has shown to be inside; and a buried profile also stops the volume comparison when
the body count grew, because part of that sweep merged, which the count alone did not see. A
sweep that really dropped a contour was not produced live: SOLIDWORKS 2023 swept both of two
disjoint circles, and crossing contours are refused before the call; the warning side is
shown by `Points_inside_a_body_are_told_from_points_outside_and_on_its_faces` (inside, outside
and on two faces of a block) and the offline `StartFace` test. Before, on the code before the
fix,
`Sweep_merged_with_part_of_its_profile_inside_the_body_does_not_warn_of_lost_contours` failed
on its first sweep (a circle r 5 half in a block) "Assert.DoesNotContain() Failure: Filter
matched in collection ["The sweep's start face is 39.27 mm², but the profi"···]", and the
same with a second circle clear of the block failed (probe) "VERIFY_FAILED: … the swept volume
is 4445.354 mm³ vs profile area 67.544 mm² x path length 60 mm = 4052.655 mm³". After: it
passes (the circle, a ring r 5 / r 2, and the two circles, each with no warning and its
volume), with the ray test and the eight other sweep tests of `AdvancedFeatureLiveTests`
(line, slot, spring, circular profile, cut, twist, refusals, profile off the helix start),
10 of 10 live; `AdvancedFeatureLogicTests` 26 of 26 (two new: `StartFace`, the ring's
interior points); unit tests 278 of 278; `check_repo` ok; no description changed. Made
stricter: none (the warning and the volume comparison are let off for merged boss sweeps
only, on points shown inside). Local: (g) on `sw2023`, `9a46c32`; in
build `20261114-024455`. Upstream: not yet decided.
Held in the scenario test of build `20261114-024455` (findings 2026-11-14, point 6): (a);
(d), the dry run's refusal word for word; (e); (f); (g). Those parts closed 2026-11-14 on
the owner's word; the item stays Open for (b) and (c).
(h) Added 2026-11-16, on build `20261112-091908` (findings 2026-11-13, "two printed
plates", point 3): `feature_pattern_linear` of a rib and its own linear pattern together
failed "SOLIDWORKS created no feature (the API returned nothing). (tried 2 senses of the
direction)"; with `direction2` on the same axis as `direction` it is refused. Four ribs at
uneven spacing along one axis took a pattern and a mirror about a new plane. Not probed:
whether a pattern of a pattern is refused by SOLIDWORKS itself, so the hint would name it.
Not yet decided.
Decided 2026-11-16 by the owner: (c) taken into the run of 2026-11-16, group 1, each edge
tried alone on a failure and the failing ones named; (b) left out of it, probed 2026-11-13
and not reproduced, taken up when met again. (h) in the same group, a probe first (`## Run
2026-11-16`).
**Done in the run of 2026-11-16: (h), not reproduced.** Probe, 2026-11-16, on `9cd5638`,
`FeatureLinearPattern5` called directly with the seed and its pattern on mark 4: on a block
with a boss and its linear pattern, and on an L bracket with a rib and its linear pattern,
SOLIDWORKS 2023 built a pattern of the seed and its pattern together (2 new instances, the
volume exact), and of the pattern alone (it brings its seed along: the same 2); geometry
pattern on, the same; a body on mark 256 patterned the whole body as a second body. The
tool built both too: the boss's `[seed, pattern]` 4/4 positions, the rib's 4/4 and 6/6. So
a pattern of a feature and its own pattern is not refused by SOLIDWORKS, and the hint "a
pattern cannot take its own pattern as a seed" would be false. "The API returned nothing
(tried 2 senses)" came back only where the rib's copies touch the ribs already there (2 mm
ribs at 0 and 4, patterned 2 mm): the rib alone at that spacing fails word for word the
same, so that cause is the rib's copies, not the pattern of a pattern. A copy off the walls
in the requested sense fails `VERIFY_FAILED` "only the opposite direction does", which is
true. Decided 2026-11-16 by the run, not the owner: (h) done as not reproduced, with no code
change and the hint not changed, because SOLIDWORKS builds the pattern asked for and the
tool already passes it on; a hint for the rib's touching copies would rest on one probed
spacing. Test kept, passing before and after (no fix):
`A_rib_and_its_own_pattern_take_a_second_pattern_for_uneven_spacing` (ribs at -12, -8, 0
and 4, 1800 mm³, 6/6 positions, no warning), with `Rib_5mm_fills_the_corner_of_an_L_bracket`,
2 of 2 live. Made stricter: none. Local: on `sw2023`, in the commit that records it; not
yet in a build. Upstream: not yet decided. The item stays Open for (b) and (c).
**Probed in the run of 2026-11-16: (c), not reproduced.** Probe, 2026-11-16, on `86e7042`,
on the plate of S52's test (113 × 10, 10 thick mid plane, quarter-round R7 ends into a face
30° off their tangent), each case on a fresh plate: `feature_fillet` over the top chain
(the top line and both rounds, by `at` on the line, `propagate` on) built at 0.5, 1, 1.5,
2 and 3 mm (ΔV -6.496, -25.993, -58.585, -104.37, -235.31 mm³; at 1 mm about (1 - π/4) ×
the chain's 121 mm), the back chain at 1 mm the same, the chain from a round with `propagate`
on the same, the line and both rounds named without `propagate` the same, the line and one
round -23.954, the face z = 5 at 1 mm -50.669; `feature_chamfer` 1 mm by the line and by a
round -60.666 each. So the five failures of the finding did not come back on this plate.
What did fail, `FEATURE_FAILED` "SOLIDWORKS created no feature (the API returned nothing)"
with the hint "A radius too large for the adjacent faces fails": a round alone with
`propagate` off, at 1, 0.5 and 0.2 mm alike, and both rounds together without the line.
Each such edge ends on the top line, tangent to it and not in the call: a fillet that stops
at a tangent joint fails at any radius, so there the hint misses as the finding says, but
the cause is the joint, not the radius, and a retry per edge would name the same edges the
call gave. Decided 2026-11-16 by the run, not the owner: (c) stays Open with no code change
and no test kept, because the plan's test builds and the retry per edge was decided for a
chain that fails as a whole and has edges that pass alone, which no probed case showed; a
hint for the tangent joint ("an edge ending tangent to one not in the call: turn
`propagate` on or add that edge") is a new choice for the owner. A repro needs the
session's own part. The item stays Open for (b) and (c).
(i) Sorted out of the entry of 2026-11-16 ("the same edit, a second round", point 1), on
build `20261116-124830`: `feature_chamfer {type:"equal"}` returns two dimensions
(`D1@...`, `D3@...`) that are independent once one is driven. An equation on D1 alone left
D3 at its old value when the variable changed, a 6 x 5 chamfer where 6 x 6 was meant; no
warning, `equation_set` ok. Found only by a hand-figured volume (+24 mm³). Workaround:
drive both. Proposed: the result names both as the chamfer's legs and says they are
separate dimensions, or `equation_set` warns when one leg of an equal chamfer is driven
alone. Local. Not decided.
(j) Sorted out of the entry of 2026-11-16 ("an edit to three printed parts", point 1), the
same build: `feature_chamfer` `angle_distance` 1 / 60° on two edges where a groove's side
meets a flat wall, both unflipped and then both `flip: true`: `VERIFY_FAILED` "no chamfer
face with legs 1/1.732051 or 0.57735/1 mm along edge E25 (found legs 1/1.085649 mm)", the
hint "A distance larger than the adjacent faces fails; use smaller distances or fewer
edges". The cause was the side: one edge needed the flip and the other did not, and on the
wrong side the 1.73 leg ran past the 1 mm deep side face, so the chamfer was cut short. The
hint pointed at the distances, and the error named only the edge that failed, not that the
other held. Workaround: `flipEdges: [0]` on the pair (S51's per-edge flip, Closed), which
held on four grooves, -16.0 mm³ each as figured. Proposed: when the found legs share the
requested distance and the other leg is short, the hint says the angle's side may be
swapped (`flip`, `flipEdges`), and the error gives each edge's result. Local. Not decided.
(k) **A result reported `ok` while wrong.** Sorted out of the same entry, point 2: the leg
check of that chamfer gave both faces of each edge the same plane, "edge E24: 1 mm along
F252 (plane [-0.866, 0, -0.5]), 1.732051 mm along F253 (plane [-0.866, 0, -0.5])", where
one of the two is a face square to the axis, normal ±Y. The volume matched to the mm³, so
the chamfer was right and the reported normals were not. Not met on a `distance_distance`
9.5 / 9.4 chamfer over a cylinder's and two planes' concave edges against one flat face
(findings 2026-11-16, "an edit to two printed parts", point 1: two faces per edge, each
with its own normal), so it may belong to `angle_distance` or to plane-plane edges.
Proposed: a probe of that case; each face's own normal read. Local. Not decided.
(l) Sorted out of the entry of 2026-11-16 ("an edit to four printed parts", point 6), the
same build: `sketch_add` polygon, the contour read `"closed"` with `"regions":0`, and the
cut from it worked (volume to the hand figure). (f) fixed an over-count on a face (closed
2026-11-14); this is an under-count, on a build with that fix. Proposed: a probe of the
region count for a polygon on a plane and on a face; when SOLIDWORKS gives 0 for a closed
contour, the regions counted from the closed contours. Local. Not decided.
(m) Sorted out of the entry of 2026-11-17 ("fillets and a trim cut on three parts of an
assembly", point 3), on build `20261117-012659`: `feature_extrude` of a boss whose end lands
coplanar with an existing face, so its cap merges into that face, failed `VERIFY_FAILED: ...
no cap face at 12.2 mm along [0, -1, 0]` and was rolled back, though the boss was right.
Worked round with `keepOnFailure: true` and `model_mass` before and after (+1 280.58, as by
hand less what was solid already); two calls lost. The boss's side of (a) (S40 let a cut
off). Proposed: when no cap face is found, accept a face of the merged body lying in the
cap's plane, or report the volume and say the cap merged. Local. Not decided.
(n) Sorted out of the entry of 2026-11-17 ("a part's hole positions driven by globals after
a restart", point 2), the same build: `feature_chamfer {type:"distance_distance",
distanceMm:2, distance2Mm:3.464}` on six convex edges, then the same with `flip:true`: both
`VERIFY_FAILED` with the identical text "no chamfer face with legs 2/3.464 mm along edge E23
(found legs 2/3.011938 mm)"; `keepOnFailure` showed the 3.464 leg on the 3 wide face both
times, so `flip` changed nothing. The values swapped, no flip, held (-2 081.668 against
-2 081.73 by hand). Bears on (j): there an `angle_distance` side was swapped per edge; here
`flip` on a `distance_distance` had no effect at all, and the hint again pointed at the
distances. Not reported `ok`. Proposed: a probe of `flip` on a `distance_distance` chamfer;
the hint names the swap when the found legs hold the asked ones the other way round. Local.
Not decided.
(o) Sorted out of the same entry, point 3: `sketch_offset {segments:[Line1], chain:true,
side:"inward"}` on a loop drawn by one `sketch_add` polyline `closed:true,
construction:true` (6 lines) was refused `INVALID_ARGUMENT: The segments form an open chain,
which has no inside`. All six segments without `chain` worked, the offset lines regular, as
wanted. Either `chain` does not follow construction segments or a closed polyline's closing
joint is not merged for it. Proposed: a probe of which, and `chain` following the closed
loop. Local. Not decided.
(p) Sorted out of the entry of 2026-11-17 ("a turned printed part with two swept internal
threads", point 3), the same build: (g) again, closed 2026-11-14. `feature_sweep`, merged,
the same profile and helix: at the end of the tree it said "the start face is 2.103 mm², but
the profile sketch encloses 2.113 mm²: the rest lies inside the existing body" ((g)'s
let-off); inserted before the rollback bar it warned "(SOLIDWORKS may have used only some of
its contours)" for the same geometry. The result was right both times. Proposed: a probe of
the check with the bar mid-tree (the bodies the ray test reads may be those after the bar).
Local. Not decided.
Met again 2026-11-17 ((l); findings 2026-11-17, "a printed part with a plate tilted 20°",
point 1), build `20261117-012659`: a closed 4-point polyline on a face tilted 20° from
vertical, with one converted construction edge in the sketch, read `"closedContours":1,
"regions":0`; the through-all cut from it was the hand figure exactly.
Met again 2026-11-17 ((i); findings 2026-11-17, "a flat printed plate with holes", point 1),
the same build: an equation on `D1` of an equal chamfer left `D3` at its old value at a
second set of variables, +11.272 mm³, `equation_set` `ok` with no warning; driving `D3` too
gave the figure. New: the workaround is on no `Usage` line, so a session reading only those
does not meet it.
Met again 2026-11-17 ((l); findings 2026-11-17, "three printed parts of a box and its lid",
point 1), the same build: two closed profiles on a planar face, each a 3-point arc of 270°
and a 2-segment polyline whose ends merged onto it, read `"closedContours":2,"regions":0`;
the boss and the cut from them were the hand figures.
Met again 2026-11-17 ((l), narrowed; findings 2026-11-17, "a printed bracket of one extruded
profile", point 1), the same build: on one planar face a circle Ø3.2 read `"regions":1`;
after the through cut from it, a concentric circle Ø6.5 on the same face, enclosing the new
hole's edge, read `"regions":0`, and the blind cut from it was the figure. So the under-count
is met when the region the contour closes holds an inner loop of the face: (f)'s rule counts
a region only when every edge lies on the sketch's own contour, and the hole's edge is a
model edge. Proposed by the session for (l): a model edge that is an inner loop of the face,
wholly inside the contour, does not disqualify the region.
Decided 2026-11-21 by the owner: (c), (i) and (l) taken into the run of 2026-11-21, group 4: (c) the tangent-joint hint, (i) the equal chamfer's legs named and warned of, (l) the session's rule for an inner loop; (j), (k), (m) to (p) not in it, for the next plan, (k) first (`## Run 2026-11-21`).
(l) done in the run of 2026-11-22, group 4, split out as S120 (Waiting).
(i) done in the run of 2026-11-22, group 4, split out as S121 (Waiting).
(c) done in the run of 2026-11-22, group 4, split out as S122 (Waiting).
Met again 2026-11-24 ((m); findings 2026-11-24, "a multi-body envelope part from a vendor
drawing", point 1), build `20261124-174916`, twice in one part: a boss whose stems end on a
leg's end face (`no cap face at 80 mm along [0, 1, 0]`) and a mid-plane boss whose two caps
met two members' inner faces (`no cap face at 142 mm ...; no cap face at -142 mm`), both
rolled back though right. Worked round with `keepOnFailure: true` and `body_list` (+26 138
and +1 817 600, as by hand): four calls lost, which (m)'s proposal would have saved.
(q) Sorted out of the same entry, point 3, the same build: a mid-plane boss of a plate whose
end edge met another body's edge along a line only (edge to edge, no face shared) failed
`FEATURE_FAILED: SOLIDWORKS created no feature (the API returned nothing)`, the hint naming
profile closure and depth. The cause was the line contact (non-manifold): extended 40 further
over the member it built at once (+1 725 000 as figured); the unused sketch had to be deleted
after. Not reported `ok`. Proposed by the session: the hint names edge-only contact with an
existing body as a cause. Local. Not decided.
Decided 2026-11-27 by the owner: (q) taken into the run of 2026-11-27, group 2, the hint naming edge-only contact; (k) taken into group 2 too, a probe first, each face's own normal read (an extra the owner chose); (b), (j), (m) to (p) not in it (`## Run 2026-11-27`).
(q) done in the run of 2026-11-27, group 2, split out as S163 (Waiting).
(k) done in the run of 2026-11-27, group 2, split out as S164 (Waiting).
Met again 2026-11-27 ((l); findings 2026-11-27, the scenario test of build
`20261127-085502`, point 9): `contours.regions` 0 for closed contours in two shapes S120's
rule (closed 2026-11-22) does not cover, the features right; moved to S161.

### S65 - Missing options met while modelling - OPEN 2026-11-12

On build `20261112-091908`. (a) No body
mirror: `feature_mirror` of two extrudes made `merge:false` failed "the mirrored copy must
touch the body"; ~12 calls to build the other side (findings 2026-11-12, "two parts
changed mid-tree", point 1). (b) A sketch text's position is not in `sketch_info` and no
tool moves it ("a part's body raised", point 4). (c) An arc's centre cannot be named as a
reference where another point sits on it ("a part of extrudes", point 2). (d)
`view_screenshot` fits to reference axes too: a part 8% of the image ("a printed part's
tree deleted", point 6); proposed `hideAxes` as `hidePlanes`. (a) Decided 2026-11-13 by
the owner: a body mirror. (b) to (d) offered 2026-11-13 and not chosen; stay open. **Done in
the run of 2026-11-13: (a).** **Left open: (b) to (d)**, not chosen. (a) Cause: `feature_mirror`
mirrored features only (`InsertMirrorFeature2` with `mirrorBody` false); the copy of a feature
whose body stands alone touches no body, and SOLIDWORKS returns nothing. Fix
(`PartFeatureService.MirrorBodies`, new file `PartFeatureService.Mirror.cs`; `feature_mirror`
gains `bodies` and `merge`, `features` no longer required): the solid bodies, by id or name,
on mark 256 with the plane on mark 2, `InsertMirrorFeature2(true, false, merge, false, 0)`, a
`MirrorSolid` feature. merge off: verified one new solid body per seed, each the seed
reflected (volume, area, center of mass, 24 sampled vertices), and the volume grown by the
seeds' volume exactly. merge on: the volume grew by at most the seeds' volume (the overlap
reported), and each copy that joined a body has faces found at their reflections (planes,
cylinders, spheres; the faces on the plane join). `features` and `bodies` together, or
`merge` with `features`, are refused (`INVALID_ARGUMENT`); the feature mirror's failure hint
now names `bodies` for a body of its own. Probed: with merge on, SOLIDWORKS 2023 makes nothing
when any copy touches no body (a box on the plane and a cylinder off it, in one call), so
that call fails `FEATURE_FAILED` with the hint to mirror those with merge:false, and nothing
is left; the box alone joined its copy. Decided 2026-11-13 by the run, not the owner:
`bodies` is a list of ids or names (strings, as `features`), not `{id|name|at}` references,
because the tool list is at its byte budget (an `EntityRef` array's schema alone is ~400
bytes; with the trims below 7 bytes are left); `merge` defaults to false, because a copy of its
own is checked exactly and is what the finding wanted; a merge whose copy touches no body is
refused with the way out rather than split into two calls, because the smaller change keeps
the result as asked or nothing. The description and the `plane` and `features` texts of
`feature_mirror` were shortened to fit ("Mirror features or bodies about a plane. Verified:
copies are the seeds reflected; else rolled back."). Before, on the code before the fix,
`Mirror_of_two_unmerged_extrudes_as_bodies_adds_their_reflections` (the session's call: the
two extrudes as `features`) failed "feature_mirror: SOLIDWORKS created no feature (the API
returned nothing)". After: it passes (the same call refused with the hint, then the two
bodies mirrored: +4785.398 mm³, 5 bodies, box x -80..80), with the new
`Mirror_of_a_body_on_the_plane_with_merge_joins_it_to_its_copy` (the refused mixed merge
rolled back, the box joined, 5/6 faces, `merge` with `features` refused), the four other
mirror tests of `PartFeatureLiveTests` and
`Feature_delete_of_a_plane_its_boss_and_the_boss_s_mirror_removes_all_three_or_nothing`, and
`ContractTests.Tool_catalog_follows_the_conventions`, 8 of 8 live; unit tests 278 of 278;
`--check-catalog` ok; `check_repo` ok. Made stricter: none (the feature path is unchanged but
for its hint). Local: (a) on `sw2023`, `fd9bd83`; in build `20261114-024455`.
Upstream: not yet decided.
Held in the scenario test of build `20261114-024455` (findings 2026-11-14, point 6): (a).
That part closed 2026-11-14 on the owner's word; the item stays Open for (b) to (d).
Sorted 2026-11-16 from findings 2026-11-13, "two printed plates", on build `20261112-091908`.
(c) met again (point 4): a `vertical` relation between `{at}` on an arc centre and the
origin failed "'vertical' conflicts with existing relations or dimensions (the sketch
became no_solution)" where three concentric arcs from `sketch_offset` and three line ends
sat on that point; the same relation worked with one circle there. Likely resolved to an
offset arc's centre, which the offset relation holds, and the result does not say which
point it took. Workaround: `concentric` with the model's hole edge. (e) New (point 5): no
way to merge two segment ends drawn at the same coordinates: `{at}` twice is refused
"'merge' needs 2 different entities; some references name the same one", and a polyline
with a separate arc at its ends leaves them unmerged ("ManyOpen"). Workaround: the arc
drawn 0.2 to 0.5 off, then `coincident` on the two points, one call more a joint. Proposed
by the session: a segment's end named (`{name:'Line1', end:'start'}`), or `sketch_add`
merging ends that meet. Both not yet decided.
Decided 2026-11-16 by the owner: (e) `sketch_add` merges segment ends that meet, no new
reference form; (b) and (d) taken into the run of 2026-11-16, group 4, with (c) and (e):
(c) `resolved` names the point taken and warns of the others there (`## Run 2026-11-16`).
(f) Sorted out of the entry of 2026-11-16 ("an edit to four printed parts", point 1), on
build `20261116-124830`: `sketch_add` slot makes 2 lines and 2 arcs with no relations
(`sketch_info`: `"relations":0`), where SOLIDWORKS' own slot tool ties them. Two slots took
8 `tangent`, 4 `vertical` and 1 `equal` in `sketch_constrain` before they could be
dimensioned, and the first call, with 2 `vertical` short, left the sketch under-defined
(24.8 s). Missing: a slot with its tangencies and its centreline, so it is defined by its
two centres and a width. Proposed: `sketch_add` relates the slot's four segments as it
draws them (tangent at the four joints), and perhaps a centreline between the centres;
which form is a choice. Local. Not decided.
**Done in the run of 2026-11-16: (e).** **Left open: (b), (c), (d) and (f).** (e) Cause:
SOLIDWORKS merges an end drawn onto another end only within about 1e-5 mm (probed on 2023 SP3:
ends 1e-6 and 1e-5 mm apart merged, 1e-4 and 1e-3 mm not; a 3-point arc on a polyline's
exact ends, on a plane or a face, in one call or two, and a polyline drawn onto an arc's ends,
all merged). An arc from its centre and angles, whose ends are irrational (x 30 ± 4.330127),
on a polyline typed to 4 decimals stayed apart, and the whole sketch read "ManyOpen". Fix
(`SketchService.MergeEnds`, run by `sketch_add` after it draws): each end of a segment the
call drew is merged onto an end of another segment within 1e-4 mm drawn before it, in the call
or earlier, with `sgMERGEPOINTS` (as `sketch_relation_add` `merge`: one point, no relation),
and checked by the two ends reading one point within 1e-4 mm of where they were; a merge
SOLIDWORKS does not make is a warning, and the contour status reads them apart. Each is listed
in its entity's `merged` ("Arc1 start merged onto [34.33, 12.5] (Line1 start), 0.000027 mm
apart"), and the result's new `endsMerged` counts the call's merges, SOLIDWORKS' and the
server's. The `sketch_add` description: "relations SOLIDWORKS made, ends merged where they
meet" (was "relations and merges SOLIDWORKS made"). Before, on the code before the fix (the
merge pass off), `SketchEditLiveTests.Sketch_add_merges_an_arc_drawn_on_a_polyline_s_ends_into_one_closed_contour`
failed "contours invalid, 0 closed, 0 open, problem ManyOpen". After: it passes (1 closed
contour, 2 ends merged 0.000027 mm apart; the arc in a later call onto an older polyline, 2
closed; a 3-point arc on exact ends, SOLIDWORKS' 2 merges counted; 15 sketch points, 0
relations; an end on a line's middle, 0.001 mm and 0.2 mm off, not merged), with
`Sketch_add_lists_the_relations_and_merges_SOLIDWORKS_made`,
`Constrain_reads_a_relation_the_sketch_already_has_as_held_and_runs_the_next_steps`,
`Constrain_runs_twenty_steps_on_a_sketch_of_two_dozen_segments_within_its_limit_and_says_how_long`,
`Regions_count_the_sketchs_own_closed_regions_on_a_face_as_on_a_plane`,
`Construction_relation_and_delete_work_in_a_3D_sketch`,
`PartFeatureLiveTests.The_model_box_after_round_faces_is_the_body_s_extremes` (lines and arcs
from angles, extruded), `AdvancedFeatureLiveTests.Sweep_of_a_slot_along_a_line_is_exact_through_its_start_face`,
`M0LiveTests.Box_extrude_matches_the_oracle`,
`WeldmentLiveTests.Table_frame_from_a_3D_sketch_is_a_mitered_top_and_four_square_cut_legs`,
`BodySheetMetalLiveTests.Miter_flange_on_a_slanted_end_sketches_on_a_plane_normal_to_the_edge`
and `ContractTests.Tool_catalog_follows_the_conventions`, 12 of 12 live; unit tests 290 of
290; `--check-catalog` ok; `check_repo` ok. Made stricter: none (a merge is checked, and one
not made is said). Decided 2026-11-16 by the run, not the owner: the tolerance is 1e-4 mm, the
one the sketch tools check geometry to and `sketch_import` merges to, because an end moved by
less still reads where it was asked, and 1e-3 mm or more would join ends a caller drew apart
on purpose (the session's workaround drew them 0.2 mm off). Decided 2026-11-16 by the run, not
the owner: ends of construction geometry merge as solid ones, because a merge moves no end
beyond that tolerance and the flag does not change where the ends are. Decided 2026-11-16 by
the run, not the owner: only ends merge, an end that meets a segment's middle or a free point
does not, and two ends both drawn before the call are left as they are, because a merge there
would change geometry or a sketch the call did not draw. Decided 2026-11-16 by the run, not the
owner: the merge is `sgMERGEPOINTS` on the two points, not SOLIDWORKS' inference
(`AddToDB` off), because inference also snaps to model edges and adds relations (S80), and
the merge is one point with no relation, as the relation count and contour status read after
(verified: 0 relations, 1 closed contour). 3D sketches take the same pass; there it was not
probed, and a merge not made is warned. Local: (e) on `sw2023`, in the commit that records it
here; in build `20261116-205845`. Upstream: not yet decided.
**Done in the run of 2026-11-16: (c).** **Left open: (b), (d) and (f).** (c) Cause: `{at:[x,y]}`
on a sketch point takes the nearest of the sketch's points (`EntityResolver.SketchPointAt`, the
first of several at one place), and its label read "sketch point at [x, y]" whatever else sat
there; a relation that failed named no entity ("'vertical' conflicts with existing relations or
dimensions"). Probed on 2023 SP3: SOLIDWORKS merges a centre or an end drawn exactly on another
point (two circles on one centre, a line end on a centre: one point), so points stay apart at one
place only where a tool made them: `sketch_offset` gives each offset arc its own centre, and
three line ends drawn on the centre merged onto the last offset arc's. With the original arc
fixed, `vertical` from `{at}` on that centre to the origin failed "the sketch became over", as in
the finding; the point taken was the original arc's centre (Arc1), and with the original deleted
`sketch_relation_add` and `sketch_constrain` both moved the first offset arc (Arc2) of the two.
Fix (`EntityResolver.SeveralPointsAt`): where more than one sketch point lies within the
reference's tolerance, the label names the point taken by the segments it ends or centres and
counts the others ("sketch point at [20, 10] (Arc1 centre; 2 other points there)"), so
`resolved` names it, and a warning lists the others: "entities[0]: 3 sketch points lie within
0.01 mm of [20, 10]; took the nearest, Arc1 centre. Not taken: Arc2 centre; Arc3 centre, Line1
start, Line2 start, Line3 start. No reference picks one of those (a sketch point has no name, and
'at' takes the nearest): relate an arc's centre through its arc ('concentric'), or draw the points
apart." A relation's failures "conflicts with existing relations" and "the geometry doesn't
satisfy it" (`SketchEditService.AddRelation`, both tools) name their entities, as a dimension's
already did ("'vertical' on sketch point at [20, 10] (Arc1 centre; 2 other points there), origin
'Origin' conflicts …"). Which point is taken is unchanged; no description changed. Before, on the
code before the fix,
`SketchEditLiveTests.A_point_where_offset_arcs_centres_and_line_ends_sit_names_the_one_taken_and_warns_of_the_others`
failed "Assert.Matches() Failure: Pattern not found in value", value "'vertical' conflicts with
existing relations or di…". After: it passes (the conflict named, the label and warning in
`sketch_relation_add`, the same warning for a `sketch_constrain` step, a lone point unwarned),
with `Horizontal_relation_of_two_points_levels_the_circle_centers`,
`Merge_and_coincident_join_two_line_ends_into_one_point`, `Relation_misuse_and_conflicts_are_refused`,
`Constrain_follows_a_point_through_a_merge`, `Constrain_stops_at_a_failing_step_and_keeps_the_ones_before`,
`Dimension_and_relation_take_a_point_in_another_sketch_on_a_segment_of_the_same_name`,
`The_origin_takes_dimensions_and_relations`,
`Radial_horizontal_and_point_to_point_vertical_dimensions_drive_the_geometry`,
`Tangent_concentric_midpoint_symmetric_collinear_parallel_intersection_and_fix_hold`,
`Constrain_defines_a_rectangle_fully_in_one_call`,
`ReferenceLiveTests.A_reference_with_its_sketch_and_no_type_is_in_that_sketch` and
`ContractTests.Tool_catalog_follows_the_conventions`, 13 of 13 live; unit tests 290 of 290;
`--check-catalog` ok; `check_repo` ok. Made stricter: none (nothing refused that passed before; a
failed relation's message now names its entities). Decided 2026-11-16 by the run, not the owner:
"there" is within the reference's tolerance (`tolMm`, default 0.01 mm), not only the points at
one place, because those are the points the `at` could have meant. Decided 2026-11-16 by the run,
not the owner: no `{name, sketch}` is offered for the others, and the warning says none picks
them, because a sketch point has no name the server reads and `EntityRef` has no end or centre
qualifier; one would be repeated in every tool's schema, which is at its byte budget. Decided
2026-11-16 by the run, not the owner: the owner segment is named only where several points lie,
because finding it reads every segment of the sketch for each point reference, and a lone point's
position names it already. Decided 2026-11-16 by the run, not the owner: a relation's failure
names its entities, because a failed call returns its message only, not the warnings, and the
finding's call was a failure. Decided 2026-11-16 by the run, not the owner: `sketch_constrain`
holds the point as before (through the first segment with an end or centre at its place), not by
the point's own key, because in the probe that was the point the resolver took, and checking it
costs a segment walk per point on every step (S55). Local: (c) on `sw2023`, in the commit that
records it here; in build `20261116-205845`. Upstream: not yet decided.
**Done in the run of 2026-11-16: (b), the reading; the move stays open.** **Left open: (b)'s
move, (d) and (f).** (b) Cause: `sketch_info` lists a text as a segment (`SketchText1`, kind
`text`, as `GetSketchSegments` returns it), but read only its string: every text read at [0, 0]
with no height or angle, so a text moved also read unchanged in the edit tools' diffs. Probed on
2023 SP3: `ISketchText.GetCoordinates()` reads the point `InsertSketchText` was given, in sketch
coordinates on the Top plane (whose frame is not the model's); extruded, the outlines start
0.77 mm right of it and stand on it up to the height, round letters overshooting both by 0.26 mm,
so it is the left end of the baseline. Fix (`SketchEditService.ReadText`): a text's entry gains
`atMm` (that point, sketch mm [x, y] as the other entities), `heightMm` (its format's
`CharHeight`) and `angleDeg` (its format's `Escapement`); the point is also the text's place in
the before/after diff, so an edit that moved a text lists it in `changed`. No description
changed. The move: no tool moves sketch entities (the sketch tools trim, extend, offset, fillet,
chamfer, pattern, mirror, convert, set construction and delete), so none takes a text, and (b)
stays Open for it; `ISketchText.SetCoordinates(x, y, z)` exists, not probed. Before, on the code
before the fix (the fields added, not filled),
`SketchEditLiveTests.Info_gives_a_texts_position_height_and_angle_where_it_was_added` failed
"Assert.NotNull() Failure: Value is null" (`atMm`). After: it passes (a 10 mm "MCP 42" added at
[-30, -5] on the Top plane reads [-30, -5], 10 mm, 0°, open and finished; extruded, its box starts
at x -29.23 and spans sketch y -5.26 to 5.26), with `Info_lists_segments_contours_frame_and_dimensions`,
`Edit_reopens_a_finished_sketch_with_its_frame`,
`ModellingLiveTests.Sketch_text_at_its_height_engraves_the_top_face_with_a_cut`,
`Linear_pattern_of_a_circle_places_3_circles_20_apart`,
`Offset_rectangle_5_outward_is_110_by_70_and_inward_90_by_50`,
`Merge_and_coincident_join_two_line_ends_into_one_point` and
`Constrain_defines_a_rectangle_fully_in_one_call`, 8 of 8 live; unit tests 290 of 290;
`check_repo` ok. `docs/solidworks-api-notes.md` (WP9) gains the probe. Made stricter: none
(a reading; a text that moved now reads as changed, where it read unchanged at [0, 0]). Decided
2026-11-16 by the run, not the owner: the position, height and angle go on the text's own entry
in `segments`, not a list of their own, because `sketch_info` already lists the text there by
the name and kind other tools see, and a second list would give it twice. Decided 2026-11-16 by
the run, not the owner: the position is the point SOLIDWORKS keeps (`GetCoordinates`), the left
end of the baseline, in sketch mm as the other entities, not the outlines' box, because it is the
`at` that `sketch_add` was given and the one a move would take, and the box depends on the font.
Decided 2026-11-16 by the run, not the owner: the angle is read as SOLIDWORKS stores it, and only
0° was read live, because no tool sets another (`sketch_add` takes none). Decided 2026-11-16 by
the run, not the owner: no move tool or option in this run, because none exists to extend and a
new one costs description bytes in a tool list at its budget, which is the owner's choice. Local:
(b), the reading, on `sw2023`, in the commit that records it here; in build `20261116-205845`. Upstream:
not yet decided.
**Done in the run of 2026-11-16: (d).** **Left open: (b)'s move and (f).** (d) Cause: probed on 2023
SP3, no reference axis is in the fit. `ViewZoomtofit2` fits the part's box (`GetPartBox`), which takes
in every sketch whose feature is shown, even with the sketches toggle off (`hideSketches`), and
reference points; not a sketch hidden in the tree (absorbed by a feature), not planes or axes: an axis
of two planes 300 mm out, shown, left the fit as without it. An axis through coordinates (`ref_axis`
`two_points` with `points`) keeps its points in a 3D sketch, shown; that sketch held a 40 mm block at
0.5 % of the image with the axes toggle off, with the axis blanked, and with the sketches toggle off.
So the session's 8 % was a sketch, shown or toggled off, not the axis, and hiding axes changes no fit.
Fix (`ViewService.FitToWhatIsShown`): in a part whose box reaches past its visible bodies' box, the view
is fitted to a selection of what the image shows, `ViewZoomToSelection`, the selection cleared after
(the user's restored as before): per body the faces that reach its box's six extremes, the sketches
shown unless `hideSketches` or the toggle hides them, the planes shown unless `hidePlanes`, and the
reference points and coordinate systems shown; else `ViewZoomtofit2` as before. `view_screenshot`
gains `hideAxes` (default true): the axes toggle off for the image and back after, as `hidePlanes`.
Descriptions: the tool "… zoomed to fit, planes and axes hidden. The user's view, display, visibility,
selection and window are restored after. Use it to check shape and orientation."; `hideSketches` "Hide
sketches; in a part, also from the fit."; `zoomToFit`, `hidePlanes`, `maxSizePx`, `file` and `section`
shortened to pay for it (tools/list 253,588 bytes, 16 under the budget, 7 before).
`docs/solidworks-api-notes.md` (§13) gains the probe. Before, on the code before the fix (the fit
switched back to `ViewZoomtofit2`),
`VisualsLiveTests.Zoom_to_fit_takes_what_the_image_shows_and_axes_are_hidden` failed "with hideSketches
the block's share 0,0053, without the axis 0,2524"; its first form, `hideAxes` alone on an axis through
coordinates 300 mm out, failed "the block's share of the image with an axis 300 mm out: 0,0071, without
it 0,2524", and read 0.0054 with the axes hidden. After: it passes (the block 25.24 % of a 512 × 236
image alone; an axis through it hidden, drawn on 0.073 % of the image with `hideAxes:false`, the toggle
on again after and left off where the document had it off; a hole whose absorbed sketch holds a
centreline 150 mm out, 25.24 %; the axis through coordinates 300 mm out, its 3D sketch shown, 0.86 %,
with `hideSketches` 25.18 %), with `A_section_cuts_the_image_and_is_removed_afterwards`,
`M0LiveTests.Screenshot_returns_a_png_and_restores_display_settings`,
`SimulationLiveTests.Plots_of_kept_studies_show_the_results_range_in_an_image` and
`ContractTests.Tool_catalog_follows_the_conventions`, 5 of 5 live; unit tests 290 of 290;
`--check-catalog` ok; `check_repo` ok. Made stricter: none (`hideSketches` now leaves sketches out of a
part's fit as well as its image; where a part's box is its bodies' the fit is SOLIDWORKS' as before).
Decided 2026-11-16 by the run, not the owner: the fit carries the fix, in a part to what the image
shows, because the probe found toggled-off sketches in SOLIDWORKS' fit and no axis; a sketch shown stays
in it, as it is in the image, so an axis through coordinates still needs `hideSketches:true`, which its
text now says. Decided 2026-11-16 by the run, not the owner: `hideAxes` is added all the same, its text
saying only that it hides them, because axes are drawn in the image as planes are and it is the option
the owner chose; widening `hidePlanes` was not needed, the trims above paying for it. Decided 2026-11-16
by the run, not the owner: assemblies and drawings keep `ViewZoomtofit2`, because a component's sketches
in the fit were not probed and selecting its faces walks every body. Decided 2026-11-16 by the run, not
the owner: the fit by selection only where the part's box reaches past its bodies' (by 1e-6 m), because
elsewhere `ViewZoomtofit2` fits the same and the image stays as it was, and only the faces at each
body's box extremes are selected, because their boxes make the body's and a part of thousands of faces
would take a selection call each. Decided 2026-11-16 by the run, not the owner: the 3D sketch `ref_axis`
keeps for points given as coordinates stays shown, because hiding it changes the reference tools,
outside (d); for the next plan. Local: (d) on `sw2023`, in the commit that records it here; in build `20261116-205845`.
Upstream: not yet decided.
Held in the scenario test of build `20261116-205845` (findings 2026-11-16, points 2 to 5):
(b) the reading, a text at [12, −8] on the Right plane read `atMm` [12, −8], 6 mm, 0°, open
and after finish; (c) the point taken named and the others listed, in `sketch_relation_add`
and `sketch_constrain`, a lone point unwarned; (d) a plane and an axis 300 mm out left the
fit; (e) a 4-decimal polyline and an arc on its ends one closed contour, a later arc merged,
one 0.2 mm off not. Those parts closed 2026-11-16 by the run's rule (*Per group, the
check*: held, closed); the item stays Open for (b)'s move and (f). Two gaps met in the same
test are S87 ((e)) and S88 ((d)).
(g) Sorted out of the entry of 2026-11-17 ("one corner of a multi-edge fillet raised to its
own radius", point 1), on build `20261117-012659`: no way to change the edge set of an
existing fillet. Two edges of a ten-edge constant fillet taking a larger radius cost
`feature_delete` (dry run and real), `feature_rollback {before}`, `feature_fillet` on the
other eight, a new `feature_fillet` on the two, `feature_rollback {to:'end'}`: 6 calls where
an edit of the edges and a new fillet would be 2. Proposed by the session: `feature_fillet
{edit:"<name>", removeEdges, addEdges}` or a `feature_edges_set`. Local. Not decided.
(h) Sorted out of the entry of 2026-11-17 ("two assemblies re-linked after their parts were
moved into a subfolder", point 1), the same build: no call repoints an assembly's component
references to a new folder. Workaround that held: `doc_open {readOnly:true}` of the assembly
listed every missing path in `missingReferences`, then each moved part opened read-only
(one call each), then the assembly opened editable with no load warning and saved in place:
24 opens for 20 parts. Proposed by the session: `doc_relink {from, to}` or `doc_open
{searchFolders}`, two calls. Local. Not decided.
(i) Sorted out of the entry of 2026-11-17 ("an existing printed bracket's copy edited",
point 1), the same build: no change to an existing extrude's end condition. A base extruded
blind from a standard plane had to grow on both sides of a plane 22 from its sketch plane;
`feature_extrude` makes a new extrude only, and `dimension_set` reaches only its depth.
Workaround: the same consumed sketch extruded a second time, `direction:"reverse"`, its
depth driven by an equation: one more feature for one function. Proposed by the session: an
`edit` on `feature_extrude`, as `feature_chamfer` has, taking `end`, a second direction's
depth and a start offset. Local. Not decided.
(j) Sorted out of the same entry, point 2: a dimension's entities are not given.
`sketch_info` and `dimension_list` returned two 6 mm dimensions of one profile sketch (`D1`,
`D2`) with no word of which segments or points each measures; telling the web's thickness
from the flange's took a `dimension_set` probe and a hand-figured volume. Proposed by the
session: each dimension in `sketch_info` (and `dimension_list {sketch}`) names its kind and
the entities it measures (`Line6`-`Line8`, horizontal). Local. Not decided.
(k) Sorted out of the entry of 2026-11-18 ("a two-sheet A3 drawing of a 16-member weldment
part", point 2), on build `20261118-060615`: no tool sets a cut-list item's properties. The
cut-list table's DESCRIPTION column came empty for all 9 items, and nothing sets a cut-list
item's `Description`, so the table could not name its members; a note mapping item numbers
to member letters stood in. Proposed by the session: `model_cut_list` takes `set: [{item,
properties}]`, or `prop_set` takes a cut-list item. Local. Not decided.
(l) Sorted out of the entry of 2026-11-18 ("a custom sheet format made from the ISO one with
`vba_run`", point 4), the same build: no tool sets a mass unit. `$PRPSHEET:"Weight"`, the
part's `Weight` being `"SW-Mass@<part>"`, is evaluated in the drawing's mass unit, not the
part's: the part in kg read `10.49` in `prop_list`, the drawing's note `10489.13` after
`ForceRebuild3`, until the drawing's units were set custom with mass in kg by `vba_run`
(`swUnitSystem` 4, `swUnitsMassPropMass`). `doc_units_set` takes only the four systems.
Proposed by the session: a `massUnit` option on `doc_units_set`. Local. Not decided.
Met again 2026-11-25 ((f); findings 2026-11-25, "seven parts and two assemblies modelled
from a design doc", point 1), build `20261124-174916`: two slots by `{from, to, widthMm}`
came back as 2 lines and 2 arcs each with no relation; each took 4 `tangent`, 1 `equal` and
1 `horizontal` before its centres and width defined it, 20 steps a sketch, 19 to 33 s, on two
plates. Held once added.
(m) Sorted out of the entry of 2026-11-24 ("a multi-body envelope part from a vendor
drawing", point 5), on build `20261124-174916`: no tool makes another drawing sheet current.
`drw_annotation_list {view}` on a view of another sheet was refused "View ... is on sheet
'Sheet1', not on the current sheet 'Sheet2'. Hint: ... switch sheets in SOLIDWORKS."; worked
round with `vba_run` (`ActivateSheet`), one call. Proposed by the session: a `sheet`
argument on the drawing tools, or `drw_sheet_setup {activate}`. Local. Not decided.
(n) Sorted out of the entry of 2026-11-25 ("seven parts and two assemblies modelled from a
design doc", point 2), the same build: no dimension from a sketch point to model geometry.
`sketch_constrain {dimension:"horizontal", entities:[{type:"vertex", at}, {at}]}` was refused
"resolved to vertex V1, but this parameter needs sketch_segment or sketch_point or origin".
Wanted: two holes placed from the tip of a tilted plate, so they follow it. Workaround:
dimensioned from the origin and driven by equations of the tip's globals (4 more equations).
Proposed by the session: a model vertex or edge allowed as a dimension's entity, as
SOLIDWORKS allows. Local. Not decided.
(o) Sorted out of the same entry, point 4: `asm_mate_add {type:"distance"}` takes only a gap
above 0, so a plane offset driven by a variable now at 0 is made `coincident` and must be
replaced when the variable changes. Proposed by the session: a distance of 0 allowed, or the
schema's hint says to use coincident. Local, small. Not decided.
(p) Sorted out of the entry of 2026-11-25 ("a swept shell part with mirrored tabs", point
5), the same build: no change to an existing sketch dimension while its sketch is open.
`dimension_set` refuses an open sketch, and a new dimension on the same point was refused as
over-defining (right); the line carrying the point was deleted and drawn again, 2 calls more.
Proposed by the session: `sketch_constrain` takes an existing dimension by name with a new
value. Local. Not decided.
(q) Sorted out of the entry of 2026-11-26 ("a printed plate revised", point 2), the same
build: `sketch_fillet` takes one corner a call: 9 corners of one closed outline cost 9 calls
(about 1.2k characters each), every one `ok`. Proposed by the session: `corners: [{at,
radiusMm}, ...]`, one call. Local. Not decided.
(r) Sorted out of the entry of 2026-11-26 ("a left part remade in place as the mirror of the
right one", point 1), the same build: `part_derive {kind:"mirror"}` writes a new file only.
A part whose file an assembly already holds (its fixed base) had to keep its name and path,
so the mirror was made in place in 3 calls: `part_derive {kind:"insert"}` of the other part,
`feature_mirror {bodies}` about the Front plane, `body_delete` of the inserted body; each
held (volumes, reflected vertices). What the route's results lack is S157. Proposed by the
session: `part_derive mirror` with `into: <the target document>` doing those three, or a
line in its description that the in-place route exists. Local. Not decided.
(s) Sorted out of the entry of 2026-11-26 ("two printed parts from a written plan", point
1), the same build: `config_table` takes a dimension, `$STATE`, `$PRP`, `$MATERIAL` and
`$CONFIGURATION` columns, but no global variable (SOLIDWORKS' `$VALUE@<name>@equations`). A
bore set per configuration (five values) could not be the global the plan named; the
dimension it drives was configured instead, holding the printed radius (D - 0.3)/2, not D.
Five configurations built and read back right. Proposed by the session: a global-variable
column, so the configured value is the size the plan names. Local. Not decided.
Decided 2026-11-27 by the owner: (m) to (s) taken into the run of 2026-11-27 in their cheapest form: (m) the hint naming `drw_sheet_setup {sheet}` (which makes a sheet current since `200e7a1`), group 5; (n) a model vertex or edge as a sketch dimension's entity, (p) `dimension_set` of the open sketch's dimension made instead of refused, no new option, (q) `sketch_fillet` `corners`, all or none, group 4; (o) a probe of a distance mate of 0, then allowed or the refusal naming `coincident`, (r) a line in `part_derive`'s description for the in-place route, no `into`, (s) a probe of a global-variable column in `config_table`, group 6. (b)'s move and (f) to (l) not in it (`## Run 2026-11-27`).

### S79 - `doc_undo` undoes the step before an `equation_set`, reported `ok`, and is refused after a chamfer split by `flipEdges` - OPEN 2026-11-16

(a) Sorted out of the entry of 2026-11-16, point 9. On build `20261116-080311`: `doc_undo`
right after a `distance_distance` chamfer with `flipEdges:[1]` that moved an edge to a
chamfer of its own (S51, S75) failed `VERIFY_FAILED: Nothing changed: SOLIDWORKS had
nothing to undo, or the last step can't be undone. Hint: Check the model with
feature_list.`; both chamfers stayed. In the same part `doc_undo` of a cut made after it
worked, and of a plain `equal` chamfer in another part. Not reported `ok`; not tried on
the build before. Bears on S51 and S75 (Closed).

Proposed (a): the hint, after a chamfer call that split off a chamfer, names `feature_delete`
of both chamfers; or the chamfer call records the split so `doc_undo` removes both. Local.
Not decided.
Decided 2026-11-16 by the run, not the owner: (a) not in the run of 2026-11-16, because it
is not in group 2's files, is not reported `ok`, and is a hint only.

(b) **A result reported `ok` while wrong.** Sorted out of the entry of 2026-11-16 (the
scenario test of build `20261116-124830`), point 8. `doc_undo` (1 step) right after an
`equation_set` (a variable and an extrude's depth linked to it, no change to the geometry)
in a part whose last feature was a 10 × 10 × 5 boss returned `volumeDeltaMm3: -500`: the
boss was gone and the equation kept (a `dimension_set` on the depth was still refused
naming it). SOLIDWORKS undid the step before the server's last change, which is not an
undo step of its own. The result named no feature removed, though the description says
it "Returns the features removed or restored", and said nothing of the `equation_set`
left in place. Reported `ok`.

Proposed (b): `doc_undo` knows when the server's last change left no undo step (an
`equation_set`, and any other such call) and refuses with what it would undo instead, or
at least names the step it undid and the features it removed. Local. Not decided.
Decided 2026-11-16 by the run, not the owner: (b) stays Open, outside the run of
2026-11-16, marked as a result reported `ok` while wrong for the next plan to take first,
because `doc_undo` is in none of groups 4 and 5's files and the run's groups are set.
Decided 2026-11-21 by the owner: (b) taken into the run of 2026-11-21, group 2: `doc_undo` refuses after a change that left no undo step, naming it and the step it would undo; (a) stays Open (`## Run 2026-11-21`).
**Done in the run of 2026-11-21: (b), split out as S113 (Waiting).** **Left open: (a).**

### S80 - `sketch_add` relates what it draws to model edges and moves it off the coordinates given, with no warning - OPEN 2026-11-16

(a) Sorted out of the entries of 2026-11-16 ("an edit of two mirrored printed parts",
point 1; "a part edited by rollback", point 4), on build `20261116-124830`. A rectangle
drawn on a planar face with corners [[100, 40], [-95, 43]], its bottom on the face's
boundary edge, came back with `relations: ["coincident: a model entity, Line1 start",
"midpoint: Line3 end, a model edge"]`, 2.5 tall where 3 was drawn, and the sketch fully
defined with no dimension; the next `sketch_constrain` dimension was refused as
over-defining. Two rectangles with corners on body edges projected into the sketch plane
got "coincident: a model edge" and "midpoint: Line8 end, a model edge", and one moved: 10
wide drawn, 9 after, a corner pulled to a model edge's midpoint. The relations are listed,
as S56 (Closed) decided; nothing says the solved shape differs from the input, and the
tool's description says inference is off while the server draws. Reported `ok`.
Workaround: draw 1 mm clear of every model edge and place by dimensions, about 3 calls
more a sketch; held six times.
(b) The second entry, point 3: `sketch_add` of a 3-point arc whose centre fell exactly on
an existing corner merged the centre onto it (reported in `merged`), which made the arc's
radius equal to a plate thickness and left the radius dimension redundant. Workaround: the
mid point 0.02 off, so the centre lands near, then tangents and a radius. Bears on S65 (e),
which makes `sketch_add` merge ends that meet.

Proposed: (a) `sketch_add` compares each point it was given with where the solved sketch
has it, and warns, naming the relation, when one moved more than the sketch's tolerance;
(b) the description says a 3-point arc's centre is merged like its ends, or the centre is
not merged; weighed with S65 (e). Local. Not decided.
Met again 2026-11-17 (findings 2026-11-17, "a printed C-profile part, two bands off its
side face", point 1), build `20261117-012659`: a `rectangle` by corners on a planar face,
its two short sides on the face's boundary edges, came back with "coincident: a model edge,
Line1 start" and "coincident: a model entity, Line3 end" (a vertex), the corners as drawn;
the next `sketch_constrain` dimension from the origin to a corner was refused as
over-defining, nothing applied. Here the relations were what the part wanted (the band
follows the base's edges) and were kept: one length dimension then fully defined it, and it
held at a second set of variables; the same on a second band. New: the relations are
sometimes wanted. Proposed by the session for the fix: an option to keep or drop relations
to model geometry, the result naming them as now.
(c) Sorted out of the entry of 2026-11-18 ("a part's outline sketch redrawn and driven",
point 3), on build `20261118-060615`: a `sketch_add` `rectangle` drawn 8 x 8 came back with
`"equal: Line2, Line1"`, a square for good, so a width and a height from different variables
would later conflict. Listed in the result, so a read and not a fault; a closed `polyline`
with horizontal and vertical from `sketch_constrain` held. Proposed: weighed with (a), the
description saying a rectangle drawn square gets an `equal`, or no `equal` added. Local. Not
decided.
Decided 2026-11-21 by the owner: (a) and (c) taken into the run of 2026-11-21, group 7, warnings; (b) stays Open (`## Run 2026-11-21`).
Decided 2026-11-21 in the plan, not the owner: no option to keep or drop relations to model geometry in this run, because it is a new option the owner was not asked, and the warning shows either case.
(a) and (c) are done: moved to Waiting as S140, 2026-11-24 (the run of 2026-11-21, group 7).
(d) Sorted out of the entry of 2026-11-24 ("a multi-body envelope part from a vendor
drawing", point 2), on build `20261124-174916` (before S140's fix): a `rectangle {corners}`
on the Front plane, its corners on the projection of two model edges (a plate's outline over
existing members), failed `FEATURE_FAILED: SOLIDWORKS could not create the rectangle`, the
hint naming degenerate geometry, which it was not. The same four corners as a closed
`polyline` were drawn at once. Likely the inference of (a), here refusing outright rather
than moving the shape. Not reported `ok`. Proposed: a probe of whether the inference to the
projected edges makes the rectangle fail; then the hint names model edges under the corners
as a cause, or the rectangle is drawn as the polyline is. Local. Not decided.
Decided 2026-11-27 by the owner: (d) taken into the run of 2026-11-27, group 1, a probe first; the rectangle then drawn as the polyline is, or the hint naming model edges under its corners; (b) not in it (`## Run 2026-11-27`).
(d) is done: moved to Waiting as S160, 2026-11-27 (the run of 2026-11-27, group 1).
(a) and (c) closed 2026-11-27 as S140, (d) as S160: both held in the scenario test of build
`20261127-085502` (findings 2026-11-27, points 5 and 8). S80 stays Open for (b).

### S81 - "modified outside this server (by the user?)" again after the server's own changes - OPEN 2026-11-16, parked: probed 2026-11-22, not reproduced, taken up when met again

Sorted out of the entries of 2026-11-16 ("an edit of two mirrored printed parts", point 2;
"layout sketches added to an existing part", point 4), on build `20261116-124830`, which
has S57's fix: S57 (Closed) again. (1) `equation_set` and `doc_save` on an assembly warned
"'...SLDASM' was modified outside this server since its last change (by the user?)" on
every call after `doc_activate {allowEdit:true}`, when the only changes were the server's
own: its parts edited and saved earlier in the session, and the assembly dirty from their
rebuild when it was adopted. (2) `feature_delete {deleteEquations:true}`, which held (a
sketch and its 9 equations removed in one call, no rebuild error), warned the same of a
part the user had only been viewing, with no hint of what changed. The owner was not
working in SOLIDWORKS either time. A false alarm that, read literally, stops a session from
saving under the rule never to save while the owner works. Bears on S60 (what a session
cannot tell about the process it runs on).

Proposed: a probe of the stamp the warning compares in the two cases (an assembly adopted
by `doc_activate allowEdit` after its parts' rebuild moved its stamp; a document only
viewed in SOLIDWORKS); the stamp taken at adoption, or the warning saying what changed.
Local. Not decided.
Met again 2026-11-17 (findings 2026-11-17, "an edit to three parts and their assembly",
point 6; "fillets and a trim cut on three parts of an assembly", point 1), build
`20261117-012659`: `doc_save` of an assembly warned "'…' was modified outside this server
since its last change (by the user?)" after this server had saved two (then three) of its
parts and nothing else had touched it; the assembly was dirty only from their rebuild. As
case (1) above, twice more.
Met again 2026-11-18 (findings 2026-11-18, "a two-sheet A3 drawing of a 16-member weldment
part", point 3), build `20261118-060615`: a part saved (`dirty:false`), then
`drw_views_standard` of it warned that it "is open with unsaved changes", and a later
`doc_save` of the part warned "was modified outside this server since its last change (by
the user?)". New: a drawing made from the part, its views on a weldment configuration
(`Default<As Welded>`); not known whether the user edited the part meanwhile or the views
dirtied it. Not probed.
Decided 2026-11-21 by the owner: taken into the run of 2026-11-21, group 3, a probe of the stamp in case (1) first (`## Run 2026-11-21`).
Probed 2026-11-22 on 2023 SP3, case (1), not reproduced: the stamp is taken at adoption already
(`doc_activate` sets it after `ActivateDoc3`, upstream's code), and the assembly's update stamp
(`GetUpdateStamp`, read after every call) did not move when its parts changed or were saved, in
six variants: two 20 mm cubes in the user's assembly (opened outside the server), with and
without a coincident mate that moves one when the other grows; the parts in windows of their
own (the server's) or loaded only by the assembly, adopted by `doc_open` + `doc_activate` or by
`doc_activate {allowEdit:true}` on the listed part; the assembly adopted before or after the
parts' `dimension_set` and `doc_save`; 0 or 3 s idle between calls. Each time the stamp stood
still (106 or 107) until the assembly's own `equation_set` (+1 each), the assembly never read
dirty from its parts, and no call warned. So the cause is not what the plan named: something in
the sessions moved the assembly's stamp between calls (it read dirty "from their rebuild" there,
never here), what is not known: in-context references, features or a configuration the probe's
assembly lacks, a lightweight or large-assembly load, or the owner's SOLIDWORKS options. No fix;
case (2) and the drawing of 2026-11-18 not probed, their cause not shown to be the same. Kept as
a guard: `AssemblyLiveTests.An_assembly_adopted_after_the_server_saved_its_parts_does_not_read_as_changed_by_the_user`
(passes, 1/1; its message lists the stamp after each call). Next: when it is met again, the
session runs `sw_status` and `doc_list` (dirty) before and after the call that warns, and names
what the assembly holds (mates to changed faces, in-context parts, configurations).
Decided 2026-11-22 by the run, not the owner: the probe's test kept, though it does not fail,
because it guards the path case (1) names and its message gives the stamps a repro will need.
Parked 2026-11-27 by the owner (the opening answer of the run of 2026-11-27): probed 2026-11-22 in the run of 2026-11-21 and not reproduced, its guard test kept, not met since; taken up when met again.

### S82 - `sketch_constrain` angular where two lines meet only virtually turns the shape inside out, reported `ok` - OPEN 2026-11-16

(a) **A result reported `ok` while wrong.** Sorted out of the entry of 2026-11-16 ("a part
edited by rollback", point 1), on build `20261116-124830`: S42 (Closed) again. An angular
dimension between a 3 mm line at 20° and a long horizontal line 55 above it, which meet
only virtually far away, `valueDeg` 60 meant as the other line's slope: the line flipped
to 120°, the trapezoid's corners collapsed onto the axis (126 wide became 6), reported
`ok` with "textAt lies in the supplementary angle, so the dimension is on the angle that
was 119.99°... then 60°" (S6's line) and "geometry measures 60°". The geometry did measure
60° on the dimension's own angle, so the check passed a shape turned inside out; only the
`changed` list (a line from 126 to 6 long) showed it, and S42's sign of a line turned by
more than 90° did not fire. Workaround: `sketch_finish`, then `dimension_set` of the same
dimension to 120, which flipped it back. Carried again in another entry of that day ("an
edit to four printed parts", point 7) in a session's own words, an angular dimension's
`textAt` turning a line to the supplementary angle, not seen there; likely the same.
(b) The same entry, point 2: angular 45 at a sketch point made the intersection of two
lines (a lead-in's virtual sharp), drawn at 135° between the rays: the tool took the angle
between the rays, mirrored three segments and said so in a warning (S42's warning, held).
The repair by `dimension_set` 135 after `sketch_finish` then left a perpendicular 3 mm
segment solved to the far side, so the closed profile crossed itself
(`contours.problem: "EntXEnt"`); `dimension_set` reported `ok`, volume 0, since no feature
used the sketch yet. Workaround: the sketch drawn again with the angle dimensioned first,
at the value between the rays as drawn (135), before any distance; held.

Proposed by the session: (a) a warning when a `changed` length drops by more than half, or
a refusal when `textAt` names one sector and the value moves the line into the other (S42
was decided a warning, not a refusal, by the owner); (b) `dimension_set` on a sketch
dimension reads the sketch's contours after and warns when one became invalid. Local. Not
decided.
Decided 2026-11-21 by the owner: (a) taken into the run of 2026-11-21, group 7, a warning as S42 was decided; (b) stays Open (`## Run 2026-11-21`).
(a) is done: moved to Waiting as S141, 2026-11-24 (the run of 2026-11-21, group 7).
(a) closed 2026-11-27 as S141: held in the scenario test of build `20261127-085502`
(findings 2026-11-27, point 6). S82 stays Open for (b).

### S85 - `feature_delete`'s dry run lists as kept an absorbed sketch the real call removes - OPEN 2026-11-16

Sorted out of the entry of 2026-11-16 ("an edit to three printed parts", point 4), on build
`20261116-124830`. The dry run of a delete of a loft cut said `keptAbsorbed: ["...start
sketch", "...run sketch<2>"]`; the real call removed the `<2>` one and said so plainly:
"SOLIDWORKS also removed '...<2>' (absorbed by ...), which was not planned." The `<2>` is
the loft's reference to a sketch another feature owns. Not a wrong result, but the dry run
did not say what the call would do. Bears on S64 (d) and S54 (Closed parts and item).

Proposed: a probe of how SOLIDWORKS lists such a reference; the plan counts it as going
with the feature that holds it. Local. Not decided.
Met again 2026-11-17 (findings 2026-11-17, "an existing printed bracket's copy edited",
point 3), build `20261117-012659`, and its cause seen: `feature_extrude` of an already
consumed sketch returned `"consumedSketch":"<the profile sketch>"`, while `feature_list` then
showed the new feature's child as `<the profile sketch><3>`, a derived copy SOLIDWORKS makes
for a shared sketch. Deleting a first such extrude warned "SOLIDWORKS also removed '<the
profile sketch><2>' (absorbed by ...), which was not planned": the copy the server's own
extrude had made. Nothing was lost (the volume came back exactly). Proposed by the session:
the extrude result names the derived copy when the sketch was already consumed, and the
delete's dry run lists it as absorbed, not unplanned.

### S86 - `sketch_delete` removes a polygon's construction circle unasked, with its dimensions and relations - OPEN 2026-11-16

Sorted out of the entry of 2026-11-16 ("an edit to two printed parts", point 3), on build
`20261116-124830`. (a) `sketch_delete` of a polygon's six lines also removed its
construction circle, which carried the polygon's centre dimensions, its diameter and an
`equal` to the other polygon's circle: `"removed":["Arc1","Line1",...,"Line6"]` while the
checks said "6 segment(s) gone". The other polygon was left with no size (under-defined).
The result lists it; a warning that a segment not asked for went, with the dimensions and
relations that went with it, would have saved a `sketch_info`. (b) The new dimensions took
the freed names (`D1`, `D2`, `D3`), so the old equations on `D2` and `D3` bound to them
again; right here by luck of order. The entry does not say whether S54 (c)'s pick-up
warning came (S54, Closed).

Proposed: (a) a warning naming each segment removed that was not named, with the
dimensions and relations that went with it; (b) a probe that S54 (c)'s warning fires after
`sketch_delete` and a new dimension of a freed name. Local. Not decided.
Met again 2026-11-17 ((b); findings 2026-11-17, "a part's two plates cut back by
redrawing one sketch", point 2), build `20261117-012659`: a sketch redrawn reuses the
dimension names `D1@`, `D2@`... in the order the new dimensions are added, so an old name can
point at another size (`D5@<sketch>` was a 12.2 bound to a global before, a 20 after).
Deleting the equation before deleting the geometry held, with no rebuild error 51, and the new
binding went to the name `sketch_constrain` returned. Not said whether S54 (c)'s warning
fired; the order the session used avoided the case.
Decided 2026-11-21 by the owner: (a) taken into the run of 2026-11-21, group 7; (b) stays Open (`## Run 2026-11-21`).
(a) is done: moved to Waiting as S142, 2026-11-24 (the run of 2026-11-21, group 7).
(a) closed 2026-11-27 as S142: held in the scenario test of build `20261127-085502`
(findings 2026-11-27, point 7). S86 stays Open for (b).

### S92 - SOLIDWORKS' state that refuses every equation (S58 (b)) again: hints that blame the expression, `equation_list` wrong, a restart not told - OPEN 2026-11-18

Sorted out of four entries of 2026-11-17 ("an edit to three parts and their assembly",
points 1 to 3; "a strip and its children deleted", point 1; "a part's hole positions driven
by globals after a restart", point 1; "a turned printed part with two swept internal
threads", point 1), on build `20261117-012659`, which has S58 (b)'s message (Closed). Where
that message fired ("SOLIDWORKS rejected the equation ... though the equation is valid ...
restart") it was right; a restart (a new process) cleared the state each time. Five
sessions met it on one day. What is new:
(a) In that state other refusals blame the expression. `equation_set` of three globals that
already existed failed `VERIFY_FAILED` "SOLIDWORKS kept the previous equation "<var>"= 100mm
and did not take 73.5mm (a value the dimension can't have, such as 0?)", hint "Check the
expression (units, quoted names, existing variables)"; a dimension equation a sketch already
had failed `VERIFY_FAILED: ... the equation evaluates to 0`, rolled back. Not reported `ok`.
(b) **A result reported `ok` while wrong.** `equation_list` in that state, before any write,
read every global set to a number truncated (`= 60.4mm` as 60, 3.4 as 3, 2.5 as 2,
`59.614mm` as 59) and `0` for every row defined from others, with no warning; after a
restart the same model read right.
(c) What brings it on is not known. Met at a session's first write on a new empty part, and
after some 80 calls in a part where a `feature_delete {deleteAbsorbed, deleteEquations}` of
a sweep, a `feature_rollback`, a new sketch inserted and renamed to the deleted one's name
had gone before (equations on that sketch's dimensions refused, a plain `30.47039mm` too,
while `"Above" = 4mm` on an existing variable was taken). One entry names a long session of
many `equation_set` and `feature_delete {deleteEquations}` across parts.
(d) The restart. No tool exits SOLIDWORKS: it was `Stop-Process` from the shell, every
document saved first. `CloseMainWindow` with nothing dirty closed the window and left the
process running, most likely held by the COM references of the `solidworks-mcp` processes
`sw_status.otherServerPids` listed. Once the user said SOLIDWORKS had been restarted when it
had not: its start time, read outside the server, was 44 min before the first refusal. After
a real restart `sw_connect {launch:true}` said "a new one (before: pid …)" (S60 (c), held).
Not restarted in one session (sixteen documents open): the dimension was left a plain value.

Proposed by the sessions: (a) the `VERIFY_FAILED` path tries one plain equation on a scratch
part, or names the restart, before blaming the expression; (b) `equation_list` and
`equation_set` warn when a global set to a number reads back another value, which shows the
state before the first write too; (d) `sw_status` gives SOLIDWORKS' process start time beside
its pid and the refusal names it, so "restarted?" is read, not asked; an exit tool that
refuses with a dirty document open; the hint may say that closing the window can leave the
process alive under another server's references. Local. Not decided.
Met again 2026-11-18 ((b) and (c); findings 2026-11-18: "an edit to a printed plate", point
1; "a part's outline sketch redrawn and driven", point 1; "a shared global variable changed
in two parts", points 1, 3 and 4), build `20261118-060615`, six times in three sessions.
(b): `equation_list` read every global truncated (`60.4mm` as 60, `0.5mm` as 0) and every
derived row 0, reported `ok`, at a session's start before any write (`sw_status.otherServerPids`
listing a second server process), and mid-session, where it was read only after an
`equation_set` was refused (about 6 calls and a restart lost; taken as the sign, the restart
would have come before any write). A restart cleared it each time (`sw_connect
{launch:true}`: "a new one (before: pid ...)", S60 (c) held). Each session asked for (b)'s
warning. (c), a lead: in the third session the state followed, four times, a step that
loaded an assembly holding the part: opened read-only by the server and closed; opened by
the user in SOLIDWORKS' own window; opened editable by the server, a mate dimension set,
`asm_interference` run, closed with `discard`. The next `equation_list` read wrong at once,
the geometry unchanged (`body_list` the same volume). Not proved. Proposed by the session: a
live test, a part with a global of `60.4mm` read, an assembly holding it opened and closed,
read again; and until fixed, a usage line: edit the parts first and open the assembly last.
Decided 2026-11-21 by the owner: (c) a probe first, (a) and (b) taken into the run of 2026-11-21, group 2, (d) taken into the run of 2026-11-21, group 3 (`## Run 2026-11-21`).
Decided 2026-11-21 in the plan, not the owner: (a) names the restart from (b)'s sign, read from the document's own globals, with no plain equation tried on a scratch part, because that would make and close a document in the user's session at every refusal; (d) has no tool that exits SOLIDWORKS, because ending it is the owner's call and the opening question had no room for it.
Probed 2026-11-21 by the run ((c)): **not reproduced.** On build `20261121-205309`, in a
SOLIDWORKS 2023 SP3 process about 10 min old, no other server process, through the
server's own tools: a part (a 100 × 60 × 20 boss) with `"g" = 60.4mm`, `"k" = 2.5mm`,
`"h" = "g" / 2` and `D1@Boss-Extrude1 = "k" * 8`, saved; `equation_list` read 60.4, 2.5,
30.2, 20. Then, the part open throughout, each step followed by `equation_list` on it: an
assembly made of it (two instances, one fixed, a distance mate of 10 between their top
planes), saved and closed; that assembly opened read-only and closed; opened editable,
the mate's `D1@Distance1` set to 12.5, `asm_interference` (one pair, 31500 mm³), closed
with `discard`. Again with the part closed: the assembly opened read-only and closed (the
part went with it), opened editable, the mate set to 7.5, `asm_interference` (52500 mm³),
closed with `discard`, the part reopened. Every read gave 60.4, 2.5, 30.2, 20, and an
`equation_set` of `"m" = 1.5mm` after held. So loading an assembly that holds the part
does not bring the state on by itself, in the server's calls; the user's open in
SOLIDWORKS' own window (the lead's case (i)) and a long session were not tried.
A lead the probe adds, not proved: every wrong read in the entries is the number cut at
its decimal point (`73.5mm` as 73, not 74; `59.614mm` as 59; `0.5mm` as 0), and the
refusals in that state hit numbers with a decimal point (`73.5mm`, `30.47039mm`,
`59.614mm`, `0.5mm`) while `"Above" = 4mm` was taken, as an expression parser would that
had switched to the Windows decimal separator, here a comma. Against it: plain `6mm` and
`60mm` globals were refused too (S58 (b), and two entries of 2026-11-17). In the good
state `"q" = 60,4mm` is refused (`FEATURE_FAILED`, the syntax hint). A broadcast
`WM_SETTINGCHANGE` "intl" (settings unchanged) sent to the running SOLIDWORKS did not
bring the state on (the part read right after, and `"m" = 1.5mm` held). The test that
would settle it, for the next session that meets the state: `equation_set "q" = 60,4mm`
on a scratch part; taken there, the cause is the separator. No server-side way round
found: (c) stays Open, and (a) and (b) go on with unit tests, with no live case of the
state.
**Done in the run of 2026-11-21: (a) and (b), split out as S112 (Waiting).** **Done in the
run of 2026-11-21, group 3: (d), split out as S116 (Waiting).** **Left open: (c).**
Met again 2026-11-22 (findings 2026-11-22, the scenario test of build `20261122-030022`,
point 6), and (c)'s separator test run: **the separator lead fails.** In the state, on a
new part, `"q" = 60,4mm` was refused `FEATURE_FAILED` with the syntax hint and `"m" = 6mm`
with the restart message, so neither the comma form nor a whole number is taken: not a
decimal separator switch. What ran before in that SOLIDWORKS process (about 4 min old at
the session's first call, no equation read or written before the bad read, so when the
state came on is not known): a part with two extrudes, `dimension_set`, `feature_rename`,
`feature_suppress_set` and `doc_undo`; a two-part assembly opened read-only, a mate
suppressed and the suppress undone, a component's dimension set and set back, closed
discarded; a part opened read-only, force rebuild, STEP export and the STEP imported; a
drawing with a plan and two sections, discarded; a second assembly read-only,
`asm_interference`; a second server process of an older build alive throughout. As in the
lead of 2026-11-18, an assembly holding a part had been loaded and a component dimension
set before the bad read. Point 5's case (whole-number globals, every derived row read 0
with no warning) is S118. The run's own group 3 release met the state once too: in the full
`AssemblyLiveTests` run an equation `adoptW` = 12 was refused with S112's message, after
earlier tests in the same SOLIDWORKS, and passed rerun alone after a restart.
Met again 2026-11-25 (findings 2026-11-25, "reading two parts the owner had edited by
hand", point 1), build `20261124-174916`, SOLIDWORKS about 2 h old, no write made: in two
parts every global with a negative constant read 0 (`-30.578mm`, `-16.284mm`, `-6mm`,
`-66mm`), every positive one right (`50deg`, `30mm`, `46mm`), and every derived row only its
literal part (`"X" + 10mm` read 10, `-"Y" - 9mm` read 0), as if each variable on a right-hand
side were 0 and a negative result clamped to 0; `dimension_list` in the same minute read
every driven dimension right. S112's warning fired (on the negatives); a session checking a
positive global alone would have missed the state. New for (c): the state reads negatives as
0, not only decimals cut at the point.
A line for (c) 2026-11-25 (findings 2026-11-25, "a new part from a face sketch", point 2):
on SOLIDWORKS freshly restarted (18:42) the state was not there: negative globals read right
(`-103.5`, `-36`), and `equation_set` of seven globals, three negative, on a new part read
back right.
Met again 2026-11-25 (findings 2026-11-25, "a swept shell part with mirrored tabs", point 1),
the same build, in that same process about 1 h 50 later: `equation_set` of three positive
globals on a new empty part refused with the restart message, again after its first feature;
`equation_list` of an existing part read its negative globals 0 and every dimension row 0,
with S112's warning (right). New: three rebuilds of that part (an outline sketch edited, a
cut added) did not apply the zeros, its thickness stayed 6 and every volume matched the hand
figure (+38 764.1, -190.852): in the state the reads are wrong and the writes refused, while
the stored values hold through rebuilds. For (c): in both entries of the day the state was
met about two hours into the process, after sketch edits, mates and saves, no restart
between. Workaround: no equations, every size typed.

### S94 - `feature_helix` and `feature_sweep`: where a helix starts, and a profile off the start of a tapered helix, not in the descriptions - OPEN 2026-11-18

(a) Sorted out of the entry of 2026-11-17 ("a turned printed part with a flange and a swept
internal thread", point 2), on build `20261117-012659`: the same inputs as a helix built
earlier that day (a circle at the origin on a planar face with normal +Y, frame xAxis
[1,0,0], yAxis [0,0,-1]; `height_pitch`, `clockwise:true`, `startAngleDeg` 0): that one
started at +Z, this one at -X (`startMm` [-29.25, …, 0], `profilePlaneNormal` [0,0,1]). The
description says 0 is at Z for a top-like sketch. The part had no phase to keep, so the
profile went on the plane the result named and the volume matched. Read `startMm`, not the
description. Proposed by the session: find what sets the start (the circle's start point?)
and say it, or measure `startAngleDeg` from a stated axis whatever the circle.
(b) Sorted out of the entry of 2026-11-17 ("a turned printed part with two swept internal
threads", point 2), the same build: `feature_sweep` along a tapered `feature_helix` carries
the profile without tilting with the taper, so a profile placed above or below the helix's
start gives a thread whose depth is off by that height × tan(taper): about 0.03 mm deep
against the taper, 120 mm³ on one thread, for a profile 1 mm below; drawn with each point's
radius on the taper, it matched. Reported `ok` both ways, rightly: the geometry is
SOLIDWORKS'. Proposed by the session: one line in a description ("a profile off the start of
a tapered helix: draw its points on the taper").

Local, descriptions (the tool list is at its byte budget). Not decided.

### S96 - `sketch_constrain` does not check a relation to a model edge - OPEN 2026-11-18

Sorted out of the entry of 2026-11-17 ("three printed parts of a box and its lid", point 3),
on build `20261117-012659`. `sketch_constrain {relation:"concentric"}` between a sketch
circle and a model circular edge `{at, type:"edge"}` lying in the sketch's face: `checks`
"geometry not checked (model geometry, or a segment of a non-parallel sketch)", relation
added, `ok`. It was right (read after the cut by `face_list {surface:"cylinder", radiusMm}`),
but it is the check a session needs most for a relation to a model edge.

Proposed by the session: for a circular edge parallel to the sketch, the check reads the
edge's centre and radius against the sketch circle's centre. Local. Not decided.

### S99 - `sketch_import` of a DXF whose geometry is in a block, and an imported sketch `sketch_edit` refuses - OPEN 2026-11-18

(a) Sorted out of the entry of 2026-11-17 ("a weldment profile made from a vendor's DXF",
point 1), on build `20261117-012659`: an R12 DXF whose geometry sits in a BLOCK placed by one
INSERT (ENTITIES holds only the INSERT at 0,0, scale 1) failed `FEATURE_FAILED: The sketch
from '...' has no segments; it was removed. Hint: Check the drawing's layers and units.` The
hint points at layers and units; the cause is the block. Workaround: the block's 106 LINE and
8 ARC entities written flat into ENTITIES by a short script; the import then read 114
segments, and a 100 mm extrude of it 29 998.945 mm³ against 29 998.9 figured from the loops.
Proposed by the session: explode INSERTs on import, or say the drawing holds only block
references.
(b) The same entry, point 2: `sketch_edit` refused the imported sketch twice, `FEATURE_FAILED:
SOLIDWORKS did not open sketch '<name>' for editing. Hint: It may be suppressed, rolled back
or derived`; none of the three held (renamed by `feature_rename`, its test extrude deleted,
kept, not suppressed, `feature_list` showing it as `ProfileFeature` under the Front Plane).
So its 114 segments could not be fixed or dimensioned. Not probed whether the rename (a name
starting with a digit) or the import is the cause. Proposed: a probe of which.

Local. Not decided.

### S105 - No tool hides or shows reference geometry and sketches - OPEN 2026-11-21

Sorted out of the entry of 2026-11-18 ("a weldment frame of 16 members in one custom
profile", point 6), on build `20261118-060615`. At the end of a part the owner wanted every
reference plane, path sketch and structural member's profile sketch hidden; no tool does it
(`view_screenshot`'s `hidePlanes` and `hideSketches` hide them for the picture only, and
restore them), and `vba_run` was off. Left to the owner by hand: 5 planes, 5 path sketches,
9 profile sketches.

Proposed by the session: a `feature_visibility {features | kinds:["plane", "sketch",
"profile_sketch"], show}` that hides by name or by kind (a member's profile sketch a kind of
its own), each feature's visibility read back; or a `hide` option on `doc_save`. Local. Not
decided.

### S109 - `NOT_CONNECTED` after `asm_component_move` and `doc_rebuild` on a subassembly open beside its parent, SOLIDWORKS responding - OPEN 2026-11-21, parked: probed 2026-11-22, not reproduced, taken up when met again

Sorted out of the entry of 2026-11-18 ("a shared global variable changed in two parts",
point 6), on build `20261118-060615`. In an assembly's limit-mate configuration a component
was moved twice (13.2 s and 14.5 s), each followed by `doc_rebuild` (the second 6.7 s); the
next call, `asm_component_list`, failed `NOT_CONNECTED` "SOLIDWORKS is running (pid ...) but
can't be reached over COM". The process answered `Responding: True`, its window title naming
the subassembly as dirty. The user brought it back by hand; what it showed is not known. The
hint ("still starting", "different privileges") fits neither. Bears on S60 (b) (Closed part)
and S89 (the `NOT_CONNECTED` hint).

Proposed: a probe, the same moves and rebuilds on a subassembly open beside its parent,
`sw_status` and `dialog_list` when the connection goes; the hint names a modal dialog as a
cause where one may be up. Local. Not decided.
Decided 2026-11-21 by the owner: taken into the run of 2026-11-21, group 3, a probe; the hint names a modal dialog where one is up (`## Run 2026-11-21`).
Probed 2026-11-22 by the run, not reproduced. SOLIDWORKS 2023 SP3 started from its exe;
the live test `AssemblyLiveTests.Moves_and_rebuilds_in_a_limit_mate_configuration_of_a_subassembly_open_beside_its_parent_keep_the_connection`:
a subassembly of two 20 mm cubes, a configuration added and active, a limit distance mate in
it (20..72.5, then as in the finding its limits moved to 15..72.5 by
`dimension_tolerance_set` and its nominal set to 15), saved and left open; a parent holding
it and a third cube, with a coincident mate. After each call the session's attachment and
pid and the dialogs up were read: in the subassembly `asm_component_move` (2.2 s),
`doc_rebuild` (1.2 s), a second move (1.5 s), a forced rebuild (1.2 s),
`asm_component_list` (1.1 s); in the parent a move of its part (1.8 s), a rebuild (1.2 s),
a second move (1.5 s), a forced rebuild (1.1 s), `asm_component_list` through every level
(1.6 s), the subassembly's list again (1.3 s). Every call `ok`, attached throughout to the
same pid, no dialog after any. The finding's moves took 13 to 15 s each, so its assembly
was heavier or SOLIDWORKS short of memory (a 77 MB working set). A lead: the same entry's
SOLIDWORKS had been started by `sw_connect {launch:true}` on that build (its points 1 and
3), which ran it with `-Embedding`, out of the running object table (S89); once the server
let go of its reference, no attach could reach it again, which is the message seen. S89's
fix (`46920a1`) launches the exe and names such an instance in the hint. What made the
server let go (a disconnect error on the call after the rebuild) is not known. No server
change: no dialog was found for the hint to name. Met again: `sw_status` and `dialog_list`
read at once, and SOLIDWORKS' command line (`-Embedding` or not).
Decided 2026-11-22 by the run, not the owner: the probe's live test kept as the guard of the
sequence (as S81's), because it is the only live case of moves and rebuilds beside an open
parent and costs one test's time in the area.
Parked 2026-11-27 by the owner (the opening answer of the run of 2026-11-27): probed 2026-11-22 in the run of 2026-11-21 and not reproduced, its guard test kept, not met since; taken up when met again.

### S111 - `asm_mate_list` gives a limit mate's distance and dimension but not its limits - OPEN 2026-11-21

Sorted out of the entry of 2026-11-21 (the scenario test of build `20261121-205309`), point 5.
`asm_mate_list` on an assembly with a limit distance mate gave its distance and its
dimension but not its limits (minimum and maximum); they were readable only through
`dimension_tolerance_set`'s `previous` or a refused `dimension_set` (S110's message). A
missing field, not reported `ok` while wrong.

Proposed: `asm_mate_list` gives a limit mate's limits beside its distance (the distance
mate's `MinimumDistance` and `MaximumDistance`, an angle mate's `MinimumAngle` and
`MaximumAngle`, which S110's refusal already reads). Local, small: not in this run, for
the next plan. Not decided.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 3: the limits beside the distance, as proposed (`## Run 2026-11-27`).

### S115 - `dimension_list` on an assembly lists no dimensions, though a component's dimension is set from there - OPEN 2026-11-22

Sorted out of the entry of 2026-11-22 (the scenario test of build `20261122-002030`), point 6.
`dimension_list` on a two-part assembly returned `"dimensions":[], "total":0`, though
`dimension_set` sets a component's dimension from there by `D1@<feature>@<component>-1.Part`;
the name had to be known beforehand. A missing option, not reported `ok` while wrong.

Proposed: `dimension_list` on an assembly lists the components' dimensions by the names
`dimension_set` takes there (all, or one component's), beside the assembly's own. Local,
small: not in this run, for the next plan. Not decided.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 3: the components' dimensions by the names `dimension_set` takes, capped, no new option (`## Run 2026-11-27`).

### S129 - An auxiliary view's arrow and letter are drawn about 90 mm from the edge it was made from - OPEN 2026-11-22

Sorted out of the entry of 2026-11-22 (the scenario test of build `20261122-194424`), point
8. The auxiliary view of S1's proof (point 5 (b)), square to a straight floor edge at 24.23°
in a 1:2 plan: its arrow and letter were drawn at about [178, 101], inside the section
view's area, about 90 mm from that edge in the plan; the view's label sat at y ≈ 35, 7 mm
under the view. Nothing in the result said so. In the SOLIDWORKS UI the arrow comes up next
to the selected edge. May be SOLIDWORKS' own placement rather than the server's.

Proposed: a probe first: where the API puts the arrow for the edge the call picks, whether
the pick point decides it, and whether the arrow can be moved next to the edge, or its
position read and warned of when it lies on another view. Local, in the drawing code. Not
decided.
Decided 2026-11-22 by the run, not the owner: left out of the run, by the run's rule "a new
point needing the owner (a choice, a large change), a new Open item, not in this run",
because it needs a probe first and may be SOLIDWORKS' own: for the next plan.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 5: a probe first (an extra the owner chose) (`## Run 2026-11-27`).

### S134 - `doc_save` in place of a library feature part already known dissolved overwrites the only good file before it refuses - OPEN 2026-11-24

Sorted out of the entry of 2026-11-24 (the scenario test of build `20261124-174916`), point
3, its small part; bears on S100 (held and closed by the same point). On an owned copy of a
profile opened by `doc_open`, `doc_save {mode:"copy"}` to a `.sldprt` was `VERIFY_FAILED`
naming the file written and "SOLIDWORKS dissolved the open part's library feature (it held
Sketch1)", with the hint to close without saving; a `doc_save` in place after it was
`VERIFY_FAILED` "written without a sketch in its library feature", but by then the `.sldlfp`
on disk, the only good file (a document opened by `doc_open` has no backup), had been
overwritten, while the hint said "open the last good copy (the file before, or
doc_activate's backup)". The server already knew the open document dissolved from the first
call, so the save in place could have been refused before writing. Not reported `ok`.

Local, small, in group 6's files (`DocumentService.cs` saves).
Decided 2026-11-24 by the run, not the owner: a save in place of a part already known
dissolved is refused before writing, the file on disk left as it was. Test to start from: an
owned copy of a shipped profile opened by `doc_open`, `doc_save {mode:"copy"}` to a
`.sldprt`, then `doc_save` in place: refused, the `.sldlfp` on disk unchanged and reopening
with `LibFeat1` holding its sketch (fails now: written without its sketch, then
`VERIFY_FAILED`; the second save of
`WeldmentLiveTests.A_save_that_leaves_a_library_feature_part_without_its_sketch_is_verify_failed`
expects that write and changes with it). Default model.
Decided 2026-11-24 by the run, not the owner: taken into group 7, by the run's rule "a new
point, small and in the group's files, a later step of this run ... taken into the next
group", because it bears on S100 (d)'s own check and risks the user's only good file.
Carried 2026-11-27 into the run of 2026-11-27, group 5, its decision as written: the run of 2026-11-21 closed on the owner's word (`## Run 2026-11-27`).

### S135 - A hole table with no `atMm` is placed over the title block: the free-space search does not place tables - OPEN 2026-11-24

Sorted out of the entry of 2026-11-24 (the scenario test of build `20261124-174916`), point
10; bears on S128 (held and closed by point 7 of the same entry). `drw_table_add
{kind:"hole"}` on the top plan of a 1:2 drawing with no `atMm` (the description: "default
below the view") went to [150.8, 45.0, 295.6, 118.8], over the title block, with an overlap
warning. The free-space search S128 gave the views does not place tables. Warned, so nothing
hidden; small.

Local, small, in group 6's files (`DrawingService.Tables.cs`, `DrawingService.Placement.cs`).
Decided 2026-11-24 by the run, not the owner: a table with no `atMm` is placed by the same
free-space search, clear of the title block (and of the views, the border and the other
tables), or, where no spot is free, warned with a hint (`atMm`). Test to start from: point
10's case, a hole table with no `atMm` under a plan whose space below reaches the title
block: placed clear of it, or the hint (fails now: over the title block, only the overlap
warned). Default model.
Decided 2026-11-24 by the run, not the owner: taken into group 7, by the run's rule "a new
point, small and in the group's files, a later step of this run ... taken into the next
group", because it extends S128's placement to tables, in the drawing files group 6 changed,
needing no choice of the owner's.
Carried 2026-11-27 into the run of 2026-11-27, group 5, its decision as written: the run of 2026-11-21 closed on the owner's word (`## Run 2026-11-27`).

### S136 - A section's cut line letters overlap a table and another section's label, the result saying "no overlaps", reported `ok` - OPEN 2026-11-24

Sorted out of the entry of 2026-11-24 (the scenario test of build `20261124-174916`), point
11; S14 again (closed: its overlap checks left a section line's letters in the parent view
uncovered) and S106 (closed: the overlap checks count tables). The vertical cut of S130's
proof (point 8), drawn after a hole table, put its lower letter on the table's header row
and its upper letter on another section's label (in the PDF), while its result said "no
overlaps with other views or their annotations". Reported `ok` while wrong.

Proposed: a probe first: whether SOLIDWORKS lists a section line's letters as annotations of
the parent view, or gives their place another way (the section line's ends), so the check
can count them and warn, or the letters be moved. Local, in the drawing code. Not decided.
Decided 2026-11-24 by the run, not the owner: left out of the run, by the run's rule "a new
point needing the owner (a choice, a large change), a new Open item, not in this run",
because it needs a probe first, as S129: for the next plan.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 5: a probe first, the letters then counted by the overlap check and warned of (an extra the owner chose) (`## Run 2026-11-27`).

### S137 - `doc_close` of a part an open drawing uses is `VERIFY_FAILED` with a hint of a dialog, not of the drawing - OPEN 2026-11-24

Sorted out of the entry of 2026-11-24 (the scenario test of build `20261124-174916`), point
12. `doc_close {discard:true}` on an owned part whose drawing was still open:
`VERIFY_FAILED` "... is still open after CloseDoc. Hint: A dialog may have interrupted the
close; check dialog_list." No dialog was up; the open drawing held the part. `doc_list` then
showed the part `dirty: false`, its changes gone, and closing the drawing later said
"SOLIDWORKS also closed: <part>". Not reported `ok`; the hint misses the cause, and nothing
said the changes were discarded though the part stayed open.

Proposed: the error names the open documents that use the part ("the drawing <name> uses it;
close it first, or close the part with it") and says whether the discard took. Local, in
`DocumentService.cs`. Not decided.
Decided 2026-11-24 by the run, not the owner: not taken into this run, because it is a
behaviour group 6 did not change, found beside its proof, and group 7 is the owner's extras:
for the next plan.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 5: the error names the documents that use the part and says whether the discard took (an extra the owner chose) (`## Run 2026-11-27`).

### S138 - `doc_activate {allowEdit:true}` on a document opened read-only does not say that `allowEdit` did not apply - OPEN 2026-11-24

Sorted out of the entry of 2026-11-24 (the scenario test of build `20261124-174916`), point
13; bears on S98 (closed: a read-only document's way back to editing is `doc_close` and
`doc_open` without `readOnly`). `doc_activate {allowEdit:true}` on a document opened
`readOnly:true`: "Active and targeted.", `access` still `readOnly`; nothing said `allowEdit`
did not apply, or how to edit it. Small; the access read right.

Proposed: a warning that `allowEdit` does not apply to a document opened read-only, with
S98's reopen hint. Local, small, in `DocumentService.cs`. Not decided.
Decided 2026-11-24 by the run, not the owner: not taken into this run, because it is a
behaviour group 6 did not change, found beside its proof, and group 7 is the owner's extras:
for the next plan.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 5: a warning with S98's reopen hint (an extra the owner chose) (`## Run 2026-11-27`).

### S139 - A drawing saved by `doc_save` as and exported to PDF reads `dirty: true`, and `doc_close` is refused `DIRTY` - OPEN 2026-11-24

Sorted out of the entry of 2026-11-24 (the scenario test of build `20261124-174916`), point
14. A drawing saved by `doc_save {mode:"as"}` and then exported to PDF read `dirty: true`,
and `doc_close` was refused `DIRTY`, with no change made in between. Which call set the flag
(the save as, the export) is not known. Not S60 (a), the other way round (there
`dirty:false` over unsaved work). Small.

Proposed: a probe first: which call sets a drawing's flag after a save; then the call that
sets it says so, or leaves the flag as it found it. Local. Not decided.
Decided 2026-11-24 by the run, not the owner: not taken into this run, because it is a
behaviour group 6 did not change, found beside its proof, and group 7 is the owner's extras:
for the next plan.
Met again 2026-11-24 (findings 2026-11-24, "a multi-body envelope part from a vendor
drawing", point 6), build `20261124-174916`, on a part: `doc_close {save:false}` of a
weldment part saved in place earlier was refused `DIRTY` after its drawing had been opened,
rebuilt, saved and closed, the part not edited since its save; closed with `save: true`.
Likely the drawing's rebuild updating the cut list. New: a part a drawing uses, not only the
drawing, reads dirty after calls that edited nothing in it.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 5: a probe first of which call sets the flag (an extra the owner chose) (`## Run 2026-11-27`).

### S143 - `sketch_constrain` reads a sketch under-defined, then refuses its dimensions as over-defining - OPEN 2026-11-27

Sorted out of the entry of 2026-11-24 ("a multi-body envelope part from a vendor drawing"),
point 4, on build `20261124-174916`. A closed 8-line profile (3 lines fixed, 2 horizontal, 1
vertical, a midpoint to the origin, 3 lengths) came back from `sketch_constrain` with
`constrained: "under"`; a horizontal and a vertical dimension after it were each refused as
over-defining ("SOLIDWORKS made it a driven reference dimension"). `sketch_info` and
`model_review` still read under; the geometry was right. Either the status or the refusal is
wrong, and nothing said which.

Proposed: a probe of the case: which read gives "under" (the sketch's solve status against
its points' and segments' own), and whether the refused dimensions were redundant to the
fixed lines. Local. Not decided.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 4: a probe first, the status read right or the refusal naming what already defines the entity (`## Run 2026-11-27`).

### S145 - `model_measure` of two parallel cylinders reads `parallel: false`, reported `ok` - OPEN 2026-11-27

Sorted out of the entry of 2026-11-25 ("two printed plates trimmed and their bolt pattern cut
from 2 x 2 to 1 x 2"), point 1, on build `20261124-174916`. Between two cylindrical faces in
an assembly (a Ø5.5 and a Ø5.2 hole, `face_list` giving both axes `[0,0,1]`, and on the other
side both `[0,0,-1]`) `model_measure` returned `centerDistanceMm: 0.00497` and `parallel:
false`, four pairs alike. The axes are parallel; a reader taking the field at its word would
think the holes skewed. Reported `ok` while wrong.

Proposed by the session: for two cylinders (or cones) `parallel` computed from their axes,
or left out where the tool did not compute it. Local. Not decided.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 3: `parallel` from the axes of two cylinders or cones, or left out where not computed (`## Run 2026-11-27`).

### S147 - A sketch dimension `D1` of value 0 that no step made appears in the sketch's and its feature's dimensions - OPEN 2026-11-27

Sorted out of three entries on build `20261124-174916`: 2026-11-25 ("a printed nozzle with a
tilted slot", point 2), 2026-11-25 ("a new part from a face sketch", point 4) and 2026-11-26
("two printed parts from a written plan", point 3). `sketch_info` and `sketch_finish` list a
dimension `D1@<sketch>` of value 0 that no call made, and it is in the extrude's
`dimensions` after. Met: after a `sketch_add` rectangle that SOLIDWORKS related to a model
edge (it stayed after the four lines were deleted and redrawn); after a `horizontal` relation
between two circle centres and dimensions to a converted line (in both sketches, and in a
second part); beside a `sketch_offset`'s `D2 = 2 mm` after a `sketch_convert` of a face's
outer loop. Nothing reads it, `model_review` says nothing of it, and the geometry was right
every time. Reported `ok`. Bears on S123 (a) (`sketch_finish` lists a dimension twice).

Proposed: a probe of what `D1` is (an offset's or a converted entity's own parameter, or a
dimension SOLIDWORKS makes and hides); then the results say what it is, or leave it out.
Local. Not decided.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 4: a probe first, the results then saying what the dimension is or leaving it out (`## Run 2026-11-27`).

### S148 - A subassembly changed in memory reads stale from its open parent, and a forced rebuild dirties every referenced document - OPEN 2026-11-27

Sorted out of the entry of 2026-11-25 ("a printed nozzle with a tilted slot, ... components
replaced and added in a subassembly read from its parent"), points 3 and 4, on build
`20261124-174916`. (a) Three components inserted and mated in an open subassembly, unsaved;
then on its open parent `asm_component_list {allLevels}` listed the subassembly with its old
9 children and `asm_interference` ran over those 9, both `ok`, with no word that the
subassembly had changed in memory. Only after `doc_save` of the subassembly and `doc_rebuild
{force:true}` of the parent did the parent list 12 and the interference cover the new parts:
a check that can pass on stale data. (b) After that forced rebuild (no errors),
`model_review` on the parent gave `needs_rebuild`; a plain `doc_rebuild` (0.27 s) cleared it.
And the forced rebuild left every document the parent references dirty, read-only and
foreign ones included (about 30), so a session must take care to save only its own.

Proposed by the session: (a) the parent's listing and interference warn when a subassembly
is dirty or its child count differs from the subassembly document open beside it; (b) the
forced rebuild's result names the documents it left dirty. Local. Not decided.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 3: (a) the parent's listing and interference warn of a subassembly open and dirty or with another child count, (b) the forced rebuild names the documents it left dirty (`## Run 2026-11-27`).

### S149 - Calls that cost a long session: `feature_rename` one per call, `sw_status` and `doc_list` every document in full - OPEN 2026-11-27

Sorted out of the entry of 2026-11-25 ("a printed nozzle with a tilted slot"), point 5, the
paragraph its main agent added from the session's cost (50 min, 273 calls, 375k tokens for
three new parts, a mount edit and their mates), on build `20261124-174916`. (a) About 15
calls went on renaming sketches and features one per call: `feature_rename` has no batch
form. (b) `sw_status` and `doc_list` each returned 36 documents in full, the user's foreign
ones included, read several times for context beside an all-levels `asm_component_list` of
46 children: an estimated 60 to 80k tokens, where a filter (dirty only, owned only, one
assembly's documents) would have done. The server reported success throughout. The same
paragraph counts about 10 calls on equations set one per call as a missing batch:
`equation_set {equations}` takes one (S37, closed), and the entry before used it, so that
part is the session's, not the server's.

Proposed: (a) `feature_rename` taking a list of renames; (b) a filter on `doc_list` and a
short form of `sw_status`'s document list. Local. Not decided.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 6: (a) `feature_rename` takes `renames`, all or none; (b) `doc_list` takes a filter (owned, dirty) and `sw_status` lists the owned and dirty documents in full and counts the rest (`## Run 2026-11-27`).

### S150 - A leftover `SLDWORKS.exe` of a closed SOLIDWORKS: `NOT_CONNECTED` says "still starting", and `sw_status` may give its start time - OPEN 2026-11-27

Sorted out of two entries of 2026-11-25 on build `20261124-174916`; a case S89's fix
(Waiting) does not cover, and S116 (closed) again. (a) "reconnecting after the owner
restarted SOLIDWORKS", point 1: after the owner closed and restarted SOLIDWORKS, `sw_status`
and `sw_connect` both gave `NOT_CONNECTED: SOLIDWORKS is running (pid …) but can't be reached
over COM: it may still be starting, or it runs with different privileges …`. The one
`SLDWORKS.exe` alive had started 4 h before from its exe (no `-Embedding`), with no main
window (handle 0), a 30 MB working set and `Responding: True`: a SOLIDWORKS that was closed
but whose process never exited, so no fresh one had started. With no `-Embedding`, S89's
hint falls back to "still starting", wrong for a process hours old with no window. Cost: the
run blocked, one owner round trip. (b) "a new part from a face sketch", point 1: with two
`SLDWORKS.exe` alive (a windowless leftover started 14:44 and the one the server was attached
to, started 18:42:33), `sw_status` gave `solidWorksStarted` "2026-11-25 14:44:02", the
leftover's; a later `sw_status` in the same session read 18:42:33. Not proved (it read right
the second time), but a session asking "was it restarted" of this field may be answered by
the wrong process. Reported `ok`.

Proposed by the sessions: (a) a process out of the running object table with no main window
and a start time minutes old or more is reported as a leftover of a closed SOLIDWORKS, the
hint "end SLDWORKS.exe (pid …) and start SOLIDWORKS from its exe"; (b) `sw_status` reports
the attached process's id and start time, and lists any other `SLDWORKS.exe` as leftovers.
Local, in `SessionService` and `SessionNotes`. Not decided.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 5: (a) a windowless process out of the running object table, 3 minutes old or more, named a leftover with its hint, in an interop-free function; (b) `sw_status` reports the attached process's id and start time and lists the others as leftovers (`## Run 2026-11-27`).

### S151 - Things a tool's description does not say: a face converted for dimensions, a mirror part's link, an offset plane's sign - OPEN 2026-11-27

Sorted out of two entries of 2026-11-25 on build `20261124-174916`; none a fault, each a line
a session needed. (a) "a new part from a face sketch", point 3: `sketch_convert` of a whole
angled face, to dimension from its edges, makes the converted lines real geometry, so the
sketch had 3 closed contours and a cut would have removed the face's outline; one more call,
`sketch_construction_set`, made it 2. (b) "two plain plates with a hole cut each", point 2:
`part_derive {kind:"mirror"}` makes a part of one `Mirror part` feature that stays linked to
the source file (a change of the source reaches it); its checks name volume and box only.
(c) The same entry, point 3: `ref_plane {kind:"offset", base: front, offsetMm: -92}` reports
its dimension as `D1@<plane>: 92` with the plane at Z -92, the sign in the flip, so a later
`dimension_set` of 92 reads as the same plane.

Proposed: a clause each in the descriptions of `sketch_convert` (convert a face's edges as
construction to dimension from them), `part_derive` (mirror: linked to the source) and
`ref_plane` (a negative offset is stored as a flip with a positive value), or usage lines.
Usage. Not decided.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 6: a clause each in the three descriptions, paid for by trimming (`## Run 2026-11-27`).

### S152 - `asm_interference` on a component inside a subassembly answers "0 touching" without saying the subassembly's inner pairs are not checked - OPEN 2026-11-27

Sorted out of two entries of 2026-11-25 on build `20261124-174916` ("a new part from a face
sketch", point 5; "two plain plates with a hole cut each", point 1). `asm_interference
{components:[<one part inside a subassembly>], includeCoincident:true}` in the assembly that
holds the subassembly answered `count 0`, `touching 0`, the check line "listed 2 pair(s): 2
overlapping, 0 touching ...; 0 of the 2 involve <the part>". The part touches three to five
neighbours inside its own subassembly (the same call there listed them): the parent does not
check components of one subassembly against each other, and the result does not say so. A
reader takes "0 touching" for a part that rests on nothing. Reported `ok`, true for what it
checked.

Proposed by the sessions: when a component named is inside a subassembly, the result says
that pairs inside it are not checked and names the subassembly. Local. Not decided.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 3: the result says pairs inside a subassembly are not checked from the parent, naming it (`## Run 2026-11-27`).

### S153 - `model_measure` gives nothing for a curve crossing a face, takes no body, and its hint points at a form it refuses - OPEN 2026-11-27

Sorted out of the entry of 2026-11-25 ("a swept shell part with mirrored tabs"), points 2
and 3, on build `20261124-174916`. (a) Between a sketch spline and a cylindrical face it
crosses: `NOT_SUPPORTED` "SOLIDWORKS reported no applicable measurement", twice, where a
distance of 0 (or the crossing point) was expected. Workaround: sketch points on the curve
either side of the crossing, each measured against the face, `centerDistanceMm` against the
radius telling the side (121.73 between 120 and 123); 6 calls where 2 were meant. (b) To a
whole body: `b: {type:'body', at:[...]}` refused "'at' works for face, edge, vertex and
sketch_point; use 'name' for body", then `b: {type:'body', id:'B13'}` refused "needs face or
edge or vertex or plane or axis or point or sketch_segment or sketch_point or origin or
coordsys": the first hint points at a form the tool does not take (S64's kind). Missing: the
distance from an entity to a body (the minimum over its faces), the plain way to read a
clearance; each face was measured instead. Not reported `ok`.

Proposed by the session: (a) when Measure returns nothing for a curve and a face, report
whether they intersect (distance 0); (b) a body allowed as an entity (minimum over its
faces), or the hint not pointing at `name`. Local. Not decided.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 3: (a) a probe first, distance 0 and the crossing point where the curve crosses the face; (b) a body taken as an entity, the minimum over its faces, the hint fixed (`## Run 2026-11-27`).

### S154 - `sketch_info` reads a sketch fully defined with relations whose model edges changed under them - OPEN 2026-11-27

Sorted out of the entry of 2026-11-25 ("a swept shell part with mirrored tabs"), point 4, on
build `20261124-174916`. A sketch with `collinear` relations to a base feature's edges: after
the base sketch's outline changed (those edges shortened, one cut back by 100), the dependent
sketch and its cut came back with the rebuild warning `swFeatureErrorUnknown` (`sketch_finish`
said so, right), while `sketch_info` of it read `constrained: "fully"` and the same relation
counts, nothing about relations whose external reference was lost; the geometry had not
moved. Workaround: the two related lines deleted and drawn again with dimensions to the
origin at the same values, 4 calls a part, `model_review` then clean. Bears on S96
(`sketch_constrain` does not check a relation to a model edge).

Proposed by the session: `sketch_info` names relations whose external reference is lost
(dangling), and the warning says "dangling relation" where SOLIDWORKS knows it. Local. Not
decided.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 4: `sketch_info` names dangling relations and does not read such a sketch fully defined (`## Run 2026-11-27`).

### S157 - Results met on the in-place mirror route: an insert names no body, a body delete is named `-1`, a mirror part's delete warns of its folders - OPEN 2026-11-27

Sorted out of the entry of 2026-11-26 ("a left part remade in place as the mirror of the
right one"), points 2 to 4, on build `20261124-174916`; the route is S65 (r). (a)
`part_derive {kind:"insert"}` names the new feature but gives no body id, and
`feature_mirror {bodies:[<that feature's name>]}` was refused `ENTITY_NOT_FOUND: No body
named '<feature>'`, the body being `<feature>-<other feature>`: one call lost each time until
`body_list` was read. (b) `body_delete` names its `DeleteBody` feature `-1`, seen twice;
`feature_rename` of `-1` worked. (c) `feature_delete` of a `MirrorStock` feature (Mirror
Part) warned ten times "SOLIDWORKS also removed 'Surface Bodies1' (absorbed by ...), which
was not planned" for the folders that feature carries (`Solid Bodies1`, `Planes1`, their
`___EndTag___` entries); nothing wrong was removed. None reported `ok` while wrong.

Proposed by the session: (a) the insert's result lists the new bodies' ids, as
`feature_mirror` and `body_delete` do; (b) a readable default name ("Body-Delete1") or a
`name` input; (c) those folder types left out of the "not planned" list. Local, small. Not
decided.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 5: (a) the insert lists its bodies' ids, (b) a `-1` delete feature renamed to a readable default, no new option, (c) a Mirror Part's folders left out of the "not planned" list (`## Run 2026-11-27`).

### S158 - A live test's preference check finds Double2 (STL deviation) changed by a test that sets no preference - OPEN 2026-11-27

Filed 2026-11-27 at the closing of the run of 2026-11-21, from its "Left for the end of the
run" line. S81's probe (2026-11-22, that run's group 3) saw once, in one variant (`doc_open`
then `doc_activate` of parts loaded only by an assembly), the live fixture's "System
preferences changed by this test: Double2: 0.000131 -> 1.085e-05"; cause not known. Seen
before by the run of 2026-11-13 (Integer230 and Double2 after a fresh start;
`closed_items.md`, *Not items*), decided then "an item if it is seen again": this is that
item. Proposed: a probe of which call or start sets it (Double2 is the STL export's
deviation). Local. Not decided.
Decided 2026-11-27 in the plan, not the owner: not in the run of 2026-11-27, because the
owner's scope for it is the items sorted 2026-11-27 and the extras offered, and this one
was not offered: the next plan.

### S159 - An equal chamfer's other leg: `dimension_set` on one does not warn, and the chamfer's edit path gives no legs line - OPEN 2026-11-27

Filed 2026-11-27 at the closing of the run of 2026-11-21, from its "Left for the end of the
run" line; outside S121's step (closed: the equal chamfer's legs named, and `equation_set`
on one leg alone warned of). `dimension_set` on one leg of an equal chamfer does not warn
that the other leg stays (S64 (i)'s case through another tool), and `feature_chamfer`'s edit
path gives no legs line. Proposed: `dimension_set` warns as `equation_set` does; the edit
path's result gives the legs as a new chamfer's does. Local, small. Not decided.
Decided 2026-11-27 in the plan, not the owner: not in the run of 2026-11-27, for the reason
S158 gives: the next plan.

### S161 - `contours.regions` reads 0 for a closed contour crossed by a construction centreline, or one that runs past the face's edge - OPEN 2026-11-27

Sorted out of the entry of 2026-11-27 (the scenario test of build `20261127-085502`), point
9; S64 (l) again, in two shapes S120's rule (closed 2026-11-22: a model edge that is an
inner loop of the face no longer disqualifies the region, and the regions follow a later
`sketch_add`) does not cover. (a) A closed polyline with a construction centreline drawn
across it, on a plane with no model geometry: `closedContours` 1, `regions` 0 at `sketch_add`
and at `sketch_finish`, met twice (a six-sided outline and a trapezoid); the extrudes from
them exact (1 617 000 and 55 964.845). (b) Two closed four-sided profiles on a planar face
that run past the face's outer edge: `closedContours` 2, `regions` 0; the boss from them
exact (+7 183.229). The same sketches read the same on build `20261117-012659`: not a
regression. The count only is wrong; the features were right. Reported `ok`.

Proposed: a probe of what SOLIDWORKS gives for these sketches' regions (a construction
segment across the contour, a face edge the contour crosses); then the regions counted from
the closed contours where those make them 0, as S64 (l) proposed. Local. Not decided.
Decided 2026-11-27 by the run, not the owner: left out of the run, by the run's rule "a new
point needing the owner (a choice, a new option, a large change), a new Open item, not in
this run", because it needs a probe first and is in no file of group 1's: for the next plan.

### S162 - A large `sketch_add`'s result repeats each entity's check and merge line: 12.7k characters for 100 lines - OPEN 2026-11-27

Sorted out of the entry of 2026-11-27 (the scenario test of build `20261127-085502`), point
10: a wasteful pattern, small. The 100-line `sketch_add` of S101's proof returned 12.7k
characters, about 127 a line: each entity repeats `"checks":["1 line created"]` and its merge
line. Nothing in it was wrong.

Proposed by the session: for a large call, a summary (entities drawn, merges, warnings, the
contour) with per-entity rows only where something differs. A change to the result's shape,
the owner's choice. Local. Not decided.
Decided 2026-11-27 by the run, not the owner: left out of the run, by the run's rule "a new
point needing the owner (a choice, a new option, a large change), a new Open item, not in
this run", because the result's shape is the owner's choice: for the next plan.

## Waiting

### S89 - SOLIDWORKS started by `sw_connect launch` cannot be reached from a later server process - BUILT 2026-11-22, proof: a session that runs `sw_connect {launch:true}` with SOLIDWORKS closed

Sorted out of the entry of 2026-11-16 (the scenario test of build `20261116-205845`), point
6; S60 (b) again, closed 2026-11-14 for another cause. SOLIDWORKS started by `sw_connect
{launch:true}` runs as `SLDWORKS.exe -Embedding` (parent: the DCOM launcher). After the
session's client was stopped and resumed, the new server process got `NOT_CONNECTED:
SOLIDWORKS is running (pid …) but can't be reached over COM: it may still be starting, or it
runs with different privileges …` on every call, `sw_connect` too, with SOLIDWORKS
responding, the old server process gone and none listed, same user and level. Likely: a
SOLIDWORKS started through COM is not in the running object table, so only the process that
started it holds it. Workaround: SOLIDWORKS stopped (a throwaway part's unsaved changes lost)
and started from its exe; `sw_connect` attached at once. Met when a headless scenario test is
stopped and resumed, which the run's check now does unasked.

Proposed: `launch` starts the exe, so any later process attaches; the `NOT_CONNECTED` hint
names a SOLIDWORKS started with `-Embedding` by another server process. Local, in
`SessionService` and `SessionNotes`, not group 4's files: for the next plan, not this run.
Not decided.
Met again 2026-11-17 (findings 2026-11-17, "a turned printed part with a flange and a swept
internal thread", point 1), build `20261117-012659`: SOLIDWORKS started by an earlier
session's `sw_connect {launch:true}` (after a restart for S92) still ran and responded, with
no main window title, when the next session's server started; `sw_status` and `sw_connect`
both gave `NOT_CONNECTED: ... it may still be starting, or it runs with different privileges
…`, no other `solidworks-mcp` process running. Workaround as above. A tell the hint could
use: a process with no main window and no other server alive is this case, never "still
starting".
Met again 2026-11-17 (findings 2026-11-17, "a part's hole positions driven by globals after a
restart", point 5), the same build, another side of it: `sw_connect {launch:true}` started
SOLIDWORKS with no visible window (`MainWindowTitle` empty). The user, seeing none, started a
second instance and opened an assembly there; the next `doc_open` of a part in the hidden
instance came back `"readOnly":true` with "the file is open elsewhere (sharing violation)",
and `sw_status` listed only the hidden instance's documents. The warning was clear; nothing
showed that a second `SLDWORKS.exe` was running. Workaround: the hidden instance's documents
closed (none dirty), its process ended, `sw_connect` attached to the user's, then
`doc_activate {allowEdit:true}` on the user's word. Proposed by the session: `sw_status`
lists every `SLDWORKS.exe` with its start time and which one the server is attached to, and
`launch` shows the window (or says it is hidden); the proposal above (`launch` starts the exe)
covers the second.
Met again 2026-11-18 (findings 2026-11-18, "a custom sheet format made from the ISO one with
`vba_run`", point 1), build `20261118-060615`: after the server process was restarted (to
turn on `SWMCP_VBA`), every call failed `NOT_CONNECTED` on a SOLIDWORKS that `sw_connect
{launch:true}` had started. An outside COM client's `GetActiveObject("SldWorks.Application")`
failed the same way (`Operation unavailable`), which bears out the running object table as
the cause. Workaround as above, by the user; unsaved drawing properties lost and set again
(9 calls, two user round trips).
Decided 2026-11-21 by the owner: taken into the run of 2026-11-21, group 3: `launch` starts the exe, so any later process attaches; the hint names a SOLIDWORKS started through COM (`## Run 2026-11-21`).
Cause: `sw_connect {launch:true}` started SOLIDWORKS by COM activation
(`Activator.CreateInstance`), which runs it as `SLDWORKS.exe -Embedding`; such an instance
is not in the running object table, so only the process that created it holds it. Probed
live: the instance so launched had its window titled (the server set `Visible`) after its
launcher exited, and its command line read `... SLDWORKS.exe -Embedding`. Fix: `Launch`
reads the exe from `SldWorks.Application`'s `LocalServer32`, starts it as a process
(window shown, as the user would), and waits until it is in the running object table and
`StartupProcessCompleted` (180 s, as before; an exe that exits during startup is
`NOT_CONNECTED` with its exit code). `NOT_CONNECTED` for a running SOLIDWORKS out of the
table reads each `SLDWORKS.exe`'s command line (`SessionNotes.CommandLine`, by
`NtQueryInformationProcess`); with `-Embedding` it says "it was started through COM by an
earlier server process: only it could reach it", the hint "close that SOLIDWORKS (saving
their work; with no window shown, end SLDWORKS.exe) and start SOLIDWORKS from its exe, then
call sw_connect", never "still starting"; with another server process running it names
that process as the one that started it, keeping "never end it yourself". No description
changed.
Before: the new `M0LiveTests.Launch_starts_a_SOLIDWORKS_that_any_later_process_reaches`
(SOLIDWORKS ended, `sw_connect {launch:true}` from a fresh session, then a VBScript
`GetObject(, "SldWorks.Application")` in `cscript`) failed, "An outside COM client could
not reach the launched SOLIDWORKS: ... Microsoft VBScript runtime error: ActiveX component
can't create object: 'GetObject'", and the fixture's own session then got the old
`NOT_CONNECTED ... it may still be starting ...`. After: it passes (the outside client reads
revision 31, the fixture's session finds "a new one"), 1/1; with `Connect_reports_the_revision`,
`Connect_says_whether_it_found_the_same_SOLIDWORKS_process_as_before` and
`Status_gives_a_failed_call_s_error_code_not_its_text`, 3/3; unit tests 322/322 (new in
`SessionNoteTests`: the hint with and without another server process, the `-Embedding`
tell, the command line read, the exe from a `LocalServer32` value); `check_repo` ok.
Made stricter: a SOLIDWORKS started through COM is no longer reported as possibly still
starting; nothing that passed is refused.
Decided 2026-11-22 by the run, not the owner: the tell is `-Embedding` on `SLDWORKS.exe`'s
command line, not the empty main window title the plan named, because the probe showed a
COM-started SOLIDWORKS with a titled window, and one still starting has no title either:
the command line marks this case and only it.
Decided 2026-11-22 by the run, not the owner: with another server process running, a
SOLIDWORKS started through COM is named so too, the hint naming that process, because
`-Embedding` holds whichever process started it, and "still starting" is false either way.
Decided 2026-11-22 by the run, not the owner: the live test ends SOLIDWORKS itself and
skips when a document is open, because launch can only be tested with none running and
the assembly fixture needs one at its start.
Local: on `sw2023`, `46920a1`; in build `20261122-030022`. Upstream: not yet
decided.
Checked 2026-11-22 by the run's check (build 20261122-030022, point 2): with SOLIDWORKS
running, launch attached to it, no second instance; the launch path not reached.
Decided 2026-11-22 by the run, not the owner: the proof moved to a session that runs
`sw_connect {launch:true}` with SOLIDWORKS closed, because a headless check may not end
SOLIDWORKS; the live test holds the path meanwhile.

### S118 - S112's sign misses S92's state when the globals are whole numbers: every derived row read 0 with no warning, and a batch refused blaming the expression, reported `ok` - BUILT 2026-11-22, proof: a session that meets S92's state

Sorted out of the entry of 2026-11-22 (the scenario test of build `20261122-030022`), point
5, on S112's fix (Closed); S92's state met live. A part with ten globals, all set to whole
numbers (`120mm`, `80mm`, ...), and thirteen dimension equations driven by them, opened
read-only as the session's first equation read: `equation_list` read the ten globals right
and every one of the thirteen dimension rows `"value":0`, reported `ok`, no warning. One
batch `equation_set` of the ten globals to new values: `VERIFY_FAILED` "SOLIDWORKS kept the
previous equation "<global>"= 120mm and did not take 140mm (a value the dimension can't
have, such as 0?) ... and 6 more", hint "Check the expression (units, quoted names, existing
variables)", rolled back: the expression blamed, S112 (a)'s restart wording absent. Cause:
S112's sign is a global set to a plain number that reads back another value; in the state
only a decimal number reads wrong (cut at the point) and a whole number reads right, so a
document whose globals are whole numbers shows no sign though every derived row reads 0. The
single `Add2` refusal on a new part did name the state (S116's proof, point 1).

Local, in group 2's files (`EquationReadback.cs`, the equation tools).
Decided 2026-11-22 by the run, not the owner: the sign is also a dimension row that reads 0
while the globals it uses read non-zero (or a dimension row's value against the dimension's
own value read from the model); and in a batch where every row is refused while that sign
shows, the refusal names the state and the restart before the expression. Test to start
from: the interop-free rule by unit tests (fails now: no such rule); the live equation tests
passing in the good state. Default model.
Decided 2026-11-22 by the run, not the owner: taken into group 4, by the run's rule "a new
point, small and in the group's files, a later step of this run ... taken into the next
group", because it extends S112's own sign and message in the equation files, found by group
3's check.
Cause: S112's sign judged only a global set to a plain number; in the state a whole number
reads right, and the rows defined from others, which all read 0, were not judged, so
`Misreads` came back empty and both tools went on as in the good state.
Fix: `EquationReadback.Misreads` (`src/SolidWorksMcp.Core/Services/EquationReadback.cs`) also
judges the rows that are not a plain number and use only globals of the document that read
non-zero: when two or more such rows have a value and every one of them reads 0, each is
named (`GlobalMisread.FromOthers`), and `Sign` reads "SOLIDWORKS reads all 13 rows defined
from globals that read non-zero as 0 ("D1@Sketch1", ... and 10 more): rows over non-zero
values all read 0, the sign of the state ...". `equation_list` and `equation_set` read the
sign through `Misreads` already, so the list warns, and a batch refused while it shows (the
scenario test's ten globals) names the state and the restart with S112's hint in place of
"Check the expression", "kept the previous equation" dropping its guess at the value. How
it keeps a healthy part clean: a row over a global of 0, over a dimension or over a name
with no row is not judged, one such row alone is not judged, and one row of 0 beside
another that is not (`"g" - "h"` with `g` = `h`) shows no sign. No description changed.
Before: the new `EquationReadbackTests.Every_row_from_non_zero_globals_read_0_is_the_sign_when_the_globals_are_whole_numbers`
(three whole-number globals read right, three dimension rows and a global from them read
0) failed, "Assert.Equal() Failure: Collections differ ... Expected: ["D1@Sketch1",
"D2@Sketch1", "D1@Boss-Extrude1", "half"] Actual: []". After: it passes, with three more
new tests for a healthy part (the same rows read right; a 0 beside a non-zero row; a single
row of 0 and rows over a global of 0, a dimension or an unknown name: no sign), unit tests
343/343; live, every test that calls `equation_list` or `equation_set` (the eleven of
`PropertiesLiveTests`, six of `FeatureManagementLiveTests`, three of `AssemblyLiveTests`,
the equal chamfer one of `PartFeatureLiveTests`), 21/21 in the good state, no warning where
none was; `PropertiesOfflineTests` 22/22; `check_repo` ok. No live case of the state (S92
(c)'s probe did not bring it on): the new sign is proved by the unit tests only.
Made stricter: `equation_list` and `equation_set` warn, and a refusal names the state, on a
document where every row from non-zero globals reads 0; nothing that passed is refused.
Decided 2026-11-22 by the run, not the owner: the sign is a row over non-zero globals read 0,
judged from the rows alone, not a dimension row against the dimension's value read from the
model, because it needs no other read of the model, covers a global defined from others as
well, and stays interop-free for the unit tests.
Decided 2026-11-22 by the run, not the owner: the rows count only when two or more of them
are judged and every one reads 0, because one row from globals can be 0 on a healthy part
(`"g" - "h"`), and a sign on it would tell the user to restart a healthy SOLIDWORKS; a
document with one such row in the state stays unflagged, as before.
Decided 2026-11-22 by the run, not the owner: an all-refused batch names the state through
the widened sign (S112's rule: any refusal while the sign shows), not by the count of
refused rows alone, because a batch of values the dimensions can't take is refused the same
way in the good state, where the restart would be a false lead.
Local: on `sw2023`, `ed486b0`; in build `20261122-142805`. Upstream: not yet
decided.
Checked 2026-11-22 by the run's check (build 20261122-142805, point 8): SOLIDWORKS was in the
good state. A part with ten whole-number globals and thirteen rows from them listed every row
right, no warning; the ten globals changed in one batch and back, 173 487.816 then 108 553.103
mm³, no warning; on a probe part a global `"W" - "V" - 1mm` reading 0 beside rows that read
non-zero gave only the old "evaluates to 0" line. So no false alarm in the good state; the
state did not come on, and the new sign and the refusal's wording were not seen live.
Decided 2026-11-22 by the run, not the owner: the proof moved to a session that meets S92's
state, because the state cannot be brought on at will (S92 (c)'s probe did not reproduce it)
and a check in the good state shows only that the sign raises no false alarm; the unit tests
hold the rule meanwhile.
Checked 2026-11-25 by two working sessions in S92's state (findings 2026-11-25, "reading two
parts the owner had edited by hand", point 1; "a swept shell part with mirrored tabs", point
1), build `20261124-174916`: the warning of the state fired both times and was right, but in
both the negative globals read 0, so S112's own sign fired as well, and in the first the
derived rows read their literal part, not 0, which this sign does not judge. The widened sign
alone was not seen live; the proof stays a session in the state whose globals read right.

### S156 - `dimension_set` of a linear pattern's count adds instances that form nothing, reported `ok` with a volume change of 0 - BUILT 2026-11-27, proof: the scenario test of the group 2 build

Sorted out of the entry of 2026-11-26 ("a printed plate revised: a bolt pattern grown"),
point 1, on build `20261124-174916`. `dimension_set` of a linear pattern's direction-1 count
from 1 to 2 (a 1 x 2 pattern of a through hole, made with direction 1 unused) reported `ok`,
`volumeDeltaMm3: 0`, no warning: the new column went along SOLIDWORKS' default sense of
direction 1, off the body, and formed nothing, where two Ø5.5 x 6 holes (-285.1) were meant.
Caught only by the hand figure. Workaround: the pattern deleted and made again by
`feature_pattern_linear {direction:"-x", direction2:"-y"}`, which checked the instance faces
and flipped direction 1 itself ("SOLIDWORKS' default ran the other way"); 3 calls with the
rollback. Reported `ok` while wrong. Bears on S125 (an instance that formed nothing) and S7
(`dimension_set`'s checks).

Proposed by the session: `dimension_set` on a pattern's count checks what
`feature_pattern_linear` checks (instance faces formed), or at least warns when a count
change leaves the volume unchanged. Local. Not decided.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 2: `dimension_set` of a pattern's count checks each instance formed and warns (a warning, as S42), the empty instances and the flip named (`## Run 2026-11-27`).

Cause: `dimension_set` checked a changed value's read-back, rebuild errors and sketches it
shrank, and gave the volume change, but nothing of a pattern's instances: a count raised
along a direction whose sense SOLIDWORKS picks itself (the pattern made with direction 1
unused, so never proven) put the new column off the body, where cuts form nothing, and the
call came back `ok` with `volumeDeltaMm3: 0`.
Fix: `dimension_set` checks each linear pattern whose dimension the batch set, instance by
instance, as `feature_pattern_linear` checks a new one: the seed's face signatures against
the pattern's faces at each grid place, both senses of each direction tried (the
definition's seeds and direction references read, through `AccessSelections` released at
once when a plain read gives none). The instances that formed nothing are named 1-based
(along direction 1, direction 2), and a direction none of whose instances formed anything is
named as running off the body in SOLIDWORKS' own sense, its flip the way (`feature_delete`
and `feature_pattern_linear` with the direction's sign, which flips it itself), or the count
set back; otherwise "past the body's edge or merge". Any count dimension raised whose
rebuild leaves the part's volume unchanged warns as well. Warnings only (S42). The check is
`PartFeatureService.CheckLinearInstances` over a shared `EmptyInstances` (new
`PartFeatureService.Instances.cs`), for S125's volume per instance to use; the text is
`FeatureManagementLogic.EmptyInstancesWarning` and `CountVolumeUnchanged`. No description
changed.
Before: the new `PartFeatureLiveTests.Dimension_set_of_a_pattern_count_warns_of_instances_that_formed_nothing`
(a hole at x 40, z -20 in the 100 x 20 x 60 block, patterned 1 x 2 with direction 1 `+x`
unused and direction 2 `+z` 30; `D1@LPattern1` set 1 to 2, the new column at x 60, off the
block) failed: "Assert.Contains() Failure: Filter not matched in collection Collection: []"
(`volumeDeltaMm3` 0 passed, no warning).
After: it passes, with a case where every instance forms (direction 2 raised to 3 rows 20
apart: one hole's volume, no warning); the live tests that call `dimension_set` on a part
(two of `AdvancedFeatureLiveTests`, five of `FeatureManagementLiveTests`, one each of
`M0LiveTests`, `ReferenceLiveTests` and `TopDownLiveTests`, two of `SketchEditLiveTests`)
and S41's `Linear_pattern_runs_direction_2_the_requested_way_when_its_own_way_leaves_the_part`,
14/14; unit tests 370/370; the LiveTests project offline 125/125 (606 live skipped), the new
`FeatureManagementLogicTests.Empty_pattern_instances_are_named_with_the_way_out` among them;
`check_repo` ok.
Made stricter: `dimension_set` on a part warns of a linear pattern's instances that formed
nothing and of a count raised with the volume unchanged; nothing that passed is refused.
Decided 2026-11-27 by the run, not the owner: the instance check runs for any dimension of a
linear pattern the batch sets, a spacing as well as a count, because a spacing moves
instances off the body the same way and the check is the same; other patterns (circular,
curve, fill) get the volume warning only, because the plan names the linear pattern's check
and the volume warning covers the rest.
Decided 2026-11-27 by the run, not the owner: the flip is named, not made, by
`dimension_set`, because no tool flips an existing pattern's direction and the plan adds no
option; `feature_pattern_linear` with the direction's sign is the way it names.
Decided 2026-11-27 by the run, not the owner: both warnings stand when both apply, because
the volume line also covers a pattern the instance check cannot read, and both are true.
Local: on `sw2023`, in the commit that moves it here; not yet in a build. Upstream: not yet
decided.

### S125 - `feature_pattern_linear`'s volume per instance averages over an instance that formed nothing - BUILT 2026-11-27, proof: the scenario test of the group 2 build

Sorted out of the entry of 2026-11-22 (the scenario test of build `20261122-142805`), point
10, in point 2's second case (S84 (b)): a 10 x 10 x 3 block patterned toward -x, 4 x 30,
the last instance off the block. The result read "volume change per instance 200 mm³ (3
instance(s))", where two instances added 300 each and the one off the block added nothing.
The warnings said that 2 of 15 instance faces were not found, so nothing was hidden, but the
per-instance figure names no instance's real volume. Small; reported `ok`.

Cause: `PerInstance` divided the volume change by every instance of the pattern. A second
cause showed when the fix was made: `EmptyInstances` (S156's instance check) counted an
instance as formed when any of its seed faces matched, and a plane matches by plane alone, so
the off-block instance's two side faces parallel to the shift matched the seed's own and the
other instances' (3 of its 5 faces "found"), and it was not seen as empty.
Fix: the per-instance figure is averaged over the instances that formed, the others named:
"volume change per instance 300 mm³ (2 of 3 instance(s); place 4 formed nothing and is left
out)", places counted as S156's warning counts them (the seed is place 1; a grid as (i, j)).
`feature_pattern_linear` takes the empty instances from the shared
`PartFeatureService.EmptyInstances` (S156), which now counts only the seed faces the
instance's shift moves (a plane parallel to the shift lies where another instance's face
lies), falling back to all the seed's faces when the shift moves none (a cut along its own
axis). The result's text only; the circular pattern's line is as it was. No description
changed.
Before: the new `PartFeatureLiveTests.Linear_pattern_volume_per_instance_leaves_out_an_instance_that_formed_nothing`
failed: "Assert.Contains() Failure: Sub-string not found; String: "volume change per
instance 200 mm³ (3 instance(s))"; Not found: "300 mm³"". With only `PerInstance` changed it
still failed with the same message (the off-block instance not seen as empty), which showed
the second cause.
After: it passes; the live tests that call `feature_pattern_linear` or the shared check
(ten of `PartFeatureLiveTests` with S156's `Dimension_set_of_a_pattern_count_warns_of_instances_that_formed_nothing`,
and `AdvancedFeatureLiveTests.A_rib_and_its_own_pattern_take_a_second_pattern_for_uneven_spacing`),
12/12; unit tests 370/370; `check_repo` ok.
Made stricter: an instance off the body is not counted as formed because some of its planes
coincide with other instances' planes, so `dimension_set`'s S156 warning also names such an
instance; the per-instance volume line is no longer an average over an instance that formed
nothing; nothing that passed is refused.
Decided 2026-11-27 by the run, not the owner: the instance that formed nothing is left out
of the average and named beside it (the item's preferred form), because the figure then
stays the real volume of an instance and the omission is said.
Decided 2026-11-27 by the run, not the owner: the shared `EmptyInstances` is made stricter
rather than a second check written for this line, because the item and the plan say to share
S156's, and the coincident-plane miss would otherwise make the two warnings disagree.
Local: on `sw2023`, in the commit that moves it here; not yet in a build. Upstream: not yet
decided.

### S124 - `feature_fillet` builds a radius twice the width of the faces it blends, consuming a face, reported `ok` with no warning - BUILT 2026-11-27, proof: the scenario test of the group 2 build

Sorted out of the entry of 2026-11-22 (the scenario test of build `20261122-142805`), point
9; the entry named it a new point of S64, filed as an item of its own. On a plate of a 10 x
10 section, a constant R20 fillet of the long bottom front edge, `propagate` off, meant as a
probe of the radius hint, built: −4 675.894 mm³ (42 % of the part), `faceCountDelta` −1,
the box's bottom rising from y −5 to −3.84; `checks` only "volume" and "round face r 20 mm".
The round runs across most of the front and bottom faces (from the side, a quarter of the
section cut off by the arc): SOLIDWORKS overflowed the fillet. A session would expect a
refusal or at least a warning. Reported `ok` while wrong.

Local, in group 4's fillet files (`FeatureService.Fillets.cs`, `FeatureVerifier.cs`).
Decided 2026-11-22 by the run, not the owner: a constant fillet warns when a face of the
edge is gone after the fillet, or when the radius exceeds the width of a face it blends,
naming that face; a warning, as the owner decided warnings for S42, and no new option.
Test to start from: point 9's case, R20 on the edge of the 10 x 10 section: a warning naming
the consumed face (fails now: none). Default model.
Decided 2026-11-22 by the run, not the owner: taken into group 7, by the run's rule "a new
point, small and in the group's files, a later step of this run ... taken into the next
group", because it is a
warning in the fillet checks group 4 changed (S122), needing no choice of the owner's, found
by group 4's check.
Carried 2026-11-27 into the run of 2026-11-27, group 2, its decision as written: the run of 2026-11-21 closed on the owner's word (`## Run 2026-11-27`).

Cause: a constant fillet was checked for its volume and a round face of the radius, both of
which an overflowed round passes, and nothing compared the radius with the faces it blends.
Fix: `feature_fillet` (constant) reads, before the fillet, each face beside an edge of the
call (and beside the edges of faces in it): its persistent reference, its width (the
farthest of five points on each edge of its boundary from the edge) and how far the round
reaches into it, r·tan(α/2) with α the angle between the two faces' outward normals (r at a
right angle). After the fillet it warns, naming each face, when the round reaches past a
face or the face is gone (its reference no longer resolves, or a plane's resolves off that
plane): "The R20 mm fillet reaches past faces it blends: F3 (plane, normal [0, 0, 1]) is 10
mm wide beside E2, the round reaching 20 mm into it and is gone after the fillet; ...
SOLIDWORKS may have run the round over them: check the shape (view_screenshot) or use a
smaller radius." Ids are given only to the faces warned of (`EntityRegistry.IdFor` by a
persistent reference, for a face gone after). The text and the reach are interop-free
(`FilletOverflow` in `Verify/FilletJoints.cs`). A warning only (S42). No description changed.
Before: the new `ModellingLiveTests.A_fillet_whose_round_reaches_past_the_faces_it_blends_warns_naming_them_and_an_ordinary_one_does_not`
(a bar x ±50, y 0..10, z ±5; R1 on the top back edge, then R20 on the bottom front edge,
`propagate` off) failed: "Assert.Single() Failure: The collection did not contain any
matching items ... Collection: []" (R20 built, −4 703 mm³, no warning at all).
After: it passes: the R1 fillet gives no such warning; the R20 one names both faces of the
edge, each 10 mm wide and gone (a face listing after it finds no face at z = 5 or y = 0).
With it the live tests that call the constant fillet (S64 (c)'s tangent-joint test, five of
`M0LiveTests`, two of `PartFeatureLiveTests`, two of `FeatureManagementLiveTests`, one of
`ReferenceLiveTests`), 12/12; unit tests 372/372, the new `FilletJointsTests` cases of the
reach and the text among them; the LiveTests project offline 125/125 (608 live skipped);
`check_repo` ok.
Made stricter: a constant fillet whose round reaches past a face it blends, or consumes it,
warns naming the face; nothing that passed is refused.
Decided 2026-11-27 by the run, not the owner: "the radius exceeds the width" is read as the
round's reach into the face, r·tan(α/2), not the bare radius, because at an edge other than
a right angle the round meets the face farther (acute) or nearer (obtuse) than r, and the
bare radius would warn wrongly at an obtuse edge and miss an acute one.
Decided 2026-11-27 by the run, not the owner: a face's width is its farthest boundary point
from the edge, because that never warns of a face the round does not pass; a face that is
narrow only near the edge (an L-shaped face) is caught by the gone check when consumed.
Local: on `sw2023`, in the commit that moves it here; not yet in a build. Upstream: not yet
decided.

### S146 - A two-direction linear pattern's dimensions do not say which direction each belongs to - BUILT 2026-11-27, proof: the scenario test of the group 2 build

Sorted out of the entry of 2026-11-25 ("two printed plates trimmed and their bolt pattern cut
from 2 x 2 to 1 x 2", point 2) and the entry of 2026-11-26 ("two printed parts from a written
plan", point 2), on build `20261124-174916`. `dimension_list {feature:<linear pattern>}` gives
two counts and two spacings (`D1` to `D4`), and `feature_pattern_linear`'s own result lists
`dimensions` as `D4`, `D2`, `D3`, `D1` (the counts marked `count`), with nothing saying which
spacing or count is direction 1 and which direction 2. Told apart by a `dimension_set` probe
and `face_list` of the instance cylinders (the volume the same for either direction), or
guessed and checked only at a second set of variables. Reported `ok`. Bears on S65 (j) (a
sketch dimension's entities not given).

Proposed by the sessions: each row labelled with its role and direction (`spacing1`,
`count2`, and the direction's reference), as `sketch_offset` names its distance. Local. Not
decided.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 2: each row of a linear pattern's dimensions carries its role and direction (`count1`, `spacing2`), a field in the result (`## Run 2026-11-27`).

Cause: a linear pattern's dimensions are D1 and D2 (the counts of direction 1 and 2) and D3
and D4 (their spacings), and nothing in a result said so: `FeatureVerifier.Dimensions` read
the display dimensions by name and value only.
Fix: each row of a linear pattern's dimensions carries `role`: `count1`, `spacing1`,
`count2` or `spacing2`, in `feature_pattern_linear`'s result, `dimension_list {feature}`,
`feature_info` and `feature_rename` (all through `FeatureVerifier.Dimensions`, for an
`LPattern`). A field of the result, no description changed (the catalog's bytes untouched).
The role comes from the dimension's name (counts by number: D1 is direction 1's, D2 direction
2's; spacings likewise, D3 and D4; probed live on a 2 x 3 and a 1 x 2 pattern, and a pattern
of one direction has D1 and D3 only) and is checked against the pattern's definition
(`ILinearPatternFeatureData`: counts and spacings of both directions); a row whose value the
definition does not bear out in either order gets no role. The rule is interop-free
(`Model/LinearPatternRoles.cs`).
Before: the new `PartFeatureLiveTests.Linear_pattern_dimensions_say_which_direction_each_belongs_to`
(a 2 x 3 pattern, 30 and 20 mm; then 2 x 2 at 20 and 20 mm; then one direction) failed:
"Assert.Single() Failure: The collection did not contain any matching items ... Collection:
[DimensionInfo { ... Name = "D4@LPattern1", Role = null ...}, ...]".
After: it passes, with the live tests that call the changed code (the two-direction grid, the
S156 `dimension_set` pattern test, `Dimension_list_returns_the_depth_...`,
`Dimension_list_gives_the_equation_...`, `Rename_reads_back_...`,
`Feature_info_reads_the_extrude_and_fillet_definitions`), 6/6; unit tests 377/377, the new
`LinearPatternRolesTests` (five cases) among them; `check_repo` ok. No catalog check: no
description changed.
Made stricter: nothing refused; a role is left out rather than guessed.
Decided 2026-11-27 by the run, not the owner: the field is named `role` and holds the role
with its direction in one word (`spacing2`), because the owner's decision names those
values, and a second field for the direction would only repeat the digit.
Decided 2026-11-27 by the run, not the owner: the role is read from the dimension's name
order and checked against the definition, rather than matched by value alone, because equal
counts or spacings (2 x 2 at the same spacing) cannot be told apart by value.
Local: on `sw2023`, in the commit that moves it here; not yet in a build. Upstream: not yet
decided.

### S163 - A boss that merges into a body and meets it along an edge only fails `FEATURE_FAILED`, the hint naming profile closure and depth only - BUILT 2026-11-27, proof: the scenario test of the group 2 build

Split out of S64 (q), sorted from the entry of 2026-11-24 ("a multi-body envelope part from a
vendor drawing", point 3), build `20261124-174916`: a mid-plane boss of a plate whose end edge
met another body's edge along a line only (edge to edge, no face shared) failed
`FEATURE_FAILED: SOLIDWORKS created no feature (the API returned nothing)`, the hint naming
profile closure and depth. The cause was the line contact (non-manifold); the unused sketch
had to be deleted after. Proposed by the session: the hint names edge-only contact with an
existing body as a cause.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 2, the hint naming
edge-only contact (`## Run 2026-11-27`).

Cause: `FeatureVerifier.Hint` gave an extrude's failure two causes only, a cut pointing away
from the material, and for a boss a profile not closed or a depth not positive. SOLIDWORKS
refuses a boss that would join a body by a face and meet another part of the same body along
an edge only (the result is non-manifold) and returns nothing.
Fix: the hint of `feature_extrude`'s failures (`FeatureVerifier.Hint`) adds, for a boss, "and
that the boss does not meet a body it merges into along an edge only (a line, no face shared:
non-manifold; overlap a face instead)". A text in a result: no description changed, the
catalog's bytes untouched. Reproduced by hand first: a U-shaped body (50 x 20 x 10, a 10 x 10
notch) and a plate 20 x 20 on the front plane, mid-plane 20, its bottom face on one arm and
its edge on the other arm's top edge along 10 mm: `FEATURE_FAILED ... created no feature`. A
boss that touched a separate body by an edge only was not refused (a second body, 2 bodies).
Before: the new
`PartFeatureLiveTests.A_boss_that_meets_its_body_along_an_edge_only_fails_and_the_hint_names_that`
failed: "the hint names no edge-only contact: FEATURE_FAILED: feature_extrude: SOLIDWORKS
created no feature (the API returned nothing). Hint: For a cut, the direction may point away
from the material (use direction 'auto', the default, or through_all_both); for a boss, check
that the profile is closed and the depth positive."
After: it passes (the failure code, the hint naming "along an edge", the volume unchanged at
9 000 mm3 after the rollback), with the live tests that read an extrude failure
(`The_servers_own_failed_kept_and_rolled_back_changes_do_not_read_as_the_users`,
`Mirror_into_the_body_or_about_a_curved_face_is_rejected`), 3/3; unit tests 377/377;
`check_repo` ok. No catalog check: no description changed.
Made stricter: nothing refused or newly failed; only the hint's text grows.
Decided 2026-11-27 by the run, not the owner: the hint lists edge-only contact among the
causes rather than the server telling it, because telling it needs the new body's geometry
before SOLIDWORKS refuses (the call returns nothing, and the rollback leaves no body to
read), and the hint is read only after a failure, where a cause named beside the others costs
nothing.
Local: on `sw2023`, in the commit that moves it here; not yet in a build. Upstream: not yet
decided.

### S164 - A chamfer leg that takes a face whole is named by the face beyond it: both legs of an edge on one plane, and a side swap warned that did not happen - BUILT 2026-11-27, proof: the scenario test of the group 2 build

Split out of S64 (k), sorted from the entry of 2026-11-16 ("an edit to three printed parts",
point 2), build `20261116-124830`: the leg check of an `angle_distance` 1 / 60° chamfer on
two edges where a groove's side meets a flat wall, with `flipEdges`, gave both faces of each
edge the same plane, "edge E24: 1 mm along F252 (plane [-0.866, 0, -0.5]), 1.732051 mm along
F253 (plane [-0.866, 0, -0.5])", where one of the two is a face square to the axis. The volume
matched, so the chamfer was right and the reported faces were not. Proposed: a probe of that
case; each face's own normal read.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 2, a probe first,
each face's own normal read (`## Run 2026-11-27`).

Probe (2026-11-28, build `20261127-085502`): a block with a groove 4 wide and 1 deep, the
chamfer 1 / 60° on its two top edges. Unflipped, or one edge flipped, an edge put the 1.73 leg
on the 1 mm side and failed `VERIFY_FAILED` (S64 (j)); both flipped, it built (-34.641 mm3,
as figured) and read "edge E1: 1 mm along F70 (plane [0, 1, 0]), 1.732051 mm along F71 (plane
[0, 1, 0])", the same for E2 with F70 again, and warned "The first distance went on plane [0,
1, 0] for edge E1 but on the other face for edge E2", which was not so.
Cause: the 1 mm leg took the 1 mm side face whole. The leg check names the face beside each
leg's end edge (`ReadEdge`: the other face of the chamfer face's boundary edge); with the side
gone, that is the groove's floor, parallel to the top face, so both legs read one plane, and
the side key (`SideKey`, the same reading) made both edges' sides look swapped. Normals were
read right; the face was the wrong one.
Fix (`PartFeatureService.cs`): each chamfer edge keeps its two faces as they were before the
chamfer (`ChamferEdge.Faces`: geometry, side key, description). A leg whose face beside its
end does not hold it (a plane not containing the leg beside a line; beside a circle, a plane
square to the axis for a radial leg or a cylinder on the axis for an axial one; other faces
not decided, read as before) is named by the original face that holds it: "1 mm along a face
taken whole (plane [1, 0, 0]; the leg ends on F70 (plane [0, 1, 0]))". The side key of that
leg is the original face's, for the mixed-sides warning and for `flipEdges`' side reading
(`HoldFlips`), which compared the same keys. A text in a result: no description changed.
Before: the new
`PartFeatureLiveTests.A_chamfer_leg_that_takes_a_face_whole_names_that_face_not_the_one_beyond`
failed: "both legs of an edge on one plane: volume | legs measured on 2 edge(s) | edge E1: 1
mm along F10 (plane [0, 1, 0]), 1,732051 mm along F11 (plane [0, 1, 0]); the chamfer face meets
the first at 60° and the second at 30° | edge E2: 1 mm along F10 (plane [0, 1, 0]), 1,732051
mm along F12 (plane [0, 1, 0]); ...".
After: it passes (the volume -103.923, each edge's 1 mm leg on plane [±1, 0, 0] and its
1.732051 leg on plane [0, 1, 0], no mixed-sides warning), with the live tests that make a
chamfer (`PartFeatureLiveTests`' ten chamfer tests and
`Linear_pattern_along_a_slanted_edge_takes_the_side_the_axis_names`, two auxiliary-view tests
of `DrawingLiveTests`, two `Sketch_finish_...` tests of `FeatureManagementLiveTests`), 15/15;
unit tests 377/377; `check_repo` ok. No catalog check: no description changed.
Made stricter: nothing refused or newly failed; a leg's face is named by the face it took, and
a mixed-sides warning is no longer given where the sides were not mixed.
Decided 2026-11-27 by the run, not the owner: a leg that took its face whole names that face
by its plane as it was, with no id, and the face beyond by its id, because the face is gone
(an id would point at nothing) and the face beyond is the one a caller can still pick.
Decided 2026-11-27 by the run, not the owner: a face beside the leg that the check cannot
decide (a cone, a spline, a cylinder beside a line) keeps the reading as before, because
naming an original face there would be a guess.
Local: on `sw2023`, in the commit that moves it here; not yet in a build. Upstream: not yet
decided.
