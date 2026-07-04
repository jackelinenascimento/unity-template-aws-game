# Tasks — Ecto-Escape
**Time:** Ctrl+Alt+Delas · Connect Byte × AWS

Siga esta ordem durante o evento. Cada tarefa tem um responsável principal, o que fazer e como verificar que funcionou.

---

## Fase 0 — Preparação (antes do evento)

| # | Responsável | Tarefa | Verificação |
|---|---|---|---|
| 0.1 | Todos | Instalar Kiro | Kiro abre e conecta |
| 0.2 | Todos | Instalar Unity Hub | Hub aparece na barra |
| 0.3 | Todos | Instalar Unity 6.0 (6000.0.50f1) | Editor abre sem erro |
| 0.4 | 🔌 Hardware | Instalar Arduino IDE ou VS Code + PlatformIO | Compila um sketch vazio |
| 0.5 | Todos | Clonar o repositório | Pasta tem `Assets/`, `Packages/`, `ProjectSettings/` |
| 0.6 | 🕹️ Unity | Abrir projeto no Unity Hub → confirmar sem erros | Console sem erros vermelhos |

---

## Fase 1 — Personagem (Blink)

| # | Responsável | Tarefa | Verificação |
|---|---|---|---|
| 1.1 | 🎨 Artista | Gerar sprite do Blink (ChatGPT/Kiro, prompt em `Docs/storytelling/`) | PNG com fundo transparente, 512×512 |
| 1.2 | 🎨 Artista | Gerar sprite sheet de expressões (10 expressões) | PNG fatiável em grade |
| 1.3 | 🕹️ Unity | Importar `personagem.png` em `Assets/Art/Personagem/` | Aparece no painel Project |
| 1.4 | 🕹️ Unity | Configurar Texture Type: Sprite (2D and UI) | Sem erro de import |
| 1.5 | 🕹️ Unity | Fatiar expressões no Sprite Editor (Grid 512×512) | Sprites individuais visíveis |
| 1.6 | 🕹️ Unity | Criar GameObject `Player` na Hierarchy, arrastar sprite | Blink aparece na Scene |
| 1.7 | 🕹️ Unity | Adicionar `Rigidbody2D` (GravityScale:3, FreezeRotZ) | Blink cai quando Play |
| 1.8 | 🕹️ Unity | Adicionar `CapsuleCollider2D` + Physics Material | Collider ajustado ao sprite |
| 1.9 | 🕹️ Unity | Adicionar `Animator` e criar clipes `idle.anim` / `walking.anim` | Animação aparece no Animator |
| 1.10 | 🕹️ Unity | Criar parâmetro `IsMoving` (Bool) no Animator Controller | Parâmetro visível no Animator |
| 1.11 | ⚙️ Gameplay | Adicionar `PlayerController.cs` ao Player | Script aparece no Inspector |
| 1.12 | ⚙️ Gameplay | Adicionar `PlayerInput` (InputSystem_Actions, Send Messages) | Sem erro de compilação |
| 1.13 | 🕹️ Unity | Criar filho `GroundCheck` nos pés do Blink | Transform visível na Hierarchy |
| 1.14 | 🕹️ Unity | Configurar `Ground Layer` e `Ground Check` no Inspector | Campos preenchidos |
| 1.15 | ⚙️ Gameplay | Marcar `Force Keyboard Fallback`, apertar Play, testar A/D/Espaço | Blink anda e pula |

---

## Fase 2 — Cenário

| # | Responsável | Tarefa | Verificação |
|---|---|---|---|
| 2.1 | 🏞️ Cenário | Gerar tileset (prompt em `Assets/Art/Chão/README.md`) | PNG 256×256 por tile, sem espaçamento |
| 2.2 | 🏞️ Cenário | Gerar assets de decoração e fundo (laboratório sci-fi) | PNGs com fundo transparente |
| 2.3 | 🕹️ Unity | Importar e fatiar tileset em `Assets/Art/Chão/` | Tiles individuais no Project |
| 2.4 | 🕹️ Unity | Criar objeto `Ground` na cena com `Sprite Renderer` + `BoxCollider2D` | Collider verde na Scene |
| 2.5 | 🕹️ Unity | Criar Layer `Ground` e aplicar no Ground | Layer visível no Inspector |
| 2.6 | 🕹️ Unity | Importar decoração em `Assets/Art/Decoração/` e posicionar na cena | Visual aparece sem afetar física |
| 2.7 | 🕹️ Unity | Ajustar `Sorting Layer` para decoração ficar atrás do Blink | Blink aparece na frente |
| 2.8 | 🕹️ Unity | Apertar Play — Blink fica em cima do chão sem cair | Blink parado sobre o Ground |

