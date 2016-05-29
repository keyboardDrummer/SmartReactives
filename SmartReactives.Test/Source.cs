
using SmartReactives.Extensions;

namespace SmartReactives.Postsharp.Test
{
	public class Source //: HasNotifyPropertyChanged
	{
	    ReactiveVariable<bool> woop = new ReactiveVariable<bool>();

	    public bool Woop
	    {
	        get { return woop.Value; }
	        set { woop.SetValueIfChanged(value); } //TODO sure about this?
	    }

	    public void FlipWoop()
		{
			Woop = !Woop;
		}
	}
}
