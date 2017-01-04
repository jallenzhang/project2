using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;
using System.Xml;

namespace WebNotifications.Utils.AliSecuriy
{
    /// <summary>
    /// 类名：Function
    /// 功能：支付宝接口公用函数类
    /// 详细：该类是请求、通知返回两个文件所调用的公用函数核心处理文件，不需要修改
    /// 版本：1.0
    /// 日期：2011-09-01
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
    /// </summary>
    public class Function
    {
        /// <summary>
        /// 验签（不排序 Notify验签用这个）
        /// </summary>
        /// <param name="content">待验签字符串</param>
        /// <param name="signedString">签名（支付宝返回sign）</param>
        /// <param name="publickey">支付宝公钥</param>
        /// <param name="input_charset">编码格式</param>
        /// <returns>返回验签结果，true(相同)，false(不相同)</returns>
        public static bool Verify(string content, string signedString, string publickey, string input_charset) 
        {
            bool b = RSAFromPkcs8.verify(content, signedString, publickey, input_charset);
            return b;
        }

        /// <summary>
        /// 返回 XML字符串 节点value
        /// </summary>
        /// <param name="xmlDoc">XML格式 数据</param>
        /// <param name="xmlNode">节点</param>
        /// <returns>节点value</returns>
        public static string GetStrForXmlDoc(string xmlDoc, string xmlNode)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlDoc);
            XmlNode xn = xml.SelectSingleNode(xmlNode);
            return xn.InnerText;
        }
    }
}