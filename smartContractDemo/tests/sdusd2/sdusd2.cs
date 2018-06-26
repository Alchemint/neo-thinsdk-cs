using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ThinNeo;
using smartContractDemo.tests;

namespace smartContractDemo
{
    class sdusd2Test : ITest
    {
        public string Name => "SDUSD2 合约测试";

        public string ID => "sdusd2";
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

        public sdusd2Test()
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
            infos["openCdp"] = test_openCdp;
            infos["lock"] = test_lock;
            infos["draw"] = test_draw;
            infos["free"] = test_free;
            infos["wipe"] = test_wipe;
            infos["shut"] = test_shut;
            infos["bite"] = test_bite;
            //infos["balanceOfRedeem"] = test_balanceOfRedeem;
            //infos["redeem"] = test_redeem;
            //infos["give"] = test_give;
            infos["getTXInfo"] = test_getTXInfo;
            infos["getCdp"] = test_getCdp;
            infos["getCdpTxInfo"] = test_getCdpTxInfo;
            infos["setConfig"] = test_setConfig;
            infos["getConfig"] = test_getConfig;
            infos["totalGenerate"] = test_totalGenerate;
            infos["setCallScript"] = test_setCallScript;
            infos["mintSDT"] = test_mintSDT;
            infos["transferSDT"] = test_transferSDT;
            infos["transferFromSDT"] = test_transferFromSDT;
            infos["setAccount"] = test_setAccount;


            this.submenu = new List<string>(infos.Keys).ToArray();
        }

        public delegate void Method(ThinNeo.ScriptBuilder sb);//第一步：定义委托类型

