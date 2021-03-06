﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ThinNeo;
using System.IO;
using System.Linq;
using smartContractDemo.tests;

namespace smartContractDemo
{
    class sneoTest : ITest
    {
        public string Name => "SNEO 合约测试";

        public string ID => "sneo";
        byte[] prikey;
        byte[] prikey_admin;
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

        public sneoTest()
        {
            this.initMenu();
        }

        private void initMenu()
        {
            infos = new Dictionary<string, testAction>();
            infos["assets"] = test_assets;
            infos["totalSupply"] = test_totalSupply;
            infos["name"] = test_name;
            infos["symbol"] = test_symbol;
            infos["decimals"] = test_decimals;
            infos["balanceOf"] = test_BalanceOf;
            infos["transfer"] = test_Transfer;
            infos["getTXInfo"] = test_getTXInfo;
            infos["mintTokensNEO"] = test_mintTokensNEO;
            //infos["mintTokensGAS"] = test_mintTokensGAS;
            infos["refund"] = test_refund;
            infos["setClaimAccount"] = test_setClaimAccount;
            infos["setAdminAccount"] = test_setAdminAccount;
            infos["getAccount"] = test_getAccount2;
            infos["getRefund"] = test_getRefund;
            infos["getRefundTarget"] = test_getRefundTarget;

            //infos["claimStep1"] = test_claimStep1;
            infos["claimSelf"] = test_claimStep2;
            infos["claimConGas"] = test_claimStep3;
            infos["claimSimple"] = test_claimSimple;
            infos["claimGas"] = test_claimStep4;

            this.submenu = new List<string>(infos.Keys).ToArray();
        }

        public delegate void Method(ThinNeo.ScriptBuilder sb);//第一步：定义委托类型

        public async Task Demo()
        {
            //得到合约代码
            var urlgetscript = Helper.MakeRpcUrl(Config.api, "getcontractstate", new MyJson.JsonNode_ValueString(Config.sc_sneo));
            var resultgetscript = await Helper.HttpGet(urlgetscript);
            var _json = MyJson.Parse(resultgetscript).AsDict();
            var _resultv = _json["result"].AsList()[0].AsDict();

            //Console.WriteLine(_resultv["script"].AsString());

            n55contract = ThinNeo.Helper.HexString2Bytes(_resultv["script"].AsString());

            //Console.WriteLine(n55contract.Length);


            showMenu();

            prikey_admin = ThinNeo.Helper.GetPrivateKeyFromWIF(Config.testwif_admin);
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

        //查询全局资产余额
        async Task test_assets()
        {
            Dictionary<string, List<Utxo>> dir = await Helper.GetBalanceByAddress(Config.api, this.address);

            if (dir.ContainsKey(Config.id_GAS))
            {
                List<Utxo> gaslist = dir[Config.id_GAS];
                decimal sumgas = 0;
                for (var i = 0; i < gaslist.Count; i++)
                {
                    sumgas = sumgas + gaslist[i].value;
                }
                Console.WriteLine("GAS:" + sumgas);
            }

            if (dir.ContainsKey(Config.id_NEO))
            {
                List<Utxo> neolist = dir[Config.id_NEO];
                decimal sumneo = 0;
                for (var i = 0; i < neolist.Count; i++)
                {
                    sumneo = sumneo + neolist[i].value;
                }
                Console.WriteLine("NEO:" + sumneo);
            }
        }

        //查询总量
        async Task test_totalSupply()
        {
            var result = await sneo_common.api_InvokeScript(Config.sneo, "totalSupply", null);
            sneo_common.ResultItem item = result.value;

            Console.WriteLine(Helper.changeDecimals(item.subItem[0].AsInteger(), 8));
        }

        //查询名字
        async Task test_name()
        {
            var result = await sneo_common.api_InvokeScript(Config.sneo, "name", null);
            sneo_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsString());
        }

        //查询标志
        async Task test_symbol()
        {
            var result = await sneo_common.api_InvokeScript(Config.sneo, "symbol", null);
            sneo_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsString());
        }

