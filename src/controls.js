// Touch + keyboard input. Edge-triggered: each press fires onPress(ctrl) once.

const KEY_MAP = {
  ArrowUp: 'forward', ArrowDown: 'back',
  ArrowLeft: 'rotateCCW', ArrowRight: 'rotateCW',
  KeyW: 'up', KeyS: 'down', KeyA: 'left', KeyD: 'right',
  KeyQ: 'rotateCCW', KeyE: 'rotateCW',
  Space: 'forward', KeyF: 'forward',
  ShiftLeft: 'back', ShiftRight: 'back', KeyB: 'back',
};

export function bindControls(onPress) {
  const buttons = document.querySelectorAll('.ctrl-btn[data-ctrl]');
  buttons.forEach((btn) => {
    const ctrl = btn.dataset.ctrl;
    let pressed = false;

    const press = (e) => {
      e.preventDefault();
      if (pressed) return;
      pressed = true;
      btn.classList.add('pressed');
      onPress(ctrl);
    };
    const release = (e) => {
      if (e) e.preventDefault();
      pressed = false;
      btn.classList.remove('pressed');
    };

    btn.addEventListener('touchstart', press, { passive: false });
    btn.addEventListener('touchend', release, { passive: false });
    btn.addEventListener('touchcancel', release, { passive: false });
    btn.addEventListener('mousedown', press);
    btn.addEventListener('mouseup', release);
    btn.addEventListener('mouseleave', release);
  });

  // Keyboard: edge-triggered, no autorepeat
  const downKeys = new Set();
  window.addEventListener('keydown', (e) => {
    const ctrl = KEY_MAP[e.code];
    if (!ctrl) return;
    e.preventDefault();
    if (downKeys.has(e.code)) return;
    downKeys.add(e.code);
    onPress(ctrl);
  });
  window.addEventListener('keyup', (e) => {
    const ctrl = KEY_MAP[e.code];
    if (!ctrl) return;
    e.preventDefault();
    downKeys.delete(e.code);
  });

  // Prevent page scroll on the canvas area
  document.addEventListener('touchmove', (e) => {
    if (e.target.closest('#game')) e.preventDefault();
  }, { passive: false });
}
