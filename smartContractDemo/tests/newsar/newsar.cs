﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ThinNeo;
using static MyJson;
using smartContractDemo.tests;
using System.Numerics;
using System.IO;

namespace smartContractDemo
{
    class newsarTest : ITest
    {
        public string Name => "SAR 合约测试";

        public string ID => "newsar";
        byte[] prikey;
        public string address;
        byte[] scripthash;
        byte[] pubkey;
        Hash160 reg_sc;//注册器合约地址
        public delegate Task testAction();
        public Dictionary<string, testAction> infos = null;// = new Dictionary<string, testAction>();
        string[] submenu;
        public int ten_pow = 100000000;
        void subPrintLine(string line)
        {
            Console.WriteLine("    " + line);
        }

        public newsarTest()
        {
            this.initMenu();
        }

        private void initMenu()
        {
            infos = new Dictionary<string, testAction>();
            infos["openSAR4C"] = test_openSAR;
            infos["getSAR4C"] = test_getSAR;
            infos["getAllSAR4C"] = test_getAllSAR4C;
            infos["reserve"] = test_lock;
            infos["recharge"] = test_recharge;
            infos["expande"] = test_draw;
            infos["withdraw"] = test_free;
            infos["withdrawT"] = test_withdrawT;
            infos["contract"] = test_wipe;
            infos["close"] = test_shut;
            infos["rescue"] = test_bite;
            infos["rescueT"] = test_rescueT;
            infos["getRescue"] = test_getRescue;

            //infos["balanceOfRedeem"] = test_balanceOfRedeem;
            //infos["redeem"] = test_redeem;
            //infos["give"] = test_give;
            infos["setAccount"] = test_setCallScript;
            infos["setOracleAccount"] = test_setCallScript2;
            infos["setBondAccount"] = test_setBondAccount;
            infos["removeBondAccount"] = test_removeBondAccount;

            infos["getBondGlobal"] = test_getBondGlobal;
            infos["getTXInfo"] = test_getTXInfo;
            infos["getSARTxInfo"] = test_getSARTxInfo;
            infos["setConfig"] = test_setConfig;
            infos["getConfig"] = test_getConfig;
            infos["setUpgrade"] = test_setUpgrade;

            //infos["mintSDT"] = test_mintSDT;


            this.submenu = new List<string>(infos.Keys).ToArray();
        }

        public delegate void Method(ThinNeo.ScriptBuilder sb);//第一步：定义委托类型

