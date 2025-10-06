using System.Collections.Generic;

public class CommandParameters {
	
	private const char PARAMETER_ID = '-';
	private Dictionary<string, string> parameters = new();

	public CommandParameters(string[] rawParameters) {
		for (int i = 0; i < rawParameters.Length; i++) {

			if (rawParameters[i].StartsWith(PARAMETER_ID)) {
				string name = rawParameters[i];
				string value = "";

				if (i + 1 < rawParameters.Length && !rawParameters[i+1].StartsWith(PARAMETER_ID)) {
					value = rawParameters[i+1];
					i++;
				}

				parameters.Add(name, value);
			}
		}
	}

	public bool TryGetValue<T>(string paramName, out T value, T defaultValue = default(T)) {
		if (parameters.TryGetValue(paramName, out string paramValue)) {
			if (TryCastParam(paramValue, out value)) {
				return true;
			}
		}

		value = defaultValue;
		return false;
	}

	private bool TryCastParam<T>(string paramValue, out T value) {
		if (typeof(T) == typeof(bool)) {
			if (bool.TryParse(paramValue, out bool boolValue)) {
				value = (T)(object)boolValue;
				return true;
			}
		} else if (typeof(T) == typeof(int)) {
			if (int.TryParse(paramValue, out int intValue)) {
				value = (T)(object)intValue;
				return true;
			}
		} else if (typeof(T) == typeof(float)) {
			if (float.TryParse(paramValue, out float floatValue)) {
				value = (T)(object)floatValue;
				return true;
			}
		} else if (typeof(T) == typeof(string))  {
			value = (T)(object)paramValue;
			return true;
		}

		value = default(T);
		return false;
	}
}
