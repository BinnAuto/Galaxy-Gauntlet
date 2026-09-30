namespace GalaxyGauntlet.scripts.SaveData
{
    public class SaveSlot(string slotName)
    {
        public string SlotName { get; set; } = slotName;

        public int CurrentLevelNumber { get; set; } = 1;

        public int LevelCount { get; set; } = 0;

        public string LevelHash { get; set; }
    }
}
