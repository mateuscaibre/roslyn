﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CodeStyle;
using Microsoft.CodeAnalysis.CSharp.CodeStyle;
using Microsoft.CodeAnalysis.CSharp.ExtractLocalFunction;
using Microsoft.CodeAnalysis.CSharp.Test.Utilities;
using Microsoft.CodeAnalysis.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.CodeRefactorings.ExtractMethod
{
    public class ExtractLocalFunctionTests : AbstractCSharpCodeActionTest
    {
        protected override CodeRefactoringProvider CreateCodeRefactoringProvider(Workspace workspace, TestParameters parameters)
            => new CSharpExtractLocalFunctionCodeRefactoringProvider();

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestPartialSelection_StaticOptionTrue()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        bool b = true;
        System.Console.WriteLine([|b != true|] ? b = true : b = false);
    }
}",
@"class Program
{
    static void Main(string[] args)
    {
        bool b = true;
        System.Console.WriteLine({|Rename:NewMethod|}(b) ? b = true : b = false);

        static bool NewMethod(bool b)
        {
            return b != true;
        }
    }
}", options: Option(CSharpCodeStyleOptions.PreferStaticLocalFunction, CodeStyleOptions.TrueWithSilentEnforcement));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestPartialSelection_StaticOptionFalse()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        bool b = true;
        System.Console.WriteLine([|b != true|] ? b = true : b = false);
    }
}",
@"class Program
{
    static void Main(string[] args)
    {
        bool b = true;
        System.Console.WriteLine({|Rename:NewMethod|}(b) ? b = true : b = false);

        bool NewMethod(bool b)
        {
            return b != true;
        }
    }
}", options: Option(CSharpCodeStyleOptions.PreferStaticLocalFunction, CodeStyleOptions.FalseWithSilentEnforcement));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestPartialSelection_StaticOptionDefault()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        bool b = true;
        System.Console.WriteLine([|b != true|] ? b = true : b = false);
    }
}",
@"class Program
{
    static void Main(string[] args)
    {
        bool b = true;
        System.Console.WriteLine({|Rename:NewMethod|}(b) ? b = true : b = false);

        static bool NewMethod(bool b)
        {
            return b != true;
        }
    }
}", options: Option(CSharpCodeStyleOptions.PreferStaticLocalFunction, CSharpCodeStyleOptions.PreferStaticLocalFunction.DefaultValue));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestUseExpressionBodyWhenPossible()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        bool b = true;
        System.Console.WriteLine([|b != true|] ? b = true : b = false);
    }
}",
@"class Program
{
    static void Main(string[] args)
    {
        bool b = true;
        System.Console.WriteLine({|Rename:NewMethod|}(b) ? b = true : b = false);

        static bool NewMethod(bool b) => b != true;
    }
}",
options: Option(CSharpCodeStyleOptions.PreferExpressionBodiedMethods, CSharpCodeStyleOptions.WhenPossibleWithSilentEnforcement));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestUseExpressionWhenOnSingleLine_AndIsOnSingleLine()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        bool b = true;
        System.Console.WriteLine([|b != true|] ? b = true : b = false);
    }
}",
@"class Program
{
    static void Main(string[] args)
    {
        bool b = true;
        System.Console.WriteLine({|Rename:NewMethod|}(b) ? b = true : b = false);

        static bool NewMethod(bool b) => b != true;
    }
}",
options: Option(CSharpCodeStyleOptions.PreferExpressionBodiedMethods, CSharpCodeStyleOptions.WhenOnSingleLineWithSilentEnforcement));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestUseExpressionWhenOnSingleLine_AndIsOnSingleLine2()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        bool b = true;
        System.Console.WriteLine(

            [|b != true|]
                ? b = true : b = false);
    }
}",
@"class Program
{
    static void Main(string[] args)
    {
        bool b = true;
        System.Console.WriteLine(

            {|Rename:NewMethod|}(b)
                ? b = true : b = false);

        static bool NewMethod(bool b) => b != true;
    }
}",
options: Option(CSharpCodeStyleOptions.PreferExpressionBodiedMethods, CSharpCodeStyleOptions.WhenOnSingleLineWithSilentEnforcement));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestUseExpressionWhenOnSingleLine_AndNotIsOnSingleLine()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        bool b = true;
        System.Console.WriteLine([|b != 
            true|] ? b = true : b = false);
    }
}",
@"class Program
{
    static void Main(string[] args)
    {
        bool b = true;
        System.Console.WriteLine({|Rename:NewMethod|}(b) ? b = true : b = false);

        static bool NewMethod(bool b)
        {
            return b !=
                        true;
        }
    }
}",
options: Option(CSharpCodeStyleOptions.PreferExpressionBodiedMethods, CSharpCodeStyleOptions.WhenOnSingleLineWithSilentEnforcement));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestUseExpressionWhenOnSingleLine_AndNotIsOnSingleLine2()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        bool b = true;
        System.Console.WriteLine([|b !=/*
*/true|] ? b = true : b = false);
    }
}",
@"class Program
{
    static void Main(string[] args)
    {
        bool b = true;
        System.Console.WriteLine({|Rename:NewMethod|}(b) ? b = true : b = false);

        static bool NewMethod(bool b)
        {
            return b !=/*
*/true;
        }
    }
}",
options: Option(CSharpCodeStyleOptions.PreferExpressionBodiedMethods, CSharpCodeStyleOptions.WhenOnSingleLineWithSilentEnforcement));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestUseExpressionWhenOnSingleLine_AndNotIsOnSingleLine3()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        bool b = true;
        System.Console.WriteLine([|"""" != @""
""|] ? b = true : b = false);
    }
}",
@"class Program
{
    static void Main(string[] args)
    {
        bool b = true;
        System.Console.WriteLine({|Rename:NewMethod|}() ? b = true : b = false);

        static bool NewMethod()
        {
            return """" != @""
"";
        }
    }
}",
options: Option(CSharpCodeStyleOptions.PreferExpressionBodiedMethods, CSharpCodeStyleOptions.WhenOnSingleLineWithSilentEnforcement));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestReadOfDataThatDoesNotFlowIn()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        int x = 1;
        object y = 0;
        [|int s = true ? fun(x) : fun(y);|]
    }

    private static T fun<T>(T t)
    {
        return t;
    }
}",
@"class Program
{
    static void Main(string[] args)
    {
        int x = 1;
        object y = 0;
        {|Rename:NewMethod|}(x, y);

        static void NewMethod(int x, object y)
        {
            int s = true ? fun(x) : fun(y);
        }
    }

    private static T fun<T>(T t)
    {
        return t;
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestMissingOnGoto()
        {
            await TestMissingInRegularAndScriptAsync(
@"delegate int del(int i);

class C
{
    static void Main(string[] args)
    {
        del q = x => {
            [|goto label2;
            return x * x;|]
        };
    label2:
        return;
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestOnStatementAfterUnconditionalGoto()
        {
            await TestInRegularAndScriptAsync(
@"delegate int del(int i);

class C
{
    static void Main(string[] args)
    {
        del q = x => {
            goto label2;
            [|return x * x;|]
        };
    label2:
        return;
    }
}",
@"delegate int del(int i);

class C
{
    static void Main(string[] args)
    {
        del q = x =>
        {
            goto label2;
            return {|Rename:NewMethod|}(x);
        };
    label2:
        return;

        static int NewMethod(int x)
        {
            return x * x;
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestOnNamespace()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    void Main()
    {
        [|System|].Console.WriteLine(4);
    }
}",
@"class Program
{
    void Main()
    {
        {|Rename:NewMethod|}();

        static void NewMethod()
        {
            System.Console.WriteLine(4);
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestOnType()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    void Main()
    {
        [|System.Console|].WriteLine(4);
    }
}",
@"class Program
{
    void Main()
    {
        {|Rename:NewMethod|}();

        static void NewMethod()
        {
            System.Console.WriteLine(4);
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestOnBase()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    void Main()
    {
        [|base|].ToString();
    }
}",
@"class Program
{
    void Main()
    {
        {|Rename:NewMethod|}();

        void NewMethod()
        {
            base.ToString();
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestOnActionInvocation()
        {
            await TestInRegularAndScriptAsync(
@"using System;

class C
{
    public static Action X { get; set; }
}

class Program
{
    void Main()
    {
        [|C.X|]();
    }
}",
@"using System;

class C
{
    public static Action X { get; set; }
}

class Program
{
    void Main()
    {
        {|Rename:GetX|}()();

        static Action GetX()
        {
            return C.X;
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task DisambiguateCallSiteIfNecessary1()
        {
            await TestInRegularAndScriptAsync(
@"using System;

class Program
{
    static void Main()
    {
        byte z = 0;
        Goo([|x => 0|], y => 0, z, z);
    }

    static void Goo<T, S>(Func<S, T> p, Func<T, S> q, T r, S s) { Console.WriteLine(1); }
    static void Goo(Func<byte, byte> p, Func<byte, byte> q, int r, int s) { Console.WriteLine(2); }
}",

@"using System;

class Program
{
    static void Main()
    {
        byte z = 0;
        Goo({|Rename:NewMethod|}(), y => (byte)0, z, z);

        static Func<byte, byte> NewMethod()
        {
            return x => 0;
        }
    }

    static void Goo<T, S>(Func<S, T> p, Func<T, S> q, T r, S s) { Console.WriteLine(1); }
    static void Goo(Func<byte, byte> p, Func<byte, byte> q, int r, int s) { Console.WriteLine(2); }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task DisambiguateCallSiteIfNecessary2()
        {
            await TestInRegularAndScriptAsync(
@"using System;

class Program
{
    static void Main()
    {
        byte z = 0;
        Goo([|x => 0|], y => { return 0; }, z, z);
    }

    static void Goo<T, S>(Func<S, T> p, Func<T, S> q, T r, S s) { Console.WriteLine(1); }
    static void Goo(Func<byte, byte> p, Func<byte, byte> q, int r, int s) { Console.WriteLine(2); }
}",

@"using System;

class Program
{
    static void Main()
    {
        byte z = 0;
        Goo({|Rename:NewMethod|}(), y => { return (byte)0; }, z, z);

        static Func<byte, byte> NewMethod()
        {
            return x => 0;
        }
    }

    static void Goo<T, S>(Func<S, T> p, Func<T, S> q, T r, S s) { Console.WriteLine(1); }
    static void Goo(Func<byte, byte> p, Func<byte, byte> q, int r, int s) { Console.WriteLine(2); }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task DontOverparenthesize()
        {
            await TestAsync(
@"using System;

static class C
{
    static void Ex(this string x)
    {
    }

    static void Inner(Action<string> x, string y)
    {
    }

    static void Inner(Action<string> x, int y)
    {
    }

    static void Inner(Action<int> x, int y)
    {
    }

    static void Outer(Action<string> x, object y)
    {
        Console.WriteLine(1);
    }

    static void Outer(Action<int> x, int y)
    {
        Console.WriteLine(2);
    }

    static void Main()
    {
        Outer(y => Inner(x => [|x|].Ex(), y), - -1);
    }
}

static class E
{
    public static void Ex(this int x)
    {
    }
}",

@"using System;

static class C
{
    static void Ex(this string x)
    {
    }

    static void Inner(Action<string> x, string y)
    {
    }

    static void Inner(Action<string> x, int y)
    {
    }

    static void Inner(Action<int> x, int y)
    {
    }

    static void Outer(Action<string> x, object y)
    {
        Console.WriteLine(1);
    }

    static void Outer(Action<int> x, int y)
    {
        Console.WriteLine(2);
    }

    static void Main()
    {
        Outer(y => Inner(x => {|Rename:GetX|}(x).Ex(), y), (object)- -1);

        static string GetX(string x)
        {
            return x;
        }
    }
}

static class E
{
    public static void Ex(this int x)
    {
    }
}",

parseOptions: Options.Regular);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task DontOverparenthesizeGenerics()
        {
            await TestAsync(
@"using System;

static class C
{
    static void Ex<T>(this string x)
    {
    }

    static void Inner(Action<string> x, string y)
    {
    }

    static void Inner(Action<string> x, int y)
    {
    }

    static void Inner(Action<int> x, int y)
    {
    }

    static void Outer(Action<string> x, object y)
    {
        Console.WriteLine(1);
    }

    static void Outer(Action<int> x, int y)
    {
        Console.WriteLine(2);
    }

    static void Main()
    {
        Outer(y => Inner(x => [|x|].Ex<int>(), y), - -1);
    }
}

static class E
{
    public static void Ex<T>(this int x)
    {
    }
}",

@"using System;

static class C
{
    static void Ex<T>(this string x)
    {
    }

    static void Inner(Action<string> x, string y)
    {
    }

    static void Inner(Action<string> x, int y)
    {
    }

    static void Inner(Action<int> x, int y)
    {
    }

    static void Outer(Action<string> x, object y)
    {
        Console.WriteLine(1);
    }

    static void Outer(Action<int> x, int y)
    {
        Console.WriteLine(2);
    }

    static void Main()
    {
        Outer(y => Inner(x => {|Rename:GetX|}(x).Ex<int>(), y), (object)- -1);

        static string GetX(string x)
        {
            return x;
        }
    }
}

static class E
{
    public static void Ex<T>(this int x)
    {
    }
}",

parseOptions: Options.Regular);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task PreserveCommentsBeforeDeclaration_1()
        {
            await TestInRegularAndScriptAsync(
@"class Construct
{
    public void Do() { }
    static void Main(string[] args)
    {
        [|Construct obj1 = new Construct();
        obj1.Do();
        /* Interesting comment. */
        Construct obj2 = new Construct();
        obj2.Do();|]
        obj1.Do();
        obj2.Do();
    }
}",

@"class Construct
{
    public void Do() { }
    static void Main(string[] args)
    {
        Construct obj1, obj2;
        {|Rename:NewMethod|}(out obj1, out obj2);
        obj1.Do();
        obj2.Do();

        static void NewMethod(out Construct obj1, out Construct obj2)
        {
            obj1 = new Construct();
            obj1.Do();
            /* Interesting comment. */
            obj2 = new Construct();
            obj2.Do();
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task PreserveCommentsBeforeDeclaration_2()
        {
            await TestInRegularAndScriptAsync(
@"class Construct
{
    public void Do() { }
    static void Main(string[] args)
    {
        [|Construct obj1 = new Construct();
        obj1.Do();
        /* Interesting comment. */
        Construct obj2 = new Construct();
        obj2.Do();
        /* Second Interesting comment. */
        Construct obj3 = new Construct();
        obj3.Do();|]
        obj1.Do();
        obj2.Do();
        obj3.Do();
    }
}",

@"class Construct
{
    public void Do() { }
    static void Main(string[] args)
    {
        Construct obj1, obj2, obj3;
        {|Rename:NewMethod|}(out obj1, out obj2, out obj3);
        obj1.Do();
        obj2.Do();
        obj3.Do();

        static void NewMethod(out Construct obj1, out Construct obj2, out Construct obj3)
        {
            obj1 = new Construct();
            obj1.Do();
            /* Interesting comment. */
            obj2 = new Construct();
            obj2.Do();
            /* Second Interesting comment. */
            obj3 = new Construct();
            obj3.Do();
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task PreserveCommentsBeforeDeclaration_3()
        {
            await TestInRegularAndScriptAsync(
@"class Construct
{
    public void Do() { }
    static void Main(string[] args)
    {
        [|Construct obj1 = new Construct();
        obj1.Do();
        /* Interesting comment. */
        Construct obj2 = new Construct(), obj3 = new Construct();
        obj2.Do();
        obj3.Do();|]
        obj1.Do();
        obj2.Do();
        obj3.Do();
    }
}",

@"class Construct
{
    public void Do() { }
    static void Main(string[] args)
    {
        Construct obj1, obj2, obj3;
        {|Rename:NewMethod|}(out obj1, out obj2, out obj3);
        obj1.Do();
        obj2.Do();
        obj3.Do();

        static void NewMethod(out Construct obj1, out Construct obj2, out Construct obj3)
        {
            obj1 = new Construct();
            obj1.Do();
            /* Interesting comment. */
            obj2 = new Construct();
            obj3 = new Construct();
            obj2.Do();
            obj3.Do();
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction), CompilerTrait(CompilerFeature.Tuples)]
        public async Task TestTuple()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        [|(int, int) x = (1, 2);|]
        System.Console.WriteLine(x.Item1);
    }
}" + TestResources.NetFX.ValueTuple.tuplelib_cs,
@"class Program
{
    static void Main(string[] args)
    {
        (int, int) x = {|Rename:NewMethod|}();
        System.Console.WriteLine(x.Item1);

        static (int, int) NewMethod()
        {
            return (1, 2);
        }
    }
}" + TestResources.NetFX.ValueTuple.tuplelib_cs);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction), Test.Utilities.CompilerTrait(Test.Utilities.CompilerFeature.Tuples)]
        public async Task TestTupleDeclarationWithNames()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        [|(int a, int b) x = (1, 2);|]
        System.Console.WriteLine(x.a);
    }
}" + TestResources.NetFX.ValueTuple.tuplelib_cs,
@"class Program
{
    static void Main(string[] args)
    {
        (int a, int b) x = {|Rename:NewMethod|}();
        System.Console.WriteLine(x.a);

        static (int a, int b) NewMethod()
        {
            return (1, 2);
        }
    }
}" + TestResources.NetFX.ValueTuple.tuplelib_cs);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction), Test.Utilities.CompilerTrait(Test.Utilities.CompilerFeature.Tuples)]
        public async Task TestTupleDeclarationWithSomeNames()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        [|(int a, int) x = (1, 2);|]
        System.Console.WriteLine(x.a);
    }
}" + TestResources.NetFX.ValueTuple.tuplelib_cs,
@"class Program
{
    static void Main(string[] args)
    {
        (int a, int) x = {|Rename:NewMethod|}();
        System.Console.WriteLine(x.a);

        static (int a, int) NewMethod()
        {
            return (1, 2);
        }
    }
}" + TestResources.NetFX.ValueTuple.tuplelib_cs);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction), Test.Utilities.CompilerTrait(Test.Utilities.CompilerFeature.Tuples)]
        public async Task TestTupleWith1Arity()
        {
            await TestInRegularAndScriptAsync(
@"using System;
class Program
{
    static void Main(string[] args)
    {
        ValueTuple<int> y = ValueTuple.Create(1);
        [|y.Item1.ToString();|]
    }
}" + TestResources.NetFX.ValueTuple.tuplelib_cs,
@"using System;
class Program
{
    static void Main(string[] args)
    {
        ValueTuple<int> y = ValueTuple.Create(1);
        {|Rename:NewMethod|}(y);

        static void NewMethod(ValueTuple<int> y)
        {
            y.Item1.ToString();
        }
    }
}" + TestResources.NetFX.ValueTuple.tuplelib_cs);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction), Test.Utilities.CompilerTrait(Test.Utilities.CompilerFeature.Tuples)]
        public async Task TestTupleLiteralWithNames()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        [|(int, int) x = (a: 1, b: 2);|]
        System.Console.WriteLine(x.Item1);
    }
}" + TestResources.NetFX.ValueTuple.tuplelib_cs,
@"class Program
{
    static void Main(string[] args)
    {
        (int, int) x = {|Rename:NewMethod|}();
        System.Console.WriteLine(x.Item1);

        static (int, int) NewMethod()
        {
            return (a: 1, b: 2);
        }
    }
}" + TestResources.NetFX.ValueTuple.tuplelib_cs);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction), Test.Utilities.CompilerTrait(Test.Utilities.CompilerFeature.Tuples)]
        public async Task TestTupleDeclarationAndLiteralWithNames()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        [|(int a, int b) x = (c: 1, d: 2);|]
        System.Console.WriteLine(x.a);
    }
}" + TestResources.NetFX.ValueTuple.tuplelib_cs,
@"class Program
{
    static void Main(string[] args)
    {
        (int a, int b) x = {|Rename:NewMethod|}();
        System.Console.WriteLine(x.a);

        static (int a, int b) NewMethod()
        {
            return (c: 1, d: 2);
        }
    }
}" + TestResources.NetFX.ValueTuple.tuplelib_cs);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction), Test.Utilities.CompilerTrait(Test.Utilities.CompilerFeature.Tuples)]
        public async Task TestTupleIntoVar()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        [|var x = (c: 1, d: 2);|]
        System.Console.WriteLine(x.c);
    }
}" + TestResources.NetFX.ValueTuple.tuplelib_cs,
@"class Program
{
    static void Main(string[] args)
    {
        (int c, int d) x = {|Rename:NewMethod|}();
        System.Console.WriteLine(x.c);

        static (int c, int d) NewMethod()
        {
            return (c: 1, d: 2);
        }
    }
}" + TestResources.NetFX.ValueTuple.tuplelib_cs);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction), Test.Utilities.CompilerTrait(Test.Utilities.CompilerFeature.Tuples)]
        public async Task RefactorWithoutSystemValueTuple()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        [|var x = (c: 1, d: 2);|]
        System.Console.WriteLine(x.c);
    }
}",
@"class Program
{
    static void Main(string[] args)
    {
        (int c, int d) x = {|Rename:NewMethod|}();
        System.Console.WriteLine(x.c);

        static (int c, int d) NewMethod()
        {
            return (c: 1, d: 2);
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction), Test.Utilities.CompilerTrait(Test.Utilities.CompilerFeature.Tuples)]
        public async Task TestTupleWithNestedNamedTuple()
        {
            // This is not the best refactoring, but this is an edge case
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        [|var x = new System.ValueTuple<int, int, int, int, int, int, int, (string a, string b)>(1, 2, 3, 4, 5, 6, 7, (a: ""hello"", b: ""world""));|]
        System.Console.WriteLine(x.c);
    }
}" + TestResources.NetFX.ValueTuple.tuplelib_cs,
@"class Program
{
    static void Main(string[] args)
    {
        (int, int, int, int, int, int, int, string, string) x = {|Rename:NewMethod|}();
        System.Console.WriteLine(x.c);

        static (int, int, int, int, int, int, int, string, string) NewMethod()
        {
            return new System.ValueTuple<int, int, int, int, int, int, int, (string a, string b)>(1, 2, 3, 4, 5, 6, 7, (a: ""hello"", b: ""world""));
        }
    }
}" + TestResources.NetFX.ValueTuple.tuplelib_cs);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction), Test.Utilities.CompilerTrait(Test.Utilities.CompilerFeature.Tuples)]
        public async Task TestDeconstruction()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        var (x, y) = [|(1, 2)|];
        System.Console.WriteLine(x);
    }
}" + TestResources.NetFX.ValueTuple.tuplelib_cs,
@"class Program
{
    static void Main(string[] args)
    {
        var (x, y) = {|Rename:NewMethod|}();
        System.Console.WriteLine(x);

        static (int, int) NewMethod()
        {
            return (1, 2);
        }
    }
}" + TestResources.NetFX.ValueTuple.tuplelib_cs);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction), Test.Utilities.CompilerTrait(Test.Utilities.CompilerFeature.Tuples)]
        public async Task TestDeconstruction2()
        {
            await TestInRegularAndScriptAsync(
@"class Program
{
    static void Main(string[] args)
    {
        var (x, y) = (1, 2);
        var z = [|3;|]
        System.Console.WriteLine(z);
    }
}" + TestResources.NetFX.ValueTuple.tuplelib_cs,
@"class Program
{
    static void Main(string[] args)
    {
        var (x, y) = (1, 2);
        int z = {|Rename:NewMethod|}();
        System.Console.WriteLine(z);

        static int NewMethod()
        {
            return 3;
        }
    }
}" + TestResources.NetFX.ValueTuple.tuplelib_cs);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        [Test.Utilities.CompilerTrait(Test.Utilities.CompilerFeature.OutVar)]
        public async Task TestOutVar()
        {
            await TestInRegularAndScriptAsync(
@"class C
{
    static void M(int i)
    {
        int r;
        [|r = M1(out int y, i);|]
        System.Console.WriteLine(r + y);
    }
}",
@"class C
{
    static void M(int i)
    {
        int r;
        int y;
        {|Rename:NewMethod|}(i, out r, out y);
        System.Console.WriteLine(r + y);

        static void NewMethod(int i, out int r, out int y)
        {
            r = M1(out y, i);
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        [Test.Utilities.CompilerTrait(Test.Utilities.CompilerFeature.Patterns)]
        public async Task TestIsPattern()
        {
            await TestInRegularAndScriptAsync(
@"class C
{
    static void M(int i)
    {
        int r;
        [|r = M1(3 is int y, i);|]
        System.Console.WriteLine(r + y);
    }
}",
@"class C
{
    static void M(int i)
    {
        int r;
        int y;
        {|Rename:NewMethod|}(i, out r, out y);
        System.Console.WriteLine(r + y);

        static void NewMethod(int i, out int r, out int y)
        {
            r = M1(3 is int {|Conflict:y|}, i);
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        [Test.Utilities.CompilerTrait(Test.Utilities.CompilerFeature.Patterns)]
        public async Task TestOutVarAndIsPattern()
        {
            await TestInRegularAndScriptAsync(
@"class C
{
    static void M()
    {
        int r;
        [|r = M1(out /*out*/ int /*int*/ y /*y*/) + M2(3 is int z);|]
        System.Console.WriteLine(r + y + z);
    }
} ",
@"class C
{
    static void M()
    {
        int r;
        int y, z;
        {|Rename:NewMethod|}(out r, out y, out z);
        System.Console.WriteLine(r + y + z);

        static void NewMethod(out int r, out int y, out int z)
        {
            r = M1(out /*out*/  /*int*/ y /*y*/) + M2(3 is int {|Conflict:z|});
        }
    }
} ");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        [Test.Utilities.CompilerTrait(Test.Utilities.CompilerFeature.Patterns)]
        public async Task ConflictingOutVarLocals()
        {
            await TestInRegularAndScriptAsync(
@"class C
{
    static void M()
    {
        int r;
        [|r = M1(out int y);
        {
            M2(out int y);
            System.Console.Write(y);
        }|]

        System.Console.WriteLine(r + y);
    }
}",
@"class C
{
    static void M()
    {
        int r;
        int y;
        {|Rename:NewMethod|}(out r, out y);

        System.Console.WriteLine(r + y);

        static void NewMethod(out int r, out int y)
        {
            r = M1(out y);
            {
                M2(out int y);
                System.Console.Write(y);
            }
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        [Test.Utilities.CompilerTrait(Test.Utilities.CompilerFeature.Patterns)]
        public async Task ConflictingPatternLocals()
        {
            await TestInRegularAndScriptAsync(
@"class C
{
    static void M()
    {
        int r;
        [|r = M1(1 is int y);
        {
            M2(2 is int y);
            System.Console.Write(y);
        }|]

        System.Console.WriteLine(r + y);
    }
}",
@"class C
{
    static void M()
    {
        int r;
        int y;
        {|Rename:NewMethod|}(out r, out y);

        System.Console.WriteLine(r + y);

        static void NewMethod(out int r, out int y)
        {
            r = M1(1 is int {|Conflict:y|});
            {
                M2(2 is int y);
                System.Console.Write(y);
            }
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestCancellationTokenGoesLast()
        {
            await TestInRegularAndScriptAsync(
@"using System;
using System.Threading;

class C
{
    void M(CancellationToken ct)
    {
        var v = 0;

        [|if (true)
        {
            ct.ThrowIfCancellationRequested();
            Console.WriteLine(v);
        }|]
    }
}",
@"using System;
using System.Threading;

class C
{
    void M(CancellationToken ct)
    {
        var v = 0;
        {|Rename:NewMethod|}(v, ct);

        static void NewMethod(int v, CancellationToken ct)
        {
            if (true)
            {
                ct.ThrowIfCancellationRequested();
                Console.WriteLine(v);
            }
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestUseVar1()
        {
            await TestInRegularAndScriptAsync(
@"using System;

class C
{
    void Goo(int i)
    {
        [|var v = (string)null;

        switch (i)
        {
            case 0: v = ""0""; break;
            case 1: v = ""1""; break;
        }|]

        Console.WriteLine(v);
    }
}",
@"using System;

class C
{
    void Goo(int i)
    {
        var v = {|Rename:NewMethod|}(i);

        Console.WriteLine(v);

        static string NewMethod(int i)
        {
            var v = (string)null;

            switch (i)
            {
                case 0: v = ""0""; break;
                case 1: v = ""1""; break;
            }

            return v;
        }
    }
}", options: Option(CSharpCodeStyleOptions.VarForBuiltInTypes, CodeStyleOptions.TrueWithSuggestionEnforcement));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestUseVar2()
        {
            await TestInRegularAndScriptAsync(
@"using System;

class C
{
    void Goo(int i)
    {
        [|var v = (string)null;

        switch (i)
        {
            case 0: v = ""0""; break;
            case 1: v = ""1""; break;
        }|]

        Console.WriteLine(v);
    }
}",
@"using System;

class C
{
    void Goo(int i)
    {
        string v = {|Rename:NewMethod|}(i);

        Console.WriteLine(v);

        static string NewMethod(int i)
        {
            var v = (string)null;

            switch (i)
            {
                case 0: v = ""0""; break;
                case 1: v = ""1""; break;
            }

            return v;
        }
    }
}", options: Option(CSharpCodeStyleOptions.VarWhenTypeIsApparent, CodeStyleOptions.TrueWithSuggestionEnforcement));
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task ExtractLocalFunctionCall()
        {
            await TestInRegularAndScriptAsync(@"
class C
{
    public static void Main()
    {
        void Local() { }
        [|Local();|]
    }
}", @"
class C
{
    public static void Main()
    {
        void Local() { }
        {|Rename:NewMethod|}();

        void NewMethod()
        {
            Local();
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task ExtractLocalFunctionCallWithCapture()
        {
            await TestInRegularAndScriptAsync(@"
class C
{
    public static void Main(string[] args)
    {
        bool Local() => args == null;
        [|Local();|]
    }
}", @"
class C
{
    public static void Main(string[] args)
    {
        bool Local() => args == null;
        {|Rename:NewMethod|}(args);

        void NewMethod(string[] args)
        {
            Local();
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task ExtractLocalFunctionDeclaration()
        {
            await TestMissingInRegularAndScriptAsync(@"
class C
{
    public static void Main()
    {
        [|bool Local() => args == null;|]
        Local();
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task ExtractLocalFunctionInterior()
        {
            await TestInRegularAndScriptAsync(@"
class C
{
    public static void Main()
    {
        void Local()
        {
            [|int x = 0;
            x++;|]
        }
        Local();
    }
}", @"
class C
{
    public static void Main()
    {
        void Local()
        {
            {|Rename:NewMethod|}();

            static void NewMethod()
            {
                int x = 0;
                x++;
            }
        }
        Local();
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task Bug3790()
        {
            await TestInRegularAndScriptAsync(@"
class Test
{
    void method()
    {
        static void Main(string[] args)
        {
            int v = 0;
            for(int i=0 ; i<5; i++)
            {
                [|v = v + i;|]
            }
        }
    }
}", @"
class Test
{
    void method()
    {
        static void Main(string[] args)
        {
            int v = 0;
            for(int i=0 ; i<5; i++)
            {
                v = {|Rename:NewMethod|}(v, i);
            }

            static int NewMethod(int v, int i)
            {
                v = v + i;
                return v;
            }
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task Bug3790_1()
        {
            await TestInRegularAndScriptAsync(@"
class Test
{
    void method()
    {
        static void Main(string[] args)
        {
            int v = 0;
            for(int i=0 ; i<5; i++)
            {
                [|v = v + i|];
            }
        }
    }
}", @"
class Test
{
    void method()
    {
        static void Main(string[] args)
        {
            int v = 0;
            for(int i=0 ; i<5; i++)
            {
                v = {|Rename:NewMethod|}(v, i);
            }

            static int NewMethod(int v, int i)
            {
                return v + i;
            }
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task Bug3790_2()
        {
            await TestInRegularAndScriptAsync(@"
class Test
{
    void method()
    {
        static void Main(string[] args)
        {
            int v = 0;
            for(int i=0 ; i<5; i++)
            {
                [|i = v = v + i|];
            }
        }
    }
}", @"
class Test
{
    void method()
    {
        static void Main(string[] args)
        {
            int v = 0;
            for(int i=0 ; i<5; i++)
            {
                i = {|Rename:NewMethod|}(ref v, i);
            }

            static int NewMethod(ref int v, int i)
            {
                return v = v + i;
            }
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestMissingInExpressionBodyProperty()
        {
            await TestMissingInRegularAndScriptAsync(@"
class Program
{
    int field;

    public int Blah => [|this.field|];
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestMissingInExpressionBodyIndexer()
        {
            await TestMissingInRegularAndScriptAsync(@"
class Program
{
    int field;

    public int this[int i] => [|this.field|];
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestMissingInExpressionBodyPropertyGetAccessor()
        {
            await TestMissingInRegularAndScriptAsync(@"
class Program
{
    int field;

    public int Blah
    {
        get => [|this.field|];
        set => field = value;
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestMissingInExpressionBodyPropertySetAccessor()
        {
            await TestMissingInRegularAndScriptAsync(@"
class Program
{
    int field;

    public int Blah
    {
        get => this.field;
        set => field = [|value|];
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestMissingInExpressionBodyIndexerGetAccessor()
        {
            await TestMissingInRegularAndScriptAsync(@"
class Program
{
    int field;

    public int this[int i]
    {
        get => [|this.field|];
        set => field = value;
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestMissingInExpressionBodyIndexerSetAccessor()
        {
            await TestMissingInRegularAndScriptAsync(@"
class Program
{
    int field;

    public int this[int i]
    {
        get => this.field;
        set => field = [|value|];
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestTupleWithInferredNames()
        {
            await TestAsync(@"
class Program
{
    void M()
    {
        int a = 1;
        var t = [|(a, b: 2)|];
        System.Console.Write(t.a);
    }
}",
@"
class Program
{
    void M()
    {
        int a = 1;
        var t = {|Rename:GetT|}(a);
        System.Console.Write(t.a);

        static (int a, int b) GetT(int a)
        {
            return (a, b: 2);
        }
    }
}", TestOptions.Regular7_1);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestDeconstruction4()
        {
            await TestAsync(@"
class Program
{
    void M()
    {
        [|var (x, y) = (1, 2);|]
        System.Console.Write(x + y);
    }
}",
@"
class Program
{
    void M()
    {
        int x, y;
        {|Rename:NewMethod|}(out x, out y);
        System.Console.Write(x + y);

        static void NewMethod(out int x, out int y)
        {
            var (x, y) = (1, 2);
        }
    }
}", TestOptions.Regular7_1);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestDeconstruction5()
        {
            await TestAsync(@"
class Program
{
    void M()
    {
        [|(var x, var y) = (1, 2);|]
        System.Console.Write(x + y);
    }
}",
@"
class Program
{
    void M()
    {
        int x, y;
        {|Rename:NewMethod|}(out x, out y);
        System.Console.Write(x + y);

        static void NewMethod(out int x, out int y)
        {
            (x, y) = (1, 2);
        }
    }
}", TestOptions.Regular7_1);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestIndexExpression()
        {
            await TestInRegularAndScriptAsync(TestSources.Index + @"
class Program
{
    static void Main(string[] args)
    {
        System.Console.WriteLine([|^1|]);
    }
}",
TestSources.Index +
@"
class Program
{
    static void Main(string[] args)
    {
        System.Console.WriteLine({|Rename:NewMethod|}());

        static System.Index NewMethod()
        {
            return ^1;
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestRangeExpression_Empty()
        {
            await TestInRegularAndScriptAsync(TestSources.Index + TestSources.Range + @"
class Program
{
    static void Main(string[] args)
    {
        System.Console.WriteLine([|..|]);
    }
}",
TestSources.Index +
TestSources.Range + @"
class Program
{
    static void Main(string[] args)
    {
        System.Console.WriteLine({|Rename:NewMethod|}());

        static System.Range NewMethod()
        {
            return ..;
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestRangeExpression_Left()
        {
            await TestInRegularAndScriptAsync(TestSources.Index + TestSources.Range + @"
class Program
{
    static void Main(string[] args)
    {
        System.Console.WriteLine([|..1|]);
    }
}",
TestSources.Index +
TestSources.Range + @"
class Program
{
    static void Main(string[] args)
    {
        System.Console.WriteLine({|Rename:NewMethod|}());

        static System.Range NewMethod()
        {
            return ..1;
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestRangeExpression_Right()
        {
            await TestInRegularAndScriptAsync(TestSources.Index + TestSources.Range + @"
class Program
{
    static void Main(string[] args)
    {
        System.Console.WriteLine([|1..|]);
    }
}",
TestSources.Index +
TestSources.Range + @"
class Program
{
    static void Main(string[] args)
    {
        System.Console.WriteLine({|Rename:NewMethod|}());

        static System.Range NewMethod()
        {
            return 1..;
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestRangeExpression_Both()
        {
            await TestInRegularAndScriptAsync(TestSources.Index + TestSources.Range + @"
class Program
{
    static void Main(string[] args)
    {
        System.Console.WriteLine([|1..2|]);
    }
}",
TestSources.Index +
TestSources.Range + @"
class Program
{
    static void Main(string[] args)
    {
        System.Console.WriteLine({|Rename:NewMethod|}());

        static System.Range NewMethod()
        {
            return 1..2;
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestAnnotatedNullableReturn()
            => TestInRegularAndScriptAsync(
@"#nullable enable

class C
{
    public string? M()
    {
        [|string? x = null;
        x?.ToString();|]

        return x;
    }
}",
@"#nullable enable

class C
{
    public string? M()
    {
        string? x = {|Rename:NewMethod|}();

        return x;

        static string? NewMethod()
        {
            string? x = null;
            x?.ToString();
            return x;
        }
    }
}");


        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestAnnotatedNullableParameters1()
            => TestInRegularAndScriptAsync(
@"#nullable enable

class C
{
    public string? M()
    {
        string? a = null;
        string? b = null;
        [|string? x = a?.Contains(b).ToString();|]

        return x;
    }
}",
@"#nullable enable

class C
{
    public string? M()
    {
        string? a = null;
        string? b = null;
        string? x = {|Rename:NewMethod|}(a, b);

        return x;

        static string? NewMethod(string? a, string? b)
        {
            return a?.Contains(b).ToString();
        }
    }
}");

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestAnnotatedNullableParameters2()
            => TestInRegularAndScriptAsync(
@"#nullable enable

class C
{
    public string M()
    {
        string? a = null;
        string? b = null;
        int c = 0;
        [|string x = (a + b + c).ToString();|]

        return x;
    }
}",
@"#nullable enable

class C
{
    public string M()
    {
        string? a = null;
        string? b = null;
        int c = 0;
        string x = {|Rename:NewMethod|}(a, b, c);

        return x;

        static string NewMethod(string? a, string? b, int c)
        {
            return (a + b + c).ToString();
        }
    }
}");

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestAnnotatedNullableParameters3()
            => TestInRegularAndScriptAsync(
@"#nullable enable

class C
{
    public string M()
    {
        string? a = null;
        string? b = null;
        int c = 0;
        return [|(a + b + c).ToString()|];
    }
}",
@"#nullable enable

class C
{
    public string M()
    {
        string? a = null;
        string? b = null;
        int c = 0;
        return {|Rename:NewMethod|}(a, b, c);

        static string NewMethod(string? a, string? b, int c)
        {
            return (a + b + c).ToString();
        }
    }
}");

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestAnnotatedNullableParameters4()
            => TestInRegularAndScriptAsync(
@"#nullable enable

class C
{
    public string? M()
    {
        string? a = null;
        string? b = null;
        return [|a?.Contains(b).ToString()|];
    }
}",
@"#nullable enable

class C
{
    public string? M()
    {
        string? a = null;
        string? b = null;
        return {|Rename:NewMethod|}(a, b);

        static string? NewMethod(string? a, string? b)
        {
            return a?.Contains(b).ToString();
        }
    }
}");

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestFlowStateNullableParameters1()
            => TestInRegularAndScriptAsync(
@"#nullable enable

class C
{
    public string M()
    {
        string? a = string.Empty;
        string? b = string.Empty;
        return [|(a + b + a).ToString()|];
    }
}",
@"#nullable enable

class C
{
    public string M()
    {
        string? a = string.Empty;
        string? b = string.Empty;
        return {|Rename:NewMethod|}(a, b);

        static string NewMethod(string a, string b)
        {
            return (a + b + a).ToString();
        }
    }
}");

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestFlowStateNullableParameters2()
            => TestInRegularAndScriptAsync(
@"#nullable enable

class C
{
    public string? M()
    {
        string? a = string.Empty;
        string? b = string.Empty;
        return [|(a + b + a).ToString()|];
    }
}",
@"#nullable enable

class C
{
    public string? M()
    {
        string? a = string.Empty;
        string? b = string.Empty;
        return {|Rename:NewMethod|}(a, b);

        static string NewMethod(string a, string b)
        {
            return (a + b + a).ToString();
        }
    }
}");

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestFlowStateNullableParameters3()
            => TestInRegularAndScriptAsync(
@"#nullable enable

class C
{
    public string M()
    {
        string? a = null;
        string? b = null;
        return [|(a + b + a)?.ToString()|] ?? string.Empty;
    }
}",
@"#nullable enable

class C
{
    public string M()
    {
        string? a = null;
        string? b = null;
        return {|Rename:NewMethod|}(a, b) ?? string.Empty;

        static string? NewMethod(string? a, string? b)
        {
            return (a + b + a)?.ToString();
        }
    }
}");

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestFlowStateNullableParameters_MultipleStates()
            => TestInRegularAndScriptAsync(
@"#nullable enable

class C
{
    public string M()
    {
        string? a = string.Empty;
        string? b = string.Empty;
        [|string? c = a + b;
        a = string.Empty;
        c += a;
        a = null;
        b = null;
        b = ""test"";
        c = a?.ToString();|]
        return c ?? string.Empty;
    }
}",
@"#nullable enable

class C
{
    public string M()
    {
        string? a = string.Empty;
        string? b = string.Empty;
        string? c = {|Rename:NewMethod|}(ref a, ref b);
        return c ?? string.Empty;

        static string? NewMethod(ref string? a, ref string? b)
        {
            string? c = a + b;
            a = string.Empty;
            c += a;
            a = null;
            b = null;
            b = ""test"";
            c = a?.ToString();
            return c;
        }
    }
}");

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestFlowStateNullableParameters_MultipleStatesNonNullReturn()
            => TestInRegularAndScriptAsync(
@"#nullable enable

class C
{
    public string M()
    {
        string? a = string.Empty;
        string? b = string.Empty;
        [|string? c = a + b;
        a = string.Empty;
        b = string.Empty;
        a = null;
        b = null;
        c = null;
        c = a + b;|]
        return c ?? string.Empty;
    }
}",
@"#nullable enable

class C
{
    public string M()
    {
        string? a = string.Empty;
        string? b = string.Empty;
        string? c = {|Rename:NewMethod|}(ref a, ref b);
        return c ?? string.Empty;

        static string NewMethod(ref string? a, ref string? b)
        {
            string? c = a + b;
            a = string.Empty;
            b = string.Empty;
            a = null;
            b = null;
            c = null;
            c = a + b;
            return c;
        }
    }
}");

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestFlowStateNullableParameters_MultipleStatesNullReturn()
            => TestInRegularAndScriptAsync(
@"#nullable enable

class C
{
    public string M()
    {
        string? a = string.Empty;
        string? b = string.Empty;
        [|string? c = a + b;
        a = string.Empty;
        b = string.Empty;
        a = null;
        b = null;
        c = a?.ToString();|]
        return c ?? string.Empty;
    }
}",
@"#nullable enable

class C
{
    public string M()
    {
        string? a = string.Empty;
        string? b = string.Empty;
        string? c = {|Rename:NewMethod|}(ref a, ref b);
        return c ?? string.Empty;

        static string? NewMethod(ref string? a, ref string? b)
        {
            string? c = a + b;
            a = string.Empty;
            b = string.Empty;
            a = null;
            b = null;
            c = a?.ToString();
            return c;
        }
    }
}");

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestFlowStateNullableParameters_RefNotNull()
            => TestInRegularAndScriptAsync(
@"#nullable enable

class C
{
    public string M()
    {
        string? a = string.Empty;
        string? b = string.Empty;
        [|var c = a + b;
        a = string.Empty;
        c += a;
        b = ""test"";
        c = a + b +c;|]
        return c;
    }
}",
@"#nullable enable

class C
{
    public string M()
    {
        string? a = string.Empty;
        string? b = string.Empty;
        string c = {|Rename:NewMethod|}(ref a, ref b);
        return c;

        static string NewMethod(ref string a, ref string b)
        {
            var c = a + b;
            a = string.Empty;
            c += a;
            b = ""test"";
            c = a + b + c;
            return c;
        }
    }
}");

        // There's a case below where flow state correctly asseses that the variable
        // 'x' is non-null when returned. It's wasn't obvious when writing, but that's 
        // due to the fact the line above it being executed as 'x.ToString()' would throw
        // an exception and the return statement would never be hit. The only way the return
        // statement gets executed is if the `x.ToString()` call succeeds, thus suggesting 
        // that the value is indeed not null.
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestFlowNullableReturn_NotNull1()
            => TestInRegularAndScriptAsync(
@"#nullable enable

class C
{
    public string? M()
    {
        [|string? x = null;
        x.ToString();|]

        return x;
    }
}",
@"#nullable enable

class C
{
    public string? M()
    {
        string? x = {|Rename:NewMethod|}();

        return x;

        static string NewMethod()
        {
            string? x = null;
            x.ToString();
            return x;
        }
    }
}");

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestFlowNullableReturn_NotNull2()
            => TestInRegularAndScriptAsync(
@"#nullable enable

class C
{
    public string? M()
    {
        [|string? x = null;
        x?.ToString();
        x = string.Empty;|]

        return x;
    }
}",
@"#nullable enable

class C
{
    public string? M()
    {
        string? x = {|Rename:NewMethod|}();

        return x;

        static string NewMethod()
        {
            string? x = null;
            x?.ToString();
            x = string.Empty;
            return x;
        }
    }
}");
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestFlowNullable_Lambda()
            => TestInRegularAndScriptAsync(
@"#nullable enable

using System;

class C
{
    public string? M()
    {
        [|string? x = null;
        Action modifyXToNonNull = () =>
        {
            x += x;
        };

        modifyXToNonNull();|]

        return x;
    }
}",
@"#nullable enable

using System;

class C
{
    public string? M()
    {
        string? x = {|Rename:NewMethod|}();

        return x;

        static string? NewMethod()
        {
            string? x = null;
            Action modifyXToNonNull = () =>
            {
                x += x;
            };

            modifyXToNonNull();
            return x;
        }
    }
}");

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestFlowNullable_LambdaWithReturn()
            => TestInRegularAndScriptAsync(
@"#nullable enable

using System;

class C
{
    public string? M()
    {
        [|string? x = null;
        Func<string?> returnNull = () =>
        {
            return null;
        };

        x = returnNull() ?? string.Empty;|]

        return x;
    }
}",
@"#nullable enable

using System;

class C
{
    public string? M()
    {
        string? x = {|Rename:NewMethod|}();

        return x;

        static string NewMethod()
        {
            string? x = null;
            Func<string?> returnNull = () =>
            {
                return null;
            };

            x = returnNull() ?? string.Empty;
            return x;
        }
    }
}");

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestExtractReadOnlyMethod()
        {
            await TestInRegularAndScriptAsync(
@"struct S1
{
    readonly int M1() => 42;
    void Main()
    {
        [|int i = M1() + M1()|];
    }
}",
@"struct S1
{
    readonly int M1() => 42;
    void Main()
    {
        {|Rename:NewMethod|}();

        readonly void NewMethod()
        {
            int i = M1() + M1();
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestExtractReadOnlyMethodInReadOnlyStruct()
        {
            await TestInRegularAndScriptAsync(
@"readonly struct S1
{
    int M1() => 42;
    void Main()
    {
        [|int i = M1() + M1()|];
    }
}",
@"readonly struct S1
{
    int M1() => 42;
    void Main()
    {
        {|Rename:NewMethod|}();

        void NewMethod()
        {
            int i = M1() + M1();
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestExtractNonReadOnlyMethodInReadOnlyMethod()
        {
            await TestInRegularAndScriptAsync(
@"struct S1
{
    int M1() => 42;
    readonly void Main()
    {
        [|int i = M1() + M1()|];
    }
}",
@"struct S1
{
    int M1() => 42;
    readonly void Main()
    {
        {|Rename:NewMethod|}();

        void NewMethod()
        {
            int i = M1() + M1();
        }
    }
}");
        }


        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestExtractNullableObjectWithExplicitCast()
        => TestInRegularAndScriptAsync(
@"#nullable enable

using System;

class C
{
    void M()
    {
        object? o = null;
        var s = (string?)[|o|];
        Console.WriteLine(s);
    }
}",
@"#nullable enable

using System;

class C
{
    void M()
    {
        object? o = null;
        var s = (string?){|Rename:GetO|}(o);
        Console.WriteLine(s);

        static object? GetO(object? o)
        {
            return o;
        }
    }
}");

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestExtractNotNullableObjectWithExplicitCast()
        => TestInRegularAndScriptAsync(
@"#nullable enable

using System;

class C
{
    void M()
    {
        object? o = new object();
        var s = (string)[|o|];
        Console.WriteLine(s);
    }
}",
@"#nullable enable

using System;

class C
{
    void M()
    {
        object? o = new object();
        var s = (string){|Rename:GetO|}(o);
        Console.WriteLine(s);

        static object GetO(object o)
        {
            return o;
        }
    }
}");

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestExtractNotNullableWithExplicitCast()
        => TestInRegularAndScriptAsync(
@"#nullable enable

using System;

class A
{
}

class B : A 
{
}

class C
{
    void M()
    {
        B? b = new B();
        var s = (A)[|b|];
    }
}",
@"#nullable enable

using System;

class A
{
}

class B : A 
{
}

class C
{
    void M()
    {
        B? b = new B();
        var s = (A){|Rename:GetB|}(b);

        static B GetB(B b)
        {
            return b;
        }
    }
}");

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestExtractNullableWithExplicitCast()
        => TestInRegularAndScriptAsync(
@"#nullable enable

using System;

class A
{
}

class B : A 
{
}

class C
{
    void M()
    {
        B? b = null;
        var s = (A)[|b|];
    }
}",
@"#nullable enable

using System;

class A
{
}

class B : A 
{
}

class C
{
    void M()
    {
        B? b = null;
        var s = (A){|Rename:GetB|}(b);

        static B? GetB(B? b)
        {
            return b;
        }
    }
}");

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestExtractNotNullableWithExplicitCastSelected()
        => TestInRegularAndScriptAsync(
@"#nullable enable

using System;

class C
{
    void M()
    {
        object? o = new object();
        var s = [|(string)o|];
        Console.WriteLine(s);
    }
}",
@"#nullable enable

using System;

class C
{
    void M()
    {
        object? o = new object();
        var s = {|Rename:GetS|}(o);
        Console.WriteLine(s);

        static string GetS(object o)
        {
            return (string)o;
        }
    }
}");

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestExtractNullableWithExplicitCastSelected()
        => TestInRegularAndScriptAsync(
@"#nullable enable

using System;

class C
{
    void M()
    {
        object? o = null;
        var s = [|(string?)o|];
        Console.WriteLine(s);
    }
}",
@"#nullable enable

using System;

class C
{
    void M()
    {
        object? o = null;
        var s = {|Rename:GetS|}(o);
        Console.WriteLine(s);

        static string? GetS(object? o)
        {
            return (string?)o;
        }
    }
}");
        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestExtractNullableNonNullFlowWithExplicitCastSelected()
        => TestInRegularAndScriptAsync(
@"#nullable enable

using System;

class C
{
    void M()
    {
        object? o = new object();
        var s = [|(string?)o|];
        Console.WriteLine(s);
    }
}",
@"#nullable enable

using System;

class C
{
    void M()
    {
        object? o = new object();
        var s = {|Rename:GetS|}(o);
        Console.WriteLine(s);

        static string? GetS(object o)
        {
            return (string?)o;
        }
    }
}");

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public Task TestExtractNullableToNonNullableWithExplicitCastSelected()
        => TestInRegularAndScriptAsync(
@"#nullable enable

using System;

class C
{
    void M()
    {
        object? o = null;
        var s = [|(string)o|];
        Console.WriteLine(s);
    }
}",
@"#nullable enable

using System;

class C
{
    void M()
    {
        object? o = null;
        var s = {|Rename:GetS|}(o);
        Console.WriteLine(s);

        static string? GetS(object? o)
        {
            return (string)o;
        }
    }
}");

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestExtractLocalFunction_EnsureUniqueFunctionName()
        {
            await TestInRegularAndScriptAsync(
@"class Test
{
    static void Main(string[] args)
    {
        [|var test = 1;|]

        static void NewMethod()
        {
            var test = 1;
        }
    }
}",
@"class Test
{
    static void Main(string[] args)
    {
        {|Rename:NewMethod1|}();

        static void NewMethod()
        {
            var test = 1;
        }

        static void NewMethod1()
        {
            var test = 1;
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestExtractLocalFunctionWithinLocalMethod_EnsureUniqueMethodName()
        {
            await TestInRegularAndScriptAsync(
@"class Test
{
    static void Main(string[] args)
    {
        static void NewMethod()
        {
            var NewMethod2 = 0;
            [|var test = 1;|]

            static void NewMethod1()
            {
                var test = 1;
            }
        }
    }
}",
@"class Test
{
    static void Main(string[] args)
    {
        static void NewMethod()
        {
            var NewMethod2 = 0;
            {|Rename:NewMethod2|}();

            static void NewMethod1()
            {
                var test = 1;
            }

            static void NewMethod2()
            {
                var test = 1;
            }
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task TestExtractNonStaticLocalMethod_WithDeclaration()
        {
            await TestInRegularAndScriptAsync(
@"class Test
{
    static void Main(string[] args)
    {
        [|ExistingLocalFunction();

        void ExistingLocalFunction()
        {
        }|]
    }
}",
@"class Test
{
    static void Main(string[] args)
    {
        {|Rename:NewMethod|}();

        static void NewMethod()
        {
            ExistingLocalFunction();

            void ExistingLocalFunction()
            {
            }
        }
    }
}");
        }

        [Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractLocalFunction)]
        public async Task ArgumentlessReturnWithConstIfExpression()
        {
            await TestInRegularAndScriptAsync(
@"using System;

class Program
{
    void Test()
    {
        if (true)
            [|if (true)
                return;|]
        Console.WriteLine();
    }
}",
@"using System;

class Program
{
    void Test()
    {
        if (true)
        {
            {|Rename:NewMethod|}();
            return;
        }
        Console.WriteLine();

        static void NewMethod()
        {
            if (true)
                return;
        }
    }
}");
        }
    }
}
