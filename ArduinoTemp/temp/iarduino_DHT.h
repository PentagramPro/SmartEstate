#ifndef iarduino_DHT_h
#define iarduino_DHT_h

#if defined(ARDUINO) && (ARDUINO >= 100)
#include <Arduino.h>
#else
#include <WProgram.h>
#endif

#define	DHT_OK				 0			//	Данные получены и обработаны без ошибок
#define	DHT_ERROR_CHECKSUM	-1			//	Ошибка контрольной суммы (данные прочтены не корректно, мешает другое устройство на той же шине или имеются помехи)
#define	DHT_ERROR_DATA		-2			//	Данные не соответствуют заявленным в datasheet на сенсоры
#define	DHT_ERROR_NO_REPLY	-3			//	Данных нет, датчик не реагирует или отсутствует

class iarduino_DHT{
	public:		iarduino_DHT(uint8_t);	//	подключение (номер вывода к которому подключён датчик)
		int8_t	read();					//	возвращает № ошибки
		float 	hum;					//	значение влажности   в %
		float	tem;					//	значение температуры в °C
	private:
	    uint8_t	PinDHT;					//	номер вывода к которому подключён датчик
		int8_t	readSDA();				//	чтение шины SDA в массив reply
		uint8_t	model;					//	тип сенсора 11 или 22
		uint8_t	reply[6];				//	массив из 5ти байт
};

#endif