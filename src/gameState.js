// Central game state. The game is a sequence of discrete "steps".
// At each step the player either advances (straight) or must first aim
// in the bend's direction to straighten the tunnel, then advance.

import { getStep, totalSteps, DIRECTION_VECTORS } from './colon.js';

// Meter tuning
const PAIN_WRONG_DIR       = 0.18; // press wrong direction on a bend
const PAIN_PUSH_UNSOLVED   = 0.22; // press forward on unsolved bend
const PAIN_DRAIN_PER_SEC   = 0.05;
const PAIN_HIGH_LOOP_BONUS = 0.20; // extra pain per fwd press when loop is high

const LOOP_PER_FORWARD     = 0.06; // every successful advance forms a bit of loop
const LOOP_WRONG_DIR       = 0.10;
const LOOP_PUSH_UNSOLVED   = 0.15;
const LOOP_DRAIN_ON_BACK   = 0.30; // one-shot drain when backing up

const LOOP_HIGH_THRESHOLD     = 0.50; // pain spike kicks in
const LOOP_DISABLE_THRESHOLD  = 0.75; // left/right become unusable

// Lumen vanishing-point easing
const OFFSET_EASE = 6; // higher = snappier transition (per second)

// Tip aim range: magnitude 1 for insertion steering, 2 for wall inspection
const AIM_MAX_INSERTION = 1;
const AIM_MAX_WITHDRAWAL = 2;

export function createGameState() {
  return {
    stepIndex: 0,
    bendSolved: false,
    painMeter: 0,
    loopMeter: 0,
    phase: 'playing', // 'playing' | 'won' | 'lost'
    // Animated lumen vanishing-point offset (-1..1 on each axis)
    lumenOffset: { x: 0, y: 0 },
    // Persistent scope-tip aim from direction presses. Pressing up sets this
    // to (0,-1); the lumen view subtracts it so the visible tunnel tilts
    // opposite to the pressed direction. Cleared on advance or solve.
    tipAim: { x: 0, y: 0 },
    // Net rotation in 90° increments: +1 per CW, -1 per CCW. Capped at ±4.
    netRotation: 0,
    lastPressFeedback: null, // {type: 'wrong' | 'solved' | 'advance', t: 0}
  };
}

export function resetGameState(state) {
  state.stepIndex = 0;
  state.bendSolved = false;
  state.painMeter = 0;
  state.loopMeter = 0;
  state.phase = 'playing';
  state.lumenOffset.x = 0;
  state.lumenOffset.y = 0;
  state.tipAim.x = 0;
  state.tipAim.y = 0;
  state.netRotation = 0;
  state.lastPressFeedback = null;
}

