using System;

namespace MyServer.Server.Attri
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class Http : Attribute
    {
        private string path;

        public Http(string path)
        {
            this.Path = path;
        }

        public string Path { get => path; set => path = value; }
    }
}
