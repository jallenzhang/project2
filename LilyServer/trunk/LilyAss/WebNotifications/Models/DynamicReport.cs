using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WebNotifications.Models
{
    public class DynamicReport
    {
        public string filename = "";
        public XmlDocument doc = null;
        public XmlNode root = null;
        public XmlNamespaceManager xnm = null;
        public string ns = "";
        public DynamicReport()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }
        public DynamicReport(string filename)
        {
            ns = "http://schemas.microsoft.com/sqlserver/reporting/2005/01/reportdefinition";
            this.filename = filename;
            doc = new XmlDocument();
            doc.Load(filename);
            root = doc.DocumentElement;
            xnm = new XmlNamespaceManager(doc.NameTable);
            xnm.AddNamespace("rd", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner");
            xnm.AddNamespace("default", ns);
            xnm.PushScope();
        }

        private XmlNode CreateNode(string nodename, string innertext)
        {
            XmlNode node = null;
            node = doc.CreateNode(XmlNodeType.Element, nodename, ns);
            node.InnerText = innertext;
            return node;
        }

        private XmlNode CreateNode(string nodename)
        {
            XmlNode node = null;
            node = doc.CreateNode(XmlNodeType.Element, nodename, ns);
            return node;
        }

        public void AddParamter(string name, string type, string prompt)
        {
            XmlNode node = null;
            XmlNode refCd = root.SelectSingleNode("//default:ReportParameters", xnm);
            XmlElement docFrag = doc.CreateElement("ReportParameter", ns);
            docFrag.SetAttribute("Name", name);
            node = doc.CreateNode(XmlNodeType.Element, "DataType", ns);
            node.InnerText = type;
            docFrag.AppendChild(node);
            node = doc.CreateNode(XmlNodeType.Element, "Nullable", ns);
            node.InnerText = "true";
            docFrag.AppendChild(node);
            node = doc.CreateNode(XmlNodeType.Element, "Prompt", ns);
            node.InnerText = prompt;
            docFrag.AppendChild(node);
            refCd.InsertAfter(docFrag, refCd.LastChild);
            doc.Save(filename);
        }

        public void AddTableColumn()
        {
            XmlNode refCd = root.SelectSingleNode("//default:Report//default:Body//default:ReportItems//default:Table//default:TableColumns", xnm);
            XmlNode docFrag = CreateNode("TableColumn");
            XmlNode width = CreateNode("Width", "2.5cm");
            docFrag.AppendChild(width);
            refCd.AppendChild(docFrag);
        }

        //动态增加详细列 
        public void AddDetailsCell(string ColName, string ColValue)
        {
            XmlNode refCd = root.SelectSingleNode("//default:Report//default:Body//default:ReportItems//default:Table//default:Details//default:TableRows//default:TableRow//default:TableCells", xnm);
            XmlElement docFrag = doc.CreateElement("TableCell", ns);
            XmlNode reportitems = CreateNode("ReportItems");
            XmlElement textbox = doc.CreateElement("Textbox", ns);
            textbox.SetAttribute("Name", ColName);
            XmlNode zindex = CreateNode("ZIndex", "20");
            textbox.AppendChild(zindex);
            XmlNode style = CreateNode("Style");
            textbox.AppendChild(style);
            XmlNode borderstyle = CreateNode("BorderStyle");
            style.AppendChild(borderstyle);
            XmlNode defaul = CreateNode("Default", "Solid");
            borderstyle.AppendChild(defaul);
            XmlNode textalign = CreateNode("TextAlign", "Center");
            style.AppendChild(textalign);
            XmlNode PaddingLeft = CreateNode("PaddingLeft", "2pt");
            style.AppendChild(PaddingLeft);
            XmlNode PaddingBottom = CreateNode("PaddingBottom", "2pt");
            style.AppendChild(PaddingBottom);
            XmlNode FontFamily = CreateNode("FontFamily", "宋体");
            style.AppendChild(FontFamily);
            XmlNode VerticalAlign = CreateNode("VerticalAlign", "Middle");
            style.AppendChild(VerticalAlign);
            XmlNode PaddingTop = CreateNode("PaddingTop", "2pt");
            style.AppendChild(PaddingTop);
            XmlNode PaddingRight = CreateNode("PaddingRight", "2pt");
            style.AppendChild(PaddingRight);
            XmlNode cangrow = CreateNode("CanGrow", "true");
            textbox.AppendChild(cangrow);
            XmlNode value = CreateNode("Value", ColValue);
            textbox.AppendChild(value);
            reportitems.AppendChild(textbox);
            docFrag.AppendChild(reportitems);
            refCd.InsertAfter(docFrag, refCd.LastChild);
        }

        //动态增加表头
        public void AddTableHeaderCell(string colname, string paramvalue)
        {
            XmlNode node = null;
            XmlNode refCd = root.SelectSingleNode("//default:Report//default:Body//default:ReportItems//default:Table//default:Header//default:TableRows//default:TableRow//default:TableCells", xnm);
            XmlElement docFrag = doc.CreateElement("TableCell", ns);
            XmlNode reportitems = CreateNode("ReportItems");
            XmlElement textbox = doc.CreateElement("Textbox", ns);
            textbox.SetAttribute("Name", colname);
            XmlNode zindex = CreateNode("ZIndex", "50");
            textbox.AppendChild(zindex);
            XmlNode style = CreateNode("Style");
            XmlNode borderstyle = CreateNode("BorderStyle");
            style.AppendChild(borderstyle);
            XmlNode defaul = CreateNode("Default", "Solid");
            borderstyle.AppendChild(defaul);
            XmlNode textalign = CreateNode("TextAlign", "Center");
            style.AppendChild(textalign);
            XmlNode PaddingLeft = CreateNode("PaddingLeft", "2pt");
            style.AppendChild(PaddingLeft);
            XmlNode PaddingBottom = CreateNode("PaddingBottom", "2pt");
            style.AppendChild(PaddingBottom);
            XmlNode FontFamily = CreateNode("FontFamily", "宋体");
            style.AppendChild(FontFamily);
            //XmlNode FontWeight = CreateNode("FontWeight", "700");
            //style.AppendChild(FontWeight);
            XmlNode VerticalAlign = CreateNode("VerticalAlign", "Bottom");
            style.AppendChild(VerticalAlign);
            XmlNode PaddingTop = CreateNode("PaddingTop", "2pt");
            style.AppendChild(PaddingTop);
            XmlNode PaddingRight = CreateNode("PaddingRight", "2pt");
            style.AppendChild(PaddingRight);
            textbox.AppendChild(style);
            XmlNode cangrow = CreateNode("CanGrow", "true");
            textbox.AppendChild(cangrow);
            XmlNode value = CreateNode("Value", paramvalue);
            textbox.AppendChild(value);
            reportitems.AppendChild(textbox);
            docFrag.AppendChild(reportitems);
            refCd.InsertAfter(docFrag, refCd.LastChild);
        }

        //动态添加页眉
        public void AddTableHaderFirstRowSingleCell(string colname, string paramvalue)
        {
        }
    }
}