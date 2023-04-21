using System;

namespace MyServer.Database.Attri
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Key : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Idx : Attribute
    {

    }
}
