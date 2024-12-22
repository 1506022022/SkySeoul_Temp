using System.Collections;
using System.Collections.Generic;

public class PlayerData
{
    private static PlayerData instance;

    public static PlayerData Instance()
    {
        if (instance == null)
        {
            instance = new PlayerData();
        }
        return instance;
    }

    public CharacterType characterType = CharacterType.Han;
}
