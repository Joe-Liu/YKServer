using Dapper;
using MyServer.Database.Attri;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyServer.Database
{
    public class IDB<T>
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();

        public string tableName { get; protected set; }
        public string Sql_QueryAll { get; protected set; }
        public string Sql_QueryByIdx { get; protected set; }
        public string Sql_QueryByKey { get; protected set; }
        public string Sql_Insert { get; protected set; }
        public string Sql_InsertOrUpdate { get; protected set; }
        public string Sql_Update { get; protected set; }
        public virtual void Init()
        {
            List<string> fileds = new List<string>();
            List<string> keyFileds = new List<string>();
            List<string> idxFileds = new List<string>();
            List<string> otherfileds = new List<string>();

            Type type = typeof(T);
            System.Reflection.MemberInfo info = type;
            object[] attributes = info.GetCustomAttributes(false);
            for (int i = 0; i < attributes.Length; i++)
            {
                if (attributes[i] is Table)
                {
                    tableName = ((Table)attributes[i]).Name;
                    break;
                }
            }
            bool isOther = true;
            foreach (var f in type.GetProperties())
            {
                string fName = f.Name;
                fileds.Add(fName);
                isOther = true;
                foreach (Attribute a in f.GetCustomAttributes(true))
                {
                    isOther = false;
                    if (a is Key)
                        keyFileds.Add(f.Name);
                    else if (a is Idx)
                        idxFileds.Add(f.Name);
                    else
                        isOther = true;
                }
                if (isOther)
                    otherfileds.Add(fName);
            }

            Sql_QueryAll = $@"SELECT * FROM {tableName}";

            StringBuilder one = new StringBuilder();
            StringBuilder two = new StringBuilder();
            int cnt = fileds.Count;
            for (int i = 0; i < cnt; i++)
            {
                one.Append(fileds[i]);
                two.Append("@");
                two.Append(fileds[i]);
                if (i < cnt - 1)
                {
                    one.Append(",");
                    two.Append(",");
                }
            }
            Sql_Insert = $@"INSERT INTO {tableName}({one.ToString()}) VALUES({two.ToString()})";

            StringBuilder three = new StringBuilder();
            cnt = otherfileds.Count;
            for (int i = 0; i < cnt; i++)
            {
                three.Append(otherfileds[i]);
                three.Append("=@");
                three.Append(otherfileds[i]);
                if (i < cnt - 1)
                    three.Append(", ");
            }
            Sql_InsertOrUpdate = $@"INSERT INTO {tableName}({one.ToString()}) VALUES({two.ToString()}) ON DUPLICATE KEY UPDATE {three.ToString()}";

            one.Clear();
            cnt = idxFileds.Count;
            for (int i = 0; i < cnt; i++)
            {
                one.Append(idxFileds[i]);
                one.Append("=@");
                one.Append(idxFileds[i]);
                if (i < cnt - 1)
                    one.Append(" AND ");
            }
            Sql_QueryByIdx = $@"SELECT * FROM {tableName} WHERE {one.ToString()}";


            one.Clear();
            cnt = keyFileds.Count;
            for (int i = 0; i < cnt; i++)
            {
                one.Append(keyFileds[i]);
                one.Append("=@");
                one.Append(keyFileds[i]);
                if (i < cnt - 1)
                    one.Append(" AND ");
            }
            Sql_QueryByKey = $@"SELECT * FROM {tableName} WHERE {one.ToString()}";


            two.Clear();
            cnt = otherfileds.Count;
            for (int i = 0; i < cnt; i++)
            {
                two.Append(otherfileds[i]);
                two.Append("=@");
                two.Append(otherfileds[i]);
                if (i < cnt - 1)
                    two.Append(" AND ");
            }
            Sql_Update = $@"UPDATE {tableName} SET {two.ToString()} WHERE {one.ToString()}";
        }

        protected void ShowAllSqls()
        {
            logger.Debug(tableName);
            logger.Debug(Sql_QueryAll);
            logger.Debug(Sql_Insert);
            logger.Debug(Sql_InsertOrUpdate);
            logger.Debug(Sql_QueryByIdx);
            logger.Debug(Sql_QueryByKey);
            logger.Debug(Sql_Update);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<T> QueryAll()
        {
            var conn = DBManager.instance.SqlConn;
            var result = conn.Query<T>(Sql_QueryAll).ToList();
            return result;
        }

        /// <summary>
        /// new { idx1 = value1, idx2 = value2, ...}
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<T> QueryByIdx(object param)
        {
            var conn = DBManager.instance.SqlConn;
            var result = conn.Query<T>(Sql_QueryByIdx, param).ToList();
            return result;
        }

        /// <summary>
        /// new { idx1 = value1, idx2 = value2, ...}
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<T> QueryByKey(object param)
        {
            var conn = DBManager.instance.SqlConn;
            var result = conn.Query<T>(Sql_QueryByIdx, param).ToList();
            return result;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public int Insert(T t)
        {
            var conn = DBManager.instance.SqlConn;
            var result = conn.Execute(Sql_Insert, t);
            return result;
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public int Update(T t)
        {
            var conn = DBManager.instance.SqlConn;
            var result = conn.Execute(Sql_Update, t);
            return result;
        }

        /// <summary>
        /// 有则更新，无则插入
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public int InsertOrUpdate(T t)
        {
            var conn = DBManager.instance.SqlConn;
            var result = conn.Execute(Sql_InsertOrUpdate, t);
            return result;
        }
    }
}
