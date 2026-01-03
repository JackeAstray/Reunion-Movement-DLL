using System;
using System.Collections.Generic;

namespace ReunionMovementDLL
{
    /// <summary>
    /// 游戏框架入口。
    /// </summary>
    public static class ReunionMovementEntry
    {
        private static readonly ReunionMovementLinkedList<ReunionMovementModule> reunionMovementModules = new ReunionMovementLinkedList<ReunionMovementModule>();

        /// <summary>
        /// 所有游戏框架模块轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public static void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (ReunionMovementModule module in reunionMovementModules)
            {
                module.Update(elapseSeconds, realElapseSeconds);
            }
        }

        /// <summary>
        /// 关闭并清理所有游戏框架模块。
        /// </summary>
        public static void Shutdown()
        {
            for (LinkedListNode<ReunionMovementModule> current = reunionMovementModules.Last; current != null; current = current.Previous)
            {
                current.Value.Shutdown();
            }

            reunionMovementModules.Clear();
            ReferencePool.ClearAll();
            Utility.Marshal.FreeCachedHGlobal();
            ReunionMovementLog.SetLogHelper(null);
        }

        /// <summary>
        /// 获取游戏框架模块。
        /// </summary>
        /// <typeparam name="T">要获取的游戏框架模块类型。</typeparam>
        /// <returns>要获取的游戏框架模块。</returns>
        /// <remarks>如果要获取的游戏框架模块不存在，则自动创建该游戏框架模块。</remarks>
        public static T GetModule<T>() where T : class
        {
            Type interfaceType = typeof(T);
            if (!interfaceType.IsInterface)
            {
                throw new ReunionMovementException(Utility.Text.Format("必须通过接口获取模块，但 '{0}' 不是接口。", interfaceType.FullName));
            }

            if (!interfaceType.FullName.StartsWith("ReunionMovement.", StringComparison.Ordinal))
            {
                throw new ReunionMovementException(Utility.Text.Format("必须获取游戏框架模块，但 '{0}' 不是游戏框架模块。", interfaceType.FullName));
            }

            string moduleName = Utility.Text.Format("{0}.{1}", interfaceType.Namespace, interfaceType.Name.Substring(1));
            Type moduleType = Type.GetType(moduleName);
            if (moduleType == null)
            {
                throw new ReunionMovementException(Utility.Text.Format("无法找到游戏框架模块类型 '{0}'。", moduleName));
            }

            return GetModule(moduleType) as T;
        }

        /// <summary>
        /// 获取游戏框架模块。
        /// </summary>
        /// <param name="moduleType">要获取的游戏框架模块类型。</param>
        /// <returns>要获取的游戏框架模块。</returns>
        /// <remarks>如果要获取的游戏框架模块不存在，则自动创建该游戏框架模块。</remarks>
        private static ReunionMovementModule GetModule(Type moduleType)
        {
            foreach (ReunionMovementModule module in reunionMovementModules)
            {
                if (module.GetType() == moduleType)
                {
                    return module;
                }
            }

            return CreateModule(moduleType);
        }

        /// <summary>
        /// 创建游戏框架模块。
        /// </summary>
        /// <param name="moduleType">要创建的游戏框架模块类型。</param>
        /// <returns>要创建的游戏框架模块。</returns>
        private static ReunionMovementModule CreateModule(Type moduleType)
        {
            ReunionMovementModule module = (ReunionMovementModule)Activator.CreateInstance(moduleType);
            if (module == null)
            {
                throw new ReunionMovementException(Utility.Text.Format("无法创建模块 '{0}'。", moduleType.FullName));
            }

            LinkedListNode<ReunionMovementModule> current = reunionMovementModules.First;
            while (current != null)
            {
                if (module.Priority > current.Value.Priority)
                {
                    break;
                }

                current = current.Next;
            }

            if (current != null)
            {
                reunionMovementModules.AddBefore(current, module);
            }
            else
            {
                reunionMovementModules.AddLast(module);
            }

            return module;
        }
    }
}
