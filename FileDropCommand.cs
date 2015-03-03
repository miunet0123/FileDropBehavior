using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;


namespace miunet.Windows.FileDrop
{
	public class FileDropCommand : ICommand
	{
		#region ICommand Members

		public event EventHandler CanExecuteChanged;

		bool ICommand.CanExecute( object parameter )
		{
			if( !( parameter is DragEventArgs ) )
				return false;

			return this.CanExecute( (DragEventArgs)parameter );
		}


		void ICommand.Execute( object parameter )
		{
			var files = parameter as string[];
			if( files == null )
			{
				return;
			}

			this.Execute( files );
		}

		#endregion


		private readonly Predicate<DragEventArgs> _CanExecute;
		private readonly Action<string[]> _Execute;

		public DragDropEffects DragDropEffects { get; set; }


		public FileDropCommand( Action<string[]> execute, Predicate<DragEventArgs> canExecute = null )
		{
			this.DragDropEffects = DragDropEffects.Copy;

			if( execute == null ) throw new ArgumentNullException( "execute" );

			_Execute = execute;
			_CanExecute = canExecute;
		}


		public virtual bool CanExecute( DragEventArgs parameter )
		{
			return _CanExecute == null || _CanExecute( parameter );
		}


		public virtual void Execute( string[] parameter )
		{
			_Execute( parameter );
		}


		public void RaiseCanExecuteChanged()
		{
			var eventHandler = Interlocked.CompareExchange( ref this.CanExecuteChanged, null, null );
			if( eventHandler != null )
			{
				eventHandler( this, new EventArgs() );
			}
		}
	}
}