---

## Fase 3 — Hardware (ESP32 + OLED)

| # | Responsável | Tarefa | Verificação |
|---|---|---|---|
| 3.1 | 🔌 Hardware | Ligar OLED no ESP32: VCC→3V3, GND→GND, SDA→GPIO21, SCL→GPIO22 | Ligações físicas feitas |
| 3.2 | 🔌 Hardware | Instalar bibliotecas `Adafruit_SSD1306` e `Adafruit_GFX` no Arduino IDE | Library Manager mostra instaladas |
| 3.3 | 🔌 Hardware | Escrever (ou pedir ao Kiro) o firmware com `showIdle/showTreasure/showDamage` | Código compila sem erro |
| 3.4 | 🔌 Hardware | Adicionar leitura dos botões físicos e envio de `L1/L0/R1/R0/U` | Código compila sem erro |
| 3.5 | 🔌 Hardware | Fazer upload para o ESP32 | Upload concluído |
| 3.6 | 🔌 Hardware | Abrir Serial Monitor (115200) e digitar `I`, `T`, `D` | OLED mostra cada carinha |
| 3.7 | 🔌 Hardware | Apertar botões físicos e conferir mensagens no Serial Monitor | `L1`, `R1`, `U` aparecem no monitor |

---

## Fase 4 — Mecânicas de Gameplay

| # | Responsável | Tarefa | Verificação |
|---|---|---|---|
| 4.1 | ⚙️ Gameplay | Verificar `.NET Framework` em Project Settings → Player | Api Compatibility Level correto |
| 4.2 | ⚙️ Gameplay | Criar GameObject `ESP32SerialReader` + adicionar script | Singleton instanciado |
| 4.3 | ⚙️ Gameplay | Configurar `Port Name` e `Baud Rate: 115200` no Inspector | Campos preenchidos |
| 4.4 | 🔌 Hardware | Fechar Serial Monitor do Arduino | Serial Monitor fechado |
| 4.5 | ⚙️ Gameplay | Apertar Play, conferir `[ESP32] Conectado em...` na Console | Mensagem verde na Console |
| 4.6 | ⚙️ Gameplay | Desmarcar `Force Keyboard Fallback`, testar botões físicos | Blink anda e pula com controle |

### Data Cores

| # | Responsável | Tarefa | Verificação |
|---|---|---|---|
| 4.7 | 🎨 Artista | Gerar sprites dos Data Cores (pen drive holográfico, chip) | PNGs com fundo transparente |
| 4.8 | 🕹️ Unity | Importar sprites em `Assets/Art/Decoração/` | Aparecem no Project |
| 4.9 | 🕹️ Unity | Criar `DataCore_01` na cena: sprite + `CircleCollider2D` (Is Trigger) + Tag Player no Player | Collider configurado |
| 4.10 | ⚙️ Gameplay | Adicionar script `Collectible.cs` ao DataCore_01 | Script no Inspector |
| 4.11 | ⚙️ Gameplay | Apertar Play, coletar o item — verificar Console e se some | `[GameManager]` aparece na Console |
| 4.12 | 🕹️ Unity | Duplicar DataCore_01 → renomear → posicionar DataCore_02 e DataCore_03 | 3 itens na cena |

### Sistema de pontuação e vitória

| # | Responsável | Tarefa | Verificação |
|---|---|---|---|
| 4.13 | 🕹️ Unity | Criar `Canvas` + `ScoreText` (TMP) | Texto visível na tela |
| 4.14 | ⚙️ Gameplay | Criar `GameOverPanel` (inativo) com título e `BtnReiniciar` | Painel oculto no início |
| 4.15 | ⚙️ Gameplay | Criar GameObject `GameManager` + script + configurar Inspector | Campos preenchidos |
| 4.16 | ⚙️ Gameplay | Testar: coletar os 3 Data Cores → painel aparece e jogo pausa | `Time.timeScale = 0` |

### Obstáculos

