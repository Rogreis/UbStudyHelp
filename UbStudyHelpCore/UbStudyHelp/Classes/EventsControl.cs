using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UbStudyHelp.Classes;

namespace UrantiaBook.Classes
{
    /// <summary>
    /// Used to fire a click on a new Table Of Contents item
    /// </summary>
    /// <param name="oTOC_Entry"></param>
    public delegate void dlTOCClicked(TOC_Entry entry);

    /// <summary>
    /// Used to fire a click on some seach result entry
    /// </summary>
    /// <param name="loc"></param>
    public delegate void dlSeachClicked(TOC_Entry entry);

    /// <summary>
    /// Used to fire a click on index
    /// </summary>
    /// <param name="loc"></param>
    public delegate void dlIndexClicked(TOC_Entry entry);

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

    public delegate void dlFontChanged(ControlsAppearance appearance);


    public static class EventsControl
    {
        public static event dlSeachClicked SeachClicked = null;

        public static event dlIndexClicked IndexClicked = null;

        public static event dlTOCClicked TOCClicked = null;

        public static event dlSendMessage SendMessage = null;

        public static event dlCurrentBrowserTrack CurrentBrowserTrack = null;

        public static event dlBrowserTrackAdded BrowserTrackAdded = null;

        public static event dlLeftTranslationChanged LeftTranslationChanged = null;

        public static event dlRightTranslationChanged RightTranslationChanged = null;

        public static event dlBilingualChanged BilingualChanged = null;

        public static event dlFontChanged FontChanged = null;


        public static void FireSeachClicked(TOC_Entry entry)
        {
            SeachClicked?.Invoke(entry);
        }

        public static void FireIndexClicked(TOC_Entry entry)
        {
            IndexClicked?.Invoke(entry);
        }

        public static void FireTOCClicked(TOC_Entry entry)
        {
            TOCClicked?.Invoke(entry);
        }

        public static void FireCurrentBrowserTrack(BrowserPosition track)
        {
            CurrentBrowserTrack?.Invoke(track);
        }

        public static void FireBrowserTrackAdded(BrowserPosition track)
        {
            BrowserTrackAdded?.Invoke(track);
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
        
        public static void FireFontChanged(ControlsAppearance appearance)
        {
            FontChanged?.Invoke(appearance);
        }



    }
}
