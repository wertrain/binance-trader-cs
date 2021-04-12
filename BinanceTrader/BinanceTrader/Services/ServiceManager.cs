using BinanceTrader.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinanceTrader.Services
{
    /// <summary>
    /// サービス管理クラス
    /// </summary>
    public class ServiceManager
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static ServiceManager Instance { get; } = new ServiceManager();

        /// <summary>
        /// すべてのサービス
        /// </summary>
        public List<ServiceBase> Services { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private ServiceManager()
        {
            Services = new List<ServiceBase>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CreateService<T>() where T : ServiceBase, new()
        {
            var service = Services.Find(s => s is T);
            if (service != null) return null;

            service = new T();
            if (service.Create())
            {
                Services.Add(service);
                return (T)service;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public T CreateService<T>(ServiceBase.SetupParameter param) where T : ServiceBase, new()
        {
            var service = Services.Find(s => s is T);
            if (service != null) return null;

            service = new T();
            if (service.Create(param))
            {
                Services.Add(service);
                return (T)service;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveService<T>()
        {
            var service = Services.Find(s => s is T);
            if (service == null) return;

            service.Destroy();
            Services.Remove(service);
        }

        /// <summary>
        /// サービスを取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>() where T : ServiceBase
        {
            return Services.Find(s => s is T) as T;
        }
    }
}
