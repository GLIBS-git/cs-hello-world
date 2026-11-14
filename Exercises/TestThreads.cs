using System.Collections.Concurrent;
using System.Diagnostics;

namespace Exercises
{
    public class TestThreads
    {
        public static void Starter()
        {
            // Uncomment the test you want to run:
            //TestAsync.Starter();
            //TestThread.TestMap();
            //new TestThread(30, 1000000).Execute();
            //new TestThread(30, 1).Execute_2();
            //new TestThread_2(90, 1000000).Execute();
            //TestTasks.Starter();
            TestThread_1.Start();
            //TestThreads_3.Start();
        }
    }
    public class TestThread_1
    {
        public static void Start()
        {
            Console.WriteLine("Main started.");
            TestThread_1.TestAsync();
            Console.WriteLine("Main finished.");
        }
        public static async void TestAsync()
        {
            Console.WriteLine("Thread started.");
            await Task.Delay(2000);
            //throw new Exception("Test exception!");
            Console.WriteLine("Thread finished.");
        }
    }
    public class TestAsync
    {
        public async static Task Starter()
        {
            Console.WriteLine($"Starting Starter.");
            await TestAsync.TestAsync_1(1, 3000);
            await TestAsync.TestAsync_1(2, 3000);
            //TestAsync.TestAsync_1(1, 3000);
            //TestAsync.TestAsync_1(2, 3000);
            //Task task_1 = TestAsync.TestAsync_1(1, 3000);
            //Task task_2 = TestAsync.TestAsync_1(2, 3000);
            //await task_1;
            //await task_2;
            Console.WriteLine($"Finishing Starter.");
            Thread.Sleep(5000);
        }
        public static async Task TestAsync_1(int _id, int _delay)
        {
            Console.WriteLine($"Starting async {_id}.");
            //Thread.Sleep(_delay);
            //await Task.Delay(_delay);
            //Task.Delay(_delay);
            Console.WriteLine($"Finishing async {_id}.");
        }
    }
    public class TestTasks
    {
        public static void Starter()
        {
            new TestTask(90, 1000000).Execute();
        }

        public class TestTask
        {
            protected ThreadBuffer threadBuffer;
            protected Task[] tasks;
            public TestTask(int _numberOfTasks, int _loops)
            {
                this.threadBuffer = new ThreadBuffer();
                this.threadBuffer.numberOfTasks = _numberOfTasks;
                this.threadBuffer.loops = _loops;
                this.tasks = new Task[_numberOfTasks];
            }

