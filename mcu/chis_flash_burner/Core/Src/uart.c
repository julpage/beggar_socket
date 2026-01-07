#include <string.h>
#include <stdint.h>

#include "main.h"
#include "usbd_cdc_if.h"
// #include "usbd_def.h"
// #include "usbd_cdc.h"

#include "uart.h"
#include "cart_adapter.h"

#define BATCH_SIZE_RW 512
#define BATCH_SIZE_RESPON 512

#define SIZE_CMD_HEADER 3
#define SIZE_RESPON_HEADER 2
#define SIZE_BASE_ADDRESS 4
#define SIZE_BYTE_COUNT 2
#define SIZE_CRC 2
#define SIZE_BUFF_SIZE 2
#define SIZE_LATENCY 1

// 命令头
typedef struct __attribute__((packed))
{
    uint16_t cmdSize;
    uint8_t cmdCode;
    uint8_t payload[]; // 最后两个字节是crc
} Desc_cmdHeader_t;

// 命令身 写
typedef struct __attribute__((packed))
{
    uint32_t baseAddress;
    uint8_t payload[]; // 最后两个字节是crc
} Desc_cmdBody_write_t;

// 命令身 读
typedef struct __attribute__((packed))
{
    uint32_t baseAddress;
    uint16_t readSize;
    uint16_t crc16;
} Desc_cmdBody_read_t;

// 响应包
typedef struct __attribute__((packed))
{
    uint16_t crc16;
    uint8_t payload[];
} Desc_respon_t;

uint16_t cmdBuf_p = 0;
uint8_t cmdBuf[5500];

// uint16_t responBuf_p = 0;
uint8_t responBuf[5500];

Desc_cmdHeader_t *uart_cmd = (Desc_cmdHeader_t *)cmdBuf;
Desc_respon_t *uart_respon = (Desc_respon_t *)responBuf;

volatile uint8_t busy = 0;

extern USBD_HandleTypeDef hUsbDeviceFS;

void romGetID();
void romEraseChip();
void romEraseBlock();
void romEraseSector();
void romProgram();
void romWrite();
void romRead();
void ramWrite();
void ramRead();
void ramProgramFlash();
void ramWrite_forFram();
void ramRead_forFram();

void gbcWrite();
void gbcRead();
void gbcRomProgram();
void gbcWrite_forFram();
void gbcRead_forFram();

uint16_t modbusCRC16(uint8_t *buf, uint16_t len)
{
    uint16_t crc = 0xffff;

    for (int i = 0; i < len; i++)
    {
        crc = crc ^ buf[i];
        for (int ii = 0; ii < 8; ii++)
        {
            uint16_t temp = crc & 0x0001;
            crc = crc >> 1;
            crc = crc & 0x7fff;
            if (temp)
                crc = crc ^ 0xa001;
        }
    }

    return crc;
}

void uart_setControlLine(uint8_t rts, uint8_t dtr)
{
    static uint8_t currentRts = 0;
    static uint8_t currentDtr = 0;

    if (((currentRts == 0) && (rts != 0)) ||
        ((currentDtr == 0) && (dtr != 0)))
    {
        cmdBuf_p = 0;
        memset(cmdBuf, 0, sizeof(cmdBuf));
    }

    currentRts = rts;
    currentDtr = dtr;
}

void uart_responData(uint8_t *dat, uint16_t len)
{
    // uart_respon->crc16 = modbusCRC16(dat, len); // 计算crc

    if (dat != NULL)
        memcpy(uart_respon->payload, dat, len); // 填充数据

    USBD_CDC_HandleTypeDef *hcdc = (USBD_CDC_HandleTypeDef *)hUsbDeviceFS.pClassData;

    // 分批发送
    uint16_t packSize = SIZE_CRC + len;
    uint16_t transCount = 0;
    while (transCount < packSize)
    {
        uint16_t transLen = packSize - transCount;
        if (transLen > BATCH_SIZE_RESPON)
            transLen = BATCH_SIZE_RESPON;

        while (hcdc->TxState != 0)
            ;
        CDC_Transmit_FS(responBuf + transCount, transLen);

        transCount += transLen;
    }
}

