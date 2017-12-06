using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace exactmobile.ussdcommon
{
    public class ContentItem
    {
        public int ItemNumber { get; set; }
        public int WapextContentItemID{get;set;}
        public string SingleCode {get;set;}
        public string Title{get;set;}
        public string Artist{get;set;}
        public decimal Price{get;set;}
        public int SingleCodeContentTypeIDRef{get;set;}
        public int ContentCatalogIDRef { get; set; }
    }
}
