# 🧱 Art / Material — Ctrl+Alt+Delas · Ecto-Escape

Physics Materials 2D do projeto — controlam como o Blink desliza ou freia ao encostar em superfícies.

---

## Arquivo existente

| Arquivo | O que faz |
|---|---|
| `Material.physicsMaterial2D` | Physics Material 2D aplicado ao Collider do Blink |

---

## O que é um Physics Material 2D

Um `Physics Material 2D` define duas propriedades de colisão:

| Propriedade | Efeito |
|---|---|
| **Friction** | Quanto o objeto desacelera ao deslizar sobre uma superfície (0 = gelo, 1 = borracha) |
| **Bounciness** | Quanto o objeto quica ao colidir (0 = sem quique, 1 = quique total) |

---

## Como aplicar ao Blink

1. Selecione o **Player** na Hierarchy.
2. No Inspector, clique no `Collider2D` (BoxCollider2D ou CapsuleCollider2D).
3. No campo **Material**, arraste `Material.physicsMaterial2D`.

---

## Quando editar

- Se o Blink está **deslizando demais nas paredes** → aumente o `Friction`.
- Se o Blink está **pregando nas bordas das plataformas** → reduza o `Friction` para `0` (deixa o Rigidbody2D cuidar disso).
- Se quiser que o Blink **quique ao cair** → aumente o `Bounciness` (não recomendado para plataformer padrão).

> Para a maioria dos plataformers 2D, a configuração recomendada é `Friction: 0` e `Bounciness: 0` — a movimentação fica totalmente controlada pelo script.
