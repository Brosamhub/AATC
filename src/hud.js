// Win/lose overlay + current segment banner.

import { getStep } from './colon.js';

const overlay = document.getElementById('overlay');
const overlayTitle = document.getElementById('overlay-title');
const overlayMsg = document.getElementById('overlay-msg');
const restartBtn = document.getElementById('restart-btn');

export function bindRestart(handler) {
  restartBtn.addEventListener('click', handler);
  restartBtn.addEventListener('touchend', (e) => { e.preventDefault(); handler(); });
}

export function showOverlay(phase) {
  if (phase === 'won') {
    overlayTitle.textContent = 'Cecum Reached!';
    overlayMsg.textContent = 'Excellent scoping. The patient thanks you.';
  } else if (phase === 'lost') {
    overlayTitle.textContent = 'Patient Discomfort';
    overlayMsg.textContent = 'The patient could not tolerate the procedure. Try steering more carefully.';
  }
  overlay.classList.remove('hidden');
}

export function hideOverlay() {
  overlay.classList.add('hidden');
}

export function renderSegmentBanner(ctx, rect, gameState) {
  const step = getStep(gameState.stepIndex);
  let label = step.name;
  if (step.bend && !gameState.bendSolved) {
    label += `  →  aim ${step.bend.toUpperCase()}`;
  } else if (step.bend && gameState.bendSolved) {
    label += `  ✓  press FWD`;
  }

  ctx.save();
  ctx.font = 'bold 14px -apple-system, sans-serif';
  const textW = ctx.measureText(label).width;
  const padX = 12, padY = 6;
  const bx = rect.x + rect.w / 2 - (textW + padX * 2) / 2;
  const by = rect.y + 8;

  ctx.fillStyle = 'rgba(10, 4, 8, 0.75)';
  ctx.strokeStyle = '#5a3040';
  ctx.lineWidth = 1.5;
  roundRect(ctx, bx, by, textW + padX * 2, 22 + padY, 6);
  ctx.fill();
  ctx.stroke();

  let color = '#f4dde4';
  if (step.bend && !gameState.bendSolved) color = '#ffd04a';
  else if (step.bend && gameState.bendSolved) color = '#7fffa0';
  ctx.fillStyle = color;
  ctx.textBaseline = 'top';
  ctx.fillText(label, bx + padX, by + padY + 2);
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
