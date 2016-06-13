#include "iarduino_DHT.h"

		iarduino_DHT::iarduino_DHT(uint8_t i){PinDHT=i;}

int8_t	iarduino_DHT::read(){
//		читаем шину SDA
		int8_t i=readSDA(); if(i<0){return i;}
//		записываем полученные данные в переменные (с учётом модели и знака температуры)
		hum=float(reply[1]+(model==11?reply[0]: reply[0]      <<8))*(model==11?1:                    0.1 );
		tem=float(reply[3]+(model==11?reply[2]:(reply[2]&0x7F)<<8))*(model==11?1:(reply[2]&0x80?-0.1:0.1));
		return 0;
}

int8_t	iarduino_DHT::readSDA(){
//		обнуляем массив
		reply[0]=reply[1]=reply[2]=reply[3]=reply[4]=0;
//		прижимаем шину к земле на 25мс
		pinMode(PinDHT,OUTPUT); digitalWrite(PinDHT,LOW); delay(25);
//		отпускаем шину
		pinMode(PinDHT,INPUT);
//		проверяем реакцию датчика на сигнал "старт"
		if(pulseIn(PinDHT,HIGH,600)==0){return -3;}
//		читаем ответ датчика (40бит данных)
		for(int i=0,j=0; i<40; i++){j=pulseIn(PinDHT,HIGH,600); if(j){reply[i/8]<<=1; reply[i/8]+=j>45?1:0;}else{return -2;}}
//		проверяем не отправляет ли датчик в шину больше 40 бит данных
		if(pulseIn(PinDHT,HIGH,600)>0){return -2;}
//		проверяем контрольную сумму
		if(((reply[0]+reply[1]+reply[2]+reply[3])&0xff)!=reply[4]){return -1;}
//		определяем модель сенсора
		if(reply[1]||reply[3]||reply[2]&0x80){model=22;}else if(reply[0]>3||reply[2]>4){model=11;}
		return 0;
}
