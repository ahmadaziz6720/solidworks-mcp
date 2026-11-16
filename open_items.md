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

**Open: S4, S5, S7, S10 to S12, S33, S34, S52, S60, S61, S64, S65, S71, S72, S79 to S86.**
Sorted 2026-11-16, working sessions' entries on build `20261116-124830` (findings
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
  --add-dir "<the fork>"`; cut short, `claude -p
  --resume <uuid>` from the same folder. It starts with `sw_status` reading the new
  version (step 6's short check). Its `findings.md` entry is committed here unasked and
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
- [x] S65 (b): committed
- [ ] S65 (d): committed
- [ ] Group 4: full live tests and release (build: )
- [ ] Group 4: scenario test (session id: ), its entry committed and sorted
- [ ] S52: committed
- [ ] S61 (a): committed
- [ ] S72: committed
- [ ] S71: committed
- [ ] Group 5: full live tests and release (build: )
- [ ] Group 5: scenario test (session id: ), its entry committed and sorted
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

### S61 - A STEP into an assembly - OPEN 2026-11-12

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
here; not yet in a build. Upstream: not yet decided.
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
records it here; not yet in a build. Upstream: not yet decided.
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
(b), the reading, on `sw2023`, in the commit that records it here; not yet in a build. Upstream:
not yet decided.

### S71 - A frequency study with no fixtures fails in the solver - OPEN 2026-11-14

Seen in the release runs
of 2026-11-14: `SimulationLiveTests.Frequency_of_the_cantilever_column_matches_beam_theory_and_a_free_bar_has_six_rigid_body_modes`
fails at its second study, the free-free column (`sim_run` frequency, 8 modes, no fixed
faces, default mesh size, mesh quality High), with "The solver did not finish:
RunAnalysis returned 24 (swsRunAnalysisErrorAnalysisFailed)" after ~40 s; its first study,
the fixed-foot one (4 modes, 2.5 mm mesh), passes. Four runs of four, two of them on a
fresh SOLIDWORKS with 1.6-2.0 GB free; the same on build `091908`'s code, and no commit
since that build touches `SimulationService` or the test. The server picks no solver, so
SOLIDWORKS 2023 chooses one for a stiffness matrix with six rigid-body modes. Not probed:
whether a solver choice (FFEPlus, Direct Sparse), a mesh size or the soft-spring option
lets it finish. Local, upstream's test. Decided 2026-11-14 by the run, not the owner: an
item, open, as S67, not fixed in the release run, because it fails the same on the code
before the run.
Taken into the run of 2026-11-16, group 5, a probe of soft springs and the solver first
(`## Run 2026-11-16`).

### S72 - A SOLIDWORKS process in a bad state: a material applied at 1 kg/m³ with a warning only, a linked property in metres, every equation refused - OPEN 2026-11-14

