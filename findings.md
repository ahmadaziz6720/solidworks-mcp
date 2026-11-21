# Findings

What sessions found using `solidworks-mcp`, this fork's server, written by them without
being asked, one dated entry per session at the end of this file. Those sessions mostly
run in other repos: by the owner's Git rule they write here and never commit. The
paragraph that tells them to is in the owner's global `CLAUDE.md`.

What an entry holds:

- a call that failed, or that reported `ok` while the result was wrong (these matter
  most: the server's value is that its checks can be trusted);
- a workaround the session needed, and what it cost (calls, a document thrown away);
- a tool or option that was missing for the task at hand;
- a call pattern that wasted tokens (many calls for one thing, a large result read once);
- what the session learned about using the server on SOLIDWORKS 2023 that the next
  session should know.

Check [`open_items.md`](open_items.md) first: when a point bears on an item, name it
(`S3, met again`). A session with no findings writes no entry.

One entry: heading `**YYYY-MM-DD: <what was done, in general terms>**`, then a line naming
the server build (the `bin\0.1.0-<stamp>` folder `sw_status` or the registration points
at), then numbered points. Each point names the tool, what happened against what was
expected, the workaround if any, and whether the server reported success.

**The problem only, never its source** (owner's rule, 2026-11-09; this fork is public once
pushed). An entry does not say where the problem was met:

- no repo, project or client name;
- no file, part, assembly, drawing or document name, and no path;
- no report, section or session reference.

It describes the geometry in general terms ("a slope rising 7.5° from the end of a flat
line", "a sheet-metal part with a blind hole"). It gives the tool calls and the numbers
that show the fault, and nothing that identifies the design. The same holds for the items
in `open_items.md` and `closed_items.md` and for an issue or PR sent upstream.

**Sorting.** A session in this repo sorts the entries into `open_items.md`, and adds a line
under each entry it sorted, naming where each point went by the entry's own numbers:
`Sorted into S6 (1, 3); S2 (2). Held: S1 (5). Not filed (4): one-off.` (`Held:` names the
Waiting items a point proves.) Nothing here is decided until it is an item there. An entry is not edited once its session has ended, and none is deleted.

---

**2026-11-09: first tests of the fork on SOLIDWORKS 2023 SP3**

Written by the session that set up this register, from that day's four test sessions.
Builds `0.1.0-20261109-145700` (interop patch) and `…-163234` (plus the section fix).
Tasks: a sheet-like tray part with sloped walls, ears and holes; a hinge assembly with a
limit mate; an A3 working drawing of the tray with two sections, made twice.

1. **`drw_view_section` drew its cut line at `viewCentre + scale · input`**, so both sections
   missed the part, while every check reported passed. Cause in `DrawInView`
   (`Xform.Identity`). Fixed locally (`1de5067` + `3f43c50`), run and held in build
   `…-163234`: both sections hatched and in place. Sent as PR #1.
2. **After any `drw_view_section` the view's sketch stays open**: `doc_undo` refuses with
   `SKETCH_STATE`, and `sketch_finish` refuses drawings (`WRONG_DOC_TYPE`). Nothing added
   after a section can be undone. Still there with the fix.
3. **No tool deletes or moves a drawing view, dimension or note.** With 2, every misplaced
   annotation meant throwing the drawing away: the second drawing test was rebuilt four
   times (≈160 calls against ≈45 for a clean run). The largest token cost of the day.
4. **`drw_dimension_add` kept two wrong dimensions**: an angular that came out as the
   supplement (155.77° for 24.23°) was reported as "matches the geometry", and a
   `vertical` between two edge-on circles came out as a 180° angle, unchecked. Issue #5.
5. **`doc_open` on a valid STEP gave 16 surface bodies and no solid, reported `ok`**; an
   older AP203 STEP failed with a modal dialog. 3D Interconnect is forced off, and the
   legacy translator fails on this install. File > Open gives the right solid. Each import
   also leaves system option `Double 2` changed. Issue #2.
6. **`sketch_dimension_add` angular refuses the acute angle at a shared vertex**
   (`VERIFY_FAILED`, "7.5° instead of 172.5°") although 7.5° was asked. Workaround: a
   construction line collinear with the first line, the other way. Issue #3.
7. **`dimension_set` left the part in two solid bodies without a warning** (an extrude
   raised past a later cut's profile); seen only as `bodies: 2`. Issue #4.
8. **A fully defined sketch costs about 15 calls**: `sketch_relation_add` and
   `sketch_dimension_add` take one relation or dimension each. The tray's geometry took
   ≈53 calls, two sketches of ≈15 calls each.
9. **Five `sw_help` calls at session start read 23.5k characters**, the largest result of
   the modelling session. A session that knows the tools does not need them.
10. **Drawing output that needs manual work**: dimensions as `100,00` (2 decimals, decimal
    comma from the locale, no precision option); no parentheses for reference dimensions;
    hole callouts without `4×` / `6×` and as blind depth (the holes were modelled blind,
    one feature each); the title block's DWG NO is the file name, material and weight
    empty; "SECTION A-A" labels fixed below the view; an aligned section cannot be
    unaligned or moved.
11. **A dimension at a section's cut face is not possible**: `drw_dimension_add` takes model
    edges and vertices only, so the wall height where the plane cuts (42.5) went into a note.
12. **Section orientation, not in the tool description**: the arrows point along the line
    direction turned 90° clockwise, and the view is drawn as seen along them. Above a top
    view, a line drawn right → left gives the section floor-down.
13. **Lesson**: make the risky drawing dimensions (angles, edge-on circles) before the first
    section, while `doc_undo` still works, or try them on a throwaway drawing.
14. **Held, worth keeping**: Hole Wizard countersink with `dimension_set` on
    `Near C'Sink Dia.` (6.9 → 6.5) survives a forced rebuild; `material_add` +
    `material_set` gave PETG without registering a folder; custom properties linked to a
    dimension (`"D1@Wall"`) read back; hinge and limit mates stop the drag at both limits;
    interference matches the hand value; saving to the project folder works without
    `SWMCP_WRITE_ROOTS`.
15. **Small**: a component reported `fully` defined after the limit mate, `under` after a
    drag. Two open documents can share a title (an import named like its source); handles
    tell them apart.

Sorted into S1 (1); S2 (2); S3 (3); S4 (4); S5 (5); S6 (6); S7 (7); S8 (8); S9 (9);
S10 (10); S11 (11); S12 (12). Held: (13), (14), kept here as notes. Not filed (15): minor,
seen once.

**2026-11-09: drawing and setup lessons from the first tests, moved here from a memory**

Builds `0.1.0-20261109-145700` and `…-163234`. These were kept as a memory in the testing
workspace; they are workarounds for the server, so they belong here. Point 6 is the
baseline for S8 and S9, measured before any fix, to compare against after.

1. **`drw_dimensions_auto` is not usable.** On a part whose sketches are fully defined it
   gave 25 dimensions: construction dimensions of a cut profile and of an extrude height
   that only serves a later cut, two of the same length, overlapping texts, some off the
   sheet, and none for features whose sketches had no dimensions. With no tool to remove
   them (S3) the drawing was restarted. Reported as success. Workaround: manual
   `drw_dimension_add` only.
2. **Which model axis points where on the sheet is not reported.** In a top view model −Z
   points up the sheet. `textAtMm` for an angular is placed by that sign; guessed wrong,
   the dimension came out as the supplement (S4). Proposal: the view's model-to-sheet axes
   in the `drw_view_add` / `drw_info` result.
3. **The sheet format's drawable area is not reported.** The ISO A3 format's left border is
   at x ≈ 19 mm; a note placed at x = 18 overwrote the zone letters. Proposal: border and
   title block rectangles in `drw_info`.
4. **A projected side view from a top view came rotated 90°** (model Y horizontal), not
   usable as a side view, with no option to rotate it (upstream ROADMAP T10.10).
5. **Hole callouts follow the model, with no override**: `n×` only when the holes are one
   feature patterned, "through" only for through holes. Holes made one feature each, blind
   to the plate's thickness, gave `Ø4,50 ↧5,00` without a count (S10). Usage until then:
   model holes as one feature plus a pattern, end condition through.
6. **Baseline before S8/S9, build `…-163234`**: tool schemas 161 tools, 247 690 characters
   (median 1 355, `sketch_add` 4 407); server instructions 2 786. A part with two fully
   defined sketches, shell, mirror and two Hole Wizard holes: ≈53 calls for the geometry,
   ≈35k characters of results, 560–750 characters per call on average; each fully defined
   sketch ≈15 calls. Five `sw_help` calls at the start: 23.5k characters. A two-section A3
   drawing: ≈45–68 calls in a clean run, ≈160 with four restarts (S2, S3).
7. **Setup**: `tools\install.ps1` without `-NoRegister` registers the server at user scope
   under the name `solidworks`, which clashes with any other server of that name. Register
   by hand at local scope instead.

Sorted into S13 (1); S14 (2, 3); S10 (4, 5); S8, S9 (6); S15 (7).

**2026-11-09: which build a session is running cannot be told from inside it**

Builds `…-163234` (running) and `…-181555` (registered).

1. **`sw_status` reports `"serverVersion":"0.1.0"` only**, with no build stamp and no commit.
   A session that was open while the server was rebuilt and registered again under a new
   name kept running the old process (the old name, build `…-163234`), while
   `claude mcp list` already showed the new exe (`…-181555`). Nothing inside the session
   shows the difference. Entries here are asked to name the build, and test results are
   kept per build, so a wrong guess pins a result to the wrong commit. Proposal: the build
   stamp and the commit in `serverVersion` (for example `0.1.0-20261109-181555+18cbc57`,
   from the publish folder name and `SourceRevisionId` / `InformationalVersion`), and a
   `-dirty` mark when built from a tree with uncommitted changes.

Sorted into S16 (1).

**2026-11-09: scenario test of the angular fix: a shelled part with two fully defined sketches, mirror and Hole Wizard holes, then a two-section A3 drawing**

Build `…-181555` (`18cbc57`, from the registration; `sw_status` shows `0.1.0` only).

1. **S6 held.** `sketch_dimension_add` angular between a flat line and a slope rising 7.5°
   from its end (shared vertex), `textAt` inside the 7.5°, `valueDeg` 7.5: accepted at once,
   checks `"textAt lies in the supplementary angle, so 7.5° is dimensioned, not the 172.5°
   between the rays"`, sketch fully defined without the construction line. The cut gave
   the same volume as before, and later `dimension_set` 7.5 → 10 → 7.5 on that dimension
   rebuilt clean with one body. The other side still works: `textAt` in the obtuse sector
   with `valueDeg` 170 was accepted and measured 170. That sketch now takes 11 calls (16
   with the workaround).
2. **Angular without `textAt` on the same pair of lines, `valueDeg` 10: refused with a hint
   that points the wrong way.** `VERIFY_FAILED: After setting 10°, the geometry measures
   170° (the sketch did not solve to it); the old value was restored and it was removed.
   Hint: Other relations or dimensions may fix this geometry; check sketch_info.` The
   sketch had no relations or dimensions at all. The default text point lies in the ray
   angle (172.5°), so the obtuse one was dimensioned and the value given was the acute one.
   Correctly refused and rolled back; only the hint misleads. Proposal: when `textAt` is
   absent and `valueDeg` is nearer the supplement than the ray angle, either take the
   supplement or say "pass textAt inside the angle you mean". Bears on S6.
3. **S2, met again**, and its hint is wrong: `doc_undo` after `drw_view_section` →
   `SKETCH_STATE: Sketch 'Sketch3' is open; undo would act on the sketch edits. Hint: The
   user is editing it; ask them to exit the sketch first.` Nobody was: the section call
   left the view sketch open (`sw_status` `openSketch`).
4. **`feature_mirror` warns on a correct result**: mirroring a boss whose base face merges
   into a face of the body gave `"The new faces' area is 1.05 x 1 seed copies: instances
   overlap, merge or are clipped."` Volume and 9/9 reflected faces were right; same warning
   in build `…-145700`. Noise that a session has to read past.
5. **Calls (S8/S9, before S8 is built)**: no `sw_help` call at all, the session followed a
   recorded call sequence. Part geometry, material, properties and the design-intent
   changes: 47 calls, 25.8k characters of results (baseline ≈53 and ≈35k plus 23.5k for
   `sw_help`). The drawing with two sections, up to the failed undo: 12 calls, 6.8k.
   Sections: both lines in plain sheet mm, cut where drawn, 1 and 3 hatched faces (S1 holds).

Sorted into S6 (1, 2); S2 (3); S17 (4); S8, S9, S1 (5).

**2026-11-09: moving a part with its drawings, and an assembly with its parts, to new folders by Pack and Go**

Build `bin\0.1.0-20261109-181555`.

1. **`doc_pack` from a document opened with `doc_open readOnly:true` left every copy
   referencing the originals.** Three runs: a drawing whose views all show one part, a
   second drawing of the same part, and an assembly of two parts, each into a new folder,
   no prefix or suffix. Each time the copies were written and the call failed with
   `VERIFY_FAILED: Some copies still reference other files: <drawing> → <original part path>`.
   The error is honest (the hint says the copies exist), but it does not say why, and
   read-only is the likely cause (not tested with a writable open). Workaround that held:
   close everything, remove the originals, reopen the copies. SOLIDWORKS then found each
   reference in the referencing document's folder, no missing-reference warning, so the
   copies work, though the paths stored in them are still the old ones. Proposal: refuse a
   read-only source up front, or reopen it writable for the pack, or name read-only as the
   cause in the error.
2. **`doc_pack includeDrawings:true` on a part whose drawings lie in another folder copied
   no drawing and reported success** (`1 file(s) written`). Pack and Go only looks for
   drawings next to the model, which the description does not say. Proposal: say so in the
   description, and add `noDrawingsFound` (or similar) to the result when the flag was set
   and none was copied.
3. **Missing: renaming a file with its references.** `doc_pack` only adds a prefix or
   suffix, so a part and its drawings cannot be given new names (e.g. a suffix dropped)
   without breaking the references. A rename tool, or `doc_pack` taking new names per file,
   would cover it.
4. **Every `doc_open` of a native part, drawing or assembly warned that system option
   `Double 2` changed** (e.g. `0.000131… → 0.0000787…`, a different value per document),
   telling the session to tell the user. So far it was noted only for STEP imports (S5).
   `Double 2` is `swSTLDeviation` (`SystemPrefIds.StlDeviation`), coupled with the STL
   quality. The values scale with the opened model (in metres: ≈0.13 mm for the largest
   part, ≈0.028 mm for the smallest assembly), which fits SOLIDWORKS deriving the deviation
   from the model size while the STL quality is a preset. `doc_open` does not write it, so
   SOLIDWORKS changes it on its own and the warning is noise on every open, asking the
   session to bother the user about an STL export setting. Proposal: leave the coupled STL
   group out of the after-call comparison unless the call itself wrote it. Bears on S5.
5. **`doc_close` of an assembly warned `SOLIDWORKS also closed: , .`**: the closed
   components' names are blank. Same on closing a drawing (`also closed: .`).

Sorted into S18 (1, 2, 3); S17 (4, 5).

**2026-11-09: scenario test of the angular hint without textAt: the shelled part with two fully defined sketches, holes and design-intent changes, then two drawing sections**

Build `bin\0.1.0-20261109-195750` (`f87a3df`, from the registration and `open_items.md`;
`sw_status` shows `0.1.0` only).

1. **S6, second part, held in a real sketch.** In the slope-cut sketch (closed profile, a
   horizontal line, two verticals, a vertical point relation and one dimension already in
   it), `sketch_dimension_add` angular between the flat line and the slope sharing its end,
   `valueDeg` 7.5, no `textAt`: `VERIFY_FAILED: After setting 7.5°, the lines make 172.5°
   between the rays, so 7.5° went to the angle across the vertex; the dimension was on the
   172.5° between the rays, not the 7.5°; … Hint: To set the 7.5° angle, pass textAt inside
   it, e.g. textAt [60.388, 38.958] (sketch mm).` The point lies on the bisector of the
   acute wedge. The same call with that `textAt` was accepted (`textAt lies in the
   supplementary angle`), the sketch went fully defined, and every later check of the
   part came out as before (cut volume, holes, mass, the angle driven 7.5 → 10 → 7.5 with
   one body). In a bare sketch of two free lines, the earlier repro (`valueDeg` 10, no
   `textAt`) gave the same message and hint, and following the hint set 10° (geometry
   measures 10°). The plain case is unchanged: 170° without `textAt` on a second pair was
   accepted and measured 170°. One extra call over a sketch made with `textAt` from the
   start.
2. **The texts name the acute angle by its current size, which reads like a value when
   another one is asked.** With `valueDeg` 10 the hint says `To set the 7.5° angle, pass
   textAt…`, and the accepted call then checks `"so 7.5° is dimensioned, not the 172.5°
   between the rays"` while the value set and measured is 10°. A session can take 7.5 for
   a value to use. Proposal: "the acute angle (now 7.5°)" or "the angle that is now 7.5°".
   Wording only; the server reported correctly. Bears on S6.
3. **S17, met again**: `feature_mirror` of the two ear bosses still warns `"The new faces'
   area is 1.05 x 1 seed copies: instances overlap, merge or are clipped."` on a correct
   result (volume and 9/9 reflected faces right).
4. **Calls (S8/S9)**: part geometry, material, properties and design intent 51 calls,
   27.4k characters of results, no `sw_help` (the slope sketch 12 calls, one of them the
   deliberate refusal). The bare two-line check 8 calls; the drawing with both sections 8
   calls, 3.7k. Both sections cut where drawn, 1 and 3 hatched faces (S1 holds).

Sorted into S6 (1, 2); S17 (3); S8, S9, S1 (4). The build line: S16. Point 4 of the
`…-181555` entry, rewritten in the same session: S17.

**2026-11-09: scenario test of the drawing-sketch fix: a two-section A3 drawing of a shelled part with 23 dimensions, two hole callouts and notes, with undo tried after each kind of step; short checks of a part, a STEP round trip and an assembly**

Build `bin\0.1.0-20261109-204719` (`44fb220`, from the registration and `open_items.md`;
`sw_status` shows `0.1.0` only).

1. **S2 held.** `sw_status` and `model_info` show no open sketch in a drawing: a new one,
   after a standard view, after each of two `drw_view_section`, and in a saved drawing with
   two sections opened read-only. `doc_rebuild` runs in both (it refused with
   `SKETCH_STATE` before), and `doc_undo` after a section no longer refuses for a sketch.
   In a part a real open sketch is still reported (`openSketch: Sketch1`) and
   `doc_rebuild` still refuses it; after `sketch_finish`, `doc_undo` of a boss extrude
   took it back (−1 617 000 mm³). Both sections cut where drawn, 3 and 1 hatched faces
   (S1 holds).
2. **S19, met again, and worse than refusing: `doc_undo` removed a sheet note while
   reporting that nothing changed.** With a section on record, `drw_note_add` then
   `doc_undo` answered `VERIFY_FAILED: Nothing changed: SOLIDWORKS had nothing to undo, or
   the last step can't be undone. Hint: Check the model with feature_list.`, while
   `drw_info` counted the sheet's notes 65 before and 64 after; the note was gone from the
   sheet. Seen twice: once with a 17-line note, once with a one-word probe note, counted
   before and after. A session that trusts the error goes on without the note. Also as
   S19 says: `doc_undo` after `drw_views_standard` and after a `drw_dimension_add` (no
   section yet) refuses `NOT_OWNED`, and after a section `VERIFY_FAILED` with the section
   kept. The hint names `feature_list`, which says nothing about drawing annotations.
3. **`drw_dimension_add` returned 9 of 23 dimensions without a value check, silently.**
   Every one with a circular edge in it (a line to a hole edge, two hole edges, `vertical`
   or `horizontal`): the result has no `expected` and no "matches the geometry" among the
   checks, and no warning that the value was not compared, while the tool says each value
   is verified or removed. The other 14 (lines, vertices, angles, single edges, both
   callouts) carry the check. Here all 9 values were right by the design; with a wrong
   pick they would have been kept the same way. Bears on S4.
4. **S17, met again.** Every `doc_open` (drawing, part, assembly, STEP import) warned that
   system option `Double 2` changed, each time another value. `doc_close` listed the
   documents it also closed with blank names (`also closed: .` / `, .`).
5. **A document opened `readOnly:true` became dirty without an edit**: an assembly after
   only `asm_interference`, `model_info` and `view_screenshot`; a drawing after
   `doc_rebuild`. `doc_close` then refused with `DIRTY` and needed `discard:true`: one extra
   call each.
6. **Large results read once**: `drw_note_add` echoes the whole note text (1.4k characters
   back for a 1.4k note); `sw_status` repeats ten `recentOps` with their full error texts,
   up to 2.6k characters per call.
7. **Calls (S8/S9)**: the drawing 50 calls, 30.8k characters of results, of which 16 were
   this test's undo, status, rebuild and count probes and the note added again; the drawing itself followed a
   recorded call sequence, no `sw_help`, nothing rebuilt. Whole session 79 calls, 43.9k.
   STEP export and import still give 16 surface bodies and no solid (S5).

Sorted into S2, S1 (1); S19 (2); S20 (3); S17 (4); S21 (5); S22 (6); S8, S9, S5 (7). The
build line: S16.

**2026-11-09: scenario test of the drawing-undo refusal: undo tried in a new drawing before and after a section, and in a part; short checks of a STEP round trip and an assembly**

Build `bin\0.1.0-20261109-215810`; `sw_status` reports `serverVersion`
`0.1.0-sw2023-20261109-215810`, tag `build-20261109-215810` on `6bb5d94`.

1. **S19 held.** `doc_undo` in a drawing refused `NOT_SUPPORTED` before anything was
   undone, both after a plan view and a note (no section yet) and after a section and a
   second note. `drw_info` read back the same views and every note: 2 views, the second
   note still there (sheet notes up by one, not down as in build `…-204719`). The hint
   (undo in SOLIDWORKS with Ctrl+Z, then `drw_info`) is clear and blames no one. In a
   new part, `doc_undo` after a boss extrude still removed it (volume back to 0).
2. **S16, the stamp part held.** `sw_status` names the build without the registration
   being read. The commit is still not in it; it came from the tag.
3. **S17, met again.** Each `doc_open` (a part read-only, a STEP import, an assembly
   read-only) warned that `Double 2` changed, each time another value. `doc_close` of a
   new drawing warned `also closed: .`, of the assembly `also closed: , .`.
4. **S21, met again.** An assembly opened `readOnly:true` was dirty after only
   `asm_interference`; closed with `discard:true`.
5. **S5, met again.** STEP AP214 export then `doc_open`: 16 surface bodies, no solid.
6. **Calls**: 28 for the whole test, 13.2k characters of results, 2 errors (the two
   intended refusals). No `sw_help`.

Sorted into S19 (1); S16 (2); S17 (3); S21 (4); S5 (5); S9 (6).

**2026-11-09: scenario test of the drawing edit tools: list, move, delete and note text in a finished drawing (a plan view and two section views, 21 dimensions, 2 hole callouts, a 17-line sheet note); short checks of undo in a part, a STEP round trip and an assembly**

Build `bin\0.1.0-20261109-230924`; `sw_status` reports `serverVersion`
`0.1.0-sw2023-20261109-230924`, tag `build-20261109-230924` on `1dcb6ed` (code `ddca400`).

1. **S3 held.** `drw_annotation_list` per view gave every dimension and callout with name,
   value and position (13 + 2 callouts, 5, 3). `drw_annotation_edit` moved a dimension and
   the sheet note (positions read back), deleted a dimension (view 25 → 24 annotations, no
   longer listed). `drw_view_edit` move of the parent view by +10 mm in x: both section
   views followed, named in `alsoMoved`, their dimensions and the callouts with them. A
   section view moved off its alignment was refused `VERIFY_FAILED`, put back (bounds
   before the next move confirm it) and the hint said to move along the alignment; along
   it the move held. `drw_view_delete` of a section view removed it with its 5 dimensions
   and label. `doc_undo` in the drawing refused `NOT_SUPPORTED` with the hint naming the
   three edit tools. Saved, exported to PDF: the PDF shows every change.
2. **`drw_annotation_edit` action `text` wrote a literal backslash-n into a note and
   reported `text read back`.** The parameter says "\n starts a new line". Typed as the two
   characters `\` `n`, the text was stored as one long line with `\n` printed in it; the
   check compared it with the input and passed. Sent again with real line breaks it was
   right. `drw_note_add` has the same wording and, from the code, the same handling. Either
   turn a literal `\n` into a line break, or refuse text that holds one, and say "a line
   break" in the description.
3. **`drw_view_edit` move: "no overlaps with other views" while the moved view's label
   landed on another view's annotation.** A section view above its parent lowered 4 mm
   along its alignment: its label ("SECTION X-X") then sat on the letter of the other
   section line's arrow in the parent view. The check covers view bounds only, not labels,
   section-line letters or dimensions. Bears on S14.
4. **`drw_view_delete` of a parent took its section view with it**, against the
   description ("the child views … that stay"). Reported honestly (`removed` lists both, a
   warning says SOLIDWORKS also removed it), but `annotations` counts the named view only
   (14 dimensions, 10 center marks), not the section's 3 dimensions and label that went
   too. The description could say a section or detail view goes with its parent, before a
   caller relies on it staying; nothing brings it back in a drawing (no undo).
5. **Sheet-format notes are listed and edited as if they were the caller's.**
   `drw_annotation_list` for the sheet's notes returned 65, of which 64 belong to the sheet
   format (border zone letters and numbers, title block labels and fields): 9.5k characters
   to find the one note the session made, with nothing marking which is which.
   `drw_annotation_edit` then moved a border zone letter, reported `position read back`, and
   the drawing reported `dirty:false` afterwards (closed without `discard`), so whether the
   move would have been saved is unknown. Suggest a `format:true` flag and a filter, and a
   refusal or a warning for an edit in the format. Bears on S14, S22.
6. **A section view's label is listed with text `" "`** (a single space), kind `note`, in
   both section views: the caller cannot tell it is the view label.
7. **Lesson**: moving a parent view in x also moved the section view placed beside it
   (aligned horizontally) by the same 10 mm in x; `alsoMoved` says so. Plan the parent's
   move with every child in mind.
8. **S22, met again.** A `drw_annotation_edit` move or text change of the 1.4k-character
   note returned the whole text twice (before and after), 3.2k characters per call.
9. **Short checks.** In a new part `doc_undo` after a boss extrude still removed it (volume
   → 0). An assembly opened `readOnly:true`: `asm_interference` 0 at the saved position,
   then `doc_close` refused `DIRTY` (S21, met again). STEP AP214 export reported `solids:
   1`; `doc_open` of it gave 16 surface bodies, no solid (S5, met again). Every `doc_open`
   warned `Double 2` changed, each time another value, and `doc_close` of the drawing and
   the assembly listed `also closed: .` / `, .` (S17, met again).
10. **Calls**: 41 for the whole test, 43.2k characters of results, 3 errors (the two
    intended refusals and the `DIRTY` above). The drawing edits 25 calls; the sheet's note
    list (9.5k) and the three note edits (3.2k each) are 44% of the characters. No
    `sw_help`.

Sorted into S3 (1, 2, 4 to 7); S14 (3); S22 (8); S19, S21, S5, S17 (9); S9 (10).

**2026-11-10: scenario test of the drawing edit tools' follow-ups: a literal backslash-n in a note, deleting a parent view, the sheet format's notes, a section view's label and the cut echo, in a finished drawing (a plan view and two section views, 21 dimensions, 2 hole callouts, a 17-line sheet note); short checks of undo in a part, a STEP round trip and an assembly**

Build `bin\0.1.0-20261109-235702`; `sw_status` reports `serverVersion`
`0.1.0-sw2023-20261109-235702`, tag `build-20261109-235702` on `3926863` (code `79c7b4b`).

1. **S3's five follow-ups held.** (a) `drw_note_add` with `\` `n` typed as two characters
   stored three lines (read back by `drw_annotation_list`, and the PDF shows them);
   `drw_annotation_edit` action `text` the same, two lines. (b) `drw_view_delete` of the
   parent was refused `INVALID_ARGUMENT`, naming both section views, nothing deleted; with
   `withChildren:true` all three went, `removed` lists them, and `annotations` summed every
   view (15 + 5 + 3 = 23 dimensions, 2 labels, 10 center marks); the sheet's notes stayed.
   (c) The sheet's list returned the one note the session made, `formatNotesSkipped: 64`;
   with `format:true` the 64 are marked `format: true`; a move of a border zone letter was
   refused with a hint pointing to `prop_set` for the title block. (d) A section view's
   label is `kind: view_label`, and a move of it read back. (e) A move of the 1.4k note
   echoed `NOTES… (1405 characters)` in `before` and `after`, 371 characters for the call.
2. **`drw_note_add` echoes its input, not what was stored.** With `\n` typed as two
   characters the result's `text` held the backslash and the n (shown escaped in the JSON),
   next to `text read back`, while the note held real line breaks. A caller reading the
   result would think the backslash went in. `drw_annotation_edit`'s `after` shows the
   read-back text; `drw_note_add` could do the same (and cut it as in edits, S22).
3. **A view label's `text` is a placeholder**, `"label of 'Section View X-X'"`, not the
   label's own text (`SECTION X-X` on the sheet). Enough to tell which view it belongs to;
   the label's wording cannot be read.
4. **S14, met again, for an annotation move.** `drw_annotation_edit` moved a section view's
   label 4.3 mm down, `position read back`, no warning; on the PDF the label's first letters
   sit on the arrow letter of the other section line in the parent view. Moving a label is
   new in this build, so it is the easiest way to land one on another view's annotation.
5. **`drw_info` still counts the sheet format's notes.** It reported `notes: 65` for the
   sheet (66 after one note was added) while `drw_annotation_list` reported 1 (2) with 64
   skipped. The two tools disagree on what a sheet note is.
6. **Short checks.** In a new part `doc_undo` after a boss extrude removed it (volume →
   0). STEP AP214 export reported `solids: 1`; `doc_open` of it gave 16 surface bodies, no
   solid (S5, met again). An assembly opened `readOnly:true`: `asm_interference` 0 at the
   saved position, then `doc_close` refused `DIRTY` (S21, met again). Every `doc_open`
   warned `Double 2` changed, each time another value; `doc_close` of the drawing and of
   the assembly listed `also closed: .` and `, .` (S17, met again).
7. **Calls**: 33 for the whole test, 27.1k characters of results, 3 errors (the two
   intended refusals and the `DIRTY` above). The drawing 17 calls; the one `format:true`
   list is 10.5k, 39% of the characters. No `sw_help`.

Sorted into S3 (1, 2, 3, 5); S22 (2, 7); S14 (4); S5, S21, S17 (6); S9 (7).

**2026-11-10: scenario test of the drawing follow-ups (text read back by `drw_note_add`, a view label's words, the sheet's note count in `drw_info`) in a finished drawing (a plan view and two section views, 23 dimensions, 2 hole callouts, a 17-line sheet note); short checks of undo in a part, a STEP round trip and an assembly**

Build `bin\0.1.0-20261110-010708`; `sw_status` reports `serverVersion`
`0.1.0-sw2023-20261110-010708`, tag `build-20261110-010708` on `7e99e8d` (code `c5df767`).

1. **S3's three follow-ups held.** (f) `drw_note_add` with `\` `n` typed as two characters
   returned `"text":"PROBE F\nLINE 2"`, a real line break in the JSON, not the escaped
   backslash of the last build; a 134-character note (two lines) came back as its first 60
   characters and `… (134 characters)`, the count of the stored text (the typed `\n` counted
   once). `drw_annotation_list` and the PDF show both notes on two lines. (g) The two view
   labels list as `SECTION A-A` and `SECTION B-B`. (h) `drw_info` gives the sheet `notes: 1`
   (3 after the two probes), the same as `drw_annotation_list` (1, 3; `formatNotesSkipped:
   64`).
2. **`drw_info` counts a view's label as a note.** Each section view reports `notes: 1`,
   while `drw_annotation_list` for that view with `kind: note` returns 0; the 1 is its
   `view_label`. The sheet-level count now agrees with the list, but the per-view counts
   still differ from the list's kinds. The labels could be counted apart or left out, as
   the template's notes were.
3. **S14, met again, for a new note.** `drw_note_add` at a point 120 mm from the sheet's
   right edge, a first line about 180 mm long at 2.5 mm text height: `position read back`,
   no warning. In the PDF the line runs past the page edge and its last words are cut off.
   The result has the position but not the note's extent, so a caller can't see the
   overrun without exporting the PDF.
4. **Short checks.** In a new part `doc_undo` after a boss extrude removed it (volume →
   0). STEP AP214 export reported `solids: 1`; `doc_open` of it gave 16 surface bodies, no
   solid (S5, met again). An assembly opened `readOnly:true`: `asm_interference` 0 at the
   saved position, then `doc_close` refused `DIRTY` (S21, met again). Every `doc_open`
   warned `Double 2` changed, each time another value; `doc_close` of the drawing and of
   the assembly listed `also closed: .` and `, .` (S17, met again).
5. **Calls**: 28 for the whole test, 16.0k characters of results, 1 error (the `DIRTY`
   above). The drawing took 12 calls; the two full sheet lists (1.6k and 2.0k, mostly the
   1.4k note in full) and the two `drw_info` calls (1.6k each) are 42% of the characters.
   No `sw_help`.

Sorted into S3 (1, 2); S14 (3); S5, S21, S17 (4); S9 (5).

**2026-11-10: scenario test of build 014952 (a view's label counted apart in drw_info)**

Build `bin\0.1.0-20261110-014952`; `sw_status` reports `serverVersion`
`0.1.0-sw2023-20261110-014952`, tag `build-20261110-014952` on `a0d55c5` (code `59a15f4`).

1. **S3's follow-up (i) held.** On a copy of a finished drawing (a plan view, two section
   views cut from it), `drw_info` gives each section view `notes: 0, viewLabels: 1`; the plan
   view and the sheet carry no `viewLabels` field. `drw_annotation_list` agrees: `kind:
   view_label` 2 (`SECTION A-A`, `SECTION B-B`), `kind: note` 1 (the sheet's own note). After
   `drw_note_add` into one section view, that view reads `dimensions: 5, notes: 1,
   viewLabels: 1`, the same as its list (5 dimensions, 1 `view_label`, 1 `note`). Then
   `drw_view_delete` of the plan with `withChildren` removed all three views and summed
   `dimensions: 23, notes: 1, centerMarks: 10, viewLabels: 2`, each matching the counts
   read before. Reported success throughout, and correct.
2. **S22, met again.** The sheet's `drw_annotation_list` with `kind: note` returned the
   one sheet note's 1.4k text in full (1.6k characters for one annotation).
3. **Short checks.** In a new part `doc_undo` after a boss extrude removed it (volume →
   0). STEP AP214 export reported `solids: 1`; `doc_open` of it gave 16 surface bodies, no
   solid (S5, met again). An assembly opened `readOnly:true`: `asm_interference` 0 at the
   saved position, then `doc_close` refused `DIRTY` (S21, met again). Every `doc_open`
   warned `Double 2` changed, each time another value; `doc_close` of the drawing and of
   the assembly listed `also closed: .` and `, .` (S17, met again).
4. **Calls**: 28 for the whole test, 16.5k characters of results, 1 error (the `DIRTY`
   above). The drawing took 11 calls. Each `drw_info` is 1.6k for three views, the model's
   full path repeated in every view. No `sw_help`.

Sorted into S3, closed (1); S22 (2, 4); S19, S5, S21, S17 (3); S9 (4).

**2026-11-10: scenario test of build 024431 (dimensions to circular edges checked)**

Build `bin\0.1.0-20261110-024431`; `sw_status` reports `serverVersion`
`0.1.0-sw2023-20261110-024431`, tag `build-20261110-024431` on `f07c42e` (code `8c88427`).

1. **S20 held.** On a copy of a finished drawing, the nine plan-view dimensions that came
   back unchecked in build `…-204719` were deleted and added again with the same calls
   (`horizontal` and `vertical`, a line to a hole edge or two hole edges). All nine now
   carry `expected` equal to `value` and a check naming how the circle was measured
   (`value 100 mm matches the geometry ('b' to its centre)`, `('a' to its centre, 'b' to
   its centre)`). Values right by the design; the exported PDF shows them where asked.
2. **A single circle with `linear` warns, as designed, but the warning reads garbled and
   the value could be checked.** SOLIDWORKS made it a diameter (Ø4.5 in the PDF); the result
   has `kind: linear`, no `expected`, and the warning "the expected linear between the
   circle alone is not computed here". The circle's diameter is known, so this one could be
   compared like `diameter`. Reported success, value right.
3. **An edge-on circle (not yet run by the live tests).** In a section view a hole's edge
   is seen edge-on as a line. `vertical` from a vertex to it: `expected 2`, `('b' to its
   centre)`, right. `horizontal` with the same two entities: SOLIDWORKS made 2 mm (the
   vertical distance, it seems) and the tool refused `VERIFY_FAILED` against the expected
   57 to the centre and removed it: the right outcome. The hint ("pass textAtMm next to the
   quantity … or use kind horizontal / vertical") was already followed by the call and does
   not say that SOLIDWORKS could not make that direction to an edge-on circle (S17).
   A near or far arc condition was not tried: no tool sets one.
4. **`drw_annotation_edit` delete moved other dimensions, unreported.** Three horizontal
   dimensions were stacked below the view (text at y 109, 101, 93). After the one at 109
   was deleted, the `before` of the next two deletes gave both at y 109: SOLIDWORKS had
   closed up the stack. The delete result says only the count went down by one and the
   name is no longer listed. Here they were deleted too, so no harm; in a drawing that
   keeps them, the layout changes without a word. Bears on S3.
5. **Short checks.** In a new part `doc_undo` after a boss extrude removed it (volume →
   0). STEP AP214 export reported `solids: 1`; `doc_open` of it gave 16 surface bodies, no
   solid (S5, met again). An assembly opened `readOnly:true` was `dirty: true` after only
   `asm_interference` (0 at the saved position); closed with `discard:true` up front
   (S21, met again). Every `doc_open` warned `Double 2` changed, each time another value;
   `doc_close` of the drawing and of the assembly listed `also closed: .` and `, .`
   (S17, met again).
6. **Calls**: 42 for the whole test, 21.2k characters of results, 1 error (the
   `VERIFY_FAILED` in 3). The drawing took 27: nine deletes and nine adds, one at a time,
   ~290 and ~590 characters each. No `sw_help`.

Sorted into S20, held, stays Waiting (1, 3); S20 follow-ups (a) and (b) (2, 3); S24, new
(4); S19, S5, S21, S17 (5); S9, S8 (6).

**2026-11-10: scenario test of build 034022 (S20 follow-ups: linear on one circle, edge-on hint)**

Build `bin\0.1.0-20261110-034022`; `sw_status` reports `serverVersion`
`0.1.0-sw2023-20261110-034022`, tag `build-20261110-034022` on `0ac1b7a` (code `8b0fbbd`).

1. **S20 follow-up (a) held.** On a copy of a finished drawing, `drw_dimension_add` `linear`
   on a single hole edge seen face-on: `kind: diameter`, `expected` equal to `value`, checks
   "SOLIDWORKS made a diameter dimension of the circle for kind linear" and "value … matches
   the geometry". The PDF shows it as a diameter, as asked.
2. **S20 follow-up (b) held.** In a section view, `horizontal` from a vertex to a hole edge
   seen edge-on: refused `VERIFY_FAILED` (SOLIDWORKS made 2 mm, the vertical distance) and
   removed, hint "SOLIDWORKS did not make a horizontal dimension to the centre of edge E2,
   a circle seen edge-on in this view. Dimension to a vertex or a straight edge instead, or
   in a view where the circle is seen face-on." Right and actionable. `vertical` with the
   same two entities is still checked (`expected 2`, `('b' to its centre)`), and a
   `horizontal` from a straight edge to a face-on hole edge still carries `expected` and
   `('b' to its centre)`. Not run: the hint for a mismatch with no edge-on circle (no case
   found that SOLIDWORKS gets wrong), and a near or far arc condition (no tool sets one).
3. **`linear` on a single edge-on circle: the warning now reads clearly, but SOLIDWORKS
   draws it as a diameter.** The result has `kind: linear`, no `expected`, and the warning
   "no expected linear value is computed for a circle seen edge-on alone in this view".
   The value is the circle's diameter, and the PDF prints it with a Ø prefix: SOLIDWORKS
   made a linear diameter dimension of the edge-on hole. The diameter is known, so it
   could be compared as in 1, and `kind` could say what the sheet shows. Reported success,
   value right. Small; bears on S20.
4. **A dimension laid on another accepted without a word.** A `horizontal` given
   `textAtMm` on the row of an existing dimension from the same edge: its dimension line
   lies on the other's, the two texts side by side, `textAtMm` echoed, no warning. Placed
   so by the call; the result does not show the clash. Bears on S14.
5. **Short checks.** In a new part `doc_undo` after a boss extrude removed it (volume → 0).
   STEP AP214 export reported `solids: 1`; `doc_open` of it gave 16 surface bodies, no
   solid (S5, met again). An assembly opened `readOnly:true` was `dirty: true` after only
   `asm_interference` (0 at the saved position) (S21, met again). Every `doc_open` warned
   `Double 2` changed; `doc_close` of the drawing and of the assembly listed `also closed:
   .` and `, .` (S17, met again).
6. **Calls**: 25 for the whole test, 11.6k characters of results, 1 error (the intended
   `VERIFY_FAILED` in 2). The drawing took 10. No `sw_help`.

Sorted into S20, held, follow-up (c) new (1, 2, 3); S14 (4); S5, S21, S17, S19 (5); S9 (6).

**2026-11-10: scenario test of build 064352 (S20 follow-up c: linear on one circle seen edge-on)**

Build `bin\0.1.0-20261110-064352`; `sw_status` reports `serverVersion`
`0.1.0-sw2023-20261110-064352`, tag `build-20261110-064352` on `b00311b` (code `23bd0d7`).

1. **S20 follow-up (c) held.** On a copy of a finished drawing, in a section view where a
   hole's edge is seen edge-on (drawn as a horizontal segment): `linear` on that edge
   alone came back `kind: linear`, `expected` equal to `value` (the hole's diameter),
   checks "SOLIDWORKS made a linear dimension with Ø across the circle, seen edge-on" and
   "value … matches the geometry", no warning. `horizontal` on the same edge: the same,
   `kind: horizontal`, checked. In a second section view, rotated so the same kind of
   edge is drawn as a vertical segment, `vertical` on it alone was checked too (the
   diameter, check with Ø). So the projected share works in both sheet directions. The
   PDF prints all three as Ø with the diameter.
2. **Follow-ups (a) and (b) still hold.** `linear` on a hole edge seen face-on: `kind:
   diameter`, checked. In the section, `horizontal` from a vertex to the edge-on hole:
   `VERIFY_FAILED` (2 mm made), removed, with the edge-on hint; `vertical` with the same
   two entities checked (`('b' to its centre)`).
3. **`vertical` on a single edge-on circle drawn horizontally: the hint is generic.**
   `FEATURE_FAILED: SOLIDWORKS created no vertical dimension on edge E1 …`, hint "Check
   that the entities can carry this kind of dimension (two parallel lines or two points
   for a distance, a circle for radial/diameter)". The edge is a circle, so the hint reads
   as if it should have worked. The server knows the circle is seen edge-on and in which
   sheet direction it is drawn; the hint could say so and point to `linear` or the other
   direction. Nothing made, failure reported honestly. Small; bears on S20.
4. **Dimension text outside the drawing frame accepted without a word.** A `textAtMm` at
   y 292 on an A3 sheet (297 high) put the text over the border's zone strip; the result
   echoed `textAtMm`, no warning. Placed so by the call. Bears on S14.
5. **Lesson: pick a hole's edge on the circle, not at its centre.** A point at a hole's
   centre resolved to its edge with "no edge exactly at …; used the nearest one, 2.25 mm
   away" (the radius), and `strict: true` refused it `ENTITY_NOT_FOUND`. Right, since the
   centre is not on the edge; a session that thinks in hole centres should expect it. The
   warning could say the point is the centre of the circle it chose.
6. **Short checks.** In a new part `doc_undo` after a boss extrude removed it (volume →
   0). STEP AP214 export reported `solids: 1`; `doc_open` of it gave 16 surface bodies, no
   solid (S5, met again). An assembly opened `readOnly:true` was `dirty: true` after only
   `asm_interference` (0 at the saved position) (S21, met again). Every `doc_open` warned
   `Double 2` changed; `doc_close` of the drawing and of the assembly listed `also closed:
   .` and `, .` (S17, met again).
7. **Calls**: 28 for the whole test, 13.2k characters of results, 3 errors (the intended
   `VERIFY_FAILED` in 2, the `FEATURE_FAILED` probe in 3, the centre pick in 5). The
   drawing took 13, of them 8 dimension adds at ~110–600 characters each and one
   `drw_info` (1.6k) to read the second section's orientation. No `sw_help`.

Sorted into S20, held, follow-up (d) new (1, 2, 3); S14 (4); S25 new (5); S19, S5, S21, S17 (6); S9 (7).

**2026-11-10: scenario test of build 091645 (build identity, three noise fixes, read-only close)**

Build `bin\0.1.0-20261110-091645`, `serverVersion` `0.1.0-sw2023-20261110-091645+76edd54`.

1. **S16 held.** `sw_status` gave `serverVersion` `0.1.0-sw2023-20261110-091645+76edd54`
   from the first call; the commit equals the one tag `build-20261110-091645` points at,
   no `.dirty`. The session no longer needs the registration or git to name its build.
2. **S17 held, all three.** (a) On a copy of a finished part, an ear feature's mirror was
   deleted and made again with `feature_mirror` about the front plane: volume +7183.229,
   9/9 faces reflected, the area ratio (1.052) only as a check line ("it meets the body
   differently from the seed"), no warning. (b) No `doc_open` warned `Double 2`: an
   assembly and a drawing opened with nothing active, a ~230 mm part and a STEP import
   opened while a 10 mm part was active. (c) `doc_close` of an assembly listed `also
   closed: <part>, <part>.` and of a drawing `also closed: <part>.`, names filled.
3. **S21 held.** An assembly opened `readOnly:true`, then `asm_interference`,
   `model_info` and `view_screenshot` (dirty after): `doc_close` with no flags closed it,
   "opened read-only, so its unsaved changes were discarded", `discardedChanges: true`.
   A drawing opened read-only, then `doc_rebuild`: the same. A part opened for editing and
   changed still got `DIRTY` from `doc_close` with no flags, as it should. File dates of
   the read-only documents unchanged on disk.
4. **`feature_delete` with `deleteChildren` warned once per absorbed sketch of a child.**
   Deleting a mirror feature with two hole features under it: `removed` listed the
   mirror, the two holes and their four sketches, each with a `why` ("absorbed by
   <hole>"), and `warnings` repeated each sketch, "SOLIDWORKS also removed 'SketchN'
   (absorbed by <hole>), which was not planned." Four warnings on a delete that did what
   was asked: a child's absorbed sketches go with the child in SOLIDWORKS whatever
   `deleteAbsorbed` says. The `why` already says it; the warning reads as if something
   went wrong. Same kind as S17.
5. **A drawing's referenced part is listed `foreign`.** After `doc_open` of a drawing
   (read-only), `doc_list` showed the part it references as a separate document, `access:
   foreign`, though the user never opened it; `doc_close` of the drawing then closed it
   too (`also closed`). Right in SOLIDWORKS terms, but `foreign` reads as "the user's
   document", which the `doc_close` description says is never closed. Small; a session
   reading `doc_list` should know a reference loaded by a drawing or assembly shows up so.
6. **Short checks.** In a new part `doc_undo` after a boss extrude removed it (volume →
   0). STEP AP214 export reported `solids: 1`; `doc_open` of it gave 16 surface bodies, no
   solid (S5, met again). Assembly interference 0 at the saved position.
7. **Calls**: 30 for the whole test, 16.8k characters of results, 2 errors (the intended
   `DIRTY` in 3, and one call sent without its arguments). The largest results were
   `feature_list` (2.1k) to find the mirror and `feature_mirror` (1.9k, nine new faces).
   No `sw_help`.

Sorted into S16, held, closed (1); S17, held, closed (2); S21, held, closed (3); S26 new (4);
S27 new (5); S19, S5 (6); S9 (7). Not filed (6): interference 0, as expected.

**2026-11-10: scenario test of build 115800 (`sketch_constrain`)**

Build `bin\0.1.0-20261110-115800`, `serverVersion` `0.1.0-sw2023-20261110-115800+4f4699c`.

1. **S8 held.** A thin-walled tray part was built again from the recorded call sequence of
   an earlier build, with each sketch's relations and dimensions sent as one
   `sketch_constrain` call. A six-line profile with a centreline (horizontal, vertical, two
   coincident with the origin, two symmetric, four dimensions): 10 steps in one call, every
   check as the single tools give it, `relationsAfter` 13, `constrained: fully`, 1.2k
   characters (before: 10 calls, 4.7k). A five-line cut profile with a slope rising 7.5°
   from the end of a flat line (two relations, a `vertical` between the origin and a point
   `{at}` on a segment end, five dimensions, the angle with `textAt` in the supplementary
   sector): 8 steps in one call, the angle accepted with the S6 check line, `fully`, 1.2k
   (before: 8 calls, 3.3k, without the deliberate refusal). Volumes at every step equal
   the earlier builds' to the last digit, and the design-intent changes (angle, wall,
   length, width) rebuilt clean with one body. The part's geometry and design-intent steps
   took 33 calls and 21.1k characters against 49 and 26.9k for the same sequence in build
   `…-195750` (baseline `…-163234`: ≈53, ≈35k).
2. **`material_set` from this server's library failed, and SOLIDWORKS corrupted the
   library while doing it.** `material_set {name:"PETG", database:"solidworks-mcp"}` gave
   `VERIFY_FAILED` ("the material of 'Default' reads '(none)'"), where earlier builds applied
   it. During that call (inside its 0.42 s) the `.sldmat` was rewritten: SOLIDWORKS' own
   header (`<?xml version="1.0"?>` and an opening `mstns:materials` element with its four
   namespaces) was put in front of the server's UTF-16 file, BOM and second declaration
   included. Afterwards `material_list` only warned "Could not parse … Unexpected XML
   declaration … Line 3" and found no PETG; `material_add {…, overwrite:true}` failed
   `INTERNAL: XmlException` instead of saying the library is unreadable and what to do.
   With the file moved aside, `material_add` wrote a new library (both checks passed) and
   the next `material_set` failed and corrupted it in the same way, so it can be repeated.
   `material_set` only calls `SetMaterialPropertyName2`, so the write is SOLIDWORKS'. The
   material code has not changed since the interop commit, so the cause is probably in how
   this SOLIDWORKS session loads a custom library (the SOLIDWORKS process was restarted
   since the last build that passed). Not proven. The `VERIFY_FAILED` was right; nothing
   reported `ok` wrongly. What is missing is a clear error in `material_add` and
   `material_set` when a library does not parse, and some guard against SOLIDWORKS
   rewriting a library the server wrote (or at least a way to recover from it). The
   library was left as the last `material_add` wrote it, and the part without a material.
   **After the owner restarted SOLIDWORKS** (same build, server process unchanged), the
   saved part reopened, `material_set` with the same arguments on that clean library
   passed (PETG, 0.221 kg, density matches) and the library was not touched (same size and
   date). So the rewrite came from the state of the earlier SOLIDWORKS session, not from
   the build. How that session got there is not known. What remains for the server: a
   library SOLIDWORKS corrupts makes `material_add` fail `INTERNAL`, and `material_set`
   gives a hint about the name, not about a library that no longer parses.
3. **A custom property linked to a dimension resolved in metres.** `prop_set
   {name:"Wall", value:"\"D1@Wall\""}` on a 3 mm shell read back `resolved:
   "0,0030000000000000"` (also in `prop_list`); earlier builds gave `"3,00"`. The part was
   made with `doc_new` (MMGS). Seen in the same SOLIDWORKS session as point 2. After the
   restart the same saved part read back `"3,00"` in `prop_list`: the session's state
   again, not the build.
4. **Short checks.** Two sections on a 1:2 plan of the part: the centreline one had one
   hatched face, the one through the ear holes had three, as before. STEP AP214 export
   `solids: 1`; `doc_open` of it gave 16 surface bodies, no solid (S5, met again).
   Interference 0 at the saved position in an assembly opened read-only, closed without
   flags (S21 still holds).
5. **Calls**: 56 for the whole test, 33.3k characters, 3 errors (the two `material_set`
   failures and the `material_add` on the corrupted library). Seven calls went on point 2,
   and four more for the check after the restart.
   No `sw_help`.

Sorted into S8, held, closed (1, 5); S28 new (2); S5 (4). Not filed (3): the SOLIDWORKS
session's state, gone after the restart; (4) S21 still holds.

**2026-11-10: scenario test of build 134233 (S28: `material_add` into `Custom Materials`)**

Build `bin\0.1.0-20261110-134233`, `serverVersion` `0.1.0-sw2023-20261110-134233+943e3e9`
(= tag `build-20261110-134233`). Same SOLIDWORKS process as the fork's live tests of the
build; one other server process was attached (`otherServerPids`).

1. **S28 held.** On a copy of a thin-walled tray part, with
   `Custom Materials.sldmat` holding one material SOLIDWORKS wrote (a plastic with seven
   properties) and the old `solidworks-mcp` library holding PETG:
   (a) `material_add {name:"PETG", 1270, 2 GPa, 0.38}` with no `database` went into
   `Custom Materials`, `backup` named, and the backup was the old file byte for byte; all
   three checks passed. A second write kept that backup unchanged.
   (b) `material_set {name:"PETG"}` was `ENTITY_AMBIGUOUS` (in `Custom Materials` and
   `solidworks-mcp`), hint "Pass 'database'"; with `database:"Custom Materials"` it passed,
   0.221420661 kg, density matches. The library was not touched by SOLIDWORKS (same hash
   and time after the call).
   (c) `material_add` of the existing material's name in lower case with `overwrite:true`:
   `NOT_OWNED`, left alone. `PETG` again without `overwrite`: `INVALID_ARGUMENT` with the
   hint to pass it; with it, `replaced: true`, other material unchanged. `PETG` with
   `overwrite:true` into the old `solidworks-mcp` library: replaced, no backup (its own).
   (d) A copy of the library SOLIDWORKS corrupted in the test of build `…-115800`, put in
   the custom material folder under another name: `material_list` warns with the parser's
   line and leaves it out of `databases`; `material_add` into it, `material_set` from it,
   and `material_set` of a name found nowhere all `IO_ERROR`, naming the file, "Line 3,
   position 4", that SOLIDWORKS can rewrite a library it has loaded, and to move it aside or
   restore it. The file was not written (same hash). The rewrite during `material_set` was
   not met in this SOLIDWORKS session either, so that branch is still untested.
2. **The library keeps tabs and CRLF, not its bytes.** After the first `material_add`, the
   declaration read `encoding="utf-16"` (was `UTF-16`) and every empty element of the
   material the server did not write gained a space before `/>`
   (`<EX … value="0.20E+10" />`). Values and order unchanged, SOLIDWORKS reads it, and
   the check "other materials read back unchanged" is true by value. Only cosmetic, but a
   diff of the user's library shows every line of their own materials changed.
   Writing the same material twice gives the same bytes.
3. **A name typo becomes a library error while any library does not parse.**
   `material_set` of an unknown name with an unreadable library present gives only the
   `IO_ERROR` about that library; the closest names from the libraries that parse (the
   usual miss) are not listed. Right that it can't say "not found" for sure; the closest
   names would still help.
4. **Lesson: PETG now sits in two libraries**, so every `material_set {name:"PETG"}`
   needs `database` until the old `solidworks-mcp` library is retired.
5. **Short checks.** A 10 mm block extruded in a new part, `doc_undo` removed it (volume →
   0). STEP AP214 export `solids: 1`; `doc_open` of it gave 16 surface bodies, no solid
   (S5, met again). Interference 0 at the saved position in an assembly opened read-only,
   closed without flags. Two sections on a 1:2 plan of the part: the centreline one had one
   hatched face, the one through the ear holes three.
6. **Calls**: 34 for the whole test, 16.2k characters, 6 errors (all deliberate: the
   ambiguous name, `NOT_OWNED`, the missing `overwrite`, three `IO_ERROR`). 14 calls on S28.
   No `sw_help`.

Sorted into S28, held, closed (1); its follow-ups fixed in `9ab2fbe` (2, 3); point 4
done on the test machine (the old library moved aside), recorded under S28; S5 (5); S9
(6).

**2026-11-10: scenario test of build 144220 (S28's follow-ups)**

Build `bin\0.1.0-20261110-144220`, `serverVersion` `0.1.0-sw2023-20261110-144220+9fd26ce`
(= tag `build-20261110-144220`). One other server process was attached (`otherServerPids`).

1. **S28's follow-ups held.** On a copy of a thin-walled tray part, with
   `Custom Materials.sldmat` holding a material SOLIDWORKS wrote and the server's PETG, and
   the old `solidworks-mcp` library moved aside:
   (a) `material_set {name:"PETG"}` with no `database` passed (only one library has it
   now), 0.221420661 kg; the library's hash and time unchanged after the call.
   (b) `material_add` of a new material (1240 kg/m³, 3.5 GPa, 0.36, yield 55 MPa) into
   `Custom Materials`: a diff of the library before and after (UTF-16 decoded) shows only
   the 11 added lines; the BOM, the declaration's `UTF-16`, the user's empty elements
   (`"/>"` with no space), CRLF and the last line end are byte for byte as before. The
   backup kept from the earlier test was not touched. All three checks passed.
   (c) With a library SOLIDWORKS had corrupted put in the custom material folder,
   `material_set` of a typo (`PETGG`) is `IO_ERROR` that now starts with the closest names
   from the libraries that parse (`PETG`, `PET`, …) and then names the bad library, its
   line and what to do. `material_set` of a name found in a library that parses passed,
   with the parse warning, and SOLIDWORKS did not rewrite the library (same bytes after).
   The bad library was not written (same hash). The rewrite during `material_set` was not
   met, so that branch is still untested.
2. **`drw_view_section`: `modelBoxMm` of a section is the whole model's box, not what the
   view shows.** A section across a tray where the cut shows a shorter profile than the
   model's full height (cut through the side ears, looking along +X): `boundsMm`
   horizontal 72.68–107.32, `modelBoxMm` 67.24–101.38, the full 68.28 mm model extent at
   1:2, so it reaches 5.4 mm outside the view's own outline on the cut-away side. The PDF
   draws the section from 78.6 to 101.35: the box's one edge is right, the other is
   11.4 sheet mm off. The section's checks all passed (one view, three hatched faces).
   A caller that places dimension text or another view from `modelBoxMm` (as the
   description of `drw_views_standard` suggests for plain views) lands beside the section.
   Bears on S14 (what a view's frame and extents are on the sheet). Either the cut
   geometry's box, or a word in the description that a section keeps the model's box.
3. **Short checks.** A 20 mm block extruded 10 in a new part, `doc_undo` removed it
   (volume → 0). STEP AP214 export `solids: 1`; `doc_open` of it gave 16 surface bodies, no
   solid (S5, met again). Interference 0 at the saved position in an assembly opened
   read-only, closed without flags. Two sections on a 1:2 plan of the part: the
   centreline one one hatched face, the one through the ear holes three.
4. **Calls**: 28 for the whole test, 13.3k characters, 1 error (deliberate: the typo
   beside a bad library). 5 calls on S28. No `sw_help`.

Sorted into S28, its follow-ups held (1); S14, met again, a section's `modelBoxMm` (2); S5
(3); S9 (4).

**2026-11-11: scenario test of build 140208 (S29, S14, S22, S25, S26, S20 d, S6)**

Build `bin\0.1.0-20261111-140208`, `serverVersion` `0.1.0-sw2023-20261111-140208+8ea77a4`
(= tag `build-20261111-140208`).

1. **S29 held; `model_mass` and `material_set` still read a stale material name.** A 20 ×
   20 block, `config_table` rows A, B, C (`D1` 10/20/30) with `$MATERIAL` `AISI 1020` in A
   only, `Default` active: rows A 0.0316 kg, B 0.008, C 0.012. Then `material_set
   {name:"6061 Alloy", configuration:"C"}` with `Default` active. Each configuration
   activated and measured: A 7900 kg/m³ 0.0316, B 1000 0.008, C 2700 0.0324, `Default` 1000
   0.004; the table read back `Default` null, A `AISI 1020`, B null, C `6061 Alloy`. Right.
   But `model_mass` in `Default` gave `"material":"AISI 1020"` beside density 1000, and a
   following `material_set {name:"ABS"}` (no `configuration`, `Default` active) reported
   `"previous":"AISI 1020"`, both reported `ok`. `model_mass` reads
   `GetMaterialPropertyName2("")`; the table reads by the configuration's name and gets none.
   Likely the row's material that reached `Default` (S29's cause) and was taken back by name
   still answers to `""`; not probed further, and `material_set`'s read of `previous` not
   looked up in the code. That `material_set` changed `Default`
   only (A, B, C kept theirs): the branch S29 had not looked into held on this part.
2. **S6 held; one wording left.** Two lines at a shared vertex, the slope at 7.5°,
   `sketch_dimension_add angular` 10° without `textAt`: refused `VERIFY_FAILED`, hint "To
   set the angle that is now 7.500423°, pass textAt inside it, e.g. textAt [62.581, 0.825]".
   With that `textAt` accepted, 10° measured. The accepted call's check reads "the dimension
   is on the angle that is now 7.500423°, not on the one between the rays, now 172.499577°",
   while after the call they are 10° and 170°: in a result, "was" fits.
3. **S26 held; `dryRun` disagrees.** On a copy of a thin-walled tray part, `feature_delete`
   of a mirror with `deleteChildren`: the mirror, its two Hole Wizard children and their four
   sketches (`absorbed by <hole>`), no warning. The same call with `dryRun:true` just before
   listed those four sketches under `keptAbsorbed`, then the real call removed them. A
   `dryRun` that says kept what will go.
4. **S14's section point held.** A section through a tray's side holes (looking along +X,
   1:2): `modelBoxMm` horizontal 78.624–101.376, the PDF's 78.6–101.35 (was 67.24–101.38),
   both in `drw_info` of a saved drawing and in a new `drw_view_section` result; the
   centreline section's box is the full height, as it should be. Met again (S14): two
   `diameter` dimensions on the same hole edge, no `textAtMm`, both placed at the same sheet
   point, the second one a duplicate; no warning for either.
5. **S22 held.** `sw_status` after a failed call: 1.26k characters for ten `recentOps`, the
   failure as `"error":"VERIFY_FAILED"` only. A sheet's note list with a 1405-character
   note: 232 characters, `"NOTES… (1405 characters)"`. `drw_info` with three views 1.44k
   (was 1.6k), the full path in the first view only. Left: `drw_views_standard` and
   `drw_view_section` results still carry the full path, which `drw_info` cut.
6. **S25 held.** `drw_dimension_add diameter` picked at a 4.5 mm hole's centre: "… is the
   centre of a circular edge (radius 2.25 mm), not a point on it; used the circle centred
   there". Lesson: a first pick 4.5 mm off the centre (on the circle's far side, mistaken for
   it) got the old warning, "used the nearest one, 2.25 mm away", rightly, but 2.25 is also
   the radius and read as the centre case; an `edge_list` call found the real centre. The
   old warning naming the circle's centre when the edge is a circle would have saved it.
7. **S20 (d) held.** In a section view, `vertical` on a hole edge seen edge-on and drawn
   horizontally: `FEATURE_FAILED`, hint "a circle seen edge-on in this view, drawn as a
   horizontal segment of its diameter, so it has no vertical extent. Use kind linear or
   horizontal …". (c) held again: `linear` on the same edge, `expected` 4.5 with the Ø check.
8. **Short checks.** A 20 mm block extruded 10, `doc_undo` removed it (volume → 0). STEP
   AP214 export `solids: 1`; `doc_open` of it gave 16 surface bodies, no solid (S5, met
   again). Interference 0 at the saved position in an assembly opened read-only. Two new
   sections on a 1:2 plan: the centreline one one hatched face, the one through the side
   holes three.
9. **Calls**: 60 for the whole test, 25.2k characters, 2 errors (both deliberate: S6's
   refusal and S20 d's). One `edge_list` spent on the wrong pick of point 6. No `sw_help`.

Sorted into S30 new, S9 (1, 9). Held: S29 (1); S6, one wording left (2); S26, follow-up (a)
(3); S14's section point, and met again (4); S22, follow-up (a) (5); S25, follow-up (a)
(6); S20 d (7). S5 and S19 met again (8).

**2026-11-11: a caller's manual written against the tool reference: tools missing for a design of several parts**

No build was run. Read from `docs/tools.md`, `ROADMAP.md` and the source on `sw2023`
(the tree of build `20261111-140208`), while writing the note S9 asks for: how a
session plans a part, a design of several part files, and a change to an existing
model. Every point is a missing tool or an untested path, none a failure seen.

1. **Missing: a document's equations linked to a text file.** `equation_set`,
   `equation_list` and `equation_delete` act on one document, and nothing in the source
   reaches `IEquationMgr.FilePath` or `LinkToFile`. A design of several part files that
   share key sizes (a wheel diameter three parts fit to) has no way to hold one number
   once: each part gets its own global variable of the same name, a change is one
   `equation_set` per part, and a change the user makes by hand in one part does not
   reach the others. The other ways to link files are short of it too: `asm_part_new`
   with a face is one extrude off that face (T9.22 partly, T9.23 not built), and
   `part_derive` `insert` carries solids, surfaces and material, not sketches or
   planes. Proposed: link, unlink and read a document's external equation file, the
   file's variables listed by `equation_list` with their source. General: fit for a PR.
2. **`model_review` against a rules file (T20.8, partly built).** The rules a caller now
   checks by reading `feature_list`: a feature left with its default name; a fillet or
   chamfer that is not after the last boss, cut or hole; a sketch placed on a face or
   edge of a later feature; a dimension holding a literal where a global variable of
   that meaning exists; no global variable at all. As a server check they cost almost
   no tokens and give the same answer each time. The first of points 2 to 4 to build.
3. **Explain a model in one call (T20.7, not built).** Reading a part to change it
   takes `model_info`, `feature_list`, `dimension_list` and `equation_list`, three to
   five listings of raw rows. One call returning each feature with the dimensions that
   drive it, the sketch relations, and the fragile references (a sketch on a face a
   fillet made, an under-defined sketch) is what an edit and a text index of a model
   both need.
4. **Session export as a replayable recipe (T3.15, not built).** A part's successful
   calls as text would give a SOLIDWORKS part a diffable source. The heaviest of the
   three; an idea for now.
5. **`model_compare` against an earlier revision of the same file, from the source,
   not run.** `other` as a file is opened read-only with `OpenDoc6`. A revision copied
   out of version control keeps its file name, and SOLIDWORKS does not open a second
   document of that name: the call ends in `IoError`, whose hint says to compare by
   handle, which cannot help when the two are one name. The caller has to write the
   copy under another name. For an assembly the copy opens, but its components resolve
   to the documents of those names already in memory, so two revisions of an assembly
   compare the same parts and only the top level differs; nothing in the result says
   so. Proposed: the description names both, or the tool copies `other` to a temporary
   name itself and warns when an assembly's components came from memory.
6. **Tools a manual leans on that no entry here has met.** `equation_set`,
   `feature_rename`, `model_review`, `model_compare`, `asm_component_list` and
   `asm_mate_list` on an assembly the user built by hand, `asm_part_new`, `asm_replace`,
   and `doc_activate` with `allowEdit`. The scenario test covers a part, a STEP round
   trip, a two-part assembly and a drawing. Proposed: a scenario item that opens an
   existing assembly, reads its components and mates, and changes one dimension; and one
   that sets a global variable, renames a feature and reads `model_review`.

Sorted into S33 new (1); S34 new (2, 3, 4); S35 new (5); S36 new (6). The manual itself:
S9.

**2026-11-11: a machined part from scratch by a caller's manual: global variables, equations, renames, `model_review`**

Build `bin\0.1.0-20261111-140208` (`8ea77a4`). A plate with a cylindrical boss, a bore
through it, a blind counterbore from the boss top on an offset plane, four clearance holes
(one Hole Wizard hole and a 2 × 2 linear pattern) and four corner fillets; every key size
a global variable. 83 calls, 32.4k characters of results, no error, no `sw_help`. Bears on
the previous entry's point 6 (tools no entry had met): `equation_set`, `feature_rename` and
`model_review` met here, all held.

1. **`equation_set` held on 2023.** Ten global variables created (`PlateLength = 120mm`
   …); thirteen dimensions linked to them, among them a dimension whose feature had been
   renamed to a name with spaces (`D1@Base plate sketch = "PlateLength"`), an offset
   plane's offset (`"PlateThickness" + "BossHeight"`), a linear pattern's two spacings,
   and a Hole Wizard placement sketch's two dimensions (`"HolePitchX" / 2`). Each result
   read the value back and `driven: true`. All ten globals then changed to a second set
   and the volume compared with a hand figure: 173487.816 mm³ against 173487.8, one body,
   `model_review` clean; set back, 108553.103 mm³ as before. Reported `ok` and right.
2. **No batch form: 43 of 83 calls were `equation_set`.** One `lhs` per call: 10 to
   create the globals, 13 to link dimensions (two of them the placement sketch of point
   4), 20 to change the ten globals and set them
   back. Each call rebuilds. Proposed: `equations:[{lhs, expression}]` with one rebuild,
   all or nothing and verified per row, as `dimension_set {values}` and
   `sketch_constrain {steps}` already are. Wasteful pattern; no item covers it.
3. **`equation_set`'s result carries no volume or body count**, unlike `dimension_set`'s.
   `dimension_set` refuses an equation-driven dimension, so in a model whose key sizes
   are global variables every size change goes through `equation_set`, and S7's usage
   line (read `bodies` after each `dimension_set`) cannot be followed from the result: a
   `model_mass` (or `model_review`) after the change was needed. A global change can split
   a body as a dimension change can. Proposed: the same `volumeDeltaMm3` and `bodies` in
   the result. Bears on S7.
4. **`feature_hole` leaves its placement sketch under-defined and does not say so.** The
   result names the hole's own sketch dimensions (`Hole Dia.@Sketch5`, `Hole Depth@…`) but
   not the placement sketch (`Sketch6`) or its status. The only sign was `model_review`:
   `{"severity":"info","kind":"sketch_under_defined","where":"Sketch6"}`; a pattern of
   that hole then rests on a point that can move. Workaround, 5 calls: `sketch_edit`,
   one `sketch_constrain` (horizontal and vertical from the origin, `fully`),
   `sketch_finish`, two `equation_set`. Proposed: `feature_hole` returns the placement
   sketch's name and constrained status, and an option to dimension each point from the
   origin; perhaps `model_review` ranks an under-defined sketch as a warning, not info.
5. **`feature_rename` held**, on a feature and on a sketch: `previous` and the renamed
   dimensions came back (`D1@Base plate`, `D1@Base plate sketch`). Every feature tool's
   `name` does the same in the creating call, so `feature_rename` is needed only for a
   sketch or after the fact.
6. **`model_review` held**: 459 characters with a missing material (warning) and the
   under-defined sketch of point 4 (info); 177 characters with nothing found, `20
   feature(s), 6 sketch(es)`, `geometry checked`.
7. **Small**: `sketch_add` of a centre rectangle returns six segments, `sides` names four;
   the two construction diagonals (`Line5`, `Line6`, needed for a midpoint relation to
   the origin) are not named as such, and were guessed. `material_list {filter:"6061"}`
   returned 4 rows in 1268 characters, about two thirds of it the list of databases,
   repeated on every call.

Sorted into S37 new (2, 3); S38 new (4); S39 new (7); S36, held (1, 5, 6). S7 (3); S9,
the call count.

**2026-11-11: two printed parts of ~25 and ~10 features built from sketches, and their assembly**

Build `0.1.0-sw2023-20261110-064352`. About 330 calls over two parts and an assembly.

1. `feature_extrude` cut, blind, whose far end leaves the body (a hole through a plate, a
   pocket that opens through a slanted underside): `VERIFY_FAILED: ... no floor face at 12 mm
   along [-1, 0, 0]`, rolled back, though SOLIDWORKS made the right cut. `through_all` was no
   option, the cut would have gone on through other walls. Workaround: `keepOnFailure:true`,
   then the volume read from the next feature's result; the `name` given is not applied on
   that path (the feature stays `Cut-Extrude2`), one `feature_rename` more. Met twice. No
   open item covers it. Asked: a blind cut that breaks out passes on its volume, or an
   `upToFace` / `through next` end.
2. `feature_pattern_linear` with `direction2`: the second direction ran toward +z whether
   `direction2` was `-z` or `+z`, reported `ok` with two warnings (`4/6 instance faces`,
   `2 of 6 instance faces were not found`) and a volume change of one instance for a
   2 x 2 pattern. The instances in the wrong place cut air, so nothing failed. Direction 1
   is flipped when SOLIDWORKS runs it backwards; direction 2 is not. A mirror made from
   that pattern then cut two holes where none belonged. Workaround: a pattern in one
   direction, then `feature_mirror`. Reported success: yes, wrongly. No open item.
3. `feature_pattern_linear` with `directionRef`: `direction:"-y"` was refused (`not parallel
   to edge`), its hint says to give the sign as `"+"/"-"`, and `"+"` is then refused (`'+' is
   not an axis`). Leaving `direction` out worked. The hint names a value the tool does not
   take.
4. `sketch_dimension_add` angular between two lines that meet at a vertex, `textAt` inside
   the wedge between the rays, 50 asked where the rays stood 130 apart: the solver flipped
   one line to the mirrored solution, the result said `value reads back 50°, geometry
   measures 50°`, and only the `changed` list showed a line now running the other way. The
   sketch then went `fully` defined in the wrong shape. Bears on S6. Workaround: delete the
   line, redraw it, dimension the angle the rays really make (130) first, the distances
   after. Asked: a warning when a dimension flips a segment's direction or moves an end
   point further than the value's own change explains.
5. `sketch_relation_add` `merge` of a free line's end onto a point held by relations moved
   the free line whole, so a second merge pulled the shape back into the flipped solution
   of point 4. Right in SOLIDWORKS terms; for the next session: redraw at the exact
   coordinates of the held points, do not merge onto them.
6. `sketch_offset`: the offset has no dimension a caller can name (`D5@<sketch>` not found
   after four dimensions), so it cannot be driven by an equation; and a sketch that reads
   `constrained: fully` while open is listed `sketch_under_defined` by `model_review` once
   it holds an offset. Met in both parts. No open item.
7. `feature_chamfer` `equal`, on a concave edge and on a circular edge: `'Chamfer1' was
   created but has no faces`, then the tool fell back to distance-distance by itself and
   held. Twice in two tries: the equal type may never work on this version.
8. `sketch_add` draws without relations (a closed polyline of axis-parallel lines has
   none), so each sketch took 2 relation calls and 3 to 6 dimension calls; of the ~330
   calls about 150 were `sketch_relation_add` and `sketch_dimension_add`. S8 met again:
   relations and dimensions in the `sketch_add` call, or horizontal / vertical inferred
   for lines drawn so, would have halved the job.
9. Relations on several entities in one call (`vertical` on three lines, `equal` on three
   circles, points aligned with `horizontal` / `vertical`) all worked and are worth using;
   a polygon's side is picked by a point off its midpoint, since the midpoint lies on the
   construction circle too (`ENTITY_AMBIGUOUS`, rightly).
10. `doc_open` of a part: `SOLIDWORKS system options changed during this call and are not
    back to the user's values: Double 2 was 0.000131..., now 0.0000920...`. Bears on S17 if
    that is among the noise it lists; otherwise new.
11. Results of `feature_extrude`, `feature_mirror` and `feature_fillet` list up to twelve
    new faces each, 2 to 4k characters, read for one id or none. S22 met again: a
    `faces:false`, or ids only.
12. SOLIDWORKS was restarted between two turns; the document the user had saved reopened
    with `access: owned` and every feature there. A sketch open at the time of the save
    came back closed with its segments kept.

Sorted into S40 new (1); S41 new (2, 3); S42 new (4, 5); S43 new (6); S44 new (7); S45 new
(11). S8, Closed, met on a build before its fix (8), cause in S46. S17, Closed, met on a
build before its fix (10). Not filed (9, 12): held as documented.

**2026-11-11: a release left unregistered in a second project that uses the server**

Build `0.1.0-sw2023-20261110-064352` (the one a second project still runs); latest
release `0.1.0-sw2023-20261111-140208`. Found while reading a session's call count, not
by a call.

1. The release steps in `CLAUDE.md`, step 4, register the new exe "(scope local, in the
   owner's project)", one project. The server is registered at local scope in two of the
   owner's projects, each holding its own exe path in `~/.claude.json`. Builds `…-064352`
   and before were registered in both; the five after it (`…-091645` to `…-140208`) only
   in the project where the scenario test runs. A session in the other project then ran a
   job of two parts and an assembly on `…-064352`, without `sketch_constrain` (build
   `…-115800`), and spent ~150 of ~330 calls on `sketch_relation_add` and
   `sketch_dimension_add` one each. `sw_status` reported the old build rightly; nothing
   compared it with the latest release. Bears on S15 (registration by hand at local
   scope) and S16 (the build seen from inside a session). Proposed: step 4 lists every
   project that registers the server and re-registers each, or reads them from
   `~/.claude.json` (`projects.*.mcpServers.solidworks-mcp.command`) and names any left on
   an older build in the question to the owner.

Sorted into S46 new (1).

**2026-11-12: scenario test of the modelling items (a shelled tray, a machined part of global variables, probes in throwaway parts)**

Build `0.1.0-sw2023-20261112-091908` (`serverVersion` `…-091908+276d378`). 128 calls, 69.9k
characters, 2 errors, both refusals the test asked for.

1. Held: S37, S42, S41, S45, S40, S38, S43, S18, S31, S44, S39, S27, S35, S30 and S6's
   wording. In order: ten global variables in one `equation_set`, thirteen dimension links
   in one more, each row read back, `bodies: 1` in each result, the ten changed and set back
   in one call each with the volume back exact; a machined part of ten globals took 39
   calls and 26.3k characters against 83 and 32.4k on build `20261111-140208`. No "turned" /
   "moved" warning in nine fully defined sketches; a 130° wedge given 50° with `textAt`
   inside it warns with both angles, the line that turned and a `textAt` in the other
   angle. A 2 x 2 pattern toward `-x` and `-z` lands all four holes, "direction flipped",
   no instance-face warning. `newFaces` id and surface only (a boss's result 0.75k), in
   full with `faceDetail:true`. A blind cut 15 deep in a 10 mm block passes on its volume
   with the no-floor line and takes its `name`. `feature_hole` names each placement sketch
   `under` with a warning; after `sketch_constrain` and two equations `model_review` is
   clean. `sketch_offset` names `D3@<sketch>` = 5, stays `fully`, an equation of 8 gives
   the 116 x 76 loop, the sketch not in `model_review`. `doc_pack` of an assembly open
   read-only refused before writing; a part with no drawing beside it `noDrawingsFound`.
   `feature_delete` `deleteChildren`: dry run and delete list the same seven items.
   `feature_chamfer` `equal` on a bore's circular edge: no warning, volume as computed. A
   centre rectangle's `diagonals`; `material_list {filter:"6061"}` 0.65k, one database.
   `doc_open` of a drawing read-only: its model in `doc_list` with `loadedBy`.
   `model_compare` against a file whose name is open: refused, both paths named. Rows A to C,
   a material in A and C: `model_mass` in `Default` names none, with the warning; then
   `material_set` in `Default` reads 7900 kg/m³ and `model_mass` names it. The accepted
   angle's check reads "the angle that was 7.5°, … then 172.5°" (S6).
2. `feature_extrude` cut with `name`, no floor (S40): the check line names the depth
   dimension by the feature's name before the rename, "no floor at 15 mm: … (D1@Cut-Extrude1
   reads 15 mm)", while `dimensions` in the same result gives `D1@<name>`. A caller who
   takes the name from the check gets one that no longer exists. Reported success, rightly;
   wording.
3. `material_set` with the configuration whose material SOLIDWORKS names but does not
   apply (S30): the call applies it (`densityKgM3` 7900, "matches the database") and still
   carries the warning "… its mass has 1000 kg/m³ … the material is not applied, so it is
   reported as none", written as of now. It is the reason `previous` is absent; in a result
   whose material is applied it reads as if the call failed. "was not applied" / "the
   previous one is reported as none" would fit.
4. `feature_hole` with several points (S38): one warning per hole, the same 220-character
   sentence with the feature and sketch names changed. Six holes: 1.3k of the result's 3.6k,
   four holes 0.9k. One warning naming the sketches, then the way out once, would say the
   same. Wasteful, not wrong.
5. `equation_set` with `equations` (S37): the largest results of the session, 3.9k for
   thirteen dimension links and 3.1k for ten globals changed, each row giving the equation,
   the previous equation (or the driven dimension) and two checks. Few calls now, but these
   two kinds of call are a quarter of the part's characters. A short row form (lhs, value,
   previous value), the full one by a flag as `faceDetail` does for faces, would cut it.
   Not wrong.

Sorted into S48 new (2 to 5). Held: S37, S42, S41, S45, S40, S38, S43, S18, S31, S44, S39,
S27, S35, S30 and S6's wording (1).

**2026-11-12: scenario test of the drawing and assembly items (a hand-mated two-part assembly, a drawing of a shelled tray at 1:2, a STEP round trip, a two-part assembly's interference)**

Build `bin\0.1.0-20261112-091908` (`serverVersion` `0.1.0-sw2023-20261112-091908+276d378`).
42 calls, 21.4k characters, no errors.

1. `drw_info` and `drw_sheet_setup` (S14): on an ISO A3 sheet at scale 1:2, `drawableMm`
   reads [40, 20, 820, 574] and `titleBlockMm` [460, 20, 820, 130.5], twice the right
   [20, 10, 410, 287] and [230, 10, 410, 65.25] (the sheet is 420 × 297). A new drawing
   at 1:1 reads them right; `drw_sheet_setup {scale:"1:2"}` on it returns the doubled pair
   at once, so the factor is the sheet scale. Every check built on them is then wrong and
   reported success: a note at x 18 warns of crossing the border, but against x 40 (a note
   at x 30 would be warned wrongly); a note at [300, 40], on the real title block, gets
   "clear of the border, the title block and the other annotations"; a dimension with
   `textAtMm` [150, 292], over the zone strip and outside the border at 287, gets the same;
   a section view whose `boundsMm` reaches y 289.01 gets "inside the border". The live
   tests that hold these values are likely on a 1:1 sheet; a test on a 1:2 sheet would
   fail now.
2. Held: `drw_info` `axes` on all three views (a top view: right +X, up −Z, out +Y);
   `drw_note_add` `extentMm` and `drw_dimension_add` `textBoxMm`; a second `diameter` on
   the same hole edge without `textAtMm` warned "overlaps dimension '…' (the same value: a
   duplicate?)" with the first one's name (S14). `drw_annotation_edit` delete of the first
   of three stacked dimensions: `othersMoved` names the three SOLIDWORKS closed up, before
   and after, with the warning; other deletes "stayed where they were" (S24). The views of
   `drw_views_standard` and of two new sections name the model's file only, the path once
   at the top; a pick one diameter off a hole's centre warns "used the nearest one, 2.25 mm
   away: a circular edge of radius 2.25 mm centred at […]" (S32). `drw_dimensions_auto`'s
   description says it needs cleaning and points to `drw_dimension_add` (S13). Sections
   through a centreline (1 hatched face) and through two holes (3) as before. An assembly
   mated by hand (S36): `asm_component_list` 2 components, the first fixed, the second
   `under` with 2 mates; `asm_mate_list` concentric and coincident, `inError: 0`;
   `model_mass` volume 127 402.659 mm³ as computed; a part's length 60 → 80 through the
   assembly, rebuild, mates still without error, 133 685.844 mm³ as computed.
3. `dimension_set` on an assembly with a component's dimension
   (`D1@Boss-Extrude1@<component>-1`): the change is right, but the result names it
   `D1@Boss-Extrude1`, without the component, and carries no volume change, which the
   description promises and a part's result gives. With two components holding a
   `D1@Boss-Extrude1` the row would not say which changed. Reported success, rightly.
4. `dimension_set` through an assembly the server opened changed a part that `doc_list`
   lists as `foreign` (`loadedBy` the assembly), leaving it dirty; closing the assembly
   with `discard` closed and discarded it too ("SOLIDWORKS also closed"), and the part's
   file was not touched. Whether a referenced part counts as the assembly's for rule 5 is
   not said anywhere; the result does not mention the other document either. Not wrong as
   such; worth a word in the result ("also changes <part>, not saved").
5. STEP AP214 export of a shelled solid and `doc_open` of it: 16 surface bodies, as before
   (S5). Interference of a two-part assembly at its saved position: 0, as before.

Sorted into S47 new (1); S49 new (3, 4). Held: S14 but for its frame, S24, S32, S13 and
S36's assembly item (2). S5 met again (5).

**2026-11-12: reading a two-part assembly made by an earlier session, and moving it to a new folder**

Build `0.1.0-sw2023-20261112-091908+276d378`, SOLIDWORKS 2023 (revision 31.3.0).

1. `asm_mate_list` gave three coincident mates, each entity `{"component": ..., "kind":
   "plane"}` with no name, face id or point. Two of the three had been made on planar
   faces picked by `at`, one on two Right Planes: all six read the same, so which plane
   or face a mate takes cannot be told. `feature_info` on a mate gives its type and
   parents only. Workaround: none in the server; the earlier session's own `asm_mate_add`
   inputs were read instead. Reported success. Bears on S36 (`asm_mate_list` on an
   assembly built by hand). Proposed: per entity, a reference plane's name, or for a face
   its id and a point on it, as `face_list` gives.
2. `sw_connect {launch:true}` warned "SOLIDWORKS revision 31.3.0 is untested (tested: 33.x
   = 2025). Please report problems." This build is made and tested on 2023 (31.x); the
   warning has the tested revision the wrong way round. Same root as M0's revision check
   expecting `33.` (S2, S17).
3. S21 held: an assembly opened `readOnly:true` was dirty right after opening (its parts
   resolved from a new folder), and `doc_close` closed it without `discard`,
   `discardedChanges: true`, "SOLIDWORKS also closed" naming both parts.
4. Learned: an assembly and its parts moved together into a new folder while SOLIDWORKS
   was closed open with no load warning, the parts found beside the assembly; one
   `doc_open` for edit and `doc_save` in place stores the new paths.
5. Later in the same session, ~35 min after the last call: `doc_list`, `sw_status` and
   `sw_connect` all `NOT_CONNECTED: SOLIDWORKS is running (pid 19144) but can't be
   reached over COM`, the SOLIDWORKS this server had launched itself, not as
   administrator. `sw_status` listed `otherServerPids: [17116]`: a second server
   process, from another session started meanwhile on the same machine. Whether the
   second server took the COM attachment, or SOLIDWORKS stopped answering (its process
   at ~29 MB), the hint does not tell; it names privileges only. Proposed: when another
   server process is listed, the hint says it may hold SOLIDWORKS.

Sorted into S62 new (1); S60 new (2, 5). Held: S21 (3). Not filed (4): a lesson.

**2026-11-12: six printed and stand-in parts from sketches, a three-part subassembly redone in its own frame, and a ten-component assembly with a driven angle mate**

Build `0.1.0-sw2023-20261112-091908+276d378`, SOLIDWORKS 2023 (revision 31.3.0). Written
by the next session from the first one's transcript: the first ran out of its limit
before writing.

1. `asm_mate_add` `distance` between a component's plane and the assembly's parallel
   plane put the component on the opposite side, twice: a profile inserted at X +75 with
   `distanceMm: 75` landed at X -75, a side plate inserted at Z +85.2 landed at Z -85.2.
   Each reported success, `"planes 75 mm apart"`. Workaround: `feature_delete` of the
   mate, `asm_component_move` back, the mate again with `flip: true`; then every
   distance mate's side read from the result's `originMm`. Reported success. No item
   covers it. Proposed: a check that the component stayed on the side it was inserted
   (or a signed distance), or at least the side in the `verified` line.
2. `ref_plane` has no plane on a face: `kind: offset` with `offsetMm: 0` is refused
   ("needs offsetMm (non-zero ...)"), `kind: angle` with `angleDeg: 0` is refused
   ("between 0 and 180 (exclusive)"), and a sketch line as the axis of an angled plane is
   refused ("'axis' resolved to sketch segment Line7, but this parameter needs edge or
   axis"). So a named plane on a planar face, for mates to take in place of the face, could
   not be made; the mates stayed on faces. Missing tool or option. No item covers it.
3. `ref_plane` (and so `ref_axis`) in an assembly: `WRONG_DOC_TYPE: This tool needs a
   part, but '...' is a assembly`. An assembly-level axis for a pivot could not be made;
   the pivot was mated to a component's hole instead. Missing. Bears on S36.
4. `sketch_constrain` with 24 steps (horizontals, verticals, linear dimensions on a
   16-segment closed profile) passed the 50 s limit: `TIMEOUT: ... SOLIDWORKS is still
   executing it`. It completed at ~66 s, all steps applied. Workaround: waited, did not
   retry. Bears on S8 (one call for many steps): a batch this size needs the limit
   raised, or the result's time.
5. `sketch_constrain` `equal` on two lines of a rectangle: `VERIFY_FAILED: ... SOLIDWORKS
   ignored the relation 'equal' ... no relation was added`, steps after it not run. The
   relation was in fact added: the next call's second side dimension was refused as
   over-defining ("SOLIDWORKS made it a driven reference dimension"), and the sketch
   finished fully defined with one 56 dimension. Reported failure while it succeeded. No
   item covers it.
6. Learned: `sketch_add` merges segment ends given at the same point exactly, but a
   line end typed to 4 decimals at an arc's computed end (3 µm off) stays open; a
   `coincident` between `at` picks on each merged them ("points merged (9 → 8 sketch
   points)"). A `coincident` of two `at` picks at the same already-merged point is refused
   ("needs 2 different entities"), which is right but reads like a fault.
7. Held: `equation_set` with an `int()` expression driving a circular pattern's count
   (`int((Hi - Lo) / 10) + 1`) gave count 3 and read back. `equation_set` on an
   assembly's angle mate dimension (`D1@Angle1`) drove the mate; three values of the
   global variable each solved, with `asm_interference` 0 at each. `material_set` found
   PETG in `Custom Materials`. `equation_delete` removed an unused global.

Sorted into S50 new (1); S59 new (2, 3); S55 new (4); S56 new (5, 6). Held (7).

**2026-11-12: STEP parts brought toward an existing assembly, then stopped**

Build `0.1.0-sw2023-20261112-091908`, SOLIDWORKS revision 31.3.0 (`sw_connect`
warned "untested (tested: 33.x = 2025)").

1. `asm_insert` refuses a STEP: "'path' must be a .sldprt or .sldasm file, not
   '.step'". SOLIDWORKS inserts one by 3D Interconnect, a component linked to the STEP
   that follows it when the file changes, and an assembly made by hand held five such.
   Missing option; no item covers it. Not needed by the job in the end.
2. `doc_open` of a single-solid STEP written by build123d imported it as an
   **assembly** with one virtual component (`SOLID-1`, a path to a `SOLID.sldprt` never
   written), and reported `"solidBodies":0,"surfaceBodies":0`. `model_mass` on that
   assembly read 234 829 mm3, a solid, so S5 (surfaces only) did not show here, but the
   body counts in the import result read zero for an assembly import: reported as
   success with a count that is wrong. Bears on S5's "check `solidBodies` after
   `doc_open`" usage line: for an assembly import that check reads 0 on a good solid.
3. The same import left an empty part `Part2` open (`model_info`: 4 features, no
   bodies, `access: owned`), not named in `doc_open`'s result. `doc_close` of another,
   read-only assembly then closed it with that assembly's references ("SOLIDWORKS also
   closed: Part2, ..."), and a later `doc_close {doc:"D1"}` gave `DOC_CLOSED`. Stray
   document from the import; S27-like, no item covers the import case.

Sorted into S61 new (1 to 3), and a line in S5 (2).

**2026-11-12: a helical screw part rebuilt from a script, mated into an assembly**

Build `0.1.0-sw2023-20261112-091908`, SOLIDWORKS revision 31.3.0.

1. `feature_helix` on a circle sketched on the top plane (sketch x = model +X),
   `startAngleDeg` 0: the helix started at `[0,0,15]`, model +Z, not on the sketch's
   +x. The result's `startMm` showed it, so this was caught; the description does not
   say where the angle is measured from. `dimension_set` of the start angle to 90 then
   moved it (a sweep with its profile on the plane through +X ran), but nothing reads
   the start point back after such a change: `feature_info` on the helix gives only its
   dimensions, and `model_measure` refuses a point by `at` ("'at' works for face, edge,
   vertex and sketch_point"). The hand was confirmed only after the sweep, by two
   `face_list {near}` probes on the swept face. Proposed: the description says the
   angle's zero, and `feature_info` on a helix returns `startMm` as the create does.
2. `feature_helix` and `feature_info` list the revolutions dimension as
   `"D5@...","value":5527.777778,"unit":"mm"` for 5.527778 turns: a count reported in
   mm and scaled by 1000. Reported as success. S-items on units do not name it.
3. `feature_sweep` (merged into an existing body, the profile partly buried in it)
   warned "The sweep's start face is 109.52 mm², but the profile sketch encloses
   155.295 mm² (SOLIDWORKS may have used only some of its contours)". The sweep was
   right: the 45.8 mm² missing was the part of the start face inside the existing body.
   A false alarm on a merged sweep; worth comparing only for an unmerged one.
4. `model.box` in every feature, `equation_set` and component result is a loose bound
   over B-spline faces: ±30.63 for a body trimmed to Ø60, and Y -16.3 to 216.3 where
   the real lowest and highest faces were -4 and 215 (`face_list {extreme}`). After a
   change of global variables the box looked like a failed trim; the volume, matching
   the reference to 0.003%, was the check that held. Proposed: the box tight
   (`GetPartBox` with the tight option, or bodies' `GetExtremePoint`), or flagged
   approximate.
5. `sketch_constrain` coincident of a polygon's centre with the origin, on a face
   sketch at 179 mm off the origin: "SOLIDWORKS ignored the relation ... no relation
   was added". `sketch_add`'s polygon had already brought a coincident relation with it
   (`sketch_info`: coincident 1). The error's hint names the case; one call more.
6. What held: `sketch_constrain` (ten calls, each sketch fully defined in one),
   `equation_set` with 15 dimension equations in one call, its `volumeMm3` and `bodies`
   reading a three-variable change and its undo (S7's usage line met with no extra
   `model_mass`), `feature_chamfer` with a query `{curve:'line', extreme:'+y'}`
   `expect` 6, `ref_axis` two planes, `asm_mate_add` axis-to-axis and plane-to-plane,
   `asm_interference` with an overlap of 17.904 mm³ that matched a hand figure exactly.

Sorted into S63 new (1, 2); S64 new (3); S52 new (4); S56 (5). Held (6).

**2026-11-12: a second helical part rebuilt from a script, mated on the first**

Build `0.1.0-sw2023-20261112-091908`, SOLIDWORKS revision 31.3.0.

1. **`equation_set` refused every equation in one SOLIDWORKS session**, reported as a
   syntax fault: `FEATURE_FAILED: SOLIDWORKS rejected the equation "ScrewD" = 60mm. Hint:
   Check the syntax ...`, from `EquationMgr.Add2 < 0`. It failed alike for a global
   variable, for a bare `60`, for a dimension equation (`D1@<extrude> = 199.5mm`), on a
   part with a body, after `doc_save` to a file, after `doc_close` and `doc_open`, and on
   a new empty part made by `doc_new` for the test. Nine calls in all. The same build had
   set 4 calls of equations earlier the same day in the same session (PID unchanged);
   before the failure the session had opened one part read-only and closed it, and run
   some hours. No dialog (`sw_status`: `uiBlocked:false`). After SOLIDWORKS was exited
   and started again (new PID), the same 21 equations went in one call, and a five-variable
   change and its undo ran. Workaround: the geometry built with plain values, the
   equations set after the restart; cost the restart and ~12 calls. The hint sends the
   caller to the syntax, where a plain `"X" = 60mm` is not the cause. Proposed: when
   `Add2` refuses an equation of the form `"Name" = <number><unit>`, the error says the
   syntax is valid and SOLIDWORKS refused it, and suggests a restart; or a probe on a
   scratch equation tells the two apart. No item covers it.
2. **`sw_connect` after a restart the owner thought was done** returned
   `processId 22664`, the PID before, with no word that it was the same process; only
   comparing it with `sw_status` showed SOLIDWORKS had not been exited. Proposed:
   `sw_connect` says whether it attached to the process it last saw or a new one. Bears on
   S16 (telling what a session runs).
3. `sketch_add` of one hexagon (`polygon`, 6 sides) on a planar face reported
   `"regions":2` for a single closed contour (`segments 6`); on a reference plane the
   same call reported `regions 1`. The cut that followed took the hexagon only (volume
   change exactly the hexagon's area times the depth). Count looks like it includes the
   face's own boundary; reported as success, harmless here, misleading when a caller
   checks regions before a feature.
4. What held: `feature_helix` `height_pitch`, start 90°, `startMm [15,0,0]` on +X and the
   end point read back at the expected 120° past it, right-hand; `feature_sweep` of a
   ten-turn flight merged into one body with the profile 1 mm off the helix's start;
   `face_list {near, withinMm}` probes confirming the swept face's phase at a cut plane;
   `equation_set` with 21 rows in one call; `asm_mate_add` axis-to-axis and two
   plane-to-plane between two floating parts; `asm_interference` reading only a known
   overlap.

Sorted into S58 new (1); S60 (2); S64 (3). Held (4).

**2026-11-12: a part of extrudes, chamfers and reference planes, one sketch of two splines with driven points**

Build `0.1.0-sw2023-20261112-091908`.

1. `sketch_constrain` with 31 steps (15 relations, symmetric pairs among them, and 16
   dimensions from the origin to spline through-points) ran 83 s and returned `TIMEOUT`
   at 50 s; `sw_status` then showed it `completed (after the caller gave up)`,
   `succeeded: true`. The earlier calls of 9 to 14 steps took ~15 s, so ~1.5 to 2.7 s a
   step, rising with the sketch's size. No item covers a batch tool's time limit. A caller
   cannot tell beforehand that a batch will pass the limit: proposed, either a limit that
   scales with the step count, or the description saying the rough cost per step.
2. `sketch_constrain` `coincident` of `{name:"origin"}` with `{at:[0,0]}`, where an arc's
   centre and a centerline's start sit at the same point: it took one of them ("point on
   the entity"), and a step naming `{at:[0,-44.2], type:"sketch_point"}` for the arc failed
   `ENTITY_NOT_FOUND`, since an arc's mid is not a sketch point. The sketch came out fully
   defined (the two points had merged when drawn), so no harm; a caller cannot name an
   arc's centre otherwise.
3. What held: `equation_set` with `sin ( 60deg )` in an expression; `feature_chamfer`
   equal distance on three concave edges (arc and two lines) adding material;
   `ref_plane` offset with a negative offset; `sketch_add` of a polyline and two splines
   meeting end to end as one closed contour.
4. `feature_extrude` blind cut (`cut:true`, `depthMm:13`, `direction:"normal"`) from a
   plane inside the body whose end lies outside the body (the cut runs out of a sloped
   face before its depth): `VERIFY_FAILED` "no floor face at 13 mm along [0, 1, 0]
   (planar faces at 0, 0 mm)", rolled back. The cut was right; with `keepOnFailure:true`
   it stayed, its volume as figured. The floor check assumes a blind cut ends inside
   material; proposed, accept a cut whose end cap lies wholly outside the body when the
   volume fell. Then `feature_extrude` with `sketch` naming the consumed sketch made a
   second feature from it (a shared sketch, a 45° draft cut), verified, which the
   description does not say is allowed.

Sorted into S55 (1); S65 new (2); S64 (4). Held (3).

**2026-11-12: two parts mated into a subassembly; a limit angle mate in a second configuration of the top assembly**
Build `0.1.0-sw2023-20261112-091908+276d378`.

1. `asm_component_set` has no "solve as flexible" for a subassembly component: a
   subassembly's own free motion (a part turning about an axis inside it) cannot be
   opened up in the assembly above from the server. Not done; left to the owner by hand.
   Missing option, no open item seen covering it.
2. After `feature_suppress_set` of an angle mate in the active configuration and
   `asm_mate_add` of a limit angle mate (min 30, max 50) on the same two planes,
   `asm_component_list` still gives the subassembly component `constrained: "fully"`
   (`mates: 0`, its mates being on its child), and `model_review` reports no floating
   component. A limit mate leaves one degree of freedom, so "fully" may be wrong, or may
   be what SOLIDWORKS itself says of a limit mate; not settled in the session (the
   owner was to drag it). Worth a live test: a component held by a limit mate only on
   its last degree of freedom, its constrained status read against the tree's (-).
3. What held: `asm_mate_add` of a limit angle mate, verified "angle 40°, within the
   limits 30..50°" and the limits read back; `feature_rename` of it reported its one
   dimension with the limits as a tolerance (40 ± 10), which tells a caller the limits
   cannot be driven by `equation_set`. `config_add` then `feature_suppress_set
   {configs:"this"}`: a mate added in a new configuration came already suppressed in the
   other one (`previous: true`), as SOLIDWORKS does by default. `doc_close` of an
   assembly closed its parts with it and said so in a warning.

Sorted into S62 (1, 2). Held (3).

**2026-11-12: a feature with children replaced in a part driven by equations**

Build `0.1.0-sw2023-20261112-091908+276d378`.

1. `feature_delete {deleteChildren:true, deleteAbsorbed:true}` removed an extrude, its
   sketch, a child cut on its face and a mirror of that cut, and reported `ok` with the
   warning "'Equations' now has rebuild error 51 (swSketchErrorExtRefFail)". The seven
   equations that drove the deleted sketches' dimensions stayed behind, dangling. The
   dry run did not list them. Workaround: seven `equation_delete` calls, one per lhs,
   since it takes no list. Proposal: `feature_delete` lists the equations whose lhs
   dimension it removes (dry run too) and deletes them with an option; `equation_delete`
   takes several lhs in one call, as `equation_set` does since S37.
2. Lesson, held: `equation_set` with `sqr()` and `^` on mm globals
   (`a - (b - c/2) * sqr((c/2)^2 + (d - a)^2) / (c/2)`) evaluated as plain numbers and
   verified the driven values to 1e-6. `feature_rollback` to insert a feature mid-tree, then
   to `end`, worked as described: the later cuts took the new boss.

Sorted into S54 new (1). Held (2).

**2026-11-12: a part edited while an assembly the server opened held it**

Build `0.1.0-sw2023-20261112-091908+276d378`.

1. `doc_open` of a part the server had itself opened, saved and closed earlier in the
   session, now loaded by an assembly the server opened (`access: owned`), returned the
   part as `foreign` ("was already open ... the user's document ... this server won't
   change it", `loadedBy: D61`). The next `equation_set` failed `NOT_OWNED`. Workaround:
   `doc_activate {allowEdit:true}` (a backup made), one call. Expected: a part loaded by an
   owned assembly, opened from the same path the server had owned, counts as owned, or
   the message says the assembly loaded it rather than "the user's document".

Sorted into S62 (1).

**2026-11-12: a part's body raised by its variables, two small parts made, two assemblies re-mated**

Build `0.1.0-sw2023-20261112-091908+276d378`.

1. `sketch_constrain`, 18 steps on a closed 9-line polyline and a centreline (8 horizontal/vertical/collinear, 2 coincident, 2 `symmetric` of two lines about the centreline, 6 dimensions): `TIMEOUT` at 50 s; `sw_status` showed it completed at 54.6 s, succeeded. The same tool ran 83 s earlier the same day. Calls of 14 steps without `symmetric` took a few seconds, so the two `symmetric` steps are the suspect. Two `sw_status` calls to recover. No open item covers it.
2. Same sketch: the two `symmetric` (line, line, centreline) steps left the far ends of the second pair free: `constrained: under` until one more vertical dimension. A test dimension that would duplicate an X was refused as over-defining, so the lines' X was held and their Y was not. Lesson, or a hint: symmetric lines do not fix their endpoints along the centreline.
3. `feature_extrude` boss, sketch on an offset plane parallel to Right (normal +X), `direction:"normal"` and then `"+x"`: `FEATURE_FAILED: SOLIDWORKS created no feature (the API returned nothing)`. `direction:"reverse"` built it at once. The +X side had the body touching the profile's edges, no overlap. Workaround: the plane moved 2 out and the boss reversed. The hint (closed profile, positive depth) did not apply. No open item covers it.
4. `sketch_info` on a sketch of text lists each text's characters but not its position, and no tool moves a sketch text: text placed with no relation cannot be read or moved after the part's sizes change. Workaround: `feature_delete` with `deleteAbsorbed`, a new sketch, the text again (5 calls). Missing: the text's insertion point in `sketch_info`, and a move.
5. `asm_component_list {allLevels:true}`: a rotated component's `box` is its part box transformed, not tight: min Y 285.83 where the lowest vertex, by `model_measure`, is at 290.85. Reported without a word that it is loose; a session checking a clearance from it would be 5 mm wrong. A word in the description, or a tight box.
6. `asm_mate_add` distance, no `flip`: the component landed on the side opposite the one meant (Y -55 for +55), `ok`, "planes 55 mm apart": met again, as in the entry of 2026-11-12 on the side of a distance mate.
7. `asm_insert` into an assembly and `doc_rebuild` of another warned "was modified outside this server since its last change (by the user?)" while the owner was not in SOLIDWORKS; both had only been changed by this server's calls on documents they load. A false alarm that reads as the owner editing.

Sorted into S55 (1); S56 (2); S64 (3); S65 (4); S52 (5); S50 (6); S57 new (7).

**2026-11-12: a part placed in an assembly by hand, turned and tilted, no mates**

Build `0.1.0-sw2023-20261112-091908+276d378`.

1. `asm_component_move` / `asm_component_list`: after a rotation off the 90° grid (7.5° about X), the component's `box` is the part's own box rotated and re-boxed, not the box of its geometry: it read max Y 295.78 while the highest face, read with `face_list {component, normal:+y}`, lay at 262.9 (a corner of the part's box that no geometry reaches). Reported `ok`, nothing says the box is loose. Workaround: place by a face from `face_list`, 1 call. Proposal: a tight box (`IBody2.GetExtremePoint` along each axis, or the body's box computed in the assembly frame), or the word "loose after rotation" in the result. No open item covers it.
2. Worked as described, worth knowing: `asm_replace` of a fixed, unmated part by another file kept the old transform; float, `asm_component_move {absolute, rotation}`, fix took 3 calls, each read back.

Sorted into S52 (1). Held (2).

**2026-11-12: a printed part's walls redrawn to hang plumb in the world, through a tilted plane**

Build `0.1.0-sw2023-20261112-091908+276d378`.

1. `feature_extrude` boss, `direction:"0,-0.642788,0.766044"` (a vector 40° off the sketch normal), blind 55: built along the sketch normal, `directionModel:[0,0,1]`, checks `volume`, `cap at 55 mm`, no warning. The code (`ResolveDirection`) uses a vector only to pick the side by its sign against the normal; the tool's description ("a model axis like +y | x,y,z") reads as an extrude along that vector. Workaround: an angle reference plane square to the wanted direction, the profile converted onto it (`sketch_convert` of the sketch's segments, linked, fully defined), `through_all_both`, then a trim cut; 6 calls more. Proposal: refuse a direction not parallel to the normal, or extrude along it (`FeatureExtrusion3` with a direction reference), and say which in the description. No open item covers it.
2. `sketch_constrain`, 20 steps on a closed 10-line polyline and a centreline, 3 `symmetric` (point, point, centreline) among them: `TIMEOUT` at 50 s, completed at 89.5 s, succeeded. Met again, as point 1 of the entry before (symmetric steps the suspect); 10 `sw_status` calls while waiting. An 18-step call on the next sketch with no `symmetric` (horizontals, verticals, two `collinear`, 8 dimensions) came back in a few seconds.
3. `feature_delete ["Side wall","Wall plane","Mirror"]` with `deleteAbsorbed`: `VERIFY_FAILED: 'Side wall' still exist after the delete (removed: 'Wall plane', 'Mirror')`. The call failed but changed the model: two of the three were gone. A second call for the one left held. The `dryRun` before it, on the first two, listed `dependents: ["Mirror"]` and no word that the real call would refuse them without `deleteChildren` (it did: `INVALID_ARGUMENT`). Lesson: a failed delete may have deleted part of its list; read `removed` in the error.
4. Worth knowing, held: `feature_mirror` warned "the copy's faces have 1.271 x the seed's area: it meets the body differently from the seed". It was right: a later boss had filled the seed counterbore on one side, so the mirror was not symmetric. The check caught a design clash nothing else reported.

Sorted into S53 new (1); S55 (2); S64 (3). Held (4).

**2026-11-12: a printed part's tree deleted to its sketches and rebuilt as bodies combined, across two sessions**

Build `0.1.0-sw2023-20261112-091908+276d378`.

1. `sw_status` and `doc_activate` reported `dirty:false` for a part that carried ~2 hours of unsaved feature work from the session before (no `doc_save` in that session; a second server process listed in `otherServerPids`). The work was intact in the open document and saved by this session. A session that trusts `dirty` to decide whether to save would have closed or left it. No open item covers it.
2. `sketch_constrain` `collinear` between a segment of the open sketch and one of another, closed sketch: `INVALID_ARGUMENT: 'collinear' needs 2 different entities; some references name the same one`, both with `{name:"Line1", sketch:"<other>", type:"sketch_segment"}` (the open sketch also had a `Line1`) and with `{at:[...], sketch:"<other>"}`. The reference with `sketch` set resolved to the open sketch's segment. Workaround: `sketch_convert` the other sketch's segment, then constrain to the converted one (3 calls more). No open item covers it.
3. `sketch_constrain` with 3 `symmetric` steps among ~20: `TIMEOUT` at 50 s, finished at ~95 s; `sketch_info` meanwhile `BUSY`. Met again, as point 2 of the entry before.
4. `ref_plane {kind:"angle", axis:{name:"Line2", sketch:..., type:"sketch_segment"}}`: `INVALID_ARGUMENT: 'axis' resolved to sketch segment Line2, but this parameter needs edge or axis`. SOLIDWORKS takes a sketch line as a plane's rotation axis; workaround `ref_axis {kind:"two_planes"}` first, one call more. No open item covers it.
5. `sketch_constrain` `merge` of two coincident endpoints given by the same `at` point: refused, "needs 2 different entities"; a `coincident` of the endpoint to the other segment then `VERIFY_FAILED ... no relation was added` (the points were already merged by `sketch_add`). The sketch was fully defined by one more dimension. Lesson: endpoints a polyline was drawn onto are merged already; `sketch_info`'s relation count says so.
6. `view_screenshot` zoom-to-fit includes reference axes: a construction axis ~150 mm from a ~100 mm part left the part ~8% of a 1870 px image. Proposal: `hideAxes` (default true, as `hidePlanes`) or fit to bodies only. No open item covers it.
7. Held, worth knowing: `body_combine` add and subtract read back each body's volume and the overlap (`union 535043.921 = 25588.063 + 527223.352 − overlap 17767.494`); `feature_mirror` of two cuts checked 6/6 faces at their reflected positions; `asm_interference` filtered to one component came back in one call.

Sorted into S60 (1); S64 (2); S55 (3); S59 (4); S56 (5); S65 (6). Held (7).

**2026-11-12: two parts changed mid-tree: tool bodies cut from two bodies before a combine, and a through-all cut replaced by a loft**

Build `0.1.0-sw2023-20261112-091908+276d378`.

1. `feature_mirror {features:[two extrudes made with merge:false]}` about the Right Plane: `FEATURE_FAILED: feature_mirror: SOLIDWORKS created no feature (the API returned nothing). Hint: The mirrored copy must touch the body`. The copies were separate tool bodies, not meant to touch. There is no body mirror tool (Insert > Features > Mirror with bodies to mirror). Workaround: the other side built feature by feature, axis, two planes, two sketches, two extrudes, ~12 calls more. No open item covers it.
2. `sketch_add` of a rectangle on a plane that cuts through an existing tool body: the rectangle came back with 1 or 2 `coincident` relations to that body's edges, though inference is said to be off while the server draws. Two later `sketch_constrain` dimensions were then refused as over-defining ("SOLIDWORKS made it a driven reference dimension; it was removed"). The sketch ended fully defined through those relations. The result of `sketch_add` does not say a relation was added; `sketch_info` showed it (`relationTypes: coincident`). No open item covers it.
3. Renaming a new sketch to the name of a deleted one (`feature_rename`): SOLIDWORKS bound the deleted sketch's equations to the new sketch's `D1`-`D4` by name, though they meant other sizes. The result showed it only as `driven:true` on those dimensions. Proposal: `feature_rename` warns when the new name makes existing equations drive the renamed feature's dimensions, and lists them.
4. `feature_delete {deleteChildren:true}` of a plane whose offset an equation drove: `warnings: ["'Equations' now has rebuild error 51 (swSketchErrorExtRefFail)."]`; the equation stayed, its left side renamed with the feature (`D1@<renamed plane>`), until `equation_delete`. Proposal: the dry run lists the equations that drive the features to be deleted, and the delete removes them with them, or offers to.
5. `equation_set` on a reference plane's offset gives the volume change but not where the plane went; an offset that put the plane on the wrong side of its base passed every check, found only by a later feature's volume against a hand figure. Proposal: when an equation drives a reference plane or axis, the result gives its frame (origin, normal), as `ref_plane` does.
6. `equation_set` with a negative offset for a plane: refused, "a value the dimension can't have, such as 0?". Right, but no tool changes an existing plane's flip; it had to be made again with its sketch. Proposal: `ref_plane` edit of `flip` on an existing plane, or the hint naming the flip.
7. Warning `'<part>' was modified outside this server since its last change (by the user?)` came after the server's own calls and nothing by the user: after `feature_rollback` re-activated the part, after a failed `feature_mirror` that was rolled back, and after `feature_delete`. A false alarm in this session. No open item covers it.
8. Held, worth knowing: `ref_axis {kind:"two_points", entities:[{type:"sketch_point", sketch:<closed sketch>, at:[...]} x2]}` made an axis through two points of a closed sketch, a way round point 4 of the earlier entry of the same day (a sketch segment refused as a plane's axis). `body_combine` subtract with two tools at once gave each body's overlap; `feature_loft` cut between two closed five-line sketches mapped vertex to vertex, all faces planar.

Sorted into S65 (1); S56 (2); S54 (3, 4); S58 (5); S59 (6, 8); S57 (7).

**2026-11-13: an existing part edited: a section changed by a variable, a loft cut, a boss added and removed, grooves patterned**

Build `bin\0.1.0-sw2023-20261112-091908`.

1. `equation_set` (three globals changed) returned `volumeDeltaMm3: 32995.863`, `bodies: 1` and only `warnings: ["The rebuild reported problems; feature_list shows features with errors."]`, while three features had failed (`ICE: swSketchErrorExtRefFail` on a boss, the cut on it and a mirror), so the volume and box read as a success with the boss missing. A `model_review` call was needed to name them. Proposal: the warning names the failing features and their error, as `feature_delete`'s warning names `'Equations' ... error 51`. Reported as success, with a warning. No open item covers it.
2. `feature_delete` of three features with `deleteAbsorbed` left the equations of their dimensions: `'Equations' now has rebuild error 51 (swSketchErrorExtRefFail)`. Cleared by six `equation_delete` calls, one each: no batch form. Proposals: `feature_delete` lists the equations that drive the deleted dimensions (and an option removes them), and `equation_delete` takes several left-hand sides in one call, as `equation_set` does.
3. Held, worth knowing: `feature_loft` cut between two fully defined five-line sketches on parallel planes, one with a 0.05 edge standing for nothing (a loft cannot start from a point), gave the expected volume (-898 against ~-880); `sketch_mirror` of a closed four-line profile about a centreline left the sketch fully defined; `feature_pattern_linear` of a cut whose instance meets the body differently reported `volume change per instance` with a clear warning that the area differs, correct here. `equation_set` with an expression that reads another sketch's dimension (`"D4@<sketch>"`) set and verified fine.
4. `sketch_add` of a construction line drawn from one converted corner to the opposite one (after `sketch_convert` of a face) merged its ends onto the corners without saying so; the following `sketch_constrain` `coincident` of that end with the converted line then failed `VERIFY_FAILED: SOLIDWORKS ignored the relation 'coincident' ... no relation was added`, and steps 2 to 6 were not run. The sketch was in fact fully defined after a `midpoint`. Proposal: `sketch_add` reports end points it merged onto existing points (as `sketch_constrain` reports relation counts), and the ignored-relation hint names an existing coincidence when that is the reason. Reported as a failure; no harm, one extra call.

Sorted into S58 (1); S54 (2); S56 (4). Held (3).

**2026-11-13: four small printed parts (a plate on a tilted seat, a sleeve, a tube, a bearing stand-in) and six components mated into an existing assembly**

Build `0.1.0-sw2023-20261112-091908+276d378`.

1. `asm_mate_add` `distance` between parallel planes, no `flip`: the component landed on the side opposite the one meant, twice in one assembly (Y 29.28 for 268.76, Y 142.42 for 195.62), each reported `ok`, "planes … mm apart". Met again, as in the entries of 2026-11-12 on the side of a distance mate; no item yet. The pattern held all session: unflipped puts `a` on the minus side of `b`'s normal, `flip:true` on the plus side. Workaround: `feature_delete` of the mate and the same call with `flip:true`, two extra calls each; after that, each distance mate was given its flip from the side it should land on and read back from `originMm`, all four right first time. The proposal stands: a signed distance, or the side in the `verified` line.
2. `sketch_add` `rectangle` by corners that happened to be square (7 x 7) got an `equal` relation from SOLIDWORKS that the result did not report. The next `sketch_constrain` then failed at its last step: `VERIFY_FAILED: … would over-define the sketch (SOLIDWORKS made it a driven reference dimension)`, because the height was already tied to the width. `sketch_info` showed `relationTypes: {equal: 1}`. Workaround: `sketch_finish`, `feature_delete` of the sketch, and a `polyline` drawn not square, four extra calls. Proposal: `sketch_add` lists the relations SOLIDWORKS added to the entities it drew (a square rectangle's `equal`), or suppresses that inference as it does snapping. The error itself was clear and no harm was done.
3. `asm_interference` with `includeCoincident:true` returned 9 touching pairs, each with volume 0, plus the warning `SOLIDWORKS counted 2 interference(s) but listed 25.` The three numbers (9, 2, 25) do not explain each other. Without `includeCoincident` it read 0, which was right. Proposal: drop the warning when every listed pair has zero volume, or say what each count is.
4. Held, worth knowing: `feature_revolve` `cut:true` with `axis` naming a solid line of the closed profile, not a centerline, worked. It was a stepped profile on a standard plane, its axis tilted 30° (a Ø3 hole and a Ø6 counterbore along one axis): "3 face(s) coaxial with the axis [0.866, 0, 0.5]", -800.0 against ~800 by hand, and `feature_mirror` of it reflected right. A fully defined sketch on the Top Plane took 15 steps in one `sketch_constrain`, with the angle measured against a construction line from the origin.

Sorted into S50 (1); S56 (2); S62 (3). Held (4).

**2026-11-13 (later): a groove's overhanging side chamfered away in a part with a pattern, a hole swapped from through to blind**

Build `0.1.0-sw2023-20261112-091908+276d378`.

1. `feature_chamfer` `angle_distance` and `distance_distance` do not say which face takes the distance (or the angle), and `flip` is one switch for every edge in the call. Two edges in one call, the same groove's side mirrored and then patterned, took the distance on opposite faces: in the result one chamfer face stood 60° from the face meant, the other 30°. The leg check caught it: `VERIFY_FAILED … no chamfer face with legs 1/1.732051 or 0.57735/1 mm along edge E6 (found legs 1/1.085649 mm)`. Only one edge was named, though both were in the call. Workaround: one feature per edge, each with its own `flip`, found by trying. Five attempts and four deletions before a pair was right. Proposal: the result names, per edge, the face that took the distance and the angle the chamfer face makes with each adjacent face; `flip` per edge.
2. `feature_chamfer` `distance_distance` 1.4835 / 2.2 on an edge whose one face is 1.4835 wide: `VERIFY_FAILED … (found legs 1.4835/1.550921 mm)` with and without `flip`, the face then 32° off the roof, not what either order asks. The intent (consume the 1.48 face, steep along the other) needed `angle_distance` with the distance on the narrow face. A short note in the description would save the tries: a distance equal to a face's width consumes it; give the steepness as an angle from that face.
3. A chamfer meant to consume a face whole, its distance typed to 4 decimals (1.4835 for 1.483537), passed with `ok` and left a sliver of that face, 0.0003 mm², seen only in a later `face_list`. A typed 1 for an exact 1 consumed the face with `faceCountDelta: 0`. Driving the distance by an equation to the exact value removed the sliver. Proposal: the chamfer's check flags a face of the selected edges left with an area under, say, 0.01 mm².
4. `feature_delete` of a feature that an earlier call had kept with `keepOnFailure` warned `'… .SLDPRT' was modified outside this server since its last change (by the user?)`, three times in a row. Nobody else touched the part; the owner was not in SolidWorks. Likely a false alarm from the kept feature. Proposal: a feature this server kept on failure counts as its own change.
5. Held: `equation_delete` of a sketch's two driven dimensions before `feature_delete` with `deleteAbsorbed` left no equation error (the lesson of the 2026-11-12 entry, done in that order). `dimension_set` on a plain hole diameter in a part full of equations: -10.933 against -10.9 by hand.

Sorted into S51 new (1 to 3); S57 (4). Held (5).

**2026-11-13: a revolved part remade as one extrude, a groove set patterned again and chamfered, parts swapped and mated in an assembly**

Build `0.1.0-sw2023-20261112-091908`.

1. `asm_mate_add` `distance` between two parallel planes, twice: with the alignment the earlier mates of the same kind used (`aligned` or `anti_aligned`, no `flip`), the component was solved onto the far side of the other plane, 73 mm and 5.5 mm the wrong way. Reported `ok`, correctly for the parameters given. Workaround: `feature_delete` of the mate and the same call with `flip:true`, 2 calls each time. Proposal: the result gives the signed offset of the component's plane along `b`'s normal, or the call takes a point the component should end up toward, so the side is chosen and not found by trying.
2. `feature_chamfer` `angle_distance` on the edges of a second pattern instance, one side each: one side needed `flip:true`, the other none. The first try failed `VERIFY_FAILED: … no chamfer face with legs 1/1.732051 or 0.57735/1 mm along edge E12 (found legs 1/1.085649 mm)` and was rolled back. The check held. This was met before in the 2026-11-13 entry; it is the same here.
3. The `feature_chamfer` that followed that rollback warned `'… .SLDPRT' was modified outside this server since its last change (by the user?)`. Nobody else was in SolidWorks; this server had only run `equation_set`, `feature_pattern_linear` and the failed chamfer. Point 4 of the previous entry meets it after a `keepOnFailure`; this case had none. Likely cause: the rolled-back feature. Proposal as there: a rollback counts as this server's own change.
4. `asm_interference` with `includeCoincident:true` returned 10 pairs, all 0 mm³, with the warning `SOLIDWORKS counted 2 interference(s) but listed 26.` Neither 2 nor 26 matches the 10 returned. Without `includeCoincident`, count 0. Proposal: say what the two counts are and how 26 became 10.
5. Replacing a revolve whose sketch had 6 dimensions driven by equations took 6 `equation_delete` calls for those, 2 more for the variables they used, then `feature_delete` with `deleteAbsorbed`. That is 9 calls for one feature. No equation error came of it. Proposal: `equation_delete` takes a list, or `feature_delete` offers to drop the equations of the dimensions it removes.
6. S33, met again: four key sizes of one part (a thickness, a length, a washer, a flange) were typed again as global variables in a second part that places a groove from them. A change to one means an `equation_set` in each.
7. Held: `asm_insert` with a 180° rotation about X read the placement back. `material_list {filter}` found the steel at once. `doc_close` of an assembly warned which of its references it closed with it.
8. Point 3, met again with its cause narrowed: on a second round, `'… was modified outside this server since its last change (by the user?)'` came on the two `feature_chamfer` calls that each followed a `VERIFY_FAILED` rollback, and on no other call. The rollback is the trigger.
9. Held: `doc_activate {allowEdit:true}` on three documents the owner had open as references of an open assembly; each backup was written, and the edits and `doc_save` went through with no conflict. `dimension_set` on two mates' `D1@DistanceN` moved the parts that depend on them.
10. `feature_chamfer` `angle_distance` on the two circular end edges of a tube, in one call: `ok`, with `2 edge(s) not leg-checked (curved edge or non-planar faces)`. One end came out right and the other with its legs swapped, 1 radial and 0.577 axial. Only the volume showed it: -28.07 against -28.32 by hand, the sum of -14.16 and -13.91. `edge_list` on the feature confirmed it. This is the flip case of point 2 on curved edges, which the leg check skips, so here the server reported success on a wrong result. Proposal: on circular edges, measure the legs from the chamfer's two boundary circles (their radii and axial offset). Workaround: one chamfer per end, the second with `flip`. `feature_mirror` of the first chamfer about the mid-plane failed: `FEATURE_FAILED: … SOLIDWORKS created no feature (the API returned nothing)`.

Sorted into S50 (1); S51 (2, 10); S57 (3, 8); S62 (4); S54 (5); S33, met again (6). Held
(7, 9).

**2026-11-13: one chamfer over three edges split into two, mid-tree, in a part with 50 features**

Build `0.1.0-sw2023-20261112-091908`.

1. Missing: no call changes an existing chamfer's edge set or gives one edge its own distance. A distance-distance chamfer ran over three concave edges at the back of a flange, an arc and two straight ones, the flange 10 wide along the arc and 8.07 along the straight ones, so one distance (7.57) left 2.43 of flat along the arc. The fix took 7 calls: `feature_rollback` to the chamfer, `feature_delete`, two `feature_chamfer`, `equation_set`, `dimension_set`, `feature_rollback {to:'end'}`. All reported success and were right; the 42 features below the bar rebuilt with no error. Proposal: an edit of a chamfer's or fillet's edges, or per-edge distances in one `feature_chamfer`.
2. `feature_delete` of a chamfer whose `D1` an equation drives: removed, with the warning `'Equations' now has rebuild error 51 (swSketchErrorExtRefFail)`. The equation stayed. A new `feature_chamfer` given the same `name` took it over: its result read `D1@… value 9.5, driven:true` while the equation still held the old expression (7.57), so for one call a dimension was marked driven by an equation it did not follow. `equation_set` on that lhs then reported `created:false` with the old text as `previous`, and the value held. Bears on the earlier proposal that `feature_delete` offers to drop the equations of the dimensions it removes; here keeping it was what the job wanted, so it should be a choice.
3. The curved-edge leg check, met again (`1 edge(s) not leg-checked (curved edge or non-planar faces)`). Workaround that cost one call: make the chamfer with legs 0.1 apart (9.5 and 9.4), read the new cone face with `faceDetail:true`, `axisOriginMm` y 15.4 on an edge at y 6 and `halfAngleDeg` 45.30, which says `distance2Mm` is the axial leg, then `dimension_set` the second leg to its real value. The cone's apex height and half angle are in the result already: the server could work the two legs from them itself.

Sorted into S51 (1, 3); S54 (2).

**2026-11-13: a plate's two ends cut to a quarter round through its thickness, mirrored, and its top edges rounded**

Build `0.1.0-sw2023-20261112-091908`.

1. `feature_fillet` and `feature_chamfer` over a tangent chain, a 99 mm straight edge running into a quarter-round R7 edge at each end, the round ending at a vertex on a face 30° off its tangent, in a plate 10 thick: radius or distance 1 failed five times, `FEATURE_FAILED: … SOLIDWORKS created no feature (the API returned nothing)`, with `propagate` on and off, by `at` and by id, and on the round edge alone. The straight edges alone took R1. R0.5 over the whole chain with `propagate:true` went through, -13.0 against 13.0 by hand. The hint (`A radius too large for the adjacent faces`) was right in the end but the faces are 7 and 10 wide, so nothing in it pointed at 1 being too large. Proposal: on this failure, retry per edge internally and name the edge that fails.
2. `model.box` in the result of that fillet, and of every `equation_set` after it, read 0.09 outside the body: max x 56.649842 on a body that ends at 56.559923, y ±5.089919 on a plate ±5. The fillet's faces there are tori; before the fillet the box was exact. Reported with `ok`. The volume was right.
3. A stale edge id: after a fillet changed the edges, a second `feature_fillet {edges:[{id:"E25"}]}`, an id from the listing before it, resolved to another, short edge and filleted it (`ok`, -0.666). `edge_list` after showed the round edges as E28 to E31. Proposal: an id from before a topology change is refused, or the result says the ids were renumbered.
4. The `modified outside this server` warning, met again: it came on each call that followed a `FEATURE_FAILED` with no feature made, as it does after a `VERIFY_FAILED` rollback.
5. Held: `equation_set` with `equations`, three driven dimensions and then two variables, each one rebuild with the volume in the result; `feature_mirror` of a revolved cut and an extruded cut together, `3/3` faces reflected; `feature_delete` of the two fillets in one call.

Sorted into S64 (1); S52 (2); S66 new (3); S57 (4). Held (5).

**2026-11-13: two printed plates of the same features with opposite extrude sides, and an assembly's mates redone**

Build `0.1.0-sw2023-20261112-091908+276d378`.

1. `sketch_constrain` past the 50 s limit with no `symmetric` step: 12 relations and 42 dimensions on a sketch of six closed polylines (27 lines) returned `TIMEOUT` and completed at 359 s, all applied; the same batch on a second part, again ~360 s. A 15-step batch of `coincident`, `tangent` and `horizontal` on 8 segments took 55 s, a 17-step one on 9 segments 42 s. The time per step grows with the sketch, ~6.7 s a step at 27 segments. Each wait cost 3 to 4 `sw_status` and background sleeps. S55, met again; its suspect, `symmetric`, was not in these.
2. `feature_pattern_circular` reported `ok` with its instances turned the other way from what was meant: a 10° pattern of a cut at 30° about a hole whose cylinder face reads its axis as [0,0,-1] put them at 20° and 10°, the result saying "instance faces at the expected positions: 10/10 (3 x 10° about [0, 0, -1], counter-clockwise)". The check confirms the sense the server chose, and the tool has no option for the sense. `totalAngleDeg` 350 was refused ("go past 360°"). Workaround: the seed moved to the other end of the range, 4 calls. On the same part a 270° pattern said "rotation sense flipped (SOLIDWORKS' default missed the part)" and landed right. Proposal: a `reverse` option, and the instances' angles reported about a model axis the caller names. Bears on S41.
3. `feature_pattern_linear` of a rib and its own linear pattern together failed "SOLIDWORKS created no feature (the API returned nothing). (tried 2 senses of the direction)"; with `direction2` the same axis as `direction` it is refused. Four ribs at uneven spacing along one axis took a pattern and a mirror about a new plane.
4. A `vertical` relation between `{at}` on an arc centre and the origin failed "'vertical' conflicts with existing relations or dimensions (the sketch became no_solution)", where three concentric arcs from `sketch_offset` and three line ends sat on that point. The same relation worked in a sketch with one circle there. Likely resolved to an offset arc's centre, which the offset relation holds; the result does not say which point it took. Workaround: `concentric` with the model's hole edge. S65 (c), met again.
5. No way to merge two segment ends drawn at the same coordinates: `{at}` twice is refused "'merge' needs 2 different entities; some references name the same one", and a polyline with a separate arc at its ends leaves them unmerged ("ManyOpen"). Workaround: the arc drawn 0.2 to 0.5 off, then `coincident` on the two distinct points, 1 call more per joint. Proposal: a segment's end named (`{name:'Line1', end:'start'}`), or `sketch_add` merging ends that meet.
6. On a face sketch a rectangle's corner on the face's vertex got a relation from SOLIDWORKS, unreported; a `horizontal` dimension from the origin to it was then refused "would over-define the sketch". The opposite corner, also on a vertex, got none. S56, met again.
7. `dimension_set` on a mate's angle driven by an equation refused it "is a driven (reference) dimension: it reports a value but doesn't control the geometry". It is driving, through an equation; the hint should name the equation's variable, as for a part's dimension.
8. Held: the angular dimension's warning that `textAt` lay in the supplement, which let it be fixed by `dimension_set` 130; `feature_rib` flipping its side when the first met no wall, with a warning; `asm_replace` keeping 21 of 22 mates and naming the one on a face it lost; `asm_mate_add` coincident of two faces anti-aligned, `originMm` read back; `equation_set` with 27 equations in one call.

Sorted into S73 new (2); S64 (h) new (3); S65 (c), met again (4), and S65 (e) new (5); S74
new (7). S55 (1) and S56 (b) (6), met again on the build before their fixes in build
`20261114-024455`, both Closed. Not filed (8): works as described.

**2026-11-14: scenario test of the run's items (S47 to S59, S60 b-d, S61 b-c, S62 a/d/e, S63, S64 a/d-g, S65 a, S66)**

Build `0.1.0-sw2023-20261114-024455+cfd987e` (`sw_status` and the tag agree).

1. A SOLIDWORKS process in a bad state, met before only as the restart hint's reason: on a process started before the session (what ran in it earlier is not known), every equation was refused, a batch of ten globals and then `"W" = 60mm` alone in a new part; S58 (b)'s message came as designed ("though the equation is valid … restart SOLIDWORKS"). In the same process `material_set "AISI 1020"`, with or without a configuration, applied the material at 1 kg/m³: the result gave `densityKgM3: 1`, `databaseDensityKgM3: 7900`, the check "material reads back" and only a warning ("The mass properties give 1 kg/m³, the database 7900 kg/m³ (bodies with their own material, or an overridden mass?)"), with no error, though the description says the mass properties are verified to show its density. A material from the user library at 1270 kg/m³ read right in that process. A property linked to a 3 mm dimension (`"D1@<feature>"`) resolved `0,0030000000000000`, metres. After a restart of SOLIDWORKS (by the session, on the owner's word) the equations went in at once, a database material read 2700 = its database, and the restart message was the right advice. The same three symptoms were met together once before, in an old SOLIDWORKS session (S28's test). Proposals: `material_set` refuses (`VERIFY_FAILED`, rolled back) when the density read differs from the database's, as it once did; and its message, like S58 (b)'s, names a restart when a database material reads ~1 kg/m³. Bears on S58 (b), S30.
2. `feature_chamfer` `edit` with `flipEdges` flips every edge, not the indexed ones, reported `ok`: a `distance_distance` 1/2 chamfer on two bottom edges of a block meeting at a corner (one along x, one along z) took 1 mm on the bottom face for both. `edit` with the same two edges and `flipEdges:[1]` made both take 1 mm on their side faces (the per-edge checks say so, and the chamfer faces' normals [0, -0.894, -0.447] and [0.447, -0.894, 0] confirm it), with no mixed-sides warning; `flipEdges:[0]` put both back on the bottom. A new chamfer with `flipEdges:[1]` on two other edges gave both their first distance on the side faces, consistent with each other, so what index 0 got there cannot be told. S51 (a), (d).
3. `ref_plane` `angle` with `axis: {name:"Line1", sketch:"Sketch6"}` failed `ENTITY_NOT_FOUND: No feature named 'Line1'`; with `type:"sketch_segment"` added it went through and checked right ("contains sketch segment Line1, 30° to plane 'Front Plane'"). With `sketch` given the reference is a sketch segment or point (the server's own convention), so the type could be inferred. One call more. S59 (c).
4. `dimension_set` through an assembly: the line naming the part reads "Also changed, not saved: '<part>.SLDPRT', a component's own document." without handle and access in a new assembly whose parts the server had not listed yet, and "'<part>.SLDPRT' (D19, foreign), …" in an assembly opened from file. S49, wording.
5. `doc_open` of a STEP written by `model_export` from an assembly of three 20 mm cubes and a cylinder r 5 × 30 turned 45° about [1, 1, 0]: the cubes came in as solids where they were, the cylinder as 3 surface bodies, its component at the identity rotation with box [-5.0001, 0, -5] .. [6, 30, 5.427], neither the turned cylinder's box nor a tight one of r 5, and without `approximate`. S61 (b)'s message said "3 solid bodies, 3 surface bodies" plainly. Bears on S5 and on S52's fallback. `model_export` of that assembly reported `solids: 2` (two distinct parts, four components).
6. Held, each as its item says: S47 frame [20, 10, 410, 287] / [230, 10, 410, 65.25] on a 1:2 sheet in `drw_info` and `drw_sheet_setup`, a note on the title block, one across the border, one clear, dimension text at y 292 and a section reaching y 289.01 warned; S66 an edge id from before a notch `ENTITY_STALE`, an untouched one used; S53 a boss along a vector 40° off the normal, volume π·r²·h·cos 40°; S52 (a) a disc's torus fillet box exact, (b) a turned component's box the expected extremes; S50 the signed side and the crossing warning; S48 (a) to (d); S49 the component in the row and the volume, in a hand-mated assembly too; S54 (a) to (c) with `deleteEquations` and a list delete; S55 `durationS` (10 steps 9.7 s); S56 (a) "already held" and (b) relations listed; S57 no warning after a rolled-back `VERIFY_FAILED` and after a change through an assembly; S58 (a) features named in `featureErrors`, (b) both messages, (c) `frame` in a batch row; S59 (a), (b), (d); S60 (c) "same as before" and, after the restart, "a new one (before: pid …)", (d) no revision warning; S61 (c) the empty part named; S62 (a) plane names and face ids, (d) a part loaded by the server's assembly owned, (e) listed/overlapping/touching; S63 start at +Z, 90° to +X read back, turns `scalar`; S64 (a), (d) (the dry run's refusal word for word), (e), (f), (g); S65 (a). Regression: the shelled tray step by step, each volume as before, and its design intent with one body; the machined part of global variables in 37 calls, 16.3k (39 / 26.3k before), intent and back; the two sections (1 and 3 hatched faces); STEP of the tray still 16 surface bodies (S5).

Sorted into S72 new (1); S51, back to Open (2); S59, back to Open (3); S49, back to Open (4);
S52, back to Open, and S5, met again (5). Held (6): S47, S48, S50, S53 to S58, S63 and S66,
closed; the held parts of S49, S52, S59 to S62, S64 and S65. S58 (b) held in (1) too.

**2026-11-16: scenario test of build 20261116-050908 (S51, S73)**

Build `bin\0.1.0-sw2023-20261116-050908` (`serverVersion` `…-050908+dedd986`, tag
`build-20261116-050908`), SOLIDWORKS 2023 SP3.

1. S51, held. A `distance_distance` 1/2 chamfer on a 60 × 40 × 20 block's two bottom edges meeting at a corner (one along x, one along z) took 1 mm on the bottom face for both; `edit` with the same two edges and `flipEdges:[1]` left edge 0 with 1 mm on the bottom face and gave edge 1 its 1 mm on the side face, the chamfer faces' normals [0, -0.447, 0.894] and [0.447, -0.894, 0] (read with `face_list`) agree. The result: `alsoCreated` `Chamfer2`, the warning "SOLIDWORKS keeps one flip for chamfer edges that meet at a corner: edge E1 moved to 'Chamfer2' …" and the mixed-sides warning. A new chamfer with `flipEdges:[1]` on the opposite corner's two edges: edge 0 unflipped (1 mm on the bottom, its face [0, -0.447, -0.894] in the chamfer split off), edge 1 flipped, the same two warnings. Two edges apart (the top's long edges) with `flipEdges:[1]`: one chamfer, no split, the per-edge flip held; `edit` of it with `flipEdges:[0]` swapped exactly the two sides. Reported `ok`, right each time.
2. S51, small, reported `ok`: when an edge moves to a chamfer of its own, `newFaces` lists only the named chamfer's face. The new chamfer with `flipEdges:[1]` above said `faceCountDelta` 2 and `newFaces` one face; the split-off chamfer's face had to be found with `face_list {feature}` (one call). Expected: the faces of both chamfers, or those of `alsoCreated` named apart.
3. S51, lesson: `edit` with `flipEdges` alone is refused, `INVALID_ARGUMENT: 'flipEdges' indexes 'edges': pass the chamfer's edges in 'edges' as well.` The edges must be repeated, in the order the indexes mean, though the chamfer holds them; one call more. The message is clear.
4. S73, held. A 100 × 100 × 10 plate on the front plane, a Ø10 hole at the origin whose face reads its axis [0, 0, -1], a Ø4 seed cut at 30° on r 30; `feature_pattern_circular` 3 × 10° (`equalSpacing:false`) about the Ø10 face: the instances at 20° and 10° (read with `face_list`), the check "3 x 10° about [0, 0, -1] (the axis as read), counter-clockwise: the instances at +10°, +20° from the seed; the axis as read points the other way from +z: about +z, clockwise, at -10°, -20°". After `doc_undo`, the same with `reverse:true`: the instances at 40° and 50°, the check the mirror of it ("clockwise … about +z, counter-clockwise, at +10°, +20°"). Not met: a pattern whose default misses the part ("as reverse:true"), and `reverse` that misses it ("or drop reverse").
5. Held, regression: `feature_chamfer` `equal` 2 mm on a bore's circular bottom edge (a plate with a Ø20 bore), −134.041, legs measured on the circle ("2 mm along … plane, 2 mm along … cylinder r 10", 45° and 45°), no warning; `doc_undo` of a pattern and of 2 steps (a cut and a pattern); a 1:2 sheet's frame, the two sections of the shelled tray (1 and 3 hatched faces); interference 0 in a saved hinge assembly. STEP of the shelled tray still 16 surface bodies (S5, met again).

Sorted into S75 (2), new; S5 (5), met again. Held: S51 (1), S73 (4). Not filed (3): by design,
`flipEdges` indexes the edges as the call names them, and the message says so.

**2026-11-16: scenario test of build 20261116-080311 (S68, S69, S70, S62 b, S75)**

Build `bin\0.1.0-20261116-080311` (`serverVersion` `0.1.0-sw2023-20261116-080311+643fc77`, tag
`build-20261116-080311`), SOLIDWORKS 2023 SP3, started by the session (`sw_connect launch`)
after the first attempt stopped for low memory.

Setup: a 20 mm cube part (origin at a corner); a subassembly of it twice (one fixed at the
origin, one floating at [0, 15, 30]); a top assembly with the subassembly fixed at z 100
inserted first, then the cube twice at the top level, floating, so `<cube>-1` and `<cube>-2`
exist both at the top level and as `<sub>-1/<cube>-1`, `<sub>-1/<cube>-2`.

1. S70, held. `asm_component_move` `<cube>-1` by [0, 0, 10] moved the top-level one (50, 40, 0 → 50, 40, 10); `asm_component_set` `<cube>-2` `visible:false` hid the top-level one, the nested ones visible and in place (`asm_component_list allLevels`). A mate with `component: "<cube>-2"` also took the top-level one. Not met: the refusal of a short name only nested components share.
2. S69, held. `asm_mate_add` coincident, `{component:"<cube>-2", name:"Top Plane"}` with `{component:"<sub>-1/<cube>-2", name:"Top Plane"}`: "planes coincide (0 mm apart)", the top-level cube's origin to y 15, the mate listing the nested component's `Top Plane`.
3. S68, held. Coincident `{component:"<cube>-1", name:"Origin"}` with `{name:"Origin"}` (the assembly's): "points coincide", the component's origin read back [0, 0, 0]. A component's origin with a vertex of a component inside the subassembly (`type:"vertex", at` in that part's mm): the origin at the vertex, [20, 15, 150]. Small: `{component, type:"origin"}` with no name was refused `INVALID_ARGUMENT: 'a' needs 'id', 'where', 'at' or 'name'.` (one call more), though the server's instructions list `origin` as a reference type and a component has one origin; and in the mate's row (result and `asm_mate_list`) an origin reads `kind: "point"` with `pointMm` only, so it cannot be told from a sketch point.
4. S62 (b), held. `asm_component_set` on the subassembly: `flexible:true` "rigid → flexible", `false` "flexible → rigid", `false` again "nothing: it was already as requested", each row with `flexible` and `previous.flexible`; on a part `INVALID_ARGUMENT: '…' is a part: only a subassembly is solved as flexible or rigid.` Flexible again, with the top-level cube fixed: `Front Plane` of the nested floating cube coincident with the top-level cube's `Front Plane` moved the nested cube from z 130 to z 0 while the subassembly stayed at z 100.
5. S62 (b), reported `ok`: setting the subassembly rigid while that mate held its child where the subassembly's own file does not put it returned "flexible → rigid" with no warning; the child went back to z 130 and the mate went into error (`asm_mate_list`: `inError: 1`, `error: 51`, `errorName: "swSketchErrorExtRefFail"`). Expected: a warning, or a check line, naming the mates the change put in error. The error name, a sketch-error name on a mate, may come from the wrong enum.
6. S62 (b), reported `ok`: a child of a flexible subassembly that floats in its own file, and that the mate in 4 had just moved, reads `fixed: true, constrained: "fully"` in `asm_component_list allLevels` and in the mate's `components`; the same before, while rigid. A caller cannot see from the rows that the child can move.
7. Bears on S70, reported `ok`: the mate in 4 moved the nested cube and, through the two earlier mates, the top-level cube of the same short name (20, 15, 150 → 20, 15, 20). The result gave `moved: ["<cube>-2"]`, one short name that is the top-level one's full name, and `components` listed the nested cube and the fixed one but not the top-level one that moved. Expected: `moved` with full names (`<sub>-1/<cube>-2`), every component that moved listed.
8. S75, held. A `distance_distance` 1/2 chamfer with `flipEdges:[1]` on a 60 × 40 × 20 block's two bottom edges meeting at a corner: `newFaces` two faces, the second with `feature: "Chamfer2"`, `newFaceCount` 2 = `faceCountDelta`, `alsoCreated` `Chamfer2` and both warnings as before. A plain chamfer elsewhere: one face, no `feature`.
9. `doc_undo` right after that split chamfer: `VERIFY_FAILED: Nothing changed: SOLIDWORKS had nothing to undo, or the last step can't be undone. Hint: Check the model with feature_list.` Both chamfers stayed. In the same part, `doc_undo` of a cut made after it worked, and of a plain `equal` chamfer in another part. Not reported `ok`; the hint could say that a chamfer split by `flipEdges` is undone by `feature_delete` of both chamfers. Bears on S51, S75. Not tried on the build before.
10. Held, regression: a hand-mated two-part assembly (fixed part, shaft with concentric and coincident): 2 components, 2 mates without error, volume 127 402.659; the shaft's length 60 → 80 through the assembly, +6283.185, mates without error, and back; closed with `discard`, the files byte for byte unchanged. Interference 0 in a saved hinge assembly. A 1:2 sheet's frame [20, 10, 410, 287] / [230, 10, 410, 65.25], the shelled tray's two sections (1 and 3 hatched faces). `feature_chamfer` `equal` 2 mm on a Ø20 bore's bottom edge −134.041, no warning. STEP of the shelled tray still 16 surface bodies (S5, met again).

Sorted into S76 (5), new; S77 (6), new; S78 (3, 7), new; S79 (9), new; S5 (10), met again. Held: S70 (1), S69 (2), S68 (3), S62 (4), S75 (8). Not filed (10): the rest held, regression.

**2026-11-16: scenario test of build 20261116-124830 (S49, S74, S67, S76, S77, S78)**

Build `bin\0.1.0-20261116-124830` (`serverVersion` `0.1.0-sw2023-20261116-124830+ad5f6ba`, tag
`build-20261116-124830`), SOLIDWORKS 2023 SP3, started by the session (`sw_connect launch`).

Setup for 3–6: a 20 mm cube part (origin at a corner), saved and closed; a subassembly of it
twice (one fixed at the origin, one floating at [0, 15, 30]); a top assembly with the
subassembly fixed at z 100, the cube fixed at [50, 40, 0] and the cube floating at the top
level, so `<cube>-2` exists both at the top level and as `<sub>-1/<cube>-2`.

1. S67, held. A 100 × 60 × 20 block: `feature_rollback {to:"Boss-Extrude1", before:true}` → `rolledBack: ["Sketch1", "Boss-Extrude1"]`, −120 000, volume 0, bodies 0; `end` restores. The same after `feature_rename` to `Plate`. A 10 × 10 × 5 boss on top, `before` it: `["Sketch2", "Boss-Extrude2"]`, −500, the block's extrude active; `end` +500.
2. S74, held, a part's dimension and a mate's. `"H" = 20mm` and `D1@Plate = "H"` in one `equation_set`; `dimension_set D1@Plate 25`: `INVALID_ARGUMENT: 'D1@Plate' is driven by the equation "D1@Plate" = "H", which uses "H"; …` with the hint naming `equation_set` lhs `H` or `D1@Plate`. In an assembly of two cubes (parallel top planes, angle mate 30° on the front planes), `"TILT" = 40`, `D1@Angle1 = "TILT"`, `dimension_set D1@Angle1 50`: the same refusal naming `"TILT"`; the angle still 40 (`asm_mate_list`). Following the hint, `equation_set TILT 50deg`: the mate's angle read 50.
3. S78, held, all three. (b) `asm_mate_add` coincident `{component:"<cube>-2", type:"origin"}` with `{component:"<sub>-1/<cube>-2", type:"origin"}`, no name: accepted, both resolved "origin 'Origin' of …". (c) The rows, in the result and in `asm_mate_list`: `kind: "origin"`, `name: "Origin"`, `pointMm`. (a) With the top-level `<cube>-2` held on the nested one by that and two plane mates, the subassembly flexible, the nested cube's `Top Plane` coincident with the fixed cube's: `moved: ["<cube>-2", "<sub>-1/<cube>-2"]`, the nested cube's sibling not listed. The same for the two mates after it.
4. S77, held. In that mate's row the nested cube reads `fixed: false`, `constrained: "under"`; after its `Front Plane` and `Right Plane` too, `fixed: false`, `"fully"`, at the fixed cube's origin; `asm_component_list allLevels` the same, its fixed sibling `fixed: true`. While rigid (before), the nested floating cube read `fixed: true, "fully"`, as decided.
5. S76, held. `asm_component_set` the subassembly `flexible:false`: "flexible → rigid" with the warning "The change put 3 mate(s) in error: 'Coincident4' (error 51, mate not satisfied), 'Coincident5' (…), 'Coincident6' (…). They no longer hold; set it back, or edit or delete them (asm_mate_list)."; `asm_mate_list` `inError: 3`, `errorName: "mate not satisfied"`. `flexible:true` again: no warning, `inError: 0`.
6. S49, held. In the top assembly, the cube's own document closed and never listed: `dimension_set D1@Boss-Extrude1@<cube>-1` 20 → 25: the row named as given, `volumeDeltaMm3` 8000 (four instances), "Also changed, not saved: '<cube>.SLDPRT' (D5, owned), a component's own document."; `doc_list` gives D5, owned, dirty. In a hand-mated assembly opened from file, the shaft's length 60 → 80 → 60 through it: "'<shaft>.SLDPRT' (D10, foreign)", ±6283.185, mates without error; closed with `discard`, the three files byte for byte unchanged.
7. Bears on S52, reported `ok`: with the subassembly flexible and its child moved by top-level mates from [0, 15, 130] to [50, 40, 0], the subassembly's row (`asm_component_list`, `asm_component_set`) kept `box` [0, 0, 100]–[20, 35, 150], `approximate: true`: the box of its own file's placement, which no longer holds the child at all. "Approximate" reads as loose, not as a box of another state.
8. `doc_undo` (1 step) right after an `equation_set` (a variable and the extrude's depth linked to it, no geometry change) in a part where the last feature was a 10 × 10 × 5 boss: the result `volumeDeltaMm3: -500`, the boss gone, the equation kept (a `dimension_set` on the depth was still refused naming it). SOLIDWORKS undid the step before the server's last change, which itself is not an undo step. The result named no feature removed (the description: "Returns the features removed or restored") and said nothing of the `equation_set` left in place. Reported `ok`. Bears on S79 (`doc_undo`).
9. Held, regression: `feature_chamfer` `equal` 2 mm on a Ø20 bore's bottom edge −134.041, legs measured, no warning; a 1:2 sheet's frame [20, 10, 410, 287] / [230, 10, 410, 65.25]; the shelled tray's two sections (1 and 3 hatched faces); interference 0 in a saved hinge assembly. STEP of the shelled tray still 16 surface bodies (S5, met again).

90 calls, 45.6k characters of results; 4 errors (3 refusals meant, 1 feature name mistyped by the session).

Sorted into S52 (7), part (c); S79 (8), part (b); S5 (9), met again. Held: S67 (1), S74 (2),
S78 (3), S77 (4), S76 (5), S49 (6). Not filed (9): the rest of it held, regression.

**2026-11-16: an edit of two mirrored printed parts and their assembly: features replaced in place, new ribs on end faces, mates moved, interference over a tilt range**

Server build `0.1.0-sw2023-20261116-124830+ad5f6ba`.

1. **`sketch_add` made relations to model edges and changed the size drawn, reported ok.** A rectangle drawn on a planar face with corners [[100,40],[-95,43]] (its bottom on the face's boundary edge) came back with `relations: ["coincident: a model entity, Line1 start", "midpoint: Line3 end, a model edge"]`, 2.5 tall where 3 was drawn, and the sketch fully defined with no dimension. No warning that the solved shape differed from the input. The first `sketch_constrain` dimension was then refused as over-defining. The tool's own description says inference snapping is off while the server draws. Workaround: `sketch_delete` of the four lines, the rectangle redrawn clear of every model edge (Y 54..57) and moved into place by its dimensions in one `sketch_constrain`: 3 calls more. The same pattern drawn clear of edges held five more times (triangles on end faces, a rectangle on the other part).
2. **`equation_set` and `doc_save` on an assembly warned "'…SLDASM' was modified outside this server since its last change (by the user?)"** on every call after `doc_activate {allowEdit:true}`, when the only changes were the server's own: its parts edited and saved earlier in the session, and the assembly dirty from their rebuild when adopted. The owner was not working in SOLIDWORKS. A false alarm that, read literally, would stop a session from saving under the "never while the owner is working" rule. Bears on S60.
3. **Held, worth knowing:** `feature_delete {deleteAbsorbed, deleteEquations}` removed a flange, a pattern and the mirror of that pattern in one call, with their five equations: no rebuild error 51, the volume as figured to the mm³. `feature_rollback` to after a feature, a new sketch and extrude inserted there, and back to the end, held three times a part. `feature_mirror` of a rib about a reference plane: volume and instance faces checked.

Sorted into S80 (1), new; S81 (2), new, S57 again. Held: S54 (3), closed.

**2026-11-16: the same edit, a second round: sketches reshaped in place, a pattern and mirror remade, an edge chamfer driven by equation**

Server build `0.1.0-sw2023-20261116-124830+ad5f6ba`.

1. **`feature_chamfer {type:"equal"}` returns two dimensions (`D1@…`, `D3@…`) that are independent once one is driven**: an equation on D1 alone left D3 at its old value when the driving variable changed, a chamfer of 6 x 5 where 6 x 6 was meant; no warning, `equation_set` ok. Found only by a hand-figured volume (+24 mm³). Workaround: drive both. The result could say that an equal chamfer's two legs are separate dimensions.
2. **Held:** `sketch_edit` + `sketch_delete` of one line + `sketch_add` of two lines with exact end points (merged onto the existing ones, reported) + two relations reshaped a fully defined triangle into a rectangle keeping its dimensions and equations, twice a part. `feature_pattern_linear` reported faces merged by an instance flush with an existing face ("0.612 x 1 seed copies: instances overlap, merge or are clipped"), which was the intent.

Sorted into S64 (1), part (i). Held: S56 (2), closed.

**2026-11-16: layout sketches added to an existing part, driven by global variables, no solid changed**

Server build `0.1.0-sw2023-20261116-124830+ad5f6ba`.

1. **Held: `sketch_fillet` on a corner whose sharp point carried two dimensions** (a horizontal and a vertical from the origin, and a horizontal to another point) kept all three on the virtual sharp; the sketch stayed fully defined and the dimensions were then driven by equation and moved correctly at a second set of values. Worth knowing: dimension the sharp corner first, fillet after.
2. **Held: `sketch_constrain` angular `textAt` in the supplementary sector** was reported plainly ("textAt lies in the supplementary angle, so the dimension is on the angle that was 45°…") and the value read back right; no retry needed.
3. **Held: `equation_set` with `equations`**, 11 new globals one of them derived from two others, then 8 and 9 dimension equations, one call each, `volumeDeltaMm3: 0` as expected for sketches only.
4. **Held: `feature_delete {deleteEquations:true}` on a sketch with 9 driven dimensions** removed the sketch and all 9 equations in one call, listed in the result, no rebuild error after. The same call warned "'<doc>' was modified outside this server since its last change (by the user?)" on a document the user had only been viewing; harmless here, but it gave no hint of what changed.

Sorted into S81 (4), new. Held: S6 (2), closed; S37 (3), closed. Not filed (1): held, a usage lesson (dimension the sharp corner first, fillet after).

**2026-11-16: a part edited by rollback (a cut and a loft cut before a later cut, a boss from offsets, one mirror), three new parts from side and section sketches, one part copied and its outline driven, an assembly with a component replaced and two parts mated**

Server build `0.1.0-sw2023-20261116-124830+ad5f6ba`.

1. **`sketch_constrain` angular between two lines that meet only virtually, far away (a 3 mm line at 20° and a long horizontal line 55 above it), `valueDeg` 60 meant as the other line's slope: the line flipped to 120°, the trapezoid's corners collapsed onto the axis (126 wide became 6), reported `ok`** with "textAt lies in the supplementary angle, so the dimension is on the angle that was 119.99°… then 60°" and "geometry measures 60°". The geometry did measure 60 on the dimension's own angle, so the check passed a shape turned inside out; only the `changed` list (a line from 126 to 6 long) showed it. Workaround: finish the sketch and `dimension_set` the same dimension to 120, which flipped it back. The result could flag a `changed` length that drops by more than half, or refuse a flip when `textAt` names one sector and the value moves the line to the other.
2. **The same tool, angular 45 at a sketch point made the intersection of two lines (a lead-in's virtual sharp), drawn at 135° between the rays: it took the angle between the rays, mirrored three segments, and said so plainly in a warning** (held as a warning). The repair by `dimension_set` 135 after `sketch_finish` then left a perpendicular 3 mm segment solved to the far side, so the closed profile crossed itself (`contours.problem: "EntXEnt"`); `dimension_set` reported `ok`, volume 0, since no feature used the sketch yet. Workaround: delete the sketch and draw again with the angle dimensioned first, at the value between the rays as drawn (135), before any distance; held. Lesson: an angle at a virtual intersection goes in before the distances, as the warning's own text says.
3. **`sketch_add` of a 3-point arc whose centre falls exactly on an existing corner merged the centre onto it** (reported in `merged`), which made the arc's radius equal to a plate thickness and left the radius dimension redundant. Workaround: a mid point 0.02 off, so the centre lands near, then tangents and a radius. Worth a line in the tool's description: a 3-point arc's centre is merged like its ends.
4. **`sketch_add` of two `rectangle`s with corners on edges of the body projected into the sketch plane added relations to model edges** ("coincident: a model edge", "midpoint: Line8 end, a model edge") and **moved one rectangle: 10 wide drawn, 9 wide after**, a corner pulled to a model edge's midpoint. Reported in `relations`, not as a warning. Workaround: draw 1 off every model edge and place by dimensions. The result could warn when an auto relation moves a point the caller gave.
5. **`asm_mate_add` distance between two parallel planes, `alignment: aligned`, 80: landed on the far side (origin at -80, the part inserted at +80)**, with the warning that says so; again with `flip: true` it landed right (origin read back +80) but warned in the same words ("80 mm along its normal before, -80 mm after… If it should stay on the side it was on, delete the mate…"), which reads as a second failure. The second warning could say the side now differs from where it was inserted, not that it is wrong. The side check on `originMm` held.
6. **`feature_mirror` of a cut, a loft cut and a boss about the symmetry plane warned "the copy's faces have 0.86 x the seed's area"** while the volume was exactly twice the seeds' net (-2 627.2 = 2 x -1 313.6) and 14/14 instance faces were found. Here the loft cut's seed also cut a plate that a later feature removes, and faces merged differently; no clash. Lesson for callers: the area ratio alone is not a clash when the volume and the instance faces both match.
7. **Held: `feature_rollback` to insert before a cut, then `equation_set` on the cut's sketch dimension after rolling to the end**: the step moved and the volume change matched the hand figure to the mm³ (+2 887.2).
8. **Held: `asm_replace` of a part mated by two faces and a plane**: three mates reattached to the new file, none in error; the new part sat where the old did.
9. **S52, met again in part:** `body_list` gave a box max of 149.016816 on a part whose `equation_set` and `model_mass` boxes said 149.016716, same body, no change between.

Sorted into S82 (1, 2), new, S42 again; S80 (3, 4), new; S83 (5), new; S84 (6), new; S52 (9), met again. Not filed (7, 8): held.

**2026-11-16: an edit to four printed parts and their assembly: features deleted and mirrored again, slots, pilots, a pattern, a second set**

Build `0.1.0-sw2023-20261116-124830+ad5f6ba`.

1. **`sketch_add` slot made 2 lines and 2 arcs with no relations** (`sketch_info`: `"relations":0`), where SOLIDWORKS' own slot tool ties them. Two slots took 8 `tangent`, 4 `vertical` and 1 `equal` in `sketch_constrain` before they could be dimensioned, and the first call, with 2 `vertical` short, left the sketch under-defined (24.8 s). Missing: a slot with its tangencies and its centreline, so it is defined by its two centres and a width.
2. **`feature_pattern_linear` warned "'LPattern1' has rebuild warning 51 (swSketchErrorExtRefFail)"**, naming the feature by the name it had before the call's own `name` renamed it; the next `model_review` was clean, no warning. Either the warning is transient and could be dropped after the rename's rebuild, or it should name the renamed feature.
3. **Held: `equation_set` that left a sketch self-intersecting reported it plainly**: "the equations went in, but 2 feature(s) now fail to rebuild … error 51 swSketchErrorExtRefFail", the volume "the model without them", and `sketch_info` gave the contour `"invalid"`, `"problem":"EntXEnt"`, which named the cause.
4. **Held: `feature_delete` dry run** listed the dependent mirror and said the real call would refuse; with `deleteChildren`, `deleteAbsorbed` and `deleteEquations` it took the features, the mirror and the four equations, and `feature_mirror` made the mirror again over the rest.
5. **`feature_mirror` warned "the copy's faces have 1.001 x the seed's area"** on a boss and a hole mirrored about the symmetry plane, the instance faces 7/7 and the volume as figured; no clash (the earlier entry's point 6, met again).
6. **`sketch_add` polygon: the contour read `"closed"` with `"regions":0`**, and the cut from it worked (volume to the hand figure). The regions count looks wrong for a closed hexagon.
7. **Carried from earlier the same day, in that session's own words, not seen here:** "textAt pada dimensi angular membalik garis ke sudut suplemen", an angular dimension's `textAt` turning a line to the supplementary angle; the earlier entry's points 1 and 2, met again.

Sorted into S65 (1), part (f); S84 (2, 5), new; S64 (6), part (l); S82 (7), new. Held: S54 (4), closed. Not filed (3): held.

**2026-11-16: an edit to three printed parts: a loft cut and its offset plane removed, a cut extended or made again from the loft's start sketch, a mirror and four chamfers made again, second sets**

Server build `0.1.0-sw2023-20261116-124830+ad5f6ba`.

1. **`feature_chamfer` `angle_distance` 1 / 60° on two edges where a groove's side meets a flat wall face, both unflipped and then both `flip: true`: `VERIFY_FAILED` "no chamfer face with legs 1/1.732051 or 0.57735/1 mm along edge E25 (found legs 1/1.085649 mm)", Hint "A distance larger than the adjacent faces fails; use smaller distances or fewer edges".** The cause was the side: one edge needed the flip and the other did not, and on the wrong side the 1.73 leg ran past the 1-deep side face, so the chamfer was cut short. The hint pointed at the distances, not the side; and the error named only the edge that failed, not that the other one held. Workaround: `flipEdges: [0]` on the pair, which held on all four grooves, `-16.0` each as figured (S51 bears on `edit` only; the new chamfer's `flipEdges` held). The hint could say, when the found legs share the requested distance and the other leg is short, that the angle's side may be swapped (`flip` / `flipEdges`), and name each edge's result.
2. **`feature_chamfer`'s leg check, reported `ok`, gave both faces of each edge the same plane**: "edge E24: 1 mm along F252 (plane [-0.866, 0, -0.5]), 1.732051 mm along F253 (plane [-0.866, 0, -0.5])". One of the two is a face square to the axis, normal ±Y; only the wall's normal was right. The volume matched to the mm³, so the chamfer was right and the report's normals are not.
3. **`equation_set` on an offset plane's distance to 0 was refused and rolled back**, plainly ("a value the dimension can't have, such as 0?"). So a cut sketched on an offset plane cannot be brought down onto its base plane: the cut had to be made again from a sketch on the base plane, which took its children (a mirror and seven chamfers) with it, ~14 calls. Lesson for callers: a feature that may one day start from the base plane is sketched on it, with a blind depth, not on an offset plane.
4. **`feature_delete` dry run said `keptAbsorbed: ["…start sketch", "…run sketch<2>"]`; the real call removed the `<2>` one** and said so: "SOLIDWORKS also removed '…<2>' (absorbed by …), which was not planned." The `<2>` is the loft's reference to a sketch another feature owns; the dry run could list it as going.
5. **Held: `feature_delete` with `deleteChildren`, `deleteEquations`**, dry run first, took a loft cut, its plane, two cuts' mirror and seven chamfers with their equations, nothing left for error 51; `feature_rollback`, a cut from the kept sketch, the mirror and the chamfers made again, every volume to the hand figure (-20 529.0 against 20 528, -16.0 each).
6. **Held: the face count at the second set** (`body_list`): 242, 19 and 94 faces at both sets on the three parts, the loft's faces gone as figured.
7. **`feature_mirror` area ratio again (0.996, 1.006)** with the volume exactly the seed's and the instance faces all found: no clash, as the earlier entries' point 6.

Sorted into S64 (1), part (j); S64 (2), part (k); S85 (4), new; S84 (7), new. Held: S54 (5), closed. Not filed (3, 6): SOLIDWORKS' own limit, refused plainly with its cause, a lesson for callers (3); held (6).

**2026-11-16: an edit to two printed parts: a chamfer deleted and made again earlier in the tree by rollback, a sketch line deleted, second sets**

Server build `0.1.0-sw2023-20261116-124830+ad5f6ba`.

1. **`feature_chamfer` `distance_distance` 9.5 / 9.4 on three edges (a cylinder's and two planes' concave edges against one flat face): the leg check named two different faces per edge, with their right normals** ("9.5 mm along F284 (plane [0, 1, 0]), 9.4 mm along F285 (cylinder r 34.2)"). The earlier entry's point 2, both faces given the same plane, did not recur here; it may belong to `angle_distance` or to plane-plane edges.
2. **Held**: `feature_delete` with `deleteEquations` (dry run first), `feature_rollback` to a feature and back to the end, `equation_delete` then `sketch_delete` of the line its dimension sat on, a `coincident` of two loose ends solved to the right one of two intersections, and the face count at the second set (`body_list`), every volume near the hand figure.
3. **`sketch_delete` of a polygon's six lines also removed its construction circle**, which carried the polygon's centre dimensions, its diameter and an `equal` to the other polygon's circle: `"removed":["Arc1","Line1",…,"Line6"]` while the checks said "6 segment(s) gone". The other polygon was left with no size (under-defined). The result does list it; a warning that a segment not asked for went, with the dimensions and relations that went with it, would have saved a `sketch_info`. The new dimensions took the freed names (`D1`, `D2`, `D3`), so the old equations on `D2`/`D3` bound to them again; right here by luck of order.

Sorted into S64 (1), part (k); S86 (3), new. Not filed (2): held.

**2026-11-16: scenario test of build 20261116-205845 (S59 (c), S65 (e), S65 (c), S65 (b), S65 (d))**

Build `bin\0.1.0-20261116-205845` (`serverVersion` `0.1.0-sw2023-20261116-205845+ba2ce83`, tag
`build-20261116-205845`), SOLIDWORKS 2023 SP3, started by the session (`sw_connect launch`), then
started again by hand (point 6).

1. S59 (c), held. A line on the Top plane in a finished sketch: `ref_plane` `angle` 30° on `top`
   with `axis: {name:"Line1", sketch:"Sketch1"}`, no `type`: resolved "sketch segment Line1",
   checked "contains sketch segment Line1, 30° to plane 'Top Plane'". `ref_axis` `two_points`
   with `entities: [{at:[-20, 0], sketch:"Sketch1"}, {at:[20, 0], sketch:"Sketch1"}]`, no `type`:
   two sketch points, the axis along [-20, 0, 0] → [20, 0, 0].
2. S65 (e), held. An open polyline typed to 4 decimals ([32.5, 14.3301] … [32.5, 5.6699]) and an
   arc from its centre [30, 10], r 5, −60° to 60° on its ends, in one call: `endsMerged: 2`, 1
   closed contour; the same arc in a later call onto an older polyline: 2 merged; an arc drawn
   0.2 mm off (centre x + 0.2): not merged, the contour still open. Small, reported `ok`: the
   second merge read "0.000054 mm apart" where the drawn gap was 0.000027: the first merge had
   moved the arc (its centre to y 10.000027), and the second then moved the line's end 0.000054
   from where it was typed (`sketch_info`: the polyline's top line at y 14.330154, typed 14.3301).
   Within the 1e-4 tolerance, so nothing wrong in the geometry; the reported distance is the one
   after the earlier merge, not the gap drawn, and one end moved twice the gap.
3. S65 (c), held. A circle r 5 at [20, 10], `sketch_offset` `both` 2 (two offset circles, own
   centres), three lines from [20, 10] (`sketch_add` merged their starts onto the inner offset's
   centre, listed in `merged`), the circle fixed. `sketch_relation_add vertical` `{at:[20, 10]}`
   with the origin: `VERIFY_FAILED: 'vertical' on sketch point at [20, 10] (Arc1 centre; 2 other
   points there), origin 'Origin' conflicts …` (a true conflict: the fixed centre is at x 20).
   `horizontal` `{at:[20, 10]}` with a line's end: accepted, `resolved` label with the owner and
   the count, the warning listing "Arc2 centre; Arc3 centre, Line1 start, Line2 start, Line3
   start" and that no reference picks them; the same warning from a `sketch_constrain` step; the
   lone point in the same calls unwarned.
4. S65 (b), held. A text "UJI 7", 6 mm, at [12, −8] on the Right plane (a sketch frame not the
   model's): `sketch_info` `atMm: [12, -8]`, `heightMm: 6`, `angleDeg: 0`, open and after finish.
5. S65 (d), held as designed, and a gap. A 40 mm block, a plane 300 mm out on `right` and an axis
   of it with `top`: the default screenshot fills the image with the block; with
   `hidePlanes:false, hideAxes:false` the plane is in the fit (the block small, as asked). Then an
   axis by `ref_axis two_points` at z 300: the default screenshot shows the block at about a tenth
   of the image beside two near-invisible dots (the axis's 3D sketch points); with
   `hideSketches:true` it fills the image. Reported `ok`, with nothing in the result saying the
   fit took in a sketch that reaches past the bodies, which the server checks for. A caller who
   made the axis by points and wants the part gets a small one, as in the finding that opened
   (d). Proposed: a warning in that case naming the sketch and `hideSketches`, or the 3D sketch
   `ref_axis` makes for its points hidden when made.
6. Bears on S60 (b), met again. SOLIDWORKS started by `sw_connect {launch:true}` runs as
   `SLDWORKS.exe -Embedding` (parent: the DCOM launcher). After the session's client was
   restarted, the new server process got `NOT_CONNECTED: SOLIDWORKS is running (pid …) but can't
   be reached over COM: it may still be starting, or it runs with different privileges …` on
   every call, `sw_connect` too, with SOLIDWORKS responding, its window up, the old server
   process gone and none listed, same user and level. Likely: a SOLIDWORKS started through COM
   is not in the running object table, so only the process that started it holds it. Workaround:
   SOLIDWORKS stopped (a throwaway part's unsaved changes lost) and started from its exe, then
   `sw_connect` attached at once. The hint named neither cause; it could say SOLIDWORKS was
   started by another server process with `-Embedding`, and `launch` could start the exe so any
   later process attaches.
7. Held, regression: the shelled tray rebuilt from the procedure, every volume exact (1 617 000 →
   201 850.83 → 160 562.414 → 174 928.873 → −318.086 → 174 346.977, the 6.5 c'sink kept after a
   force rebuild), PETG 221.42 g, a dimension-linked property `3,00`, the design-intent round
   clean with 1 body; `feature_chamfer` `equal` 2 mm on a Ø20 bore's edge −134.041; a hand-mated
   assembly's two mates without error and 127 402.659 → +6283.185 → back; interference 0 in a
   saved hinge assembly; a 1:2 sheet's frame and the tray's two sections (1 and 3 hatched faces).
   STEP of the shelled tray still 16 surface bodies (S5, met again).

106 calls, 46.8k characters of results; 7 errors (6 `NOT_CONNECTED`: 3 before SOLIDWORKS was
started, 3 after the restart of point 6; 1 refusal meant).

Sorted into S87 (2), new, S65 (e) held; S88 (5), new, S65 (d) held; S89 (6), new, S60 again; S5 (7), met again. Held: S59 (1), closed; S65 (3, 4), parts (c) and (b), closed.

**2026-11-17: scenario test of build 20261117-012659 (S52 (c), S61 (a), S72, S71, S87, S88)**

Build `bin\0.1.0-20261117-012659` (`serverVersion` `0.1.0-sw2023-20261117-012659+098a10b`, tag
`build-20261117-012659`), SOLIDWORKS 2023 SP3 started from its exe by the session (not by `launch`,
S89), `sw_connect` attached at once.

1. S61 (a), held for a STEP inserted as an assembly's first component; **not held when another
   component is already in the assembly**, reported `ok`. `asm_insert` of a STEP of a 20 mm cube:
   every time one component, "1 solid body (8000 mm³)", "linked … by 3D Interconnect", the part
   named `<step folder>\<name>.step.SLDPRT` and "not written until the assembly is saved". Five
   cases, each with a fresh copy of the STEP in its own folder:
   (a) first component of a new assembly (fixed by SOLIDWORKS), `doc_save as` in the STEP's
   folder: part written there; (b) the same with the assembly saved one folder up: written beside
   the STEP; (c) a part inserted first, then the STEP (floating), `doc_save as`: **not written**;
   (d) the STEP inserted into an assembly already holding a subassembly and a part, `doc_save as`
   then `doc_save` in place: **not written**; (e) the STEP inserted, after a subassembly, into the
   saved assembly of (b), `doc_save` in place: **not written**. In (c) to (e) the part stays open,
   `dirty: true`, listed `foreign` (so `doc_save` cannot save it either), and nothing in any result
   says so; `doc_save` reports the assembly saved. Closing the assembly closed the part unsaved
   (warning "SOLIDWORKS also closed: …step.SLDPRT" only). Reopened, (c) reads `missingReferences`
   `<assembly folder>\<name>.step.sldprt` (the assembly's folder, not the STEP's) and the component
   `suppression: suppressed`: the STEP's component is lost from the saved assembly. The live test
   covers the first-component case only. Also seen: the assemblies of (d) and (e) read `dirty:
   true` again after their in-place save, with no change made, once other documents had been
   activated. Proposed: after the assembly's save, check that each such part was written, and save
   it (the component's document) when not, or say in the save's result that it was not.
2. S52 (c), held. A subassembly of two 20 mm cubes (one fixed, one floating) at [0, 0, 100], made
   flexible, its floating child mated by Top/Front/Right planes onto a top-level cube at [50, 40,
   0]: `asm_component_list` row of the subassembly [0, 0, 0] .. [70, 60, 120], not `approximate`
   (the union of its children as placed); rigid at insert [0, 0, 100] .. [20, 35, 150] exact; made
   rigid again (`asm_component_set`) [0, 0, 100] .. [20, 35, 150] with the three mates named in
   error (S76 held), flexible again [0, 0, 0] .. [70, 60, 120].
3. S88, held. A 40 mm block and an axis by `ref_axis two_points` at z 300: the default screenshot
   warns "The fit takes in 3DSketch1, shown past the bodies, which leaves them small in the image;
   hideSketches:true leaves sketches out of the image and the fit."; with `hideSketches:true` the
   block fills the image and no warning.
4. S71, held. `sim_run frequency` on the same 40 mm AISI 1020 block, 8 modes, no fixtures: check
   "solver FFEPlus for the free body (read back)", solver status 0, six modes 0 .. 0.0021 Hz, then
   35 490.8 Hz, 13.5 s; with the bottom face fixed, 4 modes from 13 380 Hz and no solver check.
5. S87, held. The polyline typed to 4 decimals and the arc centre [30, 10] r 5, −60° to 60°, in one
   call: both merges read "0.000027 mm apart" (the gap as drawn); `sketch_info` gives the polyline's
   ends exactly as typed (y 14.3301 and 5.6699) and the arc's centre at x 29.999953, radius 5 (the
   arc took the gap). The same arc onto an older polyline in a later call: 0.000027 twice; the arc
   0.2 mm off not merged.
6. S72, held as far as can be tried: `material_set` AISI 1020 "density 7900 kg/m³ matches the
   database", PETG from the custom library 1270 the same, no warning. The refusal (a process that
   applies 1 kg/m³) was not met, so not tried.
7. Small, met in three scenario tests now: `drw_view_section` with no `atMm` places Section B-B of
   a 1:2 plan so that its own view label overlaps the title block by about 0.2 mm, and warns of it
   ("view label … overlaps the title block"). The warning is right; the free-space placement could
   count the label it is about to make.
8. Held, regression: the shelled tray rebuilt from the procedure, every volume exact (1 617 000 →
   201 850.83 → 160 562.414 → 174 928.873 → −318.086 → 174 346.977, the 6.5 c'sink kept after a
   force rebuild), PETG 221.42 g, a dimension-linked property `3,00`, the design-intent round clean
   with 1 body; both sketches `fully` in one `sketch_constrain` (11.2 s / 7.8 s);
   `feature_chamfer` `equal` 2 mm on a Ø20 bore's edge −134.041; a hand-mated assembly's two mates
   without error, 127 402.659 → +6283.185 → back, the shaft's box Ø20 × 60 tight; interference 0
   in a saved hinge assembly; a 1:2 sheet and the tray's two sections (1 and 3 hatched faces). STEP
   of the shelled tray still 16 surface bodies (S5, met again).

128 calls, 73.0k characters of results; 1 error (`doc_close` `DIRTY` on an assembly that had
read clean after its save, point 1).

Sorted into S61 (1), part (a) failed, reported `ok` while wrong; S90 (7), new; S5 (8), met again, the rest of the regression held. Held: S52 (2), part (c), closed (S76 held again there); S88 (3), S71 (4), S87 (5) and S72 (6), closed.

**2026-11-17: an edit to three parts and their assembly: tabs and holes square to a tilted face, a handle, a limit mate in a second configuration**

Build `0.1.0-sw2023-20261117-012659`.

1. S58 (b)'s state met again, in a session's first writes: `equation_set` of six globals failed with "SOLIDWORKS rejected the equation ... though the equation is valid ... restart", right. The next call, three globals that already existed, failed with `VERIFY_FAILED`: "SOLIDWORKS kept the previous equation "<var>"= 100mm and did not take 73.5mm (a value the dimension can't have, such as 0?)", and the hint "Check the expression (units, quoted names, existing variables)": wrong, the same state. A new empty part's `equation_set` then gave the restart message. Proposed: the `VERIFY_FAILED` path tries one equation on a scratch part, or names the restart, before blaming the expression. Not reported `ok`.
2. In that state, before any write, `equation_list` reported wrong values with no warning: a global `= 60.4mm` read `"value":60`, `3.4mm` as 3, `2.5mm` as 2, and `"value":0` for every expression row. After a restart the same model reads 60.4, and the expressions their values. Reported `ok` while wrong. Proposed: `equation_list` warns when a global set to a number reads back another value, which also detects the state before the first write.
3. No tool exits SOLIDWORKS: the restart was `Stop-Process` from the shell, every document saved first, then `sw_connect launch:true`, which reported `"process":"a new one (before: pid 24492, ...)"` (S60 (c) held). Missing: an exit that refuses with a dirty document open.
4. S84, met again: `feature_mirror` of three features about the Right Plane warned "the copy's faces have 1.004 x the seed's area: it meets the body differently from the seed", on a mirror whose volume closed exactly against the hand figure (the seed side's volume and its two refilled holes, to 0.01 mm³).
5. Held: `feature_revolve` with `axis:{name:'Line1'}`, a profile side used as the axis, cut and boss, on a plane offset from the Top Plane, the faces checked coaxial with [0.866, 0, 0.5]; `asm_mate_add` distance with `minMm`/`maxMm` in a new configuration, read back "within the limits", created suppressed in the other configuration with no call; `feature_delete` `deleteChildren` with `deleteAbsorbed` and `deleteEquations`, its dry run naming the child first.
6. S81, met again: `doc_save` of an assembly warned "'…' was modified outside this server since its last change (by the user?)" after this server had saved two of its parts and nothing else had touched it.
7. Held: `model_measure` between two cylindrical faces of two components, picked by `at` with `tolMm` 0.3, gave `centerDistanceMm: 0` and the radial gap, which proved two holes made in two parts coaxial in one call.

Sorted into S92 (1, 2, 3), new; S84 (4), met again; S81 (6), met again. Not filed (5, 7): held, no item.

**2026-11-17: fillets and a trim cut on three parts of an assembly, edited in place**

Build `0.1.0-sw2023-20261117-012659+098a10b`.

1. S81, met again: `doc_save` of the assembly warned "'…' was modified outside this server since its last change (by the user?)" after this server had saved three of its parts and the user had not touched SOLIDWORKS; the assembly was dirty only from their rebuild.
2. Held: `doc_activate {allowEdit:true}` on a part loaded only by an open assembly (`access: foreign`) adopted it with a backup, and `feature_delete` with `deleteEquations` then `feature_fillet` remade a fillet at the end of its tree under the old name; `face_list {near}` read the small face it had left open before and its absence after.
3. `feature_extrude` of a boss whose end lands coplanar with an existing face, so the cap merges into it, fails `VERIFY_FAILED: … no cap face at 12.2 mm along [0, -1, 0]` and is rolled back, though the boss is right. Worked round with `keepOnFailure: true` and `model_mass` before and after (+1 280.58, as by hand less what was solid already); two calls lost. Proposed: when no cap face is found, accept a face of the merged body lying in the cap's plane, or report the volume and say the cap merged.

Sorted into S81 (1), met again; S64 (3), part (m). Not filed (2): held, no item.

**2026-11-17: a part's two plates cut back by redrawing one sketch and shortening another, a fillet at the end of the tree remade**

Build `0.1.0-sw2023-20261117-012659+098a10b`.

1. A rebuild warning on a feature far from the change was carried by no write's result. `dimension_set` of one sketch dimension, then a sketch redrawn (`equation_delete` of its one equation, `sketch_edit`, `sketch_delete` of all five segments, `sketch_add` of a closed polyline, `sketch_constrain`, `sketch_finish`) and `equation_set` binding the new dimension to the old global: none of these results named that a fillet at the end of the tree now had rebuild warning 13 (`swFeatureErrorFilletNoEdge`), one of its four edges lost; `equation_set` read `"volumeDeltaMm3":0` and the model's volume, nothing else. Only `model_review` showed it. The hand figure caught it first: +37.6 mm³ over it, one R5 edge of 7 mm. Reported `ok`, the result incomplete, not wrong. Proposed: a write that rebuilds compares the tree's rebuild errors and warnings before and after, and names each feature that gained one. Workaround: `model_review` after every sketch redrawn, before the next step; the fillet `feature_delete`d and remade by `at` on its four edges, -72.728 as first made (3 calls).
2. Learned: a sketch redrawn reuses the dimension names `D1@`, `D2@`... in the order the new dimensions are added, so an old name can point at another size: `D5@<sketch>` was the 12.2 bound to a global before, and a 20 after. Deleting the equation before deleting the geometry held, with no rebuild error 51; the new binding went to the name `sketch_constrain` returned for that step.
3. Held: `sketch_add` polyline `closed:true` of 8 points gave 8 lines with merged ends and no relations, then one `sketch_constrain` of 8 relations and 8 dimensions to the origin gave `fully` in 17.8 s; `feature_fillet` by four `at` points within 0.0012 mm picked the right edges on both sides of a mirror; `doc_activate {allowEdit:true}` on an assembly open and saved by the user, then `doc_rebuild` and `model_review` read no mate error.
4. Point 1 met again in the same session: a second redraw of the same sketch dropped the same fillet edge, again named by no write's result (`sketch_finish`, `equation_set`), again only by `model_review`. The fillet references a mirrored body's edge far from the sketch; `feature_fillet` by `at` on the same four points resolved three of the four edges to the same ids as before and the dropped one to a new id each time (`E53`, then `E60`).

Sorted into S93 (1, 4), new; S86 (2), met again. Not filed (3): held, no item.

**2026-11-17: one corner of a multi-edge fillet raised to its own radius, and a sketch dimension change on a part opened by the user**

Build `0.1.0-sw2023-20261117-012659+098a10b`, SOLIDWORKS 2023 (31.3.0).

1. Missing tool: no way to change the edge set of an existing fillet. Two of a ten-edge constant fillet had to take a larger radius; it cost `feature_delete` (dry run, then real), `feature_rollback {before}` to the fillet's place, `feature_fillet` on the other eight, a new `feature_fillet` on the two, `feature_rollback {to:'end'}`: 6 calls where an edit of the fillet's edges (and a new one on the two) would be 2. Proposal: `feature_fillet {edit:"<name>", removeEdges, addEdges}` or a `feature_edges_set`.
2. Learned: `feature_fillet` by `at` on a point 0 mm from an edge picked that edge, as asked, but the point was the wrong edge (an 8 mm edge of a hex pocket beside the 6 mm corner meant). The call reported `ok` correctly. What caught it was `face_list {feature}` after: each new face's `axisOriginMm` and area set against the deleted fillet's faces, read before the delete (33.5 mm² against 69.9). Reading the old fillet's faces before a delete, and comparing axes after the remake, cost 2 calls and is the check that tells the edges apart; the volume alone could not, with no hand figure per edge.
3. Held: `dimension_set` of two sketch dimensions in one call moved a tilted strut's lower edge and left its upper edge to 0.0001 mm; `feature_rollback {before:true}` then a fillet inserted at the bar, then `{to:'end'}`, rebuilt every later feature with the final volume equal to the hand sum to 0.001 mm³, face count unchanged.

Sorted into S65 (1), part (g). Not filed (2, 3): the point given picked the edge asked, a lesson on reading the old faces before a delete (2); held (3).

**2026-11-17: a strip and its children deleted from a part, two horizontal holes cut, the holes' height to be driven by a new global**

Build `0.1.0-sw2023-20261117-012659+098a10b`, SOLIDWORKS 2023 (31.3.0).

1. `equation_set` refused every equation in a part where the session's earlier calls had held: `FEATURE_FAILED: SOLIDWORKS rejected the equation "<name>" = 6mm, though the equation is valid: a global variable set to a number. SOLIDWORKS can come to refuse every equation, in a new part too, until it is restarted.` A batch of six, then one global alone, then one dimension equation (`"D2@<sketch>" = "ExtH" - 6mm`) all failed. In the same part, `feature_delete {deleteEquations:true}` had just deleted seven equations with no error. The diagnosis in the message was clear and nothing was half-applied. No open item covers it; the fork's own record names it only in a live-test note (a valid equation refused, passing alone after a restart). Not restarted: the user had sixteen documents open, so the dimension was left a plain value. Proposed: an item for it, with what puts SOLIDWORKS into that state if it can be found (here the process had run a long session with many `equation_set` and `feature_delete {deleteEquations}` across parts), and whether a document close and reopen clears it short of a restart.
2. Held: `feature_delete {dryRun}` then real, six features with `deleteAbsorbed` and `deleteEquations`, listed exactly the nine features and seven equations it then removed; the volume change matched the hand figure to the 23 mm³ a fillet left behind took.
3. Held: one sketch on a standard plane with two circles, `sketch_constrain` of six steps to `fully`, then `feature_extrude {cut, end:"through_all_both"}` cut both side walls of a hollow part at once: -145.267 against 145.27 by hand, four cylinder faces.

Sorted into S92 (1), new. Not filed (2, 3): held, no item.

**2026-11-17: a part's hole positions driven by globals after a restart, a tapered hollow part of drafted extrudes and cuts, mated in an assembly**

Build `0.1.0-sw2023-20261117-012659+098a10b`, SOLIDWORKS 2023 (31.3.0).

1. The entry of 2026-11-17 ("a strip and its children deleted", point 1), met again and cleared: `equation_set` refused a batch of six (`FEATURE_FAILED: SOLIDWORKS rejected the equation "<name>" = 6mm, though the equation is valid …`) after the user said SOLIDWORKS had been restarted. It had not been since the failure: the process's start time (read outside the server) was 44 min before the first refused call, and `sw_status.recentOps` held those failed calls in the same process. Learned for the restart: `CloseMainWindow` on `SLDWORKS.exe`, with no document dirty, closed the window with no prompt but left the process running (no title, responding), most likely held by the COM references of the two `solidworks-mcp` processes `sw_status.otherServerPids` listed; ending the process, then `sw_connect {launch:true}`, gave `"process":"a new one (before: pid …)"`, and the same batch of six held at once, `volumeDeltaMm3: 0`. Proposed: `sw_status` names the process start time beside the PID, and a refusal of this kind names it, so "restarted?" is answered by reading, not by a shell; the refusal's hint may say that closing the window can leave the process alive under another server's references.
2. `feature_chamfer {type:"distance_distance", distanceMm:2, distance2Mm:3.464}` on six convex edges between a 3-wide face and a deep face, then the same with `flip:true`: both `VERIFY_FAILED` with the identical text "no chamfer face with legs 2/3.464 mm along edge E23 (found legs 2/3.011938 mm)", six edges each. `keepOnFailure` showed the chamfer face 30° from the narrow face, i.e. 3.464 on the 3-wide face both times: `flip` changed nothing. Swapping the values (`distanceMm:3.464, distance2Mm:2`, no flip) held, -2 081.668 against -2 081.73 by hand, each edge's legs named on its faces. Bears on S64 (j): there an `angle_distance` side was swapped per edge; here `flip` on a `distance_distance` had no effect at all, and the hint again pointed at the distances. Not reported as success.
3. `sketch_offset {segments:[Line1], chain:true, side:"inward"}` on a loop drawn by one `sketch_add` polyline `closed:true, construction:true` (6 lines): `INVALID_ARGUMENT: The segments form an open chain, which has no inside`. Passing all six segments without `chain` worked, and the offset lines came out regular, not construction (what was wanted). Either `chain` does not follow construction segments or the closing joint of a closed polyline is not merged for it.
4. Held: `feature_extrude {draftDeg:30, draftOutward:true}` on a 6-line outline converted from a face, boss and cut, volumes to 0.02 mm³ of hand figures; `feature_extrude {cut, sketch:"<used sketch>"}` reusing the sketch of a previous cut in the other direction; `sketch_start` on a cut's ceiling face (normal −Y) with `sketch_convert` of its face; one `equation_set` batch of 20 globals in a new part; `asm_insert` with `rotation` matching another component's transform read from `asm_component_list`, then three plane mates, fully defined; `asm_interference {components, includeCoincident}` naming the one touching pair.
5. Learned, the same session: `sw_connect {launch:true}` started SOLIDWORKS with no visible window (`MainWindowTitle` empty). The user, seeing none, started a second instance and opened an assembly there; the next `doc_open` of a part in the hidden instance came back `"readOnly":true` with `"the file is open elsewhere (sharing violation)"`, and `sw_status` listed only the hidden instance's documents, nothing of the other process. The warning was clear; what was missing was any sign that a second `SLDWORKS.exe` was running. Workaround: the hidden instance's documents closed (none dirty), its process ended, `sw_connect` attached to the user's (`"process":"a new one (before: pid …)"`), then `doc_activate {allowEdit:true}` on the user's word. Proposed: `sw_status` lists every `SLDWORKS.exe` with its start time and which one the server is attached to, and `sw_connect {launch:true}` shows the window (or says it is hidden).

Sorted into S92 (1), new; S64 (2, 3), parts (n) and (o); S89 (5), met again. Not filed (4): held, no item.

**2026-11-17: two assemblies re-linked after their parts were moved into a subfolder**

Build `0.1.0-sw2023-20261117-012659+098a10b`, SOLIDWORKS 2023 (31.3.0).

1. Missing tool (bears on S65): no call repoints an assembly's component references to a new folder. Workaround that held: `doc_open {readOnly:true}` of the assembly listed every missing path in `missingReferences` (16 of 20, then 4 of 28 for the parent); `doc_close`; `doc_open {readOnly:true}` of each moved part (16, then 4, one call each); the assembly opened again editable with no load warning, `model_review` 0 errors, `doc_save` in place, and `asm_component_list {allLevels:true}` read every path in the new folder. 24 opens for 20 parts; a `doc_relink {from, to}` or `doc_open {searchFolders}` would make it two calls.
2. Learned: an assembly opened read-only with missing references reads `dirty: true` at once, with no change made; `doc_close` then discards silently, as it says. Closing it also closed a reference opened with it (a warning names it), so a part needed later must be opened on its own.

Sorted into S65 (1), part (h). Not filed (2): SOLIDWORKS' own behaviour on a read-only open with missing references, and the warning said it.

**2026-11-17: a turned printed part with two swept internal threads (one tapered), a two-part assembly**

Build `0.1.0-sw2023-20261117-012659+098a10b`.

1. S58 (b)'s state, met again twice. (a) On a new empty part, the first `equation_set` (a batch of 37 global variables) failed `FEATURE_FAILED: SOLIDWORKS rejected the equation "<name>" = 59.614mm, though the equation is valid`; one alone failed the same. SOLIDWORKS closed until no process was left, `sw_connect {launch:true}` gave "a new one (before: pid …)", and the same batch passed. (b) About 30 minutes and some 80 calls later, after a `feature_delete {deleteAbsorbed, deleteEquations}` of a sweep, a `feature_rollback` to before the next feature, a new profile sketch and sweep inserted there, and that sketch renamed to the deleted one's name: every equation on that sketch's dimensions failed `FEATURE_FAILED` (a plain `30.47039mm` too, and after a second rename to a new name), `"Above" = 4mm` on an existing variable passed, and `"D1@<sketch>" = "ZWindow"` on another sketch, an equation it already had, failed `VERIFY_FAILED: … the equation evaluates to 0` and was rolled back. `equation_list` then read every value wrong: `59` for `= 59.614mm`, `0` for each variable defined from others. A save, a restart (new PID) and a reopen cleared it; the same four equations then passed. Proposal: `equation_list` and `equation_set` flag this state when a global variable set to a constant reads back another value, and name a restart then, before the next call is spent on syntax.
2. Learned, `feature_sweep` along a tapered `feature_helix`: the profile is carried without tilting with the taper, so a profile placed off the helix's start, above or below it, gives a thread whose depth is off by that height × tan(taper). A ridge profile hung about 1 mm below the start came out about 0.03 deep against the taper, 120 mm³ on one thread; drawn with each point's radius on the taper, it matched. Reported `ok` both ways, rightly: the geometry is SOLIDWORKS'. Proposal: one line in `feature_sweep`'s or `feature_helix`'s description ("a profile off the start of a tapered helix: draw its points on the taper").
3. `feature_sweep`, merged, same profile and helix: at the end of the tree it said "the start face is 2.103 mm², but the profile sketch encloses 2.113 mm²: the rest lies inside the existing body"; inserted before the rollback bar it said "(SOLIDWORKS may have used only some of its contours)" for the same geometry. The S64 (g) let-off does not seem to apply to a sweep inserted mid-tree. The result was right both times.
4. `feature_delete` of a sweep reported "'<revolve cut>' now has rebuild error 51 (swSketchErrorExtRefFail)" for the next feature, whose sketch is on a standard plane with the origin only; after the sweep was re-made before it, `model_review` read no error. Whether the error was real while it lasted is not known.
5. `body_list`'s `box` for a part with swept threads read Y -2.03 to 104.80 while `model.box` in every feature result read 0 to 102.89, the part's real extent (the sweeps were trimmed at both ends). It looks like the B-spline faces' control-point box. Read `model.box` for extents.

Sorted into S92 (1), new; S94 (2), new; S64 (3), part (p), S64 (g) again; S84 (4), met again; S52 (5), met again.

**2026-11-17: a turned printed part with a flange and a swept internal thread, rebuilt from a script**

Build `0.1.0-sw2023-20261117-012659+098a10b`.

1. S89, met again. SOLIDWORKS started by an earlier session's `sw_connect {launch:true}` (after a restart for S58 (b)) was still running and responding, with no main window title, when the next session's server started. `sw_status` and `sw_connect` both gave `NOT_CONNECTED: SOLIDWORKS is running (pid …) but can't be reached over COM: it may still be starting, or it runs with different privileges …`, and no other `solidworks-mcp` process was running. Workaround as in S89: the user's word, the process ended, `sw_connect {launch:true}` gave a new pid and attached. A tell the hint could use: a process with no main window and no other server alive is S89's case, never "still starting".
2. Learned, `feature_helix` start azimuth. Same inputs as a helix built earlier today (a circle at the origin on a planar face with normal +Y, frame xAxis [1,0,0], yAxis [0,0,-1]; `height_pitch`, `clockwise:true`, `startAngleDeg` 0): that one started at +Z, this one at -X (`startMm` [-29.25, …, 0], `profilePlaneNormal` [0,0,1]). The description says 0 is at Z for a top-like sketch. The part had no phase to keep, so the profile went on the plane the result named and the volume matched. The result's `startMm` is what to read, not the description. Proposal: find what sets the start (the circle's start point?) and say so in the description, or make `startAngleDeg` measured from a stated axis whatever the circle.
3. Learned, `feature_extrude {cut, end:"through_all"}` from a sketch on the bed plane, for two holes in a flange under a part that steps out above it: it cut on through the step-out (-193.2 against -61.6 for the flange alone), reported `ok`, rightly. A blind cut to the flange's thickness gave the figure, with the check line "no floor at 5 mm: the cut broke out of the body before its end", which read well. No server change.
4. Entry of today ("a turned printed part with two swept internal threads"), point 5, met again: `body_list`'s box read Y to 99.86 where `model.box` read 97.954, the swept thread trimmed at the top. Read `model.box`.

Sorted into S89 (1), met again; S94 (2), new; S52 (4), met again. Not filed (3): the cut went through as asked, and the entry says no server change.

**2026-11-17: a printed part with a plate tilted 20°, holes and a notch sketched on the tilted face, one mirror, driven by global variables**

Build `0.1.0-sw2023-20261117-012659+098a10b`, SOLIDWORKS 2023 (31.3.0), started by `sw_connect {launch:true}`.

1. S64 (l), met again: `sketch_add` of a closed 4-point polyline on a planar face tilted 20° from vertical (with one converted, construction edge in the sketch) read `"status":"closed","closedContours":1,"regions":0`. The `through_all` cut from it removed -2 277.150 mm³, the hand figure exactly. Not reported `ok` while wrong; the region count only.
2. S84, met again in another tool: `feature_extrude {cut, depthMm:4, direction:"+x"}` from a standard plane through a 4 thick wall, the depth equal to the wall on purpose, said in `checks` "no floor at 4 mm: the cut broke out of the body before its end (D1@... reads 4 mm)". The volume matched (-45.804). Right result, worded as a fault; a blind cut whose depth equals the material it crosses could say "through: the end lies on the far face".
3. Held, for the next session: `sketch_start` on a tilted face returns a frame whose origin is the part origin projected onto the face; `{name:"origin"}` in `sketch_constrain` then dimensions from that projection (a horizontal 37.5 along the face's x, which was model Z). `sketch_convert` of the face's bottom edge, `sketch_construction_set`, and a linear dimension from a circle centre `{at}` to that converted line placed the holes; they followed the face at a second set of variables (tilt 20 to 15, the plate moved 4 forward). `relation:"vertical"` between two circle centres given by `{at}` worked.
4. Held: an angular dimension of 110° and of 70° at a real corner, `textAt` inside the interior sector, came back as asked (S82 not met); `equation_set` on an angular dimension with `90deg + "Tilt"` and `"Tilt" = 20deg`, and `tan("Tilt")` in a length equation, both evaluated in degrees.
5. Held: `feature_chamfer {type:"equal"}` on four circular hole edges on a tilted face, `checks` naming each edge's two faces with their own normals (S64 (k) not met: a cylinder and a plane), -230.383 as figured; both returned legs (`D1`, `D3`) driven by one expression each (S64 (i)'s workaround). `feature_mirror` of three cut features about the Front Plane: "instance faces 7/7", no area line (S84 (a) not met).

Sorted into S64 (1), met again; S84 (2), part (c). Not filed (3, 4, 5): held, no item.

**2026-11-17: a read-only check of a printed part driven by global variables, against the script it was rebuilt from**

Build `0.1.0-sw2023-20261117-012659+098a10b`, SOLIDWORKS 2023 (31.3.0).

1. `dimension_list` marks every dimension an equation drives `driven: true` (38 of 42 here), and only the four set as plain values (two hole diameters, one 45° angle) without it. Its description says the flag is for reference dimensions, so the result reads as 38 reference dimensions in a part whose sketches are all fully defined. Reported `ok`; the values were right. Bears on the closed item where SOLIDWORKS' `swDimensionDriven` for an equation-driven dimension was met in `dimension_set`; earlier entries read `driven:true` as "an equation drives it". Proposal: two fields, `reference` (a true reference dimension) and `equation` (the equation's text when one drives it), or the description changed to say what the flag means.
2. Held: `doc_open {readOnly:true}`, `model_review`, `equation_list`, `body_list`, `feature_list`, `face_list` by surface type (axes, cone apex radius and half angle read straight off), `edge_list {ofFace, loop}`, `model_measure` between two parallel tilted faces, `doc_close` with no prompt: thirteen calls for a full interface check, none wasted.

Sorted into S95 (1), new. Not filed (2): held, no item.

**2026-11-17: a printed strap-and-tab part, one profile of lines and two concentric arcs extruded, a cut, a full round fillet and a fillet, driven by global variables**

Build `0.1.0-sw2023-20261117-012659+098a10b`, SOLIDWORKS 2023 (31.3.0).

1. Held, first use in these findings: `feature_fillet {kind:"full_round"}` with `faces`, `centerFaces` and `faces2` each one `{at, type:"face"}` (the two side faces 15 apart and the end face between them): one cylindrical face r 7.5, `checks` "round face r 7.5 mm", -120.714 mm³ as figured, and it followed a second set of variables (side faces 18 apart, r 9) with no rebuild warning. Its result carries no dimension, rightly: the radius is the width's.
2. Held: `sketch_add` of two arcs given by the same `center` merged the second's centre onto the first's ("Arc2 centre merged onto [0, 8.5] (Arc1 centre)"), so the arcs were concentric with no relation asked; a 10-segment profile then took 15 relations and 5 dimensions in one `sketch_constrain` to fully defined, each step verified (19 s).
3. Held: `feature_extrude {cut, end:"through_all", direction:"-z"}` from the Front Plane into a part lying wholly at Z <= 0, -118.791 as figured; a 9-row `equation_set` driving sketch, extrude and fillet dimensions, `volumeDeltaMm3: 0`, and a 7-row second set returning the figured volume to 0.001 mm³ (S7's usage: `model.bodies` read in the result, 1).

Not filed (1, 2, 3): held, no item.

**2026-11-17: a flat printed plate with holes, two windows and two ribs, one mirror, driven by global variables**

Build `0.1.0-sw2023-20261117-012659+098a10b`, SOLIDWORKS 2023 (31.3.0).

1. S64 (i), met again (one line, as this day's earlier entries): an equation on `D1` of a `feature_chamfer {type:"equal"}` left `D3` at its old value at a second set of variables, +11.272 mm³ against the hand figure, `equation_set` ok with no warning; driving `D3` too gave the figure. The workaround is not on a `Usage` line, so a session reading only those does not meet it.
2. Held: a centre rectangle drawn at [0, 0] on the Top Plane came out coincident with the origin (`sketch_constrain` said "already held"); `relation:"horizontal"` and `"vertical"` between a centre point `{at, type:"sketch_point"}` and `{name:"origin"}`, and a `vertical` dimension from the origin to a line, each verified; a 26-row and a 24-row `equation_set` in one call each, the 24 driving dimensions with `volumeDeltaMm3: 0`; `feature_fillet` and `feature_chamfer` by `{at}` on 4 to 8 edges, each to the figured volume.

Sorted into S64 (1), met again. Not filed (2): held, no item.

**2026-11-17: three printed parts of a box and its lid, driven by global variables: walls, corner bosses as arc-and-tangent-line profiles, a mirror, and a lid variant made from a copy of the first lid**

Build `0.1.0-sw2023-20261117-012659+098a10b`, SOLIDWORKS 2023 (31.3.0).

1. S64 (l), met again, on two parts: `sketch_add` on a planar face of two closed profiles, each a 3-point arc of 270° and an open 2-segment polyline whose ends merged onto the arc's ("endsMerged":4), read `"closedContours":2,"regions":0`, and again at `sketch_finish`. The boss from it (+5 545.081 mm³) and the cut (-137.847) were the hand figures exactly. The region count only.
2. S84, met again: `feature_mirror` of four features (two boss profiles, a pad, two holes) about the plane of symmetry of a symmetric box, `checks` "the copy's faces have 1.021 x the seed's area: it meets the body differently from the seed", and of two cuts on a plate, 1.003 x; "instance faces 8/8" and "7/7", the volumes exact (+4 959.740, -186.102). Right results worded as faults. And as this day's earlier entry, point 2: a blind cut of depth equal to the wall it crosses said "no floor at 2 mm: the cut broke out of the body before its end", the volume exact.
3. `sketch_constrain {relation:"concentric"}` between a sketch circle and a model circular edge `{at, type:"edge"}` lying in the sketch's face: `checks` "geometry not checked (model geometry, or a segment of a non-parallel sketch)", relation added, `ok`. It was right (read after the cut by `face_list {surface:"cylinder", radiusMm}`, the axes on the edges' centres), but the check is the one the session needs most for a relation to a model edge. Proposal: for a circular edge parallel to the sketch, the check reads the edge's centre and radius against the sketch circle's centre.
4. Held, a variant part from a copy in 12 calls: `doc_save {mode:"copy"}` of an owned part, `doc_open` of the copy (owned, editable), `feature_delete {deleteAbsorbed:true, deleteEquations:true}` of a cut (its sketch and four equations listed and gone, +3 578.470), `equation_delete` of the global variable no equation then read, `feature_rollback {to, before:true}` before the last fillet, a new sketch and cut inserted there, `feature_rollback {to:"end"}`: the fillet back, the volume as figured (-23.177 on the roll forward), `model_review` clean at both sets.
5. Held: `feature_rollback` before a through-all cut that had also cut material a later feature was planned to remove, a cut inserted above it, and `to:"end"`: the roll forward's `volumeDeltaMm3` read the through-all cut's own share (-48.255), so the reorder was checked against the hand figure with no extra call.

Sorted into S64 (1), met again; S84 (2), met again; S96 (3), new. Not filed (4, 5): held, no item.

**2026-11-17: a printed plate with two walls standing off it in an L, a boss and its bore, four through holes, four countersunk holes in the walls by chamfer, a mitred root fillet, driven by global variables**

Build `0.1.0-sw2023-20261117-012659+098a10b`, SOLIDWORKS 2023 (31.3.0).

1. `feature_fillet` r 3 on two concave edges at right angles, each where a wall meets the plate, meeting in a corner whose third edge (the walls' own inside corner) is not filleted: `volumeDeltaMm3` +470.443 against +470.609 figured, which a second kernel gives exactly; at a second set (r 4) the part read -0.237 against the figure. The faces are the mitre the figure assumes: each round's area is its full length's less 9(π/2 - 1) = 5.137 mm², read 5.137 and 5.132. So the 0.166 (2 ppm of the part) is SOLIDWORKS' own volume of that corner, and every other feature of the part read its figure to 0.001. Reported `ok`, rightly. Lesson for the next session: a hand figure held to 0.001 mm³ misses by a few tenths at a fillet-to-fillet corner; tell a shape fault from it by the rounds' areas (`face_list {surface:"cylinder"}`), not by the volume alone. No open item covers it.
2. `material_list {filter}` with no match returns `"materials":[]`, `"databases":[]` and a warning naming the closest names. Since `databases` lists only the databases the filter reaches, a miss does not show which databases were read, so a caller cannot tell from it that a custom database was searched before calling a material missing (here a custom database is on the machine and was not named). Reported `ok`. Proposal: on a miss, `databases` lists every database searched, with its count, as a call with no filter does. No open item covers it.
3. S64 (i), the workaround held from the start: an equal chamfer of 2 mm on four circular hole edges (two on a face normal +Z, two on one normal +X), both `D1` and `D3` driven by one expression in the same `equation_set`; the second set (legs 2.25) read the figure. `checks` named each edge's two faces and 45° on both, the volume -146.608 as figured.
4. Held: a global variable written `( "A" + "B" + "C" + abs( "A" - "B" - "C" ) ) / 2`, a max with no IIF, evaluated on both branches across two sets; a sketch on a face whose frame came back with sketch x along model -Z (`faceBoundsMm` [-100, 4, -5, 35]), circles placed in those sketch coordinates and dimensioned from `{name:"origin"}`, each verified; 30 dimension equations in one `equation_set`, `volumeDeltaMm3: 0`.

Sorted into S97 (2), new. Not filed (1, 3, 4): SOLIDWORKS' own volume at a fillet-to-fillet corner, reported `ok` rightly, a lesson (1); S64 (i)'s workaround and the rest held (3, 4).

**2026-11-17: a printed bracket of one extruded profile, a through hole and a concentric counterbore sketched on the same face, one mirror, driven by global variables**

Build `0.1.0-sw2023-20261117-012659+098a10b`.

1. S64 (l), met again, and narrowed: on one planar face, `sketch_add` of a circle Ø3.2 read `"closedContours":1,"regions":1`; after the through cut from it, `sketch_add` of a concentric circle Ø6.5 on the same face, enclosing the new hole's edge, read `"closedContours":1,"regions":0`, and again at `sketch_finish`. The blind cut from it (-50.281 mm³) and the mirror after were the hand figures exactly. So the under-count is met when the region the contour closes holds an inner loop of the face: S64 (f)'s rule counts a region only when every edge lies on the sketch's own contour, and the hole's edge is a model edge. Proposed for (l): a model edge that is an inner loop of the face, wholly inside the contour, does not disqualify the region. The region count only; nothing reported `ok` while wrong.

Sorted into S64 (1), met again.

**2026-11-17: a printed C-profile part, two bands off its side face, two hole pairs and two rectangular slots cut, two fillets and a countersink chamfer, driven by global variables**

Build `0.1.0-sw2023-20261117-012659+098a10b`.

1. S80, met again, here with the coordinates kept: `sketch_add` of a `rectangle` by corners on a planar face, its two short sides on the face's boundary edges, returned `relations` "coincident: a model edge, Line1 start" and "coincident: a model entity, Line3 end" (a vertex); `sketch_info` read the corners as drawn, nothing moved. The next `sketch_constrain` horizontal dimension from `{name:"origin"}` to a corner was refused: "would over-define the sketch (SOLIDWORKS made it a driven reference dimension); it was removed", no step applied. The relations were what the part wanted (the band follows the base's edges) and were kept: one length dimension then fully defined it, and it held at a second set of the variables. The same on the second band (both ends on edges). So the auto relations are sometimes wanted: a proposal for S80's fix, an option to keep or drop relations to model geometry, with the result naming them as now. Reported `ok`, and the refusal was clear.

Sorted into S80 (1), met again.

**2026-11-17: an existing printed bracket's copy edited: its base made wider on both sides of its sketch plane, a gusset mirrored about a new plane, two fillets, driven by global variables**

Build `0.1.0-sw2023-20261117-012659+098a10b`.

1. **Missing: a change to an existing extrude's end condition.** A base extruded blind from a standard plane had to grow on both sides of a plane 22 from its sketch plane. `feature_extrude` makes a new extrude only (blind, through all, mid plane, a direction); nothing changes an existing one's end condition, gives it a second direction or an offset start, and `dimension_set` reaches only its depth. Workaround: the same consumed sketch extruded a second time, `direction:"reverse"`, its depth driven by an equation: one more feature for one function. Proposed: an `edit` on `feature_extrude`, as `feature_chamfer` has, taking `end`, a second direction's depth, and a start offset.
2. **A dimension's entities are not given.** `sketch_info` and `dimension_list` returned two dimensions of 6 mm in one profile sketch (`D1`, `D2`) with no word of which segment or points each measures; telling the web's thickness from the flange's took a `dimension_set` probe and a hand-figured volume (+2 298.222 for one, +4 224 expected for the other). Proposed: each dimension in `sketch_info` (and `dimension_list {sketch}`) names its kind and the entities it measures (`Line6`-`Line8`, horizontal).
3. **`feature_extrude` of an already consumed sketch: the result names the shared sketch, and the delete calls its copy unplanned.** The result read `"consumedSketch":"<the profile sketch>"`, while `feature_list` then showed the new feature's child as `<the profile sketch><3>`, a derived copy SOLIDWORKS makes for a shared sketch. Deleting a first such extrude (made at the tree's end by mistake) warned `SOLIDWORKS also removed '<the profile sketch><2>' (absorbed by ...), which was not planned.`: the copy the server's own extrude had made. Nothing was lost (the volume came back exactly). Proposed: the extrude result names the derived copy when the sketch was already consumed, and the delete's dry run lists it as absorbed, not unplanned.
4. **Lesson: `feature_mirror` and `feature_fillet` read exactly against the hand figures** on a part edited in place: a 3-face gusset mirrored about an offset plane +4 960.000, two root fillets R5 (one stopping at the gussets' inner faces) +518.263, four corner fillets by a `{where:{curve, parallelTo, lengthMm, extreme}, expect:2}` query twice -128.761. The `equation_set` batch of eleven new globals and nine existing dimensions in one call held (+5 465.378 as figured), and so did the second set of nine variables (+20 419.903) and the set back.

Sorted into S65 (1, 2), parts (i) and (j); S85 (3), met again. Not filed (4): held, no item.

**2026-11-17: read-only checks of rebuilt parts and an assembly**

1. **`doc_open` with `readOnly: true` reports the document as `access: "owned"`** and says this server may edit it. A read-only check opened six parts and an assembly so, and wrote nothing only because its brief said so: the result invites an edit the open was meant to rule out. No open item names it (grep of `open_items.md` for readOnly / owned). Proposed: a read-only open reports its own access (`readOnly`), and the edit tools refuse on it with NOT_OWNED's hint.

Sorted into S98 (1), new.

**2026-11-17: a weldment profile made from a vendor's DXF of an aluminium extrusion section, for a weldment frame**

Build `0.1.0-sw2023-20261117-012659+098a10b`.

1. **`sketch_import` of an R12 DXF whose geometry sits in a BLOCK placed by one INSERT** (ENTITIES holds only the INSERT at 0,0, scale 1): `FEATURE_FAILED: The sketch from '...' has no segments; it was removed. Hint: Check the drawing's layers and units.` The hint points at layers and units; the cause is the block. Workaround: the block's 106 LINE and 8 ARC entities written out flat into ENTITIES by a short script; the import then read `114 segment(s) ... 8 arc, 106 line` and a 100 mm extrude of it gave `volumeDeltaMm3` 29 998.945 against 29 998.9 figured from the DXF's loops (net 299.989 mm²). Proposed: explode INSERTs on import (or say the drawing holds only block references). No open item covers it.
2. **`sketch_edit` refuses an imported sketch**: right after the import (sketch renamed by `feature_rename`, its test extrude deleted with `feature_delete`, the sketch kept, not suppressed, `feature_list` showing it as `ProfileFeature` with parent Front Plane), `sketch_edit` twice: `FEATURE_FAILED: SOLIDWORKS did not open sketch '<name>' for editing. Hint: It may be suppressed, rolled back or derived`. None of the three held. So the 114 segments could not be fixed or dimensioned and the sketch stays under-defined. Not probed whether the rename (a name starting with a digit) or the import is the cause. No open item covers it.
3. **Missing: saving a part as a library feature part (.sldlfp)**, which is what a weldment profile is and what `weldment_member` reads (`<weldment profiles>\<standard>\<type>.sldlfp`, sizes as its configurations). `doc_save` refuses any extension but the native one ("doc_save writes native .sldprt files"), and `model_export` has no such format, so a custom profile cannot be made end to end through the server: the user had to Save As by hand and add the folder to File Locations. Proposed: `doc_save` takes `.sldlfp` for a part (SaveAs3 to that extension), and `weldment_member` (or a small tool) can add a profile folder to the Weldment Profiles file location. No open item covers it.
4. **`weldment_member` with a custom profile fails, cause not found**: a library feature part saved by hand from a part holding one imported profile sketch (point 2's, 4 closed contours, 114 segments, under-defined), in its own standard folder under an added Weldment Profiles location, size as its one configuration. The lookup found it (no "No profile" error). On a 100 mm line on the Front plane: `VERIFY_FAILED: weldment_member: 'Weldment' was created but has no faces ... Hint: The segments must form a path a profile can follow`; on a new line on the Top plane with `keepOnFailure`: `FEATURE_FAILED: SOLIDWORKS created no feature (the API returned nothing)`. A built-in square tube on the first line worked (13 369.911 mm³, as figured). So the path is fine and the hint misleads; whether the profile file or the server's insert is at fault is not yet known. Also: `doc_open` refuses `.sldlfp` ("can't open '.SLDLFP' files"), so the profile could not be read back through the server. No open item covers it.

Sorted into S99 (1, 2), new; S100 (3, 4), new; S91 (4), its cause found by the entry of 2026-11-18.

**2026-11-18: reading a hand-saved weldment profile (.sldlfp) that would not sweep, read-only**

1. **The likely cause of the previous entry's point 4, found by reading, not yet proved.** `feature_list` on the open library feature part: `LibFeat1` (type `LibraryFeature`) with the profile sketch as its child, and the References folder empty. A built-in profile shipped with SOLIDWORKS 2023 (`iso\square tube.sldlfp`, opened by the user, which sweeps on the same path) shows its sketch at the top of the tree, marked L, with a plane and a sketch point under References and no `LibFeat1`. `sketch_info` on the custom profile: 114 segments, 4 closed contours, 0 open, 4 regions, so the sketch itself is not the fault. The custom file is being saved again with only the sketch selected; a sweep after that settles it. The hint "The segments must form a path a profile can follow" misleads here: the path was fine. Proposed: `weldment_member`, when the profile it found holds no top-level sketch (or its sketch sits inside a library feature), say so in place of the path hint. No open item covers it.
2. Held: `feature_list`, `config_list`, `sketch_info` and `feature_info` read a document the user had open (foreign, not adopted) with `doc` given, no edit asked. Still missing: `doc_open` of a `.sldlfp` (previous entry, point 4), so the built-in profile had to be opened by the user for the comparison.

Sorted into S100 (2), new. Not filed (1): its likely cause was wrong, as the next entry's point 3 says (filed into S100), and its path hint is S91's.

**2026-11-18: a custom weldment profile with one size made to work, and why `weldment_member` cannot reach it**

Build `0.1.0-sw2023-20261117-012659+098a10b`.

1. **Cause of the 2026-11-17 entry's point 4, proved: the folder layout of a profile with one configuration.** SOLIDWORKS reads a library feature part with several configurations at `<standard>\<type>.sldlfp` (type = file name, size = configuration), as the shipped profiles are, but one with a single configuration only at `<standard>\<type>\<size>.sldlfp` (type = subfolder, size = file name). A one-configuration profile placed directly in the standard folder is ignored: the Structural Member PropertyManager lists no Type, and clicking the path then raises "`<standard>\.sldlfp` was not found"; `InsertStructuralWeldment5` with its explicit path made no member, and `weldment_member` reported `VERIFY_FAILED: 'Weldment' was created but has no faces ... Hint: The segments must form a path a profile can follow`, a hint about the path when the profile was the fault. Moved into `<standard>\<type>\<size>.sldlfp`, the same file swept by hand at once, 29 998.945 mm³ on 100 mm = its area × length. **Missing: `weldment_member` resolves only `<standard>\<type>.sldlfp`** (`WeldmentService.Profile`: `Directory.GetFiles(stdDir, "*.sldlfp")`, size from `GetConfigurationNames`), so it cannot use this layout. Proposed: resolve `type` as a subfolder too, sizes from its file names, and pass that file's configuration; and refuse a one-configuration file found directly in a standard folder, naming the layout, instead of inserting. A shipped profile copied in under another name worked in both places, so only the layout decides.
2. **A subfolder of a standard folder is a Type to SOLIDWORKS.** A `ref` subfolder beside the profile made the UI list `ref` as the only Type. The server's resolution is unaffected; a note for whoever lays profiles out.
3. **The 2026-11-18 read-only entry's point 1 was wrong.** `feature_list` on a library feature part shows its marked sketch inside a `LibFeat1` of type `LibraryFeature`; the FeatureManager shows the sketch at the top level marked L and no `LibFeat1`. Taken as the fault, it cost two hand re-saves (Dissolve Library Feature, Add to Library). Proposed: `feature_list` says the wrapper is internal, or lists the sketch as the library feature it is.
4. **`doc_save` of an adopted library feature part reported success and the profile sketch was gone on reopening.** On a copy of a shipped profile, adopted by `doc_activate {allowEdit:true}` after the user opened it: its configuration table deleted (`feature_delete`, one table refused "Could not select"), `config_add` and three `config_delete`, `sketch_edit` of its profile sketch, `sketch_delete` of all 16 segments, `sketch_add` of 112, `sketch_finish` (112 segments, 4 closed contours), `doc_save` (`savedTo` the .sldlfp, 41 145 bytes against 74 903 before), `doc_close`. Reopened by the user: no profile sketch, `LibFeat1` with no children. Not known which step dropped it; nothing reported a fault. A `feature_list` after the save would have caught it before the file was closed.
5. **`sketch_add` of 112 entities (106 lines, 4 arcs, 2 circles) took 183 s in a library feature part and 254 s in a new part**, `TIMEOUT` at 50 s both times, completed later (`sw_status` `lateCompletions`, succeeded). About 1.6 to 2.3 s an entity; a hand-sized sketch would need a batch path or a longer limit for large adds.
6. Held: `feature_sweep` of a 4-contour profile along a 100 mm line read exactly profile area × length (29 998.945 = 299.989 × 100); `weldment_member` with a shipped profile copied into a custom standard folder, under its own and under another file name, made the member at once (13 369.911 mm³).

Sorted into S91 (1), the item made from it; S100 (3, 4), new; S101 (5), new. Not filed (2, 6): a note on how SOLIDWORKS reads a profile folder (2); held (6).

**2026-11-18: scenario test of build 20261118-060615 (S91)**

Build `0.1.0-sw2023-20261118-060615+ab34750` (`bin\0.1.0-20261118-060615`).

1. **S91, proof held.** A custom one-size profile laid out as `<standard>\<type>\<size>.sldlfp`, in a profile folder already registered in SOLIDWORKS, used as it is: `weldment_member` along a 100 mm line, `size` typed `"20x40"`, read `profile` `<standard> / <type> / 20 x 40` with the size file as `profilePath`, 1 member, 29 998.945 mm³, the same as that profile swept by hand in the 2026-11-18 entry's point 1. Along a closed 300 × 200 rectangle with mitered corners: 4 members, lengths 320 and 220, one body each. A wrong type was refused listing the subfolder type, a wrong size listing `20 x 40`. A copy of the same file put directly in the standard folder (removed afterwards) was refused `INVALID_ARGUMENT` "has one configuration ... would make no member", the hint naming `<standard>\<name>\<size>.sldlfp`; `feature_list` afterwards showed no feature added. A shipped iso `square tube 40 x 40 x 4` in the same part still worked (53 479.645 mm³ on 100 mm).
2. **"No profile" lists a profile the server refuses.** With that one-configuration copy in the standard folder, a wrong `type` was refused with "Profiles: <the copy's name>, <the subfolder type>". The copy is offered as a type although the next call with it is refused. Proposed: leave a one-configuration file in the standard folder out of the list, or mark it ("not usable: one configuration").
3. Lesson for callers: `weldment_member` takes a `SketchN` number for the profile sketch it brings in (shown under the member as the library sketch's name), so the next `sketch_start` returned `Sketch3`, not `Sketch2`. A call guessing `sketch:"Sketch2"` was refused `ENTITY_NOT_FOUND`: use the name `sketch_start` returned.
4. Held in the short checks: part A read-only with a forced rebuild 174 346.977 mm³, 221.42 g PETG; STEP export and re-import still 16 surface bodies (S5, met again); drawing at 1:2 with both sections (1 and 3 hatched faces); the bore chamfer on a copy of the machined part −134.041 mm³ with no warning; hinge assembly interference 0 at its saved position; the hand-mated assembly 2 mates without error, 127 402.659 mm³, shaft 60 → 80 → 60 with ±6 283.185 mm³ and mates still clean, closed without saving.

Sorted into S102 (2), new; S5 (4), met again, the rest of the regression held. Held: S91 (1), closed. Not filed (3): a lesson for callers, `sketch_start` returns the sketch's name.

**2026-11-18: a weldment frame of 16 members in one custom profile, from five path sketches on offset planes, every dimension driven by global variables**

Build `0.1.0-sw2023-20261118-060615+ab34750`.

1. **`weldment_member` on a sketch whose lines run in two directions and do not touch fails with the path hint.** One 2D sketch of four lines, two vertical and two horizontal, no line touching another, all segments by default, `corner:"none"`: `VERIFY_FAILED: weldment_member: 'Weldment' was created but has no faces, so it changed no geometry. Rolled back: deleted Weldment. Hint: The segments must form a path a profile can follow (no zero-length or overlapping lines).` The same sketch with `segments:["Line1","Line2"]` (the two parallel lines), then `["Line3","Line4"]`, made both members at once, volumes exact. Likely cause: SOLIDWORKS takes a group of segments only when they are contiguous or all parallel. Proposed: the server splits the segments into groups by that rule (one group per parallel set, as groups of one feature), or the hint names the rule. Bears on S91's hint point, met again with another cause.
2. **The result does not say which way the profile lies.** `rotateDeg` 0 on vertical lines in a sketch on a Right-offset plane put the section's long side along the sketch normal; 90 put it in the plane, the same for horizontal lines there and in Front-offset sketches; a Top-offset sketch needed 0 for the long side vertical. Found only from `model.box` and `body_list` boxes, one undo. Proposed: each member in the result gives its section's extents along the model axes (or the long side's direction), so a wrong rotation shows in the call that made it.
3. **`material_list` lists one database twice**, under its short (8.3) path and its long path, so every row comes twice (`filter:"606"`: 24 rows for 12 materials, `databases` two entries of 259 each). New; no open item covers it.
4. **`model_review` lists 8 under-defined profile sketches for 9 member features** (`2040 profile` to `2040 profile7`, info). The profile sketch is an imported DXF (S99: it cannot be opened to be defined), so the info is expected; the count one short is not explained. Not probed.
5. Held: `equation_set` with 43 rows in one call (5 reference-plane offsets, 38 sketch dimensions), `volumeDeltaMm3` 0; a Front-offset plane made with `offsetMm:-590` reads its dimension as 590 and an equation drives it; the second set of three globals +194 993.145 = 650 mm × the section area, bodies 16 and 114 faces each before and after, and back to the first volume exactly; `doc_undo` of one member feature removed it with its profile sketch and its internal plane; `feature_rename` of finished features while another sketch was open; `model_cut_list {update:true}` 9 items, `TOTAL LENGTH` 12950, every body once; S91's layout, nine members in one session.
6. **Missing: a tool to hide or show reference geometry and sketches.** At the end of a part the owner wants every reference plane, path sketch and structural member's profile sketch hidden; no tool does it (`view_screenshot`'s `hidePlanes`/`hideSketches` only hide them for the picture and restore them after), and `vba_run` was off. Left to the owner by hand: 5 planes, 5 path sketches, 9 profile sketches. Proposed: a `feature_visibility {features:[...] | kinds:["plane","sketch","profile_sketch"], show:false}` that hides by name or by kind (a structural member's profile sketch as a kind of its own), verified by reading each feature's visibility back; or a `hide` option on `doc_save`. No open item covers it.

Sorted into S103 (1, 2), new; S104 (3), new; S105 (6), new. Held: S91 (5), closed. Not filed (4): the profile sketches' info is expected (S99, an imported sketch cannot be defined), the count one short not probed.

**2026-11-18: a two-sheet A3 drawing of a 16-member weldment part: three views at 1:10, nine dimensions, lettered members, notes, an iso view and its cut-list table**

Build `0.1.0-sw2023-20261118-060615+ab34750`.

1. **The overlap checks pass a note laid over a table and over another view's geometry.** `drw_table_add {kind:"cut_list", atMm:[408,285]}` returned the rows but no extent; the table at the default template came out about 201 × 71 mm (sheet 211..412 × 214..285), over a side view at 179..251. A 145 × 118 mm note added after it at [238,222] read `"clear of the border, the title block and the other annotations"`, while it lay across both the table and the side view's members; only the PDF showed it. Proposed: `drw_table_add` returns the table's `extentMm` and warns of what it lands on; the note and dimension checks count tables and the views' `boundsMm`, not only annotations.
2. **Missing: a cut-list item's properties.** The cut-list table's DESCRIPTION column came empty for all 9 items, and no tool sets a cut-list item's `Description` (or any of its properties), so the table cannot name its members; a note mapping item numbers to member letters stood in. Proposed: `model_cut_list` takes `set: [{item, properties}]`, or `prop_set` takes a cut-list item.
3. **The part read as changed after the drawing was made from it.** The part was saved (`dirty:false`), then `drw_views_standard` of it warned `'pilot frame.SLDPRT' is open with unsaved changes`, and a later `doc_save` of the part warned `was modified outside this server since its last change (by the user?)`. Not known whether the user had edited it meanwhile or the drawing's views (cut list in `Default<As Welded>`) dirtied it. Not probed.
4. Lesson for callers: on an extrusion with rounded corners, an edge picked on the face next to the corner sits inset by the radius: two such edges read 599 for 600 (correctly, the geometry is 599 apart); the edges on the faces square to the measured direction gave 600.
5. Held: `drw_views_standard` at an explicit scale with its overflow warning; `drw_view_delete`, five `drw_view_edit` moves; ten `drw_dimension_add`, each value checked against the geometry; `drw_annotation_edit` move and delete of notes, a dimension and a table; `drw_sheet_add`; `model_export` of the two sheets into one PDF. Still open, met: S10 (dimensions at two decimals with a comma, `1200,00`; title block not filled).

Sorted into S106 (1), new; S65 (2), part (k); S81 (3), met again; S10 (5), met again, the rest held. Not filed (4): a lesson for callers, an edge on the face beside a rounded corner sits inset by the radius.

**2026-11-18: an edit to a printed plate (a wall and two pilots) and a new flat part with a pattern and a mirror, mated into a subassembly**

Build `0.1.0-sw2023-20261118-060615`.

1. **S92 (b), met again, at a session's start.** `equation_list` on a part, before any write, read every global set to a number truncated (`60.4mm` as 60, `73.5mm` as 73, `3.4mm` as 3) and `0` for every row defined from others, with no warning; reported `ok`. SOLIDWORKS had been started by an earlier session about 25 minutes before, and `sw_status.otherServerPids` listed a second server process. Ended by `Stop-Process` with no document dirty (`Get-Process` read no `SLDWORKS.exe` after), `sw_connect {launch:true}` reported "a new one (before: pid …)" (S60 (c) held); after it the same list read true and every `equation_set` held. S92's proposal (b), a warning when a constant reads back other than it was set, would have shown this without the caller comparing text to value.
2. Held, each figure against a hand figure: `feature_extrude` boss mid-plane and blind, cuts blind and through all from a reference plane; `feature_pattern_linear` (5 at 10 along +x, "12/12 instance faces"); `feature_mirror` of a cut and its pattern; two `equation_set` batches of 14 and 19 rows on fresh sketches; second sets read by `body_list` face counts, `face_list {near}` and `sketch_info`; `asm_insert` floating, three plane `coincident` mates; `asm_interference` with a limited component list, an overlap of 130 mm³ matching the hand figure; `dimension_set` on an assembly's distance mate, three settings; `model_export` STEP twice.
3. **A lesson for callers, small**: after `doc_new` of a part, `asm_insert {doc:<assembly>}` placed it and warned "Re-activated '<assembly>' (the active document was '<part>')"; the next `asm_mate_add` calls without `doc` were refused `WRONG_DOC_TYPE`, the target still the new part. Correct by the rules (the target is set by `doc_new` / `doc_activate`, not by a `doc` argument), but the warning reads as if the assembly were now the target. One `doc_activate` fixed it, three calls lost. Proposed: the warning adds "the target is still '<part>'".

Sorted into S92 (1), met again; S107 (3), new. Not filed (2): held, no item waits on these tools.

**2026-11-18: a part's outline sketch redrawn and driven, a local boss added by rollback, both checked in an assembly**

Build `0.1.0-sw2023-20261118-060615`.

1. **S92 (b), met again, mid-session, and read too late.** The first `equation_list` of the job, on a part opened earlier in the same SOLIDWORKS session, read every global set to a number truncated (`60.4mm` as 60, `3.4mm` as 3, `0.5mm` as 0) and every derived row as `0`, reported `ok`. Two reads later the first `equation_set` (a batch of eight, one a new global `= 0.5mm`) was refused `FEATURE_FAILED` with the restart hint, and a plain one-row `equation_set` of an existing dimension was refused too. The sketch had already been redrawn with plain values, so it was saved so and its equations bound after the restart (SOLIDWORKS ended by PID, started again, `sw_connect` read "a new one", documents reopened and `doc_activate {allowEdit:true}` again): every `equation_set` after held, ten in all. Cost: ~6 calls and one restart; had the truncated read been taken as the sign, the restart would have come before any write. The server could say so in `equation_list`'s result: a warning when a numeric global's `value` differs from the number its `text` gives.
2. Held, each against a hand figure to 0.01 mm³: a closed polyline redrawn in an existing extrude's sketch (old lines deleted, a centreline from the origin to the top line's midpoint, symmetric, equal, five dimensions, fully defined) and driven by five closed forms; a two-rectangle boss sketched on a face of an earlier feature with the bar rolled back before a cut, extruded blind 4 along the sketch normal (+800.000), the cut after it whole again on `rollback to end` (-58.905); second sets of three and six variables, back to the first volume exactly.
3. **A lesson for callers**: `sketch_add` `rectangle` adds an `equal` relation between two adjacent sides when they are drawn the same length (here 8 x 8), which makes it a square for good: a later width and height from different variables would conflict. Drawn as a closed `polyline` instead, with horizontal and vertical added in `sketch_constrain`, it held. The result did list the relation (`"equal: Line2, Line1"`), so it is a read, not a fault.

Sorted into S92 (1), met again; S80 (3), part (c). Not filed (2): held, no item waits on these tools.

**2026-11-18: a custom sheet format made from the ISO one with `vba_run`, then filled by properties on a two-sheet drawing**

Build `0.1.0-sw2023-20261118-060615+ab34750`.

1. **S89, met again.** `sw_connect {launch:true}` started SOLIDWORKS; after the server process was restarted (to turn on `SWMCP_VBA`), every call returned `NOT_CONNECTED: SOLIDWORKS is running (pid …) but can't be reached over COM`. An outside COM client (`GetActiveObject("SldWorks.Application")`) failed the same way, `Operation unavailable`, so nothing but the process that launched it could reach it. Workaround: the user closed SOLIDWORKS and started it from the Start menu; unsaved drawing properties were lost and set again (9 calls). Cost here: two user round trips.
2. **S10, done with `vba_run`, and it held.** No tool edits a sheet format (`drw_annotation_list {format:true}` lists the format's notes, "not editable here"). With `vba_run`: `DrawingDoc.EditTemplate`; the format sketch's lines by `GetActiveSketch2().GetSketchSegments()`, selected with `Select4(True, Nothing)` when both ends lie in the block's box, deleted with `Extension.DeleteSelection2(0)` (25 lines); the format's notes from the sheet view's `GetFirstNote` chain, which `Annotation.Select3(True, Nothing)` did **not** select (0 of 36, no error) while `Annotation.Select2(True, 0)` did (36); new lines by `SketchManager.CreateLine` with `AddToDB = True`, circles and centrelines; notes by `ModelDoc2.InsertNote("$PRP:""Name""")`, then `GetTextFormat`/`SetTextFormat` for height, `SetPosition2`, `Annotation.Color`; `DrawingDoc.EditSheet`; `Sheet.SaveFormat(path)` wrote the `.slddrt` (True). `drw_sheet_setup {format:<path>}` on each sheet then read back the new format, `titleBlockMm` found from the new lines. 4 `vba_run` calls. Proposed for S10: a tool for the format, built on these calls (delete in a box, add lines, add linked notes, save the format).
3. **`drw_annotation_list {format:true}` gives a format note's resolved text, not its link**: an empty title-block field read `""`, so which property fills it could not be told without `vba_run` (`Note.PropertyLinkedText`). Proposed: list the link beside the text.
4. **A lesson for callers: `$PRPSHEET:"Weight"` with the part's `Weight` = `"SW-Mass@<part>"` is evaluated in the drawing's mass unit, not the part's.** The part set to kg read `10.49` in `prop_list`; the drawing's note still read `10489.13` after `ForceRebuild3`, until the drawing's own units were set custom with mass in kg (`swUnitSystem` 4, `swUnitsMassPropMass` kg). `doc_units_set` takes only the four systems, so no tool sets a mass unit; proposed: a `massUnit` option.
5. Held: `prop_set` with a linked value (`"SW-Material@<part>"`) read back its resolved value; `doc_activate {allowEdit:true}` with its backup; `model_export` of both sheets into one PDF.
6. **`doc_new {type:"drawing"}` with no `template` replaces the default template's sheet format with ISO, and its units with MMGS.** With SOLIDWORKS' default drawing template set to one holding a custom format and custom units (mass in kg), `doc_new` reported "Created drawing from <that template>; ... format a3 - iso.slddrt" and units MMGS; with `template:<the same file>, units:"template"` it kept both ("as the template has it", units custom). Not reported wrong, but a user who set a default template does not get it. Proposed: keep the template's sheet whenever the default drawing template is not the shipped one, and say so; or a server setting for the default drawing template.
7. Held: `vba_run` setting system options (`SetUserPreferenceStringValue` for the default drawing template and the template and sheet-format file locations) and saving a drawing as a template (`Extension.SaveAs3` to `.drwdot`, errs 0).

Sorted into S89 (1), met again; S10 (2, 3), met again; S65 (4), part (l); S108 (6), new. Not filed (5, 7): held, no item waits on these calls.

**2026-11-18: a shared global variable changed in two parts, their assembly checked**

Build `0.1.0-sw2023-20261118-060615`.

1. **S92 (b), met again, on a part opened read-only with no write in the session yet.** `equation_list` read a global `= 60.4mm` as 60 and every row defined from others as 0, with no warning, after some 15 calls in the session (`doc_new`, `drw_sheet_setup`, `vba_run`, `doc_save`, `doc_close` on drawings, then `doc_open {readOnly}`, `model_export`, `doc_close`). After a restart (a new process, `sw_connect {launch:true}`: "a new one (before: pid ...)") the same part read right and two `equation_set` calls held. So the state came before any `equation_set` of this session; what brought it on is still not known.
2. Held: `equation_set` with two rows, one a new dimension equation, verified both; `asm_interference {includeCoincident}` on an assembly opened read-only after a `doc_rebuild`; `doc_close` of that assembly discarded its rebuild and closed its parts, said so.
3. **S92 (c), a lead on what brings it on: both times it followed a read-only open.** After the restart in point 1, two parts opened for edit read right and took `equation_set`; then an assembly was opened `{readOnly:true}`, rebuilt, checked and closed ("SOLIDWORKS also closed: ..." its parts). A part of that assembly, opened again for edit 15 minutes later, read `= 60.4mm` as 60, `= 2.5mm` as 2 and every derived row 0. The state of point 1 had also followed a `doc_open {readOnly:true}`. A second restart (`Stop-Process`, nothing dirty, `sw_connect {launch:true}`: "a new one (before: pid ...)") cleared it again, and the same part took `equation_set` (`-6.743` mm³, as figured). Not proved, two cases; proposed: a live test that opens a part read-only, closes it, opens it for edit and reads `equation_list`.
4. **S92 (c), the lead firmed up: the state follows an assembly being loaded, read-only or not, by the server or by the user.** Third and fourth times in the session. (i) After a restart, the server edited and saved two parts (reads right); the user then opened an assembly holding them in SOLIDWORKS' own window, and the next `equation_list`, on both parts, read every global truncated and every derived row 0. (ii) After another restart, a part read right and took four `equation_set`, a linear pattern and its spacing equation; then the server opened an assembly holding it, editable this time (not read-only), set a distance mate three times, ran `asm_interference` and closed it with `discard`; `equation_list` on the part, still open, read wrong at once. So point 3's read-only open is not the cause; loading an assembly that contains the part is the one step all four share. The geometry did not change (`body_list` the same volume), and a STEP exported in that state carried it. Proposed: a live test, a part with a global of `60.4mm`, read; an assembly containing it opened and closed; read again. And until it is fixed: edit parts first and open the assembly last, as this session did in its last round.
5. `feature_pattern_linear` returned `ok` with "warning 51 (swSketchErrorExtRefFail)" on the new pattern; after an `equation_set` on its spacing the warning was gone and `model_review` clean. Not wrong, but the result left it unsaid whether the warning was real; the session spent a review on it.
6. **The connection was lost after `asm_component_move` and `doc_rebuild` on a subassembly open beside its parent.** In an assembly's limit-mate configuration, a component was moved twice (13.2 s and 14.5 s) with a rebuild after each (the second 6.7 s); the next call, `asm_component_list`, failed `NOT_CONNECTED` "SOLIDWORKS is running (pid ...) but can't be reached over COM". The process answered `Responding: True` with a 77 MB working set, its window title naming the subassembly as dirty. The user brought it back by hand; what it showed is not known. Before it, `dimension_tolerance_set` had changed a limit mate's range (type `limit`, lower -23.5), read back as `limits: [15, 72.5]`; `dimension_set` of the nominal to the new minimum was refused first (`INVALID_ARGUMENT`, "rejected 15 mm ... as invalid") until the tolerance was widened. Proposed: `dimension_set`'s hint could say a limit mate's nominal must lie inside its limits, and `dimension_tolerance_set` is the way to move them.

Sorted into S92 (1, 3, 4), met again; S84 (5), met again; S109 (6), new; S110 (6), new. Not filed (2): held, no item waits on these tools.

**2026-11-21: scenario test of build 20261121-175345 (S61 (a) again)**

Build `bin\0.1.0-20261121-175345` (`serverVersion` `0.1.0-sw2023-20261121-175345+e3b1b7e`, tag
`build-20261121-175345`), SOLIDWORKS 2023 SP3 already running, attached at once.

1. **S61 (a), not held where the assembly is saved in another folder than the STEP; reported `ok`
   with "Also wrote …".** `asm_insert` of a STEP of a 20 mm cube, each case a fresh copy of the STEP
   in its own folder, the STEP never the first component unless said. Every insert: one component,
   "1 solid body (8000 mm³)", linked by 3D Interconnect, part `<step folder>\<name>.step.SLDPRT`, and
   that part now listed `owned` (was `foreign`). Held, reopened with every component `resolved`:
   (a) STEP first, assembly saved in the STEP's folder (no "Also wrote", SOLIDWORKS wrote it);
   (c) a part first, the STEP after, `doc_save as` in the STEP's folder: "Also wrote '<step
   folder>\<name>.step.SLDPRT': the assembly's save left it unwritten", the part `dirty: false`,
   reopened 2 components, 16 000 mm³; (d) the STEP after a subassembly and a part, `doc_save as`
   then `doc_save` in place (the in-place save quiet, the assembly not dirty again); (f) a part
   first, the assembly saved, the STEP inserted, `doc_close {save:true}`: warning "Also wrote …",
   reopened resolved. **Not held** where the assembly lies in another folder than the STEP: (e) the
   STEP inserted after a subassembly into a saved assembly whose folder is not the STEP's, `doc_save`
   in place: "Also wrote '<step folder>\<name>.step.SLDPRT'", the file there, yet `doc_open` of the
   assembly read `missingReferences` `<assembly folder>\<name>.step.sldprt`, the component
   `suppression: suppressed`, the assembly `dirty: true` on open. Proved twice more: (g) a part
   first, the STEP in a subfolder of the assembly's folder, `doc_save as`: written, reopened the same
   way, missing in the assembly's folder; (h) the same with a second `doc_save` in place after the
   part was on disk (the file grew 49 635 → 50 683 bytes): still missing on reopen. So the assembly
   file stores the part as if it lay in the assembly's own folder; the cases that held are the ones
   where that folder is the STEP's. The live test of the fix saves the assembly in the STEP's folder,
   so it cannot see this. The STEP as first component in another folder (assembly one folder up)
   resolved as before. Proposed: write the owned unwritten part before the assembly's save, so the
   assembly stores the path the part has, or repoint the component after writing it; and read the
   reference back (reopen, or the saved file's dependencies) before saying the part was written.
2. Held, short checks: a part's `doc_save as` with no message; a drawing's `doc_save as`
   (.SLDDRW) written; a part opened read-only, force rebuild 174 346.977 mm³, mass 221.42 g; STEP
   export and `doc_open` of it, 16 surface bodies (S5, as before); a hinge assembly read-only,
   interference 0; a hand-built two-part assembly read-only, 2 components, 2 mates, 127 402.659
   mm³; a 1:2 plan with two sections, 1 and 3 hatched faces.

Sorted into S61 (1), not held, back to Open; S5 (2), met again. Not filed (2): the other short checks held, no item waits on these tools.

**2026-11-21: scenario test of build 20261121-205309 (S61 (a), S83, S107, S110)**

Build `bin\0.1.0-20261121-205309` (`serverVersion` `0.1.0-sw2023-20261121-205309+1fb1fda`, tag
`build-20261121-205309`), SOLIDWORKS 2023 SP3 already running, attached at once. An older server
process of the previous build was still running beside it (`otherServerPids`), not touched.

1. **S61 (a), held in every case, the three that failed included.** `asm_insert` of a STEP of a
   20 mm cube, each case a fresh copy of the STEP in its own folder. Every insert: one component, "1
   solid body (8000 mm³)", linked by 3D Interconnect, and the message now "its part '<step
   folder>\<name>.step.SLDPRT' is written beside the STEP"; the file was on disk right after the
   insert, the part listed `owned` and clean. Reopened after each save, every component `resolved`,
   no `missingReferences`, the assembly not dirty on open: (g) a part first, the STEP from a
   subfolder of the assembly's folder, `doc_save as` (2 components, 16 000 mm³); (h) the same with a
   second `doc_save` in place; (e) the STEP from an unrelated folder into a saved assembly holding a
   subassembly, a part and an earlier STEP component, `doc_save` in place (4 components, 40 000 mm³);
   (d) the STEP after a subassembly and a part, `doc_save as` then in place (no dirty-again); (a) the
   STEP first, assembly in the STEP's folder, its fixed state kept; (b) the STEP first, assembly one
   folder up; (c) a part first, the STEP after, `doc_save as` in the STEP's folder; (f) a part first,
   the assembly saved, the STEP inserted, a coincident mate added to the STEP component, `doc_close
   {save:true}`: reopened with the mate (0 in error) and the component where the mate put it. No
   "Also wrote" and no warning in any save, as the part was already written at the insert. The
   "modified outside this server" warning the fork's session saw on a `doc_save as` after a STEP
   inserted first did not appear in case (a) (no refused insert or listing in between).
2. Seen with S61 (a), not faults: the STEP component is named `<name>.step-2` even as the only
   instance in the assembly (the repoint deletes the linked `-1` and adds the written part); the
   result names it, so a mate by that name works, but a caller guessing `-1` would miss it. On
   reopen the repointed components were listed before the others (`<step>-2`, part, `<step>-2`,
   subassembly), not in insert order.
3. **S83, held.** Two 20 mm cubes, the second inserted at x +80; distance 80 `aligned` between their
   right planes: landed at x −80 with the old wording ("…-80 mm along its normal before, 80 mm
   after. If it should stay on the side it was on, delete the mate and add it again with flip the
   other way."). Mate deleted, added again with `flip:true`: landed at +80, warning "…on the other
   side of b … from where it was before the mate, as flip asks: 80 mm along its normal before, -80
   mm after. If it should have stayed, add the mate again without flip."
4. **S110, held.** Limit distance mate 20..72.5 at 30; `dimension_set` 15: `INVALID_ARGUMENT` "is a
   limit mate's nominal: 15 mm lies outside its limits 20 to 72.5 mm. Nothing was changed. Hint: …
   move the limits first with dimension_tolerance_set (type limit, upper and lower deviations from
   the nominal)". Following it, `dimension_tolerance_set` limit upper 42.5 lower −15 read back
   `limits: [15, 72.5]`; 15 then set, 50 set, 80 refused naming "15 to 72.5" (the limits stay put
   when the nominal moves).
5. Missing, small: `asm_mate_list` gives a limit mate's distance and dimension but not its limits;
   they were readable only through `dimension_tolerance_set`'s `previous` or a refused
   `dimension_set`.
6. **S107, held.** `doc_new` of a part, then `asm_insert {doc:<assembly>}`: warning "Re-activated
   '<assembly>' (the active document was '<part>'; the target is still '<part>', doc_activate to
   change it)."
7. Held, regression: a hand-built two-part assembly (2 components, plate fixed, shaft 2 mates,
   127 402.659 mm³; shaft length 60 → 80 through `dimension_set` in the assembly, mates without
   error, 133 685.844 mm³, back to 60, closed discarded, files unchanged); a part opened read-only,
   force rebuild 174 346.977 mm³, mass 221.42 g, area 116 546.84 mm²; STEP export and `doc_open` of
   it, 16 surface bodies (S5, as before); a 1:2 plan with two sections on the owner's A3 template, 1
   and 3 hatched faces, `doc_save as` .SLDDRW written; a hinge assembly read-only, interference 0.

Sorted into S111 (5), new; S5 (7), met again, the rest of the regression held. Held: S61 (1), S83 (3), S110 (4), S107 (6). Not filed (2): seen with S61 (a), not faults (the STEP component named `<name>.step-2`, which the result names; the reopen order).
