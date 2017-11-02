using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    /// <summary>
    /// Steam游戏封面下载器
    /// </summary>
    internal class SteamCoverDownloader
    {
        #region classes
        private enum DownloadItemStates
        {
            Waiting,
            Downloading,
            Successed,
            Failed,
            Cancelled
        }

        private class DownloadItem
        {
            public DownloadItem(Int64 appID, bool enableRetry)
            {
                AppID = appID;
                EnableRetry = enableRetry;
                State = DownloadItemStates.Waiting;
            }

            public Int64 AppID { get; private set; }
            public DownloadItemStates State { get; set; }
            public bool EnableRetry { get; private set; }
        }

        private struct DownloadResult
        {
            public DownloadResult(string smallPath, string normalPath)
            {
                SmallPath = smallPath;
                NormalPath = normalPath;
            }
            public string SmallPath { get; private set; }
            public string NormalPath { get; private set; }
        }
        #endregion

        public SteamCoverDownloader()
        {
            _syncObj = new object();
            _retryTimer = new Timer(RetryTimerProc);
            _items = new List<DownloadItem>();
        }

        /// <summary>
        /// 如果下载失败了则定时重试，定时30秒
        /// </summary>
        private const Int32 RetryTick = 30000;

        /// <summary>
        /// 同步对象
        /// </summary>
        private readonly object _syncObj;
        /// <summary>
        /// 如果下载失败了则定时重试所需要的定时器
        /// </summary>
        private Timer _retryTimer;
        /// <summary>
        /// 下载项集合
        /// </summary>
        private List<DownloadItem> _items;
        /// <summary>
        /// 取消Token
        /// </summary>
        private CancellationTokenSource _currentCts;

        /// <summary>
        /// 游戏封面下载完成事件
        /// </summary>
        public event EventHandler<SteamCoverDownloadedArgs> Downloaded;
        private void OnDownloaded(Int64 appID, string smallPath, string normalPath, SteamCoverDownloadResultStates state)
        {
            var args = new SteamCoverDownloadedArgs(appID, smallPath, normalPath, state);
            Volatile.Read(ref Downloaded)?.Invoke(this, args);
        }

        /// <summary>
        /// 下载指定游戏封面
        /// </summary>
        /// <param name="appID">游戏ID</param>
        /// <param name="enableRetry">失败后是否自动重试</param>
        public void Download(Int64 appID, bool enableRetry)
        {
            lock (_syncObj)
            {
                // 已经添加到了下载集合里
                if (_items.Any(i => i.AppID == appID)) return;

                _items.Add(new DownloadItem(appID, enableRetry));
            }

            DownloadItems();
        }

        /// <summary>
        /// 取消下载游戏封面
        /// </summary>
        /// <param name="appID">游戏ID</param>
        public void Cancel(Int64 appID)
        {
            lock (_syncObj)
            {
                var item = _items.FirstOrDefault(i => i.AppID == appID);
                if (item != null)
                {
                    _items.Remove(item);

                    if (item.State == DownloadItemStates.Downloading) _currentCts?.Cancel();
                }
            }
        }

        /// <summary>
        /// 取消所有游戏封面下载
        /// </summary>
        public void CancelAll()
        {
            lock (_syncObj)
            {
                _items.Clear();
                _currentCts?.Cancel();
            }
        }

        /// <summary>
        /// 下载重试计时器处理函数
        /// </summary>
        /// <param name="state">备用参数，没用</param>
        private void RetryTimerProc(object state)
        {
            DownloadItems();
        }

        /// <summary>
        /// 按队列下载游戏封面
        /// </summary>
        private async void DownloadItems()
        {
            CancellationTokenSource cts;
            lock (_syncObj)
            {
                // 避免重复下载（确保同时只有一个线程执行DownloadItems）
                if (_currentCts != null) return;
                // 设置取消Token
                cts = _currentCts = new CancellationTokenSource();
                // 确保解除重试定时器，避免浪费定时器资源
                _retryTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }

            while (true)
            {
                DownloadItem item = null;
                lock (_syncObj)
                {
                    // 从下载集合中取出一个等待项
                    item = _items.FirstOrDefault(i => i.State == DownloadItemStates.Waiting);
                    // 没有需要处理的等待项了，退出循环
                    if (item == null) break;
                    // 如果取消Token在下载上一个项时被取消过，则重新创建（CancellationTokenSource是不能复用的）
                    if (cts.IsCancellationRequested)
                        cts = _currentCts = new CancellationTokenSource();
                    // 设置下载项状态为正在下载
                    item.State = DownloadItemStates.Downloading;
                }

                DownloadResult result = new DownloadResult(null, null);
                try
                {
                    result = await DownloadForAppID(item.AppID, cts.Token);

                    lock (_syncObj)
                    {
                        item.State = DownloadItemStates.Successed;
                        // 成功后重下载集合中移除
                        _items.Remove(item);
                    }
                }
                catch (Exception)
                {
                    if (cts.IsCancellationRequested)
                    {
                        // 取消下载，不需用同步锁，因为这个下载项已经被从集合移除了
                        item.State = DownloadItemStates.Cancelled;
                        // 不用移除，因为在调用Cancel的时候已经移除了
                        // _items.Remove(item);
                    }
                    else
                    {
                        lock (_syncObj)
                        {
                            // 下载失败
                            item.State = DownloadItemStates.Failed;

                            // 不需要重试的项直接移除
                            if (!item.EnableRetry) _items.Remove(item);
                        }
                    }
                }

                if (item.State == DownloadItemStates.Successed)
                    OnDownloaded(item.AppID, result.SmallPath, result.NormalPath, SteamCoverDownloadResultStates.Successed);
                else if (item.State == DownloadItemStates.Cancelled)
                    OnDownloaded(item.AppID, null, null, SteamCoverDownloadResultStates.Cancelled);
                else if (item.State == DownloadItemStates.Failed && !item.EnableRetry)
                    OnDownloaded(item.AppID, null, null, SteamCoverDownloadResultStates.Failed);
            }// while true

            lock (_syncObj)
            {
                // 重置失败项为等待状态
                foreach (var item in _items)
                {
                    if (item.State == DownloadItemStates.Failed && item.EnableRetry)
                        item.State = DownloadItemStates.Waiting;
                }

                // 清理取消Token，保证下次执行DownloadItems时能通过重复过滤条件
                if (cts != null && !cts.IsCancellationRequested)
                    cts.Dispose();

                _currentCts = null;

                // 设置重试定时
                if (_items.Count > 0)
                    _retryTimer.Change(RetryTick, Timeout.Infinite);
            }
        }

        /// <summary>
        /// 下载指定游戏封面
        /// </summary>
        /// <param name="appID">游戏ID</param>
        /// <returns></returns>
        private async Task<DownloadResult> DownloadForAppID(Int64 appID, CancellationToken ct)
        {
            HttpClient client = new HttpClient();
            long t = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
            string smallUrl = $"http://cdn.steamstatic.com.8686c.com/steam/apps/{appID}/header_292x136.jpg?t={t}";
            string normalUrl = $"http://cdn.steamstatic.com.8686c.com/steam/apps/{appID}/header.jpg?t={t}";
            string smallTmpPath, normalTmpPath;
            // 注册取消操作
            ct.Register(() => client.CancelPendingRequests());
            // 获取小图
            using (var stream = await client.GetStreamAsync(smallUrl))
            {
                string tmpPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
                using (var fileStream = new FileStream(tmpPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    await stream.CopyToAsync(fileStream, 81920, ct);
                }

                smallTmpPath = tmpPath;
            }
            // 获取普通大小封面
            using (var stream = await client.GetStreamAsync(normalUrl))
            {
                string tmpPath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
                using (var fileStream = new FileStream(tmpPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    await stream.CopyToAsync(fileStream, 81920, ct);
                }

                normalTmpPath = tmpPath;
            }

            string coverFolder = SteamLibraryEnviroment.GetGameCoverFolder(true);// 确保文件夹存在
            string smallPath = SteamLibraryEnviroment.GetGameSmallCoverPath(appID);
            string normalPath = SteamLibraryEnviroment.GetGameNormalCoverPath(appID);
            File.Copy(smallTmpPath, smallPath, true);
            File.Copy(normalTmpPath, normalPath, true);

            return new DownloadResult(smallPath, normalPath);
        }
    }
}
