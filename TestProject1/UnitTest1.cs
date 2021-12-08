using AssemblyBrowserProject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace TestProject1
{

    [TestClass]
    public class UnitTest1
    {
        TreeComponent tree;
        private void testSetup()
        {
            AssemblyTreeBuilder asm =
      new AssemblyTreeBuilder("D:\\smth\\походу уник\\spp\\spp3\\TestAssembly\\bin\\Debug\\net5.0\\TestAssembly.dll");
            asm.Build();
            tree = asm.GetRoot();
        }
        
        [TestMethod]
        public void TestNamespacesStructure()
        {
            testSetup();
            TreeComposite curr = tree.GetTreeComposite();
   
            Assert.AreEqual(curr.Name, "root", "Root namespace hasn't been created!");

            Assert.IsTrue(curr.Find("TestAssembly") >= 0, "TestAssembly namespace should be in root!");
            Assert.IsTrue(curr.Find("A") >= 0, "class A should be in root!");
            Assert.IsTrue(curr.Find("B") >= 0, "class B should be in root!");

            curr = curr.GetChildAt(curr.Find("TestAssembly")).GetTreeComposite();

            Assert.IsTrue(curr.Find("A") >= 0, "class A should be in TestAssembly!");
            Assert.IsTrue(curr.Find("B") >= 0, "class B should be in TestAssembly!");
            Assert.IsTrue(curr.Find("C") >= 0, "class C should be in TestAssembly!");

            Assert.IsTrue(curr.Find("N1") >= 0, "N1 namespace should be in TestAssembly!");
            Assert.IsTrue(curr.Find("N2") >= 0, "N2 namespace should be in TestAssembly!");
            Assert.IsTrue(curr.Find("N4") >= 0, "N4 namespace should be in TestAssembly!");
            Assert.AreEqual(curr.Find("N3"), -1, "N3 namespace should not be in TestAssembly!");

            TreeComposite oldCurr = curr;

            curr = curr.GetChildAt(curr.Find("N4")).GetTreeComposite();
            Assert.AreEqual(curr.Find("T"), 0, "class T should be in N4!");
            curr = oldCurr;

            curr = curr.GetChildAt(curr.Find("N2")).GetTreeComposite();
            Assert.AreEqual(curr.Find("F1"), -1, "F1 namespace should not be in TestAssembly!");
            Assert.AreEqual(curr.Find("F3"), -1, "F3 namespace should not be in TestAssembly!");
            Assert.AreEqual(curr.Find("F2"), 0, "F2 namespace should be in TestAssembly!");

            curr = curr.GetChildAt(curr.Find("F2")).GetTreeComposite();

            Assert.IsTrue(curr.Find("A") >= 0, "class A should be in F2!");
            Assert.IsTrue(curr.Find("B") >= 0, "class B should be in F2!");
            Assert.IsTrue(curr.Find("C") >= 0, "class C should be in F2!");

            curr = oldCurr;
            curr = curr.GetChildAt(curr.Find("N1")).GetTreeComposite();

            Assert.AreEqual(curr.Find("T1"), -1, "T1 namespace should not be in N1!");
            Assert.AreEqual(curr.Find("T2"), 0, "T2 namespace should be in N1!");

            curr = curr.GetChildAt(curr.Find("T2")).GetTreeComposite();
            Assert.AreEqual(curr.Find("X"), 0, "X namespace should be in T2!");
        }

        [TestMethod]
        public void TestNestedTypes()
        {
            testSetup();
            TreeComposite curr = tree.GetTreeComposite();
            curr = curr.GetChildAt(curr.Find("ClassesStructTest")).GetTreeComposite();
            TreeComposite oldCurr = curr;

            curr = curr.GetChildAt(curr.Find("InnerNamespace")).GetTreeComposite();
            Assert.IsTrue(curr.Find("B") >= 0, "class B should be in InnerNamespace");
            Assert.IsTrue(curr.Find("D") >= 0, "class D should be in InnerNamespace");

            curr = oldCurr;
            curr = curr.GetChildAt(curr.Find("A")).GetTreeComposite();

            Assert.IsTrue(curr.Find("F") >= 0, "struct F should be in class A");
            Assert.IsTrue(curr.Find("C") >= 0, "class C should be in class A");

            curr = curr.GetChildAt(curr.Find("C")).GetTreeComposite();
            Assert.IsTrue(curr.Find("T") >= 0, "T class should be in C!");
            curr = curr.GetChildAt(curr.Find("T")).GetTreeComposite();
            Assert.IsTrue(curr.Find("P") >= 0, "P class should be in T!");
            curr = curr.GetChildAt(curr.Find("P")).GetTreeComposite();
            Assert.IsTrue(curr.Find("Z") >= 0, "Z class should be in P!");
            curr = curr.GetChildAt(curr.Find("Z")).GetTreeComposite();
            Assert.IsTrue(curr.Find("H") >= 0, "H struct should be in Z!");
            curr = curr.GetChildAt(curr.Find("H")).GetTreeComposite();
            Assert.IsTrue(curr.ComponentType == COMPONENT_TYPE.STRUCT, "H is supposed to be a struct!");
        }

        [TestMethod]
        public void TestFieldsTypes()
        {
            testSetup();
            TreeComposite curr = tree.GetTreeComposite();
            TreeComposite oldCurr = curr;

            curr = curr.GetChildAt(curr.Find("M")).GetTreeComposite();
            Assert.IsTrue(curr.Find("myArray") >= 0, "double[] myArray should be in M!");
            Assert.IsTrue(curr.Find("strP") >= 0, "P strP should be in M!");
            Assert.IsTrue(curr.Find("f") >= 0, "float f should be in M!");
            Assert.IsTrue(curr.Find("zzz") >= 0, "long zzz should be in M!");
            Assert.IsTrue(curr.Find("sss") >= 0, "string sss should be in M!");

            curr = oldCurr;
            curr = curr.GetChildAt(curr.Find("P")).GetTreeComposite();
            Assert.IsTrue(curr.Find("a") >= 0, "int a should be in P!");
            Assert.IsTrue(curr.Find("d") >= 0, "double d should be in P!");
            Assert.IsTrue(curr.Find("f") >= 0, "float f should be in P!");
            Assert.IsTrue(curr.Find("str") >= 0, "string str should be in P!");
            Assert.IsTrue(curr.Find("c") >= 0, "char c should be in P!");
            Assert.IsTrue(curr.Find("field") >= 0, "M field should be in P!");
            Assert.IsTrue(curr.Find("dt") >= 0, "DateTime dt should be in P!");
            Assert.IsTrue(curr.Find("rand") >= 0, "Random rand should be in P!");
        }

        [TestMethod]
        public void TestFieldsCategory()
        {
            testSetup();
            TreeComposite curr = tree.GetTreeComposite();
            TreeComposite oldCurr = curr;

            COMPONENT_TYPE checkAgainstField = COMPONENT_TYPE.FIELD;

            curr = curr.GetChildAt(curr.Find("M")).GetTreeComposite();
            Assert.AreEqual(curr.GetChildAt(curr.Find("myArray")).ComponentType, checkAgainstField, "double[] myArray should be a field!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("strP")).ComponentType, checkAgainstField, "P strP should be a field!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("f")).ComponentType, checkAgainstField, "float f should be a field!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("zzz")).ComponentType, checkAgainstField, "long zzz should be a field!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("sss")).ComponentType, checkAgainstField, "string sss should be a field!");

            curr = oldCurr;
            curr = curr.GetChildAt(curr.Find("P")).GetTreeComposite();
            Assert.AreEqual(curr.GetChildAt(curr.Find("a")).ComponentType, checkAgainstField, "int a should be a field!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("d")).ComponentType, checkAgainstField, "double d should be a field!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("f")).ComponentType, checkAgainstField, "float f should be a field!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("str")).ComponentType, checkAgainstField, "string str should be a field!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("c")).ComponentType, checkAgainstField, "char c should be a field!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("field")).ComponentType, checkAgainstField, "M field should be a field!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("dt")).ComponentType, checkAgainstField, "DateTime dt should be a field!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("rand")).ComponentType, checkAgainstField, "Random rand should be a field!");
        }

        [TestMethod]
        public void TestPropertiesTypes()
        {
            testSetup();
            TreeComposite curr = tree.GetTreeComposite();
            TreeComposite oldCurr = curr;

            curr = curr.GetChildAt(curr.Find("Q")).GetTreeComposite();
            Assert.IsTrue(curr.Find("MyArray") >= 0, "Double[] myArray should be in Q!");
            Assert.IsTrue(curr.Find("StrP") >= 0, "P StrP should be in Q!");
            Assert.IsTrue(curr.Find("F") >= 0, "float F should be in Q!");
            Assert.IsTrue(curr.Find("Zzz") >= 0, "long Zzz should be in Q!");
            Assert.IsTrue(curr.Find("Sss") >= 0, "string Sss should be in Q!");

            curr = oldCurr;
            curr = curr.GetChildAt(curr.Find("R")).GetTreeComposite();
            Assert.IsTrue(curr.Find("A") >= 0, "int A should be in R!");
            Assert.IsTrue(curr.Find("D") >= 0, "double D should be in R!");
            Assert.IsTrue(curr.Find("F") >= 0, "float F should be in R!");
            Assert.IsTrue(curr.Find("Str") >= 0, "string Str should be in R!");
            Assert.IsTrue(curr.Find("C") >= 0, "char C should be in R!");
            Assert.IsTrue(curr.Find("Field") >= 0, "M Field should be in R!");
            Assert.IsTrue(curr.Find("Dt") >= 0, "DateTime Dt should be in R!");
            Assert.IsTrue(curr.Find("Rand") >= 0, "Random Rand should be in R!");
        }

        [TestMethod]
        public void TestPropertiesCategory()
        {
            testSetup();
            TreeComposite curr = tree.GetTreeComposite();
            TreeComposite oldCurr = curr;

            COMPONENT_TYPE checkAgainstProperty = COMPONENT_TYPE.PROPERTY;

            curr = curr.GetChildAt(curr.Find("Q")).GetTreeComposite();
            Assert.AreEqual(curr.GetChildAt(curr.Find("MyArray")).ComponentType, checkAgainstProperty, "Double[] myArray should be a property!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("StrP")).ComponentType, checkAgainstProperty, "P StrP should be a property!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("F")).ComponentType, checkAgainstProperty, "float F should be a property!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("Zzz")).ComponentType, checkAgainstProperty, "long Zzz should be a property!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("Sss")).ComponentType, checkAgainstProperty, "string Sss should be a property!");

            curr = oldCurr;
            curr = curr.GetChildAt(curr.Find("R")).GetTreeComposite();
            Assert.AreEqual(curr.GetChildAt(curr.Find("A")).ComponentType, checkAgainstProperty, "int A should be a property!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("D")).ComponentType, checkAgainstProperty, "double D should be a property!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("F")).ComponentType, checkAgainstProperty, "float F should be a property!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("Str")).ComponentType, checkAgainstProperty, "string Str should be a property!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("C")).ComponentType, checkAgainstProperty, "char C should be a property!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("Field")).ComponentType, checkAgainstProperty, "M Field should be a property!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("Dt")).ComponentType, checkAgainstProperty, "DateTime Dt should be a property!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("Rand")).ComponentType, checkAgainstProperty, "Random Rand should be a property!");
        }

        [TestMethod]
        public void TestMethodsTypes()
        {
            testSetup();
            TreeComposite curr = tree.GetTreeComposite();
            TreeComposite oldCurr = curr;

            curr = curr.GetChildAt(curr.Find("S")).GetTreeComposite();
            Assert.IsTrue(curr.Find("MyArray") >= 0, "Method myArray should be in S!");
            Assert.IsTrue(curr.Find("StrP") >= 0, "Method StrP should be in S!");
            Assert.IsTrue(curr.Find("F") >= 0, "Method F should be in S!");
            Assert.IsTrue(curr.Find("Zzz") >= 0, "Method Zzz should be in S!");
            Assert.IsTrue(curr.Find("Sss") >= 0, "Method Sss should be in S!");

            curr = oldCurr;
            curr = curr.GetChildAt(curr.Find("T")).GetTreeComposite();
            Assert.IsTrue(curr.Find("A") >= 0, "Method A should be in T!");
            Assert.IsTrue(curr.Find("D") >= 0, "Method D should be in T!");
            Assert.IsTrue(curr.Find("F") >= 0, "Method F should be in T!");
            Assert.IsTrue(curr.Find("Str") >= 0, "Method Str should be in T!");
            Assert.IsTrue(curr.Find("C") >= 0, "Method C should be in T!");
            Assert.IsTrue(curr.Find("Field") >= 0, "Method Field should be in T!");
            Assert.IsTrue(curr.Find("Dt") >= 0, "Method Dt should be in T!");
            Assert.IsTrue(curr.Find("Rand") >= 0, "Method Rand should be in T!");
        }

        [TestMethod]
        public void TestMethodsCategory()
        {
            testSetup();
            TreeComposite curr = tree.GetTreeComposite();
            TreeComposite oldCurr = curr;

            COMPONENT_TYPE checkAgainstMethod = COMPONENT_TYPE.METHOD;

            curr = curr.GetChildAt(curr.Find("S")).GetTreeComposite();
            Assert.AreEqual(curr.GetChildAt(curr.Find("MyArray")).ComponentType, checkAgainstMethod, "Double[] myArray should be a method!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("StrP")).ComponentType, checkAgainstMethod, "P StrP should be a method!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("F")).ComponentType, checkAgainstMethod, "float F should be a method!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("Zzz")).ComponentType, checkAgainstMethod, "long Zzz should be a method!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("Sss")).ComponentType, checkAgainstMethod, "string Sss should be a method!");

            curr = oldCurr;
            curr = curr.GetChildAt(curr.Find("T")).GetTreeComposite();
            Assert.AreEqual(curr.GetChildAt(curr.Find("A")).ComponentType, checkAgainstMethod, "int A should be a method!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("D")).ComponentType, checkAgainstMethod, "double D should be a method!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("F")).ComponentType, checkAgainstMethod, "float F should be a method!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("Str")).ComponentType, checkAgainstMethod, "string Str should be a method!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("C")).ComponentType, checkAgainstMethod, "char C should be a method!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("Field")).ComponentType, checkAgainstMethod, "M Field should be a method!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("Dt")).ComponentType, checkAgainstMethod, "DateTime Dt should be a method!");
            Assert.AreEqual(curr.GetChildAt(curr.Find("Rand")).ComponentType, checkAgainstMethod, "Random Rand should be a method!");
        }

        [TestMethod]
        public void TestExtensionMethod()
        {
            testSetup();
            TreeComposite curr = tree.GetTreeComposite();
            curr = curr.GetChildAt(curr.Find("Extension")).GetTreeComposite();
            Assert.AreEqual(curr.Find("StringExtension"), -1, "Static class StringExtension is not supposed to exit!");
            Assert.AreEqual(curr.Find("String"), 0, "Class String is supposed to exit!");

            curr = curr.GetChildAt(curr.Find("String")).GetTreeComposite();
            Assert.AreEqual(curr.Find("CharCount"), 0, "Extension method CharCound is not found!");
        }

    }
}
