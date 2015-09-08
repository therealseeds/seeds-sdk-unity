using System;
using System.Xml;

public static class SeedsUtilities
{
    public static XmlElement AppendChildElement(this XmlNode xmlNode, string tag)
    {
        var xmlElement = xmlNode.OwnerDocument.CreateElement(tag);
        xmlNode.AppendChild(xmlElement);
        return xmlElement;
    }

    public static XmlElement AppendChildElement(this XmlNode xmlNode, string tag, string value)
    {
        var xmlElement = xmlNode.AppendChildElement(tag);
        var xmlText = xmlNode.OwnerDocument.CreateTextNode(value);
        xmlElement.AppendChild(xmlText);
        return xmlElement;
    }
}
