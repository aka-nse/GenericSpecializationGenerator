// See https://aka.ms/new-console-template for more information
using GenericSpecializationGenerator.DebugApp;

var sample = new SampleClass();
sample.Foo("hogehoge");
sample.Foo(123);
sample.Bar("hoge");
sample.Bar(123);
sample.Baz(1, 1);
sample.Baz(1, 1.0);
sample.Baz(1.0, 1);
sample.Baz("", 0.1);
