# ✨ Art / Decoração — Ctrl+Alt+Delas

Elementos **puramente visuais** que deixam a fase com cara própria do time.
Nenhum destes objetos bloqueia ou interage com o personagem — são só visuais.

---

## O que fica aqui

- Props decorativos (caixas, terminais, neons, plantas futuristas…)
- Efeitos de partícula (sparkles, explosões, portais)
- Ícones de UI e HUD
- Qualquer sprite que não seja personagem, chão ou cenário estrutural

---

## 🎨 Prompts para gerar (ChatGPT ou Kiro)

### Props e decoração
```
2D sci-fi laboratory decoration props.
Crates, neon signs, holographic panels, plants, cables.
Transparent background.
Flat vector.
Game-ready.
No text.
```

### Itens coletáveis
```
2D collectible asset sheet.
Microchip, Energy crystal, Byte cube.
Transparent background.
Flat vector.
Game-ready.
```

### Obstáculos
```
2D game asset sheet.
3 obstacles: Energy barrier, Laser emitter, Broken robot.
Transparent background.
Flat vector.
Game-ready.
```

### Efeitos de partícula
```
2D VFX asset sheet.
Sparkles, explosion, energy burst, cloud puff, portal.
Transparent background.
Flat vector.
Game-ready.
```

### HUD / Interface
```
Futuristic game UI kit.
Score panel, timer, health bar, victory panel, game over panel.
Transparent background.
Flat vector.
Game-ready.
```

---

## 🕹️ Como colocar na cena

1. Arraste o sprite para a **Scene**.
2. Posicione com o **Transform**.
3. Ajuste `Order in Layer` no Sprite Renderer para controlar profundidade.
4. **Sem Collider2D** — a menos que seja um obstáculo (aí vai para a cena com script `Obstacle`).

---

## ⚠️ Atenção

- Tamanho recomendado: **256 × 256 px**.
- Fundo transparente obrigatório.
- Itens coletáveis precisam do script `Collectible.cs` e `Collider2D` com **Is Trigger** — configure na cena, não aqui.
