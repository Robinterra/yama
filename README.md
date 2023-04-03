# Yama Compiler
This is a compiler written in C# for the Yama language.

## What is the Yama language?

It is an object-oriented language for microcontrollers such as ARM Cortex-M and Cortex-A. Currently, it supports:
 - ARM-T32 Assemlber (testet on STM32F3Discovery)
 - ARM-A32 Linux like Raspbian (Testet on Raspberry 2 and Raspberry 3)

### Why this new language? Why not use C#, Java, or C?
Because of fun.

## Documentation
*comming soon*  😅

## What is comming soon ?
chronological order
1. Networkstreaming on ARM Linux
2. Web API Server Framework

## What can current use?
 - AVR and ARM Assembler and Runtime Assembler (ARM Current testet)
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

## What are the current features of Yama language?

- It supports AVR and ARM Assembler as well as Runtime Assembler (ARM has been tested).
- ARM Assembler and Runtime Assembler can be translated to binary.
- It has a Debugger for Assembler and binary code, which supports Runtime assembly.
- It supports both static and non-static methods.
- It has Array Get Set methods and call, Enums, and Properties (non-static Properties are available).
- It also supports Classes and Object instances (Heap), Structs (Stack), enums, Delegate methods.
- It has Conditional Compilation with regions and endregion, and `#defalgo <name>,<mode>:` which reads from a JSON file an assembly algo and replaces it with its assembly commands.
- Its keywords include if, else, while, for, continue, break, return, this, base, true, false, null, class, enum, static, namespace, using, is, as, and primitive.
- It has Simple Inheritance which supports base, this, override methods from the base class, "is" to check in an if statement whether a class is the right type and cast it to its type.
- It is possible to have a class hierarchy like this: `class A`, `class B : A`, `class C : A`, `class D : B`.
- However, it is not possible to have a class hierarchy like this: `class A`, `class B`, `class C : A, B` or like this: `class A : B`, `class B : A`.

## Getting Started

dotnet build

### Install
I use a link to YamaCompiler and use it
```
sudo ln -s /mnt/c/pro/learncs/bin/Release/netcoreapp3.1/YamaCompiler /usr/bin/yama
```

### Prerequisites
* dotnet

## Running the tests
*placeholder*

## Example
The examples are outdated, new ones will follow soon.
 - A Example Project with Arduino Uno [Blinking Led](https://github.com/Robinterra/blinkledyama)
 - A Example Project run on STM32F401 [ARM Test](https://github.com/Robinterra/blinkledYamaSTM32.git)

### A Yama Code snippet:
 - The usage of "System" and "System.IO" is important for writing programs.
 - Note that the destructor is automatically called when an object is no longer in use and is about to be destroyed. Therefore, it is important to implement an empty destructor for proper memory management. To call the destructor, simply use the syntax `YourInstanceVariableName = ~YourInstanceVariableName;`. This will ensure that the destructor is called and the memory allocated by the object is freed.

```csharp
namespace "Program"
{
    using "System";
    using "System.IO";
    using "System.Runtime";

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
                this.globalVariable = value;
            }
        }

        public static int main()
        {
            Console.PrintLine("Hello World!");

            MyStartClass classTest = new MyStartClass();

            classTest.PropertyOne = 5;

            if (classTest.PropertyOne < 10)
            {
                classTest.PropertyOne = classTest.PropertyOne + 1;

                int outputInt = classTest.PropertyOne;
                String outputText = outputInt.ToString();

                Console.Print("The Number is: 0x");
                Console.PrintLine(outputText.Content);
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
Create a `config.yproj` in your Project root folder. Its recommed to use a project config file.
When you use a config file then you have only to enter `yama build`.
````
//comment this line if a Raspberry Pi is available:
Target:"runtime"

//and include the following three lines if a Raspberry Pi is available:
//Target:"arm-a32-linux"
//Skip:0x10000
//OSHeader:"Linux"

StartNamespace:"Program"
Optimize:2

Out:"bin/out.yexe"
IROut:"bin/out.ir" //this line is optional
AsmOut:"bin/out.S" //this line is optional

Source:"src/"

ReflectionActive:true

package: {
    git.repository:"https://github.com/Robinterra/jsonyama.git"
    git.branch:"master"
}

// following lines are usefull for the gpio on STM32 (bare metal) or on a Raspberry Pi with Raspbian
/*package: {
    git.repository:"https://github.com/Robinterra/armlibrary.git"
    git.branch:"master"
}

package: {
    git.repository:"https://github.com/Robinterra/ps2interface.git"
    git.branch:"master"
}*/
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
    print <subcommand>  Zum Einstellen des Consolen Output: Um den ParserTree auszugeben 'tree', um die Parsetime für einzelne Dateien auszugeben 'parsetime', um die Zeit der einzlenen Pahsen auszugeben 'phasetime'
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
obsolete
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
./YamaCompiler build
./YamaCompiler debug def runtime ./bin/out.S
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
