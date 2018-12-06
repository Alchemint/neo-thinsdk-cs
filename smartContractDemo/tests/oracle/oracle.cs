using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ThinNeo;
using System.Numerics;
using ThinNeo.SmartContract;
using smartContractDemo.tests;

namespace smartContractDemo
{
    class oracleSwTest : ITest
    {
        public string Name => "Oracle" +  "合约测试";

        public string ID => "sw";
        byte[] adminPrikey = ThinNeo.Helper.GetPrivateKeyFromWIF(Config.testwif_admin);
        byte[] prikey;
        public string address = string.Empty;
        public string sdtAssetID = string.Empty;
        public string tokenizedAssetID = string.Empty;
        public ThinNeo.Hash160 shash;
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

        public oracleSwTest()
        {
            this.initMenu();
        }

        private void initMenu()
        {
            infos = new Dictionary<string, testAction>();

            infos["setAddress"] = test_setAddress;
            infos["addParaAddrWhit"] = test_addParaAddrWhit; 
            infos["removeParaAddrWhit"] = test_removeParaAddrWhit;
            infos["setTypeA"] = test_setTypeA;
            infos["getTypeA"] = test_getTypeA; 
            infos["setTypeB"] = test_setTypeB;
            infos["getTypeB"] = test_getTypeB;

            infos["setStructConfig"] = test_setStructConfig;
            infos["getStructConfig"] = test_getStructConfig;
            infos["getApprovedAddr"] = test_getApprovedAddr; 
            infos["getAddrWithParas"] = test_getAddrWithPrice; 
            infos["getAddrWithConfigs"] = test_getAddrWithConfigs;

            infos["setAccount"] = test_setAccount; 
            infos["getAccount"] = test_getAccount;

            infos["test_setUpgrade"] = test_setUpgrade;
            //infos["getTestData"] = test_getTestData;
            //infos["setAnchorPrice"] = test_setAnchorPrice;
            //infos["getAnchorPrice"] = test_getAnchorPrice;

            this.submenu = new List<string>(infos.Keys).ToArray();
        }

        public delegate void Method(ThinNeo.ScriptBuilder sb);//第一步：定义委托类型