void uart_responAck()
{
    USBD_CDC_HandleTypeDef *hcdc = (USBD_CDC_HandleTypeDef *)hUsbDeviceFS.pClassData;

    while (hcdc->TxState != 0)
        ;

    uint8_t ack = 0xaa;
    CDC_Transmit_FS(&ack, 1);
}

// usb 接收回调
void uart_cmdRecv(uint8_t *buf, uint32_t len)
{
    if (busy)
        return;

    uint16_t remainSize = sizeof(cmdBuf) - cmdBuf_p;
    if (len > remainSize)
        return;

    memcpy(cmdBuf + cmdBuf_p, buf, len);
    cmdBuf_p += len;
}

void uart_clearRecvBuf()
{
    cmdBuf_p = 0;
    //memset(cmdBuf, 0, sizeof(cmdBuf));
    busy = 0;
}

void uart_cmdHandler()
{
    // 判断命令结束
    if (cmdBuf_p > 2)
    {
        if (uart_cmd->cmdSize == cmdBuf_p)
        {
            // check crc
            // uint16_t cmdCrc = *((uint16_t *)(uart_cmd->payload + uart_cmd->cmdSize - 3 - 2));
            // uint16_t localCrc = modbusCRC16(cmdBuf, uart_cmd->cmdSize - 2);

            // if (cmdCrc != localCrc)
            //     uart_clearRecvBuf();

            busy = 1;
            HAL_GPIO_WritePin(led_GPIO_Port, led_Pin, 0);

            // execute cmd
            switch (uart_cmd->cmdCode)
            {
            case 0xf0: // rom id获取
                romGetID();
                break;

            case 0xf1: // rom chip擦除
                romEraseChip();
                break;

            case 0xf2: // rom blcok擦除
                romEraseBlock();
                break;

            case 0xf3: // rom sector擦除
                romEraseSector();
                break;

            case 0xf4: // rom program
                romProgram();
                break;

            case 0xf5: // rom 写入透传
                romWrite();
                break;

            case 0xf6: // rom 读取透传
                romRead();
                break;

            case 0xf7: // ram 写入透传
                ramWrite();
                break;

            case 0xf8: // ram 读取透传
                ramRead();
                break;

            case 0xf9: // 编程flash存档
                ramProgramFlash();
                break;

            case 0xe7: // ram 带延迟写入
                ramWrite_forFram();
                break;

            case 0xe8: // ram 带延迟读取
                ramRead_forFram();
                break;

            case 0xfa: // gbc 写入透传
                gbcWrite();
                break;

            case 0xfb: // gbc 读取透传
                gbcRead();
                break;

            case 0xfc: // gbc rom编程
                gbcRomProgram();
                break;

            case 0xea: // gbc 带延迟写入
                gbcWrite_forFram();
                break;

            case 0xeb: // gbc 带延迟读取
                gbcRead_forFram();
                break;

            default:
                break;
            }

            HAL_GPIO_WritePin(led_GPIO_Port, led_Pin, 1);
            return;
        }
    }
}

void romWaitForDone(uint32_t addr, uint16_t expectedValue)
{
    uint16_t value;
    while (1)
    {
        cart_romRead(addr, &value, 1);
        if ((value & 0x0080) == (expectedValue & 0x0080))
        {
            cart_romRead(addr, &value, 1);
            cart_romRead(addr, &value, 1);
            break;
        }
        if (cmdBuf_p == 0)
        {
            break;
        }
    }
}

