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

**Open: S4, S5, S7, S10 to S12, S33, S34, S52, S60, S64, S65, S79 to S86, S89, S90, S92 to S110.**
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

A run is planned, `## Run 2026-11-16` below: 20 points of 16 items in five groups, each
its own build and scenario test; S60 parked and S64 (b) left out on the owner's word.

## Run 2026-11-16

The run takes 20 points of 16 open items (S49, S51, S52, S59 (c), S61 (a), S62 (b), S64
(c) and (h), S65 (b) to (e), S67 to S74) in five groups, each group its own build and its
own scenario test. Phase 2 (`backlog:run`) goes unattended after the opening answer below,
across as many sessions as it needs, from the first step not ticked under *Where the run
stands*.

### How it runs

- **Unattended.** After the opening answer nothing is asked. A decision met mid-run is
  the run's: the option that keeps the server's checks honest and its results true, the
  smaller change when two are equal, written into its item as "Decided <date> by the run,
  not the owner: ..., because ..." and listed in the report. Plugin questions later in the
  run (`clockify`'s offer after a commit, `git-workflow`'s push notes) are not asked; each
  is named in one line of the report.
- **Never stop on one item.** A fix that does not hold after a fair try, or a probe that
  finds no way, stays Open with what was tried written into it, its partial code in a
  stash named for it (`git stash push -m run-1116-<S> -- <files>`), and the run goes on.
  SOLIDWORKS hung or gone: end it and start it again (granted). A permission prompt or a
  dialog nobody answers is the one thing it cannot pass: it writes where it stood below,
  and stops.
- **One SOLIDWORKS, so serial** (`serial: true`): one item at a time, in the main tree on
  `sw2023`, and never a live test beside the scenario session. **One fresh subagent per
  item** (the run of 2026-11-13: one resumed agent reached ~450k tokens, a fresh one per
  item ~160-180k), the main session thin: it hands the item over, reads the ten-line
  report and the diff, and commits. Models: the default model for every fix and probe
  below (each needs diagnosis) and for the check; Sonnet for a group's tests and release.
- **Per item**, as `CONTRIBUTING.md` (*Tests*) and the owner's rules give it: first the
  live test that shows the fault, run on the current code (a test that does not fail
  closes the item instead, the test kept when worth keeping); then the fix; then that test
  and the live tests that call the changed code (`--filter-method`, about 18 s each, never
  the whole area); unit tests offline; `python tools/check_repo.py`; a description changed
  means `--check-catalog` and `ContractTests.Tool_catalog_follows_the_conventions` (the
  tool list is at its byte budget, 11 bytes under: any new option is paid for by trimming,
  as S51, S59 and S65 (a) did). A probe item is a live probe first, its result written into
  the item; the fix follows when it shows the cause, else the item stays Open with the
  probe recorded.
