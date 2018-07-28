using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ThinNeo;
using System.IO;
using System.Linq;
using System.Numerics;
using smartContractDemo.tests;

namespace smartContractDemo
{
    class oracleTest : ITest
    {
        public string Name => "Token 合约测试";

        public string ID => "oracle";
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

        public oracleTest()
        {
            this.initMenu();
        }

        private void initMenu()
        {
            infos = new Dictionary<string, testAction>();
          
            infos["setAccount"] = test_setAccount;
            infos["setPrice"] = test_setPrice;
            infos["setAnchorPrice"] = test_setAnchorPrice;
            infos["getPrice"] = test_getPrice;
            infos["getAnchorPrice"] = test_getAnchorPrice;
            this.submenu = new List<string>(infos.Keys).ToArray();
        }

        public delegate void Method(ThinNeo.ScriptBuilder sb);//第一步：定义委托类型

        public async Task Demo()
        {
            //得到合约代码
            var urlgetscript = Helper.MakeRpcUrl(Config.api, "getcontractstate", new MyJson.JsonNode_ValueString(oracle_common.sc));
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

        //设置账户
        async Task test_setAccount()
        {
            var result = await oracle_common.api_SendTransaction(prikey, oracle_common.sc_wneo, "setAccount",
               "(addr)" + this.address);
            subPrintLine(result);
        }

        //设置价格信息
        async Task test_setPrice()
        {
            var result = await oracle_common.api_SendTransaction(prikey, oracle_common.sc_wneo, "setPrice", 
                "(str)sdt_price",
                "(addr)"+this.address,
                "(int)8000000");
            subPrintLine(result);

        }

        //查询配置信息
        async Task test_getPrice()
        {
            Console.WriteLine("key:sdt_price");

            var result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getPrice", "(str)sdt_price");
            oracle_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }


        //查询配置信息
        async Task test_getAnchorPrice()
        {
            Console.WriteLine("key:anchor_type_usd");

            var result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getAnchorPrice", "(str)anchor_type_usd");
            oracle_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //设置锚定物信息
        async Task test_setAnchorPrice()
        {
            var result = await oracle_common.api_SendTransaction(prikey, oracle_common.sc_wneo, "setAnchorPrice", "(str)anchor_type_usd", "(int)100000000");
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
