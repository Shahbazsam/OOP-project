using System;

public class Money : IEquatable<Money>, IComparable<Money>
{
    private char sign;
    private int integerPart;
    private int fractionalPart;
    private string currency;

    private static readonly string[] currencies = { "Dollars", "Rubbles", "Euros", "Yen" };

    public char Sign
    {
        get { return sign; }
        set { sign = value; }
    }

    public int IntegerPart
    {
        get { return integerPart; }
        set { integerPart = Math.Max(0, value); }
    }

    public int FractionalPart
    {
        get { return fractionalPart; }
        set { fractionalPart = Math.Clamp(value, 0, 99); }
    }

    public string Currency
    {
        get { return currency; }
        set { currency = value; }
    }

    public Money()
    {
        Random random = new Random();
        Sign = '+';
        IntegerPart = random.Next(1, 1000);
        FractionalPart = random.Next(0, 100);
        Currency = currencies[random.Next(0, currencies.Length)];
    }

    public Money(string currency, char sign, int integerPart, int fractionalPart)
    {
        this.currency = currency;
        this.sign = sign;
        this.integerPart = Math.Max(0, integerPart);
        this.fractionalPart = Math.Clamp(fractionalPart, 0, 99);
    }

    public Money(Money other)
    {
        this.currency = other.Currency;
        this.sign = other.Sign;
        this.integerPart = other.IntegerPart;
        this.fractionalPart = other.FractionalPart;
    }

    public Money(string moneyString)
    {
        SetMoney(moneyString);
    }

    public string DisplayMoney()
    {
        return $"{Sign}{IntegerPart}.{FractionalPart:D2} {Currency}";
    }

    public void SetSign(char newSign)
    {
        if (newSign == '+' || newSign == '-')
        {
            Sign = newSign;
        }
        else
        {
            throw new ArgumentException("Invalid sign for the value.");
        }
    }

    public void SetIntegerPart(int newIntegerPart)
    {
        IntegerPart = newIntegerPart;
    }

    public void SetFractionalPart(int newFractionalPart)
    {
        FractionalPart = newFractionalPart;
    }

    public void SetCurrency(string newCurrency)
    {
        Currency = newCurrency;
    }

    public void SetMoney(string moneyString)
    {
        string[] parts = moneyString.Split(' ');
        if (parts.Length != 2)
        {
            throw new ArgumentException("Invalid money string format");
        }

        string[] numberParts = parts[0].Split('.');
        if (numberParts.Length != 2)
        {
            throw new ArgumentException("Invalid money string format");
        }

        if (numberParts[0].Length > 0)
        {
            Sign = numberParts[0][0];
            IntegerPart = int.Parse(numberParts[0].Substring(1));
        }
        else
        {
            Sign = '+';
            IntegerPart = 0;
        }

        FractionalPart = int.Parse(numberParts[1]);
        Currency = parts[1];
    }

    public void AddValue(char valueSign, int integerValue, int fractionalValue)
    {
        if (valueSign != '+' && valueSign != '-')
        {
            throw new ArgumentException("Invalid sign for the value.");
        }

        char resultSign;
        int resultIntegerPart;
        int resultFractionalPart;

        if (sign == valueSign)
        {
            resultSign = sign;
            resultIntegerPart = integerPart + integerValue;
            resultFractionalPart = fractionalPart + fractionalValue;
        }
        else
        {
            int thisValue = integerPart * 100 + fractionalPart;
            int otherValue = integerValue * 100 + fractionalValue;

            if (thisValue >= otherValue)
            {
                int difference = thisValue - otherValue;
                resultIntegerPart = difference / 100;
                resultFractionalPart = difference % 100;
                resultSign = sign;
            }
            else
            {
                resultSign = valueSign;
                int difference = otherValue - thisValue;
                resultIntegerPart = difference / 100;
                resultFractionalPart = difference % 100;
            }
        }

        if (resultFractionalPart >= 100)
        {
            resultIntegerPart += resultFractionalPart / 100;
            resultFractionalPart %= 100;
        }

        sign = (resultIntegerPart == 0 && resultFractionalPart == 0) ? '+' : resultSign;
        integerPart = resultIntegerPart;
        fractionalPart = resultFractionalPart;
    }

