using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace OS_3._4.FIFO_Planner
{
    class ThreadSample
    {
        /// <summary>
        /// Поток, который бужет выполнять работу
        /// </summary>
        Thread thrd;
        /// <summary>
        /// До какого числа считаем
        /// </summary>
        long goal;
        /// <summary>
        /// Экземпляр генератора случайных чисел
        /// </summary>
        static Random random=new Random();

        /// <summary>
        /// Текущий счетчик
        /// </summary>
        long count;

        /// <summary>
        /// Минимальное и максимальное значения, до которых может считать поток
        /// </summary>
        const long minValue = 10000000;
        const long maxValue = 100000000;

        /// <summary>
        /// Метод, вызываемый при завершении работы потока
        /// </summary>
        Action callback;

        bool stopped;
        public ThreadSample(string name, Action callback)
        {
            goal = RandomRangeLong(minValue, maxValue);//Случайным образом выбираем цель
            thrd = new Thread(new ThreadStart(this.Run));
            thrd.Name = name;
            //thrd.IsBackground = true;
            count = 0;
            this.callback = callback;
        }
        /// <summary>
        /// Начало выполнения работы потока
        /// </summary>
        public void Start()
        {
            if (thrd != null && thrd.ThreadState == ThreadState.Unstarted) //Если поток не запущен, запускаем его
                thrd.Start();
        }
        /// <summary>
        /// Приостановка работы потока
        /// </summary>
        public void Suspend()
        {
            try
            {
                if (thrd.ThreadState == ThreadState.Running || thrd.ThreadState == ThreadState.WaitSleepJoin) //Если поток работает, приостанавливаем его
                    thrd.Suspend();
            }
            catch{}
        }

        /// <summary>
        /// Запуск приостаовленного потока
        /// </summary>
        public void Resume()
        {
            if (thrd.ThreadState == ThreadState.Suspended) //Если поток приостановлен
                thrd.Resume();
            else
                if (thrd.ThreadState == ThreadState.Unstarted) //Иначе если его еще не запустили
                    thrd.Start();
        }

        /// <summary>
        /// Остановка выполнения потока
        /// </summary>
        public void Stop()
        {
            stopped = true;
        }

        /// <summary>
        /// Симуляция работы потока
        /// </summary>
        private void Run()
        {
            stopped = false;
            while (count<goal && !stopped)
                ++count;
     //       callback(); //Если отработали, будим вызывавший поток
        }

        /// <summary>
        /// Генерирует случайное число для типа long
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static long RandomRangeLong(long min, long max)
        {
            long result = random.Next((Int32)(min >> 32), (Int32)(max >> 32)); //Генерируем первое число типа Int
            result = (result << 32); //записываем его в первые 32 бита
            result = result | (long)random.Next((Int32)min, (Int32)max); //Генерируем второе число и записываем его в последние 32 бита
            return result;
        }

        /// <summary>
        /// Свойство для получения текущего состояния счетчика
        /// </summary>
        public long GetCurrentCount
        {
            get { return count; }
        }

        /// <summary>
        /// Свойство для получения цели потока
        /// </summary>
        public long GetMaxCount
        {
            get { return goal; }
        }

        /// <summary>
        /// Свойство для получения имени потока
        /// </summary>
        public string GetName
        {
            get { return thrd.Name; }
        }

        public ThreadState State
        {
            get { return thrd.ThreadState; }
        }
    }
}
