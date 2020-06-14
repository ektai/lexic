/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ektai Solutions LTD expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ektai Solutions LTD by email at sales@lexic.xyz
 *
 * The interactive user interfaces in modified source and object code versions of LEXIC must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original LEXIC logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by LEXIC" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.WhiteLabel;
using HtmlAgilityPack;

namespace ASC.Web.Studio.Utility.HtmlUtility
{
    public class HtmlUtility
    {
        private static readonly Regex htmlTags = new Regex(@"</?html[^>]*>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex rxNumeric = new Regex(@"^[0-9]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);


        public static string GetFull(string html, bool removeAsccut = true)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return html ?? string.Empty;
            }

            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlTags.Replace(html, string.Empty));
                if (removeAsccut)
                {
                    var nodes = doc.DocumentNode.SelectNodes("//div[translate(@class,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='asccut']");
                    if (nodes != null)
                    {
                        foreach (var node in nodes)
                        {
                            node.Attributes.Remove("class");
                            var styleAttr = doc.CreateAttribute("style");
                            styleAttr.Value = "display:inline;";
                            node.Attributes.Append(styleAttr);
                        }
                    }
                }

                ProcessMaliciousTag(doc);
                ProcessMaliciousAttributes(doc);
                ProcessAscUserTag(doc);
                ProcessCodeTags(doc);
                ProcessExternalLinks(doc);
                ProcessZoomImages(doc);

                return doc.DocumentNode.InnerHtml;
            }
            catch (Exception e)
            {
                LogManager.GetLogger("ASC.Web").Error(e);
                return string.Format("{0}<br/>Please contact us: <a href='mailto:{1}'>{1}</a>", e.Message, CompanyWhiteLabelSettings.Instance.Email);
            }
        }

        private static string GetLanguageAttrValue(HtmlNode node)
        {
            var attr = node.Attributes["lang"];
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
            {
                return attr.Value;
            }
            return string.Empty;
        }

        private static LangType GetLanguage(HtmlNode node)
        {
            var result = LangType.Unknown;

            switch (GetLanguageAttrValue(node).ToLower())
            {
                case "c":
                    result = LangType.C;
                    break;
                case "cpp":
                case "c++":
                    result = LangType.CPP;
                    break;
                case "csharp":
                case "c#":
                case "cs":
                    result = LangType.CS;
                    break;
                case "asp":
                    result = LangType.Asp;
                    break;
                case "html":
                case "htm":
                    result = LangType.Html;
                    break;
                case "xml":
                    result = LangType.Xml;
                    break;
                case "js":
                case "jsript":
                case "javascript":
                    result = LangType.JS;
                    break;
                case "sql":
                case "tsql":
                    result = LangType.TSql;
                    break;
                case "vb":
                case "vbnet":
                    result = LangType.VB;
                    break;
            }

            return result;
        }

        private static void ProcessMaliciousAttributes(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.SelectNodes("//*");
            if (nodes == null || nodes.Count == 0)
            {
                return;
            }

            foreach (var node in nodes)
            {
                var toRemove = node.Attributes
                                   .Where(htmlAttribute =>
                                       {
                                           var name = htmlAttribute.Name;
                                           if (name.Contains("/")) name = name.Split('/').Last();
                                           var value = htmlAttribute.Value.Replace("&Tab;", "").TrimStart();
                                           return name.StartsWith("on", StringComparison.OrdinalIgnoreCase)
                                                  || value.StartsWith("javascript", StringComparison.OrdinalIgnoreCase)
                                                  || value.StartsWith("data", StringComparison.OrdinalIgnoreCase)
                                                  || value.StartsWith("vbscript", StringComparison.OrdinalIgnoreCase)
                                                  || value.StartsWith(">", StringComparison.OrdinalIgnoreCase);
                                       }
                    ).ToList();
                foreach (var htmlAttribute in toRemove)
                {
                    if (node.Name == "img" && htmlAttribute.Name == "src")
                        continue;

                    node.Attributes.Remove(htmlAttribute);
                }
            }
        }

        private static void ProcessZoomImages(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.SelectNodes("//img[@_zoom]");

            if (nodes == null) return;
            foreach (var node in nodes)
            {
                if (node.ParentNode != null && (node.ParentNode.Name ?? "").ToLower() == "a") continue;

                var srcAttribute = node.Attributes["src"];
                if (srcAttribute == null || string.IsNullOrEmpty(srcAttribute.Value)) continue;

                var zoomAttribute = node.Attributes["_zoom"];
                if (zoomAttribute == null || string.IsNullOrEmpty(zoomAttribute.Value)) continue;

                var borderAttribute = node.Attributes["border"];
                if (borderAttribute == null)
                {
                    borderAttribute = doc.CreateAttribute("border");
                    node.Attributes.Append(borderAttribute);
                }
                borderAttribute.Value = "0";

                var imgSrc = srcAttribute.Value;

                if (!rxNumeric.IsMatch(zoomAttribute.Value))
                {
                    imgSrc = zoomAttribute.Value;
                }

                if (node.ParentNode != null)
                {
                    var hrefNode = doc.CreateElement("a");

                    var hrefAttribute = doc.CreateAttribute("href");
                    hrefAttribute.Value = imgSrc;
                    hrefNode.Attributes.Append(hrefAttribute);

                    hrefAttribute = doc.CreateAttribute("class");
                    hrefAttribute.Value = "screenzoom";
                    hrefNode.Attributes.Append(hrefAttribute);

                    string title = null;
                    var titleAttribute = node.Attributes["title"];
                    if (titleAttribute != null)
                    {
                        title = titleAttribute.Value;
                    }
                    else
                    {
                        var altAttribute = node.Attributes["alt"];
                        if (altAttribute != null)
                        {
                            title = altAttribute.Value;
                        }
                    }
                    if (!string.IsNullOrEmpty(title))
                    {
                        hrefAttribute = doc.CreateAttribute("title");
                        hrefAttribute.Value = title;
                        hrefNode.Attributes.Append(hrefAttribute);
                    }

                    node.ParentNode.ReplaceChild(hrefNode, node);
                    hrefNode.AppendChild(node);
                }
            }
        }

        private static void ProcessMaliciousTag(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.SelectNodes("//*");
            if (nodes == null || nodes.Count == 0)
                return;

            foreach (var node in nodes.Where(node => Regex.IsMatch(node.Name, "\\W")))
            {
                node.ParentNode.RemoveChild(node);
            }

            nodes = doc.DocumentNode.SelectNodes("//script|//meta|//style");

            if (nodes == null || nodes.Count == 0)
                return;

            foreach (var node in nodes.Where(node => node.ParentNode != null))
            {
                node.ParentNode.RemoveChild(node);
            }
        }

        private static void ProcessAscUserTag(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.SelectNodes("//div[@__ascuser]");
            if (nodes == null || nodes.Count == 0) return;

            foreach (var node in nodes)
            {
                var userId = new Guid(node.Attributes["__ascuser"].Value);
                node.Attributes.RemoveAll();
                var styleAttr = doc.CreateAttribute("style");
                styleAttr.Value = "display:inline;";
                node.Attributes.Append(styleAttr);
                node.InnerHtml = CoreContext.UserManager.GetUsers(userId).RenderProfileLinkBase();
            }
        }

        private static void ProcessCodeTags(HtmlDocument doc)
        {
            var scripts = doc.DocumentNode.SelectNodes("//code");
            if (scripts != null)
            {
                foreach (var node in scripts)
                {
                    var textNode = doc.CreateTextNode(Highlight.HighlightToHTML(node.InnerHtml, GetLanguage(node), true).Replace(@"class=""codestyle""", string.Format(@"class=""codestyle"" _wikiCodeStyle=""{0}""", GetLanguageAttrValue(node))));
                    node.ParentNode.ReplaceChild(textNode, node);
                }
            }
        }

        private static void ProcessExternalLinks(HtmlDocument doc)
        {
            var links = doc.DocumentNode.SelectNodes("//a");

            if (links == null) return;

            var context = HttpContext.Current;

            if (context == null) return;

            var uri = context.Request.GetUrlRewriter();

            if (uri == null) return;

            var internalHost = uri.Host;

            if ((uri.Port != 80 && uri.Scheme.Equals("http", StringComparison.InvariantCultureIgnoreCase))
                || (uri.Port != 443 && uri.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase)))
            {
                internalHost = string.Format(@"^{2}:\/\/{0}:{1}", internalHost, uri.Port, uri.Scheme);
            }
            else
            {
                internalHost = string.Format(@"^{2}:\/\/{0}(:{1})?", internalHost, uri.Port, uri.Scheme);
            }

            var rxInternalHost = new Regex(internalHost, RegexOptions.Compiled | RegexOptions.CultureInvariant);

            foreach (var node in links)
            {
                ProcessExternalLink(node, rxInternalHost);
            }
        }

        private static void ProcessExternalLink(HtmlNode node, Regex rxIntLink)
        {
            var attrHref = node.Attributes["href"];
            if (attrHref == null) return;

            if (!rxIntLink.IsMatch(attrHref.Value))
            {
                var attrTarg = node.Attributes["target"];
                if (attrTarg == null)
                {
                    attrTarg = node.OwnerDocument.CreateAttribute("target");
                    node.Attributes.Append(attrTarg);
                }

                attrTarg.Value = "_blank";
            }
        }
    }
}