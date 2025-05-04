using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericSpecialization;

namespace GenericSpecializationGenerator.DebugApp;

internal partial class SampleInnerClass
{
    public partial interface IContainer2
    {
        public partial struct Container3
        {
            public partial class Container4<T1, T2>
            {
                [PrimaryGeneric(nameof(Add_default))]
                public partial T3 Add<T3, T4>(T3 x, T4 y);

                private T3 Add_default<T3, T4>(T3 x, T4 y)
                    => throw new NotSupportedException();

                private static int Add(int x, int y)
                    => x + y;

                private static double Add(double x, double y)
                    => x + y;

                private static string Add(string x, string y)
                    => x + y;

                private static string Add(string x, int y)
                    => $"{x}{y}";

                private static string Add(string x, double y)
                    => $"{x}{y}";
            }
        }
    }
}
