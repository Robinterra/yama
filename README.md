# Yama Compiler

This is a Compiler writen in C# for the Language Yama

## What is the Language Yama ?

It is a Object-oriented Language for Microcontroller like ARM Cortex-M and AVR.
Currently support:
 - AVR Assembler (avrasm and avr-gcc)
 - ARM-T32 Assemlber (testet on STM32F3Discovery)

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
1. Array, LinkListand List in Framework
2. Add AVR Translate from Assembler to Binary
3. How to add a new Assembly this Language - docu
4. Show in a Blog little projects with yama - my own reason.
 * You want support me or you have ideas then please send me a email: robin.dandrea@gmail.com / robin.dandrea@versalitic.com

## What can current use?
 - AVR and ARM Assembler and Runtime Assembler (ARM Current testet) (AVR current not compatible)
 - ARM Assembler and Runtime Assembler can be translate to Binary
 - Debugger for Assembler and binary code (Support Runtime assembly)
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
 - keyword they work = if, else, while, for, continue, break, return, this, base, true, false, null, class, enum, static, namespace, using, is, as, primitive
 - Simple Inheritance (base, this, override methods from the base class, "is" to check in a if statement a class is the right type and cast it to his type).
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
* assembler for your Platfform (for ARM-T32 a Assembler is include)

## Running the tests

*placeholder*

## Built With

* dotnet build

## Example

 - A Example Project with Arduino Uno [Blinking Led](https://github.com/Robinterra/blinkledyama)
 - A Example Project run on STM32F401 [ARM Test](https://github.com/Robinterra/blinkledYamaSTM32.git)

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
        private int globalVariable;

        public int PropertyOne
        {
            get
            {
                return this.globalVariable;
            }
            set
            {
                this.globalVariable = invalue;
            }
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

        public this ~()
        {

        }

    }
}

```
### Project Config File
Create a `config.yproj` in your Project root folder.
````
Target:"arm-t32"
Skip:0x20000000
StartNamespace:"Program"
Optimize:2

Out:"bin/outYama.bin"
IROut:"bin/out.ir"
AsmOut:"bin/out.S"

Define:"stm32f4"
Source:"src/"

package: {
    git.repository:"https://github.com/Robinterra/armlibrary.git"
    git.branch:"master"
}

package: {
    git.repository:"https://github.com/Robinterra/ps2interface.git"
    git.branch:"master"
}
````

### Compiler Arguments
```
Compiler for Yama, a Object-oriented Language for Microcontroller like ARM Cortex-M and AVR
Examples:
yama build skip 0x08000000 out ./bin/out.bin define STM32F401 def arm-t32 inc ./src/
yama build out ./out.bin def runtime ./test.yama

build               Build a Yama Programm
    file <file>         One file which to Compile
    include <folder>    Include all files from this folder (recursive). Only classes which namespace is using will be compile
    asmoutput <file>    The output Filename (Default:No assembler output)
    output <file>       The output Filename (Default:out.bin) Shortcut:out
    optimize <level>    Configuration of Code Opitmizen (None, Level1, SSA (Default))
    definition <name>   Set the Compiler definition for translate in assembler
    define <define>     One Define for conditional compilation
    print <subcommand>  Gibt <subcommand> (tree) in der Console aus
    skip <value>        The Skip value at top from binary code (Hex Format)
    start <namespace>   The start namespace that is to compile (default:Program)
    irout <file>        The output file of the IR Code
    extension <file>    A Directory with .json Extensions for the Compiler definition

assemble            Assemble a Assembler file to Binary
    size <value>        The Size of the Memory (Hex Format)
    file <file>         One file which to Compile
    definition <name>   Set the Compiler definition for translate in assembler
    output <file>       The output Filename (Default:out.bin) Shortcut:out
    skip <value>        The Skip value at top from binary code (Hex Format)

run                 Run/Debug a Binary File
    size <value>        The Size of the Memory (Hex Format)
    file <file>         One file which to Compile

debug               Debug a Yama Source File
    size <value>        The Size of the Memory (Hex Format)
    file <file>         One file which to Compile
```

### avr-gcc
```console
./YamaCompile build out "./bin/out.S" define atmega328p define avr-gcc def avr inc "./src"

avr-gcc -nostartfiles -mmcu=atmega328p -o ./bin/out.elf ./bin/out.S

avr-objcopy -j .text -j .data -O ihex ./bin/out.elf ./bin/out.hex

avrdude -F -e -v -p m328p -c arduino -P /dev/ttyACM0 -b 115200 -U flash:w:"out.hex":i
```
### runtime
#### Translate To Binary
```console
./YamaCompiler build ao temp.S out "./out.bin" def runtime ./test.yama
```
#### Debug
```console
./YamaCompiler run out.bin
./YamaCompiler debug out "./out.bin" def runtime ./temp.S
```
### arm-gcc
* STM32F401 0x0800 0000
* SAM3X8E 0x0008 0000

#### Translate to Binary
```console
./YamaCompiler build skip 0x80000 out "./out.bin" define SAM3X8E def arm-t32 ./iftest.yama
```
#### Translate to Binary and Assembler
```console
./YamaCompiler build skip 0x80000 ao "out.S" out "./out.bin" define SAM3X8E def arm-t32 ./iftest.yama
```
## Authors

* **Robin D'Andrea** - *Robinterra* - [Robinterra](https://github.com/Robinterra)