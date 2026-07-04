# ⚙️ Scripts — Ctrl+Alt+Delas · Ecto-Escape

Código C# do jogo. Todos os scripts estão prontos e funcionando — **não reescreva do zero**, use o Kiro para editar ou estender o que existe.

---

## 📄 Mapa dos scripts

| Script | Papel no jogo | Singleton? |
|---|---|:---:|
| `PlayerController.cs` | Movimento do Blink (ESP32 + teclado fallback) | não |
| `ESP32SerialReader.cs` | Comunicação USB Serial bidirecional | ✅ |
| `GameManager.cs` | Conta Data Cores, exibe vitória | ✅ |
| `Collectible.cs` | Lógica de cada Data Core coletável | não |
| `HealthSystem.cs` | Vidas do Blink, invencibilidade e morte | não |
| `HUDManager.cs` | HUD de score e Polvinas | ✅ |
| `DamageZone.cs` | Zona genérica de dano | não |
| `Obstacle.cs` | Trigger de dano para lasers, drones e ácido | não |
| `LevelEnd.cs` | Portal de fuga — fim da fase | não |
| `MovingPlatform.cs` | Plataformas cinéticas verticais | não |
| `SceneAutoSetup.cs` | Fecha automaticamente a cena demo quando faltam objetos mínimos | não |

---

## `PlayerController.cs`

Controla o **Blink** usando os botões físicos do ESP32, com fallback automático para teclado/gamepad (New Input System).

**Dependências obrigatórias no GameObject:**
- `Rigidbody2D` — física
- `Animator` — animações idle/walking
- `PlayerInput` (com `InputSystem_Actions` + Behavior: **Send Messages**) — fallback teclado

**Campos no Inspector:**

| Campo | Valor padrão | Descrição |
|---|---|---|
| `Move Speed` | 5 | Velocidade horizontal (u/s) |
| `Jump Force` | 10 | Impulso de pulo |
| `Ground Layer` | — | Layer `Ground` dos objetos de chão |
| `Ground Check` | — | Transform filho nos pés do Blink |
| `Ground Check Radius` | 0.1 | Raio da verificação de chão |
| `Force Keyboard Fallback` | false | ✅ Marque para testar sem ESP32 |

**Parâmetro Animator obrigatório:** `IsMoving` (Bool — nome exato, case-sensitive)

**API pública:**
```csharp
TriggerTreasureReaction()  // → SendTreasure() → OLED feliz
TriggerDamageReaction()    // → SendDamage()   → OLED assustado
TriggerIdleReaction()      // → SendIdle()     → OLED neutro
```

---

## `ESP32SerialReader.cs`

Singleton — `DontDestroyOnLoad`. Abre a porta serial em uma thread de background e expõe o estado dos botões para o `PlayerController`.

**Protocolo de comunicação:**

| Direção | Mensagem | Significado |
|---|---|---|
| ESP32 → Unity | `L1` / `L0` | Esquerda pressionada / solta |
| ESP32 → Unity | `R1` / `R0` | Direita pressionada / solta |
| ESP32 → Unity | `D1` / `D0` | Baixo pressionado / solto (reservado) |
| ESP32 → Unity | `U` | Pulo (borda, um frame) |
| Unity → ESP32 | `I` | Carinha neutra — Blink navegando |
| Unity → ESP32 | `T` | Carinha feliz — Data Core coletado / vitória |
| Unity → ESP32 | `D` | Carinha assustada — obstáculo atingido |

**Configuração no Inspector:**

| Campo | Windows | Mac / Linux |
|---|---|---|
| `Port Name` | `COM3` | `/dev/tty.usbserial-XXXX`, `/dev/ttyUSB0`, `/dev/ttyACM0` ou vazio |
| `Baud Rate` | `115200` | `115200` |
| `Auto Connect On Start` | `true` | `true` |
| `Auto Detect Port On Unix` | — | `true` |

**API pública:**
```csharp
ESP32SerialReader.Instance.ButtonLeft   // bool — mantido enquanto pressionado
ESP32SerialReader.Instance.ButtonRight  // bool
ESP32SerialReader.Instance.ButtonDown   // bool (reservado)
ESP32SerialReader.Instance.JumpPressed  // bool — true por exatamente 1 frame

ESP32SerialReader.Instance?.SendIdle();
ESP32SerialReader.Instance?.SendTreasure();
ESP32SerialReader.Instance?.SendDamage();
ESP32SerialReader.Instance?.SendPolvina();
```

> ⚠️ **Configuração obrigatória do .NET:** `Edit → Project Settings → Player → Other Settings → Api Compatibility Level → .NET Framework`. Sem isso `System.IO.Ports` não existe.
>
> ✅ **Pronto para Unix:** se não houver ESP32 conectado, a abertura da serial falha de forma graciosa e o jogo continua rodando com teclado.

---

## `GameManager.cs`

Singleton. No `Awake`, conta todos os `Collectible` da cena com `FindObjectsByType`. Quando `_collectedCount >= _totalCollectibles`, exibe o painel de vitória, pausa o jogo e envia `T` para o OLED.

