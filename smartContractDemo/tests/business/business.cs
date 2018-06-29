using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ThinNeo;
using System.IO;
using System.Linq;
using smartContractDemo.tests;

namespace smartContractDemo
{
    class businessTest : ITest
    {
        public string Name => "Business 合约测试";

        public string ID => "business";
        byte[] prikey;
        public string address;
        byte[] scripthash;
        byte[] pubkey;
        Hash160 reg_sc;//注册器合约地址
        public delegate Task testAction();
        public Dictionary<string, testAction> infos = null;// = new Dictionary<string, testAction>();
        string[] submenu;
        byte[] n55contract;
        public static ThinNeo.Hash256 lasttxid;

        void subPrintLine(string line)
        {
            Console.WriteLine("    " + line);
        }

        public businessTest()
        {
            this.initMenu();
        }

        private void initMenu()
        {
            infos = new Dictionary<string, testAction>();
            infos["openSAR4B"] = test_openSAR4B;
            infos["getSAR4B"] = test_getSAR4B;
            infos["reserve"] = test_reserve;
            infos["expande"] = test_expande;
            infos["contract"] = test_contract;
            infos["withdraw"] = test_withdraw;
            infos["setConfig"] = test_setConfig;
            infos["setAccount"] = test_setAccount;
            infos["getConfig"] = test_getConfig;

            this.submenu = new List<string>(infos.Keys).ToArray();
        }

        public delegate void Method(ThinNeo.ScriptBuilder sb);//第一步：定义委托类型

        public async Task Demo()
        {
            //得到合约代码
            var urlgetscript = Helper.MakeRpcUrl(Config.api, "getcontractstate", new MyJson.JsonNode_ValueString(business_common.sc));
            var resultgetscript = await Helper.HttpGet(urlgetscript);
            var _json = MyJson.Parse(resultgetscript).AsDict();
            var _resultv = _json["result"].AsList()[0].AsDict();

            n55contract = ThinNeo.Helper.HexString2Bytes(_resultv["script"].AsString());

            //Console.WriteLine("reg=" + _resultv["script"].AsString());

            showMenu();

            prikey = ThinNeo.Helper.GetPrivateKeyFromWIF(Config.testwif);
            pubkey = ThinNeo.Helper.GetPublicKeyFromPrivateKey(prikey);
            address = ThinNeo.Helper.GetAddressFromPublicKey(pubkey);
            scripthash = ThinNeo.Helper.GetPublicKeyHashFromAddress(address);

            while (true)
            {
                var line = Console.ReadLine().Replace(" ", "").ToLower();
                if (line == "?" || line == "？")
                {
                    showMenu();
                }
                else if (line == "")
                {
                    continue;
                }
                else if (line == "0")
                {
                    return;
                }
                else//get .test's info
                {
                    var id = int.Parse(line) - 1;
                    var key = submenu[id];
                    subPrintLine("[begin]" + key);
                    try
                    {
                        await infos[key]();
                    }
                    catch (Exception err)
                    {
                        subPrintLine(err.Message);
                    }
                    subPrintLine("[end]" + key);
                }
            }
        }

        void showMenu()
        {
            for (var i = 0; i < submenu.Length; i++)
            {
                var key = submenu[i];
                subPrintLine((i + 1).ToString() + ":" + key);
            }
            subPrintLine("0:exit");
            subPrintLine("?:show this");
        }

        async Task test_not_implement_yet()
        {
            subPrintLine("尚未实现");
        }

        async Task test_setAccount()
        {
            var addr = ThinNeo.Helper.GetAddressFromScriptHash(business_common.sc_wneo);
            Console.WriteLine("address:" + addr);

            var result = await business_common.api_SendTransaction(prikey, business_common.sc_wneo, "setAccount", "(addr)" + addr);
            subPrintLine(result);
        }

