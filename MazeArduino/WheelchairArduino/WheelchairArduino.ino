#include <SoftwareSerial.h>
#include <SerialCommand.h>

#define outputLeftA 2
#define outputLeftB 3
int counterLeft = 0; 
int aStateLeft;
int aLastStateLeft; 

#define outputRightA 4
#define outputRightB 5
int counterRight = 0; 
int aStateRight;
int aLastStateRight;

SerialCommand sCmd;
 
void setup() { 
   pinMode (outputLeftA,INPUT);
   pinMode (outputLeftB,INPUT);

   pinMode (outputRightA,INPUT);
   pinMode (outputRightB,INPUT);
   
   Serial.begin (9600);
   while (!Serial);

  sCmd.addCommand("N", askName);
  sCmd.addDefaultHandler(errorHandler);
   
   // Reads the initial state of the outputALeft
   aLastStateLeft = digitalRead(outputLeftA);  

    // Reads the initial state of the outputARight
   aLastStateRight = digitalRead(outputRightA); 
 }

void askName(){
  Serial.println("N Wheelchair");
}

void errorHandler () {
  Serial.println("E unreadable command.");
}

void loop() { 
   //CheckLeft();
   CheckRight();
 }

void CheckLeft(){
  aStateLeft = digitalRead(outputLeftA); // Reads the "current" state of the outputA
   // If the previous and the current state of the outputA are different, that means a Pulse has occured
   if (aStateLeft != aLastStateLeft){     
     // If the outputB state is different to the outputA state, that means the encoder is rotating clockwise
     if (digitalRead(outputLeftB) != aStateLeft) { 
       Serial.println("LeftUp");
     } else {
       Serial.println("LeftDown");
     }
     //Serial.println(counterLeft);
     //Serial.println(counterLeft);
   } 
   aLastStateLeft = aStateLeft; // Updates the previous state of the outputA with the current state
}

void CheckRight(){
  aStateRight = digitalRead(outputRightA); // Reads the "current" state of the outputA
   // If the previous and the current state of the outputA are different, that means a Pulse has occured
   if (aStateRight != aLastStateRight){     
     // If the outputB state is different to the outputA state, that means the encoder is rotating clockwise
     if (digitalRead(outputRightB) != aStateRight) { 
       counterRight ++;
     } else {
       counterRight --;
     }
     Serial.print("Right ");
     Serial.println(counterRight);
   } 
   aLastStateRight = aStateRight; // Updates the previous state of the outputA with the current state
}
