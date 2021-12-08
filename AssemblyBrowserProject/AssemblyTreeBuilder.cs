using System;
using System.Reflection;
using System.Runtime.CompilerServices;



namespace AssemblyBrowserProject
{

    public class AssemblyTreeBuilder
    {
        BindingFlags ALL_FLAG = BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance;

        public AssemblyTreeBuilder(String dllPath)
        {
            this.dllPath = dllPath;
        }

        public void Build()
        {
            root = new NamespaceTreeComposite("root", COMPONENT_TYPE.NAMESPACE);
            Assembly asm = Assembly.LoadFrom(dllPath);
            FillTreeByNamespaces(asm);
            FillTreeByTypes(asm);
        }

        private String[] GetSplittedNamespace(String namespaceStr)
        {
            return namespaceStr?.Split('.');
        }

        private void AddNamespaceNodes(String[] domains, int curr, TreeComponent comp)
        {
            if (domains == null) return;
            if (curr == domains.Length) return;
            int index = comp.Find(domains[curr]);
            if (index == -1)
            {
                comp.Add(new NamespaceTreeComposite(domains[curr], COMPONENT_TYPE.NAMESPACE));
                AddNamespaceNodes(domains, curr + 1, comp.GetLastChild());
            }
            else
            {
                AddNamespaceNodes(domains, curr + 1, comp.GetChildAt(index));
            }
        }

        private void FillTreeByNamespaces(Assembly asm)
        {
            foreach (Type type in asm.GetTypes())
            {
                String[] subSpaces = GetSplittedNamespace(type.Namespace);
                AddNamespaceNodes(subSpaces, 0, root);
            }
        }

        private TreeComponent GetDeclaringTreeNode(String[] domains)
        {
            if (domains == null) return root;
            TreeComponent curr = root;
            foreach (string s in domains){
                curr = curr.GetChildAt(curr.Find(s));
            }
            return curr;
        }

        private void ProcessExtensionClass(Type extClass)
        {
            TreeComponent parent = GetDeclaringTreeNode(GetSplittedNamespace(extClass.Namespace));
            foreach (MethodInfo m in extClass.GetMethods(ALL_FLAG))
            {
                ParameterInfo[] p = m.GetParameters();
                if (m.IsDefined(typeof(ExtensionAttribute), true))
                {
                    TreeComponent createdClass = TreeComponent.CreateTreeComponent(p[0].ParameterType, true);
                    TreeComponent cmp = parent.Add(createdClass);

                    TreeComponent methodComponent = TreeComponent.CreateTreeComponent(m);
                    (methodComponent as MethodTreeLeaf).IsExtensionMethod = true;

                    cmp.Add(methodComponent);
                }
            }
        }

        private void FillTreeByTypes(Assembly asm)
        {
            foreach (var type in asm.GetTypes())
            {
                TreeComponent created = TreeComponent.CreateTreeComponent(type, true);

                if (created == null && type.IsAbstract && type.IsSealed)
                {
                    ProcessExtensionClass(type);
                } else if (created != null)
                {
                    TreeComponent parent = GetDeclaringTreeNode(GetSplittedNamespace(type.Namespace));
                    TreeComponent newlyInserted = parent.Add(created);

                    AddTypeConstructors(type.GetConstructors(ALL_FLAG), newlyInserted);
                    AddTypeProperties(type.GetProperties(ALL_FLAG), newlyInserted);
                    AddTypeFields(type.GetFields(ALL_FLAG), newlyInserted);
                    AddTypeMethods(type.GetMethods(ALL_FLAG), newlyInserted);
                    AddTypeNodes(type, newlyInserted);
                }
            }
        }

        private void AddTypeMethods(MethodInfo[] info, TreeComponent cmp)
        {
            foreach (MethodInfo m in info)
            {
                cmp.Add(TreeComponent.CreateTreeComponent(m));
            }
        }

        private void AddTypeFields(FieldInfo[] info, TreeComponent cmp)
        {
            foreach (FieldInfo f in info){
                cmp.Add(TreeComponent.CreateTreeComponent(f));
            }
        }

        private void AddTypeConstructors(ConstructorInfo[] info, TreeComponent cmp)
        {
            foreach (ConstructorInfo c in info)
            {
                cmp.Add(TreeComponent.CreateTreeComponent(c));
            }
        }

        private void AddTypeProperties(PropertyInfo[] info, TreeComponent cmp)
        {
            foreach (PropertyInfo p in info)
            {
                cmp.Add(TreeComponent.CreateTreeComponent(p));
            }
        }

        private void AddTypeNodes(Type type, TreeComponent node)
        {
            foreach (var nestedType in type.GetTypeInfo()
                                           .DeclaredNestedTypes)
            {
                TreeComponent cmp = null;
                cmp = node.Add(TreeComponent.CreateTreeComponent(nestedType, false));

                AddTypeConstructors(nestedType.GetConstructors(ALL_FLAG), cmp);
                AddTypeProperties(nestedType.GetProperties(ALL_FLAG), cmp);
                AddTypeFields(nestedType.GetFields(ALL_FLAG), cmp);
                AddTypeMethods(nestedType.GetMethods(ALL_FLAG), cmp);

                AddTypeNodes(nestedType, cmp);
            }
        }

        public TreeComponent GetRoot() { return root; }

        private TreeComponent root;
        private String dllPath;
    }


}
