namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public interface IMissionSelect
    {
        void AddPlayer(Player NewPlayer);
        void UpdateCharacter(Player PlayerToUpdate);
        void UpdateSelectedMap(string CurrentDifficulty, string SelectedMapPath);
        void UpdateRoomSubtype(string RoomSubtype);
        void UpdateReadyOrHost();
    }
}