// 获取rom id
// i 2B.包大小 0xf0 2B.CRC
//   05 00 f0 61 85
// o 2B.CRC 8B.数据
void romGetID()
{
    uint16_t cmd;

    /* Issue Autoselect Command Sequence */
    cmd = 0xaa;
    cart_romWrite(0x555, &cmd, 1);
    cmd = 0x55;
    cart_romWrite(0x2aa, &cmd, 1);
    cmd = 0x90;
    cart_romWrite(0x555, &cmd, 1);

    uint16_t id[4];                // e.g             S29GL256S  JS28F256
    cart_romRead(0x00, id + 0, 1); // Manufacture ID  0001h      0089h
    cart_romRead(0x01, id + 1, 1); // Device ID       227Eh      227Eh
    cart_romRead(0x0e, id + 2, 1); // Device ID       2222h      2222h = 256 Mb
    cart_romRead(0x0f, id + 3, 1); // Device ID       2201h      2201h

    /* Write Software RESET command */
    cmd = 0xf0;
    cart_romWrite(0, &cmd, 1);

    uart_clearRecvBuf();
    uart_responData((uint8_t *)id, 8);
}

// 全片抹除
// i 2B.包大小 0xf1 2B.CRC
//   05 00 f1 a0 45
// o 0xaa
void romEraseChip()
{
    uint16_t cmd;

    /* Issue inlock sequence command */
    cmd = 0xaa;
    cart_romWrite(0x555, &cmd, 1);
    cmd = 0x55;
    cart_romWrite(0x2aa, &cmd, 1);
    cmd = 0x80;
    cart_romWrite(0x555, &cmd, 1);
    cmd = 0xaa;
    cart_romWrite(0x555, &cmd, 1);
    cmd = 0x55;
    cart_romWrite(0x2aa, &cmd, 1);
    /* Write Chip Erase Command to Base Address */
    cmd = 0x10;
    cart_romWrite(0x555, &cmd, 1);

    uart_clearRecvBuf();
    uart_responAck();
}

// 块擦除
// i 2B.包大小 0xf2 4B.BlockAddress 2B.CRC
// o 0xaa
void romEraseBlock()
{
    // 本项目无用
    uart_clearRecvBuf();
    uart_responAck();
}

// 扇区擦除
// i 2B.包大小 0xf3 4B.SectorAddress 2B.CRC
// o 0xaa
void romEraseSector()
{
    Desc_cmdBody_write_t *desc_write = (Desc_cmdBody_write_t *)(uart_cmd->payload);

    // 扇区地址
    uint32_t sectorAddress = (desc_write->baseAddress >> 1) & 0x00ff0000;

    uint16_t cmd;

    /* Issue unlock sequence command */
    cmd = 0xaa;
    cart_romWrite(0x555, &cmd, 1);
    cmd = 0x55;
    cart_romWrite(0x2aa, &cmd, 1);
    cmd = 0x80;
    cart_romWrite(0x555, &cmd, 1);
    cmd = 0xaa;
    cart_romWrite(0x555, &cmd, 1);
    cmd = 0x55;
    cart_romWrite(0x2aa, &cmd, 1);
    /* Write Sector Erase Command to Offset */
    cmd = 0x30;
    cart_romWrite(sectorAddress, &cmd, 1);

    romWaitForDone(sectorAddress, 0xffff);

    uart_clearRecvBuf();
    uart_responAck();
}

