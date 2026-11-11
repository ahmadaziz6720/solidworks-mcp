# Open Items

What sessions found in this fork's server and what was decided about each, S-numbered.
The raw entries are in [`findings.md`](findings.md); this file holds the items. Each item
names where it goes: **upstream** (an issue or PR at `soheil-mp/solidworks-mcp`), **local**
(a fix on `sw2023`, sent upstream as a PR when it is general), **usage** (a note for the
sessions that use the server), or **not taken**.

Three sections: **Open**, not yet fixed; **Waiting**, fixed and not yet settled (a PR not
merged, a fix not yet run in a session); **Closed**, merged upstream, run and held, or
decided against, with the record.

**Open: S4, S5, S7, S10 to S14, S24, S30, S32 to S34, S36, S46.**
Decided 2026-11-11 by the owner, each in its item below. Every open item can be worked on
but these: S4, S5, S7 and S12 wait on upstream; S10, S11, S33 and S34 are parked until a
job needs them; S36 is done in the scenario test, not here. S43 and S44 were found on
build `…-064352`; their code is unchanged in `20261111-140208`. Each fix starts with its
live test on the current build: an item whose test does not fail there is closed instead.

**Order of work** (the owner, 2026-11-11): modelling first, the largest code first, then
the small ones; drawings after; the release last.

1. Modelling, large: S37, S42, S41, S45, S40 and S38 are done (Waiting, on `sw2023`, in
   no build yet).
2. Modelling, smaller: S43, S18, S31, S44, S39, S27 and S35 are done (Waiting); S30
   probed, waits on the owner. S6's wording is done on `sw2023` and on
   `fix/angular-supplement`, the push to PR #6 waiting on the owner's word.
3. Drawings: S14 (the largest: view axes, border and title block, extents), S24, S32,
   S13 (a description line).
4. Release: S46's line in `CLAUDE.md` step 4, then a build as that file says, every
   project registered, and the opening sentence for the scenario test. A build may come
   earlier, after a group, when the owner asks. Before it: `docs/tools.md` regenerated
   (`solidworks-mcp --tools-md docs\tools.md`), since `equation_set`, the pattern
   tools, `sketch_dimension_add` and the 37 tools with `faceDetail` changed their
   descriptions or parameters and the file was left as it was; and the live tests of
   the areas touched in full (Properties, SketchEdit, PartFeature, Modelling, Assembly),
   of which only the tests that call the changed code were run.

---

## Open

**S4. `drw_dimension_add` keeps wrong values** (supplement reported as a match; an angle for
`vertical`). Upstream: issue #5.

**S5. `doc_open` STEP imports surfaces with 3D Interconnect forced off, reported `ok`.**
Upstream: issue #2. Usage until fixed: import by File > Open and `doc_activate`, or check
`solidBodies` after `doc_open`. Still so in builds `…-204719`, `…-215810` and `…-230924`:
a STEP round trip gave 16 surface bodies and no solid. Again in `…-235702`,
`…-010708`, `…-014952`, `…-024431`, `…-034022`, `…-064352`, `…-091645`, `…-115800`,
`…-134233`, `…-144220` and `20261111-140208` (export `solids: 1`, import 16 surface bodies).

**S7. `dimension_set` accepts a change that splits the body.** Upstream: issue #4. Usage:
read `bodies` in the result after each `dimension_set`. A model driven by global variables
changes through `equation_set`, whose result has no `bodies`: read `model_mass` after it
until the build with S37 (b), then `model.bodies` in its result.

**S10. Drawing format and title block.** Precision and decimal separator of drawing
dimensions, parentheses for reference dimensions, `n×` in hole callouts, DWG NO from a
property, material and weight, section label text and place, unaligning a view, rotating
a projected view (a side view projected from a top view came rotated 90°). Upstream
ROADMAP T10.3, T10.10, T10.12 partly. Not yet decided which to build. Usage for hole
callouts until then: model holes as one feature plus a pattern, end condition through, so
the callout gets `n×` and "through". Parked 2026-11-11 by the owner: taken up when a job
needs it.

**S11. No dimension to a section's cut face.** `drw_dimension_add` takes model entities only.
Parked 2026-11-11 by the owner: taken up when a job needs it.

**S12. Section orientation is not in the tool description.** Arrow side and view direction
(findings 2026-11-09, point 12). Upstream: proposed in PR #1's text; a description change
when PR #1 is settled.

**S13. `drw_dimensions_auto` gives dimensions that cannot be used, reported as success.**
On a part with fully defined sketches: 25 dimensions, construction ones among them,
duplicates, overlapping texts, some off the sheet, none for features whose sketches had
none. Without a delete tool they could not be removed, so the drawing was restarted; with
S3's `drw_annotation_edit` (Waiting) they can be deleted one by one. Usage: manual
`drw_dimension_add` only. Decided 2026-11-11 by the owner: no code change; the
description says the result needs cleaning and points to `drw_dimension_add`.

**S14. A drawing view's axes and the sheet's drawable area are not reported.** Which model
axis points where on the sheet (in a top view model −Z points up) decides the side for an
angular's `textAtMm`, guessed wrong it gives the supplement (S4). The format's border and
title block are not reported either: a note at x = 18 mm overwrote the zone letters of the
ISO A3 border at ≈19 mm. Local: model-to-sheet axes in the `drw_view_add` / `drw_info`
result, border and title block rectangles in `drw_info`; then PR. Met again in the scenario
test of build `…-230924`: `drw_view_edit` move reported "no overlaps with other views"
while the moved section view's label sat on the letter of another section line in the
parent view; the check covers view bounds only, not labels, section letters or
dimensions. Again in `…-235702`: `drw_annotation_edit` moved a section label 4.3 mm onto
the other section line's letter, `position read back`, no warning; moving a label is the
easiest way to land one on another view's annotation. Again in `…-010708`: `drw_note_add`
120 mm from the sheet's right edge with a first line ≈180 mm long, `position read back`, no
warning, the line cut off at the page edge in the PDF; the result gives the position, not
the note's extent. Again in `…-034022`: `drw_dimension_add` `horizontal` with `textAtMm` on
the row of an existing dimension from the same edge, its dimension line on the other's and
the two texts side by side, `textAtMm` echoed, no warning; the result does not show a clash
with the view's other dimensions. Again in `…-064352`: a dimension's `textAtMm` at y 292 on an
A3 sheet (297 high) put the text over the border's zone strip, `textAtMm` echoed, no warning.
Again in `…-144220` (point 2): a section's `modelBoxMm` is the whole model's box, not what
the view shows. Cut through a tray's side ears looking along +X at 1:2, `boundsMm`
horizontal 72.68–107.32, `modelBoxMm` 67.24–101.38 (the full 68.28 mm extent), while the
PDF draws the section from 78.6 to 101.35: one edge right, the other 11.4 sheet mm off. The
section's checks passed. A caller placing text or a view from `modelBoxMm`, as
`drw_views_standard`'s description suggests for plain views, lands beside the section.
Local, either: the cut geometry's box for a section, or a word in the description that a
section keeps the model's box. That point is fixed in `ff6eaec` on `sw2023` (the owner,
2026-11-11: the box, not the word): for a section of a part `modelBoxMm` is the box of
what the view draws, from its visible edges (a line by its ends, a full circle by its
projected radius, other edges by points along them), taken when it lies inside the
model's box; a section of an assembly keeps the model's box, and `drw_info` says both.
New live test (a block with a taller end cut in the middle, both ways round: 60 high one
way, 100 the other) fails without it. In build `20261111-140208`; to check in its
scenario test. Held there (findings 2026-11-11, point 4): the same section through a
tray's side holes read 78.624–101.376 against the PDF's 78.6–101.35 (was 67.24–101.38),
in `drw_info` of a saved drawing and in a new `drw_view_section` result; a centreline
section kept the full height. The item's other points (axes, the border and title block,
labels, dimension and note extents) stay open. Met again in that test: two `diameter`
dimensions on the same hole edge without `textAtMm` were both placed at the same sheet
point, the second a duplicate, with no warning for either.

**S24. `drw_annotation_edit` delete moves other dimensions, unreported.** In the scenario
test of build `…-024431`: three horizontal dimensions stacked below a view, text at y 109,
101 and 93 mm. After the one at 109 was deleted, the `before` of the next two deletes gave
both at y 109: SOLIDWORKS had closed up the stack. The result says only that the count
went down by one and the name is no longer listed; in a drawing that keeps them, the
layout changes without a word. Bears on S3 (Closed), whose tool it is. Local: read the
view's other annotations' positions before and after a delete and name each that moved.
Decided 2026-11-11 by the owner: name each that moved, before and after; do not put them
back (a caller moves one with `drw_annotation_edit` if it wants).

