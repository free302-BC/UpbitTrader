using Universe.Coin.TradeLogic.Model;

namespace Universe.Coin.TradeLogic.Model
{
    public interface IAccount : IApiModel
    {
        decimal AvgBuyPrice { get; set; }
        bool AvgBuyPriceModified { get; set; }
        decimal Balance { get; set; }
        string Currency { get; set; }
        decimal Locked { get; set; }
        string UnitCurrency { get; set; }

        string ToString();
    }
}