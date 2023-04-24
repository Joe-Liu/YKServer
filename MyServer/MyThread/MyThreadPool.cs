using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyServer.MyThread
{
    /// <summary>
    /// 需要一个线程池
    /// 指定线程数量
    /// 指定线程执行对应任务
    /// </summary>
    public class MyThreadPool
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();

        public delegate void MyThreadAction(object state);
        public int maxThreads { get; protected set; }
        public MyThreadState[] threadStates;

        public MyThreadPool(int maxThreads, MyThreadAction threadAction)
        {
            this.maxThreads = maxThreads;
            threadStates = new MyThreadState[maxThreads];

            for (int i = 0; i < maxThreads; i++)
            {
                threadStates[i] = new MyThreadState();
                threadStates[i].Start(threadAction);
            }
        }

        public void Enqueue(int threadIdx, object state)
        {
            threadStates[threadIdx].Enqueue(state);
        }

        public void Dispose()
        {
            for (int i = 0; i < maxThreads; i++)
            {
                threadStates[i].Dispose();
            }
        }

        public class MyThreadState
        {
            private Thread _thread;
            private ConcurrentQueue<object> _stateQueue;
            private bool _isDisposed = false;
            private int _threadId;
            private bool _threadStopFlg = false;

            public MyThreadState()
            {
                _thread = new Thread(ThreadProc);
                _stateQueue = new ConcurrentQueue<object>();
                _threadId = _thread.ManagedThreadId;
                logger.Info($"Thread {_threadId} Create!");
            }

            public void Start(object parameter)
            {
                _thread.Start(parameter);
            }

            public void Dispose()
            {
                if (_isDisposed)
                    return;
                _threadStopFlg = true;
                _thread?.Join();
                _stateQueue = null;
                _thread = null;
                _isDisposed = true;
                logger.Info($"Thread {_threadId} Disposed!");
            }

            public void Enqueue(object state)
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException("MyThreadState has been disposed.");
                }
                _stateQueue.Enqueue(state);
            }

            private void ThreadProc(object param)
            {
                MyThreadAction _threadAction = (MyThreadAction)param;
                while (true)
                {
                    if (_stateQueue.TryDequeue(out object state))
                        _threadAction?.Invoke(state);
                    else
                        Thread.Sleep(1);

                    if (_threadStopFlg) break;
                }
            }
        }
    }
}
