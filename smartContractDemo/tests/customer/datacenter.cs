﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ThinNeo;
using System.IO;
using System.Linq;
using System.Numerics;
using smartContractDemo.tests;

namespace smartContractDemo
{
    class datacenterTest : ITest
    {
        public string Name => "Token 合约测试";

        public string ID => "data";
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

        public datacenterTest()
        {
            this.initMenu();
        }

        private void initMenu()
        {
            infos = new Dictionary<string, testAction>();
          
            infos["setAccount"] = test_setAccount;
            infos["setPrice"] = test_setPrice;
            infos["setConfig"] = test_setConfig;
            infos["setStructConfig"] = test_setStructConfig;
            infos["setMedian"] = test_setMedian;
            //infos["setPow"] = test_setPow;
            //infos["setNEOPrice"] = test_setNEOPrice;
            infos["getNeoPrice"] = test_getNeoPrice;

            //infos["setAnchorPrice"] = test_setAnchorPrice;
            infos["getConfig"] = test_getConfig;
            infos["getStructConfig"] = test_getStructConfig;
            infos["getPrice"] = test_getPrice;
            infos["getAccount"] = test_getAccount;
            infos["setUpgrade"] = test_setUpgrade;
            //infos["getMedian"] = test_getMedian;
            //infos["getAnchorPrice"] = test_getAnchorPrice;

            this.submenu = new List<string>(infos.Keys).ToArray();
        }

        public delegate void Method(ThinNeo.ScriptBuilder sb);//第一步：定义委托类型

        public async Task Demo()
        {
            //得到合约代码
            var urlgetscript = Helper.MakeRpcUrl(Config.api, "getcontractstate", new MyJson.JsonNode_ValueString(Config.sc_oracle));
            var resultgetscript = await Helper.HttpGet(urlgetscript);
            var _json = MyJson.Parse(resultgetscript).AsDict();
            var _resultv = _json["result"].AsList()[0].AsDict();

            n55contract = ThinNeo.Helper.HexString2Bytes(_resultv["script"].AsString());

            //Console.WriteLine("reg=" + _resultv["script"].AsString());

            showMenu();
            //prikey_admin = ThinNeo.Helper.GetPrivateKeyFromWIF(Config.testwif_admin);
            prikey = ThinNeo.Helper.GetPrivateKeyFromWIF(Config.testwif_admin);
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

        //设置账户
        async Task test_setAccount()
        {
            Console.WriteLine("addr:");
            string addr = Console.ReadLine();

            var result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setAccount",
                "(str)admin_account",
               "(addr)" + addr);
            subPrintLine(result);
        }

        async Task test_getStructConfig()
        {
            var result = await oracle_common.api_InvokeScript(Config.oracle, "getStructConfig");
            oracle_common.ResultItem item = result.value;
            oracle_common.ResultItem[] items = item.subItem[0].subItem;

            if (items != null)
            {
                Console.WriteLine("liquidate_line_rate_b:" + items[0].AsInteger());
                Console.WriteLine("liquidate_line_rate_c:" + items[1].AsInteger());
                Console.WriteLine("liquidate_dis_rate_c:" + items[2].AsInteger());
                Console.WriteLine("fee_rate_c:" + items[3].AsInteger());
                Console.WriteLine("liquidate_top_rate_c:" + items[4].AsInteger());
                Console.WriteLine("liquidate_line_rateT_c:" + items[5].AsInteger());
                Console.WriteLine("issuing_fee_c:" + items[6].AsInteger());
                Console.WriteLine("issuing_fee_b:" + items[7].AsInteger());
                Console.WriteLine("debt_top_c:" + items[8].AsInteger());
            }
        }

        async Task test_getNeoPrice()
        {
            var result = await datacenter_common.api_InvokeScript(Config.oracle, "getNeoPrice", "(int)1");
            datacenter_common.ResultItem item = result.value;
            Console.WriteLine("neo_price:" + item.subItem[0].AsInteger());


            result = await datacenter_common.api_InvokeScript(Config.oracle, "getNeoPrice", "(int)2");
             item = result.value;
            Console.WriteLine("neo_price:" + item.subItem[0].AsInteger());

            result = await datacenter_common.api_InvokeScript(Config.oracle, "getNeoPrice", "(int)3");
            item = result.value;
            Console.WriteLine("neo_price:" + item.subItem[0].AsInteger());

            result = await datacenter_common.api_InvokeScript(Config.oracle, "getNeoPrice", "(int)4");
            item = result.value;
            Console.WriteLine("neo_price:" + item.subItem[0].AsInteger());

            result = await datacenter_common.api_InvokeScript(Config.oracle, "getNeoPrice", "(int)5");
             item = result.value;
            Console.WriteLine("neo_price:" + item.subItem[0].AsInteger());
        }


