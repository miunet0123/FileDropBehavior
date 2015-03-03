using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;


namespace miunet.Windows.FileDrop
{
	public class FileDropBehavior : Behavior<FrameworkElement>
	{
		public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
			"Command", typeof( ICommand ), typeof( FileDropBehavior ), new PropertyMetadata( default( ICommand ) ) );

		public ICommand Command
		{
			get { return (ICommand)GetValue( CommandProperty ); }
			set { SetValue( CommandProperty, value ); }
		}


		protected override void OnAttached()
		{
			base.AssociatedObject.AllowDrop = true;
			base.AssociatedObject.PreviewDragOver += this.AssociatedObject_PreviewDragOver;
			base.AssociatedObject.Drop += this.AssociatedObject_Drop;
			base.OnAttached();
		}


		protected override void OnDetaching()
		{
			base.AssociatedObject.PreviewDragOver -= this.AssociatedObject_PreviewDragOver;
			base.AssociatedObject.Drop -= this.AssociatedObject_Drop;
			base.OnDetaching();
		}


		private void AssociatedObject_PreviewDragOver( object sender, DragEventArgs e )
		{
			if( !e.Data.GetDataPresent( DataFormats.FileDrop, true ) )
			{
				e.Effects = DragDropEffects.None;
				e.Handled = true;
				return;
			}


			var command = this.Command;
			if( command == null || !command.CanExecute( e ) )
			{
				e.Effects = DragDropEffects.None;
				e.Handled = true;
				return;
			}

			e.Effects = DragDropEffects.Copy;
			e.Handled = true;
		}


		private void AssociatedObject_Drop( object sender, DragEventArgs e )
		{
			if( !e.Data.GetDataPresent( DataFormats.FileDrop, true ) )
			{
				return;
			}


			var command = this.Command;
			if( command == null || !command.CanExecute( e ) )
			{
				return;
			}

			var dropFiles = e.Data.GetData( DataFormats.FileDrop ) as string[];
			command.Execute( dropFiles );
			e.Handled = true;
		}
	}
}
