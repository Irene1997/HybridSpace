#include <SoftwareSerial.h>
#include <SerialCommand.h>

#define MagnetPin 2
#define BlinkPin 3

SerialCommand sCmd;

int magnetState = 0, prevMagnetState = 0;
long timeToDoorCheck = 0;

long timeToChange = 0;
int state = 0;
int ledState = 0;

void setup() {
  Serial.begin(9600);
  while (!Serial);

  sCmd.addCommand("PING", pingHandler);
  sCmd.addCommand("ECHO", echoHandler);
  sCmd.addCommand("ASK_STATE", ask_magnetState);
  sCmd.addCommand("LED_ON", led_on);
  sCmd.addCommand("LED_OFF", led_off);
  sCmd.addDefaultHandler(errorHandler);

  pinMode(MagnetPin, INPUT);
  pinMode(LED_BUILTIN, OUTPUT);
}

void loop() {
  if (millis() > timeToDoorCheck){
    timeToDoorCheck += 50;
  // Reading the magnet state
  magnetState = digitalRead(MagnetPin);
  if (magnetState != prevMagnetState){
    if (magnetState == HIGH){
      led_off();
      Serial.println("Open");
    } else {
      led_on();
      Serial.println("Close");
    }
    prevMagnetState = magnetState;
  }

  // Processing incomming commands
  if (Serial.available() > 0)
    sCmd.readSerial();

}

  // Blinking the two LEDs on one pin
  if (millis() > timeToChange){
    state++;
    state &= 3;
    timeToChange = millis() + 1000;
  }

  if (state != ledState){
    if (state == 0){
      pinMode(BlinkPin, INPUT);
      ledState = 0;
    } else{
      pinMode(BlinkPin, OUTPUT);
      if (state == 1 || (state == 3 && ledState >= 2)){
        digitalWrite(BlinkPin, LOW);
        ledState = 1;
      } else if (state == 2 || (state == 3 && ledState <= 1)){
        digitalWrite(BlinkPin, HIGH);
        ledState = 2;
      }
    }
  }

  delay(1);  
}

void ask_magnetState(){
  magnetState = digitalRead(MagnetPin);
  if (magnetState == HIGH){
    led_off();
    Serial.println("Open");
  } else {
    led_on();
    Serial.println("Close");
  }
  prevMagnetState = magnetState;
}

void led_on(){
  digitalWrite(LED_BUILTIN, HIGH);
}

void led_off(){
  digitalWrite(LED_BUILTIN, LOW);
}

void pingHandler ()
{
  Serial.println("PONG");
}

void echoHandler ()
{
  char *arg;
  arg = sCmd.next();
  if (arg != NULL)
    Serial.println(arg);
  else
    Serial.println("nothing to echo");
}

void errorHandler (const char *command)
{
  Serial.println("Some error happened");
}