        //查询最小单位
        async Task test_decimals()
        {
            var result = await sneo_common.api_InvokeScript(Config.sneo, "decimals", null);
            sneo_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //查询余额
        async Task test_BalanceOf()
        {
            Console.WriteLine("Input target address:");
            string addr = Console.ReadLine();
            if (addr.Length == 0)
                addr = this.address;

            var result = await sneo_common.api_InvokeScript(Config.sneo, "balanceOf", "(addr)" + addr);
            sneo_common.ResultItem item = result.value;

            Console.WriteLine(Helper.changeDecimals(item.subItem[0].AsInteger(), 8));
        }

        //转账
        async Task test_Transfer()
        {
            Console.WriteLine("Input target address:");
            string addressto = Console.ReadLine();

            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await sneo_common.api_SendbatchTransaction(prikey, Config.sneo, "transfer",
              "(addr)" + this.address,
              "(addr)" + addressto,
              "(int)" + amount);
            subPrintLine(result);
        }

        async Task test_setClaimAccount()
        {
            Console.WriteLine("Input  address:");
            string addr = Console.ReadLine();

            var result = await sneo_common.api_SendbatchTransaction(prikey_admin, Config.sneo, "setAccount",
                "(str)claim_account",
              "(addr)" + addr);
            subPrintLine(result);
        }

        async Task test_setAdminAccount()
        {
            Console.WriteLine("Input  address:");
            string addr = Console.ReadLine();

            var result = await sneo_common.api_SendbatchTransaction(prikey_admin, Config.sneo, "setAccount",
                "(str)admin_account",
              "(addr)" + addr);
            subPrintLine(result);
        }

        async Task test_getAccount()
        {

            var result = await sneo_common.api_InvokeScript(Config.sneo, "getAccount",
              "(str)admin_account");

            sneo_common.ResultItem item = result.value;
            Console.WriteLine("admin:"+ThinNeo.Helper.GetAddressFromScriptHash(item.subItem[0].AsHash160()));

             result = await sneo_common.api_InvokeScript(Config.sneo, "getAccount",
              "(str)claim_account");

            item = result.value;
            Console.WriteLine("claim:"+ThinNeo.Helper.GetAddressFromScriptHash(item.subItem[0].AsHash160()));
        }

        async Task test_getAccount2()
        {

            string key1 = "6163636f756e740061646d696e5f6163636f756e74";
            var url = Helper.MakeRpcUrl(Config.api, "getstorage", new MyJson.JsonNode_ValueString(Config.sc_sneo), new MyJson.JsonNode_ValueString(key1));
            string result = await Helper.HttpGet(url);
            Console.WriteLine("admin_account：" + result);

            string key2 = "6163636f756e7400636c61696d5f6163636f756e74";
            url = Helper.MakeRpcUrl(Config.api, "getstorage", new MyJson.JsonNode_ValueString(Config.sc_sneo), new MyJson.JsonNode_ValueString(key2));
            result = await Helper.HttpGet(url);
            Console.WriteLine("claim_account：" + result);

        }


        async Task test_getTXInfo()
        {
            Console.WriteLine("Input txid:");
            string txid = Console.ReadLine();
            var result = await sneo_common.api_InvokeScript(Config.sneo, "getTxInfo", "(hex256)" + txid);
            sneo_common.ResultItem item = result.value;
            sneo_common.ResultItem[] items = item.subItem[0].subItem;

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
                    sb.EmitAppCall(Config.sneo);//nep5脚本
                    script = sb.ToArray();
                }
                var targetaddr = ThinNeo.Helper.GetAddressFromScriptHash(Config.sneo);
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
                    sb.EmitAppCall(Config.sneo);//nep5脚本
                    script = sb.ToArray();
                }
                var targetaddr = ThinNeo.Helper.GetAddressFromScriptHash(Config.sneo);
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


