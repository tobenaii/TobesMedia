using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TobesMediaCore.Data.Media;

namespace TobesMediaCommon.Data.Media
{
    public class MediaPage
    {
        public int Pages { get; private set; }
        public int Page { get; private set; }
        public List<MediaBase> Media { get; private set; }

        public MediaPage(int pages, int page, List<MediaBase> media)
        {
            Pages = pages;
            Page = page;
            Media = media;
        }
    }
}
