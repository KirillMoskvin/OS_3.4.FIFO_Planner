using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace OS_3._4.FIFO_Planner
{
    class FIFOPlanner
    {
        /// <summary>
        /// Потоки, работу которых организует планировщик
        /// </summary>
        Queue<ThreadSample> threads;

        /// <summary>
        /// исполняемый поток
        /// </summary>
        ThreadSample currentThread;
        /// <summary>
        /// Сколько потоков было создано к настоящему моменту
        /// </summary>
        int count;

        /// <summary>
        /// Поток, в котором запускается планировщик
        /// </summary>
        Thread thrd;
        /// <summary>
        /// Лог состояния планировщика
        /// </summary>
        TextBox textBox;

        /// <summary>
        /// Константы для определения квантов времени, выделяемых потоку
        /// </summary>
        const int minQuantum = 10;
        const int maxQuantum = 200;
        /// <summary>
        /// Квант времени, выделяемый потоком
        /// </summary>
        int quantum;

        /// <summary>
        /// Вероятность создания нового потока на текущей итерации
        /// </summary>
        const int newThreadProbability = 10;

        static Random random = new Random();

        /// <summary>
        /// Признак остановки потока
        /// </summary>
        bool stopped;
        /// <summary>
        /// Конструктор создает новый объект класса FIFOPlanner
        /// </summary>
        /// <param name="startNum">Изначальное количество потоков в очереди</param>
        public FIFOPlanner(int startNum, TextBox tb) 
        {
            threads = new Queue<ThreadSample>();
            count = 0;
            textBox = tb;
            for (int i = 0; i < startNum; ++i) //создаем изначальные потоки и добавляем их в очередь
                CreateNewThread();
            thrd = null;
        }

        /// <summary>
        /// Метод раюоты потока
        /// </summary>
        public void Run()
        {
            lock(this)
            {
                stopped = false;
                while(!stopped)
                {
                    try
                    {
                        quantum = random.Next(minQuantum, maxQuantum); //Генерируем время, которыое мы выделим очередному потоку
                        if (threads.Count == 0 || random.Next(0, 100) < newThreadProbability) //Если очередь пуста, либо случайным образом создаем новый поток
                            CreateNewThread();
                        currentThread = threads.Dequeue();//достаем из очереди тот поток, чья очередь работать
                        currentThread.Resume();
                        Thread.Sleep(quantum); //Даём потоку совершить работу
                        
                    }
                    catch(ThreadInterruptedException ex)
                    { }
                    finally
                    {
                        currentThread.Suspend();
                        if (!stopped)
                        {
                            try
                            {
                                textBox.Invoke(new Action(() => textBox.AppendText("Поток " + currentThread.GetName + " досчитал до " + currentThread.GetCurrentCount +
                                    " (из " + currentThread.GetMaxCount + ")" + "\r\n")));
                                if (currentThread.GetCurrentCount == currentThread.GetMaxCount)//если поток завершил работу
                                    textBox.Invoke(new Action(() => textBox.AppendText("Поток " + currentThread.GetName + " завершил работу\r\n")));
                                else
                                    threads.Enqueue(currentThread);//недоработавший поток ставим в очередь на выполнение
                            }
                            catch
                            { }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Создает новый экземпляр потока
        /// </summary>
        public void CreateNewThread()
        {
            ++count;
            ThreadSample ts = new ThreadSample(count.ToString(), CallbackMethod);
            threads.Enqueue(ts);
            textBox.Invoke(new Action(() => textBox.AppendText("Создан поток " + count.ToString() + "\r\n")));
            
        }

        /// <summary>
        /// Запуск потока планировщика
        /// </summary>
        public void Start()
        {
            if (thrd == null)
            {
                thrd = new Thread(new ThreadStart(Run));
              //  thrd.IsBackground = true;
                thrd.Start();
            }
            
        }

        /// <summary>
        /// Приостановка потока планировщика
        /// </summary>
        public void Pause()
        {
            if (thrd.ThreadState == ThreadState.Running || thrd.ThreadState==ThreadState.WaitSleepJoin)
                thrd.Suspend();
            currentThread.Suspend();
        }
        /// <summary>
        /// Возобновление работы планировщика
        /// </summary>
        public void Resume()
        {
            currentThread.Resume();
            if (thrd.ThreadState == ThreadState.Suspended)
                thrd.Resume();
        }

        /// <summary>
        /// остановка планировщика
        /// </summary>
        public void Stop()
        {
            stopped = true;
            Thread.Sleep(5);
            foreach (ThreadSample thread in threads)
            {
                if (thread.State == ThreadState.Suspended)
                    thread.Resume();
                thread.Stop();
            }
            Thread.Sleep(5);
            threads.Clear();
            Thread.Sleep(5);
            textBox.Clear();
            count = 0;

        }
        /// <summary>
        /// Метод, который будет вызывать дочерний поток при завершении работы
        /// </summary>
        public void CallbackMethod()
        {
       //     if (thrd.ThreadState!=ThreadState.Stopped)
                try
                {
       //             if (thrd.ThreadState == ThreadState.WaitSleepJoin)
                        thrd.Interrupt();
                }
                catch(ThreadInterruptedException)
                {
                    thrd.Resume();
                }
        }
    }
}