**Campos no Inspector:**

| Campo | Descrição |
|---|---|
| `Game Over Panel` | Painel UI que aparece ao vencer (arraste o GameObject) |
| `Scene Name` | Nome exato da cena para reiniciar (confira em Build Settings) |
| `Score Text` | TMP opcional da HUD com `Data Cores: X/Y` |
| `Result Title Text` | TMP opcional para o título da vitória |
| `Result Body Text` | TMP opcional para o subtítulo da vitória |

**API pública:**
```csharp
GameManager.Instance.OnItemCollected(CollectibleType.DataCore);
GameManager.Instance.RestartGame();      // botão "Jogar Novamente"
```

**Estado exposto:**
```csharp
GameManager.Instance.AllRequiredCollected
GameManager.Instance.CollectedRequiredCount
GameManager.Instance.TotalRequiredCollectibles
GameManager.Instance.TryCompleteLevelFromPortal()
GameManager.Instance.OnPlayerDeath()
```

---

## `Collectible.cs`

Coloque em cada item coletável da cena. Requer `Collider2D` com **Is Trigger** marcado.

Ao ser coletado:
1. Se for `PenDrive` ou `DataCore`, conta para vitória
2. Se for `Polvina`, restaura 1 vida
3. Dispara feedback no OLED (`T` ou `P`)
4. Spawna efeito visual (opcional — campo `Collect Effect`)
5. Toca som (opcional — campo `Collect Sound`)
6. `Destroy(gameObject)` — some da cena

> O Player precisa ter a **Tag** `Player` (Edit → Project Settings → Tags).

---

## `HealthSystem.cs`

Vai no `Player`. Controla o total de Polvinas, aplica invencibilidade temporária e avisa o `GameManager` quando Blink morre.

**API pública:**
```csharp
health.TakeDamage();
health.AddLife();
health.GetCurrentLives();
health.IsInvincible;
```

---

## `HUDManager.cs`

Singleton da HUD. Escuta `HealthSystem` e `GameManager` para manter:
1. contador `Data Cores: X/Y`;
2. fileira de Polvinas no canto da tela.

Se a cena não estiver pronta, ele pode ser configurado pelo `SceneAutoSetup`.

---

## `DamageZone.cs`

Script genérico de dano para qualquer trigger do laboratório.

Ao tocar no Player:
1. procura `HealthSystem`;
2. chama `TakeDamage(damageAmount)`;
3. registra log se o dano realmente entrou.

---

## `LevelEnd.cs`

O portal de fuga do Blink. Requer `Collider2D` com **Is Trigger** marcado.

Ao o Player tocar:
1. Exibe `gameOverPanel`
2. `Time.timeScale = 0f` — pausa
3. `ESP32SerialReader.Instance?.SendTreasure()` — carinha feliz

**Campos no Inspector:** `Game Over Panel`, `Scene Name` (mesmo padrão do GameManager)

---

## `Obstacle.cs`

Script enxuto para qualquer obstáculo com `Collider2D` marcado como **Is Trigger**.

Ao tocar no Player:
1. `ESP32SerialReader.Instance?.SendDamage()` — carinha de dano no OLED
2. `Debug.Log(...)` — ajuda a testar colisão na Console

**Campo no Inspector:** `Obstacle Name` — rótulo opcional para identificar o obstáculo no log.

---

## `MovingPlatform.cs`

Plataformas do laboratório que sobem e descem. Requer `Rigidbody2D` com **Body Type: Kinematic** + **Collision Detection: Continuous** + **Freeze Rotation Z**.

| Campo | Padrão | Descrição |
|---|---|---|
| `Move Distance` | 3 | Distância percorrida a partir da posição inicial |
| `Speed` | 2 | Velocidade (u/s) |
| `Wait Time` | 0.5 | Pausa nos extremos (segundos) |

Usa `MovePosition` (respeitando física). Gizmos cyan visíveis no Editor.

---

## `SceneAutoSetup.cs`

Bootstrap da cena demo. Se a fase estiver incompleta, ele:
1. força fallback de teclado no Player;
2. cria `ScoreText` na HUD quando não existir;
3. garante `HealthSystem` no Player;
4. instancia `GameManager` e `HUDManager` quando ausentes;
5. garante 3 itens obrigatórios + 1 Polvina;
6. garante pelo menos 3 `Obstacle`.

Isso reduz a divergência entre o que a documentação promete e o que a cena já vem montada no repositório.

---

## Testes

Há testes automatizados em `Assets/Tests/Editor/GameplaySystemsTests.cs` cobrindo:
1. contagem correta dos coletáveis obrigatórios;
2. clamp de vidas e morte no `HealthSystem`;
3. criação do setup mínimo pela `SceneAutoSetup`.

---

## 💡 Como pedir ao Kiro para criar um novo script

```
Read the existing Collectible script for reference.
Create a new [Nome] script that [comportamento desejado].
Keep it simple and consistent with the existing code style.
Explain every change.
```

> **Regra de ouro Ctrl+Alt+Delas:** uma alteração de cada vez → testa → só então pede a próxima.
