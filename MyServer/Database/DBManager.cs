using MyServer.Database.Base;
using MyServer.Utils;
using MySql.Data.MySqlClient;
using NLog;
using System;
using System.Collections.Generic;

namespace MyServer.Database
{
    public class DBManager : Singleton<DBManager>
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public string conString = "server=192.168.4.185;port=3307;user=root;password=Playmore123!;database=test";

        /// <summary>
        /// 数据库的读写管理可以改为多线程的
        /// 每个线程分配一个SqlConnection
        /// </summary>
        public MySqlConnection SqlConn { get; private set; }

        /// <summary>
        /// 组件的方式自动收集Cache对象
        /// Cache对象有静态配置类的和动态数据类的
        /// </summary>
        /// 
        private Dictionary<Type, ICache> caches;

        public override void Init()
        {
            this.caches = ClassUtil.Fetch<Dictionary<Type, ICache>>();
            //收集所有的Cache对象
            foreach (var type in ClassUtil.GetClasses("MyServer.Database.Cache"))
            {
                AddCache(type);
            }

            //初始化所有的Cache对象
            foreach(var cache in this.caches)
            {
                cache.Value.Init();
            }
        }

        public override void Exit()
        {
            //flush
            //执行所有缓存对象的Exit
            foreach (var cache in this.caches)
            {
                cache.Value.Exit();
            }
            //close sql connection
            if (SqlConn != null)
                SqlConn.Close();
        }

        public override void Start()
        {
            SqlConn = new MySqlConnection(conString);
            try
            {
                SqlConn.Open();
            }catch(Exception ex)
            {
                logger.Error(ex);
                return;
            }

            //执行所有缓存对象的缓存
            foreach (var cache in this.caches)
            {
                cache.Value.Start();
            }
        }

        public override void Load()
        {
        }


        public ICache AddCache(Type type)
        {
            if (this.caches != null && this.caches.ContainsKey(type))
                throw new Exception($"entity already has cache: {type.FullName}");

            ICache cache = Activator.CreateInstance(type) as ICache;
            //cache.Parent = this;
            this.caches.Add(type, cache);
            return cache;
        }

        /// <summary>
        /// 添加Cache对象
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <returns></returns>
        public K AddCache<K>() where K : ICache
        {
            Type type = typeof(K);
            if (this.caches != null && this.caches.ContainsKey(type))
                throw new Exception($"entity already has cache: {type.FullName}");

            ICache cache = Activator.CreateInstance(type) as ICache;
            //cache.Parent = this;
            this.caches.Add(type, cache);
            return cache as K;
        }

        /// <summary>
        /// 获取Cache对象
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <returns></returns>
        public K GetCache<K>() where K : ICache
        {
            if (this.caches == null)
                return null;
            ICache cache;
            if (!this.caches.TryGetValue(typeof(K), out cache))
                return default;
            return (K)cache;
        }
    }
}
