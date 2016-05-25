using System.ComponentModel;
using System.Runtime.CompilerServices;
using SmartReactives.PostSharp.NotifyPropertyChanged;

namespace SmartReactives.PostsharpExamples
{
	public class HasNotifyPropertyChanged : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		
		[RaisesNotifyPropertyChanged]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}