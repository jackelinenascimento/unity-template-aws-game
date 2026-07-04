# 👻 Ecto-Escape

## 📖 A História

Você é **Blink**, um fantasma camarada, fofo e tecnológico que nasceu de
uma falha de energia em um laboratório de inteligência artificial de
ponta. Blink adora sua nova vida digital e adora navegar pelos
circuitos, mas acabou de descobrir que os cientistas agendaram uma
**limpeza completa do sistema** para o final do dia. Se ele não agir
rápido, será deletado para sempre!

Para sobreviver e conseguir escapar do laboratório em direção à
liberdade da internet, Blink precisa hackear o mainframe central. O
problema é que o sistema está trancado por uma criptografia pesada. A
única forma de abrir o portal de fuga é correndo pelas plataformas do
laboratório e coletando os **Núcleos de Dados (Data Cores)** que os
cientistas deixaram espalhados antes que o tempo acabe.

------------------------------------------------------------------------

## 🎮 Elementos do Jogo

O jogo foi totalmente personalizado para se ambientar nessa corrida
espacial/tecnológica:

-   **Protagonista:** **Blink**, o fantasminha digital (corpo branco com
    sutis detalhes em azul). Por ser um ser ectoplásmico, ele flutua
    levemente ao andar e pular.
-   **Cenário:** Um laboratório *high-tech* repleto de
    supercomputadores, cabos de fibra óptica brilhantes, luzes de neon e
    painéis holográficos.
-   **Itens Coletáveis (Data Cores):** Pen drives holográficos e chips
    de dados espalhados pelo mapa que liberam o acesso ao portal final.
-   **Obstáculos do Laboratório:**
    -   *Lasers de Segurança:* Barreiras vermelhas intermitentes que
        causam dano ao toque.
    -   *Drones de Patrulha:* Pequenos robôs flutuantes que vigiam os
        corredores digitais.
    -   *Ácido Fluorescente:* Poças de descarte químico radioativo sobre
        as plataformas.

------------------------------------------------------------------------

## 💻 Integração com Hardware (Unity ↔ ESP32 ↔ OLED)

  -------------------------------------------------------------------------------
  Ação no Jogo  Efeito no      Comando USB      Expressão no   Estado do Blink
                Unity                               OLED       
  ------------- ------------ ---------------- ---------------- ------------------
  **Navegando   Flutuando          `I`           **Carinha     *"Só de boa
  normal**      pela fase                         Neutra**     navegando pelos
                                                               circuitos."*

  **Coleta um   Item some e        `T`           **Carinha     *"Mais um passo
  Data Core**   pontua                            Feliz**      perto da
                                                               liberdade!"*

  **Colide com  Dispara            `D`           **Carinha     *"Ai! Quase me
  obstáculo**   animação de                     Assustada**    desintegraram!"*
                dano                                           

  **Chega ao    Ativa tela         `T`           **Carinha     *"Fugi! Partiu
  portal        de vitória /                      Feliz**      internet!"*
  final**       Pausa                                          
  -------------------------------------------------------------------------------

------------------------------------------------------------------------

## 🚀 Requisitos Mínimos Atendidos

-   1 Fase jogável ambientada no laboratório futurista.
-   1 Personagem jogável (Blink) controlado via hardware/ESP32.
-   Pelo menos 3 itens coletáveis espalhados.
-   Pelo menos 3 obstáculos posicionados estrategicamente.
-   Comunicação serial ativa e reações visuais sincronizadas no OLED.
