using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Toolkit.Services.DataLoader
{
    public interface IDataLoaderService
    {
        public static IDataLoaderService Implement => new DataLoaderService();

        /// <summary>
        /// 设置同步加载器
        /// </summary>
        /// <param name="loadKey">加载键</param>
        /// <param name="loader">加载器</param>
        /// <param name="destoryAfterLoad">在加载完毕后是否销毁</param>
        void SetupLoader(string loadKey, Action loader, bool destoryAfterLoad = true);

        /// <summary>
        /// 设置异步加载器
        /// </summary>
        /// <param name="loadKey">加载键</param>
        /// <param name="asyncLoader">异步加载器</param>
        /// <param name="destoryAfterLoad">在加载完毕后是否销毁</param>
        void SetupAsyncLoader(string loadKey, Func<CancellationToken, Task> asyncLoader, bool destoryAfterLoad = true);

        /// <summary>
        /// 开始进行同步加载任务
        /// </summary>
        /// <param name="loadKey">加载键</param>
        void StartLoad(string loadKey);

        /// <summary>
        /// 使用异步方式进行异步加载任务
        /// </summary>
        /// <param name="loadKey">加载键</param>
        /// <param name="cancellationToken">取消口令</param>
        /// <returns></returns>
        Task StartAsyncLoadAsync(string loadKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取是否已经被加载了
        /// </summary>
        /// <param name="loadKey">加载键</param>
        /// <returns>如果键不存在，则默认返回 false</returns>
        bool IsLoaded(string loadKey);
    }
}
