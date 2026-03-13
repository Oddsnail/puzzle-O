namespace origin.character {
    public interface ICharacterService {
        bool HasCharacter(string ID);
        CharacterConfigData GetInfo(string ID);
        CHARACTER GetCharacter(string ID);
        CHARACTER AddCharacter(string ID);
        CHARACTER AddClient(string ID);
        void RemoveCharacter(string ID);
    }
}
