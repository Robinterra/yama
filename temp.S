.syntax unified
.thumb
mov r0, r12
mov r10,r0
ldr r11,=0x1
sub r12,sp,r10
str r11,[r10,#0]
str r12,[r10,#4]
ldr r12,=main
blx r12
mov r1, r12
mov r12, #3
exec r12
main:
push {r7,r8,r9,r10,r11,lr}
sub sp, sp, #0x20
ldr r12, =JUMPHELPER_1
str r12, [sp,#4]
mov r11, r12
ldr r12, =InputOutput_Exist_1
mov r1,r11
blx r12
mov r11, r12
ldr r12, =bool_Achtung_1
mov r1,r11
blx r12
cmp r12, #0
beq JUMPHELPER_2
ldr r12, =JUMPHELPER_3
mov r11, r12
ldr r12, =Console_PrintText_1
mov r1,r11
blx r12
b JUMPHELPER_4
JUMPHELPER_2:
ldr r12, =JUMPHELPER_5
mov r11, r12
ldr r12, =Console_PrintText_1
mov r1,r11
blx r12
ldr r12, =Console_ReadText_0
blx r12
str r12, [sp,#12]
ldr r12, =JUMPHELPER_6
mov r11, r12
ldr r12, =Console_PrintText_1
mov r1,r11
blx r12
ldr r12, =Console_ReadText_0
blx r12
str r12, [sp,#16]
ldr r12, =JUMPHELPER_7
mov r11, r12
ldr r12, =Console_PrintText_1
mov r1,r11
blx r12
ldr r12, =Console_Read_0
blx r12
str r12, [sp,#20]
ldr r12,[sp,#4]
mov r11, r12
mov r10, r12
ldr r12, =ReadStream_new_2
mov r1,r10
mov r2,r11
blx r12
str r12, [sp,#24]
ldr r12,[sp,#16]
mov r11, r12
mov r10, r12
ldr r12, =WriteStream_new_2
mov r1,r10
mov r2,r11
blx r12
str r12, [sp,#28]
mov r12,#0x64
mov r11, r12
ldr r12,[sp,#28]
mov r10, r12
ldr r12,[sp,#24]
mov r9, r12
ldr r12,[sp,#12]
mov r8, r12
mov r7, r12
ldr r12, =Sacrament_new_5
mov r1,r7
mov r2,r8
mov r3,r9
mov r4,r10
mov r5,r11
blx r12
str r12, [sp,#32]
mov r12,#0x1
mov r11, r12
ldr r12,[sp,#20]
mov r10, r12
ldr r12, =int_Equal_2
mov r1,r10
mov r2,r11
blx r12
cmp r12, #0
beq JUMPHELPER_8
ldr r12,[sp,#32]
mov r11, r12
ldr r12, =Sacrament_Encrypt_1
mov r1,r11
blx r12
JUMPHELPER_8:
mov r12,#0x2
mov r11, r12
ldr r12,[sp,#20]
mov r10, r12
ldr r12, =int_Equal_2
mov r1,r10
mov r2,r11
blx r12
cmp r12, #0
beq JUMPHELPER_9
ldr r12, =JUMPHELPER_10
mov r11, r12
ldr r12, =Console_PrintText_1
mov r1,r11
blx r12
JUMPHELPER_9:
ldr r12,[sp,#32]
mov r11, r12
ldr r12, =Sacrament_DeCtor_1
mov r1,r11
blx r12
ldr r12,[sp,#24]
mov r11, r12
ldr r12, =ReadStream_DeCtor_1
mov r1,r11
blx r12
ldr r12,[sp,#28]
mov r11, r12
ldr r12, =WriteStream_DeCtor_1
mov r1,r11
blx r12
mov r12,#0x0
b JUMPHELPER_4
mov r12,#0x0
b JUMPHELPER_4
JUMPHELPER_4:
add sp, sp, #0x20
pop {r7,r8,r9,r10,r11,lr}
bx lr
Sacrament_Encrypt_1:
push {r10,r11,lr}
sub sp, sp, #0xC
str r1, [sp,#4]
JUMPHELPER_12:
ldr r12,[sp,#4]
ldr r12,[r12,#0xC]
mov r11, r12
ldr r12,[sp,#4]
ldr r12,[r12,#0x4]
mov r10, r12
ldr r12,[r12,#0x0]
ldr r12,[r12,#0x4]
mov r1,r10
mov r2,r11
blx r12
str r12, [sp,#8]
cmp r12, #0
beq JUMPHELPER_13
ldr r12,[sp,#8]
mov r11, r12
ldr r12,[sp,#4]
mov r10, r12
ldr r12, =Sacrament_EncryptBlock_2
mov r1,r10
mov r2,r11
blx r12
str r12, [sp,#12]
mov r11, r12
ldr r12,[sp,#4]
ldr r12,[r12,#0x8]
mov r10, r12
ldr r12,[r12,#0x0]
ldr r12,[r12,#0x0]
mov r1,r10
mov r2,r11
blx r12
ldr r12,[sp,#12]
mov r11, r12
ldr r12, =ByteArray_DeCtor_1
mov r1,r11
blx r12
ldr r12,[sp,#8]
mov r11, r12
ldr r12, =ByteArray_DeCtor_1
mov r1,r11
blx r12
b JUMPHELPER_12
JUMPHELPER_13:
mov r12,#0xFF
b JUMPHELPER_14
JUMPHELPER_14:
add sp, sp, #0xC
pop {r10,r11,lr}
bx lr
Sacrament_EncryptBlock_2:
push {r9,r10,r11,lr}
sub sp, sp, #0x20
str r1, [sp,#4]
str r2, [sp,#8]
ldr r12,[sp,#8]
mov r11, r12
ldr r12, =ByteArray_Size_2_Get
mov r1,r11
blx r12
str r12, [sp,#12]
mov r11, r12
ldr r12, =ByteArray_New_2_Get
mov r1,r11
blx r12
str r12, [sp,#16]
ldr r12,[sp,#4]
ldr r12,[r12,#0x0]
str r12, [sp,#20]
mov r12,#0x0
str r12, [sp,#24]
mov r12,#0x0
str r12, [sp,#28]
b JUMPHELPER_16
JUMPHELPER_17:
mov r12,#0x1
mov r11, r12
ldr r12,[sp,#28]
mov r10, r12
mov r1,r10
mov r2,r11
add r12,r1,r2
str r12, [sp,#28]
JUMPHELPER_16:
ldr r12,[sp,#12]
mov r11, r12
ldr r12,[sp,#28]
mov r10, r12
ldr r12, =int_LessThen_2
mov r1,r10
mov r2,r11
blx r12
cmp r12, #0
beq JUMPHELPER_18
ldr r12,[sp,#28]
mov r11, r12
ldr r12,[sp,#8]
mov r10, r12
ldr r12, =ByteArray_Data_3_Get
mov r1,r10
mov r2,r11
blx r12
str r12, [sp,#32]
ldr r12,[sp,#24]
mov r11, r12
ldr r12,[sp,#20]
mov r10, r12
ldr r12, =ByteArray_Data_3_Get
mov r1,r10
mov r2,r11
blx r12
mov r11, r12
ldr r12,[sp,#32]
mov r10, r12
mov r1,r10
mov r2,r11
eor r12,r1,r2
mov r11, r12
ldr r12,[sp,#28]
mov r10, r12
ldr r12,[sp,#16]
mov r9, r12
ldr r12, =ByteArray_Data_3_Set
mov r1,r9
mov r2,r10
mov r3,r11
blx r12
mov r12,#0x1
mov r11, r12
ldr r12,[sp,#24]
mov r10, r12
mov r1,r10
mov r2,r11
add r12,r1,r2
str r12, [sp,#24]
ldr r12,[sp,#20]
mov r11, r12
ldr r12, =ByteArray_Size_2_Get
mov r1,r11
blx r12
mov r11, r12
ldr r12,[sp,#24]
mov r10, r12
ldr r12, =int_LessThen_2
mov r1,r10
mov r2,r11
blx r12
cmp r12, #0
beq JUMPHELPER_19
b JUMPHELPER_17
JUMPHELPER_19:
mov r12,#0x0
str r12, [sp,#24]
b JUMPHELPER_17
JUMPHELPER_18:
ldr r12,[sp,#16]
b JUMPHELPER_20
JUMPHELPER_20:
add sp, sp, #0x20
pop {r9,r10,r11,lr}
bx lr
Sacrament_new_5:
push {r10,r11,lr}
sub sp, sp, #0x14
str r1, [sp,#4]
str r2, [sp,#8]
str r3, [sp,#12]
str r4, [sp,#16]
str r5, [sp,#20]
mov r12,#0x10
mov r11, r12
mov r1,r11
ldr r12, =MemoryManager_Malloc_1
blx r12
mov r11, r12
str r12, [sp,#4]
ldr r12,[sp,#8]
mov r10, r12
ldr r12,[sp,#4]
str r10,[r12,#0x0]
ldr r12,[sp,#12]
mov r10, r12
ldr r12,[sp,#4]
str r10,[r12,#0x4]
ldr r12,[sp,#16]
mov r10, r12
ldr r12,[sp,#4]
str r10,[r12,#0x8]
ldr r12,[sp,#20]
mov r10, r12
ldr r12,[sp,#4]
str r10,[r12,#0xC]
ldr r12,[sp,#4]
b JUMPHELPER_22
JUMPHELPER_22:
add sp, sp, #0x14
pop {r10,r11,lr}
bx lr
Sacrament_DeCtor_1:
push {r11,lr}
sub sp, sp, #0x4
str r1, [sp,#4]
str r1, [sp,#4]
ldr r12,[sp,#4]
mov r11, r12
mov r2,r11
ldr r12, =MemoryManager_Free_1
blx r12
add sp, sp, #0x4
pop {r11,lr}
bx lr
bool_Achtung_1:
push {lr}
sub sp, sp, #0x4
str r1, [sp,#4]
ldr r12,[sp,#4]
cmp r12, #0
beq JUMPHELPER_26
mov r12,#0x0
b JUMPHELPER_27
JUMPHELPER_26:
mov r12,#0xFF
b JUMPHELPER_27
JUMPHELPER_27:
add sp, sp, #0x4
pop {lr}
bx lr
int_LessThen_2:
cmp r1,r2
mov r12, #0xff
blt JUMPHELPER_29
mov r12, #0x0
JUMPHELPER_29:
bx lr
int_GreaterThen_2:
cmp r1,r2
mov r12, #0xff
bgt JUMPHELPER_32
mov r12, #0x0
JUMPHELPER_32:
bx lr
int_LessThenEquals_2:
cmp r1,r2
mov r12, #0xff
ble JUMPHELPER_35
mov r12, #0x0
JUMPHELPER_35:
bx lr
int_Equal_2:
cmp r1,r2
mov r12, #0xff
beq JUMPHELPER_38
mov r12, #0x0
JUMPHELPER_38:
bx lr
MemoryManager_Malloc_1:
push {r8,r9,r10,r11,lr}
sub sp, sp, #0x14
str r1, [sp,#4]
mov lr, r0
mov r12, lr
str r12, [sp,#20]
JUMPHELPER_41:
mov r12,#0xFF
cmp r12, #0
beq JUMPHELPER_42
ldr r12,[sp,#20]
str r12, [sp,#8]
ldr r12, [r12, #4]
str r12, [sp,#16]
ldr r12,[sp,#8]
ldr r11, [r12, #4]
add r12, r11, r12
add r12, r12, #8
str r12, [sp,#20]
ldr r12,[sp,#8]
ldr r12,[r12, #0]
cmp r12, #2
beq JUMPHELPER_43
mov r12, #0x0
JUMPHELPER_43:
cmp r12, #0
beq JUMPHELPER_44
b JUMPHELPER_41
JUMPHELPER_44:
ldr r12,[sp,#16]
mov r11, r12
ldr r12,[sp,#4]
mov r10, r12
ldr r12, =int_LessThenEquals_2
mov r1,r10
mov r2,r11
blx r12
cmp r12, #0
beq JUMPHELPER_45
b JUMPHELPER_42
JUMPHELPER_45:
ldr r12,[sp,#8]
ldr r12, [r12, #0]
cmp r12, #1
beq JUMPHELPER_46
mov r12, #0x0
JUMPHELPER_46:
cmp r12, #0
beq JUMPHELPER_47
mov r12,#0x0
b JUMPHELPER_48
JUMPHELPER_47:
b JUMPHELPER_41
JUMPHELPER_42:
ldr r12,[sp,#8]
ldr r12, [r12]
str r12, [sp,#12]
ldr r12,[sp,#16]
mov r11, r12
ldr r12,[sp,#12]
mov r10, r12
ldr r12,[sp,#4]
mov r9, r12
ldr r12,[sp,#8]
mov r8, r12
ldr r12, =MemoryManager_ReservedNewBlock_4
mov r1,r8
mov r2,r9
mov r3,r10
mov r4,r11
blx r12
b JUMPHELPER_48
JUMPHELPER_48:
add sp, sp, #0x14
pop {r8,r9,r10,r11,lr}
bx lr
MemoryManager_ReservedNewBlock_4:
push {r9,r10,r11,lr}
sub sp, sp, #0x14
str r1, [sp,#4]
str r2, [sp,#8]
str r3, [sp,#12]
str r4, [sp,#16]
ldr r12,[sp,#8]
mov r11, r12
mov r12,#0x2
mov r10, r12
ldr r12,[sp,#4]
mov r9, r12
ldr r12, =MemoryManager_CreateBlock_3
mov r1,r9
mov r2,r10
mov r3,r11
blx r12
mov r12, #0x8
mov r11, r12
ldr r12,[sp,#4]
mov r10, r12
mov r1,r10
mov r2,r11
add r12,r1,r2
str r12, [sp,#4]
mov r12,#0x0
mov r11, r12
ldr r12,[sp,#12]
mov r10, r12
ldr r12, =int_Equal_2
mov r1,r10
mov r2,r11
blx r12
cmp r12, #0
beq JUMPHELPER_50
ldr r12,[sp,#4]
b JUMPHELPER_51
JUMPHELPER_50:
ldr r12,[sp,#8]
mov r11, r12
ldr r12,[sp,#4]
mov r10, r12
mov r1,r10
mov r2,r11
add r12,r1,r2
str r12, [sp,#20]
mov r12, #0x8
mov r11, r12
ldr r12,[sp,#8]
mov r10, r12
mov r1,r10
mov r2,r11
add r12,r1,r2
str r12, [sp,#8]
mov r11, r12
ldr r12,[sp,#16]
mov r10, r12
mov r1,r10
mov r2,r11
sub r12,r1,r2
str r12, [sp,#8]
mov r11, r12
mov r12,#0x1
mov r10, r12
ldr r12,[sp,#20]
mov r9, r12
ldr r12, =MemoryManager_CreateBlock_3
mov r1,r9
mov r2,r10
mov r3,r11
blx r12
ldr r12,[sp,#4]
b JUMPHELPER_51
JUMPHELPER_51:
add sp, sp, #0x14
pop {r9,r10,r11,lr}
bx lr
MemoryManager_CreateBlock_3:
push {lr}
sub sp, sp, #0xC
str r1, [sp,#4]
str r2, [sp,#8]
str r3, [sp,#12]
ldr r12,[sp,#4]
mov lr, r12
ldr r12,[sp,#8]
str r12, [lr, #0]
ldr r12,[sp,#12]
str r12, [lr, #4]
add sp, sp, #0xC
pop {lr}
bx lr
MemoryManager_Free_1:
push {r10,r11,lr}
sub sp, sp, #0x4
str r1, [sp,#4]
mov r12,#0x0
mov r11, r12
ldr r12,[sp,#4]
mov r10, r12
ldr r12, =int_Equal_2
mov r1,r10
mov r2,r11
blx r12
cmp r12, #0
beq JUMPHELPER_55
mov r12,#0x0
b JUMPHELPER_56
JUMPHELPER_55:
ldr r12,[sp,#4]
sub r12, r12, #8
mov r11, #0
str r11,[r12]
mov r12, #0x8
mov r11, r12
ldr r12,[sp,#4]
mov r10, r12
mov r1,r10
mov r2,r11
sub r12,r1,r2
mov r11, r12
ldr r12, =MemoryManager_CleanMerge2Blocks_1
mov r1,r11
blx r12
b JUMPHELPER_56
JUMPHELPER_56:
add sp, sp, #0x4
pop {r10,r11,lr}
bx lr
MemoryManager_CleanMerge2Blocks_1:
push {lr}
sub sp, sp, #0x4
str r1, [sp,#4]
ldr r12,[sp,#4]
push {r10}
mov lr,r12
ldr r11,[lr,#0]
cmp r11, #0
bne JUMPHELPER_58
ldr r11,[lr,#4]
add lr,lr,r11
add lr,lr,#8
ldr r10,[lr,#0]
cmp r10,#0x2
beq JUMPHELPER_58
ldr r11,[lr, #4]
add r11, r11, #8
mov lr,r12
str r10,[lr,#0]
ldr r10,[lr, #4]
add r10, r10, r11
str r10,[lr,#4]
JUMPHELPER_58:
pop {r10}
mov r12,#0xFF
b JUMPHELPER_59
JUMPHELPER_59:
add sp, sp, #0x4
pop {lr}
bx lr
ByteArray_DeCtor_1:
push {r11,lr}
sub sp, sp, #0x4
str r1, [sp,#4]
str r1, [sp,#4]
ldr r12,[sp,#4]
mov r11, r12
mov r2,r11
ldr r12, =MemoryManager_Free_1
blx r12
add sp, sp, #0x4
pop {r11,lr}
bx lr
ByteArray_Data_3_Get:
push {r9,r10,r11,lr}
sub sp, sp, #0x14
str r1, [sp,#4]
str r2, [sp,#8]
ldr r12,[sp,#4]
ldr r12,[r12,#0x0]
mov r11, r12
ldr r12,[sp,#8]
mov r10, r12
ldr r12, =int_GreaterThen_2
mov r1,r10
mov r2,r11
blx r12
cmp r12, #0
beq JUMPHELPER_63
mov r12,#0x1
b JUMPHELPER_64
JUMPHELPER_63:
ldr r12,[sp,#4]
mov r11, r12
ldr r12,[sp,#8]
mov r10, r12
mov r12,#0x4
mov r9, r12
mov r1,r9
mov r2,r10
add r12,r1,r2
mov r10, r12
mov r1,r10
mov r2,r11
add r12,r1,r2
str r12, [sp,#12]
mov r12,#0x3
mov r11, r12
ldr r12,[sp,#12]
mov r10, r12
mov r1,r10
mov r2,r11
and r12,r1,r2
str r12, [sp,#16]
mov r11, r12
ldr r12,[sp,#12]
mov r10, r12
mov r1,r10
mov r2,r11
eor r12,r1,r2
str r12, [sp,#20]
mov r12,#0x3
mov r11, r12
ldr r12,[sp,#8]
mov r10, r12
mov r1,r10
mov r2,r11
and r12,r1,r2
str r12, [sp,#16]
ldr r12,[sp,#20]
ldr r12,[r12,#0x0]
str r12, [sp,#12]
mov r12,#0x3
mov r11, r12
ldr r12,[sp,#16]
mov r10, r12
mov r1,r10
mov r2,r11
lsl r12,r1,r2
mov r11, r12
ldr r12,[sp,#12]
mov r10, r12
mov r1,r10
mov r2,r11
asr r12,r1,r2
str r12, [sp,#12]
mov r12,#0xFF
mov r11, r12
ldr r12,[sp,#12]
mov r10, r12
mov r1,r10
mov r2,r11
and r12,r1,r2
b JUMPHELPER_64
JUMPHELPER_64:
add sp, sp, #0x14
pop {r9,r10,r11,lr}
bx lr
ByteArray_Data_3_Set:
push {r9,r10,r11,lr}
sub sp, sp, #0x18
str r1, [sp,#4]
str r2, [sp,#12]
str r3, [sp,#8]
ldr r12,[sp,#4]
ldr r12,[r12,#0x0]
mov r11, r12
ldr r12,[sp,#12]
mov r10, r12
ldr r12, =int_GreaterThen_2
mov r1,r10
mov r2,r11
blx r12
cmp r12, #0
beq JUMPHELPER_66
mov r12,#0x0
b JUMPHELPER_67
JUMPHELPER_66:
mov r12,#0xFF
mov r11, r12
ldr r12,[sp,#8]
mov r10, r12
mov r1,r10
mov r2,r11
and r12,r1,r2
str r12, [sp,#8]
ldr r12,[sp,#4]
mov r11, r12
ldr r12,[sp,#12]
mov r10, r12
mov r12,#0x4
mov r9, r12
mov r1,r9
mov r2,r10
add r12,r1,r2
mov r10, r12
mov r1,r10
mov r2,r11
add r12,r1,r2
str r12, [sp,#16]
mov r12,#0x3
mov r11, r12
ldr r12,[sp,#16]
mov r10, r12
mov r1,r10
mov r2,r11
and r12,r1,r2
str r12, [sp,#20]
mov r11, r12
ldr r12,[sp,#16]
mov r10, r12
mov r1,r10
mov r2,r11
eor r12,r1,r2
str r12, [sp,#24]
mov r12,#0x3
mov r11, r12
ldr r12,[sp,#20]
mov r10, r12
mov r1,r10
mov r2,r11
lsl r12,r1,r2
str r12, [sp,#20]
mov r11, r12
ldr r12,[sp,#8]
mov r10, r12
mov r1,r10
mov r2,r11
lsl r12,r1,r2
str r12, [sp,#8]
ldr r12,[sp,#20]
mov r11, r12
mov r12,#0xFF
mov r10, r12
mov r1,r10
mov r2,r11
lsl r12,r1,r2
str r12, [sp,#20]
mov r11, r12
ldr r12,[sp,#24]
ldr r12,[r12,#0x0]
mov r10, r12
mov r1,r10
mov r2,r11
orr r12,r1,r2
mov r11, r12
ldr r12,[sp,#20]
mov r10, r12
mov r1,r10
mov r2,r11
eor r12,r1,r2
mov r11, r12
ldr r12,[sp,#24]
str r11,[r12,#0x0]
ldr r12,[sp,#8]
mov r11, r12
ldr r12,[sp,#24]
ldr r12,[r12,#0x0]
mov r10, r12
mov r1,r10
mov r2,r11
orr r12,r1,r2
mov r11, r12
ldr r12,[sp,#24]
str r11,[r12,#0x0]
JUMPHELPER_67:
add sp, sp, #0x18
pop {r9,r10,r11,lr}
bx lr
ByteArray_New_2_Get:
push {r10,r11,lr}
sub sp, sp, #0xC
str r1, [sp,#4]
ldr r12,[sp,#4]
str r12, [sp,#8]
mov r12,#0x4
mov r11, r12
ldr r12,[sp,#4]
mov r10, r12
mov r1,r10
mov r2,r11
add r12,r1,r2
str r12, [sp,#4]
mov r12,#0x3
mov r11, r12
ldr r12,[sp,#4]
mov r10, r12
mov r1,r10
mov r2,r11
and r12,r1,r2
cmp r12, #0
beq JUMPHELPER_69
mov r12,#0x3
mov r11, r12
ldr r12,[sp,#4]
mov r10, r12
mov r1,r10
mov r2,r11
orr r12,r1,r2
str r12, [sp,#4]
mov r12,#0x3
mov r11, r12
ldr r12,[sp,#4]
mov r10, r12
mov r1,r10
mov r2,r11
eor r12,r1,r2
mov r11, r12
mov r12,#0x4
mov r10, r12
mov r1,r10
mov r2,r11
add r12,r1,r2
str r12, [sp,#4]
JUMPHELPER_69:
ldr r12,[sp,#4]
mov r11, r12
ldr r12, =MemoryManager_Malloc_1
mov r1,r11
blx r12
str r12, [sp,#12]
ldr r12,[sp,#8]
mov r11, r12
ldr r12,[sp,#12]
str r11,[r12,#0x0]
ldr r12,[sp,#12]
b JUMPHELPER_70
JUMPHELPER_70:
add sp, sp, #0xC
pop {r10,r11,lr}
bx lr
ByteArray_New_2_Set:
push {lr}
sub sp, sp, #0x8
str r1, [sp,#8]
add sp, sp, #0x8
pop {lr}
bx lr
ByteArray_Size_2_Get:
push {lr}
sub sp, sp, #0x4
str r1, [sp,#4]
ldr r12,[sp,#4]
ldr r12,[r12,#0x0]
b JUMPHELPER_74
JUMPHELPER_74:
add sp, sp, #0x4
pop {lr}
bx lr
ByteArray_Size_2_Set:
push {lr}
sub sp, sp, #0x4
str r1, [sp,#4]
add sp, sp, #0x4
pop {lr}
bx lr
Console_PrintText_1:
push {lr}
sub sp, sp, #0x4
str r1, [sp,#4]
ldr r12,[sp,#4]
mov r1, r12
mov r12, #4
exec r12
add sp, sp, #0x4
pop {lr}
bx lr
Console_Read_0:
push {lr}
sub sp, sp, #0x0
mov r12, #2
exec r12
add sp, sp, #0x0
pop {lr}
bx lr
Console_ReadText_0:
push {lr}
sub sp, sp, #0x0
mov r12, #5
exec r12
add sp, sp, #0x0
pop {lr}
bx lr
InputOutput_ExecWith2Args_4:
push {lr}
sub sp, sp, #0x10
str r4, [sp,#16]
ldr r12,[sp,#16]
exec r12
add sp, sp, #0x10
pop {lr}
bx lr
InputOutput_ExecWith1Arg_3:
push {lr}
sub sp, sp, #0xC
str r3, [sp,#12]
ldr r12,[sp,#12]
exec r12
add sp, sp, #0xC
pop {lr}
bx lr
InputOutput_Exist_1:
push {r9,r10,r11,lr}
sub sp, sp, #0x4
str r1, [sp,#4]
mov r12,#0x6
mov r11, r12
ldr r12,[sp,#4]
mov r10, r12
mov r12,#0x5
mov r9, r12
ldr r12, =InputOutput_ExecWith1Arg_3
mov r1,r9
mov r2,r10
mov r3,r11
blx r12
add sp, sp, #0x4
pop {r9,r10,r11,lr}
bx lr
Stream_WriteNextBytes_2:
push {lr}
sub sp, sp, #0x4
str r1, [sp,#4]
mov r12,#0x0
b JUMPHELPER_91
JUMPHELPER_91:
add sp, sp, #0x4
pop {lr}
bx lr
Stream_ReadNextBytes_2:
push {lr}
sub sp, sp, #0x4
str r1, [sp,#4]
mov r12,#0x0
b JUMPHELPER_93
JUMPHELPER_93:
add sp, sp, #0x4
pop {lr}
bx lr
WriteStream_WriteNextBytes_2:
push {r8,r9,r10,r11,lr}
sub sp, sp, #0xC
str r1, [sp,#4]
ldr r12, =JUMPHELPER_89
str r12, [sp,#8]
str r2, [sp,#12]
mov r12,#0x6
mov r11, r12
ldr r12,[sp,#12]
mov r10, r12
ldr r12,[sp,#4]
ldr r12,[r12,#0x4]
mov r9, r12
mov r12,#0xA
mov r8, r12
ldr r12, =InputOutput_ExecWith2Args_4
mov r1,r8
mov r2,r9
mov r3,r10
mov r4,r11
blx r12
b JUMPHELPER_96
JUMPHELPER_96:
add sp, sp, #0xC
pop {r8,r9,r10,r11,lr}
bx lr
WriteStream_new_2:
push {r8,r9,r10,r11,lr}
sub sp, sp, #0xC
str r1, [sp,#4]
ldr r12, =JUMPHELPER_89
str r12, [sp,#8]
str r2, [sp,#12]
mov r12,#0x8
mov r11, r12
mov r1,r11
ldr r12, =MemoryManager_Malloc_1
blx r12
mov r11, r12
str r12, [sp,#4]
ldr r12, =JUMPHELPER_94
mov r10, r12
ldr r12,[sp,#4]
str r10,[r12,#0x0]
mov r12,#0x6
mov r10, r12
ldr r12,[sp,#12]
mov r9, r12
mov r12,#0x9
mov r8, r12
ldr r12, =InputOutput_ExecWith1Arg_3
mov r1,r8
mov r2,r9
mov r3,r10
blx r12
mov r10, r12
ldr r12,[sp,#4]
str r10,[r12,#0x4]
ldr r12,[sp,#4]
b JUMPHELPER_98
JUMPHELPER_98:
add sp, sp, #0xC
pop {r8,r9,r10,r11,lr}
bx lr
WriteStream_DeCtor_1:
push {r9,r10,r11,lr}
sub sp, sp, #0x8
str r1, [sp,#4]
ldr r12, =JUMPHELPER_89
str r12, [sp,#8]
str r1, [sp,#4]
ldr r12,[sp,#4]
mov r11, r12
mov r2,r11
ldr r12, =MemoryManager_Free_1
blx r12
mov r12,#0x6
mov r11, r12
ldr r12,[sp,#4]
ldr r12,[r12,#0x4]
mov r10, r12
mov r12,#0x8
mov r9, r12
ldr r12, =InputOutput_ExecWith1Arg_3
mov r1,r9
mov r2,r10
mov r3,r11
blx r12
add sp, sp, #0x8
pop {r9,r10,r11,lr}
bx lr
ReadStream_ReadNextBytes_2:
push {r8,r9,r10,r11,lr}
sub sp, sp, #0xC
str r1, [sp,#4]
ldr r12, =JUMPHELPER_89
str r12, [sp,#8]
str r2, [sp,#12]
mov r12,#0x6
mov r11, r12
ldr r12,[sp,#12]
mov r10, r12
ldr r12,[sp,#4]
ldr r12,[r12,#0x4]
mov r9, r12
mov r12,#0x7
mov r8, r12
ldr r12, =InputOutput_ExecWith2Args_4
mov r1,r8
mov r2,r9
mov r3,r10
mov r4,r11
blx r12
b JUMPHELPER_103
JUMPHELPER_103:
add sp, sp, #0xC
pop {r8,r9,r10,r11,lr}
bx lr
ReadStream_new_2:
push {r8,r9,r10,r11,lr}
sub sp, sp, #0xC
str r1, [sp,#4]
ldr r12, =JUMPHELPER_89
str r12, [sp,#8]
str r2, [sp,#12]
mov r12,#0x8
mov r11, r12
mov r1,r11
ldr r12, =MemoryManager_Malloc_1
blx r12
mov r11, r12
str r12, [sp,#4]
ldr r12, =JUMPHELPER_101
mov r10, r12
ldr r12,[sp,#4]
str r10,[r12,#0x0]
mov r12,#0x6
mov r10, r12
ldr r12,[sp,#12]
mov r9, r12
mov r12,#0x6
mov r8, r12
ldr r12, =InputOutput_ExecWith1Arg_3
mov r1,r8
mov r2,r9
mov r3,r10
blx r12
mov r10, r12
ldr r12,[sp,#4]
str r10,[r12,#0x4]
ldr r12,[sp,#4]
b JUMPHELPER_105
JUMPHELPER_105:
add sp, sp, #0xC
pop {r8,r9,r10,r11,lr}
bx lr
ReadStream_DeCtor_1:
push {r9,r10,r11,lr}
sub sp, sp, #0x8
str r1, [sp,#4]
ldr r12, =JUMPHELPER_89
str r12, [sp,#8]
str r1, [sp,#4]
ldr r12,[sp,#4]
mov r11, r12
mov r2,r11
ldr r12, =MemoryManager_Free_1
blx r12
mov r12,#0x6
mov r11, r12
ldr r12,[sp,#4]
ldr r12,[r12,#0x4]
mov r10, r12
mov r12,#0x8
mov r9, r12
ldr r12, =InputOutput_ExecWith1Arg_3
mov r1,r9
mov r2,r10
mov r3,r11
blx r12
add sp, sp, #0x8
pop {r9,r10,r11,lr}
bx lr
.data JUMPHELPER_1="input.txt"
.data JUMPHELPER_3="file not found"
.data JUMPHELPER_5="Please enter your Password: "
.data JUMPHELPER_6="Please enter your output File: "
.data JUMPHELPER_7="Encrypt or decrypt (1/2): "
.data JUMPHELPER_10="Decrypt"
.datalist JUMPHELPER_89={Stream_WriteNextBytes_2,Stream_ReadNextBytes_2}
.datalist JUMPHELPER_94={WriteStream_WriteNextBytes_2}
.datalist JUMPHELPER_101={ReadStream_ReadNextBytes_2}
