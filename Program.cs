using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool start = true;
            Users user = new Users();
            Text txt = new Text();
            Stopwatch sw = new Stopwatch();

            while (start)
            {
                Console.Clear();
                Console.Write("Введите ваш ник: ");
                user.Name = Console.ReadLine();

                Console.Clear();
                List<char> list = Text.AddText();
                Console.WriteLine("\nЧтобы начать, нажмите Enter");
                while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }

                sw.Restart();
                bool completed = txt.ReadText(list, sw);
                sw.Stop();

                if (completed)
                {
                    user.CharactersPerMinute = (int)(list.Count / (sw.Elapsed.TotalMinutes));
                    user.CharactersPerSecond = list.Count / sw.Elapsed.TotalSeconds;
                }
                else
                {
                    user.CharactersPerMinute = 0;
                    user.CharactersPerSecond = 0;
                }

                user.AddTableRecords();
                user.DrewTableRecords();

                Console.WriteLine("Чтобы попробовать еще раз, нажмите Enter. Чтобы выйти, нажмите Escape.");
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape)
                {
                    start = false;
                }
            }
        }
    }

    internal class Users
    {
        public string Name;
        public int CharactersPerMinute;
        public double CharactersPerSecond;
        private static string filePath = "TableRecords.json";
        private static List<Users> ListUser = new List<Users>();

        public Users()
        {
            LoadTableRecords();
        }

        public void AddTableRecords()
        {
            var existingUser = ListUser.FirstOrDefault(u => u.Name == Name);
            if (existingUser != null)
            {
                existingUser.CharactersPerMinute = CharactersPerMinute;
                existingUser.CharactersPerSecond = CharactersPerSecond;
            }
            else
            {
                ListUser.Add(this);
            }
            SaveTableRecords();
        }

        public void DrewTableRecords()
        {
            Console.WriteLine("Таблица рекордов:");
            foreach (var element in ListUser)
            {
                Console.WriteLine($"{element.Name} - Символов в минуту: {element.CharactersPerMinute}, Символов в секунду: {element.CharactersPerSecond:F2}");
            }
        }

        private static void LoadTableRecords()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                ListUser = JsonConvert.DeserializeObject<List<Users>>(json) ?? new List<Users>();
            }
        }

        private static void SaveTableRecords()
        {
            string json = JsonConvert.SerializeObject(ListUser);
            File.WriteAllText(filePath, json);
        }
    }

    internal class Text
    {
        public static List<char> AddText()
        {
            string text = "There are four types of schools in the English and Welsh education system - nursery.";
            Console.WriteLine(text);
            return new List<char>(text);
        }

        public bool ReadText(List<char> list, Stopwatch sw)
        {
            int index = 0;
            while (index < list.Count && sw.Elapsed.TotalMinutes < 1)
            {
                Console.SetCursorPosition(index, 1);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(list[index]);
                Console.ForegroundColor = ConsoleColor.Gray;
                char inputChar = Console.ReadKey(true).KeyChar;
                if (inputChar == list[index])
                {
                    index++;
                }
            }
            return index == list.Count;
        }
    }
}
