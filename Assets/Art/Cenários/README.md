# 🏙️ Art / Cenários — Ctrl+Alt+Delas

Assets de **fundo e estrutura visual** da fase — paredes, tubos, computadores, terminais, caixas.
Estes elementos são **puramente visuais**: sem Collider2D, o personagem passa por eles.

---

## O que fica aqui

- Imagem de fundo (background) da fase
- Elementos decorativos grandes (paredes, estruturas futuristas)
- Prefabs de decoração de cenário prontos para arrastar na cena

> Plataformas e chão que o personagem pisa ficam em `Chão/`.

---

## 🏞️ Prompt para gerar (ChatGPT ou Kiro)

```
Modular sci-fi laboratory assets.
Transparent background.
Walls.
Platforms.
Doors.
Pipes.
Computers.
Terminals.
Crates.
Game-ready.
Flat vector.
No text.
```

---

## 🕹️ Como colocar na cena (sem colisão)

1. Arraste o sprite para a **Scene**.
2. Posicione com o **Transform** onde quiser.
3. **Pronto** — nenhum componente extra necessário.

### Profundidade (quem aparece na frente)
Se o personagem sumir atrás de um elemento, ajuste no **Sprite Renderer**:
- `Sorting Layer` → crie: `Background` < `Default` < `Foreground`
- `Order in Layer` → número maior = mais na frente

---

## ⚠️ Atenção

- Fundo transparente obrigatório.
- Tamanho recomendado: **256 × 256 px** (ou múltiplo para elementos grandes).
- Não adicione Collider2D aqui. Se o elemento precisar de colisão física, trate como plataforma em `Chão/`.
