# ⚙️ Scripts

Código C# do jogo. Todos os scripts já estão prontos e funcionando — **não reescreva do zero**, use o Kiro para editar ou estender o que existe.

---

## 📄 Scripts existentes

### `PlayerController.cs` — 🕹️ Movimento do personagem
Controla o fantasminha usando os **botões físicos do ESP32** via Serial, com fallback automático para teclado/gamepad.

| Campo no Inspector | O que faz |
|--------------------|-----------|
| `Move Speed` | Velocidade horizontal |
| `Jump Force` | Força do pulo |
| `Ground Layer` | Layer dos objetos de chão |
| `Ground Check` | Transform filho nos pés do personagem |
| `Force Keyboard Fallback` | ✅ Marque para testar sem o ESP32 |

**Parâmetro Animator obrigatório:** `IsMoving` (Bool)

---

### `ESP32SerialReader.cs` — 🔌 Comunicação com o hardware
Singleton. Gerencia a comunicação bidirecional pela porta USB.

**ESP32 → Unity (botões):**
| Mensagem | Efeito |
|----------|--------|
| `L1` / `L0` | Esquerda pressionada / solta |
| `R1` / `R0` | Direita pressionada / solta |
| `U` | Pulo |

**Unity → ESP32 (carinhas no OLED):**
| Método | Letra enviada | Carinha |
|--------|:---:|---------|
| `SendIdle()` | `I` | Neutra `(O_O)` |
| `SendTreasure()` | `T` | Feliz `(^u^)` |
| `SendDamage()` | `D` | Assustada `(>_<)` |

> ⚠️ Configure `Port Name` no Inspector: Windows → `COM3`, Mac/Linux → `/dev/tty.usbserial-XXXX`

---

### `GameManager.cs` — 🏆 Estado do jogo
Singleton. Conta os coletáveis e mostra o painel de fim de jogo quando o último é coletado.

| Campo no Inspector | O que faz |
|--------------------|-----------|
| `Game Over Panel` | Painel UI que aparece ao vencer |
| `Scene Name` | Nome da cena para reiniciar |

---

### `Collectible.cs` — ⭐ Item coletável
Coloque em qualquer item da cena. Requer `Collider2D` com **Is Trigger** marcado.

Ao ser coletado: avisa o `GameManager` → envia `T` para o OLED → toca efeito/som (opcionais) → some.

> O Player precisa ter a **Tag** `Player`.

---

### `LevelEnd.cs` — 🏁 Final da fase
Coloque no objeto do fim da fase. Requer `Collider2D` com **Is Trigger** marcado.

Ao o Player tocar: exibe painel de fim → pausa o jogo (`Time.timeScale = 0`) → envia `T` para o OLED.

---

### `MovingPlatform.cs` — 🔼 Plataforma móvel
Sobe e desce entre dois pontos. Requer `Rigidbody2D` com **Body Type: Kinematic**.

| Campo | O que faz |
|-------|-----------|
| `Move Distance` | Distância percorrida (para cima) |
| `Speed` | Velocidade |
| `Wait Time` | Pausa nos extremos |

---

## 💡 Como pedir ao Kiro para criar um novo script

```
Read the existing Collectible script for reference.
Create a new [Nome] script that [descrição do comportamento].
Keep it simple and consistent with the existing code style.
Explain every change.
```

> Regra de ouro: **uma alteração de cada vez** → testa → só então pede a próxima.
