using System;
using System.Collections.Generic;
using System.IO;

namespace Debtor.Core
{
    public class BorrowerManager
    {
        private List<Borrower> Borrowers { get; set; }

        private string FileName { get; set; } = "borrowers.txt";

        public BorrowerManager()
        {
            Borrowers = new List<Borrower>();

            if (!File.Exists(FileName))
            {
                return;
            }

            var fileLines = File.ReadAllLines(FileName);

            foreach (var line in fileLines)
            {
                var lineItems = line.Split(';');

                if (decimal.TryParse(lineItems[1], out var amountInDecimal) && (DateTime.TryParse(lineItems[2], out var dateInDateTime)))
                {
                    AddBorrower(lineItems[0], amountInDecimal, dateInDateTime, false);
                }
            }
        }

        public void AddBorrower(string name, decimal amount, DateTime date, bool shouldSaveToFile = true)
        {
            var borrower = new Borrower
            {
                Name = name,
                Amount = amount,
                Date = date
            };

            Borrowers.Add(borrower);

            if (shouldSaveToFile)
            {
                File.AppendAllLines(FileName, new List<string> { borrower.ToString() });
            }
        }

        public bool DeleteBorrower(string name)
        {
            foreach (var borrower in Borrowers)
            {
                if (borrower.Name == name)
                {
                    Borrowers.Remove(borrower);

                    SaveBorrowersToFile(true);
                    return true;
                }
            }

            return false;
        }

        public List<string> ListBorrowers()
        {
            var borrowersString = new List<string>();
            var indexer = 1;

            foreach (var borrower in Borrowers)
            {
                var borrowerString = $"{indexer}. {borrower.Name} - {borrower.Amount.ToString("C2")}. Data pożyczki: {borrower.Date}";
                indexer++;

                borrowersString.Add(borrowerString);
            }

            return borrowersString;
        }

        public decimal SumBorrows()
        {
            var sumOfBorrows = (decimal)default;

            foreach (var borrower in Borrowers)
            {
                sumOfBorrows += borrower.Amount;
            }

            return sumOfBorrows;
        }

        public decimal SubtractLoan(string name, decimal amount)
        {

            foreach (var borrower in Borrowers)
            {
                if ((borrower.Name == name) && (borrower.Amount >= amount))
                {
                    borrower.Amount -= amount;
                    SaveBorrowersToFile(true);

                    return amount;
                }
            }

            return -1;
        }

        public List<Borrower> CheckBorrowersDeadlines(decimal loanRatio, int deadline)
        {
            var borrowersWhoPassedDeadline = new List<Borrower>();
            var interestRatio = loanRatio / 100;
            var isChangeMade = false;

            foreach (var borrower in Borrowers)
            {
                var borrowerDeadline = borrower.Date.AddDays(deadline);

                if (DateTime.Now > borrowerDeadline)
                {
                    isChangeMade = true;

                    borrower.Amount += interestRatio * borrower.Amount;
                    borrower.Date = borrowerDeadline;

                    borrowersWhoPassedDeadline.Add(borrower);
                }
            }

            SaveBorrowersToFile(isChangeMade);
            return borrowersWhoPassedDeadline;
        }

        private void SaveBorrowersToFile(bool shouldSaveToFile)
        {
            if (!shouldSaveToFile)
                return;

            var borrowersToSave = new List<string>();

            foreach (var borrower in Borrowers)
            {
                borrowersToSave.Add(borrower.ToString());
            }

            File.Delete(FileName);
            File.WriteAllLines(FileName, borrowersToSave);
        }
    }
}