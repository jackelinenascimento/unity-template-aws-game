# 📚 Docs — Ctrl+Alt+Delas · Ecto-Escape

Toda a documentação do projeto em um só lugar.

---

## 📂 Subpastas

| Pasta | Conteúdo |
|---|---|
| `storytelling/` | Narrativa oficial do jogo — **leia antes de criar qualquer asset** |
| `Diversos/` | Manual do evento, instruções de instalação, guia de montagem do hardware |
| `Hardware/` | Firmware versionado do ESP32 + OLED |
| `Design/` | Decisões de design do time, paleta de cores, referências visuais |
| `Story/` | Rascunhos e material extra de narrativa |

---

## 🔗 Documentos principais

| Documento | Onde abrir | O que cobre |
|---|---|---|
| `storytelling/Ecto-Escape.md` | Editor de texto / Kiro | Narrativa, personagens, obstáculos, tabela OLED |
| `Diversos/manual.html` | Navegador | Guia passo a passo completo do evento |
| `Diversos/instrucoes.html` | Navegador | Checklist de instalação, links, times |
| `Diversos/Montagem.pdf` | PDF viewer | Diagrama de ligação ESP32 + OLED |
| `Hardware/esp32-oled.ino` | Arduino IDE / PlatformIO | Firmware base da integração com OLED e botões |
| `Design/README.md` | Kiro | Template de decisões de design do time |
| `Story/README.md` | Kiro | Template de história extra |

---

## 📖 Ordem de leitura recomendada

1. **`storytelling/Ecto-Escape.md`** — entenda quem é o Blink e qual o universo do jogo
2. **`Diversos/instrucoes.html`** — cheque se todas as ferramentas estão instaladas
3. **`Diversos/Montagem.pdf`** — quem for de Hardware leia antes de mexer no ESP32
4. **`Hardware/esp32-oled.ino`** — firmware base para subir no ESP32
5. **`Diversos/manual.html`** — referência completa durante o evento

---

## 💡 Dica para o time

Preencha `Design/decisoes.md` logo nos primeiros minutos com: tema, paleta de cores e personalizações escolhidas. Isso alinha as 5 pessoas antes de gerarem assets em estilos diferentes.
