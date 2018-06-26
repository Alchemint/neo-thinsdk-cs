using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ThinNeo;

namespace smartContractDemo
{
    class callTest : ITest
    {
        public string Name => "Call 合约测试";

        public string ID => "call";
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

        public callTest()
        {
            this.initMenu();
        }

        private void initMenu()
        {
            infos = new Dictionary<string, testAction>();

            infos["settargetW"] = test_settargetW;
            infos["settargetP"] = test_settargetP;
            infos["settargetSD"] = test_settargetSD;
            infos["settargetSdt"] = test_settargetSDT;
            //infos["callScript"] = test_setCallScript;
            infos["setWneoCallScript"] = test_setWneoCallScript;
            infos["setPneoCallScript"] = test_setPneoCallScript;
            infos["setSDCallScript"] = test_setSDCallScript;
            //infos["call01"] = test_call01;
            //infos["call02"] = test_call02;
            //infos["call03"] = test_call03;


            this.submenu = new List<string>(infos.Keys).ToArray();
        }

        public delegate void Method(ThinNeo.ScriptBuilder sb);//第一步：定义委托类型

        public async Task Demo()
        {
            //得到合约代码
            var urlgetscript = Helper.MakeRpcUrl(smartContractDemo.tests.Config.api, "getcontractstate", new MyJson.JsonNode_ValueString(call_common.call_sc));
            var resultgetscript = await Helper.HttpGet(urlgetscript);
            var _json = MyJson.Parse(resultgetscript).AsDict();
            var _resultv = _json["result"].AsList()[0].AsDict();

            n55contract = ThinNeo.Helper.HexString2Bytes(_resultv["script"].AsString());

            //Console.WriteLine("reg=" + _resultv["script"].AsString());

            showMenu();

            prikey = ThinNeo.Helper.GetPrivateKeyFromWIF(smartContractDemo.tests.Config.testwif);
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

        //设置W合约参数
        async Task test_settargetW()
        {
            var target = new ThinNeo.Hash160(wneo_common.sc);
            var result = await call_common.api_SendTransaction(this.prikey, call_common.main, "setTargetWneo", "(hex160)" + target.ToString());
            subPrintLine("result=" + result);
        }
        //设置P合约参数
        async Task test_settargetP()
        {
            var target = new ThinNeo.Hash160(pneo_common.sc);
            var result = await call_common.api_SendTransaction(this.prikey, call_common.main, "setTargetPneo", "(hex160)" + target.ToString());
            subPrintLine("result=" + result);
        }
        //设置SD合约参数
        async Task test_settargetSD()
        {
            var target = new ThinNeo.Hash160(sdusd_common.sc);
            var result = await call_common.api_SendTransaction(this.prikey, call_common.main, "setTargetSDUSD", "(hex160)" + target.ToString());
            subPrintLine("result=" + result);
        }
        
        //设置SD合约参数
        async Task test_settargetSDT()
        {
            var target = new ThinNeo.Hash160(sdt_common.sc);
            var result = await call_common.api_SendTransaction(this.prikey, call_common.main, "setTargetSDT", "(hex160)" + target.ToString());
            subPrintLine("result=" + result);
        }
        //设置SD合约参数
        async Task test_setCallScript()
        {
            //sdusdCallScript
            //pneoCallScript
            //wneoCallScript
            Console.WriteLine("write type:");
            string type = Console.ReadLine();

            Console.WriteLine("Input target address:");
            string addr = Console.ReadLine();
            var result = await call_common.api_SendTransaction(this.prikey, call_common.main, "setCallScript", "(str)" + type,"(addr)"+addr);
            subPrintLine("result=" + result);
        }

        async Task test_setWneoCallScript()
        {
            //sdusdCallScript
            //pneoCallScript
            //wneoCallScript
            var addr = ThinNeo.Helper.GetAddressFromScriptHash(wneo_common.sc_wneo);
            Console.WriteLine("address:"+addr);
            var result = await call_common.api_SendTransaction(this.prikey, call_common.main, "setCallScript", "(str)wneoCallScript", "(addr)" + addr);
            subPrintLine("result=" + result);
        }

        async Task test_setPneoCallScript()
        {
            //sdusdCallScript
            //pneoCallScript
            //wneoCallScript
            var addr = ThinNeo.Helper.GetAddressFromScriptHash(pneo_common.sc_pneo);
            Console.WriteLine("address:" + addr);
            var result = await call_common.api_SendTransaction(this.prikey, call_common.main, "setCallScript", "(str)pneoCallScript", "(addr)" + addr);
            subPrintLine("result=" + result);
        }

        async Task test_setSDCallScript()
        {
            //sdusdCallScript
            //pneoCallScript
            //wneoCallScript
            var addr = ThinNeo.Helper.GetAddressFromScriptHash(sdusd_common.sc_sdusd);
            Console.WriteLine("address:" + addr);
            var result = await call_common.api_SendTransaction(this.prikey, call_common.main, "setCallScript", "(str)sdusdCallScript", "(addr)" + addr);
            subPrintLine("result=" + result);
        }

        //async Task test_call01()
        //{
        //    var result = await call_common.api_SendTransaction(this.prikey, call_common.call_02, "call_01", 
        //        "(int)" + 1,"(int)"+2);
        //    subPrintLine("result=" + result);
        //}


        //async Task test_call02()
        //{
        //    var result = await call_common.api_SendTransaction(this.prikey, call_common.call_02, "call_02",
        //        "(int)" + 5, "(int)" + 1);
        //    subPrintLine("result=" + result);
        //}

        //async Task test_call03()
        //{
        //    var result = await call_common.api_InvokeScript(call_common.call_02, "call_03", null);
        //    call_common.ResultItem item = result.value;

        //    Console.WriteLine(item.subItem[0].AsInteger());
        //}
    }

}