// rom program
// i 2B.包大小 0xf4 4B.始地址 nB.数据 2B.CRC
// o 0xaa
void romProgram()
{
    Desc_cmdBody_write_t *desc_write = (Desc_cmdBody_write_t *)(uart_cmd->payload);

    // 基地址
    uint32_t baseAddress = desc_write->baseAddress;
    uint32_t wordAddress = baseAddress >> 1;
    // 写入总数量
    uint16_t byteCount = uart_cmd->cmdSize - SIZE_CMD_HEADER - SIZE_BASE_ADDRESS - SIZE_BUFF_SIZE - SIZE_CRC;
    uint16_t wordCount = byteCount / 2;
    // 编程buff大小
    uint16_t bufferWriteBytes = *((uint16_t *)(desc_write->payload));
    // 数据
    uint16_t *dataBuf = (uint16_t *)(desc_write->payload + SIZE_BUFF_SIZE);

    uint32_t writtenCount = 0;

    while (writtenCount < wordCount)
    {
        uint16_t cmd;
        uint32_t startingAddress = wordAddress + writtenCount;

        // 不能多字节编程
        if (bufferWriteBytes == 0)
        {
            /* Issue Load Write Buffer Command Sequence */
            /* Issue unlock command sequence */
            cmd = 0xaa;
            cart_romWrite(0x555, &cmd, 1);
            cmd = 0x55;
            cart_romWrite(0x2aa, &cmd, 1);
            /* Write Program Command */
            cmd = 0xa0;
            cart_romWrite(0x555, &cmd, 1);

            cart_romWrite(startingAddress,
                          dataBuf + writtenCount,
                          1);

            romWaitForDone(startingAddress, *(dataBuf + writtenCount));
            if (cmdBuf_p == 0)
            {
                uart_clearRecvBuf();
                return;
            }

            writtenCount++;
        }
        // 可以多字节编程
        else
        {
            // 5.4.1.2
            // Write Buffer Programming allows up to 512 bytes to be programmed in one operation.
            uint16_t writeLen = wordCount - writtenCount;
            if (writeLen > (bufferWriteBytes / 2))
                writeLen = (bufferWriteBytes / 2);
            // uint32_t sectorAddress = startingAddress & 0xffff0000;

            /* Issue Load Write Buffer Command Sequence */
            /* Issue unlock command sequence */
            cmd = 0xaa;
            cart_romWrite(0x555, &cmd, 1);
            cmd = 0x55;
            cart_romWrite(0x2aa, &cmd, 1);
            /* Issue Write to Buffer Command at Sector Address */
            cmd = 0x25;
            cart_romWrite(startingAddress, &cmd, 1);

            /* Write Number of Locations to program */
            cmd = writeLen - 1;
            cart_romWrite(startingAddress, &cmd, 1);

            /* Load Data into Buffer */
            cart_romWrite(startingAddress,
                          dataBuf + writtenCount,
                          writeLen);

            /* Issue Program Buffer to Flash command */
            cmd = 0x29;
            cart_romWrite(startingAddress, &cmd, 1);

            romWaitForDone(startingAddress + writeLen - 1, *(dataBuf + writtenCount + writeLen - 1));
            if (cmdBuf_p == 0)
            {
                uart_clearRecvBuf();
                return;
            }

            writtenCount += writeLen;
        }
    }

    uart_clearRecvBuf();
    uart_responAck();
}

// rom写入透传
// i 2B.包大小 0xf5 4B.始地址 nB.数据 2B.CRC
// o 0xaa
void romWrite()
{
    Desc_cmdBody_write_t *desc_write = (Desc_cmdBody_write_t *)(uart_cmd->payload);

    // 基地址
    uint32_t baseAddress = desc_write->baseAddress;
    // 写入总数量
    uint16_t byteCount = uart_cmd->cmdSize - SIZE_CMD_HEADER - SIZE_BASE_ADDRESS - SIZE_CRC;
    uint16_t wordCount = byteCount / 2;
    // 数据
    uint16_t *dataBuf = (uint16_t *)desc_write->payload;

    cart_romWrite(baseAddress,
                  dataBuf,
                  wordCount);

    uart_clearRecvBuf();
    uart_responAck();
}

// rom 读取透传
// i 2B.包大小 0xf6 4B.始地址 2B.读取数量 2B.CRC
// o 2B.CRC nB.数据
void romRead()
{
    Desc_cmdBody_read_t *desc_read = (Desc_cmdBody_read_t *)(uart_cmd->payload);

    // 基地址
    uint32_t baseAddress = desc_read->baseAddress;
    uint32_t wordAddress = baseAddress >> 1;
    // 读取总数量
    uint16_t byteCount = desc_read->readSize;
    uint16_t wordCount = byteCount / 2;
    // 数据
    uint16_t *dataBuf = (uint16_t *)uart_respon->payload;

    cart_romRead(
        wordAddress,
        dataBuf,
        wordCount);

    // 返回数据
    uart_clearRecvBuf();
    uart_responData(NULL, byteCount);
}

