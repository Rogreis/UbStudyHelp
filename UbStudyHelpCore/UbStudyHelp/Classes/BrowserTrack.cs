using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UbStudyHelp;

namespace UbStudyHelp.Classes
{


    /// <summary>
    /// One step in the user track of used paragraphs
    /// </summary>
    public class BrowserPosition
    {
        public int leftTranslatioID = -1;
        public int rightTranslatioID = -1;
        TOC_Entry entry = null;


        public BrowserPosition(int leftTranslatioID, int rightTranslatioID, TOC_Entry entry)
        {
            this.leftTranslatioID = leftTranslatioID;
            this.rightTranslatioID = rightTranslatioID;
            this.entry = entry;
        }

        public TOC_Entry Entry
        {
            get
            {
                return entry;
            }
        }

        public override string ToString()
        {
            return entry.ToString();
        }

    }

    /// <summary>
    /// Store/Retrieve the track os paragraphs acesses by the user
    /// </summary>
    public static class BrowserTrack
    {
        private static TrackStack<BrowserPosition> browserHistory = new TrackStack<BrowserPosition>(App.ParametersData.MaxTrackItems);

        private static Stack<BrowserPosition> stack = new Stack<BrowserPosition>();

        private static int currentBrowserHistoy = -1;

        private static TOC_Entry lastTrackEntry = null;

        //public static BrowserPosition CurrentPosition
        //{
        //    get
        //    {
        //        if (browserHistory.Count > 0 && currentBrowserHistoy >= 0)
        //            return browserHistory.Peek();
        //        else
        //            return null;
        //    }
        //}


        public static void Add(int leftTranslatioID, int rightTranslatioID, TOC_Entry entry)
        {
            if (entry == lastTrackEntry)
                return;
            BrowserPosition position = new BrowserPosition(leftTranslatioID, rightTranslatioID, entry);
            browserHistory.Push(position);
            if (currentBrowserHistoy < 0)
                currentBrowserHistoy = 0;
            EventsControl.FireBrowserTrackAdded(browserHistory.Peek());
            lastTrackEntry = entry;
        }


        public static int Count
        {
            get
            {
                return browserHistory.Count;
            }
        }


    }


}
