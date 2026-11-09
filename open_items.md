# Open Items

What sessions found in this fork's server and what was decided about each, S-numbered.
The raw entries are in [`findings.md`](findings.md); this file holds the items. Each item
names where it goes: **upstream** (an issue or PR at `soheil-mp/solidworks-mcp`), **local**
(a fix on `sw2023`, sent upstream as a PR when it is general), **usage** (a note for the
sessions that use the server), or **not taken**.

Three sections: **Open**, not yet fixed; **Waiting**, fixed and not yet settled (a PR not
merged, a fix not yet run in a session); **Closed**, merged upstream, run and held, or
decided against, with the record.

**Open: S2 to S5, S7 to S18.** Highest value to build locally, for working drawings and
tokens: S2 and S3 first, then S8. S5 and S7 wait a while for upstream's answer to issues #2
and #4. S16 is small and makes every later test result traceable to a commit.

---

## Open

**S2. A section leaves the view sketch open, so undo is blocked.** After `drw_view_section`
`sw_status` shows the sketch open, `doc_undo` refuses (`SKETCH_STATE`), `sketch_finish`
refuses drawings. Local: close the view sketch at the end of the call (and of
`drw_view_detail`), then PR. Mentioned in PR #1's text, not fixed there. Met again in build
`…-181555`, where `doc_undo`'s hint also says the user is editing the sketch: nobody was.
The fix should leave no sketch open; the hint, if it stays, should name the tool that left
it.

**S3. No tool deletes or moves a drawing view, dimension or note.** Upstream ROADMAP T10.4
(🟡). Local: `drw_annotation_list` / move / delete and a view delete, with read-back checks,
then PR. With S2, every placement error means rebuilding the drawing.

**S4. `drw_dimension_add` keeps wrong values** (supplement reported as a match; an angle for
`vertical`). Upstream: issue #5.

**S5. `doc_open` STEP imports surfaces with 3D Interconnect forced off, reported `ok`.**
Upstream: issue #2. Usage until fixed: import by File > Open and `doc_activate`, or check
`solidBodies` after `doc_open`.

**S7. `dimension_set` accepts a change that splits the body.** Upstream: issue #4. Usage:
read `bodies` in the result after each `dimension_set`.

**S8. One relation or dimension per call.** A fully defined sketch takes ≈15 calls. Local:
relations and dimensions in `sketch_add` or a batch tool, each still verified. Upstream has
no item for it; PR when built. Baseline to compare a fix against: findings 2026-11-09,
"drawing and setup lessons", point 6 (build `…-163234`, ≈53 calls and ≈35k characters for
the test part's geometry). In build `…-181555`, without a fix: 47 calls, 25.8k. In build
`…-195750`: 51 calls, 27.4k (the slope sketch 12, one a deliberate refusal).

**S9. `sw_help` at session start reads ≈23.5k characters.** Usage: a short note of the tools
and conventions the sessions here use, read instead. Where the note lives (a skill, the
global `CLAUDE.md`, a memory) is the owner's call. The scenario test of build `…-181555`
followed a recorded call sequence and made no `sw_help` call at all; so did that of
`…-195750`.

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
part could go upstream. Met again in the test of `…-195750`, whose build was read from
the registration.

**S17. Warnings and texts that are noise.** `feature_mirror` warns "instances overlap,
merge or are clipped" on a correct mirror whose base face merges into the body (area ratio
1.05; seen in builds `…-145700`, `…-181555` and `…-195750`). Every `doc_open` of a native
document warns that system option `Double 2` changed, a different value each time (S5 has
the same warning on STEP imports). `Double 2` is `swSTLDeviation`, coupled with the STL
quality; SOLIDWORKS derives it from the opened model's size, and `doc_open` does not write
it. Proposal: leave the coupled STL group out of the after-call comparison unless the call
itself wrote it. `doc_close` lists the components it also closed with blank names
(`also closed: , .`). Local, small; then PR.

**S18. `doc_pack` faults.** (1) From a document opened `readOnly:true`, the copies still
reference the originals (`VERIFY_FAILED: Some copies still reference other files`), three
times; read-only is the likely cause, not yet tested with a writable open. Workaround that
held: close all, remove the originals, reopen the copies. (2) `includeDrawings:true` copies
no drawing when the drawings lie in another folder, and reports success; Pack and Go only
looks next to the model. (3) Missing: new names per file (only prefix or suffix). Local:
refuse or reopen a read-only source, report `noDrawingsFound`, say both in the description;
then PR. (3) not yet decided.

## Waiting

**S1. `drw_view_section` / `drw_view_detail` drew in view-sketch coordinates.** Local:
`1de5067` + `3f43c50` on `sw2023`, run and held 2026-11-09 in build `…-163234`. Upstream:
PR #1 (branch `fix/section-sheet-coords`, one commit `81475a5`), tested on 2023 SP3 only;
on 2025 the mapping may be wrong if sheet coordinates work there. Closes when PR #1 is
merged or refused; if merged, the local commits drop out at the next rebase. Held again
in build `…-195750` (two sections, cut where drawn). Not yet run: `drw_view_detail`, views
at an angle.

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
