using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {

        int worldXSize = 128;
        int worldYSize = 16;
        bool dontPrintMemory = false;

        // Создание объекта world 
        int worldSize = worldXSize * worldYSize;
        int worldByteSize = worldSize / 8;
        int worldLineSize = worldXSize / 8;
        var worldMemoryArray = new byte[worldByteSize];
        var world = new Memory<byte>(worldMemoryArray);

        // Заполнение объекта memory случайными значениями
        var random = new Random();
        random.NextBytes(worldMemoryArray);
        //заполним нижнюю строку world единицами
        for (int i = worldByteSize - worldLineSize; i < worldByteSize; i++)
        {
            worldMemoryArray[i] = 0xFF;
        }

        //содание объекта mask
        var maskMemoryArray = new byte[worldByteSize];
        var mask = new Memory<byte>(maskMemoryArray);
        //заполним нижнюю строку world единицами
        for (int i = worldByteSize - worldLineSize; i < worldByteSize; i++)
        {
            maskMemoryArray[i] = 0xFF;
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

    static void GroupingWaveMain(Memory<byte> world, Memory<byte> mask)
    {

    }

    static void ApplyMaskToWorld(Memory<byte> world, Memory<byte> mask)
    {
        Span<byte> worldSpan = world.Span;
        Span<byte> maskSpan = mask.Span;

        for (int i = 0; i < worldSpan.Length; i++)
        {
            worldSpan[i] &= maskSpan[i];
        }
    }

    static void PrintMemoryInBlocks(ReadOnlySpan<byte> data, int blockBitSize)
    {
        int blockByteSize = blockBitSize / 8;

        for (int i = 0; i < data.Length; i += blockByteSize)
        {
            var block = data.Slice(i, Math.Min(blockByteSize, data.Length - i));
            PrintBlock(block);
            Console.WriteLine(); // Добавлен перевод строки между блоками
        }
    }

    static void PrintBlock(ReadOnlySpan<byte> block)
    {
        for (int i = 0; i < block.Length; i++)
        {
            Console.Write(Convert.ToString(block[i], 2).PadLeft(8, '0'));
        }
    }
}