# Modern Programming Platforms (.NET)
****
## ***1. Tracer*** :white_check_mark:
Measuring execution time of methods. 
Retrieves the following information about the measured method:
- method name;
- name of the class with the measured method;
- execution time of the method.
## ***2. Faker*** :white_check_mark:
Generator of DTO with random test data. Generating must be recursive (if the DTO field is another DTO, then it must also be created using Faker).
Values may be generated using predefined methods for common types or user-defined method for particular DTO field.
## ***3. Assembly Browser*** :white_check_mark:
WPF MVVM-patterned graphical utility to retrieve information about arbitary.NET assembly. The contents of the loaded assembly is presented in a TreeView including
- namespaces;
  - data types;
    - fields, properties and methods (information about methods in addition to the name should include the signature, about fields and properties-the type).
## ***4. Test Generator*** :white_check_mark:
Multithreaded Nunit template code generator for classes. Generating is performed in the producer-consumer pipeline mode. 
Generator takes in account modifiers and dependencies of the tested class, returned value type and arguments types of methods.
## ***5. Dependency Injection Container*** :white_check_mark:
Generalized and configurable object factory. Creating dependencies should be done recursively if necessary. 
Implemented two options for the lifetime of dependencies:
- instance per dependency;
- thread-safe singleton.
Dependency can have generic type or multiple implenentations. Also it is possible to create named dependency and to get it by name.
