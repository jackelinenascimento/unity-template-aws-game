# 🟫 Art / Chão — Ctrl+Alt+Delas

Sprites de **chão e plataformas** — os elementos que o personagem pisa.
Diferente do cenário, estes precisam de **Collider2D** para funcionar com física.

---

## O que fica aqui

- Tileset do chão (sprite sheet com os blocos)
- Sprites de plataformas individuais
- Prefabs de plataformas prontas (com Collider2D configurado)

---

## 🏞️ Prompt para gerar o tileset (ChatGPT ou Kiro)

```
Create a modular 2D sci-fi laboratory floor tileset.
Exactly 20 square tiles.
Each tile exactly 256x256 pixels.
Perfect aligned grid.
No spacing.
No transparent padding.
Every tile must seamlessly connect.
Flat vector.
Transparent background.
No text.
```

---

## 🕹️ Como colocar chão na cena

### Jeito 1 — Objeto simples *(mais rápido)*
1. Hierarchy → botão direito → `2D Object > Sprite`
2. Renomeie para `Ground`
3. No Sprite Renderer, coloque o sprite de chão
4. `Add Component` → **Box Collider 2D**
5. Para alargar: `Transform > Scale X` (ex: `10`)

### Jeito 2 — Tilemap *(melhor para muitos blocos)*
1. Importe o tileset nesta pasta
2. No Sprite Editor, fatie em **Grid by Cell Size** (`256 × 256`)
3. `Window > 2D > Tile Palette` → **Create New Palette** → arraste os sprites
4. Pinte a fase com o pincel da Tile Palette
5. No GameObject do Tilemap: `Add Component` → **Tilemap Collider 2D**

---

## ⚙️ Configurações obrigatórias no chão

| Componente | Configuração |
|------------|-------------|
| `Sprite Renderer` | sprite do chão |
| `Box Collider 2D` | ajustado ao sprite |
| `Layer` | **Ground** (crie em Edit → Project Settings → Tags and Layers) |
| `Rigidbody2D` | **não adicionar** no chão |

> O chão fica parado. O `PlayerController` usa a Layer `Ground` para detectar quando pode pular.

---

## ⚠️ Atenção

- Tamanho de cada tile: **256 × 256 px**.
- Fundo transparente obrigatório.
- A Layer `Ground` **deve ser a mesma** configurada no campo `Ground Layer` do `PlayerController`.
