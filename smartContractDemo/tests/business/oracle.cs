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

        public string ID => "or";
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
            infos["setConfig"] = test_setConfig;
            //infos["setAnchorPrice"] = test_setAnchorPrice;
            infos["getConfig"] = test_getConfig;
            infos["getPrice"] = test_getPrice;
            //infos["getAnchorPrice"] = test_getAnchorPrice;
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
            Console.WriteLine("addr:");
            string addr = Console.ReadLine();

            var result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setAccount",
               "(addr)" + addr,"(int)"+1);
            subPrintLine(result);
        }

        //设置价格信息
        async Task test_setPrice()
        {
            var result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setPrice", 
                "(str)sds_price",
                "(addr)"+this.address,
                "(int)10000000");
            subPrintLine(result);

            result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setPrice",
                "(str)neo_price",
                "(addr)" + this.address,
                "(int)2000000000");
            subPrintLine(result);

            result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setPrice",
                "(str)gas_price",
                "(addr)" + this.address,
                "(int)200000000");
            subPrintLine(result);

            /*  
            *  anchor_type_usd    1*100000000
            *  anchor_type_cny    6.8*100000000
            *  anchor_type_eur    0.875*100000000
            *  anchor_type_jpy    120*100000000
            *  anchor_type_gbp    0.7813 *100000000
            *  anchor_type_gold   0.000838 * 100000000
            */

            result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setPrice", "(str)anchor_type_usd", "(addr)" + this.address, "(int)100000000");
            subPrintLine(result);

            result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setPrice", "(str)anchor_type_eur", "(addr)" + this.address, "(int)87500000");
            subPrintLine(result);

            result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setPrice", "(str)anchor_type_jpy", "(addr)" + this.address, "(int)1200000000");
            subPrintLine(result);

            result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setPrice", "(str)anchor_type_gbp", "(addr)" + this.address, "(int)78130000");
            subPrintLine(result);

            result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setPrice", "(str)anchor_type_gold", "(addr)" + this.address, "(int)838000");
            subPrintLine(result);

        }

        //设置价格信息
        async Task test_setConfig()
        {
            var result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setConfig",
                "(str)liquidate_rate_b",
                "(int)50");
            subPrintLine(result);

            result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setConfig",
                "(str)liquidate_rate_c",
                "(int)150");
            subPrintLine(result);

            result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setConfig",
                "(str)clear_rate",
                "(int)110");
            subPrintLine(result);

            result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setConfig",
               "(str)service_fee",
               "(int)1000000000");
            subPrintLine(result);

            result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setConfig", "(str)anchor_type_usd", "(int)1");
            subPrintLine(result);

            result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setConfig", "(str)anchor_type_eur", "(int)1");
            subPrintLine(result);

            result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setConfig", "(str)anchor_type_jpy", "(int)1");
            subPrintLine(result);

            result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setConfig", "(str)anchor_type_gbp", "(int)1");
            subPrintLine(result);

            result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setConfig", "(str)anchor_type_gold", "(int)1");
            subPrintLine(result);

        }

        async Task test_getConfig() {
            var result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getConfig", "(str)liquidate_rate_b");
            oracle_common.ResultItem item = result.value;
            Console.WriteLine("liquidate_rate_b:" + item.subItem[0].AsInteger());

            result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getConfig", "(str)liquidate_rate_c");
            item = result.value;
            Console.WriteLine("liquidate_rate_c:" + item.subItem[0].AsInteger());

            result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getConfig", "(str)clear_rate");
            item = result.value;
            Console.WriteLine("clear_rate:" + item.subItem[0].AsInteger());

            result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getConfig", "(str)service_fee");
            item = result.value;
            Console.WriteLine("service_fee:" + item.subItem[0].AsInteger());

            result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getConfig", "(str)anchor_type_usd");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());

            result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getConfig", "(str)anchor_type_eur");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());

            result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getConfig", "(str)anchor_type_jpy");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());

            result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getConfig", "(str)anchor_type_gbp");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());

            result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getConfig", "(str)anchor_type_gold");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //查询配置信息
        async Task test_getPrice()
        {
            var result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getPrice", "(str)sds_price");
            oracle_common.ResultItem item = result.value;
            Console.WriteLine("sds_price:"+item.subItem[0].AsInteger());

            result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getPrice", "(str)neo_price");
            item = result.value;
            Console.WriteLine("neo_price:" + item.subItem[0].AsInteger());

            result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getPrice", "(str)gas_price");
            item = result.value;
            Console.WriteLine("gas_price:" + item.subItem[0].AsInteger());

            result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getPrice", "(str)anchor_type_usd");
            item = result.value;
            Console.WriteLine("anchor_type_usd:" + item.subItem[0].AsInteger());

            result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getPrice", "(str)anchor_type_eur");
            item = result.value;
            Console.WriteLine("anchor_type_eur:"+item.subItem[0].AsInteger());

            result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getPrice", "(str)anchor_type_jpy");
            item = result.value;
            Console.WriteLine("anchor_type_jpy:" + item.subItem[0].AsInteger());

            result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getPrice", "(str)anchor_type_gbp");
            item = result.value;
            Console.WriteLine("anchor_type_gbp:"+item.subItem[0].AsInteger());

            result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getPrice", "(str)anchor_type_gold");
            item = result.value;
            Console.WriteLine("anchor_type_gold:"+item.subItem[0].AsInteger());
        }


        //查询配置信息
        async Task test_getAnchorPrice()
        {
            Console.WriteLine("key:anchor_type_usd");

            var result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getAnchorPrice", "(str)anchor_type_usd");
            oracle_common.ResultItem item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());

            result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getAnchorPrice", "(str)anchor_type_eur");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());

            result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getAnchorPrice", "(str)anchor_type_jpy");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());

            result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getAnchorPrice", "(str)anchor_type_gbp");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());

            result = await oracle_common.api_InvokeScript(oracle_common.sc_wneo, "getAnchorPrice", "(str)anchor_type_gold");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //设置锚定物信息
        async Task test_setAnchorPrice()
        {
            /*  
             *  anchor_type_usd    1*100000000
             *  anchor_type_cny    6.8*100000000
             *  anchor_type_eur    0.875*100000000
             *  anchor_type_jpy    120*100000000
             *  anchor_type_gbp    0.7813 *100000000
             *  anchor_type_gold   0.000838 * 100000000
             */

            var result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setAnchorPrice", "(str)anchor_type_usd", "(int)100000000");
            subPrintLine(result);

            result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setAnchorPrice", "(str)anchor_type_eur", "(int)87500000");
            subPrintLine(result);

            result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setAnchorPrice", "(str)anchor_type_jpy", "(int)1200000000");
            subPrintLine(result);

            result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setAnchorPrice", "(str)anchor_type_gbp", "(int)78130000");
            subPrintLine(result);

            result = await oracle_common.api_SendbatchTransaction(prikey, oracle_common.sc_wneo, "setAnchorPrice", "(str)anchor_type_gold", "(int)838000");
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
