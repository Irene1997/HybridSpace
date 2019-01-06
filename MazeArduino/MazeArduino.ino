#include <SoftwareSerial.h>
#include <SerialCommand.h>

SerialCommand sCmd;

// Setting consttant values
const int columnOffset = A0, columnAmount = 6, rowOffset = 8, rowAmount = 5, doorOffset = 2, doorAmount = 6, enemyAmount = 5;
const unsigned long debounceDelay = 50;

// Declaring some variables, they seem self explainatairy enough
int activeColumn = -1, activeRow = -1, activeEnemy = 0;
int playerPosition [2] = {-1, -1};
int enemyPositions [enemyAmount * 2];
int doorState = 0;
unsigned long lastDoorsCheck = 0, lastDebounceTime = 0, lastToggleTime = 0;
bool showPlayer = true, blinkEnemies = false;

// Setup code
void setup() {
  // Fill the enemyPositions with out of reach coordinates
  for (int i = 0; i < enemyAmount * 2; ++i) {
    enemyPositions[i] = -1;
  }

  // Start the serial communication
  Serial.begin(9600);
  while (!Serial);

  // Assigning methods to incomming commands
  sCmd.addCommand("S", startCommunication);
  sCmd.addCommand("P", newPlayerPosition);
  sCmd.addCommand("M", newEnemyPosition);
  sCmd.addDefaultHandler(errorHandler);
  
  // Set all pinModes and standard outputs
  for (int i = 0; i < columnAmount; ++i) {
    pinMode(columnOffset + i, INPUT);
  }
  for (int i = 0; i < rowAmount; ++i) {
    pinMode(rowOffset + i, OUTPUT);
    digitalWrite(rowOffset + i, LOW);
  }
  for (int i = 0; i < doorAmount; ++i) {
    pinMode(doorOffset + i, INPUT);
    if (digitalRead(doorOffset + i) == LOW){
      doorState |= 1 << i;
    }
  }
  
  // Run a test for the leds
  //testLeds();
}

// Returns the name of the Arduino
void startCommunication(){
  askDoorState();
}

// Returns the current door states
void askDoorState () {
  Serial.print("D ");
  Serial.println(doorState);
}

// Changes the player position
void newPlayerPosition () {
  char *arg;
  int c, r;
  arg = sCmd.next();
  if (arg == NULL) {
    Serial.println("E P misses column");
    return;
  } else {
    c = atol(arg);
  }
  arg = sCmd.next();
  if (arg == NULL) {
    Serial.println("E P misses column");
    return;
  } else {
    r = atol(arg);
  }
  playerPosition[0] = c;
  playerPosition[1] = r;
}

// Changes an enemy's position
void newEnemyPosition () {
  char *arg;
  int i, c, r;
  arg = sCmd.next();
  if (arg == NULL) {
    Serial.println("E M misses index.");
    return;
  } else {
    i = atol(arg);
  }
  arg = sCmd.next();
  if (arg == NULL) {
    Serial.println("E M misses column");
    return;
  } else {
    c = atol(arg);
  }
  arg = sCmd.next();
  if (arg == NULL) {
    Serial.println("E M misses row");
    return;
  } else {
    r = atol(arg);
  }
  enemyPositions[i * 2] = c;
  enemyPositions[i*2+1] = r;
}

// Returns an error to the serial
void errorHandler () {
  //Serial.println("E unreadable command.");
}

// Turns on every LED one by one for a little while
void testLeds (){
  for (int r = 0; r < rowAmount; ++r) {
    for (int c = 0; c < columnAmount; ++c) {
      showPosition(c, r);
      delay(300);
    }
  }
  showNoPosition();
}

// Update code
void loop() {
  // Checks the door states periodically
  if ((millis() - lastDoorsCheck) > 50) {
    checkDoors();
    lastDoorsCheck = millis();
  }

  // Turns on the LEDs according to the positions
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
  // Toggles the enemy LEds
  if (millis() - lastToggleTime > 200) {
    lastToggleTime = millis();
    blinkEnemies = !blinkEnemies;
  }
  
  // Processes incomming commands
  if (Serial.available() > 0) {
    sCmd.readSerial();
  }
  
  // A litle delay for stability
  delay(1);  
}

// Checks the states of the doors
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

// Turns on the led on the given position
void showPosition (int c, int r) {
  if (c < 0 || c >= columnAmount || r < 0 || r >= rowAmount){
    showNoPosition();
    return;
  }
  if (activeColumn != c) {
    pinMode(activeColumn + columnOffset, INPUT);
    activeColumn = c;
    pinMode(activeColumn + columnOffset, OUTPUT);
    digitalWrite(activeColumn + columnOffset, LOW);
  }

  if (activeRow != r) {
    digitalWrite(activeRow + rowOffset, LOW);
    activeRow = r;
    digitalWrite(activeRow + rowOffset, HIGH);
  }
}

// Turns off all leds
void showNoPosition () {
  if (activeColumn != -1) {
    pinMode(activeColumn + columnOffset, INPUT);
    activeColumn = -1;
  }
  if (activeRow != -1) {
    digitalWrite(activeRow + rowOffset, LOW);
    activeRow = -1;
  }
}
