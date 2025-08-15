#include <stdint.h>

#include "main.h"
#include "cart_adapter.h"

void cart_setDirection_ad(uint8_t dir)
{
    if (dir == 0)
    {
        GPIOB->CRH = 0x88888888; // 8-15 上下拉输入
        GPIOB->CRL = 0x88888888; // 7-0  上下拉输入
        GPIOB->ODR = 0x0000ffff; // 15-0 上拉
    }
    else
    {
        GPIOB->CRH = 0x33333333; // 8-15 推挽输出, 50mhz
        GPIOB->CRL = 0x33333333; // 7-0  推挽输出, 50mhz
    }
}

void cart_setDirection_a(uint8_t dir)
{
    if (dir == 0)
    {
        GPIOA->CRL = 0x88888888;  // 7-0 上下拉输入
        GPIOA->BSRR = 0x000000ff; // 7-0 上拉
    }
    else
    {
        GPIOA->CRL = 0x33333333; // 7-0 推挽输出, 50mhz
    }
}

uint16_t cart_readBus_ad()
{
    return (GPIOB->IDR) & 0x0000ffff;
}

uint8_t cart_readBus_a()
{
    return (GPIOA->IDR) & 0x000000ff;
}

void cart_writeBus_ad(uint16_t value)
{
    GPIOB->ODR = value;
}

void cart_writeBus_a(uint8_t value)
{
    uint32_t _ = GPIOA->ODR;
    GPIOA->ODR = (_ & 0xffffff00) | value;
}

void cart_romRead(uint32_t addr, uint16_t *buf, uint16_t len)
{
    // latch base address
    cart_setDirection_a(1);
    cart_setDirection_ad(1);

    cart_writeBus_a((addr & 0x00ff0000) >> 16);
    cart_writeBus_ad((addr & 0x0000ffff));

    cs1_GPIO_Port->BSRR = cs1_Pin << 16; // cs1=0 126ns

    // read bus
    cart_setDirection_ad(0);
    for (int i = 0; i < len; i++)
    {
        rd_GPIO_Port->BSRR = rd_Pin << 16; // rd=0 126ns

        // tOE >25ns, tACC >110ns
        *buf = cart_readBus_ad();
        rd_GPIO_Port->BSRR = rd_Pin; // rd=1 126ns

        buf++;
    }

    // release bus
    cs1_GPIO_Port->BSRR = cs1_Pin; // cs1=1 126ns
    cart_setDirection_a(0);
}

void cart_romWrite(uint32_t addr, uint16_t *buf, uint16_t len)
{
    // latch base address
    cart_setDirection_a(1);
    cart_setDirection_ad(1);

    cart_writeBus_a((addr & 0x00ff0000) >> 16);
    cart_writeBus_ad((addr & 0x0000ffff));

    cs1_GPIO_Port->BSRR = cs1_Pin << 16; // cs1=0 126ns

    // write bus
    for (int i = 0; i < len; i++)
    {
        cart_writeBus_ad(*buf);

        wr_GPIO_Port->BSRR = wr_Pin << 16;
        // data setup 30ns, we low 25ns, address hold 45ns
        wr_GPIO_Port->BSRR = wr_Pin;

        buf++;
    }

    // release bus
    cs1_GPIO_Port->BSRR = cs1_Pin;
    cart_setDirection_a(0);
    cart_setDirection_ad(0);
}

void cart_ramRead(uint16_t addr, uint8_t *buf, uint16_t len)
{
    cart_setDirection_a(0);
    cart_setDirection_ad(1);

    cs2_GPIO_Port->BSRR = cs2_Pin << 16; // cs2=0 126ns

    // read bus
    for (int i = 0; i < len; i++)
    {
        cart_writeBus_ad(addr);

        rd_GPIO_Port->BSRR = rd_Pin << 16; // rd=0 126ns

        // address to dq 105ns, oe to dq 25ns
        *buf = cart_readBus_a();

        rd_GPIO_Port->BSRR = rd_Pin; // rd=1 126ns

        addr++;
        buf++;
    }

    // release bus
    cs2_GPIO_Port->BSRR = cs2_Pin; // cs2=1 126ns
    cart_setDirection_ad(0);
}

void cart_ramWrite(uint16_t addr, uint8_t *buf, uint16_t len)
{
    cart_setDirection_a(1);
    cart_setDirection_ad(1);

    cs2_GPIO_Port->BSRR = cs2_Pin << 16; // cs2=0 126ns

    // write bus
    for (int i = 0; i < len; i++)
    {
        cart_writeBus_ad(addr);
        cart_writeBus_a(*buf);

        wr_GPIO_Port->BSRR = wr_Pin << 16;
        //  address hold 70ns, data setup 20ns, write cycle 105ns
        wr_GPIO_Port->BSRR = wr_Pin;

        addr++;
        buf++;
    }

    // release bus
    cs2_GPIO_Port->BSRR = cs2_Pin;
    cart_setDirection_a(0);
    cart_setDirection_ad(0);
}

//
// for gb/gbc
//

void cart_gbcRead(uint16_t addr, uint8_t *buf, uint16_t len)
{
    cart_setDirection_a(0);
    cart_setDirection_ad(1);

    cs1_GPIO_Port->BSRR = cs1_Pin << 16; // cs1=0 126ns

    // read bus
    for (int i = 0; i < len; i++)
    {
        cart_writeBus_ad(addr);

        rd_GPIO_Port->BSRR = rd_Pin << 16; // rd=0 126ns

        // address to dq 105ns, oe to dq 25ns
        *buf = cart_readBus_a();

        rd_GPIO_Port->BSRR = rd_Pin; // rd=1 126ns

        addr++;
        buf++;
    }

    cs1_GPIO_Port->BSRR = cs1_Pin; // cs1=1 126ns

    cart_setDirection_ad(0);
}

void cart_gbcWrite(uint16_t addr, uint8_t *buf, uint16_t len)
{
    cart_setDirection_a(1);
    cart_setDirection_ad(1);

    cs1_GPIO_Port->BSRR = cs1_Pin << 16;

    // write bus
    for (int i = 0; i < len; i++)
    {
        cart_writeBus_ad(addr);
        cart_writeBus_a(*buf);

        wr_GPIO_Port->BSRR = wr_Pin << 16;
        wr_GPIO_Port->BSRR = wr_Pin;

        addr++;
        buf++;
    }

    cs1_GPIO_Port->BSRR = cs1_Pin;

    cart_setDirection_a(0);
    cart_setDirection_ad(0);
}
