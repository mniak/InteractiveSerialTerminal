using System;
using System.ComponentModel;
using System.IO.Ports;

namespace InteractiveSerialTerminal
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Init();
            using (var serial = new SerialPort("COM11"))
            {
                serial.Open();
                StartBackgroundWork(serial);
                while (true)
                {
                    var key = Console.ReadKey();
                    switch (key.Key)
                    {
                        case ConsoleKey.L:
                            if (key.Modifiers == ConsoleModifiers.Control)
                            {
                                Console.Clear();
                                Init();
                            }
                            break;

                        case ConsoleKey.Escape:
                            WriteByte(serial, 0x04 /*EOT*/);
                            break;

                        case ConsoleKey.A:
                        case ConsoleKey.Enter:
                        case ConsoleKey.Spacebar:
                            WriteByte(serial, 0x06 /*ACK*/);
                            break;

                        case ConsoleKey.N:
                        case ConsoleKey.Delete:
                        case ConsoleKey.Backspace:
                            WriteByte(serial, 0x15 /*NAK*/);
                            break;
                    }
                }
            }
        }

        private static void Init()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Interactive serial terminal");
        }

        private static void WriteByte(SerialPort serial, byte b)
        {
            serial.Write(new[] { b }, 0, 1);
        }

        private static void StartBackgroundWork(SerialPort serial)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += (s, e) =>
            {
                while (true)
                {
                    var b = serial.ReadByte();
                    ConsoleWriteByte(b);
                }

            };
            bw.RunWorkerAsync();
        }

        private static void ConsoleWriteByte(int b)
        {
            //if (b >= 0x20 && b <= 0x7f)
            //{
            //    Console.ForegroundColor = ConsoleColor.Blue;
            //    Console.Write((char)b);
            //}
            //else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(b.ToString("X"));
                Console.Write(" ");
            }
        }
    }
}
