#include <SoftwareSerial.h>
#include <SerialCommand.h>

#define MagnetPin 2

SerialCommand sCmd;

int state = 0, prevState = 0;

void setup() {
  Serial.begin(9600);
  while (!Serial);

  sCmd.addCommand("PING", pingHandler);
  sCmd.addCommand("ECHO", echoHandler);
  sCmd.addCommand("ASK_STATE", ask_state);
  sCmd.addCommand("LED_ON", led_on);
  sCmd.addCommand("LED_OFF", led_off);
  sCmd.addDefaultHandler(errorHandler);

  pinMode(MagnetPin, INPUT);
  pinMode(LED_BUILTIN, OUTPUT);
}

void loop() {
  // Your operations here
  state = digitalRead(MagnetPin);
  if (state != prevState){
    if (state == HIGH){
      led_off();
      Serial.println("Open");
    } else {
      led_on();
      Serial.println("Close");
    }
    prevState = state;
  }

  if (Serial.available() > 0)
    sCmd.readSerial();

  delay(50);  
}

void ask_state(){
  state = digitalRead(MagnetPin);
  if (state == HIGH){
    led_off();
    Serial.println("Open");
  } else {
    led_on();
    Serial.println("Close");
  }
  prevState = state;
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

