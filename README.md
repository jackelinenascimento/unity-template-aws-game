# 🎮 Unity Template — AWS Game · Connect Byte

Jogo 2D de plataforma em Unity que se comunica com hardware físico (ESP32 + display OLED).
O personagem é o **fantasminha do Kiro**, e cada ação no jogo dispara uma reação na telinha física.

---

## 🗺️ Estrutura do Projeto

```
unity-template-aws-game/      ← raiz = projeto Unity
├── Assets/
│   ├── Art/                  → sprites, animações, cenários, chão, decoração
│   ├── Scenes/               → fases do jogo (.unity)
│   ├── Scripts/              → código C# de gameplay
│   └── Settings/             → configurações de render (URP)
├── Docs/
│   ├── Diversos/             → manual, instruções e guia de montagem
│   ├── Design/               → rascunhos e decisões de design
│   └── Story/                → narrativa e lore do jogo
├── Packages/                 → dependências gerenciadas pelo Unity
└── ProjectSettings/          → configurações internas do projeto
```

---

## ⚡ Fluxo Central

```
Jogadora faz algo no jogo
        ↓
Unity percebe o evento
        ↓
ESP32SerialReader envia uma letra pela USB
        ↓
ESP32 recebe e desenha a carinha no OLED
```

| Evento no jogo           | Letra enviada | Carinha no OLED |
|--------------------------|:---:|-----------------|
| Jogo rodando normal      | `I` | Neutra `(O_O)`  |
| Pegou um item coletável  | `T` | Feliz `(^u^)`   |
| Bateu num obstáculo      | `D` | Assustada `(>_<)` |

---

## 🛠️ Pré-requisitos

| Ferramenta | Versão |
|------------|--------|
| Unity Hub  | qualquer recente |
| Unity Editor | **6.0 (6000.0.50f1)** |
| Kiro       | última versão |
| Arduino IDE ou VS Code + PlatformIO | para o firmware do ESP32 |
| Git        | para clonar o repositório |

---

## 🚀 Como começar

```bash
# 1. Clonar o repositório
git clone https://github.com/Connect-Byte-Hangout/unity-template-aws-game.git

# 2. Abrir a pasta clonada no Unity Hub
#    (a pasta raiz já é o projeto Unity)
```

> ⚠️ Não salve dentro do OneDrive — o Unity não gosta de pastas sincronizadas.

---

## 👥 Papéis da Equipe

| Papel | Responsabilidade | Ferramenta |
|-------|-----------------|------------|
| 🎨 Artista | Personagem, expressões, ícones | ChatGPT / Kiro |
| 🏞️ Cenário | Tiles, plataformas, decoração, itens | ChatGPT / Kiro |
| 🕹️ Unity | Importar sprites, montar fase, colliders | Unity |
| ⚙️ Gameplay | Mecânicas, scripts, pontuação, HUD | Kiro + Unity |
| 🔌 Hardware | ESP32, OLED, botões, Serial | Arduino IDE / VS Code |

---

## 📋 Checklist mínimo de entrega

- [ ] Projeto abre no Unity e roda sem erros
- [ ] Personagem (fantasminha do Kiro) aparece e se move
- [ ] Pelo menos 3 itens coletáveis funcionando
- [ ] Pelo menos 3 obstáculos funcionando
- [ ] Painel de fim de jogo ao coletar tudo
- [ ] ESP32 conectado — botões movem o personagem
- [ ] OLED mostra as 3 carinhas (neutra, feliz, assustada)
- [ ] Pelo menos 4 personalizações aplicadas

---

## 📚 Documentação

- **Manual completo:** `Docs/Diversos/manual.html`
- **Instruções do evento:** `Docs/Diversos/instruções.html`
- **Guia de montagem do hardware:** `Docs/Diversos/Montagem.pdf`

---

## 💜 Connect Byte × AWS
Tecnologia com acolhimento, criatividade e muita troca. ☁️✨
