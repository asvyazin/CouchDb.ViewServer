namespace CouchDb.ViewServer.NLog
{
	using global::NLog;
	using global::NLog.Targets;

	[Target("CouchDb.ViewServer.Log")]
	public class LogTarget: TargetWithLayout
	{
		private readonly IViewServer viewServer;

		public LogTarget(IViewServer viewServer)
		{
			this.viewServer = viewServer;
		}

		protected override async void Write(LogEventInfo logEvent)
		{
			await this.viewServer.Log(logEvent.Message);
		}
	}
}