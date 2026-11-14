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

**2026-11-14: scenario test of the run's items (S47 to S59, S60 b-d, S61 b-c, S62 a/d/e, S63, S64 a/d-g, S65 a, S66)**

Build `0.1.0-sw2023-20261114-024455+cfd987e` (`sw_status` and the tag agree).

1. A SOLIDWORKS process in a bad state, met before only as the restart hint's reason: on a process started before the session (what ran in it earlier is not known), every equation was refused, a batch of ten globals and then `"W" = 60mm` alone in a new part; S58 (b)'s message came as designed ("though the equation is valid … restart SOLIDWORKS"). In the same process `material_set "AISI 1020"`, with or without a configuration, applied the material at 1 kg/m³: the result gave `densityKgM3: 1`, `databaseDensityKgM3: 7900`, the check "material reads back" and only a warning ("The mass properties give 1 kg/m³, the database 7900 kg/m³ (bodies with their own material, or an overridden mass?)"), with no error, though the description says the mass properties are verified to show its density. A material from the user library at 1270 kg/m³ read right in that process. A property linked to a 3 mm dimension (`"D1@<feature>"`) resolved `0,0030000000000000`, metres. After a restart of SOLIDWORKS (by the session, on the owner's word) the equations went in at once, a database material read 2700 = its database, and the restart message was the right advice. The same three symptoms were met together once before, in an old SOLIDWORKS session (S28's test). Proposals: `material_set` refuses (`VERIFY_FAILED`, rolled back) when the density read differs from the database's, as it once did; and its message, like S58 (b)'s, names a restart when a database material reads ~1 kg/m³. Bears on S58 (b), S30.
2. `feature_chamfer` `edit` with `flipEdges` flips every edge, not the indexed ones, reported `ok`: a `distance_distance` 1/2 chamfer on two bottom edges of a block meeting at a corner (one along x, one along z) took 1 mm on the bottom face for both. `edit` with the same two edges and `flipEdges:[1]` made both take 1 mm on their side faces (the per-edge checks say so, and the chamfer faces' normals [0, -0.894, -0.447] and [0.447, -0.894, 0] confirm it), with no mixed-sides warning; `flipEdges:[0]` put both back on the bottom. A new chamfer with `flipEdges:[1]` on two other edges gave both their first distance on the side faces, consistent with each other, so what index 0 got there cannot be told. S51 (a), (d).
3. `ref_plane` `angle` with `axis: {name:"Line1", sketch:"Sketch6"}` failed `ENTITY_NOT_FOUND: No feature named 'Line1'`; with `type:"sketch_segment"` added it went through and checked right ("contains sketch segment Line1, 30° to plane 'Front Plane'"). With `sketch` given the reference is a sketch segment or point (the server's own convention), so the type could be inferred. One call more. S59 (c).
4. `dimension_set` through an assembly: the line naming the part reads "Also changed, not saved: '<part>.SLDPRT', a component's own document." without handle and access in a new assembly whose parts the server had not listed yet, and "'<part>.SLDPRT' (D19, foreign), …" in an assembly opened from file. S49, wording.
5. `doc_open` of a STEP written by `model_export` from an assembly of three 20 mm cubes and a cylinder r 5 × 30 turned 45° about [1, 1, 0]: the cubes came in as solids where they were, the cylinder as 3 surface bodies, its component at the identity rotation with box [-5.0001, 0, -5] .. [6, 30, 5.427], neither the turned cylinder's box nor a tight one of r 5, and without `approximate`. S61 (b)'s message said "3 solid bodies, 3 surface bodies" plainly. Bears on S5 and on S52's fallback. `model_export` of that assembly reported `solids: 2` (two distinct parts, four components).
6. Held, each as its item says: S47 frame [20, 10, 410, 287] / [230, 10, 410, 65.25] on a 1:2 sheet in `drw_info` and `drw_sheet_setup`, a note on the title block, one across the border, one clear, dimension text at y 292 and a section reaching y 289.01 warned; S66 an edge id from before a notch `ENTITY_STALE`, an untouched one used; S53 a boss along a vector 40° off the normal, volume π·r²·h·cos 40°; S52 (a) a disc's torus fillet box exact, (b) a turned component's box the expected extremes; S50 the signed side and the crossing warning; S48 (a) to (d); S49 the component in the row and the volume, in a hand-mated assembly too; S54 (a) to (c) with `deleteEquations` and a list delete; S55 `durationS` (10 steps 9.7 s); S56 (a) "already held" and (b) relations listed; S57 no warning after a rolled-back `VERIFY_FAILED` and after a change through an assembly; S58 (a) features named in `featureErrors`, (b) both messages, (c) `frame` in a batch row; S59 (a), (b), (d); S60 (c) "same as before" and, after the restart, "a new one (before: pid …)", (d) no revision warning; S61 (c) the empty part named; S62 (a) plane names and face ids, (d) a part loaded by the server's assembly owned, (e) listed/overlapping/touching; S63 start at +Z, 90° to +X read back, turns `scalar`; S64 (a), (d) (the dry run's refusal word for word), (e), (f), (g); S65 (a). Regression: the shelled tray step by step, each volume as before, and its design intent with one body; the machined part of global variables in 37 calls, 16.3k (39 / 26.3k before), intent and back; the two sections (1 and 3 hatched faces); STEP of the tray still 16 surface bodies (S5).
