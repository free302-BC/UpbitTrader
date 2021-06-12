namespace Universe.Coin.TradeLogic
{
    public interface IWsRequest
    {
        IWsRequest BuildDefault();

        void Add(string type, string market);
        void Add(string type, string market, (string key, object value) option);
        string ToJson();
        byte[] ToJsonBytes();
    }
}