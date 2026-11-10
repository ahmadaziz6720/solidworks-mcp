# Open Items

What sessions found in this fork's server and what was decided about each, S-numbered.
The raw entries are in [`findings.md`](findings.md); this file holds the items. Each item
names where it goes: **upstream** (an issue or PR at `soheil-mp/solidworks-mcp`), **local**
(a fix on `sw2023`, sent upstream as a PR when it is general), **usage** (a note for the
sessions that use the server), or **not taken**.

Three sections: **Open**, not yet fixed; **Waiting**, fixed and not yet settled (a PR not
merged, a fix not yet run in a session); **Closed**, merged upstream, run and held, or
decided against, with the record.

**Open: S4, S5, S7, S9 to S15, S18, S22 to S27.** S8 waits for its scenario test. S5 and S7
wait a while for upstream's answer to issues #2 and #4.

---

## Open

**S4. `drw_dimension_add` keeps wrong values** (supplement reported as a match; an angle for
`vertical`). Upstream: issue #5.

**S5. `doc_open` STEP imports surfaces with 3D Interconnect forced off, reported `ok`.**
Upstream: issue #2. Usage until fixed: import by File > Open and `doc_activate`, or check
`solidBodies` after `doc_open`. Still so in builds `…-204719`, `…-215810` and `…-230924`:
a STEP round trip gave 16 surface bodies and no solid. Again in `…-235702`,
`…-010708`, `…-014952`, `…-024431`, `…-034022`, `…-064352` and `…-091645` (export `solids: 1`,
import 16 surface bodies).

**S7. `dimension_set` accepts a change that splits the body.** Upstream: issue #4. Usage:
read `bodies` in the result after each `dimension_set`.

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
orientation) and `…-091645` (30 calls, 16.8k; the largest `feature_list` 2.1k and
`feature_mirror` 1.9k).

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
none. Without a delete tool they could not be removed, so the drawing was restarted; with
S3's `drw_annotation_edit` (Waiting) they can be deleted one by one. Usage: manual
`drw_dimension_add` only. Not yet decided whether to fix, or to say in the description
that the result needs cleaning.

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

**S15. `tools\install.ps1` registers at user scope under the name `solidworks`.** That
clashes with any other server of that name. Usage: `-NoRegister`, and register by hand at
local scope. Upstream: perhaps an option for the name and scope; not yet decided.

**S18. `doc_pack` faults.** (1) From a document opened `readOnly:true`, the copies still
reference the originals (`VERIFY_FAILED: Some copies still reference other files`), three
times; read-only is the likely cause, not yet tested with a writable open. Workaround that
held: close all, remove the originals, reopen the copies. (2) `includeDrawings:true` copies
no drawing when the drawings lie in another folder, and reports success; Pack and Go only
looks next to the model. (3) Missing: new names per file (only prefix or suffix). Local:
refuse or reopen a read-only source, report `noDrawingsFound`, say both in the description;
then PR. (3) not yet decided.

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
every view, 1.6k for three views.

**S23. `Tolerances_read_back_and_fits_follow_the_ISO_tables` fails on 2023 SP3.** A
FeatureManagement live test: `Expected: 0,021`, `Actual: 0`. Seen 2026-11-09 while fixing
S19, with and without that fix, in a SOLIDWORKS session open since 18:05. Not yet looked
into: a long session, a 2023 difference in reading tolerances or fits, or the test.

**S24. `drw_annotation_edit` delete moves other dimensions, unreported.** In the scenario
test of build `…-024431`: three horizontal dimensions stacked below a view, text at y 109,
101 and 93 mm. After the one at 109 was deleted, the `before` of the next two deletes gave
both at y 109: SOLIDWORKS had closed up the stack. The result says only that the count
went down by one and the name is no longer listed; in a drawing that keeps them, the
layout changes without a word. Bears on S3 (Closed), whose tool it is. Local: read the
view's other annotations' positions before and after a delete and name each that moved.
Not yet decided whether to put them back as well.

**S25. A point at a hole's centre resolves to the hole's edge with a bare distance.** In the
scenario test of build `…-064352` (point 5): a dimension's entity given at a hole's centre
resolved to the hole's edge with "no edge exactly at …; used the nearest one, 2.25 mm away"
(the radius), and `strict: true` refused it `ENTITY_NOT_FOUND`. Both right, since the centre
is not on the edge; but a session that thinks in hole centres reads the warning as a near
miss. Local, small: when the point is a circular edge's centre, the warning says so
(`used the circle centred there`). Usage until then: pick a hole's edge on the circle. Then
PR with other resolver work; not yet decided.

**S26. `feature_delete` with `deleteChildren` warns once per absorbed sketch of a child.** In
the scenario test of build `…-091645` (point 4): deleting a mirror with two hole features
under it, `removed` listed the mirror, the two holes and their four sketches, each with a
`why` ("absorbed by <hole>"), and `warnings` repeated each sketch, "SOLIDWORKS also removed
'SketchN' (absorbed by <hole>), which was not planned". Four warnings on a delete that did
what was asked: a child's absorbed sketches go with the child whatever `deleteAbsorbed`
says. Same kind as S17. Local, small: a sketch absorbed by a feature that is itself removed
is planned, listed with its `why` and not warned; a sketch removed that nothing removed
absorbed still warns. Then PR with other noise fixes; not yet decided.

**S27. A document loaded as a reference shows as `foreign` in `doc_list`.** In the scenario
test of build `…-091645` (point 5): after `doc_open` of a drawing (read-only), the part it
references was listed as a document of its own, `access: foreign`, though nobody opened
it, and `doc_close` of the drawing closed it too (`also closed`). Right in SOLIDWORKS
terms, but `foreign` reads as the user's document, which `doc_close` says it never closes.
Usage: a part listed after opening a drawing or an assembly may be its reference, loaded
with it. Local, small: mark such a document (an `access` value or a `loadedBy`), and say in
the `doc_list` description. Not yet decided.

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
extrude. In a part again in `…-014952`, `…-024431`, `…-034022`, `…-064352` and `…-091645`.

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
made, the failure reported honestly. Upstream: branch `fix/dimension-circles` (one commit
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
characters of a fully defined sketch against the baseline above. Upstream: PR when held.

## Closed

Each goes back to Waiting if it is sent upstream, as S3 did.

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
owner's word. Upstream: (b) and (c) are general; a PR not yet decided.

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
