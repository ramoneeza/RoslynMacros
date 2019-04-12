# RoslynMacros
AOP Alternative to T4. Autocomplete classes with macros

It Is a global Net.Core 2.2 tool that allows to generate automatic C# code according to the configuration of Attributes, Scripts and Macros created in the project.

With Custom attributes You can decorate classes, interfaces, properties, etc. and mark them to run Scripts against them.

The Scripts will select a particular macro (as if it were a template) and generate the necessary code.

*Example*:

-Attribute: AutoImplement --> Decorate a class to implement an Interface.

-Script: AutoImplement --> Traverses the syntactic tree looking for Classes decorated with the attribute "Autoimplement" and
                          executing the appropriate macro.
                          
-Macro: Autoimplement.$interface.csmacro --> This template tells how implement the elements of the interface into the class.
