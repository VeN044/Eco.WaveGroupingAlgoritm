using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using static Program;

class Program
{
    static void Main(string[] args)
    {

        int worldXSize = 128;
        int worldYSize = 32;
        bool dontPrintMemory = (worldXSize > 129)? true : false ;

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // Создание объекта world 
        World world = new World(worldXSize, worldYSize);

        // Заполнение объекта memory случайными значениями
        world.Randomize();

        //заполним нижнюю строку world единицами
        world.CreateBedrock();

        stopwatch.Stop();

        // Вывод содержимого memory  
        Console.WriteLine("Исходный world:");
        if (!dontPrintMemory) PrintMemoryInBlocks(world.memory.Span, worldXSize);

        Console.WriteLine("Мир " + worldXSize + "x" + worldYSize + "=" + (worldXSize * worldYSize) + " готов за " + stopwatch.ElapsedMilliseconds + "мс. Нажмите любую кнопку для начала расчета...");
        Console.ReadKey();

        stopwatch.Reset();
        stopwatch.Start();

        //инициализируем и запустим алгоритм
        WaveAlgorithm algorithm = new WaveAlgorithm(worldXSize, worldYSize);
        algorithm.CalculateMask(world);

        Console.WriteLine("Маска рассчитана за " + stopwatch.ElapsedMilliseconds + "мс. \n");

        // Вывод содержимого mask  
        Console.WriteLine("Измененная mask:");
        if (!dontPrintMemory) PrintMemoryInBlocks(algorithm.mask.Span, worldXSize);

        // Вывод содержимого memory блоками 
        Console.WriteLine("world через mask:");
        if (!dontPrintMemory) PrintMemoryThroughtMask(world.memory.Span, algorithm.mask.Span, worldXSize, false);

        // Вывод содержимого memory блоками 
        Console.WriteLine("world через анти-mask:");
        if (!dontPrintMemory) PrintMemoryThroughtMask(world.memory.Span, algorithm.mask.Span, worldXSize, true);

        Console.WriteLine("Нажмите любую кнопку для завершения программы.");
        Console.ReadKey();

    }

    static void PrintMemoryThroughtMask(ReadOnlySpan<bool> data, ReadOnlySpan<bool> mask, int blockSize, bool invertmask = false)
    {
        for (int i = 0; i < data.Length; i += blockSize)
        {
            var block = data.Slice(i, Math.Min(blockSize, data.Length - i));
            var maskBlock = mask.Slice(i, Math.Min(blockSize, data.Length - i));
            var binaryString = new StringBuilder();
            for (int a = 0; a < block.Length; a++)
            {
                bool value = invertmask ? (block[a] && !maskBlock[a]) : (block[a] && maskBlock[a]);
                binaryString.Append(value ? "#" : " ");
            }
            Console.Write(binaryString.ToString());
            Console.WriteLine(); // Добавлен перевод строки между блоками
        }
    }

    static void PrintMemoryInBlocks(ReadOnlySpan<bool> data, int blockSize)
    {
        for (int i = 0; i < data.Length; i += blockSize)
        {
            var block = data.Slice(i, Math.Min(blockSize, data.Length - i));
            PrintBlock(block);
            Console.WriteLine(); // Добавлен перевод строки между блоками
        }
    }

    static void PrintBlock(ReadOnlySpan<bool> block)
    {
        var binaryString = new StringBuilder();
        for (int i = 0; i < block.Length; i++)
        {
            binaryString.Append(block[i] ? "#" : " ");
        }
        Console.Write(binaryString.ToString());
    }

    public class World
    {
        public bool[] worldMemoryArray;
        public Memory<bool> memory;

        public int worldSizeX;
        public int worldSizeY;
        public int worldSize;

        public World(int sizeX, int sizeY)
        {
            this.worldSizeX = sizeX;
            this.worldSizeY = sizeY;
            this.worldSize = worldSizeX * worldSizeY;

            this.worldMemoryArray = new bool[worldSize];
            this.memory = new Memory<bool>(worldMemoryArray);
        }
        public void Randomize()
        {
            var random = new Random();
            for (int i = 0; i < worldSize; i++)
            {
                bool randomValue = random.Next(2) == 0; // Генерация случайного значения true/false
                memory.Span[i] = randomValue;
            }
        }

