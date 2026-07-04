# Hardware — Ecto-Escape

Arquivos versionados para a parte física do projeto.

## Conteúdo

| Arquivo | Papel |
|---|---|
| `esp32-oled.ino` | Firmware base do ESP32 para OLED SSD1306 + botões |

## Protocolo

- Unity → ESP32: `I`, `T`, `D`
- ESP32 → Unity: `L1`, `L0`, `R1`, `R0`, `U`

## Ligações

- OLED VCC → ESP32 3V3
- OLED GND → ESP32 GND
- OLED SDA → ESP32 GPIO21
- OLED SCL → ESP32 GPIO22

## Botões usados no firmware

- Esquerda → GPIO18
- Direita → GPIO19
- Pulo → GPIO23
