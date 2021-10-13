using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UrantiaBook.Classes
{
    /// <summary>
    /// Used to fire a click on a new Table Of Contents item
    /// </summary>
    /// <param name="oTOC_Entry"></param>
    public delegate void dlTOCClicked(TOC_Entry oTOC_Entry);

    /// <summary>
    /// Used to fire a click on some seach result entry
    /// </summary>
    /// <param name="loc"></param>
    public delegate void dlSeachClicked(Location loc);
    
    /// <summary>
    /// Used to fire a click on index
    /// </summary>
    /// <param name="loc"></param>
    public delegate void dlIndexClicked(Location loc);

    /// <summary>
    /// Used to send a message to be shown in the main form
    /// </summary>
    /// <param name="Message"></param>
    public delegate void dlSendMessage(string Message);

    public delegate void dlCurrentBrowserTrack(BrowserPosition track);

    public delegate void dlBrowserTrackAdded(BrowserPosition track);

    public delegate void dlLeftTranslationChanged(Translation oLeftTranslation);

    public delegate void dlRightTranslationChanged(Translation oRightTranslation);

    public delegate void dlBilingualChanged(bool ShowBilingual);



    public static class EventsControl
    {
        private static bool AvoidNewLocationEvents = false;

        public static event dlSeachClicked SeachClicked = null;

        public static event dlIndexClicked IndexClicked = null;

        public static event dlTOCClicked TOCClicked = null;

        public static event dlSendMessage SendMessage = null;

        public static event dlCurrentBrowserTrack CurrentBrowserTrack = null;

        public static event dlBrowserTrackAdded BrowserTrackAdded = null;

        public static event dlLeftTranslationChanged LeftTranslationChanged = null;

        public static event dlRightTranslationChanged RightTranslationChanged = null;

        public static event dlBilingualChanged BilingualChanged = null;


        public static void FireSeachClicked(Location loc)
        {
            if (AvoidNewLocationEvents)
                return;
            //AvoidNewLocationEvents = true;
            SeachClicked?.Invoke(loc);
            AvoidNewLocationEvents = false;
        }

        public static void FireIndexClicked(Location loc)
        {
            if (AvoidNewLocationEvents)
                return;
            //AvoidNewLocationEvents = true;
            IndexClicked?.Invoke(loc);
            AvoidNewLocationEvents = false;
        }

        public static void FireTOCClicked(TOC_Entry entry)
        {
            if (AvoidNewLocationEvents)
                return;
            //AvoidNewLocationEvents = true;
            TOCClicked?.Invoke(entry);
            AvoidNewLocationEvents = false;
        }

        public static void FireCurrentBrowserTrack(BrowserPosition track)
        {
            if (AvoidNewLocationEvents)
                return;
            //AvoidNewLocationEvents = true;
            CurrentBrowserTrack?.Invoke(track);
            AvoidNewLocationEvents = false;
        }

        public static void FireBrowserTrackAdded(BrowserPosition track)
        {
            if (AvoidNewLocationEvents)
                return;
            //AvoidNewLocationEvents = true;
            BrowserTrackAdded?.Invoke(track);
            AvoidNewLocationEvents = false;
        }



        public static void FireSendMessage(string location, Exception ex)
        {
            string message = location + ": ";
            Exception ex2 = ex;
            while (ex2 != null)
            {
                message += ex2.Message;
                ex2 = ex2.InnerException;
            }
            SendMessage?.Invoke(message);
        }


        public static void FireSendMessage(string Message)
        {
            SendMessage?.Invoke(Message);
        }

        public static void FireLeftTranslationChanged(Translation oLeftTranslation)
        {
            LeftTranslationChanged?.Invoke(oLeftTranslation);
        }
        public static void FireRightTranslationChanged(Translation oRightTranslation)
        {
            RightTranslationChanged?.Invoke(oRightTranslation);
        }


        public static void FireBilingualChanged(bool ShowBilingual)
        {
            BilingualChanged?.Invoke(ShowBilingual);
        }



    }
}
