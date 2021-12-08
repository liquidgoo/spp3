using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AssemblyBrowserProject
{

    public enum ACCESS_MODIFIER
    {
        PUBLIC,
        PRIVATE,
        PROTECTED,
        INTERNAL,
        PROTECTED_INTERNAL,
        PRIVATE_PROTECTED
    }

    public enum COMPONENT_TYPE
    {
        FIELD,
        PROPERTY,
        METHOD,
        NAMESPACE,
        CLASS,
        STRUCT
    }

    public class TreeComponent
    {
        internal static String GetAccessModifierString(ACCESS_MODIFIER accMod)
        {
            switch (accMod)
            {
                case ACCESS_MODIFIER.PUBLIC: return "public";
                case ACCESS_MODIFIER.PRIVATE: return "private";
                case ACCESS_MODIFIER.PROTECTED: return "protected";
                case ACCESS_MODIFIER.INTERNAL: return "internal";
                case ACCESS_MODIFIER.PROTECTED_INTERNAL: return "protected internal";
                case ACCESS_MODIFIER.PRIVATE_PROTECTED: return "private protected";
            }
            return "";
        }

        public TreeComponent(String name, COMPONENT_TYPE componentType)
        {
            Name = name;
            ComponentType = componentType;
        }

        public virtual TreeComponent Add(TreeComponent cmp) { throw new NotImplementedException(); }
        public virtual void Remove(TreeComponent cmp) { throw new NotImplementedException(); }
        public virtual TreeComponent GetChildAt(int index) { throw new NotImplementedException(); }
        public virtual int Find(String name) { throw new NotImplementedException(); }
        public virtual TreeComponent GetLastChild() { throw new NotImplementedException(); }

        public static TreeComponent CreateTreeComponent(ConstructorInfo info)
        {
            ParameterInfo[] p = info.GetParameters();
            if (info.IsPublic)
            {
                return new MethodTreeLeaf(info.Name, COMPONENT_TYPE.METHOD, ACCESS_MODIFIER.PUBLIC, null, p);
            } else if (info.IsPrivate)
            {
                return new MethodTreeLeaf(info.Name, COMPONENT_TYPE.METHOD, ACCESS_MODIFIER.PRIVATE, null, p);
            }else if (info.IsFamilyOrAssembly)
            {
                return new MethodTreeLeaf(info.Name, COMPONENT_TYPE.METHOD, ACCESS_MODIFIER.PROTECTED_INTERNAL, null, p);
            }
            else if (info.IsFamilyAndAssembly)
            {
                return new MethodTreeLeaf(info.Name, COMPONENT_TYPE.METHOD, ACCESS_MODIFIER.PRIVATE_PROTECTED, null, p);
            }
            else if (info.IsFamily)
            {
                return new MethodTreeLeaf(info.Name, COMPONENT_TYPE.METHOD, ACCESS_MODIFIER.PROTECTED, null, p);
            }
            else
            {
                return new MethodTreeLeaf(info.Name, COMPONENT_TYPE.METHOD, ACCESS_MODIFIER.PRIVATE, null, p);
            }
        }

        private static ACCESS_MODIFIER GetAccessModifierFromMethod(MethodInfo m)
        {
            if (m.IsPublic) return ACCESS_MODIFIER.PUBLIC;
            if (m.IsPrivate) return ACCESS_MODIFIER.PRIVATE;
            if (m.IsFamilyOrAssembly) return ACCESS_MODIFIER.PROTECTED_INTERNAL;
            if (m.IsFamilyAndAssembly) return ACCESS_MODIFIER.PRIVATE_PROTECTED;
            if (m.IsFamily) return ACCESS_MODIFIER.PROTECTED;
            return ACCESS_MODIFIER.PRIVATE;
        }

        private static ACCESS_MODIFIER GetAccessModifierFromField(FieldInfo m)
        {
            if (m.IsPublic) return ACCESS_MODIFIER.PUBLIC;
            if (m.IsPrivate) return ACCESS_MODIFIER.PRIVATE;
            if (m.IsFamilyOrAssembly) return ACCESS_MODIFIER.PROTECTED_INTERNAL;
            if (m.IsFamilyAndAssembly) return ACCESS_MODIFIER.PRIVATE_PROTECTED;
            if (m.IsFamily) return ACCESS_MODIFIER.PROTECTED;
            return ACCESS_MODIFIER.PRIVATE;
        }

        public static TreeComponent CreateTreeComponent(MethodInfo info)
        {
            Type returnType = info.ReturnType;
            ACCESS_MODIFIER methodAccessModifier = GetAccessModifierFromMethod(info);
            return new MethodTreeLeaf(info.Name, COMPONENT_TYPE.METHOD, methodAccessModifier, returnType, info.GetParameters());
        }

        public static TreeComponent CreateTreeComponent(FieldInfo info)
        {
            Type usedType = info.FieldType;
            ACCESS_MODIFIER fieldAccessModifier = GetAccessModifierFromField(info);
            return new FieldTreeLeaf(info.Name, COMPONENT_TYPE.FIELD, fieldAccessModifier, usedType);
        }

        public static TreeComponent CreateTreeComponent(PropertyInfo info)
        {
            Type usedType = info.PropertyType;
            ACCESS_MODIFIER setAccessModifier = GetAccessModifierFromMethod(info.SetMethod);
            ACCESS_MODIFIER getAccessModifier = GetAccessModifierFromMethod(info.GetMethod);
            return new PropertyTreeLeaf(info.Name, COMPONENT_TYPE.PROPERTY, setAccessModifier, getAccessModifier, usedType);
        }

        public static TreeComponent CreateTreeComponent(Type type, bool ignoreNested)
        {
            if (ignoreNested && type.IsNested) return null;
            if (type.IsEnum || type.IsInterface ||
                    type.IsAbstract || type.IsGenericParameter ||
                    type.IsGenericType || type.IsGenericTypeDefinition) return null;
            
            if (type.IsNested)
            {
                if (type.IsClass)
                {
                    if (type.IsNestedPrivate)
                    {
                        return new ClassTreeComposite(type.Name, COMPONENT_TYPE.CLASS, ACCESS_MODIFIER.PRIVATE);
                    }
                    else if (type.IsPublic)
                    {
                        return new ClassTreeComposite(type.Name, COMPONENT_TYPE.CLASS, ACCESS_MODIFIER.PUBLIC);
                    }
                    else
                    {
                        return new ClassTreeComposite(type.Name, COMPONENT_TYPE.CLASS, ACCESS_MODIFIER.PROTECTED);
                    }
                }
                else if (type.IsValueType)
                {
                    if (type.IsNestedPrivate)
                    {
                        return new StructTreeComposite(type.Name, COMPONENT_TYPE.STRUCT, ACCESS_MODIFIER.PRIVATE);
                    }
                    else if (type.IsPublic)
                    {
                        return new StructTreeComposite(type.Name, COMPONENT_TYPE.STRUCT, ACCESS_MODIFIER.PUBLIC);
                    }
                    else
                    {
                        return new StructTreeComposite(type.Name, COMPONENT_TYPE.STRUCT, ACCESS_MODIFIER.PROTECTED);
                    }
                }

            } else
            {
                if (type.IsClass)
                {
                    if (type.IsPublic)
                    {
                        return new ClassTreeComposite(type.Name, COMPONENT_TYPE.CLASS, ACCESS_MODIFIER.PUBLIC);
                    }
                    else
                    {
                        return new ClassTreeComposite(type.Name, COMPONENT_TYPE.CLASS, ACCESS_MODIFIER.INTERNAL);
                    }
                }
                else if (type.IsValueType)
                {
                    if (type.IsPublic)
                    {
                        return new StructTreeComposite(type.Name, COMPONENT_TYPE.STRUCT, ACCESS_MODIFIER.PUBLIC);
                    }
                    else
                    {
                        return new StructTreeComposite(type.Name, COMPONENT_TYPE.STRUCT, ACCESS_MODIFIER.INTERNAL);
                    }
                }
            }

            return null;
        }

        public String Name { get; set; }
        public COMPONENT_TYPE ComponentType { get; set; }
        public virtual TreeComposite GetTreeComposite() { return null; }
    }

    class TreeLeaf : TreeComponent
    {
        public TreeLeaf(String name, COMPONENT_TYPE componentType, Type type) :
            base(name, componentType) { UsedType = type; }

        public Type UsedType { get; set; }
    }

    class MethodTreeLeaf : TreeLeaf
    {
        public MethodTreeLeaf(String name, COMPONENT_TYPE componentType, ACCESS_MODIFIER accMod, Type returnType, ParameterInfo[] pInfo) :
            base(name, componentType, returnType)
        { AccessModifier = accMod; this.pInfo = pInfo; }

        private string GetParametersString()
        {
            StringBuilder result = new StringBuilder("");
            for (int i = 0; i < pInfo.Length; ++i)
            {
                result.Append(pInfo[i].ParameterType.Name);
                result.Append(" ");
                result.Append(pInfo[i].Name);
                if (i == pInfo.Length - 1) break;
                result.Append(", ");
            }
            return result.ToString();
        }

        public override string ToString()
        {
            return GetAccessModifierString(AccessModifier) + " " + UsedType?.Name + " " + Name + "(" + GetParametersString() + ")" +
                (IsExtensionMethod ? " *Extension" : "");
        }

        public ParameterInfo[] pInfo;
        public ACCESS_MODIFIER AccessModifier { get; set; }
        public Boolean IsExtensionMethod { get; set; } = false;
    }

    class PropertyTreeLeaf : TreeLeaf
    {
        public PropertyTreeLeaf(String name, COMPONENT_TYPE componentType, ACCESS_MODIFIER setAccMod, ACCESS_MODIFIER getAccMod, Type propertyType) :
            base(name, componentType, propertyType)
        {
            SetAccessModifier = setAccMod;
            GetAccessModifier = getAccMod;
        }

        public override string ToString()
        {
            return UsedType.Name + " " + Name + 
                "{" + GetAccessModifierString(GetAccessModifier)+ " get; " 
                + GetAccessModifierString(SetAccessModifier) + " set;}";
        }

        public ACCESS_MODIFIER SetAccessModifier { get; set; }
        public ACCESS_MODIFIER GetAccessModifier { get; set; }
    }

    class FieldTreeLeaf : TreeLeaf
    {
        public FieldTreeLeaf(String name, COMPONENT_TYPE componentType, ACCESS_MODIFIER accMod, Type fieldType) :
            base(name, componentType, fieldType)
        { AccessModifier = accMod; }

        public override string ToString()
        {
            return GetAccessModifierString(AccessModifier) + " " + UsedType.Name + " " + Name;
        }

        public ACCESS_MODIFIER AccessModifier { get; set; }
    }

    public class TreeComposite : TreeComponent
    {
        public override TreeComponent Add(TreeComponent cmp) { components.Add(cmp); return components.Last(); }
        public override void Remove(TreeComponent cmp) { components.Remove(cmp); }
        public override TreeComponent GetChildAt(int index) { return components.ElementAt(index); }
        public override int Find(string name)
        {
            return components.FindIndex(x => x.Name.Equals(name));
        }
        public override TreeComponent GetLastChild()
        {
            return components.Last();
        }
        public override TreeComposite GetTreeComposite() { return this; }

        public List<TreeComponent> components = new List<TreeComponent>();

        public TreeComposite(String name, COMPONENT_TYPE componentType) : base(name, componentType) { }
    }

    class NamespaceTreeComposite : TreeComposite
    {
        public NamespaceTreeComposite(String name, COMPONENT_TYPE componentType) : base(name, componentType) { }

        public override string ToString()
        {
            return "namespace " + Name;
        }
    }

    class StructTreeComposite : TreeComposite
    {
        public StructTreeComposite(String name, COMPONENT_TYPE componentType, ACCESS_MODIFIER accMod) : base(name, componentType)
        {
            AccessModifier = accMod;
        }

        public override string ToString()
        {
            return GetAccessModifierString(AccessModifier) + " struct " + Name;
        }

        public ACCESS_MODIFIER AccessModifier { get; set; }
    }

    class ClassTreeComposite : TreeComposite
    {
        public ClassTreeComposite(String name, COMPONENT_TYPE componentType, ACCESS_MODIFIER accMod) : base(name, componentType)
        {
            AccessModifier = accMod;
        }

        public ACCESS_MODIFIER AccessModifier { get; set; }

        public override string ToString()
        {
            return GetAccessModifierString(AccessModifier) + " class " + Name;
        }

    }
}

    
