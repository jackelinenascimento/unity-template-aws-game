# 🧿 Art / Personagem

Sprites do **fantasminha do Kiro** — o personagem jogável.

---

## O que fica aqui

| Arquivo | Descrição |
|---------|-----------|
| `personagem.png` | Sprite principal (já existe) |
| `expressoes.png` | Sprite sheet com todas as expressões (gerar durante o evento) |
| Sprites fatiados | Gerados automaticamente pelo Sprite Editor do Unity |

---

## 🎨 Prompt para gerar o personagem (ChatGPT ou Kiro)

```
Cute friendly AI ghost mascot for a 2D platform game.
Front view only.
Flat vector illustration.
White body with subtle blue accents.
Rounded shape.
Transparent background.
Game-ready.
No text.
No watermark.
```

## 🎭 Prompt para gerar as expressões

```
Character expression sheet.
Same cute ghost.
Front view.
10 expressions:
idle, happy, sad, surprised, talking, thinking, excited, confused, celebrating, sleeping
Transparent background.
Flat vector.
Sprite sheet.
No text.
```

---

## ✂️ Como fatiar a sprite sheet no Unity

1. Importe o arquivo nesta pasta.
2. No Inspector: `Texture Type` → **Sprite (2D and UI)** | `Sprite Mode` → **Multiple**.
3. Clique em **Sprite Editor** → **Slice**.
4. Se as expressões estiverem em grade: `Grid by Cell Size` → ex: `512 × 512`.
5. **Slice** → **Apply**.
6. Os sprites individuais aparecem ao expandir o arquivo no Project.

---

## 🔗 Ligação com o Animator

Após fatiar, arraste os sprites para o painel **Animation** para criar os clipes.
O `PlayerController` usa o parâmetro `IsMoving` (Bool) para alternar idle ↔ walking.

> Tamanho recomendado por frame: **512 × 512 px**
