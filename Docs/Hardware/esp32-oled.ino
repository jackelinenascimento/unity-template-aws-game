#include <Wire.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>

namespace
{
  constexpr int SCREEN_WIDTH = 128;
  constexpr int SCREEN_HEIGHT = 64;
  constexpr int OLED_RESET = -1;
  constexpr int OLED_ADDRESS = 0x3C;

  constexpr int BTN_LEFT = 18;
  constexpr int BTN_RIGHT = 19;
  constexpr int BTN_JUMP = 23;

  Adafruit_SSD1306 display(SCREEN_WIDTH, SCREEN_HEIGHT, &Wire, OLED_RESET);

  bool lastLeftState = HIGH;
  bool lastRightState = HIGH;
  bool lastJumpState = HIGH;
}

void drawFace(const char* eyes, const char* mouth)
{
  display.clearDisplay();
  display.setTextColor(SSD1306_WHITE);

  display.drawRoundRect(12, 8, 104, 48, 12, SSD1306_WHITE);

  display.setTextSize(2);
  display.setCursor(30, 20);
  display.print(eyes);

  display.setTextSize(2);
  display.setCursor(42, 40);
  display.print(mouth);

  display.display();
}

void showIdle()
{
  drawFace("O O", "---");
}

void showTreasure()
{
  drawFace("^ ^", "\\_/");
}

void showDamage()
{
  drawFace("> <", "_ _");
}

void sendEdgeState(const char* pressedMessage, const char* releasedMessage, bool currentState, bool& previousState)
{
  if (currentState == previousState)
    return;

  previousState = currentState;
  Serial.println(currentState == LOW ? pressedMessage : releasedMessage);
}

void readButtons()
{
  bool leftState = digitalRead(BTN_LEFT);
  bool rightState = digitalRead(BTN_RIGHT);
  bool jumpState = digitalRead(BTN_JUMP);

  sendEdgeState("L1", "L0", leftState, lastLeftState);
  sendEdgeState("R1", "R0", rightState, lastRightState);

  if (jumpState != lastJumpState)
  {
    if (jumpState == LOW)
      Serial.println("U");

    lastJumpState = jumpState;
  }
}

void readUnityMessages()
{
  if (!Serial.available())
    return;

  char incoming = static_cast<char>(Serial.read());

  switch (incoming)
  {
    case 'I':
      showIdle();
      break;
    case 'T':
      showTreasure();
      break;
    case 'D':
      showDamage();
      break;
    default:
      break;
  }
}

void setup()
{
  pinMode(BTN_LEFT, INPUT_PULLUP);
  pinMode(BTN_RIGHT, INPUT_PULLUP);
  pinMode(BTN_JUMP, INPUT_PULLUP);

  Serial.begin(115200);
  Wire.begin(21, 22);

  if (!display.begin(SSD1306_SWITCHCAPVCC, OLED_ADDRESS))
    return;

  showIdle();
}

void loop()
{
  readUnityMessages();
  readButtons();
  delay(10);
}