| # | Responsável | Tarefa | Verificação |
|---|---|---|---|
| 4.17 | 🏞️ Cenário | Gerar sprites dos 3 obstáculos (laser, drone, ácido) | PNGs com fundo transparente |
| 4.18 | 🕹️ Unity | Criar `LaserSeguranca` na cena: sprite + `BoxCollider2D` (Is Trigger) | Collider configurado |
| 4.19 | ⚙️ Gameplay | Pedir ao Kiro o script `Obstacle.cs` (trigger → `SendDamage()`) | Script criado e compila |
| 4.20 | ⚙️ Gameplay | Adicionar `Obstacle.cs` ao LaserSeguranca, testar colisão | OLED mostra `(>_<)` |
| 4.21 | 🕹️ Unity | Duplicar e posicionar DronePatrulha e AcidoFluorescente | 3 obstáculos na cena |

### Portal de fuga

| # | Responsável | Tarefa | Verificação |
|---|---|---|---|
| 4.22 | 🕹️ Unity | Criar `LevelEnd` no fim da fase: `BoxCollider2D` (Is Trigger) | Collider posicionado |
| 4.23 | ⚙️ Gameplay | Adicionar `LevelEnd.cs`, configurar Inspector | Campos preenchidos |
| 4.24 | ⚙️ Gameplay | Testar: Blink chega no portal → painel aparece, OLED feliz | Vitória acionada |

---

## Fase 5 — Integração e Testes Finais

| # | Responsável | Tarefa | Verificação |
|---|---|---|---|
| 5.1 | Todos | Testar fluxo completo: início → 3 Data Cores → portal | Sem erros na Console |
| 5.2 | 🔌 Hardware | Verificar OLED: neutra no início, feliz ao coletar, assustada ao colidir | Todas as 3 carinhas funcionam |
| 5.3 | ⚙️ Gameplay | Verificar que `Time.timeScale` volta a 1 ao reiniciar | Jogo responde após reinício |
| 5.4 | 🕹️ Unity | Confirmar que não há erros vermelhos na Console durante gameplay | Console limpa |
| 5.5 | Todos | Conferir checklist de entrega no `README.md` raiz | Todos os itens marcados |

---

## Fase 6 — Personalizações (mínimo 4)

| # | Personalização | Responsável | Como fazer |
|---|---|---|---|
| P-01 | Personagem visual (Blink) | 🎨 Artista | Já contemplado na Fase 1 |
| P-02 | Cenário temático (laboratório) | 🏞️ Cenário | Já contemplado na Fase 2 |
| P-03 | Itens temáticos (Data Cores) | 🏞️ Cenário | Já contemplado na Fase 4 |
| P-04 | Obstáculos temáticos (laser/drone/ácido) | 🏞️ Cenário | Já contemplado na Fase 4 |
| P-05 *(extra)* | Nova expressão OLED (celebrando `C`) | 🔌 Hardware + ⚙️ Gameplay | Pedir ao Kiro: `Add SendCelebrating() method` |
| P-06 *(extra)* | Plataformas móveis | 🕹️ Unity | Adicionar `MovingPlatform.cs` a 1-2 plataformas |
| P-07 *(extra)* | Dash com duplo toque | ⚙️ Gameplay | Pedir ao Kiro: `Add Dash mechanic to PlayerController` |
| P-08 *(extra)* | HUD temática (sci-fi) | 🎨 Artista + 🕹️ Unity | Gerar sprites de UI, substituir TextMeshPro padrão |

---

## Prompts Kiro prontos para usar

### Leitura inicial (sempre primeiro)
```
Read the current Unity project.
Identify the main gameplay scripts.
Explain what each script does.
Do not modify any file yet.
```

### Criar script Obstacle
```
Read the existing Collectible script for reference.
Create a new Obstacle script that, on trigger with the Player (tag "Player"),
calls ESP32SerialReader.Instance?.SendDamage().
Keep it simple and consistent with the existing code style.
Explain every change.
```

### Firmware ESP32 completo
```
Write ESP32 (Arduino) firmware for an SSD1306 OLED (128x64, I2C).
Read a single char from Serial and draw a face for 'I' (neutral), 'T' (happy), 'D' (scared).
Also read 3 physical buttons and send "L1"/"L0", "R1"/"R0" and "U" back to Unity.
Use Adafruit_GFX shapes (circles for eyes, lines for mouth).
Baud rate: 115200. Explain the code.
```

### Nova carinha (celebrando)
```
Read the current ESP32SerialReader script.
Add a new public method SendCelebrating() that writes 'C' to Serial.
Keep all existing I/T/D methods intact.
Explain every modified file.
```

### Dash no PlayerController
```
Read the PlayerController script.
Add a Dash mechanic: when the player double-taps left or right
using ESP32SerialReader.Instance.ButtonLeft / ButtonRight, dash quickly in that direction.
Add a cooldown to prevent spam. Explain the changes step by step.
```