        public async Task Demo()
        {
            //得到合约代码
            //var urlgetscript = Helper.MakeRpcUrl(oracleSw_Com.api, "getcontractstate", new MyJson.JsonNode_ValueString(oracleSw_Com.sc));
            //var resultgetscript = await Helper.HttpGet(urlgetscript);
            //var _json = MyJson.Parse(resultgetscript).AsDict();
            //var _resultv = _json["result"].AsList()[0].AsDict();
            
            //Console.WriteLine("reg=" + _resultv["script"].AsString());

            showMenu();

            prikey = ThinNeo.Helper.GetPrivateKeyFromWIF(Config.testwif);
            pubkey = ThinNeo.Helper.GetPublicKeyFromPrivateKey(prikey);
            address = ThinNeo.Helper.GetAddressFromPublicKey(pubkey);
            scripthash = ThinNeo.Helper.GetPublicKeyHashFromAddress(address);
            shash = new ThinNeo.Hash160(Config.sc_oracle);

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

        async Task test_setAddress()
        {
            Console.WriteLine("Input address:");
            this.address = Console.ReadLine();
        }

        async Task test_addParaAddrWhit()
        {
            Console.WriteLine("Input Para:");
            string para = Console.ReadLine();

            Console.WriteLine("Input address:");
            string address = Console.ReadLine();

            Console.WriteLine("Input state(1:授权,0:取消授权):");
            string state = Console.ReadLine();

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(Config.sc_oracle);
            var result = await oracleSw_Com.api_SendTransaction(adminPrikey, shash, "addParaAddrWhit",
                "(str)" + para,
                "(addr)" + address,
                "(int)" + state);
            subPrintLine(result);

        }

        async Task test_removeParaAddrWhit()
        {
            Console.WriteLine("Input Para:");
            string para = Console.ReadLine();

            Console.WriteLine("Input address:");
            string address = Console.ReadLine();

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(Config.sc_oracle);
            var result = await oracleSw_Com.api_SendTransaction(adminPrikey, shash, "removeParaAddrWhit",
                "(str)" + para,
                "(addr)" + address);

            subPrintLine(result); 
        }

        async Task test_setAccount()
        {
            Console.WriteLine("Input Para:");
            string para = Console.ReadLine();

            Console.WriteLine("Input address:");
            string address = Console.ReadLine();
              
            ThinNeo.Hash160 shash = new ThinNeo.Hash160(Config.sc_oracle);
            var result = await oracleSw_Com.api_SendTransaction(adminPrikey, shash, "setAccount",
                "(str)" + para,
                "(addr)" + address);
            subPrintLine(result);
        }

       async Task test_getAccount()
        {
            Console.WriteLine("Input key:");
            string key = Console.ReadLine();

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(Config.sc_oracle);
            var result = await oracleSw_Com.api_InvokeScript(shash, "getAccount", "(str)" + key);

            oracleSw_Com.ResultItem item = result.value;

            if (item != null)
            {
                byte[] data = item.data;

                string value = ThinNeo.Helper.Bytes2HexString(data);

                Console.WriteLine(key + ":" + value);
            }
            else
            {
                Console.WriteLine("item is null");
            } 
        }

        //设置配置
        async Task test_setTypeA()
        {

            Console.WriteLine("Input key:");
            string key = Console.ReadLine();

            Console.WriteLine("Input value:");
            string value = Console.ReadLine();

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(Config.sc_oracle);
            var result = await oracleSw_Com.api_SendTransaction(adminPrikey, shash, "setTypeA",
                "(str)" + key,
                "(int)" + value);
            subPrintLine(result);
        }

        //获取配置
        async Task test_getTypeA()
        {
            Console.WriteLine("Input key:");
            string key = Console.ReadLine();
            ThinNeo.Hash160 shash = new ThinNeo.Hash160(Config.sc_oracle);

            var result = await oracleSw_Com.api_InvokeScript(shash, "getTypeA", "(str)" + key);

            oracleSw_Com.ResultItem item = result.value;

            if (item != null)
            {
                byte[] data = item.data;

                BigInteger value = new BigInteger(data);

                Console.WriteLine(key + ":" + value);
            }
            else
            { 
                Console.WriteLine("item is null");
            }
        }

        //获取配置
        async Task test_getTestData()
        {
            Console.WriteLine("Input index:");
            string index = Console.ReadLine();

            Console.WriteLine("Input key:");
            string key = Console.ReadLine();

            //Console.WriteLine("Input keyIndex:");
            //string keyIndex = Console.ReadLine();
             
            Console.WriteLine("Input addr:");
            string addr = Console.ReadLine();

            //Console.WriteLine("Input value:");
            //string price = Console.ReadLine();
            ThinNeo.Hash160 shash = new ThinNeo.Hash160(Config.sc_oracle);

            var result = await oracleSw_Com.api_InvokeScript(shash, "test", "(int)" + index, "(str)" + key,"(addr)" + addr);

            oracleSw_Com.ResultItem item = result.value;

            if (item != null)
            {
                byte[] data = item.data;

                BigInteger value = new BigInteger(data);

                Console.WriteLine(key + ":" + value);
            }
            else
            {
                Console.WriteLine("item is null");
            }
        }

        async Task test_setTypeB()
        { 
            Console.WriteLine("Input key:");
            string key = Console.ReadLine();

            Console.WriteLine("Input from:");
            string from= Console.ReadLine();

            Console.WriteLine("Input price:");
            string value = Console.ReadLine();

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(Config.sc_oracle);
            var result = await oracleSw_Com.api_SendTransaction(adminPrikey, shash, "setTypeB",
                "(str)" + key, 
                "(addr)" + from,
                "(int)" + value);
            subPrintLine(result);
             
            Console.WriteLine("result = " + result);
        }

        async Task test_getTypeB()
        {
            Console.WriteLine("Input key:");
            string key = Console.ReadLine();

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(Config.sc_oracle);
            var result = await oracleSw_Com.api_InvokeScript(shash, "getTypeB", "(str)" + key);

            Console.WriteLine("result = " + result);

            oracleSw_Com.ResultItem item = result.value; 

            if (item != null)
            {  
                byte[] data = item.data;

                BigInteger value = new BigInteger(data);
                 
                Console.WriteLine(key + ":" + value); 
            }
            else {

                Console.WriteLine("item is null");
            }
        }

        async Task test_getApprovedAddr()
        {
            Console.WriteLine("Input key:");
            string key = Console.ReadLine();

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(Config.sc_oracle);
            var result = await oracleSw_Com.api_InvokeScript(shash, "getApprovedAddrs", "(str)" + key);

            oracleSw_Com.ResultItem item = result.value;

            if (item != null)
            {
                byte[] data = item.data; 
            }
            else
            {

                Console.WriteLine("item is null");
            }
        }

        async Task test_getAddrWithPrice()
        {
            Console.WriteLine("Input key:");
            string key = Console.ReadLine();

            var result = await oracleSw_Com.api_InvokeScript(shash, "getAddrWithParas", "(str)" + key);

            oracleSw_Com.ResultItem item = result.value;

            if (item != null)
            {
                byte[] data = item.data;
            }
            else
            {

                Console.WriteLine("item is null");
            }
        }

        async Task test_getAddrWithConfigs()
        {
            Console.WriteLine("Input addr:");
            string key = Console.ReadLine();

            var result = await oracleSw_Com.api_InvokeScript(shash, "getAddrWithConfigs", "(addr)" + key);

            oracleSw_Com.ResultItem item = result.value;

            if (item != null)
            {
                byte[] data = item.data;
            }
            else
            {

                Console.WriteLine("item is null");
            }
        }

        async Task test_Test()
        {
            /*
            Console.WriteLine("Input index:");
            string index = Console.ReadLine();

            Console.WriteLine("Input key:");
            string key = Console.ReadLine();

            Console.WriteLine("Input val:");
            string val = Console.ReadLine();

            if (index == "1")
            {  
                var result = await oracleSw_Com.api_SendTransaction(prikey, oracleSw_Com.sc_oracle, "test", "(int)" + index, "(str)" + key, "(int)" + val);
                subPrintLine(result); 
            }

            if (index == "2")
            { 
                var result = await oracleSw_Com.api_InvokeScript(oracleSw_Com.sc_oracle, "test", "(int)" + index, "(str)" + key, "(int)" + val);

                oracleSw_Com.ResultItem item = result.value;
            }
            */
            
        }

        async Task test_setStructConfig()
        {  
            var result = await oracleSw_Com.api_SendTransaction(adminPrikey, shash, "setStructConfig");
            subPrintLine(result);
        }
        async Task test_getStructConfig()
        {
            var result = await oracleSw_Com.api_InvokeScript(shash, "getStructConfig");

            oracleSw_Com.ResultItem item = result.value; 
            oracleSw_Com.ResultItem[] items = item.subItem;

            Console.WriteLine("items = ", items);

            if (items != null)
            {
                Console.WriteLine("from:" + ThinNeo.Helper.GetAddressFromScriptHash(items[0].AsHash160()));
                Console.WriteLine("liquidate_line_rate_b:" + items[1].AsHashString());
                Console.WriteLine("liquidate_line_rate_c:" + items[2].AsHashString());
                Console.WriteLine("liquidate_dis_rate_c:" + items[3].AsInteger());
                Console.WriteLine("fee_rate_c:" + items[4].AsInteger() + " PNEO");
                Console.WriteLine("liquidate_top_rate_c:" + items[5].AsInteger() + " SDUSD");
                Console.WriteLine("liquidate_line_rateT_c:" + items[6].AsInteger());
                Console.WriteLine("issuing_fee_c:" + items[6].AsInteger());
                Console.WriteLine("issuing_fee_b:" + items[6].AsInteger());
                Console.WriteLine("debt_top_c:" + items[6].AsInteger());
            }
            else
            {
                Console.WriteLine("no txInfo exists");
            } 
        }

        async Task test_setUpgrade()
        {
            byte[] pubkey_admin = ThinNeo.Helper.GetPublicKeyFromPrivateKey(adminPrikey);
            string address = ThinNeo.Helper.GetAddressFromPublicKey(pubkey_admin);

            Dictionary<string, List<Utxo>> dir = await Helper.GetBalanceByAddress(Config.api, address);

            //从文件中读取合约脚本
            //byte[] script = System.IO.File.ReadAllBytes("C:\\Neo\\SmartContracts\\0x89151760dba47464bbed6600b651e210996a318b.avm"); //这里填你的合约所在地址
            byte[] script = System.IO.File.ReadAllBytes("C:\\Users\\wqq19\\Documents\\workspace\\solution\\OracleContract\\bin\\Debug\\0x2eaef11fa90014ccf4ae8bfabc0f58e7fc8bd591.avm");
            
            string str_script = ThinNeo.Helper.Bytes2HexString(script);
            byte[] aa = ThinNeo.Helper.HexString2Bytes(str_script);
            var ss = 1 | 2 | 4;
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
                //var shash = Config.oracleAssetID;
                sb.EmitAppCall(shash);

                string scriptPublish = ThinNeo.Helper.Bytes2HexString(sb.ToArray());
                byte[] postdata;
                var url = Helper.MakeRpcUrlPost(Config.api, "invokescript", out postdata, new MyJson.JsonNode_ValueString(scriptPublish));
                var result = await Helper.HttpPost(url, postdata);
                //string result = http.Post(api, "invokescript", new MyJson.JsonNode_Array() { new MyJson.JsonNode_ValueString(scriptPublish) },Encoding.UTF8);
                //var consume = (((MyJson.Parse(result) as MyJson.JsonNode_Object)["result"] as MyJson.JsonNode_Array)[0] as MyJson.JsonNode_Object)["gas_consumed"].ToString();
                //decimal gas_consumed = decimal.Parse(consume);
                ThinNeo.InvokeTransData extdata = new ThinNeo.InvokeTransData();
                extdata.gas = 1010;// Math.Ceiling(gas_consumed - 10);
                extdata.script = sb.ToArray();

                //拼装交易体
                ThinNeo.Transaction tran = Helper.makeTran(dir[Config.id_GAS], null, new ThinNeo.Hash256(Config.id_GAS), extdata.gas);
                tran.version = 1;
                tran.extdata = extdata;
                tran.type = ThinNeo.TransactionType.InvocationTransaction;
                byte[] msg = tran.GetMessage();
                byte[] signdata = ThinNeo.Helper.Sign(msg, adminPrikey);
                tran.AddWitness(signdata, pubkey_admin, address);
                string txid = tran.GetHash().ToString();
                byte[] data = tran.GetRawData();
                string rawdata = ThinNeo.Helper.Bytes2HexString(data);
                url = Helper.MakeRpcUrlPost(Config.api, "sendrawtransaction", out postdata, new MyJson.JsonNode_ValueString(rawdata));
                result = await Helper.HttpPost(url, postdata);

                MyJson.JsonNode_Object resJO = (MyJson.JsonNode_Object)MyJson.Parse(result);
                Console.WriteLine(resJO.ToString());
            }
        }

            async Task test_setAnchorPrice()
        {
            Console.WriteLine("Input key:");
            string key = Console.ReadLine();
             
            Console.WriteLine("Input price:");
            string value = Console.ReadLine();

            var result = await oracleSw_Com.api_SendTransaction(adminPrikey, shash, "setAnchorPrice",
                "(str)" + key, 
                "(int)" + value);
            subPrintLine(result);
        }

        async Task test_getAnchorPrice()
        {
            Console.WriteLine("Input key:");
            string key = Console.ReadLine();
            var result = await oracleSw_Com.api_InvokeScript(shash, "getAnchorPrice" , "(str)" + key);

            oracleSw_Com.ResultItem item = result.value;

            if (item != null)
            {
                byte[] data = item.data;

                BigInteger value = new BigInteger(data);

                Console.WriteLine(key + ":" + value);
            }
            else  
            {

                Console.WriteLine("item is null");
            }
        }

    }

}
