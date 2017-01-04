using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using WebNotifications.Models.YeePay;
using WebNotifications.Utils.Yeepay;
using WebNotifications.Common;

namespace WebNotifications.Helper.PayHelper
{
    public class YeepayHelper
    {
        // 验证返回结果
        public static BuyCallbackResult VerifyCallback(string p1_MerId, string r0_Cmd, string r1_Code, string r2_TrxId, string r3_Amt,
            string r4_Cur, string r5_Pid, string r6_Order, string r7_Uid, string r8_MP, string r9_BType, string rp_PayDate, string hmac)
        {
            string sbOld = "";

            sbOld += p1_MerId;
            sbOld += r0_Cmd;
            sbOld += r1_Code;
            sbOld += r2_TrxId;
            sbOld += r3_Amt;

            sbOld += r4_Cur;
            sbOld += r5_Pid;
            sbOld += r6_Order;
            sbOld += r7_Uid;
            sbOld += r8_MP;

            sbOld += r9_BType;

            string nhmac = Digest.HmacSign(sbOld, PayConfigFile.YEEPAY_KEY_VALUE);
            if (nhmac == hmac)
            {
                return new BuyCallbackResult(p1_MerId, r0_Cmd, r1_Code, r2_TrxId, r3_Amt, r4_Cur, r5_Pid, r6_Order, r7_Uid, r8_MP, r9_BType,
                    rp_PayDate, hmac, "");
            }
            else
            {
                return new BuyCallbackResult(p1_MerId, r0_Cmd, r1_Code, r2_TrxId, r3_Amt, r4_Cur, r5_Pid, r6_Order, r7_Uid, r8_MP, r9_BType,
                    rp_PayDate, hmac, "交易签名被篡改");
            }
        }
    }
}