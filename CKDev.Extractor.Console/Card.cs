struct Card {
    public const string SetField = "set";
    public const string NumberField = "collector_number";
    public const string NameField = "name";
    public const string ManaCostField = "mana_cost";
    public const string ColorIdentityField = "color_identity";
    public const string CmcField = "cmc";
    public const string TypeField = "type_line";
    public const string RarityField = "rarity";
    public const string UsdCostField = "usd";
    public const string PricesField = "prices";

    public string Set;
    public string Number;
    public string Name;
    public string ManaCost;
    public string ColorIdentity;
    public string Cmc;
    public string Type;
    public string Rarity;
    public string UsdCost;

    public static readonly HashSet<string> Fields = new HashSet<string>(StringComparer.Ordinal) {
        SetField, NumberField, NameField, ManaCostField, ColorIdentityField, CmcField, TypeField, RarityField, PricesField
    };

    public override string ToString() =>
        $"{Set}|{Number}|{Name}|{ManaCost}|{ColorIdentity}|{Cmc}|{Type}|{Rarity}|{UsdCost}";

}
