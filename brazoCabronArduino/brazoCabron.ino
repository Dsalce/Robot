#include <Servo.h>

//MeArm HAS 4 SERVOS

Servo xServo;  // create servo object, arm base servo - left right motion

Servo yServo;  // create servo object, left side servo - forward backwards motion

Servo zServo;  // create servo object, right side servo - forward backwards motion

Servo clawServo;  // create servo object, end of arm srevo - open,close the claw hand

 

//servo positions values, expects 1-180 deg.

int xPos;

int yPos;

int zPos;

int clawPos;

void setup() {
  // put your setup code here, to run once:
  // assign servo to pin numbers

  xServo.attach(11);  // attaches the servo on pin 11 to the servo object

  yServo.attach(10);  // attaches the servo on pin 10 to the servo object

  zServo.attach(9);  // attaches the servo on pin 9 to the servo object

  clawServo.attach(6);  // attaches the servo on pin 6 to the servo object

 

  // initialize serial port

  Serial.begin(9600);

 

  // Debug only send serial message to host com port terminal window in Arduino IDE

  //Serial.print("*** MeCom Test V04 ***.");   // send program name, uncomment for debug connection test

 yServo.write(90);

  clawServo.write(0);
}

void loop() {
  // put your main code here, to run repeatedly:
  //serial in packet patern = xVal,yVal,zVal,clawVal + end of packet char 'x'

   
     yServo.write(0);
     zServo.write(0);
     delay(1000);
     yServo.write(180);
     zServo.write(180);
     delay(1000);
      //yServo.write(180);
   // delay(500);
}
