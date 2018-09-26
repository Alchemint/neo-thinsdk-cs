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
    class cgasTest : ITest
    {
        public string Name => "CGAS 合约测试";

        public string ID => "cgas";
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

        public cgasTest()
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
            infos["getTXInfo"] = test_getTXInfo;
            infos["mintTokensGAS"] = test_mintTokensGAS;
            infos["refund"] = test_refund;
            infos["getRefund"] = test_getRefund;
            infos["getRefundTarget"] = test_getRefundTarget;
            infos["claimStep1"] = test_claimStep1;
            infos["claimStep2"] = test_claimStep2;
            //infos["claimStep3"] = test_claimStep3;

            this.submenu = new List<string>(infos.Keys).ToArray();
        }

        public delegate void Method(ThinNeo.ScriptBuilder sb);//第一步：定义委托类型

        public async Task Demo()
        {
            //得到合约代码
            var urlgetscript = Helper.MakeRpcUrl(Config.api, "getcontractstate", new MyJson.JsonNode_ValueString(cgas_common.sc));
            var resultgetscript = await Helper.HttpGet(urlgetscript);
            var _json = MyJson.Parse(resultgetscript).AsDict();
            var _resultv = _json["result"].AsList()[0].AsDict();

            //Console.WriteLine(_resultv["script"].AsString());

            n55contract = ThinNeo.Helper.HexString2Bytes(_resultv["script"].AsString());

            //Console.WriteLine(n55contract.Length);


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

        //查询总量
        async Task test_totalSupply()
        {
            var result = await cgas_common.api_InvokeScript(cgas_common.sc_cgas, "totalSupply", null);
            cgas_common.ResultItem item = result.value;

            Console.WriteLine(Helper.changeDecimals(item.subItem[0].AsInteger(),8));
        }

        //查询名字
        async Task test_name()
        {
            var result = await cgas_common.api_InvokeScript(cgas_common.sc_cgas, "name", null);
            cgas_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsString());
        }

        //查询标志
        async Task test_symbol()
        {
            var result = await cgas_common.api_InvokeScript(cgas_common.sc_cgas, "symbol", null);
            cgas_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsString());
        }

        //查询最小单位
        async Task test_decimals()
        {
            var result = await cgas_common.api_InvokeScript(cgas_common.sc_cgas, "decimals", null);
            cgas_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //查询余额
        async Task test_BalanceOf()
        {
            Console.WriteLine("Input target address:");
            string addr = Console.ReadLine();
 

            var result = await cgas_common.api_InvokeScript(cgas_common.sc_cgas, "balanceOf", "(addr)" + addr);
            cgas_common.ResultItem item = result.value;

            Console.WriteLine(Helper.changeDecimals(item.subItem[0].AsInteger(),8));
        }

        //转账
        async Task test_Transfer()
        {
            Console.WriteLine("Input target address:");
            string addressto = Console.ReadLine();

            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await cgas_common.api_SendbatchTransaction(prikey, cgas_common.sc_cgas, "transfer",
              "(addr)" + this.address,
              "(addr)" + addressto,
              "(int)" + amount);
            subPrintLine(result);
        }

        async Task test_getTXInfo()
        {
            Console.WriteLine("Input txid:");
            string txid = Console.ReadLine();
            var result = await cgas_common.api_InvokeScript(cgas_common.sc_cgas, "getTxInfo", "(hex256)" + txid);
            cgas_common.ResultItem item = result.value;
            cgas_common.ResultItem[] items = item.subItem[0].subItem;

            //查询交易详细信息
            Console.WriteLine("from:" + ThinNeo.Helper.GetAddressFromScriptHash(items[0].AsHash160()));
            Console.WriteLine("to:" + ThinNeo.Helper.GetAddressFromScriptHash(items[1].AsHash160()));
            Console.WriteLine("value:" + items[2].AsInteger());
        }

        //NEO兑换代币
        async Task test_mintTokensNEO()
        {
            Console.WriteLine("Input mint tokens:");
            string mount = Console.ReadLine();
            //获取地址的资产列表
            Dictionary<string, List<Utxo>> dir = await Helper.GetBalanceByAddress(Config.api, address);
            if (dir.ContainsKey(Config.id_NEO) == false)
            {
                Console.WriteLine("no neo");
                return;
            }
            ThinNeo.Transaction tran = null;
            {
                byte[] script = null;
                using (var sb = new ThinNeo.ScriptBuilder())
                {
                    var array = new MyJson.JsonNode_Array();
                    array.AddArrayValue("(str)neo");
                    sb.EmitParamJson(array);//参数倒序入
                    sb.EmitParamJson(new MyJson.JsonNode_ValueString("(str)mintTokens"));//参数倒序入
                    sb.EmitAppCall(cgas_common.sc_cgas);//nep5脚本
                    script = sb.ToArray();
                }
                var targetaddr = ThinNeo.Helper.GetAddressFromScriptHash(cgas_common.sc_cgas);
                Console.WriteLine("contract address=" + targetaddr);//往合约地址转账

                //生成交易
                tran = Helper.makeTran(dir[Config.id_NEO], targetaddr, new ThinNeo.Hash256(Config.id_NEO), Decimal.Parse(mount));
                tran.type = ThinNeo.TransactionType.InvocationTransaction;
                var idata = new ThinNeo.InvokeTransData();
                tran.extdata = idata;
                idata.script = script;
            }

            //sign and broadcast
            var signdata = ThinNeo.Helper.Sign(tran.GetMessage(), prikey);
            tran.AddWitness(signdata, pubkey, address);
            var trandata = tran.GetRawData();
            var strtrandata = ThinNeo.Helper.Bytes2HexString(trandata);
            byte[] postdata;
            var url = Helper.MakeRpcUrlPost(Config.api, "sendrawtransaction", out postdata, new MyJson.JsonNode_ValueString(strtrandata));
            var result = await Helper.HttpPost(url, postdata);
            Console.WriteLine("得到的结果是：" + result);
            var json = MyJson.Parse(result).AsDict();
            if (json.ContainsKey("result"))
            {
                var resultv = json["result"].AsList()[0].AsDict();
                var txid = resultv["txid"].AsString();

                Console.WriteLine("txid=" + txid);
            }

        }

        //GAS兑换代币
        async Task test_mintTokensGAS()
        {
            Console.WriteLine("Input mint tokens:");
            string mount = Console.ReadLine();
            //获取地址的资产列表
            Dictionary<string, List<Utxo>> dir = await Helper.GetBalanceByAddress(Config.api, address);
            if (dir.ContainsKey(Config.id_GAS) == false)
            {
                Console.WriteLine("no gas");
                return;
            }
            ThinNeo.Transaction tran = null;
            {
                byte[] script = null;
                using (var sb = new ThinNeo.ScriptBuilder())
                {
                    var array = new MyJson.JsonNode_Array();
                    array.AddArrayValue("(str)gas");
                    sb.EmitParamJson(array);//参数倒序入
                    sb.EmitParamJson(new MyJson.JsonNode_ValueString("(str)mintTokens"));//参数倒序入
                    sb.EmitAppCall(cgas_common.sc_cgas);//nep5脚本
                    script = sb.ToArray();
                }
                var targetaddr = ThinNeo.Helper.GetAddressFromScriptHash(cgas_common.sc_cgas);
                Console.WriteLine("contract address=" + targetaddr);//往合约地址转账

                //生成交易
                tran = Helper.makeTran(dir[Config.id_GAS], targetaddr, new ThinNeo.Hash256(Config.id_GAS), Decimal.Parse(mount));
                tran.type = ThinNeo.TransactionType.InvocationTransaction;
                var idata = new ThinNeo.InvokeTransData();
                tran.extdata = idata;
                idata.script = script;
            }

            //sign and broadcast
            var signdata = ThinNeo.Helper.Sign(tran.GetMessage(), prikey);
            tran.AddWitness(signdata, pubkey, address);
            var trandata = tran.GetRawData();
            var strtrandata = ThinNeo.Helper.Bytes2HexString(trandata);
            byte[] postdata;
            var url = Helper.MakeRpcUrlPost(Config.api, "sendrawtransaction", out postdata, new MyJson.JsonNode_ValueString(strtrandata));
            var result = await Helper.HttpPost(url, postdata);
            Console.WriteLine("得到的结果是：" + result);
            var json = MyJson.Parse(result).AsDict();
            if (json.ContainsKey("result"))
            {
                var resultv = json["result"].AsList()[0].AsDict();
                var txid = resultv["txid"].AsString();

                Console.WriteLine("txid=" + txid);
            }

        }

        //设置配置信息
        async Task test_setConfig()
        {
            Console.WriteLine("Input Config key:");
            string key = Console.ReadLine();

            Console.WriteLine("Input Config value:");
            string value = Console.ReadLine();


            var result = await cgas_common.api_SendTransaction(prikey, cgas_common.sc_cgas, "setConfig", "(str)" + key, "(int)" + value);
            subPrintLine(result);

        }

        async Task test_getRefundTarget() {
            Console.WriteLine("Input txid:");
            string txid = Console.ReadLine();
            var result = await cgas_common.api_InvokeScript(cgas_common.sc_cgas, "getRefundTarget", "(hex256)" + txid);
            cgas_common.ResultItem item = result.value;

            Console.WriteLine("value:" + ThinNeo.Helper.GetAddressFromScriptHash(item.subItem[0].AsHash160()));

        }

        //退款操作
        async Task test_refund()
        {
            Console.WriteLine("Input refund tokens:");
            string refund = Console.ReadLine();

            string nep55_address = ThinNeo.Helper.GetAddressFromScriptHash(cgas_common.sc_cgas);
            Console.WriteLine("nep55_address=" + nep55_address);

            //获取地址的资产列表
            Dictionary<string, List<Utxo>> dir = await Helper.GetBalanceByAddress(Config.api, nep55_address);
            if (dir.ContainsKey(Config.id_GAS) == false)
            {
                Console.WriteLine("no gas");
                return;
            }
            List<Utxo> newlist = new List<Utxo>(dir[Config.id_GAS]);
            for (var i = newlist.Count - 1; i >= 0; i--)
            {
                string txid = newlist[i].txid.ToString();
                var ret = await cgas_common.api_InvokeScript(cgas_common.sc_cgas, "getRefundTarget", "(hex256)" + txid);
                cgas_common.ResultItem item = ret.value;

                if (newlist[i].n > 0)
                    continue;

                var value = item.subItem[0].AsString();
                if (value.Length > 0)//已经标记的UTXO，不能使用
                {
                    newlist.RemoveAt(i);
                }
            }


            ThinNeo.Transaction tran = null;
            {
                byte[] script = null;
                using (var sb = new ThinNeo.ScriptBuilder())
                {
                    var array = new MyJson.JsonNode_Array();
                    array.AddArrayValue("(bytes)" + ThinNeo.Helper.Bytes2HexString(scripthash));
                    sb.EmitParamJson(array);//参数倒序入
                    sb.EmitParamJson(new MyJson.JsonNode_ValueString("(str)refund"));//参数倒序入
                    sb.EmitAppCall(cgas_common.sc_cgas);//nep5脚本
                    script = sb.ToArray();
                }
                Console.WriteLine("contract address=" + nep55_address);//往合约地址转账

                //生成交易
                tran = Helper.makeTran(newlist, nep55_address, new ThinNeo.Hash256(Config.id_GAS), Decimal.Parse(refund));
                tran.type = ThinNeo.TransactionType.InvocationTransaction;
                var idata = new ThinNeo.InvokeTransData();
                tran.extdata = idata;
                idata.script = script;

                //附加鉴证
                tran.attributes = new ThinNeo.Attribute[1];
                tran.attributes[0] = new ThinNeo.Attribute();
                tran.attributes[0].usage = ThinNeo.TransactionAttributeUsage.Script;
                tran.attributes[0].data = scripthash;
            }

            //sign and broadcast
            {//做智能合约的签名
                byte[] iscript = null;
                using (var sb = new ThinNeo.ScriptBuilder())
                {
                    sb.EmitPushString("whatever");
                    sb.EmitPushNumber(250);
                    iscript = sb.ToArray();
                }
                tran.AddWitnessScript(n55contract, iscript);
            }
            {//做提款人的签名
                var signdata = ThinNeo.Helper.Sign(tran.GetMessage(), prikey);
                tran.AddWitness(signdata, pubkey, address);
            }
            var trandata = tran.GetRawData();
            var strtrandata = ThinNeo.Helper.Bytes2HexString(trandata);

            ThinNeo.Transaction testde = new ThinNeo.Transaction();
            testde.Deserialize(new System.IO.MemoryStream(trandata));

            byte[] postdata;
            var url = Helper.MakeRpcUrlPost(Config.api, "sendrawtransaction", out postdata, new MyJson.JsonNode_ValueString(strtrandata));

            string poststr = System.Text.Encoding.UTF8.GetString(postdata);
            //Console.WriteLine("-----post info begin----");
            //Console.WriteLine(poststr);
            //Console.WriteLine("-----post info end----");
            var result = await Helper.HttpPost(url, postdata);
            Console.WriteLine("得到的结果是：" + result);
            var json = MyJson.Parse(result).AsDict();
            if (json.ContainsKey("result"))
            {
                bool bSucc = false;
                if (json["result"].type == MyJson.jsontype.Value_Number)
                {
                    bSucc = json["result"].AsBool();
                    Console.WriteLine("cli=" + json["result"].ToString());
                }
                else
                {
                    var resultv = json["result"].AsList()[0].AsDict();
                    var txid = resultv["txid"].AsString();
                    bSucc = txid.Length > 0;
                    Console.WriteLine("txid=" + txid);
                }
                if (bSucc)
                {
                    lasttxid = tran.GetHash();
                    Nep55_1.lastNep5Tran = tran.GetHash();
                    Console.WriteLine("你可以从这个UTXO拿走NEO了 txid=" + lasttxid.ToString() + "[0]");
                }
                else
                {
                    lasttxid = null;
                }
            }

        }

        async Task test_getRefund()
        {
            //if (lasttxid == null)
            //{
            //    Console.WriteLine("你还没有正确执行Refund");
            //    return;
            //}
            Console.WriteLine("refund txid:");
            var lastTxid = Console.ReadLine();

            string nep55_address = ThinNeo.Helper.GetAddressFromScriptHash(cgas_common.sc_cgas);
            Console.WriteLine("address=" + nep55_address);

            //获取地址的资产列表
            Dictionary<string, List<Utxo>> dir = await Helper.GetBalanceByAddress(Config.api, nep55_address);
            if (dir.ContainsKey(Config.id_GAS) == false)
            {
                Console.WriteLine("no gas");
                return;
            }
            List<Utxo> newlist = new List<Utxo>();

            foreach (var utxo in dir[Config.id_GAS])
            {
                if (utxo.n == 0 && utxo.txid.ToString().Equals(lastTxid))
                    newlist.Add(utxo);
            }
            if (newlist.Count == 0)
            {
                Console.WriteLine("找不到要使用的UTXO");
                return;
            }
            else
            {
                lastTxid = newlist[0].txid.ToString();
            }


            {//检查是否是前面交易存储的

                var ret = await cgas_common.api_InvokeScript(cgas_common.sc_cgas, "getRefundTarget", "(hex256)" + lastTxid);
                cgas_common.ResultItem item = ret.value;

                var value = ThinNeo.Helper.GetAddressFromScriptHash(item.subItem[0].AsHash160());
                if (value.Length == 0)//未标记的UTXO，不能使用
                {
                    Console.WriteLine("这个utxo没有标记");
                    return;
                }
                if (value != this.address)
                {
                    Console.WriteLine("这个utxo不是标记给你用的");
                    return;
                }
            }

            ThinNeo.Transaction tran = Helper.makeTran(newlist, address, new ThinNeo.Hash256(Config.id_NEO), newlist[0].value);
            tran.type = ThinNeo.TransactionType.ContractTransaction;
            tran.version = 0;


            //sign and broadcast
            {//做智能合约的签名
                byte[] iscript = null;
                using (var sb = new ThinNeo.ScriptBuilder())
                {
                    sb.EmitPushNumber(0);
                    sb.EmitPushNumber(0);
                    iscript = sb.ToArray();
                }
                tran.AddWitnessScript(n55contract, iscript);
            }


            var trandata = tran.GetRawData();
            var strtrandata = ThinNeo.Helper.Bytes2HexString(trandata);

            ThinNeo.Transaction testde = new ThinNeo.Transaction();
            testde.Deserialize(new System.IO.MemoryStream(trandata));

            byte[] postdata;
            var url = Helper.MakeRpcUrlPost(Config.api, "sendrawtransaction", out postdata, new MyJson.JsonNode_ValueString(strtrandata));

            string poststr = System.Text.Encoding.UTF8.GetString(postdata);
            //Console.WriteLine("-----post info begin----");
            //Console.WriteLine(poststr);
            //Console.WriteLine("-----post info end----");

            var result = await Helper.HttpPost(url, postdata);
            Console.WriteLine("得到的结果是：" + result);
            var json = MyJson.Parse(result).AsDict();
            if (json.ContainsKey("result"))
            {
                bool bSucc = false;
                if (json["result"].type == MyJson.jsontype.Value_Number)
                {
                    bSucc = json["result"].AsBool();
                    Console.WriteLine("cli=" + json["result"].ToString());
                }
                else
                {
                    var resultv = json["result"].AsList()[0].AsDict();
                    var txid = resultv["txid"].AsString();
                    bSucc = txid.Length > 0;
                    Console.WriteLine("txid=" + txid);
                }
                if (bSucc)
                {
                    Nep55_1.lastNep5Tran = tran.GetHash();
                    Console.WriteLine("besucc txid=" + tran.GetHash().ToString());
                }
            }

        }

        async Task test_claimStep1()
        {
            //neo总量
            //var result=  cgas_common.api_GetBalance(cgas_common.sc_cgas,this.address);

            Dictionary<string, List<Utxo>> dir = await Helper.GetBalanceByAddress(Config.api, this.address);
            List<Utxo> neolist = dir[Config.id_NEO];
            decimal sumneo = 0;
            for (var i = 0; i < neolist.Count; i++)
            {
                sumneo = sumneo + neolist[i].value;
            }
            Console.WriteLine("NEO:" + sumneo);

            ThinNeo.Transaction tran = Helper.makeTran(dir[Config.id_NEO], this.address, new ThinNeo.Hash256(Config.id_NEO), sumneo);
            tran.type = ThinNeo.TransactionType.ContractTransaction;
            tran.version = 0;

            byte[] msg = tran.GetMessage();
            byte[] signdata = ThinNeo.Helper.Sign(msg, prikey);
            tran.AddWitness(signdata, pubkey, address);

            var trandata = tran.GetRawData();
            var strtrandata = ThinNeo.Helper.Bytes2HexString(trandata);

            //ThinNeo.Transaction testde = new ThinNeo.Transaction();
            //testde.Deserialize(new System.IO.MemoryStream(trandata));

            byte[] postdata;
            var url = Helper.MakeRpcUrlPost(Config.api, "sendrawtransaction", out postdata, new MyJson.JsonNode_ValueString(strtrandata));

            string poststr = System.Text.Encoding.UTF8.GetString(postdata);
            Console.WriteLine("-----post info begin----");
            Console.WriteLine(poststr);
            Console.WriteLine("-----post info end----");

            var result = await Helper.HttpPost(url, postdata);
            Console.WriteLine("得到的结果是：" + result);
        }


        async Task test_claimStep2()
        {
            byte[] postdata;

            var url = Helper.MakeRpcUrlPost(Config.api, "getclaimtxhex", out postdata, new MyJson.JsonNode_ValueString(this.address));
            var result = await Helper.HttpPost(url, postdata);
            Console.WriteLine("得到的结果是：" + result);

            var json = MyJson.Parse(result).AsDict();
            var claimtxhex = "";
            if (json.ContainsKey("result"))
            {
                claimtxhex = json["result"].AsList()[0].AsDict()["claimtxhex"].AsString();

                byte[] buf = ThinNeo.Helper.HexString2Bytes(claimtxhex);

                ThinNeo.Transaction tran = new ThinNeo.Transaction();
                tran.Deserialize(new System.IO.MemoryStream(buf));

                byte[] msg = tran.GetMessage();
                byte[] signdata = ThinNeo.Helper.Sign(msg, this.prikey);

                tran.AddWitness(signdata, pubkey, this.address);

                var trandata = tran.GetRawData();
                var strtrandata = ThinNeo.Helper.Bytes2HexString(trandata);

                byte[] postdata2;
                url = Helper.MakeRpcUrlPost(Config.api, "sendrawtransaction", out postdata2, new MyJson.JsonNode_ValueString(strtrandata));

                var result2 = await Helper.HttpPost(url, postdata2);
                Console.WriteLine("得到的结果是：" + result2);
            }
        }


        async Task test_claimStep3()
        {
            byte[] postdata;

            var url = Helper.MakeRpcUrlPost(Config.api, "getclaimgas", out postdata, new MyJson.JsonNode_ValueString(this.address));
            var result = await Helper.HttpPost(url, postdata);
            Console.WriteLine("得到的结果是：" + result);

            var json = MyJson.Parse(result).AsDict();

            if (json.ContainsKey("result"))
            {
                //gas总量
                var gas = json["result"].AsList()[0].AsDict()["gas"].AsString();
                Console.WriteLine("gas:" + gas);

                var claims = json["result"].AsList()[0].AsDict()["claims"].AsList();
                Console.WriteLine("claims:" + claims);

                //var assetIDStr = "0x602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7"; //选择GAS支付合约调用费用
                var assetID = HexString2Bytes(Config.id_GAS.Replace("0x", "")).Reverse().ToArray();

                //构建交易体
                ThinNeo.Transaction claimTran = new ThinNeo.Transaction
                {
                    type = ThinNeo.TransactionType.ClaimTransaction,//领取Gas合约
                    attributes = new ThinNeo.Attribute[0],
                    inputs = new ThinNeo.TransactionInput[0],
                    outputs = new ThinNeo.TransactionOutput[1],
                    extdata = new ThinNeo.ClaimTransData()
                };

                claimTran.outputs[0] = new ThinNeo.TransactionOutput
                {
                    assetId = assetID,
                    toAddress = ThinNeo.Helper.GetPublicKeyHashFromAddress(this.address),
                    value = Decimal.Parse(gas)
                };

                List<ThinNeo.TransactionInput> claimVins = new List<ThinNeo.TransactionInput>();
                foreach (MyJson.IJsonNode j in (MyJson.JsonNode_Array)claims)
                {
                    claimVins.Add(new ThinNeo.TransactionInput
                    {
                        hash = ThinNeo.Debug.DebugTool.HexString2Bytes((j.AsDict()["txid"].ToString()).Replace("0x", "")).Reverse().ToArray(),
                        index = ushort.Parse(j.AsDict()["n"].ToString())
                    });
                }

                (claimTran.extdata as ThinNeo.ClaimTransData).claims = claimVins.ToArray();


                byte[] msg = claimTran.GetMessage();
                byte[] signdata = ThinNeo.Helper.Sign(msg, this.prikey);

                claimTran.AddWitness(signdata, pubkey, this.address);

                var trandata = claimTran.GetRawData();
                var strtrandata = ThinNeo.Helper.Bytes2HexString(trandata);

                byte[] postdata2;
                url = Helper.MakeRpcUrlPost(Config.api, "sendrawtransaction", out postdata2, new MyJson.JsonNode_ValueString(strtrandata));

                var result2 = await Helper.HttpPost(url, postdata2);
                Console.WriteLine("得到的结果是：" + result2);
            }
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
