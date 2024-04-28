using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        TcpListener server = null;
        try
        {
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
            server.Start();
            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Клиент подключен!");

            using (NetworkStream stream = client.GetStream())
            using (StreamReader reader = new StreamReader(stream))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.AutoFlush = true;

                writer.WriteLine("GAME_START");

                int wins = 0;
                int losses = 0;

                for (int round = 1; round <= 5; round++)
                {
                    string clientChoice = reader.ReadLine();
                    Console.WriteLine($"Клиент выбрал: {clientChoice}");

                    string serverChoice = GetRandomChoice();
                    Console.WriteLine($"Сервер выбрал: {serverChoice}");

                    string result = DetermineWinner(clientChoice, serverChoice);
                    writer.WriteLine(serverChoice);
                    writer.WriteLine(result);

                    if (result == "Вы победили!")
                        wins++;
                    else if (result == "Вы проиграли!")
                        losses++;
                }

                string gameResult = DetermineGameResult(wins, losses);
                writer.WriteLine(gameResult);
            }

            Console.WriteLine("Отключение клиента.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка: " + ex.Message);
        }
        finally
        {
            server.Stop();
        }

        Console.ReadLine();
    }

    static string GetRandomChoice()
    {
        Random random = new Random();
        int choice = random.Next(1, 4);
        switch (choice)
        {
            case 1: return "r"; 
            case 2: return "s"; 
            case 3: return "p"; 
            default: return "r";
        }
    }

    static string DetermineWinner(string choice1, string choice2)
    {
        if (choice1 == choice2)
            return "Ничья";

        if ((choice1 == "r" && choice2 == "s") ||
            (choice1 == "s" && choice2 == "p") ||
            (choice1 == "p" && choice2 == "r"))
            return "Вы победили!";

        return "Вы проиграли!";
    }

    static string DetermineGameResult(int wins, int losses)
    {
        if (wins > losses)
            return "Итог партии: Вы победили в партии!";
        else if (wins < losses)
            return "Итог партии: Вы проиграли в партии.";
        else
            return "Итог партии: Партия закончилась вничью.";
    }
}
