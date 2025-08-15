#ifndef __CART_ADAPTER_H_
#define __CART_ADAPTER_H_

void cart_setDirection_ad(uint8_t dir);
void cart_setDirection_a(uint8_t dir);

uint16_t cart_readBus_ad();
uint8_t cart_readBus_a();

void cart_writeBus_ad(uint16_t value);
void cart_writeBus_a(uint8_t value);

void cart_romRead(uint32_t addr, uint16_t *buf, uint16_t len);
void cart_romWrite(uint32_t addr, uint16_t *buf, uint16_t len);
void cart_ramRead(uint16_t addr, uint8_t *buf, uint16_t len);
void cart_ramWrite(uint16_t addr, uint8_t *buf, uint16_t len);

void cart_gbcRead(uint16_t addr, uint8_t *buf, uint16_t len);
void cart_gbcWrite(uint16_t addr, uint8_t *buf, uint16_t len);


#endif
