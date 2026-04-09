// ...And Around the Corner — main entry point.

import { createGameState, resetGameState, update, handlePress, lateralLocked } from './gameState.js';
import { bindControls } from './controls.js';
import { renderLumen } from './lumenView.js';
import { renderProgress } from './progressView.js';
import { bindRestart, showOverlay, hideOverlay, renderSegmentBanner } from './hud.js';

const canvas = document.getElementById('gameCanvas');
const ctx = canvas.getContext('2d');

const gameState = createGameState();
let lastOverlayPhase = 'playing';

const leftBtn = document.querySelector('[data-ctrl="left"]');
const rightBtn = document.querySelector('[data-ctrl="right"]');

bindControls((ctrl) => handlePress(gameState, ctrl));

bindRestart(() => {
  resetGameState(gameState);
  hideOverlay();
  lastOverlayPhase = 'playing';
});

function resizeIfNeeded() {
  const dpr = window.devicePixelRatio || 1;
  const cssW = canvas.clientWidth;
  const cssH = canvas.clientHeight;
  const wantW = Math.floor(cssW * dpr);
  const wantH = Math.floor(cssH * dpr);
  if (canvas.width !== wantW || canvas.height !== wantH) {
    canvas.width = wantW;
    canvas.height = wantH;
    ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
  }
}
window.addEventListener('resize', resizeIfNeeded);
resizeIfNeeded();

let lastT = performance.now();
function frame(now) {
  const dt = Math.min(0.05, (now - lastT) / 1000);
  lastT = now;

  resizeIfNeeded();
  update(gameState, dt);

  const W = canvas.clientWidth;
  const H = canvas.clientHeight;
  const progressRect = { x: 0, y: 0, w: W, h: Math.round(H * 0.22) };
  const lumenRect    = { x: 0, y: progressRect.h, w: W, h: H - progressRect.h };

  ctx.clearRect(0, 0, W, H);
  renderLumen(ctx, lumenRect, gameState.lumenOffset);
  renderProgress(ctx, progressRect, gameState);
  renderSegmentBanner(ctx, lumenRect, gameState);

  const locked = lateralLocked(gameState);
  leftBtn.classList.toggle('disabled', locked);
  rightBtn.classList.toggle('disabled', locked);

  if (gameState.phase !== 'playing' && lastOverlayPhase === 'playing') {
    showOverlay(gameState.phase);
    lastOverlayPhase = gameState.phase;
  }

  requestAnimationFrame(frame);
}
requestAnimationFrame(frame);