- **A subagent's brief**: its item as written here and in the item; read `CLAUDE.md`,
  `CONTRIBUTING.md`, the parts of `docs/solidworks-api-notes.md` it needs, and this
  section; never switch branches, touch `.claude/worktrees/*`, commit, push, tag or reset;
  a stash only by name and file. SOLIDWORKS 2023 (`C:\Program Files\SOLIDWORKS
  Corp\SOLIDWORKS\SLDWORKS.exe`) started if not running; live tests as
  `$env:SWMCP_LIVE_TESTS=1; dotnet test --project tests/SolidWorksMcp.LiveTests
  --filter-method "<Class>.<Method>"`. Then the item: moved to the end of Waiting with its
  cause, fix, the test before (its message), the tests after with counts, "Made stricter:
  …", its "Decided … by the run" lines, and "Local: on `sw2023`, in the commit that moves it
  here; not yet in a build. Upstream: not yet decided."; for a lettered item only the
  point done, the item staying Open for the rest; the `**Open: …**` line from `board.py
  --open-line` and `board.py --check` clean. `findings.md` is not the subagent's. Report,
  ten lines: the outcome, the failing test before, the tests after with counts, every file
  changed, its decisions, a proposed commit subject and body. Nothing that names where a
  finding came from (`findings.md`'s top).
- **Commits**: one per item (or per point of a lettered item), its fix and its note, as
  `git-workflow:pr` gives a commit on `sw2023`: as the owner, no trailer. Pushed at the
  group's release; no PR upstream (each upstream line stays "not yet decided").
- **Per group, the release**: the area's live tests in full once, on what is to be built
  (the owner's global `CLAUDE.md`); `docs/tools.md` regenerated when a description
  changed; then `CLAUDE.md`'s *Build, release and run*, steps 1 to 5: push `sw2023`,
  publish from a clean tree at the pushed commit, `--check-catalog`, register
  `solidworks-mcp` (scope local) on the new exe in both projects `~/.claude.json` lists
  (`claude_solidworks` and `Upwork/Project/clove sorter`, both on `0.1.0-20261114-024455`
  at planning), tag `build-<stamp>` pushed alone and read back, the release page with no
  files and "Scenario test: not run yet". The items of the group move to Waiting naming the
  build, status `BUILT <date>, proof: the scenario test of build <stamp>`.
- **Per group, the check**: the scenario test of that build, run headless from
  `claude_solidworks` so it loads the build just registered there: a session id made
  first and written on the group's check line, then `claude -p "Uji skenario build
  …-<stamp> (<S-items>), menurut reports/uji-skenario.md. Tanpa nanya apa-apa."
  --session-id <uuid> --permission-mode auto --allowedTools mcp__solidworks-mcp
  --add-dir "<the fork>"`; cut short, `claude -p --resume <uuid>` from the same folder,
  with the same flags. **So it does not stop half-way** (the owner, 2026-11-16, after
  group 4's was ended for low memory): before it, `dotnet build-server shutdown` and read
  free memory (7.3 GB in all here; SOLIDWORKS takes about 1 GB more once it opens); start
  it detached, by `Start-Process` with its output to a file, not as a background task of
  the running session, which Claude Code ends when memory runs low; watch it by its
  process and by the session's transcript
  (`~/.claude/projects/<claude_solidworks>/<uuid>.jsonl`). Stopped with no `findings.md`
  entry, or the transcript unwritten for 30 minutes with the process still there (stop it
  first): resume it at once, unasked, up to three times, then Blocked with what was seen.
  A SOLIDWORKS the stopped session started by `sw_connect launch` is out of reach of the
  resumed one (S89): it is closed and started from its exe, what is open in it the test's
  own copies.
  It starts with `sw_status` reading the new version (step 6's short check). Its `findings.md` entry is committed here unasked and
  sorted (`backlog:sort`), the release page's notes updated (step 7). What it finds goes
  four ways: **held**, the item to `closed_items.md`, `CLOSED <date>`; **a fix of this
  group not holding**, fixed and its group built and checked again before the next group;
  **a new point, small and in the group's files**, a later item of this run, a new
  S-number, taken into the next group; **a new point needing the owner** (a choice, a
  large change), a new Open item, not in this run. The next group starts only after the
  check is sorted.

### Items

Order: results reported `ok` while wrong first (S51, S73, S72), then the most met, then
by files. Each group's test to start from fails on the code at planning (`a869c8a`, no
server change since build `20261114-024455`).

- **Group 1, feature tools** (`PartFeatureService.cs`, `FeatureService.Fillets.cs`, the
  linear and circular pattern code):
  - **S51**, `flipEdges` in `feature_chamfer`'s `edit` (a) and (d): decided by the
    owner 2026-11-13 (per-edge flip), its fault in the scenario test of 2026-11-14. The
    edit flips exactly the indexed edges, each edge's side read back; the mixed-sides
    warning when the sides then differ. Test: the scenario's case, a `distance_distance`
    1/2 chamfer on two bottom edges of a block meeting at a corner, then `edit` with
    `flipEdges:[1]`: edge 0 keeps 1 mm on the bottom face, edge 1 takes it on its side
    face (fails now: both flip); a new chamfer with `flipEdges:[1]` reads edge 0
    unflipped. Not: `feature_mirror` of a chamfer.
  - **S73**, circular pattern sense: a `reverse` option on `feature_pattern_circular`,
    and the checks give the instances' angles about the axis as the caller named it,
    saying when the reference's own direction runs the other way. Test: a 10° pattern of
    a cut at 30° about a hole whose cylinder axis reads [0, 0, -1], three instances:
    without `reverse` as now, its checks naming the sense about the hole's axis as read;
    with `reverse` the instances at 40° and 50° (fails now: no option). Not: angles over
    360°.
  - **S64 (h)**, a pattern of a feature and its own pattern: a probe first, whether
    SOLIDWORKS takes it by any selection (the pattern feature as a seed, bodies, the
    pattern's own seed with more instances). A way found: used. None: the hint names it
    ("a pattern cannot take its own pattern as a seed: …") in place of "the API returned
    nothing", the test the refusal's text (fails now).
  - **S64 (c)**, fillet or chamfer over a tangent chain failing `FEATURE_FAILED` with a
    hint that misses: on that failure of a call of several edges, each edge is tried alone
    (rolled back after) and the error names the edges that fail alone and the faces beside
    them; nothing is left built. Test: the plate of S52's test (quarter-round ends into a
    30° face), a 1 mm fillet over the top chain (fails now: the hint names the radius).
    Not reproduced at 1 mm: the probe recorded, the item stays Open for (c).
- **Group 2, assembly selection** (`AssemblyService.cs`, `AssemblyService.Structure.cs`,
  `EntityResolver.cs`):
  - **S68**, coincident mate with an origin: a probe of how the origin is selected
    (its `SelectByID2` name and type, `swSelEXTSKETCHPOINT` "Point1@Origin"), then the
    fix. Test: a coincident mate between the assembly's origin and a component's origin
    puts the component's origin at the assembly's (fails now: status 0).
  - **S69**, a named plane of a component inside a subassembly: the select path built
    for a nested component. Test: a mate with `Top Plane` of `sub-1/part-1` (fails now:
    "Could not select").
  - **S70**, a top-level component resolving to a nested one of the same short name: a
    probe first, an assembly with `part-1` top level and `sub-1/part-1`; `{component:
    "part-1"}` must reach the top-level one. Holds already: closed, the test kept.
  - **S62 (b)**, "solve as flexible": `asm_component_set` takes `flexible` for a
    subassembly component (`IComponent2.Solving`), read back, refused for a part
    component. Test: a subassembly set flexible and back, read from SOLIDWORKS (fails
    now: no option).
  - **S75**, taken in 2026-11-16 from group 1's scenario test: `newFaces` of a chamfer
    call lists the faces of the chamfer an edge moved to as well. Test: S51's new chamfer
    with `flipEdges:[1]` on two edges at a corner, `newFaces` two faces (fails now: one).
- **Group 3, `dimension_set` and feature management** (`FeatureManagementService.cs`,
  `DocumentRegistry.cs`):
  - **S49**, the wording of the line naming a changed part: always "'<title>' (<handle>,
    <access>)", a part the server has not listed yet registered so it has a handle. Test:
    a new assembly of parts the server has not listed, `dimension_set` through it, the
    line with a handle (fails now: no handle).
  - **S74**, a mate's dimension driven by an equation: refused naming the equation and
    its variable, with `equation_set`, as for a part's dimension. Test: an angle mate
    whose dimension an equation drives, `dimension_set` on it (fails now: "driven
    (reference) dimension").
  - **S67**, `feature_rollback` before the only feature: a probe of which of the rename,
    the single feature or the check's reading of the bar causes it, then the fix. Test: a
    part with one extrude, renamed, `feature_rollback {to, before:true}` (fails now:
    `VERIFY_FAILED` "unexpected state: 'Sketch1'"); not renamed too.
  - **S76**, taken in 2026-11-16 from group 2's scenario test: `asm_component_set` warns
    of the mates a change put in error, and a mate's `errorName` comes from its own enum.
    Test: a flexible subassembly whose floating child a mate holds elsewhere, set rigid
    (fails now: "flexible → rigid" with no warning, `swSketchErrorExtRefFail`).
  - **S77**, taken in 2026-11-16 from group 2's scenario test: a probe of what
    `IComponent2` reports for a child of a flexible subassembly; a reading found, the row
    says the child can move. Test: that child's row (fails now: `fixed: true`,
    `constrained: "fully"`). None found: Open with the probe recorded.
  - **S78**, taken in 2026-11-16 from group 2's scenario test: (a) `moved` gives full names
    and every component that moved, (b) `{component, type:"origin"}` with no name takes
    the origin, (c) an origin reads as one in a mate's row. Test: a mate that moves a
    nested cube and, through earlier mates, a top-level one of the same short name (fails
    now: one short name), and an origin mate by type alone (fails now: `INVALID_ARGUMENT`).
- **Group 4, references and sketches** (`ReferenceService.cs`, `EntityResolver.cs`,
  `SketchEditService.cs`, `SketchService.cs`, `ViewService.cs`):
  - **S59 (c)**, a reference with `sketch` given and no `type` resolves in that sketch, a
    segment or a point. Test: `ref_plane` `angle` with `axis: {name:"Line1",
    sketch:"Sketch1"}` (fails now: `ENTITY_NOT_FOUND`).
  - **S65 (e)**, decided 2026-11-16 by the owner: `sketch_add` merges segment ends that
    meet (the same point within the sketch's tolerance) among the segments it adds and
    the sketch's existing ends, and says how many it merged; no new reference form. Test:
    a polyline and a separate arc drawn on its two ends close one contour (fails now:
    "ManyOpen").
  - **S65 (c)**, `{at}` on a point where several sketch points sit: `resolved` names the
    point taken (its owner segment), and a warning lists the others there with the
    `{name, sketch}` that picks each; which point it takes is not changed. Test: three
    concentric arcs from `sketch_offset` and line ends on their centre, a relation on
    `{at}` there (fails now: nothing says which).
  - **S65 (b)**, a sketch text's position: `sketch_info` gives each text's position,
    height and angle; a move only through a tool that already moves sketch entities, if
    one takes a text, else read only and (b) stays Open for the move. Test: a text added
    at a point, read back at it (fails now: no position).
  - **S65 (d)**, `view_screenshot` with `hideAxes`, as `hidePlanes`. Test: a part with an
    axis far out, the part's share of the image with `hideAxes` (fails now: no option).
- **Group 5, import, material, simulation** (`OpenService.cs`, `TightBox.cs`,
  `AssemblyService.cs` insert, `PropertiesService.cs`, `SimulationService.cs`):
  - **S52**, the box of a component holding surface bodies only (a STEP import, S5):
    the tight box reads surface bodies too, else the box is `approximate: true`. Test:
    the scenario's case, an assembly with a cylinder turned 45° exported to STEP and
    opened (fails now: a loose box without `approximate`). Not: S5 itself (upstream).
    With it S52 (c), taken in 2026-11-16 from group 3's scenario test: a flexible
    subassembly's box covers its children as they are placed, else says it is its file's
    placement; test: a flexible subassembly whose child top-level mates move out of its
    file's box (fails now: the file's box, `approximate: true`).
  - **S61 (a)**, `asm_insert` takes a STEP: a probe of the insert with 3D Interconnect
    on in a preference scope, the component linked to the file; refused with the way
    round when the probe finds none. Test: a STEP of a 20 mm cube inserted, one solid of
    8000 mm³ (fails now: refused).
  - **S72**, `material_set` at 1 kg/m³. Decided 2026-11-16 in the plan, not by the owner
    (the opening question held four others), because the description promises the
    density verified and the session proposed it: refused `VERIFY_FAILED` and rolled back
    when the density read differs from the database's by more than 1 % and no body has a
    material of its own and the mass is not overridden; at about 1 kg/m³ the message names
    a SOLIDWORKS restart, as S58 (b)'s does. The bad state cannot be made live: the rule
    in an interop-free function with a unit test (fails now: it does not exist), and the
    live tests of `material_set` passing. Not: the linked property in metres (noted, S30).
  - **S71**, a frequency study with no fixtures: a probe of the soft-spring option and of
    the solver (FFEPlus, Direct Sparse); one that finishes is set by `sim_run` for a
    frequency study with no fixtures and said in its checks. Test: upstream's
    `Frequency_of_the_cantilever_column_…` (fails now at its free-free study). None
    finishes: Open with the probe recorded.
  - **S87** and **S88**, taken in 2026-11-16 from group 4's scenario test (the run's rule:
    small, in group 4's files): `sketch_add` reports each merge's gap as drawn and moves an
    end by at most it; `view_screenshot` warns when its fit takes in a shown sketch past the
    bodies, naming `hideSketches`. Tests: the scenario's polyline and arc, the reported
    distance and the line's end (fails now: 0.000054, the end moved twice the gap); a block
    and an axis through points 300 mm out (fails now: no warning).
  - **S61 (a) again**: did not hold in the check of build `20261117-012659` (findings
    2026-11-17, point 1): a STEP inserted into an assembly that already holds a component
    has its part left unwritten by the assembly's save, open dirty and `foreign`, the
    component lost on reopen, reported `ok`. Fixed again, and group 5 built and checked
    again (the plan's rule, *Per group, the check*). Test: the scenario's case, a part
    inserted first, then the STEP, the assembly saved (fails now: the part not written).
- **For the owner**: nothing to prepare. S1 in Waiting needs a session that runs
  `drw_view_detail` and a view at an angle, outside this run.
- **Out of this run**: S4, S5, S7, S12 (wait on upstream); S10, S11, S33, S34 (parked,
  taken up when a job needs them); S60 (a) (parked 2026-11-16 on the owner's word: probed
  2026-11-13, not reproduced); S64 (b) (left out on the owner's word, the same).

### Opening answer

2026-11-16, the owner, in one `AskUserQuestion` call:

- Grants: **"Semua (Recommended)"**: "Push sw2023, tag build-<stamp>, halaman release
  (tanpa file), registrasi exe baru di claude_solidworks DAN clove sorter (keduanya
  sekarang di build 024455), uji skenario headless (claude -p dari claude_solidworks),
  commit entry-nya tanpa nanya, restart SOLIDWORKS kalau hang. Tanpa PR upstream."
- Builds: **"5 build, satu per grup (Recommended)"**.
- S65 (e): **"sketch_add merge otomatis (Recommended)"**.
- Extras, all four: park S60 (a) and S64 (b); S61 (a); S62 (b); S64 (c), S65 (b) and (d).

### Where the run stands

- [x] Phase 1: the opening question asked and answered (2026-11-16)
- [x] S51: committed (`f665a18`)
- [x] S73: committed (`9cd5638`)
- [x] S64 (h): committed (`86e7042`)
- [x] S64 (c): committed (`4a70a9b`)
- [x] Group 1: full live tests and release (build: `20261116-050908`, `dedd986`)
- [x] Group 1: scenario test (session id: `fe1d732c-6c26-4fe6-8249-8c5ce1e03251`),
  its entry committed and sorted: resumed once after a stop for low memory; S51 and
  S73 held and closed, S75 new (findings 2026-11-16, `4af10cc`)
- [x] S68: committed (`5bd1037`)
- [x] S69: committed (`3fc6c59`)
- [x] S70: committed (`a6a28d6`)
- [x] S62 (b): committed (`64cb0b0`)
- [x] S75: committed (`643fc77`)
- [x] Group 2: full live tests and release (build: `20261116-080311`, `643fc77`)
- [x] Group 2: scenario test (session id: `98f558e8-fa78-4f15-bb54-1981e3df0738`), its
  entry committed and sorted: resumed once after the stop for low memory; S68, S69, S70,
  S62 (b), S75 held and closed, S76, S77, S78 and S79 new (findings 2026-11-16, `54ce718`)
- [x] S49: committed (`5c776ef`)
- [x] S74: committed (`972bc54`)
- [x] S67: committed (`1da8d57`)
- [x] S76: committed (`96dd93f`)
- [x] S77: committed (`14a0e56`)
- [x] S78: committed (`ad5f6ba`)
- [x] Group 3: full live tests and release (build: `20261116-124830`, `ad5f6ba`)
  Live: AssemblyLiveTests 53, FeatureManagementLiveTests 36, TopDownLiveTests 4,
  AssemblyStructureLiveTests 2, SketchEditLiveTests 74, PartFeatureLiveTests 49,
  ReferenceLiveTests 46, all passed (S74's test failed once in the full Assembly class run,
  SOLIDWORKS refusing a valid equation, passed alone after a restart); unit 290.
- [x] Group 3: scenario test (session id: `9f4eae18-a900-4e4a-8818-c5ee41ec6ad3`), its entry committed and sorted:
  S49, S74, S67, S76, S77, S78 held and closed; S52 and S79 gained a point (findings 2026-11-16, `f236686`).
  The run stopped here 2026-11-16 on the owner's word, to go on in a new session from
  group 4. For the next releases: group 3's full live tests took 86 min, class by
  class with SOLIDWORKS restarted between (one batch stopped by its "critically low
  on memory" dialog, ~1.4 GB free); S74's live test failed once in the full class
  run on SOLIDWORKS refusing every equation (S58 b's state) and passed alone after a
  restart. Cost this session (for `learned.md` at the run's end), tokens / minutes:
  S49 110k/9, S74 132k/9, S67 121k/8, S76 140k/12, S77 168k/15, S78 195k/21 (each
  the default model); group 3's release 105k/87 (Sonnet); the sorts of groups 2 and
  3 124k/4 and 111k/3; group 2's scenario test resumed once, group 3's in one run.
- [x] S59 (c): committed (`d8dd2ee`)
- [x] S65 (e): committed (`27408b8`)
- [x] S65 (c): committed (`d339521`)
- [x] S65 (b): committed (`6826759`)
- [x] S65 (d): committed (`f91176e`)
- [x] Group 4: full live tests and release (build: `20261116-205845`, `ba2ce83`)
  Live: ReferenceLiveTests 47, SketchEditLiveTests 77, VisualsLiveTests 2, M0LiveTests 23,
  ModellingLiveTests 9, PartFeatureLiveTests 49, all passed, no flake; unit 290. Run class by
  class with SOLIDWORKS restarted between: the live tests took 43 min (7, 11, 1, 3, 2, 10 min
  of test time), the whole release about 45 min, no "low on memory" dialog this time.
  `docs/tools.md` regenerated and committed alone (`ba2ce83`). Cost this session (for
  `learned.md` at the run's end), tokens / minutes: S59 (c) 127k/8, S65 (e) 171k/13,
  S65 (c) 191k/15, S65 (b) 144k/7, S65 (d) 242k/26 (each the default model); group 4's
  release 105k/45 (Sonnet).
- [x] Group 4: scenario test (session id: `5e28cb8b-9a48-46e6-addb-0c43e04ad1b8`), its entry committed and sorted
  Stopped 2026-11-16 before it wrote an entry: Claude Code ended the background session for
  low system memory and says not to start it again unasked. Resumed 2026-11-16 on the
  owner's word (and resumed unasked from then on, the check's rule above), detached, and
  ended the same evening: all five held, the entry `4f8700f`, sorted into S59 (closed), S65
  (b) to (e) (closed), S87 and S88 (group 5), S89 (Open), S5 met again.
- [x] S52: committed (`44869a2`)
- [x] S61 (a): committed (`1b0203a`)
- [x] S72: committed (`47c9f62`)
  For the release: `PropertiesLiveTests.A_custom_material_is_listed_…` fails on this
  machine before and after S72 (a second classification in the custom library, from an
  earlier run), not a fault of this group.
- [x] S71: committed (`e18996e`)
- [x] S87: committed (`0b7698f`)
- [x] S88: committed (`5320223`)
- [x] Group 5: full live tests and release (build: `20261117-012659`, `098a10b`)
  Live: AssemblyLiveTests 56, AssemblyStructureLiveTests 2, PropertiesLiveTests 25, SimulationLiveTests 17,
  SketchEditLiveTests 77, VisualsLiveTests 3, M0LiveTests 23; all passed but two. Known failure, not this group's:
  `PropertiesLiveTests.A_custom_material_is_listed_…` (24/25, as before S72). Flake: `AssemblyLiveTests.
  Dimension_set_on_a_mate_angle_an_equation_drives_…` refused a valid equation in the full class run (55/56) and passed
  alone after a SOLIDWORKS restart. Run class by class with SOLIDWORKS restarted between, free memory 2.6 GB
  before the first start after the build servers were shut down (0.5 GB with them up and SOLIDWORKS open); no "low on
  memory" dialog. The live tests took 65 min of test time (37, 1, 3, 6, 13, 1, 3 min, and the 1 min rerun of the flake;
  75 min on the clock with the restarts), the whole release about 82 min. Unit 299. `docs/tools.md` regenerated and committed alone (`098a10b`).
  Cost (for `learned.md` at the run's end), tokens / minutes: S52 162k/19, S61 (a) 175k/15,
  S72 139k/14 (resumed once after an API network error), S71 113k/13, S87 108k/25 (resumed
  once after the usage limit), S88 136k/11 (each the default model); group 5's release
  104k/83 (Sonnet); the scenario test 13 min, one run; its sort a subagent (default model).
- [x] Group 5: scenario test (session id: `793c7f74-1bf1-46d3-9199-eed2f71d1e46`), its entry committed and sorted
  One run, ~13 min, no resume; S71, S72, S87, S88 held and closed, S52 (c) held and closed;
  S61 (a) not held, fixed again (below); S90 new; S5 met again (findings 2026-11-17, `da55ff5`).
- [x] S61 (a) again: committed
  Cost: 153k tokens / 14 min (the default model, one run).
- [ ] Group 5 again: full live tests and release (build: )
- [ ] Group 5 again: scenario test (session id: ), its entry committed and sorted
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

### S52 - The boxes in results are loose, and say nothing of it - OPEN 2026-11-12

On build
`20261112-091908`. (a) `model.box` in feature, `equation_set` and component results is a
loose bound over B-spline and torus faces: ±30.63 for a body trimmed to Ø60; Y -16.3 to
216.3 where the faces were at -4 and 215; 0.09 outside a filleted plate (findings
2026-11-12, "a helical screw part", point 4; 2026-11-13, "a plate's two ends", point 2).
After a change of variables the box looked like a failed trim. (b) `asm_component_list`'s
and `asm_component_move`'s `box` for a rotated component is the part's box rotated and
re-boxed: 5 mm low at 7.5° about X, 33 mm high in another (findings 2026-11-12, "a part's
body raised", point 5; "a part placed in an assembly by hand", point 1). A clearance read
from it is wrong. Workaround: `face_list {extreme}` or `{component, normal}`, or the volume.
Local: a tight box (`GetPartBox` tight, or the bodies' `GetExtremePoint` in the
assembly's frame), or the word "approximate" in the result and the descriptions.
Taken into the run of 2026-11-13, no choice in it: a tight box (the bodies' extreme
points, in the assembly's frame for a component); where it cannot be read, the box is
marked approximate. Fixed in the run of 2026-11-13. Cause: `model.box` was the union of the
bodies' `GetBodyBox`, a loose bound over torus and spline faces, and a component's `box`
was `IComponent2.GetBox`, the part's box turned and re-boxed. Fix: a new `TightBox` reads
each body's `GetExtremePoint` along ±x, ±y, ±z of the target frame (for a component, the
directions taken into its part space and the points mapped back through its transform).
`model.box` in every result built from a snapshot (feature results, `equation_set`,
`dimension_set` and the other feature-management results, `model_info`, an import's
summary, sheet metal) and the `box` of `asm_component_list`, `asm_insert`,
`asm_component_move` and the pattern results use it. A body whose extremes can't be read
adds its own box, and a component without bodies (lightweight, or a sub-assembly that
gives none) keeps SOLIDWORKS' box; either way the box carries `approximate: true`. Not
covered by a live test: that fallback (no case on hand where `GetExtremePoint` fails).
New live tests: `The_model_box_after_round_faces_is_the_body_s_extremes` (a disc with its
top edge rounded; the plate of the finding, quarter-round ends into a 30° face with the
top chain rounded R0.5; an elliptic cylinder; `model_info`'s box too) and
`A_tilted_component_s_box_is_its_body_s_extremes` (a shaft r 5 × 30 turned 45° about
[1, 1, 0], the box from the cylinder's ends and rims). On the code before both failed:
"box [-56.582209, -5, -5] .. [56.589919, 5, 5.089919], expected [-56.5, -5, -5] .. [56.5,
5, 5]" (the finding's 0.09) and "box [-6.767767, -3.232233, -6.035534] .. [11.161165,
28.838835, 21.035534], expected [-4.946093, -2.605027, -4.330127] .. [9.339491,
28.211629, 19.330127]". After: both pass, with the box assertions of `Insert_puts_the_origin_…`,
`Insert_with_a_rotation_…`, `Move_translates_and_rotates_…`, `Subassemblies_are_inserted_…`,
`Circular_pattern_turns_the_seed_…`, `Box_extrude_…`, `Mid_plane_extrude_…` and
`Cut_from_an_open_sketch_…` (10/10); unit tests 276/276; `--check-catalog` ok (no
description changed: the `approximate` field speaks for itself); `check_repo` ok. Local:
on `sw2023`, `8dff953`; in build `20261114-024455`. Upstream: not yet
decided.
Held in its scenario test (findings 2026-11-14, point 6): (a) a disc's torus fillet box
exact, (b) a turned component's box the expected extremes. Not held (point 5): `doc_open`
of a STEP that `model_export` wrote from an assembly of three 20 mm cubes and a cylinder r
5 × 30 turned 45° about [1, 1, 0] brought the cylinder in as 3 surface bodies, its
component at the identity rotation with box [-5.0001, 0, -5] .. [6, 30, 5.427]: neither
the turned cylinder's box nor a tight one of r 5, and without `approximate`. Bears on S5
and on the fallback above. Back to Open 2026-11-14 for that. Not yet decided.
Taken into the run of 2026-11-16, group 5: the tight box reads surface bodies too, else
`approximate: true` (`## Run 2026-11-16`).
(c) Sorted out of the entry of 2026-11-16 (the scenario test of build `20261116-124830`),
point 7, reported `ok`: a flexible subassembly whose child top-level mates had moved from
[0, 15, 130] to [50, 40, 0] kept in its row (`asm_component_list`, `asm_component_set`)
the box [0, 0, 100] .. [20, 35, 150] with `approximate: true`: the box of its own file's
placement, which no longer holds the child at all. "Approximate" reads as loose, not as
the box of another state. It is the fallback above (a component that gives no bodies
keeps SOLIDWORKS' box). Proposed: a flexible subassembly's box from its children as they
are placed (the tight box over each), else a word saying it is the file's placement.
Local. Not decided.
Decided 2026-11-16 by the run, not the owner: (c) taken into the run of 2026-11-16, group
5, with S52, because it is in S52's files (`TightBox.cs`) and group 5 holds S52.
Met again 2026-11-16 (findings 2026-11-16, "a part edited by rollback", point 9), build
`20261116-124830`: `body_list` gave a box max of 149.016816 on a part whose
`equation_set` and `model_mass` boxes said 149.016716, the same body, nothing changed
between. New: `body_list`'s box is not the tight box (`IBody2.GetBodyBox` through
`SwExt.Box`, it seems), so two tools give two boxes for one body.
**Done in the run of 2026-11-16: the STEP case, not reproduced as a fault of the box.**
Test `A_step_s_surface_bodies_have_the_box_of_their_faces_as_imported`, the scenario's case
(a 20 mm cube and a shaft r 5 × 30 turned 45° about [1, 1, 0], STEP AP214 by `model_export`,
opened by `doc_open`). With the oracle of an r 5 cylinder as placed it failed on the code
before: "box [-5.000116, 0, -5] .. [6, 30, 5.427307], expected [-5, 0, -5] .. [5, 30, 5]".
The probe showed the box is already tight and true to what SOLIDWORKS imported: the row
reads the surface bodies too (sheet bodies are taken since the run of 2026-11-13), and the
shaft came in at the identity, its turn lost, its caps planes bounded by two spline curves
out to x 6 and z 5.427 (89.816 mm² each, a Ø10 disc being 78.540): `GetExtremePoint`,
`GetBodyBox`, the faces' boxes and their tessellation all reach there. Both are the
import's (S5, the probe recorded there). Decided 2026-11-16 by the run, not the owner: the
STEP case done as not reproduced, with no code change, because the box is the extremes of
the faces the document holds, and a box of the shape before the export would describe
geometry that is not there. The test kept with the oracle of the faces' tessellation
(within 0.05 mm, a new test hook `TestHooks.TessBoxAsync`; an `approximate` box passes),
passing on the code before and after.
**Done in the run of 2026-11-16: (c), fixed.** Cause: a subassembly gives no bodies through
`IComponent2.GetBodies3`, so its row fell back to `IComponent2.GetBox`, the box of its own
file's placement, marked approximate. Fix (`AssemblyService.CompBox`): a component without
bodies takes the union of its unsuppressed children's boxes as they are placed, each child
by the same rule (its bodies' extremes through its own transform, a nested subassembly its
children's), approximate only when a child's is; SOLIDWORKS' box stays the last fallback
(lightweight, empty). Decided 2026-11-16 by the run, not the owner: for every subassembly,
rigid too, not only a flexible one, because a rigid one's `GetBox` is the same turned and
re-boxed bound that (b) replaced for parts, and one rule is a smaller change than one per
solving. Test before: `A_flexible_subassembly_s_box_covers_its_children_as_they_are_placed`
(a subassembly of two 20 mm cubes at [0, 0, 100], made flexible, its floating child mated to
a cube's top plane at y 60): "box [-10, -10, 90] .. [50, 10, 110], expected [-10, -10, 90]
.. [50, 70, 110]". After: it passes (the flexible row [-10, -10, 90] .. [50, 70, 110] in
`asm_component_list` and `asm_component_set`, the rigid row exact, neither approximate),
with the live tests that read component boxes and subassemblies
(`Subassemblies_are_inserted_…`, the S61 STEP import, the flexible and rigid ones of S62,
S76 to S78, `A_tilted_…`, `Insert_puts_the_origin_…`, `Move_translates_…`, the subassembly
mate, `Two_components_form_a_subassembly_…`; 13/13 with the two new); unit tests 290/290;
`check_repo` ok; no description changed.
Made stricter: none. Local: on `sw2023`, in the commit that records it; in build `20261117-012659` (the S52 (c) point).
Upstream: not yet decided.
Decided 2026-11-16 by the run, not the owner: the item stays Open, not to Waiting, for
`body_list`'s box (met again 2026-11-16, above), a point found after the plan and not in
the run, because an item marked fixed would hide it.
Held 2026-11-17: (c), the scenario test of build `20261117-012659` (findings 2026-11-17,
point 2): a subassembly of two 20 mm cubes (one fixed, one floating) at [0, 0, 100], made
flexible, its floating child mated by its three planes onto a top-level cube at [50, 40,
0]: its row in `asm_component_list` [0, 0, 0] .. [70, 60, 120], not `approximate`, the
union of its children as placed; rigid at insert [0, 0, 100] .. [20, 35, 150], exact; set
rigid again the same, flexible again [0, 0, 0] .. [70, 60, 120]. (c) closed 2026-11-17 by
the run's rule (*Per group, the check*: held, closed); the item stays Open for `body_list`'s
box.
Met again 2026-11-17 (findings 2026-11-17, "a turned printed part with two swept internal
threads", point 5; "a turned printed part with a flange and a swept internal thread", point
4), build `20261117-012659`: `body_list`'s box read Y -2.03 to 104.80 for a part with swept
threads where `model.box` in every feature result read 0 to 102.89, the part's real extent
(the sweeps trimmed at both ends), and 99.86 against 97.954 in the other part. The same
point as above (`body_list`'s box is not the tight box), the gap much larger on B-spline
faces: it reads like their control points' box. Workaround: read `model.box` for extents.

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

### S81 - "modified outside this server (by the user?)" again after the server's own changes - OPEN 2026-11-16

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

### S83 - `asm_mate_add` distance with `flip` that lands right warns in the words of a wrong side - OPEN 2026-11-16

Sorted out of the entry of 2026-11-16 ("a part edited by rollback", point 5), on build
`20261116-124830`. `asm_mate_add` distance between two parallel planes, `alignment:
aligned`, 80: it landed on the far side (origin at -80, the part inserted at +80), with
the warning that says so (S50, Closed: held). Again with `flip: true` it landed right
(origin read back +80) but warned in the same words ("80 mm along its normal before, -80
mm after... If it should stay on the side it was on, delete the mate..."), which reads as
a second failure. The side check on `originMm` held.

Proposed: with `flip` asked, the warning says the side now differs from where the part
was inserted, as asked, not that it may be wrong. Local. Not decided.

### S84 - Feature results that read as faults on a right result: a mirror's area line, a pattern's rebuild warning under its old name - OPEN 2026-11-16

(a) Sorted out of three entries of 2026-11-16 ("a part edited by rollback", point 6; "an
edit to four printed parts", point 5; "an edit to three printed parts", point 7), on build
`20261116-124830`. `feature_mirror` of features about the symmetry plane gave "the copy's
faces have 0.86 x the seed's area" (then 1.001, 0.996, 1.006) while the volume was exactly
the seeds' (-2 627.2 = 2 x -1 313.6 in one) and every instance face was found (14/14,
7/7). The line is in `checks`, not a warning (`AreaNote`, S17 (a)'s fix, Closed: as
designed), yet each session took it for a warning and checked the volume by hand; "it
meets the body differently from the seed" reads as a clash.
(b) "An edit to four printed parts", point 2: `feature_pattern_linear` warned
"'LPattern1' has rebuild warning 51 (swSketchErrorExtRefFail)", naming the feature by the
name it had before the call's own `name` renamed it; the next `model_review` was clean.

Proposed: (a) when every instance face is found and the volume matches, the line says so
("faces merge differently from the seed's; volume and instance faces match"), or it is
left out below a wider ratio; (b) the rebuild warnings read after the rename's rebuild,
naming the renamed feature, a transient one dropped. Local. Not decided.
(c) Sorted out of the entry of 2026-11-17 ("a printed part with a plate tilted 20°",
point 2), on build `20261117-012659`: the same in another tool. `feature_extrude {cut,
depthMm:4, direction:"+x"}` from a standard plane through a 4 mm wall, the depth equal to the
wall on purpose, said in `checks` "no floor at 4 mm: the cut broke out of the body before its
end (D1@... reads 4 mm)" (S64 (a)'s line); the volume matched (-45.804). A right result
worded as a fault. Met again the same day ("three printed parts of a box and its lid", point
2), a blind cut 2 deep through a 2 mm wall, the volume exact. Proposed by the session: a
blind cut whose depth equals the material it crosses says "through: the end lies on the far
face". Local. Not decided.
Met again 2026-11-17 ((a); findings 2026-11-17, "an edit to three parts and their assembly",
point 4; "three printed parts of a box and its lid", point 2), build `20261117-012659`:
"the copy's faces have 1.004 x the seed's area: it meets the body differently from the
seed" on a mirror of three features whose volume closed exactly against the hand figure (to
0.01 mm³); 1.021 x and 1.003 x on two more mirrors, "instance faces 8/8" and "7/7", the
volumes exact.
Met again 2026-11-17 ((b), in another tool; findings 2026-11-17, "a turned printed part with
two swept internal threads", point 4), the same build: `feature_delete` of a sweep reported
"'<revolve cut>' now has rebuild error 51 (swSketchErrorExtRefFail)" for the next feature,
whose sketch is on a standard plane with the origin only; after the sweep was made again
before it, `model_review` read no error. Whether the error was real while it lasted is not
known.
Met again 2026-11-18 ((b); findings 2026-11-18, "a shared global variable changed in two
parts", point 5), build `20261118-060615`: `feature_pattern_linear` returned `ok` with
"warning 51 (swSketchErrorExtRefFail)" on the new pattern; after an `equation_set` on its
spacing the warning was gone and `model_review` clean. The session spent a review to tell
whether it was real.

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

### S89 - SOLIDWORKS started by `sw_connect launch` cannot be reached from a later server process - OPEN 2026-11-16

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

### S90 - `drw_view_section` with no `atMm` places a section so its own label overlaps the title block - OPEN 2026-11-17

Sorted out of the entry of 2026-11-17 (the scenario test of build `20261117-012659`), point
7; met in three scenario tests. `drw_view_section` with no `atMm` placed a section of a 1:2
plan where its own view label overlaps the title block by about 0.2 mm, and warned of it
("view label … overlaps the title block"). The warning is right, so not reported `ok`
while wrong; the free-space placement does not count the label the call is about to make.

Proposed: the free-space placement counts the view's label with the view before it places
it. Local, in the drawing code, not group 5's files: for the next plan, not this run. Not
decided.

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

### S93 - A write that rebuilds does not name a feature far from the change that gained a rebuild warning - OPEN 2026-11-18

Sorted out of the entry of 2026-11-17 ("a part's two plates cut back by redrawing one
sketch", points 1 and 4), on build `20261117-012659`. `dimension_set` of one sketch
dimension, then a sketch redrawn (`equation_delete` of its one equation, `sketch_edit`,
`sketch_delete` of all five segments, `sketch_add` of a closed polyline, `sketch_constrain`,
`sketch_finish`) and `equation_set` binding the new dimension to the old global: none of the
results named that a fillet at the end of the tree now had rebuild warning 13
(`swFeatureErrorFilletNoEdge`), one of its four edges lost; `equation_set` read
`"volumeDeltaMm3":0` and the model's volume. Only `model_review` showed it; the hand figure
caught it first (+37.6 mm³). Met again in the same session on a second redraw: the same edge
dropped, named by no write's result. The fillet references a mirrored body's edge far from
the sketch; by `at` on the same four points three edges resolved to the same ids as before
and the dropped one to a new id each time. `equation_set` reads new rebuild errors and
warnings since S58 (a) (Closed); here the warning came with the redraw, whose sketch tools
do not. Reported `ok`, the result incomplete, not wrong. Workaround: `model_review` after
every sketch redrawn; the fillet deleted and remade by `at` (3 calls).

Proposed by the session: a write that rebuilds compares the tree's rebuild errors and
warnings before and after, and names each feature that gained one. Local. Not decided.

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

### S95 - `dimension_list` marks a dimension an equation drives `driven: true`, which its description gives as a reference dimension - OPEN 2026-11-18

Sorted out of the entry of 2026-11-17 ("a read-only check of a printed part driven by global
variables", point 1), on build `20261117-012659`. `dimension_list` marked 38 of 42 dimensions
`driven: true`, every one an equation drives, and only the four plain values without it. Its
description says the flag is for reference dimensions, so the result reads as 38 reference
dimensions in a part whose sketches are all fully defined. The values were right; reported
`ok`. SOLIDWORKS gives `swDimensionDriven` for a dimension an equation drives (met in
`dimension_set`, S74, Closed); earlier entries read `driven:true` as "an equation drives it"
(S54, Closed).

Proposed by the session: two fields, `reference` (a true reference dimension) and `equation`
(the equation's text when one drives it), or the description saying what the flag means.
Local. Not decided.

### S96 - `sketch_constrain` does not check a relation to a model edge - OPEN 2026-11-18

Sorted out of the entry of 2026-11-17 ("three printed parts of a box and its lid", point 3),
on build `20261117-012659`. `sketch_constrain {relation:"concentric"}` between a sketch
circle and a model circular edge `{at, type:"edge"}` lying in the sketch's face: `checks`
"geometry not checked (model geometry, or a segment of a non-parallel sketch)", relation
added, `ok`. It was right (read after the cut by `face_list {surface:"cylinder", radiusMm}`),
but it is the check a session needs most for a relation to a model edge.

Proposed by the session: for a circular edge parallel to the sketch, the check reads the
edge's centre and radius against the sketch circle's centre. Local. Not decided.

### S97 - `material_list` with no match does not say which databases it read - OPEN 2026-11-18

Sorted out of the entry of 2026-11-17 ("a printed plate with two walls standing off it in an
L", point 2), on build `20261117-012659`. `material_list {filter}` with no match returned
`"materials":[]`, `"databases":[]` and a warning naming the closest names. `databases` lists
only the databases the filter reaches, so a miss does not show which were read, and a caller
cannot tell that a custom database was searched before calling a material missing (one is on
the machine, not named). Reported `ok`.

Proposed by the session: on a miss, `databases` lists every database searched with its
count, as a call with no filter does. Local. Not decided.

### S98 - `doc_open {readOnly:true}` reports `access: owned` and says the server may edit it - OPEN 2026-11-18

Sorted out of the entry of 2026-11-17 ("read-only checks of rebuilt parts and an assembly",
point 1). A read-only check opened six parts and an assembly with `readOnly: true`; each came
back `access: "owned"`, with the line that this server may edit it. Nothing was written only
because the session's brief said so: the result invites an edit the open was meant to rule
out.

Proposed by the session: a read-only open reports its own access (`readOnly`), and the edit
tools refuse on it with NOT_OWNED's hint. Local. Not decided.

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

### S100 - Library feature parts (.sldlfp): not opened or saved through the server, read as a wrapper, and a save that lost the sketch, reported `ok` - OPEN 2026-11-18

Sorted out of the entries of 2026-11-17 ("a weldment profile made from a vendor's DXF",
points 3 and 4) and 2026-11-18 ("reading a hand-saved weldment profile", point 2; "a custom
weldment profile with one size made to work", points 3 and 4), on build `20261117-012659`.
A weldment profile is a library feature part, the file `weldment_member` reads (S91).
(a) `doc_open` refuses `.sldlfp` ("can't open '.SLDLFP' files"): a profile could not be read
back, and a shipped one had to be opened by the user for a comparison; the read tools then
read it with `doc` given (foreign, not adopted).
(b) `doc_save` refuses any extension but the native one ("doc_save writes native .sldprt
files") and `model_export` has no such format, so a custom profile cannot be made end to end:
the user saved it as `.sldlfp` by hand and added the folder to File Locations. Proposed by
the session: `doc_save` takes `.sldlfp` for a part (SaveAs3 to that extension), and a tool or
an option adds a profile folder to the Weldment Profiles file location.
(c) `feature_list` on a library feature part shows its marked sketch inside a `LibFeat1` of
type `LibraryFeature`, where the FeatureManager shows the sketch at the top level marked L
and no `LibFeat1`. A session took that as the profile's fault, which cost two hand re-saves
(Dissolve Library Feature, Add to Library). Proposed by the session: `feature_list` says the
wrapper is internal, or lists the sketch as the library feature it is.
(d) **A result reported `ok` while wrong.** `doc_save` of an adopted library feature part
reported success and the profile sketch was gone on reopening. On a copy of a shipped
profile, adopted by `doc_activate {allowEdit:true}`: its configuration table deleted
(`feature_delete`, one table refused "Could not select"), `config_add` and three
`config_delete`, `sketch_edit` of the profile sketch, `sketch_delete` of all 16 segments,
`sketch_add` of 112, `sketch_finish` (4 closed contours), `doc_save` (`savedTo` the .sldlfp,
41 145 bytes against 74 903 before), `doc_close`. Reopened by the user: no profile sketch,
`LibFeat1` with no children. Not known which step dropped it; nothing reported a fault. A
`feature_list` after the save would have caught it before the close. Proposed: a probe of
which step, and `doc_save` of a library feature part checking its library feature still
holds its sketch.

Local. Not decided.

### S101 - `sketch_add` of a hundred entities takes minutes and times out - OPEN 2026-11-18

Sorted out of the entry of 2026-11-18 ("a custom weldment profile with one size made to
work", point 5), on build `20261117-012659`. `sketch_add` of 112 entities (106 lines, 4 arcs,
2 circles) took 183 s in a library feature part and 254 s in a new part, `TIMEOUT` at 50 s
both times, completed later (`sw_status` `lateCompletions`, succeeded): about 1.6 to 2.3 s an
entity. A profile of a hundred segments needs a batch path or a longer limit for large adds.
`sketch_import` of the same geometry from a DXF was the way that worked (S99).

Proposed: a probe of where the time goes (the per-entity checks, S65 (e)'s merge pass, the
contour read), then a batch path or a limit that grows with the count. Local. Not decided.

### S102 - `weldment_member`'s "No profile" lists a one-configuration file it then refuses - OPEN 2026-11-18

Sorted out of the entry of 2026-11-18 (the scenario test of build `20261118-060615`), point
2; met in S91's own fix (Closed). With a one-configuration copy of a profile directly in the
standard folder, a wrong `type` was refused "Profiles: <the copy's name>, <the subfolder
type>". The copy is offered as a type although the next call with it is refused
(`INVALID_ARGUMENT` "has one configuration ... would make no member"). Not reported `ok`.

Proposed by the session: leave a one-configuration file in the standard folder out of the
list, or mark it ("not usable: one configuration"). Local. Not decided.

### S103 - `weldment_member`: lines in two directions that do not touch fail with the path hint, and the result does not say which way the section lies - OPEN 2026-11-21

Sorted out of the entry of 2026-11-18 ("a weldment frame of 16 members in one custom
profile", points 1 and 2), on build `20261118-060615`.
(a) `weldment_member` on one 2D sketch of four lines, two vertical and two horizontal, none
touching another, all segments by default, `corner:"none"`, failed `VERIFY_FAILED: ...
'Weldment' was created but has no faces ... Rolled back`, hint "The segments must form a path
a profile can follow (no zero-length or overlapping lines)". The same sketch with `segments`
the two parallel lines, then the other two, made both members, volumes exact. Likely:
SOLIDWORKS takes a group of segments only when they are contiguous or all parallel. S91's
hint point (Closed), met with another cause. Not reported `ok`.
(b) The result does not say which way the section lies. `rotateDeg` 0 on vertical lines in
a sketch on a Right-offset plane put the section's long side along the sketch normal, 90 in
the plane; the same for horizontal lines there and on Front-offset planes; on a Top-offset
plane 0 gave the long side vertical. Found only from `model.box` and `body_list`'s boxes, and
one undo.

Proposed by the session: (a) the server splits the segments into groups by that rule (one
group per parallel set, as groups of one feature), or the hint names the rule; (b) each
member in the result gives its section's extents along the model axes, or the long side's
direction, so a wrong rotation shows in the call that made it. Local. Not decided.

### S104 - `material_list` lists one database twice, under its short and its long path - OPEN 2026-11-21

Sorted out of the entry of 2026-11-18 ("a weldment frame of 16 members in one custom
profile", point 3), on build `20261118-060615`. `material_list {filter:"606"}` returned 24
rows for 12 materials and two entries in `databases` of 259 each: one database under its 8.3
path and under its long path. Reported `ok`. Bears on S97 (the databases a miss reads).

Proposed: the database paths taken to their long form before they are listed and searched.
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

### S106 - The drawing's overlap checks count neither tables nor views, and `drw_table_add` gives no extent - OPEN 2026-11-21

Sorted out of the entry of 2026-11-18 ("a two-sheet A3 drawing of a 16-member weldment
part", point 1), on build `20261118-060615`. **A result reported `ok` while wrong.**
`drw_table_add {kind:"cut_list", atMm:[408,285]}` returned the rows but no extent; the table
came out about 201 x 71 mm, over a side view. A 145 x 118 mm note added after it read "clear
of the border, the title block and the other annotations" while it lay across both the table
and the side view's members; only the PDF showed it.

Proposed by the session: `drw_table_add` returns the table's `extentMm` and warns of what it
lands on; the note and dimension checks count tables and the views' `boundsMm`, not only
annotations. Local. Not decided.

### S107 - `asm_insert {doc}` warns it re-activated the assembly, which reads as if the target changed - OPEN 2026-11-21

Sorted out of the entry of 2026-11-18 ("an edit to a printed plate and a new flat part",
point 3), on build `20261118-060615`. After `doc_new` of a part, `asm_insert
{doc:<assembly>}` placed it and warned "Re-activated '<assembly>' (the active document was
'<part>')"; the next `asm_mate_add` calls without `doc` were refused `WRONG_DOC_TYPE`, the
target still the new part. Right by the rules (the target is set by `doc_new` and
`doc_activate`, not by `doc`), but three calls lost; one `doc_activate` fixed it.

Proposed by the session: the warning adds "the target is still '<part>'". Local. Not
decided.

### S108 - `doc_new {type:"drawing"}` with no `template` replaces a custom default template's format with ISO and its units with MMGS - OPEN 2026-11-21

Sorted out of the entry of 2026-11-18 ("a custom sheet format made from the ISO one with
`vba_run`", point 6), on build `20261118-060615`. With SOLIDWORKS' default drawing template
set to one holding a custom format and custom units (mass in kg), `doc_new {type:"drawing"}`
reported "Created drawing from <that template>; ... format a3 - iso.slddrt" and units MMGS;
with `template:<the same file>, units:"template"` it kept both. Said in the result, so not
reported wrong, but a user who set a default template does not get it.

Proposed by the session: keep the template's sheet whenever the default drawing template is
not the shipped one, and say so; or a server setting for the default drawing template.
Local. Not decided.

### S109 - `NOT_CONNECTED` after `asm_component_move` and `doc_rebuild` on a subassembly open beside its parent, SOLIDWORKS responding - OPEN 2026-11-21

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

### S110 - `dimension_set` of a limit mate's nominal outside its limits is refused without naming them - OPEN 2026-11-21

Sorted out of the entry of 2026-11-18 ("a shared global variable changed in two parts",
point 6), on build `20261118-060615`. `dimension_set` of a limit mate's nominal to a new
minimum was refused (`INVALID_ARGUMENT`, "rejected 15 mm ... as invalid") until
`dimension_tolerance_set` widened the range (type `limit`, lower -23.5, read back as
`limits: [15, 72.5]`). Not reported `ok`.

Proposed by the session: the hint says a limit mate's nominal must lie inside its limits,
and that `dimension_tolerance_set` moves them. Local. Not decided.


## Waiting

### S1 - `drw_view_section` / `drw_view_detail` drew in view-sketch coordinates - BUILT 2026-11-09, proof: a session that runs `drw_view_detail` and a view at an angle

Local:
`1de5067` + `3f43c50` on `sw2023`, run and held 2026-11-09 in build `…-163234`. Upstream:
PR #1 (branch `fix/section-sheet-coords`, one commit `81475a5`), tested on 2023 SP3 only;
on 2025 the mapping may be wrong if sheet coordinates work there. Under the rule of 2026-11-14
(the owner) PR #1 no longer holds it: it closes when `drw_view_detail` and a view at an
angle have run and held, and moves to Merged upstream if PR #1 is merged (the local
commits then drop out at the next rebase). Held again
in builds `…-195750` and `…-204719` (two sections, cut where drawn). Not yet run:
`drw_view_detail`, views at an angle. Not yet checked: `drw_view_edit broken_out` passes its circle in sheet
coordinates straight into the active view (its comment says sketch coordinates are the
sheet's there), which S1 found false for sections and details; its live test only counts
the cut, so a circle landing elsewhere on the part would pass.

**Upstream: PR #1, open.**

### S61 - A STEP into an assembly - BUILT 2026-11-21, proof: the scenario test of the next build

On build `20261112-091908` (findings 2026-11-12, "STEP
parts brought toward an existing assembly"). (a) `asm_insert` refuses a STEP; SOLIDWORKS
inserts one by 3D Interconnect, linked to the file. (b) `doc_open` of a single-solid STEP
came in as an assembly of one virtual component, reported `solidBodies: 0, surfaceBodies:
0`, while `model_mass` read a solid: the body counts are wrong for an assembly import, and
S5's usage line ("check `solidBodies`") reads 0 on a good solid. (c) The import left an
empty `Part2` open, unnamed in the result, later closed with another assembly. Bears on
S5. Not needed by that job. Taken into the run of 2026-11-13: (b) and (c), no choice in
them: the body counts read through the components for an assembly import, the stray
document named in the result. (a) offered 2026-11-13 and not chosen; stays open. (b) and
(c) fixed in the run of 2026-11-13; (a) still open. What makes it an assembly (live, 2023
SP3, 3D Interconnect off as `doc_open` sets it): a STEP with an assembly level in its
product structure, here one written by `model_export` from an assembly of one 20 mm cube
(the writer of the finding wraps a single solid the same way); a STEP of a single part
still comes in as a part. The component points to `<step name>_1.sldprt` beside the STEP,
not written until saved, and SOLIDWORKS leaves an empty part (`Part2`, `Part4`: no bodies,
used by nothing) open; closing it leaves the assembly and its component whole (noted in
`docs/solidworks-api-notes.md` §1). (b) Cause: `OpenService.Summarize` read only the
component count and box of an assembly, so `solidBodies` and `surfaceBodies` kept their
default 0. Fix: for an assembly import they are counted through the part components
(`GetBodies3`, suppressed ones and subassemblies skipped), `volumeMm3` summed from those
solids, and the message says "1 component(s) (1 solid body)". (c) Cause: the import marked
every document it created as owned ("created by importing …") and named none but the
result. Fix: `imported.otherDocuments` names each, as "D3 'Part4' (part, no bodies)", and
the message says it was left open, with `doc_close {doc, discard:true}` for an empty one;
it is not closed by the server. Before, on the code before the fix: the new
`AssemblyLiveTests.A_step_imported_as_an_assembly_counts_bodies_through_its_components_and_names_what_it_left_open`
failed ("… "SolidBodies":0,"SurfaceBodies":0 … "Components":1 … | new documents: D3
'Part2' part owned"). After: it passes, with `ExportLiveTests.An_exported_step_imports_back_…`,
`An_exported_stl_imports_back_with_a_body`, `Open_refuses_missing_files_…`,
`A_file_that_is_already_open_…` and `RecognitionLiveTests.An_imported_step_block_…`, 6/6
live; `FaceEditLiveTests.Delete_heals_a_hole_in_an_imported_STEP_body_without_history`
fails the same before and after (the import reads volume 0: S5, not this item); unit tests
284/284; `ContractTests` 7/7; `check_repo` ok; no description changed. Made stricter:
none. Decided 2026-11-13 by the run, not the owner: (1) the empty part is named, with the
close hint, and not closed by `doc_open`, because the decision was to name it and it is a
document SOLIDWORKS opened; (2) every other document the import created is named, with its
body count, not only empty parts, because the result then says all that the import left
open; (3) `volumeMm3` is given for an assembly import too, from the components' solids,
because a part import gives it and a count without it reads as less than `model_mass`
shows; (4) only part components are counted (subassemblies skipped), because a
subassembly's bodies are its parts' and would count twice. Local: (b) and (c) on `sw2023`,
`a81f93d`; in build `20261114-024455`. Upstream: not yet decided.
Held in the scenario test of build `20261114-024455` (findings 2026-11-14, points 5, 6):
(b)'s message said "3 solid bodies, 3 surface bodies" plainly; (c) the empty part named.
Those parts closed 2026-11-14 on the owner's word; the item stays Open for (a).
Decided 2026-11-16 by the owner: (a) taken into the run of 2026-11-16, group 5, a probe of
the insert through 3D Interconnect first (`## Run 2026-11-16`).
(a) done 2026-11-16 in the run of 2026-11-16 (group 5). Probe (live, 2023 SP3, a STEP of a
20 mm cube written from the part): `AddComponent5` of the STEP returns null, with 3D
Interconnect on and with the STEP loaded first by `LoadFile4` (with Interconnect on, a
part holding an `MBimport` feature and no path); `OpenDoc6` of it fails (2097152);
`IAssemblyDoc.InsertImportedComponent` with 3D Interconnect on returns 0 and the
component, one solid of 8000 mm³, linked to the file (`IFeature.Is3DInterconnectFeature`);
with it off, 1 and nothing. Its part is `<step file name>.SLDPRT` in SOLIDWORKS' working
directory (a folder of the user's, not the STEP's), not written until the assembly is
saved, which writes it there. Cause: `asm_insert` took a .sldprt or .sldasm only
(`ResolveFile`) and loaded it by `OpenDoc6` for `AddComponent5`, and neither takes a STEP.
Fix: a .step or .stp goes in by `InsertImportedComponent` with 3D Interconnect on and
Import Diagnostics off in a preference scope, the working directory set to the STEP's
folder for the call, both restored after, so its part lies beside the STEP; then it is
placed and verified as any component, and the checks add its solid bodies with their
volume and its 3D Interconnect feature; surface bodies only warn, no bodies refuse and
roll back; the message names the link and the part, not written until the assembly is
saved; `configuration` with a STEP is refused; SOLIDWORKS refusing the insert gives
`FEATURE_FAILED` with the way round (doc_open the STEP, doc_save it as a .sldprt,
asm_insert that), and another format's refusal names the same. The `path` description
names .step. Before: the new
`AssemblyLiveTests.A_step_is_inserted_as_a_component_linked_to_the_file_with_its_solid`
failed ("'path' must be a .sldprt or .sldasm file, not '.step' (got
'…\stepins_….step')."). After: it passes (one component, origin [50, 0, 0], box ±10 mm
about it, "1 solid body (8000 mm³)", `model_mass` 8000 mm³, the part beside the STEP and
written by the assembly's save, `configuration` refused), with
`Insert_puts_the_origin_at_originMm_…`, `Insert_with_a_rotation_…`,
`Insert_refuses_unsaved_missing_and_foreign_files_…` (a missing STEP now an I/O error, an
.igs still refused), `Subassemblies_are_inserted_…`,
`Configurations_are_chosen_at_insert_…`, `Replace_swaps_the_file_…` and
`A_step_imported_as_an_assembly_…`, 8/8 live; unit tests 290/290;
`ContractTests.Tool_catalog_follows_the_conventions` 1/1; `--check-catalog` ok;
`check_repo` ok. Noted in `docs/solidworks-api-notes.md` §8. Made stricter: none. Decided
2026-11-16 by the run, not the owner: (1) the working directory is the STEP's folder for
the insert, because SOLIDWORKS otherwise names the part in whatever folder its working
directory is and the assembly's save writes it there; (2) a STEP that comes in as surface
bodies only is kept with a warning, not refused, because the insert is what was asked and
the result says what came in (S5's case); (3) only .step and .stp are taken, not IGES or
the other formats 3D Interconnect reads, because the item and its test are a STEP's; (4)
the `path` description adds ".step" and nothing more, the link named in the result,
because the tool list is at its byte budget. Local: on `sw2023`, in the commit that
records it; in build `20261117-012659` (the S61 (a) point). Upstream: not yet decided.
(a) not held 2026-11-17, back to not done, reported `ok` while wrong: the scenario test of
build `20261117-012659` (findings 2026-11-17, point 1). `asm_insert` of a STEP of a 20 mm
cube in five cases, each a fresh copy of the STEP in its own folder: every result one
component, "1 solid body (8000 mm³)", linked by 3D Interconnect, its part
`<step folder>\<name>.step.SLDPRT`, "not written until the assembly is saved". Held where
the STEP is the assembly's first component (fixed by SOLIDWORKS), the assembly saved in the
STEP's folder or one folder up: the part written beside the STEP. Not held when the
assembly already holds a component: a part inserted first and the STEP after (floating),
then `doc_save as`; the STEP into an assembly holding a subassembly and a part,
`doc_save as` then `doc_save` in place; the STEP after a subassembly into a saved assembly,
`doc_save` in place. In each the part was not written: left open, `dirty: true`, listed
`foreign` (so `doc_save` cannot save it), nothing in any result saying so, `doc_save`
reporting the assembly saved. Closing the assembly closed the part unsaved, a warning
naming it the only sign; reopened, the assembly read `missingReferences`
`<assembly folder>\<name>.step.sldprt` (the assembly's folder, not the STEP's) and the
component `suppression: suppressed`: the component lost from the saved assembly. The live
test covers the first-component case only. Seen with it, possibly the same cause: the
assemblies saved in place read `dirty: true` again after the save, with no change made,
once other documents had been activated (a later `doc_close` refused `DIRTY` on one that
had read clean after its save). Proposed by the session: after the assembly's save, check
that each such part was written, and save it (the component's document) when not, or say
in the save's result that it was not. By the run's rule (*Per group, the check*: a fix of
this group not holding), (a) is fixed again and group 5 built and checked again
(`## Run 2026-11-16`).
(a) fixed again 2026-11-21 in the run of 2026-11-16 (group 5 again). Cause (live, 2023
SP3): SOLIDWORKS' save of an assembly writes the part of a 3D Interconnect component only
when it is the assembly's first component; inserted after another component (fixed or
floating alike, probed), `SaveAs3` or `Save3` of the assembly reports success and leaves
the part unwritten, open, dirty and hidden. The part's own `Save3` then fails with
`swReadOnlySaveError` though it is not opened read-only, while `IModelDocExtension.SaveAs3`
to its own path writes it. Nothing claimed the part, so it was `foreign` and `doc_save`
could not save it either. Fix: `asm_insert` of a STEP registers the part it made (no file
yet) as owned, "created by inserting <step>"; `doc_save` (every mode) and `doc_close
{save}` of an assembly then write each owned component part not on disk by `SaveAs3` to
its own path, read back, the save's message naming it ("Also wrote '…': the assembly's
save left it unwritten."); a component file not on disk that is not the server's, not
loaded, or not written is named in a warning ("… will be missing when the assembly is
reopened"), so the save no longer reports `ok` silently. Before: the new
`AssemblyLiveTests.A_step_inserted_after_a_part_is_written_by_the_assemblys_save_and_resolves_on_reopen`
(a part inserted, then the STEP floating, `doc_save as`, closed and reopened) failed
("…\stepafter_….step.SLDPRT was not written by the assembly's save: {… "Dirty":false,
"Access":"owned" …} | open: D1 'stepafter_block_….SLDPRT' owned dirty=False, D4
'stepafter_….step.SLDPRT' foreign dirty=True, D3 'stepafter_asm_….SLDASM' owned
dirty=False"). After: it passes (the part owned after the insert, written beside the STEP
by the save and clean; reopened with no missing reference, the component resolved), with
`A_step_is_inserted_as_a_component_linked_…`, 2/2; then `Insert_puts_the_origin_at_originMm_…`,
`Subassemblies_are_inserted_…`, `Replace_swaps_the_file_…`,
`Insert_refuses_unsaved_missing_and_foreign_files_…`, `A_step_imported_as_an_assembly_…`,
`Pack_and_go_copies_…`, `AssemblyStructureLiveTests.A_speedpak_configuration_…`,
`TopDownLiveTests.Saved_bodies_become_parts_…`, `M0LiveTests.Dirty_owned_documents_need_save_or_discard`
and `Save_copy_writes_a_fresh_file_…`, 10/10 live; unit tests 302/302; `check_repo` ok; no
description changed. Noted in `docs/solidworks-api-notes.md` §8. Made stricter: every
assembly save warns of a component file it leaves off disk. Not looked into: assemblies
saved in place reading `dirty: true` again once others were activated; not shown to share
this cause (the test's assembly read clean after its save). Decided 2026-11-21 by the run,
not the owner: (1) the part a STEP insert makes is owned by the server, only when its file
did not exist before the insert, because the server created it and `doc_save` and
`doc_close` must be able to act on it, while an existing file stays the user's; (2) the
check runs after every assembly save (in place, as, copy, `doc_close {save}`), at all
levels and for any component, not only 3D Interconnect parts, because any component whose
file is not on disk is lost on reopen and the result should say so; (3) a part that cannot
be written is a warning, not an error, because the assembly itself was written and an
error would read as if it had not; (4) the part is written under the write-folder policy
(`ForWrite`), a refusal named in the warning, because a server write outside the allowed
folders is refused everywhere else too. Local: on `sw2023`, in the commit that records it;
not yet in a build. Upstream: not yet decided.
