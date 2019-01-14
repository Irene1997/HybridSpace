#define BlinkPin 13

long timeToChange = 0;
int state = 0;
int ledState = 0;

void setup() {
  // put your setup code here, to run once:
  pinMode(BlinkPin, INPUT);
}

void loop() {
  // put your main code here, to run repeatedly:
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
