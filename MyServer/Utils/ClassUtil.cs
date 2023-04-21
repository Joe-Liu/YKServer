using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MyServer.Utils
{
    public static class ClassUtil
    {
        public static IEnumerable<string> GetClasseNames(string nameSpace)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            return asm.GetTypes()
                .Where(type => type.Namespace == nameSpace)
                .Select(type => type.Name);
        }

        public static IEnumerable<Type> GetClasses(string nameSpace)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            return asm.GetTypes()
                .Where(type => type.Namespace == nameSpace);
        }

        public static T Fetch<T>()
        {
            return (T)Fetch(typeof(T));
        }

        public static object Fetch(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}
