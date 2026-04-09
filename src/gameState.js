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
  state.lastPressFeedback = null;
}

/** Handle a single button press event (edge-triggered). */
export function handlePress(state, ctrl) {
  if (state.phase !== 'playing') return;

  const step = getStep(state.stepIndex);

  if (ctrl === 'up' || ctrl === 'down' || ctrl === 'left' || ctrl === 'right') {
    // High loop locks out lateral steering — the scope is too coiled to twist.
    if ((ctrl === 'left' || ctrl === 'right') && state.loopMeter >= LOOP_DISABLE_THRESHOLD) {
      state.lastPressFeedback = { type: 'wrong', t: 0 };
      return;
    }
    if (step.bend && step.bend === ctrl) {
      // Correct direction on a bend: solve and let the lumen ease to center.
      state.bendSolved = true;
      state.tipAim.x = 0;
      state.tipAim.y = 0;
      state.lastPressFeedback = { type: 'solved', t: 0 };
    } else {
      // Wrong direction on a bend, or any direction on a straight segment:
      // mis-aim the scope tip. Each press adds its unit vector to the
      // current aim, axis-wise, clamped to [-1, 1]. This produces a 3x3
      // grid of aim positions: pressing the opposite of an existing axis
      // cancels just that axis, pressing a perpendicular adds a diagonal,
      // pressing the same direction stays pinned at the extreme.
      const v = DIRECTION_VECTORS[ctrl];
      state.tipAim.x = clamp(state.tipAim.x + v.x, -1, 1);
      state.tipAim.y = clamp(state.tipAim.y + v.y, -1, 1);
      if (step.bend) {
        state.painMeter = clamp01(state.painMeter + PAIN_WRONG_DIR);
        state.loopMeter = clamp01(state.loopMeter + LOOP_WRONG_DIR);
        state.lastPressFeedback = { type: 'wrong', t: 0 };
      }
    }
    // No snap — the per-frame update() eases lumenOffset toward the new
    // target, so both tilting away and returning to center animate smoothly.
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
  // Diagonals can have magnitude > 1; normalize so the vanishing point
  // stays inside the eyepiece for all 8 outer aim positions.
  const mag = Math.hypot(dx, dy);
  if (mag > 1) {
    dx /= mag;
    dy /= mag;
  }
  return { x: dx, y: dy };
}

function clamp01(v) { return Math.max(0, Math.min(1, v)); }
function clamp(v, lo, hi) { return Math.max(lo, Math.min(hi, v)); }
