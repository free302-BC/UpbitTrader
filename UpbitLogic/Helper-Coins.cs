using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.IO;
using System.Text.Unicode;
using System.Text.Encodings.Web;
using Universe.Utility;
using Universe.CryptoLogic;
using System.Text.Json;

namespace Universe.Coin.Upbit
{
    using CoinDic = Dictionary<string, (string English, string Korean)>;
    using CurrencyDic = Dictionary<CurrencyId, string[]>;

    public partial class Helper
    {
        const string _CoinNameFile = "coin-name.json";
        const string _MarketCoinsFile = "market-coins.json";
        public static readonly CoinDic CoinNames;
        public static readonly CurrencyDic MarketCoins;
        public static void buildCoinNameJson()
        {
            //string[] _conisKRW = { }, _englishKRW = { }, _koreanKRW = { };
            //string[] _coinsBTC = { }, _englishBTC = { }, _koreanBTC = { };
            string[] _coinsUSDT = { };

            var opt = JsonSerializer.Deserialize<JsonSerializerOptions>(File.ReadAllText(_jsonOptionFile))!;           
            opt.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.HangulSyllables);
            var coins = new Dictionary<string, _cnp>();
            for (int i = 0; i < _conisKRW.Length; i++) coins[_conisKRW[i]] = (_englishKRW[i], _koreanKRW[i]);
            for (int i = 0; i < _coinsBTC.Length; i++) coins[_coinsBTC[i]] = (_englishBTC[i], _koreanBTC[i]);
            File.WriteAllText(_CoinNameFile, JsonSerializer.Serialize(coins, opt));

            var markets = new CurrencyDic();
            markets.Add(CurrencyId.KRW, _conisKRW);
            markets.Add(CurrencyId.BTC, _coinsBTC);
            markets.Add(CurrencyId.USDT, _coinsUSDT);
            File.WriteAllText(_MarketCoinsFile, JsonSerializer.Serialize(markets, opt));
        }

        class _cnp//coin name pair
        {
            public string English;
            public string Korean;
            public static implicit operator _cnp((string, string) p) => new _cnp { English = p.Item1, Korean = p.Item2 };
        }