/** Handle a single button press event (edge-triggered). */
export function handlePress(state, ctrl) {
  if (state.phase !== 'playing' && state.phase !== 'withdrawal') return;

  const step = getStep(state.stepIndex);

  if (ctrl === 'up' || ctrl === 'down' || ctrl === 'left' || ctrl === 'right') {
    // High loop locks out lateral steering — the scope is too coiled to twist.
    if ((ctrl === 'left' || ctrl === 'right') && state.loopMeter >= LOOP_DISABLE_THRESHOLD) {
      state.lastPressFeedback = { type: 'wrong', t: 0 };
      return;
    }
    // Apply the press: add the direction vector to tipAim.
    const v = DIRECTION_VECTORS[ctrl];
    const aimMax = state.phase === 'withdrawal' ? AIM_MAX_WITHDRAWAL : AIM_MAX_INSERTION;
    const newX = clamp(state.tipAim.x + v.x, -aimMax, aimMax);
    const newY = clamp(state.tipAim.y + v.y, -aimMax, aimMax);
    state.tipAim.x = newX;
    state.tipAim.y = newY;

    if (step.bend && !state.bendSolved) {
      // Check if this press brought the displayed offset to center,
      // accounting for any prior rotation. Displayed = bend - tipAim.
      const bv = DIRECTION_VECTORS[step.bend];
      const dx = bv.x - newX;
      const dy = bv.y - newY;
      if (Math.abs(dx) + Math.abs(dy) < 0.1) {
        // Solved: lumen is centered.
        state.bendSolved = true;
        state.tipAim.x = 0;
        state.tipAim.y = 0;
        state.lastPressFeedback = { type: 'solved', t: 0 };
      } else {
        // Check if the press moved the lumen closer to center or further away.
        const oldDx = bv.x - (newX - v.x);
        const oldDy = bv.y - (newY - v.y);
        const oldDist = Math.abs(oldDx) + Math.abs(oldDy);
        const newDist = Math.abs(dx) + Math.abs(dy);
        if (newDist >= oldDist) {
          // Moved further away or no improvement — wrong move.
          state.painMeter = clamp01(state.painMeter + PAIN_WRONG_DIR);
          state.loopMeter = clamp01(state.loopMeter + LOOP_WRONG_DIR);
          state.lastPressFeedback = { type: 'wrong', t: 0 };
        }
      }
    }
    // No snap — the per-frame update() eases lumenOffset toward the new
    // target, so both tilting away and returning to center animate smoothly.
    return;
  }

  if (ctrl === 'rotateCW' || ctrl === 'rotateCCW') {
    // Block if already at 360° in this direction.
    if (ctrl === 'rotateCW' && state.netRotation >= 4) return;
    if (ctrl === 'rotateCCW' && state.netRotation <= -4) return;

    // Rotate the *displayed* lumen offset 90°. The displayed offset is
    // (bend - tipAim), so we compute the current target, rotate it, then
    // back-solve for the new tipAim that produces that rotated target.
    let bx = 0, by = 0;
    if (step.bend && !state.bendSolved) {
      const v = DIRECTION_VECTORS[step.bend];
      bx = v.x;
      by = v.y;
    }
    const tx = bx - state.tipAim.x;
    const ty = by - state.tipAim.y;
    let rx, ry;
    if (ctrl === 'rotateCW') {
      rx = ty; ry = -tx;   // CW scope → CCW image: (x,y)→(y,-x)
    } else {
      rx = -ty; ry = tx;   // CCW scope → CW image: (x,y)→(-y,x)
    }
    // new tipAim = bend - rotated target. No clamping here — rotation is
    // just re-expressing the same offset at a new angle, and the values
    // may need to exceed the normal aimMax to represent that correctly.
    state.tipAim.x = bx - rx;
    state.tipAim.y = by - ry;
    // Check if rotation brought the display to center.
    if (step.bend && !state.bendSolved && Math.abs(rx) + Math.abs(ry) < 0.1) {
      state.bendSolved = true;
      state.tipAim.x = 0;
      state.tipAim.y = 0;
      state.lastPressFeedback = { type: 'solved', t: 0 };
    }
    state.netRotation += (ctrl === 'rotateCW') ? 1 : -1;
    return;
  }

  if (ctrl === 'forward') {
    const canAdvance = !step.bend || state.bendSolved;
    const visualStraight = Math.abs(state.tipAim.x) + Math.abs(state.tipAim.y) < 0.1;

    // Forward only works when the lumen looks straight — neither an unsolved
    // bend nor a lingering tip-aim from a stray direction press. Otherwise
    // the scope just grinds against the wall: pain and loop, no advance.
    if (!canAdvance || !visualStraight) {
      let pain = PAIN_PUSH_UNSOLVED;
      if (state.loopMeter >= LOOP_HIGH_THRESHOLD) pain += PAIN_HIGH_LOOP_BONUS;
      state.painMeter = clamp01(state.painMeter + pain);
      state.loopMeter = clamp01(state.loopMeter + LOOP_PUSH_UNSOLVED);
      state.lastPressFeedback = { type: 'wrong', t: 0 };
      return;
    }

    state.stepIndex += 1;
    state.bendSolved = false;
    state.tipAim.x = 0;
    state.tipAim.y = 0;
    // Each successful advance forms a bit of loop in the scope.
    state.loopMeter = clamp01(state.loopMeter + LOOP_PER_FORWARD);
    // High loop = pain spikes for every forward press from now on.
    if (state.loopMeter >= LOOP_HIGH_THRESHOLD) {
      state.painMeter = clamp01(state.painMeter + PAIN_HIGH_LOOP_BONUS);
    }
    state.lastPressFeedback = { type: 'advance', t: 0 };
    if (state.stepIndex >= totalSteps() - 1) {
      state.stepIndex = totalSteps() - 1;
      state.phase = 'won';
    }
    return;
  }

  if (ctrl === 'back') {
    if (state.stepIndex > 0) {
      state.stepIndex -= 1;
      state.bendSolved = false;
    }
    // Pulling back re-centers the scope tip: the lumen eases to straight.
    state.tipAim.x = 0;
    state.tipAim.y = 0;
    // Backing up always dramatically reduces the loop, even at the start.
    state.loopMeter = clamp01(state.loopMeter - LOOP_DRAIN_ON_BACK);
    return;
  }
}

/** True when left/right buttons are locked out by a high loop meter. */
export function lateralLocked(state) {
  return state.loopMeter >= LOOP_DISABLE_THRESHOLD;
}

/** True when the given rotation direction is locked (360° reached). */
export function rotationLocked(state, dir) {
  if (dir === 'cw') return state.netRotation >= 4;
  if (dir === 'ccw') return state.netRotation <= -4;
  return false;
}

/** Per-frame update: animations, pain drain, lose check. */
export function update(state, dt) {
  // Pain drains slowly
  state.painMeter = clamp01(state.painMeter - PAIN_DRAIN_PER_SEC * dt);

  // Animate lumen offset toward target
  const target = getLumenTarget(state);
  const k = 1 - Math.exp(-OFFSET_EASE * dt);
  state.lumenOffset.x += (target.x - state.lumenOffset.x) * k;
  state.lumenOffset.y += (target.y - state.lumenOffset.y) * k;

  // Feedback timer (unused visually for now, reserved for flashes)
  if (state.lastPressFeedback) {
    state.lastPressFeedback.t += dt;
    if (state.lastPressFeedback.t > 0.5) state.lastPressFeedback = null;
  }

  if (state.painMeter >= 1) {
    state.phase = 'lost';
  }
}

/** Where the lumen vanishing point should sit right now. */
function getLumenTarget(state) {
  const step = getStep(state.stepIndex);
  let bx = 0, by = 0;
  if (step.bend && !state.bendSolved) {
    const v = DIRECTION_VECTORS[step.bend];
    bx = v.x;
    by = v.y;
  }
  // Subtract tip aim: pressing up tilts the scope tip up, so the lumen
  // (which was straight ahead) appears below the player's aim point.
  let dx = bx - state.tipAim.x;
  let dy = by - state.tipAim.y;
  // Cap magnitude at 2 (full wall inspection range). For diagonal
  // positions, normalize to keep the vanishing point on a circle.
  const maxMag = state.phase === 'withdrawal' ? 2 : 1;
  const mag = Math.hypot(dx, dy);
  if (mag > maxMag) {
    dx = dx / mag * maxMag;
    dy = dy / mag * maxMag;
  }
  return { x: dx, y: dy };
}

function clamp01(v) { return Math.max(0, Math.min(1, v)); }
function clamp(v, lo, hi) { return Math.max(lo, Math.min(hi, v)); }