        public async Task Demo()
        {
            //得到合约代码
            //var urlgetscript = Helper.MakeRpcUrl(newsar_common.api, "getcontractstate", new MyJson.JsonNode_ValueString(newsar_common.sc));
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
        async Task test_setCallScript2()
        {
            byte[] prikey_admin = ThinNeo.Helper.GetPrivateKeyFromWIF(Config.testwif_admin);

            var addr = ThinNeo.Helper.GetAddressFromScriptHash(oracle_common.sc_wneo);
            Console.WriteLine("oracle address:" + addr);

            var result = await newsar_common.api_SendbatchTransaction(prikey_admin, newsar_common.sc_sar, "setAccount",
               "(str)oracle_account",
               "(addr)" + addr);
            subPrintLine(result);
        }

            //授权转账操作
            async Task test_setCallScript()
        {

           byte[] prikey_admin = ThinNeo.Helper.GetPrivateKeyFromWIF(Config.testwif_admin);

            //var addr = ThinNeo.Helper.GetAddressFromScriptHash(sds_common.sc_sds);
            //Console.WriteLine("sds address:" + addr);

            //var result = await newsar_common.api_SendbatchTransaction(prikey_admin, newsar_common.sc_sar, "setAccount",
            //   "(str)sds_account",
            //   "(addr)" + addr);
            //subPrintLine(result);

            //addr = ThinNeo.Helper.GetAddressFromScriptHash(datacenter_common.sc_wneo);
            //Console.WriteLine("oracle address:" + addr);

            //result = await newsar_common.api_SendbatchTransaction(prikey_admin, newsar_common.sc_sar, "setAccount",
            //   "(str)oracle_account",
            //   "(addr)" + addr);
            //subPrintLine(result);

            //addr = ThinNeo.Helper.GetAddressFromScriptHash(sdusd_common.sc_sdusd);
            //Console.WriteLine("sdusd address:" + addr);

            //result = await newsar_common.api_SendbatchTransaction(prikey_admin, newsar_common.sc_sar, "setAccount",
            //   "(str)sdusd_account",
            //   "(addr)" + addr);
            //subPrintLine(result);

           var addr = ThinNeo.Helper.GetAddressFromScriptHash(Config.sar4c);
            Console.WriteLine("sar address:" + addr);
           var result = await newsar_common.api_SendbatchTransaction(prikey_admin, newsar_common.sc_sar, "setAccount",
               "(str)storage_account",
               "(addr)" + addr);
            subPrintLine(result);

            //addr = ThinNeo.Helper.GetAddressFromScriptHash(cneo_common.sc_cneo);
            //Console.WriteLine("cneo address:" + addr);
            //result = await newsar_common.api_SendbatchTransaction(prikey_admin, newsar_common.sc_sar, "setAccount",
            //   "(str)cneo_price",
            //   "(addr)" + addr);
            //subPrintLine(result);

            //addr = ThinNeo.Helper.GetAddressFromScriptHash(sneo_common.sc_sneo);
            //Console.WriteLine("sneo address:" + addr);

            //result = await newsar_common.api_SendbatchTransaction(prikey_admin, newsar_common.sc_sar, "setAccount",
            //   "(str)sasset_account",
            //   "(addr)" + addr);
            //subPrintLine(result);

            //addr = ThinNeo.Helper.GetAddressFromScriptHash(cgas_common.sc_cgas);
            //Console.WriteLine("cgas address:" + addr);
            //result = await newsar_common.api_SendbatchTransaction(prikey_admin, newsar_common.sc_sar, "setAccount",
            //   "(str)cgas_price",
            //   "(addr)" + addr);
            //subPrintLine(result);

            //addr = ThinNeo.Helper.GetAddressFromScriptHash(sneo_common.sc_sneo);
            //Console.WriteLine("sneo address:" + addr);
            //result = await newsar_common.api_SendbatchTransaction(prikey_admin, newsar_common.sc_sar, "setAccount",
            //   "(str)sneo_price",
            //   "(addr)" + addr);
            //subPrintLine(result);

        }

        async Task test_setBondAccount()
        {
            prikey = ThinNeo.Helper.GetPrivateKeyFromWIF(Config.testwif_admin);

            Console.WriteLine("Input address:");
            string addr = Console.ReadLine();

            var result = await newsar_common.api_SendbatchTransaction(prikey, newsar_common.sc_sar, "setBondAccount",
              "(addr)" + addr);
            subPrintLine(result);

        }

        async Task test_removeBondAccount()
        {
            Console.WriteLine("Input address:");
            string addr = Console.ReadLine();

            var result = await newsar_common.api_SendbatchTransaction(prikey, newsar_common.sc_sar, "removeBondAccount",
              "(addr)" + addr);
            subPrintLine(result);
        }

        //创建CDP在仓
        async Task test_openSAR()
        {
            Console.WriteLine("SAR Asset type:");
            string assetType = Console.ReadLine();

            var result = await newsar_common.api_SendbatchTransaction(prikey, newsar_common.sc_sar, "openSAR4C", 
                "(addr)" + this.address,
                "(str)"+assetType);
            subPrintLine(result);
        }

        //锁仓
        async Task test_lock()
        {
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(newsar_common.sc_sar);
            var result = await newsar_common.api_SendbatchTransaction(prikey, newsar_common.sc_sar, "reserve",
                "(addr)" + this.address,
                "(int)" +  double.Parse(amount)* ten_pow);
            subPrintLine(result);
        }

        async Task test_recharge()
        {
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(newsar_common.sc_sar);
            var result = await newsar_common.api_SendbatchTransaction(prikey, newsar_common.sc_sar, "recharge",
                "(addr)" + this.address,
                "(int)" + double.Parse(amount) * ten_pow);
            subPrintLine(result);
        }

        //取钱
        async Task test_draw()
        {
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();
            var result = await newsar_common.api_SendbatchTransaction(prikey, newsar_common.sc_sar, "expande",
                "(addr)" + this.address,
                "(int)" + double.Parse(amount) * ten_pow);
            subPrintLine(result);
        }

        //释放
        async Task test_free()
        {
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(newsar_common.sc_sar);
            var result = await newsar_common.api_SendbatchTransaction(prikey, shash, "withdraw",
                "(addr)" + this.address,
                "(int)" + double.Parse(amount) * ten_pow);
            subPrintLine(result);
        }

        async Task test_withdrawT()
        {
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            ThinNeo.Hash160 shash = new ThinNeo.Hash160(newsar_common.sc_sar);
            var result = await newsar_common.api_SendbatchTransaction(prikey, shash, "withdrawT",
                "(addr)" + this.address,
                "(int)" + double.Parse(amount) * ten_pow);
            subPrintLine(result);
        }

        //平仓
        async Task test_wipe()
        {
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await newsar_common.api_SendbatchTransaction(prikey, newsar_common.sc_sar, "contract",
                "(addr)" + this.address,
                "(int)" + double.Parse(amount) * ten_pow);
            subPrintLine(result);
        }

        //转移CDP
        async Task test_give()
        {
            Console.WriteLine("Input to Address:");
            string toAddress = Console.ReadLine();

            var result = await newsar_common.api_SendbatchTransaction(prikey, newsar_common.sc_sar, "give",
                "(addr)" + this.address,
                "(addr)" + toAddress);
            subPrintLine(result);
        }

        //关闭在仓
        async Task test_shut()
        {
            var result = await newsar_common.api_SendbatchTransaction(prikey, newsar_common.sc_sar, "close",
                "(addr)" + this.address);
            subPrintLine(result);
        }

        //强制关闭在仓
        async Task test_bite()
        {
            Console.WriteLine("Input other address:");
            var otherAdd = Console.ReadLine();

            Console.WriteLine("Input amount:");
            var mount = Console.ReadLine();

            var result = await newsar_common.api_SendbatchTransaction(prikey, newsar_common.sc_sar, "rescue",
                "(addr)" + otherAdd,
                "(addr)" + this.address,
                "(int)"+ double.Parse(mount) * ten_pow);
            subPrintLine(result);
        }

        async Task test_rescueT()
        {
            //Console.WriteLine("Input other address:");
            //var otherAdd = Console.ReadLine();

            Console.WriteLine("Input amount:");
            var mount = Console.ReadLine();

            //var sneoAddr = ThinNeo.Helper.GetAddressFromScriptHash(sneo_common.sc_sneo);
            //Console.WriteLine("sneo address:" + sneoAddr);

            //var sdusdAddr = ThinNeo.Helper.GetAddressFromScriptHash(sdusd_common.sc_sdusd);
            //Console.WriteLine("sdusd address:" + sdusdAddr);

            //var oracleAddr = ThinNeo.Helper.GetAddressFromScriptHash(oracle_common.sc_wneo);
            //Console.WriteLine("oracle address:" + oracleAddr);


            var result = await newsar_common.api_SendbatchTransaction(prikey, newsar_common.sc_sar, "rescueT",
                "(addr)" + this.address,
                "(addr)" + this.address,
                "(int)" + double.Parse(mount) * ten_pow);
            subPrintLine(result);
        }

        async Task test_getRescue()
        {
            Console.WriteLine("Input asset type:");
            var assetType = Console.ReadLine();
            var result = await newsar_common.api_InvokeScript(newsar_common.sc_sar, "getRescue",
                "(str)"+ assetType,
                "(addr)" + this.address);
            newsar_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //赎回剩余PNEO
        async Task test_redeem()
        {
            var result = await newsar_common.api_SendbatchTransaction(prikey, newsar_common.sc_sar, "redeem",
                "(addr)" + this.address);
            subPrintLine(result);
        }

        async Task test_balanceOfRedeem()
        {
            var result = await newsar_common.api_InvokeScript(newsar_common.sc_sar, "balanceOfRedeem", "(addr)" + this.address);
            newsar_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //查询需要赎回余额
        async Task test_getBondGlobal()
        {
            var result = await newsar_common.api_InvokeScript(newsar_common.sc_sar, "getBondGlobal");
            newsar_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //查询交易信息
        async Task test_getTXInfo()
        {
            Console.WriteLine("Input txid:");
            string txid = Console.ReadLine();
            var result = await newsar_common.api_InvokeScript(newsar_common.sc_sar, "getTXInfo", "(hex256)" + txid);
            newsar_common.ResultItem item = result.value;
            newsar_common.ResultItem[] items = item.subItem[0].subItem;

            //查询交易详细信息
            Console.WriteLine("from:" + ThinNeo.Helper.GetAddressFromScriptHash(items[0].AsHash160()));
            Console.WriteLine("to:" + ThinNeo.Helper.GetAddressFromScriptHash(items[1].AsHash160()));
            Console.WriteLine("value:" + items[2].AsInteger());
        }

        //查询SAR信息
        async Task test_getSAR()
        {
            Console.WriteLine("Current address:"+this.address);
            Console.WriteLine("Input address:");
            string addr = Console.ReadLine();
            if (addr == null || addr == "")
                addr = address;
            var result = await newsar_common.api_InvokeScript(newsar_common.sc_sar, "getSAR4C", "(addr)" + addr);
            newsar_common.ResultItem item = result.value;
            newsar_common.ResultItem[] items = item.subItem[0].subItem;

            if (items != null)
            {
                Console.WriteLine("from:" + ThinNeo.Helper.GetAddressFromScriptHash(items[0].AsHash160()));
                Console.WriteLine("txid:" + items[1].AsHashString());
                Console.WriteLine("locked:" + items[2].AsInteger());
                Console.WriteLine("hasDrawed:" + items[3].AsInteger());
                Console.WriteLine("assetType:" + items[4].AsString());
                Console.WriteLine("status:" + items[5].AsInteger());
                Console.WriteLine("bondLocked:" + items[6].AsInteger());
                Console.WriteLine("bondDrawed:" + items[7].AsInteger());
                Console.WriteLine("lastHeight:" + items[8].AsInteger());
                Console.WriteLine("fee:" + items[9].AsInteger());
                Console.WriteLine("sdsFee:" + items[10].AsInteger());


            }
            else
            {
                Console.WriteLine("no sar exists");
            }
        }

        async Task test_getAllSAR4C()
        {
            //查询所有状态为1的SAR
            DateTime dt = DateTime.Now;
            Console.WriteLine("Start time:" + dt);

            byte[] postdata;
            var url = Helper.MakeRpcUrlPost(Config.api, "getsar4CListByType", out postdata,
                new JsonNode_ValueNumber(1),
                new JsonNode_ValueNumber(1000),
                new JsonNode_ValueNumber(1));
            var result = await Helper.HttpPost(url, postdata);

            List<string> list = new List<string>();
            MyJson.JsonNode_Object json = MyJson.Parse(result) as MyJson.JsonNode_Object;
            JsonNode_Array arrs = json["result"].AsList();

            foreach (JsonNode_Object ob in arrs)
            {
                string addr = ob["addr"].AsString();

                Console.WriteLine("addr" + addr);

                //查询旧合约SAR
                var result2 = await newsar_common.api_InvokeScript(newsar_common.sc_sar, "getSAR4C", "(addr)" + addr);
                newsar_common.ResultItem item = result2.value;
                newsar_common.ResultItem[] items = item.subItem[0].subItem;

                if (items != null)
                {
                    string owner = ThinNeo.Helper.GetAddressFromScriptHash(items[0].AsHash160());
                    string txid = items[1].AsHashString();
                    BigInteger locked = items[2].AsInteger();
                    BigInteger hasDrawed = items[3].AsInteger();
                    string assetType = items[4].AsString();
                    BigInteger status = items[5].AsInteger();

                    Console.WriteLine("###############");
                    Console.WriteLine("from:" + owner);
                    Console.WriteLine("txid:" + txid);
                    Console.WriteLine("locked:" + locked);
                    Console.WriteLine("hasDrawed:" + hasDrawed);
                    Console.WriteLine("assetType:" + assetType);
                    Console.WriteLine("status:" + status);
                    //Console.WriteLine("bondLocked:" + items[6].AsInteger());
                    //Console.WriteLine("bondDrawed:" + items[7].AsInteger());


                }
            }
            DateTime end = DateTime.Now;
            Console.WriteLine("End time:" + end);

        }

        //查询SAR详细交易信息
        async Task test_getSARTxInfo()
        {
            Console.WriteLine("Input txid:");
            string txid = Console.ReadLine();
            var result = await newsar_common.api_InvokeScript(newsar_common.sc_sar, "getSARTxInfo", "(hex256)" + txid);
            newsar_common.ResultItem item = result.value;
            newsar_common.ResultItem[] items = item.subItem[0].subItem;

            if (items != null)
            {
                Console.WriteLine("from:" + ThinNeo.Helper.GetAddressFromScriptHash(items[0].AsHash160()));
                Console.WriteLine("cdpTxid:" + items[1].AsHashString());
                Console.WriteLine("txid:" + items[2].AsHashString());
                Console.WriteLine("operated:" + items[3].AsInteger());
                Console.WriteLine("hasLocked:" + items[4].AsInteger() + " PNEO");
                Console.WriteLine("hasDrawed:" + items[5].AsInteger() + " SDUSD");
                Console.WriteLine("type:" + items[6].AsInteger());
            }
            else
            {
                Console.WriteLine("no txInfo exists");
            }

        }

        //设置配置信息
        async Task test_setConfig()
        {
            //Console.WriteLine("Input config key:");
            //string key = Console.ReadLine();

            //Console.WriteLine("Input config value:");
            //string value = Console.ReadLine();
            byte[] prikey_admin = ThinNeo.Helper.GetPrivateKeyFromWIF(Config.testwif_admin);

            var result = await newsar_common.api_SendbatchTransaction(prikey_admin, newsar_common.sc_sar, "setConfig", "(str)sar_state", "(int)1");
            subPrintLine(result);

        }

        //查询配置信息
        async Task test_getConfig()
        {
            //Console.WriteLine("Input config key:");
            //string key = Console.ReadLine();

            var result = await newsar_common.api_InvokeScript(newsar_common.sc_sar, "getConfig", "(str)sar_state");
            newsar_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //升级合约
        async Task test_setUpgrade()
        {
            byte[] prikey_admin = ThinNeo.Helper.GetPrivateKeyFromWIF(Config.testwif_admin);
            byte[]  pubkey_admin = ThinNeo.Helper.GetPublicKeyFromPrivateKey(prikey_admin);
            string address = ThinNeo.Helper.GetAddressFromPublicKey(pubkey_admin);

            Dictionary<string, List<Utxo>> dir = await Helper.GetBalanceByAddress(Config.api, address);

            //从文件中读取合约脚本
            byte[] script = System.IO.File.ReadAllBytes("C:\\Neo\\SmartContracts\\0x801c2e175451d4e732b49cb95db3fa988f915234.avm"); //这里填你的合约所在地址
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
                array.AddArrayValue("(int)" + 7);
                array.AddArrayValue("(str)合约升级测试");//name
                array.AddArrayValue("(str)1");//version
                array.AddArrayValue("(str)ss");//author
                array.AddArrayValue("(str)1");//email
                array.AddArrayValue("(str)sssss");//desc
                sb.EmitParamJson(array);//参数倒序入
                sb.EmitParamJson(new MyJson.JsonNode_ValueString("(str)upgrade"));//参数倒序入
                var shash = newsar_common.sc_sar;
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
                byte[] signdata = ThinNeo.Helper.Sign(msg, prikey_admin);
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


    }

}