**S30. `model_mass` and `material_set` name a material the configuration does not have.**
In the scenario test of build `20261111-140208` (point 1): with a material in rows A and
C only, `model_mass` in `Default` gave `"material":"AISI 1020"` beside a density of 1000,
and a following `material_set` with no `configuration` reported `"previous":"AISI 1020"`,
both `ok`; the table, which reads by the configuration's name, gave none for `Default`.
Both read `GetMaterialPropertyName2("")`. Likely the material that reached `Default`
while row A was set (S29's cause) and was given back by name still answers to `""`; not
probed. Local: read by the active configuration's name in both, and probe whether S29's
putting back leaves something behind under `""`. Probed 2026-11-11 on `sw2023` at
`fa434e5`, live: a block, `config_table` rows A (`AISI 1020`), B, C, `material_set` 6061
in C with `Default` active. Read by each configuration's name, `Default` has no material
until it is activated again after A, B and C; from then on `GetMaterialPropertyName2`
by its own name, by `""` and `MaterialIdName` all give `AISI 1020`, after a rebuild too,
while its mass reads 1000 kg/m³ and its body names no material. So reading by the
active configuration's name does not fix it: SOLIDWORKS keeps a name it does not apply.
A test that fails on it (`model_mass` names `AISI 1020` in `Default`) and the reads by
name in six services were written and set aside, not committed. Waits on the owner: what the tools
report when the name and the density disagree (e.g. the name with a warning, or no
material), and whether to look for where the name comes from (S29's putting back, or
`Default`'s activation).

**S32. Two small result follow-ups from the scenario test of build `20261111-140208`.**
(a) From S22 (point 5): the results of `drw_views_standard` and `drw_view_section` still
carry the model's full path in each view, which `drw_info` cut. (b) From S25 (point 6): a
pick one diameter off a hole's centre rightly got "used the nearest one, 2.25 mm away",
but 2.25 is also the radius and it read as the centre case, and an `edge_list` call went
to finding the real centre; when the edge taken is a circle, that warning can name its
centre and radius. Local, small. Not yet fixed.

**S33. No equations linked to a file.** From reading the source (findings 2026-11-11,
first entry, point 1): `equation_set`, `equation_list` and `equation_delete` act on one
document, and nothing reaches `IEquationMgr.FilePath` or `LinkToFile`. A design of several
part files that share key sizes holds each as its own global variable per part: a change
is one `equation_set` per part, and one made by hand in one part does not reach the others.
`asm_part_new` with a face (T9.22 partly, T9.23 not built) and `part_derive` `insert`
(solids, surfaces, material) do not cover it. Local: link, unlink and read a document's
external equation file, `equation_list` naming each variable's source; general, fit for a
PR. Parked 2026-11-11 by the owner: taken up when a design of several parts needs it.

**S34. Reading and checking a model costs a caller listings and judgment.** From reading
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

**S36. Tools no session had run.** From the tool reference (findings 2026-11-11, first
entry, point 6): `equation_set`, `feature_rename`, `model_review`, `model_compare`,
`asm_component_list` and `asm_mate_list` on an assembly built by hand, `asm_part_new`,
`asm_replace`, `doc_activate` with `allowEdit`. The scenario test covers a part, a STEP
round trip, a two-part assembly and a drawing. Run since on build `20261111-140208`
(that day's second entry, points 1, 5, 6), all held: `equation_set` (ten globals,
thirteen dimensions linked, among them an offset plane's offset, a pattern's spacings and
a placement sketch's dimensions; the volume after a change of all ten against a hand
figure, 173487.816 mm³ against 173487.8), `feature_rename` on a feature and a sketch, and
`model_review` with and without findings. Proposed: a scenario item that opens an assembly
built by hand, reads its components and mates and changes one dimension; one that sets a
global variable, renames a feature and reads `model_review`. Usage: the scenario test,
the owner's call. Decided 2026-11-11 by the owner: both scenario items, written into the
scenario test where it is kept, not here. Closes when they are there.

**S46. A release is registered in one project, the server in two.** From reading a
session's call count (findings 2026-11-11, fourth entry): `CLAUDE.md`'s release step 4
registers the new exe "(scope local, in the owner's project)". The server is registered
at local scope in two of the owner's projects, each with its own exe path in
`~/.claude.json`; builds up to `…-064352` went to both, the five after it only to the one
where the scenario test runs. A session in the other project ran two parts and an
assembly on `…-064352`, without `sketch_constrain` (S8), ~150 of ~330 calls single
relations and dimensions. `sw_status` reported the old build rightly; nothing compared it
with the latest release. Bears on S15 and S16. Local, in this repo's release steps:
step 4 reads every project's `mcpServers.solidworks-mcp.command` from `~/.claude.json`,
names each on an older build in the question to the owner, and registers the new exe in
each. The owner's call. Taken up outside this repo the same day: a session-start hook of
the owner's re-registers a project's `solidworks-mcp` on the newest `build-` tag, run by
hand once (it moved the stale project to `20261111-140208`), not yet run at a session
start. With it, step 4 may only need to say that the hook does the other projects.
Decided 2026-11-11 by the owner: a line in step 4, to register the new exe by hand in
every project `~/.claude.json` lists with `solidworks-mcp`, until that hook has held at
a session start.

## Waiting

**S1. `drw_view_section` / `drw_view_detail` drew in view-sketch coordinates.** Local:
`1de5067` + `3f43c50` on `sw2023`, run and held 2026-11-09 in build `…-163234`. Upstream:
PR #1 (branch `fix/section-sheet-coords`, one commit `81475a5`), tested on 2023 SP3 only;
on 2025 the mapping may be wrong if sheet coordinates work there. Closes when PR #1 is
merged or refused; if merged, the local commits drop out at the next rebase. Held again
in builds `…-195750` and `…-204719` (two sections, cut where drawn). Not yet run:
`drw_view_detail`, views at an angle. Not yet checked: `drw_view_edit broken_out` passes its circle in sheet
coordinates straight into the active view (its comment says sketch coordinates are the
sheet's there), which S1 found false for sections and details; its live test only counts
the cut, so a circle landing elsewhere on the part would pass.

**S2. A drawing always showed a sketch open, so `doc_undo` and `doc_rebuild` refused it.**
Reported as a section leaving the view sketch open. Cause, found live on 2023 SP3: in a
drawing `ISketchManager.ActiveSketch` is never null. It names the sheet's own sketch from
the new drawing on, and the active view's own sketch once a view is activated (after a
section, detail or broken-out view), and `sw_status`, `model_info`, `doc_undo` and
`doc_rebuild` read it as a sketch in edit: from the first view, not only after a section.
Closing it is not possible (`InsertSketch(true)` keeps it, `ActivateSheet` moves it to the
sheet's). Local: `44fb220` on `sw2023`, `SwExt.SketchInEdit` leaves those two sketches
out; any other active sketch is still reported. Three live tests fail without it (a new
drawing and a section; a detail; a broken-out view). Drawing, FeatureManagement, M0 and
Contract live tests 83/86 on 2023 SP3; the three that fail are not this fix's: M0's
revision check expects 2025 (`33.`), and auto dimensions and center marks inserted
nothing, also without the fix, in a SOLIDWORKS session open since 18:05
(`drw_dimensions_auto`'s hint says a long session can stop inserting model annotations).
Held in the scenario test of build `…-204719`: no open sketch in a new drawing, after a
standard view, after each of two sections and in a saved drawing opened read-only;
`doc_rebuild` runs; in a part a real open sketch is still reported and refused. Undoing
drawing work still does not work, for other reasons: S19. Upstream: branch
`fix/drawing-sketch` (one commit `3bde32f`, cherry-picked onto `upstream/main`), PR #7.
Closes when PR #7 is settled.

**S19. `doc_undo` did not work in drawings, and removed work while reporting failure.**
Found while fixing S2, live on 2023 SP3: after `drw_views_standard`, `drw_view_projected`
and `drw_note_add`, `doc_undo` refused `NOT_OWNED` ("no change by this server is on
record"), since a drawing's update stamp hardly moves with drawing changes, and the text
blamed the user. After `drw_view_section` there is a step on record, and `EditUndo2` left
the section view (`VERIFY_FAILED: Nothing changed`). In the scenario test of build
`…-204719`, with a section on record, `drw_note_add` then `doc_undo` answered
`VERIFY_FAILED: Nothing changed` while the note was gone (notes 65 → 64), twice. Decided by
the owner on 2026-11-09: refuse it in drawings. Local: `a7cd981` on `sw2023`, `NOT_SUPPORTED`
before `EditUndo2`, with a text that says why and a hint to undo in SOLIDWORKS (Ctrl+Z) and
read the drawing back with `drw_info`; the tool description says so. New live test (a
section, a note, then `doc_undo`: refused, views and notes as before) fails without it with
that `VERIFY_FAILED`. Drawing, FeatureManagement and Contract live tests on 2023 SP3: 94,
all passing but S23's, which fails without the fix too. Upstream: branch
`fix/drawing-undo` (one commit `b4cc3e4`, cherry-picked onto `upstream/main`), PR #8. In
build `…-215810` (tag `build-20261109-215810`). Held in its scenario test: in a drawing,
refused `NOT_SUPPORTED` after a view and a note and again after a section and a second
note, with the views and every note kept (`drw_info`); in a part, `doc_undo` still took
back a boss extrude. Closes when PR #8 is settled. Tracking drawing changes by view and
annotation counts stays a later option. A misplaced drawing item is moved or removed with
S3's tools (Waiting) instead of an undo. Held again in build `…-230924`: refused in a
drawing, the hint naming the three edit tools; in a part an undo took back a boss
extrude. In a part again in `…-014952`, `…-024431`, `…-034022`, `…-064352`, `…-091645` and `20261111-140208`.

**S6. `sketch_dimension_add` angular refused the acute angle at a shared vertex.** Local:
`18cbc57` on `sw2023`. The supplement is kept when `textAt` lies in its sector, and the value
set after it is checked against the supplement. New live test fails without the fix;
`SketchEditLiveTests` 58/58 on 2023 SP3. Held in the scenario test of build `…-181555`.
Upstream: PR #6 (branch `fix/angular-supplement`, `dce4afe`, "Fixes #3"). Second part, the
scenario entry's point 2: without `textAt`, a value that lands in the angle across the
vertex was refused with a hint to look for other relations. Still refused and rolled back,
now saying so, with a hint that gives a `textAt` inside the other angle. Local: `f87a3df`
on `sw2023`; on `fix/angular-supplement` as `ec2c32b`, pushed to PR #6. New live test
fails without it; `SketchEditLiveTests` 59/59 on 2023 SP3. Held in the scenario test of
build `…-195750`, in a real sketch and in the bare two-line repro (one extra call over a
sketch made with `textAt` from the start). Left open from that test, local and small, for
PR #6 too: the hint and the accepted call's check name the acute angle by its current
size (`To set the 7.5° angle` when 10° was asked), which reads like a value to use; say
"the angle that is now 7.5°". Done in `f1d5fb8` on `sw2023`, the hint and the check, each
asserted in the live test of the hint; in build `20261111-140208`. Not yet on
`fix/angular-supplement`: a push there joins PR #6, the owner's call. Held in the scenario
test of build `20261111-140208` (findings 2026-11-11, point 2): the hint reads "To set the
angle that is now 7.500423°", and its `textAt` was accepted. One wording left from it,
small: the accepted call's check says "the angle that is now 7.500423°, not on the one
between the rays, now 172.499577°" in a result where they are 10° and 170°; "was" fits a
result (the hint, given before any change, keeps "now"). Closes when that wording is in
PR #6 and it is merged or refused. Decided 2026-11-11 by the owner: fix that wording on
`sw2023`, then push it with `f1d5fb8`'s change to `fix/angular-supplement`, joining PR #6.
Fixed in `48d111f` on `sw2023`: the accepted call's check says "the angle that was
7.500423°, not on the one between the rays, then 172.499577°"; the hint keeps "now". The
live test of the hint asserts it; the 33 angle tests passed. Both commits cherry-picked
clean onto `fix/angular-supplement` (`aa59f21`, `434d9e8`, in its worktree), not pushed
yet: the push joins PR #6.

**S20. `drw_dimension_add` returned some dimensions without a value check, silently.** In the
scenario test of build `…-204719`, 9 of 23: every one with a circular edge in it (a line to
a hole edge, two hole edges, `vertical` or `horizontal`). Their results carried no
`expected` and no "matches the geometry" check, and no warning, while the tool says each
value is verified or removed. Bears on S4. Local: `8c88427` on `sw2023`. A circle among the
two entities is measured as the dimension does: at its centre, or face-on at its near or
far side, from the arc end conditions read back from the dimension (two circles: the other
order if the first does not fit). What still cannot be compared (an assembly view, a
curve, a single circle with `linear`) comes with a warning "not compared with the
geometry"; the tool description says both. New live test (six dimensions to hole edges:
a line to a hole, two holes, `horizontal`, `vertical`, `linear`) fails without it
(`horizontal: expected none, wanted 50`). Drawing live tests 44/44 and unit tests 248/248
on 2023 SP3. In build `…-024431` (tag `build-20261110-024431`). Held in its scenario
test (findings 2026-11-10, build `…-024431`, points 1 and 3): the nine dimensions of build
`…-204719`'s test added again with the same calls, each with `expected` equal to `value`
and a check naming how the circle was measured (`('b' to its centre)`, `('a' to its
centre, 'b' to its centre)`); an edge-on circle in a section view, `vertical` checked
against its centre, `horizontal` (SOLIDWORKS made 2 mm against the expected 57) refused
`VERIFY_FAILED` and removed. Still not run: a near or far side (no tool sets one).
Follow-ups from it, small, before the PR (the owner, 2026-11-10): (a) a single circle with
`linear` became a diameter in SOLIDWORKS (Ø4.5) and came back `kind: linear`, no
`expected`, with a warning that reads garbled ("the expected linear between the circle
alone is not computed here"); the circle's diameter is known, so it can be compared as
`diameter` is. (b) The `horizontal` refusal's hint says to pass `textAtMm` or use
`horizontal` / `vertical`, which the call already did; it should say that SOLIDWORKS did
not make that direction to an edge-on circle. Fixed in `8b0fbbd` on `sw2023`: (a) a
distance on a single circle that SOLIDWORKS makes a diameter or a radius
(`IDisplayDimension.Type2`) is compared with the circle's diameter or radius, `kind` says
what was made and a check says so (`SOLIDWORKS made a diameter dimension of the circle for
kind linear`); the warning for a value still not compared reads "no expected linear value
is computed for a circle seen edge-on alone in this view". (b) A `horizontal` or
`vertical` that does not match, with a circle seen edge-on among the two, has the hint
"SOLIDWORKS did not make a horizontal dimension to the centre of …, a circle seen edge-on
in this view", pointing to a vertex, a straight edge or a face-on view; one with no such
circle no longer says to use horizontal / vertical either. Two new live tests fail without
it (`kind linear`; the old hint); Drawing and Contract live tests 51/53 on 2023 SP3, the two
that fail are auto dimensions and center marks inserting nothing, also without the fix (a
long SOLIDWORKS session, as under S2); unit tests 248/248. In build `…-034022` (tag
`build-20261110-034022`). Held in its scenario test (findings 2026-11-10, build
`…-034022`, points 1 and 2): (a) `linear` on a single hole edge seen face-on came back
`kind: diameter`, `expected` equal to `value`, with the check that SOLIDWORKS made a
diameter, and the PDF shows a diameter; (b) in a section view, `horizontal` from a vertex
to a hole edge seen edge-on refused `VERIFY_FAILED` (2 mm made), removed, with the new
hint; `vertical` with the same two entities and a `horizontal` from a straight edge to a
face-on hole edge still checked. Not run: the hint for a mismatch with no edge-on circle
(no case found that SOLIDWORKS gets wrong), a near or far side. Follow-up from it, small
(point 3): (c) `linear` on a single circle seen edge-on comes back `kind: linear`, no
`expected`, with the new warning, while SOLIDWORKS made a linear diameter dimension (Ø in
the PDF, the value the circle's diameter); the diameter is known, so it can be compared as
in (a). Reported success, value right. Upstream: PR when (a) and (b) have held in a
scenario test, which they now have; (c) goes in first (the owner, 2026-11-10), then the PR
from `upstream/main` with `8c88427`, `8b0fbbd` and (c)'s fix. (c) fixed in `23bd0d7` on
`sw2023`. Probed live first (a block with the Ø20 hole, a section through its centre, the
hole's back edge alone): `linear` and `horizontal` give `Type2` linear (not diameter), the
prefix `<MOD-DIAM>`, value 20; `vertical` is not made (`FEATURE_FAILED`). A single circle
seen exactly edge-on (its axis in the sheet plane) is now compared with its projection
(`linear` the diameter, `horizontal` / `vertical` the diameter times the projected
direction's share), and a check says the dimension carries the Ø (`SOLIDWORKS made a linear
dimension with Ø across the circle, seen edge-on`). `kind` stays what SOLIDWORKS made
(`linear` / `horizontal`), since `Type2` reads linear; the Ø is in the check. A circle seen
at an angle keeps the warning, which now says "a circle seen at an angle". New live test
fails without it (`linear: expected none, wanted 20`); Drawing and Contract live tests 54/54
on 2023 SP3, unit tests 248/248. Not run: a section whose cut leaves the hole's axis
oblique on the sheet (`horizontal` there would be checked against the projected share). In
build `…-064352` (tag `build-20261110-064352`). Held in its scenario test (findings
2026-11-10, build `…-064352`, points 1 and 2): (c) `linear` and `horizontal` on a hole edge
seen edge-on and drawn horizontally, alone, each `expected` equal to `value` (the diameter),
with the Ø check and no warning; in a second section rotated so the edge is drawn
vertically, `vertical` on it alone checked too; the PDF prints all three as Ø with the
diameter. (a) and (b) held again. Still not run: an oblique edge-on hole, a near or far
side. Follow-up from it, small, not in the PR (point 3): (d) `vertical` on an edge-on circle
drawn horizontally is refused `FEATURE_FAILED` with the generic hint "Check that the
entities can carry this kind of dimension (… a circle for radial/diameter)", which reads as
if it should have worked; the server knows the circle is seen edge-on and its drawn
direction, so the hint can say so and point to `linear` or the other direction. Nothing
made, the failure reported honestly. (d) fixed in `0b21139` on `sw2023`: the hint says the
circle is seen edge-on, drawn as a segment of its diameter with no extent that way, and
to use `linear` or the other direction; asserted in the edge-on live test, which fails
without it. In build `20261111-140208`. Held in its scenario test (findings 2026-11-11,
point 7): `FEATURE_FAILED` with the new hint; (c) held again. Upstream: branch `fix/dimension-circles` (one commit
`5a54b9f`, `8c88427`, `8b0fbbd` and `23bd0d7` squashed onto `upstream/main`), PR #10; Drawing and Contract live tests 46/46 on it on 2023 SP3 (the
interop patch on top, uncommitted). Closes when PR #10 is settled.

**S3. No tool deleted or moved a drawing view, dimension or note.** Upstream ROADMAP T10.4
(🟡). Local: `cc685a9` on `sw2023`. `drw_annotation_list` (current sheet or one view: name,
kind, view, text or value, position, attachment), `drw_annotation_edit` (move, delete, a
note's text; each read back, a move or text that does not read back is put back and
refused), `drw_view_edit` kind `move` (aligned views follow and are named; a projected view
moved off its parent's alignment is put back and refused) and `drw_view_delete` (the views
that went, and the children that stay: a projected view stays when its parent goes). And
`ddca400`: `doc_undo`'s refusal in drawings points to these tools. Four new live tests;
Drawing and Contract live tests on 2023 SP3: 49 of 49. In build `…-230924` (tag
`build-20261109-230924`). Held in its scenario test (findings 2026-11-09, build
`…-230924`, point 1) on a finished drawing: list per view, a dimension and the sheet note
moved, a dimension deleted, the parent moved with both sections following, a section moved
off its alignment refused and put back, a section deleted, the PDF showing every change.
Follow-ups from that test: (a) action `text` wrote a typed `\n` as two characters and
reported `text read back` (`drw_note_add` the same); (b) `drw_view_delete` of a parent took
its section view too, against the description, and `annotations` counted the named view's
only; (c) the sheet's list held the sheet format's notes (64 of 65: border zones, title
block), unmarked, and an edit moved a zone letter; (d) a section view's label was listed
as a note with text " "; (e) S22's echo. Fixed in `79c7b4b`: a typed `\n` is a line
break in both tools; `drw_view_delete` refuses a view holding a section, detail or
auxiliary view's cut unless `withChildren`, and sums the counts of every view that went;
the format's notes (`IAnnotation.OwnerType` 2) are left out unless `format=true`, marked,
and refused by the edit; `view_label` is a kind; texts over 80 characters come back cut in
edit results. Each has an assertion that fails without it; Drawing and Contract live tests
on 2023 SP3: 50 of 50. In build `…-235702` (tag `build-20261109-235702`). Held in its
scenario test (findings 2026-11-10, point 1): all five. Small follow-ups from it: (f) `drw_note_add`'s result gives the input text, not the text read back (a
typed `\n` shown as typed while the note holds a line break), and in full (S22); (g) a view
label's `text` is a placeholder, not its wording; (h) `drw_info` counts the sheet format's
notes among the sheet's (65 against the list's 1). Fixed in `c5df767`: the text read back,
cut over 80 characters; the label's words from `INote.GetTextAtIndex` ("SECTION A-A");
the template's annotations left out of the count. Drawing and Contract live tests: 50 of
50. In build `…-20261110-010708` (tag `build-20261110-010708`). Held in its scenario test
(findings 2026-11-10, build `…-010708`, point 1): all three; a typed `\n` came back as a
line break, a 134-character note as its first 60 characters and the count; the labels
read `SECTION A-A` and `SECTION B-B`; the sheet's `notes` equal to the list's (1, then 3).
Follow-up from it, small: (i) `drw_info` still counted a view's label among that view's
notes (`notes: 1` for each section view, the list with `kind: note` 0). Fixed in
`59a15f4`: a label is left out of `notes` and counted apart as `viewLabels` (present
when a view has one), in `drw_info` and in `drw_view_delete`'s summed counts. The label
live test reads the section view's counts (notes 0, viewLabels 1) and fails without it;
Drawing and Contract live tests on 2023 SP3: 50 of 50. In build `…-20261110-014952` (tag
`build-20261110-014952`). Held in its scenario test (findings 2026-11-10, build `…-014952`,
point 1): each section view `notes: 0, viewLabels: 1`, the plan view and the sheet with no
`viewLabels`, equal to the list; after a note added, `notes: 1, viewLabels: 1`; and
`drw_view_delete` with `withChildren` summed `viewLabels: 2` with the other counts. Closed
2026-11-10 on the owner's word: every follow-up, (a) to (i), ran and held, and nothing was
sent upstream. Back to Waiting the same day, when it was sent (below).
Usage: moving a parent moves every view aligned with it, a section placed beside it too
(point 7). Upstream: branch `feat/drawing-edit` (one commit `d406f45`, `cc685a9`,
`79c7b4b`, `c5df767` and `59a15f4` squashed onto `upstream/main`, its live tests moved to
the end of the class so it merges clean with PR #8), PR #9;
Drawing and Contract live tests 47/47 on it on 2023 SP3. `ddca400` stays local, as it
stands on PR #8. Editing a dimension's value (T10.4) stays out: a drawing
dimension's value follows the model. Closes when PR #9 is settled.

**S37. `equation_set` took one equation per call and said nothing of the body.** In a
machined part on build `20261111-140208` (findings 2026-11-11, second entry, points 2, 3).
(a) 43 of 83 calls were `equation_set`: 10 globals created, 13 dimensions linked, 20 to
change the ten and set them back, each call a rebuild. (b) Its result carried no
`volumeDeltaMm3` or `bodies`, unlike `dimension_set`'s, which refuses an equation-driven
dimension: in a model driven by global variables S7's usage line could not be followed
from the result, and a `model_mass` after each change was needed. Bears on S7. Decided
2026-11-11 by the owner: both. Fixed in `c26b997` on `sw2023`. (a) `equations:[{lhs,
expression}]` in place of `lhs` / `expression`, up to 100: every row checked before the
first change (a name another row creates counts as known, a loop inside the batch is
refused), written so that a variable comes before the equations that use it, one rebuild,
each row verified as a single call verifies it, and every row put back when one fails.
(b) `volumeDeltaMm3` and `model {volumeMm3, bodies, box}` for a part, for one equation
and for a batch. Found while testing, both fixed in it: (c) a rollback left a dimension
that got its first equation in the call at the new value (deleting the equation keeps
it); it is set back, and the volume compared with the one before. (d) An equation changed
in place to one SOLIDWORKS won't take (a slot's depth of 0) was dropped without an error:
the old text stayed and the call came back `ok` with a warning "SOLIDWORKS stored the
equation as" the old one, a result reported `ok` that was wrong, in single calls too. It
is refused `VERIFY_FAILED` ("kept the previous equation") and rolled back. A new equation
of that kind was already refused by SOLIDWORKS (`FEATURE_FAILED`); the rows written before
it are now taken back. Two new live tests (a slot block: three equations in one call with
the slot's row before the variable it uses, the volume against the slot's own, a slot
through the block reading `bodies: 2`; a batch refused at the arguments, at a new
equation of 0 and at a changed one, the equations and the volume as before each time);
the second fails without (d), no exception thrown. With the two older equation tests 4/4
on 2023 SP3, Contract 7/7, unit tests 259/259. To check in the scenario test: a part of
global variables set up in one call, its call count against the 83, and `bodies` after a
change that splits it. Upstream: a PR not yet decided.

**S42. `sketch_dimension_add` angular could solve to the mirrored shape and report the angle matched.**
In two printed parts on build `…-064352` (findings 2026-11-11, third entry, point 4): two
lines meeting at a vertex 130° apart, 50° asked, `textAt` inside the wedge: one line went
to the mirrored solution, the result read `value reads back 50°, geometry measures 50°`,
only the `changed` list showed it, and the sketch went `fully` defined in the wrong shape.
Bears on S6, whose fix keeps the supplement by `textAt` and checks the value, not the
shape. Workaround: delete and redraw the line, dimension the angle the rays really make
first, the distances after. Usage (point 5): `sketch_relation_add` `merge` of a free
line's end onto a point held by relations moves the free line whole, and a second merge
pulled the shape back into the mirrored solution; redraw at the held points' exact
coordinates instead of merging onto them. Decided 2026-11-11 by the owner: a warning, not
a refusal, since it can be what the caller meant. Fixed in `ef2eddd` on `sw2023`, in
`sketch_dimension_add`, `sketch_relation_add` and each step of `sketch_constrain`
(the warning starts "Step n:"). Found live first: the solver does not turn a line round
here. With one line held horizontal and the other fixed the angle is refused as
over-defining; with freedom left, a line turns by the 80° between 130° and 50°, which is
the mirrored shape and no reversal. So three signs, one warning each: (a) an angular
dimension set to within 0.5° of the angle the lines already made across the vertex, while
it sits on the one between the rays: the warning names both angles, the lines that
turned and a `textAt` inside the other angle; (b) a line turned by more than 90°, or an
arc that bends the other way, after a dimension or a relation; (c) for a linear
dimension, a segment's end moved more than twice what the value changed by, over 1 % of
the sketch's extent. Three new live tests, each asserting a warning the tools did not
give: the 130° wedge by the single tool and as step 2 of `sketch_constrain`; a line
turned round by a horizontal dimension and by a `coincident` relation; a point on a line
rising 80° moved 28.8 mm by 5 mm of horizontal distance; and no warning for a dimension
that only moves what it measures. `SketchEditLiveTests` 66/66 on 2023 SP3. (c) also fires
for a point that slides along a line steeper than 60° to the dimension, where the move
is right: a warning the owner may find noise. To check in the scenario test: the wedge
of point 4 warns, and a fully defined sketch of the test part gives no warning.

**S41. `feature_pattern_linear` `direction2` ignored its sign, reported `ok`.** In two
printed parts on build `…-064352` (findings 2026-11-11, third entry, point 2): a 2 x 2
pattern, the second direction ran toward +z whether `direction2` was `-z` or `+z`; `ok`
with two warnings (`4/6 instance faces`, `2 of 6 instance faces were not found`) and a
volume change of one instance. The misplaced instances cut air, so nothing failed, and a
mirror of that pattern then cut two holes where none belonged. Workaround: a pattern in
one direction, then `feature_mirror`. (b) Point 3: with `directionRef`, `direction:"-y"`
was refused (`not parallel to edge`) with a hint to give the sign as `"+"/"-"`, and `"+"`
was then refused (`'+' is not an axis`). Fixed in `689a2fa` on `sw2023`. (a) Cause:
instances that cut air leave no faces, so both senses of direction 2 matched the same
faces and the pattern was not read as reversed. A pattern that leaves a whole direction
without one instance found beyond the seed's row is now taken back and that direction
made the other way; when that fits and is the sense asked, it is kept, with no warning.
When the sense asked is the one that leaves the part, it is made again and kept as
before, with its warning and a check line saying the other sense was tried. A pattern
whose instances merge is not retried: one instance found along a direction proves its
sense. (b) With `directionRef`, `direction` names the side the pattern runs toward along
the reference, so an axis not parallel to it is taken; only one square to it is refused,
with a hint that names what the tool takes. This is more than the hint the item asked
for: the hint could not be made true otherwise, and a caller had no way to turn a slanted
reference round. The same in `asm_pattern`; both descriptions say it. Two new live tests
fail without it (a 3 x 2 grid toward `-z` from z = 20: 2 of 5 instances' volume; a
pattern along a chamfer's slanted edge toward `-x`: `not parallel to edge E2`). The
twelve linear pattern tests of the part and three of the assembly pass on 2023 SP3. To
check in the scenario test: a 2 x 2 pattern whose second direction SOLIDWORKS runs the
wrong way by itself.

**S45. Feature results listed every new face in full.** In two printed parts on build
`…-064352` (findings 2026-11-11, third entry, point 11): `feature_extrude`,
`feature_mirror` and `feature_fillet` listed up to twelve new faces each, 2 to 4k
characters, read for one id or none. Same kind as S22 (Closed), whose fix did not cover
faces. Decided 2026-11-11 by the owner: a short list by default (each face's id and
kind), the full detail by a flag. Fixed in `cbd79a7` on `sw2023`: `newFaces` is
`[{id, surface}]` unless `faceDetail:true`, which gives each face as before (area, pick
point, normal or axis, radius). In the 37 tools whose result carries `newFaces`: the
feature tools, the body and surface tools, `face_edit` and the sheet-metal flanges,
hem and bend; the server guide says so. The services still return every face in full
and the cut is made where the result is written, so nothing a tool checks changed. A
unit test of the two forms and a live test (a boss's three faces as a tool returns them,
its cap's id from the short list taking the next sketch); the short form is the new
behaviour, so neither is a test that fails without a fault. Unit tests 260/260,
Contract 7/7; `tools/list` 242.4 to 246.5 KB for the new parameter. To check in the
scenario test: the characters of a `feature_extrude`, a `feature_mirror` and a
`feature_fillet` result against 2 to 4k, and one call with `faceDetail:true`.

**S40. A blind cut whose far end leaves the body was refused, though SOLIDWORKS made it.**
In two printed parts on build `…-064352` (findings 2026-11-11, third entry, point 1): a
hole through a plate and a pocket opening through a slanted underside, `feature_extrude`
cut, blind, gave `VERIFY_FAILED: ... no floor face at 12 mm along [-1, 0, 0]` and was
rolled back; the cut was right. `through_all` would have gone through other walls. Met
twice. Workaround: `keepOnFailure:true`, the volume read from the next result; on that
path `name` was not applied (`Cut-Extrude2` kept), one `feature_rename` more. Decided
2026-11-11 by the owner: the cut passes on its volume with a check line, and `name` is
applied on the `keepOnFailure` path; `upToFace` / `through next` parked. Fixed in
`c1d3bc5` on `sw2023`. (a) A cut with no floor face at all passes on the volume it
removed, with the check "no floor at 30 mm: the cut broke out of the body before its
end" and the depth dimension's name; a cut with a floor at another depth than asked is
still refused, and so is a boss with no cap. (b) A feature kept by `keepOnFailure` takes
the call's `name`, and the error ends "The result was kept as '<name>' (keepOnFailure)";
in `feature_extrude` only, the other feature tools keep SOLIDWORKS' name on that path.
New live test (a blind cut 30 deep in a 20 mm block, its volume the hole's; a 5 mm
pocket still showing "floor at 5 mm"; a boss inside the body kept as `Inner`) fails
without it (`no floor face at 30 mm along [0, -1, 0]`). With the Modelling live tests
10/10 on 2023 SP3. Not run: a pocket that opens through a slanted face on one side
only, where part of a floor may be left at the asked depth (then it reads as a floor,
with no line). To check in the scenario test: a blind hole deeper than its plate.

**S38. `feature_hole` leaves its placement sketch under-defined, unreported.** In the same
part (point 4): the result names the hole's own sketch dimensions but not the placement
sketch or its status; only `model_review` showed it, as `info`
(`sketch_under_defined`), and a pattern of the hole rests on a point that can move.
Workaround, 5 calls: `sketch_edit`, one `sketch_constrain` (horizontal and vertical from
the origin, `fully`), `sketch_finish`, two `equation_set`. Local: return the placement
sketch's name and constrained status, an option to dimension each point from the origin;
perhaps `model_review` ranks an under-defined sketch as a warning. Decided 2026-11-11 by
the owner: the placement sketch's name and constrained status in the result, and a warning
when it is under-defined; no dimensioning option (`sketch_constrain` does it in one call);
`model_review`'s rank stays. Fixed in `dbd72ed` on `sw2023`: the result's
`placementSketches` gives, per hole, its feature, the sketch that places it and its
`constrained` status, read after the hole is made (of the hole's two sketches, the one
with points and no segment but construction geometry; the other is the profile with
`Hole Dia.`), and a warning names each one not fully defined with the way out
(`sketch_edit`, `sketch_constrain`, `sketch_finish`). The tool's description says so.
A live test (two through holes: two placement sketches, each `under`, each in
`model_review` as `sketch_under_defined`, neither the profile) fails without it
(`placementSketches` null). The hole tests 10/10 and the five other live tests that make
holes (Drawing 4, FeatureManagement 1) passed; unit tests 260/260. To check in the
scenario test: a `feature_hole` result names its placement sketch as `under` with the
warning; after `sketch_constrain` `model_review` no longer lists it.

**S43. `sketch_offset` has no dimension a caller can name, and reads under-defined.** Same
parts (point 6), met in both: the offset's dimension could not be found (`D5@<sketch>`
not found after four dimensions), so no equation can drive it; and a sketch that read
`constrained: fully` while open is listed `sketch_under_defined` by `model_review` once
it holds an offset. Local: return the offset dimension's full name in the result; read
why `model_review` and the sketch's own status disagree. Fixed in `fa434e5` on `sw2023`. The cause: `SketchOffset2` was called with its
last argument, the dimension, off, so the offset was held by its relation alone; the
status read while open did not count that until a rebuild, hence `fully` open and
`under` after. The offset is now made with its dimension (one per side for `both`), and
the result's `dimensions` names each (`D3@<sketch>`), with a check line, and a warning if
SOLIDWORKS adds none. A live test (a fully defined 100 x 60 rectangle offset 5 outward:
`D3 = 5`, still `fully`, not in `model_review` after finish; `D3` set to 8 gives a
116 x 76 loop) fails without it (`dimensions` null); the six offset tests passed, the
both-sides one now reading two dimensions of 5; unit tests 260/260. To check in the
scenario test: an offset's result names its dimension, an `equation_set` on it holds,
and `model_review` does not list the sketch.

**S18. `doc_pack` faults.** (1) From a document opened `readOnly:true`, the copies still
reference the originals (`VERIFY_FAILED: Some copies still reference other files`), three
times; read-only is the likely cause, not yet tested with a writable open. Workaround that
held: close all, remove the originals, reopen the copies. (2) `includeDrawings:true` copies
no drawing when the drawings lie in another folder, and reports success; Pack and Go only
looks next to the model. (3) Missing: new names per file (only prefix or suffix). Local:
refuse or reopen a read-only source, report `noDrawingsFound`, say both in the description;
then PR. (3) not yet decided. Decided 2026-11-11 by the owner: (1) refuse a source opened
read-only, with a hint to open it writable; (2) report `noDrawingsFound`; both in the
description. (3) parked. Fixed in `fc5e4a4` on `sw2023`. (1) Confirmed live: from an
assembly opened read-only the copies were written and `VERIFY_FAILED` said they still
referenced the originals; such a document is now refused before anything is written
(`INVALID_ARGUMENT`, "open read-only", the hint to close and open it writable). (2) With
drawings asked for and none in Pack and Go's list, `noDrawingsFound: true` and a warning
that it looks only next to each model under its name. Both in the description. The pack
live test now does both (on the code before, the read-only pack got `VERIFY_FAILED`);
the two pack tests passed, unit tests 260/260. (3) stays parked, out of this item. To
check in the scenario test: `doc_pack` of a read-only document is refused, and a part
whose drawing lies elsewhere reads `noDrawingsFound`.

**S31. `feature_delete` `dryRun` says kept what the delete removes.** In the scenario test
of build `20261111-140208` (point 3, from S26): a mirror with two Hole Wizard children,
`deleteChildren`, `dryRun:true` listed the holes' four sketches under `keptAbsorbed`; the
real call then removed them, each `absorbed by <hole>`. A child's absorbed sketches go
with the child whatever `deleteAbsorbed` says. Local, small: the plan lists them as
removed with that `why`, so the dry run and the delete agree. Fixed in `40ffab9` on `sw2023`: with `deleteChildren`, a
child's absorbed sketches are planned as removed (`absorbed by <child>`), whatever
`deleteAbsorbed` says; it still decides for the requested features' own sketches. The
live test of a delete with children now runs a dry run first and compares both lists
with the delete's; on the code before, the dry run lacked the hole's two sketches. The
FeatureManagement delete tests passed. To check in the scenario test: a mirror with two
holes, `deleteChildren`, `dryRun` then the delete, the same lists.

**S44. `feature_chamfer` `equal` makes a chamfer with no faces on 2023.** Same parts (point
7): on a concave edge and on a circular edge, `'Chamfer1' was created but has no faces`,
then the tool fell back to distance-distance by itself and held. Twice in two tries.
Reported success, the right chamfer. Local, small: if `swChamferEqualDistance` never works
on 2023 SP3 (a live probe first), go straight to distance-distance and drop the warning,
in the interop patch or behind a version check. Probed 2026-11-11, live: the equal type made a chamfer with no
faces on a convex straight edge, a convex circular edge and a concave circular edge, each
time. Fixed in `2a1f5ae` on `sw2023`, behind a version check: on revision 31 (2023) an
equal chamfer is made as distance-distance with equal legs at once, no failed attempt,
no warning; other versions keep the attempt and its fallback, untried here. The equal
chamfer live test now asserts no warning and adds one on a boss's concave base; on the
code before it carried the warning. Chamfer tests and the two others that make
chamfers passed. To check in the scenario test: an `equal` chamfer's result has no
warning.

**S39. Two small result follow-ups from the same part** (point 7). (a) `sketch_add` of a
centre rectangle returns six segments and `sides` names four: the two construction
diagonals, needed for a midpoint relation to the origin, are not named and were guessed.
(b) `material_list {filter:"6061"}`: 4 rows in 1268 characters, about two thirds the list
of databases, repeated on every call. Same kind as S22 (Closed). Local, small. Fixed on
`sw2023`: (a) in `7742f2c`, a centre rectangle's entity names its two construction
diagonals in `diagonals` (a corner rectangle has none); (b) in `0470a2e`, `material_list`
with a filter lists only the databases it reaches (by name or by a row found), all of
them without one. A live test of each fails without it (no
`diagonals`; 4 of 5 databases holding none of the rows). To check in the scenario test:
a centre rectangle's `diagonals`, and `material_list {filter:"6061"}`'s length.

**S27. A document loaded as a reference shows as `foreign` in `doc_list`.** In the scenario
test of build `…-091645` (point 5): after `doc_open` of a drawing (read-only), the part it
references was listed as a document of its own, `access: foreign`, though nobody opened
it, and `doc_close` of the drawing closed it too (`also closed`). Right in SOLIDWORKS
terms, but `foreign` reads as the user's document, which `doc_close` says it never closes.
Usage: a part listed after opening a drawing or an assembly may be its reference, loaded
with it. Local, small: mark such a document (an `access` value or a `loadedBy`), and say in
the `doc_list` description. Decided 2026-11-11 by the owner: a `loadedBy` field naming the
document that loaded it; `access` keeps its values. Fixed in `e340317` on `sw2023`: `doc_open` marks
each document that came in with the one it opened and that the server does not own;
`doc_list` gives it `loadedBy`, that document's handle; the description says so. The
drawing-close live test now reads `doc_list` first (the part foreign, `loadedBy` the
drawing's handle; on the code before, null); Export and TopDown live tests and the pack
test, which open files, passed. Only `doc_open` marks; a reference loaded otherwise (an
`asm_insert`) has none. To check in the scenario test: `doc_open` of a drawing, then
`doc_list`.

**S35. `model_compare` against an earlier revision of the same file name.** From the
source, not run (findings 2026-11-11, first entry, point 5): `other` is opened with
`OpenDoc6`, and SOLIDWORKS does not open a second document of a name already open, so the
call ends `IoError`, whose hint (compare by handle) cannot help. For an assembly copied
under another name, its components resolve to the open documents of those names, so two
revisions compare the same parts and nothing says so. Local: the description names both,
or the tool copies `other` to a temporary name and warns when an assembly's components
came from memory. To confirm live first. Decided 2026-11-11 by the owner: confirm both
live, then a line in the tool's description naming each case; the copy to a temporary
name parked. Confirmed live 2026-11-11 and fixed in `4e6d556` on `sw2023`. (1) Worse than
read: a full path to the earlier revision was taken for the open document of that file
name, so the call ended "'other' is this document", and with another open document of
that name it would have compared that one, reported `ok`. A full path is now that file;
SOLIDWORKS does not open it, and the `IoError` names the open document in the way and
how to get past it. (2) The copied assembly opened and read 30 mm deep, the open part's
size, not the 20 mm of its own files. Both named in the description. A live test of both
(on the code before, `INVALID_ARGUMENT` for the first). To check in the scenario test:
`model_compare` against a copy of an open part in another folder under its name.

## Closed

Each goes back to Waiting if it is sent upstream, as S3 did.

**S9. `sw_help` at session start reads ≈23.5k characters.** Usage: a short note of the tools
and conventions the sessions here use, read instead. Where the note lives (a skill, the
global `CLAUDE.md`, a memory) is the owner's call. The scenario test of build `…-181555`
followed a recorded call sequence and made no `sw_help` call at all; so did those of
`…-195750`, `…-204719` and `…-215810` (28 calls, 13.2k characters for the whole test), and
`…-230924` (41 calls, 43.2k characters; 44% of them the sheet's note list and three note
edits, S3 and S22), `…-235702` (33 calls, 27.1k) and `…-010708` (28 calls, 16.0k; two
full sheet lists and two `drw_info` calls 42% of them), `…-014952` (28 calls, 16.5k),
`…-024431` (42 calls, 21.2k), `…-034022` (25 calls, 11.6k), `…-064352` (28 calls, 13.2k;
8 dimension adds at ≈110–600 characters each, one `drw_info` of 1.6k to read a section's
orientation), `…-091645` (30 calls, 16.8k; the largest `feature_list` 2.1k and
`feature_mirror` 1.9k), `…-134233` (34 calls, 16.2k; 14 of them on S28) `…-144220`
(28 calls, 13.3k) and `20261111-140208` (60 calls, 25.2k for seven items; one `edge_list`
spent on a wrong pick, S25). On 2026-11-11 a session wrote a caller's manual against the
tool reference (findings, that day's first entry), and one built a machined part by it on
`20261111-140208`: 83 calls, 32.4k, no `sw_help`, 43 of the calls `equation_set` (S37). Closed 2026-11-11 on the owner's word, not taken here: a caller's note belongs with the skill that drives the server, outside this repo.

**S15. `tools\install.ps1` registers at user scope under the name `solidworks`.** That
clashes with any other server of that name. Usage: `-NoRegister`, and register by hand at
local scope. Upstream: perhaps an option for the name and scope; not yet decided. Closed 2026-11-11 on the owner's word, not taken: the server is registered by hand at local scope, and a session-start hook of the owner's keeps each project on the newest build (S46).

**S29. `A_table_makes_three_sizes_each_with_its_own_volume_and_reads_back` fails on 2023
SP3.** A Configuration live test: the table read back has no `$MATERIAL` column
(`Collection: ["D1@Boss-Extrude1", "$STATE@Cut-Extrude1", "$PRP@PartNo"]`, `Not found:
"$MATERIAL"`), though its rows set `AISI 1020`. Seen 2026-11-10 while fixing S28, with and
without that fix (on `4533273` too), in a SOLIDWORKS session open since before the
scenario test of build `…-115800`, whose point 2 met a SOLIDWORKS state that broke
`material_set` until a restart. Not yet looked into: that session's state, a 2023
difference in reading a table's material column, or the test. Looked into 2026-11-11, in a SOLIDWORKS session just started: it still failed, so not
the session's state. Cause, found live: on 2023 SP3 `SetMaterialPropertyName2` with a
configuration's name reaches other configurations too. A material set for the active
configuration went to every configuration (each one's mass shows it once activated), and
one set for another configuration reached the active one. So `config_table` gave a row's
`$MATERIAL` to every configuration and reported it as that row's, a result reported `ok`
that was wrong, and the table read back had no `$MATERIAL` column because the
configurations all read alike; `material_set` with `configuration` did the same. Fixed in
`ba35548` on `sw2023`: with other configurations present the call is made while another
one is active, the others' materials are read before and after, and one that changed is
given back what it had (none included); one that can't be put back is a warning. New
live test (a material set in two of three configurations, each configuration's mass
against its own density) fails without it (`Expected: 0,118`, `Actual: 0,947`); the
table test passes again and also reads no material in the rows that set none.
Configuration live tests 5/5. Not changed: `material_set` with no `configuration`, which
may reach every configuration of a part that has several; not yet looked into. In build
`20261111-140208`; to check in its scenario test: a part with three configurations, a
material in one by `config_table` and in another by `material_set`, the mass of each.
Held there (findings 2026-11-11, point 1): `AISI 1020` in row A only, `6061 Alloy` by
`material_set` in C with `Default` active, each configuration activated and measured:
A 7900 kg/m³, B 1000, C 2700, `Default` 1000, and the table read back the same. A
`material_set` with no `configuration` then changed `Default` only, so the branch not
looked into held on that part. What it found beside it is S30. Closed 2026-11-11 on the owner's word.

**S22. Results that repeat what the caller already has.** `drw_note_add` echoes the whole
note text (1.4k characters back for a 1.4k note); `sw_status` repeats ten `recentOps`
with their full error texts, up to 2.6k characters per call. Found in the scenario test of
build `…-204719`. Local, small: a length and the first line for the note, error codes only
in `recentOps`; then PR. Bears on S8 and S9 (tokens). Met again in build `…-230924`:
`drw_annotation_edit` on the 1.4k note returned its text twice (`before` and `after`), 3.2k
characters per call; that echo cut to 80 characters in `79c7b4b` (S3). `drw_note_add`'s
echo and `sw_status`'s `recentOps` stay; the first cut in `c5df767` (S3 f). Build
`…-235702`'s scenario test: the one `format:true` list 10.5k, 39% of its characters.
`drw_note_add`'s cut held in `…-010708` (S3 f); `drw_annotation_list` still gives a
note's text in full (a sheet list 1.6k to 2.0k, mostly the 1.4k note); again in `…-014952`,
`kind: note` 1.6k for the one note. Also there: `drw_info` repeats the model's full path in
every view, 1.6k for three views. Fixed in `ff3494f` on `sw2023`: `recentOps` keeps a failed call's error code, the whole
text only for a call its caller gave up on (a timeout, which nobody read);
`drw_annotation_list` cuts a text over 80 characters to its start and its length, as the
edit does, unless `fullText=true`; `drw_info` gives a model's full path in the first view
that shows it and its file name in the views after. Live tests: a failed `material_set`
read back from `sw_status` as its code, failing without the fix; the views' `model`,
failing without it; the list's cut asserted in the annotations test, not run without the
fix (it takes the new parameter). In build `20261111-140208`; to check in its scenario
test: the characters of `sw_status` after a failed call, of a sheet list with the long
note, and of `drw_info` with three views. Held there (findings 2026-11-11, point 5):
`sw_status` 1.26k for ten `recentOps`, the failure as its code only; a sheet's note list
with a 1405-character note 232 characters; `drw_info` with three views 1.44k (was 1.6k).
Follow-up from it, small: (a) the results of `drw_views_standard` and `drw_view_section`
still carry the model's full path in each view, which `drw_info` cut. Closed 2026-11-11 on the owner's word; follow-up (a)
is S32.

**S25. A point at a hole's centre resolves to the hole's edge with a bare distance.** In the
scenario test of build `…-064352` (point 5): a dimension's entity given at a hole's centre
resolved to the hole's edge with "no edge exactly at …; used the nearest one, 2.25 mm away"
(the radius), and `strict: true` refused it `ENTITY_NOT_FOUND`. Both right, since the centre
is not on the edge; but a session that thinks in hole centres reads the warning as a near
miss. Local, small: when the point is a circular edge's centre, the warning says so
(`used the circle centred there`). Usage until then: pick a hole's edge on the circle. Then
PR with other resolver work; not yet decided. Fixed in `0b21139` on `sw2023`: when the point is the centre of the circular edge taken,
the warning reads "… is the centre of a circular edge (radius r mm), not a point on it;
used the circle centred there"; a point off the edge that is not its centre keeps the old
warning, and `strict` is as it was. New live test (a diameter picked at a 2 mm hole's
centre, and 0.5 mm off it) fails without it. In build `20261111-140208`; to check in its
scenario test. Held there (findings 2026-11-11, point 6): a diameter picked at a 4.5 mm
hole's centre gave the new warning. Follow-up from it, small: (a) a pick one diameter off
the centre, on the circle's far side, rightly got the old warning, "used the nearest one,
2.25 mm away", but 2.25 is also the radius and it read as the centre case; an `edge_list`
call was spent finding the real centre. When the edge taken is a circle, the old warning
can name its centre and radius too. Upstream: not yet decided. Closed 2026-11-11 on the owner's word; follow-up (a) is S32.

**S26. `feature_delete` with `deleteChildren` warns once per absorbed sketch of a child.** In
the scenario test of build `…-091645` (point 4): deleting a mirror with two hole features
under it, `removed` listed the mirror, the two holes and their four sketches, each with a
`why` ("absorbed by <hole>"), and `warnings` repeated each sketch, "SOLIDWORKS also removed
'SketchN' (absorbed by <hole>), which was not planned". Four warnings on a delete that did
what was asked: a child's absorbed sketches go with the child whatever `deleteAbsorbed`
says. Same kind as S17. Local, small: a sketch absorbed by a feature that is itself removed
is planned, listed with its `why` and not warned; a sketch removed that nothing removed
absorbed still warns. Then PR with other noise fixes; not yet decided. Fixed in `0bcb0ea` on `sw2023`: a sketch absorbed by a removed feature is listed with its
`why` and not warned about, unless that feature is one the call named and was to keep its
sketches (`deleteAbsorbed` false). New live test (a block with a Hole Wizard hole, the
block deleted with its children: no warning, the hole's sketch `absorbed by` it) fails
without it. In build `20261111-140208`; to check in its scenario test. Held there
(findings 2026-11-11, point 3): a mirror deleted with its two Hole Wizard children and
their four sketches, each `absorbed by <hole>`, no warning. Follow-up from it: (a) the
same call with `dryRun:true` listed those four sketches under `keptAbsorbed`, and the real
call then removed them: a dry run that says kept what will go. The plan should list a
child's absorbed sketches as removed whatever `deleteAbsorbed` says, as the delete does.
Upstream: not yet decided. Closed 2026-11-11 on the owner's word; follow-up (a) is S31.

**S23. `Tolerances_read_back_and_fits_follow_the_ISO_tables` fails on 2023 SP3.** A
FeatureManagement live test: `Expected: 0,021`, `Actual: 0`. Seen 2026-11-09 while fixing
S19, with and without that fix, in a SOLIDWORKS session open since 18:05. Not yet looked
into: a long session, a 2023 difference in reading tolerances or fits, or the test. Looked into 2026-11-11: it passed in a SOLIDWORKS session started minutes before, alone
and in the FeatureManagement live tests in full (89 of 89 with SketchEdit). Its failures
came from SOLIDWORKS sessions open for hours, not from the server or the test. Closed
2026-11-11 on the owner's word.

**S28. `material_add` kept its own library; a library that did not parse failed
`INTERNAL`.** In the scenario test of build `…-115800` (point 2): `material_set` of a
material from this server's library failed `VERIFY_FAILED`, and during the call SOLIDWORKS
put its own XML header in front of the server's UTF-16 file, so the library no longer
parsed. `material_list` only warned; `material_add {…, overwrite:true}` then failed
`INTERNAL: XmlException` with nothing on what to do, and `material_set` hinted at the
name. After a SOLIDWORKS restart the same call passed and the library was left alone: the
rewrite came from that SOLIDWORKS session's state, not the build. Decided 2026-11-10 by the
owner: `material_add` writes into SOLIDWORKS' own custom library, `Custom Materials.sldmat`
in the custom material folder, as its default, in place of a library of its own.
Local: `bbc98e6` on `sw2023`. (a) The default database is `Custom Materials`. The marker
moved from the file to each material's `description`, which SOLIDWORKS keeps: a material
the server did not write is refused `NOT_OWNED` whatever the case of its name, `overwrite`
replaces only its own, and a library the server created (`solidworks-mcp.sldmat`) still
owns all its materials. A library it did not create is copied to
`<file>.before-solidworks-mcp` before its first write (result `backup`); the file keeps its
layout (tabs, CRLF), and a check reads its other materials back unchanged. (b) A library
that does not parse is `IO_ERROR` naming the file and the parser's line, that SOLIDWORKS
can rewrite a library it has loaded, and to move it aside or restore it (the backup named
when there is one): in `material_add`, and in `material_set` for that database, for a name
not found while a library does not parse, and when SOLIDWORKS rewrote the library while
failing to apply a material from it. Live tests on 2023 SP3, each failing without the fix:
`A_custom_material_is_listed_assigned_and_used_by_simulation_with_its_own_stiffness` (now
into `Custom Materials`: its own materials listed unchanged, one of them refused with
`overwrite`, `material_set` from it and the simulation; the file restored byte for byte)
and `A_material_library_that_does_not_parse_is_a_named_io_error_in_material_add_and_material_set`
(S28's corrupted header). The rewrite during `material_set` has no live test: it could not be
brought about. Usage until the build: add the material to `Custom Materials` by hand
(Material editor) and `material_set {database:"Custom Materials"}`. Upstream: not yet
decided. In build `20261110-134233` (tag `build-20261110-134233`, `PropertiesLiveTests`
16/16 on it). Held in its scenario test (findings 2026-11-10, build `…-134233`, point 1):
into `Custom Materials` with the backup byte for byte, `material_set` from it, `NOT_OWNED`
for SOLIDWORKS' material in lower case, `overwrite` only for its own, and `IO_ERROR` in
`material_add`, `material_set` and a name found nowhere for the library SOLIDWORKS had
corrupted, the file not written. Not met: the rewrite during `material_set`, still untested.
Two follow-ups from that test, fixed in `9ab2fbe` on `sw2023`: (a) the library written back
kept tabs and CRLF but not its bytes ("UTF-16" became "utf-16", " />" in the user's own
materials, point 2); now only the added material's lines differ, live test
`A_library_as_SOLIDWORKS_writes_it_keeps_its_bytes_but_for_the_added_material`. (b) A name
found nowhere while a library did not parse lost the closest names (point 3); they are
listed again. Each fails without the fix; `PropertiesLiveTests` 17/17 and the offline
tests 21/21 on it. On the test machine the owner's `Custom Materials.sldmat` was given
SOLIDWORKS' spelling back (proven equal to its backup without the server's PETG), and the
old `solidworks-mcp.sldmat`, whose PETG equals the one in `Custom Materials`, moved aside
to `solidworks-mcp.sldmat.retired` (point 4), so `material_set {name:"PETG"}` needs no
`database`. Seen once while testing: the custom material's cantilever bent 0.4035 mm
against 0.4013 ± 0.002 and passed on a rerun. Closed 2026-11-10 on the owner's word; the
follow-ups in build `20261110-144220` (tag `build-20261110-144220`). Held in its scenario
test (findings 2026-11-10, build `…-144220`, point 1): `material_set {name:"PETG"}` with no
`database` passed, the library untouched; a `material_add` into `Custom Materials` changed
only the 11 added lines (BOM, `UTF-16`, `"/>"`, CRLF and the last line end as before); a
typo beside a corrupted library gave `IO_ERROR` with the closest names first, and a name in
a library that parses passed with the warning. The rewrite during `material_set` was not
met there either.

**S8. One relation or dimension per call.** A fully defined sketch took ≈15 calls. Asked:
relations and dimensions in `sketch_add` or a batch tool, each still verified. Upstream has
no item for it; PR when built. Baseline to compare a fix against: findings 2026-11-09,
"drawing and setup lessons", point 6 (build `…-163234`, ≈53 calls and ≈35k characters for
the test part's geometry). In build `…-181555`, without a fix: 47 calls, 25.8k. In build
`…-195750`: 51 calls, 27.4k (the slope sketch 12, one a deliberate refusal). In build
`…-204719`: a two-section A3 drawing in 50 calls, 30.8k, 16 of them the test's own probes.
In build `…-024431`: a drawing's nine dimensions deleted and added again one at a time, 27
calls (≈290 and ≈590 characters each).
Local: `14c6ff2` on `sw2023`, a batch tool, `sketch_constrain {steps:[…]}`: each step a
relation or a dimension as the two single tools take it, run through their code (split
out of them) with the same checks. Arguments and references are all checked before the
first step, against the sketch as the call found it; a point `{at}` on a segment end is
held by that segment, so later steps find it after the solver moved it or a merge replaced
it. A failing step is undone as the single tools undo it, the steps before it are kept and
the rest not run, and the error names both. Not in `sketch_add`: a step failing there
would have to take the drawn entities back too. Four new live tests (a rectangle fully
defined in one call, a point followed through a merge, a stop at a conflicting step,
everything checked before the first step) fail without it; with the 21 sketch live tests
that call the split code, 25/25 on 2023 SP3. The drawing part (`drw_dimension_add` one at
a time, build `…-024431`) is not in it. To check in the scenario test: the call count and
characters of a fully defined sketch against the baseline above.
In build `…-115800` (`4f4699c`); `SketchEditLiveTests` 63/63 on it. Held in its scenario
test (findings 2026-11-10, build `…-115800`, points 1 and 5): a six-line profile with a
centreline fully defined in one call of 10 steps, 1.2k characters (before: 10 calls, 4.7k);
a five-line cut profile with a 7.5° slope, a point `{at}` on a segment end and the angle in
the supplementary sector, 8 steps in one call, 1.2k (before: 8 calls, 3.3k); volumes equal
the earlier builds' to the last digit. The part's geometry and design-intent steps took 33
calls and 21.1k characters, against 49 and 26.9k in build `…-195750` and ≈53 and ≈35k in
the baseline. Closed 2026-11-10 on the owner's word. Upstream: a PR not yet decided.
Met again 2026-11-11 (findings, third entry, point 8) by a session left on build
`…-064352`, before the fix: ~150 of ~330 calls single relations and dimensions. Cause:
S46. Asked there too: horizontal / vertical inferred for lines drawn so in `sketch_add`;
not taken for now, `sketch_constrain` covers the call count.

**S16. The build a session runs could not be told from inside it.** `sw_status` reported
`serverVersion` `0.1.0` only. A session open across a rebuild keeps the old process while
the registration already shows the new exe, and a test result got pinned to the wrong
build. Met in the tests of `…-195750` and `…-204719`, whose builds were read from the
registration. The stamp part needs no code: the release rule in `CLAUDE.md` (2026-11-09)
publishes with `-p:Version=<v>-sw2023-<stamp>`, and it held in the scenario test of build
`…-215810`, the first so built. The commit part: the SDK already stamped it after `+` in
`InformationalVersion`, and `serverVersion` cut it off. Local: `e75c465` on `sw2023`,
`BuildVersion` keeps it shortened to 7 (`0.1.0-sw2023-<stamp>+<commit>`), for `sw_status`
and the MCP server info, and `Directory.Build.props` appends `.dirty` when `src` or the
props differ from that commit. A live test fails without it (`"0.1.0"`); unit tests for
the format. In build `…-091645` (its `serverVersion` `…-091645+76edd54`). Held in its
scenario test (findings 2026-11-10, build `…-091645`, point 1): `serverVersion`
`0.1.0-sw2023-20261110-091645+76edd54` from the first call, the commit the one tag
`build-20261110-091645` points at, no `.dirty`. Closed 2026-11-10 on the owner's word.
Upstream: perhaps the commit part, without the stamp; not yet decided.

**S17. Warnings and texts that were noise**, each met in every scenario test from build
`…-145700` on, on a result that was right. (a) `feature_mirror` warned "instances overlap,
merge or are clipped" on a correct mirror whose base face merges into the body (area ratio
1.05). (b) Every `doc_open` warned that system option `Double 2` (`swSTLDeviation`)
changed, a different value each time, STEP imports too (S5). Cause, found live on 2023 SP3:
the deviation SOLIDWORKS reads back follows the size of the active model, so opening a
3 m part while a 100 mm one is active "changes" it with nothing written. (c) `doc_close`
listed the documents it also closed with blank names (`also closed: , .`): their titles
were read after `CloseDoc`, from documents already gone. Local: `10771bb` on `sw2023`.
(a) with every seed face found reflected, the area difference is a check line, not a
warning; a mirror with a face missing still warns. (b) `doc_open` and imports leave the
coupled STL group out of their audit; `model_export`, which writes it, still checks it.
(c) the titles are read before the close, `(untitled)` for one with none. Three live
tests, each failing without its fix: a boss against a rib (seed faces 1600 mm², the copy's
1200), a 3 m part opened while a 100 mm one is active, a drawing opened from its file and a
new one closed. Export, Mirror, M0 and the close tests 31/32 on 2023 SP3; the one that
fails is M0's revision check, which expects 2025 (`33.`), as under S2. In build
`…-091645`, after the four areas' live tests in full, 119/122 (the revision check, and
auto dimensions and center marks inserting nothing, all three as under S2); held in its
scenario test (findings 2026-11-10, build `…-091645`, point 2): (a) a mirror made again
about the front plane, 9/9 faces reflected, the area ratio 1.052 a check line, no
warning; (b) no `doc_open` warned `Double 2`, an assembly, a drawing, a ~230 mm part and a
STEP import opened, the last two while a 10 mm part was active; (c) `doc_close` of an
assembly and of a drawing listed the parts also closed by name. Closed 2026-11-10 on the
owner's word. Upstream: (b) and (c) are general; a PR not yet decided. (b) met again
2026-11-11 (findings, third entry, point 10) on build `…-064352`, before the fix (S46).

**S21. A document opened `readOnly:true` became dirty without an edit**: an assembly after
only `asm_interference`, `model_info` and `view_screenshot`; a drawing after
`doc_rebuild`. `doc_close` then refused `DIRTY` and needed `discard:true`, one extra call
each. Found in the scenario test of build `…-204719`, met again in every one after it.
Decided 2026-11-10 by the owner: `doc_close` discards it. Local: `3e02192` on `sw2023`. A
document this server opened read-only closes without `save` or `discard`, its changes
discarded, the result's message saying so (`discardedChanges: true`); the user's
documents are still never discarded, and one opened for editing still needs one of them.
The `doc_close` description says so. A live test fails without it (`has unsaved
changes`). In build `…-091645`. Held in its scenario test (findings 2026-11-10, build
`…-091645`, point 3): an assembly opened read-only and made dirty by `asm_interference`, `model_info`
and `view_screenshot`, and a drawing by `doc_rebuild`, each closed with no flags,
`discardedChanges: true`, their file dates unchanged; a part opened for editing and changed
still got `DIRTY`. Closed 2026-11-10 on the owner's word. Upstream: not yet decided.

## Carried, not items

- **SW 2023 interop patch** (`27e8c25`): `swCustomInfoEquation` case removed,
  `InsertUntrimSurface` instead of `InsertUntrimSurface2`. Not sent upstream (upstream
  supports 2025 and later; ROADMAP T2.3 is theirs). Rebased with `sw2023` each update.
