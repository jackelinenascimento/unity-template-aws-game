# 🎨 Art — Ctrl+Alt+Delas · Ecto-Escape

Assets visuais do laboratório sci-fi onde Blink vive sua aventura. Cada subpasta tem papel específico — coloque os arquivos no lugar certo.

---

## 📂 Subpastas

| Pasta | Conteúdo atual | Quem gera | Quem importa |
|---|---|---|---|
| `Personagem/` | `personagem.png` — sprite do Blink | 🎨 Artista | 🕹️ Unity |
| `Cenários/` | 1 PNG fatiado em 2 sprites de fundo | 🏞️ Cenário | 🕹️ Unity |
| `Chão/` | 2 tilesets (~89 tiles) + `chao.prefab` | 🏞️ Cenário | 🕹️ Unity |
| `Decoração/` | 2 PNGs de props e decoração | 🏞️ Cenário | 🕹️ Unity |
| `Material/` | `Material.physicsMaterial2D` — fricção do Blink | ⚙️ Gameplay | — |
| `Animation/` | `Player.controller`, `idle.anim`, `walking.anim` | 🎨 Artista + 🕹️ Unity | — |

---

## 📐 Tamanhos padrão do Ecto-Escape

| Tipo | Tamanho | Observação |
|---|---|---|
| Tiles de chão / plataforma | **256 × 256 px** | Grade sem espaçamento |
| Blink (cada frame/expressão) | **512 × 512 px** | Mesmo tamanho em todos os frames |
| Obstáculos e Data Cores | **256 × 256 px** | — |
| Props de cenário / decoração | **256 × 256 px** ou múltiplo | — |
| HUD / UI | livre | — |

> Fundo **transparente** (`.png` com alpha) obrigatório em todos os sprites.

---

## 🖼️ Como importar sprites no Unity

1. Arraste o `.png` para a pasta correta no painel **Project** — nunca pelo explorador do sistema.
2. Selecione o arquivo importado.
3. No **Inspector**:
   - `Texture Type` → **Sprite (2D and UI)**
   - `Sprite Mode` → **Single** (1 sprite) ou **Multiple** (sprite sheet / tileset)
4. Se for sprite sheet ou tileset → **Sprite Editor → Slice → Grid by Cell Size** (ex: 256 × 256) → **Apply**.
5. Clique em **Apply** no Inspector.

---

## ⚠️ Regras importantes

- **Nunca mova arquivos fora do Unity** — os `.meta` ficam órfãos e quebra referências na cena.
- **Nunca coloque arquivos na raiz de `Art/`** — use sempre a subpasta correta.
- Ao commitar, inclua sempre o `.meta` junto com o arquivo original.
- Se um asset precisar de colisão física, configure-o na cena — não aqui.