    public void AddMoney(Money other)
    {
        char valueSign = other.Sign;
        int valueIntegerPart = other.IntegerPart;
        int valueFractionalPart = other.FractionalPart;

        AddValue(valueSign, valueIntegerPart, valueFractionalPart);
    }

    public void SubtractValue(char valueSign, int valueIntegerPart, int valueFractionalPart)
    {
        if (valueSign == '-')
        {
            valueIntegerPart = -valueIntegerPart;
            valueFractionalPart = -valueFractionalPart;
        }

        integerPart += valueIntegerPart;
        fractionalPart += valueFractionalPart;

        while (fractionalPart < 0)
        {
            fractionalPart += 100;
            integerPart--;
        }

        sign = (integerPart < 0) ? '-' : '+';
        integerPart = Math.Abs(integerPart);
    }

    public void SubtractMoney(Money other)
    {
        char valueSign = other.Sign;
        int valueIntegerPart = other.IntegerPart;
        int valueFractionalPart = other.FractionalPart;

        SubtractValue(valueSign, valueIntegerPart, valueFractionalPart);
    }

    public bool Equals(Money other)
    {
        if (other == null)
        {
            return false;
        }

        return sign == other.sign
            && integerPart == other.integerPart
            && fractionalPart == other.fractionalPart
            && currency == other.currency;
    }

    public int CompareTo(Money other)
    {
        if (other == null)
        {
            throw new ArgumentNullException("Money object is null.");
        }

        if (Equals(other))
        {
            return 0;
        }

        if (sign == '+' && other.sign == '-')
        {
            return 1;
        }
        if (sign == '-' && other.sign == '+')
        {
            return -1;
        }

        int thisValue = integerPart * 100 + fractionalPart;
        int otherValue = other.integerPart * 100 + other.fractionalPart;

        if (thisValue > otherValue)
        {
            return 1;
        }
        else if (thisValue < otherValue)
        {
            return -1;
        }
        else
        {
            return string.Compare(currency, other.currency, StringComparison.Ordinal);
        }
    }

    public Money AddMoneyObjs(Money other)
    {
        if (other == null)
        {
            throw new ArgumentNullException("Money object is null.");
        }

        if (currency != other.currency)
        {
            throw new InvalidOperationException("Money objects have different currencies and cannot be added.");
        }

        char resultSign;
        int resultIntegerPart, resultFractionalPart;

        if (sign == '+' && other.sign == '+')
        {
            resultSign = '+';
            resultIntegerPart = integerPart + other.integerPart;
            resultFractionalPart = fractionalPart + other.fractionalPart;
        }
        else if (sign == '-' && other.sign == '-')
        {
            resultSign = '-';
            resultIntegerPart = integerPart + other.integerPart;
            resultFractionalPart = fractionalPart + other.fractionalPart;
        }
        else if (sign == '+' && other.sign == '-')
        {
            var otherValue = new Money(other);
            otherValue.ChangeSign();
            return SubtractMoneyObjs(otherValue);
        }
        else
        {
            return other.AddMoneyObjs(this);
        }

        if (resultFractionalPart >= 100)
        {
            resultIntegerPart += resultFractionalPart / 100;
            resultFractionalPart %= 100;
        }

        return new Money(currency, resultSign, resultIntegerPart, resultFractionalPart);
    }