        public static string[] _conisKRW =
        {
            "BTC","ETH","NEO","MTL","LTC","XRP","ETC","OMG","SNT","WAVES","XEM","QTUM","LSK","STEEM","XLM","ARDR",
            "KMD","ARK","STORJ","GRS","REP","EMC2","ADA","SBD","POWR","BTG","ICX","EOS","TRX","SC","IGNIS","ONT",
            "ZIL","POLY","ZRX","LOOM","BCH","ADX","BAT","IOST","DMT","RFR","CVC","IQ","IOTA","MFT","ONG","GAS","UPP",
            "ELF","KNC","BSV","THETA","EDR","QKC","BTT","MOC","ENJ","TFUEL","MANA","ANKR","AERGO","ATOM","TT","CRE",
            "SOLVE","MBL","TSHP","WAXP","HBAR","MED","MLK","STPT","ORBS","VET","CHZ","PXL","STMX","DKA","HIVE","KAVA",
            "AHT","LINK","XTZ","BORA","JST","CRO","TON","SXP","LAMB","HUNT","MARO","PLA","DOT","SRM","MVL","PCI","STRAX",
            "AQT","BCHA","GLM","QTCON","SSX","META","OBSR","FCT2","LBC","CBK","SAND","HUM","DOGE","STRK","PUNDIX","FLOW",
            "DAWN","AXS","STX"
        };
        public static string[] _englishKRW =
        {
            "Bitcoin","Ethereum","NEO","Metal","Litecoin","Ripple","Ethereum Classic","OmiseGo","Status Network Token","Waves","NEM","Qtum","Lisk","Steem","Lumen","Ardor","Komodo","Ark","Storj","Groestlcoin","Augur","Einsteinium","Ada","SteemDollars","Power ledger","Bitcoin Gold","Icon","EOS","TRON","Siacoin","Ignis","Ontology","Zilliqa","Polymath","0x Protocol","Loom Network","Bitcoin Cash","AdEx","Basic Attention Token","IOST","DMarket","Refereum","Civic","Everipedia","IOTA","Mainframe","ONG","GAS","Sentinel Protocol","aelf","Kyber Network","Bitcoin SV","Theta Token","Endor","QuarkChain","BitTorrent","Moss Coin","Enjin","Theta Fuel","Decentraland","Ankr","Aergo","Cosmos","Thunder Token","Carry Protocol","Solve.Care","MovieBloc","12SHIPS","WAX","Hedera Hashgraph","MediBloc","MiL.k","Standard Tokenization Protocol","Orbs","VeChain","Chiliz","PIXEL","StormX","dKargo","Hive","Kava","AhaToken","Chainlink","Tezos","BORA","JUST","Crypto.com Chain","TON","Swipe","Lambda","HUNT","Maro","PlayDapp","Polkadot","Serum","MVL","PayCoin","Stratis","Alpha Quark Token","Bitcoin Cash ABC","Golem","Quiztok","SOMESING","Metadium","Observer","FirmaChain","LBRY Credits","Cobak Token","The Sandbox","Humanscape","Dogecoin","Strike","Pundi X","Flow","Dawn Protocol","Axie Infinity","Stacks"
        };
        public static string[] _koreanKRW =
        {
            "비트코인","이더리움","네오","메탈","라이트코인","리플","이더리움클래식","오미세고","스테이터스네트워크토큰","웨이브","넴","퀀텀","리스크","스팀","스텔라루멘","아더","코모도","아크","스토리지","그로스톨코인","어거","아인스타이늄","에이다","스팀달러","파워렛저","비트코인골드","아이콘","이오스","트론","시아코인","이그니스","온톨로지","질리카","폴리매쓰","제로엑스","룸네트워크","비트코인캐시","애드엑스","베이직어텐션토큰","아이오에스티","디마켓","리퍼리움","시빅","에브리피디아","아이오타","메인프레임","온톨로지가스","가스","센티넬프로토콜","엘프","카이버네트워크","비트코인에스브이","쎄타토큰","엔도르","쿼크체인","비트토렌트","모스코인","엔진코인","쎄타퓨엘","디센트럴랜드","앵커","아르고","코스모스","썬더토큰","캐리프로토콜","솔브케어","무비블록","트웰브쉽스","왁스","헤데라해시그래프","메디블록","밀크","에스티피","오브스","비체인","칠리즈","픽셀","스톰엑스","디카르고","하이브","카바","아하토큰","체인링크","테조스","보라","저스트","크립토닷컴체인","톤","스와이프","람다","헌트","마로","플레이댑","폴카닷","세럼","엠블","페이코인","스트라티스","알파쿼크","비트코인캐시에이비씨","골렘","퀴즈톡","썸씽","메타디움","옵저버","피르마체인","엘비알와이크레딧","코박토큰","샌드박스","휴먼스케이프","도지코인","스트라이크","펀디엑스","플로우","던프로토콜","엑시인피니티","스택스"
        };
        public static string[] _coinsBTC =
        {
            "ETH","LTC","XRP","ETC","OMG","CVC","DGB","SC","SNT","WAVES","NMR","XEM","LBC","QTUM","NXT","BAT","LSK",
            "RDD","STEEM","DOGE","BNT","XLM","ARDR","KMD","ARK","ADX","SYS","ANT","STORJ","GRS","REP","RLC","EMC2",
            "ADA","MANA","SBD","RCN","POWR","DNT","IGNIS","ZRX","TRX","TUSD","LRC","DMT","POLY","PRO","BCH","MFT",
            "LOOM","RFR","RVN","BFT","GO","UPP","ENJ","EDR","MTL","PAX","MOC","ZIL","BSV","IOST","NCASH","DENT","ELF",
            "BTT","VITE","IOTX","SOLVE","NKN","META","ANKR","CRO","FSN","ORBS","AERGO","PI","ATOM","STPT","LAMB","EOS",
            "LUNA","DAI","MKR","BORA","TSHP","WAXP","MED","MLK","PXL","VET","CHZ","FX","OGN","ITAM","XTZ","HIVE","HBD",
            "OBSR","DKA","STMX","AHT","PCI","RINGX","LINK","KAVA","JST","CHR","DAD","TON","CTSI","DOT","COMP","SXP",
            "HUNT","ONIT","CRV","ALGO","RSR","OXT","PLA","MARO","SAND","SUN","SRM","QTCON","MVL","GXC","AQT","AXS",
            "STRAX","BCHA","GLM","FCT2","SSX","FIL","UNI","BASIC","INJ","PROM","VAL","PSG","JUV","CBK","FOR","BFC","LINA",
            "HUM","PICA","CELO","IQ","STX","NEAR","AUCTION","DAWN","FLOW","STRK","PUNDIX","GRT","SNX"
        };
        public static string[] _englishBTC =
        {
            "Ethereum","Litecoin","Ripple","Ethereum Classic","OmiseGo","Civic","DigiByte","Siacoin","Status Network Token","Waves","Numeraire","NEM","LBRY Credits","Qtum","Nxt","Basic Attention Token","Lisk","ReddCoin","Steem","Dogecoin","Bancor","Lumen","Ardor","Komodo","Ark","AdEx","SysCoin","Aragon","Storj","Groestlcoin","Augur","iEx.ec","Einsteinium","Ada","Decentraland","SteemDollars","Ripio Credit Network","Power ledger","district0x","Ignis","0x Protocol","TRON","TrueUSD","Loopring","DMarket","Polymath","Propy","Bitcoin Cash","Mainframe","Loom Network","Refereum","Ravencoin","BnkToTheFuture","GoChain","Sentinel Protocol","Enjin","Endor","Metal","Paxos Standard","Moss Coin","Zilliqa","Bitcoin SV","IOST","Nucleus Vision","Dent","aelf","BitTorrent","Vite","IoTeX","Solve.Care","NKN","Metadium","Ankr","Crypto.com Chain","Fusion","Orbs","Aergo","Plian","Cosmos","Standard Tokenization Protocol","Lambda","EOS","Luna","Dai","Maker","BORA","12SHIPS","WAX","MediBloc","MiL.k","PIXEL","VeChain","Chiliz","Function X","Origin Protocol","ITAM","Tezos","Hive","Hive Dollar","Observer","dKargo","StormX","AhaToken","PayCoin","RINGX","Chainlink","Kava","JUST","Chromia","DAD","TON","Cartesi","Polkadot","Compound","Swipe","HUNT","ONBUFF","Curve","Algorand","Reserve Rights","Orchid","PlayDapp","Maro","The Sandbox","SUN","Serum","Quiztok","MVL","GXChain","Alpha Quark Token","Axie Infinity","Stratis","Bitcoin Cash ABC","Golem","FirmaChain","SOMESING","Filecoin","Uniswap","Basic","Injective Protocol","Prometeus","Validity","Paris Saint-Germain Fan Token","Juventus Fan Token","Cobak Token","ForTube","Bifrost","Linear","Humanscape","Pica","Celo","Everipedia","Stacks","NEAR Protocol","Bounce","Dawn Protocol","Flow","Strike","Pundi X","The Graph","Synthetix"
        };
        public static string[] _koreanBTC =
        {
            "이더리움","라이트코인","리플","이더리움클래식","오미세고","시빅","디지바이트","시아코인","스테이터스네트워크토큰","웨이브","뉴메레르","넴","엘비알와이크레딧","퀀텀","엔엑스티","베이직어텐션토큰","리스크","레드코인","스팀","도지코인","뱅코르","스텔라루멘","아더","코모도","아크","애드엑스","시스코인","아라곤","스토리지","그로스톨코인","어거","아이젝","아인스타이늄","에이다","디센트럴랜드","스팀달러","리피오크레딧네트워크","파워렛저","디스트릭트0x","이그니스","제로엑스","트론","트루USD","루프링","디마켓","폴리매쓰","프로피","비트코인캐시","메인프레임","룸네트워크","리퍼리움","레이븐코인","비에프토큰","고체인","센티넬프로토콜","엔진코인","엔도르","메탈","팩소스스탠다드","모스코인","질리카","비트코인에스브이","아이오에스티","뉴클리어스비전","덴트","엘프","비트토렌트","바이트토큰","아이오텍스","솔브케어","엔케이엔","메타디움","앵커","크립토닷컴체인","퓨전","오브스","아르고","플리안","코스모스","에스티피","람다","이오스","루나","다이","메이커","보라","트웰브쉽스","왁스","메디블록","밀크","픽셀","비체인","칠리즈","펑션엑스","오리진프로토콜","아이텀","테조스","하이브","하이브달러","옵저버","디카르고","스톰엑스","아하토큰","페이코인","링엑스","체인링크","카바","저스트","크로미아","다드","톤","카르테시","폴카닷","컴파운드","스와이프","헌트","온버프","커브","알고랜드","리저브라이트","오키드","플레이댑","마로","샌드박스","썬","세럼","퀴즈톡","엠블","지엑스체인","알파쿼크","엑시인피니티","스트라티스","비트코인캐시에이비씨","골렘","피르마체인","썸씽","파일코인","유니스왑","베이직","인젝티브프로토콜","프로메테우스","밸리디티","파리생제르맹","유벤투스","코박토큰","포튜브","바이프로스트","리니어파이낸스","휴먼스케이프","피카","셀로","에브리피디아","스택스","니어프로토콜","바운스토큰","던프로토콜","플로우","스트라이크","펀디엑스","그래프","신세틱스"
        };

    }//class
}
