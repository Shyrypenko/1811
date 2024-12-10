using System;

public class CreditCard
{
    // Properties
    public string CardNumber { get; private set; }
    public string OwnerName { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public string PIN { get; private set; }
    public decimal CreditLimit { get; private set; }
    public decimal Balance { get; private set; }

    public event Action<decimal> AccountReplenished;
    public event Action<decimal> MoneySpent;
    public event Action CreditLimitReached;
    public event Action<decimal> CreditUsed;
    public event Action PINChanged;

    public CreditCard(string cardNumber, string ownerName, DateTime expiryDate, string pin, decimal creditLimit, decimal initialBalance)
    {
        CardNumber = cardNumber;
        OwnerName = ownerName;
        ExpiryDate = expiryDate;
        PIN = pin;
        CreditLimit = creditLimit;
        Balance = initialBalance;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Deposit amount must be greater than zero.");
            return;
        }

        Balance += amount;
        Console.WriteLine($"Account replenished by {amount}. New balance: {Balance}");
        AccountReplenished?.Invoke(amount);
    }

    public void Spend(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Spending amount must be greater than zero.");
            return;
        }

        if (amount > Balance + CreditLimit)
        {
            Console.WriteLine("Insufficient funds.");
            return;
        }

        Balance -= amount;

        if (Balance < 0)
        {
            Console.WriteLine("Credit funds are being used.");
            CreditUsed?.Invoke(Balance);
        }

        Console.WriteLine($"Spent {amount}. Remaining balance: {Balance}");
        MoneySpent?.Invoke(amount);

        if (Balance == 0)
        {
            Console.WriteLine("Credit limit reached.");
            CreditLimitReached?.Invoke();
        }
    }

    public void ChangePIN(string newPIN)
    {
        if (newPIN.Length != 4 || !int.TryParse(newPIN, out _))
        {
            Console.WriteLine("PIN must be a 4-digit number.");
            return;
        }

        PIN = newPIN;
        Console.WriteLine("PIN successfully changed.");
        PINChanged?.Invoke();
    }
}

class Program
{
    static void Main()
    {
        var card = new CreditCard("1234-5678-9012-3456", "John Doe", new DateTime(2030, 12, 31), "1234", 5000, 1000);

        card.AccountReplenished += amount => Console.WriteLine($"[Event] Account replenished by: {amount}");
        card.MoneySpent += amount => Console.WriteLine($"[Event] Money spent: {amount}");
        card.CreditUsed += creditBalance => Console.WriteLine($"[Event] Credit funds used. Current credit balance: {creditBalance}");
        card.CreditLimitReached += () => Console.WriteLine($"[Event] Credit limit reached.");
        card.PINChanged += () => Console.WriteLine($"[Event] PIN has been changed.");

        card.Deposit(500);
        card.Spend(200);
        card.Spend(2000);
        card.ChangePIN("5678");
        card.Spend(5000);
    }
}