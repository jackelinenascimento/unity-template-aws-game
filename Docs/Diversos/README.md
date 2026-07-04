# 📋 Docs / Diversos — Ctrl+Alt+Delas

Material de referência do evento **Connect Byte × AWS**. Leia antes de começar o projeto.

---

## 📄 Arquivos

| Arquivo | O que é | Como abrir |
|---------|---------|------------|
| `manual.html` | Guia completo passo a passo — do clone do repositório até a entrega | Navegador |
| `instrucoes.html` | Instruções gerais do evento: instalação, times, links | Navegador |
| `Montagem.pdf` | Guia de montagem física do hardware (ESP32 + OLED + botões) | PDF viewer |

---

## 🗂️ O que cada arquivo cobre

### `manual.html`
- Divisão de papéis da equipe
- Fluxo de cada área (o que recebe, faz e entrega)
- Passo a passo: clone, Unity, personagem, cenário, hardware, mecânicas, ESP32/OLED
- Prompts prontos para gerar todos os assets com ChatGPT ou Kiro
- Como usar o Kiro para escrever código
- Checklists de entrega e de problemas comuns
- Glossário completo

### `instrucoes.html`
- Checklist de instalação (Kiro, Unity Hub, Unity 6.0)
- Links para download das ferramentas
- Repositório do projeto no GitHub
- Formação dos times

### `Montagem.pdf`
- Diagrama de ligações do ESP32 + OLED (I2C: VCC, GND, SDA, SCL)
- Instruções físicas de montagem do controle
- Referência de pinos

---

## ⚡ Protocolo de comunicação (resumo)

| Direção | Mensagem | Significado |
|---------|----------|-------------|
| ESP32 → Unity | `L1` / `L0` | Botão esquerda pressionado / solto |
| ESP32 → Unity | `R1` / `R0` | Botão direita pressionado / solto |
| ESP32 → Unity | `U` | Pulo |
| Unity → ESP32 | `I` | Carinha neutra `(O_O)` |
| Unity → ESP32 | `T` | Carinha feliz `(^u^)` |
| Unity → ESP32 | `D` | Carinha assustada `(>_<)` |
