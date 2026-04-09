// Pseudo-3D lumen view. Draws concentric ellipses receding to a vanishing
// point. The vanishing point comes from gameState.lumenOffset — centered
// when the path is straight, pushed toward the bend direction when the
// player needs to steer.

const RING_COUNT = 14;

/**
 * Render the lumen into the given rect {x, y, w, h}.
 * offset is {x, y} in the range [-1..1] on each axis.
 */
export function renderLumen(ctx, rect, offset) {
  const cx = rect.x + rect.w / 2;
  const cy = rect.y + rect.h / 2;
  const radius = Math.min(rect.w, rect.h) / 2 - 8;

  ctx.save();

  // Circular clip (scope eyepiece).
  ctx.beginPath();
  ctx.arc(cx, cy, radius, 0, Math.PI * 2);
  ctx.closePath();
  ctx.clip();

  // Outer wall color fills any gap.
  ctx.fillStyle = '#c9556a';
  ctx.fillRect(rect.x, rect.y, rect.w, rect.h);

  const offX = offset.x * radius * 0.75;
  const offY = offset.y * radius * 0.75;
  const vpX = cx + offX;
  const vpY = cy + offY;

  // Draw rings near-to-far so the small, dark vanishing-point sits on top.
  // After each ring, narrow the clip region to that ring's interior so any
  // smaller (and offset) ring drawn next stays contained inside it.
  ctx.save();
  for (let i = RING_COUNT - 1; i >= 0; i--) {
    const t = i / (RING_COUNT - 1); // 0 = far, 1 = near
    const ringR = radius * (0.05 + 0.95 * Math.pow(t, 1.4));

    // Curved tunnel: near rings stay near center, far rings slide to VP.
    const curve = Math.pow(1 - t, 1.6); // 1 at far, 0 at near
    const rx = cx + (vpX - cx) * curve;
    const ry = cy + (vpY - cy) * curve;

    const red   = Math.floor(lerp(58, 226, t));
    const green = Math.floor(lerp(24, 118, t));
    const blue  = Math.floor(lerp(28, 136, t));
    ctx.fillStyle = `rgb(${red}, ${green}, ${blue})`;

    ctx.beginPath();
    ctx.ellipse(rx, ry, ringR, ringR * 0.88, 0, 0, Math.PI * 2);
    ctx.fill();

    if (i > 2 && i % 2 === 0) {
      ctx.strokeStyle = `rgba(40, 10, 20, 0.25)`;
      ctx.lineWidth = 1.5;
      ctx.stroke();
    }

    // Constrain subsequent (smaller, deeper) rings to live inside this one.
    ctx.clip();
  }
  ctx.restore();

  // Vignette
  const grad = ctx.createRadialGradient(cx, cy, radius * 0.6, cx, cy, radius);
  grad.addColorStop(0, 'rgba(0,0,0,0)');
  grad.addColorStop(1, 'rgba(0,0,0,0.55)');
  ctx.fillStyle = grad;
  ctx.fillRect(rect.x, rect.y, rect.w, rect.h);

  ctx.restore();

  // Eyepiece border
  ctx.save();
  ctx.beginPath();
  ctx.arc(cx, cy, radius, 0, Math.PI * 2);
  ctx.strokeStyle = '#0a0408';
  ctx.lineWidth = 6;
  ctx.stroke();
  ctx.strokeStyle = '#3a2028';
  ctx.lineWidth = 2;
  ctx.stroke();
  ctx.restore();
}

function lerp(a, b, t) { return a + (b - a) * t; }
