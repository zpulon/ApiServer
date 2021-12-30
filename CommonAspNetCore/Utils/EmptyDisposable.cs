using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonAspNetCore.Utils
{
	internal class EmptyDisposable : IDisposable
	{
		public void Dispose()
		{
		}
	}

}
