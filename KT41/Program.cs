using System;
using System.Collections.Generic;

namespace KT41
{
    internal class Program
    {
        private const string MenuShortLived = "1";
        private const string MenuLongLived = "2";
        private const string MenuHeavyLoh = "3";
        private const string MenuBlockingGc = "4";
        private const string MenuRunAll = "5";
        private const string MenuPrintTable = "6";
        private const string MenuClear = "7";
        private const string MenuExit = "0";

        static void Main()
        {
            LoadSimulator simulator = new();
            List<SimulationResult> results = new();
            bool running = true;

            while (running)
            {
                PrintMenu();
                Console.Write("Выберите пункт: ");
                string? input = Console.ReadLine()?.Trim();

                switch (input)
                {
                    case MenuShortLived:
                        int shortCount = ReadPositiveInt(
                            $"Введите количество итераций (Enter для {LoadSimulator.DefaultShortLivedIterations}): ",
                            LoadSimulator.DefaultShortLivedIterations);
                        AddAndPrint(results, simulator.ShortLived(shortCount));
                        break;

                    case MenuLongLived:
                        int longCount = ReadPositiveInt(
                            $"Введите количество массивов по {LoadSimulator.LongLivedArraySize} байт (Enter для {LoadSimulator.DefaultLongLivedCount}): ",
                            LoadSimulator.DefaultLongLivedCount);
                        AddAndPrint(results, simulator.LongLived(longCount));
                        break;

                    case MenuHeavyLoh:
                        int lohCount = ReadPositiveInt(
                            $"Введите количество объектов по {LoadSimulator.HeavyLohArraySize} байт (Enter для {LoadSimulator.DefaultHeavyLohCount}): ",
                            LoadSimulator.DefaultHeavyLohCount);
                        AddAndPrint(results, simulator.HeavyLoh(lohCount));
                        break;

                    case MenuBlockingGc:
                        AddAndPrint(results, simulator.ForceBlockingCollect());
                        break;

                    case MenuRunAll:
                        AddAndPrint(results, simulator.ShortLived(LoadSimulator.DefaultShortLivedIterations));
                        AddAndPrint(results, simulator.LongLived(LoadSimulator.DefaultLongLivedCount));
                        AddAndPrint(results, simulator.HeavyLoh(LoadSimulator.DefaultHeavyLohCount));
                        AddAndPrint(results, simulator.ForceBlockingCollect());
                        PrintTable(results);
                        break;

                    case MenuPrintTable:
                        PrintTable(results);
                        break;

                    case MenuClear:
                        simulator.ClearMemory();
                        results.Clear();
                        Console.WriteLine("Данные и удерживаемые списки очищены.");
                        break;

                    case MenuExit:
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Ошибка: выберите корректный пункт меню от 0 до 7.");
                        break;
                }
            }
        }

        private static void PrintMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Меню:");
            Console.WriteLine("1. Запустить ShortLived()");
            Console.WriteLine("2. Запустить LongLived()");
            Console.WriteLine("3. Запустить HeavyLoh()");
            Console.WriteLine("4. Запустить принудительный GC.Collect(2, blocking: true)");
            Console.WriteLine("5. Запустить все тесты со стандартными значениями");
            Console.WriteLine("6. Показать таблицу результатов");
            Console.WriteLine("7. Очистить память и таблицу");
            Console.WriteLine("0. Выход");
        }

        private static int ReadPositiveInt(string prompt, int defaultValue)
        {
            while (true)
            {
                Console.Write(prompt);
                string? raw = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(raw))
                {
                    return defaultValue;
                }

                if (int.TryParse(raw.Trim(), out int value) && value > 0)
                {
                    return value;
                }

                Console.WriteLine("Ошибка: введите целое число больше нуля.");
            }
        }

        private static void AddAndPrint(List<SimulationResult> results, SimulationResult result)
        {
            results.Add(result);
            Console.WriteLine(
                $"{result.MethodName} | Время: {result.ElapsedMs:F3} мс | Gen0: +{result.Gen0Delta} | Gen1: +{result.Gen1Delta} | Gen2: +{result.Gen2Delta}");
        }

        private static void PrintTable(List<SimulationResult> rows)
        {
            if (rows.Count == 0)
            {
                Console.WriteLine("Таблица пуста.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine($"{"Метод",-28} | {"Время (мс)",-12} | {"Gen0",-6} | {"Gen1",-6} | {"Gen2",-6}");
            Console.WriteLine(new string('-', 70));

            foreach (SimulationResult row in rows)
            {
                Console.WriteLine($"{row.MethodName,-28} | {row.ElapsedMs,-12:F3} | {row.Gen0Delta,-6} | {row.Gen1Delta,-6} | {row.Gen2Delta,-6}");
            }
        }
    }
}