using System;

namespace MyServer.Utils
{
    public abstract class Singleton<T> where T : class, new()
    {
        private static T m_instance;
        public static T instance
        {
            get
            {
                if (Singleton<T>.m_instance == null)
                {
                    Singleton<T>.m_instance = Activator.CreateInstance<T>();
                    if (Singleton<T>.m_instance != null)
                    {
                        (Singleton<T>.m_instance as Singleton<T>).Init();
                    }
                }

                return Singleton<T>.m_instance;
            }
        }

        public static void Release()
        {
            if (Singleton<T>.m_instance != null)
            {
                Singleton<T>.m_instance = (T)((object)null);
            }
        }

        public virtual void Init()
        {

        }

        /// <summary>
        /// 执行简单的初始化
        /// </summary>
        public abstract void Load();

        /// <summary>
        /// 执行启动服务的操作
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// 执行关闭服务的操作
        /// </summary>
        public abstract void Exit();

    }
}
