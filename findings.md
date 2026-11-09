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
in `open_items.md` and for an issue or PR sent upstream.

**Sorting.** A session in this repo sorts the entries into `open_items.md`, and adds a line
under each entry it sorted, naming where each point went by the entry's own numbers:
`Sorted into S6 (1, 3); S2 (2). Not filed (4): one-off.` Nothing here is decided until it
is an item there. An entry is not edited once its session has ended, and none is deleted.

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
