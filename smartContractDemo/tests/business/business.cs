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

        public string ID => "bu";
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
            infos["initToken"] = test_initToken;
            infos["reserve"] = test_reserve;
            infos["expande"] = test_expande;
            infos["contract"] = test_contract;
            infos["withdraw"] = test_withdraw;
            infos["redeem"] = test_redeem;
            //infos["setConfig1"] = test_setConfig1;
            //infos["setConfigRate"] = test_setConfigRate;
            //infos["setConfigFee"] = test_setConfigFee;
            infos["setContractA"] = test_setAccount;
            infos["setUpgrade"] = test_setUpgrade;
            infos["getConfig"] = test_getConfig;
            infos["settingSAR"] = test_settingSAR;
            infos["test"] = test_str;
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

            var oracleAddr = ThinNeo.Helper.GetAddressFromScriptHash(oracle_common.sc_wneo);
            var tokenAddr = ThinNeo.Helper.GetAddressFromScriptHash(token_common.sc_wneo);

            var result = await business_common.api_SendTransaction(prikey, business_common.sc_wneo, "openSAR4B",
               "(str)" + name,
                "(str)" + symbol,
                "(int)" + 8,
                "(addr)" + this.address,
                "(str)anchor_type_usd",
                "(addr)" + oracleAddr,
                "(addr)" + tokenAddr
                );
            subPrintLine(result);
        }

        async Task test_getSAR4B()
        {
            Console.WriteLine("Input target addr:");
            string addr = Console.ReadLine();

            var result = await business_common.api_InvokeScript(business_common.sc_wneo, "getSAR4B", "(addr)" + addr);
            business_common.ResultItem item = result.value;
            business_common.ResultItem[] items = item.subItem[0].subItem;

            if (items != null)
            {
                Console.WriteLine("name:"+items[0].AsString());
                Console.WriteLine("symbol:" + items[1].AsString());
                Console.WriteLine("decimals:" + items[2].AsInteger());
                Console.WriteLine("owner:" + ThinNeo.Helper.GetAddressFromScriptHash(items[3].AsHash160()));
                Console.WriteLine("txid:" + items[4].AsHashString());
                Console.WriteLine("locked:" + items[5].AsInteger());
                Console.WriteLine("hasDrawed:" + items[6].AsInteger());
                Console.WriteLine("status:" + items[7].AsInteger());
                Console.WriteLine("anchor:" + items[8].AsString());

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

            var sdsAddr = ThinNeo.Helper.GetAddressFromScriptHash(sdt_common.sc_sdt);
            var result = await business_common.api_SendTransaction(prikey, business_common.sc_wneo, "reserve",
              "(str)" + name,
              "(addr)" + this.address, 
              "(int)" + amount,
              "(addr)" + sdsAddr);
            subPrintLine(result);
        }

        async Task test_initToken()
        {
            var sdsAddr = ThinNeo.Helper.GetAddressFromScriptHash(sdt_common.sc_sdt);
            var tokenAddr = ThinNeo.Helper.GetAddressFromScriptHash(token_common.sc_wneo);
            var oracleAddr = ThinNeo.Helper.GetAddressFromScriptHash(oracle_common.sc_wneo);

            var result = await business_common.api_SendTransaction(prikey, business_common.sc_wneo, "initToken",
             "(addr)" + this.address,
             "(addr)" + oracleAddr,
             "(addr)" + sdsAddr,
             "(addr)" + tokenAddr);
            subPrintLine(result);
        }

        //增加
        async Task test_expande()
        {
            Console.WriteLine("Input target asset:");
            string name = Console.ReadLine();

            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var oracleAddr = ThinNeo.Helper.GetAddressFromScriptHash(oracle_common.sc_wneo);
            var tokenAddr = ThinNeo.Helper.GetAddressFromScriptHash(token_common.sc_wneo);

            var result = await business_common.api_SendTransaction(prikey, business_common.sc_wneo, "expande",
              "(str)" + name,
              "(addr)" + this.address,
              "(int)" + amount,
              "(addr)" + oracleAddr,
              "(addr)" + tokenAddr);
            subPrintLine(result);
        }

        //擦除
        async Task test_contract()
        {
            Console.WriteLine("Input target asset:");
            string name = Console.ReadLine();

            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();
            var tokenAddr = ThinNeo.Helper.GetAddressFromScriptHash(token_common.sc_wneo);

            var result = await business_common.api_SendTransaction(prikey, business_common.sc_wneo, "contract",
              "(str)" + name,
              "(addr)" + this.address,
              "(int)" + amount,
              "(addr)" + tokenAddr);
            subPrintLine(result);
        }

        //提现
        async Task test_withdraw()
        {
            Console.WriteLine("Input target asset:");
            string name = Console.ReadLine();

            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var oracleAddr = ThinNeo.Helper.GetAddressFromScriptHash(oracle_common.sc_wneo);
            var sdsAddr = ThinNeo.Helper.GetAddressFromScriptHash(sdt_common.sc_sdt);
            var tokenAddr = ThinNeo.Helper.GetAddressFromScriptHash(token_common.sc_wneo);

            var result = await business_common.api_SendTransaction(prikey, business_common.sc_wneo, "withdraw",
              "(str)" + name,
              "(addr)" + this.address,
              "(int)" + amount,
              "(addr)" + oracleAddr,
              "(addr)" + sdsAddr,
              "(addr)" + tokenAddr);
            subPrintLine(result);
        }


        async Task test_redeem()
        {
            Console.WriteLine("Input target addr:");
            string sarAddr = Console.ReadLine();

            var sdsAddr = ThinNeo.Helper.GetAddressFromScriptHash(sdt_common.sc_sdt);
            var oracleAddr = ThinNeo.Helper.GetAddressFromScriptHash(oracle_common.sc_wneo);
            var tokenAddr = ThinNeo.Helper.GetAddressFromScriptHash(token_common.sc_wneo);

            var result = await business_common.api_SendTransaction(prikey, business_common.sc_wneo, "redeem",
              
              "(addr)" + this.address,
              "(addr)" + sarAddr,
              "(addr)" + sdsAddr, 
              "(addr)" + oracleAddr, 
              "(addr)" + tokenAddr);
            subPrintLine(result);
        }

        async Task test_str()
        {
             
                string name = "6bb79223ec6593e908b8c4b9df1e9d0326783f78ba12fa94b095ecff44fb6187";
                byte[] data =  ThinNeo.Helper.HexString2Bytes(name);

                string str =  "0x" + ThinNeo.Helper.Bytes2HexString(data.Reverse().ToArray());
                Console.WriteLine("txid:"+str);

                var ss = 1 | 1 | 4;
                Console.WriteLine("ss:" + ss);


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
      
        async Task test_setConfigRate()
        {
            var result = await business_common.api_SendTransaction(prikey, business_common.sc_wneo, "setConfig", "(str)config_sds_rate", "(int)50");
            subPrintLine(result);

        }

        async Task test_setConfigFee()
        {
            var result = await business_common.api_SendTransaction(prikey, business_common.sc_wneo, "setConfig", "(str)service_fee", "(int)1000000000");
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

        async Task test_settingSAR()
        {
            Console.WriteLine("Input target asset:");
            string name = Console.ReadLine();

            Console.WriteLine("Input address:");
            string addr = Console.ReadLine();
            var result = await business_common.api_SendTransaction(prikey, business_common.sc_wneo, "settingSAR", "(str)"+name, "(addr)"+ addr);
            subPrintLine(result);
        }

        //升级合约
        async Task test_setUpgrade()
        {
            //byte[] prikey = ThinNeo.Helper.GetPrivateKeyFromWIF(Config.test_wif);
            //byte[] pubkey = ThinNeo.Helper.GetPublicKeyFromPrivateKey(prikey);
            //string address = ThinNeo.Helper.GetAddressFromPublicKey(pubkey);

            Dictionary<string, List<Utxo>> dir = await Helper.GetBalanceByAddress(Config.api, address);

            //从文件中读取合约脚本
            byte[] script = System.IO.File.ReadAllBytes("C:\\Neo\\SmartContracts\\0xd6fc6a7d9c148a88f0051578d44e9422eb57ac98.avm"); //这里填你的合约所在地址
            string str_script = ThinNeo.Helper.Bytes2HexString(script);
            byte[] aa = ThinNeo.Helper.HexString2Bytes(str_script);
            var ss = 1 | 1 | 4;
            using (ThinNeo.ScriptBuilder sb = new ThinNeo.ScriptBuilder())
            {
                //倒叙插入数据
                var array = new MyJson.JsonNode_Array();
                array.AddArrayValue("(bytes)" + str_script);
                array.AddArrayValue("(bytes)0710");
                array.AddArrayValue("(bytes)05");
                array.AddArrayValue("(int)" + 5);
                array.AddArrayValue("(str)合约升级测试");//name
                array.AddArrayValue("(str)1");//version
                array.AddArrayValue("(str)ss");//author
                array.AddArrayValue("(str)1");//email
                array.AddArrayValue("(str)sssss");//desc
                sb.EmitParamJson(array);//参数倒序入
                sb.EmitParamJson(new MyJson.JsonNode_ValueString("(str)upgrade"));//参数倒序入
                var shash = business_common.sc_wneo;
                sb.EmitAppCall(shash);

                string scriptPublish = ThinNeo.Helper.Bytes2HexString(sb.ToArray());
                byte[] postdata;
                var url = Helper.MakeRpcUrlPost(Config.api, "invokescript", out postdata, new MyJson.JsonNode_ValueString(scriptPublish));
                var result = await Helper.HttpPost(url, postdata);
                //string result = http.Post(api, "invokescript", new MyJson.JsonNode_Array() { new MyJson.JsonNode_ValueString(scriptPublish) },Encoding.UTF8);
                var consume = (((MyJson.Parse(result) as MyJson.JsonNode_Object)["result"] as MyJson.JsonNode_Array)[0] as MyJson.JsonNode_Object)["gas_consumed"].ToString();
                decimal gas_consumed = decimal.Parse(consume);
                ThinNeo.InvokeTransData extdata = new ThinNeo.InvokeTransData();
                extdata.gas = 500;// Math.Ceiling(gas_consumed - 10);
                extdata.script = sb.ToArray();

                //拼装交易体
                ThinNeo.Transaction tran = Helper.makeTran(dir[Config.id_GAS], null, new ThinNeo.Hash256(Config.id_GAS), extdata.gas);
                tran.version = 1;
                tran.extdata = extdata;
                tran.type = ThinNeo.TransactionType.InvocationTransaction;
                byte[] msg = tran.GetMessage();
                byte[] signdata = ThinNeo.Helper.Sign(msg, prikey);
                tran.AddWitness(signdata, pubkey, address);
                string txid = tran.GetHash().ToString();
                byte[] data = tran.GetRawData();
                string rawdata = ThinNeo.Helper.Bytes2HexString(data);
                url = Helper.MakeRpcUrlPost(Config.api, "sendrawtransaction", out postdata, new MyJson.JsonNode_ValueString(rawdata));
                result = await Helper.HttpPost(url, postdata);

                MyJson.JsonNode_Object resJO = (MyJson.JsonNode_Object)MyJson.Parse(result);
                Console.WriteLine(resJO.ToString());
            }
        }

    }
}
