using System.Collections.Generic;
using System.Text;

public class IAPProductInfo
{
	public string productId;

	public string title;

	public string formattedPrice;

	public string unformattedPrice;

	public Dictionary<string, string> clientData;

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (clientData != null)
		{
			int num = 0;
			stringBuilder.Append("{ ");
			foreach (KeyValuePair<string, string> clientDatum in clientData)
			{
				if (num > 0)
				{
					stringBuilder.Append(", ");
				}
				num++;
				stringBuilder.Append($"{clientDatum.Key}:{clientDatum.Value}");
			}
			stringBuilder.Append(" }");
		}
		else
		{
			stringBuilder.Append("NULL");
		}
		return string.Format("[IAPProductInfo] {0}, {1}, {2}, {3}", new object[3]
		{
			productId,
			title,
			stringBuilder.ToString()
		});
	}
}