            public void Execute()
            {
                Stopwatch sw = Stopwatch.StartNew();
                for (int i = 0; i < this.threadBuffer.numberOfTasks; i++)
                {
                    TestTaskProcessor testTaskProcessor = new TestTaskProcessor(this.threadBuffer, i);
                    //tasks[i] = new Task(testTaskProcessor.Run);
                    //tasks[i].Start();
                    //tasks[i] = Task.Factory.StartNew(testTaskProcessor.Run);
                    tasks[i] = Task.Run(testTaskProcessor.Run);
                }
                //for (int i = 0; i < this.threadBuffer.numberOfTasks; i++)
                //{
                //    tasks[i].Wait();
                //}
                Task.WaitAll(tasks);
                Console.WriteLine($"Map elements: {this.threadBuffer.dict.Count}");
                foreach (KeyValuePair<int, int> _kvp in threadBuffer.dict)
                {
                    Console.WriteLine($"Key: {_kvp.Key}\n        Value: {_kvp.Value}");
                }
                Console.WriteLine($"Elapsed: {sw.Elapsed}");
            }
        }
        public class ThreadBuffer
        {
            public int numberOfTasks;
            public int threadCnt;
            public int loops;
            public ConcurrentDictionary<int, int> dict;
            public ThreadBuffer()
            {
                dict = new ConcurrentDictionary<int, int>();
            }
        }
        public class TestTaskProcessor
        {
            protected ThreadBuffer threadBuffer;
            protected int taskId;
            public TestTaskProcessor(ThreadBuffer _threadBuffer, int _taskId)
            {
                this.threadBuffer = _threadBuffer;
                this.taskId = _taskId;
            }
            public void Run()
            {
                Console.WriteLine($"Tread {this.taskId} started.");
                for (int i = 0; i < this.threadBuffer.loops; i++)
                {
                    this.threadBuffer.dict.AddOrUpdate(this.taskId, i, (_currentKey, _currentValue) => i);
                }
                Console.WriteLine($"Tread {this.taskId} finished.");
            }
        }
    }
    public class TestThread
    {
        TestThreadBuffer threadBuffer;
        Thread[] threads;
        public TestThread(int _threadCnt, int _loops)
        {
            this.threadBuffer = new TestThreadBuffer();
            threadBuffer.threadCnt = _threadCnt;
            threadBuffer.loops = _loops;
            threads = new Thread[_threadCnt];
        }
        public static void TestMap()
        {
            Stopwatch sw = Stopwatch.StartNew();
            Dictionary<int, string> dict = new Dictionary<int, string>();
            Console.WriteLine($"{dict.Count}");
            if (!dict.ContainsKey(1))
            {
                dict.Add(1, "Test!");
            }
            else
            {
                dict[1] = "Test 2!";
            }
            Console.WriteLine($"{dict[1]}");
            dict[1] = "Test 3!";
            Console.WriteLine($"{dict.Count}");
            Console.WriteLine($"{dict[1]}");
            Console.WriteLine($"{sw.Elapsed}");
        }
        public void Execute()
        {
            for (int i = 0; i < threadBuffer.threadCnt; i++)
            {
                TestThreadProcessor threadProcessor = new TestThreadProcessor(this.threadBuffer, i);
                threads[i] = new Thread(threadProcessor.Run);
                threads[i].Start();
            }
            //Thread.Sleep(5000);
            for (int i = 0; i < threadBuffer.threadCnt; i++)
            {
                threads[i].Join();
            }
            Console.WriteLine($"Map elements: {this.threadBuffer.dict.Count}");

        }
        public class TestThreadBuffer
        {
            public int threadCnt;
            public int loops;
            public Object objLocker;
            public Object objLocker_2;
            public Lock locker;
            public Mutex mutex;
            public Semaphore semaphore;
            public Dictionary<int, int> dict;
            public volatile int i1;
            public Stopwatch sw;
            public TestThreadBuffer()
            {
                this.objLocker = new Object();
                this.objLocker_2 = new Object();
                this.locker = new Lock();
                this.mutex = new Mutex();
                this.semaphore = new Semaphore(1, 1);
                this.dict = new Dictionary<int, int>();
            }
        }
        public class TestThreadProcessor
        {
            TestThreadBuffer threadBuffer;
            public int threadId;
            public TestThreadProcessor(TestThreadBuffer _threadBuffer, int _threadId)
            {
                this.threadBuffer = _threadBuffer;
                this.threadId = _threadId;
            }
            public void Run()
            {
                Stopwatch sw = Stopwatch.StartNew();
                Console.WriteLine($"Tread {this.threadId} started.");
                for (int i = 0; i < this.threadBuffer.loops; i++)
                {
                    //lock (this.threadBuffer.objLocker) // Fast!
                    //{
                    //    this.DoRun(i);
                    //}

                    //using (this.threadBuffer.locker.EnterScope()) // Fast!
                    //{
                    //    this.DoRun(i);
                    //}

                    //this.threadBuffer.locker.EnterScope(); // Fast!
                    //{
                    //    this.DoRun(i);
                    //}
                    //this.threadBuffer.locker.Exit();

                    //this.threadBuffer.mutex.WaitOne(); // Slow!
                    //{
                    //    this.DoRun(i);
                    //}
                    //this.threadBuffer.mutex.ReleaseMutex();

                    //this.threadBuffer.semaphore.WaitOne(); // Slow!
                    //{
                    //    this.DoRun(i);
                    //}
                    //this.threadBuffer.semaphore.Release();

                    //bool locked = false; // Fast!
                    //Monitor.Enter(this.threadBuffer.objLocker, ref locked);
                    //if (locked)
                    //{
                    //    Monitor.Exit(this.threadBuffer.objLocker);
                    //}

                    lock (this.threadBuffer.objLocker)
                    {
                        Monitor.Wait(this.threadBuffer.objLocker);
                        this.DoRun(i);
                        //Monitor.PulseAll(this.threadBuffer.objLocker);
                    }

                }
                Console.WriteLine($"Tread {this.threadId} finished.");
                Console.WriteLine($"Elapsed: {sw.Elapsed}");
            }
            public void DoRun(int _i)
            {
                if (!this.threadBuffer.dict.ContainsKey(this.threadId))
                {
                    this.threadBuffer.dict.Add(this.threadId, _i);
                }
                else
                {
                    this.threadBuffer.dict[this.threadId] = _i;
                }
            }
        }

        public void Execute_2()
        {
            threadBuffer.sw = Stopwatch.StartNew();
            for (int i = 0; i < threadBuffer.threadCnt; i++)
            {
                TestThreadProcessor_2 threadProcessor = new TestThreadProcessor_2(this.threadBuffer, i);
                threads[i] = new Thread(threadProcessor.Run);
                threads[i].Start();
            }
            Thread.Sleep(5000);
            Console.WriteLine($"");
            Console.WriteLine($"");
            lock (this.threadBuffer.objLocker)
            {
                Monitor.Pulse(this.threadBuffer.objLocker);
            }
            for (int i = 0; i < threadBuffer.threadCnt; i++)
            {
                threads[i].Join();
            }
            Console.WriteLine($"Map elements: {this.threadBuffer.dict.Count} at {threadBuffer.sw.Elapsed}");
            threadBuffer.sw.Stop();

        }

