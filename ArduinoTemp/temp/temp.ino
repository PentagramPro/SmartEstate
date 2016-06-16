#include "LowPower.h"
#include "iarduino_DHT.h"   // подключаем библиотеку для работы с датчиком DHT
iarduino_DHT sensor(3);   

void setup() {
  // put your setup code here, to run once:
  pinMode(2,OUTPUT);
  pinMode(3,INPUT);
  digitalWrite(2,HIGH);
  
  Serial.begin(9600);
  while (!Serial) {
    ; // wait for serial port to connect. Needed for native USB port only
  }
  Serial.setTimeout(10000);
   delay(1000);   
}

void process(String packet)
{
  if(packet.charAt(2)=='R')
  {
    switch(sensor.read()){    // читаем показания датчика
      case DHT_OK:               Serial.print((String)":00R,"+sensor.tem+","+sensor.hum+";");  break;
      case DHT_ERROR_CHECKSUM:   Serial.print(         ":04E,1;");                     break;
      case DHT_ERROR_DATA:       Serial.print(         ":04E,2;"); break;
      case DHT_ERROR_NO_REPLY:   Serial.print(         ":04E,3;");                          break;
      default:                   Serial.print(         ":04E,4;");                               break;
    }
  }
  else
  {
    Serial.print(":04E,6;");
  }
}

void loop() {
  // put your main code here, to run repeatedly:
  
  if(Serial.read()==':')
  {
    String packet = Serial.readStringUntil(';');
    if(packet.length()>=3)
    {
      process(packet);    
    }
    else
    {
      Serial.print(":04E,5;");
    }
  }
  
  delay(50);
}
