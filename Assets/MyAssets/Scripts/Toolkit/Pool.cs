using System.Collections.Generic;
using UnityEngine;

namespace MonsterSurvivors
{
    /// <summary>
    /// 泛型对象池 —— 复用 GameObject 上的 Component
    /// </summary>
    public class Pool<T> where T : Component
    {
        private T prefab;   //预制体
        private Transform container;
        private Queue<T> available = new Queue<T>();//可用对象队列
        private List<T> allObjects = new List<T>(); //所有对象列表
        
        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="prefab">要复制的预制体</param>
        /// <param name="initialCount">预先创建多少个（0 也行，按需创建）</param>
        /// <param name="containerName">Hierarchy 中的分组名称</param>
        public Pool(T prefab,int initialCount = 0,string containerName = "Pool",bool persistent = false)
        {
            this.prefab = prefab;
            GameObject containerObj = new GameObject(containerName);
            if (persistent) Object.DontDestroyOnLoad(containerObj);
            container = containerObj.transform;

            //预创建
            for(int i = 0;i < initialCount;i++)
            {
                available.Enqueue(CreateNew());
            }
        }

        /// <summary>
        /// 从池中获取一个对象（自动激活）
        /// </summary>
        public T Get()
        {
            T obj;
            // 如果没有空闲的，现场创建一个
            if (available.Count == 0)
            {
                obj = CreateNew();
            }
            else
            {
                obj = available.Dequeue();
            }

            obj.gameObject.SetActive(true);
            return obj;
        }

        /// <summary>
        /// 将对象放回池中（自动禁用）
        /// </summary>
        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(container);
            available.Enqueue(obj);
        }

        /// <summary>
        /// 回收所有正在使用的对象
        /// </summary>
        public void ReturnAll()
        {
            foreach(T obj in allObjects)
            {
                if(obj.gameObject.activeSelf)
                {
                   obj.gameObject.SetActive(false);
                   obj.transform.SetParent(container);
                   available.Enqueue(obj);
                }
            }
        }

        /// <summary>
        /// 创建一个新对象并加入池
        /// </summary>
        private T CreateNew()
        {
            T obj = Object.Instantiate(prefab, container);
            obj.gameObject.SetActive(false);   // 初始隐藏
            obj.name = prefab.name;            // 去掉 (Clone) 后缀
            allObjects.Add(obj);
            return obj;
        }

        /// <summary>
        /// 当前空闲数量
        /// </summary>
        public int AvailableCount => available.Count;

        /// <summary>
        /// 已经创建的总数
        /// </summary>
        public int TotalCount => allObjects.Count;

    }
}