// ram写入
// i 2B.包大小 0xf7 4B.基地址 nB.写入数据 2B.CRC
// o 0xaa
void ramWrite()
{
    Desc_cmdBody_write_t *desc_write = (Desc_cmdBody_write_t *)(uart_cmd->payload);

    // 基地址
    uint32_t baseAddress = desc_write->baseAddress & 0xffff;
    // 写入总数量
    uint16_t byteCount = uart_cmd->cmdSize - SIZE_CMD_HEADER - SIZE_BASE_ADDRESS - SIZE_CRC;
    // 数据
    uint8_t *dataBuf = desc_write->payload;

    // 切bank操作移至上位机完成
    // // 切bank
    // uint16_t bank;
    // if (baseAddress & 0xffff0000)
    //     bank = 1;
    // else
    //     bank = 0;
    // cart_romWrite(0x800000, &bank, 1);

    cart_ramWrite((uint16_t)baseAddress, dataBuf, byteCount);

    // 回复ack
    uart_clearRecvBuf();
    uart_responAck();
}

// ram 读取
// i 2B.包大小 0xf8 4B.基地址 2B.读取数量 2B.CRC
// o 2B.CRC nB.数据
void ramRead()
{
    Desc_cmdBody_read_t *desc_read = (Desc_cmdBody_read_t *)(uart_cmd->payload);

    // 基地址
    uint32_t baseAddress = desc_read->baseAddress;
    // 读取总数量
    uint16_t byteCount = desc_read->readSize;
    // 数据
    uint8_t *dataBuf = uart_respon->payload;

    // 切bank操作移至上位机完成
    // // 切bank
    // uint16_t bank;
    // if (baseAddress & 0xffff0000)
    //     bank = 1;
    // else
    //     bank = 0;
    // cart_romWrite(0x800000, &bank, 1);

    cart_ramRead((uint16_t)baseAddress, dataBuf, byteCount);

    // 返回数据
    uart_clearRecvBuf();
    uart_responData(NULL, byteCount);
}

void ramProgramFlash()
{
    Desc_cmdBody_write_t *desc_write = (Desc_cmdBody_write_t *)(uart_cmd->payload);

    // 基地址
    uint32_t baseAddress = desc_write->baseAddress & 0xffff;
    // 写入总数量
    uint16_t byteCount = uart_cmd->cmdSize - SIZE_CMD_HEADER - SIZE_BASE_ADDRESS - SIZE_CRC;
    // 数据
    uint8_t *dataBuf = desc_write->payload;

    // 切bank在上位机完成

    // 逐字节写入
    uint8_t cmd;
    for (int i = 0; i < byteCount; i++)
    {
        cmd = 0xaa;
        cart_ramWrite(0x5555, &cmd, 1);
        cmd = 0x55;
        cart_ramWrite(0x2aaa, &cmd, 1);
        cmd = 0xa0;
        cart_ramWrite(0x5555, &cmd, 1); // FLASH_COMMAND_PROGRAM
        cart_ramWrite((uint16_t)(baseAddress + i), dataBuf + i, 1);

        // wait for done
        uint8_t temp;
        do
        {
            cart_ramRead((uint16_t)(baseAddress + i), &temp, 1);
            if (cmdBuf_p == 0)
            {
                uart_clearRecvBuf();
                return;
            }
        } while (temp != dataBuf[i]);
    }

    // 回复ack
    uart_clearRecvBuf();
    uart_responAck();
}

