## Definition Files

```
Name: "arm-a32-linux"
CalculationBytes: 4
AdressBytes: 4
WorkingRegisterStart: 3
PlaceToKeepRegisterStart: 1
WorkingRegisterLast: 10
PlaceToKeepRegisterLast: 2
FramePointer:11
ResultRegister:12
ZuweisungsRegister: 0
AviableRegisters: [ "r0", "r1", "r2", "r3", "r4", "r5", "r6", "r7", "r8", "r9", "r10", "r11", "r12", "sp", "lr", "pc" ]
KeyPatterns: [
    "[POPREG]": "{0},{1}",
    "[PUSHREG]": "{0},{1}",
]
Algos: [
    {
        Name: "MovReg"
        Mode: "default"
        Description: "Um den return einer einzelnen Variable zu ermöglichen"
        Keys: [
            "[SSAPOP]": [ "1" ],
            "[SSAPUSH]",
        ],
        PostKeys: []
        Assemblys: [
            mov [SSAPUSH],[SSAPOP[0]]
        ]
    },
    {
        Name: "CompileHeader"
        Mode: "default"
        Description: "Das setzen des Headers, hier wird der Stack und der Heap initialisiert"
        Assemblys: [
            .syntax unified
            ldr r0,=THE_END
            mov r10,r0
            ldr r11,=0x1
            sub r12,sp,r10
            str r11,[r10,#0]
            str r12,[r10,#4]
            ldr r12,=main
            blx r12
            mov r0, r12
            mov r7, #1
            svc #0
        ]
    },
]
```