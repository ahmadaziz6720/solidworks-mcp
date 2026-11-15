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

**Open: S4, S5, S7, S10 to S12, S33, S34, S49, S52, S59 to S61, S64, S65, S67, S71, S72, S74, S75.**
Sorted 2026-11-16, the scenario test of build `20261116-050908` (findings 2026-11-16):
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
and S20 say. Next: the owner's decisions on S49, S51, S52, S59 (c), S72, and on what the
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
- [x] S62 (b): committed
- [ ] S75: committed
- [ ] Group 2: full live tests and release (build: )
- [ ] Group 2: scenario test (session id: ), its entry committed and sorted
- [ ] S49: committed
- [ ] S74: committed
- [ ] S67: committed
- [ ] Group 3: full live tests and release (build: )
- [ ] Group 3: scenario test (session id: ), its entry committed and sorted
- [ ] S59 (c): committed
- [ ] S65 (e): committed
- [ ] S65 (c): committed
- [ ] S65 (b): committed
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

### S49 - `dimension_set` through an assembly does not say which component, nor the document it changed - OPEN 2026-11-12

In the scenario test of build `20261112-091908` (findings
2026-11-12, second entry, points 3, 4), a two-part assembly mated by hand. (a)
`dimension_set` with a component's dimension (`D1@Boss-Extrude1@<component>-1`) made the
change right, but the result named it `D1@Boss-Extrude1`, without the component, and
carried no volume change, which the description promises and a part's result gives. With
two components holding a `D1@Boss-Extrude1` the row would not say which changed. (b) The
change went to a part `doc_list` lists as `foreign` (`loadedBy` the assembly), leaving it
dirty; closing the assembly with `discard` closed and discarded it too ("SOLIDWORKS also
closed"), the part's file not touched. Whether a referenced part counts as the assembly's
for the server's rule on foreign documents is not said anywhere, and the result does not
name the other document. Reported success, rightly. Bears on S27. Local: the component in
the row, the assembly's volume change, and a line naming the part changed and not saved.
Taken into the run of 2026-11-13 as proposed, no choice in it.
Fixed in the run of 2026-11-13. Cause: `dimension_set` named each row by the dimension's
own full name with the document's suffix cut (`FeatureVerifier.ShortName`), which for a
component's dimension is the part's `D1@Boss-Extrude1`; so two components holding one were
refused "listed twice", and the read-back after the rebuild looked the dimension up again by
that short name in the assembly. The volume change was read for a part only, and nothing
looked at the documents a change through the assembly reached. Fix: in an assembly each row
keeps the name as given (`D1@Boss-Extrude1@<component>-1`), which also keys the "listed
twice" check and the read-back; the volume change of an assembly is that of its mass
properties (`CreateMassProperty2().Volume`) before and after; the update stamp of every
other open document is read before the change, and each one whose stamp moved and that is
now dirty is named in a warning, "Also changed, not saved: '<title>' (<handle>, <access>), a
component's own document." Whether a referenced part counts as the assembly's for the
server's rule on foreign documents is not decided here: the fix does not need it, the change
goes through as before and is now named, with the part's access. Test before:
`AssemblyLiveTests.Dimension_set_through_an_assembly_names_the_component_the_volume_and_the_part_it_changed`
(two cubes in an assembly, both `D1@Boss-Extrude1@…` set in one call) failed on the code
before, "'D1@Boss-Extrude1' is listed twice." After: it passes, with each row named as given,
20 → 30 and 20 → 25, the volume change 6 000 mm³ and a line naming each part, and none
naming the assembly; with it the live tests that call `dimension_set` through an assembly or
on a part, `A_change_through_a_document_an_assembly_loads_does_not_read_as_the_users`,
`A_part_the_server_owned_comes_back_owned_when_its_owned_assembly_loads_it`,
`Compare_cannot_open_a_revision_…`, `Configurations_are_chosen_at_insert_…` of
`AssemblyLiveTests`, `TopDownLiveTests.An_in_context_cover_…`, the three `Dimension_set_…` and
`Rename_reads_back_and_renames_the_dimensions` of `FeatureManagementLiveTests`, M0's
`Dirty_reads_unsaved_changes_made_through_another_process`, pass (11/11; in a first mixed run
`A_part_the_server_owned_comes_back_owned_…` failed once, its message not kept, and passed in
the three runs after, alone and in two batches); unit tests 284/284; `check_repo` ok; no
description changed. Made stricter: none (a batch the old check refused, two components'
dimensions of one name, now goes through).
Local: on `sw2023`, `2a41e9e`; in build `20261114-024455`. Upstream: not yet
decided.
Release run 2026-11-14: `A_part_the_server_owned_comes_back_owned_…` failed every time, alone
too, after its asserts held: "System preferences changed by this test: Integer230: 0 -> 1".
Cause, by a probe reading preference 230 after each step: SOLIDWORKS itself writes it.
`ActivateDoc3(title, false, swDontRebuildActiveDoc)` on a document with no window of its own
(`Visible` false: the part the reopened assembly loaded, brought forward by `doc_open`'s
already-open path) sets `RebuildOnActivation` 0 -> 1; on a visible document it stays 0. No
scope held it there, so nothing restored it (`EnsureLoaded`'s scope was not on this path;
the test does not depend on S49's code). The same call sits wherever the server brings a
document forward: a write bound to such a part's handle (S62 d makes it owned) and
`doc_activate` leak it too. Fix: `SwContext.ActivateDoc`, which holds the preference at that
value in a `PreferenceScope` when the document is not visible (a visible one is activated as
before), used at the nine places that bring a target forward (bind, `doc_open`,
`doc_activate`, export, Pack and Go, view, simulation, recognition, the assembly's
reactivation); the five that bring back the previously active document are left as they
were. Noted in `docs/solidworks-api-notes.md` (*Activate*). New test
`Activating_a_part_an_assembly_loaded_leaves_the_rebuild_preference_as_it_was` (a write bound
to the loaded part's handle, then `doc_activate` of it) failed on the code before at its
first step, "Expected: 0, Actual: 1". After, each batch on a freshly restarted SOLIDWORKS: the
two tests alone 1/1 each; with them the live tests that bring a document forward through the
changed code, 17: `AssemblyLiveTests` `A_change_through_a_document_…`, `Replace_swaps_…`,
`Compare_cannot_open_…`, `Pack_and_go_copies_…`, `Configurations_are_chosen_at_insert_…`,
`Insert_refuses_…`; `ExportLiveTests` `A_file_that_is_already_open_…`,
`A_saved_part_reopens_…`, `Images_pdf_and_edrawings_are_written`; M0's
`Dirty_reads_unsaved_changes_…` and `Screenshot_returns_a_png_…`; `VisualsLiveTests`'
`A_section_cuts_the_image_…`; `FeatureManagementLiveTests`' `The_servers_own_failed_kept_…`;
`SimulationLiveTests`' `Simulation_refuses_to_change_the_users_documents`;
`RecognitionLiveTests`' `An_imported_step_block_…`; `SketchEditLiveTests`'
`Edit_reopens_a_finished_sketch_…`; `ConfigurationLiveTests`'
`A_material_set_in_one_configuration_…`: 19/19 pass. Unit tests 284/284; `check_repo` ok; no
description changed. Local: on `sw2023`, `cfd987e`; in
build `20261114-024455`.
Held in its scenario test (findings 2026-11-14, points 4, 6): the component in the row and
the volume, in a hand-mated assembly too. Left, wording (point 4): the line naming the
part reads "Also changed, not saved: '<part>.SLDPRT', a component's own document.",
without handle and access, in a new assembly whose parts the server had not listed yet,
and "'<part>.SLDPRT' (D19, foreign), …" in an assembly opened from file. Back to Open
2026-11-14 for that. Not yet decided.
Taken into the run of 2026-11-16, group 3: the line names the part with its handle and
access always, a part not yet listed registered first (`## Run 2026-11-16`).

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

### S59 - Reference geometry that cannot be made - OPEN 2026-11-12

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

### S67 - `feature_rollback` before the only feature fails its check - OPEN 2026-11-13

Seen in the run of
2026-11-13, in S60's probe, on that run's code: `feature_rollback {to:<renamed extrude>,
before:true}` on a part holding one extrude failed `VERIFY_FAILED` "… unexpected state:
'Sketch1'". A valid call refused, the rollback's state not said. Not probed: whether the
rename, the single feature or the check's reading of the bar is the cause. Local. Decided
2026-11-13 by the run, not the owner: an item of its own, open and not fixed in the run,
because the run's items were the owner's and this one has not been looked at.
Taken into the run of 2026-11-16, group 3, a probe first (`## Run 2026-11-16`).

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

### S74 - `dimension_set` on a mate's angle driven by an equation calls it a driven (reference) dimension - OPEN 2026-11-16

On build `20261112-091908` (findings 2026-11-13, "two printed plates", point 7):
`dimension_set` on an angle mate's dimension driven by an equation refused it "is a driven
(reference) dimension: it reports a value but doesn't control the geometry". It is
driving, through an equation; for a part's dimension the refusal names the equation's
variable, for a mate's it does not. A hint that misses. Bears on S37 (Closed). Local. Not
yet decided.
Taken into the run of 2026-11-16, group 3: the refusal names the equation and its variable
(`## Run 2026-11-16`).

### S75 - `feature_chamfer` lists in `newFaces` only the named chamfer's faces when an edge moves to a chamfer of its own - OPEN 2026-11-16

Sorted out of the entry of 2026-11-16, point 2. On build `20261116-050908`, with S51's fix:
a new `distance_distance` chamfer with `flipEdges:[1]` on two edges meeting at a corner
moved edge 1 to `Chamfer2` (said in a warning, `alsoCreated`), and the result gave
`faceCountDelta` 2 but `newFaces` one face, the named chamfer's; the split-off chamfer's
face took a `face_list {feature}`, one call more. Reported `ok`; the count was right.

Proposed: `newFaces` lists the faces of every chamfer the call made, those of
`alsoCreated` named apart. Local, the S51 code. Taken into the run of 2026-11-16, group 2
(a small point in group 1's files).

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

### S68 - `asm_mate_add` coincident with an origin fails - BUILT 2026-11-16, proof: the scenario test of group 2's build

Seen in the run of 2026-11-13, in
S62's tests: a coincident mate with the assembly's origin or a component's origin failed
"SOLIDWORKS created no coincident mate … (unknown error (status 0))". Not probed: how the
origin is selected for the mate. Bears on S62. Local. Decided 2026-11-13 by the run, not
the owner: an item, open, as S67.
Taken into the run of 2026-11-16, group 2, a probe of the origin's selection first (`##
Run 2026-11-16`).

Fixed in the run of 2026-11-16. Probe: the origin of a component selected by
`SelectByID2("Point1@Origin@<component>@<assembly>", "EXTSKETCHPOINT", …)` and the
assembly's by `"Point1@Origin"`, the same type, each gave selection type 25 and a mate
that solved. Cause: the resolver gives an origin as its `OriginProfileFeature`, and the
mate selected that feature itself (`Select2`), from which `AddMate5` makes no mate and
reports status 0. Fix: `SelectForMate` selects an origin by its point, `Point1@<origin>`
as `EXTSKETCHPOINT`, through the same names a component's plane takes (the
`"…@<component>@<assembly>"` form, then `GetSelectByIDString`'s); an origin that cannot be
selected so is refused naming the names tried. Test before:
`Coincident_mates_take_the_assembly_s_origin_and_a_component_s_origin` (three 20 mm cubes
floating at other places; a component's origin on the assembly's, a component's origin on
another's corner, a corner on a component's origin) failed on the code before at its first
mate: "asm_mate_add: SOLIDWORKS created no coincident mate between origin 'Origin' of
orgA_…-1 and origin 'Origin' (it reported an unknown error (status 0))." After: it (the
origins at (0, 0, 0), (10, 10, 10) and (20, 20, 20), three mates listed), with
`Plane_mates_coincident_and_distance_reproduce_the_4000_mm3_overlap` and
`Mate_list_names_the_plane_face_edge_or_vertex_each_mate_takes`, 3/3, on 2023 SP3; unit
tests 287/287; `check_repo` ok. No tool description changed.
Decided 2026-11-16 by the run, not the owner: an origin whose point cannot be selected is
refused before the mate, not selected as the origin feature as before, because that
selection makes no mate and the caller would get an unknown error (status 0) instead of
what failed.

Local: on `sw2023`, in the commit that moves it here; not yet in a build. Upstream: not yet
decided.

### S69 - A component's named plane inside a subassembly cannot be selected - BUILT 2026-11-16, proof: the scenario test of group 2's build

Seen in the
run of 2026-11-13, in S62's tests: "Could not select 'b' (plane 'Top Plane' of
sub-1/part-1)". Not probed: whether the select path for a nested component's plane is
built right. Local. Decided 2026-11-13 by the run, not the owner: an item, open, as S67.
Taken into the run of 2026-11-16, group 2 (`## Run 2026-11-16`).

Fixed in the run of 2026-11-16. Probe: for a nested component `GetSelectByIDString`
gives `sub-1@top/part-1@sub`, and `SelectByID2("Top Plane@sub-1@top/part-1@sub",
"PLANE", …)` selects the plane (true, one object); the feature's own `Select2` does too.
Cause: the first name tried, `"Top Plane@sub-1/part-1@top"` (the component's `Name2`),
is no name, yet `SelectByID2` returns false *and* selects the plane. The plane stayed
selected, so each later try (the `GetSelectByIDString` form, then `Select2`) added one
more and the count check (one object more than before) failed every time. Fix:
`SelectIds` leaves the `Name2` form out for a nested component (its `GetSelectByIDString`
form alone); `SelectForMate` drops whatever a failed try left selected before the next,
as `ref_plane`'s component select already did. The nested name noted in
`docs/solidworks-api-notes.md`. Test before:
`A_plane_of_a_component_inside_a_subassembly_takes_a_mate` (a 20 mm cube at y 15 in a
subassembly placed at z 100, a second cube at (50, 40, 0); its top plane coincident with
the nested cube's) failed on the code before: "Could not select 'b' (plane 'Top Plane'
of nest_sub_…-1/nestA_…-1) for the mate." After: it (the second cube's origin at y 15,
"planes coincide (0 mm apart)", the mate listing the nested component's `Top Plane`),
with `Coincident_mates_take_the_assembly_s_origin_and_a_component_s_origin`,
`Plane_mates_coincident_and_distance_reproduce_the_4000_mm3_overlap`,
`Mate_list_names_the_plane_face_edge_or_vertex_each_mate_takes` and
`Ref_plane_and_ref_axis_in_an_assembly_take_component_and_assembly_references`, 5/5, on
2023 SP3; unit tests 287/287; `check_repo` ok. No tool description changed.
Decided 2026-11-16 by the run, not the owner: a select try that returns false is undone
before the next, for every component plane, axis, point and origin, not only for a
nested component, because a false result that still selected leaves the selection
unknown, and the next try's count would then read wrong.

Local: on `sw2023`, in the commit that moves it here; not yet in a build. Upstream: not yet
decided.

### S70 - A top-level component may resolve to a nested one of the same short name - BUILT 2026-11-16, proof: the scenario test of group 2's build

Seen
once in the run of 2026-11-13, in S62's tests: a top-level component whose name matched a
nested component's short name seemed to resolve to the nested one. Not confirmed: a probe
first (two components, one top-level and one in a subassembly, named alike). Local.
Decided 2026-11-13 by the run, not the owner: an item, open, as S67, because a reference
that reaches the wrong component would change the wrong part.
Taken into the run of 2026-11-16, group 2, a probe first (`## Run 2026-11-16`).

Fixed in the run of 2026-11-16. Probe: a cube saved in a subassembly, that subassembly
inserted first into a new assembly, then the same cube at the top level; both are
`<cube>-1`, the nested one `<sub>-1/<cube>-1`. `asm_component_move` with the top-level
name was refused "'<sub>-1/<cube>-1' is inside subassembly '<sub>-1', which moves as
one": reproduced. Cause: the resolver (`EntityResolver.Component`, which every
`component` argument goes through) asked `IAssemblyDoc.GetComponentByName` first, and
given `part-1` it returns a nested `sub-1/part-1` that comes before the top-level
`part-1`; `asm_structure`'s and the configuration table's own lookups did the same. Fix:
the resolver matches the full name (`Name2`) over every component first, then a nested
component by its short name, then `GetComponentByName`; a short name that two or more
nested components share, with no component of that full name, is refused
`ENTITY_AMBIGUOUS` naming them. `asm_structure` and the configuration table take the full
name first too (`EntityResolver.ComponentByName`). Noted in
`docs/solidworks-api-notes.md`. Test before:
`A_top_level_component_is_reached_by_its_name_when_a_nested_one_has_the_same_short_name`
failed on the code before at its first call: "'twin_sub_…-1/twin_…-1' is inside
subassembly 'twin_sub_…-1', which moves as one." After (with the refusal of a short name
two subassemblies share, added with the fix): the top-level one moved by 10 mm in z and
hidden while the nested one stays at (0, 15, 100) and visible, a mate on its top plane
and one on a face picked in it each listing it and the nested one; with
`A_plane_of_a_component_inside_a_subassembly_takes_a_mate`,
`Component_set_floats_hides_suppresses_and_returns_the_previous_state`,
`Mates_that_cant_hold_are_refused_rolled_back_and_nothing_moves`,
`Exploded_view_moves_each_component_by_the_sum_of_its_steps`,
`Linear_pattern_places_instances_at_the_spacing_along_one_and_two_directions`,
`A_limit_angle_mate_reads_fully_constrained_as_SOLIDWORKS_marks_it_in_the_tree`,
`AssemblyStructureLiveTests.Two_components_form_a_subassembly_and_dissolve_back_with_the_same_parts`
and `ConfigurationLiveTests.An_assembly_table_picks_component_configurations_and_suppresses_components`,
9/9, on 2023 SP3; unit tests 287/287; `check_repo` ok. No tool description changed.
Made stricter: a short name that several nested components share, and no component has
as its full name, is refused instead of taking the first.
Decided 2026-11-16 by the run, not the owner: that short name is refused
`ENTITY_AMBIGUOUS` naming the components, not resolved to the first, because the first
is whichever SOLIDWORKS lists first and a call would change a part the caller did not
mean.

Local: on `sw2023`, in the commit that moves it here; not yet in a build. Upstream: not yet
decided.

### S62 - Assembly reading and options - BUILT 2026-11-16, proof: the scenario test of group 2's build

On build `20261112-091908`. (a) `asm_mate_list`
gives each entity `{component, kind: "plane"}` with no name, face id or point, so which
plane or face a mate takes cannot be told (findings 2026-11-12, "a two-part assembly",
point 1; bears on S36). (b) No "solve as flexible" for a subassembly in
`asm_component_set` (findings 2026-11-12, "two parts mated into a subassembly", point 1).
(c) A subassembly component held by a limit angle mate reads `constrained: "fully"` and
`model_review` names no floating component; whether SOLIDWORKS says the same is not
settled (same entry, point 2). To probe live first. (d) `doc_open` of a part the server
had owned, now loaded by an assembly the server opened, returned it `foreign`, "the user's
document", and `equation_set` failed `NOT_OWNED`; `doc_activate {allowEdit:true}` got past
it (findings 2026-11-12, "a part edited while an assembly the server opened held it";
bears on S27 and S49 b). (e) `asm_interference` with `includeCoincident:true`: 9 or 10
pairs of 0 mm³ with "SOLIDWORKS counted 2 interference(s) but listed 25" (or 26); the
three numbers do not explain each other (findings 2026-11-13, "four small printed parts",
point 3; "a revolved part remade", point 4). Taken into the run of 2026-11-13: (a) and
(e), no choice in them; (c) a probe first, against SOLIDWORKS' own tree. (d) Decided
2026-11-13 by the owner: a part loaded by an assembly the server owns, from a path the
server owned, counts as owned. (b) offered 2026-11-13 and not chosen; stays open. (a), (d)
and (e) fixed in the run of 2026-11-13, (c) probed and not a fault; (b) still open. (a) Cause:
the row gave `IMateEntity2.ReferenceType` and the component only; `Reference` was never read.
Fix: each entity gives `name` for a plane, axis or other feature (`Reference` is its
`IFeature`, live: "Front Plane" of the component, or of the assembly with no component),
`id` for a face, edge or vertex (registered as `face_list` registers it: the same id, live),
and `pointMm` (SOLIDWORKS' `EntityParams`, assembly mm) for a vertex or point and for an
entity with neither. (c) The probe, live on 2023 SP3: a subassembly held by concentric,
coincident and a limit angle mate (30..50°) on its child reads `GetConstrainedStatus` fully
constrained and its tree text loses "(-)"; a part held the same way reads the same. SOLIDWORKS
counts a limit mate as fixing its freedom, so `constrained: "fully"` and `model_review` agree
with it; nothing to fix. Kept as
`AssemblyLiveTests.A_limit_angle_mate_reads_fully_constrained_as_SOLIDWORKS_marks_it_in_the_tree`
(passes before and after; it reads the tree text through `TestHooks.ComponentTreeTextAsync`).
(d) Cause: the registry forgets a document's class when it closes, and a reference an owned
document loads was always tracked foreign. Fix: `DocumentRegistry` keeps the paths of
documents it owned in this server process, closed ones included; a document first seen with
such a path, with no window of its own and held as a component by an owned assembly, is owned,
"loaded by D… from a file this server owned". (e) Cause: the result gave the rows and a bare
warning; live, the first `GetInterferenceCount` on a fresh manager with coincidence on
answered 1 (the overlap) while `GetInterferences` listed 3 (the overlap and two touching
pairs). Fix: the result gives `listed` (what SOLIDWORKS listed, before the components
filter), `overlapping` and, with `includeCoincident`, `touching` (0 mm³, faces in contact),
and a check line saying so and how many of the listed involve the components named; the
count is asked again after the list, and a warning, naming both calls, comes only when they
still differ. Descriptions: `asm_mate_list` names the entities' plane name or id;
`includeCoincident` says touching pairs read 0 mm³. Before, on the code before the fix:
`Mate_list_names_the_plane_face_edge_or_vertex_each_mate_takes` failed ("Assert.All()
Failure: 2 out of 2 items in the collection did not pass", no name);
`Interference_with_coincident_faces_says_what_SOLIDWORKS_listed_overlapping_touching_and_kept`
failed (no `listed`, `overlapping` or `touching`; warning "SOLIDWORKS counted 1
interference(s) but listed 3."); `A_part_the_server_owned_comes_back_owned_when_its_owned_assembly_loads_it`
failed ("foreign: 'owned_….SLDPRT' was already open in SOLIDWORKS (the user's document,
D10) …"). After: those three and the (c) test pass, with `Plane_mates_coincident_and_distance_…`,
`Interference_of_two_overlapping_cubes_…`, `Parallel_perpendicular_and_angle_mates_…`,
`Replace_swaps_the_file_…`, `Mates_that_cant_hold_…`, `Concentric_mate_…`,
`Component_set_floats_…`, `A_change_through_a_document_an_assembly_loads_…`,
`Insert_refuses_unsaved_missing_and_foreign_files_…`,
`DrawingLiveTests.Closing_a_drawing_names_what_solidworks_closed_with_it`,
`ExportLiveTests.A_file_that_is_already_open_…` and `A_saved_part_reopens_as_owned_…`, 16/16
live; unit tests 284/284; `ContractTests` 7/7; `--check-catalog` ok; `check_repo` ok. Made
stricter: none; (d) widens ownership, and its test shows a part the user opened before the
server's assembly loaded it stays foreign. The (d) test writes with `dimension_set`: in that
SOLIDWORKS session `equation_set` refused every equation, on a new part too, until SOLIDWORKS
was restarted (its own hint). Decided 2026-11-13 by the run, not the owner: (1) `pointMm` only
for a vertex or point and for an entity with neither name nor id, because a name or id already
says which and a point says where a point is; (2) the name is the feature's own ("Front Plane")
beside `component`, not a select string, as `asm_mate_add` takes it; (3) (d) claims only a
document with no window of its own, held by an owned assembly, when first seen, not one a
drawing loads, because the decision names an assembly and a visible document may be the
user's; (4) the owned paths live in the server process, not in the persisted registry, because
the finding was within one session and a restarted server re-claims open documents already;
(5) `count` stays the rows given (after the components filter) and `listed` is added, because
callers read `count`; (6) a pair overlaps when its volume reads above 0 at 3 decimals, as its
row shows it. Local: (a), (d) and (e) on `sw2023`, `af3598c`; in
build `20261114-024455`. Upstream: not yet decided.
Held in the scenario test of build `20261114-024455` (findings 2026-11-14, point 6): (a)
plane names and face ids; (d) a part loaded by the server's assembly owned; (e) listed,
overlapping and touching. Those parts closed 2026-11-14 on the owner's word; the item
stays Open for (b).
Decided 2026-11-16 by the owner: (b) taken into the run of 2026-11-16, group 2,
`asm_component_set` `flexible` for a subassembly component (`## Run 2026-11-16`).

(b) fixed in the run of 2026-11-16, the item's last open point. Cause: `asm_component_set`
had no option for how a subassembly is solved, and `IComponent2.Solving` is read-only in
the 2023 interop. Fix: `flexible` (true flexible, false rigid) for a subassembly component,
set with the component selected through `IAssemblyDoc.CompConfigProperties4`, its
suppression, visibility, configuration and BOM exclusion passed as they are, then read back
with `Solving` (`VERIFY_FAILED` if it did not change, the earlier changes of the call
reverted); refused `INVALID_ARGUMENT` for a part component before anything changes. Each
subassembly's row (`asm_component_list`, the result and its `previous`) gives `flexible`,
null for a part. Noted in `docs/solidworks-api-notes.md`. To stay within the tool list's
byte budget, `asm_component_set`'s first sentence, which listed what its parameters say,
is now "Change a component's state." (tools/list 253,579 bytes, 25 under the budget, 33
before). Test before:
`A_subassembly_is_solved_flexible_and_rigid_again_as_SOLIDWORKS_reads_it` (a subassembly
of a fixed cube and a floating one, fixed in a top assembly beside a fixed third cube) did
not compile on the code before: "error CS0117: 'AsmComponentSetRequest' does not contain a
definition for 'Flexible'". After: it (`Solving` read 0, 1 after `flexible:true`, 0 after
`flexible:false`, "rigid → flexible" and back, the second call "nothing"; the part refused;
flexible again, the floating child's top plane mated to the third cube's moved its origin
to (40, 30, 100) while the subassembly stayed at (0, 0, 100)), with
`Component_set_floats_hides_suppresses_and_returns_the_previous_state`,
`Configurations_are_chosen_at_insert_and_switched_by_component_set`,
`Subassemblies_are_inserted_and_listed_with_their_children`,
`A_plane_of_a_component_inside_a_subassembly_takes_a_mate` and
`ContractTests.Tool_catalog_follows_the_conventions`, 6/6 live on 2023 SP3; unit tests
287/287; `--check-catalog` ok; `check_repo` ok; `docs/tools.md` regenerated. Made
stricter: none.
Decided 2026-11-16 by the run, not the owner: (1) set through `CompConfigProperties4` with
the component's other values passed as read, because `Solving` cannot be written in the
2023 interop and that call takes it, and the read-back of `Solving` is the check; (2) a
subassembly is told by its file (`.sldasm`), not by its children, because a lightweight or
empty subassembly has none to count; (3) `flexible` read back in every subassembly's row,
because the result has to show it and a caller can then see which are flexible; (4) the
bytes paid by the description's first sentence, because the parameters already say each
change it listed.

Local: (b) on `sw2023`, in the commit that moves it here; not yet in a build; proof: the
scenario test of group 2's build. Upstream: not yet decided.