            //设置价格信息
         async Task test_setMedian()
        {
            Console.WriteLine("请输入总统计数:");
            string num = Console.ReadLine();
            var result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setMedian",
                "(int)"+num);
            subPrintLine(result);

        }

        async Task test_setPow()
        {
            Console.WriteLine("请输入x:");
            string x = Console.ReadLine();

            Console.WriteLine("请输入y:");
            string y = Console.ReadLine();
            var result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setPow",
                "(int)" + x,
                "(int)" + y);
            subPrintLine(result);

        }

        async Task test_setNEOPrice()
        {
            var result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setNeoPrice",
                "(int)1",
                "(int)100");
            subPrintLine(result);

            result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setNeoPrice",
                "(int)2",
                "(int)90");
            subPrintLine(result);

            result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setNeoPrice",
                "(int)3",
                "(int)110");
            subPrintLine(result);

            result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setNeoPrice",
                "(int)4",
                "(int)120");
            subPrintLine(result);

            result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setNeoPrice",
                "(int)5",
                "(int)130");
            subPrintLine(result);
        }

            //设置价格信息
            async Task test_setPrice()
        {
            Console.WriteLine("请输入:");
            string x = Console.ReadLine();

            var result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setTypeB",
                "(str)cneo_price",
                "(addr)" + this.address,
                "(int)"+double.Parse(x)*100000000);
            subPrintLine(result);

            //result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setPrice",
            //    "(str)neo_price",
            //    "(addr)" + this.address,
            //    "(int)2000000000");
            //subPrintLine(result);

            //result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setPrice",
            //    "(str)gas_price",
            //    "(addr)" + this.address,
            //    "(int)200000000");
            //subPrintLine(result);

            /*  
            *  anchor_type_usd    1*100000000
            *  anchor_type_cny    6.8*100000000
            *  anchor_type_eur    0.875*100000000
            *  anchor_type_jpy    120*100000000
            *  anchor_type_gbp    0.7813 *100000000
            *  anchor_type_gold   0.000838 * 100000000
            */

            //result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setPrice", "(str)anchor_type_usd", "(addr)" + this.address, "(int)100000000");
            //subPrintLine(result);

            //result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setPrice", "(str)anchor_type_eur", "(addr)" + this.address, "(int)87500000");
            //subPrintLine(result);

            //result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setPrice", "(str)anchor_type_jpy", "(addr)" + this.address, "(int)1200000000");
            //subPrintLine(result);

            //result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setPrice", "(str)anchor_type_gbp", "(addr)" + this.address, "(int)78130000");
            //subPrintLine(result);

            //result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setPrice", "(str)anchor_type_gold", "(addr)" + this.address, "(int)838000");
            //subPrintLine(result);

        }

        //设置价格信息
        async Task test_setConfig()
        {
            var result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setConfig",
                "(str)liquidate_rate_b",
                "(int)50");
            subPrintLine(result);

            result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setConfig",
                "(str)liquidate_rate_c",
                "(int)150");
            subPrintLine(result);

            result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setConfig",
                "(str)clear_rate",
                "(int)90");
            subPrintLine(result);

            result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setConfig",
                "(str)resuce_rate_c",
                "(int)160");
            subPrintLine(result);

            

            result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setConfig",
               "(str)service_fee",
               "(int)1000000000");
            subPrintLine(result);

            result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setConfig", "(str)anchor_type_usd", "(int)1");
            subPrintLine(result);

            result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setConfig", "(str)anchor_type_eur", "(int)1");
            subPrintLine(result);

            result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setConfig", "(str)anchor_type_jpy", "(int)1");
            subPrintLine(result);

            result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setConfig", "(str)anchor_type_gbp", "(int)1");
            subPrintLine(result);

            result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setConfig", "(str)anchor_type_gold", "(int)1");
            subPrintLine(result);

        }

        async Task test_getConfig() {
            var result = await datacenter_common.api_InvokeScript(Config.oracle, "getConfig", "(str)result");
            datacenter_common.ResultItem item = result.value;
            Console.WriteLine("medial result:" + item.subItem[0].AsInteger());

            result = await datacenter_common.api_InvokeScript(Config.oracle, "getConfig", "(str)pow");
            item = result.value;
            Console.WriteLine("pow:" + item.subItem[0].AsInteger());

        }



        //查询配置信息
        async Task test_getPrice()
        {
            var result = await datacenter_common.api_InvokeScript(Config.oracle, "getTypeB", "(str)sds_price");
            datacenter_common.ResultItem item = result.value;
            Console.WriteLine("sds_price:"+item.subItem[0].AsInteger());

            result = await datacenter_common.api_InvokeScript(Config.oracle, "getTypeB", "(str)neo_price");
            item = result.value;
            Console.WriteLine("neo_price:" + item.subItem[0].AsInteger());

            result = await datacenter_common.api_InvokeScript(Config.oracle, "getTypeB", "(str)cneo_price");
            item = result.value;
            Console.WriteLine("cneo_price:" + item.subItem[0].AsInteger());

            result = await datacenter_common.api_InvokeScript(Config.oracle, "getTypeB", "(str)sneo_price");
            item = result.value;
            Console.WriteLine("sneo_price:" + item.subItem[0].AsInteger());

            result = await datacenter_common.api_InvokeScript(Config.oracle, "getTypeB", "(str)gas_price");
            item = result.value;
            Console.WriteLine("gas_price:" + item.subItem[0].AsInteger());

            result = await datacenter_common.api_InvokeScript(Config.oracle, "getTypeB", "(str)anchor_type_usd");
            item = result.value;
            Console.WriteLine("anchor_type_usd:" + item.subItem[0].AsInteger());

            result = await datacenter_common.api_InvokeScript(Config.oracle, "getTypeB", "(str)anchor_type_eur");
            item = result.value;
            Console.WriteLine("anchor_type_eur:"+item.subItem[0].AsInteger());

            result = await datacenter_common.api_InvokeScript(Config.oracle, "getTypeB", "(str)anchor_type_jpy");
            item = result.value;
            Console.WriteLine("anchor_type_jpy:" + item.subItem[0].AsInteger());

            result = await datacenter_common.api_InvokeScript(Config.oracle, "getTypeB", "(str)anchor_type_gbp");
            item = result.value;
            Console.WriteLine("anchor_type_gbp:"+item.subItem[0].AsInteger());

            result = await datacenter_common.api_InvokeScript(Config.oracle, "getTypeB", "(str)anchor_type_gold");
            item = result.value;
            Console.WriteLine("anchor_type_gold:"+item.subItem[0].AsInteger());
        }

        async Task test_setStructConfig()
        {
            var result = await oracle_common.api_SendbatchTransaction(prikey_admin, oracle_common.sc_wneo, "setStructConfig");
            subPrintLine(result);
        }


        //查询配置信息
        async Task test_getAnchorPrice()
        {
            Console.WriteLine("key:anchor_type_usd");

            var result = await datacenter_common.api_InvokeScript(Config.oracle, "getAnchorPrice", "(str)anchor_type_usd");
            datacenter_common.ResultItem item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());

            result = await datacenter_common.api_InvokeScript(Config.oracle, "getAnchorPrice", "(str)anchor_type_eur");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());

            result = await datacenter_common.api_InvokeScript(Config.oracle, "getAnchorPrice", "(str)anchor_type_jpy");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());

            result = await datacenter_common.api_InvokeScript(Config.oracle, "getAnchorPrice", "(str)anchor_type_gbp");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());

            result = await datacenter_common.api_InvokeScript(Config.oracle, "getAnchorPrice", "(str)anchor_type_gold");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //设置锚定物信息
        async Task test_setAnchorPrice()
        {
            /*  
             *  anchor_type_usd    1*100000000
             *  anchor_type_cny    6.8*100000000
             *  anchor_type_eur    0.875*100000000
             *  anchor_type_jpy    120*100000000
             *  anchor_type_gbp    0.7813 *100000000
             *  anchor_type_gold   0.000838 * 100000000
             */

            var result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setAnchorPrice", "(str)anchor_type_usd", "(int)100000000");
            subPrintLine(result);

            result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setAnchorPrice", "(str)anchor_type_eur", "(int)87500000");
            subPrintLine(result);

            result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setAnchorPrice", "(str)anchor_type_jpy", "(int)1200000000");
            subPrintLine(result);

            result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setAnchorPrice", "(str)anchor_type_gbp", "(int)78130000");
            subPrintLine(result);

            result = await datacenter_common.api_SendbatchTransaction(prikey, Config.oracle, "setAnchorPrice", "(str)anchor_type_gold", "(int)838000");
            subPrintLine(result);


        }

        //查询配置信息
        async Task test_getMedian()
        {
            var result = await datacenter_common.api_InvokeScript(Config.oracle, "getMedian", "(str)sds_price");
            datacenter_common.ResultItem item = result.value;
            Console.WriteLine("getMedian:" + item.subItem[0].AsInteger());

        }

        async Task test_getAccount()
        {
            Console.WriteLine("addr:");
            string addr = Console.ReadLine();

            var result = await datacenter_common.api_InvokeScript(Config.oracle, "getAccount", "(addr)"+addr);
            datacenter_common.ResultItem item = result.value;
            Console.WriteLine("addr:" + item.subItem[0].AsInteger());

        }

        //升级合约
        async Task test_setUpgrade()
        {
            byte[] prikey_admin = ThinNeo.Helper.GetPrivateKeyFromWIF(Config.testwif_admin);
            byte[] pubkey_admin = ThinNeo.Helper.GetPublicKeyFromPrivateKey(prikey_admin);
            string address = ThinNeo.Helper.GetAddressFromPublicKey(pubkey_admin);

            Console.WriteLine(address);
            Dictionary<string, List<Utxo>> dir = await Helper.GetBalanceByAddress(Config.api, address);

            //从文件中读取合约脚本
            byte[] script = System.IO.File.ReadAllBytes("C:\\Neo\\SmartContracts\\0xe7bce3dde514813762b44a11bb5767f343dafb22.avm"); //这里填你的合约所在地址
            string str_script = ThinNeo.Helper.Bytes2HexString(script);
            byte[] aa = ThinNeo.Helper.HexString2Bytes(str_script);
            var ss = 1 | 0 | 4;
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
                var shash = Config.oracle;
                sb.EmitAppCall(shash);

                string scriptPublish = ThinNeo.Helper.Bytes2HexString(sb.ToArray());
                byte[] postdata;
                var url = Helper.MakeRpcUrlPost(Config.api, "invokescript", out postdata, new MyJson.JsonNode_ValueString(scriptPublish));
                var result = await Helper.HttpPost(url, postdata);
                //string result = http.Post(api, "invokescript", new MyJson.JsonNode_Array() { new MyJson.JsonNode_ValueString(scriptPublish) },Encoding.UTF8);
                //var consume = (((MyJson.Parse(result) as MyJson.JsonNode_Object)["result"] as MyJson.JsonNode_Array)[0] as MyJson.JsonNode_Object)["gas_consumed"].ToString();
                //decimal gas_consumed = decimal.Parse(consume);
                ThinNeo.InvokeTransData extdata = new ThinNeo.InvokeTransData();
                extdata.gas = 510;// Math.Ceiling(gas_consumed - 10);
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


        async Task test_median() {

            double[] arr = new double[] { 1,1.1,2.3,4.5,7,8};
            //为了不修改arr值，对数组的计算和修改在tempArr数组中进行
            double[] tempArr = new double[arr.Length];
            arr.CopyTo(tempArr, 0);

            //对数组进行排序
            double temp;
            for (int i = 0; i < tempArr.Length; i++)
            {
                for (int j = i; j < tempArr.Length; j++)
                {
                    if (tempArr[i] > tempArr[j])
                    {
                        temp = tempArr[i];
                        tempArr[i] = tempArr[j];
                        tempArr[j] = temp;
                    }
                }
            }

            //针对数组元素的奇偶分类讨论
            if (tempArr.Length % 2 != 0)
            {
                  double ret =  tempArr[arr.Length / 2 + 1];
                  Console.WriteLine(ret);
            }
            else
            {
                Console.WriteLine((tempArr[tempArr.Length / 2] +
                    tempArr[tempArr.Length / 2 + 1]) / 2.0);
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
