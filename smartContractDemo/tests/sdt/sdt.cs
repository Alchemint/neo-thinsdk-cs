using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ThinNeo;
using smartContractDemo.tests;

namespace smartContractDemo
{
    class sdtTest : ITest
    {
        public string Name => "SDT 合约测试";

        public string ID => "sdt";
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

        public sdtTest()
        {
            this.initMenu();
        }

        private void initMenu()
        {
            infos = new Dictionary<string, testAction>();

            infos["assets"] = test_assets;
            infos["totalSupply"] = test_totalSupply;
            infos["name"] = test_name;
            infos["init"] = test_init;
            infos["symbol"] = test_symbol;
            infos["decimals"] = test_decimals;
            infos["balanceOf"] = test_BalanceOf;
            infos["transfer"] = test_Transfer;
            infos["transferApp"] = test_TransferApp;
            infos["getTXInfo"] = test_getTXInfo;
            infos["approve"] = test_approve;
            infos["cancelApprove"] = test_cancelApprove;
            infos["allowance"] = test_allowance;
            infos["transferFrom"] = test_transferFrom;
            infos["setBlackHole"] = test_setBlackHoleAccount;
            infos["setConfigScript"] = test_setConfigScript;
            infos["setHoleTypeScript"] = test_setHoleTypeScript;
            infos["setMintTypeScript"] = test_setMintTypeScript;
            infos["burn"] = test_burn;
            infos["mint"] = test_mint;
            infos["getstorage"] = test_getstorage;
            infos["toByte"] = test_toByte;


            this.submenu = new List<string>(infos.Keys).ToArray();
        }


        public delegate void Method(ThinNeo.ScriptBuilder sb);//第一步：定义委托类型