        public void CreateBedrock()
        {
            for (int i = worldSize - worldSizeX; i < worldSize; i++)
            {
                worldMemoryArray[i] = true;
            }
        }


    }

    public class WaveAlgorithm
    {
        public int maskSizeX;
        public int maskSizeY;
        public int maskSize;

        public bool[] maskMemoryArray;
        public Memory<bool> mask;

        public WaveAlgorithm (int maskSizeX, int maskSizeY)
        {
            this.maskSizeX = maskSizeX;
            this.maskSizeY = maskSizeY;
            this.maskSize = maskSizeX * maskSizeY;
            this.maskMemoryArray = new bool[maskSizeX * maskSizeY];
            this.mask = new Memory<bool>(maskMemoryArray);
        }

        public void AddBedrockToMask()
        {
            for (int i = maskSize - maskSizeX; i < maskSize; i++)
            {
                maskMemoryArray[i] = true;
            }
        }

        public void CalculateMask(World worldClass)
        {
            Memory<bool> world = worldClass.memory;
            coordinateHelper coo = new coordinateHelper(worldClass.worldSizeX, worldClass.worldSizeY);

            this.AddBedrockToMask();
            //Сотрим ячейки выше маски ( не смещать маску , а сместить адресацию маски)
            //дополнить маску но по адрессу без смешения
            Span<bool> worldSpan = world.Span;
            Span<bool> maskSpan = this.mask.Span;

            for (int t = 0; t < 15; t++)
            {

                //смотрим по смещение вверх
                for (int i = 0; i < worldSpan.Length; i++)
                {
                    if (worldSpan[i] && maskSpan[i]
                        && (coo.GetUp(i) >= 0)
                        )
                    {
                        int coordinate = coo.GetUp(i);
                        maskSpan[coo.GetUp(i)] = true;
                    }
                }
                //смотрим по смещение влево
                for (int i = 0; i < worldSpan.Length; i++)
                {
                    if (worldSpan[i] && maskSpan[i]
                        && (coo.GetLeft(i) >= 0)
                        )
                    {
                        int coordinate = coo.GetLeft(i);
                        maskSpan[coo.GetLeft(i)] = true;
                    }
                }
                //смотрим по смещение вниз
                for (int i = 0; i < worldSpan.Length; i++)
                {
                    if (worldSpan[i] && maskSpan[i]
                        && (coo.GetDown(i) >= 0)
                        )
                    {
                        int coordinate = coo.GetDown(i);
                        maskSpan[coo.GetDown(i)] = true;
                    }
                }
                //смотрим по смещение вправо
                for (int i = 0; i < worldSpan.Length; i++)
                {
                    if (worldSpan[i] && maskSpan[i]
                        && (coo.GetRight(i) >= 0)
                        )
                    {
                        int coordinate = coo.GetRight(i);
                        maskSpan[coo.GetRight(i)] = true;
                    }
                }
            }


        }

    }

    public class coordinateHelper
    {
        int worldSizeX;
        int worldSizeY;

        public coordinateHelper(int worldSizeX, int worldsizeY)
        {
            this.worldSizeX = worldSizeX;
            this.worldSizeY = worldsizeY;
        }

        public int GetUp(int flatCoordinate)
        {
            int x = (flatCoordinate) % worldSizeX;
            int y = (flatCoordinate) / worldSizeX;

            y--;
            if (y >= worldSizeY || y < 0)
                return -1;

            return y * worldSizeX + x;
        }

        public int GetDown(int flatCoordinate)
        {
            int x = (flatCoordinate) % worldSizeX;
            int y = (flatCoordinate) / worldSizeX;

            y++;
            if (y >= worldSizeY || y < 0)
                return -1;

            return y * worldSizeX + x;
        }
        public int GetLeft(int flatCoordinate)
        {
            int x = (flatCoordinate) % worldSizeX;
            int y = (flatCoordinate) / worldSizeX;

            if (x == 0) x = worldSizeX;
            x--;
            

            return y * worldSizeX + x;
        }
        public int GetRight(int flatCoordinate)
        {
            int x = (flatCoordinate) % worldSizeX;
            int y = (flatCoordinate) / worldSizeX;

            x++;
            if (x >= worldSizeX) x = 0;

            return y * worldSizeX + x;
        }
    }
}