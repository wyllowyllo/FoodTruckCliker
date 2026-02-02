namespace Goods.Domain
{
    public readonly struct Currency 
    {
        public readonly int Value;

        public Currency(int value)
        {
            Value = value > 0 ? value : 0;
        }

        public static Currency operator +(Currency a, Currency b)
        {
            return new Currency(a.Value + b.Value);
        }

        public static Currency operator -(Currency a, Currency b)
        {
            return new Currency(a.Value - b.Value);
        }

        public static bool operator ==(Currency a, Currency b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(Currency a, Currency b)
        {
            return a.Value != b.Value;
        }
        
        public static implicit operator Currency(int value)
        {
            return new Currency(value);
        }
        
        public static explicit operator int(Currency currency)
        {
            return currency.Value;
        }
        
        
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
