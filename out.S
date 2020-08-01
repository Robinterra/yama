ldd r2,Y+5
ldd r3,Y+6
movw r4, r2
ldd r2,Y+3
ldd r3,Y+4
ldi r2,lo8(gs(+))
ldi r3,hi8(gs(+))
std Y+7,r2
std Y+8,r3
