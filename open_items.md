# Open Items

What sessions found in this fork's server and what was decided about each, S-numbered.
The raw entries are in [`findings.md`](findings.md); this file holds the items. Each item
names where it goes: **upstream** (an issue or PR at `soheil-mp/solidworks-mcp`), **local**
(a fix on `sw2023`, sent upstream as a PR when it is general), **usage** (a note for the
sessions that use the server), or **not taken**.

Three sections: **Open**, not yet fixed; **Waiting**, fixed and not yet settled (a PR not
merged, a fix not yet run in a session); **Closed**, merged upstream, run and held, or
decided against, with the record.

**Open: S2 to S5, S7 to S12.** Highest value to build locally, for working drawings and
tokens: S2 and S3 first, then S8. S5 and S7 wait a while for upstream's answer to issues #2
and #4.

---

## Open

**S2. A section leaves the view sketch open, so undo is blocked.** After `drw_view_section`
`sw_status` shows the sketch open, `doc_undo` refuses (`SKETCH_STATE`), `sketch_finish`
refuses drawings. Local: close the view sketch at the end of the call (and of
`drw_view_detail`), then PR. Mentioned in PR #1's text, not fixed there.

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
no item for it; PR when built.

**S9. `sw_help` at session start reads ≈23.5k characters.** Usage: a short note of the tools
and conventions the sessions here use, read instead. Where the note lives (a skill, the
global `CLAUDE.md`, a memory) is the owner's call.

**S10. Drawing format and title block.** Precision and decimal separator of drawing
dimensions, parentheses for reference dimensions, `n×` in hole callouts, DWG NO from a
property, material and weight, section label text and place, unaligning a view. Upstream
ROADMAP T10.3, T10.10, T10.12 partly. Not yet decided which to build.

**S11. No dimension to a section's cut face.** `drw_dimension_add` takes model entities only.
Not yet decided.

**S12. Section orientation is not in the tool description.** Arrow side and view direction
(findings 2026-11-09, point 12). Upstream: proposed in PR #1's text; a description change
when PR #1 is settled.

## Waiting

**S1. `drw_view_section` / `drw_view_detail` drew in view-sketch coordinates.** Local:
`1de5067` + `3f43c50` on `sw2023`, run and held 2026-11-09 in build `…-163234`. Upstream:
PR #1 (branch `fix/section-sheet-coords`, one commit `81475a5`), tested on 2023 SP3 only;
on 2025 the mapping may be wrong if sheet coordinates work there. Closes when PR #1 is
merged or refused; if merged, the local commits drop out at the next rebase. Not yet run:
`drw_view_detail`, views at an angle.

**S6. `sketch_dimension_add` angular refused the acute angle at a shared vertex.** Local:
`18cbc57` on `sw2023`. The supplement is kept when `textAt` lies in its sector, and the value
set after it is checked against the supplement. New live test fails without the fix;
`SketchEditLiveTests` 58/58 on 2023 SP3. Held in the scenario test of build `…-181555`.
Upstream: PR #6 (branch `fix/angular-supplement`, one commit `dce4afe`, "Fixes #3"). Closes
when PR #6 is merged or refused. **Next:** the scenario entry's point 2. Without `textAt`,
a `valueDeg` nearer the supplement is refused with a misleading hint. Fix it as a second
commit on `fix/angular-supplement`, which joins PR #6, and on `sw2023`, with its own live
test.

## Closed

None yet.

## Carried, not items

- **SW 2023 interop patch** (`27e8c25`): `swCustomInfoEquation` case removed,
  `InsertUntrimSurface` instead of `InsertUntrimSurface2`. Not sent upstream (upstream
  supports 2025 and later; ROADMAP T2.3 is theirs). Rebased with `sw2023` each update.
