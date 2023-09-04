using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {

        int worldXSize = 128;
        int worldYSize = 16;

        // Создание объекта Memory 
        int worldSize = worldXSize * worldYSize;
        int worldByteSize = worldSize / 8;
        var memoryArray = new byte[worldByteSize];
        var world = new Memory<byte>(memoryArray);

        // Заполнение объекта memory случайными значениями
        var random = new Random();
        random.NextBytes(memoryArray);
        //заполним нижнюю строку единицами
        for (int i = worldByteSize - (worldXSize / 8); i < worldByteSize; i++)
        {
            memoryArray[i] = 0xFF;
        }

        // Ротация бит влево на 1 бит для каждого блока 
        int blockBitSize = 128;
        int blockByteSize = blockBitSize / 8;

        // Вывод содержимого memory блоками 
        Console.WriteLine("Исходная память:");
        PrintMemoryInBlocks(world.Span, blockBitSize);

        for (int i = 0; i <= worldByteSize - blockByteSize; i += blockByteSize)
        {
            var block = world.Slice(i, blockByteSize);
            RotateBitsLeft(block.Span);
        }

        // Вывод содержимого memory блоками 
        Console.WriteLine("Измененная память:");
        PrintMemoryInBlocks(world.Span, blockBitSize);
    }


    static void RotateBitsLeft(Span<byte> data)
    {
        byte carry = (byte)(data[data.Length - 1] >> 7);

        for (int i = data.Length - 1; i > 0; i--)
        {
            byte current = data[i];
            byte shifted = (byte)((current << 1) | (data[i - 1] >> 7));
            data[i] = shifted;
        }

        byte first = data[0];
        byte rotated = (byte)((first << 1) | carry);
        data[0] = rotated;
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