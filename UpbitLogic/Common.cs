﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Universe.Coin.Upbit
{
    using ApiDic = Dictionary<ApiId, (string Path, string Method, string Comment)>;
    using CoinDic = Dictionary<CoinId, (string English, string Korean)>;
    using CurrencyDic = Dictionary<CurrencyId, HashSet<CoinId>>;

    public enum ApiId
    {
        APIKeyInfo, AccountInfo,
        AccountWallet,
        CandleDays, CandleMinutes, CandleMonth, CandleWeeks,
        DepositCoinAddress, DepositCoinAddresses, DepositGenerateCoinAddress, DepositInfo, DepositInfoAll,
        MarketInfoAll,
        OrderCancel, OrderChance, OrderInfo, OrderInfoAll, OrderNew, OrderOrderbook,
        TradeTicker, TradeTicks,
        WithdrawChance, WithdrawCoin, WithdrawInfo, WithdrawInfoAll, WithdrawKrw
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

    ///// <summary>
    ///// string ~ CoinId conversion
    ///// </summary>
    //public readonly struct CoinID
    //{
    //    public readonly CoinId Id;
    //    public CoinID(CoinId cid) => Id = cid;
    //    public static implicit operator CoinId(CoinID cid) => cid.Id;
    //    public static implicit operator CoinID(CoinId cid) => new CoinID(cid);
    //    public static implicit operator CoinID(string cid) => new CoinID(Enum.Parse<CoinId>(cid));
    //    public static implicit operator string(CoinID cid) => cid.Id.ToString();
    //    public static CoinID BTC = CoinId.BCH;
    //}

    //public class CoinId
    //{
    //    public string Coin;
    //    public CoinId(string coinId) => Coin = coinId;
    //    public static implicit operator string(CoinId id) =>id.Coin;
    //    public static implicit operator CoinId(string id) => new CoinId(id);
    //    public static CoinId Default = new("BTC");
    //}

    //public class ApiDic : Dictionary<ApiId, (string Path, string Method, string Comment)> { }
    //public class CoinDic : Dictionary<CoinId, (string English, string Korean)> { }
    //public class CurrencyDic : Dictionary<CurrencyId, HashSet<CoinId>> { }

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
