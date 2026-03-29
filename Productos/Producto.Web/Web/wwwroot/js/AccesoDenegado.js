(() => {
    const zone = document.getElementById("chaosZone");
    if (!zone) return;

    const sprites = [];
    const maxSprites = 120;
    const spawnEveryMs = 260;
    const friction = 0.999;
    const wallBounce = 0.92;
    const collisionBounce = 0.96;
    const settleThreshold = 0.035;
    const imageSrc = "/img/caine.webp";

    let lastSpawn = 0;
    let lastTime = performance.now();
    let simulationLocked = false;

    function rand(min, max) {
        return Math.random() * (max - min) + min;
    }

    function clamp(value, min, max) {
        return Math.max(min, Math.min(max, value));
    }

    function getZoneBounds() {
        return {
            width: zone.clientWidth,
            height: zone.clientHeight
        };
    }

    function canSpawn(size, bounds) {
        const padding = 8;

        for (let attempt = 0; attempt < 160; attempt++) {
            const x = rand(padding, bounds.width - size - padding);
            const y = rand(padding, bounds.height - size - padding);

            let overlaps = false;

            for (const s of sprites) {
                const dx = (x + size / 2) - (s.x + s.size / 2);
                const dy = (y + size / 2) - (s.y + s.size / 2);
                const dist = Math.hypot(dx, dy);
                const minDist = (size / 2) + (s.size / 2) + 4;

                if (dist < minDist) {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps) {
                return { x, y };
            }
        }

        return null;
    }

    function createSprite() {
        if (simulationLocked || sprites.length >= maxSprites) return;

        const bounds = getZoneBounds();
        const size = rand(52, 98);
        const spot = canSpawn(size, bounds);

        if (!spot) {
            const avgSpeed = sprites.length
                ? sprites.reduce((acc, s) => acc + Math.hypot(s.vx, s.vy), 0) / sprites.length
                : 0;

            if (avgSpeed < 0.12 || sprites.length > 45) {
                simulationLocked = true;
                for (const s of sprites) {
                    s.vx *= 0.25;
                    s.vy *= 0.25;
                }
            }
            return;
        }

        const el = document.createElement("img");
        el.src = imageSrc;
        el.alt = "Caine";
        el.className = "physics-caine";
        el.style.width = `${size}px`;
        el.style.height = `${size}px`;

        zone.appendChild(el);

        const speed = rand(0.8, 2.2);
        const angle = rand(0, Math.PI * 2);

        sprites.push({
            el,
            x: spot.x,
            y: spot.y,
            size,
            vx: Math.cos(angle) * speed,
            vy: Math.sin(angle) * speed,
            rotation: rand(0, 360),
            rotationSpeed: rand(-3, 3),
            mass: size * 0.1,
            sleeping: false,
            scaleX: 1,
            scaleY: 1,
            squashX: 0,
            squashY: 0
        });
    }

    function resolveWallCollision(s, bounds) {
        if (s.x <= 0) {
            s.x = 0;
            s.vx = Math.abs(s.vx) * wallBounce;
        }

        if (s.x + s.size >= bounds.width) {
            s.x = bounds.width - s.size;
            s.vx = -Math.abs(s.vx) * wallBounce;
        }

        if (s.y <= 0) {
            s.y = 0;
            s.vy = Math.abs(s.vy) * wallBounce;
        }

        if (s.y + s.size >= bounds.height) {
            s.y = bounds.height - s.size;
            s.vy = -Math.abs(s.vy) * wallBounce;
        }
    }

    function applySquash(sprite, nx, ny, strength) {
        const squashAmount = Math.min(0.22, strength);

        sprite.squashX += Math.abs(nx) * squashAmount;
        sprite.squashY += Math.abs(ny) * squashAmount;

        sprite.squashX = Math.min(sprite.squashX, 0.22);
        sprite.squashY = Math.min(sprite.squashY, 0.22);

        sprite.sleeping = false;
    }

    function resolveSpriteCollision(a, b) {
        const ax = a.x + a.size / 2;
        const ay = a.y + a.size / 2;
        const bx = b.x + b.size / 2;
        const by = b.y + b.size / 2;

        let dx = bx - ax;
        let dy = by - ay;
        let dist = Math.hypot(dx, dy);

        const minDist = (a.size / 2) + (b.size / 2);

        if (dist === 0) {
            dx = rand(-0.5, 0.5);
            dy = rand(-0.5, 0.5);
            dist = Math.hypot(dx, dy);
        }

        if (dist < minDist) {
            const nx = dx / dist;
            const ny = dy / dist;
            const overlap = minDist - dist;

            const totalMass = a.mass + b.mass;
            const pushA = (b.mass / totalMass) * overlap;
            const pushB = (a.mass / totalMass) * overlap;

            a.x -= nx * pushA;
            a.y -= ny * pushA;
            b.x += nx * pushB;
            b.y += ny * pushB;

            const rvx = b.vx - a.vx;
            const rvy = b.vy - a.vy;
            const velAlongNormal = rvx * nx + rvy * ny;

            if (velAlongNormal < 0) {
                const impulse = -(1 + collisionBounce) * velAlongNormal / ((1 / a.mass) + (1 / b.mass));
                const ix = impulse * nx;
                const iy = impulse * ny;

                a.vx -= ix / a.mass;
                a.vy -= iy / a.mass;
                b.vx += ix / b.mass;
                b.vy += iy / b.mass;

                const squashStrength = Math.abs(velAlongNormal) * 0.045;
                applySquash(a, nx, ny, squashStrength);
                applySquash(b, nx, ny, squashStrength);
            }

            const tangentSpin = (a.vx - b.vx) * ny - (a.vy - b.vy) * nx;
            a.rotationSpeed += tangentSpin * 0.12;
            b.rotationSpeed -= tangentSpin * 0.12;

            a.sleeping = false;
            b.sleeping = false;
        }
    }

    function update(dt) {
        const bounds = getZoneBounds();

        for (const s of sprites) {
            if (!s.sleeping) {
                s.x += s.vx * dt;
                s.y += s.vy * dt;

                s.vx *= Math.pow(friction, dt);
                s.vy *= Math.pow(friction, dt);
                s.rotation += s.rotationSpeed * dt;
                s.rotationSpeed *= 0.992;

                resolveWallCollision(s, bounds);

                const speed = Math.hypot(s.vx, s.vy);
                if (speed < settleThreshold && Math.abs(s.rotationSpeed) < 0.05 && simulationLocked) {
                    s.vx = 0;
                    s.vy = 0;
                    s.rotationSpeed = 0;
                    s.sleeping = true;
                }
            }

            s.squashX *= Math.pow(0.82, dt);
            s.squashY *= Math.pow(0.82, dt);

            s.scaleX += ((1 + s.squashY - s.squashX) - s.scaleX) * 0.22 * dt;
            s.scaleY += ((1 + s.squashX - s.squashY) - s.scaleY) * 0.22 * dt;
        }

        for (let i = 0; i < sprites.length; i++) {
            for (let j = i + 1; j < sprites.length; j++) {
                resolveSpriteCollision(sprites[i], sprites[j]);
            }
        }

        for (const s of sprites) {
            s.el.style.transform = `translate(${s.x}px, ${s.y}px) rotate(${s.rotation}deg) scale(${s.scaleX}, ${s.scaleY})`;
        }
    }

    function loop(now) {
        const dt = Math.min((now - lastTime) / 16.6667, 2);
        lastTime = now;

        if (!simulationLocked && now - lastSpawn >= spawnEveryMs) {
            createSprite();
            lastSpawn = now;
        }

        update(dt);
        requestAnimationFrame(loop);
    }

    function boot() {
        zone.innerHTML = "";
        sprites.length = 0;
        simulationLocked = false;
        lastSpawn = 0;
        lastTime = performance.now();

        for (let i = 0; i < 4; i++) {
            createSprite();
        }

        requestAnimationFrame(loop);
    }

    window.addEventListener("resize", () => {
        const bounds = getZoneBounds();
        for (const s of sprites) {
            s.x = clamp(s.x, 0, Math.max(0, bounds.width - s.size));
            s.y = clamp(s.y, 0, Math.max(0, bounds.height - s.size));
        }
    });

    boot();
})();