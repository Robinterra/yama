__SP_H__ = 0x3e
__SP_L__ = 0x3d
__SREG__ = 0x3f
__tmp_reg__ = 0
__zero_reg__ = 1
ProgramTest_Main_0:
push r16
push r17
push r18
push r19
push r20
push r21
push r28
push r29
in r28,__SP_L__
in r29,__SP_H__
ldd r24,Y+5
ldd r25,Y+6
movw r16, r24
ldd r24,Y+3
ldd r25,Y+4
movw r18, r24
ldi r24,lo8(gs(Int_Addition_2))
ldi r25,hi8(gs(Int_Addition_2))
movw r30, r24
icall
std Y+7,r24
std Y+8,r25
adiw r28, 6
in __tmp_reg__,__SREG__
cli
out __SP_H__,r29
out __SREG__,__tmp_reg__
out __SP_L__,r28
pop r29
pop r28
pop r21
pop r20
pop r19
pop r18
pop r17
pop r16
ret
ProgramTest_new_0:
push r16
push r17
push r18
push r19
push r20
push r21
push r28
push r29
in r28,__SP_L__
in r29,__SP_H__
adiw r28, 6
in __tmp_reg__,__SREG__
cli
out __SP_H__,r29
out __SREG__,__tmp_reg__
out __SP_L__,r28
pop r29
pop r28
pop r21
pop r20
pop r19
pop r18
pop r17
pop r16
ret
ProgramTest_DeCtor_0:
push r16
push r17
push r18
push r19
push r20
push r21
push r28
push r29
in r28,__SP_L__
in r29,__SP_H__
adiw r28, 6
in __tmp_reg__,__SREG__
cli
out __SP_H__,r29
out __SREG__,__tmp_reg__
out __SP_L__,r28
pop r29
pop r28
pop r21
pop r20
pop r19
pop r18
pop r17
pop r16
ret
ProgramTest_abcdef_1:
push r16
push r17
push r18
push r19
push r20
push r21
push r28
push r29
in r28,__SP_L__
in r29,__SP_H__
adiw r28, 6
in __tmp_reg__,__SREG__
cli
out __SP_H__,r29
out __SREG__,__tmp_reg__
out __SP_L__,r28
pop r29
pop r28
pop r21
pop r20
pop r19
pop r18
pop r17
pop r16
ret
Bool_Achtung_1:
push r16
push r17
push r18
push r19
push r20
push r21
push r28
push r29
in r28,__SP_L__
in r29,__SP_H__
adiw r28, 6
in __tmp_reg__,__SREG__
cli
out __SP_H__,r29
out __SREG__,__tmp_reg__
out __SP_L__,r28
pop r29
pop r28
pop r21
pop r20
pop r19
pop r18
pop r17
pop r16
ret
Bool_new_0:
push r16
push r17
push r18
push r19
push r20
push r21
push r28
push r29
in r28,__SP_L__
in r29,__SP_H__
adiw r28, 6
in __tmp_reg__,__SREG__
cli
out __SP_H__,r29
out __SREG__,__tmp_reg__
out __SP_L__,r28
pop r29
pop r28
pop r21
pop r20
pop r19
pop r18
pop r17
pop r16
ret
Int_KleinAls_2:
push r16
push r17
push r18
push r19
push r20
push r21
push r28
push r29
in r28,__SP_L__
in r29,__SP_H__
adiw r28, 6
in __tmp_reg__,__SREG__
cli
out __SP_H__,r29
out __SREG__,__tmp_reg__
out __SP_L__,r28
pop r29
pop r28
pop r21
pop r20
pop r19
pop r18
pop r17
pop r16
ret
Int_Equal_2:
push r16
push r17
push r18
push r19
push r20
push r21
push r28
push r29
in r28,__SP_L__
in r29,__SP_H__
adiw r28, 6
in __tmp_reg__,__SREG__
cli
out __SP_H__,r29
out __SREG__,__tmp_reg__
out __SP_L__,r28
pop r29
pop r28
pop r21
pop r20
pop r19
pop r18
pop r17
pop r16
ret
Int_Subtraktion_2:
push r16
push r17
push r18
push r19
push r20
push r21
push r28
push r29
in r28,__SP_L__
in r29,__SP_H__


        // Lade a ins register
        ldd r18,Y+4 ; // lade a ins register
        ldd r19,Y+5 ; // lade a ins register

        // Lade b ins register
        ldd r24,Y+6 ; // lade b ins register
        ldd r25,Y+7 ; // lade b ins register

        // addiere a und b
        sub r24,r18 ; // addiere a und b
        sbc r25,r19 ; // addiere a und b

        //speichere in result
        std Y+8,r24 ; // lege in result ab
        std Y+9,r25 ; // lege in result ab

        
adiw r28, 6
in __tmp_reg__,__SREG__
cli
out __SP_H__,r29
out __SREG__,__tmp_reg__
out __SP_L__,r28
pop r29
pop r28
pop r21
pop r20
pop r19
pop r18
pop r17
pop r16
ret
Int_Incrementation_1:
push r16
push r17
push r18
push r19
push r20
push r21
push r28
push r29
in r28,__SP_L__
in r29,__SP_H__


        // Lade a ins register
        ldd r18,Y+1 ; // lade a ins register
        ldd r19,Y+2 ; // lade a ins register

        // increment a
        inc r18 ; // increment
        adc r19, r1 ; // increment

        //speichere in a zurück
        std Y+1,r18 ; // lege in a ab
        std Y+2,r19 ; // lege in a ab

        
adiw r28, 6
in __tmp_reg__,__SREG__
cli
out __SP_H__,r29
out __SREG__,__tmp_reg__
out __SP_L__,r28
pop r29
pop r28
pop r21
pop r20
pop r19
pop r18
pop r17
pop r16
ret
Int_Addition_2:
push r16
push r17
push r18
push r19
push r20
push r21
push r28
push r29
in r28,__SP_L__
in r29,__SP_H__


        // Lade a ins register
        ldd r18,Y+1 ; // lade a ins register
        ldd r19,Y+2 ; // lade a ins register

        // Lade b ins register
        ldd r24,Y+3 ; // lade b ins register
        ldd r25,Y+4 ; // lade b ins register

        // addiere a und b
        add r24,r18 ; // addiere a und b
        adc r25,r19 ; // addiere a und b

        //speichere in result
        std Y+5,r24 ; // lege in result ab
        std Y+6,r25 ; // lege in result ab

        
adiw r28, 6
in __tmp_reg__,__SREG__
cli
out __SP_H__,r29
out __SREG__,__tmp_reg__
out __SP_L__,r28
pop r29
pop r28
pop r21
pop r20
pop r19
pop r18
pop r17
pop r16
ret
Int_new_0:
push r16
push r17
push r18
push r19
push r20
push r21
push r28
push r29
in r28,__SP_L__
in r29,__SP_H__
adiw r28, 6
in __tmp_reg__,__SREG__
cli
out __SP_H__,r29
out __SREG__,__tmp_reg__
out __SP_L__,r28
pop r29
pop r28
pop r21
pop r20
pop r19
pop r18
pop r17
pop r16
ret