In the scenario test of build
`20261114-024455` (findings 2026-11-14, point 1), on a SOLIDWORKS process started before
the session (what ran in it earlier is not known): every equation was refused, a batch of
ten globals and then `"W" = 60mm` alone in a new part, with S58 (b)'s message as designed.
In the same process `material_set "AISI 1020"`, with or without a configuration, applied
the material at 1 kg/m³: `densityKgM3: 1`, `databaseDensityKgM3: 7900`, the check
"material reads back" and only a warning ("The mass properties give 1 kg/m³, the database
7900 kg/m³ (bodies with their own material, or an overridden mass?)"), no error, though
the description says the mass properties are verified to show its density. A user-library
material at 1270 kg/m³ read right in that process. A property linked to a 3 mm dimension
(`"D1@<feature>"`) resolved `0,0030000000000000`, metres. After a restart of SOLIDWORKS
the equations went in at once and a database material read 2700 kg/m³, its database
density. The same three symptoms were met together once before, in an old SOLIDWORKS
session (S28's test). Reported `ok` while wrong. Proposed by the session: `material_set`
refuses (`VERIFY_FAILED`, rolled back) when the density read differs from the database's,
as it once did, and its message, like S58 (b)'s, names a restart when a database material
reads ~1 kg/m³. Bears on S58 (b), S30. Local. Not yet decided.
Decided 2026-11-16 in the plan, not by the owner, because the description promises the
density verified and the session proposed it: `material_set` refuses (`VERIFY_FAILED`,
rolled back) when the density read differs from the database's by more than 1 % with no
body material of its own and no mass override, naming a restart at about 1 kg/m³. Taken
into the run of 2026-11-16, group 5 (`## Run 2026-11-16`).

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

### S85 - `feature_delete`'s dry run lists as kept an absorbed sketch the real call removes - OPEN 2026-11-16

Sorted out of the entry of 2026-11-16 ("an edit to three printed parts", point 4), on build
`20261116-124830`. The dry run of a delete of a loft cut said `keptAbsorbed: ["...start
sketch", "...run sketch<2>"]`; the real call removed the `<2>` one and said so plainly:
"SOLIDWORKS also removed '...<2>' (absorbed by ...), which was not planned." The `<2>` is
the loft's reference to a sketch another feature owns. Not a wrong result, but the dry run
did not say what the call would do. Bears on S64 (d) and S54 (Closed parts and item).

Proposed: a probe of how SOLIDWORKS lists such a reference; the plan counts it as going
with the feature that holds it. Local. Not decided.

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

### S59 - Reference geometry that cannot be made - BUILT 2026-11-16, proof: the scenario test of build <stamp>

On build `20261112-091908`. (a) No plane
on a planar face: `ref_plane` `offset` 0 and `angle` 0 are refused (findings 2026-11-12,
"six printed and stand-in parts", point 2). (b) `ref_plane` and `ref_axis` in an assembly:
`WRONG_DOC_TYPE` (same entry, point 3; bears on S36). (c) A sketch line as the axis of an
`angle` plane is refused, "needs edge or axis", which SOLIDWORKS takes (same entry, point
2; "a printed part's tree deleted", point 4); way round: `ref_axis` `two_planes` or
`two_points` first (findings 2026-11-12, "two parts changed mid-tree", point 8). (d) No
change of an existing plane's `flip`; a plane made on the wrong side is made again with its
sketch (same entry, point 6). Missing options. Decided 2026-11-13 by the owner: all four.
**Done in the run of 2026-11-13, all four.** (a) Cause: `offsetMm` 0 and `angleDeg` 0 were refused by the argument checks.
Fix: `offsetMm` 0 makes a plane coincident with the base (constraint 4), checked parallel
and on it ("coincident with face F…"); `angleDeg` 0 is taken (0 up to 180 exclusive),
the plane through the axis parallel to the base, checked as any angle plane. (c) Cause:
`axis` resolved edges and axes only. Fix: a sketch line is taken (`LineOf` reads its ends
in model space through the sketch's transform), checked as an edge is. (d) Fix: `ref_plane`
with `edit: "<plane>"` and `flip: true` toggles `IRefPlaneFeatureData.ReversedReferenceDirection`
of the plane's distance or angle reference (probed: `ReverseDirection` is not the flip) and
rebuilds; checked: an offset plane lies as far on the other side of its base, an angle
plane still holds its axis at the same angle the other way, and no feature gained a
rebuild error; else it is flipped back (`VERIFY_FAILED`, or `REBUILD_ERROR` naming the
broken features). The result gives the frame, the checks ("flipped: 15 mm along the
normal … (was 25 mm)", the children rebuilt), the volume change and `model`. Decided
2026-11-13 by the run, not the owner: (1) the flip is `ref_plane`'s `edit` parameter, not
a tool of its own (`kind` optional with it), as `feature_chamfer`'s `edit`, because the
tool list was at its byte budget; (2) a flip that breaks a feature is undone, as
`dimension_set` does, because a result must not report a broken model as done; (3) only
offset and angle planes are flipped (a standard, three-point, mid or coincident plane is
refused), because only their flip moves the plane; (4) `flip` with `offsetMm` 0 is
ignored with a warning, because SOLIDWORKS's flip option leaves a coincident plane's
frame as it was (probed), so it would be reported as a flip that did nothing; (5) to
make room, descriptions trimmed: `ref_plane` "Reference (datum) plane" to "Reference
plane", its `axis` "straight edge or axis" to "edge, axis or sketch line"; `ref_axis`
"Verified: its direction and position match the references" to "Verified against the
references", its `entities` example without "or [{type:'face', at:[x,y,z]}]";
`ref_point` "the point SOLIDWORKS reports is where requested" to "the point is where
requested", its `entities` "e.g. [{type:'edge', at:[x,y,z]}]" to "(edges, faces)";
`ref_coordsys` "frame (origin, x, y and z axes)" to "frame (origin, axes)" (tools/list
253,593 bytes, 11 under the budget, 7 before). Before, on the code before the fix,
`Plane_on_a_planar_face_takes_offset_0_and_angle_0` failed "kind 'offset' needs offsetMm
(non-zero; negative = the other side of the base).", `Angle_plane_turns_about_a_sketch_line`
failed "'axis' resolved to sketch segment Line1, but this parameter needs edge or axis.";
`Edit_flips_a_plane_to_the_other_side_and_rebuilds_its_children` is new API (no edit
existed): an offset plane flipped with a body on it and back (box top 20, then 30), an
angle plane turned the other way, a flip that leaves a cut in the air undone with
`REBUILD_ERROR`, and four refusals that change nothing. After: those three and the
plane, axis-by-edge and coordinate-system tests of `ReferenceLiveTests` (18), with
`An_equation_on_a_plane_s_offset_gives_where_the_plane_went` and the 7 `ContractTests`, 26 of 26;
unit tests 278 of 278; `--check-catalog` ok; `check_repo` ok. Made stricter: none.
(b) Cause: `ref_plane` and `ref_axis` took a part only (`ctx.Part()`). Fix: both take a
part or an assembly. In an assembly the new plane or axis is the assembly's own; its
references are the assembly's planes or a component's faces, edges, vertices, planes and
axes (`{component, name}`, or `at:` in assembly coordinates). A component's geometry reads
in its part's space (`docs/solidworks-api-notes.md` §8), so each reference is mapped
through the component's transform (`AsmXformData`) before the request is checked against
it, a component's plane, axis or point read from the feature in its own document; a
component's plane or axis is selected by its `SelectByID2` name, as `asm_mate_add` does
(`AssemblyService.SelectIds`, now internal); `EntityResolver`'s results carry the
component they came from. The checks add "made in the tree of assembly '…'". Decided
2026-11-13 by the run, not the owner: (1) `ref_plane` `edit` (the flip) stays part-only,
refused `WRONG_DOC_TYPE` in an assembly, because the component a flipped plane's base
belongs to cannot be read from the plane's definition without a probe; (2) `ref_point`
and `ref_coordsys` stay part-only, because the item names plane and axis only. Before, on
the code before the fix (HEAD's `ReferenceService.cs`), the new
`AssemblyLiveTests.Ref_plane_and_ref_axis_in_an_assembly_take_component_and_assembly_references`
failed `WRONG_DOC_TYPE` "This tool needs a part, but 'asm_ref_…' is a assembly." After: it
passes (a box turned 90° about z and moved, a shaft turned about x: planes offset from a
component's face, from its Top plane and from the assembly's Right plane, at an angle
about its edge, midway between two of its faces, normal to its edge, through three points
in an assembly 3D sketch; axes from its Top plane and the assembly's Front plane, along
its edge, through two of its vertices, of the shaft's cylinder; each where the transform
puts it, the box part's tree unchanged, the flip refused with nothing changed), with the
`Plane_*`, `Axis_*`, `Point_*` and `Coordinate_system_*` tests of `ReferenceLiveTests` and
`An_equation_on_a_plane_s_offset_gives_where_the_plane_went` (18), the ten other plane
tests of `ReferenceLiveTests` (offset, angle, three points, parallel, mid, flip, open
sketch) and the 7 `ContractTests`, 36 of 36 live; unit tests 278 of 278; `check_repo` ok;
no description changed. Made stricter: none.
Local: on `sw2023`, (a), (c) and (d) in `3ca75cd`, (b) in `42589af`;
in build `20261114-024455`. Upstream: not yet decided.
Held in its scenario test (findings 2026-11-14, points 3, 6): (a), (b) and (d). (c) went
through only with the type given: `ref_plane` `angle` with `axis: {name:"Line1",
sketch:"Sketch6"}` failed `ENTITY_NOT_FOUND: No feature named 'Line1'`; with
`type:"sketch_segment"` added it checked right ("contains sketch segment Line1, 30° to
plane 'Front Plane'"). With `sketch` given the reference is a sketch segment or point, the
server's own convention, so the type could be inferred. One call more. Back to Open
2026-11-14 for that. Not yet decided.
Taken into the run of 2026-11-16, group 4: (c), with `sketch` given and no `type` the
reference resolves in that sketch (`## Run 2026-11-16`).
(c), the type, done in the run of 2026-11-16, group 4. Cause: `EntityResolver` read a name
with no `type` as a feature's (or as the one kind the parameter allows), and read `sketch`
only once the type said sketch segment or point, so `{name:"Line1", sketch:"Sketch6"}` was
looked up as a feature. Fix: with `sketch` given and no `type` (and no `component`), the
reference is in that sketch: a name is a segment of it, an `at` of [x, y] a point of it,
in every tool that resolves through `EntityResolver` (`ref_plane`'s axis, `ref_axis`'s
points, a revolve's axis, a sketch tool's reference in another sketch). Before, on the
code before the fix, the new
`ReferenceLiveTests.A_reference_with_its_sketch_and_no_type_is_in_that_sketch` failed
`ENTITY_NOT_FOUND` "No feature named 'Line1'." After: it passes (an angle plane at 30°
about a sketch line given as `{name, sketch}`, an axis through two of its points given as
`{at:[x,y], sketch}`, a segment name not in the sketch refused `ENTITY_NOT_FOUND` "No
sketch segment named 'Line9' in Sketch2" with the tree unchanged), with
`Angle_plane_turns_about_a_sketch_line` and the `Axis_*`, `Point_*` and
`Coordinate_system_*` tests of `ReferenceLiveTests`, the three `SketchEditLiveTests` that
take a reference in another sketch,
`PartFeatureLiveTests.Revolve_around_a_named_centerline_when_the_sketch_has_two` and
`ModellingLiveTests.A_curve_pattern_puts_five_holes_equally_along_a_sketch_line`, 20 of 20
live; unit tests 290 of 290; `check_repo` ok; no description changed. Made stricter: a
reference with `sketch` and no `type` whose name is a feature's (`{name:"Front Plane",
sketch:"Sketch2"}`) was that feature, its `sketch` ignored; it is now looked up in the
sketch and refused `ENTITY_NOT_FOUND` when no segment there has that name. Decided
2026-11-16 by the run, not the owner: (1) the inference is in `EntityResolver`, so it
holds for every tool taking a reference with `sketch`, not `ref_plane` alone, because
`sketch` names the owning sketch in all of them ("Owning sketch
(sketch_segment/sketch_point)"); (2) a name is a segment, never a point, because sketch
points are taken by `at` only here and have no name the server reads, so a name both could
match does not arise; `{name:"origin", sketch}` stays the origin, as the sketch tools read
it; (3) with `component` (an assembly) nothing is inferred, because a component's sketch
segment is not resolved by name today with the type either, a change of its own; (4) an
`at` of three values with `sketch` is not taken as a sketch point, because a sketch point's
`at` is [x, y] and dropping z would take a point the request did not name.
Local: on `sw2023`, in the commit that moves it here; not yet in a build. Upstream: not yet
decided.
