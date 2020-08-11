# Yama Compiler

This is a Compiler writen in C# for the Language Yama

## What is the Language Yama ?

It is a object oriented programming Language, which is direct translate in Assembly.

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

## Getting Started

dotnet build

### Prerequisites

* dotnet
* assembler for your Platfform (maybe in future i write my own assembler)

## Running the tests

*placeholder*

### Break down into end to end tests


## Built With

* dotnet build

## Example


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

        }
    }
}
```

### avr-gcc
 YamaCompile out out.S define atmega328p define avr-gcc def avr inc bin/Debug/netcoreapp3.1/System bin/Debug/netcoreapp3.1/iftest.yama
 avr-gcc -nostartfiles -mmcu=atmega328p -o out.elf out.S

## Authors

* **Robin D'Andrea** - *Robinterra* - [Robinterra](https://github.com/Robinterra)