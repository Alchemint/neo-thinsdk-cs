﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Numerics;
using ThinNeo;
using smartContractDemo.tests;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using static MyJson;
using System.Linq;

namespace smartContractDemo
{
    class sdsTest : ITest
    {
        public string Name => "SDS 合约测试";

        public string ID => "sds";
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

        public sdsTest()
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
            infos["queryBalanceOf"] = test_QueryBalanceOf;
            infos["batchTransfer"] = test_batchTransfer;
            infos["batchTransferFor2"] = test_batchTransfer2;
            infos["transfer"] = test_Transfer;
            infos["transferApp"] = test_TransferApp;
            infos["transferNEO"] = test_transferNEO;
            infos["batchTransferNEO"] = test_batchTransferNEO;
            infos["getTXInfo"] = test_getTXInfo;
            infos["getstorage"] = test_getstorage;
            infos["toByte"] = test_toByte;
            infos["cal"] = test_cal;
            infos["queryAddress"] = test_queryAllAddress;
            infos["queryAllTransfer"] = test_queryAllTransfer;
            infos["batchCal"] = test_batchCal;
            infos["batchCal2"] = test_batchCal2;
            infos["test"] = test_main;
            this.submenu = new List<string>(infos.Keys).ToArray();
        }

        public delegate void Method(ThinNeo.ScriptBuilder sb);//第一步：定义委托类型

        public async Task Demo()
        {
            //得到合约代码
            //var urlgetscript = Helper.MakeRpcUrl(sds_common.api, "getcontractstate", new MyJson.JsonNode_ValueString(Config.sds));
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

            var url = Helper.MakeRpcUrl(Config.api, "getstorage", new MyJson.JsonNode_ValueString(Config.sc_sds), new MyJson.JsonNode_ValueString(revkey));

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
            var result = await sds_common.api_InvokeScript(Config.sds, "totalSupply", null);
            sds_common.ResultItem item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //查询名字
        async Task test_name()
        {
            BigInteger sum = 9627308228749 + 2363900000000 + 99956313965785032 + 1175000000000;
            Console.WriteLine(sum);

            var result = await sds_common.api_InvokeScript(Config.sds, "name", null);
            sds_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsString());
        }

        //初始操作
        async Task test_init()
        {
            var result = await sds_common.api_SendTransaction(prikey, Config.sds, "init", null);
            subPrintLine(result);
        }

