using SmartReactives.Core;

namespace SmartReactives.Test
{
    public class Dependent : IListener
    {
		public string Name { get; }

	    public Dependent(string name = "")
	    {
		    Name = name;
	    }

	    public void Notify()
        {
        }

		public bool StrongReference => false;
	}
}