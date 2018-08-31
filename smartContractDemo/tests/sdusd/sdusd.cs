using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ThinNeo;
using smartContractDemo.tests;
using System.Linq;

namespace smartContractDemo
{
    class sdusdTest : ITest
    {
        public string Name => "SDUSD 合约测试";

        public string ID => "sdusd";
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

        public sdusdTest()
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
            infos["setAccount"] = test_setAccount;
            infos["getAccount"] = test_getAccount;
            infos["getStorage"] = test_getStorage;
            //infos["openSAR"] = test_openSAR;
            //infos["lock"] = test_lock;
            //infos["draw"] = test_draw;
            //infos["free"] = test_free;
            //infos["wipe"] = test_wipe;
            //infos["shut"] = test_shut;
            //infos["bite"] = test_bite;
            //infos["balanceOfRedeem"] = test_balanceOfRedeem;
            //infos["redeem"] = test_redeem;
            //infos["give"] = test_give;
            //infos["getTXInfo"] = test_getTXInfo;
            //infos["getSAR"] = test_getSAR;
            //infos["getSARTxInfo"] = test_getSARTxInfo;
            //infos["setConfig"] = test_setConfig;
            //infos["getConfig"] = test_getConfig;
            //infos["totalGenerate"] = test_totalGenerate;
            //infos["mintSDT"] = test_mintSDT;


            this.submenu = new List<string>(infos.Keys).ToArray();
        }

        public delegate void Method(ThinNeo.ScriptBuilder sb);//第一步：定义委托类型

        public async Task Demo()
        {
            //得到合约代码
            //var urlgetscript = Helper.MakeRpcUrl(sdusd_common.api, "getcontractstate", new MyJson.JsonNode_ValueString(sdusd_common.sc));
            //var resultgetscript = await Helper.HttpGet(urlgetscript);
            //var _json = MyJson.Parse(resultgetscript).AsDict();
            //var _resultv = _json["result"].AsList()[0].AsDict();
            
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

        async Task test_getStorage()
        {
            //var rev = ThinNeo.Helper.HexString2Bytes(key).Reverse().ToArray();
            var revkey = ThinNeo.Helper.GetPublicKeyHashFromAddress("AZ77FiX7i9mRUPF2RyuJD2L8kS6UDnQ9Y7");
            string revhash =  revkey.ToString();

            Console.WriteLine(revhash);
            
            var rev = ThinNeo.Helper.HexString2Bytes(revhash.Replace("0x","")).Reverse().ToArray();
            var key2 = ThinNeo.Helper.Bytes2HexString(rev);

            Console.WriteLine(key2);
            //ThinNeo.Helper.Get
            string key = "11" + key2;
            var url = Helper.MakeRpcUrl(Config.api, "getstorage", new MyJson.JsonNode_ValueString(sds_common.sc), new MyJson.JsonNode_ValueString(key));
            string result = await Helper.HttpGet(url);
            Console.WriteLine("得到的结果是：" + result);
        }

        async Task test_setAccount()
        {
            var addr = ThinNeo.Helper.GetAddressFromScriptHash(sar_common.sc_sar);
            Console.WriteLine("sar address:" + addr);

            var result = await sdusd_common.api_SendbatchTransaction(prikey, sdusd_common.sc_sdusd, "setAccount",
               "(addr)" + addr);
            subPrintLine(result);
        }

        async Task test_getAccount()
        {
            var addr = ThinNeo.Helper.GetAddressFromScriptHash(sar_common.sc_sar);
            Console.WriteLine("sar address:" + addr);

            var result = await sdusd_common.api_InvokeScript(sdusd_common.sc_sdusd, "getAccount",
               "(addr)" + addr);

            sdusd_common.ResultItem item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());
        }


        //查询总量
        async Task test_totalSupply()
        {
            var result = await sdusd_common.api_InvokeScript(sdusd_common.sc_sdusd, "totalSupply",null);
            sdusd_common.ResultItem item = result.value;
            
            Console.WriteLine(item.subItem[0].AsInteger());
        }

