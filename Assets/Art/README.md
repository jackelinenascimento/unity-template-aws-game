# 🎨 Art

Todos os assets visuais do jogo. Cada subpasta tem um papel específico — coloque os arquivos no lugar certo para o time conseguir se localizar.

---

## 📂 Subpastas

| Pasta | Conteúdo | Quem gera | Quem importa |
|-------|----------|-----------|--------------|
| `Personagem/` | Sprite do fantasminha e suas expressões | 🎨 Artista | 🕹️ Unity |
| `Cenários/` | Fundos, paredes, estruturas de fundo | 🏞️ Cenário | 🕹️ Unity |
| `Chão/` | Tiles e plataformas (com física) | 🏞️ Cenário | 🕹️ Unity |
| `Decoração/` | Elementos decorativos sem colisão | 🏞️ Cenário | 🕹️ Unity |
| `Animation/` | Controllers e clipes de animação | 🎨 Artista · 🕹️ Unity | — |

---

## 📐 Tamanhos recomendados

| Tipo de asset | Tamanho |
|---------------|---------|
| Tiles de chão / plataforma | **256 × 256 px** |
| Personagem (cada frame) | **512 × 512 px** |
| Obstáculos e itens coletáveis | **256 × 256 px** |
| Elementos de cenário / decoração | **256 × 256 px** (ou múltiplo) |
| HUD / UI | tamanho livre |

> Padronize o tamanho **dentro de cada categoria** — misturar escalas diferentes causa dor de cabeça na hora de montar a fase.

---

## 🖼️ Como importar sprites corretamente no Unity

1. Arraste o arquivo `.png` para a pasta correta no painel **Project**.
2. Selecione o arquivo importado.
3. No **Inspector**, configure:
   - `Texture Type` → **Sprite (2D and UI)**
   - `Sprite Mode` → **Single** (1 sprite) ou **Multiple** (sprite sheet)
4. Se for sprite sheet → clique em **Sprite Editor** → **Slice** → **Apply**.
5. Clique em **Apply** no Inspector.

---

## ⚠️ Atenção

- **Fundo transparente** (`.png` com alpha) é obrigatório para todos os sprites do jogo.
- Nunca coloque arquivos diretamente na raiz de `Art/` — use sempre a subpasta correta.
- Arquivos `.meta` são gerados automaticamente — não delete e não mova fora do Unity.
