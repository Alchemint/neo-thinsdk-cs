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
    class dcTest : ITest
    {
        public string Name => "DataCenter 合约测试";

        public string ID => "dc";
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

        public dcTest()
        {
            this.initMenu();
        }

        private void initMenu()
        {
            infos = new Dictionary<string, testAction>();
          
            infos["setAccount"] = test_setAccount;
            infos["setPrice"] = test_setPrice;
            infos["setConfig"] = test_setConfig;
            infos["setMedian"] = test_setMedian;
            infos["setNEOPrice"] = test_setNEOPrice;

            //infos["setAnchorPrice"] = test_setAnchorPrice;
            infos["getTypeA"] = test_getTypeA;
            infos["getPrice"] = test_getPrice;
            infos["getAccount"] = test_getAccount;
            infos["getMedian"] = test_getMedian;
            //infos["getAnchorPrice"] = test_getAnchorPrice;
            infos["median"] = test_median;
            this.submenu = new List<string>(infos.Keys).ToArray();
        }

        public delegate void Method(ThinNeo.ScriptBuilder sb);//第一步：定义委托类型

        public async Task Demo()
        {
            //得到合约代码
            var urlgetscript = Helper.MakeRpcUrl(Config.api, "getcontractstate", new MyJson.JsonNode_ValueString(dc_common.sc));
            var resultgetscript = await Helper.HttpGet(urlgetscript);
            var _json = MyJson.Parse(resultgetscript).AsDict();
            var _resultv = _json["result"].AsList()[0].AsDict();

            n55contract = ThinNeo.Helper.HexString2Bytes(_resultv["script"].AsString());

            //Console.WriteLine("reg=" + _resultv["script"].AsString());

            showMenu();
            prikey_admin = ThinNeo.Helper.GetPrivateKeyFromWIF(Config.testwif_admin);
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

            var result = await dc_common.api_SendbatchTransaction(prikey, dc_common.sc_dc, "setAccount",
               "(addr)" + addr,"(int)"+1);
            subPrintLine(result);
        }

        //设置价格信息
        async Task test_setMedian()
        {
            var result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setPrice",
                "(str)neo_price_01",
                "(addr)" + this.address,
                "(int)10000000");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setPrice",
                "(str)neo_price_02",
                "(addr)" + this.address,
                "(int)20000000");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setPrice",
                "(str)neo_price_03",
                "(addr)" + this.address,
                "(int)30000000");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setPrice",
                "(str)neo_price_04",
                "(addr)" + this.address,
                "(int)40000000");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setPrice",
                "(str)neo_price_05",
                "(addr)" + this.address,
                "(int)50000000");
            subPrintLine(result);
        }

        async Task test_setNEOPrice()
        {
            Console.WriteLine("type:");
            string type = Console.ReadLine();

            Console.WriteLine("price:");
            string price = Console.ReadLine();
            var result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setPrice",
                "(str)"+type,
                "(addr)" + this.address,
                "(int)"+price);
            subPrintLine(result);
        }

            //设置价格信息
            async Task test_setPrice()
        {
            var result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setPrice", 
                "(str)sds_price",
                "(addr)"+this.address,
                "(int)10000000");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setPrice",
                "(str)neo_price",
                "(addr)" + this.address,
                "(int)2000000000");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setPrice",
                "(str)cneo_price",
                "(addr)" + this.address,
                "(int)2000000000");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setPrice",
                "(str)sneo_price",
                "(addr)" + this.address,
                "(int)2000000000");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setPrice",
                "(str)gas_price",
                "(addr)" + this.address,
                "(int)200000000");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setPrice",
                "(str)cgas_price",
                "(addr)" + this.address,
                "(int)200000000");
            subPrintLine(result);

            /*  
            *  anchor_type_usd    1*100000000
            *  anchor_type_cny    6.8*100000000
            *  anchor_type_eur    0.875*100000000
            *  anchor_type_jpy    120*100000000
            *  anchor_type_gbp    0.7813 *100000000
            *  anchor_type_gold   0.000838 * 100000000
            */

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setPrice", "(str)anchor_type_usd", "(addr)" + this.address, "(int)100000000");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setPrice", "(str)anchor_type_eur", "(addr)" + this.address, "(int)87500000");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setPrice", "(str)anchor_type_jpy", "(addr)" + this.address, "(int)1200000000");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setPrice", "(str)anchor_type_gbp", "(addr)" + this.address, "(int)78130000");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setPrice", "(str)anchor_type_gold", "(addr)" + this.address, "(int)838000");
            subPrintLine(result);

        }

        //设置价格信息
        async Task test_setConfig()
        {
            //B端抵押率
            var result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setConfig",
                "(str)liquidate_rate_b",
                "(int)50");
            subPrintLine(result);

            //C端抵押率
            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setConfig",
                "(str)liquidate_rate_c",
                "(int)150");
            subPrintLine(result);

            //C端清算折扣
            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setConfig",
                "(str)clear_rate",
                "(int)90");
            subPrintLine(result);
            
            //C端费用率  15秒的费率 乘以10的8次方
            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setConfig",
                "(str)fee_rate_c",
                "(int)148");
            subPrintLine(result);

            //C端最高可清算抵押率
            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setConfig",
                "(str)resuce_rate_c",
                "(int)160");
            subPrintLine(result);

            //C端伺机者可清算抵押率
            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setConfig",
                "(str)bond_rate_c",
                "(int)120");
            subPrintLine(result);

            //C端发行费用
            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setConfig",
                "(str)release_rate_c",
                "(int)1");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setConfig",
               "(str)service_fee",
               "(int)1000000000");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setConfig",
             "(str)release_max_c",
             "(int)1000000000000000");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setConfig", "(str)anchor_type_usd", "(int)1");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setConfig", "(str)anchor_type_eur", "(int)1");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setConfig", "(str)anchor_type_jpy", "(int)1");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setConfig", "(str)anchor_type_gbp", "(int)1");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey_admin, dc_common.sc_dc, "setConfig", "(str)anchor_type_gold", "(int)1");
            subPrintLine(result);

        }

        async Task test_getTypeA() {
            var result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getTypeA", "(str)liquidate_rate_b");
            dc_common.ResultItem item = result.value;
            Console.WriteLine("liquidate_rate_b:" + item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getTypeA", "(str)liquidate_rate_c");
            item = result.value;
            Console.WriteLine("liquidate_rate_c:" + item.subItem[0].AsInteger());



            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getTypeA", "(str)clear_rate");
            item = result.value;
            Console.WriteLine("clear_rate:" + item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getTypeA", "(str)resuce_rate_c");
            item = result.value;
            Console.WriteLine("resuce_rate_c:" + item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getTypeA", "(str)bond_rate_c");
            item = result.value;
            Console.WriteLine("bond_rate_c:" + item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getTypeA", "(str)release_rate_c");
            item = result.value;
            Console.WriteLine("release_rate_c:" + item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getTypeA", "(str)service_fee");
            item = result.value;
            Console.WriteLine("service_fee:" + item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getTypeA", "(str)release_max_c");
            item = result.value;
            Console.WriteLine("release_max_c:" + item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getTypeA", "(str)fee_rate_c");
            item = result.value;
            Console.WriteLine("fee_rate_c:" + item.subItem[0].AsInteger());
            

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getTypeA", "(str)anchor_type_usd");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getTypeA", "(str)anchor_type_eur");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getTypeA", "(str)anchor_type_jpy");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getTypeA", "(str)anchor_type_gbp");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getTypeA", "(str)anchor_type_gold");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());
        }

        //查询配置信息
        async Task test_getPrice()
        {
            var result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getPrice", "(str)sds_price");
            dc_common.ResultItem item = result.value;
            Console.WriteLine("sds_price:"+item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getPrice", "(str)neo_price");
            item = result.value;
            Console.WriteLine("neo_price:" + item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getPrice", "(str)sneo_price");
            item = result.value;
            Console.WriteLine("sneo_price:" + item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getPrice", "(str)cneo_price");
            item = result.value;
            Console.WriteLine("cneo_price:" + item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getPrice", "(str)gas_price");
            item = result.value;
            Console.WriteLine("gas_price:" + item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getPrice", "(str)anchor_type_usd");
            item = result.value;
            Console.WriteLine("anchor_type_usd:" + item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getPrice", "(str)anchor_type_eur");
            item = result.value;
            Console.WriteLine("anchor_type_eur:"+item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getPrice", "(str)anchor_type_jpy");
            item = result.value;
            Console.WriteLine("anchor_type_jpy:" + item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getPrice", "(str)anchor_type_gbp");
            item = result.value;
            Console.WriteLine("anchor_type_gbp:"+item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getPrice", "(str)anchor_type_gold");
            item = result.value;
            Console.WriteLine("anchor_type_gold:"+item.subItem[0].AsInteger());
        }


        //查询配置信息
        async Task test_getAnchorPrice()
        {
            Console.WriteLine("key:anchor_type_usd");

            var result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getAnchorPrice", "(str)anchor_type_usd");
            dc_common.ResultItem item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getAnchorPrice", "(str)anchor_type_eur");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getAnchorPrice", "(str)anchor_type_jpy");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getAnchorPrice", "(str)anchor_type_gbp");
            item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());

            result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getAnchorPrice", "(str)anchor_type_gold");
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

            var result = await dc_common.api_SendbatchTransaction(prikey, dc_common.sc_dc, "setAnchorPrice", "(str)anchor_type_usd", "(int)100000000");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey, dc_common.sc_dc, "setAnchorPrice", "(str)anchor_type_eur", "(int)87500000");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey, dc_common.sc_dc, "setAnchorPrice", "(str)anchor_type_jpy", "(int)1200000000");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey, dc_common.sc_dc, "setAnchorPrice", "(str)anchor_type_gbp", "(int)78130000");
            subPrintLine(result);

            result = await dc_common.api_SendbatchTransaction(prikey, dc_common.sc_dc, "setAnchorPrice", "(str)anchor_type_gold", "(int)838000");
            subPrintLine(result);


        }

        //查询配置信息
        async Task test_getMedian()
        {
            var result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getMedian", "(str)sds_price");
            dc_common.ResultItem item = result.value;
            Console.WriteLine("getMedian:" + item.subItem[0].AsInteger());

        }

        async Task test_getAccount()
        {
            Console.WriteLine("addr:");
            string addr = Console.ReadLine();

            var result = await dc_common.api_InvokeScript(dc_common.sc_dc, "getAccount", "(addr)"+addr);
            dc_common.ResultItem item = result.value;
            Console.WriteLine("addr:" + item.subItem[0].AsInteger());

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