        async Task test_totalGenerate()
        {
            var result = await sdusd_common.api_InvokeScript(sdusd_common.sc_sdusd, "totalGenerate", null);
            sdusd_common.ResultItem item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //查询名字
        async Task test_name()
        {
            var result = await sdusd_common.api_InvokeScript(sdusd_common.sc_sdusd, "name", null);
            sdusd_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsString());
        }
        //查询标志
        async Task test_symbol()
        {
            var result = await sdusd_common.api_InvokeScript(sdusd_common.sc_sdusd, "symbol", null);
            sdusd_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsString());
        }
        //查询最小单位
        async Task test_decimals()
        {
            var result = await sdusd_common.api_InvokeScript(sdusd_common.sc_sdusd, "decimals", null);
            sdusd_common.ResultItem item = result.value;

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
            string strhash = ThinNeo.Helper.Bytes2HexString(hash);

            var result = await sdusd_common.api_InvokeScript(sdusd_common.sc_sdusd, "balanceOf", "(bytes)" + strhash);
            sdusd_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //转账
        async Task test_Transfer()
        {
            Console.WriteLine("Input target address:");
            string addressto = Console.ReadLine();
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(sdusd_common.sc_sdusd);

            var result = await sdusd_common.api_SendTransaction(prikey, shash, "transfer",
              "(addr)" + address,
               "(addr)" + addressto,
                "(int)" + amount
              );
            subPrintLine(result);
        }

        
        async Task test_mintSDT()
        {
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(sdusd_common.sc_sdusd);

            var result = await sdusd_common.api_SendTransaction(prikey, shash, "mintSDT",
              "(addr)" + address,
                "(int)" + amount
              );
            subPrintLine(result);
        }

        //创建CDP在仓
        async Task test_openSAR()
        {
            var result = await sdusd_common.api_SendTransaction(prikey, sdusd_common.sc_sdusd, "openSAR", "(addr)" + this.address);
            subPrintLine(result);
        }

        //锁仓
        async Task test_lock()
        {
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(sdusd_common.sc_sdusd);
            var result = await sdusd_common.api_SendTransaction(prikey, sdusd_common.sc_sdusd, "lock",
                "(addr)" + this.address,
                "(int)" + amount);
            subPrintLine(result);
        }

        //取钱
        async Task test_draw()
        {
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();
            var result = await sdusd_common.api_SendTransaction(prikey, sdusd_common.sc_sdusd, "draw",
                "(addr)" + this.address,
                "(int)" + amount);
            subPrintLine(result);
        }

        //释放
        async Task test_free()
        {
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(sdusd_common.sc_sdusd);
            var result = await sdusd_common.api_SendTransaction(prikey, shash, "free",
                "(addr)" + this.address,
                "(int)" + amount);
            subPrintLine(result);
        }

        //平仓
        async Task test_wipe()
        {
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await sdusd_common.api_SendTransaction(prikey, sdusd_common.sc_sdusd, "wipe",
                "(addr)" + this.address, 
                "(int)" + amount);
            subPrintLine(result);
        }

        //转移CDP
        async Task test_give()
        {
            Console.WriteLine("Input to Address:");
            string toAddress = Console.ReadLine();

            var result = await sdusd_common.api_SendTransaction(prikey, sdusd_common.sc_sdusd, "give",
                "(addr)" + this.address,
                "(addr)" + toAddress);
            subPrintLine(result);
        }

        //关闭在仓
        async Task test_shut()
        {

            var result = await sdusd_common.api_SendTransaction(prikey, sdusd_common.sc_sdusd, "shut",
                "(addr)" + this.address);
            subPrintLine(result);
        }

        //强制关闭在仓
        async Task test_bite()
        {
            Console.WriteLine("Input other address:");
            var otherAdd = Console.ReadLine();
            var result = await sdusd_common.api_SendTransaction(prikey, sdusd_common.sc_sdusd, "bite",
                "(addr)" + otherAdd,
                "(addr)" + this.address);
            subPrintLine(result);
        }

        //赎回剩余PNEO
        async Task test_redeem()
        {
            var result = await sdusd_common.api_SendTransaction(prikey, sdusd_common.sc_sdusd, "redeem",
                "(addr)" + this.address);
            subPrintLine(result);
        }


        //查询需要赎回余额
        async Task test_balanceOfRedeem()
        {
            var result = await sdusd_common.api_InvokeScript(sdusd_common.sc_sdusd, "balanceOfRedeem", "(addr)" + this.address);
            sdusd_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //查询交易信息
        async Task test_getTXInfo()
        {
            Console.WriteLine("Input txid:");
            string txid = Console.ReadLine();
            var result = await sdusd_common.api_InvokeScript(sdusd_common.sc_sdusd, "getTXInfo", "(hex256)"+txid);
            sdusd_common.ResultItem item = result.value;
            sdusd_common.ResultItem[] items = item.subItem[0].subItem;

            //查询交易详细信息
            Console.WriteLine("from:" + ThinNeo.Helper.GetAddressFromScriptHash(items[0].AsHash160()));
            Console.WriteLine("to:" + ThinNeo.Helper.GetAddressFromScriptHash(items[1].AsHash160()));
            Console.WriteLine("value:" + items[2].AsInteger());
        }

        //查询CDP交易信息
        async Task test_getSAR()
        {
            Console.WriteLine("Input address:");
            string address = Console.ReadLine();
            var result = await sdusd_common.api_InvokeScript(sdusd_common.sc_sdusd, "getSAR", "(addr)" + address);
            sdusd_common.ResultItem item = result.value;
            sdusd_common.ResultItem[] items = item.subItem[0].subItem;

            if (items != null)
            {
                Console.WriteLine("from:" + ThinNeo.Helper.GetAddressFromScriptHash(items[0].AsHash160()));
                Console.WriteLine("txid:" + items[1].AsHashString());
                Console.WriteLine("locked:" + items[2].AsInteger());
                Console.WriteLine("hasDrawed:" + items[3].AsInteger());
            }
            else {
                Console.WriteLine("no cdp exists");
            }
        }

        //查询CDP详细交易信息
        async Task test_getSARTxInfo()
        {
            Console.WriteLine("Input txid:");
            string txid = Console.ReadLine();
            var result = await sdusd_common.api_InvokeScript(sdusd_common.sc_sdusd, "getSARTxInfo", "(hex256)" + txid);
            sdusd_common.ResultItem item = result.value;
            sdusd_common.ResultItem[] items = item.subItem[0].subItem;

            if (items != null)
            {
                Console.WriteLine("from:" + ThinNeo.Helper.GetAddressFromScriptHash(items[0].AsHash160()));
                Console.WriteLine("cdpTxid:" + items[1].AsHashString());
                Console.WriteLine("txid:" + items[2].AsHashString());
                Console.WriteLine("operated:" + items[3].AsInteger());
                Console.WriteLine("hasLocked:" + items[4].AsInteger()+" PNEO");
                Console.WriteLine("hasDrawed:" + items[5].AsInteger()+" SDUSD");
                Console.WriteLine("type:" + items[6].AsInteger());
            }else {
                Console.WriteLine("no txInfo exists");
            }

        }

        //设置配置信息
        async Task test_setConfig()
        {
            Console.WriteLine("Input config key:");
            string key = Console.ReadLine();

            Console.WriteLine("Input config value:");
            string value = Console.ReadLine();


            var result = await sdusd_common.api_SendTransaction(prikey, sdusd_common.sc_sdusd, "setConfig", "(str)" + key, "(int)" + value);
            subPrintLine(result);

        }

        //查询配置信息
        async Task test_getConfig()
        {
            Console.WriteLine("Input config key:");
            string key = Console.ReadLine();

            var result = await sdusd_common.api_InvokeScript(sdusd_common.sc_sdusd, "getConfig", "(str)" + key);
            sdusd_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

    }

}
