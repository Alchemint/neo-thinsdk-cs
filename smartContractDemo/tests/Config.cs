using System;
using System.Collections.Generic;
using System.Text;
using ThinNeo;
namespace smartContractDemo.tests
{
    class Config
    {
        public readonly static string superadminAddress = "ALjSnMZidJqd18iQaoCgFun6iqWRm2cVtj";
        public  static string test_wif = "KwwJMvfFPcRx2HSgQRPviLv4wPrxRaLk7kfQntkH8kCXzTgAts8t";


        public readonly static Hash160 dapp_nnc = new Hash160("0x12329843449f29a66fb05974c2fb77713eb1689a");//nnc 合约代码
        public readonly static Hash160 dapp_sgas  = new Hash160("0x07185f19053c0f8a064921d7ca798a5e6ba957cb");//sgas 新合约地址
        public static readonly Hash160 sc_nns = new Hash160("0x77e193f1af44a61ed3613e6e3442a0fc809bb4b8");//nns 跳板合约地址
        public static readonly Hash160 domaincenterhash = new Hash160("0x7754b7dbacd840f7b9f1b02277d8745901df8a22");//nns 域名中心合约地址


        public static readonly Hash160 dapp_multisign = new Hash160("0x4c0f57b61d997297560190b1e397fe6d58fce94a");  //应用合约多签验证测试
        public const string id_GAS = "0x602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7";


        public readonly static string api_local = "http://localhost:20332";

        public readonly static string api = "https://api.nel.group/api/testnet";

        //public readonly static string api = "https://api.nel.group/api/mainnet";

        //public const string api = "http://192.168.0.101:59908/api/privatenet";

        public const string rpc = "http://192.168.0.105:10332";

        //wallet7 KxqSjrFYMjxzD7cdxLXePr18NreEYKgEwNVfWEzkEu9FCD9cRhG5
        //wallet8 L46VDpr5F8vMJ3M3pM5N9qkpaCg21t1TjY1DF11KVvv98GCoLEL3
        //wallet5 Kz4is5qddLo5f6Ek8uv2GzguJzWABKjbpJdP6nMhXds16oJstZsG
        //wallet1 KzprnMDQHhK7jnJ3dNNq5C2AfJdy58oGyphnZtc6t78NE26nhq7S
        //wallet2 KxwkPdCtioE8vKfLETqK2UfMWX4XaooP8rP85wRrAiFzDa3Ydkt3
        //wallet3 KxkmnsoANJT8ygPaiYTe9houe4XD6bNzu1yHBYs5NUr9WL6Mufyg
        //gxl KxtvHw6fBeEj1TQEd7revdhyETPC8w8jEVE3yVthhLyWQnxMm6tr
        //wallet1 test L5a9Hihm4Lu46deJ6mfBRAvPPitJTyWK6g8yRP1iFPpGMBzecQcS
        //walletgxl KxtvHw6fBeEj1TQEd7revdhyETPC8w8jEVE3yVthhLyWQnxMm6tr
        public const string testwif = "KzprnMDQHhK7jnJ3dNNq5C2AfJdy58oGyphnZtc6t78NE26nhq7S";

        public const string id_NEO = "0xc56f33fc6ecfcd0c225c4ab356fee59390af8560be0e930faebe74a6daff7c9b";

        public readonly static string root = "wei";

        public static void changeWif(string wif)
        {
            if (wif.Length == 52)
                Config.test_wif = wif;
        }
        
        public static void LogLn(string content,ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(content);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void Log(string content, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(content);
            Console.ForegroundColor = ConsoleColor.White;
        }

    }
}
