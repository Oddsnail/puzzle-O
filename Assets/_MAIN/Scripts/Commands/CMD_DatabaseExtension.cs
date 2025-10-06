using UnityEngine;

public abstract class CMD_DatabaseExtension {

    public static void Extend(CommandDatabase database) { }

    public static CommandParameters ConvertParameters(string[] data) => new(data);
}
