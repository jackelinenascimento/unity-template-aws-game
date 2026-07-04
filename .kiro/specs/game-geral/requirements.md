# Requirements — Ecto-Escape
**Time:** Ctrl+Alt+Delas · Connect Byte × AWS

---

## 1. Visão Geral

Ecto-Escape é um jogo 2D de plataforma desenvolvido em Unity 6.0 no qual a jogadora controla **Blink**, um fantasminha digital, por um laboratório sci-fi coletando **Data Cores** para hackear o mainframe e escapar antes de ser deletado. O diferencial é a integração com hardware físico: um **ESP32** recebe comandos do jogo e exibe as reações emocionais do Blink num **display OLED** em tempo real, enquanto botões físicos controlam o personagem.

---

## 2. Requisitos Funcionais

### RF-01 — Personagem (Blink)
- O jogador controla Blink horizontalmente (esquerda/direita) e executa pulos.
- O movimento principal é via **botões físicos do ESP32** (mensagens `L1/L0`, `R1/R0`, `U` via Serial USB).
- Um **fallback de teclado/gamepad** (New Input System) deve funcionar quando o ESP32 não estiver conectado — controlado pelo campo `Force Keyboard Fallback` no Inspector.
- O sprite do Blink deve virar horizontalmente (flip) conforme a direção de movimento.
- O Animator do Blink deve alternar entre os estados `idle` e `walking` usando o parâmetro `IsMoving` (Bool).
- Blink deve detectar o chão via `Physics2D.OverlapCircle` num Transform filho (`GroundCheck`) para habilitar o pulo.

### RF-02 — Data Cores (Itens Coletáveis)
- A fase deve conter no mínimo **3 Data Cores** posicionados estrategicamente.
- Ao ser coletado, cada Data Core deve:
  - Notificar o `GameManager` (`OnItemCollected`).
  - Enviar `T` ao ESP32 via `SendTreasure()`.
  - Spawnar efeito visual (opcional).
  - Tocar som (opcional).
  - Ser destruído (`Destroy`).
- Data Cores devem usar `Collider2D` com **Is Trigger** ativo.

### RF-03 — Obstáculos
- A fase deve conter no mínimo **3 obstáculos** do universo do laboratório:
  - Laser de Segurança
  - Drone de Patrulha
  - Ácido Fluorescente
- Ao Blink colidir com um obstáculo, deve ser enviado `D` ao ESP32 via `SendDamage()`.
- Obstáculos devem usar `Collider2D` com **Is Trigger** ativo.

### RF-04 — Condição de Vitória
- Quando **todos os Data Cores** forem coletados, o `GameManager` deve:
  - Exibir o painel de vitória (`Game Over Panel`).
  - Pausar o jogo (`Time.timeScale = 0`).
  - Enviar `T` ao ESP32.
- Alternativamente, ao Blink chegar ao **portal de fuga** (`LevelEnd`), o mesmo painel deve ser exibido com as mesmas ações.
- O painel de vitória deve conter um botão "Jogar Novamente" que chama `RestartGame()`.

### RF-05 — Comunicação Serial (ESP32SerialReader)
- O Unity deve abrir a porta Serial configurada no Inspector em background thread.
- Mensagens recebidas devem ser processadas na main thread via `ConcurrentQueue`.
- O sinal de pulo (`U`) deve ser consumido em exatamente **1 frame** (`JumpPressed`).
- O Unity deve enviar caracteres únicos (`I`, `T`, `D`) para disparar expressões no OLED.
- A conexão deve falhar graciosamente: se a porta não estiver disponível, apenas registra `LogWarning` sem travar o jogo.

### RF-06 — Display OLED (Firmware ESP32)
- O firmware do ESP32 deve ler um único caractere pela Serial e exibir a expressão correspondente no OLED SSD1306 (128×64, I2C):
  - `I` → carinha neutra `(O_O)`
  - `T` → carinha feliz `(^u^)`
  - `D` → carinha assustada `(>_<)`
