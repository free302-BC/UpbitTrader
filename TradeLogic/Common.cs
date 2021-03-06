using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Universe.Coin.TradeLogic
{
    /// <summary>
    /// 
    /// </summary>
    public enum TradeEvent 
    { 
        Ticker, //현재가
        Order,  //주문 orderbook
        Trade   //체결
    }
    
    /// <summary>
    /// 매수시 결제에 사용되는 화폐
    /// </summary>
    public enum CurrencyId { KRW, USDT, BTC }

    public enum CoinId : uint
    {
        BTC, ETH, NEO, MTL, LTC, XRP, ETC, OMG, SNT, WAVES, XEM, QTUM, LSK, STEEM, XLM, ARDR, KMD, ARK, STORJ,
        GRS, REP, EMC2, ADA, SBD, POWR, BTG, ICX, EOS, TRX, SC, IGNIS, ONT, ZIL, POLY, ZRX, LOOM, BCH, ADX,
        BAT, IOST, DMT, RFR, CVC, IQ, IOTA, MFT, ONG, GAS, UPP, ELF, KNC, BSV, THETA, EDR, QKC, BTT, MOC, ENJ,
        TFUEL, MANA, ANKR, AERGO, ATOM, TT, CRE, SOLVE, MBL, TSHP, WAXP, HBAR, MED, MLK, STPT, ORBS, VET, CHZ,
        PXL, STMX, DKA, HIVE, KAVA, AHT, LINK, XTZ, BORA, JST, CRO, TON, SXP, LAMB, HUNT, MARO, PLA, DOT, SRM,
        MVL, PCI, STRAX, AQT, BCHA, GLM, QTCON, SSX, META, OBSR, FCT2, LBC, CBK, SAND, HUM, DOGE, STRK, PUNDIX,
        FLOW, DAWN, AXS, STX, DGB, NMR, NXT, RDD, BNT, SYS, ANT, RLC, RCN, DNT, TUSD, LRC, PRO, RVN, BFT, GO,
        PAX, NCASH, DENT, VITE, IOTX, NKN, FSN, PI, LUNA, DAI, MKR, FX, OGN, ITAM, HBD, RINGX, CHR, DAD, CTSI,
        COMP, ONIT, CRV, ALGO, RSR, OXT, SUN, GXC, FIL, UNI, BASIC, INJ, PROM, VAL, PSG, JUV, FOR, BFC, LINA,
        PICA, CELO, NEAR, AUCTION, GRT, SNX
    }
    
    /// <summary>
    /// 현재가의 추세
    /// </summary>
    public enum TickerDir
    {
        E,  //even
        F,  //down
        R   //rise
    }

    /// <summary>
    /// 거래 방향
    /// </summary>
    public enum TradeTickDir : int
    {
        A = -1,  //ask 매도
        B = 1  //bid 매수
    }

    /// <summary>
    /// 알고리즘 계산 결과
    /// </summary>
    public enum TimingSignal
    {
        N,
        Buy = -1,  //매수 신호
        Hold = -2, //매수 상태 유지
        Sell = +1, //매도 신호
    }

    public enum CandleUnit
    {
        None = 0, M1 = 1, M3 = 3, M5 = 5, M15 = 15, M10 = 10, M30 = 30, M60 = 60, M240 = 240,
        DAY = 1440, WEEK = 10080, MONTH = 302400
    }

    /// <summary>
    /// moving average 윈도우 함수
    /// </summary>
    public enum WindowFunction : int
    {
        None, Identical, Linear, Gaussian
    }

    /// <summary>
    /// 웹 API 호출 결과
    /// </summary>
    public enum ApiResultCode
    {
        Ok = 0,             //정상결과, 1개 이상의 아이템
        OkEmpty = 1,        //정상결과, 0개 아이템
        TooMany = -1,       //비정상, 너무 빠른 요청
        UnknowError = -2    //그외의 에러
    }

    public enum HttpMethod { GET, POST, DELETE }
    
    public enum ApiId
    {
        None = 0,
        APIKeyInfo, AccountInfo,
        AccountWallet,
        CandleDays, CandleMinutes, CandleMonth, CandleWeeks,
        DepositCoinAddress, DepositCoinAddresses, DepositGenerateCoinAddress, DepositInfo, DepositInfoAll,
        MarketInfoAll,
        OrderCancel, OrderChance, OrderInfo, OrderInfoAll, OrderNew, OrderOrderbook,
        TradeTicker, TradeTicks,
        WithdrawChance, WithdrawCoin, WithdrawInfo, WithdrawInfoAll, WithdrawKrw
    }

    public class KeyPair
    {
        public KeyPair(string a, string s)
        {
            Access = a;
            Secret = s;
        }
        public string Access;
        public string Secret;
        public static implicit operator (string access, string secret)(KeyPair pair) => (pair.Access, pair.Secret);
        public static implicit operator KeyPair((string access, string secret) key) => new KeyPair(key.access, key.secret);
    }


}
