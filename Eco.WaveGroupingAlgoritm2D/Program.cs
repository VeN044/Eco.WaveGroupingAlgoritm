using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

class Program
{
    static void Main(string[] args)
    {

        int worldXSize = 128;
        int worldYSize = 16;
        bool dontPrintMemory = false;

        // Создание объекта world 
        int worldSize = worldXSize * worldYSize;
        var worldMemoryArray = new bool[worldSize];
        var world = new Memory<bool>(worldMemoryArray);

        // Заполнение объекта memory случайными значениями
        var random = new Random();
        for (int i = 0; i < worldSize; i++)
        {
            bool randomValue = random.Next(2) == 0; // Генерация случайного значения true/false

            world.Span[i] = randomValue;
        }
        //заполним нижнюю строку world единицами
        for (int i = worldSize - worldXSize; i < worldSize; i++)
        {
            worldMemoryArray[i] = true;
        }

        //содание объекта mask
        var maskMemoryArray = new bool[worldSize];
        var mask = new Memory<bool>(maskMemoryArray);
        //заполним нижнюю строку world единицами
        for (int i = worldSize - worldXSize; i < worldSize; i++)
        {
            maskMemoryArray[i] = true;
        }

        // Вывод содержимого memory  
        Console.WriteLine("Исходный world:");
        if(!dontPrintMemory) PrintMemoryInBlocks(world.Span, worldXSize);

        // Вывод содержимого mask  
        Console.WriteLine("Исходный mask:");
        if (!dontPrintMemory) PrintMemoryInBlocks(mask.Span, worldXSize);

        GroupingWaveMain(world, mask);


        // Вывод содержимого memory блоками 
        Console.WriteLine("Измененная память:");
        if (!dontPrintMemory) PrintMemoryInBlocks(world.Span, worldXSize);
    }

    static void GroupingWaveMain(Memory<bool> world, Memory<bool> mask)
    {

    }

    static void ApplyMaskToWorld(Memory<bool> world, Memory<bool> mask)
    {
        Span<bool> worldSpan = world.Span;
        Span<bool> maskSpan = mask.Span;

        for (int i = 0; i < worldSpan.Length; i++)
        {
            worldSpan[i] &= maskSpan[i];
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
            binaryString.Append(block[i] ? "1" : "0");
        }
        Console.Write(binaryString.ToString());
    }
}