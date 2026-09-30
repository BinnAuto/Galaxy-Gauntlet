using System.Linq;
using System.Text.Json;

namespace GalaxyGauntlet.scripts.SaveData
{
    public static class SaveData
    {
        public static string AutosaveSlotName = "autosave";

        public static SaveDataInstance Instance;

        private const string SaveDataPath = "user://savedata.gg";

        public static void Load()
        {
            Instance = new();
            if(false == Godot.FileAccess.FileExists(SaveDataPath))
            {
                Save();
                return;
            }

            using var file = Godot.FileAccess.Open(SaveDataPath, Godot.FileAccess.ModeFlags.Read);
            string saveContent = file.GetAsText();
            Instance = JsonSerializer.Deserialize<SaveDataInstance>(saveContent);
        }


        public static bool SaveSlotExists(string slotName)
        {
            return FindSlotIndex(slotName) != -1;
        }


        public static int FindSlotIndex(string slotName)
        {
            return Instance.FindSlotIndex(slotName);
        }


        public static SaveSlot GetSaveSlot(string slotName)
        {
            int index = FindSlotIndex(slotName);
            if(index == -1)
            {
                return null;
            }

            return Instance.SaveSlots[index];
        }


        public static void WriteSaveSlot(SaveSlot saveSlot)
        {
            int index = FindSlotIndex(saveSlot.SlotName);
            if (index == -1)
            {
                Instance.SaveSlots = Instance.SaveSlots.Append(saveSlot).ToArray();
            }
            else
            {
                Instance.SaveSlots[index] = saveSlot;
            }
            Save();
        }


        public static void DeleteSaveSlot(string slotName)
        {
            Instance.SaveSlots = Instance.SaveSlots.Where(e => e.SlotName != slotName).ToArray();
            Save();
        }


        public static void Save()
        {
            string saveData = SCGlobal.ToJson(Instance);
            using var file = Godot.FileAccess.Open(SaveDataPath, Godot.FileAccess.ModeFlags.Write);
            file.StoreString(saveData);
        }
    }
}
