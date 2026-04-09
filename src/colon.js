// The colon is a linear sequence of "steps". Each step is either straight
// (just press forward) or a bend that requires the player to aim in a
// specific direction before advancing.

export const colonSteps = [
  { name: 'Rectum',           bend: null  },
  { name: 'Rectum',           bend: null  },
  { name: 'Sigmoid Colon',    bend: 'left'  },
  { name: 'Sigmoid Colon',    bend: 'up'    },
  { name: 'Sigmoid Colon',    bend: null  },
  { name: 'Descending Colon', bend: null  },
  { name: 'Descending Colon', bend: null  },
  { name: 'Splenic Flexure',  bend: 'right' },
  { name: 'Splenic Flexure',  bend: 'up'    },
  { name: 'Transverse Colon', bend: null  },
  { name: 'Transverse Colon', bend: null  },
  { name: 'Hepatic Flexure',  bend: 'down'  },
  { name: 'Hepatic Flexure',  bend: 'left'  },
  { name: 'Ascending Colon',  bend: null  },
  { name: 'Ascending Colon',  bend: null  },
  { name: 'Cecum',            bend: null  },
];

export function totalSteps() {
  return colonSteps.length;
}

export function getStep(index) {
  const i = Math.max(0, Math.min(index, colonSteps.length - 1));
  return colonSteps[i];
}

/** Unit vector for a bend direction. +x right, +y down (screen coords). */
export const DIRECTION_VECTORS = {
  up:    { x:  0, y: -1 },
  down:  { x:  0, y:  1 },
  left:  { x: -1, y:  0 },
  right: { x:  1, y:  0 },
};
