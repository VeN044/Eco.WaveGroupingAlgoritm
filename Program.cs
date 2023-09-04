using System;
using System.IO;

namespace Eco.WaveGroupingAlgoritm
{
    class Program
    {
        static void Main(string[] args)
        {
            // Создание объекта Memory размером 2000 бит
            int bitSize = 2000;
            int byteSize = bitSize / 8;
            var memoryArray = new byte[byteSize];
            var memory = new Memory<byte>(memoryArray);

            // Заполнение объекта memory случайными значениями
            var random = new Random();
            random.NextBytes(memoryArray);

            // Вывод содержимого memory
            Console.WriteLine("исходная память:");
            PrintMemory(memory.Span);

            // Ротация бит влево на 1 бит для каждого блока по 100 бит
            int blockBitSize = 100;
            int blockByteSize = blockBitSize / 8;
            for (int i = 0; i <= byteSize - blockByteSize; i += blockByteSize)
            {
                var block = memory.Slice(i, blockByteSize);
                RotateBitsLeft(block.Span);
            }

            // Вывод содержимого memory
            Console.WriteLine("Измененная память:");
            PrintMemory(memory.Span);
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

        static void PrintMemory(ReadOnlySpan<byte> data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Console.Write(Convert.ToString(data[i], 2).PadLeft(8, '0'));
            }

            Console.WriteLine();
        }
    }
}