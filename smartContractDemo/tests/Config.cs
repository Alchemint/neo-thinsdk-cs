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

        /**PrivateNet**/
        public readonly static string sc_sar4b = "0x4925720b1955ccbf413bb275a44f1e6487e766cd";
        public readonly static Hash160 sar4b = new Hash160(sc_sar4b);

        public readonly static string sc_tokenized = "0x1c65c45a35fb30a95c128cf9774ee70dface1eb9";
        public readonly static Hash160 tokenized = new Hash160(sc_tokenized);

        public readonly static string sc_sar4c = "0x4fe8e51c4aa92a4e9714a33d4df38f026cd783cb";
        public readonly static Hash160 sar4c = new Hash160(sc_sar4c);

        public readonly static string sc_sdusd = "0x3569cfd93512e8bf3d7ab1980d5c03a767455cbe";
        public readonly static Hash160 sdusd = new Hash160(sc_sdusd);

        public readonly static string sc_oracle = "0x2eaef11fa90014ccf4ae8bfabc0f58e7fc8bd591";
        public readonly static Hash160 oracle = new Hash160(sc_oracle);

        public readonly static string sc_sds = "0xfea5b131f9a9af5e97ec421001a69e2e8fe183ae";
        public readonly static Hash160 sds = new Hash160(sc_sds);

        public readonly static string sc_cneo = "0xc074a05e9dcf0141cbe6b4b3475dd67baf4dcb60";
        public readonly static Hash160 cneo = new Hash160(sc_cneo);

        /**TestNet**/
        //public readonly static string sc_sar4b = "0x0ac768a6c37c40048cb47e411c2e286488a0adda";
        //public readonly static Hash160 sar4b = new Hash160(sc_sar4b);

        //public readonly static string sc_tokenized = "0x5949f9f4e8ce35483a04bd387c5f379f5738178e";
        //public readonly static Hash160 tokenized = new Hash160(sc_tokenized);

        //public readonly static string sc_sar4c = "0x90d7391c4562a81431e4e18f1034af5fbdaea254";
        //public readonly static Hash160 sar4c = new Hash160(sc_sar4c);

        //public readonly static string sc_sdusd = "0x33d7b9332110a66c6c16d71e80a5ba0dddb4207c";
        //public readonly static Hash160 sdusd = new Hash160(sc_sdusd);

        //public readonly static string sc_oracle = "0xe7bce3dde514813762b44a11bb5767f343dafb22";
        //public readonly static Hash160 oracle = new Hash160(sc_oracle);

        //public readonly static string sc_sds = "0x4b4deb4caad37fcfbadcfefc0bebfc869ff523ea";
        //public readonly static Hash160 sds = new Hash160(sc_sds);

        //public readonly static string sc_cneo = "0xc074a05e9dcf0141cbe6b4b3475dd67baf4dcb60";
        //public readonly static Hash160 cneo = new Hash160(sc_cneo);


        public readonly static string api_local = "http://localhost:20332";

        //public readonly static string api = "https://api.nel.group/api/testnet";

        //public readonly static string api = "https://api.nel.group/api/mainnet";

        public readonly static string api = "http://api.alchemint.io/api/privatenet";

        //public readonly static string api = "http://api.alchemint.io/api/testnet";

        //public readonly static string api = "http://api.alchemint.io/api/mainnet";

        public const string rpc = "http://47.52.188.214:10332";

        public const string testwif = "KzprnMDQHhK7jnJ3dNNq5C2AfJdy58oGyphnZtc6t78NE26nhq7S";

        public const string testwif_admin = "KzprnMDQHhK7jnJ3dNNq5C2AfJdy58oGyphnZtc6t78NE26nhq7S";

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
