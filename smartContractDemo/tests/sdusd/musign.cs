using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ThinNeo;
using smartContractDemo.tests;
using System.Linq;

namespace smartContractDemo
{
    class muSignTest : ITest
    {
        public string Name => "Musign 合约测试";

        public string ID => "mu";
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

        public muSignTest()
        {
            this.initMenu();
        }

        private void initMenu()
        {
            infos = new Dictionary<string, testAction>();

            infos["setAccount"] = test_setAccount;
            infos["setCallAccount"] = test_setCallAccount;
            infos["getCallAccount"] = test_getCallAccount;
            infos["getStorage"] = test_getStorage;
            infos["init"] = test_init;
            infos["getNmae"] = test_getName;
            this.submenu = new List<string>(infos.Keys).ToArray();
        }

        public delegate void Method(ThinNeo.ScriptBuilder sb);//第一步：定义委托类型

        public async Task Demo()
        {
            //得到合约代码
            //var urlgetscript = Helper.MakeRpcUrl(sdusd_common.api, "getcontractstate", new MyJson.JsonNode_ValueString(sdusd_common.sc));
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

        async Task test_getStorage()
        {
            //var rev = ThinNeo.Helper.HexString2Bytes(key).Reverse().ToArray();
            var revkey = ThinNeo.Helper.GetPublicKeyHashFromAddress("AZ77FiX7i9mRUPF2RyuJD2L8kS6UDnQ9Y7");
            string revhash =  revkey.ToString();

            Console.WriteLine(revhash);
            
            var rev = ThinNeo.Helper.HexString2Bytes(revhash.Replace("0x","")).Reverse().ToArray();
            //var key2 = ThinNeo.Helper.Bytes2HexString(rev);
            var key2 = "61646d696e5f6163636f756e74";
            Console.WriteLine(key2);
            //ThinNeo.Helper.Get
            string key = "16" + key2;
            var url = Helper.MakeRpcUrl(Config.api, "getstorage", new MyJson.JsonNode_ValueString(Config.sc_musign), new MyJson.JsonNode_ValueString(key));
            string result = await Helper.HttpGet(url);
            Console.WriteLine("得到的结果是：" + result);
        }

        async Task test_setCallAccount()
        {
            byte[] prikey_admin = ThinNeo.Helper.GetPrivateKeyFromWIF(Config.testwif_admin);

            var addr = ThinNeo.Helper.GetAddressFromScriptHash(Config.sar4c);
            Console.WriteLine("sar address:" + addr);

            var result = await sdusd_common.api_SendbatchTransaction(prikey_admin, Config.musign, "setCallAccount",
               "(addr)" + addr);
            subPrintLine(result);
        }

        async Task test_setAccount()
        {
            byte[] prikey_admin = ThinNeo.Helper.GetPrivateKeyFromWIF(Config.testwif_admin);

            var result = await sdusd_common.api_SendbatchTransaction(prikey_admin, Config.musign, "setAccount",
                "(str)admin_account",
               "(addr)ALCpGKEircfDFdkY6PiUmfYaFMNVVsn296");
            subPrintLine(result);
        }

        async Task test_init()
        {
            Console.WriteLine("input name:");
            string name = Console.ReadLine();
            var result = await sdusd_common.api_SendbatchTransaction(prikey, Config.musign, "init",
               "(str)"+name);
            subPrintLine(result);
        }

        async Task test_getName()
        {
            var result = await sdusd_common.api_InvokeScript(Config.musign, "getName");
            sdusd_common.ResultItem item = result.value;
            Console.WriteLine(item.subItem[0].AsString());
        }

        async Task test_getCallAccount()
        {
            var addr = "AZ77FiX7i9mRUPF2RyuJD2L8kS6UDnQ9Y7";
            Console.WriteLine("sar address:" + addr);

            var result = await sdusd_common.api_InvokeScript(Config.musign, "getCallAccount",
               "(addr)" + addr);

            sdusd_common.ResultItem item = result.value;
            Console.WriteLine(item.subItem[0].AsInteger());
        }

    }

}
