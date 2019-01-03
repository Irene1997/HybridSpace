#include <SoftwareSerial.h>
#include <SerialCommand.h>

SerialCommand sCmd;

const int ledPinOffset = 2, columnAmount = 6, rowAmount = 5, buttonPin = 13, doorOffset = A0, doorAmount = 6, enemyAmount = 5;
const unsigned long debounceDelay = 50;

int activeColumn = -1, activeRow = -1, activeEnemy = 0;
int playerPosition [2] = {-1, -1};
int enemyPositions [enemyAmount * 2];
int doorState = 0;
unsigned long lastDoorsCheck = 0, lastDebounceTime = 0, lastToggleTime = 0;
int buttonReading = HIGH, buttonState = HIGH;
bool showPlayer = true, blinkEnemies = false;

void setup() {
  for (int i = 0; i < enemyAmount * 2; ++i) {
    enemyPositions[i] = -1;
  }
  
  Serial.begin(9600);
  while (!Serial);

  sCmd.addCommand("N", askName);
  sCmd.addCommand("D", askDoorState);
  sCmd.addCommand("P", newPlayerPosition);
  sCmd.addCommand("E", newEnemyPosition);
  sCmd.addDefaultHandler(errorHandler);

  for (int i = 0; i < columnAmount; ++i) {
    pinMode(ledPinOffset + i, INPUT);
  }
  for (int i = 0; i < rowAmount; ++i) {
    pinMode(ledPinOffset + columnAmount + i, OUTPUT);
    digitalWrite(ledPinOffset + columnAmount + i, LOW);
  }
  pinMode(buttonPin, INPUT_PULLUP);
  for (int i = 0; i < doorAmount; ++i) {
    pinMode(doorOffset + i, INPUT_PULLUP);
    if (digitalRead(doorOffset + i) == LOW){
      doorState |= 1 << i;
    }
  }

  testLeds();
}

void askName(){
  Serial.println("N MazeArduino");
}

void askDoorState () {
  Serial.println("D " + doorState);
}

void newPlayerPosition () {
  char *arg;
  int c, r;
  arg = sCmd.next();
  if (arg == NULL) {
    Serial.println("E player misses its column number.");
    return;
  } else {
    c = atol(arg);
  }
  arg = sCmd.next();
  if (arg == NULL) {
    Serial.println("E player misses its column number.");
    return;
  } else {
    r = atol(arg);
  }
  playerPosition[0] = c;
  playerPosition[1] = r;
}

void newEnemyPosition () {
  char *arg;
  int i, c, r;
  arg = sCmd.next();
  if (arg == NULL) {
    Serial.println("E enemy misses its index.");
    return;
  } else {
    i = atol(arg);
  }
  arg = sCmd.next();
  if (arg == NULL) {
    Serial.println("E enemy misses its column number.");
    return;
  } else {
    c = atol(arg);
  }
  arg = sCmd.next();
  if (arg == NULL) {
    Serial.println("E enemy misses its row number");
    return;
  } else {
    r = atol(arg);
  }
  enemyPositions[i * 2] = c;
  enemyPositions[i*2+1] = r;
}

void errorHandler () {
  Serial.println("E unreadable command.");
}

void testLeds (){
  for (int r = 0; r < rowAmount; ++r) {
    for (int c = 0; c < columnAmount; ++c) {
      showPosition(c, r);
      delay(300);
    }
  }
  showNoPosition();
}

void loop() {
  if ((millis() - lastDoorsCheck) > 50) {
    checkDoors();
    lastDoorsCheck = millis();
  }

  //buttonReading = digitalRead(buttonPin);
  //if (buttonReading != buttonState) {
  //  lastDebounceTime = millis();
  //}
  //if ((millis() - lastDebounceTime) > debounceDelay) {
  //  if (buttonReading != buttonState) {
  //    buttonState = buttonReading;
  //    if (buttonState == LOW) {
  //    }
  //  }
  //}

  if (showPlayer){
    showPosition(playerPosition[0], playerPosition[1]);
    showPlayer = false;
  } else {
    if (blinkEnemies) {
        showPosition(enemyPositions[activeEnemy * 2], enemyPositions[activeEnemy * 2 + 1]);
      } else {
        showNoPosition();
      }
    if (++activeEnemy >= enemyAmount){
      showPlayer = true;
      activeEnemy = 0;
    }
  }
  if (millis() - lastToggleTime > 200) {
    lastToggleTime = millis();
    blinkEnemies = !blinkEnemies;
  }
  // Processing incomming commands
  if (Serial.available() > 0) {
    sCmd.readSerial();
  }

  delay(1);  
}

void checkDoors() {
  int newState = 0;
  for (int i = 0; i < doorAmount; ++i) {
    newState |= 1 << i;
  }
  if (newState != doorState) {
    doorState = newState;
    askDoorState();
  }
}

void showPosition (int c, int r) {
  if (c < 0 || c >= columnAmount || r < 0 || r >= rowAmount){
    showNoPosition();
    return;
  }
  if (activeColumn != c) {
    pinMode(activeColumn + ledPinOffset, INPUT);
    activeColumn = c;
    pinMode(activeColumn + ledPinOffset, OUTPUT);
    digitalWrite(activeColumn + ledPinOffset, LOW);
  }

  if (activeRow != r) {
    digitalWrite(activeRow + ledPinOffset + columnAmount, LOW);
    activeRow = r;
    digitalWrite(activeRow + ledPinOffset + columnAmount, HIGH);
  }
}

void showNoPosition () {
  if (activeColumn != -1) {
    pinMode(activeColumn + ledPinOffset, INPUT);
    activeColumn = -1;
  }
  if (activeRow != -1) {
    digitalWrite(activeRow + ledPinOffset + columnAmount, LOW);
    activeRow = -1;
  }
}
