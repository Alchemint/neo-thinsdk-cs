using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ThinNeo;

namespace smartContractDemo
{
    class penoTest : ITest
    {
        public string Name => "PNEO 合约测试";

        public string ID => "pneo";
        byte[] prikey;
        public string address;
        byte[] scripthash;
        byte[] pubkey;
        Hash160 reg_sc;//注册器合约地址
        public delegate Task testAction();
        public Dictionary<string, testAction> infos = null;// = new Dictionary<string, testAction>();
        string[] submenu;

        void subPrintLine(string line)
        {
            Console.WriteLine("    " + line);
        }

        public penoTest()
        {
            this.initMenu();
        }

        private void initMenu()
        {
            infos = new Dictionary<string, testAction>();

            infos["totalSupply"] = test_totalSupply;
            infos["name"] = test_name;
            infos["symbol"] = test_symbol;
            infos["decimals"] = test_decimals;
            infos["balanceOf"] = test_BalanceOf;
            infos["transfer"] = test_Transfer;
            infos["exWtoP"] = test_exWtoP;
            infos["totalDestory"] = test_totalDestory;
            //infos["exPtoW"] = test_exPtoW;
            infos["getTXInfo"] = test_getTXInfo;
            infos["setCallScript"] = test_setCallScript;


            this.submenu = new List<string>(infos.Keys).ToArray();
        }

        public delegate void Method(ThinNeo.ScriptBuilder sb);//第一步：定义委托类型

        public async Task Demo()
        {
            //得到合约代码
            //var urlgetscript = Helper.MakeRpcUrl(pneo_common.api, "getcontractstate", new MyJson.JsonNode_ValueString(pneo_common.sc));
            //var resultgetscript = await Helper.HttpGet(urlgetscript);
            //var _json = MyJson.Parse(resultgetscript).AsDict();
            //var _resultv = _json["result"].AsList()[0].AsDict();
            
            //Console.WriteLine("reg=" + _resultv["script"].AsString());

            showMenu();

            prikey = ThinNeo.Helper.GetPrivateKeyFromWIF(smartContractDemo.tests.Config.testwif);
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

        //查询总量
        async Task test_totalSupply()
        {
            var result = await pneo_common.api_InvokeScript(pneo_common.sc_pneo, "totalSupply", null);
            pneo_common.ResultItem item = result.value;
            
            Console.WriteLine(item.subItem[0].AsInteger());
        }

        async Task test_totalDestory()
        {
            var result = await pneo_common.api_InvokeScript(pneo_common.sc_pneo, "totalDestory", null);
            pneo_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //查询名字
        async Task test_name()
        {
            var result = await pneo_common.api_InvokeScript(pneo_common.sc_pneo, "name", null);
            pneo_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsString());
        }

        //查询标志
        async Task test_symbol()
        {
            var result = await pneo_common.api_InvokeScript(pneo_common.sc_pneo, "symbol", null);
            pneo_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsString());
        }

        //查询最小单位
        async Task test_decimals()
        {
            var result = await pneo_common.api_InvokeScript(pneo_common.sc_pneo, "decimals", null);
            pneo_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //查询余额
        async Task test_BalanceOf()
        {
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

            var result = await pneo_common.api_InvokeScript(pneo_common.sc_pneo, "balanceOf", "(addr)" + addr);
            pneo_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //授权转账操作
        async Task test_setCallScript()
        {
    
            var addr = ThinNeo.Helper.GetAddressFromScriptHash(call_common.main);
            Console.WriteLine("address:" + addr);
            var result = await pneo_common.api_SendTransaction(prikey, pneo_common.sc_pneo, "setCallScript",
               "(addr)" + addr);
            subPrintLine(result);
        }

        //转账
        async Task test_Transfer()
        {
            Console.WriteLine("Input target address:");
            string addressto = Console.ReadLine();
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await pneo_common.api_SendTransaction(prikey, pneo_common.sc_pneo, "transfer",
              "(addr)" + this.address,"(addr)" + addressto,"(int)" + amount);
            subPrintLine(result);
        }

        //W转换成P
        async Task test_exWtoP()
        {
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await pneo_common.api_SendTransaction(prikey, pneo_common.sc_pneo, "WNeoToPNeo",
              "(addr)" + this.address,"(int)" + amount);
            subPrintLine(result);
        }
        
        //P转换成W
        async Task test_exPtoW()
        {
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await pneo_common.api_SendTransaction(prikey, pneo_common.sc_pneo, "exPtoW",
              "(addr)" + this.address,"(int)" + amount);
            subPrintLine(result);
        }

        //增发代币
        async Task test_increase()
        {
            Console.Write("current address:"+ address);
            Console.WriteLine("  Input amount:");
            string amount = Console.ReadLine();

            var result = await pneo_common.api_SendTransaction(prikey, pneo_common.sc_pneo, "increase",
                "(addr)" + this.address,
                "(int)" + amount);
            subPrintLine(result);
        }

        //销毁代币
        async Task test_destory()
        {
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await pneo_common.api_SendTransaction(prikey, pneo_common.sc_pneo, "destory",
                "(addr)" + this.address,
                "(int)" + amount);
            subPrintLine(result);
        }

        async Task test_getTXInfo()
        {
            Console.WriteLine("Input txid:");
            string txid = Console.ReadLine();
            var result = await pneo_common.api_InvokeScript(pneo_common.sc_pneo, "getTXInfo", "(hex256)"+ txid);
            pneo_common.ResultItem item = result.value;
            pneo_common.ResultItem[] items = item.subItem[0].subItem;
            
            //查询交易详细信息
            Console.WriteLine("from:"+ThinNeo.Helper.GetAddressFromScriptHash(items[0].AsHash160()));
            Console.WriteLine("to:"+ThinNeo.Helper.GetAddressFromScriptHash(items[1].AsHash160()));
            Console.WriteLine("value:"+items[2].AsInteger());
        }
    }

}
