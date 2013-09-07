namespace CouchDb.ViewServer.NLog
{
	using global::NLog;
	using global::NLog.Targets;

	[Target("CouchDb.ViewServer.Log")]
	public class LogTarget: TargetWithLayout
	{
		private readonly IViewServerProtocol viewServerProtocol;

		public LogTarget(IViewServerProtocol viewServerProtocol)
		{
			this.viewServerProtocol = viewServerProtocol;
		}

		protected override async void Write(LogEventInfo logEvent)
		{
			await this.viewServerProtocol.Log(logEvent.Message);
		}
	}
}