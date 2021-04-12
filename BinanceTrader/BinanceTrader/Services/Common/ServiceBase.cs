using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinanceTrader.Services.Common
{
    /// <summary>
    /// サービスの基底クラス
    /// このクラスを継承して作成したクラスは Service を通して管理
    /// </summary>
    public class ServiceBase
    {
        /// <summary>
        /// セットアップパラメータ
        /// </summary>
        public class SetupParameter
        {

        }

        /// <summary>
        /// 作成される際に呼び出される
        /// </summary>
        /// <returns></returns>
        public virtual bool Create(SetupParameter param)
        {
            return true;
        }


        /// <summary>
        /// 作成される際に呼び出される
        /// </summary>
        /// <returns></returns>
        public virtual bool Create()
        {
            return true;
        }

        /// <summary>
        /// 破棄される前に呼び出される
        /// </summary>
        public virtual void Destroy()
        {

        }
    }
}
