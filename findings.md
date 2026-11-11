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
