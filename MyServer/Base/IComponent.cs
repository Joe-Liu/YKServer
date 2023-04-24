using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Base
{
    /// <summary>
    /// 组件基类
    /// </summary>
    public class IComponent
    {
        protected Dictionary<Type, IComponent> components;

        public IComponent Parent { set; get; }

        /// <summary>
        /// 添加一个组件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IComponent AddComponent(Type type)
        {
            if (this.components == null)
                this.components = new Dictionary<Type, IComponent>();
            if (this.components.ContainsKey(type))
                throw new Exception($"already has component: {type.FullName}");

            IComponent component = Activator.CreateInstance(type) as IComponent;
            component.Parent = this;
            component.Init();
            this.components.Add(type, component);
            return component;
        }

        /// <summary>
        /// 添加一个组件
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <returns></returns>
        public K AddComponent<K>() where K : IComponent
        {
            Type type = typeof(K);
            if (this.components == null)
                this.components = new Dictionary<Type, IComponent>();
            if (this.components.ContainsKey(type))
                throw new Exception($"already has component: {type.FullName}");

            IComponent component = Activator.CreateInstance(type) as IComponent;
            component.Parent = this;
            component.Init();
            this.components.Add(type, component);
            return component as K;
        }

        /// <summary>
        /// 获取一个组件
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <returns></returns>
        public K GetComponent<K>() where K : IComponent
        {
            if (this.components == null)
                return null;
            IComponent component;
            if (!this.components.TryGetValue(typeof(K), out component))
                return default;
            return (K)component;
        }

        protected virtual void Init() { }
    }
}