    public Money SubtractMoneyObjs(Money other)
    {
        if (other == null)
        {
            throw new ArgumentNullException("Money object is null.");
        }

        if (currency != other.currency)
        {
            throw new InvalidOperationException("Money objects have different currencies and cannot be subtracted.");
        }

        char resultSign;
        int resultIntegerPart, resultFractionalPart;

        if (sign == '+' && other.sign == '-')
        {
            resultSign = '+';
            resultIntegerPart = integerPart + other.integerPart;
            resultFractionalPart = fractionalPart - other.fractionalPart;
        }
        else if (sign == '-' && other.sign == '+')
        {
            resultSign = '-';
            resultIntegerPart = integerPart - other.integerPart;
            resultFractionalPart = fractionalPart - other.fractionalPart;
        }
        else if (sign == '+' && other.sign == '+')
        {
            resultSign = (integerPart > other.integerPart || (integerPart == other.integerPart && fractionalPart >= other.fractionalPart)) ? '+' : '-';
            resultIntegerPart = Math.Abs(integerPart - other.integerPart);
            resultFractionalPart = Math.Abs(fractionalPart - other.fractionalPart);

            while (resultFractionalPart < 0)
            {
                resultIntegerPart--;
                resultFractionalPart += 100;
            }
        }
        else
        {
            resultSign = (other.integerPart > integerPart || (other.integerPart == integerPart && other.fractionalPart >= fractionalPart)) ? '+' : '-';
            resultIntegerPart = Math.Abs(other.integerPart - integerPart);
            resultFractionalPart = Math.Abs(other.fractionalPart - fractionalPart);

            while (resultFractionalPart < 0)
            {
                resultIntegerPart--;
                resultFractionalPart += 100;
            }
        }

        return new Money(currency, resultSign, resultIntegerPart, resultFractionalPart);
    }

    private void ChangeSign()
    {
        sign = (sign == '+') ? '-' : '+';
    }

    public Money ConvertToCurrency(string targetCurrency)
    {
        double exchangeRate = 1.0;

        switch (currency)
        {
            case "Dollars":
                if (targetCurrency == "Rubbles")
                    exchangeRate = 97.37;
                else if (targetCurrency == "Yen")
                    exchangeRate = 149.81;
                else if (targetCurrency == "Euros")
                    exchangeRate = 0.95;
                break;

            case "Rubbles":
                if (targetCurrency == "Dollars")
                    exchangeRate = 0.010;
                else if (targetCurrency == "Yen")
                    exchangeRate = 1.54;
                else if (targetCurrency == "Euros")
                    exchangeRate = 0.0097;
                break;

            case "Yen":
                if (targetCurrency == "Dollars")
                    exchangeRate = 0.0067;
                else if (targetCurrency == "Rubbles")
                    exchangeRate = 0.0063;
                else if (targetCurrency == "Euros")
                    exchangeRate = 0.0063;
                break;

            case "Euros":
                if (targetCurrency == "Dollars")
                    exchangeRate = 1.05;
                else if (targetCurrency == "Rubbles")
                    exchangeRate = 102.90;
                else if (targetCurrency == "Yen")
                    exchangeRate = 157.81;
                break;
        }

        double convertedValue = (integerPart * exchangeRate) + (fractionalPart * exchangeRate) / 100.0;
        char resultSign = (sign == '+') ? '+' : '-';
        int resultIntegerPart = (int)convertedValue;
        int resultFractionalPart = (int)((convertedValue - resultIntegerPart) * 100);

        return new Money(targetCurrency, resultSign, resultIntegerPart, resultFractionalPart);
    }

    static void Main(string[] args)
    {
        // Sample usage of the Money class
        Money money = new Money("Dollars", '+', 100, 50);
        Console.WriteLine("Original Money: " + money.DisplayMoney());

        money.AddValue('-', 25, 75);
        Console.WriteLine("Money after adding -25.75: " + money.DisplayMoney());

        Money otherMoney = new Money("Dollars", '+', 50, 25);
        money.AddMoney(otherMoney);
        Console.WriteLine("Money after adding another Money object: " + money.DisplayMoney());

        Money money2 = new Money("Euros", '-', 75, 30);
        Console.WriteLine("Another Money object: " + money2.DisplayMoney());

        Money sum = money.AddMoneyObjs(money2);
        Console.WriteLine("Sum of Money objects: " + sum.DisplayMoney());

        Money diff = money.SubtractMoneyObjs(money2);
        Console.WriteLine("Difference of Money objects: " + diff.DisplayMoney());

        Money converted = money.ConvertToCurrency("Rubbles");
        Console.WriteLine("Money converted to Rubbles: " + converted.DisplayMoney());
    }
}