void ramWrite_forFram()
{
    Desc_cmdBody_write_t *desc_write = (Desc_cmdBody_write_t *)(uart_cmd->payload);

    // 基地址
    uint32_t baseAddress = desc_write->baseAddress & 0xffff;
    // 写入总数量
    uint16_t byteCount = uart_cmd->cmdSize - SIZE_CMD_HEADER - SIZE_BASE_ADDRESS - SIZE_CRC - SIZE_LATENCY;
    // 延迟周期
    uint8_t latency = desc_write->payload[0];
    // 数据
    uint8_t *dataBuf = desc_write->payload + SIZE_LATENCY;

    for (int i = 0; i < byteCount; i++)
    {
        cart_ramWrite((uint16_t)(baseAddress + i), dataBuf + i, 1); // 逐个字节写

        for (int ii = 0; ii < latency; ii++)
            __NOP();
    }

    // 回复ack
    uart_clearRecvBuf();
    uart_responAck();
}

void ramRead_forFram()
{
    Desc_cmdBody_read_t *desc_read = (Desc_cmdBody_read_t *)(uart_cmd->payload);

    // 基地址
    uint32_t baseAddress = desc_read->baseAddress;
    // 读取总数量
    uint16_t byteCount = desc_read->readSize;
    // 延迟周期
    uint8_t latency = uart_cmd->payload[SIZE_BASE_ADDRESS + SIZE_BYTE_COUNT];
    // 数据
    uint8_t *dataBuf = uart_respon->payload;

    for (int i = 0; i < byteCount; i++)
    {
        cart_ramRead((uint16_t)(baseAddress + i), dataBuf + i, 1); // 逐个字节读

        for (int ii = 0; ii < latency; ii++)
            __NOP();
    }

    // 返回数据
    uart_clearRecvBuf();
    uart_responData(NULL, byteCount);
}

////////////////////////////////////////////////////////////
/// 下面是gbc的功能
////////////////////////////////////////////////////////////

void gbcWrite()
{
    Desc_cmdBody_write_t *desc_write = (Desc_cmdBody_write_t *)(uart_cmd->payload);

    // 基地址
    uint32_t baseAddress = desc_write->baseAddress & 0xffff;
    // 写入总数量
    uint16_t byteCount = uart_cmd->cmdSize - SIZE_CMD_HEADER - SIZE_BASE_ADDRESS - SIZE_CRC;
    // 数据
    uint8_t *dataBuf = desc_write->payload;

    cart_gbcWrite((uint16_t)baseAddress, dataBuf, byteCount);

    // 回复ack
    uart_clearRecvBuf();
    uart_responAck();
}

void gbcRead()
{
    Desc_cmdBody_read_t *desc_read = (Desc_cmdBody_read_t *)(uart_cmd->payload);

    // 基地址
    uint32_t baseAddress = desc_read->baseAddress;
    // 读取总数量
    uint16_t byteCount = desc_read->readSize;
    // 数据
    uint8_t *dataBuf = uart_respon->payload;

    cart_gbcRead((uint16_t)baseAddress, dataBuf, byteCount);

    // 返回数据
    uart_clearRecvBuf();
    uart_responData(NULL, byteCount);
}

