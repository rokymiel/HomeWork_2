using System;
using System.Collections.Generic;
using System.IO;

namespace HW2
{
    delegate void ErrorNotificationType(string message);
    delegate double MathOperation(double a, double b);
    class Calculator
    {
        public static double Calculate(MathOperation operation, double x, double y)
        {
            return operation(x, y);
        }
        public static event ErrorNotificationType ErrorNotification;
        public static void Error(string m)
        {
            ErrorNotification(m);
        }
    }

    class MainClass
    {
        const string exprPath = "expressions.txt";
        const string exprAnswPath = "answers.txt";
        const string exprChecker = "expressions_checker.txt";
        const string resultPath = "results.txt";
        const string resultErrors = "results_errors.txt";

        static Dictionary<String, MathOperation> operations;
        static MainClass()
        {
            operations = new Dictionary<string, MathOperation>();
            operations.Add("+", (x, y) => x + y);
            operations.Add("-", (x, y) => x - y);
            operations.Add("*", (x, y) => x * y);
            operations.Add("/", (x, y) => x / y);
            operations.Add("^", (x, y) => Math.Pow(x, y));
        }
        /// <summary>
        /// Вычисление результата арифметической операции.
        /// </summary>
        /// <param name="expr">Выражение.</param>
        /// <returns>Ответ.</returns>
        public static double Calculate(string expr)
        {
            string[] arguments = expr.Split(' ');
            double operA = double.Parse(CheckDouble(arguments[0]));
            double operB = double.Parse(CheckDouble(arguments[2]));
            string operP = arguments[1];
            double res = operations[operP](operA, operB);
            if (double.IsInfinity(res) && operB == 0) { throw new DivideByZeroException(); }
            else if (double.IsInfinity(res)) throw new OverflowException();
            if (double.IsNaN(res)) throw new NotFiniteNumberException();
            return res;


        }

        public static void Main(string[] args)
        {
            try
            {
                Calculator.ErrorNotification += ConsoleErrorHandler;
                Calculator.ErrorNotification += ResultErrorHandler;

                //Вычисление результата арифметический выражений.
                //Console.WriteLine("Поиск решения начался...");
                //FindAnswer();
                //Console.WriteLine("Поиск решения закончился");

                // Проверка ответов в файлах.
                Console.WriteLine("Проверка ответов началась...");
                CheckAnswers();
                Console.WriteLine("ПРоверка ответов закончилась");
            }
            catch (Exception ex) { Calculator.Error(ex.Message); }


        }
        static void ResultErrorHandler(string message)
        {
            try
            {
                File.AppendAllText(resultErrors, message + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при работе с записи ошибок в файл");
            }
        }
        static void ConsoleErrorHandler(string message)
        {
            Console.WriteLine($"Error: {message} Time of error:{DateTime.Now}");
        }
        /// <summary>
        /// Замена знака.
        /// </summary>
        /// <param name="str">Исходная строка.</param>
        /// <returns>Выходная стока.</returns>
        public static string CheckDouble(string str)
        {
            str = str.Replace('.', ',');
            return str;
        }
        /// <summary>
        /// Проверка ответов из файлов.
        /// </summary>
        public static void CheckAnswers()
        {
            string[] answers = ReadAllLines(exprAnswPath);
            string[] checker = ReadAllLines(exprChecker);
            int count = 0;
            for (int i = 0; i < checker.Length; i++)
            {

                if (checker[i] != answers[i])
                {
                    count++;
                    Console.WriteLine($"{checker[i]}!={answers[i]}");
                }
            }
            Console.WriteLine($"Всего ошибок/не совпадений ответов: {count}");
        }
        /// <summary>
        /// Parse строки в double.
        /// </summary>
        /// <param name="s">Строка.</param>
        /// <param name="n">Числовая ссылочная переменная.</param>
        /// <returns>Получилось ли перевести.</returns>
        public static bool GetDoubleNum(string s, out double n)
        {

            return double.TryParse(CheckDouble(s), out n);
        }
        /// <summary>
        /// Вычисление арифметических выражений из файла.
        /// </summary>
        public static void FindAnswer()
        {
            try
            {
                string[] expr = ReadAllLines(exprPath);
                string[] answ = new string[expr.Length];
                int i = 0;
                foreach (var item in expr)
                {
                    try
                    {
                        answ[i] = $"{Calculate(item):F3}";
                        
                    }
                    catch (KeyNotFoundException)
                    {
                        Calculator.Error("не верный оператор");
                        answ[i] = "неверный оператор";
                    }
                    catch (DivideByZeroException)
                    {
                        Calculator.Error("bruh");
                        answ[i] = "bruh";
                    }
                    catch (NotFiniteNumberException)
                    {
                        Calculator.Error("не число");
                        answ[i] = "не число";

                    }
                    catch (OverflowException)
                    {
                        Calculator.Error("переполнение");
                        answ[i] = "∞";
                    }
                    finally { i++; }
                }
                WriteAllLines(exprAnswPath, answ);
            }
            catch (IOException ex)
            {
                //Console.WriteLine("Ошибка ввода/вывода");
                Calculator.Error(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                //Console.WriteLine("У вас нет разрешения на создание файла.");
                Calculator.Error(ex.Message);
            }
            catch (System.Security.SecurityException ex)
            {
                //Console.WriteLine("Ошибка безопасности.");
                Calculator.Error(ex.Message);
            }

        }
        /// <summary>
        /// Запись в файл.
        /// </summary>
        /// <param name="path">Путь файла.</param>
        /// <param name="str">Массив строк.</param>
        public static void WriteAllLines(string path, string[] str)
        {
            File.WriteAllLines(path, str);
        }
        /// <summary>
        /// Чтение из файла.
        /// </summary>
        /// <param name="path">Путь файла.</param>
        /// <returns>Массив строк.</returns>
        public static string[] ReadAllLines(string path)
        {
            string[] str;
            str = File.ReadAllLines(path);

            return str;
        }
    }
}
