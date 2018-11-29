#include <SoftwareSerial.h>
#include <SerialCommand.h>

SerialCommand sCmd;

void setup() {
  pinMode(LED_BUILTIN, OUTPUT); 
  
  Serial.begin(9600);
  while (!Serial);

  sCmd.addCommand("PING", pingHandler);
  sCmd.addCommand("ECHO", echoHandler);
  sCmd.addCommand("LED_ON", led_on);
  sCmd.addCommand("LED_OFF", led_off);
  sCmd.addDefaultHandler(errorHandler);

  
}

void loop() {
  
  // Your operations here

  if (Serial.available() > 0)
    sCmd.readSerial();

  delay(50);  
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