        public async Task Demo()
        {
            //得到合约代码
            //var urlgetscript = Helper.MakeRpcUrl(sdt_common.api, "getcontractstate", new MyJson.JsonNode_ValueString(sdt_common.sc));
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

        async Task test_toByte()
        {
            //totalSupply
            //new byte[]{116, 111, 116, 97, 108, 83, 117, 112, 112, 108, 121}
            byte[] b = HexToBytes("746f74616c537570706c79");
            foreach (var item in b)
            {
                Console.Write($"{item}, ");
            }
            Console.ReadLine();
        }

        static byte[] HexToBytes(string hexString)
        {
            hexString = hexString.Trim();
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }

        //查询存储的值
        async Task test_getstorage()
        {
            Console.WriteLine("Input target key:");
            string key = Console.ReadLine();
            byte[] byteArray = System.Text.Encoding.Default.GetBytes(key);
            //var rev = ThinNeo.Helper.HexString2Bytes(key);
            var revkey = ThinNeo.Helper.Bytes2HexString(byteArray);

            var url = Helper.MakeRpcUrl(Config.api, "getstorage", new MyJson.JsonNode_ValueString(sdt_common.sc), new MyJson.JsonNode_ValueString(revkey));

            string result = await Helper.HttpGet(url);
            Console.WriteLine("得到的结果是：" + result);
        }
        //查询全局资产余额
        async Task test_assets()
        {
            Dictionary<string, List<Utxo>> dir = await Helper.GetBalanceByAddress(Config.api, this.address);

            if (dir.ContainsKey(Config.id_GAS))
            {
                List<Utxo> gaslist = dir[Config.id_GAS];
                decimal sumgas = 0;
                for (var i=0;i< gaslist.Count;i++) {
                    sumgas = sumgas + gaslist[i].value;
                }
                Console.WriteLine("GAS:"+sumgas);
            }

            if (dir.ContainsKey(Config.id_NEO))
            {
                List<Utxo> neolist = dir[Config.id_NEO];
                decimal sumneo = 0;
                for (var i=0;i< neolist.Count;i++) {
                    sumneo = sumneo + neolist[i].value;
                }
                Console.WriteLine("NEO:"+ sumneo);
            }
        }

        //查询总量
        async Task test_totalSupply()
        {
            var result = await sdt_common.api_InvokeScript(sdt_common.sc_sdt, "totalSupply",null);
            sdt_common.ResultItem item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //查询名字
        async Task test_name()
        {
            var result = await sdt_common.api_InvokeScript(sdt_common.sc_sdt, "name", null);
            sdt_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsString());
        }

        //初始操作
        async Task test_init()
        {
            var result = await sdt_common.api_SendTransaction(prikey,sdt_common.sc_sdt,"init",null);
            subPrintLine(result);
        }

        //查询标志
        async Task test_symbol()
        {
            var result = await sdt_common.api_InvokeScript(sdt_common.sc_sdt, "symbol", null);
            sdt_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsString());
        }
        //查询最小单位
        async Task test_decimals()
        {
            var result = await sdt_common.api_InvokeScript(sdt_common.sc_sdt, "decimals", null);
            sdt_common.ResultItem item = result.value;

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

            var result = await sdt_common.api_InvokeScript(sdt_common.sc_sdt, "balanceOf", "(addr)" + addr);

            sdt_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //转账
        async Task test_Transfer()
        {
            Console.WriteLine("Input target address:");
            string addressto = Console.ReadLine();
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await sdt_common.api_SendTransaction(prikey, sdt_common.sc_sdt, "transfer",
               "(addr)" + address,
              "(addr)" + addressto,
              "(int)" + amount
              );
            subPrintLine(result);
        }

        //转账
        async Task test_TransferApp()
        {
            Console.WriteLine("Input target address:");
            string addressto = Console.ReadLine();
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await sdt_common.api_SendTransaction(prikey, sdt_common.sc_sdt, "transfer_contract",
               "(addr)" + address,
              "(addr)" + addressto,
              "(int)" + amount
              );
            subPrintLine(result);
        }

        //设置手续费账户
        async Task test_setBlackHoleAccount()
        {
            Console.WriteLine("Input target address:");
            string addressto = Console.ReadLine();

            var result = await sdt_common.api_SendTransaction(prikey, sdt_common.sc_sdt, "setBlackHole",
              "(addr)" + addressto);
            subPrintLine(result);
        }

        async Task test_mint()
        {
            Console.WriteLine("Input mint mount:");
            string mount = Console.ReadLine();

            var result = await sdt_common.api_SendTransaction(prikey, sdt_common.sc_sdt, "mint",
              "(addr)" + this.address,"(int)"+ mount);
            subPrintLine(result);
        }

        //设置合约调用
        async Task test_setConfigScript()
        {
            Console.WriteLine("address:");
            string addr = Console.ReadLine();

            var result = await sdt_common.api_SendTransaction(prikey, sdt_common.sc_sdt, "setCallScript",
                "(str)callScript1",
              "(addr)" + addr);
            subPrintLine(result);
        }

        //1:使用账户1  2:使用账户2
        async Task test_setHoleTypeScript()
        {
            Console.WriteLine("Input holeType:");
            string holeType = Console.ReadLine();

            var result = await sdt_common.api_SendTransaction(prikey, sdt_common.sc_sdt, "setCallScript",
                "(str)blackHoleType",
              "(int)" + holeType);
            subPrintLine(result);
        }

        //1:合约调用  2:管理员设置
        async Task test_setMintTypeScript()
        {
            Console.WriteLine("Input mintType:");
            string mintType = Console.ReadLine();

            var result = await sdt_common.api_SendTransaction(prikey, sdt_common.sc_sdt, "setCallScript",
                "(str)mintType",
              "(int)" + mintType);
            subPrintLine(result);
        }

        //转账
        async Task test_burn()
        {
            Console.WriteLine("Input burn mount:");
            string amount = Console.ReadLine();

            var result = await sdt_common.api_SendTransaction(prikey, sdt_common.sc_sdt, "burn",
                "(addr)" + this.address,
              "(int)" + amount);
            subPrintLine(result);
        }

        //查询交易信息
        async Task test_getTXInfo() {
            Console.WriteLine("Input txid:");
            string txid = Console.ReadLine();
            var result = await sdt_common.api_InvokeScript(sdt_common.sc_sdt, "getTXInfo", "(hex256)" + txid);
            sdt_common.ResultItem item = result.value;
            sdt_common.ResultItem[] items = item.subItem[0].subItem;

            //查询交易详细信息
            Console.WriteLine("from:" + ThinNeo.Helper.GetAddressFromScriptHash(items[0].AsHash160()));
            Console.WriteLine("to:" + ThinNeo.Helper.GetAddressFromScriptHash(items[1].AsHash160()));
            Console.WriteLine("value:" + items[2].AsInteger());
        }

        //授权操作
        async Task test_approve()
        {
            Console.WriteLine("Input spender address:");
            string spender = Console.ReadLine();

            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await sdt_common.api_SendTransaction(prikey, sdt_common.sc_sdt, "approve",
               "(addr)" + this.address,
              "(addr)" + spender,
              "(int)" + amount);
            subPrintLine(result);
        }

        //取消授权操作
        async Task test_cancelApprove()
        {
            Console.WriteLine("Input spender address:");
            string spender = Console.ReadLine();

            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await sdt_common.api_SendTransaction(prikey, sdt_common.sc_sdt, "cancelApprove",
               "(addr)" + this.address,
              "(addr)" + spender);
            subPrintLine(result);
        }

        //查询授权金额
        async Task test_allowance()
        {
            Console.WriteLine("Input owner address:");
            string owner = Console.ReadLine();

            Console.WriteLine("Input spender address:");
            string spender = Console.ReadLine();

            var result = await sdt_common.api_InvokeScript(sdt_common.sc_sdt, "allowance", 
              "(addr)" + owner,
              "(addr)" + spender);

            sdt_common.ResultItem item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //授权转账操作
        async Task test_transferFrom()
        {
            Console.WriteLine("Input owner address:");
            string owner = Console.ReadLine();

            Console.WriteLine("Input target address:");
            string addressto = Console.ReadLine();
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await sdt_common.api_SendTransaction(prikey, sdt_common.sc_sdt, "transferFrom",
               "(addr)" + owner,
               "(addr)" + this.address,
              "(addr)" + addressto,
              "(int)" + amount
              );
            subPrintLine(result);
        }

    }

}
