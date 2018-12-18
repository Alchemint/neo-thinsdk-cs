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

        public readonly static string sc_musign = "0xd1756fe9d2dfc0e59d161b6ff16c2f4d28ac915c";
        public readonly static Hash160 musign = new Hash160(sc_musign);

        ///**PrivateNet**/
        //public readonly static string sc_sar4b = "0x203205eef5a81c093d04b5c591d8f2308598caa2";
        //public readonly static Hash160 sar4b = new Hash160(sc_sar4b);

        //public readonly static string sc_tokenized = "0x8e9530352c4e8b5c6c5eea5b82c7c46fd89cae6a";
        //public readonly static Hash160 tokenized = new Hash160(sc_tokenized);

        //public readonly static string sc_sar4c = "0x271dd92f34e5a3dab47b8a12eb0ca0a0cee89d4b";
        //public readonly static Hash160 sar4c = new Hash160(sc_sar4c);

        //public readonly static string sc_sdusd = "0x46d31e98371937464b44a60bed2ba027b036da59";
        //public readonly static Hash160 sdusd = new Hash160(sc_sdusd);

        //public readonly static string sc_oracle = "0xfde69a7dd2a1c948977fb3ce512158987c0e2197";
        //public readonly static Hash160 oracle = new Hash160(sc_oracle);

        //public readonly static string sc_sds = "0xfea5b131f9a9af5e97ec421001a69e2e8fe183ae";
        //public readonly static Hash160 sds = new Hash160(sc_sds);

        ////public readonly static string sc_cneo = "0xc074a05e9dcf0141cbe6b4b3475dd67baf4dcb60";
        ////public readonly static Hash160 cneo = new Hash160(sc_cneo);

        //public readonly static string sc_sneo = "0xa2aeccd6a7a7808b9959866f5463e5dcb911a578";
        //public readonly static Hash160 sneo = new Hash160(sc_sneo);

        //2018-12-17
        //public readonly static string sc_sneo = "0x358b1d9d2d489e074f33bd6979615f8b5828d0a8";
        //public readonly static Hash160 sneo = new Hash160(sc_sneo);

        /**TestNet**/
        //public readonly static string sc_sar4b = "0x7a173e56605ff5f47a1ffd7b071970eacf98ca25";
        //public readonly static Hash160 sar4b = new Hash160(sc_sar4b);

        //public readonly static string sc_tokenized = "0x5125f12e6b2b03f20873895dcf9c39aaf89940e9";
        //public readonly static Hash160 tokenized = new Hash160(sc_tokenized);

        //public readonly static string sc_sar4c = "0x56ee43dabb7e9b1bbfc2b059a459a165d8cacf5e";
        //public readonly static Hash160 sar4c = new Hash160(sc_sar4c);

        //public readonly static string sc_sdusd = "0x80fe8494d517a0c9caaabd5ddffd48593f67d70f";
        //public readonly static Hash160 sdusd = new Hash160(sc_sdusd);

        //public readonly static string sc_oracle = "0xfde69a7dd2a1c948977fb3ce512158987c0e2197";
        //public readonly static Hash160 oracle = new Hash160(sc_oracle);

        //public readonly static string sc_sds = "0x4b4deb4caad37fcfbadcfefc0bebfc869ff523ea";
        //public readonly static Hash160 sds = new Hash160(sc_sds);

        public readonly static string sc_cneo = "0xc074a05e9dcf0141cbe6b4b3475dd67baf4dcb60";
        public readonly static Hash160 cneo = new Hash160(sc_cneo);

        //public readonly static string sc_sneo = "0x789afc2bba96905d628cb41598c8f8cfcf213b58";
        //public readonly static Hash160 sneo = new Hash160(sc_sneo);

        /**SwNet**/
        //public readonly static string sc_sar4b = "0x0ac768a6c37c40048cb47e411c2e286488a0adda";
        //public readonly static Hash160 sar4b = new Hash160(sc_sar4b);

        //public readonly static string sc_tokenized = "0x0ac768a6c37c40048cb47e411c2e286488a0adda";
        //public readonly static Hash160 tokenized = new Hash160(sc_tokenized);

        //public readonly static string sc_sar4c = "0x52760e26c7828dfb810062f13240edbd29433a88";
        //public readonly static Hash160 sar4c = new Hash160(sc_sar4c);

        //public readonly static string sc_sdusd = "0x0a517e427e0de64e9fdbf66ee4dc94ef9e7dad9c";
        //public readonly static Hash160 sdusd = new Hash160(sc_sdusd);

        //public readonly static string sc_oracle = "0xd796fe60bce275135febe1ff900d9b68d83ca560";
        //public readonly static Hash160 oracle = new Hash160(sc_oracle);

        //public readonly static string sc_sds = "0xfea5b131f9a9af5e97ec421001a69e2e8fe183ae";
        //public readonly static Hash160 sds = new Hash160(sc_sds);

        //public readonly static string sc_cneo = "0xce7b4dd09b23530baa8d5c9f5e8423eb2dc5476e";
        //public readonly static Hash160 cneo = new Hash160(sc_cneo);

        //** Mainnet** 
        public readonly static string sc_sar4b = "0x203205eef5a81c093d04b5c591d8f2308598caa2";
        public readonly static Hash160 sar4b = new Hash160(sc_sar4b);

        public readonly static string sc_tokenized = "0x8e9530352c4e8b5c6c5eea5b82c7c46fd89cae6a";
        public readonly static Hash160 tokenized = new Hash160(sc_tokenized);

        public readonly static string sc_sar4c = "0x271dd92f34e5a3dab47b8a12eb0ca0a0cee89d4b";
        public readonly static Hash160 sar4c = new Hash160(sc_sar4c);

        public readonly static string sc_sdusd = "0x7146278a76c33fc6bb870fcaa428e3cdb16809ac";
        public readonly static Hash160 sdusd = new Hash160(sc_sdusd);

        public readonly static string sc_oracle = "0xfde69a7dd2a1c948977fb3ce512158987c0e2197";
        public readonly static Hash160 oracle = new Hash160(sc_oracle);

        public readonly static string sc_sds = "0x6fad54d8cc692fc808fd97a207836a846c217705";
        public readonly static Hash160 sds = new Hash160(sc_sds);

        public readonly static string sc_sneo = "0xa2aeccd6a7a7808b9959866f5463e5dcb911a578";
        public readonly static Hash160 sneo = new Hash160(sc_sneo);

        public readonly static string api_local = "http://localhost:20332";

        //public readonly static string api = "https://api.nel.group/api/testnet";

        //public readonly static string api = "https://api.nel.group/api/mainnet";

       //public readonly static string api = "http://api.alchemint.io/api/pri";

       //public readonly static string api = "http://api.alchemint.io/api/testnet";

       public readonly static string api = "http://api.alchemint.io/api/mainnet";

        public const string testwif = "";

        public const string testwif_admin = "";

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
