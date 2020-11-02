# Yama Compiler

This is a Compiler writen in C# for the Language Yama

## What is the Language Yama ?

It is a Object-oriented Language for Microcontroller like ARM Cortex-M and AVR.
Currently support:
 - AVR Assembler (avrasm and avr-gcc)
 - ARM-T32 Assemlber (arm-gcc) (current not testet)

### Why this new Language, Why dont use C#, Java or C?

I want to programming a Microcontroller so C# and Java can not use for this.
Yes i can use C, but it is not a OOP Language and i am not happy with this.
I want to programming with objects, inheritance ... etc.
My first reason with this project was to learn how to lexer Text and Build a ParseTree, but now it is little more.

## Where does the name Yama come from?

Its come from Warhammer 40000.
It was a memory from Vargard Obyron and is it a memory from Zanndrekh.
You can read it in the book Severed.

## Documentation

*comming soon*

## What is comming soon ?
chronological order
1. Better Properties - (Get/Set can current not use, make it usable)
2. Array, LinkListand List in Framework
3. How to add a new Assembly this Language - docu
4. Show in a Blog little projects with yama - my own reason.
 * You want support me or you have ideas then please send me a email: robin.dandrea@gmail.com / robin.dandrea@versalitic.com

## What can current use?
 - AVR and ARM Assembler, (ARM Current not testet)
 - static and non static Methods
 - Array Get Set Methods and Call
 - Enums
 - Properties (no static Properties aviable)
 - Classes and Object instances
 - Delegate Methods (not tested)
 - Namespaces and usings
 - Conditional Compilation
  1. region and endregion
  2. `#defalgo <name>,<mode>:` read from a json file a assembly algo and replace it with des assemlby commands
 - keyword they work = if, else, while, for, continue, break, return, this, base, true, false, null, class, enum, static, namespace, using
 - Simple Inheritance (base, this, override methods from the base class).
  1. Possible is this case: `class A` `class B : A` `class C : A` `class D : B`
  2. Not Possible is this case: `class A` `class B` `class C : A, B`
  3. Not Possible is this case: `class A : B` `class B : A`

## Getting Started

dotnet build

### Install
I use a link to YamaCompiler and use it
```
sudo ln -s /mnt/c/pro/learncs/bin/Release/netcoreapp3.1/YamaCompiler /usr/bin/yama
```

### Prerequisites

* dotnet
* assembler for your Platfform (maybe in future i write my own assembler)

## Running the tests

*placeholder*

## Built With

* dotnet build

## Example

A Example Project with Arduino Uno [Blinking Led](https://github.com/Robinterra/blinkledyama)

### A Yama Code snippet:
 - The using "System" is for the types: int and bool.
 - The using "System.IO" is only needed when you use ctor or dector, a ctor call automaticly malloc and reserved memory space.
 - When you whish to destory a object, then implement a empty dector and call it like `~YourInstanceVariabelName;` the dector call autmatlicy mallocFree.
 - Is a Semicolon needed? no

```csharp
namespace "Program"
{
    using "System";
    using "System.IO";

    public class MyStartClass
    {
        public int PropertyOne
        {
            get{}
            set{}
        }

        public static int main()
        {
            MyStartClass classTest = new MyStartClass();

            classTest.PropertyOne = 5;

            if (classTest.PropertyOne < 10)
            {
                classTest.PropertyOne = classTest.PropertyOne + 1;
            }

            return classTest.PropertyOne;
        }

        public this new()
        {
            return this;
        }
    }
}
```

### avr-gcc
```console
./YamaCompile build out "./bin/out.S" define atmega328p define avr-gcc def avr inc "./src"

avr-gcc -nostartfiles -mmcu=atmega328p -o ./bin/out.elf ./bin/out.S

avr-objcopy -j .text -j .data -O ihex ./bin/out.elf ./bin/out.hex

avrdude -F -e -v -p m328p -c arduino -P /dev/ttyACM0 -b 115200 -U flash:w:"out.hex":i
```
### arm-gcc
```console
dotnet run build out "./out.S" define SAM3X8E def arm-t32 ./bin/Debug/netcoreapp3.1/iftest.yama

arm-none-eabi-gcc -nostartfiles -nostdlib -mcpu=cortex-m3 -Ttext=0x80000 -o ./bin/out.elf out.S
```

## Authors

* **Robin D'Andrea** - *Robinterra* - [Robinterra](https://github.com/Robinterra)