using UnityEngine;

public abstract class LogicElementBase : MonoBehaviour
{
    public abstract void Execute();
}

public static class LogicOperations
{
    public static bool Pass(bool arg1) => arg1;
    public static bool Not(bool arg1) => !arg1;
    public static bool And(bool arg1, bool arg2) => arg1 && arg2;
    public static bool Or(bool arg1, bool arg2) => arg1 || arg2;
    public static bool Nand(bool arg1, bool arg2) => !(arg1 && arg2);
    public static bool Nor(bool arg1, bool arg2) => !(arg1 || arg2);
    public static bool Xor(bool arg1, bool arg2) => (arg1 || arg2) && !(arg1 && arg2);
    public static bool Xnor(bool arg1, bool arg2) => arg1 == arg2;
}

public static class LogicColors
{
    public static Color zero = Color.gray;
    public static Color one = new(0f, 0.58f, 0.78f);
}
