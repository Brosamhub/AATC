# legacy-web

A browser-based proof of concept that preceded the Unity port. Kept in the
repo as a reference for how the insertion/withdrawal mechanic feels when
played — the procedure loop here is the mechanical inspiration for the one
now living in [../Assets/Scripts/Systems/ProcedureManager.cs](../Assets/Scripts/Systems/ProcedureManager.cs).

## Run it

Open `index.html` in any modern browser. No build step, no dependencies.

```sh
open legacy-web/index.html
```

## What's in here

- `index.html` — page entry
- `styles.css` — all styling
- `src/main.js` — entry point / render loop
- `src/gameState.js` — meters, tuning, action handlers
- `src/colon.js` — path generation
- `src/controls.js` — keyboard input
- `src/hud.js` — HUD rendering
- `src/lumenView.js` — tip-of-scope viewport
- `src/progressView.js` — anatomy overview / progress bar

## Status

Frozen. Not part of Milestone 1 or any active Unity milestone. Mine it for
ideas and numbers; don't depend on it.
