const canvas = document.getElementById('game');
const ctx = canvas.getContext('2d');

const keys = {};
document.addEventListener('keydown', e => {
  keys[e.code] = true;
  if (e.code === 'Space') {
    shoot();
  }
});
document.addEventListener('keyup', e => {
  keys[e.code] = false;
});

const tank = { x: canvas.width / 2, y: canvas.height / 2, angle: 0, size: 35, speed: 2 };
const bullets = [];
let obstacles = generateLevel();

function generateLevel() {
  const themes = ['forest', 'desert', 'urban'];
  const theme = themes[Math.floor(Math.random() * themes.length)];
  document.body.className = theme;
  const obs = [];
  for (let i = 0; i < 20; i++) {
    const type = Math.random() < 0.6 ? 'tree' : 'building';
    if (type === 'tree') {
      obs.push({ type: 'tree', x: Math.random() * canvas.width, y: Math.random() * canvas.height, r: 15, health: 1 });
    } else {
      obs.push({ type: 'building', x: Math.random() * (canvas.width - 40), y: Math.random() * (canvas.height - 40), w: 40, h: 40, health: 3 });
    }
  }
  return obs;
}

function shoot() {
  bullets.push({ x: tank.x, y: tank.y, angle: tank.angle, speed: 5 });
}

function circleRectCollision(cx, cy, r, rx, ry, rw, rh) {
  const closestX = Math.max(rx, Math.min(cx, rx + rw));
  const closestY = Math.max(ry, Math.min(cy, ry + rh));
  const dx = cx - closestX;
  const dy = cy - closestY;
  return (dx * dx + dy * dy) < r * r;
}

function update() {
  let nx = tank.x;
  let ny = tank.y;
  if (keys['ArrowLeft']) tank.angle -= 0.05;
  if (keys['ArrowRight']) tank.angle += 0.05;
  if (keys['ArrowUp']) {
    nx += Math.cos(tank.angle) * tank.speed;
    ny += Math.sin(tank.angle) * tank.speed;
  }
  if (keys['ArrowDown']) {
    nx -= Math.cos(tank.angle) * tank.speed;
    ny -= Math.sin(tank.angle) * tank.speed;
  }
  let blocked = false;
  obstacles.forEach(o => {
    if (o.type === 'building') {
      if (circleRectCollision(nx, ny, tank.size, o.x, o.y, o.w, o.h)) {
        blocked = true;
      }
    }
  });
  if (!blocked) {
    tank.x = nx;
    tank.y = ny;
  }

  bullets.forEach((b, i) => {
    b.x += Math.cos(b.angle) * b.speed;
    b.y += Math.sin(b.angle) * b.speed;
    if (b.x < 0 || b.x > canvas.width || b.y < 0 || b.y > canvas.height) {
      bullets.splice(i, 1);
    }
  });

  bullets.forEach((b, bi) => {
    obstacles.forEach((o, oi) => {
      if (o.type === 'tree') {
        const dx = b.x - o.x;
        const dy = b.y - o.y;
        if (Math.hypot(dx, dy) < o.r) {
          o.health--;
          bullets.splice(bi, 1);
          if (o.health <= 0) obstacles.splice(oi, 1);
        }
      } else if (o.type === 'building') {
        if (b.x > o.x && b.x < o.x + o.w && b.y > o.y && b.y < o.y + o.h) {
          o.health--;
          bullets.splice(bi, 1);
          if (o.health <= 0) {
            o.type = 'rubble';
          }
        }
      }
    });
  });

  tank.x = Math.max(tank.size, Math.min(canvas.width - tank.size, tank.x));
  tank.y = Math.max(tank.size, Math.min(canvas.height - tank.size, tank.y));
}

function draw() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);

  obstacles.forEach(o => {
    if (o.type === 'tree') {
      ctx.fillStyle = 'green';
      ctx.beginPath();
      ctx.arc(o.x, o.y, o.r, 0, Math.PI * 2);
      ctx.fill();
    } else if (o.type === 'building') {
      ctx.fillStyle = 'gray';
      ctx.fillRect(o.x, o.y, o.w, o.h);
    } else if (o.type === 'rubble') {
      ctx.fillStyle = '#555';
      ctx.fillRect(o.x, o.y, o.w, o.h);
    }
  });

  ctx.save();
  ctx.translate(tank.x, tank.y);
  ctx.rotate(tank.angle);
  ctx.fillStyle = 'olive';
  ctx.beginPath();
  ctx.arc(0, 0, tank.size, 0, Math.PI * 2);
  ctx.fill();
  ctx.strokeStyle = 'black';
  ctx.lineWidth = 8;
  ctx.beginPath();
  ctx.moveTo(0, 0);
  ctx.lineTo(tank.size, 0);
  ctx.stroke();
  ctx.restore();

  ctx.fillStyle = 'black';
  bullets.forEach(b => {
    ctx.beginPath();
    ctx.arc(b.x, b.y, 3, 0, Math.PI * 2);
    ctx.fill();
  });
}

function loop() {
  update();
  draw();
  requestAnimationFrame(loop);
}
loop();
