# 👻 Ecto-Escape
### Time: Ctrl+Alt+Delas · Connect Byte × AWS

Jogo 2D de plataforma em Unity onde **Blink**, um fantasminha digital, precisa hackear o mainframe de um laboratório sci-fi coletando **Data Cores** antes que o sistema seja deletado. Cada ação no jogo dispara uma reação em tempo real no display OLED físico via ESP32.

---

## 📖 A História

> Blink nasceu de uma falha de energia num laboratório de IA. Fofo, tecnológico e cheio de vida digital, ele descobriu que os cientistas agendaram uma **limpeza completa do sistema** para o final do dia. Se não hackear o mainframe a tempo, será deletado para sempre.
>
> A única saída: correr pelas plataformas do laboratório e coletar os **Núcleos de Dados (Data Cores)** espalhados pelos circuitos.

---

## ⚡ Fluxo Central

```
Jogadora faz algo no jogo
        ↓
Unity percebe o evento
        ↓
ESP32SerialReader envia uma letra pela USB
        ↓
ESP32 recebe e desenha a carinha do Blink no OLED
```

| Ação no jogo | Letra | Carinha no OLED | Estado do Blink |
|---|:---:|---|---|
| Navegando normal | `I` | Neutra `(O_O)` | *"Só de boa pelos circuitos."* |
| Coletou um Data Core | `T` | Feliz `(^u^)` | *"Mais um passo pra liberdade!"* |
| Colidiu com obstáculo | `D` | Assustada `(>_<)` | *"Quase me desintegraram!"* |
| Chegou ao portal final | `T` | Feliz `(^u^)` | *"Fugi! Partiu internet!"* |

---

## 🗺️ Estrutura do Projeto

```
unity-template-aws-game/          ← raiz = projeto Unity
├── Assets/
│   ├── Art/                      → sprites, animações, cenários, chão, decoração
│   │   ├── Animation/            → Animator Controller + clipes idle/walking
│   │   ├── Cenários/             → backgrounds e estruturas de fundo
│   │   ├── Chão/                 → tilesets e plataformas (com física)
│   │   ├── Decoração/            → props visuais sem colisão
│   │   ├── Material/             → Physics Material 2D do personagem
│   │   └── Personagem/           → sprite do Blink
│   ├── Scenes/                   → fases do jogo (.unity)
│   ├── Scripts/                  → código C# de gameplay
│   └── Settings/                 → configurações de render (URP 17.5)
├── Docs/
│   ├── Diversos/                 → manual do evento, instruções, Montagem.pdf
│   ├── Design/                   → decisões de design e referências visuais
│   ├── Hardware/                 → firmware versionado do ESP32 + OLED
│   ├── Story/                    → pasta de story geral
│   └── storytelling/             → narrativa oficial: Ecto-Escape.md
├── Packages/                     → dependências Unity (manifest.json)
└── ProjectSettings/              → configurações internas do projeto
```

---

## 🎮 Elementos do Jogo

| Elemento | Nome / Tipo | Descrição |
|---|---|---|
| Personagem | **Blink** | Fantasminha digital branco com detalhes em azul |
| Itens | **Data Cores** | Pen drives holográficos e chips de dados |
| Obstáculo 1 | Lasers de Segurança | Barreiras vermelhas intermitentes |
| Obstáculo 2 | Drones de Patrulha | Robôs flutuantes nos corredores |
| Obstáculo 3 | Ácido Fluorescente | Poças de descarte radioativo nas plataformas |
| Cenário | Laboratório hi-tech | Supercomputadores, neon, fibra óptica, hologramas |

---

## 🛠️ Stack Técnica

| Ferramenta | Versão |
|---|---|
| Unity Editor | **6.0 — 6000.0.50f1** |
| Universal Render Pipeline | 17.5.0 |
| Input System | 1.19.0 |
| 2D Tilemap + Extras | 1.0.0 / 8.0.3 |
| 2D Animation + PSD Importer | 15.1.0 / 14.0.3 |
| TextMeshPro (ugui) | 2.5.0 |
| Kiro | última versão |
| Arduino IDE ou VS Code + PlatformIO | firmware ESP32 |

---

## 🚀 Como começar

```bash
git clone https://github.com/Connect-Byte-Hangout/unity-template-aws-game.git
# Abrir a pasta raiz clonada no Unity Hub (não entre em subpastas)
```

