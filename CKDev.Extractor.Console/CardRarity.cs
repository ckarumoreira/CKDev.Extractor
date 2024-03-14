public readonly struct CardRarity {
    private CardRarity(string name, string key) {
        Name = name;
        Key = key;
    }

    public readonly string Name;
    public readonly string Key;

    public static readonly CardRarity Common = new CardRarity("common", "C");
    public static readonly CardRarity Uncommon = new CardRarity("uncommon", "U");
    public static readonly CardRarity Rare = new CardRarity("rare", "R");
    public static readonly CardRarity Mythic = new CardRarity("mythic", "M");
    public static readonly CardRarity Special = new CardRarity("special", "S");

    public static string GetKey(string? name) {
        if (name is not null) {
            if (StringComparer.OrdinalIgnoreCase.Compare(Common.Name, name) is 0) {
                return Common.Key;
            }
            if (StringComparer.OrdinalIgnoreCase.Compare(Uncommon.Name, name) is 0) {
                return Uncommon.Key;
            }
            if (StringComparer.OrdinalIgnoreCase.Compare(Rare.Name, name) is 0) {
                return Rare.Key;
            }
            if (StringComparer.OrdinalIgnoreCase.Compare(Mythic.Name, name) is 0) {
                return Mythic.Key;
            }
            if (StringComparer.OrdinalIgnoreCase.Compare(Special.Name, name) is 0) {
                return Special.Key;
            }
        }

        return "Invalid Input";
    }
}
