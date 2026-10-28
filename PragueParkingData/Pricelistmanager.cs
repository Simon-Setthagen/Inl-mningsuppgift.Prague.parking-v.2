namespace Data;
public static class PriceListManager
{
    public static PriceList LoadPrices()
    {
        PriceList prices = new();

        if (!File.Exists("PriceList.txt"))
            return prices;

        foreach (string line in File.ReadAllLines("PriceList.txt"))
        {
            string[] parts = line.Split('=');

            if (parts.Length != 2)
                continue;

            if (parts[0] == "CAR")
                prices.CarPrice = decimal.Parse(parts[1]);

            if (parts[0] == "MC")
                prices.McPrice = decimal.Parse(parts[1]);
        }

        return prices;
    }
}
