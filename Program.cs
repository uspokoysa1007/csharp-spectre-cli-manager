using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace SpectreCliManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "C# Spectre CLI System Manager";
            Console.CursorVisible = false;

            string[] menuItems = new string[]
            {
                "1. Просмотр процессов (Top Memory Consumers)",
                "2. Инспекция дисковых накопителей",
                "3. Тест анимированного спиннера задач",
                "4. Информация о системе и среде",
                "5. Выход"
            };

            int selectedIndex = 0;
            bool running = true;

            while (running)
            {
                Console.Clear();
                DrawHeader();

                for (int i = 0; i < menuItems.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("  > " + menuItems[i] + "  ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine("    " + menuItems[i]);
                        Console.ResetColor();
                    }
                }

                DrawFooter();

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex = (selectedIndex - 1 + menuItems.Length) % menuItems.Length;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex = (selectedIndex + 1) % menuItems.Length;
                        break;
                    case ConsoleKey.Enter:
                        running = HandleMenuAction(selectedIndex);
                        break;
                    case ConsoleKey.Escape:
                        running = false;
                        break;
                }
            }

            Console.CursorVisible = true;
            Console.ResetColor();
            Console.Clear();
            Console.WriteLine("[+] До свидания!");
        }

        static void DrawHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║           C# SPECTRE INTERACTIVE CLI SYSTEM MANAGER          ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
        }

        static void DrawFooter()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("----------------------------------------------------------------");
            Console.WriteLine("Навигация: [↑/↓] Выбор пункта | [Enter] Открыть | [Esc] Выход");
            Console.ResetColor();
        }

        static bool HandleMenuAction(int index)
        {
            Console.Clear();
            switch (index)
            {
                case 0:
                    ShowProcesses();
                    break;
                case 1:
                    ShowDisks();
                    break;
                case 2:
                    ShowSpinnerDemo();
                    break;
                case 3:
                    ShowSystemInfo();
                    break;
                case 4:
                    return false;
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Нажмите любую клавишу для возврата в меню...");
            Console.ResetColor();
            Console.ReadKey(true);
            return true;
        }

        static void ShowProcesses()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[ TOP 15 ПРОЦЕССОВ ПО ПАМЯТИ ]");
            Console.ResetColor();
            Console.WriteLine("{0,-10} {1,-30} {2,15}", "PID", "Имя процесса", "Память (МБ)");
            Console.WriteLine(new string('-', 60));

            try
            {
                var processes = Process.GetProcesses()
                    .OrderByDescending(p => {
                        try { return p.WorkingSet64; } catch { return 0L; }
                    })
                    .Take(15);

                foreach (var p in processes)
                {
                    double memMb = p.WorkingSet64 / (1024.0 * 1024.0);
                    Console.WriteLine("{0,-10} {1,-30} {2,12:F1} MB", p.Id, Truncate(p.ProcessName, 28), memMb);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        static void ShowDisks()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[ ДИСКОВЫЕ НАКОПИТЕЛИ ]");
            Console.ResetColor();
            Console.WriteLine("{0,-8} {1,-12} {2,12} {3,12} {4,16}", "Диск", "Тип", "Всего", "Свободно", "Заполнение");
            Console.WriteLine(new string('-', 68));

            foreach (var d in DriveInfo.GetDrives())
            {
                if (d.IsReady)
                {
                    double totalGb = d.TotalSize / (1024.0 * 1024.0 * 1024.0);
                    double freeGb = d.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0);
                    double usedPct = ((totalGb - freeGb) / totalGb) * 100.0;
                    string bar = RenderBar(usedPct, 10);

                    Console.WriteLine("{0,-8} {1,-12} {2,9:F1} GB {3,9:F1} GB  {4} {5:F0}%",
                        d.Name, d.DriveType, totalGb, freeGb, bar, usedPct);
                }
            }
        }

        static void ShowSpinnerDemo()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[ ТЕСТ СПИННЕРА ВЫПОЛНЕНИЯ ЗАДАЧ ]");
            Console.ResetColor();

            string[] spinner = new string[] { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };
            string[] tasks = new string[] {
                "Инициализация конфигурации...",
                "Сбор телеметрии ОС...",
                "Проверка целостности компонентов...",
                "Генерация отчета..."
            };

            foreach (var task in tasks)
            {
                for (int frame = 0; frame < 20; frame++)
                {
                    Console.Write($"\r  {spinner[frame % spinner.Length]} {task} ");
                    Thread.Sleep(60);
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\r  [✓] {task} Готово!  ");
                Console.ResetColor();
            }
        }

        static void ShowSystemInfo()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[ СИСТЕМНАЯ ИНФОРМАЦИЯ ]");
            Console.ResetColor();
            Console.WriteLine("  Имя машины       : " + Environment.MachineName);
            Console.WriteLine("  Пользователь     : " + Environment.UserName);
            Console.WriteLine("  Версия ОС        : " + Environment.OSVersion);
            Console.WriteLine("  Процессоры (ядра): " + Environment.ProcessorCount);
            Console.WriteLine("  Версия .NET      : " + Environment.Version);
            Console.WriteLine("  Время работы     : " + TimeSpan.FromMilliseconds(Environment.TickCount).ToString(@"dd\.hh\:mm\:ss"));
        }

        static string RenderBar(double pct, int width)
        {
            int filled = (int)Math.Round((pct / 100.0) * width);
            filled = Math.Max(0, Math.Min(width, filled));
            return "[" + new string('█', filled) + new string('░', width - filled) + "]";
        }

        static string Truncate(string val, int maxLen)
        {
            if (string.IsNullOrEmpty(val)) return "";
            return val.Length <= maxLen ? val : val.Substring(0, maxLen - 3) + "...";
        }
    }
}
