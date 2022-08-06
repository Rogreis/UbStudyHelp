using UbStandardObjects.Objects;

namespace UbStandardObjects
{
	public delegate void ShowMessage(string message, bool isError = false, bool isFatal = false);

	public delegate void ShowStatusMessage(string message);

	public delegate void ShowPaperNumber(short paperNo);

	public static class StaticObjects
	{
		/// <summary>
		/// Control file name for different translations versions
		/// </summary>
		public const string ControlFileName = "UbControlFile.json";

		/// <summary>
		/// This is the object to store log
		/// </summary>
		public static Log Logger { get; set; }

		public static Parameters Parameters { get; set; }

		public static Book Book { get; set; } = null;



	}
}
