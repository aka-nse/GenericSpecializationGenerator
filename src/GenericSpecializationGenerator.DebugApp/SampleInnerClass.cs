using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericSpecialization;

namespace GenericSpecializationGenerator.DebugApp.SampleInner;

public partial class ContainerClass
{
    public partial class Nested
    {
        [PrimaryGeneric(nameof(Add_default))]
        public static partial T1 Add<T1, T2>(T1 x, T2 y);
        public static T1 Add_default<T1, T2>(T1 x, T2 y)
            => throw new NotSupportedException();

        private static int Add(int x, int y)
            => x + y;

        private static double Add(double x, double y)
            => x + y;

        private static double Add(double x, int y)
            => x + y;

        private static int Add(int x, double y)
            => (int)(x + y);
    }
}

public partial class ContainerClass<T3, T4, T5>
{
    public partial class Nested
    {
        [PrimaryGeneric(nameof(Add_default))]
        public static partial T1 Add<T1, T2>(T1 x, T2 y);
        public static T1 Add_default<T1, T2>(T1 x, T2 y)
            => throw new NotSupportedException();

        private static int Add(int x, int y)
            => x + y;

        private static double Add(double x, double y)
            => x + y;

        private static double Add(double x, int y)
            => x + y;

        private static int Add(int x, double y)
            => (int)(x + y);
    }
}

public partial interface IContainerInterface
{
    public partial class Nested
    {
        [PrimaryGeneric(nameof(Add_default))]
        public static partial T1 Add<T1, T2>(T1 x, T2 y);
        public static T1 Add_default<T1, T2>(T1 x, T2 y)
            => throw new NotSupportedException();

        private static int Add(int x, int y)
            => x + y;

        private static double Add(double x, double y)
            => x + y;

        private static double Add(double x, int y)
            => x + y;

        private static int Add(int x, double y)
            =>(int)(x + y);
    }
}

public partial interface IContainerInterface<T3, T4, T5>
{
    public partial class Nested
    {
        [PrimaryGeneric(nameof(Add_default))]
        public static partial T1 Add<T1, T2>(T1 x, T2 y);
        public static T1 Add_default<T1, T2>(T1 x, T2 y)
            => throw new NotSupportedException();

        private static int Add(int x, int y)
            => x + y;

        private static double Add(double x, double y)
            => x + y;

        private static double Add(double x, int y)
            => x + y;

        private static int Add(int x, double y)
            => (int)(x + y);
    }
}

public partial struct ContainerStruct
{
    public partial class Nested
    {
        [PrimaryGeneric(nameof(Add_default))]
        public static partial T1 Add<T1, T2>(T1 x, T2 y);
        public static T1 Add_default<T1, T2>(T1 x, T2 y)
            => throw new NotSupportedException();

        private static int Add(int x, int y)
            => x + y;

        private static double Add(double x, double y)
            => x + y;

        private static double Add(double x, int y)
            => x + y;

        private static int Add(int x, double y)
            => (int)(x + y);
    }
}

public partial struct ContainerStruct<T3, T4, T5>
{
    public partial class Nested
    {
        [PrimaryGeneric(nameof(Add_default))]
        public static partial T1 Add<T1, T2>(T1 x, T2 y);
        public static T1 Add_default<T1, T2>(T1 x, T2 y)
            => throw new NotSupportedException();

        private static int Add(int x, int y)
            => x + y;

        private static double Add(double x, double y)
            => x + y;

        private static double Add(double x, int y)
            => x + y;

        private static int Add(int x, double y)
            => (int)(x + y);
    }
}
