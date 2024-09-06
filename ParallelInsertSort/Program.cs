using System;
using System.Diagnostics;
using System.Threading.Tasks;
class Program
{
    static void Main()
    {
        // Считывание размерности массива для сортировки и его объявление
        Console.WriteLine("Введите число элементов массива:\n");
        int n = Convert.ToInt32(Console.ReadLine());
        int[] nums = new int[n];
        // Создание объекта класса Stopwatch для замера скоросты выполнения программы
        Stopwatch stopWatch = new Stopwatch();
        Console.WriteLine("Введите способ сортировки (0 - последовательный, 1 - параллельный):\n");
        if (Console.ReadLine() == "0")
        {
            // Заполнение массива случайными числами
            ArrayFill(nums);
            Console.WriteLine("Исходный массив:\n");
            // Вывод в консоль массива до сортировки
            PrintArray(nums);
            // Начало отсчета времени
            stopWatch.Start();
            // Сортировка массива
            InsertSort(nums, 0, nums.Length);
            // Конец отсчета времени
            stopWatch.Stop();
            // Определение формата времени
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("Отсортированный массив:\n");
            // Вывод в консоль массива после сортировки
            PrintArray(nums);
            Console.WriteLine("Время выполнения: " + elapsedTime + "\n");
        }
        else
        {
            // Заполнение массива случайными числами заново
            ArrayFill(nums);
            Console.WriteLine("Исходный массив:\n");
            // Вывод в консоль массива до сортировки
            PrintArray(nums);
            // Начало отсчета времени
            stopWatch.Start();
            // Сортировка массива
            ParallelInsertSort(nums);
            // Конец отсчета времени
            stopWatch.Stop();
            // Определение формата времени
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            Console.WriteLine("Отсортированный параллельно массив:\n");
            // Вывод в консоль массива после сортировки
            PrintArray(nums);
            Console.WriteLine("Время выполнения: " + elapsedTime + "\n");
        }
        Console.ReadKey();
    }

    static void ArrayFill(int[] array) // Метод заполнения массива случайными числами от 10 до 1000
    {
        Random random = new Random();
        int n = array.Length;
        for (int i = 0; i < n; i++)
        {
            array[i] = random.Next(10, 1000);
        }
    }

    static void PrintArray(int[] array) // Метод вывода массива в консоль
    {
        foreach (var item in array)
        {
            Console.Write(item + " ");
        }
        Console.WriteLine("\n");
    }

    static void ParallelInsertSort(int[] array)
    {
        int N = array.Length;
        int numThreads = Environment.ProcessorCount; // Количество потоков
        int chunkSize = N / numThreads; // Размер подмассива

        // Параллельная сортировка подмассивов
        Parallel.For(0, numThreads, t =>
        {
            int start = t * chunkSize;
            int end = (t == numThreads - 1) ? N : start + chunkSize;
            InsertSort(array, start, end);
        });

        // Объединение отсортированных подмассивов
        MergeSortedChunks(array, chunkSize, numThreads);
    }

    static void InsertSort(int[] array, int start, int end) // Классическая сортировка вставками
    {
        for (int i = start + 1; i < end; i++)
        {
            int value = array[i];
            int j = i - 1;
            while (j >= start && array[j] > value)
            {
                array[j + 1] = array[j];
                j--;
            }
            array[j + 1] = value;
        }
    }

    static void MergeSortedChunks(int[] array, int chunkSize, int numThreads) // Объединение подмассивов
    {
        int N = array.Length;
        for (int size = chunkSize; size < N; size *= 2)
        {
            for (int i = 0; i < N; i += 2 * size)
            {
                int mid = Math.Min(i + size - 1, N - 1);
                int end = Math.Min(i + 2 * size - 1, N - 1);
                Merge(array, i, mid, end);
            }
        }
    }

    static void Merge(int[] array, int start, int mid, int end) // Сборка по элементам подмассивов
    {
        int[] temp = new int[end - start + 1];
        int i = start, j = mid + 1, k = 0;

        while (i <= mid && j <= end)
        {
            if (array[i] <= array[j])
            {
                temp[k++] = array[i++];
            }
            else
            {
                temp[k++] = array[j++];
            }
        }

        while (i <= mid)
        {
            temp[k++] = array[i++];
        }

        while (j <= end)
        {
            temp[k++] = array[j++];
        }

        Array.Copy(temp, 0, array, start, temp.Length);
    }
}