        //初始化
        async Task test_openSAR4B()
        {
            Console.WriteLine("Input target asset:");
            string name = Console.ReadLine();

            Console.WriteLine("Input target symbol:");
            string symbol = Console.ReadLine();

            var result = await business_common.api_SendTransaction(prikey, business_common.sc_wneo, "openSAR4B",
               "(str)" + name,
                "(str)" + symbol,
                "(int)" + 8,
                "(addr)" + this.address);
            subPrintLine(result);
        }

        async Task test_getSAR4B()
        {
            Console.WriteLine("Input target asset:");
            string name = Console.ReadLine();

            Console.WriteLine("Input address:");
            string address = Console.ReadLine();

            var result = await business_common.api_InvokeScript(business_common.sc_wneo, "getSAR4B", "(str)" + name);
            business_common.ResultItem item = result.value;
            business_common.ResultItem[] items = item.subItem[0].subItem;

            if (items != null)
            {
                Console.WriteLine("name:"+items[0].AsString());
                Console.WriteLine("totalSupply:" + items[1].AsInteger());
                Console.WriteLine("symbol:" + items[2].AsString());
                Console.WriteLine("decimals:" + items[3].AsInteger());
                Console.WriteLine("owner:" + ThinNeo.Helper.GetAddressFromScriptHash(items[4].AsHash160()));
                Console.WriteLine("txid:" + items[5].AsHashString());
                Console.WriteLine("locked:" + items[6].AsInteger());
                Console.WriteLine("hasDrawed:" + items[7].AsInteger());
            }
            else
            {
                Console.WriteLine("no sar exists");
            }
        }

        //锁仓
        async Task test_reserve()
        {
            Console.WriteLine("Input target asset:");
            string name = Console.ReadLine();

            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await business_common.api_SendTransaction(prikey, business_common.sc_wneo, "reserve",
              "(str)" + name,
              "(addr)" + this.address, 
              "(int)" + amount);
            subPrintLine(result);
        }

        //增加
        async Task test_expande()
        {
            Console.WriteLine("Input target asset:");
            string name = Console.ReadLine();

            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await business_common.api_SendTransaction(prikey, business_common.sc_wneo, "expande",
              "(str)" + name,
              "(addr)" + this.address,
              "(int)" + amount);
            subPrintLine(result);
        }

        //擦除
        async Task test_contract()
        {
            Console.WriteLine("Input target asset:");
            string name = Console.ReadLine();

            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await business_common.api_SendTransaction(prikey, business_common.sc_wneo, "contract",
              "(str)" + name,
              "(addr)" + this.address,
              "(int)" + amount);
            subPrintLine(result);
        }

        //提现
        async Task test_withdraw()
        {
            Console.WriteLine("Input target asset:");
            string name = Console.ReadLine();

            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await business_common.api_SendTransaction(prikey, business_common.sc_wneo, "withdraw",
              "(str)" + name,
              "(addr)" + this.address,
              "(int)" + amount);
            subPrintLine(result);
        }

        //授权转账操作
        async Task test_setCallScript()
        {
            var addr = ThinNeo.Helper.GetAddressFromScriptHash(call_common.main);
            Console.WriteLine("address:" + addr);

            var result = await business_common.api_SendTransaction(prikey, business_common.sc_wneo, "setCallScript",
               "(addr)" + addr
              );
            subPrintLine(result);
        }

        //设置配置信息
        async Task test_setConfig()
        {
            Console.WriteLine("Input Config key:");
            string key = Console.ReadLine();

            Console.WriteLine("Input Config value:");
            string value = Console.ReadLine();


            var result = await business_common.api_SendTransaction(prikey, business_common.sc_wneo, "setConfig", "(str)" + key, "(int)" + value);
            subPrintLine(result);

        }

        //查询配置信息
        async Task test_getConfig()
        {
            Console.WriteLine("Input config key:");
            string key = Console.ReadLine();

            var result = await business_common.api_InvokeScript(business_common.sc_wneo, "getConfig", "(str)" + key);
            business_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

    }
}
