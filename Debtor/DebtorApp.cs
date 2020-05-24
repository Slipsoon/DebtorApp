using System;
using Debtor.Core;

namespace Debtor
{
    public class DebtorApp
    {
        public BorrowerManager BorrowerManager { get; set; } = new BorrowerManager();

        public void IntroduceDebtorApp()
        {
            Console.WriteLine("Hey, witam w aplikacji Dłużnik. Zapisujemy tutaj listę dłużników, tak abyś wiedział ile kasy Ci kto wisi.");
        }

        public void AddBorrower()
        {
            Console.Clear();

            Console.Write("Podaj nazwę dłużnika, którego chcesz dodać do listy: ");

            var userName = Console.ReadLine();

            Console.Write("Podaj kwotę długu: ");

            var userAmount = Console.ReadLine();
            var amountInDecimal = default(decimal);

            while (!decimal.TryParse(userAmount, out amountInDecimal) || amountInDecimal <= 0)
            {
                Console.WriteLine("Podano niepoprawną ktotę");
                Console.Write("Podaj kwotę długu: ");

                userAmount = Console.ReadLine();
            }

            var date = DateTime.Now;

            BorrowerManager.AddBorrower(userName, amountInDecimal, date);
            Console.WriteLine($"\nDłużnik {userName} został dodany do listy dłużników!");
        }

        public void DeleteBorrower()
        {
            Console.Clear();

            ListAllBorrowers();

            Console.Write("\nPodaj nazwę dłużnika, którego chcesz usunąć z listy: ");

            var userName = Console.ReadLine();

            var isBorrowerFound = BorrowerManager.DeleteBorrower(userName);

            if(isBorrowerFound)
                Console.WriteLine($"Dłużnik {userName} został usunięty z listy!");

            if(!isBorrowerFound)
                Console.WriteLine($"Nie odnaleziono dłużnika o podanej nazwie: {userName}");
        }

        public void ListAllBorrowers()
        {
            Console.Clear();

            Console.WriteLine("Oto lista Twoich dłużników:\n ");

            foreach (var borrower in BorrowerManager.ListBorrowers())
            {
                Console.WriteLine(borrower);
            }

            ListSumOfBorrows();
        }

        public void ListSumOfBorrows()
        {
            Console.Write("\nOto suma pożyczek Twoich dłużników: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{BorrowerManager.SumBorrows().ToString("C2")}.\n");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void SubtractAmountOfLoan()
        {
            ListAllBorrowers();

            Console.Write("\nPodaj imię dłużnika, któremu chcesz odjąć dług: ");
            var userName = Console.ReadLine();

            Console.Write("\nPodaj kwotę odejmowanego długu: ");
            var userAmount = Console.ReadLine();

            var parsedAmountInDecimal = default(decimal);
            while (!decimal.TryParse(userAmount, out parsedAmountInDecimal) || parsedAmountInDecimal <= 0)
            {
                Console.WriteLine("\nPodano niepoprawną kwotę");
                Console.Write("\nPodaj kwotę odejmowanego długu: ");

                userAmount = Console.ReadLine();
            }

            var subtractedAmount = BorrowerManager.SubtractLoan(userName, parsedAmountInDecimal);

            if (subtractedAmount == -1)
                Console.WriteLine("\nKwota nie została odjęta. Być może wprowadzono złą nazwę użytkownika.");

            if (subtractedAmount > 0)
                Console.WriteLine($"\nPożyczka dłużnika {userName} została odjęta o kwotę {parsedAmountInDecimal} zł");
        }

        public void ListBorrowersWhoPassedDeadlines()
        {
            Console.Clear();

            var loanRatio = 5;
            var deadline = 7;

            var borrowersDeadlines = BorrowerManager.CheckBorrowersDeadlines(loanRatio, deadline);

            if (borrowersDeadlines.Count == 0)
                Console.WriteLine($"Żaden dłużnik nie przekroczył {deadline}-dniowego terminu spłaty");

            foreach (var borrower in borrowersDeadlines)
            {
                Console.WriteLine($"Dłużnik {borrower.Name} przekroczył {deadline}-dniowy termin spłaty.\n" +
                                  $"Oprocentowanie długu w wysokości {loanRatio}% zostało dodane do całkowitej kwoty dłużnika.\n" +
                                  $"Nowy termin wzięcia pożyczki został wyznaczony na {borrower.Date}\n");
            }
        }

        public void AskForAction()
        {
            var userInput = default(string);

            while (userInput != "exit")
            {
                Console.WriteLine("\nPodaj czynność, którą chcesz wykonać:\n");
                Console.WriteLine("add - Dodawanie dłużnika");
                Console.WriteLine("del - Usuwanie dłużnika");
                Console.WriteLine("list - Wypisywanie listy dłużników");
                Console.WriteLine("sum - Wypisywanie sumy pożyczek dłużników");
                Console.WriteLine("sub - Odejmowanie części kwoty pożyczki dłużnika");
                Console.WriteLine("dlines - Weryfikacja terminów spłaty długu przez dłużników");
                Console.WriteLine("exit - Wyjście z programu");

                Console.Write("\nWybrana czynność: ");

                userInput = Console.ReadLine();
                userInput = userInput.ToLower();

                switch (userInput)
                {
                    case "add":
                        AddBorrower();
                        break;
                    case "del":
                        DeleteBorrower();
                        break;
                    case "list":
                        ListAllBorrowers();
                        break;
                    case "sum":
                        Console.Clear();
                        ListSumOfBorrows();
                        break;
                    case "sub":
                        SubtractAmountOfLoan();
                        break;
                    case "dlines":
                        ListBorrowersWhoPassedDeadlines();
                        break;
                    case "exit":
                        break;

                    default:
                        Console.WriteLine("Podano złą wartość");
                        break;
                }
            }
        }
    }
}