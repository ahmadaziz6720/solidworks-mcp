# Open Items

What sessions found in this fork's server and what was decided about each, S-numbered.
The raw entries are in [`findings.md`](findings.md); this file holds the items. Each item
names where it goes: **upstream** (an issue or PR at `soheil-mp/solidworks-mcp`), **local**
(a fix on `sw2023`, sent upstream as a PR when it is general), **usage** (a note for the
sessions that use the server), or **not taken**.

Three sections: **Open**, not yet fixed; **Waiting**, fixed and not yet settled (a PR not
merged, a fix not yet run in a session); **Closed**, merged upstream, run and held, or
decided against, with the record.

**Open: S3 to S5, S7 to S22.** Next: S19, decided (refuse `doc_undo` in drawings), since
it can remove work while reporting failure. Then, for working drawings and tokens: S3, S20,
then S8. S5 and S7 wait a while for upstream's answer to issues #2 and #4. S16 is small and
makes every later test result traceable to a commit.

---

## Open

**S3. No tool deletes or moves a drawing view, dimension or note.** Upstream ROADMAP T10.4
(🟡). Local: `drw_annotation_list` / move / delete and a view delete, with read-back checks,
then PR. With S19, every placement error means rebuilding the drawing.

**S4. `drw_dimension_add` keeps wrong values** (supplement reported as a match; an angle for
`vertical`). Upstream: issue #5.

**S5. `doc_open` STEP imports surfaces with 3D Interconnect forced off, reported `ok`.**
Upstream: issue #2. Usage until fixed: import by File > Open and `doc_activate`, or check
`solidBodies` after `doc_open`. Still so in build `…-204719`: a STEP round trip gave 16
surface bodies and no solid.

**S7. `dimension_set` accepts a change that splits the body.** Upstream: issue #4. Usage:
read `bodies` in the result after each `dimension_set`.

**S8. One relation or dimension per call.** A fully defined sketch takes ≈15 calls. Local:
relations and dimensions in `sketch_add` or a batch tool, each still verified. Upstream has
no item for it; PR when built. Baseline to compare a fix against: findings 2026-11-09,
"drawing and setup lessons", point 6 (build `…-163234`, ≈53 calls and ≈35k characters for
the test part's geometry). In build `…-181555`, without a fix: 47 calls, 25.8k. In build
`…-195750`: 51 calls, 27.4k (the slope sketch 12, one a deliberate refusal). In build
`…-204719`: a two-section A3 drawing in 50 calls, 30.8k, 16 of them the test's own probes.

**S9. `sw_help` at session start reads ≈23.5k characters.** Usage: a short note of the tools
and conventions the sessions here use, read instead. Where the note lives (a skill, the
global `CLAUDE.md`, a memory) is the owner's call. The scenario test of build `…-181555`
followed a recorded call sequence and made no `sw_help` call at all; so did those of
`…-195750` and `…-204719`.

**S10. Drawing format and title block.** Precision and decimal separator of drawing
dimensions, parentheses for reference dimensions, `n×` in hole callouts, DWG NO from a
property, material and weight, section label text and place, unaligning a view, rotating
a projected view (a side view projected from a top view came rotated 90°). Upstream
ROADMAP T10.3, T10.10, T10.12 partly. Not yet decided which to build. Usage for hole
callouts until then: model holes as one feature plus a pattern, end condition through, so
the callout gets `n×` and "through".

**S11. No dimension to a section's cut face.** `drw_dimension_add` takes model entities only.
Not yet decided.

**S12. Section orientation is not in the tool description.** Arrow side and view direction
(findings 2026-11-09, point 12). Upstream: proposed in PR #1's text; a description change
when PR #1 is settled.

**S13. `drw_dimensions_auto` gives dimensions that cannot be used, reported as success.**
On a part with fully defined sketches: 25 dimensions, construction ones among them,
duplicates, overlapping texts, some off the sheet, none for features whose sketches had
none. With S3 open they cannot be removed, so the drawing was restarted. Usage: manual
`drw_dimension_add` only. Not yet decided whether to fix, or to say in the description
that the result needs cleaning.

**S14. A drawing view's axes and the sheet's drawable area are not reported.** Which model
axis points where on the sheet (in a top view model −Z points up) decides the side for an
angular's `textAtMm`, guessed wrong it gives the supplement (S4). The format's border and
title block are not reported either: a note at x = 18 mm overwrote the zone letters of the
ISO A3 border at ≈19 mm. Local: model-to-sheet axes in the `drw_view_add` / `drw_info`
result, border and title block rectangles in `drw_info`; then PR.

**S15. `tools\install.ps1` registers at user scope under the name `solidworks`.** That
clashes with any other server of that name. Usage: `-NoRegister`, and register by hand at
local scope. Upstream: perhaps an option for the name and scope; not yet decided.

**S16. The build a session runs cannot be told from inside it.** `sw_status` reports
`serverVersion` `0.1.0` only. A session open across a rebuild keeps the old process while
the registration already shows the new exe, and a test result gets pinned to the wrong
build. Local: the build stamp and commit in `serverVersion` (e.g.
`0.1.0-20261109-181555+18cbc57`, `-dirty` from an uncommitted tree), from
`InformationalVersion` / `SourceRevisionId`; the stamp part may stay local, the commit
part could go upstream. Met again in the tests of `…-195750` and `…-204719`, whose builds
were read from the registration.

**S17. Warnings and texts that are noise.** `feature_mirror` warns "instances overlap,
merge or are clipped" on a correct mirror whose base face merges into the body (area ratio
1.05; seen in builds `…-145700`, `…-181555` and `…-195750`). Every `doc_open` of a native
document warns that system option `Double 2` changed, a different value each time (S5 has
the same warning on STEP imports). `Double 2` is `swSTLDeviation`, coupled with the STL
quality; SOLIDWORKS derives it from the opened model's size, and `doc_open` does not write
it. Proposal: leave the coupled STL group out of the after-call comparison unless the call
itself wrote it. `doc_close` lists the components it also closed with blank names
(`also closed: , .`). Local, small; then PR. Both warnings met again in build `…-204719`,
`Double 2` on every `doc_open` (drawing, part, assembly, STEP import).

