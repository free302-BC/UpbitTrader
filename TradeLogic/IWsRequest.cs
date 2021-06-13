namespace Universe.Coin.TradeLogic
{
    public interface IWsRequest
    {
        IWsRequest ToDefault();

        void AddTrade(CurrencyId currency, CoinId coin, (string key, object value) option = default);
        void AddOrderbook(CurrencyId currency, CoinId coin, (string key, object value) option = default);
        void AddTicker(CurrencyId currency, CoinId coin, (string key, object value) option = default);

        //void Add(string type, string market, (string key, object value) option);

        string ToJson();
        byte[] ToJsonBytes();
    }
}