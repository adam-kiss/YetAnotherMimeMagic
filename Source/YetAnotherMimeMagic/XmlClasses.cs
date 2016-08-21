﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

//  <? xml version="1.0" encoding="UTF-8"?>
//  <!DOCTYPE mime-info[
//      < !ELEMENT mime - info(mime - type) +>
//      < !ATTLIST mime - info xmlns CDATA #FIXED "http://www.freedesktop.org/standards/shared-mime-info">

//      < !ELEMENT mime - type(comment +, (acronym, expanded-acronym)? , (icon ? | generic - icon ? | glob | magic | treemagic | root - XML | alias | sub -class-of)*)>
//      <!ATTLIST mime-type type CDATA #REQUIRED>

//      <!-- a comment describing a document with the respective MIME type.Example: "WMV video" -->
//      <!ELEMENT comment(#PCDATA)>
//      <!ATTLIST comment xml:lang CDATA #IMPLIED>

//      <!-- a comment describing the respective unexpanded MIME type acronym.Example: "WMV" -->
//      <!ELEMENT acronym (#PCDATA)>
//      <!ATTLIST acronym xml:lang CDATA #IMPLIED>

//      <!-- a comment describing the respective expanded MIME type acronym.Example: "Windows Media Video" -->
//      <!ELEMENT expanded-acronym (#PCDATA)>
//      <!ATTLIST expanded-acronym xml:lang CDATA #IMPLIED>

//      <!ELEMENT icon EMPTY>
//      <!ATTLIST icon name CDATA #REQUIRED>

//      <!-- a generic icon name as per the Icon Naming Specification, only required if computing
//      it from the mime-type would not work, See "generic-icon" in the Shared Mime Specification -->
//      <!ELEMENT generic-icon EMPTY>
//      <!ATTLIST generic-icon name (application-x-executable|audio-x-generic|folder|font-x-generic|image-x-generic|package-x-generic|text-html|text-x-generic|text-x-generic-template|text-x-script|video-x-generic|x-office-address-book|x-office-calendar|x-office-document|x-office-presentation|x-office-spreadsheet) #REQUIRED>

//      <!ELEMENT glob EMPTY>
//      <!ATTLIST glob pattern CDATA #REQUIRED>
//      <!ATTLIST glob weight CDATA "50">
//      <!ATTLIST glob case-sensitive CDATA #IMPLIED>

//      <!ELEMENT magic (match)+>
//      <!ATTLIST magic priority CDATA "50">

//      <!ELEMENT match(match)*>
//      <!ATTLIST match offset CDATA #REQUIRED>
//      <!ATTLIST match type (string|big16|big32|little16|little32|host16|host32|byte) #REQUIRED>
//      <!ATTLIST match value CDATA #REQUIRED>
//      <!ATTLIST match mask CDATA #IMPLIED>

//      <!ELEMENT treemagic (treematch)+>
//      <!ATTLIST treemagic priority CDATA "50">

//      <!ELEMENT treematch(treematch)*>
//      <!ATTLIST treematch path CDATA #REQUIRED>
//      <!ATTLIST treematch type (file|directory|link) #IMPLIED>
//      <!ATTLIST treematch match-case (true|false) #IMPLIED>
//      <!ATTLIST treematch executable(true|false) #IMPLIED>
//      <!ATTLIST treematch non-empty(true|false) #IMPLIED>
//      <!ATTLIST treematch mimetype CDATA #IMPLIED>

//      <!ELEMENT root-XML EMPTY>
//      <!ATTLIST root-XML namespaceURI CDATA #REQUIRED>
//      <!ATTLIST root-XML localName CDATA #REQUIRED>

//      <!ELEMENT alias EMPTY>
//      <!ATTLIST alias type CDATA #REQUIRED>

//      <!ELEMENT sub-class-of EMPTY>
//      <!ATTLIST sub-class-of type CDATA #REQUIRED>
//  ]>

namespace akiss.GitHub.YetAnotherMimeMagic
{
    [XmlRoot(Namespace = "http://www.freedesktop.org/standards/shared-mime-info", ElementName = "mime-info")]
    public class MimeInfo
    {
        [XmlElement("mime-type")]
        public List<MimeType> MimeTypes;
    }

    [XmlRoot(ElementName = "mime-type")]
    public class MimeType
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type;

        [XmlElement(ElementName = "comment")]
        public string Comment;

        [XmlElement(ElementName = "acronym")]
        public string Acronym;

        [XmlElement(ElementName = "expanded-acronym")]
        public string ExpandedAcronym;

        // we don't need this
        // [XmlElement(ElementName = "generic-icon")]

        [XmlElement(ElementName = "glob")]
        public List<Glob> Globs;

        [XmlElement(ElementName = "magic")]
        public Magic Magic;

        // it is for detect folder content, we don't need this
        //[XmlElement(ElementName = "treemagic")]

        [XmlElement(ElementName = "sub-class-of")]
        public SubClassOf SubClassOf;
    }

    [XmlRoot(ElementName = "glob")]
    public class Glob
    {
        [XmlAttribute(AttributeName = "pattern")]
        public string Pattern;
    }

    [XmlRoot(ElementName = "magic")]
    public class Magic
    {
        [XmlElement(ElementName = "match")]
        public List<Match> Matches;
    }

    [XmlRoot(ElementName = "sub-class-of")]
    public class SubClassOf
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type;
    }

    [XmlRoot(ElementName = "match")]
    public class Match
    {
        [XmlIgnore]
        public string SearchPattern;

        [XmlAttribute(AttributeName = "value", DataType = "string")]
        public string Value;

        [XmlAttribute(AttributeName = "type")]
        public string Type;

        [XmlAttribute(AttributeName = "offset")]
        public string Offset;

        [XmlElement(ElementName = "match")]
        public List<Match> Matches;
    }
}