void gbcRomProgram()
{
    Desc_cmdBody_write_t *desc_write = (Desc_cmdBody_write_t *)(uart_cmd->payload);

    // 基地址
    uint32_t baseAddress = desc_write->baseAddress & 0xffff;
    // 写入总数量
    uint16_t byteCount = uart_cmd->cmdSize - SIZE_CMD_HEADER - SIZE_BASE_ADDRESS - SIZE_BUFF_SIZE - SIZE_CRC;
    // 编程buff大小
    uint16_t bufferWriteBytes = *((uint16_t *)(desc_write->payload));
    // 数据
    uint8_t *dataBuf = desc_write->payload + SIZE_BUFF_SIZE;

    uint32_t writtenCount = 0;

    while (writtenCount < byteCount)
    {
        uint8_t cmd;
        uint32_t startingAddress = baseAddress + writtenCount;

        // 不能多字节编程编程
        if (bufferWriteBytes == 0)
        {

            cmd = 0xaa;
            cart_gbcWrite(0xaaa, &cmd, 1);
            cmd = 0x55;
            cart_gbcWrite(0x555, &cmd, 1);
            cmd = 0xa0;
            cart_gbcWrite(0xaaa, &cmd, 1); // FLASH_COMMAND_PROGRAM
            cart_gbcWrite((uint16_t)(startingAddress), dataBuf + writtenCount, 1);

            // wait for done
            uint8_t temp;
            do
            {
                cart_gbcRead((uint16_t)(startingAddress), &temp, 1);
                if (cmdBuf_p == 0)
                {
                    uart_clearRecvBuf();
                    return;
                }
            } while (temp != dataBuf[writtenCount]);

            writtenCount++;
        }
        // 可以多字节编程
        else
        {
            uint16_t writeLen = byteCount - writtenCount;
            if (writeLen > bufferWriteBytes)
                writeLen = bufferWriteBytes;
            // uint32_t sectorAddress = startingAddress & 0xffff0000;

            cmd = 0xaa;
            cart_gbcWrite(0xaaa, &cmd, 1);
            cmd = 0x55;
            cart_gbcWrite(0x555, &cmd, 1);
            cmd = 0x25;
            cart_gbcWrite(startingAddress, &cmd, 1);

            cmd = writeLen - 1;
            cart_gbcWrite(startingAddress, &cmd, 1);

            cart_gbcWrite(startingAddress,
                          dataBuf + writtenCount,
                          writeLen);

            cmd = 0x29;
            cart_gbcWrite(startingAddress, &cmd, 1);

            // wait for done
            uint8_t temp;
            do
            {
                cart_gbcRead((uint16_t)(startingAddress + writeLen - 1), &temp, 1);
                if (cmdBuf_p == 0)
                {
                    uart_clearRecvBuf();
                    return;
                }
            } while (temp != *(dataBuf + writtenCount + writeLen - 1));

            writtenCount += writeLen;
        }
    }

    // 回复ack
    uart_clearRecvBuf();
    uart_responAck();
}

void gbcWrite_forFram()
{
    Desc_cmdBody_write_t *desc_write = (Desc_cmdBody_write_t *)(uart_cmd->payload);

    // 基地址
    uint32_t baseAddress = desc_write->baseAddress & 0xffff;
    // 写入总数量
    uint16_t byteCount = uart_cmd->cmdSize - SIZE_CMD_HEADER - SIZE_BASE_ADDRESS - SIZE_LATENCY - SIZE_CRC;
    // 延迟周期
    uint8_t latency = desc_write->payload[0];
    // 数据
    uint8_t *dataBuf = desc_write->payload + SIZE_LATENCY;

    for (int i = 0; i < byteCount; i++)
    {
        cart_gbcWrite((uint16_t)(baseAddress + i), dataBuf + i, 1); // 逐个字节写

        for (int ii = 0; ii < latency; ii++)
            __NOP();
    }

    // 回复ack
    uart_clearRecvBuf();
    uart_responAck();
}

void gbcRead_forFram()
{
    Desc_cmdBody_read_t *desc_read = (Desc_cmdBody_read_t *)(uart_cmd->payload);

    // 基地址
    uint32_t baseAddress = desc_read->baseAddress;
    // 读取总数量
    uint16_t byteCount = desc_read->readSize;
    // 延迟周期
    uint8_t latency = uart_cmd->payload[SIZE_BASE_ADDRESS + SIZE_BYTE_COUNT];
    // 数据
    uint8_t *dataBuf = uart_respon->payload;

    for (int i = 0; i < byteCount; i++)
    {
        cart_gbcRead((uint16_t)(baseAddress + i), dataBuf + i, 1); // 逐个字节读

        for (int ii = 0; ii < latency; ii++)
            __NOP();
    }

    // 返回数据
    uart_clearRecvBuf();
    uart_responData(NULL, byteCount);
}
