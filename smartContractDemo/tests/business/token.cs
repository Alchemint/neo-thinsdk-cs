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
    class tokenTest : ITest
    {
        public string Name => "Token 合约测试";

        public string ID => "token";
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

        public tokenTest()
        {
            this.initMenu();
        }

        private void initMenu()
        {
            infos = new Dictionary<string, testAction>();
            infos["init"] = test_init;
            infos["totalSupply"] = test_totalSupply;
            infos["name"] = test_name;
            infos["symbol"] = test_symbol;
            infos["decimals"] = test_decimals;
            infos["balanceOf"] = test_BalanceOf;
            infos["transfer"] = test_Transfer;
            infos["getTXInfo"] = test_getTXInfo;
            infos["setCallScript"] = test_setCallScript;
            infos["setConfig"] = test_setConfig;
            this.submenu = new List<string>(infos.Keys).ToArray();
        }

        public delegate void Method(ThinNeo.ScriptBuilder sb);//第一步：定义委托类型

        public async Task Demo()
        {
            //得到合约代码
            var urlgetscript = Helper.MakeRpcUrl(Config.api, "getcontractstate", new MyJson.JsonNode_ValueString(token_common.sc));
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

        //初始化
        async Task test_init()
        {
            Console.WriteLine("Input target asset:");
            string name = Console.ReadLine();

            Console.WriteLine("Input target symbol:");
            string symbol = Console.ReadLine();

            var result = await token_common.api_SendTransaction(prikey, token_common.sc_wneo, "init",
               "(str)" + name,
                "(str)" + symbol,
                "(int)" + 8,
                "(addr)" + this.address);
            subPrintLine(result);
        }  
        
        //查询总量
        async Task test_totalSupply()
        {
            Console.WriteLine("Input target asset:");
            string name = Console.ReadLine();

            var result = await token_common.api_InvokeScript(token_common.sc_wneo, "totalSupply", "(str)" + name);
            token_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }


        //查询名字
        async Task test_name()
        {
            Console.WriteLine("Input target asset:");
            string name = Console.ReadLine();

            var result = await token_common.api_InvokeScript(token_common.sc_wneo, "name", "(str)" + name);
            token_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsString());
        }

        //查询标志
        async Task test_symbol()
        {
            Console.WriteLine("Input target asset:");
            string name = Console.ReadLine();
            var result = await token_common.api_InvokeScript(token_common.sc_wneo, "symbol", "(str)"+name);
            token_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsString());
        }

        //查询最小单位
        async Task test_decimals()
        {
            Console.WriteLine("Input target asset:");
            string name = Console.ReadLine();

            var result = await token_common.api_InvokeScript(token_common.sc_wneo, "decimals", "(str)"+name);
            token_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //查询余额
        async Task test_BalanceOf()
        {
            Console.WriteLine("Input target asset:");
            string name = Console.ReadLine();

            Console.WriteLine("Input target address (" + this.address + "):");
            string addr;
            try
            {
                addr = Console.ReadLine();
                if (addr == "\n")
                {
                    addr = this.address;
                }
            }
            catch (Exception e)
            {
                addr = this.address;
            }

            byte[] hash = ThinNeo.Helper.GetPublicKeyHashFromAddress(addr);

            var result = await token_common.api_InvokeScript(token_common.sc_wneo, "balanceOf","(str)" + name, "(addr)" + addr);
            token_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //转账
        async Task test_Transfer()
        {
            Console.WriteLine("Input target asset:");
            string name = Console.ReadLine();
            Console.WriteLine("Input target address:");
            string addressto = Console.ReadLine();
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await token_common.api_SendTransaction(prikey, token_common.sc_wneo, "transfer",
                "(str)" + name,
              "(addr)" + this.address,
              "(addr)" + addressto,
              "(int)" + amount);
            subPrintLine(result);
        }

        async Task test_getTXInfo()
        {
            Console.WriteLine("Input txid:");
            string txid = Console.ReadLine();
            var result = await token_common.api_InvokeScript(token_common.sc_wneo, "getTXInfo", "(hex256)" + txid);
            token_common.ResultItem item = result.value;
            token_common.ResultItem[] items = item.subItem[0].subItem;

            //查询交易详细信息
            Console.WriteLine("from:" + ThinNeo.Helper.GetAddressFromScriptHash(items[0].AsHash160()));
            Console.WriteLine("to:" + ThinNeo.Helper.GetAddressFromScriptHash(items[1].AsHash160()));
            Console.WriteLine("value:" + items[2].AsInteger());
        }

        //授权转账操作
        async Task test_setCallScript()
        {
            var addr = ThinNeo.Helper.GetAddressFromScriptHash(business_common.sc_wneo);
            Console.WriteLine("address:" + addr);

            var result = await token_common.api_SendTransaction(prikey, token_common.sc_wneo, "setCallScript",
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


            var result = await token_common.api_SendTransaction(prikey, token_common.sc_wneo, "setConfig", "(str)" + key, "(int)" + value);
            subPrintLine(result);

        }



        public static byte[] HexString2Bytes(string str)
        {
            byte[] b = new byte[str.Length / 2];
            for (var i = 0; i < b.Length; i++)
            {
                b[i] = byte.Parse(str.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return b;
        }

    }
}