        //查询标志
        async Task test_symbol()
        {
            var result = await sds_common.api_InvokeScript(Config.sds, "symbol", null);
            sds_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsString());
        }
        //查询最小单位
        async Task test_decimals()
        {
            var result = await sds_common.api_InvokeScript(Config.sds, "decimals", null);
            sds_common.ResultItem item = result.value;

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

            var result = await sds_common.api_InvokeScript(Config.sds, "balanceOf", "(addr)" + addr);

            sds_common.ResultItem item = result.value;

            Console.WriteLine(item.subItem[0].AsInteger());
        }

        async Task test_cal()
        {
            string path = "D:\\balance.txt";

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);

            string str = "";
            double mount = 0.0;
            while (str != null)
            {
                str = sr.ReadLine();
                if (string.IsNullOrEmpty(str)) break;

                Console.WriteLine("mount:" + str);
                mount = mount + double.Parse(str);

            }
            sr.Close();
            Console.WriteLine("total:" + mount);
        }

        async Task test_QueryBalanceOf()
        {

            string path = "D:\\addr2.txt";

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);

            string str = "";
            BigInteger sum = 0;
            while (str != null)
            {
                str = sr.ReadLine();
                Console.WriteLine("address:" + str);
                var result = await sds_common.api_InvokeScript(Config.sds, "balanceOf", "(addr)" + str);

                sds_common.ResultItem item = result.value;

                BigInteger mount = item.subItem[0].AsInteger();

                // Console.WriteLine("mount:" + mount);

                sum = sum + mount;
                Thread.Sleep(50);
                Console.WriteLine("total:" + sum);
            }
            sr.Close();
            Console.WriteLine("total:" + sum);
        }

        //转账
        async Task test_Transfer()
        {
            Console.WriteLine("Input target address:");
            string addressto = Console.ReadLine();
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await sds_common.api_SendTransaction(prikey, Config.sds, "transfer",
               "(addr)" + address,
              "(addr)" + addressto,
              "(int)" + amount
              );
            subPrintLine(result);
        }

        //批量转账
        async Task test_batchTransfer()
        {
            string path = "D:\\address\\0921.csv";

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);

            string str = "";
            while (str != null)
            {
                str = sr.ReadLine();
                if (!string.IsNullOrEmpty(str))
                {
                    string[] xu = new String[2];
                    xu = str.Split(',');
                    //转账地址
                    string addressto = xu[0];
                    //转账金额
                    string m = xu[1];

                    decimal dmount = decimal.Parse(m);
                    decimal mount = dmount * 100000000;
                    string mstr = Math.Round(mount, 0).ToString();
                    string newPath = @"D:\address\balances0921_result.txt";
                    string str2 = addressto + "," + m + "\r\n";
                    File.AppendAllText(newPath, str2);
                    if (m != "0")
                    {
                        //var re = await sds_common.api_InvokeScriptByRPC(Config.sds, "balanceOf",
                        //        "(addr)" + addressto);
                        //sds_common.ResultItem item = re.value;
                        //BigInteger mount = item.subItem[0].AsInteger();

                        //if (mount == 0)
                        //{
                        var result = await sds_common.api_SendbatchTransfer(prikey, Config.sds, "transfer",
                          "(addr)" + this.address,
                          "(addr)" + addressto,
                          "(int)" + mstr
                          );
                        Console.WriteLine("address:" + addressto + " mount:" + m);
                        subPrintLine(result);
                        File.AppendAllText(newPath, result + "\r\n");
                        Thread.Sleep(10);
                        //}
                    }
                }
            }
            sr.Close();

        }


        //批量转账
        async Task test_batchTransfer2()
        {
            DateTime dt = DateTime.Now;
            Console.WriteLine("Start time:" + dt);
            byte[] postdata;

            string api = "http://api.alchemint.io/api/privatenet";
            //查询交易，总数可能很多
            var url = Helper.MakeRpcUrlPost(api, "getaddrs", out postdata,
                new JsonNode_ValueNumber(1000),
                new JsonNode_ValueNumber(1));
            var result = await Helper.HttpPost(url, postdata);

            List<string> list = new List<string>();
            MyJson.JsonNode_Object json = MyJson.Parse(result) as MyJson.JsonNode_Object;
            JsonNode_Array arrs = json["result"].AsList();

            foreach (JsonNode_Object ob in arrs)
            {
                string to = ob["addr"].AsString();
                Console.WriteLine("to:"+to);
                result = await sds_common.api_SendbatchTransfer(prikey, Config.sds, "transfer",
                     "(addr)" + this.address,
                     "(addr)"+to,
                     "(int)" + 10000000000
                     );
                subPrintLine(result);
            }
            DateTime end = DateTime.Now;
            Console.WriteLine("End time:" + end);
            //等待时间

        }

        //转账
        async Task test_TransferApp()
        {
            Console.WriteLine("Input target address:");
            string addressto = Console.ReadLine();
            Console.WriteLine("Input amount:");
            string amount = Console.ReadLine();

            var result = await sds_common.api_SendTransaction(prikey, Config.sds, "transfer_contract",
               "(addr)" + address,
              "(addr)" + addressto,
              "(int)" + amount
              );
            subPrintLine(result);
        }

        async Task test_transferNEO()
        {
            Console.WriteLine("transfer to:");
            var to = Console.ReadLine();

            Console.WriteLine("send mount:");
            var mount = Console.ReadLine();

            //获取地址的资产列表
            Dictionary<string, List<Utxo>> dir = await Helper.GetBalanceByAddress(Config.api, this.address);
            if (dir.ContainsKey(Config.id_NEO) == false)
            {
                Console.WriteLine("no neo");
                return;
            }
            List<Utxo> newlist = dir[Config.id_NEO];

            ThinNeo.Transaction tran = Helper.makeTran(newlist, to, new ThinNeo.Hash256(Config.id_NEO), decimal.Parse(mount));

            var signdata = ThinNeo.Helper.Sign(tran.GetMessage(), prikey);
            tran.AddWitness(signdata, pubkey, address);

            var trandata = tran.GetRawData();
            var strtrandata = ThinNeo.Helper.Bytes2HexString(trandata);

            byte[] postdata;
            var url = Helper.MakeRpcUrlPost(Config.api, "sendrawtransaction", out postdata, new MyJson.JsonNode_ValueString(strtrandata));

            string poststr = System.Text.Encoding.UTF8.GetString(postdata);
         
            var result = await Helper.HttpPost(url, postdata);
            Console.WriteLine("得到的结果是：" + result);

        }

        async Task test_batchTransferNEO()
        {
            DateTime dt = DateTime.Now;
            Console.WriteLine("Start time:" + dt);
            byte[] postdata;

            string api = "http://api.alchemint.io/api/privatenet";
            //查询交易，总数可能很多
            var url = Helper.MakeRpcUrlPost(api, "getaddrs", out postdata,
                new JsonNode_ValueNumber(1000),
                new JsonNode_ValueNumber(1));
            var result = await Helper.HttpPost(url, postdata);

            List<string> list = new List<string>();
            MyJson.JsonNode_Object json = MyJson.Parse(result) as MyJson.JsonNode_Object;
            JsonNode_Array arrs = json["result"].AsList();

            foreach (JsonNode_Object ob in arrs)
            {
                string to = ob["addr"].AsString();
                Console.WriteLine("to:" + to);
                //获取地址的资产列表
                Dictionary<string, List<Utxo>> dir = await Helper.GetBalanceByAddress(Config.api, this.address);
                if (dir.ContainsKey(Config.id_NEO) == false)
                {
                    Console.WriteLine("no neo");
                    return;
                }
                List<Utxo> newlist = dir[Config.id_NEO];

                ThinNeo.Transaction tran = Helper.makeTran(newlist, to, new ThinNeo.Hash256(Config.id_NEO), 100);

                var signdata = ThinNeo.Helper.Sign(tran.GetMessage(), prikey);
                tran.AddWitness(signdata, pubkey, address);

                var trandata = tran.GetRawData();
                var strtrandata = ThinNeo.Helper.Bytes2HexString(trandata);

                 url = Helper.MakeRpcUrlPost(Config.api, "sendrawtransaction", out postdata, new MyJson.JsonNode_ValueString(strtrandata));

                string poststr = System.Text.Encoding.UTF8.GetString(postdata);

                 result = await Helper.HttpPost(url, postdata);
                Console.WriteLine("得到的结果是：" + result);
                Thread.Sleep(40000);
            }
            DateTime end = DateTime.Now;
            Console.WriteLine("End time:" + end);
            //等待时间
        }


        //查询交易信息
        async Task test_getTXInfo()
        {
            Console.WriteLine("Input txid:");
            string txid = Console.ReadLine();
            var result = await sds_common.api_InvokeScript(Config.sds, "getTXInfo", "(hex256)" + txid);
            sds_common.ResultItem item = result.value;
            sds_common.ResultItem[] items = item.subItem[0].subItem;

            //查询交易详细信息
            Console.WriteLine("from:" + ThinNeo.Helper.GetAddressFromScriptHash(items[0].AsHash160()));
            Console.WriteLine("to:" + ThinNeo.Helper.GetAddressFromScriptHash(items[1].AsHash160()));
            Console.WriteLine("value:" + items[2].AsInteger());
        }

        //查询所有地址
        async Task test_queryAllAddress()
        {
            DateTime dt = DateTime.Now;
            Console.WriteLine("Start time:" + dt);
            byte[] postdata;
            //查询交易，总数可能很多
            var url = Helper.MakeRpcUrlPost(Config.api, "getnep5transfersbyasset", out postdata,
                new JsonNode_ValueString(Config.sc_sds),
                new JsonNode_ValueNumber(10000),
                new JsonNode_ValueNumber(1));
            var result = await Helper.HttpPost(url, postdata);
            System.IO.File.WriteAllText(@"D:\address\addssssss0907.json", result, Encoding.UTF8);

            List<string> list = new List<string>();
            MyJson.JsonNode_Object json = MyJson.Parse(result) as MyJson.JsonNode_Object;
            JsonNode_Array arrs = json["result"].AsList();

            foreach (JsonNode_Object ob in arrs)
            {
                string from = ob["from"].AsString();
                string to = ob["to"].AsString();
                if (!string.IsNullOrEmpty(from))
                {
                    list.Add(from);
                    if (from == to)
                    {
                        Console.WriteLine("from:" + from + "/to:" + to);
                    }
                }
                if (!string.IsNullOrEmpty(to))
                {
                    list.Add(to);
                }
            }
            BigInteger sum = 0;
            List<string> adds = list.Distinct().ToList();

            Console.WriteLine("total address:" + adds.Count);
            string[] balances = new string[] { };
            foreach (string s in adds)
            {
                //int index = adds.IndexOf(s);
                //Console.WriteLine("address:" + s);
                //调用RPC
                //var re = await sds_common.api_InvokeScriptByRPC(Config.sds, "balanceOf",
                //"(addr)" + s);
                //调用API
                var re = await sds_common.api_InvokeScript(Config.sds, "balanceOf",
                "(addr)" + s);

                sds_common.ResultItem item = re.value;

                BigInteger mount = item.subItem[0].AsInteger();
                sum = sum + mount;
                if (mount > 0)
                {
                    //排除掉所有switcheo地址
                    //if (s != "AKJQMHma9MA8KK5M8iQg8ASeg3KZLsjwvB")
                    //{
                    string str = s + "," + mount + "\r\n";
                    string newPath = @"D:\address\balances0907.txt";
                    File.AppendAllText(newPath, str);
                    //}
                }
            }

            Console.WriteLine("sum:" + sum);
            DateTime end = DateTime.Now;
            Console.WriteLine("End time:" + end);
        }

        //查询所有地址
        async Task test_queryAllTransfer()
        {
            DateTime dt = DateTime.Now;
            Console.WriteLine("Start time:" + dt);
            byte[] postdata;
            //查询交易，总数可能很多
            var url = Helper.MakeRpcUrlPost(Config.api, "getnep5transfersbyasset", out postdata,
                new JsonNode_ValueString(Config.sc_sds),
                new JsonNode_ValueNumber(10000),
                new JsonNode_ValueNumber(1));
            var result = await Helper.HttpPost(url, postdata);
            //System.IO.File.WriteAllText(@"D:\address\addssssss0907.json", result, Encoding.UTF8);

            List<string> list = new List<string>();
            MyJson.JsonNode_Object json = MyJson.Parse(result) as MyJson.JsonNode_Object;
            JsonNode_Array arrs = json["result"].AsList();

            foreach (JsonNode_Object ob in arrs)
            {
                string from = ob["from"].AsString();
                string to = ob["to"].AsString();
                string value = ob["value"].AsString();
                string txid = ob["txid"].AsString();
                int index = ob["blockindex"].AsInt();

                 url = Helper.MakeRpcUrlPost(Config.api, "getblocktime", out postdata,
               new JsonNode_ValueNumber(index));
                 result = await Helper.HttpPost(url, postdata);

                json = MyJson.Parse(result) as MyJson.JsonNode_Object;

                 arrs = json["result"].AsList();
                 string time = arrs[0].AsDict()["time"].AsString();

                //long unixTimeStamp = 1478162177;
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
                DateTime dt2 = startTime.AddSeconds(double.Parse(time));
                string dt2str = dt2.ToString("yyyy/MM/dd HH:mm:ss:ffff");
                System.Console.WriteLine();
                //json["result"].AsDict()["time"];
                //JsonNode_Array arrs = json["result"].AsList();


                string str = txid+","+from+ "," + to +","+value+","+ dt2str + "\r\n";
                string newPath = @"D:\address\transfers0907.csv";
                File.AppendAllText(newPath, str);
            }
            DateTime end = DateTime.Now;
            Console.WriteLine("End time:" + end);
        }

        //批量计算
        async Task test_batchCal()
        {
            string path = "D:\\address\\balances0801.txt";

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);

            BigInteger sum = 0;
            string str = "";
            while (str != null)
            {
                str = sr.ReadLine();
                if (!string.IsNullOrEmpty(str))
                {
                    string[] xu = new String[2];
                    xu = str.Split(',');
                    //转账地址
                    string addressto = xu[0];
                    //转账金额
                    string m = xu[1];

                    BigInteger dmount = BigInteger.Parse(m);
                    sum = sum + dmount;
                }
            }
            Console.WriteLine("sum:" + sum);
            sr.Close();

        }

        //批量计算
        async Task test_batchCal2()
        {
            string path = "D:\\address\\balances0801_01.txt";

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8);

            BigInteger sum = 0;
            string str = "";
            while (str != null)
            {
                str = sr.ReadLine();
                if (!string.IsNullOrEmpty(str))
                {
                    string[] xu = new String[2];
                    xu = str.Split(',');
                    //转账地址
                    string s = xu[0];
                    //转账金额
                    string m = xu[1];

                    //decimal dmount = decimal.Parse(m);
                    //decimal mount =  dmount * 100000000;
                    ////导出余额
                    //string  mstr =  Math.Round(mount, 0).ToString();
                    BigInteger mountRe = BigInteger.Parse(m);

                    var re = await sds_common.api_InvokeScriptByRPC(Config.sds, "balanceOf",
              "(addr)" + s);
                    sds_common.ResultItem item = re.value;

                    BigInteger mountSrc = item.subItem[0].AsInteger();
                    //Console.WriteLine(s + ","+m);
                    if (mountRe != mountSrc)
                    {
                        Console.WriteLine(s + "/mountSrc:" + mountSrc + "/mountRe:" + mountRe);
                    }
                }
            }
            sr.Close();

        }

        //批量计算
        async Task test_main()
        {
            //1|1|4
            int num = 1 | 1 | 4;
            Console.WriteLine("num:"+num);
            bool need_storage = (bool)(object)num;
            Console.WriteLine("bool:"+need_storage);
        }

    }
}
