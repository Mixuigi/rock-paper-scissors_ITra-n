using System;
using System.Security.Cryptography;
using System.Text;

namespace ITransition_RockPaperScissors
{
    class Program
    {
        private static byte[] CurrentKey = new byte[16];
        private static byte[] CurrentHash;
        private static RandomNumberGenerator Generator = RandomNumberGenerator.Create();
        private static HMACSHA256 HMAC;
        private static String[] Choices;
        static void Main(string[] args)
        {
            Choices = args;
            CheckArguments();
            int chosen = GetCPUChoice();
            CurrentHash = GetHashFromString(Choices[chosen]);
            PrintWelcome();
            int userChosen = GetUserChoice();
            Console.WriteLine($"Computer choice: {Choices[chosen]}\r\nYour choice: {Choices[userChosen]}");
            switch (DetermineWinner(chosen, userChosen))
            {
                case WinnerCodes.Cpu:
                    Console.WriteLine("Computer won!");
                    break;
                case WinnerCodes.User:
                    Console.WriteLine("You won!");
                    break;
                case WinnerCodes.Draw:
                    Console.WriteLine("Its a draw!");
                    break;
            }
            Console.WriteLine($"KEY: {BitConverter.ToString(CurrentKey).Replace("-", String.Empty)}");
        }
        private static void CheckArguments()
        {
            if ((Choices.Length % 2) == 0 || Choices.Length <= 1)
            {
                Console.WriteLine("Необходимо передать нечетное количество параметров! Для игры необходимо больше 1 параметра!");
                Environment.Exit(0);
            }
            for (int i=0;i< Choices.Length-1;i++)
            {
                if (Choices[i] == Choices[i + 1])
                {
                    Console.WriteLine($"Параметры не должны быть одинаковыми!\r\n" +
                        $"{Choices[i]} == {Choices[i+1]}");
                    Environment.Exit(0);
                }
            }
        }

        private static int GetCPUChoice()
        {
            Random random = new Random();
            int number = random.Next(0, Choices.Length);
            return number;
        }
        private static byte[] GetHashFromString(String value)
        {
            Generator.GetBytes(CurrentKey);
            HMAC = new HMACSHA256(CurrentKey);
            byte[] choiceEncoded = Encoding.UTF8.GetBytes(value);
            return HMAC.ComputeHash(choiceEncoded);
        }
        private static void PrintWelcome()
        {
            Console.Clear();
            Console.WriteLine($"HMAC: {BitConverter.ToString(CurrentHash).Replace("-", String.Empty)}");
            Console.WriteLine("Choices:");
            for (int i = 0; i < Choices.Length; i++)
            {
                Console.WriteLine($"[{i + 1}]{Choices[i]}");
            }
            Console.WriteLine("[0]Exit");
        }
        private static int GetUserChoice()
        {
            int chosen;
            while (true)
            {
                if(int.TryParse(Console.ReadLine(),out chosen))
                {
                    if (chosen > 0 && chosen <= Choices.Length)
                    {
                        return chosen - 1;
                    }
                    else if (chosen == 0) Environment.Exit(0);
                }
                PrintWelcome();
                Console.Write("Неверно введен пункт меню! Повторите ввод: ");
            }
        }
        private static WinnerCodes DetermineWinner(int cpuChoice,int userChoice)
        {
            if (cpuChoice == userChoice) return WinnerCodes.Draw;
            int current = cpuChoice;
            int distance = 0;
            while (true)
            {
                if (current == userChoice) break;
                current = GetPrevious(current);
                distance++;
            }
            if ((distance % 2) == 0) return WinnerCodes.User; else return WinnerCodes.Cpu;
        }
        private static int GetPrevious(int current)
        {
            if ((current - 1) < 0) return Choices.Length - 1;
            return current - 1;
        }
    }
}