- As carinhas são desenhadas com `Adafruit_GFX` (círculos para olhos, linhas para boca).
- O firmware também deve ler os botões físicos e enviar `L1/L0`, `R1/R0`, `U` de volta ao Unity.
- Baud rate: **115200** em ambos os lados.

### RF-07 — HUD
- A cena deve ter um elemento de UI (TextMeshPro) mostrando a contagem de Data Cores coletados.
- O painel de vitória deve começar **inativo** (`SetActive(false)`) e ser ativado apenas ao vencer.

### RF-08 — Plataformas Móveis (opcional / desafio)
- A fase pode conter plataformas cinéticas usando `MovingPlatform.cs`.
- Cada plataforma móvel requer `Rigidbody2D` com Body Type **Kinematic** e `Collision Detection: Continuous`.

---

## 3. Requisitos Não-Funcionais

### RNF-01 — Motor e versão
- Unity **6.0 (6000.0.50f1)**, render pipeline **URP 17.5.0**.
- API Compatibility Level: **.NET Framework** (obrigatório para `System.IO.Ports`).

### RNF-02 — Input
- Input System: `com.unity.inputsystem 1.19.0`.
- O asset `InputSystem_Actions` já existe em `Assets/` e deve ser referenciado no componente `PlayerInput` do Blink com Behavior: **Send Messages**.

### RNF-03 — Assets visuais
- Todos os sprites devem ter fundo **transparente** (`.png` com alpha).
- Tamanho padrão: tiles **256×256 px**, frames do Blink **512×512 px**.
- Sprites devem ser importados com `Texture Type: Sprite (2D and UI)`.

### RNF-04 — Organização de cena
- Blink deve ter a **Tag** `Player`.
- Chão e plataformas devem ter a **Layer** `Ground`.
- A layer `Ground` deve ser a mesma referenciada no campo `Ground Layer` do `PlayerController`.

### RNF-05 — Controle de versão
- Arquivos `.meta` devem sempre acompanhar seus pares no commit.
- Pastas `Library/`, `Temp/`, `obj/`, `Build/`, `.vs/` devem estar no `.gitignore`.

### RNF-06 — Personalizações mínimas
- O jogo deve apresentar no mínimo **4 personalizações** em relação ao template base:
  - Personagem visual próprio (Blink)
  - Cenário temático (laboratório sci-fi)
  - Itens nomeados (Data Cores)
  - Obstáculos temáticos (laser, drone, ácido)

---

## 4. Restrições

| Restrição | Detalhe |
|---|---|
| Tempo de desenvolvimento | Duração do evento (1 dia) |
| Hardware | ESP32 + OLED SSD1306 I2C 128×64 |
| Linguagem de script | C# (Unity) + C++ Arduino (firmware) |
| Ferramenta de código | Kiro (principal) |
| Ferramenta de arte | ChatGPT / Kiro (geração de imagens) |
| Plataforma alvo | Windows / Mac (build local, rodar no editor) |

---

## 5. Casos de Uso Principais

| ID | Ator | Ação | Resultado esperado |
|---|---|---|---|
| UC-01 | Jogadora | Pressiona botão direita no ESP32 | Blink anda para a direita, sprite vira, `IsMoving = true` |
| UC-02 | Jogadora | Pressiona botão pulo | Blink pula (somente se `isGrounded = true`) |
| UC-03 | Blink | Colide com Data Core | Item some, `_collectedCount++`, OLED mostra `(^u^)` |
| UC-04 | Blink | Colide com obstáculo | OLED mostra `(>_<)` |
| UC-05 | Blink | Coleta o último Data Core | Painel de vitória aparece, jogo pausa, OLED mostra `(^u^)` |
| UC-06 | Blink | Chega no LevelEnd | Mesmo resultado de UC-05 |
| UC-07 | Jogadora | Clica em "Jogar Novamente" | Cena recarrega, `Time.timeScale = 1` |
| UC-08 | Unity | Inicia com ESP32 desconectado | `LogWarning` na Console, jogo roda normalmente com teclado |
