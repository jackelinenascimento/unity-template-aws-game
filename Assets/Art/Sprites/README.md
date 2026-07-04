# 🖼️ Art / Sprites — Ctrl+Alt+Delas · Ecto-Escape

Sprites dos **obstáculos e itens coletáveis** do laboratório — movidos de `Docs/imgs/` para cá para que o Unity consiga importá-los.

---

## Arquivos

| Arquivo | Elemento do jogo | Papel |
|---|---|---|
| `chipdrive.png` | **Data Core** — pen drive holográfico coletável | Item coletável |
| `laser.png` | **Laser de Segurança** — barreira vermelha | Obstáculo |
| `poca.png` | **Ácido Fluorescente** — poça radioativa | Obstáculo |
| `polvina.png` | **Polvina / Drone** — robô de patrulha | Obstáculo |

---

## ⚙️ Como configurar no Unity (faça ao abrir o projeto)

Para cada sprite acima:

1. Selecione o arquivo no painel **Project**.
2. No **Inspector**:
   - `Texture Type` → **Sprite (2D and UI)**
   - `Sprite Mode` → **Single**
   - `Alpha Is Transparency` → ✅ marcado
3. Clique em **Apply**.

---

## 🕹️ Como usar na cena

### Data Core (chipdrive.png)
1. Arraste para a **Scene**, renomeie para `DataCore_01`.
2. `Add Component` → **CircleCollider2D** → marque **Is Trigger**.
3. `Add Component` → arraste o script **Collectible.cs**.
4. Repita para `DataCore_02` e `DataCore_03`.

### Obstáculos (laser, poca, polvina)
1. Arraste para a **Scene**, renomeie (`LaserSeguranca`, `AcidoFluorescente`, `DronePatrulha`).
2. `Add Component` → **BoxCollider2D** → marque **Is Trigger**.
3. `Add Component` → arraste o script **Obstacle.cs** (criar com Kiro se não existir).

---

## ⚠️ Atenção

- Os `.meta` já foram gerados — **não delete** nem mova os arquivos fora do Unity.
- Fundo transparente: confirme que `Alpha Is Transparency` está marcado.
- Tamanho recomendado de exibição: ajuste `Pixels Per Unit` no Inspector se o sprite parecer muito grande ou pequeno na cena.
