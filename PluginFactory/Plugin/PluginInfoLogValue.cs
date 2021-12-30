using PluginFactory.Basic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginFactory.Plugin
{
	internal class PluginInfoLogValue : IReadOnlyList<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable, IReadOnlyCollection<KeyValuePair<string, object>>
	{
		private PluginInfo _pluginInfo;

		private string _formatted;

		private List<KeyValuePair<string, object>> _values;

		private List<KeyValuePair<string, object>> Values
		{
			get
			{
				if (_values == null)
				{
					List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
					list.Add(new KeyValuePair<string, object>("PluginID", _pluginInfo.Id));
					list.Add(new KeyValuePair<string, object>("PluginName", _pluginInfo.Name));
					list.Add(new KeyValuePair<string, object>("PluginAlias", _pluginInfo.Alias ?? string.Empty));
					list.Add(new KeyValuePair<string, object>("PluginType", _pluginInfo.PluginType.FullName));
					_values = list;
				}
				return _values;
			}
		}

		public KeyValuePair<string, object> this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new IndexOutOfRangeException("index");
				}
				return Values[index];
			}
		}

		public int Count => Values.Count;

		public PluginInfoLogValue(PluginInfo pluginInfo)
		{
			if (pluginInfo == null)
			{
				throw new ArgumentNullException("pluginInfo");
			}
			_pluginInfo = pluginInfo;
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Values.GetEnumerator();
		}

		public override string ToString()
		{
			if (_formatted == null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(Resources.PluginInfo + ": ");
				for (int i = 0; i < Values.Count; i++)
				{
					KeyValuePair<string, object> keyValuePair = Values[i];
					stringBuilder.Append(keyValuePair.Key);
					stringBuilder.Append(": ");
					foreach (object item in (IEnumerable<object>)keyValuePair.Value)
					{
						stringBuilder.Append(item);
						stringBuilder.Append(", ");
					}
					stringBuilder.Remove(stringBuilder.Length - 2, 2);
					stringBuilder.AppendLine();
				}
				_formatted = stringBuilder.ToString();
			}
			return _formatted;
		}
	}
}
