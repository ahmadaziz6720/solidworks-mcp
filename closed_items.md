# Closed Items

The S-items closed: run and held in a session, so ready to use, or decided against, each
with its record, newest first as `open_items.md` kept them; a new one goes at the end. An
item whose PR was merged at `soheil-mp/solidworks-mcp` is marked `merged upstream` in its
status, its local commits dropping out of `sw2023` at the next rebase (none yet). Then
what the fork carries that is not an item, and the runs done. Moved here from
`open_items.md` on 2026-11-15, in the `backlog` plugin's format (its `format.md`).

## Closed

### S2 - A drawing always showed a sketch open, so `doc_undo` and `doc_rebuild` refused it - CLOSED 2026-11-14

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
Closed 2026-11-14 under the new rule (the owner: a PR is optional, Closed means ready to
use): it held in its scenario test. PR #7 stays open; merged, the item moves to
Merged upstream.

**Upstream: PR #7, open.**

### S19 - `doc_undo` did not work in drawings, and removed work while reporting failure - CLOSED 2026-11-14

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
back a boss extrude. Tracking drawing changes by view and
annotation counts stays a later option. A misplaced drawing item is moved or removed with
S3's tools (Closed) instead of an undo. Held again in build `…-230924`: refused in a
drawing, the hint naming the three edit tools; in a part an undo took back a boss
extrude. In a part again in `…-014952`, `…-024431`, `…-034022`, `…-064352`, `…-091645` and `20261111-140208`.
Closed 2026-11-14 under the new rule (the owner: a PR is optional, Closed means ready to
use): it held in its scenario test. PR #8 stays open; merged, the item moves to
Merged upstream.

**Upstream: PR #8, open.**

### S6 - `sketch_dimension_add` angular refused the acute angle at a shared vertex - CLOSED 2026-11-14

Local:
`18cbc57` on `sw2023`. The supplement is kept when `textAt` lies in its sector, and the
value set after it is checked against the supplement. New live test fails without the fix;
`SketchEditLiveTests` 58/58 on 2023 SP3. Held in the scenario test of build `…-181555`.
Upstream: PR #6 (branch `fix/angular-supplement`, `dce4afe`, "Fixes #3"). Second part, the
scenario entry's point 2: without `textAt`, a value that lands in the angle across the
vertex was refused with a hint to look for other relations. Still refused and rolled back,
now saying so, with a hint that gives a `textAt` inside the other angle. Local: `f87a3df`
on `sw2023`; on `fix/angular-supplement` as `ec2c32b`, pushed to PR #6. New live test
fails without it; `SketchEditLiveTests` 59/59 on 2023 SP3. Held in the scenario test of
build `…-195750`, in a real sketch and in the bare two-line repro (one extra call over a
sketch made with `textAt` from the start). Left open from that test, local and small, for
PR #6 too: the hint and the accepted call's check name the acute angle by its current size
(`To set the 7.5° angle` when 10° was asked), which reads like a value to use; say "the
angle that is now 7.5°". Done in `f1d5fb8` on `sw2023`, the hint and the check, each
asserted in the live test of the hint; in build `20261111-140208`. Not yet on
`fix/angular-supplement`: a push there joins PR #6, the owner's call. Held in the scenario
test of build `20261111-140208` (findings 2026-11-11, point 2): the hint reads "To set the
angle that is now 7.500423°", and its `textAt` was accepted. One wording left from it,
small: the accepted call's check says "the angle that is now 7.500423°, not on the one
between the rays, now 172.499577°" in a result where they are 10° and 170°; "was" fits a
result (the hint, given before any change, keeps "now"). Closes when that wording is in PR
#6 and it is merged or refused. Decided 2026-11-11 by the owner: fix that wording on
`sw2023`, then push it with `f1d5fb8`'s change to `fix/angular-supplement`, joining PR #6.
Fixed in `48d111f` on `sw2023`: the accepted call's check says "the angle that was
7.500423°, not on the one between the rays, then 172.499577°"; the hint keeps "now". The
live test of the hint asserts it; the 33 angle tests passed. Both commits cherry-picked
clean onto `fix/angular-supplement` (`aa59f21`, `434d9e8`, in its worktree), not pushed
yet: the push joins PR #6. Pushed 2026-11-12 on the owner's word: `fix/angular-supplement`
at `434d9e8` joins PR #6, after the four angular live tests passed on it on 2023 SP3 (the
interop patch on top, uncommitted); the PR's body has a section on the two commits. In
build `20261112-091908`. Held in its scenario test (findings 2026-11-12, first entry,
point 1): the accepted angle's check reads "the angle that was 7.5°, … then 172.5°".
Closed 2026-11-14 under the new rule (the owner: a PR is optional, Closed means ready to
use): it held in its scenario test. PR #6 stays open; merged, the item moves to
Merged upstream.

**Upstream: PR #6, open.**

**Upstream: issue #3, open.**

### S20 - `drw_dimension_add` returned some dimensions without a value check, silently - CLOSED 2026-11-14

In the
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
interop patch on top, uncommitted). Decided 2026-11-14 by
the owner: (d) joins PR #10. Done 2026-11-14: from `0b21139`, only the `DrawingService`
hunk and the edge-on test's assertion, as `d801b06` on `fix/dimension-circles` (the
`EntityResolver` change and its test, S25's, left out); Drawing and Contract live tests in
full on it on 2023 SP3 (the interop patch on top, uncommitted) 45/46, the one failed
`Auto_dimensions_bring_in_the_sketch_dimensions_once` ("only 0 model dimension(s)
inserted", late in the run, code the PR does not touch) passed alone after SOLIDWORKS was
restarted, as under S2; unit tests 248/248, `--check-catalog` ok. Pushed on the owner's
word, joining #10 (2 commits, mergeable); #10's body gained a section on it, its test
counts and the auto-dimensions note, and lost the "Not in this PR" line on the old hint,
read back equal. Offered
the same day and not chosen: a new PR for S14 with S47 (`8d105c3`, `f71b034`), no longer
waiting on S47.
Closed 2026-11-14 under the new rule (the owner: a PR is optional, Closed means ready to
use): it held in its scenario test. PR #10 stays open; merged, the item moves to
Merged upstream.

**Upstream: PR #10, open.**

### S3 - No tool deleted or moved a drawing view, dimension or note - CLOSED 2026-11-14

Upstream ROADMAP T10.4
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
dimension's value follows the model. Decided 2026-11-14 by
the owner: S24's fix (`479b885`, `othersMoved` on a delete, held in the scenario test of
build `20261112-091908`) joins PR #9, a fault in a tool the PR adds. Done 2026-11-14:
cherry-picked onto `feat/drawing-edit` in a worktree as `2c2a98a`; the source merged clean,
the live test file conflicted (`sw2023` holds S14's tests there) and only the S24 test went
in, placed with the PR's annotation tests; Drawing and Contract live tests in full on it on
2023 SP3 (the interop patch on top, uncommitted) 48/48, unit tests 248/248,
`--check-catalog` ok (164 tools). Pushed on the owner's word, joining #9 (2 commits,
mergeable); #9's body gained a section on it and the test, and lost the "Not in this PR"
line on a stacked row's delete, read back equal.
Closed 2026-11-14 under the new rule (the owner: a PR is optional, Closed means ready to
use): it held in its scenario test. PR #9 stays open; merged, the item moves to
Merged upstream.

**Upstream: PR #9, open.**

### S14 - A drawing view's axes and the sheet's drawable area are not reported - CLOSED 2026-11-14

Which model
axis points where on the sheet (in a top view model −Z points up) decides the side for an
angular's `textAtMm`, guessed wrong it gives the supplement (S4). The format's border and
title block are not reported either: a note at x = 18 mm overwrote the zone letters of the
ISO A3 border at ≈19 mm. Local: model-to-sheet axes in the `drw_view_add` / `drw_info`
result, border and title block rectangles in `drw_info`; then PR. Met again in the
scenario test of build `…-230924`: `drw_view_edit` move reported "no overlaps with other
views" while the moved section view's label sat on the letter of another section line in
the parent view; the check covers view bounds only, not labels, section letters or
dimensions. Again in `…-235702`: `drw_annotation_edit` moved a section label 4.3 mm onto
the other section line's letter, `position read back`, no warning; moving a label is the
easiest way to land one on another view's annotation. Again in `…-010708`: `drw_note_add`
120 mm from the sheet's right edge with a first line ≈180 mm long, `position read back`,
no warning, the line cut off at the page edge in the PDF; the result gives the position,
not the note's extent. Again in `…-034022`: `drw_dimension_add` `horizontal` with
`textAtMm` on the row of an existing dimension from the same edge, its dimension line on
the other's and the two texts side by side, `textAtMm` echoed, no warning; the result does
not show a clash with the view's other dimensions. Again in `…-064352`: a dimension's
`textAtMm` at y 292 on an A3 sheet (297 high) put the text over the border's zone strip,
`textAtMm` echoed, no warning. Again in `…-144220` (point 2): a section's `modelBoxMm` is
the whole model's box, not what the view shows. Cut through a tray's side ears looking
along +X at 1:2, `boundsMm` horizontal 72.68–107.32, `modelBoxMm` 67.24–101.38 (the full
68.28 mm extent), while the PDF draws the section from 78.6 to 101.35: one edge right, the
other 11.4 sheet mm off. The section's checks passed. A caller placing text or a view from
`modelBoxMm`, as `drw_views_standard`'s description suggests for plain views, lands beside
the section. Local, either: the cut geometry's box for a section, or a word in the
description that a section keeps the model's box. That point is fixed in `ff6eaec` on
`sw2023` (the owner, 2026-11-11: the box, not the word): for a section of a part
`modelBoxMm` is the box of what the view draws, from its visible edges (a line by its
ends, a full circle by its projected radius, other edges by points along them), taken when
it lies inside the model's box; a section of an assembly keeps the model's box, and
`drw_info` says both. New live test (a block with a taller end cut in the middle, both
ways round: 60 high one way, 100 the other) fails without it. In build `20261111-140208`;
to check in its scenario test. Held there (findings 2026-11-11, point 4): the same section
through a tray's side holes read 78.624–101.376 against the PDF's 78.6–101.35 (was
67.24–101.38), in `drw_info` of a saved drawing and in a new `drw_view_section` result; a
centreline section kept the full height. The item's other points (axes, the border and
title block, labels, dimension and note extents) stay open. Met again in that test: two
`diameter` dimensions on the same hole edge without `textAtMm` were both placed at the
same sheet point, the second a duplicate, with no warning for either. Fixed in `8d105c3`
on `sw2023` (2026-11-12): `drw_info` and `drw_sheet_setup` give `drawableMm` and
`titleBlockMm` (ISO A3: [20, 10, 410, 287] and [230, 10, 410, 65.25]), from the format's
lines on the sheet's own view (`IView.GetLines4`), else its zone margins; each view gives
`axes` (the model direction to the sheet's right, up and out; a top view's up is -Z);
`drw_note_add` gives `extentMm` (`INote.GetExtent`) and `drw_dimension_add` `textBoxMm`
(estimated from the display data, 0.6 x the height a character); both, and a
`drw_annotation_edit` move, warn when the text runs off the sheet, crosses the border,
lies on the title block or on another annotation, which is named (the same value there: "a
duplicate?"); a new or moved view's check covers the border, the title block and its own
annotations against the other views'. Found live: after a view moves, its label's
`INote.GetExtent` still reads the old place, so the check shifts the boxes read before the
move by the view's move. Four new live tests, each failing without it; `DrawingLiveTests`
54/54 on 2023 SP3 with it, and 56/56 with S24 and S32 (the S24 test after its own setup
was fixed). Not covered: a section line's letters in the parent view (not checked whether
SOLIDWORKS lists them as annotations), and a dimension whose dimension line runs on
another's with the texts side by side (only overlapping texts are caught). In build
`20261112-091908`; a PR upstream after it held. Its scenario test (findings 2026-11-12,
second entry): held for `axes` on all three views (a top view: right +X, up −Z, out +Y),
`extentMm`, `textBoxMm`, and a second `diameter` on the same hole edge warned "overlaps
dimension '…' (the same value: a duplicate?)" with the first one's name (point 2). Failed
for the frame (point 1): on a sheet at 1:2 `drawableMm` and `titleBlockMm` read twice
their values, and the checks built on them were wrong and reported success; that is S47.
The PR upstream waited on S47, which held in the scenario test of build
`20261114-024455` and closed 2026-11-14; the PR is not made yet.
Closed 2026-11-14 under the new rule (the owner: a PR is optional, Closed means ready to
use): it held in its scenario test, with S47. No PR made; one is optional.

### S47 - The sheet's frame grows with the sheet's scale, and the checks on it pass wrong, reported `ok` - CLOSED 2026-11-14

In the scenario test of build `20261112-091908` (findings 2026-11-12,
second entry, point 1), from S14's `8d105c3`: on an ISO A3 sheet at 1:2, `drw_info` gave
`drawableMm` [40, 20, 820, 574] and `titleBlockMm` [460, 20, 820, 130.5], twice the right
[20, 10, 410, 287] and [230, 10, 410, 65.25]. A new drawing at 1:1 read them right, and
`drw_sheet_setup {scale:"1:2"}` on it gave the doubled pair at once, so the factor is the
sheet's scale. Every check built on them went wrong: a note at x 18 was warned of the
border, but against x 40 (one at x 30 would be warned wrongly); a note at [300, 40], on
the real title block, and a dimension with `textAtMm` [150, 292], over the zone strip and
outside the border at 287, each read "clear of the border, the title block and the other
annotations"; a section view whose `boundsMm` reached y 289.01 read "inside the border".
S14's live tests ran on 1:1 sheets. Likely cause, from the source, not probed: `FrameOf`
in `DrawingService` reads the format's lines by `GetLines4` on the sheet's own view, whose
coordinates are that view's and scale with the sheet. Local: the frame in sheet
millimetres at any scale, a live test on a 1:2 sheet that fails now. Bears on S14, whose
PR waits on this. Decided 2026-11-12 by the owner: as proposed, and first, before S48
and S49. Fixed in the run of 2026-11-13. Cause, as read from the source and confirmed
live: `IView.GetLines4` on the sheet's own view gives the format's lines in that view's
units, which are sheet millimetres divided by the sheet's scale (at 1:2 a line 20 mm from
the edge reads 40, at 2:1 it reads 10). Fix: `FrameOf` multiplies the lines' ends by the
sheet's scale (`ISheet.GetProperties2` [2] / [3], as `drw_info` already reads it) before
the border and blocks are found; the zone margins, already in sheet mm, are unchanged.
Every check on the frame (`drw_info`, `drw_sheet_setup`, a view's border and title block
check, a note's, a dimension's and a moved annotation's) reads it through `FrameOf`. New
live test `The_frame_is_in_sheet_mm_at_a_sheet_scale_other_than_1_1` (an ISO A3 sheet at
1:2: `drawableMm`, `titleBlockMm` in `drw_sheet_setup` and `drw_info`, a note at
[300, 40] warned "overlaps the title block", a note at [30, 200] clear of the border; then
2:1, the same frame); it failed on the code before ("Expected [20, 10, 410, 287], Actual
[40, 20, 820, 574]"). After: it passes, and with the S14 frame tests, `Sheet_setup_…`,
`Standard_views_…` (both) and `Sheets_are_added_…`, 9/9 on 2023 SP3; unit tests 276/276;
`check_repo` ok. Local: on `sw2023`, `f71b034`; in build `20261114-024455`.
Upstream: not yet decided.
Held in its scenario test (findings 2026-11-14, point 6): on a 1:2 sheet `drw_info` and
`drw_sheet_setup` gave [20, 10, 410, 287] / [230, 10, 410, 65.25]; a note on the title
block, one across the border, one clear, dimension text at y 292 and a section reaching y
289.01, each warned or clear as it should be. Closed 2026-11-14 on the owner's word.

### S66 - A stale edge id resolves to another edge, reported `ok` - CLOSED 2026-11-14

On build
`20261112-091908` (findings 2026-11-13, "a plate's two ends", point 3): after a fillet
changed the edges, `feature_fillet {edges:[{id:"E25"}]}` with an id from the listing
before it filleted another, short edge, `ok`, -0.666; `edge_list` after named the meant
edges E28 to E31. Local: an id read before a topology change is refused, or the result
says the ids were renumbered. Taken into the run of 2026-11-13, no choice in it: an id
whose entity no longer matches its signature is refused `ENTITY_STALE`, with the hint to
list again. Fixed in the run of 2026-11-13. Cause, probed live: a later
feature that trims or splits an edge gives it a new persistent reference, while the old
one still resolves (status ok) to the trimmed edge or to the fragment that holds the
listed point, so the signature check (kind, curve type, the listed mid-point still on the
edge) passed; the listing after gave the same edge a new id. A notch cut across a 100 mm
edge left its id resolving to the 80 mm fragment, and a fillet on it reported `ok`. Fix:
`EntityRegistry.Resolve` compares an edge's current persistent reference with the one
stored at listing, and a mismatch is `ENTITY_STALE` ("was changed by a later feature
(trimmed, split or rebuilt)", hint: re-run `edge_list`, or reference it by point). An edge
no feature touched keeps its reference and its id. Decided 2026-11-13 by the run, not the
owner: the reference check is for edges only; faces keep the point check, because a face
a later cut or fillet trimmed is still the face a sketch or a mate on it means, and
refusing it would make every sketch after a cut list again; bodies are unchanged. New live
test `An_edge_id_listed_before_a_cut_split_it_is_refused_stale` (the notch, then a fillet
by the old id is refused; then, by ids from that one listing, a fillet on an edge the cut
did not touch, a boss elsewhere, and a fillet on a third edge after both, each removing the
analytic volume: an untouched edge's reference stays byte-equal across unrelated features); it failed on the code before ("Assert.Throws() Failure: No exception was
thrown"). After: it passes, with `Point_on_a_corner_…`, `Queries_with_expect_…` and
`Convex_fillet_…` (4/4), and the two fastener tests that mate by edge ids listed before
components went in (`Screws_with_washers_…`, `A_screw_in_a_blind_hole_…`, 2/2); unit tests 276/276; `check_repo` ok. Local:
on `sw2023`, `657fc23`; in build `20261114-024455`. Upstream: not yet
decided.
Held in its scenario test (findings 2026-11-14, point 6): an edge id from before a notch
refused `ENTITY_STALE`, an untouched one used. Closed 2026-11-14 on the owner's word.

### S53 - `feature_extrude` with a `direction` vector off the sketch normal extrudes along the normal, reported `ok` - CLOSED 2026-11-14

On build `20261112-091908` (findings 2026-11-12, "a printed part's
walls redrawn", point 1): `direction:"0,-0.642788,0.766044"`, 40° off the normal, built
along the normal (`directionModel:[0,0,1]`), checks passed, no warning. `ResolveDirection`
takes a vector only for its sign against the normal; the description ("a model axis like +y
| x,y,z") reads as an extrude along it. Workaround: a plane square to the wanted direction
and the profile converted onto it, 6 calls more. Local: refuse a vector not parallel to the
normal, or extrude along it (a direction reference), and say which in the description.
Decided 2026-11-13 by the owner: extrude along the vector (a direction reference), and the
description says so. Fixed in the run of 2026-11-13. Cause: `ResolveDirection` read any axis or
vector only for its sign against the sketch normal. Fix: a vector not parallel to the
normal (and not in the sketch plane; at most 89° off it) is drawn as a 10 mm line in a new
3D sketch from the sketch origin, and after the insert it is made the extrude's direction
reference (`IExtrudeFeatureData2.SetDirectionReference`, `ModifyDefinition`); the depth
is measured along the vector, as SOLIDWORKS does with a direction reference. The 3D
sketch stays as the feature's parent and shows in `alsoCreated`; a rollback deletes it.
The check: the caps are parallel to the sketch plane, at depth·cos θ from it (reported
"cap at 10 mm" in the depth along the vector), and a point of the profile moved by the
depth along the vector lies on a cap ("profile moved along [...]"), which an extrude along
the normal fails. The `direction` description says a vector or axis is extruded along,
the depth too. A vector parallel to the normal, or a model axis, works as before. New live
test `A_direction_off_the_sketch_normal_extrudes_along_it` (a Ø10 boss on the top plane
10 mm along [0, cos 40°, sin 40°], and a cut into a block along [0, -cos 40°, sin 40°]);
it failed on the code before ("Expected: 0.766…, Actual: 1": `directionModel` along the
normal). After: it passes, with `Box_extrude_…`, `Mid_plane_extrude_…`,
`Cut_from_an_open_sketch_…`, `A_boss_buried_…` and `Concave_fillet_…` (6/6) and the
catalog contract test (1/1, `--check-catalog` ok); unit tests 276/276; `check_repo` ok. The
volume of the slanted boss reads 4e-5 under π·r²·h (SOLIDWORKS' mass properties of the
slanted side); the test allows 0.05 mm³. Decided 2026-11-13 by the run, not the owner: the
direction reference is a 3D sketch line kept in the tree, not a reference axis, because an
axis needs two reference points or planes made first (three features, not one). Local: on
`sw2023`, `59beaae`; in build `20261114-024455`. Upstream: not yet decided.
Held in its scenario test (findings 2026-11-14, point 6): a boss along a vector 40° off
the normal, volume π·r²·h·cos 40°. Closed 2026-11-14 on the owner's word.

### S50 - `asm_mate_add` `distance` puts the component on the other side, reported `ok` - CLOSED 2026-11-14

Met five times in four sessions on build `20261112-091908` (findings 2026-11-12, second
entry, point 1; "a part's body raised", point 6; 2026-11-13, "four small printed parts",
point 1; "a revolved part remade", point 1): a component inserted at X +75 with `distanceMm:
75` to the assembly's parallel plane landed at X -75, another at Z -85.2 for +85.2, others
at Y -55 for +55 and 73 mm and 5.5 mm the wrong way, each `ok`, "planes … mm apart".
Right for the parameters given, but nothing in the result says the side, and a caller
reads the success as the place it meant. The pattern held in one session: with no `flip`,
`a` lands on the minus side of `b`'s normal, with `flip:true` on the plus side. Workaround:
`feature_delete` of the mate and the same call with `flip:true`, two calls each time;
then the side read from the result's `originMm`. Local, proposed by the sessions: the
signed offset of the component's plane along `b`'s normal in the `verified` line, a warning
when the component crossed to the other side of `b` from where it was inserted, or a point
the component should end toward. Taken into the run of 2026-11-13, no choice in it: the
signed offset of the component's plane along `b`'s normal in the `verified` line, and a
warning when the component crossed `b` from the side it was inserted on; the mate itself
is not changed. Fixed in the run of 2026-11-13. Cause: the check measured the distance
between the planes unsigned ("planes 75 mm apart"), so either side passed and nothing said
which. Fix: for a distance mate (limit distance too) the geometry of `a` and `b` is read
before the mate as well, and `AsmMateIntent.Side` adds a `verified` line with the signed
offset of `a`'s plane (or point) along `b`'s plane normal, the normal printed with it ("a
(plane 'Right Plane' of …) at 75 mm along the normal of b (plane 'Right Plane') [-1, 0, 0]";
with `b` a point and `a` a plane, `b`'s offset along `a`'s normal), and a warning when the
sign changed from before the mate: "The distance mate put a (…) on the other side of b (…):
-75 mm along its normal before, 75 mm after. If it should stay on the side it was on,
delete the mate and add it again with flip the other way." The mate is not changed. Seen in
the test: the assembly's Right Plane reads its normal as [-1, 0, 0] here, which is why the
normal is printed with the offset. New live test
`A_distance_mate_says_which_side_the_component_landed_on` (a cube inserted at x +75, its
right plane 75 mm from the assembly's, no flip; another at z +40 to the front plane with
flip: the offset times the printed normal is where each landed, warned exactly when it
crossed; the first did cross, as the sessions met it) and a unit test
`A_distance_mate_s_side_is_the_signed_offset_and_a_crossing_is_warned`. On the code before
the live test failed: no `verified` line with the side (`["Distance1 (distance) created and
solved without errors", "planes 75 mm apart"]`). After: it passes, with
`Plane_mates_coincident_and_distance_…`, `Limit_distance_and_symmetric_mates_…` and
`New_mate_types_without_their_entities_…` (4/4); unit tests 277/277; `--check-catalog` ok
(no description changed); `check_repo` ok. Local: on `sw2023`, `1bc3cbb`; in
build `20261114-024455`. Upstream: not yet decided.
Held in its scenario test (findings 2026-11-14, point 6): the signed side and the crossing
warning. Closed 2026-11-14 on the owner's word.

### S54 - Equations outlive the features whose dimensions they drive, and bind by name - CLOSED 2026-11-14

On
build `20261112-091908`, met in five sessions (findings 2026-11-12, "a feature with
children replaced", point 1; "two parts changed mid-tree", points 3, 4; 2026-11-13, "an
existing part edited", point 2; "a revolved part remade", point 5; "one chamfer over three
edges", point 2). (a) `feature_delete`, with `deleteChildren` and `deleteAbsorbed`, leaves
the equations of the dimensions it removes, `ok` with "'Equations' now has rebuild error 51
(swSketchErrorExtRefFail)"; the dry run does not list them. (b) `equation_delete` takes one
lhs: seven calls, six, eight for one feature. (c) A new feature or sketch given a deleted
one's name takes over its equations: `feature_rename` bound a deleted sketch's equations to
the new sketch's `D1` to `D4`, meant for other sizes, shown only as `driven:true`; a new
chamfer of the same name read `D1 … 9.5, driven:true` while the equation still held 7.57.
Keeping an equation was once what the job wanted, so removal should be a choice. Local:
the dry run and the delete list the equations on the dimensions to go, an option removes
them, `equation_delete` takes a list as `equation_set` does since S37, and a rename or a
new name that picks up existing equations warns and names them. Decided 2026-11-13 by the
owner: the dry run and the delete list the equations on the dimensions that go, with a
warning; `deleteEquations:true` removes them, the default keeps them; `equation_delete`
takes a list; a rename or a new name that picks up existing equations warns and names
them. Fixed in the run of 2026-11-13. Cause: an equation names its
dimension by text ("D1@Boss-Extrude1"); SOLIDWORKS keeps it when the feature goes and binds
it to whatever later carries that name. `feature_delete` saw the equations only as the
rebuild error they left, `equation_delete` took one lhs, and no call compared the names the
equations use before and after it. Fix: (a) `feature_delete` reads the equations whose
left-hand side is a dimension (or the suppression) of a feature that goes, in the dry run
from the plan, in the delete from what was removed, and returns them in `equations`. Kept
by default, with a warning that names them and says they bind by name; `deleteEquations:
true` deletes them after the features, one rebuild, before the rebuild errors are read (so
"'Equations' now has rebuild error 51" goes with them), and returns `equationsDeleted:
true`. Other equations that use those dimensions on their right-hand side are warned about
and kept. (b) `equation_delete` takes `equations`, a list of left-hand sides: all found,
and no global variable deleted that a staying equation uses, before the first goes (a
variable goes together with the equations that use it); one rebuild; checked gone. (c)
Every write call on a part or assembly reads, when it starts, the equations that name a
dimension the document does not have (`EquationBindings.Unbound`, in `SwContext.Bind`), and
at its end warns, naming them and the feature, about those that now find their dimension
(`WarnBound`, in `AfterWrite`): a new feature or sketch given the name, a `feature_rename`,
a dimension added later, an undo. Decided 2026-11-13 by the run, not the owner: (1) the
pick-up check sits in the session's write path, for every call, rather than in
`feature_rename` and each tool that names a feature, because the name comes from
SOLIDWORKS' own numbering, a `name` argument in a dozen tools, a rename or a dimension
added later, and one check at the call's edge covers them all; its cost is one read of the
equations per write and one dimension lookup per name they use, nothing in a document
without equations. (2) Equations that use a deleted feature's dimensions only on their
right-hand side are warned about, not deleted by `deleteEquations`, because they drive
dimensions that stay. (3) A suppression equation (`"Fillet1" = "suppressed"`) is listed and
deleted with its feature, but the pick-up warning reads dimension names only. (4) The list
is `equations` (string[]) beside `lhs`, the name `equation_set` gives its list. On the code
before, the live tests failed: `Feature_delete_lists_the_equations_on_the_dimensions_that_go`
(no warning in the dry run named the depth's equation) and
`A_name_that_picks_up_a_deleted_feature_s_equations_warns` (no warning on an extrude named
after the deleted one); the list of `equation_delete` had no argument to test on that code.
After: those two, in their final form, `Feature_delete_with_deleteEquations_removes_them_and_leaves_no_rebuild_error`,
`Equation_delete_takes_a_list_checked_before_anything_goes` and the offline
`Equations_name_their_features_and_dimensions` pass (5/5); the live tests on the changed
paths, the four `Feature_delete_…`, the two `Rename_…`, `Units_set_switches_…`,
`Undo_after_a_fillet_removes_it`, `Undo_never_goes_beyond_…`, `Dimension_set_changes_…`,
`A_global_variable_drives_…`, `Equation_errors_are_typed_…`, `Several_equations_go_in_one_call_…`
and `A_batch_with_a_row_that_fails_…`, pass (14/14); contract tests 7/7; unit tests
277/277; `--check-catalog` ok; `check_repo` ok. Local: on `sw2023`, `9939f28`; in
build `20261114-024455`. Upstream: not yet decided.
Held in its scenario test (findings 2026-11-14, point 6): (a) to (c), with
`deleteEquations` and a list delete. Closed 2026-11-14 on the owner's word.

### S58 - `equation_set` results that mislead - CLOSED 2026-11-14

On build `20261112-091908`. (a) Three globals
changed, three features failed (`swSketchErrorExtRefFail` on a boss, the cut on it, a
mirror); the result gave `volumeDeltaMm3`, `bodies: 1` and only "The rebuild reported
problems; feature_list shows features with errors", so it read as a success with the boss
missing (findings 2026-11-13, "an existing part edited", point 1). (b) In one SOLIDWORKS
session every equation was refused, `"X" = 60mm` and dimension equations alike, on a new
empty part too, with a hint to check the syntax; after a restart the same 21 went in one
call (findings 2026-11-12, "a second helical part", point 1). Nine calls and a restart.
(c) An equation on a reference plane's offset gives the volume change, not where the plane
went; one on the wrong side of its base passed every check (findings 2026-11-12, "two parts
changed mid-tree", point 5). Local: (a) the warning names each failing feature and its
error; (b) when SOLIDWORKS refuses a plain `"Name" = <number><unit>`, say the syntax is
valid and suggest a restart, or tell the two apart on a scratch equation; (c) a driven
plane's or axis's frame in the result. Taken into the run of 2026-11-13 as proposed, no
choice in it; (b) by its message, a probe equation only if it is cheap. Fixed in the run of 2026-11-13. Probe first (b):
on SOLIDWORKS 2023 SP3, a global named `sin`, `cos`, `tan`, `sec`, `cosec`, `cotan`, `arcsin`,
`arccos`, `atn`, `arcsec`, `exp`, `log`, `sqr`, `int`, `sgn`, `abs` or `pi` (any case) is refused
by `IEquationMgr.Add2`; `arccosec`, `arccotan`, `iif`, `if`, `max`, `min`, `mod`, `round`, `sqrt`,
`ln`, `e`, `mm`, `in`, names with a space, a dot, a hyphen or a leading digit are taken. So a
refused plain global is proof of the refusing state only when its name is none of those.
Cause: (a) `equation_set` read the rebuild's return only, and a feature that failed gave one
generic line under a result whose volume and bodies read as a success; (b) every refusal by
`Add2` got the same hint about syntax; (c) a dimension's result was its value, nothing about
the feature it drives. Fix: (a) the features' rebuild errors are read before the write and
after the rebuild (`FeatureManagementService.ErrorMap`/`NewErrors`, made internal for it);
each feature that fails now and did not before is named with its type and its error in a
warning ("The equations went in, but N feature(s) now fail to rebuild: 'Shell1' (Shell) error
… swFeatureError…"), new warnings in their own, and both results carry them in
`featureErrors`. (b) `EquationText.Refused` chooses the message: a global named like a
function says so; any other global set to a plain number says the equation is valid and that
SOLIDWORKS can refuse every equation until it is restarted, the hint "save, restart
SOLIDWORKS and repeat the call"; any other refusal keeps the syntax hint and adds that a
refused plain `"W" = 1mm` means a restart. (c) A dimension of a reference plane or axis
gives that feature's `frame` (as `ref_plane` and `ref_axis` report it,
`ReferenceService.FrameOf`) in the result, single and batch. Decided 2026-11-13 by the run,
not the owner: (1) the equations are kept when features fail, with the warning and
`featureErrors`, not rolled back as `dimension_set` does, because the decision names a
warning and a session may mean to fix the broken features next; (2) no probe equation is
written to tell the refusing state apart, because the only cheap probe is on the user's own
document and the message covers it; the names SOLIDWORKS refuses are told apart instead,
because without that the restart message would be wrong for them. On the code before, the
live tests failed: a shell made 25 mm thick by an equation on a 20 mm block gave no warning
naming 'Shell1' or its error, and an equation on an offset plane's distance gave no `frame`
(both in their final form `An_equation_that_breaks_a_feature_names_it_and_its_error` and
`An_equation_on_a_plane_s_offset_gives_where_the_plane_went`; a fillet radius was tried
first and does not break: SOLIDWORKS lets r100 overflow the block). (b) has no state to
provoke: `A_refused_plain_global_points_to_a_restart` (offline) checks the message choice
and `A_global_named_like_a_function_is_refused_for_its_name` the path live. After: those
four pass; the live tests on the changed paths, `A_global_variable_drives_…`,
`Equation_delete_takes_a_list_…`, `Equation_errors_are_typed_…`, `Several_equations_go_in_one_call_…`,
`A_batch_with_a_row_that_fails_…`, `Offset_plane_from_top_lies_at_y_25`,
`Negative_offset_and_flip_both_…`, `Axis_from_front_and_right_planes_…`,
`Dimension_set_restores_the_value_when_a_feature_breaks` and
`Feature_delete_lists_the_equations_…`, pass (14/14 with the four); contract tests 7/7;
unit tests 277/277; `check_repo` ok. Local: on `sw2023`, `32698d5`;
in build `20261114-024455`. Upstream: not yet decided.
Held in its scenario test (findings 2026-11-14, points 1, 6): (a) features named in
`featureErrors`; (b) both messages, and in a process that refused every equation, a batch
of ten globals and then `"W" = 60mm` alone, the restart message came as designed and was
the right advice: after a restart the equations went in at once; (c) `frame` in a batch
row. What else that process did wrong is S72. Closed 2026-11-14 on the owner's word.

### S63 - `feature_helix`: where its start is, and its turns in mm - CLOSED 2026-11-14

On build
`20261112-091908` (findings 2026-11-12, "a helical screw part", points 1, 2). (a)
`startAngleDeg` 0 on a top-plane circle started at model +Z, not the sketch's +x; the
description does not say where the angle's zero is, and after a `dimension_set` of it
nothing reads the start back (`feature_info` gives dimensions only; `model_measure` takes
no point by `at`). (b) The revolutions dimension reads `"value": 5527.777778, "unit":
"mm"` for 5.527778 turns, in `feature_helix` and `feature_info`: a count reported in mm
and ×1000, `ok`. Local: the zero in the description, `startMm` in `feature_info`, the count
without a length unit. Taken into the run of 2026-11-13 as proposed, no choice in it. Fixed in the run of 2026-11-13. Probe first (SOLIDWORKS 2023
SP3): the revolutions dimension has `IDimension.GetType()` -1 (`swDimensionParamTypeUnknown`,
shown as `swScalarDimension`) and `SystemValue` the plain number of turns; the start angle turns
the start counter-clockwise about the sketch normal, whatever `clockwise` and `reverse` say,
from model +Z on a top-plane sketch (a plane parallel to it too), +X on the front, -Z on the
right; on the top plane that is the sketch's -y, on the other two its +x, and not the circle's
own start point, so no rule in sketch terms holds. Cause: (a) the description said "Start angle
in degrees" and nothing more, and `feature_info` had no reader for a helix's definition; (b)
every dimension that is not angular or integer was converted as a length (m to mm), the
revolutions too, in `FeatureVerifier.Dimension` (feature results, `feature_info`,
`dimension_list`) and in `FeatureManagementLogic` (`dimension_set`, the configuration table).
Fix: (a) `startAngleDeg` says "Start, deg CCW about the sketch normal; 0 at Z (top), X (front),
-Z (right)."; `feature_info` gives a helix's definition (`kind: helix`, its `definedBy` as
`type`, `pitchMm`, `revolutions`, `heightMm`, `startAngleDeg`, `clockwise`, `reverse`,
`taperDeg`) and `startMm`, read from the curve as `feature_helix` reads it
(`AdvancedFeatureService.HelixStart`), so a `dimension_set` of the start angle reads back;
`feature_info`'s description names helices. (b) A dimension of unknown type is unit `scalar`,
its value as SOLIDWORKS holds it (rounded to 6 places), in `FeatureManagementLogic.UnitOf` /
`FromApi` / `ToApi`, which `FeatureVerifier.Dimension` now uses too, so every list and
`dimension_set` agree; it takes no tolerance (refused like a count); an equation's check on it
compares the plain number. The helix and `feature_info` descriptions were shortened elsewhere
to stay within the tool list's byte budget. Decided 2026-11-13 by the run, not the owner: (1)
the unit is `scalar`, for every dimension of unknown type, not `count` (whole numbers only,
which `dimension_set` enforces) and not a helix-only `turns`, because the cause is the
dimension's type, the same in every list, and SOLIDWORKS names it a scalar dimension; (2)
`dimension_set` takes a scalar as given, checked finite only, SOLIDWORKS' status and the
read-back judging its range, because the server cannot know the range of a dimension whose
type SOLIDWORKS does not give; (3) the zero is stated for the three standard planes in model
terms, `startMm` giving it on any other, because the probe found no rule in sketch terms. On
the code before (the new result fields declared, nothing filling them), the live tests failed:
`Helix_revolutions_read_as_a_number_without_a_unit_wherever_its_dimension_is_listed`
(`Assert.Equal() Failure: Strings differ, Expected: "scalar", Actual: "mm"`: 4.25 turns read
4250 mm) and
`Helix_start_angle_turns_counter_clockwise_about_the_sketch_normal_and_feature_info_reads_the_start`
(the start points held; `Assert.NotNull() Failure: Value is null` for `feature_info`'s
definition). After: those two pass (2/2); the live tests on the changed paths, the five
`Helix_…`/`Tapered_helix_…`/`Spiral_is_flat_…`, `Sweep_spring_along_a_helix_matches_pappus`,
`Sweep_with_the_profile_away_from_the_helix_start_is_rolled_back`, the two `Feature_info_…`,
`Dimension_list_returns_the_depth_…`, the three `Dimension_set_…`, the two `Tolerances_…`,
`A_global_variable_drives_the_extrude_depth`, `Several_equations_go_in_one_call_…` and
`A_table_makes_three_sizes_…`, pass (18/18); the offline `FeatureManagementLogicTests` 24/24
(`Values_convert_by_dimension_type` with the scalar); contract tests 7/7; unit tests 278/278;
`--check-catalog` ok; `check_repo` ok. Local: on `sw2023`, `e642451`;
in build `20261114-024455`. Upstream: not yet decided.
Held in its scenario test (findings 2026-11-14, point 6): the start at +Z, 90° read back
at +X, the turns `scalar`. Closed 2026-11-14 on the owner's word.

### S56 - Relations SOLIDWORKS adds while drawing go unreported, and a relation it already has reads as refused - CLOSED 2026-11-14

On build `20261112-091908` (findings 2026-11-12, "six printed and
stand-in parts", points 5, 6; "a helical screw part", point 5; "a part's body raised",
point 2; "a printed part's tree deleted", point 5; "two parts changed mid-tree", point 2;
2026-11-13, "an existing part edited", point 4; "four small printed parts", point 2). (a)
`sketch_constrain` `equal` reported `VERIFY_FAILED: … SOLIDWORKS ignored the relation …
no relation was added`, steps after it not run, while the relation was added: a failure
reported on a success. (b) `sketch_add` brings relations it does not report: an `equal`
on a rectangle drawn square, `coincident` to a body's edges on a plane through it,
polygon centres, segment ends merged onto existing points. The next dimension is then
refused as over-defining, or a `coincident` "ignored". Lessons the sessions left: ends a
polyline was drawn onto are merged already, `symmetric` lines do not hold their ends along
the centreline. Local: `sketch_add` lists the relations and merges SOLIDWORKS made on what
it drew; the "ignored" check reads whether the relation or its equivalent now holds, and
says so. Taken into the run of 2026-11-13 as proposed, no choice in it; SOLIDWORKS'
inference itself is not changed.
Cause: with inference off (`AddToDB`) SOLIDWORKS still relates what it draws (a corner
rectangle drawn square gets an `equal`, a corner on the origin a `coincident` to it, a
polygon a `tangent` and `patterned` relations) and merges a segment end drawn on an
existing end; `sketch_add` read none of it. `SketchAddConstraints` then ignores a relation
the sketch already has, and `AddRelation` took "nothing added" for a refusal. Fix (new
`SketchRelations`): `sketch_add` gives each entity `relations` ("equal: Line2, Line1",
"coincident: origin, Line1 start") and `merged` ("Line5 start merged onto [40, 40] (Line3
end, Line4 end)"), read from the relations on its segments and their points (by
`GetDefinitionEntities2`, which names the origin and model geometry where `GetEntities`
gives the sketch's own external point) and from point ids known before it; a relation
SOLIDWORKS ignored is looked up on the entities, and when the sketch holds it the step
succeeds with the check `already held: …` and equal relation counts, its geometry checked
as for an added one (a failing check now undoes nothing, as nothing changed);
`sketch_info`'s relation names moved to the same class. The `sketch_add` and
`sketch_relation_add` descriptions say so, shortened to stay within the tool list's byte
budget. Not reproduced on 2023: a `coincident` to a body's edges (a rectangle on a block's
outline, on the Front plane through it, got only its corner's `coincident` to the origin);
one would be listed as "a model edge". The `symmetric` lesson is not part of this fix.
Decided 2026-11-13 by the run, not the owner: (1) "already held" is a relation the sketch
holds on those entities (the same type or an equivalent one, collinear for parallel,
coradial for equal; a chain for equal, parallel, collinear, concentric and merge; a
horizontal and a vertical line for perpendicular, two of one for parallel; a point that is
the segment's own end or centre), not geometry that merely satisfies it, which stays
`VERIFY_FAILED`, because geometry satisfied by chance is not held and the check must stay
honest; (2) a reference to model geometry (an edge, a face) is never read as held, as its
key cannot tell one edge from another, so it fails as before; (3) `sketch_add` lists every
relation it made, a rectangle's horizontals and verticals included, up to 20 per entity,
not only the unexpected ones, because which are expected would be a per-kind table the
server would have to keep true; (4) "already held" is said in the checks, with no new
result field, the smaller change. On the code before (the two result fields declared,
nothing filling them), the live tests failed: `Sketch_add_lists_the_relations_and_merges_SOLIDWORKS_made`
(`Assert.NotNull() Failure: Value is null` for the square rectangle's relations) and
`Constrain_reads_a_relation_the_sketch_already_has_as_held_and_runs_the_next_steps`
(`Step 1 of 2 (equal relation) failed: SOLIDWORKS ignored the relation 'equal' for sketch
segment Line1, sketch segment Line2: no relation was added. No step before it was applied;
steps 2-2 were not run.`). After: those two pass (2/2), with the live tests on the changed
paths: the relation, merge, pierce, origin, `Constrain_…`, mirrored-shape, centre-rectangle,
offset, `Info_…` and 3D-sketch tests of `SketchEditLiveTests`, `Box_extrude_matches_the_oracle`,
`Top_plane_sketch_frame_is_x_0_minus_y`, the 3D-sketch weldment frame, the slot sweep and
the ellipse model box (27/27 with the two); unit tests 278/278; contract tests 7/7;
`--check-catalog` ok; `check_repo` ok. Local: on `sw2023`, `0acb318`;
in build `20261114-024455`. Upstream: not yet decided.
Held in its scenario test (findings 2026-11-14, point 6): (a) "already held", (b) the
relations listed. Closed 2026-11-14 on the owner's word.

### S55 - `sketch_constrain` passes the 50 s limit, with `symmetric` steps the suspect - CLOSED 2026-11-14

On
build `20261112-091908`, five times (findings 2026-11-12, "six printed and stand-in parts",
point 4; "a part of extrudes", point 1; "a part's body raised", point 1; "a printed part's
walls", point 2; "a printed part's tree deleted", point 3): 24, 31, 18, 20 and ~20 steps
returned `TIMEOUT` and completed at 54.6 to 95 s, all steps applied; meanwhile
`sketch_info` was `BUSY`, and up to ten `sw_status` calls went to waiting. Calls of 14 to 18
steps with no `symmetric` came back in seconds; those with two or three `symmetric` steps
took 55 to 95 s. A caller cannot tell beforehand. Bears on S8 (Closed). Local: probe the
cost of a `symmetric` step; then a limit that scales with the steps, or the cost in the
description, or the result's time. Decided 2026-11-13 by the owner: a probe of the cost of
each kind of step first; then a time limit that grows with the steps and their kinds, the
time in the result, the cost per step in the description.
Probe (live, a temporary test with the server's reads timed; a sketch of 23 segments on
Front: a closed 10-line outline, a centreline, a closed 6-line one, six circles). Each kind
in a call of its own: every kind cost the same, 3.6 to 4.3 s a step (horizontal 3.6-4.0,
vertical 3.6-4.1, coincident 3.9-4.0, equal 3.8, parallel 3.9, symmetric of points, of
circles and of lines 3.9-4.3; linear, diameter and vertical dimensions 3.6-3.8). Of that,
SOLIDWORKS' `SketchAddConstraints` took 8 to 37 ms (symmetric 10 to 37), a dimension's add
and value about 40 ms. The rest was the server reading the sketch back (`SkState.Take`):
every segment's name, construction flag, length, type and end points, one COM call each at
about 3.6 ms (~13 calls a line, ~1.0 s for the segments, ~0.25 s for the points), and a step
read it three times: as its start, after the add, and again as the next step's start.
Resolving the references before the first step added ~0.3 s a point `{at}` and ~0.1 s a
name. In one call of 20 steps, 11 had run at 57 s. So `symmetric` is not the cause: the
calls that ran past 50 s were long ones, and the time grows with the steps and the sketch's
size, whatever the kind; the calls said to come back in seconds were not reproduced. `BUSY`
follows from it: after the `TIMEOUT` the call keeps the one SOLIDWORKS thread until it
ends, and a `sketch_info` queued behind it gives up after its 20 s queue wait.
Cause: three full reads of the sketch a step, each ~50 ms a segment, under a fixed 50 s
limit. Fix (`SketchEditService`): (1) the state read after a step's add is carried as the
next step's start (`Carry`), so a step reads the sketch once; a step changes the sketch only
through its add, so every check reads what it read before; `sketch_relation_add` and
`sketch_dimension_add` are unchanged; (2) the limit is the default or 30 s + 5 s a step,
whichever is more (20 steps: 130 s); (3) the result has `durationS`, the seconds the call
ran in SOLIDWORKS; (4) the tool sends progress every 5 s, as the other long tools do; (5)
the description says "~2 s a step on 25 segments, growing with size", its other sentences
shortened to stay within the tool list's byte budget. The same 20 steps now run in 34.1 s
(1.7 s a step, resolving included). Not cut: the reference resolution (~5 s for 20 steps)
and the ~13 calls a segment.
Decided 2026-11-13 by the run, not the owner: (1) one allowance a step whatever its kind,
not a weight per kind, because the probe found every kind within 15 % of the others: the
cost is the server's read, which grows with the sketch, not with the kind; (2) 5 s a step
plus 30 s, which covers a sketch of about 80 segments at the measured rate, because the
sketch's size is not known before the call starts, and a limit too short reports a running
call as hung, which is this item's fault; (3) the reads were cut by carrying the state, not
by reading less a segment (bulk `GetLines2` / `GetArcs2`, no z in 2D), because carrying
keeps every check on the same values and the bulk calls' order against segment names is
not verified; (4) the field is `durationS`, the name `sw_status`'s `recentOps` uses.
On the code before, the new live test
`Constrain_runs_twenty_steps_on_a_sketch_of_two_dozen_segments_within_its_limit_and_says_how_long`
(20 steps, three `symmetric`, on 23 segments) failed: `TIMEOUT` "'sketch_constrain' has
been running for 50 s and exceeded its 50 s limit. SOLIDWORKS is still executing it and it
may still complete — do not simply retry." After: it passes (34.1 s, all 20 steps, the three
`points symmetric`, `durationS` within the wall time and under 50 s, the limit 130 s for 20
steps and 50 s for 3), with every live test of `SketchEditLiveTests` that calls
`sketch_relation_add`, `sketch_dimension_add` or `sketch_constrain` (31/31 with it); unit
tests 278/278; `ContractTests.Tool_catalog_follows_the_conventions` 1/1; `--check-catalog`
ok; `check_repo` ok. Local: on `sw2023`, `1861854`; in
build `20261114-024455`. Upstream: not yet decided.
Held in its scenario test (findings 2026-11-14, point 6): `durationS` given (10 steps, 9.7
s). Closed 2026-11-14 on the owner's word.

### S57 - "modified outside this server … (by the user?)" after the server's own calls - CLOSED 2026-11-14

On
build `20261112-091908`, six sessions, the owner out of SOLIDWORKS each time (findings
2026-11-12, "a part's body raised", point 7; "two parts changed mid-tree", point 7;
2026-11-13, "a groove's overhanging side", point 4; "a revolved part remade", points 3, 8;
"a plate's two ends", point 4). It came on the call after a `VERIFY_FAILED` rollback, a
`keepOnFailure`, a `FEATURE_FAILED` with no feature, a `feature_rollback`, and on an
assembly changed only through documents it loads. A false alarm that reads as the owner
editing. Local: a rollback, a kept or failed feature and a change through a loaded
document count as the server's own. Taken into the run of 2026-11-13 as proposed, no
choice in it. Fixed in the run of 2026-11-13. Cause: the warning compared the document's update
stamp with the one recorded after the server's last call that succeeded (`SwSession.RunAsync`
ran `SwContext.AfterWrite` only then), so what a failed call left (a `VERIFY_FAILED`
rollback, a feature kept with `keepOnFailure`, a `FEATURE_FAILED` with no feature) read as
someone else's; and only the bound document's stamp was recorded, so a document a call
changed through another (a part by `dimension_set` through its assembly, a top assembly by
`asm_insert` into its subassembly, the subassembly by a change to its part) warned on its
own next call. Fix: the warning compares with a new `StampSeen` per document
(`DocumentRegistry.Entry`), set with `StampAfterLastOp` after a completed call and, through
`SwContext.AfterFailedWrite`, after a failed one when the document matched its record at the
call's start (a document someone else changed keeps its warning); every other tracked
document that matched its record at the start and changed during the call takes its new
stamp; the stamp compared is read before `Bind` re-activates the document. `doc_undo` still
holds to `StampAfterLastOp`, the last completed change, as strict as before; its refusal now
names "through another document" among the causes. Decided 2026-11-13 by the run, not the
owner: (1) a change during a call counts as the server's in every other tracked document
that matched its record at the call's start, not only in those that load or are loaded by
the bound one, because what a call reaches (components, subassemblies, parents) cannot be
listed cheaply and SOLIDWORKS is running the server's call while it changes; (2) after a
failed call `doc_undo` stays refused as before, because what a rollback left on SOLIDWORKS'
undo stack is not known; (3) the stamp is compared before the re-activation in `Bind`,
because a change by the user is in it already and the activation can only add the server's.
On the code before, both new live tests failed:
`FeatureManagementLiveTests.The_servers_own_failed_kept_and_rolled_back_changes_do_not_read_as_the_users`
("after a VERIFY_FAILED rollback: '…' was modified outside this server since its last
change (by the user?).") and
`AssemblyLiveTests.A_change_through_a_document_an_assembly_loads_does_not_read_as_the_users`
("after dimension_set through the subassembly: … '….SLDPRT' was modified outside this
server since its last change (by the user?)."). A part changed through its own document
did not make its assembly warn, on the code before too. After: both pass (2/2), each with a
change made outside the server still warning; the live tests on the changed paths, the three
`Undo_…`, `Rollback_to_the_extrude_…`, `Rebuild_of_a_clean_part_…`,
`Dimension_set_restores_the_value_when_a_feature_breaks`, `Units_set_switches_…`,
`A_blind_cut_that_leaves_the_body_…_a_kept_failure_takes_its_name`, M0's
`A_boss_buried_in_the_body_is_rolled_back`, `Status_gives_a_failed_call_s_error_code_…`,
`Foreign_documents_are_read_only`, `Dirty_owned_documents_need_save_or_discard`, and
`Insert_puts_the_origin_…`, `Subassemblies_are_inserted_…`, `Compare_cannot_open_a_revision_…`
of `AssemblyLiveTests`, pass (15/15); unit tests 278/278; `check_repo` ok; no description
changed. Made stricter: none.
Local: on `sw2023`, `3a58652`; in build `20261114-024455`. Upstream: not yet
decided.
Held in its scenario test (findings 2026-11-14, point 6): no warning after a rolled-back
`VERIFY_FAILED`, nor after a change through an assembly. Closed 2026-11-14 on the owner's
word.

### S48 - Four small result follow-ups from the modelling scenario test of build `20261112-091908` - CLOSED 2026-11-14

(findings 2026-11-12, first entry, points 2 to 5). (a) From S40: a
blind cut with `name` and no floor, the check line names the depth dimension by the
feature's name before the rename, "no floor at 15 mm: … (D1@Cut-Extrude1 reads 15 mm)",
while `dimensions` in the same result gives `D1@<name>`; a caller taking the name from the
check gets one that no longer exists. Reported success, rightly; wording. (b) From S30:
`material_set` with the configuration whose material SOLIDWORKS names but does not apply
applies it (`densityKgM3` 7900, "matches the database") and still carries the warning
"… the material is not applied, so it is reported as none", as of now; it is why
`previous` is absent, and in a result whose material is applied it reads as a failure.
"was not applied" / "the previous one is reported as none" would fit. (c) From S38:
`feature_hole` with several points gives one warning per hole, the same 220-character
sentence with the names changed: six holes 1.3k of a 3.6k result, four holes 0.9k. One
warning naming the sketches, then the way out once. Wasteful, not wrong. (d) From S37:
`equation_set` with `equations` gave the session's largest results, 3.9k for thirteen
dimension links and 3.1k for ten globals changed, a quarter of the part's characters;
each row gives the equation, the previous one (or the driven dimension) and two checks.
Proposed by the session: a short row form (lhs, value, previous value), the full one by a
flag as `faceDetail` does for faces. Not wrong. Local, small. Taken into the run of
2026-11-13: (a) to (c) as proposed, no choice in them. (d) Decided 2026-11-13 by the
owner: a short row (lhs, value, previous value) by default, the full row by a flag, as
`faceDetail` does. (d) fixed in the run of 2026-11-13, then (a) to (c) (below). Cause: a
batch's result serialized every `EquationChange` in full (the equation, the previous one,
the dimension, the checks). Fix: `ResultJson.Serialize` takes `detail` (was `faceDetail`)
and shortens any result with the new `IShortRows`, as it does `newFaces`; `equation_set`'s
batch rows are then `{lhs, value, previousValue}` (no `previousValue`: the equation is
new), and `equationDetail=true` gives them in full. The warnings and `featureErrors` (S58)
stay whatever the flag. Decided 2026-11-13 by the run, not the owner: (1) only the batch
is shortened, the single call keeps its full result, because the finding is about batches
and one row is a few hundred characters; (2) a row keeps `frame` when the dimension drives
a reference plane or axis, because it is there so that a plane on the wrong side shows
(S58 c); (3) the `equations` parameter's example drops its second entry, because the flag
took the tool list over its byte budget. Before, on the code before the fix,
`A_batch_result_gives_short_rows_unless_asked_in_full` (ten globals and a depth link)
failed: "Collections differ at index 0, Expected: lhs, Actual: equation". After:
it passes, the eleven rows under 900 characters; with
`An_equation_on_a_plane_s_offset_gives_where_the_plane_went`,
`Several_equations_go_in_one_call_…`, `An_equation_that_breaks_a_feature_names_it_and_its_error`,
`A_batch_with_a_row_that_fails_…`, `Equation_delete_takes_a_list_…` and
`A_feature_result_lists_its_new_faces_short_…` 7 of 7 live; unit tests 278 of 278 (with
`A_result_with_short_rows_gives_them_in_place_…`); `ContractTests.Tool_catalog_follows_the_conventions`
and `--check-catalog` ok; `check_repo` ok. Local: (d) on `sw2023`, `ffde4db`;
in build `20261114-024455`. Upstream: not yet decided.
(a) to (c) fixed in the run of 2026-11-13. (a) Cause: `feature_extrude` wrote the no-floor
check line while the cut still had SOLIDWORKS' name and applied `name` after it, so the
dimension in the line kept the old name. Fix: the rename takes the check lines with it, a
`@<old name> ` in a line reading `@<new name> `. (b) Cause: `material_set` and
`model_mass` shared one sentence, written for `model_mass`, where it is the state now. Fix:
`material_set`'s reads as of before the call, "SOLIDWORKS named the material '…' for '…',
but its mass had … kg/m³, not the … kg/m³ of '…': it was not applied, so the previous one is
reported as none."; `model_mass`'s is as before. (c) Cause: one warning per placement
sketch not fully defined. Fix: one warning for them all, "The placement sketches '<sketch>'
of <hole>, … are under defined, so those holes (and a pattern of them) can move:
sketch_edit each, sketch_constrain its point(s) from the origin until it reads fully, then
sketch_finish.", each sketch's state in the list when they differ; a single hole keeps its
sentence as before. Before, on the code before the fix, each test with the point's asserts
added: `PartFeatureLiveTests.A_blind_cut_that_leaves_the_body_…_a_kept_failure_takes_its_name`
failed ("Assert.Contains() Failure: Filter not matched in collection: ["volume", "no floor
at 30 mm: the cut broke out of the body b"···]": no line naming `D1@ThroughHole`, while
`dimensions` did); `ConfigurationLiveTests.A_material_named_but_not_applied_reads_as_none_and_is_applied_again`,
both cases, failed ("Sub-string not found … Not found: "was not applied, so the previous
one is reported a"···"); `PartFeatureLiveTests.Hole_names_each_placement_sketch_and_warns_while_it_is_under_defined`
failed ("Assert.Single() Failure: The collection contained 2 matching items", one per
hole). After: the three pass (4/4, the theory's two cases); with
`A_blind_cut_from_a_plane_inside_the_body_…`, `Hole_through_all_at_two_points_…`,
`Hole_drill_6x10_…` (one hole, the sentence as before), `Hole_on_an_explicit_face_…` and
`Hole_failing_at_the_second_point_…` of `PartFeatureLiveTests`,
`A_name_that_picks_up_a_deleted_feature_s_equations_warns` and
`The_servers_own_failed_kept_and_rolled_back_changes_…` of `FeatureManagementLiveTests`,
`ConfigurationLiveTests.A_material_set_in_one_configuration_stays_out_of_the_others`, and
`Material_set_AISI_1020_gives_the_analytic_mass` and `Material_set_rejects_unknown_names_…`
of `PropertiesLiveTests`, 14/14 live; unit tests 284/284; `check_repo` ok; no description
changed. Made stricter: none. Decided 2026-11-13 by the run, not the owner: (1) in (c), a
single hole keeps its own sentence and several share one naming each as "'<sketch>' of
<hole>", because the finding is about the repeat and a hole's name holds parentheses
("Ø5.0 (5) Diameter Hole1"); (2) in (a), only `feature_extrude`'s rename carries its check
lines, because it is the one feature tool whose check line names a dimension. Local: (a)
to (c) on `sw2023`, `ef358df`; in build `20261114-024455`. Upstream: not
yet decided.
Held in its scenario test (findings 2026-11-14, point 6): (a) to (d). Closed 2026-11-14 on
the owner's word.

### S37 - `equation_set` took one equation per call and said nothing of the body - CLOSED 2026-11-12

In a
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
change that splits it. Upstream: a PR not yet decided. In build `20261112-091908`. Held
in its scenario test (findings 2026-11-12, first entry, point 1): ten global variables in
one call and thirteen dimension links in one more, each row read back, `bodies: 1` in each
result, the ten changed and set back in one call each with the volume back exact; the
machined part took 39 calls and 26.3k characters against 83 and 32.4k. Not run there: a
change that splits the body. Follow-up: these are the session's largest results (S48 d). Closed 2026-11-12 on the owner's word.

### S42 - `sketch_dimension_add` angular could solve to the mirrored shape and report the angle matched - CLOSED 2026-11-12

In two printed parts on build `…-064352` (findings 2026-11-11, third
entry, point 4): two lines meeting at a vertex 130° apart, 50° asked, `textAt` inside the
wedge: one line went to the mirrored solution, the result read `value reads back 50°,
geometry measures 50°`, only the `changed` list showed it, and the sketch went `fully`
defined in the wrong shape. Bears on S6, whose fix keeps the supplement by `textAt` and
checks the value, not the shape. Workaround: delete and redraw the line, dimension the
angle the rays really make first, the distances after. Usage (point 5):
`sketch_relation_add` `merge` of a free line's end onto a point held by relations moves
the free line whole, and a second merge pulled the shape back into the mirrored solution;
redraw at the held points' exact coordinates instead of merging onto them. Decided
2026-11-11 by the owner: a warning, not a refusal, since it can be what the caller meant.
Fixed in `ef2eddd` on `sw2023`, in `sketch_dimension_add`, `sketch_relation_add` and each
step of `sketch_constrain` (the warning starts "Step n:"). Found live first: the solver
does not turn a line round here. With one line held horizontal and the other fixed the
angle is refused as over-defining; with freedom left, a line turns by the 80° between 130°
and 50°, which is the mirrored shape and no reversal. So three signs, one warning each:
(a) an angular dimension set to within 0.5° of the angle the lines already made across the
vertex, while it sits on the one between the rays: the warning names both angles, the
lines that turned and a `textAt` inside the other angle; (b) a line turned by more than
90°, or an arc that bends the other way, after a dimension or a relation; (c) for a linear
dimension, a segment's end moved more than twice what the value changed by, over 1 % of
the sketch's extent. Three new live tests, each asserting a warning the tools did not
give: the 130° wedge by the single tool and as step 2 of `sketch_constrain`; a line turned
round by a horizontal dimension and by a `coincident` relation; a point on a line rising
80° moved 28.8 mm by 5 mm of horizontal distance; and no warning for a dimension that only
moves what it measures. `SketchEditLiveTests` 66/66 on 2023 SP3. (c) also fires for a
point that slides along a line steeper than 60° to the dimension, where the move is right:
a warning the owner may find noise. To check in the scenario test: the wedge of point 4
warns, and a fully defined sketch of the test part gives no warning. In build
`20261112-091908`. Held in its scenario test (findings 2026-11-12, first entry, point 1):
no warning in nine fully defined sketches; the 130° wedge given 50° with `textAt` inside it
warned with both angles, the line that turned and a `textAt` in the other angle. Closed 2026-11-12 on the owner's word.

### S41 - `feature_pattern_linear` `direction2` ignored its sign, reported `ok` - CLOSED 2026-11-12

In two
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
wrong way by itself. In build `20261112-091908`. Held in its scenario test (findings
2026-11-12, first entry, point 1): a 2 x 2 pattern toward `-x` and `-z` landed all four
holes, "direction flipped", no instance-face warning. Closed 2026-11-12 on the owner's word.

### S45 - Feature results listed every new face in full - CLOSED 2026-11-12

In two printed parts on build
`…-064352` (findings 2026-11-11, third entry, point 11): `feature_extrude`,
`feature_mirror` and `feature_fillet` listed up to twelve new faces each, 2 to 4k
characters, read for one id or none. Same kind as S22 (Closed), whose fix did not cover
faces. Decided 2026-11-11 by the owner: a short list by default (each face's id and kind),
the full detail by a flag. Fixed in `cbd79a7` on `sw2023`: `newFaces` is `[{id, surface}]`
unless `faceDetail:true`, which gives each face as before (area, pick point, normal or
axis, radius). In the 37 tools whose result carries `newFaces`: the feature tools, the
body and surface tools, `face_edit` and the sheet-metal flanges, hem and bend; the server
guide says so. The services still return every face in full and the cut is made where the
result is written, so nothing a tool checks changed. A unit test of the two forms and a
live test (a boss's three faces as a tool returns them, its cap's id from the short list
taking the next sketch); the short form is the new behaviour, so neither is a test that
fails without a fault. Unit tests 260/260, Contract 7/7; `tools/list` 242.4 to 246.5 KB
for the new parameter. To check in the scenario test: the characters of a
`feature_extrude`, a `feature_mirror` and a `feature_fillet` result against 2 to 4k, and
one call with `faceDetail:true`. In build `20261112-091908`. Held in its scenario test
(findings 2026-11-12, first entry, point 1): `newFaces` id and surface only (a boss's
result 0.75k), in full with `faceDetail:true`. Closed 2026-11-12 on the owner's word.

### S40 - A blind cut whose far end leaves the body was refused, though SOLIDWORKS made it - CLOSED 2026-11-12

In two printed parts on build `…-064352` (findings 2026-11-11, third entry, point 1): a
hole through a plate and a pocket opening through a slanted underside, `feature_extrude`
cut, blind, gave `VERIFY_FAILED: ... no floor face at 12 mm along [-1, 0, 0]` and was
rolled back; the cut was right. `through_all` would have gone through other walls. Met
twice. Workaround: `keepOnFailure:true`, the volume read from the next result; on that
path `name` was not applied (`Cut-Extrude2` kept), one `feature_rename` more. Decided
2026-11-11 by the owner: the cut passes on its volume with a check line, and `name` is
applied on the `keepOnFailure` path; `upToFace` / `through next` parked. Fixed in
`c1d3bc5` on `sw2023`. (a) A cut with no floor face at all passes on the volume it
removed, with the check "no floor at 30 mm: the cut broke out of the body before its end"
and the depth dimension's name; a cut with a floor at another depth than asked is still
refused, and so is a boss with no cap. (b) A feature kept by `keepOnFailure` takes the
call's `name`, and the error ends "The result was kept as '<name>' (keepOnFailure)"; in
`feature_extrude` only, the other feature tools keep SOLIDWORKS' name on that path. New
live test (a blind cut 30 deep in a 20 mm block, its volume the hole's; a 5 mm pocket
still showing "floor at 5 mm"; a boss inside the body kept as `Inner`) fails without it
(`no floor face at 30 mm along [0, -1, 0]`). With the Modelling live tests 10/10 on 2023
SP3. Not run: a pocket that opens through a slanted face on one side only, where part of a
floor may be left at the asked depth (then it reads as a floor, with no line). To check in
the scenario test: a blind hole deeper than its plate. In build `20261112-091908`. Held
in its scenario test (findings 2026-11-12, first entry, point 1): a blind cut 15 deep in a
10 mm block passed on its volume with the no-floor line and took its `name`. Follow-up:
the check line names the depth dimension by the name before the rename (S48 a). Closed 2026-11-12 on the owner's word.

### S38 - `feature_hole` leaves its placement sketch under-defined, unreported - CLOSED 2026-11-12

In the same
part (point 4): the result names the hole's own sketch dimensions but not the placement
sketch or its status; only `model_review` showed it, as `info` (`sketch_under_defined`),
and a pattern of the hole rests on a point that can move. Workaround, 5 calls:
`sketch_edit`, one `sketch_constrain` (horizontal and vertical from the origin, `fully`),
`sketch_finish`, two `equation_set`. Local: return the placement sketch's name and
constrained status, an option to dimension each point from the origin; perhaps
`model_review` ranks an under-defined sketch as a warning. Decided 2026-11-11 by the
owner: the placement sketch's name and constrained status in the result, and a warning
when it is under-defined; no dimensioning option (`sketch_constrain` does it in one call);
`model_review`'s rank stays. Fixed in `dbd72ed` on `sw2023`: the result's
`placementSketches` gives, per hole, its feature, the sketch that places it and its
`constrained` status, read after the hole is made (of the hole's two sketches, the one
with points and no segment but construction geometry; the other is the profile with `Hole
Dia.`), and a warning names each one not fully defined with the way out (`sketch_edit`,
`sketch_constrain`, `sketch_finish`). The tool's description says so. A live test (two
through holes: two placement sketches, each `under`, each in `model_review` as
`sketch_under_defined`, neither the profile) fails without it (`placementSketches` null).
The hole tests 10/10 and the five other live tests that make holes (Drawing 4,
FeatureManagement 1) passed; unit tests 260/260. To check in the scenario test: a
`feature_hole` result names its placement sketch as `under` with the warning; after
`sketch_constrain` `model_review` no longer lists it. In build `20261112-091908`. Held in
its scenario test (findings 2026-11-12, first entry, point 1): each placement sketch named
`under` with a warning; after `sketch_constrain` and two equations `model_review` was
clean. Follow-up: one warning per hole, the same sentence each time (S48 c). Closed 2026-11-12 on the owner's word.

### S43 - `sketch_offset` has no dimension a caller can name, and reads under-defined - CLOSED 2026-11-12

Same
parts (point 6), met in both: the offset's dimension could not be found (`D5@<sketch>` not
found after four dimensions), so no equation can drive it; and a sketch that read
`constrained: fully` while open is listed `sketch_under_defined` by `model_review` once it
holds an offset. Local: return the offset dimension's full name in the result; read why
`model_review` and the sketch's own status disagree. Fixed in `fa434e5` on `sw2023`. The
cause: `SketchOffset2` was called with its last argument, the dimension, off, so the
offset was held by its relation alone; the status read while open did not count that until
a rebuild, hence `fully` open and `under` after. The offset is now made with its dimension
(one per side for `both`), and the result's `dimensions` names each (`D3@<sketch>`), with
a check line, and a warning if SOLIDWORKS adds none. A live test (a fully defined 100 x 60
rectangle offset 5 outward: `D3 = 5`, still `fully`, not in `model_review` after finish;
`D3` set to 8 gives a 116 x 76 loop) fails without it (`dimensions` null); the six offset
tests passed, the both-sides one now reading two dimensions of 5; unit tests 260/260. To
check in the scenario test: an offset's result names its dimension, an `equation_set` on
it holds, and `model_review` does not list the sketch. In build `20261112-091908`. Held in
its scenario test (findings 2026-11-12, first entry, point 1): `D3@<sketch>` = 5, still
`fully`, an equation of 8 gave the 116 x 76 loop, the sketch not in `model_review`. Closed 2026-11-12 on the owner's word.

### S18 - `doc_pack` faults - CLOSED 2026-11-12

(1) From a document opened `readOnly:true`, the copies still
reference the originals (`VERIFY_FAILED: Some copies still reference other files`), three
times; read-only is the likely cause, not yet tested with a writable open. Workaround that
held: close all, remove the originals, reopen the copies. (2) `includeDrawings:true`
copies no drawing when the drawings lie in another folder, and reports success; Pack and
Go only looks next to the model. (3) Missing: new names per file (only prefix or suffix).
Local: refuse or reopen a read-only source, report `noDrawingsFound`, say both in the
description; then PR. (3) not yet decided. Decided 2026-11-11 by the owner: (1) refuse a
source opened read-only, with a hint to open it writable; (2) report `noDrawingsFound`;
both in the description. (3) parked. Fixed in `fc5e4a4` on `sw2023`. (1) Confirmed live:
from an assembly opened read-only the copies were written and `VERIFY_FAILED` said they
still referenced the originals; such a document is now refused before anything is written
(`INVALID_ARGUMENT`, "open read-only", the hint to close and open it writable). (2) With
drawings asked for and none in Pack and Go's list, `noDrawingsFound: true` and a warning
that it looks only next to each model under its name. Both in the description. The pack
live test now does both (on the code before, the read-only pack got `VERIFY_FAILED`); the
two pack tests passed, unit tests 260/260. (3) stays parked, out of this item. To check in
the scenario test: `doc_pack` of a read-only document is refused, and a part whose drawing
lies elsewhere reads `noDrawingsFound`. In build `20261112-091908`. Held in its scenario
test (findings 2026-11-12, first entry, point 1): an assembly open read-only was refused
before anything was written; a part with no drawing beside it read `noDrawingsFound`. Closed 2026-11-12 on the owner's word.

### S31 - `feature_delete` `dryRun` says kept what the delete removes - CLOSED 2026-11-12

In the scenario test
of build `20261111-140208` (point 3, from S26): a mirror with two Hole Wizard children,
`deleteChildren`, `dryRun:true` listed the holes' four sketches under `keptAbsorbed`; the
real call then removed them, each `absorbed by <hole>`. A child's absorbed sketches go
with the child whatever `deleteAbsorbed` says. Local, small: the plan lists them as
removed with that `why`, so the dry run and the delete agree. Fixed in `40ffab9` on
`sw2023`: with `deleteChildren`, a child's absorbed sketches are planned as removed
(`absorbed by <child>`), whatever `deleteAbsorbed` says; it still decides for the
requested features' own sketches. The live test of a delete with children now runs a dry
run first and compares both lists with the delete's; on the code before, the dry run
lacked the hole's two sketches. The FeatureManagement delete tests passed. To check in the
scenario test: a mirror with two holes, `deleteChildren`, `dryRun` then the delete, the
same lists. In build `20261112-091908`. Held in its scenario test (findings 2026-11-12,
first entry, point 1): `deleteChildren`, the dry run and the delete listed the same seven
items. Closed 2026-11-12 on the owner's word.

### S44 - `feature_chamfer` `equal` makes a chamfer with no faces on 2023 - CLOSED 2026-11-12

Same parts
(point 7): on a concave edge and on a circular edge, `'Chamfer1' was created but has no
faces`, then the tool fell back to distance-distance by itself and held. Twice in two
tries. Reported success, the right chamfer. Local, small: if `swChamferEqualDistance`
never works on 2023 SP3 (a live probe first), go straight to distance-distance and drop
the warning, in the interop patch or behind a version check. Probed 2026-11-11, live: the
equal type made a chamfer with no faces on a convex straight edge, a convex circular edge
and a concave circular edge, each time. Fixed in `2a1f5ae` on `sw2023`, behind a version
check: on revision 31 (2023) an equal chamfer is made as distance-distance with equal legs
at once, no failed attempt, no warning; other versions keep the attempt and its fallback,
untried here. The equal chamfer live test now asserts no warning and adds one on a boss's
concave base; on the code before it carried the warning. Chamfer tests and the two others
that make chamfers passed. To check in the scenario test: an `equal` chamfer's result has
no warning. In build `20261112-091908`. Held in its scenario test (findings 2026-11-12,
first entry, point 1): `equal` on a bore's circular edge, no warning, the volume as
computed. Closed 2026-11-12 on the owner's word.

### S39 - Two small result follow-ups from the same part - CLOSED 2026-11-12

(point 7). (a) `sketch_add` of a
centre rectangle returns six segments and `sides` names four: the two construction
diagonals, needed for a midpoint relation to the origin, are not named and were guessed.
(b) `material_list {filter:"6061"}`: 4 rows in 1268 characters, about two thirds the list
of databases, repeated on every call. Same kind as S22 (Closed). Local, small. Fixed on
`sw2023`: (a) in `7742f2c`, a centre rectangle's entity names its two construction
diagonals in `diagonals` (a corner rectangle has none); (b) in `0470a2e`, `material_list`
with a filter lists only the databases it reaches (by name or by a row found), all of them
without one. A live test of each fails without it (no `diagonals`; 4 of 5 databases
holding none of the rows). To check in the scenario test: a centre rectangle's
`diagonals`, and `material_list {filter:"6061"}`'s length. In build `20261112-091908`.
Held in its scenario test (findings 2026-11-12, first entry, point 1): a centre
rectangle's `diagonals`; `material_list {filter:"6061"}` 0.65k (was 1.27k), one database. Closed 2026-11-12 on the owner's word.

### S27 - A document loaded as a reference shows as `foreign` in `doc_list` - CLOSED 2026-11-12

In the
scenario test of build `…-091645` (point 5): after `doc_open` of a drawing (read-only),
the part it references was listed as a document of its own, `access: foreign`, though
nobody opened it, and `doc_close` of the drawing closed it too (`also closed`). Right in
SOLIDWORKS terms, but `foreign` reads as the user's document, which `doc_close` says it
never closes. Usage: a part listed after opening a drawing or an assembly may be its
reference, loaded with it. Local, small: mark such a document (an `access` value or a
`loadedBy`), and say in the `doc_list` description. Decided 2026-11-11 by the owner: a
`loadedBy` field naming the document that loaded it; `access` keeps its values. Fixed in
`e340317` on `sw2023`: `doc_open` marks each document that came in with the one it opened
and that the server does not own; `doc_list` gives it `loadedBy`, that document's handle;
the description says so. The drawing-close live test now reads `doc_list` first (the part
foreign, `loadedBy` the drawing's handle; on the code before, null); Export and TopDown
live tests and the pack test, which open files, passed. Only `doc_open` marks; a reference
loaded otherwise (an `asm_insert`) has none. To check in the scenario test: `doc_open` of
a drawing, then `doc_list`. In build `20261112-091908`. Held in its scenario test
(findings 2026-11-12, first entry, point 1): `doc_open` of a drawing read-only, its model
in `doc_list` with `loadedBy`. Met there too: a part loaded by an assembly is changed
through it by `dimension_set`, unsaid in the result (S49 b). Closed 2026-11-12 on the owner's word.

### S35 - `model_compare` against an earlier revision of the same file name - CLOSED 2026-11-12

From the
source, not run (findings 2026-11-11, first entry, point 5): `other` is opened with
`OpenDoc6`, and SOLIDWORKS does not open a second document of a name already open, so the
call ends `IoError`, whose hint (compare by handle) cannot help. For an assembly copied
under another name, its components resolve to the open documents of those names, so two
revisions compare the same parts and nothing says so. Local: the description names both,
or the tool copies `other` to a temporary name and warns when an assembly's components
came from memory. To confirm live first. Decided 2026-11-11 by the owner: confirm both
live, then a line in the tool's description naming each case; the copy to a temporary name
parked. Confirmed live 2026-11-11 and fixed in `4e6d556` on `sw2023`. (1) Worse than read:
a full path to the earlier revision was taken for the open document of that file name, so
the call ended "'other' is this document", and with another open document of that name it
would have compared that one, reported `ok`. A full path is now that file; SOLIDWORKS does
not open it, and the `IoError` names the open document in the way and how to get past it.
(2) The copied assembly opened and read 30 mm deep, the open part's size, not the 20 mm of
its own files. Both named in the description. A live test of both (on the code before,
`INVALID_ARGUMENT` for the first). To check in the scenario test: `model_compare` against
a copy of an open part in another folder under its name. In build `20261112-091908`. Held
in its scenario test (findings 2026-11-12, first entry, point 1): against a file whose
name is open, refused, both paths named. Closed 2026-11-12 on the owner's word.

### S30 - `model_mass` and `material_set` name a material the configuration does not have - CLOSED 2026-11-12

In the scenario test of build `20261111-140208` (point 1): with a material in rows A and C
only, `model_mass` in `Default` gave `"material":"AISI 1020"` beside a density of 1000,
and a following `material_set` with no `configuration` reported `"previous":"AISI 1020"`,
both `ok`; the table, which reads by the configuration's name, gave none for `Default`.
Both read `GetMaterialPropertyName2("")`. Likely the material that reached `Default` while
row A was set (S29's cause) and was given back by name still answers to `""`; not probed.
Local: read by the active configuration's name in both, and probe whether S29's putting
back leaves something behind under `""`. Probed 2026-11-11 on `sw2023` at `fa434e5`, live:
a block, `config_table` rows A (`AISI 1020`), B, C, `material_set` 6061 in C with
`Default` active. Read by each configuration's name, `Default` has no material until it is
activated again after A, B and C; from then on `GetMaterialPropertyName2` by its own name,
by `""` and `MaterialIdName` all give `AISI 1020`, after a rebuild too, while its mass
reads 1000 kg/m³ and its body names no material. So reading by the active configuration's
name does not fix it: SOLIDWORKS keeps a name it does not apply. A test that fails on it
(`model_mass` names `AISI 1020` in `Default`) and the reads by name in six services were
written and set aside, not committed. Waits on the owner: what the tools report when the
name and the density disagree (e.g. the name with a warning, or no material), and whether
to look for where the name comes from (S29's putting back, or `Default`'s activation).
Decided 2026-11-12 by the owner: report, and apply again. `model_mass` and `material_set`
say the configuration has no material (SOLIDWORKS still names one) with a warning, and
`material_set` with that configuration applies the material again, so the name and the
density agree. Setting the document's density, or a custom material by `material_add`, was
offered and not taken: it hides the mismatch. Fixed in `bb23828` on `sw2023`: where the
named material's database density and the measured one disagree (and no body has its own
material), `model_mass` reports no material and `material_set` no previous one, each with
a warning naming both densities, and `material_set` with that material applies it again. A
live test (rows A to C, `AISI 1020` in A, `6061 Alloy` in C, each activated in turn, then
`Default`) fails without it (`model_mass` names `AISI 1020` at 1000 kg/m³); with it, both
paths (`configuration` named and not) read 7900 kg/m³ after `material_set`; with the 16
other live tests that call `model_mass` or `material_set`, 17/17. Not changed: the other
readers of the name (`model_info`, cost, `model_review`, simulation), which still read it
by `""`. To check in the scenario test: the same part, `model_mass` in `Default`, then
`material_set` and its mass. In build `20261112-091908`. Held in its scenario test
(findings 2026-11-12, first entry, point 1): rows A to C, a material in A and C;
`model_mass` in `Default` named none, with the warning; `material_set` in `Default` read
7900 kg/m³ and `model_mass` then named it. Follow-up: `material_set`'s warning reads as of
now in a result whose material is applied (S48 b). Closed 2026-11-12 on the owner's word.

### S24 - `drw_annotation_edit` delete moves other dimensions, unreported - CLOSED 2026-11-12

In the scenario
test of build `…-024431`: three horizontal dimensions stacked below a view, text at y 109,
101 and 93 mm. After the one at 109 was deleted, the `before` of the next two deletes gave
both at y 109: SOLIDWORKS had closed up the stack. The result says only that the count
went down by one and the name is no longer listed; in a drawing that keeps them, the
layout changes without a word. Bears on S3 (Closed), whose tool it is. Local: read the
view's other annotations' positions before and after a delete and name each that moved.
Decided 2026-11-11 by the owner: name each that moved, before and after; do not put them
back (a caller moves one with `drw_annotation_edit` if it wants).
Fixed in `479b885` on `sw2023` (2026-11-12): a delete reads the other annotations'
positions before and after; each that moved is in `othersMoved` (name, view, beforeMm,
afterMm) and in a warning, else a check says they stayed where they were. New live test
(three stacked horizontal dimensions, the first deleted: SOLIDWORKS moved the others)
fails without it. In build `20261112-091908`. Held in its scenario test (findings
2026-11-12, second entry, point 2): the first of three stacked dimensions deleted,
`othersMoved` named the ones SOLIDWORKS closed up, before and after, with the warning;
other deletes read "stayed where they were". Closed 2026-11-12 on the owner's word.

### S32 - Two small result follow-ups from the scenario test of build `20261111-140208` - CLOSED 2026-11-12

(a) From S22 (point 5): the results of `drw_views_standard` and `drw_view_section` still
carry the model's full path in each view, which `drw_info` cut. (b) From S25 (point 6): a
pick one diameter off a hole's centre rightly got "used the nearest one, 2.25 mm away",
but 2.25 is also the radius and it read as the centre case, and an `edge_list` call went
to finding the real centre; when the edge taken is a circle, that warning can name its
centre and radius. Local, small. Not yet fixed. Fixed in `d3f401c` on `sw2023`
(2026-11-12): (a) the views of `drw_views_standard` and of the derived views (projected,
section, detail, auxiliary) name the model's file only; the full path stays at the
result's top or in the parent view. (b) When the nearest edge taken is a circle, the
warning names it: "used the nearest one, 1 mm away: a circular edge of radius 1 mm centred
at [30, 0, 20] mm". A live test for each fails without it. In build `20261112-091908`.
Held in its scenario test (findings 2026-11-12, second entry, point 2): the views of
`drw_views_standard` and of two new sections name the model's file only, the path once at
the top; a pick one diameter off a hole's centre warned "used the nearest one, 2.25 mm
away: a circular edge of radius 2.25 mm centred at […]". Closed 2026-11-12 on the owner's word.

### S13 - `drw_dimensions_auto` gives dimensions that cannot be used, reported as success - CLOSED 2026-11-12

On a part with fully defined sketches: 25 dimensions, construction ones among them,
duplicates, overlapping texts, some off the sheet, none for features whose sketches had
none. Without a delete tool they could not be removed, so the drawing was restarted; with
S3's `drw_annotation_edit` (Closed) they can be deleted one by one. Usage: manual
`drw_dimension_add` only. Decided 2026-11-11 by the owner: no code change; the
description says the result needs cleaning and points to `drw_dimension_add`.
Done in `5bd35ec` on `sw2023` (2026-11-12): `drw_dimensions_auto`'s description says the
result needs cleaning (duplicates, construction ones, overlapping texts) and that
`drw_dimension_add` checks each. In build `20261112-091908`. Held in its scenario test
(findings 2026-11-12, second entry, point 2): the description says so. Closed 2026-11-12 on the owner's word.

### S46 - A release is registered in one project, the server in two - CLOSED 2026-11-12

From reading a
session's call count (findings 2026-11-11, fourth entry): `CLAUDE.md`'s release step 4
registers the new exe "(scope local, in the owner's project)". The server is registered at
local scope in two of the owner's projects, each with its own exe path in
`~/.claude.json`; builds up to `…-064352` went to both, the five after it only to the one
where the scenario test runs. A session in the other project ran two parts and an assembly
on `…-064352`, without `sketch_constrain` (S8), ~150 of ~330 calls single relations and
dimensions. `sw_status` reported the old build rightly; nothing compared it with the
latest release. Bears on S15 and S16. Local, in this repo's release steps: step 4 reads
every project's `mcpServers.solidworks-mcp.command` from `~/.claude.json`, names each on
an older build in the question to the owner, and registers the new exe in each. The
owner's call. Taken up outside this repo the same day: a session-start hook of the owner's
re-registers a project's `solidworks-mcp` on the newest `build-` tag, run by hand once (it
moved the stale project to `20261111-140208`), not yet run at a session start. With it,
step 4 may only need to say that the hook does the other projects. Decided 2026-11-11 by
the owner: a line in step 4, to register the new exe by hand in every project
`~/.claude.json` lists with `solidworks-mcp`, until that hook has held at a session start.
Done in `0e1de7a` on `sw2023`: step 4's question names every project with the build it
runs, and the yes registers the new exe in each. Closes when it has held at a release.
Held at build `20261112-091908` (2026-11-12): both projects were named with the build they
ran (`20261111-140208`) and registered on the new exe. Closed 2026-11-12.

### S36 - Tools no session had run - CLOSED 2026-11-12

From the tool reference (findings 2026-11-11, first
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
scenario test where it is kept, not here. Closes when they are there. Written 2026-11-12 into the scenario test where it is kept: a part of global
variables built again with a feature and a sketch renamed and `model_review` read, and an
assembly of two parts the server makes and the owner mates by hand, its components and
mates read and one length changed. Closed 2026-11-12, as decided. Both ran in the
scenario test of build `20261112-091908` (findings 2026-11-12, both entries) and held; the
assembly's length change left S49.

Each sent upstream stays here with its PR named, and moves to Merged upstream when the
PR is merged (the owner, 2026-11-14).

### S9 - `sw_help` at session start reads ≈23.5k characters - DECIDED AGAINST 2026-11-11

Usage: a short note of the tools
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

### S15 - `tools\install.ps1` registers at user scope under the name `solidworks` - DECIDED AGAINST 2026-11-11

That
clashes with any other server of that name. Usage: `-NoRegister`, and register by hand at
local scope. Upstream: perhaps an option for the name and scope; not yet decided. Closed 2026-11-11 on the owner's word, not taken: the server is registered by hand at local scope, and a session-start hook of the owner's keeps each project on the newest build (S46).

### S29 - `A_table_makes_three_sizes_each_with_its_own_volume_and_reads_back` fails on 2023 SP3 - CLOSED 2026-11-11

A Configuration live test: the table read back has no `$MATERIAL` column
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

### S22 - Results that repeat what the caller already has - CLOSED 2026-11-11

`drw_note_add` echoes the whole
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

### S25 - A point at a hole's centre resolves to the hole's edge with a bare distance - CLOSED 2026-11-11

In the
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

### S26 - `feature_delete` with `deleteChildren` warns once per absorbed sketch of a child - CLOSED 2026-11-11

In
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

### S23 - `Tolerances_read_back_and_fits_follow_the_ISO_tables` fails on 2023 SP3 - CLOSED 2026-11-11

A
FeatureManagement live test: `Expected: 0,021`, `Actual: 0`. Seen 2026-11-09 while fixing
S19, with and without that fix, in a SOLIDWORKS session open since 18:05. Not yet looked
into: a long session, a 2023 difference in reading tolerances or fits, or the test. Looked into 2026-11-11: it passed in a SOLIDWORKS session started minutes before, alone
and in the FeatureManagement live tests in full (89 of 89 with SketchEdit). Its failures
came from SOLIDWORKS sessions open for hours, not from the server or the test. Closed
2026-11-11 on the owner's word.

### S28 - `material_add` kept its own library; a library that did not parse failed `INTERNAL` - CLOSED 2026-11-10

In the scenario test of build `…-115800` (point 2): `material_set` of a
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

### S8 - One relation or dimension per call - CLOSED 2026-11-10

A fully defined sketch took ≈15 calls. Asked:
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

### S16 - The build a session runs could not be told from inside it - CLOSED 2026-11-10

`sw_status` reported
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

### S17 - Warnings and texts that were noise - CLOSED 2026-11-10

, each met in every scenario test from build
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

### S21 - A document opened `readOnly:true` became dirty without an edit - CLOSED 2026-11-10

: an assembly after
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

### S51 - `feature_chamfer` takes the distance on whichever face, one `flip` for all edges, and a curved edge's legs are not checked, reported `ok` - CLOSED 2026-11-16

On build `20261112-091908`
(findings 2026-11-13, "a groove's overhanging side", points 1 to 3; "a revolved part
remade", points 2, 10; "one chamfer over three edges", points 1, 3). (a) `angle_distance`
and `distance_distance` do not say which face takes the distance, and `flip` is one switch
for the call: two edges of one groove, mirrored and patterned, took it on opposite faces.
The leg check caught it (`VERIFY_FAILED … found legs 1/1.085649 mm`) but named one edge of
the two; five attempts and four deletes to a right pair. (b) On circular edges the legs
are not checked ("2 edge(s) not leg-checked (curved edge or non-planar faces)"): a tube's
two end chamfers in one call, one with its legs swapped, came back `ok`; only the volume
showed it (-28.07 against -28.32). The cone face's apex and half angle are in the result
with `faceDetail`, enough to work the legs out, as a session did by hand. (c) A distance
typed to 4 decimals at a face's width (1.4835 for 1.483537) left a sliver of 0.0003 mm²,
`ok`. (d) Missing: no edit of a chamfer's edge set, no per-edge distance (a fix took 7
calls with a rollback); `feature_mirror` of a chamfer failed `FEATURE_FAILED`. Usage
until then: one chamfer per edge, each with its own `flip`; give the steepness as an angle
from a face a distance should consume. Local: per edge, the face that took the distance and
the chamfer face's angle to each neighbour, `flip` per edge, the legs of a curved edge from
the cone, a warning for a selected edge's face left under ~0.01 mm². Decided 2026-11-13 by
the owner: all four, `flip` per edge and an edit of an existing chamfer's edges and
per-edge distances included. Fixed in the run of 2026-11-13, all four points. (a) The leg check now
reads, per edge, the two faces beside the chamfer face and the cross-section triangle,
and the checks say "E2: 1 mm along F16 (plane [0, 0, 1]), 2 mm along F17 (plane [0, 1,
0]); the chamfer face meets the first at 63.435° and the second at 26.565°". A warning when
one side took the first distance on one edge and the second on another (a plane by its
normal, a curved face by type, radius and axis). `flip` per edge: `flipEdges`, indexes in
`edges`, set through `IChamferFeatureData2.SetIsFlipped` and `ModifyDefinition` after the
insert (the API's flip option is one for all edges). (b) A circular edge's legs are read
from the cone: its two boundary circles as the change in radius and the shift along the
axis from the original circle, checked against the expected legs as a line's are; the
wedge angle is read from the faces' outward normals at the edge, so plane-to-cylinder
edges are checked too. (c) A face beside a chamfer face left under 0.01 mm² is warned
("left a sliver of … mm²: a distance just under that face's width"). (d) `feature_chamfer`
with `edit: "<chamfer>"` changes it through its definition: `edges` (named as before the
chamfer, resolved while `AccessSelections` has the model rolled back), `distanceMm`,
`distance2Mm`, `angleDeg`, `flipEdges` (exactly those flipped), and `distancesMm`
([first, second] per edge): an edge at distances other than the chamfer's moves to a new
chamfer inserted right after it (the rollback bar moved there and back), as one chamfer
feature holds one set of distances in the 2023 API (`IChamferFeatureData2` has no per-edge
distance). Verified as a new chamfer is; on a failure the new chamfers are deleted and the
chamfer is put back as it was (its old edges, flips and distances re-applied). Not taken:
`feature_mirror` of a chamfer (`FEATURE_FAILED`), not in the decision. Decided 2026-11-13
by the run, not the owner: the edit is `feature_chamfer`'s `edit` parameter, not a tool of
its own, and `flipEdges` and the per-edge distances index `edges` rather than repeat edge
references, because `tools/list` was at its per-tool byte budget (1536, `--check-catalog`)
before this item and a tool of its own went 16 bytes per tool over it; to bring the
additions under it, `faceDetail`'s description is shortened in every feature tool ("true:
newFaces in full.", 37 tools). Live tests, four new: `Chamfer_says_per_edge_which_face_took_each_distance`
(and `flipEdges` on one edge with the mixed-sides warning), `Chamfer_measures_the_legs_on_circular_edges`
(a rod's two end circles, 1/2 mm, volume from the rings), `Chamfer_warns_of_a_sliver_left_by_a_distance_just_under_a_face_s_width`
(19.99995 on a 20 high face), `Chamfer_edit_changes_edges_flips_and_distances_and_splits_edges_with_their_own`
(a chamfer with a hole after it: an edge added and flipped, distances changed, a too large
distance refused and put back, an edge at 3/3 moved to a new chamfer before the hole, the
last through the tool). On the code before, the first three failed: "Expected: 2, Actual:
0" (no per-edge line), "Filter not matched … 2 edge(s) not leg-checked (curved edge or
non-planar faces)", and no warning (`Warnings` null); the fourth is new API (no edit
existed). After: those four and the chamfer tests before (`Chamfer_equal_3mm_…`,
`Chamfer_two_distances_…`, `Chamfer_rejects_bad_input_…`), with `Linear_pattern_along_a_slanted_edge_…`
and `Auxiliary_view_looks_square_onto_a_chamfer` that make chamfers, 9/9, and
`ContractTests` (catalog, EntityRef schemas) 2/2, on 2023 SP3; unit tests 276/276;
`--check-catalog` ok; `check_repo` ok. Local: on `sw2023`, `16699e2`; in
build `20261114-024455`. Upstream: not yet decided.
Failed in its scenario test (findings 2026-11-14, point 2), reported `ok`:
`feature_chamfer` `edit` with `flipEdges` flips every edge, not the indexed ones. A
`distance_distance` 1/2 chamfer on two bottom edges of a block meeting at a corner (one
along x, one along z) took 1 mm on the bottom face for both; `edit` with the same two
edges and `flipEdges:[1]` made both take 1 mm on their side faces (the per-edge checks say
so, and the chamfer faces' normals [0, -0.894, -0.447] and [0.447, -0.894, 0] confirm it),
with no mixed-sides warning; `flipEdges:[0]` put both back on the bottom. A new chamfer
with `flipEdges:[1]` on two other edges gave both their first distance on the side faces,
consistent with each other, so what index 0 got there cannot be told. The per-edge checks
read the result right; the entry says nothing of (b) and (c). Back to Open 2026-11-14 for
`flipEdges` in (a) and (d). Not yet decided.
Taken into the run of 2026-11-16, group 1: `flipEdges` in `edit` flips exactly the indexed
edges, read back per edge (`## Run 2026-11-16`).

Fixed in the run of 2026-11-16, `flipEdges` in (a) and (d). Cause: SOLIDWORKS 2023 keeps
one flip for chamfer edges that meet at a corner: `IChamferFeatureData2.SetIsFlipped` on
one edge sets the flag on the other at once (read back before `ModifyDefinition`), so the
last edge set won for both; the probe found the same with tangent propagation off, and the
flip held per edge only for edges apart (the test of 2026-11-13). Fix: when the flips of a
chamfer's edges differ, the side each edge is to take is read with none flipped (the face
that took the first distance, or the other one for an edge in `flipEdges`); the flips are
then set and each edge's side read back from the chamfer faces; an edge left on the wrong
side moves to a chamfer of its own right after it (the rollback bar moved there and back),
flipped there until it takes that side, with a warning naming it ("SOLIDWORKS keeps one flip
for chamfer edges that meet at a corner: … moved to 'Chamfer2' … Edit each chamfer on its
own."). An edge that cannot be brought to its side fails `VERIFY_FAILED`, rolled back (an
edit puts the chamfer back as it was). The same for a new chamfer, an edit, and an edit's
chamfer for edges at their own distances. The per-edge checks and the mixed-sides warning
read the faces of both chamfers. Test before: `Chamfer_flipEdges_flips_only_the_indexed_edge_of_two_meeting_at_a_corner`
(a 1/2 chamfer on the block's bottom edges along x and along z meeting at a corner, `edit`
with `flipEdges:[1]`, then a new chamfer with `flipEdges:[1]`) failed: "plain (False,
False), after flipEdges [1] (True, True)", both edges on their side faces. After: it and the
chamfer tests (`Chamfer_*` in `PartFeatureLiveTests`, 8 with it), with
`Linear_pattern_along_a_slanted_edge_…` and `Auxiliary_view_looks_square_onto_a_chamfer`,
10/10, on 2023 SP3; unit tests 284/284; no tool description changed. Made stricter: each
edge's side is read back from the geometry when the flips differ, not taken from the flag
set. Decided 2026-11-16 by the run, not the owner: an edge SOLIDWORKS won't flip apart from
the edge it meets goes to a chamfer of its own (said in a warning), rather than the call
being refused, because the owner decided a flip per edge and a second chamfer is the one way
the 2023 API gives to it; one chamfer feature per side, as the per-edge distances already
are. Left: that second chamfer is a feature of its own, so a later edit naming its edge in
the first chamfer finds it chamfered by a later feature. Local: on `sw2023`, in the commit
that moves it here; in build 20261116-050908 (`dedd986`). Upstream: not yet decided.
Held 2026-11-16: the scenario test of build `20261116-050908` (findings 2026-11-16,
points 1 and 2): `edit` with `flipEdges:[1]` on two edges meeting at a corner left edge 0
on the bottom face and put edge 1 on its side face, the faces' normals agreeing, edge 1 in
`Chamfer2` with both warnings; a new chamfer the same; two edges apart kept one chamfer and
`flipEdges:[0]` swapped exactly the two; a circular edge's legs measured. Left for S75:
`newFaces` misses the split-off chamfer's face.

### S73 - `feature_pattern_circular` turns the instances the sense it picks, reported `ok`, with no option for the other - CLOSED 2026-11-16

On build `20261112-091908` (findings 2026-11-13, "two printed plates", point 2): a 10°
pattern of a cut at 30° about a hole whose cylinder face reads its axis as [0, 0, -1] put
the instances at 20° and 10°, the other way from what was meant, the result saying
"instance faces at the expected positions: 10/10 (3 x 10° about [0, 0, -1],
counter-clockwise)". The check confirms the sense the server chose, about an axis the
caller did not pick, and the tool has no option for the sense; `totalAngleDeg` 350 was
refused ("go past 360°"). Workaround: the seed moved to the other end of the range, 4
calls. On the same part a 270° pattern said "rotation sense flipped (SOLIDWORKS' default
missed the part)" and landed right. Reported `ok` while not what was asked, though what it
reported was true. Proposed by the session: a `reverse` option, and the instances' angles
reported about a model axis the caller names. Bears on S41 (Closed). Local. Not yet
decided.
Taken into the run of 2026-11-16, group 1: a `reverse` option, the angles given about the
axis as the caller named it (`## Run 2026-11-16`).

Fixed in the run of 2026-11-16. Cause: `FeatureCircularPattern5` turns a partial pattern in
SOLIDWORKS' own sense (its `flip` argument false), which the call cannot predict, and the
tool had no way to ask for the other; the check named the sense found, counter-clockwise
about the axis as the face reads it ([0, 0, -1]), which a caller thinking of +z reads the
wrong way round. Fix: `reverse` on `feature_pattern_circular` passes the flip, so the
instances turn the other way from the call without it; with it the instances must land
(no retry the other way, the hint says "or drop reverse"). The positions check now gives
the sense and the instances' angles from the seed about the axis as read, and when that
direction points the negative way (along -x, -y or -z, or its largest component negative)
the same about the axis the other way along it: "3 x 10° about [0, 0, -1] (the axis as
read), counter-clockwise: the instances at +10°, +20° from the seed; the axis as read
points the other way from +z: about +z, clockwise, at -10°, -20°" (more than four
instances: the first two, "…" and the last). Without `reverse`, a pattern whose default
missed the part is still redone flipped, its check now "rotation sense flipped, as
reverse:true (…)". The wording is an interop-free function (`CircularPatternSense`) with
unit tests. To pay for the option within the tool list's byte budget, the pattern tools'
descriptions lose what their parameters already say: "count includes the seed" in
`feature_pattern_linear`, `feature_pattern_circular` and `feature_pattern_curve`, the
curve's "spacingMm apart along the curve, or spread equally over it", and "Rotation axis"
in the circular one's `axis` (tools/list 253,571 bytes, 33 under the budget). Test before:
`Circular_pattern_names_its_sense_about_the_axis_as_read_and_turns_the_other_way_with_reverse`
(a 100 x 100 x 10 plate on the front plane, a Ø10 hole at the origin whose face reads its
axis [0, 0, -1], a Ø4 hole at 30° on r 30, a 10° pattern of three about the Ø10 face, then
the same with `reverse`) failed on the code before: the instances at 10° and 20°, and
"Assert.Contains() Failure: Sub-string not found … 'about [0, 0, -1] (the axis as read),
counter-clock…'" (the check read "2/2 (3 x 10° about [0, 0, -1], counter-clockwise)"); its
`reverse` half did not compile ("'CircularPatternRequest' does not contain a definition for
'Reverse'"). After: it (the instances at 40° and 50° with `reverse`, two volumes of the seed
each), with the circular pattern tests (`Circular_pattern_*` in `PartFeatureLiveTests`, 4
with it) and `ContractTests` (7), 11/11, on 2023 SP3; unit tests 287/287 (3 new);
`--check-catalog` ok; `check_repo` ok. Made stricter: the check names the sense about the
axis read the other way when the reference's own direction is negative, and a `reverse`
pattern that misses the part fails rather than being turned back.
Decided 2026-11-16 by the run, not the owner: `reverse` is the other way from SOLIDWORKS'
own sense (its flip), not a sense fixed about the axis as read, because the checks name
the sense found and a fixed sense would need a second feature built whenever SOLIDWORKS'
default ran the other way; the smaller change.
Decided 2026-11-16 by the run, not the owner: with `reverse` a pattern that misses the part
is rolled back instead of tried the other way, because the caller asked for that sense and
turning it back would report `ok` for what was not asked, as this item was.
Decided 2026-11-16 by the run, not the owner: the angles are given about the axis read the
other way only when the direction read points the negative way, because a positive axis
already reads as a caller expects, and saying it twice costs words in every result.
Local: on `sw2023`, in the commit that moves it here; in build 20261116-050908 (`dedd986`). Upstream: not
yet decided.
Held 2026-11-16: the scenario test of build `20261116-050908` (findings 2026-11-16,
point 4): the item's case, the instances at 20° and 10° with the check naming the sense
about the axis as read and about +z, and at 40° and 50° with `reverse`. Not met there: a
default that misses the part ("as reverse:true") and a `reverse` that misses it ("or drop
reverse"); their code paths ran in the live tests only.

### S68 - `asm_mate_add` coincident with an origin fails - CLOSED 2026-11-16

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

Local: on `sw2023`, in the commit that moves it here; in build `20261116-080311` (`643fc77`). Upstream: not yet
decided.
Held in its scenario test (findings 2026-11-16, build 20261116-080311, point 3): a
component's origin coincident with the assembly's, "points coincide", the component's
origin read back at [0, 0, 0]; a component's origin on a vertex of a component inside a
subassembly, the origin at the vertex. Left for S78: `{component, type:"origin"}` with no
name is refused, and an origin in a mate's row reads `kind: "point"`.

### S69 - A component's named plane inside a subassembly cannot be selected - CLOSED 2026-11-16

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

Local: on `sw2023`, in the commit that moves it here; in build `20261116-080311` (`643fc77`). Upstream: not yet
decided.
Held in its scenario test (findings 2026-11-16, build 20261116-080311, point 2): a
top-level cube's `Top Plane` coincident with `Top Plane` of a cube inside a subassembly,
"planes coincide (0 mm apart)", the top-level cube's origin moved to y 15, the mate
listing the nested component's `Top Plane`.

### S70 - A top-level component may resolve to a nested one of the same short name - CLOSED 2026-11-16

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

Local: on `sw2023`, in the commit that moves it here; in build `20261116-080311` (`643fc77`). Upstream: not yet
decided.
Held in its scenario test (findings 2026-11-16, build 20261116-080311, point 1): with
two cubes at the top level and two of the same short names inside a subassembly,
`asm_component_move` and `asm_component_set` `visible:false` by the short name reached the
top-level ones, the nested ones in place and visible, and a mate took the top-level one
too. Not met: the refusal of a short name only nested components share; its code path
ran in the live test only. Left for S78: a mate result's `moved` gives short names and
misses a top-level component that moved.

### S62 - Assembly reading and options - CLOSED 2026-11-16

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

Local: (b) on `sw2023`, in the commit that moves it here; in build `20261116-080311` (`643fc77`); proof: the
scenario test of build `20261116-080311`. Upstream: not yet decided.
Held in its scenario test (findings 2026-11-16, build 20261116-080311, point 4): (b)
`flexible:true` "rigid → flexible", `false` "flexible → rigid", `false` again "nothing",
each row with `flexible` and `previous.flexible`; a part refused `INVALID_ARGUMENT`;
flexible again, a mate moved the subassembly's floating child while the subassembly
stayed. With (a), (d) and (e) closed 2026-11-14 and (c) probed and not a fault, no
lettered point is left open. Left for S76: setting it rigid put a mate in error with no
warning; for S77: a child of a flexible subassembly reads fixed.

### S75 - `feature_chamfer` lists in `newFaces` only the named chamfer's faces when an edge moves to a chamfer of its own - CLOSED 2026-11-16

Sorted out of the entry of 2026-11-16, point 2. On build `20261116-050908`, with S51's fix:
a new `distance_distance` chamfer with `flipEdges:[1]` on two edges meeting at a corner
moved edge 1 to `Chamfer2` (said in a warning, `alsoCreated`), and the result gave
`faceCountDelta` 2 but `newFaces` one face, the named chamfer's; the split-off chamfer's
face took a `face_list {feature}`, one call more. Reported `ok`; the count was right.

Proposed: `newFaces` lists the faces of every chamfer the call made, those of
`alsoCreated` named apart. Local, the S51 code. Taken into the run of 2026-11-16, group 2
(a small point in group 1's files).

Fixed in the run of 2026-11-16. Cause: the result of a feature tool lists the faces of
its main feature only (`FeatureVerifier.Result`), and the chamfer an edge moved to (S51)
is a feature of its own, named in `alsoCreated`. Fix: a chamfer call, new or `edit`, lists
in `newFaces` the faces of every chamfer in `alsoCreated` after the named chamfer's (up to
twelve in all), `newFaceCount` counting them all; in the short list (no `faceDetail`) a
face of another feature than the first face's gives its `feature` too, so the split-off
chamfer's face is told apart. Test before:
`Chamfer_flipEdges_flips_only_the_indexed_edge_of_two_meeting_at_a_corner`, with
assertions added (each call's `newFaces` two faces, the named chamfer's then the
`alsoCreated` one's, the short list naming the second's feature), failed on the code
before at the new chamfer: "newFaces F25 (Chamfer1), newFaceCount 1, faceCountDelta 2,
alsoCreated Chamfer2". After: it and the other chamfer tests (`Chamfer_*` in
`PartFeatureLiveTests`), 8/8, on 2023 SP3; unit tests 288/288 (one new, the short face's
`feature`); `check_repo` ok. No tool description changed.
Decided 2026-11-16 by the run, not the owner: the short list names a face's feature only
when it is not the first face's, not on every face, because the main chamfer is the
result's `feature` already and the short list was cut to id and surface to save
characters (S45); the change is in the serializer, so any feature result whose faces
come from more than one feature gives it, which only a chamfer's does now.

Local: on `sw2023`, in the commit that moves it here; in build `20261116-080311` (`643fc77`). Upstream: not yet
decided.
Held in its scenario test (findings 2026-11-16, build 20261116-080311, point 8): a
`distance_distance` 1/2 chamfer with `flipEdges:[1]` on two edges meeting at a corner gave
`newFaces` two faces, the second with `feature: "Chamfer2"`, `newFaceCount` 2 =
`faceCountDelta`, `alsoCreated` and both warnings as before; a plain chamfer one face, no
`feature`. Left for S79: `doc_undo` right after that split chamfer is refused.

### S49 - `dimension_set` through an assembly does not say which component, nor the document it changed - CLOSED 2026-11-16

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
Fixed in the run of 2026-11-16, group 3. Cause: the line took the handle and access from
the registry's entry for the part (`DocumentRegistry.Find`), and a part the server had not
listed yet (one only the assembly loaded, its own document closed or never opened by the
server) had none, so the line named its title alone. Fix: `WarnChangedOthers` registers
such a part first (`DocumentRegistry.Track`, the call `doc_list` makes), so the line always
reads "Also changed, not saved: '<title>' (<handle>, <access>), a component's own
document.", with the handle and access `doc_list` then gives it; the access is the
registry's own rule (S62 (d): a part from a file this server owned, loaded by its owned
assembly, reads `owned`). A part registered by the call takes its update stamp after the
change as seen (S57), so a later change by the user still warns. Test before:
`AssemblyLiveTests.Dimension_set_through_an_assembly_names_a_part_not_listed_yet_by_its_handle_and_access`
(a cube saved and closed, a new assembly inserting it, its `D1@Boss-Extrude1@<component>`
set) failed on the code before, "no handle and access: Also changed, not saved:
'unlisted_….SLDPRT', a component's own document." After (SOLIDWORKS restarted first: it ran
but could not be reached over COM): it passes, 1/1, the line's handle, title and access
those of `doc_list`'s row for the part, the part dirty; with it the live tests that call
`dimension_set` through an assembly or on a part, `AssemblyLiveTests`
`Dimension_set_through_an_assembly_names_the_component_…`,
`A_change_through_a_document_an_assembly_loads_…`, `A_part_the_server_owned_comes_back_owned_…`,
`Compare_cannot_open_a_revision_…`, `Configurations_are_chosen_at_insert_…`;
`TopDownLiveTests.An_in_context_cover_…`; `FeatureManagementLiveTests`
`Dimension_set_changes_…`, `Dimension_set_refuses_…`, `Dimension_set_restores_…`,
`Rename_reads_back_and_renames_the_dimensions`; M0's `Dirty_reads_unsaved_changes_…`: 11/11
pass. Unit tests 288/288; `check_repo` ok; no description changed. Made stricter: none (the
call refuses nothing new; its line now always carries a handle and access).
Decided 2026-11-16 by the run, not the owner: a part registered by the line takes its
update stamp after the change as seen, because without a stamp on record the S57 check
would stay silent on a later change by the user, and the change this call made is the
server's.
Local: on `sw2023`, in the commit that moves it here; in build `20261116-124830` (`ad5f6ba`). Upstream: not yet
decided.
Held in its scenario test (findings 2026-11-16, build 20261116-124830, point 6): in an
assembly whose part's own document was closed and never listed, `dimension_set` of a
component's dimension 20 → 25 named the row as given, `volumeDeltaMm3` 8000 (four
instances) and "Also changed, not saved: '<part>.SLDPRT' (D5, owned), a component's own
document.", `doc_list` giving that part D5, owned, dirty; in a hand-mated assembly opened
from file, the shaft's length 60 → 80 → 60 through it named "'<part>.SLDPRT' (D10,
foreign)", ±6283.185, mates without error, the three files byte for byte unchanged after
`discard`.

### S74 - `dimension_set` on a mate's angle driven by an equation calls it a driven (reference) dimension - CLOSED 2026-11-16

On build `20261112-091908` (findings 2026-11-13, "two printed plates", point 7):
`dimension_set` on an angle mate's dimension driven by an equation refused it "is a driven
(reference) dimension: it reports a value but doesn't control the geometry". It is
driving, through an equation; for a part's dimension the refusal names the equation's
variable, for a mate's it does not. A hint that misses. Bears on S37 (Closed). Local. Not
yet decided.
Taken into the run of 2026-11-16, group 3: the refusal names the equation and its variable
(`## Run 2026-11-16`).
Fixed in the run of 2026-11-16, group 3. Cause: `dimension_set` checked a dimension's
driven state before its equations, and SOLIDWORKS reads a mate's dimension an equation
drives as driven (`swDimensionDriven`), so the refusal for a reference dimension came
first; and the equation's own refusal, where it was reached, named neither the equation
nor its variable ("is driven by an equation … Change the equation instead."). Fix: the equation is looked for first, by the name read back or the one given
(letter case ignored, a component's qualified equation not taken for the document's own
dimension), and the refusal names it and what its right-hand side uses, for a mate's
dimension and a part's alike: "'D1@Angle1' is driven by the equation "D1@Angle1" =
"TILT", which uses "TILT"; the next rebuild would overwrite a direct change.", hint
"Change it with equation_set: lhs 'TILT' for the value the equation uses, or lhs
'D1@Angle1' for a new expression." (a variable named before a dimension it also uses; an
equation of a constant names the dimension's own `lhs` and `equation_delete`). The rule is
in `FeatureManagementLogic` (`EquationOn`, `EquationDrivenRefusal`), with an offline test.
Test before: `AssemblyLiveTests.Dimension_set_on_a_mate_angle_an_equation_drives_names_the_equation_and_its_variable`
(two cubes, a parallel and a 30° angle mate on their front planes, `"TILT" = 40` and the
mate's dimension `= "TILT"`, `dimension_set` 50 on it) failed on the code before: "'D1@Angle1'
is a driven (reference) dimension: it reports a value but doesn't control the geometry."
After: it passes, the angle still 40 after the refusal; with it
`FeatureManagementLogicTests.An_equation_driven_dimension_is_refused_naming_the_equation_and_what_it_uses`
(offline) and the live tests that call `dimension_set`: `FeatureManagementLiveTests`
`Dimension_set_changes_…`, `Dimension_set_refuses_…` (now with a part's depth driven by a
variable), `Dimension_set_restores_…`, `Rename_reads_back_and_renames_the_dimensions`;
`AssemblyLiveTests` `Dimension_set_through_an_assembly_names_a_part_not_listed_yet_…`,
`Dimension_set_through_an_assembly_names_the_component_…`, `Configurations_are_chosen_at_insert_…`,
`A_part_the_server_owned_comes_back_owned_…`, `A_change_through_a_document_an_assembly_loads_…`;
`AdvancedFeatureLiveTests.Helix_start_angle_…`: 12/12 pass. Unit tests 288/288;
`check_repo` ok; no description changed. Made stricter: a part's dimension driven by an
equation had no test of its refusal; `Dimension_set_refuses_…` now asserts it names the
equation, its variable and `equation_set`, and the mate's test that nothing changed.
Decided 2026-11-16 by the run, not the owner: the equation is matched by its exact
left-hand side (letter case aside), not with a document qualifier stripped, because a
component's equation `"D1@Cut1@cube-1.Part"` would otherwise refuse the assembly's own
`D1@Cut1`, a refusal that is not true; the earlier check matched exactly too.
Local: on `sw2023`, in the commit that moves it here; in build `20261116-124830` (`ad5f6ba`). Upstream: not yet
decided.
Held in its scenario test (findings 2026-11-16, build 20261116-124830, point 2): a part's
dimension driven by `"H"` refused naming the equation, `"H"` and `equation_set` lhs `H` or
`D1@Plate`; a mate's angle driven by `"TILT"` refused the same way, the angle still 40,
and `equation_set TILT 50deg` then read 50 on the mate.

### S67 - `feature_rollback` before the only feature fails its check - CLOSED 2026-11-16

Seen in the run of
2026-11-13, in S60's probe, on that run's code: `feature_rollback {to:<renamed extrude>,
before:true}` on a part holding one extrude failed `VERIFY_FAILED` "… unexpected state:
'Sketch1'". A valid call refused, the rollback's state not said. Not probed: whether the
rename, the single feature or the check's reading of the bar is the cause. Local. Decided
2026-11-13 by the run, not the owner: an item of its own, open and not fixed in the run,
because the run's items were the owner's and this one has not been looked at.
Taken into the run of 2026-11-16, group 3, a probe first (`## Run 2026-11-16`).
Fixed in the run of 2026-11-16, group 3. Probe: the call fails the same with the extrude
renamed and not renamed, and on a part of two extrudes `before` the second fails naming
`Sketch2`, so neither the rename nor the single feature is the cause. The history walk
lists a feature's absorbed sketch just before it (`Sketch1`, owner `Boss-Extrude1`, then
`Boss-Extrude1`), and `EditRollback` before the feature rolls back that sketch too (read
from SOLIDWORKS while the bar stood there: both rolled back). Cause: the check's reading,
which expected every feature from the named one on rolled back and the ones above it
active, so the absorbed sketch read as "unexpected state". A fillet, with no sketch, never
showed it: the only `before` in the tests named a fillet. Fix: for `before`, the first
feature expected rolled back is the named one's run of absorbed features listed just
before it (its own sub-features by `GetOwnerFeature`), in `feature_rollback`'s check
(`FeatureManagementService.FirstRolledBack`); `rolledBack` lists the sketch with its
feature. Test before: `FeatureManagementLiveTests.Rollback_before_the_only_extrude_rolls_it_and_its_sketch_back`
and `…_renamed_rolls_it_back` (a 100 × 60 × 20 block, `before` its extrude, renamed
`Plate` in the second) failed on the code before: "The rollback bar is not where requested
(EditRollback returned True); unexpected state: 'Sketch1'. Tried to move it back to its
previous position." After: both pass, with
`Rollback_before_a_second_extrude_keeps_the_first_and_its_sketch` (a 10 × 10 × 5 boss on
the block, `before` it: its sketch and it rolled back, −500 mm³, the block's extrude
active) and the live tests that call `feature_rollback`:
`Rollback_to_the_extrude_hides_the_fillet_and_end_restores_it`,
`Rollback_refuses_unknown_features_and_before_end`,
`The_servers_own_failed_kept_and_rolled_back_changes_do_not_read_as_the_users`: 6/6 pass.
Unit tests 288/288; `check_repo` ok; no description changed. Made stricter: `before` a
sketched feature had no test; the three tests assert the bar's text, the features rolled
back by name, the volume change and the state after `end`.
Decided 2026-11-16 by the run, not the owner: `rolledBack` lists the absorbed sketch with
its feature, not the feature alone, because SOLIDWORKS does roll the sketch back and the
list reads `IsRolledBack` as it is.
Decided 2026-11-16 by the run, not the owner: only the named feature's own sub-features
listed right before it move the expected first feature up, not any sketch rolled back,
because the check then still reports any other feature in a state not asked for.
Local: on `sw2023`, in the commit that moves it here; in build `20261116-124830` (`ad5f6ba`). Upstream: not yet
decided.
Held in its scenario test (findings 2026-11-16, build 20261116-124830, point 1): `before`
a block's only extrude rolled back `Sketch1` and `Boss-Extrude1`, −120 000, volume 0, and
`end` restored it; the same with the extrude renamed `Plate`; `before` a 10 × 10 × 5 boss
on it rolled back its sketch and it, −500, the block's extrude active, `end` +500.

### S76 - `asm_component_set` `flexible:false` puts a mate in error, reported `ok` with no warning - CLOSED 2026-11-16

Sorted out of the entry of 2026-11-16, point 5. On build `20261116-080311`, with S62 (b):
a subassembly set flexible, a mate on its floating child held the child where the
subassembly's own file does not put it; `asm_component_set` `flexible:false` then returned
"flexible → rigid" with no warning, the child went back to where the file puts it and the
mate went into error (`asm_mate_list`: `inError: 1`, `error: 51`, `errorName:
"swSketchErrorExtRefFail"`). Reported `ok`. The name is read from `swFeatureError_e`
(`FeatureVerifier.ErrorName`), where 51 is a sketch's error: for a mate it may be the wrong
enum. Bears on S62 (Closed).

Proposed: a change of `asm_component_set` that rebuilds compares the mates in error before
and after and names those it put in error in a warning (and a check line); a mate's
`errorName` read from the enum its code belongs to, or the bare code. Local, the assembly
files. Not decided.
Decided 2026-11-16 by the run, not the owner: taken into the run of 2026-11-16, group 3,
because it is small and in group 2's files, so the next group, as the run's "How it runs"
says.
Fixed in the run of 2026-11-16, group 3. Probe: SOLIDWORKS 2023 gives code 51 to a mate it
cannot satisfy, in the case met and in a plain one (a distance mate between two fixed
parts set to a value they do not have, through `dimension_set`, refused and read as
"Distance1: swSketchErrorExtRefFail"). The 2023 interop has no mate error enum: the mate
errors are `swFeatureError_e`'s `swFeatureErrorMate*` (38 to 48), and 51 is
`swSketchErrorExtRefFail` there. The mate's error clears when the subassembly is made
flexible again. Cause: `asm_component_set` read no mate after a change, and
`FeatureVerifier.ErrorName` named every code by `swFeatureError_e` whatever the feature.
Fix: `asm_component_set` reads the mates in error before and after a change and warns
"The change put N mate(s) in error: '<mate>' (error 51, mate not satisfied). They no
longer hold; set it back, or edit or delete them (asm_mate_list)." (a mate newly in error,
with another code, or from warning to error); the change stands. `ErrorName` takes the
feature's type: for a mate a code outside `swFeatureErrorMate*` is not named by the enum,
51 reads "mate not satisfied", any other the bare code; used by `asm_mate_list`,
`asm_mate_add`'s error and warning, `feature_info`, `dimension_set`'s broken features and
`review`. The code noted in `docs/solidworks-api-notes.md`.
Test before: `AssemblyLiveTests.Setting_a_subassembly_rigid_names_the_mates_it_puts_in_error`
(the scenario's case: a subassembly with a floating child, made flexible, a top-level mate
moving the child, then rigid) failed on the code before: "no warning names the mate put in
error ('Coincident1', error 51 swSketchErrorExtRefFail); warnings: []". After: it passes,
with the live tests that call the changed code:
`Component_set_floats_hides_suppresses_and_returns_the_previous_state`,
`A_subassembly_is_solved_flexible_and_rigid_again_as_SOLIDWORKS_reads_it`,
`Configurations_are_chosen_at_insert_and_switched_by_component_set`,
`Mates_that_cant_hold_are_refused_rolled_back_and_nothing_moves`,
`Review_of_an_assembly_counts_the_floating_component_and_lists_a_suppressed_one`,
`ReferenceLiveTests.Review_reports_a_broken_shell_…_and_a_missing_material`,
`FeatureManagementLiveTests.Feature_delete_with_deleteEquations_removes_them_and_leaves_no_rebuild_error`:
8/8 pass. Unit tests 288/288; `check_repo` ok; no description changed. Made stricter: the
test asserts the mate in error by name, its code and name, the warning, no warning when
nothing breaks (made flexible, and flexible again), and the mate holding again.
Decided 2026-11-16 by the run, not the owner: the change stands with a warning, not
refused and rolled back, because the caller asked for it and setting it back is one call;
the warning says so.
Decided 2026-11-16 by the run, not the owner: a mate's 51 reads "mate not satisfied", not
the bare code, because the probe found it in both cases tried, and other codes outside
the enum's mate names stay bare; the type is passed by every caller that names a feature's
error (`feature_info`, `dimension_set`, `review`), not by `asm_mate_list` alone, because
`dimension_set` showed the same wrong name for a mate.
Local: on `sw2023`, in the commit that moves it here; in build `20261116-124830` (`ad5f6ba`). Upstream: not yet
decided.
Held in its scenario test (findings 2026-11-16, build 20261116-124830, point 5): setting a
flexible subassembly rigid while three top-level mates held its child elsewhere warned
"The change put 3 mate(s) in error: …", each mate named with error 51, "mate not
satisfied", and `asm_mate_list` read `inError: 3` with that name; flexible again, no
warning, `inError: 0`.

### S77 - A child of a flexible subassembly reads `fixed: true`, `constrained: "fully"` - CLOSED 2026-11-16

Sorted out of the entry of 2026-11-16, point 6. On build `20261116-080311`, with S62 (b):
a child of a flexible subassembly that floats in its own file, and that a mate had just
moved, reads `fixed: true, constrained: "fully"` in `asm_component_list allLevels` and in
the mate's `components`; the same while the subassembly was rigid. A caller cannot see
from the rows that the child can move. Reported `ok`. Bears on S62 (Closed).

Proposed: a probe first, what `IComponent2` reports for such a child (`IsFixed`,
`GetConstrainedStatus`, read in the subassembly's context and in the top assembly's), then
the row says the child can move when the subassembly is flexible. Local, the assembly
files. Not decided.
Decided 2026-11-16 by the run, not the owner: taken into the run of 2026-11-16, group 3, a
probe first; if no reading tells it, the item records the probe and stays Open, because it
is small and in group 2's files, so the next group, as the run's "How it runs" says.
Fixed in the run of 2026-11-16, group 3. Probe (a subassembly of a fixed cube A and a cube
B floating in its file, in a top assembly with a fixed cube C; rigid, flexible, flexible
with one top-level plane mate on B, with three): in the top assembly `IsFixed()` reads true
and `GetConstrainedStatus` 3 for A and B in every case, from `GetComponents(false)` and
from the subassembly's `GetChildren` alike; B in the subassembly's own document
(`GetModelDoc2`) reads false and 2 (under) in every case, top-level mates not seen there;
B's node in the top assembly's FeatureManager tree, under the subassembly's node, reads
"(-)" rigid, flexible and with one mate, and no mark (fully defined) with three; A's reads
"(f)". `Solving` of the subassembly reads 0 or 1 as set. Cause: the rows read `IsFixed` and
`GetConstrainedStatus` of the nested component, which SOLIDWORKS answers for the
subassembly's rigid body. Fix: a component under flexible subassemblies only (every parent
up to the top flexible) reads `fixed` and `constrained` from the mark on its node in the
top assembly's tree (`AssemblyService.FlexibleChildState`, the mark parsed by
`AsmMath.TreeMark`: "(f)" fixed, "(-)" under, "(+)" over, "(?)" no solution, none fully),
in every row built by `Info` (`asm_component_list`, a mate's `components`, `asm_insert`,
`asm_component_set`, `asm_component_move`, the pattern tools); a node not found keeps the
reading as before. Noted in `docs/solidworks-api-notes.md`.
Test before: `AssemblyLiveTests.A_child_of_a_flexible_subassembly_reads_as_able_to_move`
(the probe's assembly, flexible, B mated to C's top plane, then to its front and right
planes) failed on the code before: "the mate's row for '<sub>-1/<B>-1' reads fixed: True,
constrained: fully". After: it passes (B `fixed: false`, `under` after one mate in the
mate's row and the list's, `fully` and not fixed after three, at C's origin; A `fixed:
true`; B fixed while rigid), with the live tests that call the changed rows:
`A_subassembly_is_solved_flexible_and_rigid_again_as_SOLIDWORKS_reads_it`,
`Setting_a_subassembly_rigid_names_the_mates_it_puts_in_error`,
`Subassemblies_are_inserted_and_listed_with_their_children`,
`A_plane_of_a_component_inside_a_subassembly_takes_a_mate`,
`A_top_level_component_is_reached_by_its_name_when_a_nested_one_has_the_same_short_name`,
`A_limit_angle_mate_reads_fully_constrained_as_SOLIDWORKS_marks_it_in_the_tree`,
`Component_set_floats_hides_suppresses_and_returns_the_previous_state`: 8/8 pass. Unit
tests 289/289 (`A_tree_mark_reads_as_fixed_and_constrained` new); `check_repo` ok; no
description changed. Made stricter: no test read a nested component's `fixed` or
`constrained`; the new one asserts both for a floating and a fixed child, in the mate's row
and the list's, under-defined and fully held, and the rigid case.
Decided 2026-11-16 by the run, not the owner: the tree's mark, not the subassembly
document's `IsFixed` and `GetConstrainedStatus`, because the document does not see the
top-level mates (it read B under-defined while three mates held it) and the mark is what
SOLIDWORKS shows in the top assembly.
Decided 2026-11-16 by the run, not the owner: a child of a rigid subassembly keeps
`fixed: true`, `constrained: "fully"`, because in the top assembly it moves only with its
subassembly (the tree's "(-)" there is its own file's state); the parent's row says
`flexible: false`.
Decided 2026-11-16 by the run, not the owner: the rows only; `asm_mate_add`'s hint "both
sides are fixed" on a failed mate and `review`'s count of floating components (a child of
a flexible subassembly not counted) still read `IsFixed`, because they are not the rows
the item names and each is a change of its own, to an item if a session meets it.
Local: on `sw2023`, in the commit that moves it here; in build `20261116-124830` (`ad5f6ba`). Upstream: not yet
decided.
Held in its scenario test (findings 2026-11-16, build 20261116-124830, point 4): the
floating child of a flexible subassembly read `fixed: false`, `constrained: "under"` in
the mate's row after one mate, and `"fully"` after three, at the fixed cube's origin,
`asm_component_list allLevels` the same and its fixed sibling `fixed: true`; while rigid
it read `fixed: true`, `"fully"`, as decided.

### S78 - Mate results and origin references: `moved` short and incomplete, an origin needs a name and reads as a point - CLOSED 2026-11-16

Sorted out of the entry of 2026-11-16, points 7 and 3. On build `20261116-080311`, each
reported `ok`. (a) A mate moved a cube inside a subassembly and, through two earlier
mates, a top-level cube of the same short name; the result gave `moved:
["<cube>-2"]`, one short name, which is the top-level one's full name, and `components`
listed the nested cube and the fixed one but not the top-level one that moved (point 7;
bears on S70, Closed). The list is keyed by `Name2` before and after, which a top-level
and a nested component of the same short name may share. (b) `{component,
type:"origin"}` with no name was refused `INVALID_ARGUMENT: 'a' needs 'id', 'where', 'at'
or 'name'.`, one call more, though the server's instructions list `origin` as a reference
type and a component has one origin (point 3; bears on S68, Closed). (c) In a mate's row
(the result and `asm_mate_list`) an origin reads `kind: "point"` with `pointMm` only, so
it cannot be told from a sketch point (point 3).

Proposed: (a) `moved` gives full names (`<sub>-1/<cube>-2`) and every component that
moved, the components compared by their full path; (b) `type:"origin"` with no name takes
the component's origin, or the assembly's with no component; (c) an origin reads as one
in the row (its kind or its name). Local, the assembly files. Not decided.
Decided 2026-11-16 by the run, not the owner: taken into the run of 2026-11-16, group 3,
(a) to (c), because they are small and in group 2's files, so the next group, as the
run's "How it runs" says.
Fixed in the run of 2026-11-16, group 3, all three points.
(a) Cause: `moved` compared the ArrayData of the top-level components only
(`GetComponents(true)`), so a child of a flexible subassembly that the mate moved was never
looked at, and the one name listed was the top-level twin's, the same short name. Fix:
`asm_mate_add` reads every component at every level, by full name with its parent's,
before and after; a top-level one is listed when its placement changed, a nested one when
its placement in its parent changed (`AsmMath.Moved`, interop-free, unit-tested), so the
children a subassembly carries along are not listed besides it. `components` stays the
mated components, as its description says. Test
`A_mate_names_every_component_it_moved_by_its_full_name` (a subassembly of a fixed cube A
and a floating cube B, fixed in a top assembly with a fixed cube C and a top-level B held
on the nested B by three plane mates; flexible, the nested B mated to C's top plane) failed
on the code before: "moved: [<B>-1], expected [<sub>-1/<B>-1, <B>-1]". After: it passes,
`moved` equal to what `asm_component_list allLevels` reads as moved, A not listed.
(b) Cause: a reference needed `id`, `where`, `at` or `name` before its `type` was looked
at. Fix: `type:"origin"` alone takes the origin, of the component given or of the document
(`EntityResolver.OriginOf`, the origin feature found by its type, so in any UI language);
`{component, name:"origin"}` goes the same way. Test `An_origin_is_taken_by_its_type_alone`
(`{component, type:"origin"}` coincident with `{type:"origin"}`) failed before: "'a' needs
'id', 'where', 'at' or 'name'."; after: it passes, both resolved as `origin`, the
component's origin at [0, 0, 0].
(c) Cause: the mate entity of an origin is the point of the origin's sketch
(`ISketchPoint`, whose `GetSketch()` is the `OriginProfileFeature`), read as any point.
Fix: that entity reads `kind: "origin"` with the origin's name and its `pointMm`. Test
`A_mate_s_row_reads_an_origin_as_one` (a component's origin on another's vertex) failed
before: "the origin reads {component: <B>-1, kind: point, name: , id: }"; after: it passes,
in the mate's result and in `asm_mate_list`, the vertex still `point`.
After: the three tests and the live tests that call the changed code,
`Coincident_mates_take_the_assembly_s_origin_and_a_component_s_origin`,
`Mate_list_names_the_plane_face_edge_or_vertex_each_mate_takes`,
`A_top_level_component_is_reached_by_its_name_when_a_nested_one_has_the_same_short_name`,
`A_plane_of_a_component_inside_a_subassembly_takes_a_mate`,
`A_subassembly_is_solved_flexible_and_rigid_again_as_SOLIDWORKS_reads_it`,
`A_child_of_a_flexible_subassembly_reads_as_able_to_move`,
`Mates_that_cant_hold_are_refused_rolled_back_and_nothing_moves`,
`Plane_mates_coincident_and_distance_reproduce_the_4000_mm3_overlap`,
`Concentric_mate_puts_the_shaft_on_the_bore_axis_and_tangent_lays_one_on_the_plate`,
`Lock_mate_keeps_the_relative_position_of_two_components` and, for the origin by name in a
sketch, `Constrain_reads_a_relation_the_sketch_already_has_as_held_and_runs_the_next_steps`:
14/14 pass. Unit tests 290/290
(`Moved_lists_a_top_level_component_that_moved_and_a_nested_one_that_moved_in_its_parent`
new); `check_repo` ok; no description changed (the reference types already list
`origin`). Made stricter: no test read `moved` for a nested component, an origin by its
type, or an origin's row; the new ones assert `moved` against the component list's own
reading, and the origin's kind and name in both rows.
Decided 2026-11-16 by the run, not the owner: a nested component is in `moved` when it
moved in its subassembly, not when its subassembly carried it, because the subassembly's
own name already says that and listing every child of a moved subassembly would bury what
the mate did.
Decided 2026-11-16 by the run, not the owner: `components` stays the mated components and
`moved` lists the rest, because `components` is described as the mated ones and `moved` is
the list the item asks to be complete.
Decided 2026-11-16 by the run, not the owner: an origin reads `kind: "origin"` and the
origin's name both, because the kind tells it from a sketch point and the name is what the
other tools take back (`{component, name:"Origin"}`), as a plane's row gives its name.
Local: on `sw2023`, in the commit that moves it here; in build `20261116-124830` (`ad5f6ba`). Upstream: not yet
decided.
Held in its scenario test (findings 2026-11-16, build 20261116-124830, point 3): (b)
`{component, type:"origin"}` with no name on both sides accepted, each resolved as that
component's origin; (c) the rows, in the result and in `asm_mate_list`, `kind: "origin"`,
`name: "Origin"`, `pointMm`; (a) a mate on the nested cube of a flexible subassembly gave
`moved` with the nested cube's full name and the top-level cube of the same short name it
carried, the nested cube's sibling not listed, and the same for the two mates after it.

### S59 - Reference geometry that cannot be made - CLOSED 2026-11-16

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
Local: on `sw2023`, in the commit that moves it here; in build `20261116-205845`. Upstream: not yet
decided.
Held 2026-11-16: the scenario test of build `20261116-205845` (findings 2026-11-16,
point 1): (c) `ref_plane` `angle` 30° on `top` about `{name:"Line1", sketch:"Sketch1"}`, no
`type`, resolved "sketch segment Line1" and checked at 30°; `ref_axis` `two_points` through
two `{at:[x, y], sketch}` points, no `type`, along them. (a), (b) and (d) held 2026-11-14.

### S72 - A SOLIDWORKS process in a bad state: a material applied at 1 kg/m³ with a warning only, a linked property in metres, every equation refused - CLOSED 2026-11-17

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

Only the material point is this run's; the linked property in metres (S30) and the
equations refused (S58 (b)'s message, as designed) stay as they are.
Cause: `material_set` compared the density read with the database's and only warned
when they differed, so a material named and read back at 1 kg/m³ was reported `ok`.
Fix: the rule is `MaterialDensity.Judge` (interop-free, in the unit tests): within 0.5 %
the check "matches the database"; off by more than 1 % with no solid body of another
material and Mass Properties' "override mass" off (`IMassProperty2.GetOverrideOptions`,
`OverrideMass`), `VERIFY_FAILED`, the previous material put back (in the configuration
given) and read back, the message ending "Rolled back to '<prev>'." or "Rolled back: no
material, as before."; at 0.5 to 1.5 kg/m³ it adds "SOLIDWORKS can come to apply every
database material at 1 kg/m³, in a new part too, until it is restarted.", hint "Save the
open documents, restart SOLIDWORKS and repeat the call."; otherwise a warning naming why
(bodies with their own material, the mass overridden, within 1 %).
Test before: `MaterialDensityTests` (7 cases) did not compile, CS0103 "The name
'MaterialDensity' does not exist in the current context". After: unit tests 297 passed
(290 + 7). The bad state was met live on the fix's own first run, in a SOLIDWORKS
process that had run earlier work: the live tests that apply a database material were
refused with the new message, "the mass properties give 1 kg/m³, the database 7900
kg/m³ … until it is restarted. Rolled back to 'AISI 1020'." (or "no material, as
before"). After a restart of SOLIDWORKS, the live tests that call `material_set`, each by
`--filter-method`: 12 of 13 cases passed (`ConfigurationLiveTests` A_material_named_…
(2) and A_material_set_in_one_configuration_…, `M0LiveTests`
Status_gives_a_failed_call_…, `PropertiesLiveTests` Material_set_AISI_1020_…,
Material_set_rejects_unknown_names_…, A_material_library_that_does_not_parse_…,
Foreign_documents_…, `ReferenceLiveTests` Cost_of_the_machined_block_… and
Review_reports_…, `SimulationLiveTests` Two_bonded_halves_…); the 13th,
`PropertiesLiveTests` A_custom_material_is_listed_…, fails at `material_add`'s
byte-for-byte check of the library (line 67), before its `material_set`, and fails the
same on the code before the fix: the machine's custom library holds a second
classification with a material this server wrote, which the test's layout check does not
expect. `python tools/check_repo.py` ok; no tool description changed.
Made stricter: a density off by more than 1 % with no body material and no override was a
warning, now a refusal.
Decided 2026-11-16 by the run, not the owner: between 0.5 % and 1 % off it stays a
warning, as the match check was 0.5 % already, because the plan set the refusal at 1 % and
that is the smaller change.
Decided 2026-11-16 by the run, not the owner: "about 1 kg/m³" is 0.5 to 1.5 kg/m³,
because the bad state reads 1 and no database material is that light.
Decided 2026-11-16 by the run, not the owner: a body material or mass override that
cannot be read counts as present (a warning, not a refusal), because a refusal on an
unknown reading would refuse a material that may be right.
Decided 2026-11-16 by the run, not the owner: the rollback puts back the previous name as
read, a name SOLIDWORKS kept without applying it (S30) included, because that is the state
before the call.
Local: on `sw2023`, in the commit that moves it here; in build `20261117-012659`. Upstream: not yet
decided.
Held 2026-11-17 as far as the scenario test could try it: the scenario test of build
`20261117-012659` (findings 2026-11-17, point 6): `material_set` AISI 1020 read "density
7900 kg/m³ matches the database", PETG from the custom library at 1270 kg/m³ the same, no
warning and no false refusal. A process applying 1 kg/m³ was not met there, so the refusal
was not tried in it.
Decided 2026-11-17 by the run, not the owner: closed, because the refusal was met live by the
session that made the fix (the live tests applying a database material were refused at
1 kg/m³ in a process in the bad state and rolled back, above) and its rule is unit-tested
(`MaterialDensityTests`), and the scenario test showed no false refusal.

### S71 - A frequency study with no fixtures fails in the solver - CLOSED 2026-11-17

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
Cause: the server left the solver to SOLIDWORKS (`FrequencyStudyOptions.SolverType` reads
0, automatic, soft springs off), and SOLIDWORKS 2023 stops on a free body's six rigid-body
modes with code 24 within 2-3 s of solving.
Probe, 2026-11-16, the test's free-free study (default mesh, 8 modes), the option set
after the mesh: automatic, 24; Direct Sparse (1), 24; Direct Sparse with soft springs
(`UseSoftSpring2`), 24; FFEPlus (2), solved; automatic with soft springs, solved; FFEPlus
with soft springs, solved. Each that solved passed the test's oracle (six modes below
1 Hz, the first free bending mode within 2 % of 1293 Hz).
Fix: `sim_run` frequency with no fixtures sets `SolverType` FFEPlus, reads it back
(`VERIFY_FAILED` if it does not hold) and says "solver FFEPlus for the free body (read
back)" in its checks; a study with fixtures keeps SOLIDWORKS' choice. The probe is in
`docs/solidworks-api-notes.md` (§14, Frequency).
Test before: `SimulationLiveTests.Frequency_of_the_cantilever_column_…` failed at its
free-free study, "The solver did not finish: RunAnalysis returned 24
(swsRunAnalysisErrorAnalysisFailed)." After: it passes, with the FFEPlus check asserted
for the free study and absent for the fixed one; with it, `Buckling_of_the_fixed_free_…`,
`Sim_run_refuses_loads_on_a_frequency_study_…` and `Plots_of_kept_studies_…`: 4 of 4
passed. Unit tests 297 passed; `python tools/check_repo.py` ok; no tool description
changed.
Made stricter: the test asserts the solver check of the free study, and its absence with
fixtures.
Decided 2026-11-16 by the run, not the owner: FFEPlus, not soft springs, because soft
springs add stiffness to the model the results are read from, while FFEPlus solves the
model as it is; both are one setting.
Decided 2026-11-16 by the run, not the owner: only with no fixtures, because the fixed
study solves under SOLIDWORKS' own choice and the item is the free body.
Local: on `sw2023`, in the commit that moves it here; in build `20261117-012659`. Upstream: not yet
decided.
Held 2026-11-17: the scenario test of build `20261117-012659` (findings 2026-11-17,
point 4): `sim_run frequency` on a 40 mm AISI 1020 block, 8 modes, no fixtures, checked
"solver FFEPlus for the free body (read back)", solver status 0, six modes 0 to 0.0021 Hz
then 35 490.8 Hz, in 13.5 s; with the bottom face fixed, 4 modes from 13 380 Hz and no
solver check.

### S87 - `sketch_add`'s second merge reports the distance after the first and moves an end twice the gap - CLOSED 2026-11-17

Sorted out of the entry of 2026-11-16 (the scenario test of build `20261116-205845`), point
2. An open polyline typed to 4 decimals and an arc on its ends, in one call: `endsMerged: 2`,
one closed contour (S65 (e) held). Reported `ok`: the second merge read "0.000054 mm apart"
where the drawn gap was 0.000027; the first merge had moved the arc (its centre to y
10.000027), and the second moved the line's end 0.000054 from where it was typed
(`sketch_info`: y 14.330154, typed 14.3301). Within the 1e-4 tolerance, the geometry right;
the reported distance is the one after the earlier merge, and one end moved twice the gap.

Proposed: each merge reports the gap as drawn, and moves an end by at most that gap (the
second merge onto the point the first left, or both measured before either moves). Local,
small, in group 4's files: by the run's rule a later item of this run, taken into group 5.
Taken into the run of 2026-11-16, group 5 (`## Run 2026-11-16`).
Cause: `MergeEnds` measured each gap on the sketch as it stood, after the merges before
it, and left SOLIDWORKS' solver to pick which point moves. The arc merged onto one
polyline end moved as a whole, its other end then 0.000054 off the other polyline end,
and that merge pulled the polyline end onto it.
Fix: the ends are read once before any merge, and each gap reported and checked against
the 1e-4 mm tolerance is the one as drawn; before each merge the new end is set onto the
other with `ISketchPoint.SetCoords`, the arc bending and the polyline staying, so the merge
moves nothing; a merge that still moves the other end more than the drawn gap warns,
naming it. In `docs/solidworks-api-notes.md` (Merge).
Test before: `SketchEditLiveTests.Sketch_add_merges_an_arc_drawn_on_a_polyline_s_ends_into_one_closed_contour`,
extended: "Assert.EndsWith() Failure ... (Line3 end), 0.000054 mm apart, Expected end:
0.000027 mm apart"; with the report fixed alone, "the polyline's end typed at [34.3301,
12.5] reads [34.330154, 12.5]". After: it passes; with it,
`Sketch_add_lists_the_relations_and_merges_SOLIDWORKS_made`,
`Merge_and_coincident_join_two_line_ends_into_one_point`,
`Constrain_follows_a_point_through_a_merge`,
`Info_lists_segments_contours_frame_and_dimensions` and
`A_point_where_offset_arcs_centres_and_line_ends_sit_…`: 6 of 6 passed. Unit tests 297
passed; `python tools/check_repo.py` ok; no tool description changed.
Made stricter: the test asserts both merges' gaps and the polyline's ends within the
drawn gap of where they were typed.
Decided 2026-11-16 by the run, not the owner: the new end set onto the other before the
merge, not only both gaps measured before either merge, because measuring alone makes the
report true while the polyline's end still moves twice the gap; setting the end needs no
relation added and removed, as fixing the other end would.
Decided 2026-11-16 by the run, not the owner: a warning when a merge moves the other end
more than the drawn gap, because the result then says so instead of a silent move.
Local: on `sw2023`, in the commit that moves it here; in build `20261117-012659`. Upstream: not yet
decided.
Held 2026-11-17: the scenario test of build `20261117-012659` (findings 2026-11-17,
point 5): the polyline typed to 4 decimals and an arc centre [30, 10] r 5, −60° to 60°, in
one call: both merges read "0.000027 mm apart", the gap as drawn; `sketch_info` gave the
polyline's ends as typed (y 14.3301 and 5.6699), the arc taking the gap (centre x
29.999953, r 5). The same arc onto an older polyline in a later call: 0.000027 twice; an
arc 0.2 mm off not merged.

### S88 - `view_screenshot` fits a part small to an axis made through points, with no warning - CLOSED 2026-11-17

Sorted out of the entry of 2026-11-16 (the scenario test of build `20261116-205845`), point
5. A 40 mm block and an axis by `ref_axis two_points` at z 300: the default screenshot
showed the block at about a tenth of the image beside two near-invisible dots (the axis's 3D
sketch points); with `hideSketches:true` it filled the image. Reported `ok`, nothing in the
result saying the fit took in a sketch past the bodies, which the server checks for (S65
(d)'s fit by selection). As designed in S65 (d) ("an axis through coordinates still needs
`hideSketches:true`"), but a caller who wants the part gets a small one, the case that opened
(d).

Proposed: a warning when the fit takes in a shown sketch that reaches past the bodies,
naming it and `hideSketches`. Hiding the 3D sketch `ref_axis` makes for its points was left
by S65 (d)'s run for the next plan (it changes the reference tools) and is not taken here.
Local, small, in group 4's files: by the run's rule the warning is a later item of this run,
taken into group 5. Not decided.
Taken into the run of 2026-11-16, group 5 (`## Run 2026-11-16`).
Cause: S65 (d)'s fit selects the sketches shown, as the image shows them, and the result
said nothing when one of them reached past the bodies: the 3D sketch of an axis through
points is shown, so the fit took it in and left the part small, reported `ok`.
Fix (`ViewService.FitToWhatIsShown`, `FitReach`): when the fit is by selection, each sketch
selected is boxed in model space as the fit takes it in (its points but the centres of arcs
and ellipses, its line and arc ends, a 2D arc's circle at the axis extremes on its sweep);
one past the bodies' box by more than 10 % of its largest side is named in a warning: "The
fit takes in 3DSketch1, shown past the bodies, which leaves them small in the image;
hideSketches:true leaves sketches out of the image and the fit." Not with `hideSketches`,
the sketches toggle off or `zoomToFit:false`. The rule and the arc's extremes in an
interop-free file with unit tests. Probed live: a shallow arc's centre 180 mm out is not in
the fit (the block's share 0.2126, 0.2125 without the arc); in
`docs/solidworks-api-notes.md` (Zoom to fit). No tool description changed.
Test before: `VisualsLiveTests.A_fit_that_takes_in_a_shown_sketch_past_the_bodies_warns`,
a 40 mm block and an axis by `two_points` at z 300: "no warning of the fit taking in the
axis's sketch: []". After: it passes (the axis's sketch named, not with `hideSketches` nor
unfitted, none for the block alone nor for a shown arc 3 mm past it whose circle, r ≈ 200
mm, reaches far past; that case warned falsely while arc centres were counted, now fixed);
with it `Zoom_to_fit_takes_what_the_image_shows_and_axes_are_hidden`,
`A_section_cuts_the_image_and_is_removed_afterwards`,
`M0LiveTests.Screenshot_returns_a_png_and_restores_display_settings` and
`SimulationLiveTests.Plots_of_kept_studies_show_the_results_range_in_an_image`: 5 of 5
passed. Unit tests 299 of 299 (`FitReachTests`, 2 new); `python tools/check_repo.py` ok.
Made stricter: none (a warning added; the fit and the image as before).
Decided 2026-11-16 by the run, not the owner: a sketch warns when it reaches past the
bodies' box by more than 10 % of its largest side, not by any amount, because below that
the part's share of the image barely changes and the warning would read as a fault on a
right result (S84).
Decided 2026-11-16 by the run, not the owner: a sketch's box leaves out arc and ellipse
centres and takes a 2D arc's extremes on its sweep only, and gives a 3D arc, an ellipse or
a spline its points only, because the warning must not claim a reach the fit does not take
(a centre 180 mm out did not move the fit); it may fall short on a bulge, never past.
Decided 2026-11-16 by the run, not the owner: the warning only, no description change,
because `hideSketches`' text already says it leaves sketches out of the fit and the tool
list is at its byte budget.
Local: on `sw2023`, in the commit that moves it here; in build `20261117-012659`. Upstream: not yet
decided.
Held 2026-11-17: the scenario test of build `20261117-012659` (findings 2026-11-17,
point 3): a 40 mm block and an axis by `ref_axis two_points` at z 300: the default
screenshot warned "The fit takes in 3DSketch1, shown past the bodies, which leaves them
small in the image; hideSketches:true leaves sketches out of the image and the fit."; with
`hideSketches:true` the block filled the image, no warning.

### S91 - `weldment_member` cannot reach a one-size profile at `<standard>\<type>\<size>.sldlfp` - CLOSED 2026-11-18

Sorted out of the entry of 2026-11-18 (a custom weldment profile with one size), point 1;
that entry's other points, and the other entries of 2026-11-17 and 2026-11-18, were sorted
2026-11-18 (S92 to S102). SOLIDWORKS reads a profile of several sizes at `<standard>\<type>.sldlfp`
(sizes as its configurations) and a profile of one size only at
`<standard>\<type>\<size>.sldlfp` (the folder the type, the file name the size). The server
read only the first: a one-configuration file in the standard folder was inserted and
made a Weldment with no member, reported `VERIFY_FAILED` with a hint about the path.

Local: `e663d05` on `sw2023`. `WeldmentSpec.PickProfile` takes a type as a file of two or
more sizes or as a subfolder of size files; a size file is inserted with no configuration
named (with its own one, `"Default"`, no member was made in each of 3 runs, with `""` the
members in each of 4); a one-configuration file directly in the standard folder is refused before
anything changes, naming the layout that works; "No profile" lists both kinds of type.
Live test `WeldmentLiveTests.A_one_size_profile_is_read_from_its_type_folder_and_refused_directly_in_the_standard_folder`,
failing without the fix. In build `20261118-060615` (tag `build-20261118-060615` on
`ab34750`); scenario test not run yet.
Upstream: not yet decided.
Held 2026-11-18: the scenario test of build `20261118-060615` (findings 2026-11-18, point
1): a custom one-size profile laid out as `<standard>\<type>\<size>.sldlfp`, in a profile
folder already registered in SOLIDWORKS: `weldment_member` along a 100 mm line, `size` typed
`"20x40"`, read `profile` `<standard> / <type> / 20 x 40` with the size file as
`profilePath`, one member, 29 998.945 mm³, the same as that profile swept by hand; along a
closed 300 × 200 rectangle with mitered corners, 4 members, lengths 320 and 220, one body
each. A wrong type refused listing the subfolder type, a wrong size listing `20 x 40`. A copy
of the same file directly in the standard folder refused `INVALID_ARGUMENT` "has one
configuration ... would make no member", the hint naming `<standard>\<name>\<size>.sldlfp`,
no feature added. A shipped iso `square tube 40 x 40 x 4` in the same part still worked
(53 479.645 mm³ on 100 mm). One gap met with it is S102 (the refused file still listed as a
type).

### S61 - A STEP into an assembly - CLOSED 2026-11-21

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
in build `20261121-175345`. Upstream: not yet decided.
(a) not held 2026-11-21, back to not done, reported `ok` while wrong: the scenario test of
build `20261121-175345` (findings 2026-11-21, point 1). `asm_insert` of a STEP of a 20 mm
cube, each case a fresh copy of the STEP in its own folder: every result one component, "1
solid body (8000 mm³)", linked by 3D Interconnect, its part `<step folder>\<name>.step.SLDPRT`,
now listed `owned` (was `foreign`). Held, reopened with every component resolved: (a) the
STEP first, the assembly saved in the STEP's folder; (c) a part first, the STEP after,
`doc_save as` in the STEP's folder: "Also wrote '<step folder>\<name>.step.SLDPRT': the
assembly's save left it unwritten", the part clean, 2 components on reopen; (d) the STEP
after a subassembly and a part, `doc_save as` then `doc_save` in place, the assembly no
longer dirty again after the in-place save; (f) a part first, the assembly saved, the STEP
inserted, `doc_close {save:true}`: "Also wrote …". Not held where the assembly is saved in
another folder than the STEP: (e) the STEP after a subassembly into a saved assembly in
another folder, `doc_save` in place; (g) a part first, the STEP from a subfolder of the
assembly's folder, `doc_save as`; (h) as (g), with a second `doc_save` in place after the
part was on disk. In each the part was written beside the STEP and the save said "Also
wrote …", yet the reopened assembly read `missingReferences`
`<assembly folder>\<name>.step.sldprt`, the component `suppression: suppressed`, the
assembly `dirty: true` on open; the second in-place save of (h) did not help. The cause as
the check reads it: the assembly file stores the part as if it lay in the assembly's own
folder, so the cases that held are those where that folder is the STEP's; the fix's live
test saves the assembly in the STEP's folder and so cannot see it. Proposed by the session:
write the owned unwritten part before the assembly's save, so the assembly stores the path
the part has, or repoint the component after writing it; and read the reference back (a
reopen, or the saved file's dependencies) before saying the part was written. By the run's
rule (*Per group, the check*: a fix of this group not holding), (a) is fixed a third time
and group 5 built and checked again (`## Run 2026-11-16`).
Decided 2026-11-21 by the owner: (a) a third time taken into the run of 2026-11-21, group 1, as the check of build `20261121-175345` proposed: the part written before the assembly's save, repointed where SOLIDWORKS still stores the assembly's folder, the reference read back before "Also wrote" (`## Run 2026-11-21`).
(a) fixed a third time 2026-11-21 in the run of 2026-11-21 (group 1). Cause (live, 2023
SP3, probed): a 3D Interconnect component inserted after another component is not among
the assembly's references (`IModelDoc2.GetDependencies2` leaves it out), and the saved file
stores its part in the assembly's own folder, whether the part is written before the
assembly's save or after it; the cases that held were those where that folder is the
STEP's. `ReplaceComponents2` with the same file returns false, and a component added from
the written part while the linked one is still there shares its fault. What holds: the part
written, the linked component deleted, then `AddComponent5` of the written file at the old
transform: a plain reference, listed by `GetDocumentDependencies2`, resolved on reopen, the
part keeping its `MBimport` feature (its link to the STEP). Fix: `asm_insert` of a STEP
writes the part it made beside the STEP at once and replaces the linked component that way
(`AssemblyService.WriteAndRepoint`), its transform and fixed state kept, the new
component's document owned ("written by inserting …"), the message "its part '…' is
written beside the STEP"; a write refused (the path policy) or a delete that fails leaves
the linked component, with a warning that it may be missing on reopen; an add that fails
after the delete is `FEATURE_FAILED`, the assembly as it was. `doc_save` (every mode) and
`doc_close {save}` write an owned component part not on disk **before** the assembly's
save, no longer after; after it, a component file still not on disk is written or warned
of as before, and each part written is read back in the saved file's references
(`GetDocumentDependencies2`) and the open assembly's (`GetDependencies2`): one named in
neither is a warning naming both paths ("Wrote '…', but the saved assembly references it as
'…'"), and "Also wrote" names only the parts read back. Before: the new
`AssemblyLiveTests.A_step_from_a_subfolder_inserted_after_a_part_resolves_on_reopen_after_save_as`
(case (g)) and
`A_step_from_an_unrelated_folder_inserted_into_a_saved_assembly_resolves_on_reopen_after_save_in_place`
(case (e)) failed: the save said "Also wrote '…\steps\stepsub_….step.SLDPRT': the
assembly's save left it unwritten.", the reopened assembly read
`"MissingReferences":["…\stepsub_….step.sldprt"]` (the assembly's folder) and was dirty on
open. With only the part written before the save, both still failed, the save warning "…
references it as '(not at all)'". After: both pass, with the two STEP tests before them (`A_step_is_inserted_as_a_component_linked_…`, its part now written at the insert and the save warning of no missing file; `A_step_inserted_after_a_part_…`, case (c)), `Insert_puts_the_origin_at_originMm_…`, `Insert_refuses_unsaved_missing_and_foreign_files_…`, `Subassemblies_are_inserted_…`, `A_step_imported_as_an_assembly_…`, `Replace_swaps_the_file_…`, `Pack_and_go_copies_…`, `AssemblyStructureLiveTests.A_speedpak_configuration_…`, `M0LiveTests.Dirty_owned_documents_need_save_or_discard`, `Save_copy_writes_a_fresh_file_…` and `TopDownLiveTests.Saved_bodies_become_parts_…`, 14/14 live; unit tests 302/302; `check_repo` ok but for two absolute paths in the run's plan, not this step's; no description changed. Noted in
`docs/solidworks-api-notes.md` §8. Made stricter: the save's "Also wrote" is said only for
a part the saved assembly references at its path, else a warning naming both paths; the
STEP tests assert the part resolves on reopen with the assembly in another folder than the
STEP. Not looked into: assemblies saved in place reading `dirty: true` again (not this
step). Seen with it, not this item's: the first STEP test's `doc_save as`, after the STEP
inserted first, a refused insert and `asm_component_list`, warns "'…' was modified outside
this server since its last change (by the user?)", with the repoint disabled too; for the
next sort.
Decided 2026-11-21 by the run, not the owner: (1) the component is repointed at the insert,
not at the save, because the repoint is a delete and an add, which would lose any mate made
on the component since, and at the insert it has none; so the part is written at the
insert, before the assembly is saved, as only a part on disk can be added as a plain
component; (2) the save's read-back takes the open assembly's references beside the saved
file's, because the file's list leaves out a 3D Interconnect component that does resolve
(the first one, probed), and a warning on it would be false; (3) S61 moves whole to
Waiting, because (b) and (c) are closed and (a) was its last open point, and one number
heads one item. Local: on `sw2023`, in the commit that moves it here; built in `20261121-205309`.
Upstream: not yet decided.
Held 2026-11-21: the scenario test of build `20261121-205309` (findings 2026-11-21, point 1):
`asm_insert` of a STEP of a 20 mm cube in cases (a) to (h), each a fresh copy of the STEP in
its own folder, the three that failed on build `20261121-175345` ((e), (g), (h)) included:
the part written beside the STEP at the insert, listed `owned` and clean, the message "its
part '…' is written beside the STEP"; reopened after each save, every component
`resolved`, no `missingReferences`, the assembly not dirty on open; in (f) a coincident
mate added to the STEP component kept on reopen, 0 in error; no "Also wrote" and no
warning in any save. The "modified outside this server" warning the fix's test saw did not
appear in case (a). Seen with it, not faults (point 2): the STEP component is named
`<name>.step-2` even as the only instance (the repoint deletes the linked `-1`), which the
result names; on reopen the repointed components are listed before the others.


### S83 - `asm_mate_add` distance with `flip` that lands right warns in the words of a wrong side - CLOSED 2026-11-21

Sorted out of the entry of 2026-11-16 ("a part edited by rollback", point 5), on build
`20261116-124830`. `asm_mate_add` distance between two parallel planes, `alignment:
aligned`, 80: it landed on the far side (origin at -80, the part inserted at +80), with
the warning that says so (S50, Closed: held). Again with `flip: true` it landed right
(origin read back +80) but warned in the same words ("80 mm along its normal before, -80
mm after... If it should stay on the side it was on, delete the mate..."), which reads as
a second failure. The side check on `originMm` held.

Proposed: with `flip` asked, the warning says the side now differs from where the part
was inserted, as asked, not that it may be wrong. Local. Not decided.
Decided 2026-11-21 by the owner: taken into the run of 2026-11-21, group 1: with `flip` asked, the warning says the side differs from the insert's, as asked (`## Run 2026-11-21`).
Fixed 2026-11-21 in the run of 2026-11-21 (group 1). Cause (live, 2023 SP3, reproduced
by hand first): the side check (`AsmMateIntent.Side`, S50) compares the component's side of
b just before the mate with its side after, and warned of a crossing in one wording
whatever `flip` was. In the finding's sequence the first mate (no flip) took the part from
+80 to -80; the mate deleted, the part stayed at -80, so the flipped mate's "before" was
-80 and its move back to +80, the insert's side, read as a crossing worded as a fault.
Fix: `Side` takes `flip`; with it asked, a crossing warns "… on the other side of b from
where it was before the mate, as flip asks: … before, … after. If it should have stayed,
add the mate again without flip."; without `flip` the warning is as before. Before: the
new `AssemblyLiveTests.A_distance_mate_with_flip_that_crosses_says_so_as_asked` (a cube
inserted at x +80, distance 80 `aligned` to the fixed cube's right plane, landing at -80;
the mate deleted, added again with `flip:true`, landing at +80) failed ("Assert.Contains()
Failure: Sub-string not found. String: "The distance mate put a (plane 'Right Plane' of
fl"··· Not found: "as flip asks""). After: it passes, with
`A_distance_mate_says_which_side_the_component_landed_on` and
`Plane_mates_coincident_and_distance_reproduce_the_4000_mm3_overlap`, 3/3 live; unit tests
302/302 (`AssemblyMathTests` asserts both wordings); `check_repo` ok; no description
changed. Made stricter: the unit test asserts the no-flip wording too. Decided 2026-11-21
by the run, not the owner: the flipped warning says "from where it was before the mate",
not "from where it was inserted", because the check compares with the position just before
the mate, which in the finding's own case was not the insert's (the insert's side is where
the flipped mate brought it back to); it stays a warning, as the owner's decision words it.
Local: on `sw2023`, in the commit that moves it here; built in `20261121-205309`. Upstream: not yet
decided.
Held 2026-11-21: the scenario test of build `20261121-205309` (findings 2026-11-21, point 3):
two 20 mm cubes, the second inserted at x +80; distance 80 `aligned` between their right
planes landed at x -80 with the old wording; the mate deleted and added again with
`flip:true`, it landed at +80, warning "…on the other side of b … from where it was before
the mate, as flip asks: 80 mm along its normal before, -80 mm after. If it should have
stayed, add the mate again without flip."


### S107 - `asm_insert {doc}` warns it re-activated the assembly, which reads as if the target changed - CLOSED 2026-11-21

Sorted out of the entry of 2026-11-18 ("an edit to a printed plate and a new flat part",
point 3), on build `20261118-060615`. After `doc_new` of a part, `asm_insert
{doc:<assembly>}` placed it and warned "Re-activated '<assembly>' (the active document was
'<part>')"; the next `asm_mate_add` calls without `doc` were refused `WRONG_DOC_TYPE`, the
target still the new part. Right by the rules (the target is set by `doc_new` and
`doc_activate`, not by `doc`), but three calls lost; one `doc_activate` fixed it.

Proposed by the session: the warning adds "the target is still '<part>'". Local. Not
decided.
Decided 2026-11-21 by the owner: taken into the run of 2026-11-21, group 1: the warning adds that the target is still the other document (`## Run 2026-11-21`).
Fixed 2026-11-21 in the run of 2026-11-21 (group 1). Cause: the re-activation warning of
a write (`SwContext.Bind`) named the document it brought back and the one that was active,
and nothing of the target, which `doc` does not move; so it read as if the target had
changed. Fix: when the call has `doc` and the session's target is another document, the
warning ends "; the target is still '<target>', doc_activate to change it" (no `doc`, or
`doc` the target itself: as before). Before: the new
`AssemblyLiveTests.Insert_with_doc_after_doc_new_of_a_part_says_the_target_is_still_that_part`
(`doc_new` of a part, `asm_insert {doc:<assembly>}`) failed ("Assert.Contains() Failure:
Filter not matched in collection. Collection: ["Re-activated 'asm_tgt_eff766' (the active
document"···]"). After: it passes, with
`Insert_puts_the_origin_at_originMm_not_the_box_center_and_reports_the_auto_fix` and
`FeatureManagementLiveTests.The_servers_own_failed_kept_and_rolled_back_changes_do_not_read_as_the_users`
(the other test asserting "Re-activated"), 3/3 live; unit tests 302/302; `check_repo` ok;
no description changed. Made stricter: none (a new assertion on a warning that was not
asserted before). Decided 2026-11-21 by the run, not the owner: the target is named
whatever it is (an assembly or a part), as the plan says, and only when it differs from
the document `doc` named, because when they are the same the target did not stay anywhere
else. Local: on `sw2023`, in the commit that moves it here; built in `20261121-205309`. Upstream:
not yet decided.
Held 2026-11-21: the scenario test of build `20261121-205309` (findings 2026-11-21, point 6):
`doc_new` of a part, then `asm_insert {doc:<assembly>}`: warning "Re-activated
'<assembly>' (the active document was '<part>'; the target is still '<part>', doc_activate
to change it)."


### S110 - `dimension_set` of a limit mate's nominal outside its limits is refused without naming them - CLOSED 2026-11-21

Sorted out of the entry of 2026-11-18 ("a shared global variable changed in two parts",
point 6), on build `20261118-060615`. `dimension_set` of a limit mate's nominal to a new
minimum was refused (`INVALID_ARGUMENT`, "rejected 15 mm ... as invalid") until
`dimension_tolerance_set` widened the range (type `limit`, lower -23.5, read back as
`limits: [15, 72.5]`). Not reported `ok`.

Proposed by the session: the hint says a limit mate's nominal must lie inside its limits,
and that `dimension_tolerance_set` moves them. Local. Not decided.
Decided 2026-11-21 by the owner: taken into the run of 2026-11-21, group 1: the refusal names the limits and `dimension_tolerance_set` (`## Run 2026-11-21`).
Fixed 2026-11-21 in the run of 2026-11-21 (group 1). Cause: SOLIDWORKS answers a limit
mate's nominal outside its limits with `swSetValue_InvalidValue`, which `dimension_set`
turned into the generic "rejected 15 mm ... as invalid. Use a value that fits the
geometry." Fix: on that status in an assembly, the mate the dimension belongs to (the name's
second segment) is read for its limits (`IDistanceMateFeatureData.MinimumDistance` and
`MaximumDistance`, or the angle mate's `MinimumAngle` and `MaximumAngle`); a value outside
them is refused `INVALID_ARGUMENT` naming the limits ("lies outside its limits 20 to 72.5
mm") and, in the hint, `dimension_tolerance_set` (type `limit`, deviations from the nominal)
as the tool that moves them; the previous values are restored as before. A value inside
the limits, or a mate that reads no limits, keeps the old message. `dimension_tolerance_set`
is the right tool: the new test widens a limit mate to 5..72.5 with it (read back as
`limits: [5, 72.5]`) and the 15 then sets. Before: the new
`AssemblyLiveTests.Dimension_set_of_a_limit_mates_nominal_outside_its_limits_names_the_limits_and_the_tool_that_moves_them`
(limit mate 20..72.5 starting at 30, nominal set to 15) failed ("the limits 20 and 72.5 are
not named: SOLIDWORKS rejected 15 mm for 'D1@LimitDistance1' as invalid. Nothing was
changed. Use a value that fits the geometry."). After: it passes, with the live tests of
`dimension_set` (`AssemblyLiveTests.Dimension_set_*` 4/4 and
`FeatureManagementLiveTests.Dimension_set_*` 3/3, after a SOLIDWORKS restart: before it, the
two that use `equation_set` met S92's refused-equation state) and
`Limit_distance_and_symmetric_mates_hold_their_geometry_and_read_back` 1/1; unit tests
302/302; `check_repo` ok; no description changed. The angle limit mate's branch is written
the same way but has no test of its own. Made stricter: none (a refusal that said less now
says more; no case accepted before is refused). Decided 2026-11-21 by the run, not the
owner: the limits are read only after SOLIDWORKS rejects the value, not before the call,
because the refusal is then added to a check that already holds and a limit read that is
wrong cannot refuse a value SOLIDWORKS accepts. Local: on `sw2023`, in the commit that moves
it here; built in `20261121-205309`. Upstream: not yet decided.
Held 2026-11-21: the scenario test of build `20261121-205309` (findings 2026-11-21, point 4):
a limit distance mate 20..72.5 at 30: `dimension_set` 15 refused `INVALID_ARGUMENT` "is a
limit mate's nominal: 15 mm lies outside its limits 20 to 72.5 mm. Nothing was changed.",
the hint naming `dimension_tolerance_set` (type limit); following it, upper 42.5 lower -15
read back `limits: [15, 72.5]`, then 15 and 50 set, 80 refused naming "15 to 72.5". The
limits are not in `asm_mate_list` (point 5): S111.

### S112 - S92 (a) and (b): the equation tools read S58 (b)'s state from the document's globals and name the restart - CLOSED 2026-11-22

S92 (a) and (b), split out of S92 (which stays Open for (c) and (d)) in the run of
2026-11-21, group 2. (b) `equation_list` read every global set to a number cut at its
decimal point (`60.4mm` as 60) and every derived row as 0, reported `ok`, in the state that
refuses every equation until SOLIDWORKS is restarted; (a) in that state `equation_set`'s
`VERIFY_FAILED` ("kept the previous equation ... (a value the dimension can't have, such as
0?)", "evaluates to 0") blamed the expression. Cause: nothing compared a global's own
number with the value SOLIDWORKS read back. Fix: a new interop-free
`EquationReadback.Misreads` (`src/SolidWorksMcp.Core/Services/EquationReadback.cs`, linked
into the unit tests) takes each global whose right-hand side is a plain number in a known
unit and compares it with the value read, by the rule `equation_set`'s tier B already used
(SI or document units; `Literal`, `UnitFactor`, `Agrees`, `LiteralAgrees` moved there,
`EquationText` and `PropertiesService` forwarding). (b) `equation_list` warns on a
difference, naming each global as read ("SOLIDWORKS reads "g" = 60.4mm as 60: ... the sign
of the state in which SOLIDWORKS refuses or misreads every equation, in any document, until
it is restarted. The values listed are not to be trusted ... restart SOLIDWORKS before any
equation_set"); `equation_set` reads the sign before its first write and after its rebuild,
and warns the same on a call whose checks passed. (a) While the sign shows, its
`VERIFY_FAILED` names the state ("the expression is likely not at fault") with the hint
"Save the open documents, restart SOLIDWORKS and repeat the call; the equation needs no
change" in place of "Check the expression ...", and "kept the previous equation" drops its
guess at the value; an `Add2` refusal says the same (not for a function's name). Before:
`EquationReadbackTests` (8 tests) did not build, "error CS2001: Source file
...\Services\EquationReadback.cs could not be found" (the function did not exist). After:
unit tests 310/310 (the 8 new among them); live, the new
`PropertiesLiveTests.Globals_with_decimals_read_back_as_written_and_carry_no_warning`
(`g` = 60.4mm, `k` = 2.5mm, `s` = 0.5mm, `h` = "g" / 2 and a depth from `k`: no warning
from `equation_set` or `equation_list`, the values 60.4, 2.5, 0.5, 30.2, 20) with every
live test that calls `equation_list` or `equation_set` (the eleven equation tests of
`PropertiesLiveTests` with `A_refused_plain_global_points_to_a_restart`, four of
`FeatureManagementLiveTests`, the mate-angle one of `AssemblyLiveTests`), 17/17;
`PropertiesOfflineTests` 22/22; `check_repo` ok; no description changed. No live case of
the state: S92 (c)'s probe did not bring it on, so the warning and the refusal's words are
proved by the unit tests only, and the scenario test reads the good state clean.
Made stricter: `equation_list` and `equation_set` warn where they were silent; nothing
that passed is refused.
Decided 2026-11-21 by the run, not the owner: (a) and (b) moved to Waiting as a new item
split out of S92, not as a second S92 heading, because one number heads one item
(`board.py --check` fails a second one) and the format splits an item that way.
Decided 2026-11-21 by the run, not the owner: only a global set to a plain number in a known
unit is judged, not a row defined from others, because only its expected value is known
without evaluating SOLIDWORKS' expression; a derived row read 0 is named in the warning's
words instead.
Decided 2026-11-21 by the run, not the owner: the sign is a warning, and `equation_set`
writes on, because the plan asks for a warning and a refusal before the write could not be
told from a document whose globals were edited outside the server's rule.
Decided 2026-11-21 by the run, not the owner: an `Add2` refusal (`FEATURE_FAILED`) while the
sign shows names the state too, beyond the plan's `VERIFY_FAILED`, because its hint blamed
the syntax on the same evidence; a refused function name keeps its own reason.
Local: on `sw2023`, `f2d3dc2`; in build `20261122-002030`. Upstream: not yet
decided.
Held 2026-11-22: the scenario test of build `20261122-002030` (findings 2026-11-22, point 1),
in the good state: on a 60 × 40 × 10 block, one `equation_set` with `g` = 60.4mm, `k` =
2.5mm, `s` = 0.5mm, `h` = "g" / 2 and the boss depth = "k" * 4 + "s" read back 60.4, 2.5,
0.5, 30.2, 10.5, +1200 mm³, no warning, and `equation_list` the same values, no warning; on
a part with ten globals and thirteen linked dimensions, `equation_list` clean and one batch
of the ten globals to new values and back (173 487.816 then 108 553.103 mm³, one body), no
warning. The restart state did not occur, so the warning and the refusal's words in that
state stay proved by the unit tests only.
Checked 2026-11-22 by the run's check (build 20261122-030022, point 1): the restart message
seen live on a refused plain global. Point 5 of the same entry found the sign missing where
the globals are whole numbers: S118.

### S113 - S79 (b): `doc_undo` refuses after a server change that left no SOLIDWORKS undo step, naming it and the step it would take back - CLOSED 2026-11-22

S79 (b), split out of S79 (which stays Open for (a)) in the run of 2026-11-21, group 2.
`doc_undo` right after an `equation_set` that linked a variable to a 10 × 10 × 5 boss's
depth took back the boss (`volumeDeltaMm3: -500`) and left the equation, reported `ok`.
Cause: SOLIDWORKS keeps no undo step for some writes, so `EditUndo2` takes back the step
before them, and the server counted them as steps of its own. Probed live on 2023 SP3
(a boss, the call, `doc_undo`): no undo step after `equation_set`, `equation_delete`,
`prop_set`, `prop_delete`, `feature_rename` and `appearance_set` (each took back the boss
or the `dimension_set` before it); an undo step of its own after `material_set`,
`dimension_set` and `feature_suppress_set`; `config_add` gave "Nothing changed". Fix: a
write that leaves no undo step says so with `SwContext.NoUndoStep(change)` (replacing
`FeatureManagementService`'s own helper, which only kept the count for `feature_rollback`,
`doc_rebuild` and `doc_units_set`); `AfterWrite` then does not count it and, for a change
(all of them but `doc_rebuild`), records the tool in the document's `NoUndoChanges` until
the next counted write, which records itself as `LastUndoStep`. `doc_undo` refuses while
any is on record (`NOT_SUPPORTED`): "The server's last change in 'X' (equation_set) left no
SOLIDWORKS undo step: undo would take back the step before it instead, the server's
feature_extrude, and leave the equation_set in place.", the hint naming the change's own
tool, `feature_delete` and `dimension_set`. The description says it (trimmed, 400 to 398
bytes). The undo that does run already names the features it removes
(`Undo_after_a_fillet_removes_it`, and the probe's `removed: ["Boss-Extrude3"]`).
Before: the new `FeatureManagementLiveTests.Undo_refuses_after_a_change_that_left_no_undo_step`
failed, "Assert.Throws() Failure: No exception was thrown" (the undo ran). After: it
passes (the equation, then `prop_set` and `feature_rename` refused with the boss kept, then
a suppress undone again); with every `doc_undo`, `feature_rollback`, `doc_rebuild`,
`doc_units_set`, `feature_rename` and equation-on-delete test of
`FeatureManagementLiveTests`, the custom property, equation and appearance tests of
`PropertiesLiveTests` and the two `doc_undo` tests of `DrawingLiveTests`, 36/36; the undo
tests and `ContractTests.Tool_catalog_follows_the_conventions` again on the final code,
6/6; unit tests 310/310; `--check-catalog` ok; `check_repo` ok.
Made stricter: `doc_undo` refuses after those writes where it took back the wrong step;
nothing else that passed is refused.
Decided 2026-11-21 by the run, not the owner: the refusal holds until the next write that
is an undo step, also after the change is set back with its own tool (which is itself no
undo step), because the server cannot tell what `EditUndo2` would take back then either;
the hint points to `feature_delete` and `dimension_set` for the step before.
Decided 2026-11-21 by the run, not the owner: `feature_rollback` and `doc_units_set` refuse
an undo after them too, `doc_rebuild` does not, because the first two change what the user
would expect an undo to take back and the rebuild changes nothing.
Decided 2026-11-21 by the run, not the owner: (b) moved to Waiting as a new item split out
of S79, as S112 was out of S92, because one number heads one item.
Local: on `sw2023`, `9c14d93`; in build `20261122-002030`. Upstream: not yet
decided.
Held 2026-11-22: the scenario test of build `20261122-002030` (findings 2026-11-22, point 2),
for its case: on a block, `doc_undo` after `equation_set` refused `NOT_SUPPORTED` naming
`equation_set` and the step it would take back, the server's `feature_extrude`; after
`prop_set` refused the same, naming `prop_set`; after `feature_rename` refused; after
`dimension_set` 5 → 6 it ran (−78.54 mm³), after `feature_suppress_set` it ran (+392.699
mm³); a force `doc_rebuild` on a part opened read-only, then `doc_undo`: `NOT_OWNED` (the
rebuild not counted); a drawing refused as before. Points 3 and 4 went to S114: the
refusal's words after an undo (the server's own step called not the server's), a
component's dimension set from an assembly answered by the generic `VERIFY_FAILED` in place
of the refusal, and an undo that ran returning only `{"steps":1}`.

### S95 - `dimension_list` marks a dimension an equation drives `driven: true`, which its description gives as a reference dimension - CLOSED 2026-11-22

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
Decided 2026-11-21 by the owner: taken into the run of 2026-11-21, group 2 (`## Run 2026-11-21`).
Decided 2026-11-21 in the plan, not the owner: fields, `equation` given and `driven` only for a reference dimension, not the description's wording, because the tool list is at its byte budget and a field in a result costs none of it.
Cause: SOLIDWORKS gives `swDimensionDriven` for a reference dimension and for one an equation
drives alike, and `dimension_list` took `DrivenState` alone as the flag. Fix: a new
`FeatureManagementLogic.WithEquation` (`FeatureManagementService.cs`) matches each listed
dimension with the document's active equations by the rule `dimension_set` refuses by
(`EquationOn`); a match gives the row `equation` (the equation's text, a new field of
`DimensionInfo`) and no `driven`, any other row stays as it was, so `driven: true` is left to
a dimension no equation drives, a reference one. The description is unchanged (it already
says the flag is for reference dimensions). Before: the new
`FeatureManagementLiveTests.Dimension_list_gives_the_equation_of_a_dimension_it_drives_and_marks_only_a_reference_dimension_driven`
(an equation-driven width and a reference height) failed, "Assert.Null() Failure: Value of
type 'Nullable<bool>' has a value, Expected: null, Actual: True". After: it passes, with the
unit test `A_listed_dimension_an_equation_drives_gives_the_equation_and_no_driven_flag`, the
two other `dimension_list` live tests, `Dimension_set_refuses_bad_values_unknown_names_and_an_open_sketch`
and the unit test of `EquationOn` (6/6), then the mate-angle equation test and the limit-mate
test of `AssemblyLiveTests` (2/2); `SolidWorksMcp.UnitTests` 310/310 (the new offline fact is
in the live tests' project); `check_repo` ok; no description changed, so no catalog check.
Made stricter: nothing is refused; `dimension_list` reports `driven` for fewer rows (not for
one an equation drives) and `equation` for those.
Decided 2026-11-21 by the run, not the owner: the reference dimension in the test is made with
VBA (`DrivenState = 1`, the setter a user's "driven" checkbox calls), because the server
refuses to add one (an over-defining dimension is removed) and the live tests hold no interop.
Decided 2026-11-21 by the run, not the owner: only `dimension_list` changes, not the shared
`DimensionInfo` of other tools' results (`driven` in `dimension_add`, feature results), because
the item names that list and those results come from a dimension just made or set.
Local: on `sw2023`, `929158d`; in build `20261122-002030`. Upstream: not yet
decided.
Held 2026-11-22: the scenario test of build `20261122-002030` (findings 2026-11-22, point 5):
on a part with ten globals, `dimension_list` gave `equation` (the equation's text) and no
`driven` for each of the 14 dimensions an equation drives, the five others (three Hole
Wizard values, two pattern counts, a fillet) plain; on a block the equation-driven depth
the same. A true reference dimension was not tested live (`vbaEnabled` false, and the
server makes none); the fix's live test covers it.

### S116 - S92 (d): `sw_status` gives SOLIDWORKS' process start time, and the restart messages name it - CLOSED 2026-11-22

S92 (d), split out of S92 (which stays Open for (c)) in the run of 2026-11-21, group 3.
Whether SOLIDWORKS had been restarted was asked, not read: a session was told it had been
when it had not (its start time, read outside the server, was 44 min before the first
refusal), and closing the window left the process alive, most likely held by the COM
references of the other server processes. Cause: `sw_status` gave the pid only (`sw_connect`
gave `processStarted` since S60 (c)). Fix: `sw_status` gives `solidWorksStarted` (local
time, `yyyy-MM-dd HH:mm:ss`, null where the process can't be read) beside `solidWorksPid`,
in the partial result too (a blocked or busy SOLIDWORKS). The restart message of S58 (b)
and S92 (a), (b) names it where it is read: the `equation_set` refusal ("though the
equation is valid ... restarted") and the `VERIFY_FAILED` of the state say "The SOLIDWORKS
process started <time>; after a restart sw_status solidWorksStarted reads a later time",
and `equation_list`'s and `equation_set`'s warning of the state the same. Their hint
(`EquationReadback.RestartHintFor`) adds "Closing the window can leave the process alive
under the references of the server processes sw_status otherServerPids lists (<pids>): if
solidWorksStarted did not change, it was not restarted." Files: `SessionService.cs`,
`EquationReadback.cs`, `PropertiesService.cs`. No description changed (a field in a result
costs none of the catalog's budget).
Before: the new `M0LiveTests.Status_gives_the_start_time_of_the_SOLIDWORKS_process_beside_its_pid`
failed, "Assert.Equal() Failure: Strings differ; Expected: "2026-11-22 00:38:47"; Actual:
null" (the process's own start time against `sw_status`'s). After: it passes, 1/1; with
`Status_names_the_commit_the_server_was_built_from`,
`Connect_says_whether_it_found_the_same_SOLIDWORKS_process_as_before`,
`PropertiesLiveTests.A_refused_plain_global_points_to_a_restart` (extended: the message's
time and the hint's pids), `Equation_errors_are_typed_and_bare_numbers_warn`,
`Equation_delete_takes_a_list_checked_before_anything_goes` and
`A_global_variable_drives_the_extrude_depth`, 7/7; unit tests 323/323 (new in
`EquationReadbackTests`: the message names the start time, the hint the window and the
pids, the clause is left out where the time can't be read; the state itself is S92 (c),
not reproduced, so the message is proved by these tests alone); `check_repo` ok.
Made stricter: nothing that passed is refused; a message and a hint say more.
Decided 2026-11-22 by the run, not the owner: the start time is named in the restart
messages of the equation tools only (S58 (b), S92 (a), (b)), not in `sw_connect`'s
`NOT_CONNECTED` hints, because the plan names S58 (b)'s message and `sw_connect` already
gives `processStarted`.
Decided 2026-11-22 by the run, not the owner: no tool that exits SOLIDWORKS and no "never
end it yourself" in the hint, because S92 (d)'s plan decided ending it is the owner's
call; the hint says to read `solidWorksStarted` again, and a process not restarted is for
the owner to end.
Local: on `sw2023`, `a72b20f`; in build `20261122-030022`. Upstream: not yet
decided.
Held 2026-11-22: the scenario test of build `20261122-030022` (findings 2026-11-22, point 1),
in the state too: `sw_status` gave `solidWorksStarted` "2026-11-22 03:03:15", the process's
own creation time read outside the server to the second, beside its pid; later in the
session, with S58 (b)'s state on, a refused plain global on a new part (`"g" = 60.4mm`, then
`"m" = 6mm`) answered `FEATURE_FAILED` "... though the equation is valid ... The SOLIDWORKS
process started 2026-11-22 03:03:15; after a restart sw_status solidWorksStarted reads a
later time", the hint naming the other server process `otherServerPids` listed. The start
time and the restart message are now seen live, on the `Add2` path.

### S114 - `doc_undo` after S113: the server's own step called not the server's, a component's dimension answered "Nothing changed", an undo that names nothing - CLOSED 2026-11-22

Sorted out of the entry of 2026-11-22 (the scenario test of build `20261122-002030`), points
3 and 4, on S113's fix (Closed). (a) After a `doc_undo` had taken back the server's
`dimension_set`, the refusal after `feature_rename` said "undo would take back a step before
it that isn't the server's", though the step it would take back is the server's own
`feature_extrude`: after an undo the server no longer knows which of its steps is next. The
refusal itself was right. (b) In a two-part assembly opened read-only, `dimension_set`
`D1@Boss-Extrude1@<component>-1.Part` 60 → 80 (+6283.185 mm³), then `doc_undo`:
`VERIFY_FAILED` "Nothing changed: SOLIDWORKS had nothing to undo, or the last step can't be
undone. Hint: Check the model with feature_list."; the length stayed 80. The same with a
mate suppressed first (a suppress alone is undone there): the mate still suppressed, the
length 80. So a component's dimension set from an assembly leaves no undo step the
assembly's undo reaches, the server counts it as one, and the answer is the generic
"Nothing changed" in place of S113's refusal naming `dimension_set`. Nothing was lost; not
reported `ok`. (c) The undo that ran (a mate's suppress taken back) returned only
`{"steps":1}`, without naming the feature restored or removed, where `doc_undo`'s
description promises it.

Local, small, in group 2's files (`doc_undo` and `SwContext`'s undo accounting).
Decided 2026-11-22 by the run, not the owner: (a) after an undo the refusal says the step it
would take back "may be" another's, or the server tracks which of its steps is next; (b) a
component's `dimension_set` from an assembly counts as a change that left no undo step there,
so `doc_undo` refuses naming `dimension_set`, as S113 does; (c) an undo that runs names the
feature restored or removed. Test to start from: (b)'s case, a two-part assembly, a
component's dimension set from it, `doc_undo` refused naming `dimension_set` (fails now:
`VERIFY_FAILED`); (c) a suppress then `doc_undo` names the feature; (a) the refusal's words
after an undo. Default model.
Decided 2026-11-22 by the run, not the owner: taken into group 3, by the run's rule "a new
point, small and in the group's files, a later step of this run ... taken into the next
group", because it is a small change to group 2's undo accounting, found by group 2's check.
Cause: (a) the document's entry kept only the tool of the last counted write
(`LastUndoStep`), cleared by the undo's own `AfterWrite`, so after an undo nothing named the
step next; (b) `dimension_set` counted a component's dimension set from an assembly as an
undo step of the assembly, though the change lands in the component's own document; (c)
`doc_undo` compared features by id only, and a suppress or a dimension removes none.
Fix: (a) the entry keeps the tools of the server's counted writes in order (`UndoSteps`, the
last 64), `doc_undo` takes off the steps it undid and marks its accounting done
(`SwContext.UndoRecorded`, which `AfterWrite` then leaves); the refusal names the last one
("the step before it, the server's feature_extrude"), "which may not be the server's" when
none is kept. (b) `dimension_set` in an assembly calls `NoUndoStep()` when the call changed
another document's update stamp (`WarnChangedOthers` returns it), so `doc_undo` refuses
naming `dimension_set`; in an assembly the refusal reads "the assembly's undo would leave
the dimension_set in place, and may take back the step before it, the server's … or
nothing", as `EditUndo2` was seen to take back nothing there. (c) the result gives
`suppressed`, `unsuppressed` (feature names, a mate's included) and `dimensions` (the
dimensions whose value it changed), and these count against "Nothing changed". The
description says it, trimmed to stay in budget (398 to 397 bytes).
Before, with the result fields added and still empty: the new
`AssemblyLiveTests.Undo_in_an_assembly_names_a_mate_unsuppressed_and_refuses_after_a_components_dimension`
failed, "Assert.Equal() Failure: Collections differ; Expected: ["Parallel1"]; Actual: []",
and the new `FeatureManagementLiveTests.Undo_names_what_it_took_back_and_after_an_undo_the_server_step_next`
"Expected: ["Boss-Extrude1"]; Actual: []"; with (c) fixed, the first failed "VERIFY_FAILED:
Nothing changed: SOLIDWORKS had nothing to undo, or the last step can't be undone." and the
second "Assert.Contains() Failure: Sub-string not found ... Not found: "the server's
feature_extrude"". After: both pass, 2/2; with every `doc_undo`, `dimension_set`,
`feature_rollback`, `doc_rebuild`, `doc_units_set` and `feature_rename` test they share
the code with, the assembly `dimension_set` tests (S49, S74, S110), the stamp tests
(`The_servers_own_failed_kept_and_rolled_back_changes_do_not_read_as_the_users`,
`An_assembly_adopted_after_the_server_saved_its_parts_does_not_read_as_changed_by_the_user`),
the custom property, equation and appearance tests of `PropertiesLiveTests` and the drawing
`doc_undo` test, 22/22; `ContractTests.Tool_catalog_follows_the_conventions` 1/1; unit tests
323/323; `--check-catalog` ok; `check_repo` ok.
Made stricter: `doc_undo` refuses after a component's dimension set from an assembly, where
it answered "Nothing changed"; nothing else that passed is refused.
Decided 2026-11-22 by the run, not the owner: (a) the server tracks its steps in order rather
than saying "may be", because the step next is then named truly, and "may not be" is kept
only where the list runs out.
Decided 2026-11-22 by the run, not the owner: (b) a component's dimension is told by another
open document's update stamp changing during `dimension_set`, not by the dimension's name,
because that is the cause itself (the change lands outside the assembly) and S49's warning
already reads it; a mate's dimension changes no other document and stays an undo step.
Decided 2026-11-22 by the run, not the owner: (c) an undone dimension names the dimension,
not its feature, because the dimension is what changed and its name carries the feature.
Local: on `sw2023`, `624b0f3`; in build `20261122-030022`. Upstream: not yet
decided.
Held 2026-11-22: the scenario test of build `20261122-030022` (findings 2026-11-22, point 3):
on a block with a Ø10 × 5 boss, `dimension_set` 5 → 6 then `doc_undo` returned
`dimensions: ["D1@Boss-Extrude2"]`, −78.54 mm³ (c); `feature_rename` of the boss then
`doc_undo` refused, "undo would take back the step before it, the server's feature_extrude
instead" (a); `feature_suppress_set` then `doc_undo` gave `unsuppressed: ["Pin"]`, +392.699
mm³ (c); in a two-part assembly opened read-only, a mate suppressed then `doc_undo` gave
`unsuppressed: ["Coincident1"]` (c), and a component's `dimension_set` 60 → 80 then
`doc_undo` refused `NOT_SUPPORTED` naming `dimension_set` ("the assembly's undo would leave
the dimension_set in place, and may take back a step before it that isn't the server's or
nothing"), where the last build answered "Nothing changed" (b); an undone extrude named in
`removed` (c). Point 4 went to S117: after an undo has run past a later step, a change
before it that left no undo step (a `feature_rename`) is passed over, and the next
`doc_undo` takes back the step before it, reported `ok`.

### S119 - S84 (a) and (c): a mirror's area line and a cut's no-floor line read as faults on right results - CLOSED 2026-11-22

S84 (a) and (c), split out of S84 (which stays Open for (b)) in the run of 2026-11-21,
group 4. (a) `feature_mirror` said in `checks` "the copy's faces have 0.75 x the seed's
area: it meets the body differently from the seed" with every instance face found, a line
sessions took for a clash; (c) a blind cut whose depth equals the material it crosses said
"no floor at 20 mm: the cut broke out of the body before its end", a right result worded as a
fault. Cause: (a) `AreaNote`'s tail named the copy's meeting with the body, which reads as a
clash; (c) the cut's intent check had one line for every end with no cap at the asked depth,
and did not tell a depth that equals the material from one that is more. Fix: (a) `AreaNote`
(`PartFeatureService.cs`) ends "faces merge differently from the seed's; the instance faces
match", the ratio kept at its head; (c) `FeatureService.ExtrudeIntent`'s break-out line
asks the new `WallsReach` first: the end points of the cut's face edges, measured along the
direction as the caps are, reach the asked depth and not past it, and a blind cut then says
"through: the end lies on the far face" (the depth dimension kept at its end, as S48 (a));
walls that stop short of the depth still say "no floor at <d> mm: the cut broke out of the
body before its end", a mid-plane cut keeps its old line. No description changed.
Before: the new `PartFeatureLiveTests.A_blind_cut_whose_depth_equals_the_material_says_its_end_lies_on_the_far_face`
(a 20 mm cut through the 20 mm block) failed, "Assert.Contains() Failure: Filter not
matched in collection ... ["volume", "no floor at 20 mm: the cut broke out of the body
b"...]"; the mirror test `Mirror_of_a_boss_with_a_face_merged_into_the_body_does_not_warn`,
given the new wording, failed, "Assert.Contains() Failure: Filter not matched in collection
... ["volume", "the copy's faces have 0,75 x the seed's area: it m"..., "instance faces
at the expected positions: 4/4 ..."]". After: both pass, with the two live tests that call
the changed cut code (`A_blind_cut_that_leaves_the_body_passes_on_its_volume_and_a_kept_failure_takes_its_name`,
`A_blind_cut_from_a_plane_inside_the_body_that_leaves_it_passes_on_its_volume`), 4/4; unit
tests 323/323; `check_repo` ok. The first run of the third test lost SOLIDWORKS ("Lost the
connection ..."), which was restarted; it and the fourth passed after.
Made stricter: nothing; a cut deeper than the material still says it broke out, and the
new test holds that case.
Decided 2026-11-22 by the run, not the owner: (a) and (c) moved to Waiting as a new item
split out of S84, not as a second S84 heading, because one number heads one item and the
format splits an item that way.
Decided 2026-11-22 by the run, not the owner: the mirror's line says "the instance faces
match" and does not claim "the volume ... match" as the plan wrote it, because the mirror
holds no figure for the seed's own volume (its rule is only that the volume changed), so
the words would assert a check that is not made; a volume comparison would need the
rollback bar moved, a larger change than this wording.
Decided 2026-11-22 by the run, not the owner: the ratio stays at the head of the mirror's
line ("the copy's faces have 0.75 x the seed's area: ..."), because the plan changes the
tail only and the figure tells how far the faces differ.
Decided 2026-11-22 by the run, not the owner: "through" is said only for a blind cut, not
a mid-plane one, because the plan names a blind cut and a mid-plane cut has two ends to
read; and the depth dimension stays at the line's end, as S48 (a) names it.
Local: on `sw2023`, `6e4c9f5`; in build `20261122-142805`. Upstream: not yet
decided.
Held 2026-11-22: the scenario test of build `20261122-142805` (findings 2026-11-22, point 1):
(a) the ears of a part mirrored again about the front plane read "the copy's faces have 1.052
x the seed's area: faces merge differently from the seed's; the instance faces match",
instance faces 9/9, +7 183.229 mm³, no warning; (c) on a 100 x 60 x 20 block a blind Ø10 cut
20 deep from the top, renamed by `name`, read "through: the end lies on the far face (D1@<new
name> reads 20 mm)", −1 570.796 mm³, and a Ø4 cut 30 deep from the top of a 10 mm boss through
the block (30 of material) "through" too, −376.991 mm³; a Ø6 cut 25 deep through the 20 mm
block kept "no floor at 25 mm: the cut broke out of the body before its end".

### S84 - Feature results that read as faults on a right result: a mirror's area line, a pattern's rebuild warning under its old name - CLOSED 2026-11-22

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
Decided 2026-11-21 by the owner: (a), (b) and (c) taken into the run of 2026-11-21, group 4 (`## Run 2026-11-21`).
(a) and (c) done in the run of 2026-11-22, group 4, split out as S119 (Waiting); (b) done in
the same group, below.
(b) Cause: `FeatureVerifier.Check` warned of the new feature's rebuild warning the moment it
read it, before the tool's own rename and before `feature_pattern_linear` had settled its
sense. A pattern SOLIDWORKS ran the other way is deleted and made again flipped; the one
taken back had the warning (its instances off the body: a boss instance off the body gives an
`LPattern` warning 51, swSketchErrorExtRefFail, VERIFIED live), and its line stayed, under its
default name, while the pattern kept had none. That fits both meetings: "'LPattern1' has
rebuild warning 51" with `model_review` clean after, and a warning gone at the next rebuild.
A pattern kept with a real warning was named by its default name too ('LPattern11' for one
called by `name`). Fix: `Check` records the feature (doc, id) in `SwContext.DeferredWarnings`
instead of warning; the new `FeatureVerifier.ReportWarnings`, called by `FeatureVerifier.Result`
(which every tool calls after its rename) and at the end of every call in `SwSession` (for
the rest), reads the tree's rebuild states (`ErrorMap`), and when a deferred feature still
has one rebuilds once (`ForceRebuild3`, the What's Wrong box held off) and reads again; the
rule, in the new interop-free `RebuildWarnings.Lines` (unit-tested), reports each feature
once by the name it has now, leaves out one no longer in the tree or cleared by the rebuild,
and says "rebuild error ... after the rebuild; it had a warning when made" for one the
rebuild made worse. `Result`'s `feature.error`/`warning` are read after it. No description
changed.
`feature_delete` (met 2026-11-17, error 51 on the next feature): not the same cause. It reads
the errors after its own rebuild (`RebuildIfNeeded`), renames nothing and retries nothing,
and names features as they are called then; whether that error was real is still not known.
Not changed.
Before: the new `PartFeatureLiveTests.Linear_pattern_reports_the_rebuild_warning_of_the_pattern_it_keeps_by_its_new_name`
(a Ø10 boss at x = 40 patterned toward -x, 3 x 30, `name: "Row"`, SOLIDWORKS' own sense off
the block) failed, "Assert.DoesNotContain() Failure: Filter matched in collection ...
["'LPattern1' has rebuild warning 51 (swSketchErrorE"...]". After: it passes, with its second
case (a 10 x 10 x 3 block, 4 x 30 toward -x, the instance at x = -80 off the block: exactly one
line, "'Row' has rebuild warning 51 (swSketchErrorExtRefFail).", `feature.warning` true), and
the live tests that call the changed code: the linear pattern tests (toward -x, direction 2
off the part, a boss, bad input and rollback), two holes at two points, a blind cut kept on
failure with its name, a circular pattern, a mirror, an offset plane renamed, a convex fillet,
11/11; unit tests 326/326 (3 new); `check_repo` ok.
Made stricter: a deferred warning the rebuild turns into an error is now said as an error
(it was said as a warning, read before).
Decided 2026-11-22 by the run, not the owner: the warning is read after one forced rebuild
(`ForceRebuild3`), only when a deferred feature still has one, because `EditRebuild3` does
nothing to a model SOLIDWORKS holds as rebuilt, so "one gone after that rebuild" could not be
told, and the forced rebuild costs a call only where a warning stands.
Decided 2026-11-22 by the run, not the owner: the deferral is kept on the call
(`SwContext`) and reported in `Result` and again at the call's end, not moved into
`Result` alone, because tools that make several features (holes at several points, chamfer
groups) give `Result` only the last outcome, and a warning of an earlier one would be lost.
Decided 2026-11-22 by the run, not the owner: a deferred warning that is an error after the
rebuild is reported as a line, not a failure, because the feature passed its checks and the
call made no other change; failing it then would be a new rule the item does not ask for.
Local: on `sw2023`, `d82a7b2`; in build `20261122-142805`. Upstream: not yet
decided.
Held 2026-11-22: the scenario test of build `20261122-142805` (findings 2026-11-22, point 2):
(b) a Ø10 boss at x = 40 patterned toward −x, 3 x 30, `name` "Row": "direction flipped", +1
570.796 mm³, no rebuild line; a 10 x 10 x 3 block on the top patterned toward −x, 4 x 30,
`name` "Row2", the last instance off the block: one line, "'Row2' has rebuild warning 51
(swSketchErrorExtRefFail).", `feature.warning` true, +600 mm³. Point 10 of the same entry went
to S125: in that second case the volume per instance was averaged over the instance that
formed nothing.

### S120 - S64 (l): a closed contour read `regions: 0` around a hole of the face, and the regions lagged a later `sketch_add` - CLOSED 2026-11-22

S64 (l), split out of S64 (which stays Open for its other points) in the run of 2026-11-21,
group 4. `sketch_add` read a closed contour `"regions":0` while the feature from it was
right: a Ø6.5 circle around the edge of a Ø3.2 hole cut through the face; a polygon; a
4-point polyline on a tilted face after a converted edge; two profiles of an arc and a
polyline each. Cause, two of them. (a) (f)'s rule (`SketchService.OwnRegions`) counted a
region only when every edge of it lay on the sketch's contours; the ring between the circle
and the hole has the hole's edge, a model edge, as its inner loop, so it was dropped
(probed: 4 regions, the Ø8 circle beside it, the ring, the face around both, and the hole's
disc). (b) In an open sketch SOLIDWORKS' regions lag what a later call drew (the server adds
with AddToDB, unsolved): two circles drawn in two calls on a plane read 1 region with 2
closed contours, so a contour drawn after an earlier call can read 0. Fix: (a) the region's
loops are read (`ISketchRegion.GetFirstLoop`, `ILoop2.IsOuter`), and the new interop-free
`SketchRegions.IsOwn` (unit-tested) counts a region when its outer loop lies on the
sketch's contours and each inner loop does too or lies on an inner loop of the face the
sketch is on, a hole wholly inside the contour; the face around and a hole's disc are still
not counted. (b) `sketch_add` refreshes the regions before reading them
(`SketchService.RefreshRegions`: automatic solve off and on, which solves the sketch; no
undo step, the update stamp unchanged, probed), left alone when the user turned automatic
solve off. No description changed.
Before: the new `SketchEditLiveTests.Regions_count_a_contour_around_a_hole_of_the_face` (a
100 x 60 x 20 block, a Ø3.2 hole cut through, a concentric Ø6.5 circle on the same face)
failed, "Assert.Equal() Failure: Values differ Expected: 1 Actual: 0"; the new
`Regions_count_what_a_later_sketch_add_drew` (a second circle in a second call on the
front plane), run with the refresh left out, failed, "Assert.Equal() Failure: Values
differ Expected: 2 Actual: 1". After: both pass (the ring 1, a Ø8 circle beside it 2, the
2 mm cut from both the hand figure; a third circle around the first 3, and the circle not
moved by the solve), with (f)'s `Regions_count_the_sketchs_own_closed_regions_on_a_face_as_on_a_plane`
and the live tests that read contours or sketch on a face (convert of a face, info,
arc merged onto a polyline, engraved text, split line, curve pattern, circular pattern of
a hole, mirror about a face), 11/11; unit tests 329/329 (3 new, `SketchRegionsTests`);
`check_repo` ok. `docs/solidworks-api-notes.md` (*Regions*) gains the loops, the lag and
the refresh.
Made stricter: none. One result changed with (b): a line across a closed hexagon on a plane
reads its two halves, `regions: 2`, where it read 1, SOLIDWORKS' count from before the
line; (f)'s test now expects 2.
Decided 2026-11-22 by the run, not the owner: a loop of model edges inside the contour
counts only when every edge lies on an inner loop of the face the sketch is on, not any
loop inside the outline, because that is the rule as decided, and a model loop of another
kind (none met) keeps (f)'s refusal rather than a count no case has shown true.
Decided 2026-11-22 by the run, not the owner: the lag (b) is fixed in the same step,
because the planned test with a second circle beside the ring could not read true without
it and three of (l)'s meetings drew their contour after an earlier call; whether those
three were the lag was not reproduced on their own parts.
Decided 2026-11-22 by the run, not the owner: the refresh is made in `sketch_add` only, not
in `sketch_info` (a read) or the sketch edit tools, because the lag was shown after an add
and not probed after an edit tool; the smaller change.
Local: on `sw2023`, `c3c3b56`; in build `20261122-142805`. Upstream: not yet
decided.
Held 2026-11-22: the scenario test of build `20261122-142805` (findings 2026-11-22, point 3):
a Ø3.2 hole cut through a block's top face, then on that face a concentric Ø6.5 circle:
`regions: 1` (the ring); a Ø8 circle beside it in a second `sketch_add`: `regions: 2`; a 2 mm
cut from both, −150.812 mm³, the hand figure (ring 50.281 + disc 100.531). On the front plane
two circles in two calls read 2, a circle around the first and a hexagon in a third call 4, a
line joining two opposite corners of the hexagon 5.

### S121 - S64 (i): an equal chamfer's two legs are separate dimensions, and an equation on one said nothing of the other - CLOSED 2026-11-22

S64 (i), split out of S64 (which stays Open for its other points) in the run of 2026-11-21,
group 4. `feature_chamfer {type:"equal"}` makes a chamfer with two dimensions (SOLIDWORKS
2023 builds it as distance-distance with equal legs), independent once one is driven. An
equation on one alone left the other at its old value when the variable changed, a 6 x 5
chamfer where 6 x 6 was meant, `equation_set` `ok` with no warning; the workaround (drive
both) was on no `Usage` line. Cause: nothing in either result said the two were separate
dimensions, and `equation_set` knew nothing of a chamfer's legs. Fix: (a)
`PartFeatureService.ChamferAsync`, after the rename (the dimension names carry the feature's
name), adds a line to the result's `verified.checks` for an equal chamfer: "legs D1@X and
D3@X are separate dimensions: drive both to keep them equal (an equation on one leaves the
other as it is)"; no field and no description changed, so no byte of the catalog budget. (b)
`PropertiesService.OtherChamferLeg`, called by `equation_set` (one equation or a batch)
before it writes: for a row on a dimension of a Chamfer feature that has two mm dimensions
equal in value, when the other leg is neither a row of the same call nor already driven by
an equation, the call warns "D1@X is one leg of the chamfer 'X', whose legs are equal (3
mm); its other leg D3@X keeps 3 mm, a separate dimension, so the legs part when D1@X
changes. Set D3@X in the same call to keep them equal." The equation is still written (a
warning, not a refusal: driving one leg alone can be meant).
Before: the new `PartFeatureLiveTests.An_equal_chamfer_names_its_two_legs_and_an_equation_on_one_warns_of_the_other`
(a 100 x 20 x 60 block, two equal 3 mm chamfers on two edges; `W` = 5 mm and an equation on
one leg of the first alone; `V` = 4 mm on both legs of the second) failed on the result,
"Assert.Contains() Failure: Filter not matched in collection Collection: ["volume", "legs
measured on 1 edge(s)", "edge E1: 3 mm along F7 (plane [0, 0, 1]), 3 mm alo"...]", and, with
the check line in and the warning left out, on the equation, "Assert.Contains() Failure:
Filter not matched in collection Collection: []" (no warning). After: it passes (the one-leg
call warns naming the other leg and its value, the two-leg call has no warning, the volume
120000 - 750 - 800 mm³ as figured), with the other `Chamfer_*` tests of `PartFeatureLiveTests`
(equal, two distances, sides, edit, flipEdges, circular edges, sliver, bad input), four
equation tests of `PropertiesLiveTests` (global variable, errors, breaking a feature, a
plane's offset, several in one call) and the auxiliary view of a chamfer in
`DrawingLiveTests`, 15 of 15 live; unit tests 329/329; `check_repo` ok; no description
changed.
Made stricter: none (a warning and a line in the checks only; the equations are written as
before). Not done: the result of `feature_chamfer` with `edit` does not carry the legs line
(the call asked for was the creating one), and `dimension_set` on one leg does not warn.
Decided 2026-11-22 by the run, not the owner: the legs are named in `verified.checks`, not in
a warning, because every call of an equal chamfer would then warn (the chamfer tests assert
no warnings) and a field of its own would be a change of the result's shape; the check line
is read by a session that reads the result at all.
Decided 2026-11-22 by the run, not the owner: `equation_set` warns only when the two legs
were equal before the call, not for every two-leg chamfer, because a chamfer of two distances
meant to differ has no "other leg that stays" to warn of, and the finding's case is the
equal one; a leg already driven by an equation, or set in the same call, is not warned of.
Local: on `sw2023`, `69ae11e`; in build `20261122-142805`. Upstream: not yet
decided.
Held 2026-11-22: the scenario test of build `20261122-142805` (findings 2026-11-22, point 4):
equal 3 mm chamfers on two block edges, one named by `name`: each result's `checks` named both
legs ("legs D1@<name> and D3@<name> are separate dimensions: drive both ..."), by the new
name; a batch with a global `W` = 5mm and an equation on one leg of the first: written, −300
mm³, with the warning naming the other leg and its 3 mm; a batch with `V` = 4mm on both legs
of the second: no warning, −350 mm³; the other leg of the first in a later call of its own: no
warning, −500 mm³. The equal chamfer of a bore's edge on an earlier part carried the legs line
too, no warning.

### S122 - S64 (c): a fillet that stopped at a tangent joint failed with the hint "a radius too large" - CLOSED 2026-11-22

S64 (c), split out of S64 (which stays Open for its other points) in the run of 2026-11-21,
group 4. `feature_fillet` and `feature_chamfer` of 1 mm over a tangent chain failed five
times, `FEATURE_FAILED`, the hint "A radius too large for the adjacent faces fails" on faces
7 and 10 wide; 0.5 went through. The probe of 2026-11-16 (in S64) on a plate whose ends run
in a quarter round R7 into a face 30° off their tangent did not bring the five back, but
found the case where the hint misses: a round alone with `propagate` off, and both rounds
without the line between them, failed "SOLIDWORKS created no feature (the API returned
nothing)" at 1, 0.5 and 0.2 mm alike, each round ending tangent to the top line, not in the
call. Decided 2026-11-21 by the owner: such a failure gets the hint "edge <id> ends tangent
to <id>, not in the call: turn `propagate` on or add it", not the radius. Cause: a fillet
that stops at a tangent joint fails at any radius, and the constant fillet's failure hint
named the radius whatever the edges. Fix: `FeatureService.FilletAsync` (a constant fillet),
with `propagate` off, reads before the call (a rollback disconnects the edges) each end of
each edge in the call: the other edges at that vertex and their directions away from it (a
step of 1e-4 of the curve's parameter range, `DirectionAway`); the new interop-free
`FilletJoints` (unit-tested) keeps the joints where the two run on into each other
(opposite directions within about 1.4 mrad) and the other edge is not in the call (nor an
edge of a face in it), and gives the hint. On a failure of the call (any code the
verifier throws) that hint replaces the radius one; with no such joint, or with
`propagate` on, nothing is read and the hint is as before. Ids are given only to the edges
of a joint found, so a fillet with none moves no later id. `docs/solidworks-api-notes.md`
gains the joint (*A fillet that stops at a tangent joint fails*). No description changed.
Before: the new `ModellingLiveTests.A_fillet_that_stops_where_its_edge_ends_tangent_to_one_not_in_the_call_names_that_joint`
(that plate; the right round alone at 1 mm, then both rounds at 0.5 mm, `propagate` off;
then the round with `propagate` on) failed on the first call's hint, "Assert.Equal()
Failure: Strings differ … Expected: "edge E1 ends tangent to E3, not in the call: turn "···
Actual: "A radius too large for the adjacent faces fails; t"···". After: it passes (the
round alone names its joint with the line, both rounds name both joints and "add it", the
round with `propagate` on builds with its r 1 face), with the live tests that make a
constant fillet (convex and concave, the corner point and the query guard, the id made
stale by a cut and the ids kept with `propagate` off on a block's edges, the model box after
round faces, the compare with a saved copy, the fillet's info and its delete), 10 of 10
live; unit tests 333/333 (4 new, `FilletJointsTests`); `check_repo` ok.
Made stricter: none (a hint only; the call fails or builds as before).
Decided 2026-11-22 by the run, not the owner: `feature_chamfer` is not changed, because it
always passes SOLIDWORKS' tangent propagation (it has no `propagate`), so no chamfer call
stops at a tangent joint; the finding's chamfer failure is not this case.
Decided 2026-11-22 by the run, not the owner: only the constant fillet (`edges`, with or
without `faces`) reads the joints, not the variable one, because the probe met the joint on
the constant fillet and the variable one was not probed; the smaller change.
Decided 2026-11-22 by the run, not the owner: the joint hint replaces the radius hint on any
failure of a call that has one, not only on "created no feature", because a fillet that
stops at an open tangent joint fails whatever the radius, so the joint is the first thing to
change; a call with no such joint keeps the radius hint.
Local: on `sw2023`, `3fa3cf4`; in build `20261122-142805`. Upstream: not yet
decided.
Held 2026-11-22: the scenario test of build `20261122-142805` (findings 2026-11-22, point 5):
on the plate whose ends run in a quarter round R7 into a face 30° off their tangent, 10 thick
mid-plane, the right round alone at 1 mm, `propagate` off: `FEATURE_FAILED` with "edge E5 ends
tangent to E6, not in the call: turn `propagate` on or add it."; both rounds at 0.5: both
joints named; the top edge added to the call (all three, `propagate` off): built, −25.993 mm³,
round face r 1.

### S93 - A write that rebuilds does not name a feature far from the change that gained a rebuild warning - CLOSED 2026-11-22

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
Decided 2026-11-21 by the owner: taken into the run of 2026-11-21, group 4 (`## Run 2026-11-21`).
Done in the run of 2026-11-22, group 4. The writes that rebuild, listed: `dimension_set`,
`feature_delete`, `feature_suppress_set`, `equation_set` and `ref_plane`'s flip already compared
the tree's rebuild errors and warnings before and after (`ErrorMap`, `NewErrors`), so the
plan's `dimension_set` and `feature_delete` needed nothing. Of the sketch tools, only
`sketch_finish` closes a sketch that may have features after it: `sketch_import` closes the
new sketch it made (nothing after it), `sketch_edit` closes only a sketch it opened by
mistake, unchanged, and the feature tools close the server's open sketch to consume it.
Cause: `sketch_finish` closed the sketch (`InsertSketch`), and SOLIDWORKS rebuilt the
features after it, without reading their rebuild states. Fix: `sketch_finish` reads
`ErrorMap` before the close, closes it with the What's Wrong box held off, rebuilds when
SOLIDWORKS says the model is out of date, and warns of each feature whose state is new or
worse, wherever it is in the tree, "'Fillet1' now has rebuild warning 13
(swFeatureErrorFilletNoEdge)." The line comes from the new
`FeatureManagementService.WarnNewErrors`, which `feature_delete` and `feature_suppress_set`
now use for the same line they wrote by hand; it leaves out a feature a feature check of the
call deferred and has not yet reported (S84 (b), `ReportWarnings`), so no feature is named
by both. No description changed.
Before: the new `FeatureManagementLiveTests.Sketch_finish_names_a_feature_the_redrawn_sketch_left_with_a_rebuild_warning`
(the 100 x 20 x 60 block, an r 5 fillet on two vertical edges with `propagate` off, the
block's sketch reopened and the corner under one of the edges chamfered 10, `sketch_finish`)
failed, "Assert.Single() Failure: The collection did not contain any matching items ...
Collection: []". After: it passes, the line exactly "'Fillet1' now has rebuild warning 13
(swFeatureErrorFilletNoEdge).", and a second redraw that changes nothing names nothing;
with the live tests that call the changed code (`feature_delete`: a fillet, a parent with
children and its dry run, a child's sketch, a plane with its boss and mirror, the equations
listed and deleted; suppression round trip and refusals; `sketch_finish` of a 3D sketch, of a
reopened sketch, on the top plane, of an open line), 13 of 13 live; unit tests 333/333;
`check_repo` ok.
Made stricter: `sketch_finish` now warns of a rebuild error or warning its close gave a
feature after the sketch (it said nothing).
Decided 2026-11-22 by the run, not the owner: only `sketch_finish` gains the compare, because
the other writes the plan names (`dimension_set`, `feature_delete`) and `feature_suppress_set`,
`equation_set`, `ref_plane`'s flip already compare, and no other sketch tool closes a sketch with
features after it; a feature tool that consumes a sketch reopened by `sketch_edit` is left
as it is, the smaller change.
Decided 2026-11-22 by the run, not the owner: a new rebuild error after the close is a line,
not a failure, as `equation_set` (S58 (a)) and `feature_delete` give it, because the sketch
is what the caller drew and the close is kept; the line names the feature to fix next.
Local: on `sw2023`, `b3abaf4`; in build `20261122-142805`. Upstream: not yet
decided.
Held 2026-11-22: the scenario test of build `20261122-142805` (findings 2026-11-22, point 6):
a block with an r 5 fillet on two vertical edges (`propagate` off), the block's sketch
reopened and the corner under one edge chamfered 10 in the sketch: `sketch_finish` warned
"'Fillet1' now has rebuild warning 13 (swFeatureErrorFilletNoEdge)."; the sketch reopened and
closed again unchanged: no warning. The point's "Small" went to S123: both results listed
`D3@Sketch1` twice in `dimensions`, and `sketch_edit` and `sketch_finish` counted the sketch's
segments 7 and 5.

### S117 - After a `doc_undo` has run past a later step, a change before it that left no undo step is passed over: the next undo takes back the step before it, reported `ok` - CLOSED 2026-11-22

Sorted out of the entry of 2026-11-22 (the scenario test of build `20261122-030022`), point
4, on S113's and S114's fixes (Closed). A second boss extruded, then `feature_rename` of the
base extrude, then `feature_suppress_set` of the second boss, then `doc_undo` (ran,
`unsuppressed` the boss, right), then `doc_undo` again: it ran, `removed:
["Boss-Extrude3"]`, −392.699 mm³, and the rename stayed (`feature_list` shows the new name).
Expected: S113's refusal, as given straight after a rename ("feature_rename left no
SOLIDWORKS undo step: undo would take back the step before it ... and leave the
feature_rename in place"), since the rename is now the server's last change not undone. The
same where the rename was of the boss itself (the boss went, and its new name with it). The
result named truly what it removed, so nothing was hidden; the refusal is not armed again
once an undo has run past the step after the change. Reported `ok`.

Local, small, in the files of group 2 and group 3 (`doc_undo` and `SwContext`'s undo
accounting).
Decided 2026-11-22 by the run, not the owner: once a `doc_undo` has taken back the step after
a change that left no undo step, that change is again the server's last change not undone,
and the next `doc_undo` refuses naming it, as S113 does. Test to start from: the entry's
sequence, a boss, a rename of the base extrude, a suppress of the boss, `doc_undo`, then
`doc_undo` again: refused naming `feature_rename` (fails now: it runs, removing
`Boss-Extrude3`). Default model.
Decided 2026-11-22 by the run, not the owner: taken into group 4, by the run's rule "a new
point, small and in the group's files, a later step of this run ... taken into the next
group", because it is a small change to the undo accounting group 3 changed, found by group
3's check.
Cause: a counted write after a change that left no undo step cleared the change from the
document's record (`NoUndoChanges`), and the undo that took that write back left the record
empty, so nothing said the change was again the server's last not undone.
Fix: the record is a new interop-free `UndoLedger` (`Documents/UndoLedger.cs`, in place of
the entry's `NoUndoChanges` and `UndoSteps`): each counted step keeps the changes with no
undo step that came before it; `doc_undo` puts them on record again when it takes that
step back, so the next undo refuses as S113's does ("The server's last change in 'X'
(feature_rename) left no SOLIDWORKS undo step: undo would take back the step before it, the
server's feature_extrude instead ..."). An undo of several steps that would pass over one
is refused too, naming it and how many steps it can take back first ("... before its last
1 undo step(s) (feature_rename) left no SOLIDWORKS undo step: past them undo would take
back ...", hint "Pass steps ≤ 1."). No description changed.
Before: the new `FeatureManagementLiveTests.Undo_refuses_again_after_an_undo_past_the_step_that_followed_a_change_with_no_undo_step`
(the 100 x 20 x 60 block, an r 5 x 5 boss on its top, the base extrude renamed, the boss
suppressed, `doc_undo`, `doc_undo`) failed at the second undo, "Assert.Throws() Failure: No
exception was thrown". After: it passes (the second undo refused naming `feature_rename`
and the server's `feature_extrude`, the boss kept; a suppress again, then `steps: 2`
refused with "steps ≤ 1"); with every `doc_undo` test (`FeatureManagementLiveTests`' five,
the assembly's and the drawing's), the stamp tests
(`The_servers_own_failed_kept_and_rolled_back_changes_do_not_read_as_the_users`,
`An_assembly_adopted_after_the_server_saved_its_parts_does_not_read_as_changed_by_the_user`)
and the drawing section test that undoes in a drawing, 11 of 11 live; the new unit tests
`UndoLedgerTests` (6) in unit tests 339/339; `check_repo` ok.
Made stricter: `doc_undo` refuses after an undo past the step that followed a change with no
undo step, and an undo of several steps that would pass over one; nothing else that passed
is refused.
Decided 2026-11-22 by the run, not the owner: an undo of several steps that would pass over
such a change is refused too, naming how many it can take back first, because it is the same
fault in one call, and refusing only the one-step undo would leave it passing as `ok`.
Decided 2026-11-22 by the run, not the owner: the changes are kept with the step after them
in a new interop-free `UndoLedger`, unit-tested offline, rather than a second list beside
`UndoSteps`, because the rule is then one function each for a write, an undo and a refusal.
Local: on `sw2023`, `ee5c4c5`; in build `20261122-142805`. Upstream: not yet
decided.
Held 2026-11-22: the scenario test of build `20261122-142805` (findings 2026-11-22, point 7):
a boss patterned, the base extrude renamed, the pattern suppressed, then `doc_undo`:
`unsuppressed` the pattern; `doc_undo` again: `NOT_SUPPORTED` naming `feature_rename` and "the
server's feature_pattern_linear" before it. The pattern suppressed again, then `doc_undo
{steps:2}`: refused, "before its last 1 undo step(s) (feature_rename)", hint "Pass steps ≤
1."; one step then ran.

### S98 - `doc_open {readOnly:true}` reports `access: owned` and says the server may edit it - CLOSED 2026-11-22

Sorted out of the entry of 2026-11-17 ("read-only checks of rebuilt parts and an assembly",
point 1). A read-only check opened six parts and an assembly with `readOnly: true`; each came
back `access: "owned"`, with the line that this server may edit it. Nothing was written only
because the session's brief said so: the result invites an edit the open was meant to rule
out.

Proposed by the session: a read-only open reports its own access (`readOnly`), and the edit
tools refuse on it with NOT_OWNED's hint. Local. Not decided.
Decided 2026-11-21 by the owner: taken into the run of 2026-11-21, group 5: a read-only open reports `readOnly` and the edit tools refuse on it (`## Run 2026-11-21`).

Cause: a read-only open was registered as `Owned` with nothing to mark it, so `doc_list`,
`sw_status` and the open's result read `owned`, the message said "this server may edit it",
and the write guard in `SwContext.Bind` refused only `Foreign` documents.
Fix: the registry entry carries `ReadOnly` (set by `doc_open {readOnly:true}`, kept in the
registry file across a server restart), and `access` reads `readOnly`. The one write guard,
`SwContext.Bind`, refuses every `DocAccess.Write` call on it with `NOT_OWNED`: "'<title>'
(D<n>) was opened with readOnly, so this server won't modify it. Hint: To change it,
doc_close it and doc_open it again without readOnly. doc_save mode 'copy' writes a copy as
it is." A write that changes nothing of the design says so with `RunOptions.ReadOnlyOk`:
`doc_rebuild` and `doc_save` mode `copy`. The open's message says it is the target for
reading and the edit tools refuse it. A file SOLIDWORKS opens read-only on its own (a
read-only file, one in use) without `readOnly` asked stays `owned` and editable as before.
Descriptions: `doc_open` and its `readOnly`, `doc_list`'s access values, `sw_status` (its
access list shortened to "access"), paid by dropping `doc_open`'s "(the user's documents stay
read-only)"; the catalog went from 247.7 to 247.6 KB, `--check-catalog` ok. The guide's access
line names `readOnly`.
Before: `ExportLiveTests.A_read_only_open_is_reported`, now asserting `readOnly`, no "may
edit", a sketch and `feature_extrude` refused with `NOT_OWNED`, the volume unchanged, failed
"Assert.Equal() Failure: Strings differ, Expected: "readOnly", Actual: "owned"".
After: it passes (also `doc_list` reads `readOnly`, `doc_save` mode `copy` writes, mode `as`
refused `NOT_OWNED`); live, `A_read_only_document_that_got_dirty_closes_without_discard`
(made dirty by `doc_rebuild {force:true}` now, as sessions did, not by an extrude the guard
refuses), `A_saved_part_reopens_as_owned_target_with_its_geometry`,
`Opening_a_part_does_not_report_the_stl_options_solidworks_derives_on_open`,
`A_file_that_is_already_open_is_not_reopened_and_keeps_its_access`,
`An_exported_step_imports_back_as_one_solid_of_the_same_volume`,
`M0LiveTests.Foreign_documents_are_read_only`, `Dirty_owned_documents_need_save_or_discard`,
`Save_copy_writes_a_fresh_file_and_refuses_overwrites`,
`FeatureManagementLiveTests.Rebuild_of_a_clean_part_reports_no_errors`,
`Undo_units_rebuild_and_delete_refuse_the_users_documents`,
`AssemblyLiveTests.Pack_and_go_copies_the_assembly_its_parts_and_drawing_and_the_copies_reference_each_other`
and `TopDownLiveTests.Saved_bodies_become_parts_with_their_volumes_and_an_assembly_of_them`
pass, 13/13; unit tests 346/346 (`ContractTests.Tool_catalog_follows_the_conventions` among
them); `check_repo` ok.
Made stricter: every edit tool, `doc_save` mode `inPlace` and `as`, and `doc_undo` refuse a
document opened with `readOnly:true`.
Decided 2026-11-22 by the run, not the owner: `doc_save` on a read-only document writes mode
`copy` and refuses `inPlace` and `as` with the hint above, because a copy changes nothing of
the open document (a read of it), while `as` would turn it into an editable file the open
was meant to rule out, and `inPlace` SOLIDWORKS refuses anyway.
Decided 2026-11-22 by the run, not the owner: `doc_rebuild` still runs on a read-only
document, because a rebuild changes nothing of the design and reading its errors is what a
read-only check does; the dirt it leaves is discarded by `doc_close` as before.
Decided 2026-11-22 by the run, not the owner: the way back to editing is `doc_close` and
`doc_open` without `readOnly`, not `doc_activate {allowEdit:true}`, because SOLIDWORKS keeps
the document read-only either way and a reopen is what `doc_pack`'s read-only refusal
already asks for.
Decided 2026-11-22 by the run, not the owner: the mark follows the `readOnly` asked, not
SOLIDWORKS' `IsOpenedReadOnly`, because it is the caller's intent the access reports; a file
SOLIDWORKS opened read-only unasked stays `owned` (saved by `copy` or `as`).
Local: on `sw2023`, `027e13c`; in build `20261122-194424`. Upstream: not yet
decided.
Held 2026-11-22: the scenario test of build `20261122-194424` (findings 2026-11-22, point 1):
a part opened with `readOnly:true` read `access: "readOnly"`, the message "the target
document for reading: the edit tools refuse it"; `dimension_set`, `sketch_start`, `doc_save`
mode `as` and `inPlace` all refused `NOT_OWNED` with the reopen hint; `doc_rebuild
{force:true}`, `doc_save` mode `copy` and a STEP `model_export` ran. A read-only assembly the
same: `access: readOnly`, a component dimension's `dimension_set` refused, its parts listed
`foreign` with `loadedBy`; closing discarded the rebuild's dirt as before.

### S108 - `doc_new {type:"drawing"}` with no `template` replaces a custom default template's format with ISO and its units with MMGS - CLOSED 2026-11-22

Sorted out of the entry of 2026-11-18 ("a custom sheet format made from the ISO one with
`vba_run`", point 6), on build `20261118-060615`. With SOLIDWORKS' default drawing template
set to one holding a custom format and custom units (mass in kg), `doc_new {type:"drawing"}`
reported "Created drawing from <that template>; ... format a3 - iso.slddrt" and units MMGS;
with `template:<the same file>, units:"template"` it kept both. Said in the result, so not
reported wrong, but a user who set a default template does not get it.

Proposed by the session: keep the template's sheet whenever the default drawing template is
not the shipped one, and say so; or a server setting for the default drawing template.
Local. Not decided.
Decided 2026-11-21 by the owner: the default drawing template kept as it is when it is not one SOLIDWORKS ships, the result saying so; taken into the run of 2026-11-21, group 5 (`## Run 2026-11-21`).

Cause: `doc_new` set every drawing made without `template` up as ISO A3 (`SetupNewDrawing`
without `keepTemplateSheet`), and its `units` defaulted to `mmgs` in the tool's signature, so
a default template of the user's own lost its sheet format and units, and an explicit
`units:"mmgs"` could not be told from one left out.
Fix: `DrawingLayout.ShippedTemplate` (interop-free, unit-tested) calls a template shipped
when it lies under the install folder (`ISldWorks.GetExecutablePath`, its `data\templates`)
or under `<ProgramData>\SOLIDWORKS` (where the installer puts `Drawing.DRWDOT` and the sheet
formats). A drawing with no `template` whose default template is not shipped keeps that
template's sheet (as with a named template) and, when `units` is left out, its units; the
message says "..., the default template, kept as it is (its sheet and units) since it is not
one SOLIDWORKS ships; sheet ... as the template has it". `units` now defaults to null (MMGS
for everything else, as before); a `units` the caller names still wins. A shipped default
is set up as ISO A3 as before. Description: `doc_new` says a default template SOLIDWORKS does
not ship keeps its sheet and units, paid by "whatever the template says", "the target
document" to "the target" and the `units` text shortened; the catalog stays at 247.6 KB,
`--check-catalog` ok. The skill's drawing step says the same.
Before: `DrawingLiveTests.A_custom_default_drawing_template_keeps_its_sheet_format_and_units`
(the default drawing template set to a template the test makes in a temp folder, IPS on a
copied A3 format named `custom a3.slddrt`, and set back in a `finally`, read back equal)
failed "Assert.Equal() Failure: Strings differ, Expected: "IPS", Actual: "MMGS"".
After: it passes (its units IPS, its format kept, the message naming it; `units:"mmgs"` gives
MMGS on the same kept format); live, `New_drawing_is_A3_landscape_iso_first_angle_in_mm`
(now with the default set to the shipped `Drawing.DRWDOT`, restored after),
`Closing_a_drawing_names_what_solidworks_closed_with_it`,
`Sheet_setup_changes_what_is_given_and_keeps_the_rest`,
`Standard_views_have_the_block_proportions_without_overlaps_and_export_to_pdf`,
`A_section_with_no_atMm_is_placed_so_its_own_label_clears_the_title_block`,
`AssemblyLiveTests.Release_package_writes_a_step_per_model_the_drawing_pdf_the_flat_dxf_and_the_bom`
and `Pack_and_go_copies_the_assembly_its_parts_and_drawing_and_the_copies_reference_each_other`
pass, 8/8 (`TemplateLiveTests.Parts_and_assemblies_come_from_the_given_templates` skipped: it
looks for the 2025 templates); unit tests 355/355 (`DrawingLayoutTests` +9,
`ContractTests.Tool_catalog_follows_the_conventions` among them); `check_repo` ok. The
drawing tests' helper sets the ISO A3 sheet itself when the machine's default template is
not shipped, as their oracle is that sheet.
Made stricter: the drawing tests no longer depend on the machine's default drawing template;
the ISO A3 test sets the shipped one for itself.
Decided 2026-11-22 by the run, not the owner: "shipped" is a template under the install
folder or under `<ProgramData>\SOLIDWORKS`, because those are the folders the installer
writes (the default `Drawing.DRWDOT` is in the latter, the standard ones such as `iso.drwdot`
in the former), and any template the user made or copied lies elsewhere.
Decided 2026-11-22 by the run, not the owner: only drawings change; a part or an assembly
from a custom default template still gets MMGS when `units` is left out, because the item and
the owner's decision are about the drawing's sheet and units, and parts' units are set by
`units` the same way for every template.
Decided 2026-11-22 by the run, not the owner: `units` left out keeps the custom template's
units, and `units` named wins over them, while the sheet stays the template's either way,
because the caller who names units asks for exactly that, and the sheet has no such option
in `doc_new` (`drw_sheet_setup` changes it).
Local: on `sw2023`, `6535044`; in build `20261122-194424`. Upstream: not yet
decided.
Held 2026-11-22: the scenario test of build `20261122-194424` (findings 2026-11-22, point 2):
with SOLIDWORKS' default drawing template set to a custom A3 template (not shipped), `doc_new
{type:"drawing"}` with no `template` kept its sheet format (`drw_info`: that template's
format and title block) and its units (`custom`), the message "the default template, kept as
it is (its sheet and units) since it is not one SOLIDWORKS ships"; `units:"mmgs"` gave MMGS
on the same kept sheet, the message saying "(its sheet)"; the same template named with
`units:"template"` gave the same sheet and units.

### S90 - `drw_view_section` with no `atMm` places a section so its own label overlaps the title block - CLOSED 2026-11-22

Sorted out of the entry of 2026-11-17 (the scenario test of build `20261117-012659`), point
7; met in three scenario tests. `drw_view_section` with no `atMm` placed a section of a 1:2
plan where its own view label overlaps the title block by about 0.2 mm, and warned of it
("view label … overlaps the title block"). The warning is right, so not reported `ok`
while wrong; the free-space placement does not count the label the call is about to make.

Proposed: the free-space placement counts the view's label with the view before it places
it. Local, in the drawing code, not group 5's files: for the next plan, not this run. Not
decided.
Decided 2026-11-21 by the owner: taken into the run of 2026-11-21, group 5 (`## Run 2026-11-21`).
Cause: `Place` (`DrawingService.cs`), which `drw_view_section` and `drw_view_detail` use with no
`atMm`, searched free space for the view's geometry box only, against the other views'
outlines and the sheet's 5 mm edge: not the outline margin SOLIDWORKS draws round the view
(5.94 mm read at a 1:2 sheet), not the label it draws under a section (its box 4.03 to 12.05
mm below the geometry, 45.3 mm wide for "SECTION A-A", partly outside the outline), and not
the format's border or title block at all, so a section "down" from a view near the title
block was placed across it.
Fix: the interop-free `DrawingLayout.SectionFootprint` (the geometry box with the outline
margin and the label, constants read live) is what `SideSpot` and `FindSpot` test for a
section (a new optional `footprint` argument, the view stays aligned with its parent);
`Place` adds the format's blocks (the title block) to the occupied boxes and the border to
the area, for sections and details alike. A section with no room "down" takes "up" (its
label between it and the parent) or the nearest free place; a section with no free place
still warns as before.
Before: the new `DrawingLiveTests.A_section_with_no_atMm_is_placed_so_its_own_label_clears_the_title_block`
(1:2 sheet, the block's front view above the title block, a horizontal cut) failed: the
section at [289.06, 60.31, 350.94, 82.19] "overlaps the title block [230, 10, 410, 65.25]; view
label 'DetailItem230' of 'Section View A-A' [297.152, 54.202, 342.471, 62.22] overlaps the
title block".
After: it passes (the section above its parent, aligned, no overlap warning, "clear of the
title block"); live, every test that calls `Place` or makes a section or detail, 9/9
(`Section_through_the_middle_is_a_hatched_section_view`,
`A_section_s_model_box_is_what_lies_behind_the_cut`,
`A_drawing_shows_no_open_sketch_before_or_after_a_section`,
`Detail_of_a_corner_has_the_requested_scale`, `Detail_leaves_no_sketch_open`,
`Moving_an_annotation_or_a_view_s_label_onto_another_text_is_warned`,
`Views_move_with_their_aligned_children_and_are_deleted`,
`A_parent_with_a_section_is_deleted_only_with_its_children`, and the new one); unit tests
346/346 (new `DrawingLayoutTests.A_section_s_footprint_holds_its_outline_margin_and_its_label`
and `A_section_is_placed_where_its_label_clears_the_title_block`); `check_repo` ok. No
description changed.
Made stricter: a section or detail with no `atMm` keeps 5 mm clear of the title block and
inside the border, and a section also clear of its label's box; nothing that passed is
refused.
Decided 2026-11-22 by the run, not the owner: the title block and the border count for
details as well as sections, because the one `Place` serves both and a detail placed across
the title block is the same fault; only the label's footprint is the section's.
Decided 2026-11-22 by the run, not the owner: the outline margin and the label's box are
constants read at a 1:2 sheet on 2023 SP3 (6, 12.1 and 46 mm, rounded up), not read from the
new view, because its size is known only after it is made, and the call's own overlap check
still warns when they are off; whether the margin scales with the sheet is not known.
Local: on `sw2023`, `82087b8`; in build `20261122-194424`. Upstream: not yet
decided.
Held 2026-11-22: the scenario test of build `20261122-194424` (findings 2026-11-22, point 3):
a 1:2 plan of a tray, a horizontal section with no `atMm` below it: on the ISO A3 format
(title block top 65.25) the section's geometry box bottom at y 80, its label anchor at y
73.4, "clear of the title block", no warning; in the PDF the label clears the block by about
1.5 mm. On a custom A3 format (block top 60) the same, the label at y 73.0. Point 7 of the
same entry went to S128: an auxiliary view with no `atMm` was placed over its parent and the
title block, and the free-space search does not count tables.

### S106 - The drawing's overlap checks count neither tables nor views, and `drw_table_add` gives no extent - CLOSED 2026-11-22

Sorted out of the entry of 2026-11-18 ("a two-sheet A3 drawing of a 16-member weldment
part", point 1), on build `20261118-060615`. **A result reported `ok` while wrong.**
`drw_table_add {kind:"cut_list", atMm:[408,285]}` returned the rows but no extent; the table
came out about 201 x 71 mm, over a side view. A 145 x 118 mm note added after it read "clear
of the border, the title block and the other annotations" while it lay across both the table
and the side view's members; only the PDF showed it.

Proposed by the session: `drw_table_add` returns the table's `extentMm` and warns of what it
lands on; the note and dimension checks count tables and the views' `boundsMm`, not only
annotations. Local. Not decided.
Decided 2026-11-21 by the owner: taken into the run of 2026-11-21, group 5 (`## Run 2026-11-21`).
Cause: `drw_table_add` read no box for the table it made, and `ITableAnnotation` has no
extent member; the placement check of notes, dimensions and moved annotations (`BoxOf`,
`PlacedOnSheet`) knew only notes, labels, dimensions, hole callouts and balloons, and no
check but the views' own looked at the views' outlines, so a table and the view under it
were both invisible to it.
Fix: `BoxOf` (`DrawingService.Placement.cs`) reads a table's box from its anchor corner
(`IAnnotation.GetPosition`, `ITableAnnotation.AnchorType`) and the sum of its visible column
widths and row heights, through the interop-free `DrawingLayout.TableBox`; probed live on
2023 SP3, the box equals the box of the table's drawn lines (`IDisplayData`) to 0.001 mm for
a hole table (top-left anchor), a cut list and a revision table (top-right). `PlacedOnSheet`
counts tables. `drw_table_add` (all three kinds) returns `extentMm` and, through the new
`TableCheck`, warns "The table [..] overlaps view '<view>' [..], ..." of the views' outlines,
the border, the title block and the other annotations it lands on, else the check "clear of
the views, the border, the title block and the other annotations". `PlacementCheck` (notes,
dimensions, `drw_annotation_edit move`) counts the tables and the outlines of every view but
the annotation's own; its check reads "clear of the border, the title block, the other views
and the other annotations". A view added or moved over a table is warned ("'<view>' overlaps
table '<name>' [..]"). No description changed.
Before: the new `DrawingLiveTests.A_table_over_a_view_gives_its_extent_and_is_warned_and_so_is_a_note_across_it`
(a hole table placed at the front view's top-left corner, inside it; a note across the
table) failed, "no extent; warnings: " (no extent, no warning); the same run with the note
read the note "clear of the border, the title block and the other annotations" with no
warning, though it lay across the table and the view.
After: it passes (the table warned of the view, the note of the table and the view, the
view moved 5 mm under the table warned of it), with the cut-list and revision tests now
checking `extentMm` against the anchor (the revision table growing downward with a row);
live, every `DrawingLiveTests` test that adds a note, a dimension or a table, or moves an
annotation or a view, 28/28 (the new one included); unit tests 344/344 (new
`DrawingLayoutTests.A_table_s_box_grows_away_from_its_anchor_corner`); `check_repo` ok.
Made stricter: a table, and a note, dimension or moved annotation, lying on a view or a
table now warn where they read "clear"; a view added or moved over a table warns; nothing
that passed is refused.
Decided 2026-11-22 by the run, not the owner: a table's box is its anchor corner plus the
sum of its visible columns and rows, not the box of its drawn lines, because the probe showed
the two equal and the sums are documented members, with the anchor rule unit-tested; an
anchor it does not know gives no extent and a warning rather than a guessed box.
Decided 2026-11-22 by the run, not the owner: a note's or a dimension's own view does not
count against it (a dimension's text and a note added in a view lie in or beside their view
by design); the other views' outlines do, because a text on another view is what the PDF
showed.
Decided 2026-11-22 by the run, not the owner: the view check (a view added or moved) also
counts the tables, though the plan named only the table, note and dimension checks, because
its check claims "no overlaps with other views or their annotations" and a table is one; one
clause, no new option.
Local: on `sw2023`, `cff9e5c`; in build `20261122-194424`. Upstream: not yet
decided.
Held 2026-11-22: the scenario test of build `20261122-194424` (findings 2026-11-22, point 4):
a hole table placed with its corner inside the plan view gave `extentMm` [165, 202.52,
309.763, 215] (the PDF matches) and "The table [..] overlaps view 'Drawing View1' [..]"; a
note across the table warned of the table and the view, a note on the view only of the view,
a note in empty space read "clear of the border, the title block, the other views and the
other annotations"; a detail placed by default over the table's corner was warned "overlaps
table". Point 7 of the same entry went to S128: the free-space search placed that detail
there, not counting tables.

### S1 - `drw_view_section` / `drw_view_detail` drew in view-sketch coordinates - CLOSED 2026-11-22

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
Held 2026-11-22: the scenario test of build `20261122-194424` (findings 2026-11-22, point 5),
for `drw_view_detail` and a view at an angle: (a) `drw_view_detail` on a 1:2 plan, circle
centre [236.75, 140.7] r 8, 2:1: the circle landed on the hole asked for (the PDF, measured
to 0.1 mm), the detail showing that tab with its hole centred, 4 visible edges; (b)
`drw_view_add auxiliary` square to a straight floor edge running at 24.23° in the plan looked
along [−0.41, 0, 0.912], turned on the sheet with that edge (its fold edge at −24° in the
PDF); (c) on that turned view, a detail at one corner ([108.5, 104.1], r 10, 1:1): the circle
on the corner (measured [108.8, 104.0]), the detail showing the same two edges at the same
angles; (d) a section across the turned view (line [153.4, 42] → [198.6, 142.1], model +Y on
that view): the cut line where asked (ends within 1.7 mm on the raster), looking along
[−0.912, 0.001, −0.41], the expected normal, 2 hatched faces, a floor-and-wall cross-section
in the PDF. `drw_view_edit broken_out` not run: its circle in sheet coordinates is still not
checked. Points 6, 8 and 9 of the same entry went to S127, S129 and S130.

**Upstream: PR #1, open.**

### S126 - S52 (d): `body_list`'s box is `GetBodyBox`'s loose bound, not the tight box of the other results - CLOSED 2026-11-22

Point (d) of S52 (S52 stays Open for the rest) (findings 2026-11-16, "a part edited by rollback", point 9, build
`20261116-124830`): `body_list` gave a box max of 149.016816 where `equation_set` and
`model_mass` said 149.016716 for the same body.
Decided 2026-11-21 by the owner: taken into the run of 2026-11-21, group 5 (`## Run 2026-11-21`).
Cause: `MeasureService.BodyListAsync` built each row's box from `IBody2.GetBodyBox`
(`b.Box()`), the loose bound over torus and B-spline faces that S52 (a) replaced by `TightBox`
in every other result; `body_list` was left on the old read.
Fix: each row's box is `TightBox.Of([body])` (the body's extremes along +-x, +-y, +-z), so
it equals `model.box` for a one-body part; a body whose extremes cannot be read keeps its
loose box with `approximate: true`, as `model.box` does. Surface bodies go through the same
call. No description changed (it says "bounding box (mm)").
Before: the new `PartFeatureLiveTests.Body_list_gives_the_same_box_as_the_model` (the plate
of S52's box test, quarter-round ends into a 30 degree face, its top chain rounded R0.5)
failed, "box [-56,582209, -5, -5] .. [56,589919, 5, 5,089919], expected [-56,5, -5, -5] ..
[56,5, 5, 5]". A first version on a disc with its top edge rounded passed on the old code
(the disc's loose box is exact), so the plate is the case that fails.
After: it passes (`body_list`'s box equal to the fillet result's `model.box` and to the
extremes, no `approximate`); live, the other tests that call `body_list` or read its
boxes, `Body_list_reports_the_block`, `Body_list_of_a_two_body_part`,
`Body_list_of_a_closed_document_is_DOC_CLOSED`, `Mass_of_one_body_in_a_two_body_part` and
`Mirror_of_two_unmerged_extrudes_as_bodies_adds_their_reflections`, 6/6 with the new one;
unit tests 346/346; `check_repo` ok. No description changed, so no catalog check.
Made stricter: nothing refused; `body_list`'s boxes are now the bodies' extremes, up to
0.09 mm tighter than before.
Not done, still S52's: `BodyKit.Row` (the `bodies` rows of `body_split`, `body_combine`,
`mold_split` and the other body tools, "as `body_list` reports them") still reads the body's
`GetBodyBox`; its `BodyGeom.Box` also feeds the before/after comparisons (`SameBox`,
`Crosses`), which a tight box would change, so it needs its own look.
Decided 2026-11-22 by the run, not the owner: only `body_list`'s read changed, not
`BodyKit.Row`, because the plan names `body_list`, and `BodyGeom.Box` is also the key of the
before/after body comparisons, whose tolerance a change of box would touch; the rows of the
other body tools are left for S52's remainder.
Local: on `sw2023`, `ca11c05`; in build `20261122-194424`. Upstream: not yet
decided.
Held 2026-11-22: the scenario test of build `20261122-194424` (findings 2026-11-22, point
10): a slot 100 long, 10 wide and 10 high with its top edge rounded R2 (torus ends), and a
second body, a slot turned 30° in the front plane with its face edge rounded R1.5:
`body_list` boxes [−55, 0, −5] .. [55, 10, 5] and [−35, 35, 0] .. [35, 79.641, 8], the exact
extremes, equal to `model.box`, no `approximate`. Whether the old read was loose on these was
not checked; the live test's plate holds the case that failed.

### S97 - `material_list` with no match does not say which databases it read - CLOSED 2026-11-22

Sorted out of the entry of 2026-11-17 ("a printed plate with two walls standing off it in an
L", point 2), on build `20261117-012659`. `material_list {filter}` with no match returned
`"materials":[]`, `"databases":[]` and a warning naming the closest names. `databases` lists
only the databases the filter reaches, so a miss does not show which were read, and a caller
cannot tell that a custom database was searched before calling a material missing. Reported
`ok`.
Decided 2026-11-21 by the owner: taken into the run of 2026-11-21, group 5, with S104 (`## Run 2026-11-21`).
Cause: S39's filter on `databases` keeps the databases the matching rows (or the filter, by
name) reach, which is none on a miss.
Fix: on a miss `databases` is every database searched, with its count, as a call with no
filter gives it. The description says so ("without a filter or on a miss"), paid for by
trimming its first sentence; `--check-catalog` ok.
Before: the new `PropertiesLiveTests.A_material_list_miss_names_every_database_it_searched_with_its_count`
failed, "Assert.NotEmpty() Failure: Collection was empty". The old
`Assert.Empty(none.Databases)` in `Material_list_has_AISI_1020_at_7900_kg_per_m3` pinned the
fault and was removed.
After: it passes (a miss lists the same databases and counts as the call with no filter, the
standard one first); live, `Material_list_has_AISI_1020_at_7900_kg_per_m3`,
`Material_set_AISI_1020_gives_the_analytic_mass`, `Material_set_rejects_unknown_names_with_suggestions`,
`A_library_as_SOLIDWORKS_writes_it_keeps_its_bytes_but_for_the_added_material` and
`A_material_library_that_does_not_parse_is_a_named_io_error_in_material_add_and_material_set`
pass, 6/6 with the new one (`A_custom_material_is_listed_assigned_and_used_by_simulation_with_its_own_stiffness`
fails on this machine before and after, a custom library, not this change); unit tests
346/346; `PropertiesOfflineTests` 24/24; `ContractTests.Tool_catalog_follows_the_conventions`
and `--check-catalog` ok; `check_repo` ok.
Made stricter: nothing refused; a miss's `databases` is no longer empty.
Local: on `sw2023`, `fd64df4`; in build `20261122-194424`. Upstream: not yet
decided.
Held 2026-11-22: the scenario test of build `20261122-194424` (findings 2026-11-22, point
11): `material_list` with a filter that matches nothing listed in `databases` all five
databases with their counts (259, 207, 8, 2, 0), the warning "No material matches"; a filter
"606": 12 rows, one database, no row twice.

### S104 - `material_list` lists one database twice, under its short and its long path - CLOSED 2026-11-22

Sorted out of the entry of 2026-11-18 ("a weldment frame of 16 members in one custom
profile", point 3), on build `20261118-060615`. `material_list {filter:"606"}` returned 24
rows for 12 materials and two entries in `databases` of 259 each: one database under its 8.3
path and under its long path. Reported `ok`. Bears on S97 (the databases a miss reads).
Decided 2026-11-21 by the owner: taken into the run of 2026-11-21, group 5, with S97 (`## Run 2026-11-21`).
Cause: `MaterialCatalog.Load` dropped a repeated folder by its string, so the folder list of
File Locations naming one folder by its 8.3 path and by its long path read it twice.
Fix: each local folder is taken to its long form (`GetLongPathName`, through a resolver
`Load` takes, default the Win32 one) before the repeat check, so a database is read, listed
and searched once, under its long path; a network folder is left as written. Not in the
description.
Before: the new `PropertiesOfflineTests.One_folder_given_by_its_short_and_its_long_path_is_read_once`
(a temp folder named by `GetShortPathName` and by its long path) failed, "Assert.Single()
Failure: The collection contained 2 items", the two `MaterialDatabaseRow`s with the 8.3 and
the long path.
After: it passes, with `Folders_the_resolver_takes_to_one_long_path_are_read_once` (a fake
resolver, no 8.3 names needed), `PropertiesOfflineTests` 24/24; the live
`A_material_list_miss_names_every_database_it_searched_with_its_count` also asserts no path
twice in the databases; unit tests 346/346; `check_repo` ok.
Made stricter: nothing refused; a folder given twice under two spellings is read once.
Decided 2026-11-22 by the run, not the owner: the real 8.3 test skips when the volume has 8.3
names off, because the rule is also held by the fake-resolver test, which needs none.
Local: on `sw2023`, `fd64df4`; in build `20261122-194424`. Upstream: not yet
decided.
Held 2026-11-22: the scenario test of build `20261122-194424` (findings 2026-11-22, point
11): a filter "606" gave 12 rows from one database, none twice, and a miss listed each of the
five databases once. The doubled database cannot occur on this machine: its File Locations
name no folder by an 8.3 path, so the check saw one database once; the case itself is held by
the test that feeds a real 8.3 path (`GetShortPathName`) and the offline resolver test.

### S132 - S100 (a) and (b): `doc_open` and `doc_save` refused library feature parts (.sldlfp) - CLOSED 2026-11-24

Points (a) and (b) of S100 (S100 stays Open for (c) and (d)) (findings 2026-11-17 and
2026-11-18, on build `20261117-012659`): `doc_open` refused a `.sldlfp` ("can't open
'.SLDLFP' files"), so a weldment profile could not be read back, and `doc_save` refused any
extension but `.sldprt` for a part, so a custom profile could not be made end to end.
Decided 2026-11-21 by the owner: taken into the run of 2026-11-21, group 6; not a folder added to File Locations (`## Run 2026-11-21`).
Cause: `OpenKind.FromExtension` knew `.sldprt`, `.sldasm` and `.slddrw` only, and
`DocumentService.WithNativeExtension` took one extension per document type.
Fix: `doc_open` opens a `.sldlfp` as a part (`OpenDoc6`, `swDocPART`), its sizes listed as
`configurations` like any part's. `doc_save` modes `as` and `copy` take a `.sldlfp` path for
a part (`SaveAs3` to it, as SOLIDWORKS' Save As Lib Feat Part); a path with no extension
still gets `.sldprt`, and an assembly or drawing to `.sldlfp` is refused, the message naming
".sldprt or .sldlfp" for a part. Read back: after mode `as`, the document's path as
SOLIDWORKS reports it must be the `.sldlfp` written, else `VERIFY_FAILED` naming both.
Probed live first: `SaveAs3` of a part with one sketch to a `.sldlfp`, nothing selected,
writes a profile that reopens with its sketch and makes members (in
`docs/solidworks-api-notes.md`). Both descriptions name `.sldlfp`, paid for by trimming
("opens silently, no rebuild prompt", "in MMGS", "extension added if missing").
Before: the new `WeldmentLiveTests.A_shipped_profile_copied_to_a_temp_folder_opens_as_a_part_with_its_sizes`
(SOLIDWORKS' ISO square tube copied to a temp folder under another name) failed,
"doc_open can't open '.sldlfp' files."; the new
`WeldmentLiveTests.A_part_saved_as_sldlfp_reopens_with_its_sketch_and_makes_a_member` (a
40 x 20 rectangle on the front plane, saved by `doc_save {mode:"as"}`) failed, "doc_save
writes native .sldprt files; '.sldlfp' is not one."
After: both pass (the copy opens as an owned part at its path with "40 x 40 x 4" among its
configurations; the saved file reads back at the `.sldlfp` path, reopens as a part holding
`Sketch1`, and in a standard's type folder makes four members of 800 mm² section; a save
of the part to `.sldasm` is `INVALID_ARGUMENT`); live, with the tests that open and save
files, `A_one_size_profile_is_read_from_its_type_folder_and_refused_directly_in_the_standard_folder`,
the six open tests of `ExportLiveTests`, three of `M0LiveTests` (save copy, dirty, foreign),
`Saved_bodies_become_parts_with_their_volumes_and_an_assembly_of_them` and
`A_part_the_server_owned_comes_back_owned_when_its_owned_assembly_loads_it`, 14/14;
offline `ExportRequestTests` (`.SLDLFP` added to `Doc_open_knows_these_file_kinds`) and
`ContractTests` (`Tool_catalog_follows_the_conventions` among them), 31/31; unit tests
355/355; `--check-catalog` ok; `check_repo` ok.
Made stricter: a Save As to `.sldlfp` whose document does not read back as that file is
`VERIFY_FAILED`; nothing that passed is refused.
Decided 2026-11-24 by the run, not the owner: mode `copy` takes a `.sldlfp` too, not only
`as`, because both go through one extension rule, and a copy is how a part is kept while its
profile is written (the live tests' own profiles are written so).
Decided 2026-11-24 by the run, not the owner: `doc_save` selects nothing before it writes a
`.sldlfp`, because the probe showed the save with nothing selected keeps the sketch and makes
members; the read back is the document's path, not a reopen, because a reopen would close
the target mid-call, and the reopen with its sketch is the live test's.
Local: on `sw2023`, `af90447`; in build `20261124-174916`. Upstream: not yet
decided.
Held 2026-11-24: the scenario test of build `20261124-174916` (findings 2026-11-24, point 1): a
copy of SOLIDWORKS' shipped ISO square tube profile (`.sldlfp`) opened by `doc_open` as an
owned part, its four sizes as `configurations`; a new part, a 40 x 20 rectangle on the front
plane, saved by `doc_save {mode:"as"}` to a `.sldlfp`, read back at that path, `feature_list`
showing `LibFeat1` holding `Sketch1`; the same save to `.sldasm` refused `INVALID_ARGUMENT`
naming ".sldprt or .sldlfp"; put in a standard's type folder as `<type>\40 x 20.sldlfp`, it
made members of 800 mm² (128 000 mm³ on a 160 mm line).

### S133 - S100 (c): `feature_list` listed a library feature part's `LibFeat1` as a feature of its own - CLOSED 2026-11-24

Point (c) of S100 (S100 stays Open for (d)) (findings 2026-11-17 and 2026-11-18, on build
`20261117-012659`): `feature_list` on a library feature part (a weldment profile) showed its
marked sketch inside a `LibFeat1` of type `LibraryFeature`, where the FeatureManager shows the
sketch at the top level marked L and no `LibFeat1`; a session took it as the profile's fault
and made two hand re-saves.
Decided 2026-11-21 by the owner: taken into the run of 2026-11-21, group 6 (`## Run 2026-11-21`).
Cause: the row was listed like any feature, with nothing saying SOLIDWORKS makes it itself.
Fix: a `LibraryFeature` row of `feature_list` carries a new `note`: "SOLIDWORKS' internal
wrapper of the library feature, not a fault in the part; its profile is the sketch <the row's
children>." (or "it holds no sketch" when it has none). Other rows have no `note`. The
description is unchanged (a field in a result costs none of the catalog's budget).
Before: the new `WeldmentLiveTests.A_library_feature_part_lists_its_wrapper_as_internal_and_its_sketch_as_the_profile`
(a shipped profile copied to a temp folder, opened by `doc_open`) failed, "The LibraryFeature
row has no mark: ... LibFeat1 (LibraryFeature) [Sketch1] , Sketch1 (ProfileFeature) [...]".
After: it passes (the `LibFeat1` row carries the note naming `Sketch1`, no other row has one);
live, with the tests that call `feature_list`, `A_part_saved_as_sldlfp_reopens_with_its_sketch_and_makes_a_member`
and `M0LiveTests.A_boss_buried_in_the_body_is_rolled_back`, 3/3; unit tests 355/355;
`check_repo` ok. Not run: `--check-catalog` and the catalog test (no description changed).
Made stricter: nothing; a result gains an optional field.
Decided 2026-11-24 by the run, not the owner: the mark is a `note` on the row, not a changed
`type` or a hidden row, because the type and the row are what SOLIDWORKS reports and other
tools (`sketch_edit`, `feature_delete`) address by them; the profile's name is read from the
row's children, so a profile with no sketch says so.
Local: on `sw2023`, `def120d`; in build `20261124-174916`. Upstream: not yet
decided.
Held 2026-11-24: the scenario test of build `20261124-174916` (findings 2026-11-24, point 2):
the `LibFeat1` row carried the note "SOLIDWORKS' internal wrapper of the library feature, not a
fault in the part; its profile is the sketch Sketch1." on the shipped copy, on the copies
redrawn in point 3 and on the part saved in point 1; no other row had a note.

### S100 - Library feature parts (.sldlfp): not opened or saved through the server, read as a wrapper, and a save that lost the sketch, reported `ok` - CLOSED 2026-11-24

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
Decided 2026-11-21 by the owner: (a) to (d) taken into the run of 2026-11-21, group 6; not a folder added to File Locations (`## Run 2026-11-21`).
Decided 2026-11-21 in the plan, not the owner: (d)'s save of a library feature part that no longer holds a sketch is `VERIFY_FAILED`, the file named as written, not `ok` with a warning, because `ok` is what let the session close it.
(a) and (b) are done: moved to Waiting as S132, 2026-11-24 (the run of 2026-11-21, group 6).
(c) is done: moved to Waiting as S133, 2026-11-24 (the run of 2026-11-21, group 6).
(d) is done, the last point, so S100 moved here, 2026-11-24 (the run of 2026-11-21, group 6).
Probed live first, step by step on a copy of SOLIDWORKS' ISO square tube, each step's tree
read and a `.sldlfp` copy written and reopened: opened by `doc_open`, the whole sequence
(configuration table deleted, `config_add`, the four sizes deleted by `config_delete`,
`sketch_edit`, the 16 segments deleted and a rectangle added, `sketch_finish`, `doc_save`
in place) kept `LibFeat1` holding `Sketch1`, reopened too. Opened as the user would and
adopted by `doc_activate {allowEdit:true}`, as in the finding, `LibFeat1` was gone from the
tree right after the adoption, and the first copy was 42 030 bytes (the finding's 41 145);
deleting the table then removed the sketch, and `sketch_edit` found none. A `doc_save
{mode:"copy"}` of an owned profile to a `.sldprt` did the same; its `.sldlfp` copy after
reopened with an empty `LibFeat1`.
Cause: `doc_activate`'s backup is a `SaveAs3` copy to a `.sldprt`, and SOLIDWORKS' save of a
library feature part to a `.sldprt`, even as a copy, dissolves the library feature of the
open document; every later save of the `.sldlfp` writes it without its sketch, and
`doc_save` read back the file's size only. In `docs/solidworks-api-notes.md`.
Fix: the backup of a library feature part is a `.sldlfp` copy, which keeps it. `doc_save`
reads the part's library feature before and after the save: a part whose library feature
held something, or that is a `.sldlfp` document after the save, must still hold something
there, else `VERIFY_FAILED`, the file named as written ("... it was written without a sketch
in its library feature ...", or for a copy to a `.sldprt` "SOLIDWORKS dissolved the open
part's library feature ..."), with a hint to close without saving and reopen. No description
changed.
Before: the new `WeldmentLiveTests.A_library_feature_part_adopted_and_redrawn_keeps_its_profile_sketch_when_saved`
(the finding's sequence on a copy opened raw and adopted, saved in place, reopened) failed,
"Adopting the part left its library feature without 'Sketch1' (backup ...\lfp_adopt_tube.sldprt):
... Tables (ConfigTableFolder) [...], Sketch1 (ProfileFeature) [...] ..."; the new
`WeldmentLiveTests.A_save_that_leaves_a_library_feature_part_without_its_sketch_is_verify_failed`
(an owned copy saved as a `.sldprt` copy, then in place) failed, "doc_save copy to .sldprt
reported ok; the open part's library feature now: ... Sketch1 (ProfileFeature) ..." (no
`LibFeat1`).
After: both pass (the backup is a `.sldlfp`, the reopened profile's `LibFeat1` holds
`Sketch1`; the `.sldprt` copy is written and `VERIFY_FAILED` naming it, the save in place
after it `VERIFY_FAILED` "written without"); live, with the tests that save, adopt and open
library feature parts, the other four `.sldlfp` tests of `WeldmentLiveTests`, four of
`M0LiveTests` (save copy, dirty, dirty through another process, foreign),
`ExportLiveTests.A_saved_part_reopens_as_owned_target_with_its_geometry`,
`AssemblyLiveTests.An_assembly_adopted_after_the_server_saved_its_parts_does_not_read_as_changed_by_the_user`
and `TopDownLiveTests.Saved_bodies_become_parts_with_their_volumes_and_an_assembly_of_them`,
13/13; unit tests 355/355; `check_repo` ok. Not run: `--check-catalog` and the catalog test
(no description changed).
Made stricter: a save that leaves a library feature part with an empty library feature, or
none, is `VERIFY_FAILED` where it was `ok`; a copy of a plain part to a `.sldlfp` is not
checked (the open document stays the part it was).
Decided 2026-11-24 by the run, not the owner: the check is "the library feature holds
something", not "holds a sketch", because a library feature part other than a profile holds
features (a cut) whose sketch sits under them, and an empty `LibFeat1` is what the probe saw.
Decided 2026-11-24 by the run, not the owner: a copy to a `.sldprt` of a library feature part
is written and then `VERIFY_FAILED`, not refused before, because the file written is a good
part and the fault is the open document's, which the message names; refusing would also
refuse a part copy nobody saves the profile after.
Local: on `sw2023`, `af90447`, `def120d`, `4b13b83`; in build `20261124-174916`. Upstream: not yet
decided.
Held 2026-11-24: the scenario test of build `20261124-174916` (findings 2026-11-24, point 3):
(d), on both routes: the finding's sequence (the first configuration table deleted,
`config_add`, the four sizes deleted, `sketch_edit`, the 16 segments deleted, two rectangles
added, `sketch_finish`, `doc_save` in place, `doc_close`) on a copy opened by `doc_open` and on
a copy opened outside the server and adopted by `doc_activate {allowEdit:true}`, whose backup
is now a `.sldlfp`: both saved 66 to 67 KB (the shipped file 74.9 KB, the finding's broken one
41 KB), and reopened read-only `LibFeat1` held `Sketch1` with the eight new lines. On a third
owned copy, `doc_save {mode:"copy"}` to a `.sldprt` was `VERIFY_FAILED` naming the file written
and "SOLIDWORKS dissolved the open part's library feature (it held Sketch1)", and a `doc_save`
in place after it `VERIFY_FAILED` "written without a sketch in its library feature". (a) and
(b) held as S132 (point 1), (c) as S133 (point 2), so S100 closes whole. The same point's small
part (that save in place had already overwritten the only good file, the document opened by
`doc_open` having no backup) went to S134; unchanged from the finding, `feature_delete` of the
second table of the `Tables` folder is `FEATURE_FAILED` "Could not select", not reported `ok`.

### S102 - `weldment_member`'s "No profile" listed a one-configuration file it then refused - CLOSED 2026-11-24

Sorted out of the entry of 2026-11-18 (the scenario test of build `20261118-060615`), point
2; met in S91's own fix (Closed). With a one-configuration copy of a profile directly in the
standard folder, a wrong `type` was refused "Profiles: <the copy's name>, <the subfolder
type>". The copy is offered as a type although the next call with it is refused
(`INVALID_ARGUMENT` "has one configuration ... would make no member"). Not reported `ok`.

Proposed by the session: leave a one-configuration file in the standard folder out of the
list, or mark it ("not usable: one configuration").
Decided 2026-11-21 by the owner: taken into the run of 2026-11-21, group 6 (`## Run 2026-11-21`).
Decided 2026-11-21 in the plan, not the owner: the file is marked "not usable: one configuration", not left out, because a file left out of the list reads as one the server did not find.
Decided 2026-11-24 by the run, not the owner: a file with no configuration at all is marked "not usable: no configurations" by the same rule, because the call with it is refused as well, and a one-configuration file with a folder of its name is listed once, unmarked, as the folder's type, because that folder is what SOLIDWORKS offers.
Cause: `WeldmentSpec.PickProfile`'s "No profile" listed every file in the standard folder and every subfolder as a type, though a file there with fewer than two configurations is refused by the next call (S91).
Fix: the list reads the configurations of each file in the standard folder (only on this error path) and writes one with fewer than two as `<name> (not usable: one configuration)`, after the usable types, which stay listed alphabetically.
Before: `WeldmentLiveTests.A_one_size_profile_is_read_from_its_type_folder_and_refused_directly_in_the_standard_folder`, with a wrong type added after S91's case, failed: "Assert.Contains() Failure: Sub-string not found. String: "Profiles: flat bar, solo." Not found: "solo (not usable: one configuration)"".
After: it passes, with `Unknown_profiles_sizes_and_segments_are_refused_with_what_there_is` (live, 2/2); unit tests 355/355 (`WeldmentSpecTests.Unknown_types_and_sizes_list_both_layouts` changed: the marked list and a file with a folder of its name); `check_repo` ok. Not run: `--check-catalog` and the catalog test (no description changed).
Made stricter: nothing; a refused call's hint lists the same types plus the marked file.
Local: on `sw2023`, `3a19df8`; in build `20261124-174916`. Upstream: not yet decided.
Held 2026-11-24: the scenario test of build `20261124-174916` (findings 2026-11-24, point 4):
with a one-configuration copy put directly in a standard folder beside two type folders,
`weldment_member` with a wrong `type` listed "Profiles: <type>, <type>, <copy> (not usable: one
configuration)."; the call with the copy as `type` was refused with the folder-layout hint, as
before.

### S103 - `weldment_member`: lines in two directions that do not touch fail with the path hint, and the result does not say which way the section lies - CLOSED 2026-11-24

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
Decided 2026-11-21 by the owner: (a) and (b) taken into the run of 2026-11-21, group 6 (`## Run 2026-11-21`).
Decided 2026-11-21 in the plan, not the owner: (a) the server retries a refused group as one group per parallel set and says so, rather than the hint alone, because the hint would leave the split to every caller.
Decided 2026-11-24 by the run, not the owner: the split is tried only when the one group made no member body and its segments fall into more than one parallel set, and the groups go into one member feature, because a group that made members needs no split and one feature keeps the call one feature, as before.
Decided 2026-11-24 by the run, not the owner: (b) is `sectionMm` [x, y, z] on each member, the extents of its cross-section along the model axes (0 along the axis an axis-parallel segment runs on), for lines only (absent for an arc), because a body box mixes the length in and an arc's section turns along it.
Decided 2026-11-24 by the run, not the owner: the tool description is unchanged, because the new result fields carry their own names and the catalog is at its byte budget.
Cause: (a) SOLIDWORKS makes no member feature at all of a structural member group whose segments neither form a path nor are all parallel; the server passed every segment as one group, so only the Weldment feature was new and the check failed on it. (b) The result gave each member's body, lengths and volume, nothing of its section's orientation.
Fix: (a) when the one group made no body and `WeldmentSpec.ParallelSets` (lines parallel in either sense share a group, an arc alone) gives more than one set, the failed try is cleaned up back to the Weldment feature and the segments go again as one group per parallel set in one `InsertStructuralWeldment5`; the result gains `groups` (each group's segment names), a warning naming them, and the check ", in N groups, one per parallel set". A split that also fails names both tries ahead of the path hint. (b) Each straight member gains `sectionMm`, from the body's extreme points along each model axis' part across the segment (`WeldmentSpec.AcrossAxes`). `docs/solidworks-api-notes.md` §6 records both.
Before: `WeldmentLiveTests.Lines_in_two_directions_that_do_not_touch_are_made_as_one_group_per_parallel_set` failed "VERIFY_FAILED: weldment_member: 'Weldment' was created but has no faces, so it changed no geometry. Rolled back: deleted Weldment."; `WeldmentLiveTests.Each_member_gives_its_sections_extents_along_the_model_axes` failed "Assert.NotNull() Failure: Value is null" (the field there, not filled).
After: both pass, with the rest of `WeldmentLiveTests` and `DrawingLiveTests.Cut_list_table_lists_a_frames_members_and_weld_symbols_read_back` (live, 19/19); unit tests 357/357 (two new in `WeldmentSpecTests`: the parallel sets, the across axes); `check_repo` ok. Not run: `--check-catalog` and the catalog test (no description changed); the hint of a split that also fails (no live case of it).
Made stricter: nothing weakened; the one-body-per-segment check is unchanged and now names the groups when split.
Local: on `sw2023`, `285249f`; in build `20261124-174916`. Upstream: not yet decided.
Held 2026-11-24: the scenario test of build `20261124-174916` (findings 2026-11-24, point 5):
(a) a top-plane sketch of four lines, two along X and two along Z, none touching, default
segments, `corner:"none"`: 4 members in one feature, `groups` [[Line1, Line2], [Line3, Line4]],
the warning naming both groups, the check "in 2 groups, one per parallel set", volumes exact
(128 000 and 48 000 mm³); (b) `sectionMm` equal to each body's box across its line: lines along
X [0, 20, 40], along Z [40, 20, 0]; a vertical line on the right plane at `rotateDeg` 0 [20, 0,
40], at 90 [40, 0, 20], each matching `body_list`.

### S127 - A detail view's `centerMm` lies outside its own bounds and changes between `drw_view_detail` and `drw_info`, reported `ok` - CLOSED 2026-11-24

Sorted out of the entry of 2026-11-22 (the scenario test of build `20261122-194424`), point
6. A 2:1 detail of a 1:2 plan placed with no `atMm`: the PDF shows it centred at about [331,
178]. `drw_view_detail` gave `centerMm` [287.467, 251.306] with `boundsMm` [310.627,
155.566, 351.307, 201.046], the centre outside its own bounds; the next `drw_info` gave
`centerMm` [243.967, 328.306] (above the 297 mm sheet) and `boundsMm` [286.627, 133.966,
375.307, 222.646]. The bounds' centres are right ([331, 178.3]), their sizes differ between
the two calls (40.7 x 45.5 against 88.7 x 88.7; the detail circle drawn about 64 mm
across). A 1:1 detail given `atMm` [70, 220]: `centerMm` [194.941, 191.813], its bounds
centred at [69.9, 220.1]. Sections, auxiliary and named views read right. Reported `ok`
while wrong: a caller placing an annotation from `centerMm` would put it elsewhere.

Local, small, in group 5's files (`DrawingService.cs`, the detail view and `drw_info`).
Decided 2026-11-22 by the run, not the owner: a detail view's `centerMm` comes from the same
source as its bounds (or is the bounds' centre), and reads the same in `drw_view_detail` and
`drw_info`. Test to start from: a 2:1 detail placed by default: `centerMm` inside its
`boundsMm` and at their centre, equal across the two calls (fails now: outside its bounds,
and another value in `drw_info`). Default model.
Decided 2026-11-22 by the run, not the owner: taken into group 6, by the run's rule "a new
point, small and in the group's files, a later step of this run ... taken into the next
group", because it is a read of the detail view in the drawing files group 5 changed,
needing no choice of the owner's, found by group 5's check.
Decided 2026-11-24 by the run, not the owner: a detail view's `centerMm` is its outline's centre, not a corrected `IView.Position`, because the outline is what `boundsMm` reads and reads the same in both calls, while the position was stale in the call that made the view.
Decided 2026-11-24 by the run, not the owner: `drw_view_edit` move puts a detail view's outline centre on `atMm` (its position shifted by the offset read before the move), because its description says it moves the view's centre there, and a caller moving a detail from its new `centerMm` would otherwise have it land by that offset.
Cause: `ViewInfo` gave every view's `centerMm` as `IView.Position`. For a detail view that is where the model origin falls at the detail's scale (live: a 2:1 detail of a 1:2 view of a corner, the origin 80 x 40 mm off the outline's centre, outside it), and in the call that made the view it read another, stale value ([260.849, 161.306] there, [217.849, 138.306] in `drw_info`, the outline [253.509, 133.966, 342.189, 222.646] in both). The finding's differing bounds (40.7 x 45.5 against 88.7 x 88.7) did not recur in this step, the outline reading the same in both calls; they came back in group 6's release run (below), a matter of timing.
Fix: `ViewInfo` gives a detail view's `centerMm` as its outline's centre, so `drw_view_detail`, `drw_info` and `drw_view_edit` read one value. `MoveView` sets a detail's position to `atMm` plus the position's offset from the outline's centre, and checks the outline's centre read back within 0.05 mm ("outline centre read back"), else puts it back and refuses as before. `DrwView.CenterMm`'s doc and `docs/solidworks-api-notes.md` §9 record it.
Before: `DrawingLiveTests.Detail_center_is_the_center_of_its_bounds_in_both_reads` failed "drw_view_detail: centerMm [260,849, 161,306] is not the centre of boundsMm [253,509, 133,966, 342,189, 222,646] (drw_view_detail centerMm [260,849, 161,306], boundsMm [253,509, 133,966, 342,189, 222,646]; drw_info centerMm [217,849, 138,306], boundsMm [253,509, 133,966, 342,189, 222,646])".
After: it passes, its move of the detail included, with `Detail_of_a_corner_has_the_requested_scale`, `Detail_leaves_no_sketch_open`, `Views_move_with_their_aligned_children_and_are_deleted`, `A_section_with_no_atMm_is_placed_so_its_own_label_clears_the_title_block`, `View_moves_and_deletes_that_cannot_work_are_refused` and `Moving_an_annotation_or_a_view_s_label_onto_another_text_is_warned` (live, 7/7); unit tests 357/357; `check_repo` ok. Not run: `--check-catalog` and the catalog test (no description changed).
Made stricter: a detail's move is checked by its outline's centre, not only by the position SOLIDWORKS echoes back.
Local: on `sw2023`, `8f88347`, `05e4ee3`; in build `20261124-174916`. Upstream: not yet decided.
Again 2026-11-24 (group 6's release live tests): on `67e7395`, after S128 and S130, `DrawingLiveTests.Detail_center_is_the_center_of_its_bounds_in_both_reads` failed in the class run and alone after a restart, the same numbers both times: the centre equal, the outline 43 x 43 mm in `drw_view_detail` and 89 x 89 mm in `drw_info`, the finding's own symptom. Cause: SOLIDWORKS lays a new detail view out after the call that made it returns. A live probe in the creating call read the outline [276.309, 156.766, 319.389, 199.846] (43 mm, smaller than the 64 mm circle the 2:1 detail shows) and the stale position [260.849, 161.306] after `EditRebuild3`, twice in a row; after a 1.5 s pause, with no other call, [253.509, 133.966, 342.189, 222.646] (89 mm) and [217.849, 138.306], which every later read gives (enumerating the views, `GraphicsRedraw2`, `ForceRebuild3`, `drw_info`). So the 89 mm outline is the true one (it holds the circle; the stale one cannot), and S127's step passed by timing. S128 and S130 change neither the detail's place (the same centre both times) nor its read.
Fix: `DetailAsync` waits after its rebuild, up to 5 s in 100 ms steps (`AwaitDetailOutline`), until the detail's outline holds its circle at the detail's scale (2 x radius x detail scale / parent scale, 0.5 mm tolerance), then reads the view; if it never does, the result warns that the outline read smaller than the circle and to read it again with `drw_info`. `docs/solidworks-api-notes.md` §9 records the late layout.
Decided 2026-11-24 by the run, not the owner: the creating call waits for the outline to hold the detail circle, not for two equal reads or a fixed pause, because two reads in a row agreed while stale and a pause long enough on one machine may not be on another, while the circle is a size the true outline must reach.
Before: `DrawingLiveTests.Detail_center_is_the_center_of_its_bounds_in_both_reads` failed "boundsMm [276,309, 156,766, 319,389, 199,846] in drw_view_detail, [253,509, 133,966, 342,189, 222,646] in drw_info" (rerun in this step, the same).
After: it passes, with `Detail_of_a_corner_has_the_requested_scale`, `Detail_leaves_no_sketch_open`, `A_detail_with_no_atMm_is_placed_clear_of_a_hole_table`, `Detail_circles_off_the_part_or_out_of_range_are_refused` and `Views_move_with_their_aligned_children_and_are_deleted` (live, 6/6), and `A_section_with_no_atMm_is_placed_so_its_own_label_clears_the_title_block`, `View_moves_and_deletes_that_cannot_work_are_refused`, `Moving_an_annotation_or_a_view_s_label_onto_another_text_is_warned`, `An_auxiliary_with_no_atMm_goes_clear_along_its_alignment_or_says_there_is_no_room`, `Auxiliary_view_looks_square_onto_a_chamfer`, `Section_through_the_middle_is_a_hatched_section_view` and `A_parent_with_a_section_is_deleted_only_with_its_children` (live, 7/7), S127's and S128's other live tests; unit tests 358/358; `check_repo` ok. Not run: `--check-catalog` and the catalog test (no description changed).
Made stricter: the test checks that both reads' outline holds the 64 mm detail circle.
Held 2026-11-24: the scenario test of build `20261124-174916` (findings 2026-11-24, point 6): a
2:1 detail of a 1:2 plan with no `atMm`: `centerMm` [330.967, 178.306], the centre of
`boundsMm` [286.627, 133.966, 375.307, 222.646] (88.7 mm, holding the 64 mm circle), `drw_info`
the same values; `drw_view_edit move` to [340, 230] gave `centerMm` [340, 230] with the check
"outline centre read back", and the PDF showed it there.

### S128 - With no `atMm`, an auxiliary view lands over its parent and the title block though said to be a free spot, and the free-space search does not count tables - CLOSED 2026-11-24

Sorted out of the entry of 2026-11-22 (the scenario test of build `20261122-194424`), point
7; bears on S90 and S106, both held (S106's checks warned of each overlap, S90's section
label cleared the title block). (a) `drw_view_add auxiliary` with no `atMm`, whose
description says "default: a free spot (beside the parent for auxiliary)", put a 155 x 101
mm view on a 1:2 plan over the plan, the section below it and the title block, with the
three overlap warnings. Along its alignment there was no free spot on that sheet, so a
refusal or a hint (a smaller scale, `atMm`) would have served; `drw_view_edit move` off the
alignment is refused `VERIFY_FAILED` with the alignment hint (right). (b) A 2:1 detail with
no `atMm` was placed with its outline over a hole table's corner: the free-space search
(`Place`) counts the views, the border and the title block, not tables; the overlap check
after it does ("overlaps table"). Warned each time, so nothing hidden; the free spot the
description claims was not one.

Local, in group 5's files (`DrawingService.cs` `Place`, `DrawingLayout.cs`).
Decided 2026-11-22 by the run, not the owner: `Place` counts the sheet's tables as occupied;
where no free spot exists along an aligned view's alignment, the result says so with the
hint (a smaller scale, or `atMm`) instead of claiming a free spot. Test to start from: a
detail with no `atMm` beside a hole table, placed not over it (fails now: over the table's
corner); an auxiliary with no room along its alignment, the result giving the hint (fails
now: placed over the parent, the section and the title block, only the overlaps warned).
Default model.
Decided 2026-11-22 by the run, not the owner: taken into group 6, by the run's rule "a new
point, small and in the group's files, a later step of this run ... taken into the next
group", because it extends the placement S90 and S106 changed in group 5's files, needing
no choice of the owner's, found by group 5's check.
Decided 2026-11-24 by the run, not the owner: an auxiliary view made with no `atMm` is moved, once made (its size is known only then), to the first free spot along its alignment on the side of its parent it was made on, not across the parent, because across it the view would lie opposite its arrow.
Decided 2026-11-24 by the run, not the owner: where no spot is free the view stays where it was made and the result warns with the hint, not a refusal, because the plan asks the result to say so and the view itself is right.
Decided 2026-11-24 by the run, not the owner: the detail keeps its placement by the estimated size (now with the tables counted), with no re-placement by its real outline, because the plan's point is the tables and the overlap check after it still warns of any clash.
Cause: (b) `Place` (sections, details) built its free space from the views' outlines, the border and the title block, not the tables, though the overlap check after it counts them (S106). (a) An auxiliary view with no `atMm` was put a fixed reach (half the parent's larger side plus 30 mm) from its parent's centre along the alignment, whatever its own size, with no free-space search at all.
Fix: `FreeSpace` (`DrawingService.Placement.cs`) gives the area and what is taken, the tables on the current sheet included; `Place` uses it. `PlaceAlongAlignment` reads the new auxiliary's outline and its own label, finds the first shift along the alignment (`DrawingLayout.FirstFreeAlong`, 1 mm steps, from the parent's centre outward on that side) that clears the other views, the tables, the title block and the border, moves the view by it and reads the outline back (else puts it back and warns); with none it warns "No free spot of W x H mm for '<view>' along its alignment with '<parent>', on the side it was made on: it was left at [x, y] mm. Pass atMm, or give '<parent>' a smaller scale." `docs/solidworks-api-notes.md` §9 records it.
Before: `DrawingLiveTests.A_detail_with_no_atMm_is_placed_clear_of_a_hole_table` failed "the detail lies over the table: detail [286,808, 154,366, 334,688, 202,246], table [290,94, 170,72, 435,703, 183,2], front [159,06, 142,26, 270,94, 214,14]; warnings: 'Detail View A (2 : 1)' overlaps table 'DetailItem233' [290.94, 170.72, 435.703, 183.2]."; `DrawingLiveTests.An_auxiliary_with_no_atMm_goes_clear_along_its_alignment_or_says_there_is_no_room` failed "the auxiliary lies over its parent: aux [101,22, 64,42, 207,242, 170,442], front [159,06, 142,26, 270,94, 214,14]; warnings: 'Drawing View2' overlaps 'Drawing View1'." (its 2:1 part, the hint, not reached).
After: both pass (the auxiliary moved along its 45° alignment clear of its parent at 1:1, the hint at 2:1), with `Auxiliary_view_looks_square_onto_a_chamfer`, `A_section_with_no_atMm_is_placed_so_its_own_label_clears_the_title_block`, `Section_through_the_middle_is_a_hatched_section_view`, `Detail_of_a_corner_has_the_requested_scale`, `Detail_center_is_the_center_of_its_bounds_in_both_reads`, `Views_move_with_their_aligned_children_and_are_deleted` and `A_parent_with_a_section_is_deleted_only_with_its_children` (live, 9/9); unit tests 358/358 (`DrawingLayoutTests.An_aligned_view_moves_to_the_first_free_spot_along_its_alignment_or_to_none` new); `check_repo` ok. Not run: `--check-catalog` and the catalog test (no description changed).
Made stricter: an auxiliary's default place is checked for free space and its move read back; the free-space search counts tables.
Local: on `sw2023`, `abe65d4`; in build `20261124-174916`. Upstream: not yet decided.
Held 2026-11-24: the scenario test of build `20261124-174916` (findings 2026-11-24, point 7):
(b) the same detail, with a hole table on the sheet at [150.8, 45.0, 295.6, 118.8], was placed
clear of it; (a) an auxiliary with no `atMm` square to a slanted edge of the 1:2 plan warned
"No free spot of 155 x 110 mm … along its alignment … Pass atMm, or give 'Drawing View1' a
smaller scale.", with the overlap warnings; the same on a 1:5 plan alone was "moved 1.2 mm
along its alignment … to the first free spot", no overlap. Point 10 of the same entry went to
S135: a hole table's own default place is not searched.

### S130 - `drw_view_section` moves `atMm` onto the alignment line without a word - CLOSED 2026-11-24

Sorted out of the entry of 2026-11-22 (the scenario test of build `20261122-194424`), point
9. A section across a turned auxiliary view (S1's proof, point 5 (d)), `atMm` [340, 100]:
placed at [309.4, 32.3], the projection of that point on the line square to the cut through
the parent, partly off the sheet. The warnings said it ran off the sheet and over the title
block; none said the view was not where `atMm` asked, or why. Small; reported `ok`.

Local, small, in group 5's files (`DrawingService.cs`, the section).
Decided 2026-11-22 by the run, not the owner: `drw_view_section` warns when the view's centre
lands more than 1 mm from `atMm`, naming both points and why (the alignment with its parent).
Test to start from: point 9's case, a section given an `atMm` off its alignment: the warning
naming `atMm`, the centre and the alignment (fails now: none). Sonnet.
Decided 2026-11-22 by the run, not the owner: taken into group 6, by the run's rule "a new
point, small and in the group's files, a later step of this run ... taken into the next
group", because it is one warning in the section code group 5 changed, found by group 5's
check.
Decided 2026-11-24 by the run, not the owner: the section's centre is read as `centerMm` (its position), compared with `atMm` by the larger of the two coordinate differences, because that is the centre the result already reports, and an `atMm` on the alignment read back within 1 mm live (no warning).
Cause: `CreateSectionViewAt5` takes the position asked but SOLIDWORKS keeps a section aligned with its parent, so only the coordinate along the alignment line is kept; the code never compared the view made with `atMm`.
Fix: after the view is read, `SectionAsync` warns "'<view>' is at [x, y] mm, not at atMm [x, y]: a section view keeps its alignment with '<parent>' (on the line square to the cut through it), so only the position along that line is kept." when the centre lies more than 1 mm from `atMm` in either coordinate.
Before: `DrawingLiveTests.A_section_given_an_atMm_off_its_alignment_warns_of_where_it_went` failed "no warning that the section is not at atMm: atMm [305, 218,2], centre [305, 178,2]; warnings: " (empty).
After: it passes (a vertical cut with `atMm` 40 mm off the parent's row warned, `atMm` on the row not), with `Section_through_the_middle_is_a_hatched_section_view`, `A_section_with_no_atMm_is_placed_so_its_own_label_clears_the_title_block`, `Views_move_with_their_aligned_children_and_are_deleted` and `A_section_s_model_box_is_what_lies_behind_the_cut` (live, 5/5; the four others run before a wording change of the warning only); unit tests 358/358; `check_repo` ok. Not run: `--check-catalog` and the catalog test (no description changed). The finding's own case (a section across an auxiliary view) was not rebuilt: the same code path, the alignment being the parent's, shown here with a plain parent.
Made stricter: a section not where `atMm` asked is no longer silent.
Local: on `sw2023`, `8949e93`; in build `20261124-174916`. Upstream: not yet decided.
Held 2026-11-24: the scenario test of build `20261124-174916` (findings 2026-11-24, point 8): a
vertical cut on the plan with `atMm` [80, 140] was placed at [80, 178.2], the warning "… is at
[80, 178.2] mm, not at atMm [80, 140]: a section view keeps its alignment …", 3 hatched faces.
Point 11 of the same entry (that cut's letters over a table and a label, with "no overlaps")
went to S136.

### S131 - `drw_table_add {kind:"hole"}` with no `face` picks a tab with one hole over a floor with six countersunk holes - CLOSED 2026-11-24

Sorted out of the entry of 2026-11-22 (the scenario test of build `20261122-194424`), point
12. `drw_table_add {kind:"hole"}` on a top plan view with no `face`: one row, a Ø4.5 hole
through a 5 mm tab, its origin a vertex of the tab, while the floor facing the same way
holds six countersunk holes. The description says "default: the face toward the viewer with
the most holes"; whether countersinks count as holes there, or "toward the viewer" means the
nearest face, is not said. Small; reported `ok`.

Local, small, in group 5's files (`DrawingService.Tables.cs`).
Decided 2026-11-22 by the run, not the owner: the default face counts countersunk and
counterbored holes as holes, and the result names the face chosen and its hole count. Test
to start from: point 12's case, a part with a tab of one plain hole and a floor of six
countersunk holes facing the same way: the floor chosen, its six holes listed, the face and
the count named (fails now: the tab, one row). Default model.
Decided 2026-11-22 by the run, not the owner: taken into group 6, by the run's rule "a new
point, small and in the group's files, a later step of this run ... taken into the next
group", because it is the default face's count in the table code group 5 changed (S106),
needing no choice of the owner's, found by group 5's check.
Decided 2026-11-24 by the run, not the owner: a hole is a full circle on the face that leads, through coaxial cones (a countersink) and annular planes (a counterbore's floor), to a hole's cylinder, and a row's size may read either its bore or its diameter at the face, because the size column of a Hole Wizard hole reads the bore first while a cut's reads the edge, and the X/Y check stays as strict.
Decided 2026-11-24 by the run, not the owner: the face and its count are named for the default only, as a `resolved` entry "face (default)" with its id and a check line with the runner-up's count, because a given face is already echoed in `resolved` and the check line counts the rows.
Cause: `HolesOn` counted a circular edge of the face only when the face across it was a hole's cylinder; a countersunk hole's edge on the face meets a cone, so the floor counted 0 and the tab's plain hole won; a counterbored one counted with its counterbore's diameter.
Fix: `HolesOn` walks from each full circle down the coaxial faces (`BoreOf`: hole cylinders, cones, annular planes, up to six steps) and keeps the smallest hole cylinder as the bore; the size check accepts the bore or the diameter at the face. With no `face`, the result names the face chosen (`resolved` "face (default)", its id) and a check "face F<n> by default: toward the viewer, the most holes, <n> hole(s); next of <k> other face(s): <m>".
Before: `DrawingLiveTests.Hole_table_default_face_counts_countersunk_holes_and_names_the_face` failed with "rows [A1, 10, 10, <MOD-DIAM> 4,50 <HOLE-DEPTH> 4,00]; checks ['origin' picked at its sheet position | 1 row(s), each hole's X/Y from the origin and size read back | ...]" (the tab, one row).
After: it passes (a floor with six M4 countersunk holes and one M4 counterbore, a 5 mm tab with a Ø4.5 drill: the floor chosen, 7 rows, Y 15 and 45 from its corner, the face's id in `resolved` and the check), with `Hole_table_lists_the_hole_from_the_lower_left_corner_and_from_a_given_origin`, `A_table_over_a_view_gives_its_extent_and_is_warned_and_so_is_a_note_across_it`, `Tables_that_cant_work_are_refused` and `A_detail_with_no_atMm_is_placed_clear_of_a_hole_table` (live, 5/5); unit tests 358/358; `check_repo` ok. Not run: `--check-catalog` and the catalog test (no description changed). The finding's own part was not rebuilt.
Made stricter: a countersunk or counterbored hole is counted and checked, and the default face is named with its count.
Local: on `sw2023`, `67e7395`; in build `20261124-174916`. Upstream: not yet decided.
Held 2026-11-24: the scenario test of build `20261124-174916` (findings 2026-11-24, point 9):
`drw_table_add {kind:"hole"}` on the top plan with no `face` chose the floor, 6 countersunk
rows, `resolved` "face (default)" F25, the check "face F25 by default: toward the viewer, the
most holes, 6 hole(s); next of 4 other face(s): 1".

### S52 - The boxes in results are loose, and say nothing of it - CLOSED 2026-11-27

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
between. (d) New: `body_list`'s box is not the tight box (`IBody2.GetBodyBox` through
`SwExt.Box`, it seems), so two tools give two boxes for one body.
(d) is done: moved to Waiting as S126, 2026-11-22 (the run of 2026-11-21, group 5).
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
Decided 2026-11-21 by the owner: `body_list`'s box taken into the run of 2026-11-21, group 5, from `TightBox` (`## Run 2026-11-21`).
Closed 2026-11-27 on the owner's word (the opening answer of the run of 2026-11-27): every
point held or done, (a) and (b) in the scenario test of build `20261114-024455`, the STEP
case done as no fault of the box in the run of 2026-11-16, (c) held 2026-11-17, (d) split
out as S126 and closed 2026-11-22; nothing left open.

### S101 - `sketch_add` of a hundred entities took minutes and timed out - CLOSED 2026-11-27

Sorted out of the entry of 2026-11-18 ("a custom weldment profile with one size made to
work", point 5), on build `20261117-012659`. `sketch_add` of 112 entities (106 lines, 4 arcs,
2 circles) took 183 s in a library feature part and 254 s in a new part, `TIMEOUT` at 50 s
both times, completed later (`sw_status` `lateCompletions`, succeeded): about 1.6 to 2.3 s an
entity. A profile of a hundred segments needs a batch path or a longer limit for large adds.
`sketch_import` of the same geometry from a DXF was the way that worked (S99).

Proposed: a probe of where the time goes (the per-entity checks, S65 (e)'s merge pass, the
contour read), then a batch path or a limit that grows with the count. Local. Not decided.
Decided 2026-11-21 by the owner: taken into the run of 2026-11-21, group 7, a probe first (`## Run 2026-11-21`).
Carried 2026-11-27 into the run of 2026-11-27, group 1, its change in the tree, not committed, its first step, its decision as written: the run of 2026-11-21 closed on the owner's word (`## Run 2026-11-27`).
Probe (the run of 2026-11-21, on 2023 SP3): 100 lines in one `sketch_add` took 165 s, 150 s
of it in the server's own reads per entity, not in SOLIDWORKS' drawing: before and after each
entity it read every segment's name (to find the new ones), every point (to count the new
ones), the texts, and for the merge and relation labels who owns each point of the sketch
(115 s of the 165 alone). S65 (e)'s merge pass and the contour read are once a call and
were not the cost.
Cause: those reads grow with the sketch, so a call of n entities cost about n² reads.
Fix (`SketchService.AddAsync`, `SketchRelations.Made`): the sketch's segment names, its point
count (`GetSketchPointsCount2`) and who owns each point (`SketchRelations.PointOwners`, read at
its first use) are read once a call and kept up as each entity draws; the new segments are
those SOLIDWORKS lists past the old count (`NewSegments`), the whole list read again when it
is not in that order; texts are counted only for a text entity. `sketch_add`'s limit is the
default or 0.5 s a segment, the longer (`AddLimit`: a rectangle or slot 4, a polygon its
sides and circle, a polyline a segment a point). The result is unchanged.
Before: the new `SketchEditLiveTests.Sketch_add_of_a_hundred_lines_finishes_within_its_limit`
(a closed staircase of 100 lines, each its own entity, under 30 s) failed with the two `src`
files stashed: "'sketch_add' has been running for 50 s and exceeded its 50 s limit.
SOLIDWORKS is still executing it and it may still complete — do not simply retry."
After: it passes (the test 28.6 s in all, its fixture start included; 17 s for the add in the
probe, from 165 s), with the `sketch_add` tests that call the changed code:
`Sketch_add_lists_the_relations_and_merges_SOLIDWORKS_made` (S56),
`Sketch_add_warns_when_SOLIDWORKS_moves_what_it_drew_onto_a_model_edge` and
`Sketch_add_of_every_kind_drawn_apart_stays_where_it_was_drawn` (S80 (a)),
`Sketch_add_warns_when_a_rectangle_drawn_square_gets_an_equal` (S140),
`Sketch_add_merges_an_arc_drawn_on_a_polyline_s_ends_into_one_closed_contour` (S65 (e), S87),
`A_point_where_offset_arcs_centres_and_line_ends_sit_names_the_one_taken_and_warns_of_the_others`
and `ModellingLiveTests.Sketch_text_at_its_height_engraves_the_top_face_with_a_cut` (texts
counted), 8/8; unit tests 370/370; `check_repo` ok.
Made stricter: nothing refused; a large add gets a limit that grows with its segments.
Decided 2026-11-27 by the run, not the owner: no batch path, the reads made once a call
and the limit grown with the count, because the probe put the cost in the server's reads,
not in drawing one entity at a time, and the result and its checks stay per entity.
Decided 2026-11-27 by the run, not the owner: the review of the tree's change made the
order check of `NewSegments` also read the last old segment and, when the list is out of
order (a segment gone), read the sketch's names again, and made `AddLimit` count a
rectangle, slot and polygon by their segments, because the change as left missed a new
segment after a deletion and kept a stale name list, and counted a rectangle as one.
Local: on `sw2023`, in build `20261127-085502`. Upstream: not yet
decided.
Held 2026-11-27: the scenario test of build `20261127-085502` (findings 2026-11-27, point 1): a closed
staircase of 100 lines on the front plane, each its own entity, in one `sketch_add`: 20.66 s
(`sw_status` `recentOps`), no `TIMEOUT`, `endsMerged` 100, one closed contour of 100 segments.

### S123 - `sketch_finish` lists a dimension twice, and `sketch_edit` and `sketch_finish` count a sketch's segments apart - CLOSED 2026-11-27

Sorted out of the entry of 2026-11-22 (the scenario test of build `20261122-142805`), point
6, the "Small" in S93's case: a block's sketch reopened and the corner under one edge
chamfered 10 in the sketch. (a) Both `sketch_finish` results (the close that chamfered and a
second close unchanged) list `dimensions` `D3@Sketch1` twice, where `dimension_list` of the
sketch has the one dimension the sketch chamfer made. (b) `sketch_edit` of the same sketch
reads `segments: 7` where `sketch_finish` reads 5: the rectangle's two construction
diagonals counted by one and not the other. The model was right; two results that disagree
with each other and with `dimension_list`. Reported `ok`.

Local, small, in group 7's files (`SketchService.cs`, `SketchEditService.cs`).
Decided 2026-11-22 by the run, not the owner: (a) `sketch_finish`'s `dimensions` names each
dimension once; (b) the two tools count a sketch's segments alike, construction lines by
both or by neither. Test to start from: S93's case of point 6, the sketch chamfer:
`dimensions` lists `D3@Sketch1` once and the two counts are equal (fails now: twice, 7
against 5). Sonnet.
Decided 2026-11-22 by the run, not the owner: taken into group 7, by the run's rule "a new
point, small and in the group's files, a later step of this run ... taken into the next
group", because it is two
small reads of the sketch tools whose files are group 7's, found by group 4's check.
Carried 2026-11-27 into the run of 2026-11-27, group 1, its decision as written: the run of 2026-11-21 closed on the owner's word (`## Run 2026-11-27`).
Cause: (a) `FeatureVerifier.Dimensions` walks the feature's chain of display dimensions, and the
sketch chamfer's dimension comes up twice in it, so `sketch_finish` (and `sketch_info` and
any result that lists a feature's dimensions) named it twice, where `dimension_list` had the
one. (b) `sketch_finish` counts the segments that are not construction geometry (as its
`contours` do), and the reopen result of `sketch_edit` counted every segment, the
rectangle's two diagonals among them: 5 and 7.
Fix: `FeatureVerifier.Dimensions` lists a dimension once, by its full name; the reopen
result of `sketch_edit` counts the segments that are not construction geometry
(`SketchEditService.Reopened`), as `sketch_finish` does. `sketch_info` still lists every
segment with its `segmentCount`, construction ones included, as it lists them. No
description changed.
Before: the new `FeatureManagementLiveTests.Sketch_finish_names_each_dimension_once_and_counts_segments_as_sketch_edit_does`
(the 100 x 20 x 60 block's sketch reopened and one corner chamfered 10, `sketch_finish`,
reopened, `sketch_finish` again) failed: "Assert.Equal() Failure: Values differ Expected: 1
Actual: 2" (`D3@Sketch1` twice; its segment-count line is behind it in the test).
After: it passes, with the live tests that call the changed code (S93's
`Sketch_finish_names_a_feature_the_redrawn_sketch_left_with_a_rebuild_warning`,
`SketchEditLiveTests.Edit_reopens_a_finished_sketch_with_its_frame`,
`Edit_dimension_finish_rebuilds_the_extrude`, `Editing_tools_need_the_servers_open_sketch`,
and three `dimension_list` and tolerance tests of `FeatureManagementLiveTests`), 8/8; unit
tests 370/370; `check_repo` ok.
Made stricter: nothing refused; two results that disagreed now agree.
Decided 2026-11-27 by the run, not the owner: both tools count the segments that are not
construction geometry, not all, because `sketch_finish`'s count and its `contours` already
leave construction out and a count of the drawn outline is the one a caller compares with
a contour; `sketch_info` keeps listing all segments.
Local: on `sw2023`, in build `20261127-085502`. Upstream: not yet
decided.
Held 2026-11-27: the scenario test of build `20261127-085502` (findings 2026-11-27, point 2): a
100 x 60 x 20 block's sketch reopened (`sketch_edit` `segments: 4`, the centre rectangle's
diagonals not counted) and one corner chamfered 10 in the sketch: `sketch_finish` `segments: 5`
and `dimensions` `D3@<sketch>` once; reopened, `sketch_edit` `segments: 5`, `sketch_finish` the
same, `dimension_list` of the sketch the one dimension; volume 119 000 (120 000 - 1 000).

### S144 - A centre rectangle's centre point could not be referenced - CLOSED 2026-11-27

Sorted out of the entry of 2026-11-25 ("seven parts and two assemblies modelled from a
design doc"), point 3, on build `20261124-174916`. After `rectangle {center, widthMm,
heightMm}`, a dimension to `{at: <centre>}` was `ENTITY_AMBIGUOUS` "2 segments pass through
[46, -66]: Line5, Line6" (the construction diagonals), and to `{type:"sketch_point", at:
<centre>}` `ENTITY_NOT_FOUND` "No sketch point within 0.01 mm". Workaround: a corner
dimensioned instead, its equation offset by half the width. Bears on S140's wording ("a
centre rectangle's own centre point").
Proposed by the session: the result names the centre point, or the diagonals' intersection
is taken as a point reference. Local. Not decided.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 1: a probe first, the centre point resolved and named in the rectangle's result (`## Run 2026-11-27`).
Probe (2026-11-27, build `20261124-174916`, 2023 SP3): a centre rectangle leaves a free
sketch point at its centre, coincident with both diagonals (S140's wording holds). At the
exact centre, `{at}` and `{type:"sketch_point", at}` both took it, on the top plane, on a
face, after the rectangle was resized by dimensions, and in a sketch reopened after its
extrude. Both messages of the entry came back for a point 0.05 mm off the centre of a
100 x 10 rectangle along its long centre line: no sketch point within 0.01 mm, and both
diagonals pass within 0.01 mm of it, tied (each at 0.05 x 10/100.5 mm). The probe also
found a model point `{at:[46, 0, 66]}` (the same centre) to a dimension refused
`ENTITY_NOT_FOUND` "No sketch point within 0.01 mm of [46, 0, 66]": not projected.
Cause: (1) `{at}` sought a sketch point within the tolerance, then the segment through it,
and the diagonals cross where the point is: tied, they were refused, the point beside them
not offered; (2) the not-found named no point near; (3) the result gave no centre to aim
at; (4) `Ref` projected a model point only when every allowed kind was a sketch kind, and a
dimension also allows the origin.
Fix: (1) `SketchEditService.SegmentAt`, from a plain `{at}` that sought a point first:
segments tied through it that all pass through the sketch point nearest to it, within
1 mm (the reach `SegmentAt` already gives a nearest segment), give that point, with a
warning "entities[1]: no sketch point within 0.01 mm of [46.05, -66], and Line5, Line6 pass
through it; used the sketch point [46, -66] where they cross, 0.05 mm away."; (2)
`EntityResolver.SketchPointAt`'s not-found adds "; nearest: [46, -66] (on Line5, Line6),
0.05 mm away" (new `SketchRelations.Who`: a point's ends, else the segments it is
coincident with); (3) `sketch_add`'s centre rectangle gives `centerMm`, read from a
diagonal after the solve; (4) the origin no longer stops the projection. No description
changed; no read added to `sketch_add` beyond the one diagonal (S101).
Before: the new `SketchEditLiveTests.A_centre_rectangle_s_centre_point_is_named_and_dimensioned_to`
failed: "SwException : 'entities[1]': 2 segments pass through [46.05, -66]: Line5, Line6."
After: it passes, with the live tests that resolve sketch references and read
`sketch_add`'s result (the diagonals, the rectangle constrained in one call, point to
point dimensions, the origin, the relations of points, merge and coincident, both pierces,
the relations of every kind, relation misuse, a point followed through a merge, steps
checked first, several points at one place, a point in another sketch, the relations
`sketch_add` lists, the three S140 drift tests and `M0LiveTests.Top_plane_sketch_frame_is_x_0_minus_y`),
20/20; unit tests 370/370; `check_repo` ok.
Made stricter: nothing that passed is refused; a point beside tied segments is taken and
warned of where the call was refused, and a model point to a dimension is projected.
Decided 2026-11-27 by the run, not the owner: segments tied through the point give the
sketch point where they all cross, nearest and within 1 mm, with a warning, because the
call had asked for a point first, tied segments cannot be the answer, and `SegmentAt`
already takes a nearest segment within 1 mm with a warning; `{type:"sketch_point"}` keeps
its 0.01 mm and names the nearest point instead, the smaller change for every tool that
takes a sketch point.
Decided 2026-11-27 by the run, not the owner: the model point not projected is fixed here,
because the probe met it on the same reference and the rule `Ref` documents ("{at:[x,y,z]}
is a model point projected into the sketch") was false for dimensions.
Decided 2026-11-27 by the run, not the owner: the result field is `centerMm` (sketch
mm, model mm in a 3D sketch), named as `sketch_info`'s centres are.
Local: on `sw2023`, in build `20261127-085502`. Upstream: not yet
decided.
Held 2026-11-27: the scenario test of build `20261127-085502` (findings 2026-11-27, point 3): a
centre rectangle 100 x 10 off the origin gave `centerMm`; dimensions to `{at:<centre>}` and to
`{type:"sketch_point", at:<centre>}` were taken with no warning. On a second one, 0.05 mm off
its centre along the long centre line, `{type:"sketch_point"}` refused `ENTITY_NOT_FOUND`
naming "nearest: [-60, 40] (on Line11, Line12), 0.05 mm away", plain `{at}` took that point
with the warning "... used the sketch point [-60, 40] where they cross, 0.05 mm away.", and a
model point on the top plane (the same centre) was projected and taken.

### S155 - `sketch_add` did not merge the ends of centre-form arcs drawn end to end, and said nothing why - CLOSED 2026-11-27

Sorted out of the entry of 2026-11-25 ("a swept shell part with mirrored tabs"), point 6, on
build `20261124-174916`; S65 (e) again (closed 2026-11-16: `sketch_add` merges ends that
meet). Arcs drawn in centre form end to end at exactly the same coordinates were not merged
(the contour `ManyOpen`). Workaround that held on six junctions in three sketches: drawn 0.3
to 0.6 off and joined by `coincident`. In the same session a polyline whose end lay exactly
on an existing segment's end merged and said so ("merged onto ... (Line5 start)"), so the
merge works for lines and misses here.

Proposed: a probe of the case (whether the arcs' computed ends differ from the given
coordinates by more than the merge tolerance, or centre-form arcs are passed over); then
they are merged as lines are. Local. Not decided.
Decided 2026-11-27 by the owner: taken into the run of 2026-11-27, group 1: a probe first, centre-form arcs then merged as lines are (`## Run 2026-11-27`).
Probe (2026-11-27, build `20261124-174916`, 2023 SP3): every case at exact coordinates
merged, in one call and across calls: a slot of two lines and two centre-form arcs (front
plane, and top plane in model coordinates), the arcs drawn before the lines, arcs only end
to end and an arc on both their ends, an arc ending on a line at 53.13°, and arcs end to
end in a sketch reopened after its extrude; each read `closed` with every end in
`merged`. The merge pass does not pass centre-form arcs over. What stayed apart: tangent
arcs (r 120 and r 60) whose shared end comes from an angle typed to 2 decimals, 36.87 for
atan(3/4) = 36.8699°: their ends are 0.000107 mm apart, past the 1e-4 mm merge, and
nothing in the result said so; typed 36.8699 they merged.
Cause: an arc's end from its centre and angles moves radius × the angle's rounding (60 mm
× 1.79e-6 rad here), so ends the caller reads as one point are apart by more than the
merge tolerance, the same as two lines typed that far apart; the result named no such
pair, so the contour read `ManyOpen` with no reason.
Fix (`SketchService.MergeEnds`, `NearMisses`): after the merge pass, an end this call drew
that stays apart from another segment's end by more than 1e-4 mm and at most 0.05 mm
(`NearTol`) is named in one warning with the gap and where ("Arc2 start 0.000107 mm from
Arc1 end at [196, 72]"), with the ways to join them (the coordinates, or an arc's angles
typed to more decimals, or the 3-point form). Read from the ends the merge pass already
holds: no read of the sketch added (S101). No description changed.
Before: the new `SketchEditLiveTests.Sketch_add_names_ends_that_nearly_meet_and_stay_apart_as_centre_form_arcs_typed_rounded_do`
(the slot closed with 4 merges and no warning; the 36.87 arcs warned; the 36.8699 arcs
merged; a 0.3 mm gap not warned) failed: "Assert.Single() Failure: The collection did not
contain any matching items ... Collection: []" (no warning; the slot part passed).
After: it passes, with the live tests that call `sketch_add`'s merge:
`Merge_and_coincident_join_two_line_ends_into_one_point`,
`Constrain_follows_a_point_through_a_merge`,
`Sketch_add_lists_the_relations_and_merges_SOLIDWORKS_made`,
`Sketch_add_warns_when_SOLIDWORKS_moves_what_it_drew_onto_a_model_edge`,
`Sketch_add_of_every_kind_drawn_apart_stays_where_it_was_drawn`,
`Sketch_add_warns_when_a_rectangle_drawn_square_gets_an_equal`,
`Sketch_add_of_a_hundred_lines_finishes_within_its_limit`,
`Sketch_add_merges_an_arc_drawn_on_a_polyline_s_ends_into_one_closed_contour`,
`A_point_where_offset_arcs_centres_and_line_ends_sit_names_the_one_taken_and_warns_of_the_others`,
`Regions_count_what_a_later_sketch_add_drew` and
`AdvancedFeatureLiveTests.Sweep_of_a_slot_along_a_line_is_exact_through_its_start_face`,
12/12; unit tests 370/370; `check_repo` ok.
Made stricter: nothing refused; ends that nearly meet and stay apart are named, with the gap.
Decided 2026-11-27 by the run, not the owner: such ends are named in a warning, not merged
by a wider tolerance, because the probe found the merge already treats centre-form arcs as
lines (the plan's "merged as lines are" holds) and the gap is in the coordinates the
caller gave; merging it would move their geometry unasked, and the warning keeps the
result true with the smaller change.
Decided 2026-11-27 by the run, not the owner: the warning's band is above 1e-4 mm and up
to 0.05 mm, because an angle typed to 2 decimals moves a 500 mm arc's end up to 0.044 mm,
and a gap drawn on purpose to be joined by a relation (0.3 to 0.6 mm in the entry) is wider.
Local: on `sw2023`, in build `20261127-085502`. Upstream: not yet
decided.
Held 2026-11-27: the scenario test of build `20261127-085502` (findings 2026-11-27, point 4): a
slot of two lines and two centre-form arcs at exact coordinates, one call: 4 merges, closed.
Tangent arcs r 120 and r 60 sharing an end at an angle typed 36.87 warned "Arc4 start
0.000107 mm from Arc3 end at [196, 72]" with the three ways to join them; typed 36.8699 they
merged; two lines 0.3 mm apart were not warned.

### S140 - S80 (a) and (c): `sketch_add` said nothing when SOLIDWORKS moved what it drew, or made a rectangle drawn square equal-sided - CLOSED 2026-11-27

Points (a) and (c) of S80 (S80 stays Open for (b)) (findings 2026-11-16, 2026-11-17 and
2026-11-18, on builds `20261116-124830` to `20261118-060615`): a rectangle drawn on a planar
face, its bottom on the face's boundary edge, came back with "midpoint: Line3 end, a model
edge" among its relations and 2.5 tall where 3 was drawn, fully defined, reported `ok`; a
rectangle drawn 8 x 8 came back with "equal: Line2, Line1", a square for good. Both were
listed under the entity's `relations`; nothing said the shape was not the one asked.
Decided 2026-11-21 by the owner: (a) and (c) taken into the run of 2026-11-21, group 7, warnings; (b) stays Open (`## Run 2026-11-21`).
Cause: `AddToDB` on does not stop SOLIDWORKS relating and snapping what is drawn. Probed
live on 2023 SP3: a 195 x 5 x 10 block on the front plane, a corner rectangle from
[100, 40] to [-95, 43] on its front face got "coincident: a model entity, Line1 start" and
"midpoint: Line3 end, a model edge" (the face's 5 mm left edge, its midpoint 0.5 mm from the
corner) and read 2.5 tall in `sketch_info`; on a 200 mm block (no edge at x -95) the same
rectangle got only the coincident and stayed 3 tall. One drawn 1 mm over the first one's
bottom got "coincident: Line1, …" and was pulled onto it. Without any relation, a point is
snapped as it is drawn onto the x or y of another entity's point: in one call of fourteen
kinds, a polygon's vertex moved 0.28 mm onto the height of a slot's end 36 mm away and a
centre rectangle drawn 6 tall came out 7, no relation added (in
`docs/solidworks-api-notes.md`). sketch_add compared nothing with what it was asked.
Fix: `Draw` records each point as asked (sketch m): a line's `from` and `to`, a polyline's
points, a rectangle's two corners and the two it implies (a centre rectangle's four), a
circle's, polygon's and ellipse's centre, a polygon's first vertex, an arc's ends (and
centre), a slot's `from` and `to`, a spline's ends, a point's `at`. After the contour read
(whose automatic solve off/on solves the sketch) each is matched to the nearest point of its
entity's segments (ends and centres) or its free point; one farther than `MergeTol` (1e-4 mm)
moved. The interop-free rule (`SketchDrift.Moved`, `src/SolidWorksMcp.Core/Services/SketchDrift.cs`)
warns per entity: "entities[0] (rectangle) is not where it was drawn: SOLIDWORKS moved
corners[1] [-95, 43] to [-95, 42.5] (0.5 mm), a corner [100, 43] to [100, 42.5] (0.5 mm), by
its relations midpoint: Line3 end, a model edge. Draw it clear of other geometry and model
edges and place it by dimensions, or sketch_delete it." The relations named are the entity's
relations to other geometry that name a moved point or its segment (all of them when none
does; a centre rectangle's own centre point counts as its own); with none, "with no relation
to other geometry: snapped to it as SOLIDWORKS drew". (c): a rectangle whose relations hold an
`equal` warns "entities[0] (rectangle) was drawn square, and SOLIDWORKS made its sides equal
(equal: …): a width and a height driven apart later conflict. For a rectangle that may change
shape, draw it a closed polyline." (`SketchDrift.SquareEqual`). `sketch_start`'s description
said "Inference snapping is off while this server draws", false as the probe showed: now
"SOLIDWORKS may still snap; sketch_add warns of it." (shorter, no bytes added).
Before: the new `SketchEditLiveTests.Sketch_add_warns_when_SOLIDWORKS_moves_what_it_drew_onto_a_model_edge`
(the probe's block and rectangle) and `SketchEditLiveTests.Sketch_add_warns_when_a_rectangle_drawn_square_gets_an_equal`
(a centre rectangle 8 x 8 and a corner one 10 x 12) both failed, "Assert.Single() Failure: The
collection did not contain any matching items" (no warning).
After: both pass (the moved warning names corners[1], the implied corner, 0.5 mm and the
`midpoint` relation and not the corner's `coincident`; the rectangle 1 mm over the first is
warned naming "coincident: Line1, …"; one drawn clear is not; the 8 x 8 warns naming its
`equal`, the 10 x 12 does not), with the new
`Sketch_add_of_every_kind_drawn_apart_stays_where_it_was_drawn` (fourteen kinds, model
points among them, on a staircase with no two points within 4 mm in x or y: no warning),
3/3; live, the tests that read what `sketch_add` drew and relates (the relations and merges,
the arc merged on a polyline, the centre rectangle's diagonals, the rectangle constrained in
one call, the point followed through a merge, merge and coincident, the three region tests,
the offset rectangle, a text's position, the 3D sketch, and `M0LiveTests.Top_plane_sketch_frame_is_x_0_minus_y`),
13/13; unit tests 363/363 (new `SketchDriftTests`, 5); `--check-catalog` ok (165 tools,
247.6 KB); `ContractTests.Tool_catalog_follows_the_conventions` 1/1; `check_repo` ok.
`docs/tools.md` to regenerate at the release (`sketch_start` changed).
Made stricter: `sketch_add` warns when a point it drew is not where it was asked, and when a
rectangle drawn square was made equal-sided; nothing that passed is refused.
Decided 2026-11-24 by the run, not the owner: both are warnings, no result field and no
relation removed, because the relations are already listed per entity, the warning says
what they did, and removing a relation SOLIDWORKS made is a new behaviour the owner was not
asked (S80's plan line: no option to keep or drop relations in this run).
Decided 2026-11-24 by the run, not the owner: the points are read after the sketch is
solved, each asked point against the nearest point of its own entity, to `MergeTol`
(1e-4 mm), because the tools check sketch geometry to it and S65 (e)'s merge moves an end by
up to it; the points a kind does not hold as sketch points (a 3-point arc's mid, a spline's
inner fit points, a 3D rectangle's implied corners) are not compared.
Decided 2026-11-24 by the run, not the owner: a point moved with no relation is warned too,
as snapped, because the probe saw it (the polygon's vertex onto another entity's height) and
the shape is not the one drawn just the same.
Decided 2026-11-24 by the run, not the owner: `sketch_start`'s "Inference snapping is off
while this server draws" replaced, because the probe showed it false and the shorter text
costs no catalog bytes.
Local: on `sw2023`, in build `20261127-085502`. Upstream: not yet
decided.
Proof moved 2026-11-27: group 7 of the run of 2026-11-21 was never built (that run closed on the owner's word); this point goes into the build of the run of 2026-11-27's group 1, whose scenario test is its proof.
Held 2026-11-27: the scenario test of build `20261127-085502` (findings 2026-11-27, point 5): (a)
on a 195 x 5 x 10 block, a corner rectangle on its front face from [100, 40] to [-95, 43]
warned naming `corners[1]` [-95, 43] to [-95, 42.5] (0.5 mm), the implied corner [100, 43]
the same, and the `midpoint` relation to a model edge, not the corner's `coincident`; (c) a
centre rectangle 8 x 8 and a corner one 8 x 8 each warned naming their `equal`, a 10 x 12
did not.

### S141 - S82 (a): `sketch_constrain` angular where two lines meet only virtually turned the shape inside out, reported `ok` - CLOSED 2026-11-27

Point (a) of S82 (S82 stays Open for (b)) (finding 2026-11-16, on build `20261116-124830`):
an angular dimension between a 3 mm line and a long horizontal line 55 above it, meeting
only virtually, `valueDeg` 60 with `textAt` in the angle across the vertex: the line turned
to 120°, a trapezoid's 126 wide top became 6, reported `ok` with "geometry measures 60°";
only `changed` showed it. S42's sign of a line turned by more than 90° did not fire.
Decided 2026-11-21 by the owner: (a) taken into the run of 2026-11-21, group 7, a warning as S42 was decided; (b) stays Open (`## Run 2026-11-21`).
Cause: the value holds on the dimension's own angle, so the read-back and the measure pass;
the lines turned 60°, no line reversed, so `WarnFlips` named nothing, and an angle gives it
no value change to compare a move with. S42's mirrored-shape warning is skipped when
`textAt` picked the supplement. `dimension_set` of a sketch dimension read no sketch at all.
Reproduced live on 2023 SP3: a trapezoid on the front plane, bottom 70 (fixed), sides
leaning out 60° to a top 133.508 wide at y 55, the top's ends symmetric about a fixed
centreline, the right side a 3 mm line and a long one collinear with it; angular 60 between
the 3 mm line and the top, `textAt` [76.75, 37.7]: "the angle that was 119.999793°… then
60.000207°", the top 133.508 to 6.492, no warning (the finding's numbers). Then
`sketch_finish` and `dimension_set` 120 put it back, as the finding's workaround.
Fix: an interop-free rule (`SketchShrink.Warning`, `src/SolidWorksMcp.Core/Services/SketchShrink.cs`)
names each line less than half as long after as before, longer than 1e-6 m before, and,
for a length dimension, shorter by more than twice the value's change (a rectangle 100 set
to 40 is not warned); up to three lines, the largest drop first. `sketch_dimension_add` and
`sketch_constrain` (`AddDimension`) call it on the changed lines when a value was set:
"Step 1: After the angular dimension, a line is less than half as long as before: 'Line4'
133.509 to 6.491 mm. The value reads back, but the sketch may have turned inside out: if
that was not meant, sketch_finish, then dimension_set 'D1@Sketch1' back to 120."
`dimension_set` in a part reads the lines of each sketch whose dimensions it sets before
the values and after the rebuild (the sketch found again by name) and warns the same,
"After the dimension_set of 'D1@Sketch1', …: if that was not meant, dimension_set
'D1@Sketch1' back to 60.000207". A warning, the result unchanged; no description changed.
Before: the new `SketchEditLiveTests.An_angle_that_turns_the_shape_inside_out_warns_of_the_line_it_shrank`
(the trapezoid, `sketch_constrain`) and
`SketchEditLiveTests.Dimension_set_of_a_sketch_angle_that_turns_the_shape_inside_out_warns_of_the_line_it_shrank`
(the angle dimensioned between the rays, finished, `dimension_set` 120, then 60) both
failed, "Assert.Single() Failure: The collection did not contain any matching items" (no
warning; the top read 6.4916 in `changed`).
After: both pass, with the live tests that run `AddDimension` and `dimension_set` (the
dimensions that drive a rectangle, a diameter, angles, radial and point-to-point ones, the
supplement by `textAt` and without it, the origin, an over-defining one, S42's mirrored
shape in a dimension and in a step, the far move, a rectangle constrained in one call,
twenty steps on two dozen segments, an edited dimension rebuilding the extrude,
`FeatureManagementLiveTests`' three `dimension_set` tests and the in-context cover that
follows a grown box), 20/20; unit tests 367/367 (new `SketchShrinkTests`, 4); `check_repo` ok.
Made stricter: `sketch_dimension_add`, `sketch_constrain` and `dimension_set` warn when a
sketch line ends less than half as long as it was; nothing that passed is refused.
Decided 2026-11-24 by the run, not the owner: lines only, not arcs or circles, because a
radius or diameter set smaller shrinks an arc's length by more than twice its own change as
meant, and the finding's collapse was a line.
Decided 2026-11-24 by the run, not the owner: a length dimension's change explains a drop
up to twice itself (the sum of the length values set in that sketch for `dimension_set`),
an angle none, because a width set from 100 to 40 halves its lines as asked, and S42's far
move uses the same twice.
Decided 2026-11-24 by the run, not the owner: the warning names the way back as
`dimension_set` to the value before (after `sketch_finish` in an open sketch), because it is
the finding's own workaround, held, and the same for every cause.
Local: on `sw2023`, in build `20261127-085502`. Upstream: not yet
decided.
Proof moved 2026-11-27: group 7 of the run of 2026-11-21 was never built (that run closed on the owner's word); this point goes into the build of the run of 2026-11-27's group 1, whose scenario test is its proof.
Held 2026-11-27: the scenario test of build `20261127-085502` (findings 2026-11-27, point 6): a
trapezoid on the front plane (bottom 70 fixed, sides 60°, top 133.508 wide symmetric about a
fixed centreline, the right side a 3 mm line and a long one collinear): `sketch_constrain`
angular 60 between the 3 mm line and the top, `textAt` across the vertex, warned "'Line5'
133.509 to 6.491 mm ... dimension_set 'D1@Sketch1' back to 119.999999"; after
`sketch_finish`, `dimension_set` 120 silent, 60 warned the same with "back to 120", 120 again
silent, the extrude exact (55 964.845). No false alarm in the design-intent batches of a
part rebuilt in full (a width 120 to 130 to 120 among four values).

### S142 - S86 (a): `sketch_delete` removed a polygon's construction circle with its dimensions and relations, and said only "6 segment(s) gone" - CLOSED 2026-11-27

Point (a) of S86 (S86 stays Open for (b)) (finding 2026-11-16, on build `20261116-124830`):
`sketch_delete` of a polygon's six lines also removed its construction circle, which carried
the polygon's centre dimensions, its diameter and an `equal` to another polygon's circle;
`removed` listed it, the checks said "6 segment(s) gone", and the other polygon was left
with no size.
Decided 2026-11-21 by the owner: (a) taken into the run of 2026-11-21, group 7, a warning; (b) stays Open (`## Run 2026-11-21`).
Cause: SOLIDWORKS deletes a polygon's construction circle with its last line; `sketch_delete`
compared the sketch before and after only to check that the named segments were gone, so a
segment that went unnamed was in `removed` and nowhere else.
Fix: an interop-free rule (`SketchRemoved.Warning`, `src/SolidWorksMcp.Core/Services/SketchRemoved.cs`)
builds the warning from the segments removed that were not named, the relations that
touched them and the dimensions the sketch lost. `sketch_delete` reads the sketch's
relations (`SketchRelations.All`, each with the keys of what it relates), its dimensions and
who owns each point before the delete; after it, for each segment gone that was not named,
`SketchRelations.Touching` keeps the relations on that segment or on a point only the
removed segments owned. Read on 2023 SP3: "sketch_delete also removed a segment not named
in the call: 'Arc1' (construction circle, diameter 24 mm). Relations that tied it went too:
tangent: Line1, Arc1; coincident: origin, Arc1 centre; diameter: Arc1. The sketch lost
dimension(s) in the call: D1@Sketch1 = 24 mm. What they sized or held may now be
under-defined: sketch_info shows what is left." A warning, the result unchanged (`removed`
and the checks as before); no description changed, a call that names everything it removes
warns of nothing.
Before: the new `SketchEditLiveTests.Delete_of_a_polygons_lines_warns_that_its_construction_circle_went_too`
(a hexagon, its circle dimensioned Ø24, its six lines deleted) failed, "System.ArgumentNullException
: Value cannot be null. (Parameter 'collection')" at `Assert.Single(r.Warnings!)` (no warning).
After: it passes, with `Delete_removes_the_segment`, `Delete_needs_existing_segments_and_an_open_sketch`,
the five live tests that delete points or a 3D segment (`Pierce_puts_a_point_where_a_line_of_another_sketch_crosses_the_plane`,
`Pierce_puts_a_point_where_a_model_edge_crosses_the_plane`, `Circular_pattern_rotates_lines_groups_and_points_about_the_center`,
`Mirror_about_a_slanted_centerline_reverses_an_arc_and_copies_a_point`, `Construction_relation_and_delete_work_in_a_3D_sketch`)
and `WeldmentLiveTests.A_library_feature_part_adopted_and_redrawn_keeps_its_profile_sketch_when_saved`,
9/9; unit tests 370/370 (new `SketchRemovedTests`, 3); `check_repo` ok.
Made stricter: `sketch_delete` warns when a segment that was not named went with the ones that were; nothing that passed is refused.
Decided 2026-11-24 by the run, not the owner: the dimensions listed are all the sketch lost
in the call, not only the circle's, because a sketch dimension does not say what it
measured (no attached entity is read), and the warning says so ("the sketch lost
dimension(s) in the call"); the relations are the circle's own, exact.
Decided 2026-11-24 by the run, not the owner: a warning for any segment removed unnamed (a
polygon's circle is the case met, a slot's or a pattern's centre geometry would be
another), because the check is the same and the cause is SOLIDWORKS', not the tool's.
Decided 2026-11-24 by the run, not the owner: the relation that is the dimension itself
("diameter: Arc1") stays in the list, because filtering it by type name would hide a
relation of a type not met yet.
Local: on `sw2023`, in build `20261127-085502`. Upstream: not yet
decided.
Proof moved 2026-11-27: group 7 of the run of 2026-11-21 was never built (that run closed on the owner's word); this point goes into the build of the run of 2026-11-27's group 1, whose scenario test is its proof.
Held 2026-11-27: the scenario test of build `20261127-085502` (findings 2026-11-27, point 7): a
hexagon, its construction circle dimensioned Ø24, its six lines deleted: `removed` listed the
circle, and the warning named it ("construction circle, diameter 24 mm"), the relations that
went with it (tangent, diameter) and `D1@<sketch>` = 24 mm lost.

### S160 - S80 (d): `sketch_add` of a corner rectangle on model edges' projection failed as degenerate geometry, its polyline drawn at once - CLOSED 2026-11-27

Point (d) of S80 (S80 stays Open for (b)) (finding 2026-11-24, on build `20261124-174916`): a
`rectangle {corners}` on the Front plane, its corners on the projection of two model edges
(a plate's outline over existing members), failed `FEATURE_FAILED: SOLIDWORKS could not
create the rectangle`, the hint naming degenerate geometry, which it was not; the same four
corners as a closed `polyline` were drawn at once. Not reported `ok`.
Decided 2026-11-27 by the owner: (d) taken into the run of 2026-11-27, group 1, a probe first; the rectangle then drawn as the polyline is, or the hint naming model edges under its corners; (b) not in it (`## Run 2026-11-27`).
Probe (2026-11-27, build `20261124-174916`, 2023 SP3): corners on edges in the sketch plane,
on edges 10 mm behind it, on vertices of posts through it and on a block's top edge 142 mm in
front of it were all drawn, related "coincident: a model entity" or "a model edge". It failed
with a frame outline 142 mm in front of the plane, a post rising from its top edge: a
rectangle whose first corner lay where that top edge and the post's inner edge meet in
projection, [[-755, 964], [-105, 974]] and [[-755, 964], [-700, 974]] alike, failed every
time; the same corners in the other order ([[-700, 974], [-755, 964]]) were drawn, with
"coincident: a model entity" at that corner. `CreateCornerRectangle` returns nothing and
leaves nothing in the sketch; a line from the same corner along the top edge was drawn.
Cause: SOLIDWORKS' inference at the first corner of `CreateCornerRectangle`, on two model
edges meeting there in projection (`AddToDB` on does not stop it, S140), makes it return
nothing; the hint then blamed the coordinates.
Fix: `SketchService.Draw`, a 2D corner rectangle `CreateCornerRectangle` returns nothing for
(neither side of zero length) is drawn as a closed polyline is (`CornerLines`): four lines
from the first corner round, each made horizontal or vertical (`sgHORIZONTAL`,
`sgVERTICAL`), as the corner rectangle's own relations are; its result as a rectangle's
(sides, relations, S140's drift check of its four corners), with the check "drawn as 4
lines, made horizontal and vertical: SOLIDWORKS refused it as a corner rectangle, as it does
a first corner where model edges meet". A line of them that fails deletes the ones drawn and
is refused with the old hint. A rectangle SOLIDWORKS draws is drawn as before. No
description changed; no read added (S101).
Before: the new `SketchEditLiveTests.Sketch_add_draws_a_corner_rectangle_SOLIDWORKS_refuses_on_model_edges_as_four_lines`
(the frame and the plate's rectangle of the probe) failed: "SwException : SOLIDWORKS could
not create the rectangle."
After: it passes (4 lines on the corners asked, 2 horizontal and 2 vertical, 4 points, no
merge, one closed contour, no drift warning; a rectangle drawn clear of the edges has no
such check), with the live tests that draw and read rectangles (the relations and merges
`sketch_add` lists, the three S140 drift tests, the rectangle constrained in one call, the
dimension on a 100 wide rectangle), 7/7; unit tests 370/370; `check_repo` ok.
Made stricter: nothing that passed is refused; a corner rectangle refused before is drawn.
Decided 2026-11-27 by the run, not the owner: the four lines only when SOLIDWORKS returns
nothing, every rectangle it draws kept as it is, because that keeps the relations it makes
(the `equal` of one drawn square, S140 (c)) and every result the tests read, the smaller
change.
Decided 2026-11-27 by the run, not the owner: the lines, not the corners swapped, because
the probe showed the inference at the first corner only, and the other corner could stand
on such edges too; the polyline was drawn at once in the finding.
Decided 2026-11-27 by the run, not the owner: a check on the entity, not a warning, because
the shape is the one asked (S140's drift check still compares its corners), only the route
differs.
Decided 2026-11-27 by the run, not the owner: the centre rectangle left as it is, because no
case of it failing was met.
Local: on `sw2023`, in build `20261127-085502`. Upstream: not yet
decided.
Held 2026-11-27: the scenario test of build `20261127-085502` (findings 2026-11-27, point 8): held
on its own case. A frame outline (a top member and two posts) 142 mm in front of the front
plane, extruded 40; on the front plane a corner rectangle from [-755, 964] to [-105, 974]:
the check "drawn as 4 lines, made horizontal and vertical: SOLIDWORKS refused it as a corner
rectangle ...", 2 horizontal and 2 vertical, 4 points, one closed contour, no drift warning;
a rectangle clear of the edges drawn as before. The fallback was met on its own case only: a
frame of another shape (an L outline 132 to 142 mm in front, corners at its joint) was drawn
by SOLIDWORKS as a corner rectangle, with "coincident: a model entity", so the four lines
were not needed there, as the probe's record says.

## Not items

- **SW 2023 interop patch** (`27e8c25`): `swCustomInfoEquation` case removed,
  `InsertUntrimSurface` instead of `InsertUntrimSurface2`. Not sent upstream (upstream
  supports 2025 and later; ROADMAP T2.3 is theirs). Rebased with `sw2023` each update.
- **A preference changed once after a fresh start** (the run of 2026-11-13): a live test's
  preference check found Integer230 (rebuild on activation) 0 → 1 and Double2 (STL
  deviation) changed, on the code before the run too; its reruns passed. Decided
  2026-11-13 by the run, not the owner: not an item, because it was seen once, not
  reproduced and not tied to a call; an item if it is seen again. Seen again 2026-11-22 in
  the run of 2026-11-21: filed 2026-11-27 as S158.

## Run 2026-11-13

**The run of 2026-11-13** (planned with the owner; one session, subagents doing the
work). Every item below is decided; after the one opening question nothing in it waits
on the owner.

How it runs:

- **Unattended** (the owner, 2026-11-13: they leave the session once it starts). After
  reading this brief the session asks the owner **one** question, in one
  `AskUserQuestion` call, and then runs to the end without stopping or asking again.
  That question holds everything the run would otherwise stop for: the release's tag
  and registration (CLAUDE.md step 4, every project named with the build it runs), the
  push of `sw2023` before the release, and anything in this plan the session reads as
  unclear. **Plugin questions**: those of the session start (`git-workflow`'s waiting
  work, `clockify`'s timer, `core`'s session note) go into that one call, up to four
  questions, or are settled by this brief when it answers them; every later one
  (`clockify`'s offer after each commit, `git-workflow`'s placement and push notes) is
  not asked, as the opening sentence rules out questions for the turn ("tanpa nanya
  apa-apa"), and each is named in one line of the report. The whole run is that one
  turn: the answer to the opening question comes back inside it.
- **A decision met mid-run is the session's own**: it takes the option that keeps the
  server's checks honest and its results true, the smaller change when two are equal,
  and goes on. Each such decision is written into its item as "Decided 2026-11-13 by
  the run, not the owner: …, because …", and listed in the run's report, so the owner
  can review or reverse it.
- **Never stop on one item.** A fix that does not hold after a reasonable try (its test
  still failing, a cause not found) is left Open with what was tried written into it,
  its partial code not committed, and the run goes to the next. SOLIDWORKS hung or
  gone: restart it and go on. A permission prompt or a dialog nobody answers is the one
  thing the session cannot pass; it writes where it stood into this section first.
- **The report at the end**, in Indonesian, in the reply and in a line here: per item
  Waiting with its commit, Open with why, or Closed (its test did not fail); the
  decisions the run took; the build and the sentence for the scenario test.
- **One SOLIDWORKS, so one batch at a time.** Live tests drive the one SOLIDWORKS on this
  machine, and most batches touch the same services (`FeatureService`, `SketchService`,
  `AssemblyService`, `SwContext`), so the batches go one after another on `sw2023` in the
  main tree, not in parallel worktrees. The main session stays thin: it hands the work
  to subagents, reads each short report and the diff, and commits. **One fresh subagent
  per item** (the owner, 2026-11-13, mid-run), or per named pair of small items on the
  same files, still one after another: one agent resumed item by item through batch 2
  reached ~450k tokens, a fresh one per item ~160-180k.
- **Per item**, as `CONTRIBUTING.md` and the owner's rules give it: first the live test
  that shows the fault, run on the current code (an item whose test does not fail is
  closed instead, with the test kept if it is worth keeping); then the fix; then that
  test and the live tests that call the changed code (`--filter-method`, ≈18 s each, never
  the whole area); unit tests offline; `python tools/check_repo.py`. A probe item (marked
  so below) is a live probe first, its result written into the item; its fix follows in
  the same batch when the decision below covers it.
- **Commits**: one per item, its fix and its note moving the item to Waiting with the
  commit, as `git-workflow:pr` gives a commit on `sw2023` (no trailer). No push until the
  release; no branch, no PR upstream (each item's upstream line stays "not yet decided").
- **A subagent's brief**: its item as written here and in the item, the decision on it,
  the files likely touched, and these rules: read `CLAUDE.md`, `CONTRIBUTING.md`, the
  parts of `docs/solidworks-api-notes.md` it needs, and this brief; never switch
  branches, touch `.claude/worktrees/*`, commit, push, tag or reset; a stash only by
  name and file. SOLIDWORKS 2023 (`C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\SLDWORKS.exe`)
  started if not running, live tests as
  `$env:SWMCP_LIVE_TESTS=1; dotnet test --project tests/SolidWorksMcp.LiveTests --filter-method "<Class>.<Method>"`.
  The test rule above, a failing test that is not the item's said as such. Then
  `open_items.md`: the item moved to the end of Waiting with its cause, fix, the test
  before (its message), the tests after with counts, and "Local: on `sw2023`, in the
  commit that moves it here; not yet in a build. Upstream: not yet decided."; a partly
  done item stays Open with what was tried; the `**Open: …**` line updated and `python
  board.py --check` clean. A description changed: short (the tool list is at its byte
  budget, `--check-catalog` and `ContractTests.Tool_catalog_follows_the_conventions`
  run each time). `findings.md` is not the subagent's to edit. Report, ten lines: the
  outcome, the failing test before, the tests after with counts, every file changed,
  its "Decided … by the run" lines, a proposed commit subject and body. Findings that
  name where they came from stay out (`findings.md`'s top).
- **Descriptions changed** → `docs/tools.md` regenerated once, in the release batch.

Batches, in order (the owner's order of 2026-11-11 kept: S47, decided, first; then
modelling, the largest first; the release last):

1. **S47**, the sheet's frame at a sheet scale other than 1:1 (decided 2026-11-12).
2. **Results that are wrong, modelling**: S66 (a stale edge id resolving to another edge:
   `EntityRegistry`'s signature check), S51 (all four points, `flip` per edge and the edit
   of a chamfer's edges included), S53 (extrude along the vector), S52 (tight boxes), S50
   (a distance mate's side).
3. **Equations**: S54, S58, S48 (d), S63.
4. **Sketches**: S56, S55 (probe the cost of a `symmetric` step first), S64 (e, f).
5. **Features, smaller**: S64 (a, b as a probe, d, g), S65 (a), S59 (all four).
6. **Session, documents, assemblies**: S57, S60 (a as a probe; b, c, d), S61 (b, c), S62
   (a, c as a probe, d, e), S49, S48 (a to c).
7. **Release**, as `CLAUDE.md` gives it: `docs/tools.md` regenerated, the live tests of
   every area this run touched in full, unit tests, the push, publish, `--check-catalog`,
   then the tag and the registration as the opening question's answer gave them. The scenario test is not
   part of this run: once the build is registered, the session ends by giving the owner
   the opening sentence for it, naming the build and the items to check.

If the session ends before the last batch, this list and the items' Waiting lines say
where the next one picks up.

**Where the run stands** (the seventh session, 2026-11-14, finished batch 7, the release;
the run is done, its scenario test run and sorted 2026-11-14, as the Open line says).
**The run's report** (2026-11-14): every planned item Waiting with its commit, in build
`20261114-024455`, but for the parts left open (S60 (a), S61 (a), S62 (b), S64 (b, c),
S65 (b to d)); none closed for a test that did not fail. New open items from its own
tests: S67 to S71. The decisions it took are its "Decided 2026-11-13/14 by the run, not
the owner" lines in each item. Scenario test: run 2026-11-14, sorted as the Open line says.
Committed on `sw2023`: batch 1, S47 `f71b034`; batch 2, S66 `657fc23`, S51
`16699e2`, S53 `59beaae`, S52 `8dff953`, S50 `1bc3cbb`; batch 3, S54 `9939f28`, S58
`32698d5`, S48 (d) `ffde4db`, S63 `e642451`; batch 4, S56 `0acb318`, S55 `1861854`, S64
(e, f) `9e65c62`; batch 5, S64 (a) `576a1c2` (with (b)'s probe: not reproduced in 17
cases, stays open), S64 (d) `fe1a1af`, S64 (g) `9a46c32`, S65 (a) `fd9bd83`, S59 (a, c,
d) `3ca75cd`, S59 (b) `42589af` (S59 Waiting whole); batch 6, S57 `3a58652`, S60 (b, c,
d) `7601276` ((a) probed, not reproduced, stays Open with a usage line), S61 (b, c)
`a81f93d` ((a) stays Open), S62 (a, d, e) `af3598c` ((c) probed: SOLIDWORKS itself reads
a limit-mated component fully constrained, no fault, test kept; (b) stays Open), S49
`2a41e9e`, S48 (a to c) `ef358df` (S48 Waiting whole). S64 now holds only (b) and (c),
S65 (b) to (d), S60 (a), S61 (a), S62 (b). The four things seen in the run's own tests
are S67 to S70, open; the preference change seen once is under *Carried, not items*.
**Batch 7, the release, split so any session can pick it up** (the owner, 2026-11-13:
one fresh subagent per class or group of small classes, a line here after each, so a
session that hits its limit can be closed and the next goes on from the first step not
ticked). Done: `docs/tools.md` regenerated (`solidworks-mcp.exe --tools-md docs/tools.md`
on a Debug build; its diff matches the descriptions changed since build 091908), unit
tests 284/284, `check_repo` ok, `--check-catalog` ok (165 tools). Live classes, each run
in full (`--filter-class "SolidWorksMcp.LiveTests.<Class>"`), as passed/failed/skipped:
- [x] `M0LiveTests` 23/0/0, `ContractTests` 7/0/0
- [x] small classes: `ModellingLiveTests` 9/0/0, `ConfigurationLiveTests` 7/0/0,
  `AssemblyStructureLiveTests` 2/0/0, `TopDownLiveTests` 4/0/0, `HardwareLiveTests`
  3/0/0, `MoldingLiveTests` 4/0/0, `RecognitionLiveTests` 1/0/0,
  `SimulationExtrasLiveTests` 2/1/0 then 3/0/0 (the nonlinear study's solver returned 14
  once and passed alone; no commit since build 091908 touches it), `TemplateLiveTests`
  and `VisualsLiveTests` 0/1/0 then 1/0/0 each (the fixture refused the document the
  failed study left open). The runner now restarts SOLIDWORKS after a failed class.
- [x] `PartFeatureLiveTests` 47/0/0
- [x] `AdvancedFeatureLiveTests` 29/0/0
- [x] `FeatureManagementLiveTests` 33/0/0, in three chunks with a restart before each
  (its first run had stopped on memory: the runner read a stale second SLDWORKS process,
  so the live one, kept ~40-50 MB per test by SOLIDWORKS, was never restarted between
  classes; the heaviest test, S57's `The_servers_own_failed_kept_…`, peaks ~1.5 GB over
  its start with two parts open, no fault in the server)
- [x] `SketchEditLiveTests` 74/0/0, counting `Info_needs_an_open_sketch_or_a_sketch_name`
  as passed. It failed each run on its dialog check after its asserts held: SOLIDWORKS
  opens an untitled #32770 ("Title | Description | Button1 | Button2 | Hyperlink Text
  1/2") at the end of its start, with the Welcome screen, and closes it ~8-10 s later;
  the runner restarted SOLIDWORKS before each chunk, and this ~5 s test ended while the
  popup was up. Shown by a window watcher with no calls; the same on build 091908's code;
  passes on both after SOLIDWORKS has been up 20 s. The restart script now waits for it.
  Decided 2026-11-13 by the run, not the owner: counted as passed, no test change in this
  run, because no server call opens it; the fixture could assert only dialogs a test
  opened (those open at its start left out), as calls already mark pre-existing ones.
  A session's `dialog_list` or `sw_status` shows that popup for ~10 s after a start.
- [x] `ReferenceLiveTests` 46/0/0
- [x] `PropertiesLiveTests` 25/0/0
- [x] `AssemblyLiveTests` 33/1/0 and 7 stopped by the runner's guard (free memory under
  800 MB), 2026-11-13 23.49: failed `A_part_the_server_owned_comes_back_owned_…` (it
  failed once in S49's mixed run too and then passed three times alone: rerun it alone);
  guard-stopped, each to run alone on a fresh SOLIDWORKS with Chrome and VS Code closed:
  `A_tilted_component_s_box_is_its_body_s_extremes`,
  `Insert_refuses_unsaved_missing_and_foreign_files_and_needs_an_assembly`,
  `Subassemblies_are_inserted_and_listed_with_their_children`,
  `Ref_plane_and_ref_axis_in_an_assembly_take_component_and_assembly_references`,
  `Interference_with_coincident_faces_says_what_SOLIDWORKS_listed_overlapping_touching_and_kept`,
  `Mate_list_names_the_plane_face_edge_or_vertex_each_mate_takes`,
  `Width_centres_a_cube_between_the_box_ends_and_hinge_seats_a_shaft_in_the_bore`.
  Then Claude Code ended the runner for low system memory, at the start of
  `DrawingLiveTests`; the classes below were not run. On this 7.5 GB machine the
  assembly tests need more than the ~2 GB a fresh SOLIDWORKS leaves free with the other
  programs open; Claude Code's own reaping of background commands is turned off by
  starting it with `CLAUDE_CODE_DISABLE_BG_SHELL_PRESSURE_REAP=1`.
  The seventh session (2026-11-14) ran the eight alone, each on a fresh SOLIDWORKS: six
  passed; `A_part_the_server_owned_…` failed again alone, every assert holding, on the
  fixture's check "System preferences changed by this test: Integer230: 0 -> 1"
  (`RebuildOnActivation`, which `AssemblyService.EnsureLoaded` sets through
  `PreferenceScope`): a failure of the run's, for its own subagent;
  `Interference_with_coincident_faces_…` stopped by the guard alone too (free memory
  749 MB, SOLIDWORKS at 2.4 GB), then passed alone with the guard at 400 MB free (free
  memory never under ~1.9 GB). Ticked: 40 of 41 pass, the one failure goes to the step
  for the run's failures below.
- [x] `DrawingLiveTests` 57/0/0 (five chunks, SOLIDWORKS at most 2.4 GB)
- [x] `BodySheetMetalLiveTests` 52/0/0 (five chunks, SOLIDWORKS at most 2.8 GB)
- [x] `ExportLiveTests` 21/0/0 (the file's other three classes are offline ones, run in
  the offline step below)
- [x] `FaceEditLiveTests` 5/1/0, `SurfaceShapeLiveTests` 8/0/0, `WeldmentLiveTests`
  10/0/0 (four tests requeued alone after a guard stop at 769 MB free, all passed),
  `SimulationLiveTests` 16/1/0. Both failures fail alone too and are not the run's:
  FaceEdit's `Delete_heals_a_hole_in_an_imported_STEP_body_without_history` reads volume
  0 after the STEP import (S5, failing the same before and after S61);
  Simulation's free-free frequency study, the solver returning 24, the same on build
  `091908`'s code (new item S71).
- [x] the live project's offline classes (`…LogicTests` beside the live ones), the
  project run once without `SWMCP_LIVE_TESTS`: 119 passed, 0 failed, 526 skipped (the
  live tests, each with its skip message); unit tests 284/284; `check_repo` ok
- [x] a failure that is the run's: fixed by its own subagent, its tests rerun.
  `A_part_the_server_owned_…` (S49's test): SOLIDWORKS writes `ActivateDoc3`'s rebuild
  option into preference 230 when the document it activates has no window; the server
  now holds it for such a document (S49's "Release run 2026-11-14" paragraph), 19/19
  live tests that reach the changed calls
- [x] the push and the release: pushed (`origin/sw2023` at `cfd987e`), published
  `0.1.0-sw2023-20261114-024455` (`--check-catalog` ok, 165 tools), registered in both
  projects that list the server, tag `build-20261114-024455` pushed and read back, release
  page made, each Waiting line given its commit and the build
From `AssemblyLiveTests` on, the main session runs the remaining classes itself in one
runner call (a waiting subagent was ended for low memory twice), a subagent only for a
failure; each class's `CLASS RESULT` line in the runner's summary is its record.
A class's subagent (Sonnet): `dotnet build` first, then
`%LOCALAPPDATA%\solidworks-mcp\livetest\runall.ps1 -Classes <Class> -ChunkSize 12
-RestartEachChunk -FreshStart` (chunks of 12 tests, SOLIDWORKS restarted before each; a
guard stops a run above 4.5 GB or under 800 MB free, the Welcome screen and a "low on
memory" dialog answered; `CLASS RESULT` lines in `logs\summary.txt`); a failure rerun
alone and diagnosed, no code changed. On this 7.5 GB machine a fresh SOLIDWORKS leaves
~2.1 GB free.
SOLIDWORKS 2023: during S62 it held 8.3 GB on this 7.5 GB machine, showed a "critically
low on memory" dialog (answered No by the subagent) and `equation_set` refused every
equation; it was exited and started again, and the batch rerun held. Start batch 7's
full live runs on a freshly started SOLIDWORKS, and restart it between areas if its
memory climbs. Its Welcome screen, open after a start, kept COM from reaching it until
it was closed (S49's run). Never end a `solidworks-mcp` process (the owner's other
sessions).
`findings.md` holds an entry of 2026-11-13 from another session ("two printed plates
…"), committed on the owner's word (`e40982f`) and not sorted yet: the next session
sorts it after the release.
The opening question was asked and answered in the first session
and holds for the rest of the run, so the next session asks nothing: the release is
tagged and registered in both projects that list the server in `~/.claude.json`, and
`sw2023` is pushed at the release. Decided 2026-11-13 by the run, not the owner: a
Waiting line names its commit as "the commit that moves it here", since a commit cannot
hold its own hash; the release batch writes each item's hash and the build into its
line, from this list and `git log`. Per item, the main session reads the diff of any
check made stricter against the common flow before committing it (S66's edge check was
first shown live not to refuse ids of edges no feature touched).

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
  - **S61 (a) a third time**: did not hold in the check of build `20261121-175345`
    (findings 2026-11-21, point 1): a STEP inserted after another component, the assembly
    saved in another folder than the STEP: the part written beside the STEP, yet the
    reopened assembly looks for it in its own folder, the component lost, reported `ok`
    with "Also wrote …". Fixed again, and group 5 built and checked again. Test: the
    scenario's case, a part inserted first, the STEP from a subfolder of the assembly's
    folder (and from an unrelated folder), `doc_save as`, the assembly closed and reopened,
    the component resolved (fails now: suppressed, `missingReferences` in the assembly's
    folder).
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
- [x] S61 (a) again: committed (`e3b1b7e`)
  Cost: 153k tokens / 14 min (the default model, one run).
- [x] Group 5 again: full live tests and release (build: `20261121-175345`, `e3b1b7e`)
  Live: AssemblyLiveTests 57, AssemblyStructureLiveTests 2, TopDownLiveTests 4, M0LiveTests 23, ExportLiveTests 22,
  ConfigurationLiveTests 7, HardwareLiveTests 3, SimulationLiveTests 17 (the classes that save or close an assembly through
  `DocumentService`; TemplateLiveTests skips, the 2025 templates are not there); all passed but one. Flake, not this fix's:
  `AssemblyLiveTests.Dimension_set_on_a_mate_angle_an_equation_drives_…` refused in the full class run (56/57) and passed
  alone after a SOLIDWORKS restart. `PropertiesLiveTests` not run (no assembly save or close; its known failure stays).
  Run class by class with SOLIDWORKS restarted between, free memory 2.5 to 2.9 GB before each start after the build
  servers were shut down; no "low on memory" dialog. The live tests took 47 min of test time (28.2, 0.6, 1.4, 2.4, 3, 1.7,
  3.9, 5.2 min, and the 0.5 min rerun of the flake; about 64 min on the clock with the restarts), the whole release about
  75 min. Unit 302. No description changed, `docs/tools.md` not regenerated. Registered in `claude_solidworks` and
  `clove sorter` (its `SWMCP_VBA=on` kept); tag `build-20261121-175345` pushed and read back, release page with no files.
  Cost: Sonnet, see the report.
- [x] Group 5 again: scenario test (session id: `9ec9afb8-62d2-437f-a8ca-22e1206ae72a`), its entry committed and sorted
  Started 2026-11-21 17:56, detached (launcher pid 12152, `check5b.ps1` in the session's
  scratchpad, its output `check5b.out.txt`, an `EXIT` line at its end), after `dotnet
  build-server shutdown`, 2.7 GB free with SOLIDWORKS started from its exe.
  Ended 18:02, one run (~6 min), no resume: S61 (a) not held (assembly in another folder
  than the STEP), back to Open; the short checks held; S5 met again (findings 2026-11-21,
  `3d1a7b3`). Cost (for `learned.md`), tokens / minutes: group 5 again's release 91k/67
  (Sonnet), its sort 104k/2 (a subagent, the default model). The session of 2026-11-21
  handed over here, its context at about 120k with a whole fix, release and check ahead.
- [x] S61 (a) a third time: not started here; closed 2026-11-21 on the owner's word, the
  point taken as the first item of `## Run 2026-11-21` (its group 1), its decision and test
  as written above
- [x] Group 5 a third time: full live tests and release: not run, moved with S61 (a) to
  the run of 2026-11-21
- [x] Group 5 a third time: scenario test: not run, the same
- [x] The report (2026-11-21, at its closing): 19 of the 20 points planned held in their
  scenario tests and are closed (S49, S51, S52 (c) and S52's STEP case, S59 (c), S62 (b),
  S64 (h) as not reproduced, S65 (b) to (e), S67 to S73), with S75 to S78, S87 and S88
  taken in mid-run and closed; S64 (c) probed, not reproduced, still Open; S61 (a) fixed
  twice (builds `20261117-012659`, `20261121-175345`) and not held either time, carried
  into the run of 2026-11-21. Seven builds, seven scenario tests (group 5 three times
  planned, twice run). New items its checks found and left for the next plan: S79 (b), S89,
  S90. Closed by the owner (2026-11-21: "kenapa ga gabung dengan item baru lain. jadi
  jadiin run baru") so S61 (a) goes with the items found since, rather than in a build of
  its own.

## Run 2026-11-21

The run takes 39 points of 27 open items in seven groups, 33 steps each done by one
subagent, each group its own build and its own scenario test: S61 (a), carried from the run
of 2026-11-16 (closed 2026-11-21 on the owner's word, its section in `closed_items.md`),
with the items found since. Phase 2 (`backlog:run`) goes unattended after the opening
answer below, across as many sessions as it needs, from the first step not ticked under
*Where the run stands*.

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
  stash named for it (`git stash push -m run-1121-<S> -- <files>`), and the run goes on.
  SOLIDWORKS hung or gone: end it and start it again (granted). A permission prompt or a
  dialog nobody answers is the one thing it cannot pass: it writes where it stood below,
  and stops.
- **One SOLIDWORKS, so serial** (`serial: true`): one item at a time, in the main tree on
  `sw2023`, and never a live test beside the scenario session. **One fresh subagent per
  step**, the main session thin: it hands the step over, reads the ten-line report and the
  diff, and commits. Models as each step names: the default model for a fix or a probe that
  needs diagnosis and for the check, Sonnet for a step whose fix is written out here and for
  a group's tests and release; the run moves a step up or down by its reading, with a line
  in the report.
- **Per step**, as `CONTRIBUTING.md` (*Tests*) and the owner's rules give it: first the
  live test that shows the fault, run on the current code (a test that does not fail
  closes the point instead, the test kept when worth keeping); then the fix; then that test
  and the live tests that call the changed code (`--filter-method`, about 18 s each, never
  the whole area); unit tests offline; `python tools/check_repo.py`; a description changed
  means `--check-catalog` and `ContractTests.Tool_catalog_follows_the_conventions` (the
  tool list is at its byte budget: any new option or line is paid for by trimming, as S51,
  S59 and S65 (a) did; a field in a result costs none of it). A probe step is a live probe
  first, its result written into the item; the fix follows when it shows the cause, else
  the point stays Open with the probe recorded. Where the fault cannot be made live (a
  SOLIDWORKS state), the rule goes into an interop-free function with a unit test, as S72's
  did.
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
  yet decided."; for a lettered item only the point done, the item staying Open for the
  rest; the `**Open: …**` line from `board.py --open-line` and `board.py --check` clean.
  `findings.md` is not the subagent's. Report, ten lines: the outcome, the failing test
  before, the tests after with counts, every file changed, its decisions, a proposed commit
  subject and body. Nothing that names where a finding came from (`findings.md`'s top).
- **Commits**: one per step, its fix and its note, as `git-workflow:pr` gives a commit on
  `sw2023`: as the owner, no trailer. Pushed at the group's release; no PR upstream (each
  upstream line stays "not yet decided").
- **Per group, the release**: the area's live tests in full once, on what is to be built
  (the owner's global `CLAUDE.md`), class by class with SOLIDWORKS restarted between and
  `dotnet build-server shutdown` before (the run of 2026-11-16's group 5 again: 2.5 to 2.9
  GB free before each start, no "low on memory" dialog), the runner
  `%LOCALAPPDATA%\solidworks-mcp\livetest\runall.ps1` where it serves; a flake rerun alone
  after a restart and named. `docs/tools.md` regenerated when a description changed; then
  `CLAUDE.md`'s *Build, release and run*, steps 1 to 5: push `sw2023`, publish from a clean
  tree at the pushed commit, `--check-catalog`, register `solidworks-mcp` (scope local) on
  the new exe in every project `~/.claude.json` lists with it (at planning
  `claude_solidworks`, `Upwork/Project/clove sorter` with its `SWMCP_VBA=on` kept, and
  this repo, all on `0.1.0-20261121-175345`), tag `build-<stamp>` pushed alone and read
  back, the release page with no files and "Scenario test: not run yet". The points of the
  group move to Waiting naming the build, status `BUILT <date>, proof: the scenario test of
  build <stamp>`.
- **Per group, the check**, written whole so the run works nothing out mid-way. Before it:
  `dotnet build-server shutdown`, free memory read (about 2.7 GB free with SOLIDWORKS
  started is enough; under 2 GB, SOLIDWORKS restarted first), SOLIDWORKS started from
  `C:\Program Files\SOLIDWORKS Corp\SOLIDWORKS\SLDWORKS.exe`, never by the server's
  `launch`. A session id made first (`[guid]::NewGuid()`) and written on the group's check
  line, committed. A launcher `check<n>.ps1` in the running session's scratchpad, run from
  the owner's test project (`~\Data\Project\claude_solidworks`) so it loads the build just
  registered there: `claude -p "Uji skenario build …-<stamp> (<S-items>), menurut
  reports/uji-skenario.md. Tanpa nanya apa-apa." --session-id <uuid> --permission-mode
  auto --allowedTools mcp__solidworks-mcp --add-dir "<this
  repo>"`, its output to `check<n>.out.txt` and an `EXIT
  <code>` line at its end; started detached by `Start-Process`, never as a background task
  of the running session. Watched by its process and by the transcript's last write
  (`~/.claude/projects/C--Users-Ahmad-Aziz-Data-Project-claude-solidworks/<uuid>.jsonl`), by
  no background task; it took 6 to 13 minutes in the run of 2026-11-16. Stopped with no
  `findings.md` entry, or the transcript unwritten for 30 minutes with the process still
  there (stopped first): resumed at once, unasked, by the same launcher with `claude -p
  --resume <uuid>` and the same flags, up to three times, then Blocked with what was seen.
  A SOLIDWORKS the stopped session started by `sw_connect launch` is closed and started from
  its exe (S89). It starts with `sw_status` reading the new version (step 6's short check).
  Its `findings.md` entry is committed here unasked and sorted (`backlog:sort`), the release
  page's notes updated (step 7). What it finds goes four ways: **held**, the point to
  `closed_items.md`, `CLOSED <date>` (S1's proof in group 5: a `Held` line, and S1 closed,
  its proof being a session that runs those tools); **a fix of this group not holding**,
  back under `## Open`, fixed and its group built and checked again before the next group;
  **a new point, small and in the group's files**, a later step of this run, a new
  S-number (`board.py --next-code`), taken into the next group; **a new point needing the
  owner** (a choice, a large change), a new Open item, not in this run. The next group
  starts only after the check is sorted.
- **A check is watched by a subagent, not by the owner's "cek"** (2026-11-21, the owner,
  mid-run, after group 1's check stopped at a usage limit and waited an hour on them: "the
  point of the run is to go on without me"). Once the check is started, the session
  starts one background subagent on Haiku as its watcher: it reads the launcher's output
  file for its `EXIT` line, the process by its id and the transcript's last write, through
  `Monitor` (an until-loop of about 60 s), and returns one line when the check ended (`EXIT
  <code>`, a `findings.md` entry of the check's or none, a usage-limit line in the output)
  or when the transcript has been unwritten for 30 minutes. Its return wakes the session,
  which resumes, commits and sorts as above, and starts a new watcher after a resume. A
  usage limit in the output: the watcher waits past the reset time it names before it
  returns. The watcher ended by Claude Code (low memory, the limit): the owner's "cek"
  still serves. The launchers and the step rules of this run are kept outside the
  session's scratchpad, in `%LOCALAPPDATA%\solidworks-mcp\livetest\run-1121\`
  (`check<n>.ps1`, `brief-rules.md`, the brief every item's subagent reads).
- **The watcher, as it ran in session 2** (2026-11-22, the run): a Haiku watcher handed back at
  once twice, its `Monitor` armed and no event, so groups 2 and 3's checks were watched by a
  background `Bash` until-loop of the session itself (the launcher's `EXIT` line, `tasklist` on
  the pid, the transcript's mtime past 30 minutes, `sleep 60`), one notification at the end. The
  launcher is started with its path quoted (`-File "\"<path>\""`): unquoted, the space in the
  user folder killed it at once; the pid and the transcript read ~25 s after the start.
- **Session 3** (2026-11-22, the run): each group's release and check are handed to subagents
  too, the check at the owner's word ("uji skenario di sesi ini aja, kasi ke sub agent"): a
  Sonnet subagent frees memory, restarts SOLIDWORKS, starts `check<n>.ps1` detached, watches it
  to its `EXIT` and resumes it, one report at the end; the main session commits the entry and
  hands its sort to a default-model subagent with the decisions per point. Group 4's release
  ran `runall.ps1` at `-MinFreeMB 400` to keep the requeues down (S84 (b) runs in every feature
  call, so 24 classes, about 195 min). The owner moves to a new session after each group's
  sort, not mid-group (2026-11-22).
- **Session 4** (2026-11-22, the owner, mid-group 5, leaving the laptop): "nanti lanjut aja ya ke grup 6.
  gausah pindah sesi": after group 5's check is sorted the same session goes on into group 6 (and on), past the
  150k hand-over, its context summarized by Claude Code as needed; every step still committed and ticked, so a
  cut session is picked up from the first step not ticked. Changed by the owner on their return, still in group 5:
  "stop ya setelah uji scenario grup 5": the session stops once group 5's check is sorted, and hands over;
  group 6 starts in a new session.
  Session 4 ran group 5 whole: six item steps (subagents 106k to 256k tokens, 4 to 28 min each), the release
  (Sonnet, 136k, 192 min), the check (Sonnet, 69k, 13 min) and its sort (default model, 178k, 5 min); it handed over
  at about 160k.
- **Session 6** (2026-11-24, the run): group 6 from S127 again to its sort: S127 again (default model, 133k tokens,
  16 min), the release (Sonnet, 109k, 41 min, `DrawingLiveTests` alone rerun as decided), the check (Sonnet, 61k,
  12 min, no resume) and its sort (default model, 181k, 5 min); handed over after the sort, as the owner's rule of
  2026-11-22 gives, group 7 in a new session.
- **Left for the end of the run** (to file as items with the report): S81's probe saw once, in
  one variant (`doc_open` then `doc_activate` of parts loaded only by the assembly), the live
  fixture's "System preferences changed by this test: Double2: 0.000131 -> 1.085e-05", cause
  not known. S64 (i), now S121 (closed): `dimension_set` on one leg of an equal chamfer does not
  warn of the other, and `feature_chamfer`'s edit path gives no legs line (outside its step).

### Items

Order: S61 (a) first, carried; then the groups that hold a result reported `ok` while wrong
or the most met item (S92, eleven times; S79 (b); S89, four; S81, four; S84, six; S106; S100
(d)); then by files. Each step's test to start from is to fail on the code at planning
(`bdfcf80`, no server change since build `20261121-175345`); one that does not fail closes
its point.

- **Group 1, assembly save and STEP** (`DocumentService.cs` saves, `AssemblyService.cs`
  insert and mates, `FeatureManagementService.cs` `dimension_set`'s refusals):
  - **S61 (a) a third time** (default model): the check's proposal. The owned part a STEP
    insert left unwritten is written to its path beside the STEP **before** the assembly's
    own save, so the assembly stores the path the part has; where SOLIDWORKS still stores
    the assembly's folder, the component is repointed to the written part after it. The
    save then reads the reference back from the saved file (its dependencies,
    `ISldWorks.GetDocumentDependencies2`, or a reopen in the test) before it says "Also
    wrote"; a reference that does not name the part on disk is a warning naming both paths.
    Test: the scenario's cases (g) and (e): a part inserted first, the STEP from a subfolder
    of the assembly's folder and from an unrelated folder, `doc_save as` (and a saved
    assembly saved in place), closed and reopened, the component resolved (fails now:
    suppressed, `missingReferences` in the assembly's folder). The cases that held ((a),
    (c), (d), (f)) rerun. Not: an assembly saved in place reading dirty again.
  - **S83** (default model): with `flip` asked, the side warning says the part now lies on
    the other side from where it was inserted, as asked; without `flip` as now. Test: a
    distance mate of 80 between two parallel planes, `aligned`, `flip:true`, landing on the
    insert's side: the warning's text (fails now: "If it should stay on the side it was on,
    delete the mate").
  - **S107** (Sonnet): the re-activation warning of a call with `doc` adds "the target is
    still '<doc>'; doc_activate to change it". Test: `doc_new` of a part, `asm_insert
    {doc:<assembly>}`: the warning names the target (fails now).
  - **S110** (Sonnet): `dimension_set` of a limit mate's nominal outside its limits is
    refused naming the limits read and `dimension_tolerance_set`. Test: a limit mate with
    limits [20, 72.5], its nominal set to 15 (fails now: the limits not named).
- **Group 2, equations and undo** (`FeatureManagementService.cs` equations and `doc_undo`,
  `EquationBindings.cs`, `SwExt.FeatureManagement.cs`, `dimension_list`):
  - **S92 (c)**, a probe first (default model): the third session's lead. A part with a
    global `"g" = 60.4mm` and a row derived from it, `equation_list` read; an assembly
    holding it opened read-only by the server and closed, read again; opened editable, a
    mate dimension set, `asm_interference`, closed with `discard`, read again. Reproduced:
    the step that brings the state on written into the item and
    `docs/solidworks-api-notes.md`, the test kept (skipped with its reason if it leaves
    SOLIDWORKS in the state; SOLIDWORKS restarted after) as (a) and (b)'s live case, and a
    server-side way round where the probe shows one. Not reproduced: the point stays Open
    with the probe recorded; (a) and (b) go on with unit tests.
  - **S92 (a) and (b)** (default model): (b) `equation_list` and `equation_set` compare
    each global defined by a plain number with the value SOLIDWORKS reads back; on a
    difference (60.4 read as 60) a warning names S58 (b)'s state and the restart, and
    `equation_list` says its values are not to be trusted; (a) while that sign shows,
    `equation_set`'s `VERIFY_FAILED` ("kept the previous equation", "evaluates to 0") names
    the state and the restart in place of the expression. Test: the comparison in an
    interop-free function with unit tests (fails now: it does not exist), (c)'s live case
    where it reproduced, the live tests of `equation_list` and `equation_set` passing.
  - **S79 (b)** (default model): `doc_undo` right after a server change that left no
    SOLIDWORKS undo step (`equation_set`, and the others the step finds) refuses, naming
    that change and the step SOLIDWORKS would undo instead; when it does undo, it names the
    features removed, as its description says. Test: the scenario's case, a 10 × 10 × 5
    boss, `equation_set` linking a variable to its depth, `doc_undo`: refused, the boss kept
    (fails now: `volumeDeltaMm3: -500`, reported `ok`).
  - **S95** (Sonnet): `dimension_list` gives `equation` (its text) for a dimension an
    equation drives and `driven: true` only for a reference dimension, as its description
    says. Test: a part with a dimension an equation drives and a reference dimension: the
    first `driven:false` with its equation, the second `driven:true` (fails now: both
    `driven:true`, no equation).
- **Group 3, session and connection** (`SessionService.cs`, `SessionNotes.cs`,
  `DocumentRegistry.cs`):
  - **S89** (default model): `sw_connect {launch}` starts `SLDWORKS.exe` as a process, its
    window shown, waits for it in the running object table and attaches, so any later
    process reaches it; `NOT_CONNECTED` with a SOLIDWORKS whose main window has no title and
    no other server process alive names "started through COM by an earlier server process:
    only it could reach it; close it and start SOLIDWORKS from its exe", never "still
    starting". Test: SOLIDWORKS closed, `sw_connect {launch:true}`, then an outside COM
    client (a VBScript `GetObject(, "SldWorks.Application")`) reaches it (fails now:
    "Operation unavailable"); the hint in `SessionNoteTests`.
  - **S92 (d)** (Sonnet): `sw_status` gives SOLIDWORKS' process start time beside its pid;
    S58 (b)'s restart message names it, and its hint says that closing the window can leave
    the process alive under the references of the server processes `otherServerPids`
    lists. Test: `sw_status`'s start time equal to the process's own (fails now: absent;
    if `sw_status` gives it already, the message's part alone, by a unit test).
  - **S81** (default model): a probe of the stamp the warning compares in case (1): parts
    edited and saved by the server, then their assembly adopted by `doc_activate
    {allowEdit:true}`, `equation_set` and `doc_save` on it. The stamp taken at adoption,
    and a change the server's own saves of its parts caused not read as an outside one.
    Test: that case (fails now: "modified outside this server since its last change (by the
    user?)"). Case (2) and the drawing of 2026-11-18 only if the probe shows the same cause.
  - **S109**, a probe (default model): moves and rebuilds of a component in a limit-mate
    configuration of an assembly whose subassembly is open beside it, `sw_status` and
    `dialog_list` when the connection goes. A dialog found: the `NOT_CONNECTED` hint names a
    modal dialog where one is up (read through Win32, not COM), a unit test of the hint. Not
    reproduced: Open with the probe recorded.
  - **S114** (default model), added by group 2's check: a component's `dimension_set` from an
    assembly makes `doc_undo` refuse naming it, as S113's changes do; an undo that runs names
    the feature restored or removed; the refusal after an undo says the step "may be"
    another's. Test: (b)'s case (fails now: `VERIFY_FAILED`), then (c) and (a).
- **Group 4, feature results that read as faults** (`FeatureVerifier.cs`,
  `PartFeatureService.Mirror.cs`, `PartFeatureService.cs`, `FeatureService.Fillets.cs`,
  `SketchService.cs` regions, `Snapshot.cs`):
  - **S84 (a) and (c)** (Sonnet): (a) when every instance face is found and the volume
    matches, the area line reads "faces merge differently from the seed's; the volume and
    the instance faces match"; (c) a blind cut whose depth equals the material it crosses
    says "through: the end lies on the far face" in place of "no floor". Tests: a mirror
    whose copy's area differs from the seed's, the volume exact (fails now: "meets the body
    differently"); a cut 4 deep through a 4 mm wall (fails now: "no floor at 4 mm").
  - **S84 (b)** (default model): the rebuild warnings are read after the call's own rename
    and its rebuild, naming the feature by its new name; one gone after that rebuild is not
    reported. Test: `feature_pattern_linear` with `name`, a case that gives warning 51 if
    the step finds one; else a warning named by the new name, the test the name.
  - **S64 (l)** (default model): the session's proposal: a model edge that is an inner loop
    of the face, wholly inside the contour, does not disqualify the region. Test: a Ø3.2
    circle cut through, a concentric Ø6.5 on the same face: `regions:1` (fails now: 0).
  - **S64 (i)** (Sonnet): an equal chamfer's result names both its dimensions as its legs,
    separate once one is driven; `equation_set` on one leg alone warns that the other stays,
    naming it. Test: an `equal` chamfer, an equation on `D1` alone (fails now: no warning).
  - **S64 (c)** (default model), decided by the owner: a fillet or chamfer that fails where
    an edge in the call ends tangent to an edge not in it gets the hint "edge <id> ends
    tangent to <id>, not in the call: turn `propagate` on or add it", not the radius. Test:
    the probe's case on S52's plate, a round alone with `propagate` off (fails now: "A
    radius too large …").
  - **S93** (default model): the writes that rebuild and do not compare yet
    (`sketch_finish`, `dimension_set`, `feature_delete`, the sketch tools that close a
    sketch: the step lists them) compare the tree's rebuild errors and warnings before and
    after, as `equation_set` does since S58 (a), and name each feature that gained one.
    Test: a block, a fillet on one of its edges, the sketch redrawn so the edge goes:
    `sketch_finish` names the fillet's warning (fails now: nothing).
  - **S117** (default model), added by group 3's check: after an undo past a later step, the
    next `doc_undo` refuses naming the rename before it, as S113. Test: S117's sequence (fails now: ran).
  - **S118** (default model), added by group 3's check: S112's sign also a derived row read 0
    over non-zero globals; an all-refused batch names the state. Test: unit tests (fails now).
- **Group 5, drawings and reads** (`DrawingService.Tables.cs`,
  `DrawingService.Placement.cs`, `DrawingLayout.cs`, `DrawingService.Annotations.cs`,
  `BodyService.cs`, `PropertiesService.cs` materials, `OpenService.cs`, `DocumentService.cs`
  `doc_new`):
  - **S106** (default model): `drw_table_add` returns the table's `extentMm` and warns of
    what it lands on (a view's bounds, an annotation, the title block); the note and
    dimension checks count tables and the views' `boundsMm`. Tests: a table placed over a
    view (fails now: no extent, no warning); a note across that table (fails now: "clear").
  - **S90** (Sonnet): the free-space placement of a section with no `atMm` counts the label
    the call will make. Test: the scenario's 1:2 plan, a section with no `atMm` (fails now:
    the label overlaps the title block).
  - **S52** (Sonnet), `body_list`'s box: from `TightBox`, as `model.box`. Test: a disc with
    its top edge rounded, `body_list`'s box equal to `model.box` (fails now: loose).
  - **S97 and S104** (Sonnet): database paths taken to their long form before they are
    listed and searched, each once; on a miss `databases` lists every database searched
    with its count. Tests: a filter matching nothing (fails now: `databases: []`); no
    database twice (S104 needs an 8.3 path: the test feeds one, made by
    `GetShortPathName`).
  - **S98** (default model): `doc_open {readOnly:true}` reports `access: "readOnly"`, with
    no line that the server may edit it, and the edit tools refuse on it with NOT_OWNED's
    hint. Test: a saved part opened read-only, its access and a `feature_extrude` refused
    (fails now: `owned`, the extrude made).
  - **S108** (default model), decided by the owner: `doc_new {type:"drawing"}` with no
    `template`, where SOLIDWORKS' default drawing template is not one it ships (not under
    its install's templates folder), keeps that template's sheet format and units and says
    so. Test: the default set to a custom template the test makes in a temp folder, the
    preference restored after (the owner's own default is `gamtek a3.drwdot`): the format
    and units kept (fails now: ISO, MMGS).
  - **The check also proves S1** (the owner, extras): the scenario test runs
    `drw_view_detail` and a view at an angle.
- **Group 6, weldment and library feature parts** (`WeldmentService.cs`,
  `WeldmentSpec.cs`, `OpenService.cs`, `DocumentService.cs` saves, `feature_list`):
  - **S100 (a) and (b)** (default model): `doc_open` takes a `.sldlfp` (a part);
    `doc_save {mode:"as"}` takes a `.sldlfp` path for a part, read back. Test: a shipped
    profile copied to a temp folder opened; a part saved as `.sldlfp` and reopened (fails
    now: both refused). Not: a folder added to File Locations.
  - **S100 (c)** (Sonnet): `feature_list` on a library feature part marks the `LibFeat1`
    row as SOLIDWORKS' internal wrapper and names its sketch as the library feature's
    profile. Test: that copy's list (fails now: no mark).
  - **S100 (d)** (default model): a probe of which step of the finding's sequence drops the
    profile sketch (configuration table deleted, `config_add`, `config_delete`,
    `sketch_edit`, segments deleted and added, `sketch_finish`, `doc_save`), on a copy; the
    fix where found; and `doc_save` of a library feature part reads back that its library
    feature still holds a sketch, else `VERIFY_FAILED` saying the file was written without
    it. Test: the sequence on a copy, reopened (fails now: no sketch, reported `ok`).
  - **S102** (Sonnet): "No profile" marks a one-configuration file in the standard folder
    "not usable: one configuration". Test: S91's case (fails now: listed as a type).
  - **S103 (a) and (b)** (default model): (a) a group SOLIDWORKS refuses as a path (no
    faces) is tried again as one group per parallel set, the result saying so; (b) each
    member gives its section's extents along the model axes. Tests: four lines, two vertical
    and two horizontal, none touching: two groups, volumes exact (fails now:
    `VERIFY_FAILED`); a member's extents (fails now: absent).
  - **S127** (default model), added by group 5's check: a detail view's `centerMm` from the
    same source as its bounds (or their centre), equal in `drw_view_detail` and `drw_info`.
    Test: a 2:1 detail placed by default, `centerMm` inside and at the centre of its
    `boundsMm`, equal across the two calls (fails now: outside its bounds, another value in
    `drw_info`).
  - **S128** (default model), added by group 5's check: `Place` counts the sheet's tables;
    where no free spot exists along an aligned view's alignment, the result says so with the
    hint (a smaller scale, or `atMm`) instead of claiming a free spot. Test: a detail with no
    `atMm` beside a hole table, not over it; an auxiliary with no room along its alignment,
    the hint (fails now: over the table's corner; over the parent, no hint).
  - **S130** (Sonnet), added by group 5's check: a section whose centre lands more than 1 mm
    from `atMm` warns, naming both and why (the alignment). Test: point 9's case (fails now:
    no word).
  - **S131** (default model), added by group 5's check: the hole table's default face counts
    countersunk and counterbored holes as holes, and the result names the face chosen and its
    hole count. Test: a tab with one plain hole and a floor with six countersunk holes facing
    the same way (fails now: the tab, one row).
- **Group 7, sketch warnings and speed** (`SketchService.cs`, `SketchEditService.cs`,
  `SketchRelations.cs`), taken by the owner (extras):
  - **S80 (a) and (c)** (default model): `sketch_add` compares each point it was given with
    where the solved sketch has it and warns, naming the point, the distance and the
    relation, when one moved more than the sketch's tolerance; a rectangle drawn square that
    gets an `equal` is warned of. Test: a rectangle on a planar face, its bottom on the
    face's boundary edge, drawn 3 tall (fails now: 2.5 tall, no warning).
  - **S82 (a)** (default model): `sketch_constrain` angular, and `dimension_set` of a sketch
    dimension, warn when a `changed` length drops by more than half (a warning, as the owner
    decided S42). Test: the finding's trapezoid, angular 60 where the lines meet only
    virtually (fails now: 126 to 6, no warning).
  - **S86 (a)** (Sonnet): `sketch_delete` warns naming each segment it removed that was not
    named, with the dimensions and relations that went with it. Test: a polygon's six lines
    deleted (fails now: its circle gone unwarned).
  - **S101**, a probe (default model): where the time goes in `sketch_add` of 100 lines
    (the per-entity checks, S65 (e)'s merge pass, the contour read); the server's own passes
    made once per call where they are the cost. Test: 100 lines in one call, time and
    `TIMEOUT` (fails now: about 1.6 s an entity, past 50 s). SOLIDWORKS' own time: Open with
    the probe recorded and a timeout that grows with the count.
  - **S123** (Sonnet), added by group 4's check: `sketch_finish`'s `dimensions` names each
    dimension once, and `sketch_edit` and `sketch_finish` count segments alike. Test: S93's
    case of point 6, `D3@Sketch1` once and the two counts equal (fails now: twice, 7 and 5).
  - **S124** (default model), added by group 4's check: a constant fillet that consumes a face
    of its edge, or whose radius exceeds a blended face's width, warns naming that face (a
    warning, as the owner decided S42; no new option). Test: point 9's case, R20 on a 10 x 10
    section's edge, a warning naming the consumed face (fails now: none).
  - **S125** (Sonnet), added by group 4's check: `feature_pattern_linear`'s volume per instance
    not averaged over an instance that formed nothing, or that instance named. Test: point 2's
    second case of the entry (fails now: "200 mm³ (3 instance(s))", averaged over the empty one).
  - **S134** (default model), added by group 6's check, in group 6's files (`DocumentService.cs`
    saves): a save in place of a library feature part already known dissolved is refused before
    writing, the file on disk left as it was. Test: an owned copy of a shipped profile opened by
    `doc_open`, `doc_save {mode:"copy"}` to a `.sldprt`, then `doc_save` in place: refused, the
    `.sldlfp` on disk unchanged and reopening with its sketch (fails now: overwritten without its
    sketch, then `VERIFY_FAILED`).
  - **S135** (default model), added by group 6's check, in group 6's files
    (`DrawingService.Tables.cs`, `DrawingService.Placement.cs`): a table with no `atMm` is placed
    by the same free-space search as the views, clear of the title block, or warned with a hint
    (`atMm`) where no spot is free. Test: point 10's case, a hole table with no `atMm` below a 1:2
    plan whose space below reaches the title block (fails now: over the title block, only the
    overlap warned).
- **For the owner**: nothing to prepare. S1's proof is in group 5's check.
- **Out of this run**: S4, S5, S7, S12 (wait on upstream); S10, S11, S33, S34 (parked,
  taken up when a job needs them); S60 (a) (parked); S64 (b) (left out on the owner's word,
  2026-11-16); S64 (j), (k), (m) to (p), S65 (b)'s move and (f) to (l), S80 (b), S82 (b),
  S85, S86 (b), S94, S96, S99, S105 (new options or choices for the owner, not offered in
  this run's question: the next plan; S64 (k), reported `ok` while wrong, first).

### Opening answer

2026-11-21, the owner, in one `AskUserQuestion` call:

- Grants: **"Semua (Recommended)"**: "Sama dengan run lalu: push sw2023, tag
  build-<stamp>, halaman release tanpa file, registrasi exe baru di claude_solidworks,
  clove sorter (SWMCP_VBA=on tetap) dan solidworks-mcp, uji skenario headless (claude -p
  dari claude_solidworks) beserta stop/resume-nya, commit entry-nya tanpa nanya, restart
  SOLIDWORKS kalau hang. Tanpa PR upstream."
- Scope: **"6 grup, 27 poin (Recommended)"** (the question counted the 27 steps of groups
  1 to 6, 32 points one per letter; with the extras 33 steps, 39 points).
- S108: **"Pakai template default apa adanya (Recommended)"**.
- Extras, all four: S1's proof in group 5's check; group 7, warnings in sketches (S80 (a)
  and (c), S82 (a), S86 (a)); S64 (c)'s tangent-joint hint; the S101 probe.

### Where the run stands

- [x] Phase 1: the opening question asked and answered (2026-11-21)
- [x] S61 (a) a third time: committed (0d05bd7)
- [x] S83: committed (49a57fb)
- [x] S107: committed (70e23a3)
- [x] S110: committed (f3617b0)
- [x] Group 1: full live tests and release (build: `20261121-205309`, `1fb1fda`) (`4b8cb7c`)
  Live: AssemblyLiveTests 62, AssemblyStructureLiveTests 2, TopDownLiveTests 4, M0LiveTests 23, ExportLiveTests 22,
  ConfigurationLiveTests 6, HardwareLiveTests 3, SimulationLiveTests 17, FeatureManagementLiveTests 36 (the classes that
  save, close or insert in an assembly, call `dimension_set`, or whose tests pass `doc` through `SwContext.Bind` for S107,
  whose warning only changes its text, and carry the warning's assertions), and ModellingLiveTests 9, WeldmentLiveTests 11,
  FaceEditLiveTests 5 of 6 as extras; all passed but two things, neither a fault of this group's code. (1) Guard stops, not
  test failures: the machine was short of memory (7.5 GB total, other processes about 3 GB, free 1.5 to 2.5 GB when
  SOLIDWORKS started, against 2.5 to 2.9 GB in the last release), so `runall.ps1`'s 800 MB free-memory guard stopped
  FeatureManagementLiveTests once (10 tests done, the rest requeued one by one, 36/36 in the end) and AssemblyLiveTests five
  times (58/62; the four tests it recorded as guard-stopped passed 4/4 rerun after a restart with the guard at 250 MB).
  No "low on memory" dialog. (2) `FaceEditLiveTests.Delete_heals_a_hole_in_an_imported_STEP_body_without_history` reads
  volume 0 after the STEP import, in the full class run and alone after a restart: the known S5 failure
  (closed_items.md, run of 2026-11-16: failing the same before and after S61). `SimulationExtrasLiveTests` (3, an extra,
  not run in the last release) not run to the end: its guard stopped it at 242 MB free even with the guard lowered, so
  it was dropped. No flake. `PropertiesLiveTests` and the classes that never pass a second document not run (S107's change
  is a warning text only, and its only live assertions are `AssemblyLiveTests` and `FeatureManagementLiveTests`).
  Run class by class with SOLIDWORKS restarted before each and `dotnet build-server shutdown`. The live tests took about 87
  min on the clock (19:26 to 20:53, the guard requeues included), the whole release about 95 min. Unit 302, `check_repo`
  ok. No description changed (`--check-catalog` ok, 165 tools), `docs/tools.md` not regenerated. Registered in
  `claude_solidworks`, `clove sorter` (its `SWMCP_VBA=on` kept) and this repo, each read back from `~/.claude.json` on the
  new exe; tag `build-20261121-205309` pushed and read back, release page with no files.
  Cost: Sonnet, see the report.
- [x] Group 1: scenario test (session id: `287fca18-c22d-4f64-a5fc-e3c50a4efba2`), its entry committed and sorted (entry `351a58f`, sort `d5cf498`)
  Started 20:55 2026-11-21, free memory 2174 MB with SOLIDWORKS restarted from its exe; launcher `%LOCALAPPDATA%\solidworks-mcp\livetest\run-1121\check1.ps1` (`-Resume` to resume), first run from the first session's scratchpad, pid 13972, output check1.out.txt and check1.err.txt beside the launcher it ran from. Stopped 21:00 at the 5-hour usage limit (`EXIT 1`, no entry). Resume 1 at 21:56, free memory 2463 MB with SOLIDWORKS restarted from its exe, pid 5944, from the scratchpad copy (its output there: `%TEMP%\claude\8c3a0854f52f5604\96549089-56a6-40d7-a892-5e462fc37ecc\scratchpad\check1.out.txt`); a later resume runs the copy under `run-1121`, its output beside it. Resume 1 ended `EXIT 0` 21:58 with the entry. Sorted: S61 (a), S83, S107 and S110 held and closed; S111 new (not in this run); S5 met again. Release page notes updated with the result.
- [x] S92 (c): probed, not reproduced (recorded in S92; (a) and (b) go on with unit tests) (`c3e8f9e`)
- [x] S92 (a) and (b): committed (split out as S112 in Waiting, as the format splits an item) (`f2d3dc2`)
- [x] S79 (b): committed (split out as S113; `doc_undo`'s description changed: docs/tools.md at the release) (`9c14d93`)
- [x] S95: committed (`929158d`)
- [x] Group 2: full live tests and release (build: `20261122-002030`, `c2026da`) (`0191c27`)
  Live: PropertiesLiveTests 25 of 26, FeatureManagementLiveTests 38, DrawingLiveTests 57, AssemblyLiveTests 62, TopDownLiveTests 4,
  RecognitionLiveTests 1, and by method AdvancedFeatureLiveTests 1 (`Helix_revolutions_read_…`, a `dimension_list` caller) and
  ReferenceLiveTests 1 (`Review_reports_a_broken_shell_…`, likewise), the classes that call what group 2 changed: the equation and
  property tools, `dimension_list`, `doc_undo`, rename and rollback, `doc_rebuild`, and every write through `SwContext`'s undo
  accounting (`MaterialSet`, units and appearance have no live caller beyond those). All passed but one, not a fault of this group:
  `PropertiesLiveTests.A_custom_material_is_listed_…` (25/26) fails on the machine's custom material library (a second
  classification, from an earlier run; closed_items.md, the run of 2026-11-16, S72), before and after. (1) Guard stops, not test
  failures: the machine was short of memory again (free 0.7 to 0.8 GB with SOLIDWORKS running), so `runall.ps1`'s 800 MB guard
  stopped FeatureManagementLiveTests once (10 tests done, the rest requeued one by one; the one test that tripped the guard alone,
  `The_servers_own_failed_kept_and_rolled_back_changes_do_not_read_as_the_users`, a test of group 2's own code, passed rerun alone
  after a restart with the guard at 250 MB, 1/1) and AssemblyLiveTests eight times (2 tests done, the rest one by one: 55 passed, 7 tripped the guard
  alone and passed 7/7 rerun one by one after a restart each with the guard at 250 MB). No "low on memory" dialog. No flake.
  `M0LiveTests`, `ConfigurationLiveTests` and `SimulationLiveTests` not run (their only call is `material_set`, which group 2 did not
  change); the other classes call none of it. Run class by class with SOLIDWORKS restarted before each and `dotnet build-server
  shutdown`. The live tests took about 94 min on the clock (22:48 to 00:20, the guard requeues and the rerun included), the whole
  release about 95 min. Unit 310, `check_repo` ok. `doc_undo`'s description changed: `--check-catalog` ok (165 tools, 247.6 KB),
  `docs/tools.md` regenerated and committed alone (`c2026da`). Registered in `claude_solidworks`, `clove sorter` (its `SWMCP_VBA=on`
  kept) and this repo, each read back from `~/.claude.json` on the new exe; tag `build-20261122-002030` pushed and read back, release page with
  no files (`ahmadaziz6720/solidworks-mcp` releases, `build-20261122-002030`).
  Cost: Sonnet, see the report.
- [x] Group 2: scenario test (session id: `f634dedd-ccc6-4d15-b29a-e09cb7db3fd5`), its entry committed and sorted (entry `561c437`) (`17d73cb`)
  Started 00:23 2026-11-22, free memory 2165 MB with SOLIDWORKS restarted from its exe; launcher `%LOCALAPPDATA%\solidworks-mcp\livetest\run-1121\check2.ps1` (`-Resume` to resume), pid 17196 died at once (the launcher path, holding a space, not quoted in `-File`), started again 00:24 quoted, pid 16752, output check2.out.txt and check2.err.txt beside it. The Haiku watcher returned twice before the check ended, so the session watched it by a background until-loop of its own instead. Ended `EXIT 0` 00:29 with the entry. Sorted: S112, S113 and S95 held and closed; S114 new (S113's edges), taken into group 3; S115 new, not in this run; S5 met again. Release page notes updated.
- [x] S89: committed (`46920a1`)
- [x] S92 (d): committed (split out as S116) (`a72b20f`)
- [x] S81: committed (probed, not reproduced; stays Open, guard test kept. Seen in one variant: the fixture's "System preferences changed by this test: Double2", cause unknown, for the report) (`efde59f`)
- [x] S109: probed, not reproduced (stays Open, guard test kept; lead: the finding's SOLIDWORKS started by launch, S89) (`167edbd`)
- [x] S114: committed (doc_undo's description changed: docs/tools.md at the release) (`624b0f3`)
- [x] Group 3: full live tests and release (build: `20261122-030022`, `b45312d`) (`cf05f6c`)
  Live: M0LiveTests 25, PropertiesLiveTests 25 of 26, FeatureManagementLiveTests 39, DrawingLiveTests 57, TopDownLiveTests 4,
  RecognitionLiveTests 1, AssemblyLiveTests 65, and by method AdvancedFeatureLiveTests 1 (`Helix_revolutions_read_…`) and
  ReferenceLiveTests 1 (`Review_reports_a_broken_shell_…`), the classes that call what group 3 changed: `sw_connect` and `sw_status`
  (M0, whose new S89 test ends SOLIDWORKS and launches it; SOLIDWORKS was restarted from its exe by `swrestart.ps1` before every later
  class and at the end), the equation tools, `dimension_list`, `doc_undo` and every write through `SwContext` and `DocumentRegistry`.
  All passed but one, not a fault of this group: `PropertiesLiveTests.A_custom_material_is_listed_…` (25/26) fails on the machine's
  custom material library, before and after (closed_items.md, the run of 2026-11-16, S72). (1) One flake: in the full
  AssemblyLiveTests run `An_assembly_adopted_after_the_server_saved_its_parts_does_not_read_as_changed_by_the_user` failed with
  SOLIDWORKS refusing the equation `adoptW` = 12 ("SOLIDWORKS can come to refuse every equation ... until it is restarted", S112's
  message, after earlier tests in the same SOLIDWORKS); rerun alone after a restart it passed 1/1 (it passed in group 2's run too).
  (2) Guard stops, not test failures: free memory 0.5 to 0.8 GB with SOLIDWORKS running, so `runall.ps1`'s 800 MB guard stopped
  AssemblyLiveTests once (14 tests done, the rest requeued one by one) and two tests alone (`A_subassembly_is_solved_flexible_…`,
  `Moves_and_rebuilds_in_a_limit_mate_configuration_…`, the latter a test of S109's guard); both passed rerun alone after a restart each
  with the guard at 250 MB, 2/2. No guard stop in the other classes. No "low on memory" dialog. Run class by class with SOLIDWORKS
  restarted before each and `dotnet build-server shutdown`. The live tests took about 88 min on the clock (01:31 to 02:59, the guard
  requeues and the rerun included), the whole release about 92 min. Unit 323, `check_repo` ok. `doc_undo`'s description changed:
  `--check-catalog` ok (165 tools, 247.7 KB), `docs/tools.md` regenerated and committed alone (`b45312d`). Registered in
  `claude_solidworks`, `clove sorter` (its `SWMCP_VBA=on` kept) and this repo, each read back from `~/.claude.json` on the new exe;
  tag `build-20261122-030022` pushed and read back, release page with no files (`ahmadaziz6720/solidworks-mcp` releases,
  `build-20261122-030022`). S81 and S109 stay Open (probes not reproduced; their guard tests ran in the classes above).
  Cost: Sonnet, see the report.
- [x] Group 3: scenario test (session id: `ba47a581-7e48-4a8e-98dc-dda2b296ae23`), its entry committed and sorted (entry `f489505`; ended `EXIT 0` 03:09; S116 and S114 held and closed, S89 not proved there and its proof moved, S117 and S118 new into group 4, S92 met live, S5 again; SOLIDWORKS restarted after, as the state was on; release notes updated) (`89d32a7`)
  Started 03:03 2026-11-22, free memory 2602 MB with SOLIDWORKS restarted from its exe; launcher `%LOCALAPPDATA%\solidworks-mcp\livetest\run-1121\check3.ps1` (`-Resume` to resume), pid 17092, output check3.out.txt and check3.err.txt beside it; watched by a background until-loop of the session.
- [x] S84 (a) and (c): committed (split out as S119) (`6e4c9f5`)
- [x] S84 (b): committed (S84 to Waiting) (`d82a7b2`)
- [x] S64 (l): committed (split out as S120) (`c3c3b56`)
- [x] S64 (i): committed (split out as S121) (`69ae11e`)
- [x] S64 (c): committed (split out as S122) (`3fa3cf4`)
- [x] S93: committed (`b3abaf4`)
- [x] S117: committed (`ee5c4c5`)
- [x] S118: committed (`ed486b0`)
- [x] Group 4: full live tests and release (build: `20261122-142805`, `ed486b0`)
  Live: SketchEditLiveTests 79, PartFeatureLiveTests 52, FeatureManagementLiveTests 41, AdvancedFeatureLiveTests 30,
  BodySheetMetalLiveTests 52, ReferenceLiveTests 47, ModellingLiveTests 10, SurfaceShapeLiveTests 8, WeldmentLiveTests 11,
  FaceEditLiveTests 5 of 6, MoldingLiveTests 4, PropertiesLiveTests 25 of 26, AssemblyLiveTests 65, DrawingLiveTests 57,
  ExportLiveTests 22, M0LiveTests 25, TopDownLiveTests 4, RecognitionLiveTests 1, AssemblyStructureLiveTests 2,
  ConfigurationLiveTests 7, HardwareLiveTests 3, VisualsLiveTests 3, SimulationLiveTests 17, SimulationExtrasLiveTests 3:
  575 tests in 24 classes, as S84 (b)'s rebuild-warning report runs in every feature tool's call, so every class whose tests
  make features or sketches, plus the undo, equation, delete and suppress callers. Left out: `TemplateLiveTests` (1 test, makes
  no feature and calls none of the changed tools). All passed but two, neither a fault of this group's code: (1)
  `FaceEditLiveTests.Delete_heals_a_hole_in_an_imported_STEP_body_without_history` reads volume 0 after the STEP import, in the
  class run and alone after a restart: the known S5. (2) `PropertiesLiveTests.A_custom_material_is_listed_assigned_and_used_by_simulation_with_its_own_stiffness`
  (25/26) fails on the machine's custom material library, before and after (S72). One flake: in the full AssemblyLiveTests run
  `An_assembly_adopted_after_the_server_saved_its_parts_does_not_read_as_changed_by_the_user` failed once with SOLIDWORKS refusing
  an equation (S112's state, as in group 3's run); rerun alone after a restart it passed 1/1. Guard stops, not test failures:
  the machine was short of memory (free 0.3 to 0.8 GB with SOLIDWORKS running), and the guard ran at `-MinFreeMB 400` (not the
  default 800, to keep the requeues down): PartFeatureLiveTests once (28 tests done, the rest requeued one by one, 52/52 in the
  end), AssemblyLiveTests twice (7 tests done; then `A_flexible_subassembly_s_box_covers_its_children_as_they_are_placed` alone,
  SOLIDWORKS 4.7 GB private over the 4.5 GB limit), SimulationExtrasLiveTests at its start and one test alone, the other two of
  its tests failing on SOLIDWORKS' "continue to load SOLIDWORKS Simulation?" dialog at about 0.6 GB free; the stopped Assembly
  test and all three SimulationExtras tests passed rerun one by one after a restart each with the guard at 250 MB (`group4b.ps1`:
  2/2 and 3/3). No "low on memory" dialog. Run class by class with SOLIDWORKS restarted before each and `dotnet build-server
  shutdown` (`group4.ps1`, `group4b.ps1` in `run-1121`). The live tests took about 175 min on the clock (11:32 to 14:27, the guard
  requeues and the rerun included), the whole release about 195 min. Unit 343, `check_repo` ok. No description changed
  (`--check-catalog` ok, 165 tools, 247.7 KB), `docs/tools.md` not regenerated. Registered in `claude_solidworks`, `clove sorter`
  (its `SWMCP_VBA=on` kept) and this repo, each read back from `~/.claude.json` on the new exe; tag `build-20261122-142805` pushed and read
  back, release page with no files (`ahmadaziz6720/solidworks-mcp` releases, `build-20261122-142805`). SOLIDWORKS restarted from its exe at the end.
  Cost: Sonnet, see the report.
- [x] Group 4: scenario test (session id: `4f75820d-6e87-4a46-a4f6-093d30178ee6`), its entry committed and sorted (entry `693fd0a`; ended `EXIT 0` 14:41; S119, S84, S120, S121, S122, S93 and S117 held and closed, S118 not proved there (the good state, no false alarm) and its proof moved, S123 to S125 new into group 7, S5 again; release notes updated) (`6f49cc1`)
  Started 14:31 2026-11-22, free memory 2335 MB with SOLIDWORKS restarted from its exe; launcher `%LOCALAPPDATA%\solidworks-mcp\livetest\run-1121\check4.ps1`, launcher pid 2776, output check4.out.txt and check4.err.txt beside it; started and watched by a Sonnet subagent at the owner's word (2026-11-22, "uji skenario di sesi ini aja, kasi ke sub agent"); no resume; about 10 min.
- [x] S106: committed (`cff9e5c`)
- [x] S90: committed (`82087b8`)
- [x] S52: committed (split out as S126) (`ca11c05`)
- [x] S97 and S104: committed (material_list's description changed: docs/tools.md at the release) (`fd64df4`)
- [x] S98: committed (doc_open's description changed: docs/tools.md at the release) (`027e13c`)
- [x] S108: committed (doc_new's description changed: docs/tools.md at the release) (`6535044`)
- [x] Group 5: full live tests and release (build: `20261122-194424`, `a6a39d4`)
  Live: SketchEditLiveTests 79, PartFeatureLiveTests 53, FeatureManagementLiveTests 41, AdvancedFeatureLiveTests 30,
  BodySheetMetalLiveTests 52, ReferenceLiveTests 47, ModellingLiveTests 10, SurfaceShapeLiveTests 8, WeldmentLiveTests 11,
  FaceEditLiveTests 5 of 6, MoldingLiveTests 4, PropertiesLiveTests 26 of 27, AssemblyLiveTests 65, DrawingLiveTests 60,
  ExportLiveTests 22, M0LiveTests 25, TopDownLiveTests 4, RecognitionLiveTests 1, AssemblyStructureLiveTests 2,
  ConfigurationLiveTests 7, HardwareLiveTests 3, VisualsLiveTests 3, SimulationLiveTests 17, SimulationExtrasLiveTests 3,
  TemplateLiveTests 0 of 1 (skips itself: the 2025 templates are not installed): 578 passed of 580 run in 25 classes, as
  S98's guard runs in every write call and `doc_new` in every test. All passed but two, neither a fault of this group's code:
  `FaceEditLiveTests.Delete_heals_a_hole_in_an_imported_STEP_body_without_history` (S5, in the class run and alone after a
  restart) and `PropertiesLiveTests.A_custom_material_is_listed_…` (S72). One flake: `AssemblyLiveTests.An_assembly_adopted_after_the_server_saved_its_parts_does_not_read_as_changed_by_the_user`
  failed in the class run (S112's equation state), passed 1/1 alone after a restart. Guard stops, not test failures (free
  memory low, two other processes holding about 600 MB, the guard at `-MinFreeMB 400`): AssemblyLiveTests once (22 tests done,
  the rest one by one) and two tests alone, SimulationExtrasLiveTests at its start and each test alone; all passed rerun one by
  one after a restart each with the guard at 250 MB (`group5b.ps1`), but `A_nonlinear_study_of_a_small_load_matches_the_linear_one`,
  stopped again at 116 MB free and passed 1/1 alone at `-MinFreeMB 100` (`group5c.ps1`, below the plan's 250). No "low on
  memory" dialog. SOLIDWORKS' default drawing template read before, after the tests and at the end: the owner's own, unchanged
  (the S108 tests set and restore it). Run class by class with SOLIDWORKS restarted before each and `dotnet build-server
  shutdown`. The live tests took about 190 min on the clock (16:33 to 19:43), the whole release about 192 min. Unit 355,
  `check_repo` ok. Descriptions changed (doc_new, doc_open, doc_list, sw_status, material_list): `--check-catalog` ok (165
  tools, 247.6 KB), `docs/tools.md` regenerated and committed alone (`a6a39d4`); published from a worktree at it, as
  `open_items.md` held this note. Registered in `claude_solidworks`, `clove sorter` (its `SWMCP_VBA=on` kept) and this
  repo, each read back from `~/.claude.json` on the new exe; tag `build-20261122-194424` pushed and read back, release page
  with no files (`ahmadaziz6720/solidworks-mcp` releases, `build-20261122-194424`). SOLIDWORKS restarted from its exe at the end.
  Cost: Sonnet, 136k tokens, see the report.
- [x] Group 5: scenario test with S1's proof (session id: `0a60bd46-2242-491b-a386-102f51a0ab03`), its entry committed and sorted (entry `1b57828`; ended `EXIT 0` 19:59; S98, S108, S90, S106, S126, S97 and S104 held and closed (S104 as far as the machine allows), S1 held for the detail and a view at an angle and closed, S127, S128, S130 and S131 new into group 6, S129 new and not in this run, S5 again; release notes updated) (`bdd4c9d`)
  Started 19:48 2026-11-22, free memory 2203 MB with SOLIDWORKS restarted from its exe; launcher `%LOCALAPPDATA%\solidworks-mcp\livetest\run-1121\check5.ps1`, launcher pid 20984, output check5.out.txt and check5.err.txt beside it; started and watched by a Sonnet subagent; no resume; about 11 min.
- [x] S100 (a) and (b): committed (split out as S132) (`af90447`)
- [x] S100 (c): committed (split out as S133) (`def120d`)
- [x] S100 (d): committed (S100 to Waiting) (`4b13b83`)
- [x] S102: committed (`3a19df8`)
- [x] S103 (a) and (b): committed (the failed-split hint, changed after its live run, has no live test) (`285249f`)
- [x] S127: committed (`8f88347`)
- [x] S128: committed (`abe65d4`)
- [x] S130: committed (`8949e93`)
- [x] S131: committed (`67e7395`)
- [x] S127 again: committed (`05e4ee3`)
  Group 6's full live tests (2026-11-24, on `67e7395`, 13:21 to 16:36, `group6.ps1`, `group6b.ps1`, `group6c.ps1` in
  `run-1121`, Sonnet, 139k tokens) stopped the release before publishing: S127's own
  `DrawingLiveTests.Detail_center_is_the_center_of_its_bounds_in_both_reads` fails in the class run and alone after a
  restart, the same numbers both times: `boundsMm [276.309, 156.766, 319.389, 199.846] in drw_view_detail, [253.509,
  133.966, 342.189, 222.646] in drw_info`, the centre equal, the outline 43 x 43 mm in the creating call and 89 x 89 mm
  in `drw_info` (log `livetest\logs\DrawingLiveTests.Detail_center_is_the_center_of_its_bounds_in_both_reads.log`).
  It passed in S127's step: the outline read in the creating call may be stale (as S127 found for `Position`), or S128
  or S130, after it, change it. For a fresh default-model subagent, this report in its brief. Every other class passed
  but the known S5 and S72 tests, one flake (S112's state, passed alone) and guard stops passed rerun (SimulationExtras'
  nonlinear test at `-MinFreeMB 100`): SketchEdit 79, PartFeature 53, FeatureManagement 41, AdvancedFeature 30,
  BodySheetMetal 52, Reference 47, Modelling 10, SurfaceShape 8, Weldment 18, FaceEdit 5 of 6, Molding 4, Properties 26
  of 27, Assembly 65, Drawing 64 of 65, Export 21, M0 25, TopDown 4, Recognition 1, AssemblyStructure 2, Configuration
  6, Hardware 3, Visuals 3, Simulation 17, SimulationExtras 3; unit 358, `check_repo` ok. Nothing built, pushed or
  registered; `docs/tools.md` still to regenerate (`doc_open`, `doc_save` changed).
  Decided 2026-11-24 by the run, not the owner: after S127 again, the release's live tests rerun DrawingLiveTests in
  full and the classes that call the code it changes, not the other classes, which passed on `67e7395` and call no
  drawing code, because a fix confined to the drawing services leaves their result as it was.
- [x] Group 6: full live tests and release (build: `20261124-174916`, `3276b18`) (`136ee52`)
  Live: SketchEditLiveTests 79, PartFeatureLiveTests 53, FeatureManagementLiveTests 41, AdvancedFeatureLiveTests 30,
  BodySheetMetalLiveTests 52, ReferenceLiveTests 47, ModellingLiveTests 10, SurfaceShapeLiveTests 8, WeldmentLiveTests 18,
  FaceEditLiveTests 5 of 6, MoldingLiveTests 4, PropertiesLiveTests 26 of 27, AssemblyLiveTests 65, DrawingLiveTests 65,
  ExportLiveTests 21, M0LiveTests 25, TopDownLiveTests 4, RecognitionLiveTests 1, AssemblyStructureLiveTests 2,
  ConfigurationLiveTests 6, HardwareLiveTests 3, VisualsLiveTests 3, SimulationLiveTests 17, SimulationExtrasLiveTests 3,
  TemplateLiveTests 0 of 1 (skips itself: the 2025 templates are not installed): 588 passed of 590 run in 25 classes. The
  first full run was on `67e7395` (13:21 to 16:36, `group6.ps1`, `group6b.ps1`, `group6c.ps1`); it stopped at S127's own test
  (see "S127 again" above); after `05e4ee3` (which changes only `DrawingService.DetailAsync` and the detail test) the run
  decided on 2026-11-24 reran only DrawingLiveTests in full, the one class whose tests call `drw_view_detail` (grep of the
  live tests), `group6d.ps1`: 65 of 65 passed, 17:11 to 17:48 (36 min; the runner's own summary line reads `status=failed
  exit=` for it, an empty exit code beside "Passed! total 65 failed 0", as a second launch had left `summary.txt` locked for
  a moment; its test log and the dotnet summary both read 65 passed). The other 24 classes' counts are those of the run on
  `67e7395`, and as `05e4ee3` touches no code they call, they were not rerun. All passed but two, neither a fault of this
  group's code: `FaceEditLiveTests.Delete_heals_a_hole_in_an_imported_STEP_body_without_history` (S5, in the class run and
  alone after a restart) and `PropertiesLiveTests.A_custom_material_is_listed_…` (S72). One flake:
  `AssemblyLiveTests.An_assembly_adopted_after_the_server_saved_its_parts_does_not_read_as_changed_by_the_user` failed in
  the class run (S112's equation state), passed 1/1 alone after a restart. Guard stops, not test failures (free memory low,
  the guard at `-MinFreeMB 400`): AssemblyLiveTests twice (two tests, rerun alone, 3 of 3) and SimulationExtrasLiveTests at its
  start, each test alone; `A_nonlinear_study_of_a_small_load_matches_the_linear_one` stopped again and passed 1/1 alone at
  `-MinFreeMB 100` (`group6c.ps1`). SOLIDWORKS' default drawing template: not read again in this step (the S108 tests set and
  restore it). Run class by class with SOLIDWORKS restarted before each and `dotnet build-server shutdown`. Unit 358,
  `check_repo` ok. Descriptions changed (doc_open, doc_save): `--check-catalog` ok (165 tools, 247.6 KB), `docs/tools.md`
  regenerated and committed alone (`3276b18`, as the owner, no trailer), `sw2023` pushed to `origin`; published from the
  main tree, clean at that commit (`open_items.md` unmodified then). Registered in `claude_solidworks`, `clove sorter` (its
  `SWMCP_VBA=on` kept) and this repo, each read back from `~/.claude.json` on the new exe; tag `build-20261124-174916`
  pushed and read back (`ls-remote`), release page with no files (`ahmadaziz6720/solidworks-mcp` releases,
  `build-20261124-174916`). The nine items of the build (S100, S102, S103, S127, S128, S130, S131, S132, S133) are BUILT,
  proof: the scenario test of this build. SOLIDWORKS restarted from its exe at the end.
  Cost: Sonnet, 109k tokens, 41 min.
- [x] Group 6: scenario test (session id: `c71866a6-d4e8-4628-9225-f00fe0a0daa8`), its entry committed and sorted (entry `bc8c8a9`; ended `EXIT 0` 18:03; S132, S133, S100, S102, S103, S127, S128, S130 and S131 held and closed, S100 whole; S134 and S135 new into group 7; S136 (S14 again, reported `ok` while wrong, a probe first), S137, S138 and S139 new, not in this run; S5 again; release notes updated) (`8a3a447`)
  Started 17:53:58 2026-11-24, free memory 1809 MB with SOLIDWORKS restarted from its exe (twice; under the plan's 2 GB, started anyway); launcher `%LOCALAPPDATA%\solidworks-mcp\livetest\run-1121\check6.ps1`, launcher pid 16492, output check6.out.txt and check6.err.txt beside it; started and watched by a Sonnet subagent (61k tokens); no resume; ended `EXIT 0` 18:03, about 10 min.
- [x] S80 (a) and (c): committed (split out as S140; sketch_start's description changed: docs/tools.md at the release) (`b33b246`)
- [x] S82 (a): committed (split out as S141) (`377fbdc`)
- [x] S86 (a): committed (split out as S142)
- [x] S101: probed and fixed in the tree, not committed: the server's own reads per entity were
  the cost (100 lines 165 s, 150 s of it in those reads; 17 s once read once a call), with a
  limit of 0.5 s a segment; the change (`SketchService.cs`, `SketchRelations.cs`) and its test
  (`SketchEditLiveTests.cs`) carried into the run of 2026-11-27 as its first step
- [x] S123, S124, S125, S134 and S135: not started here; closed 2026-11-27 on the owner's word,
  carried into the run of 2026-11-27 (S123 its group 1, S124 and S125 group 2, S134 and S135
  group 5), their decisions and tests as written above
- [x] Group 7: full live tests and release: not run; S140 to S142 (S80 (a) and (c), S82 (a),
  S86 (a)), committed here and never built, go into the build of the run of 2026-11-27's
  group 1, whose scenario test is their proof
- [x] Group 7: scenario test: not run, the same
- [x] The report (2026-11-27, at its closing): six groups built and checked (builds
  `20261121-205309`, `20261122-002030`, `20261122-030022`, `20261122-142805`,
  `20261122-194424`, `20261124-174916`); held and closed: S61 (a) at its third fix, S83,
  S107, S110 (group 1); S112 (S92 (a) and (b)), S113 (S79 (b)), S95 (group 2); S116 (S92
  (d)), S114 (group 3); S119 and S84 (S84 (a) to (c)), S120, S121, S122 (S64 (l), (i), (c)),
  S93, S117 (group 4); S98, S108, S90, S106, S126 (S52 (d)), S97, S104, and S1 by its proof
  (group 5); S132, S133 and S100 (S100 (a) to (d)), S102, S103, S127, S128, S130, S131
  (group 6). Probed and not reproduced, still Open: S92 (c), S81, S109. Waiting on a
  session that meets their case: S89, S118. Group 7: S140 to S142 committed and not built,
  S101 fixed in the tree, the rest not started, all carried. New items its checks found and
  left: S111, S115, S129, S136 to S139; taken in mid-run and closed: S114, S117, S123 to
  S125 (carried), S127, S128, S130, S131, S134 and S135 (carried). Its leftovers filed at
  the closing as S158 (a preference changed by a live test, seen again) and S159 (an equal
  chamfer's other leg, outside S121's step). Closed by the owner (2026-11-27: "run baru:
  gabungkan sisa Run 2026-11-21 ... dengan item Open yang baru, S143–S157 dan part
  barunya") so its group 7 goes with the items found since, rather than in a build of its
  own.