        public class TestThreadProcessor_2
        {
            TestThreadBuffer threadBuffer;
            public int threadId;
            public TestThreadProcessor_2(TestThreadBuffer _threadBuffer, int _threadId)
            {
                this.threadBuffer = _threadBuffer;
                this.threadId = _threadId;
            }
            public void Run()
            {
                Console.WriteLine($"Tread {this.threadId} started at {threadBuffer.sw.Elapsed}.");
                for (int i = 0; i < this.threadBuffer.loops; i++)
                {
                    lock (this.threadBuffer.objLocker)
                    {
                        Console.WriteLine($"Tread {this.threadId} going on hold at {threadBuffer.sw.Elapsed}.");
                        Monitor.Wait(this.threadBuffer.objLocker);
                        Console.WriteLine($"Tread {this.threadId} waked up at {threadBuffer.sw.Elapsed}.");
                        Thread.Sleep(1000);
                        Monitor.Pulse(this.threadBuffer.objLocker);
                    }

                }
                Console.WriteLine($"        Tread {this.threadId} finished at {threadBuffer.sw.Elapsed}.");
            }
        }
    }
    public class TestThread_2
    {
        public TestThreadBuffer threadBuffer;
        public Thread[] threads;
        public TestThread_2(int _threadCnt, int _loops)
        {
            this.threadBuffer = new TestThreadBuffer();
            threadBuffer.threadCnt = _threadCnt;
            threadBuffer.loops = _loops;
            threads = new Thread[_threadCnt];
        }
        public void Execute()
        {
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < threadBuffer.threadCnt; i++)
            {
                TestThreadProcessor threadProcessor = new TestThreadProcessor(this.threadBuffer, i);
                threads[i] = new Thread(threadProcessor.Run);
                threads[i].Start();
            }
            for (int i = 0; i < threadBuffer.threadCnt; i++)
            {
                threads[i].Join();
            }
            Console.WriteLine($"Map elements: {this.threadBuffer.dict.Count}");
            foreach (KeyValuePair<int, int> _kvp in threadBuffer.dict)
            {
                Console.WriteLine($"Key: {_kvp.Key}\n        Value: {_kvp.Value}");
            }
            Console.WriteLine($"Elapsed: {sw.Elapsed}");
        }
        public class TestThreadBuffer
        {
            public int threadCnt;
            public int loops;
            public Object objLocker;
            public Object objLocker_2;
            public Lock locker;
            public Mutex mutex;
            public Semaphore semaphore;
            public ConcurrentDictionary<int, int> dict;
            public volatile int i1;
            public Stopwatch sw;
            public TestThreadBuffer()
            {
                this.objLocker = new Object();
                this.objLocker_2 = new Object();
                this.locker = new Lock();
                this.mutex = new Mutex();
                this.semaphore = new Semaphore(1, 1);
                this.dict = new ConcurrentDictionary<int, int>();
            }
        }
        public class TestThreadProcessor
        {
            TestThreadBuffer threadBuffer;
            public int threadId;
            public TestThreadProcessor(TestThreadBuffer _threadBuffer, int _threadId)
            {
                this.threadBuffer = _threadBuffer;
                this.threadId = _threadId;
            }
            public void Run()
            {
                Console.WriteLine($"Tread {this.threadId} started.");
                for (int i = 0; i < this.threadBuffer.loops; i++)
                {
                    //this.threadBuffer.dict.AddOrUpdate(this.threadId, i, (_currentKey, _currentValue) => i);
                    this.threadBuffer.dict.AddOrUpdate(this.threadId, i, (_currentKey, _currentValue) => ++_currentValue); // Summing
                    //if (!this.threadBuffer.dict.ContainsKey(this.threadId)) // Standard way for non concurrent
                    //{
                    //    if (!this.threadBuffer.dict.TryAdd(this.threadId, i))
                    //    {
                    //        Console.WriteLine($"Error adding!");
                    //    }
                    //}
                    //else
                    //{
                    //    this.threadBuffer.dict[this.threadId] = i;
                    //}
                }
                Console.WriteLine($"Tread {this.threadId} finished.");
            }
        }
    }
    public class TestThreads_3
    {
        public static void Start()
        {
            Console.WriteLine($"Starting Starter.");
            ThreadRunner threadRunner_1 = new ThreadRunner(1, 3000);
            Thread thread_1 = new Thread(threadRunner_1.Run);
            thread_1.Start();
            ThreadRunner threadRunner_2 = new ThreadRunner(2, 2000);
            Thread thread_2 = new Thread(threadRunner_2.Run);
            thread_2.Start();
            ThreadRunner threadRunner_3 = new ThreadRunner(3, 1000);
            Thread thread_3 = new Thread(threadRunner_3.Run);
            thread_3.Start();
            thread_3.Join();
            Console.WriteLine($"Join 3 passed.");
            thread_2.Join();
            Console.WriteLine($"Join 2 passed.");
            thread_1.Join();
            Console.WriteLine($"Join 1 passed.");
            Console.WriteLine($"Finishing Starter.");
        }
        public class ThreadRunner
        {
            int delay;
            int id;
            public ThreadRunner(int _id, int _delay)
            {
                this.id = _id;
                this.delay = _delay;
            }
            public void Run()
            {
                Console.WriteLine($"Starting thread {this.id}.");
                Thread.Sleep(this.delay);
                Console.WriteLine($"Finishing thread {this.id}.");
            }
        }
    }
}
