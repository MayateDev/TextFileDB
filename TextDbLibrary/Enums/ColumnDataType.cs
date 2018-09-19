namespace TextDbLibrary.Enums
{
    // TODO - Kolla på om det vore bra att ha en primarykey enum
    public enum ColumnDataType
    {
        IntType = 0,
        DoubleType,
        DecimalType,
        StringType,
        SingleRelationship,
        MultipleRelationships
    }
}