**S18. `doc_pack` faults.** (1) From a document opened `readOnly:true`, the copies still
reference the originals (`VERIFY_FAILED: Some copies still reference other files`), three
times; read-only is the likely cause, not yet tested with a writable open. Workaround that
held: close all, remove the originals, reopen the copies. (2) `includeDrawings:true` copies
no drawing when the drawings lie in another folder, and reports success; Pack and Go only
looks next to the model. (3) Missing: new names per file (only prefix or suffix). Local:
refuse or reopen a read-only source, report `noDrawingsFound`, say both in the description;
then PR. (3) not yet decided.

**S19. `doc_undo` does not work in drawings.** Found while fixing S2, live on 2023 SP3 with
the S2 fix in: after `drw_views_standard`, `drw_view_projected` and `drw_note_add`,
`doc_undo` refuses `NOT_OWNED` ("no change by this server is on record"), since a drawing's
update stamp hardly moves with drawing changes (one step for the standard views, none for a
projected view or a note), so the server's record of its own writes stays empty; the text
then blames the user. After `drw_view_section` there is a step on record, and `EditUndo2`
changes nothing: the section view stays (`VERIFY_FAILED: Nothing changed`). A raw
`EditUndo2` does take back a standard view and a projected view. Worse, in the scenario
test of build `…-204719`: with a section on record, `drw_note_add` then `doc_undo` answered
`VERIFY_FAILED: Nothing changed` while the note was gone from the sheet (notes 65 → 64),
twice; a session that trusts the error goes on without the note. The hint names
`feature_list`, which says nothing about drawing annotations. Decided by the owner on
2026-11-09: refuse `doc_undo` in drawings (`NOT_SUPPORTED`, a text that says why and what
to do instead) before calling `EditUndo2`, until an undo there can be verified; then PR.
Tracking drawing changes by view and annotation counts stays a later option. With S3 open,
a misplaced drawing item means rebuilding the drawing.

**S20. `drw_dimension_add` returns some dimensions without a value check, silently.** In the
scenario test of build `…-204719`, 9 of 23: every one with a circular edge in it (a line to
a hole edge, two hole edges, `vertical` or `horizontal`). Their results carry no
`expected` and no "matches the geometry" check, and no warning that the value was not
compared, while the tool says each value is verified or removed. The other 14 carry the
check. The 9 values were right by the design; a wrong pick would be kept the same way.
Bears on S4. Local: compare those too, or warn that the value was not checked; then PR.

**S21. A document opened `readOnly:true` becomes dirty without an edit.** An assembly after
only `asm_interference`, `model_info` and `view_screenshot`; a drawing after
`doc_rebuild`. `doc_close` then refuses `DIRTY` and needs `discard:true`, one extra call
each. Found in the scenario test of build `…-204719`. Not yet decided: let `doc_close`
discard a read-only document's changes, or say in the result which call dirtied it.

**S22. Results that repeat what the caller already has.** `drw_note_add` echoes the whole
note text (1.4k characters back for a 1.4k note); `sw_status` repeats ten `recentOps`
with their full error texts, up to 2.6k characters per call. Found in the scenario test of
build `…-204719`. Local, small: a length and the first line for the note, error codes only
in `recentOps`; then PR. Bears on S8 and S9 (tokens).

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
`fix/drawing-sketch` (one commit `3bde32f`, cherry-picked onto `upstream/main`), pushed;
the compare link was handed to the owner, PR not yet opened. Closes when the PR is
settled.

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
"the angle that is now 7.5°". Closes when that wording is in and PR #6 is merged or
refused.

## Closed

None yet.

## Carried, not items

- **SW 2023 interop patch** (`27e8c25`): `swCustomInfoEquation` case removed,
  `InsertUntrimSurface` instead of `InsertUntrimSurface2`. Not sent upstream (upstream
  supports 2025 and later; ROADMAP T2.3 is theirs). Rebased with `sw2023` each update.