        public async Task Demo()
        {
            //得到合约代码
            //var urlgetscript = Helper.MakeRpcUrl(sdusd_common2.api, "getcontractstate", new MyJson.JsonNode_ValueString(sdusd_common2.sc));
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


        //授权转账操作
        async Task test_setCallScript()
        {
            var addr = ThinNeo.Helper.GetAddressFromScriptHash(call_common.main);
            Console.WriteLine("address:" + addr);
            var result = await sdusd_common2.api_SendTransaction(prikey, sdusd_common2.sc_sdusd, "setCallScript",
               "(addr)" + addr);
            subPrintLine(result);
        }


        //查询总量
        async Task test_totalSupply()
        {
            var result = await sdusd_common2.api_InvokeScript(sdusd_common2.sc_sdusd, "totalSupply",null);
            sdusd_common2.ResultItem item = result.value;
            
            Console.WriteLine(item.subItem[0].AsInteger());
        }

        async Task test_totalGenerate()
        {
            var result = await sdusd_common2.api_InvokeScript(sdusd_common2.sc_sdusd, "totalGenerate", null);
            sdusd_common2.ResultItem item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //查询名字
        async Task test_name()
        {
            var result = await sdusd_common2.api_InvokeScript(sdusd_common2.sc_sdusd, "name", null);
            sdusd_common2.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsString());
        }
        //查询标志
        async Task test_symbol()
        {
            var result = await sdusd_common2.api_InvokeScript(sdusd_common2.sc_sdusd, "symbol", null);
            sdusd_common2.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsString());
        }
        //查询最小单位
        async Task test_decimals()
        {
            var result = await sdusd_common2.api_InvokeScript(sdusd_common2.sc_sdusd, "decimals", null);
            sdusd_common2.ResultItem item = result.value;

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

            var result = await sdusd_common2.api_InvokeScript(sdusd_common2.sc_sdusd, "balanceOf", "(bytes)" + strhash);
            sdusd_common2.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //转账
        async Task test_Transfer()
        {
            Console.WriteLine("Input target address:");
            string addressto = Console.ReadLine();
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(sdusd_common2.sc_sdusd);

            var result = await sdusd_common2.api_SendTransaction(prikey, shash, "transfer",
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

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(sdusd_common2.sc_sdusd);

            var result = await sdusd_common2.api_SendTransaction(prikey, shash, "test_mint",
                "(int)" + amount
              );
            subPrintLine(result);
        }

        async Task test_transferSDT()
        {
            Console.WriteLine("Input target:");
            string to = Console.ReadLine();

            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(sdusd_common2.sc_sdusd);

            var result = await sdusd_common2.api_SendTransaction(prikey, shash, "transfer_sdt",
                "(addr)"+to,
                "(int)" + amount
              );
            subPrintLine(result);
        }

            async Task test_transferFromSDT()
        {
            Console.WriteLine("Input target owner:");
            string owner = Console.ReadLine();

            Console.WriteLine("Input target to:");
            string to = Console.ReadLine();

            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(sdusd_common2.sc_sdusd);

            var result = await sdusd_common2.api_SendTransaction(prikey, shash, "test_transferfrom",
                "(addr)" + owner,
                "(addr)"+to,
                "(int)" + amount
              );
            subPrintLine(result);
        }

        //创建CDP在仓
        async Task test_openCdp()
        {
            var result = await sdusd_common2.api_SendTransaction(prikey, sdusd_common2.sc_sdusd, "openCDP", "(addr)" + this.address);
            subPrintLine(result);
        }

        //锁仓
        async Task test_lock()
        {
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(sdusd_common2.sc_sdusd);
            var result = await sdusd_common2.api_SendTransaction(prikey, sdusd_common2.sc_sdusd, "lock",
                "(addr)" + this.address,
                "(int)" + amount);
            subPrintLine(result);
        }

        //取钱
        async Task test_draw()
        {
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();
            var result = await sdusd_common2.api_SendTransaction(prikey, sdusd_common2.sc_sdusd, "draw",
                "(addr)" + this.address,
                "(int)" + amount);
            subPrintLine(result);
        }

        //释放
        async Task test_free()
        {
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(sdusd_common2.sc_sdusd);
            var result = await sdusd_common2.api_SendTransaction(prikey, shash, "free",
                "(addr)" + this.address,
                "(int)" + amount);
            subPrintLine(result);
        }

        //平仓
        async Task test_wipe()
        {
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await sdusd_common2.api_SendTransaction(prikey, sdusd_common2.sc_sdusd, "wipe",
                "(addr)" + this.address, 
                "(int)" + amount);
            subPrintLine(result);
        }

        //转移CDP
        async Task test_give()
        {
            Console.WriteLine("Input to Address:");
            string toAddress = Console.ReadLine();

            var result = await sdusd_common2.api_SendTransaction(prikey, sdusd_common2.sc_sdusd, "give",
                "(addr)" + this.address,
                "(addr)" + toAddress);
            subPrintLine(result);
        }

        //关闭在仓
        async Task test_shut()
        {

            var result = await sdusd_common2.api_SendTransaction(prikey, sdusd_common2.sc_sdusd, "shut",
                "(addr)" + this.address);
            subPrintLine(result);
        }

        //强制关闭在仓
        async Task test_bite()
        {
            Console.WriteLine("Input other address:");
            var otherAdd = Console.ReadLine();
            var result = await sdusd_common2.api_SendTransaction(prikey, sdusd_common2.sc_sdusd, "bite",
                "(addr)" + otherAdd,
                "(addr)" + this.address);
            subPrintLine(result);
        }

        //赎回剩余PNEO
        async Task test_redeem()
        {
            var result = await sdusd_common2.api_SendTransaction(prikey, sdusd_common2.sc_sdusd, "redeem",
                "(addr)" + this.address);
            subPrintLine(result);
        }


        //查询需要赎回余额
        async Task test_balanceOfRedeem()
        {
            var result = await sdusd_common2.api_InvokeScript(sdusd_common2.sc_sdusd, "balanceOfRedeem", "(addr)" + this.address);
            sdusd_common2.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //查询交易信息
        async Task test_getTXInfo()
        {
            Console.WriteLine("Input txid:");
            string txid = Console.ReadLine();
            var result = await sdusd_common2.api_InvokeScript(sdusd_common2.sc_sdusd, "getTXInfo", "(hex256)"+txid);
            sdusd_common2.ResultItem item = result.value;
            sdusd_common2.ResultItem[] items = item.subItem[0].subItem;

            //查询交易详细信息
            Console.WriteLine("from:" + ThinNeo.Helper.GetAddressFromScriptHash(items[0].AsHash160()));
            Console.WriteLine("to:" + ThinNeo.Helper.GetAddressFromScriptHash(items[1].AsHash160()));
            Console.WriteLine("value:" + items[2].AsInteger());
        }

        //查询CDP交易信息
        async Task test_getCdp()
        {
            Console.WriteLine("Input address:");
            string address = Console.ReadLine();
            var result = await sdusd_common2.api_InvokeScript(sdusd_common2.sc_sdusd, "getCDP", "(addr)" + address);
            sdusd_common2.ResultItem item = result.value;
            sdusd_common2.ResultItem[] items = item.subItem[0].subItem;

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
        async Task test_getCdpTxInfo()
        {
            Console.WriteLine("Input txid:");
            string txid = Console.ReadLine();
            var result = await sdusd_common2.api_InvokeScript(sdusd_common2.sc_sdusd, "getCdpTxInfo", "(hex256)" + txid);
            sdusd_common2.ResultItem item = result.value;
            sdusd_common2.ResultItem[] items = item.subItem[0].subItem;

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


            var result = await sdusd_common2.api_SendTransaction(prikey, sdusd_common2.sc_sdusd, "setConfig", "(str)" + key, "(int)" + value);
            subPrintLine(result);

        }

        async Task test_setAccount()
        {
            Console.WriteLine("Input config addr:");
            string addr = Console.ReadLine();

            var result = await sdusd_common2.api_SendTransaction(prikey, sdusd_common2.sc_sdusd, "setAccount", "(addr)" + addr);
            subPrintLine(result);
        }

        //查询配置信息
        async Task test_getConfig()
        {
            Console.WriteLine("Input config key:");
            string key = Console.ReadLine();

            var result = await sdusd_common2.api_InvokeScript(sdusd_common2.sc_sdusd, "getConfig", "(str)" + key);
            sdusd_common2.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

    }

}