> ⚠️ Não salve dentro do OneDrive — o Unity não gosta de sincronização.
>
> ✅ Para Unix, o projeto já está preparado para abrir e jogar com teclado mesmo sem o ESP32 conectado.

## ▶️ Como rodar no Unity

1. Abra o **Unity Hub**.
2. Clique em **Add** ou **Open** e selecione a pasta raiz do projeto:
   `unity-template-aws-game/`
3. Abra o projeto com o **Unity Editor 6.0 / 6000.0.50f1**.
4. Espere a primeira importação terminar completamente.
5. No painel **Project**, abra a cena principal:
   `Assets/Scenes/Basico Teste.unity`
6. Clique em **Play**.

### Rodar no Unix sem ESP32

O projeto já está preparado para abrir localmente mesmo sem hardware:

- Use `A` e `D` ou as setas para mover o Blink.
- Use `Espaço` para pular.
- Se o ESP32 não estiver conectado, deixe `Port Name` vazio no objeto `ESP32`.
- Mantenha `Auto Detect Port On Unix` ligado se quiser tentar detectar a serial automaticamente.
- Se quiser testar só no teclado, deixe o fallback de teclado habilitado no `PlayerController`.

### Rodar com ESP32

1. Conecte a placa no USB.
2. No objeto `ESP32`, confira:
   - `Baud Rate`: `115200`
   - `Port Name`: `/dev/ttyUSB0`, `/dev/ttyACM0`, `/dev/tty.usbserial-XXXX` ou vazio para auto-detect
   - `Auto Connect On Start`: ligado
3. Inicie a cena em **Play**.

### Validação rápida

- O Blink deve aparecer sem cair para fora da fase.
- Os 3 Data Cores obrigatórios devem contar no HUD.
- Os obstáculos devem causar dano.
- O portal final deve encerrar a fase quando os itens obrigatórios forem coletados.

---

## 👥 Papéis da Equipe — Ctrl+Alt+Delas

| Papel | Responsabilidade | Ferramenta |
|---|---|---|
| 🎨 Artista | Blink, expressões, ícones de UI | ChatGPT / Kiro |
| 🏞️ Cenário | Tiles, plataformas, decoração, Data Cores | ChatGPT / Kiro |
| 🕹️ Unity | Importar sprites, montar fase, colliders, Hierarchy | Unity |
| ⚙️ Gameplay | Scripts, mecânicas, HUD, comunicação OLED | Kiro + Unity |
| 🔌 Hardware | ESP32, OLED, botões físicos, firmware | Arduino IDE / VS Code |

---

## 📋 Checklist de entrega

- [ ] Projeto abre no Unity sem erros na Console
- [ ] Blink aparece e se move pelo laboratório
- [ ] Pelo menos 3 Data Cores coletáveis funcionando
- [ ] Pelo menos 3 obstáculos (laser, drone, ácido)
- [ ] Sistema de vidas com Polvinas funcionando
- [ ] Painel de vitória ao coletar todos os Data Cores
- [ ] ESP32 conectado — botões físicos movem o Blink
- [ ] OLED mostra as 3 carinhas (neutra, feliz, assustada)
- [ ] Testes automatizados das regras centrais
- [ ] Pelo menos 4 personalizações aplicadas

## 🧭 Cena Principal

- Cena jogável principal: `Assets/Scenes/Basico Teste.unity`
- `SampleScene.unity` fica apenas como template/base de referência

---

## 📚 Documentação

| Arquivo | Conteúdo |
|---|---|
| `Docs/storytelling/Ecto-Escape.md` | Narrativa oficial, personagem, obstáculos e tabela OLED |
| `Docs/Diversos/manual.html` | Guia completo passo a passo do evento |
| `Docs/Diversos/instrucoes.html` | Checklist de instalação e links |
| `Docs/Diversos/Montagem.pdf` | Diagrama de ligação ESP32 + OLED |
| `Docs/Hardware/esp32-oled.ino` | Firmware base do ESP32 + OLED + botões |
| `.kiro/specs/game-geral/` | Spec técnica completa do projeto |

---

## 💜 Ctrl+Alt+Delas × Connect Byte × AWS
Tecnologia com acolhimento, criatividade e muita troca. ☁️✨
