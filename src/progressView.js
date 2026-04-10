// Top strip: cartoon colon diagram with a scope marker, plus pain and loop meters.

import { totalSteps } from './colon.js';

// Normalized keyframes for the cartoon colon, oriented as if you are looking
// AT the patient (anatomical convention: patient's left = viewer's right).
// Rectum at bottom center, sigmoid curls to the viewer's right, descending
// runs up the viewer's right, transverse across the top, ascending down the
// viewer's left, cecum at the bottom of the ascending side.
const COLON_KEYFRAMES = [
  [0.50, 0.95], // rectum
  [0.55, 0.80], // sigmoid start
  [0.70, 0.70], // sigmoid loop
  [0.78, 0.55], // descending start
  [0.80, 0.30], // descending
  [0.78, 0.15], // splenic flexure
  [0.60, 0.12], // transverse
  [0.40, 0.14], // transverse
  [0.22, 0.15], // hepatic flexure
  [0.20, 0.30], // ascending
  [0.22, 0.55], // ascending end
  [0.20, 0.75], // cecum
];

export function renderProgress(ctx, rect, gameState) {
  const { stepIndex, painMeter, loopMeter } = gameState;

  // Background
  ctx.fillStyle = '#120a0e';
  ctx.fillRect(rect.x, rect.y, rect.w, rect.h);

  // Split: left 60% diagram, right 40% meters
  const diagW = rect.w * 0.60;
  const diagRect = { x: rect.x + 8, y: rect.y + 8, w: diagW - 16, h: rect.h - 16 };
  const metersRect = { x: rect.x + diagW, y: rect.y + 8, w: rect.w - diagW - 12, h: rect.h - 16 };

  drawColonDiagram(ctx, diagRect, stepIndex);
  drawMeters(ctx, metersRect, painMeter, loopMeter);
}

function drawColonDiagram(ctx, rect, stepIndex) {
  const pts = COLON_KEYFRAMES.map(([nx, ny]) => ({
    x: rect.x + nx * rect.w,
    y: rect.y + ny * rect.h,
  }));

  ctx.save();
  ctx.lineCap = 'round';
  ctx.lineJoin = 'round';

  ctx.strokeStyle = '#7a3a50';
  ctx.lineWidth = 18;
  drawSmoothPath(ctx, pts);

  ctx.strokeStyle = '#e27488';
  ctx.lineWidth = 10;
  drawSmoothPath(ctx, pts);

  ctx.strokeStyle = '#ffd8df';
  ctx.lineWidth = 2;
  drawSmoothPath(ctx, pts);

  // Scope marker: interpolate along the polyline using stepIndex fraction.
  const t = totalSteps() > 1 ? stepIndex / (totalSteps() - 1) : 0;
  const markerPos = sampleAlongPath(pts, t);
  ctx.fillStyle = '#ffe84a';
  ctx.strokeStyle = '#1a0608';
  ctx.lineWidth = 2;
  ctx.beginPath();
  ctx.arc(markerPos.x, markerPos.y, 6, 0, Math.PI * 2);
  ctx.fill();
  ctx.stroke();

  ctx.restore();
}

function drawSmoothPath(ctx, pts) {
  if (pts.length < 2) return;
  ctx.beginPath();
  ctx.moveTo(pts[0].x, pts[0].y);
  for (let i = 1; i < pts.length - 1; i++) {
    const mx = (pts[i].x + pts[i + 1].x) / 2;
    const my = (pts[i].y + pts[i + 1].y) / 2;
    ctx.quadraticCurveTo(pts[i].x, pts[i].y, mx, my);
  }
  const last = pts[pts.length - 1];
  ctx.lineTo(last.x, last.y);
  ctx.stroke();
}

function sampleAlongPath(pts, t) {
  t = Math.max(0, Math.min(1, t));
  let total = 0;
  const lens = [];
  for (let i = 0; i < pts.length - 1; i++) {
    const dx = pts[i + 1].x - pts[i].x;
    const dy = pts[i + 1].y - pts[i].y;
    const l = Math.hypot(dx, dy);
    lens.push(l);
    total += l;
  }
  let target = t * total;
  for (let i = 0; i < lens.length; i++) {
    if (target <= lens[i]) {
      const local = lens[i] === 0 ? 0 : target / lens[i];
      return {
        x: pts[i].x + (pts[i + 1].x - pts[i].x) * local,
        y: pts[i].y + (pts[i + 1].y - pts[i].y) * local,
      };
    }
    target -= lens[i];
  }
  return pts[pts.length - 1];
}

function drawMeters(ctx, rect, pain, loop) {
  const barH = 14;
  const gap = 10;
  const labelGap = 16;
  let y = rect.y + 18;

  drawMeter(ctx, rect.x, y, rect.w, barH, 'PAIN', pain, '#ff4a5e');
  y += barH + labelGap + gap;
  drawMeter(ctx, rect.x, y, rect.w, barH, 'LOOP', loop, '#ffc84a');
}

function drawMeter(ctx, x, y, w, h, label, value, color) {
  ctx.save();
  ctx.fillStyle = '#f0d6dc';
  ctx.font = 'bold 11px -apple-system, sans-serif';
  ctx.textBaseline = 'bottom';
  ctx.fillText(label, x, y - 2);

  ctx.fillStyle = '#2a1620';
  roundRect(ctx, x, y, w, h, 4);
  ctx.fill();

  const fillW = Math.max(0, Math.min(1, value)) * (w - 4);
  if (fillW > 0) {
    ctx.fillStyle = color;
    roundRect(ctx, x + 2, y + 2, fillW, h - 4, 3);
    ctx.fill();
  }

  ctx.strokeStyle = '#5a3040';
  ctx.lineWidth = 1.5;
  roundRect(ctx, x, y, w, h, 4);
  ctx.stroke();
  ctx.restore();
}

function roundRect(ctx, x, y, w, h, r) {
  ctx.beginPath();
  ctx.moveTo(x + r, y);
  ctx.arcTo(x + w, y, x + w, y + h, r);
  ctx.arcTo(x + w, y + h, x, y + h, r);
  ctx.arcTo(x, y + h, x, y, r);
  ctx.arcTo(x, y, x + w, y, r);
  ctx.closePath();
}
