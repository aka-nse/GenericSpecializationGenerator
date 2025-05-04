// See https://aka.ms/new-console-template for more information
using GenericSpecializationGenerator.DebugApp;
using GenericSpecializationGenerator.DebugApp.SampleInner;

Console.WriteLine(ContainerClass.Nested.Add(42, 27));
Console.WriteLine(ContainerClass<int, int, int>.Nested.Add(42, 27));
Console.WriteLine(IContainerInterface.Nested.Add(42, 27));
Console.WriteLine(IContainerInterface<int, int, int>.Nested.Add(42, 27));
Console.WriteLine(ContainerStruct.Nested.Add(42, 27));
Console.WriteLine(ContainerStruct<int, int, int>.Nested.Add(42, 27));