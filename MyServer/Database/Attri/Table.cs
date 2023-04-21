using System;

namespace MyServer.Database.Attri
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class Table : Attribute
    {
        private string name;
        private bool autoCreate;

        public Table(string name, bool autoCreate = false)
        {
            this.name = name;
            this.autoCreate = autoCreate;
        }

        public string Name { get => name; set => name = value; }
        public bool AutoCreate { get => autoCreate; set => autoCreate = value; }
    }
}
