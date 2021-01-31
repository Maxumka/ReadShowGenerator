# ReadShowGenerator
## What is this?
This is a source code generator that generates methods to convert a class instance into a string and a string into a class instance. 
For example, there is some class with several properties:
```CSharp    
public sealed class SomeClass
{
    public string SomePropertyOne { get; set; }

    public string SomePropertyTwo { get; set; }
}
```
If you add attribute Show in SomeClass. The generator will create extension method, which convert instance SomeClass to string.
```CSharp
var someInstance = new SomeClass { SomePropertyOne = "prop1", SomePropertyTwo = "prop2" };
var someInstanceString = someInstance.Show();
```
Also you can add attribute Read in SomeClass and generator will create static class with static method, which convert string to instance SomeClass.
```CSharp
var someInstanceText = "Text.SomeClass { SomePropertyOne = prop1, SomePropertyTwo = prop2 }";
var someInstance = ReadSomeClass.Read(someInstanceText);
```
## Restrictions for the class 
First, it must be a class, not a record. 
Second, the class must have a default constructor. 
That's all.
## What it looks like in visual studio 
![SourceCode](https://drive.google.com/file/d/1SfcDr0CkFuidqdjMLm0hM1-PoqvMCvRT/view?usp=sharing)
![Result](https://drive.google.com/file/d/1w3Oxuf1i8KwrqmLnYtW9aI7B1WTeNrt5/view?usp=sharing)