            var result = await sneo_common.api_SendTransaction(prikey, Config.sneo, "setConfig", "(str)" + key, "(int)" + value);
            subPrintLine(result);

        }

        async Task test_getRefundTarget()
        {
            Console.WriteLine("Input txid:");
            string txid = Console.ReadLine();
            var result = await sneo_common.api_InvokeScript(Config.sneo, "getRefundTarget", "(hex256)" + txid);
            sneo_common.ResultItem item = result.value;

            Console.WriteLine("value:" + ThinNeo.Helper.GetAddressFromScriptHash(item.subItem[0].AsHash160()));

        }

        //退款操作
        async Task test_refund()
        {
            Console.WriteLine("Input refund tokens:");
            string refund = Console.ReadLine();

            string nep55_address = ThinNeo.Helper.GetAddressFromScriptHash(Config.sneo);
            Console.WriteLine("nep55_address=" + nep55_address);

            //获取地址的资产列表
            Dictionary<string, List<Utxo>> dir = await Helper.GetBalanceByAddress(Config.api, nep55_address);
            if (dir.ContainsKey(Config.id_NEO) == false)
            {
                Console.WriteLine("no neo");
                return;
            }
            List<Utxo> newlist = new List<Utxo>(dir[Config.id_NEO]);
            for (var i = newlist.Count - 1; i >= 0; i--)
            {
                string txid = newlist[i].txid.ToString();
                var ret = await sneo_common.api_InvokeScript(Config.sneo, "getRefundTarget", "(hex256)" + txid);
                sneo_common.ResultItem item = ret.value;

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
                    sb.EmitAppCall(Config.sneo);//nep5脚本
                    script = sb.ToArray();
                }
                Console.WriteLine("contract address=" + nep55_address);//往合约地址转账

                //生成交易
                tran = Helper.makeTran(newlist, nep55_address, new ThinNeo.Hash256(Config.id_NEO), Decimal.Parse(refund));
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

            string nep55_address = ThinNeo.Helper.GetAddressFromScriptHash(Config.sneo);
            Console.WriteLine("address=" + nep55_address);

            //获取地址的资产列表
            Dictionary<string, List<Utxo>> dir = await Helper.GetBalanceByAddress(Config.api, nep55_address);
            if (dir.ContainsKey(Config.id_NEO) == false)
            {
                Console.WriteLine("no neo");
                return;
            }
            List<Utxo> newlist = new List<Utxo>();

            foreach (var utxo in dir[Config.id_NEO])
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

                var ret = await sneo_common.api_InvokeScript(Config.sneo, "getRefundTarget", "(hex256)" + lastTxid);
                sneo_common.ResultItem item = ret.value;

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
            //var result=  sneo_common.api_GetBalance(Config.sneo,this.address);

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
            var addr = ThinNeo.Helper.GetAddressFromScriptHash(Config.sneo);

            byte[] postdata;

            var url = Helper.MakeRpcUrlPost(Config.api, "getclaimgas", out postdata, new MyJson.JsonNode_ValueString(addr));
            var result = await Helper.HttpPost(url, postdata);
            //Console.WriteLine("得到的结果是：" + result);

            var json = MyJson.Parse(result).AsDict();

            if (json.ContainsKey("result"))
            {
                //gas总量
                var gas = json["result"].AsList()[0].AsDict()["gas"]+"";
                //var gas = 0.00004;
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
                    value = Decimal.Parse("0.00020153")
                };
                Console.WriteLine("claim addr:"+this.address);
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

                //做智能合约的签名
                byte[] iscript = null;
                using (var sb = new ThinNeo.ScriptBuilder())
                {
                    sb.EmitPushString("whatever");
                    sb.EmitPushNumber(250);
                    iscript = sb.ToArray();
                }

                claimTran.AddWitnessScript(n55contract, iscript);

                var trandata = claimTran.GetRawData();
                var strtrandata = ThinNeo.Helper.Bytes2HexString(trandata);

                byte[] postdata2;
                url = Helper.MakeRpcUrlPost(Config.api, "sendrawtransaction", out postdata2, new MyJson.JsonNode_ValueString(strtrandata));

                var result2 = await Helper.HttpPost(url, postdata2);
                Console.WriteLine("得到的结果是：" + result2);
            }
        }

        async Task test_claimSimple()
        {
            var addr = ThinNeo.Helper.GetAddressFromScriptHash(Config.sneo);

            byte[] postdata;

            var url = Helper.MakeRpcUrlPost(Config.api, "getclaimgas", out postdata, new MyJson.JsonNode_ValueString(addr));
            var result = await Helper.HttpPost(url, postdata);
            //Console.WriteLine("得到的结果是：" + result);

            var json = MyJson.Parse(result).AsDict();

            if (json.ContainsKey("result"))
            {
                //gas总量
                var gas = json["result"].AsList()[0].AsDict()["gas"] + "";
                //var gas = 0.00004;
                //Console.WriteLine("gas:" + gas);

                var claims = json["result"].AsList()[0].AsDict()["claims"].AsList();
                //Console.WriteLine("claims:" + claims);

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
                    value = Decimal.Parse("0.00006552")
                };
                Console.WriteLine("claim addr:" + this.address);
                List<ThinNeo.TransactionInput> claimVins = new List<ThinNeo.TransactionInput>();

                MyJson.JsonNode_Array array = (MyJson.JsonNode_Array)claims;
                MyJson.IJsonNode node = array[0];

                Console.WriteLine("claims:" + node);
                claimVins.Add(new ThinNeo.TransactionInput
                {
                    hash = ThinNeo.Debug.DebugTool.HexString2Bytes((node.AsDict()["txid"].ToString()).Replace("0x", "")).Reverse().ToArray(),
                    index = ushort.Parse(node.AsDict()["n"].ToString())
                });

                (claimTran.extdata as ThinNeo.ClaimTransData).claims = claimVins.ToArray();

                //做智能合约的签名
                byte[] iscript = null;
                using (var sb = new ThinNeo.ScriptBuilder())
                {
                    sb.EmitPushString("whatever");
                    sb.EmitPushNumber(250);
                    iscript = sb.ToArray();
                }

                claimTran.AddWitnessScript(n55contract, iscript);

                var trandata = claimTran.GetRawData();
                var strtrandata = ThinNeo.Helper.Bytes2HexString(trandata);

                byte[] postdata2;
                url = Helper.MakeRpcUrlPost(Config.api, "sendrawtransaction", out postdata2, new MyJson.JsonNode_ValueString(strtrandata));

                var result2 = await Helper.HttpPost(url, postdata2);
                Console.WriteLine("得到的结果是：" + result2);
            }
        }

        async Task test_claimStep4()
        {
            byte[] postdata;

            var url = Helper.MakeRpcUrlPost(Config.api, "getclaimgas", out postdata, new MyJson.JsonNode_ValueString(this.address));
            var result = await Helper.HttpPost(url, postdata);
            Console.WriteLine("得到的结果是：" + result);

            var json = MyJson.Parse(result).AsDict();

            if (json.ContainsKey("result"))
            {
                //gas总量
                var gas = json["result"].AsList()[0].AsDict()["gas"].AsDouble();
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
                    value = Decimal.Parse(gas + "")
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
